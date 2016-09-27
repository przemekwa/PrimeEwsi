using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Models
{
    [Table("config")]
    public class ConfigModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        public string Component { get; set; }
        public int Version { get; set; }
    }
}