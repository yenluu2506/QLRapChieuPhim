namespace CinemeBooking.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Ghe")]
    public partial class Ghe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ghe()
        {
            Ves = new HashSet<Ve>();
        }

        public int id { get; set; }

        [StringLength(50)]
        public string MaGhe { get; set; }

        [Column(TypeName = "money")]
        public decimal? GiaGhe { get; set; }

        public int TrangThai { get; set; }

        //public int idPhongChieu { get; set; }

        public bool? isCouple { get; set; }

        public virtual PhongChieu PhongChieu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ve> Ves { get; set; }
    }
}
