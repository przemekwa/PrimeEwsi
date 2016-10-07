
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace PrimeEwsi.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Models;

    using UrbanCodeMetaFileCreator;
    using Infrastructure;

    [HandleErrorException]
    public class PackController : Controller
    {
        public IPackApi PackApi { get; set; }

        public IPrimeEwsiDbApi PrimeEwsiDbApi { get; set; }

#if DEBUG
        private const string SERVERURL = "https://wro2096v.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/";
#else
        private const string SERVERURL = "https://ewsi.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/";
#endif

        public PackController(IPackApi packApi, IPrimeEwsiDbApi primeEwsiDbApi)
        {
            PackApi = packApi;
            PrimeEwsiDbApi = primeEwsiDbApi;
        }

        public ActionResult Create()
        {
            var userModel = Infrastructure.Helper.GetUserModel();

            if (userModel == null)
            {
                return RedirectToAction("New", "Register");
            }

            var model = new PackModel()
            {
               HistoryPackCollection = this.PrimeEwsiDbApi.GetHistoryPacksByUserId(userModel.UserId),
               JiraTeets = this.GetJiraTets(userModel.UserJiraCookie)
            };

            model.SetUser(userModel);

            return View(model);
        }

        public IEnumerable<JiraTeet> GetJiraTets(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return new List<JiraTeet>();
            }

            var restClient = new RestClient("https://godzilla.centrala.bzwbk:9999");

            var request = new RestRequest($"/rest/api/2/search?jql=assignee={Infrastructure.Helper.GetUserModel().UserSkp.Substring(9)}&fields=id,key,summary", Method.GET);

            request.AddCookie("JSESSIONID", cookie);

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new List<JiraTeet>();
            }

            var jResponse = JObject.Parse(response.Content);

            return jResponse["issues"]
                .Where(i => i["key"].Value<string>().Contains("TEET"))
                .Select(i => new JiraTeet
                {
                    Id = i["key"].Value<string>(),
                    Summary = i["fields"]["summary"].Value<string>()
                }).Take(10);
        }

        public ActionResult Edit(int packId)
        {
            var model = GetPackModel(packId);

            model.SetUser(Infrastructure.Helper.GetUserModel());
            model.HistoryPackCollection = this.PrimeEwsiDbApi.GetHistoryPacksByUserId(model.UserId);

            return View("Create", model);
        }

        public ActionResult Download(int packId) => this.Download(GetPackModel(packId));

        public ActionResult CreateIvec(int packId) => this.CreateIvec(GetPackModel(packId));

        private PackModel GetPackModel(int packId)
        {
            var pack = this.PrimeEwsiDbApi.GetHistoryPacksByPackId(packId);

            var packModel = new PackModel
            {
                Component = pack.Component,
                ProjectId = pack.Projects,
                Teets = pack.Teets,
                TestEnvironment = pack.Environment,
                Files = pack.Files.Split('|').ToList()
            };

            return packModel;
        }

        [HttpPost]
        [MultipleButtonAttribute(Name = "action", Argument = "Download")]
        public ActionResult Download(PackModel packModel)
        {
            packModel.SetUser(Infrastructure.Helper.GetUserModel());

            if (Validate(packModel))
            {
                packModel.HistoryPackCollection = this.PrimeEwsiDbApi.GetHistoryPacksByUserId(packModel.UserId);

                return View("Create", packModel);
            }

            var zipFileInfo = this.PackApi.CreatePackFile(packModel);

            var contentDisposition = new ContentDisposition
            {
                FileName = zipFileInfo.Name,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

            return File(System.IO.File.ReadAllBytes(zipFileInfo.FullName), MimeMapping.GetMimeMapping(zipFileInfo.Name));
        }

        [HttpPost]
        [MultipleButtonAttribute(Name = "action", Argument = "Send")]
        public ActionResult CreateIvec(PackModel packModel)
        {
            packModel.SetUser(Infrastructure.Helper.GetUserModel());

            packModel.HistoryPackCollection = this.PrimeEwsiDbApi.GetHistoryPacksByUserId(packModel.UserId);
            
            if (Validate(packModel))
            {
                return View("Create", packModel);
            }

            var packFile = this.PackApi.CreatePackFile(packModel);

            using (var client = new WebClient())
            {
#if DEBUG
                client.Credentials = new NetworkCredential(packModel.UserSkp.Substring(9), "AP6sYG9ktmWsTVcSp5roxfFytckrqyFXvxx6hN");
#else
                client.Credentials = new NetworkCredential(packModel.UserSkp.Substring(9), packModel.UserApiKey);
#endif
                var resultByte = client.UploadFile(new Uri($"{SERVERURL}{packFile.Name}"), "PUT", packFile.FullName);
                  
                packModel.SendModel = new SendModel
                {
                    Result = Infrastructure.Helper.FormatJson(Encoding.UTF8.GetString(resultByte))
                };

                return View("Create", packModel);
            }
        }

        private bool Validate(PackModel packModel)
        {
            if (string.IsNullOrEmpty(packModel.Component))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Component] - uzupełnij komponent");
            }

            if (string.IsNullOrEmpty(packModel.ProjectId))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Projects] - uzupełnij projekty");
            }

            if (string.IsNullOrEmpty(packModel.TestEnvironment))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Environment] - uzupełnij środowisko");
            }

            return !this.ModelState.IsValid;
        }
    }
}