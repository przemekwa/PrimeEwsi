using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class ErrorModel : UserModel
    {
        public string Description { get; set; }

        public ErrorModel(string description)
        {
            Description = description;
        }
    }
}