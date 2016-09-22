using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Models;

namespace PrimeEwsi.Controllers
{
    public class PackController : Controller
    {

        public PrimeEwsiContext PrimeEwsiContext { get; set; } = new PrimeEwsiContext();

   // GET: Create
        public ActionResult Create()
        {
            var userSkp = this.HttpContext.User.Identity.Name;

            var userModel = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == userSkp);

            if (userModel == null)
            {
                return RedirectToAction("New", "Register");
            }

            return View(new PackModel(userModel)
            {
                Teets = new List<string> { "Teest1"}
            });
        }


        public ActionResult Add(PackModel packModel)
        {
            ;
            return RedirectToAction("Create");
        }
    }
}