namespace PrimeEwsi.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("packs")]
    public class HistoryPackModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("userId")]
        public int UserId { get; set; }

        [Column("files")]
        public string Files { get; set; }

        [Column("environment")]
        public string Environment { get; set; }

        [Column("projects")]
        public string Projects { get; set; }

        [Column("component")]
        public string Component { get; set; }

        [Column("teets")]
        public string Teets { get; set; }

    }
}