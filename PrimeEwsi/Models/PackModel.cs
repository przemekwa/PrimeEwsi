using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class PackModel : UserModel
    {
        public IEnumerable<HistoryPackModel> HistoryPackCollection { get; set; }

        public IEnumerable<string> JiraTeets { get; set; }

        public string Teets { get; set; }
      
        public string ProjectId { get; set; }
      
        public List<string> Files { get; set; } = new List<string>();

        public string Component { get; set; }

        public string TestEnvironment { get; set; }

        public SendModel SendModel { get; set; }
    }
}