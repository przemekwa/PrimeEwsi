using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
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

        public string ServerPath { get; } = System.Web.HttpContext.Current.Server.MapPath("~");

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
                Teets = new List<string> { "Teet-34353" },
                TestEnvironment = "ZT001 - POZPP07",
                Files = "http://centralsourcesrepository/svn/svn7/trunk/OtherCS/IncomingsSln/SQL/wbk_create_fee.sql",
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

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Download")]
        public ActionResult Download(PackModel packModel)
        {
            var userModel = GetUserModel();

            var zipFileInfo = GetPack(packModel, userModel);

            return File(System.IO.File.ReadAllBytes(zipFileInfo.FullName), MimeMapping.GetMimeMapping(zipFileInfo.Name));
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Send")]
        public ActionResult Send(PackModel packModel)
        {
            var userModel = GetUserModel();
            var zipFileInfo = GetPack(packModel, userModel);

            var bytes = System.IO.File.ReadAllBytes(zipFileInfo.FullName);

            var cookieJar = new CookieContainer();

            var restClient = new RestClient("https://wro2096v.centrala.bzwbk:9999/artifactory/")
            {
                Authenticator = new HttpBasicAuthenticator("127356", "AP6sYG9ktmWsTVcSp5roxfFytckrqyFXvxx6hN"),
                CookieContainer = cookieJar
            };


            var request = new RestRequest("/bzwbk-tmp/BZWBK/PRIME/", Method.PUT);
            request.AddHeader("Content-Type", "application/zip");
            request.AddHeader("Content-Disposition",
                string.Format("file; filename=\"{0}\"; documentid={1}; fileExtension=\"{2}\"",
                Path.GetFileNameWithoutExtension(zipFileInfo.Name), "1", Path.GetExtension(zipFileInfo.Name)));
            request.AddParameter("application/zip", bytes, ParameterType.RequestBody);



            //request.AddFile("a.zip", zipFileInfo.FullName, "application/zip");
                
            
            var resonse = restClient.Execute(request);

            

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("X-Auth_User", "127356");
            //client.DefaultRequestHeaders.Add("X-Auth-Key", "AP6sYG9ktmWsTVcSp5roxfFytckrqyFXvxx6hN");
            
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/pjpeg"));
            
            //var bytes = System.IO.File.ReadAllBytes(zipFileInfo.FullName);

            //var ms = new MemoryStream(bytes);

            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/pjpeg"));
            //var content = new StreamContent(ms);


            //var response =
            //    client.PutAsync("https://wro2096v.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/", content)
            //        .Result;

            

            return View("Send", "sds");

            //using (var streamReader = new StreamReader(response.GetResponseStream()))
            //{
            //    var answer = streamReader.ReadToEnd();

                
            //}
        }

        private FileInfo GetPack(PackModel packModel, UserModel userModel)
        {
            var svnUrls =
                            packModel.Files.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(d => new SqlFile
                            {
                                Name = d.Substring(d.LastIndexOf("/") + 1),
                                URL = d
                            });

            var nc = new NetworkCredential { UserName = userModel.SvnUser, Password = userModel.SvnPassword };

            var listOfLiles = svnUrls.Select(svnUrl => Helper.DownloadFileUsingWebClient(svnUrl, nc, this.ServerPath)).ToList();

            var dc = new DeploymentPackageDeploymentComponent
            {

                Name = packModel.Component,
                Version = new DeploymentPackageDeploymentComponentVersion
                {
                    Name = this.PrimeEwsiContext.ConfigModel.Single(c => c.Component == packModel.Component).Version.ToString(),
                    Type = "Full",
                    Property = (new List<DeploymentPackageDeploymentComponentProperty>()).ToArray(),
                    FileList = svnUrls.Select(d => d.Name).ToArray()
                }
            };

            Helper.Manifestfilename = Path.Combine(this.ServerPath, "metafile.xml");

            var xmlFile = Helper.SaveXml(new DeploymentPackage
            {
                provider = "BZWBK",
                system = "PRIME",
                TestEnvironment = packModel.TestEnvironment,
                IchangeProjects = new List<string> { packModel.ProjectId }.ToArray(),
                ResolvedIssues = packModel.Teets.ToArray(),
                DeploymentComponent = new DeploymentPackageDeploymentComponent[1] { dc }
            });

            listOfLiles.Add(xmlFile);

            var zipFileInfo =
                new FileInfo(Path.Combine(this.ServerPath, $"{packModel.Component}-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture)}.zip"));

            Helper.CreateZipFile(listOfLiles, zipFileInfo.FullName);

            var cd = new ContentDisposition
            {
                FileName = zipFileInfo.Name,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            this.UpdateVersion(packModel.Component);

            return zipFileInfo;
        }
    }
}