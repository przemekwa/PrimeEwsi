using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeEwsi.Models
{
    [Table("users_profile")]
    public class UserModel
    {
        [Key]
        [Column("id")]
        public int UserId { get; set; }

        [Column("name")]
        public string UserName { get; set; }

        [Column("skp")]
        public string UserSkp { get; set; }

        [Column("svn_user")]
        public string SvnUser { get; set; }

        [Column("svn_password")]
        [DataType(DataType.Password)]
        public string SvnPassword { get; set; }

        [Column("apikey")]
        public string UserApiKey { get; set; }

        [Column("jiracookie")]
        public string UserJiraCookie { get; set; }

        public void SetUser(UserModel userModel)
        {
            this.UserId = userModel.UserId;
            this.UserName = userModel.UserName;
            this.UserSkp = userModel.UserSkp;
            this.SvnPassword = userModel.SvnPassword;
            this.SvnUser = userModel.SvnUser;
            this.UserApiKey = userModel.UserApiKey;
            this.UserJiraCookie = userModel.UserJiraCookie;
        }
    }
}