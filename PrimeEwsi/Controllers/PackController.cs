using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Models;
using RestSharp;
using RestSharp.Authenticators;
using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;

namespace PrimeEwsi.Controllers
{
    public class PackController : Controller
    {

        public PrimeEwsiContext PrimeEwsiContext { get; set; } = new PrimeEwsiContext();

   // GET: Create
        public ActionResult Create()
        {
            var userModel = GetUserModel();

            if (userModel == null)
            {
                return RedirectToAction("New", "Register");
            }

            var random = new Random();
            
            return View(new PackModel(userModel)
            {
                HistoryPackCollection = this.PrimeEwsiContext.PackCollection.Where(p => p.UserId == userModel.Id),
                Teets = "Teet-34353",
                TestEnvironment = "ZT001 - POZPP07",
                Files = new List<string>() {  "http://centralsourcesrepository/svn/svn7/trunk/OtherCS/IncomingsSln/SQL/wbk_create_fee.sql"},
                ProjectId = "Production Operations"

            });
        }

        private UserModel GetUserModel()
        {
            var userSkp = this.HttpContext.User.Identity.Name;

            var userModel = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == userSkp);
            return userModel;
        }

        public void UpdateVersion(string component)
        {
            var configModel = this.PrimeEwsiContext.ConfigModel.SingleOrDefault(m => m.Component == component);

            configModel.Version = configModel.Version+1;

            this.PrimeEwsiContext.SaveChanges();
        }

        public void AddPackToHistory(PackModel packModel)
        {
            this.PrimeEwsiContext.PackCollection.Add(new Pack
            {
                Component = packModel.Component,
                Environment = packModel.TestEnvironment,
                //Files = packModel.Files.,
                Projects = packModel.ProjectId,
                Teets = packModel.Teets,
                UserId = GetUserModel().Id
            });

            this.PrimeEwsiContext.SaveChanges();
        }

        [HttpPost]
        [MultipleButtonAttribute(Name = "action", Argument = "Download")]
        public ActionResult Add(PackModel packModel)
        {
            var userModel = GetUserModel();
            var zipFileInfo = GetPack(packModel, userModel);

            return File(System.IO.File.ReadAllBytes(zipFileInfo.FullName), MimeMapping.GetMimeMapping(zipFileInfo.Name));
        }

        [HttpPost]
        [MultipleButtonAttribute(Name = "action", Argument = "Send")]
        public ActionResult Send(PackModel packModel)
        {
            var userModel = GetUserModel();

            if (string.IsNullOrEmpty(packModel.Component))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Component] - uzupełnij komponent");
            }

            if (string.IsNullOrEmpty(packModel.Teets))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Teets] - uzupełnij teet-y");
            }

            if (string.IsNullOrEmpty(packModel.ProjectId))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Projects] - uzupełnij projekty");
            }


            if (!this.ModelState.IsValid)
            {
                packModel.Name = userModel.Name;

                packModel.HistoryPackCollection =
                    this.PrimeEwsiContext.PackCollection.Where(p => p.UserId == userModel.Id);
                return View("Create", packModel);
            }

          

            var zipFileInfo = GetPack(packModel, userModel);

            using (var client = new WebClient())
            {
                var skp = userModel.Skp.Substring(9);

                client.Credentials = new NetworkCredential(skp, userModel.ApiKey);

                var resultByte = client.UploadFile(
                    new Uri($"https://wro2096v.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/{zipFileInfo.Name}.zip"), "PUT",
                    zipFileInfo.FullName);

                return View("Send", Encoding.UTF8.GetString(resultByte));
            }
        }

        private FileInfo GetPack(PackModel packModel, UserModel userModel)
        {
            var pathToFolderWithUserPack = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), "Pack", userModel.Name, packModel.Component);

            if (Directory.Exists(pathToFolderWithUserPack))
            {
                Directory.Delete(pathToFolderWithUserPack, true);
            }

            Directory.CreateDirectory(pathToFolderWithUserPack);


            var svnUrls =
                packModel.Files.ToArray().Select(d => new SqlFile
                {
                    Name = d.Substring(d.LastIndexOf("/") + 1),
                    URL = d
                });

            var nc = new NetworkCredential
            {
                UserName = userModel.SvnUser,
                Password = userModel.SvnPassword
            };

            var listOfLiles = svnUrls.Select(svnUrl => Helper.DownloadFileUsingWebClient(svnUrl, nc, pathToFolderWithUserPack)).ToList();

            var dc = new DeploymentPackageDeploymentComponent
            {

                Name = packModel.Component,
                Version = new DeploymentPackageDeploymentComponentVersion
                {
                    Name = this.PrimeEwsiContext.ConfigModel.Single(c => c.Component == packModel.Component).Version.ToString(),
                    Type = "Incremental",
                    Property = (new List<DeploymentPackageDeploymentComponentProperty>()).ToArray(),
                    FileList = svnUrls.Select(d => d.Name).ToArray()
                }
            };


            Helper.Manifestfilename = Path.Combine(pathToFolderWithUserPack, "metafile.xml");

            var xmlFile = Helper.SaveXml(new DeploymentPackage
            {
                provider = "BZWBK",
                system = "PRIME",
                TestEnvironment = packModel.TestEnvironment,
                IchangeProjects = new List<string> { packModel.ProjectId }.ToArray(),
                ResolvedIssues = packModel.Teets.Split(','),
                DeploymentComponent = new DeploymentPackageDeploymentComponent[1] { dc }
            });

            listOfLiles.Add(xmlFile);

            var zipFileInfo =
                new FileInfo(Path.Combine(pathToFolderWithUserPack, $"{packModel.Component}-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture)}.zip"));

            Helper.CreateZipFile(listOfLiles, zipFileInfo.FullName);

            var cd = new ContentDisposition
            {
                FileName = zipFileInfo.Name,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            this.UpdateVersion(packModel.Component);

            AddPackToHistory(packModel);

            return zipFileInfo;
        }
    }
}