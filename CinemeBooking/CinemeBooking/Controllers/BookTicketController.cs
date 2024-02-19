using CinemeBooking.Models;
using CinemeBooking.Models.Payment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace CinemeBooking.Controllers
{
    public class BookTicketController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BookTicket
        public ActionResult Index(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Error");
            }

            List<Ghe> GheList = db.Ghes.ToList();

            LichChieu lc = db.LichChieux.Where(l => l.id == id).First();
            ViewBag.LichChieu = lc;
            int phongId = db.LichChieux.Select(p => p.PhongChieu.id).FirstOrDefault();
            ViewBag.RapChieu = db.RapPhims.Where(r => r.id == phongId).FirstOrDefault();

            return View(GheList);
        }

        public ActionResult CheckOut()
        {

            var totalPrice = Request.QueryString["totalPrice"];
            var lcId = Request.QueryString["lcId"];
            var gheIds = Request.QueryString.GetValues("gheIds[]"); // Lấy giá trị từ query string

            // Kiểm tra xem gheIds có giá trị không null và có ít nhất một phần tử
            if (gheIds != null && gheIds.Length > 0)
            {
                // Khởi tạo mảng chuỗi để lưu trữ giá trị từ mảng gheIds
                string[] chuoiGheIds = new string[gheIds.Length];

                // Gán giá trị từ mảng gheIds vào mảng chuỗi chuoiGheIds
                for (int i = 0; i < gheIds.Length; i++)
                {
                    chuoiGheIds[i] = gheIds[i];
                }
                ViewBag.ChuoiGheIds = chuoiGheIds;
            }
            string tkId = Request.QueryString["tkId"];
         
            //Session["Ve"] = order;
            Session["TongTien"] = totalPrice;
            Session["LCId"] = lcId;
            Session["GheIds"] = gheIds;
            Session["TKId"] = tkId;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(int TypePaymentVN)
        {
            var code = new { Success = false, Code = -1, Url = "" };

            if (Session["GheIds"] != null && Session["GheIds"] is string[] gheIds && gheIds.Length > 0)
            {
                var lcId = Convert.ToInt32(Session["LCId"]);
                var lc = db.LichChieux.FirstOrDefault(x => x.id == lcId);

                if (lc != null)
                {
                    var orderList = new List<Ve>();

                    foreach (var gheId in gheIds)
                    {
                        var ve = new Ve
                        {
                            NgayDat = DateTime.Now,
                            idTaiKhoan = (string)Session["TKId"],
                            TienBanVe = Convert.ToDecimal(Session["TongTien"]),
                            LichChieu = lc
                        };

                        if (int.TryParse(gheId, out int seatId))
                        {
                            var ghe = db.Ghes.FirstOrDefault(x => x.id == seatId);
                            if (ghe != null)
                            {
                                ve.Ghe = ghe;
                                orderList.Add(ve);
                            }
                        }
                    }

                    if (orderList.Any())
                    {
                        db.Ves.AddRange(orderList);
                        db.SaveChanges();

                        code = new { Success = true, Code = TypePaymentVN, Url = "" };

                        if (TypePaymentVN != -1)
                        {
                            var url = UrlPayment(TypePaymentVN, orderList.First().id.ToString());
                            code = new { Success = true, Code = TypePaymentVN, Url = url };
                            return Redirect(url);
                        }
                    }
                }
            }

            return Json(code);
        }



        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        int orderID = Convert.ToInt32(orderCode);
                        var itemOrder = db.Ves.FirstOrDefault(x => x.id == orderID );
                        if (itemOrder != null)
                        {
                            //itemOrder. = 2;//đã thanh toán
                            itemOrder.TrangThai = 1;
                            db.Ves.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }

        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            int orderID = Convert.ToInt32(orderCode);
            var order = db.Ves.FirstOrDefault(x => x.id ==orderID);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TienBanVe * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.NgayDat.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.id);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", Convert.ToString(order.id)); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}