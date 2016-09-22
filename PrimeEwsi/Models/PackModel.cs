using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PrimeEwsi.Models
{
    [NotMapped]
    public class PackModel2 : UserModel
    {
        public PackModel2()
        {
            
        }

        public PackModel2(UserModel userModel)
        {
            this.Id = userModel.Id;
            this.Name = userModel.Name;
            this.Skp = userModel.Skp;
            this.SvnPassword = userModel.SvnPassword;
            this.SvnUrl = userModel.SvnUrl;
            this.SvnUser = userModel.SvnUser;
        }
    }
}