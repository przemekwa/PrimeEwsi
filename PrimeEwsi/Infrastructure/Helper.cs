using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PrimeEwsi.Models;

namespace PrimeEwsi.Infrastructure
{
    public static class Helper
    {
        public static UserModel GetUserModel()
        {
            var userSkp = HttpContext.Current.User.Identity.Name;
            
            var primeEwsiContext = new PrimeEwsiContext();

            return primeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == userSkp);
        }
    }
}