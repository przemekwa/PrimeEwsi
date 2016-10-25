
namespace PrimeEwsi.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using Models;

    using UrbanCodeMetaFileCreator;
    using Infrastructure;
    using Infrastructure.Jira;
    using SimpleInjector;

    [HandleErrorException]
    public class PackController : Controller
    {
        public IPackApi PackApi { get; set; }

        public IPrimeEwsiDbApi PrimeEwsiDbApi { get; set; }

        public IJiraApi JiraApi { get; set; }

#if DEBUG
        private const string SERVERURL = "https://wro2096v.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/";
#else
        private const string SERVERURL = "https://ewsi.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/";
#endif

        public PackController(IPackApi packApi, IPrimeEwsiDbApi primeEwsiDbApi, IJiraApi jiraApi)
        {
            PackApi = packApi;
            PrimeEwsiDbApi = primeEwsiDbApi;
            JiraApi = jiraApi;
        }

        public ActionResult Create()
        {
            var userModel = Infrastructure.Helper.GetUserModel();

            if (userModel == null)
            {
                return RedirectToAction("New", "Register");
            }

            var model = new PackModel();

            this.FillPackModel(model);

            return View(model);
        }

        public ActionResult Edit(int packId)
        {
            var model = GetPackModel(packId);

            this.FillPackModel(model);
            
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
                Files = pack.Files.Split('|').ToList(),
               
            };

            return packModel;
        }

        [HttpPost]
        [MultipleButtonAttribute(Name = "action", Argument = "Download")]
        public ActionResult Download(PackModel packModel)
        {
            this.FillPackModel(packModel);

            if (Validate(packModel))
            {
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
            this.FillPackModel(packModel);

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

                var model = new PackModel
                {
                    SendModel = new SendModel
                    {
                        Result = Infrastructure.Helper.FormatJson(Encoding.UTF8.GetString(resultByte))
                    }
                };

                this.FillPackModel(model);

                return View("Create", model);
            }
        }

        private void FillPackModel(PackModel packModel)
        {
            packModel.SetUser(Infrastructure.Helper.GetUserModel());

            packModel.HistoryPackCollection = this.PrimeEwsiDbApi.GetHistoryPacksByUserId(packModel.UserId);

            packModel.JiraTeets = this.JiraApi.GetJiraTets(packModel.UserJiraCookie);

            packModel.JiraComponents = this.JiraApi.GetComponents(packModel.UserJiraCookie);

            packModel.JiraEnvironment = this.JiraApi.GetEnvironment(packModel.UserJiraCookie);

            packModel.JiraProject = this.JiraApi.GetProjects(packModel.UserJiraCookie);

        }

        private bool Validate(PackModel packModel)
        {
            if (!packModel.Files.Any())
            {
                this.ModelState.AddModelError("Błąd", "Pole [SVN Files] - uzupełnij pliki do paczki");
            }

            foreach (var file in packModel.Files)
            {
                if (!Regex.IsMatch(file, "http://.*"))
                {
                    this.ModelState.AddModelError("Błąd", "Pole [SVN Files] - podaj prawidłowy link do pliku");
                }
            }

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