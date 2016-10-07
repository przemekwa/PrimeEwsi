using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using PrimeEwsi.Infrastructure;
using PrimeEwsi.Models;
using RestSharp;

namespace PrimeEwsi.Controllers
{
    [HandleErrorException]
    public class JiraController : Controller
    {
        // GET: Jira
        public ActionResult Authentication()
        {
            var model = new JiraModel();

            model.SetUser(Helper.GetUserModel());

            return View(model);
        }

        [HttpPost]
        public ActionResult SetCookie(string user, string password)
        {
            var model = new JiraModel();

            var restClient = new RestClient("https://godzilla.centrala.bzwbk:9999");

            var request = new RestRequest("/rest/auth/1/session", Method.POST);

            request.AddJsonBody(new { username = user, password = password });

            var response = restClient.Execute(request).Content;

            var jResponse = JObject.Parse(response);

            model.Cookie = jResponse["session"]["value"].ToString();

            model.SetUser(Helper.GetUserModel());

            return View("Authentication", model);
        }
    }
}