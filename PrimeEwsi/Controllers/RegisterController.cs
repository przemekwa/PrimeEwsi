using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

        public ActionResult Edit()
        {
            return View("Update", GetUserModel());
        }

        public ActionResult Update(UserModel userModel)
        {
            var userDb = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == this.HttpContext.User.Identity.Name);

            userDb.Name = userModel.Name;
            userDb.SvnUser = userModel.SvnUser;
            userDb.SvnPassword = userModel.SvnPassword;
            userDb.ApiKey = userModel.ApiKey;

            this.PrimeEwsiContext.SaveChanges();

            return RedirectToAction("Create", "Pack");
        }

        public ActionResult Add(UserModel userModel)
        {
            userModel.Skp = this.HttpContext.User.Identity.Name;

            this.PrimeEwsiContext.UsersModel.Add(userModel);

            this.PrimeEwsiContext.SaveChanges();
            return RedirectToAction("Create", "Pack");
        }

        private UserModel GetUserModel()
        {
            var userSkp = this.HttpContext.User.Identity.Name;

            var userModel = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == userSkp);
            return userModel;
        }
    }
}