using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class ErrorModel : UserModel
    {
        public string Description { get; set; }
        public string StacTrace { get; set; }

        public ErrorModel(string description, string stacTrace)
        {
            Description = description;
            StacTrace = stacTrace;
        }
    }
}