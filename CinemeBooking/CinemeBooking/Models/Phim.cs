namespace CinemeBooking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Phim")]
    public partial class Phim
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Phim()
        {
            LichChieux = new HashSet<LichChieu>();
            TheLoais = new HashSet<TheLoai>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string TenPhim { get; set; }

        [StringLength(1000)]
        public string MoTa { get; set; }

        public int ThoiLuong { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayKhoiChieu { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        [StringLength(50)]
        public string QuocGia { get; set; }

        [Required]
        [StringLength(100)]
        public string DaoDien { get; set; }

        public string DienVien { get; set; }

        public int NamSX { get; set; }

        public string ApPhich { get; set; }

        public bool TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichChieu> LichChieux { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TheLoai> TheLoais { get; set; }
    }
}
