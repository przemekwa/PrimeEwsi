using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeEwsi.Models
{
    [Table("config")]
    public class ConfigModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        public string Component { get; set; }

        public string Version { get; set; }
   }
}