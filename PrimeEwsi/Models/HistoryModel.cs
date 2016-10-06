using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class HistoryModel : UserModel
    {
        public List<Pack> PackCollection { get; set; }
    }
}