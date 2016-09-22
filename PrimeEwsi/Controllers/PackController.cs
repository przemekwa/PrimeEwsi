﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
            UserModel userModel = GetUserModel();

            if (userModel == null)
            {
                return RedirectToAction("New", "Register");
            }

            return View(new PackModel(userModel)
            {
                Teets = new List<string> { "Teest1" }
            });
        }

        private UserModel GetUserModel()
        {
            var userSkp = this.HttpContext.User.Identity.Name;

            var userModel = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == userSkp);
            return userModel;
        }

        public ActionResult Add(PackModel packModel)
        {
            UserModel userModel = GetUserModel();

            var svnUrls =
                packModel.Files.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Select(d => new SqlFile
                {
                    Name = d.Substring(d.LastIndexOf("/")+1),
                    URL = d
                });

            var nc = new NetworkCredential {UserName = userModel.SvnUser, Password = userModel.SvnPassword};

            var listOfLiles = svnUrls.Select(svnUrl => Helper.DownloadFileUsingWebClient(svnUrl, nc, System.Web.HttpContext.Current.Server.MapPath(null))).ToList();

            var dc = new DeploymentPackageDeploymentComponent
            {
                Name = packModel.Component,
                Version = new DeploymentPackageDeploymentComponentVersion
                {
                    Type = "Full",
                    Property = (new List<DeploymentPackageDeploymentComponentProperty>()).ToArray(),
                    FileList = svnUrls.Select(d=>d.Name).ToArray()
                }
            };

            Helper.Manifestfilename = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(null), "manifest.xml");

            var xmlFile = Helper.SaveXml(new DeploymentPackage
            {
                provider = "BZWBK",
                system = "PRIME",
                IchangeProjects = new List<string> { packModel.ProjectId}.ToArray(),
                ResolvedIssues = packModel.Teets.ToArray(),
                DeploymentComponent = new DeploymentPackageDeploymentComponent[1] {dc}
            });

            listOfLiles.Add(xmlFile);

            var zipFileInfo =
                new FileInfo(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(null), $"{packModel.Component}-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture)}.zip" ));

            Helper.CreateZipFile(listOfLiles, zipFileInfo.FullName);

            var cd = new ContentDisposition
            {
                FileName = zipFileInfo.Name,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(System.IO.File.ReadAllBytes(zipFileInfo.FullName), MimeMapping.GetMimeMapping(zipFileInfo.Name));
        }
    }
}