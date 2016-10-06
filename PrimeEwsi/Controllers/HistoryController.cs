using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Infrastructure;
using PrimeEwsi.Models;

namespace PrimeEwsi.Controllers
{
    public class HistoryController : Controller
    {
        public PrimeEwsiContext PrimeEwsiContext { get; set; }

        public HistoryController(/*PrimeEwsiContext primeEwsiContext*/)
        {
            PrimeEwsiContext = new PrimeEwsiContext();
        }

        public ActionResult Show()
        {
            var model = new HistoryModel();

            model.SetUser(Helper.GetUserModel());

            model.PackCollection = this.PrimeEwsiContext.PackCollection.OrderByDescending(p=>p.Id).ToList();

            return View(model);
        }
    }
}