using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class HistoryModel : UserModel
    {
        public List<HistoryPackModel> HistoryPackModelCollection { get; set; }
    }
}