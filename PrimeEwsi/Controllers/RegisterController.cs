using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Models;

namespace PrimeEwsi.Controllers
{
    public class RegisterController : Controller
    {
        public PrimeEwsiContext PrimeEwsiContext { get; set; } = new PrimeEwsiContext();

        // GET: Register
        public ActionResult New()
        {
            return View(new UserModel
            {
                Skp = this.HttpContext.User.Identity.Name
            });
        }


        public ActionResult Add(UserModel userModel)
        {
            userModel.Skp = this.HttpContext.User.Identity.Name;

            this.PrimeEwsiContext.UsersModel.Add(userModel);

            this.PrimeEwsiContext.SaveChanges();
            return RedirectToAction("Create", "Pack");
        }
    }
}