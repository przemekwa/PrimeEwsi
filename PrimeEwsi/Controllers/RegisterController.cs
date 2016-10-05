using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Infrastructure;
using PrimeEwsi.Models;

namespace PrimeEwsi.Controllers
{
    [HandleErrorException]
    public class RegisterController : Controller
    {
        public PrimeEwsiContext PrimeEwsiContext { get; set; } 

        public RegisterController()
        {
            this.PrimeEwsiContext = new PrimeEwsiContext();
        }

        public ActionResult New()
        {
            return View(new UserModel
            {
                Skp = this.HttpContext.User.Identity.Name
            });
        }

        public ActionResult Edit()
        {
            return View("Update", Helper.GetUserModel());
        }

        public ActionResult Update(UserModel userModel)
        {
            if (Validate(userModel))
            {
                return View("Update", userModel);
            }

            var userDb = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == this.HttpContext.User.Identity.Name);

            if (userDb == null)
            {
                throw new Exception($"Brak użytkownika {this.HttpContext.User.Identity.Name} w bazie");
            }

            userDb.Name = userModel.Name;
            userDb.SvnUser = userModel.SvnUser;
            userDb.SvnPassword = userModel.SvnPassword;
            userDb.ApiKey = userModel.ApiKey;

            this.PrimeEwsiContext.SaveChanges();

            return RedirectToAction("Create", "Pack");
        }

        public ActionResult Add(UserModel userModel)
        {
            if (Validate(userModel))
            {
                return View("New", userModel);
            }

            userModel.Skp = this.HttpContext.User.Identity.Name;

            this.PrimeEwsiContext.UsersModel.Add(userModel);

            this.PrimeEwsiContext.SaveChanges();

            return RedirectToAction("Create", "Pack");
        }

        private bool Validate(UserModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.Name))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Name] - uzupełnij identyfikator");
            }

            if (string.IsNullOrEmpty(userModel.ApiKey))
            {
                this.ModelState.AddModelError("Błąd", "Pole [ApiKey] - uzupełnij klucz z artifactory");
            }

            if (string.IsNullOrEmpty(userModel.SvnPassword))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Svn Password] - uzupełnij hasło do SVN-a");
            }

            if (string.IsNullOrEmpty(userModel.SvnUser))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Svn User] - uzupełnij użytkownika SVN-a");
            }

            return !this.ModelState.IsValid;
        }
    }
}