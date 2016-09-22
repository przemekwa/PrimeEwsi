using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Models;

namespace PrimeEwsi.Controllers
{
    public class PackController : Controller
    {
        // GET: Create
        public ActionResult Create()
        {
            var userSkp = this.HttpContext.User.Identity.Name;

            return View(new PackModel
            {
                Id = userSkp
            });
        }
    }
}