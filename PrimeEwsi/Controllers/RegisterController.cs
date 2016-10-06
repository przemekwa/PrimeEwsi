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
        public IPrimeEwsiDbApi PrimeEwsiDbApi { get; set; }

        public RegisterController(IPrimeEwsiDbApi primeEwsiDbApi)
        {
            PrimeEwsiDbApi = primeEwsiDbApi;
        }

        public ActionResult New()
        {
            return View(new UserModel
            {
                UserSkp = this.HttpContext.User.Identity.Name
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

            var user = this.PrimeEwsiDbApi.GetUser(this.HttpContext.User.Identity.Name);

            userModel.UserId = user.UserId;
            userModel.UserSkp = user.UserSkp;

            this.PrimeEwsiDbApi.UpdateUser(userModel);

            return RedirectToAction("Create", "Pack");
        }

        public ActionResult Add(UserModel userModel)
        {
            if (Validate(userModel))
            {
                return View("New", userModel);
            }

            userModel.UserSkp = this.HttpContext.User.Identity.Name;

            this.PrimeEwsiDbApi.AddUser(userModel);

            return RedirectToAction("Create", "Pack");
        }

        private bool Validate(UserModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.UserName))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Name] - uzupełnij identyfikator");
            }

            if (string.IsNullOrEmpty(userModel.UserApiKey))
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