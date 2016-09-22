using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using PrimeEwsi.Models;
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
            var dc = new DeploymentPackageDeploymentComponent
            {
                Name = packModel.Component,
                Version = new DeploymentPackageDeploymentComponentVersion
                {
                    Type = "Full",
                    Property = (new List<DeploymentPackageDeploymentComponentProperty>
                    {
                        new DeploymentPackageDeploymentComponentProperty
                        {
                        Name = "dsad"
                    }}).ToArray(),

                    
                    FileList = packModel.Files.Split(new []  { "\\r\\n"}, StringSplitOptions.RemoveEmptyEntries)
                }
            };

            Helper.Manifestfilename = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(null), "manifest.xml");

            var xmlFile = Helper.SaveXml(new DeploymentPackage
            {
                DeploymentComponent = new DeploymentPackageDeploymentComponent[1] {dc}
            });

            var pathToZipFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(null), "file.zip");

            Helper.CreateZipFile(new List<FileInfo>() {xmlFile}, pathToZipFile);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "file.zip",
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(System.IO.File.ReadAllBytes(pathToZipFile), MimeMapping.GetMimeMapping("file.zip"));
        }
    }
}