namespace CinemeBooking.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;

    [Table("Ve")]
    public partial class Ve
    {
        public int id { get; set; }

        public int LoaiVe { get; set; }

        [StringLength(128)]
        public string idTaiKhoan { get; set; }

        public DateTime NgayDat { get; set; }

        [Column(TypeName = "money")]
        public decimal TienBanVe { get; set; }

        public int TrangThai { get; set; }

        public virtual Ghe Ghe { get; set; }

        public virtual LichChieu LichChieu { get; set; }
    }
}
