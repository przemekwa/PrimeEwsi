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
        public IPrimeEwsiDbApi PrimeEwsiDbApi { get; set; }

        public HistoryController(/*PrimeEwsiContext primeEwsiContext*/)
        {
            this.PrimeEwsiDbApi = new PrimeEwsiDbApi(new PrimeEwsiContext());
        }

        public ActionResult Show()
        {
            var model = new HistoryModel();

            model.SetUser(Helper.GetUserModel());

            model.HistoryPackModelCollection =
                this.PrimeEwsiDbApi.GetHistoryPacksByUserId(model.UserId).OrderByDescending(p => p.Id).ToList();

            return View(model);
        }
    }
}