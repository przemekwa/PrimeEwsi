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

        public PackApi PackApi { get; set; } = new PackApi(new PrimeEwsiContext(), new UrbanCodeMetaFIleApi());

        public ActionResult Create()
        {
            var userModel = GetUserModel();

            if (userModel == null)
            {
                return RedirectToAction("New", "Register");
            }

            return View(new PackModel(userModel)
            {
                HistoryPackCollection = this.PrimeEwsiContext.PackCollection.Where(p => p.UserId == userModel.Id)
                //Teets = "Teet-34353",
                //TestEnvironment = "ZT001 - POZPP07",
                //Files =
                //    new List<string>()
                //    {
                //        "http://centralsourcesrepository/svn/svn7/trunk/OtherCS/IncomingsSln/SQL/wbk_create_fee.sql"
                //    },
            });
        }



        [HttpPost]
        [MultipleButtonAttribute(Name = "action", Argument = "Download")]
        public ActionResult Add(PackModel packModel)
        {
            packModel.InitUser(GetUserModel());

            if (Validate(packModel))
            {
                packModel.HistoryPackCollection = this.PrimeEwsiContext.PackCollection.Where(p => p.UserId == packModel.Id);

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
        public ActionResult Send(PackModel packModel)
        {
            packModel.InitUser(this.GetUserModel());

            if (Validate(packModel))
            {
                packModel.HistoryPackCollection = this.PrimeEwsiContext.PackCollection.Where(p => p.UserId == packModel.Id);

                return View("Create", packModel);
            }

            var packFile = this.PackApi.CreatePackFile(packModel);

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(packModel.Skp.Substring(9), packModel.ApiKey);

                var resultByte = client.UploadFile(
                    new Uri($"https://wro2096v.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/{packFile.Name}.zip"), "PUT",
                    packFile.FullName);

                return View("Send", Encoding.UTF8.GetString(resultByte));
            }
        }

        private bool Validate(PackModel packModel)
        {
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

            if (string.IsNullOrEmpty(packModel.TestEnvironment))
            {
                this.ModelState.AddModelError("Błąd", "Pole [Environment] - uzupełnij środowisko");
            }

            return !this.ModelState.IsValid;
        }

        private UserModel GetUserModel()
        {
            var userSkp = this.HttpContext.User.Identity.Name;

            var userModel = this.PrimeEwsiContext.UsersModel.SingleOrDefault(m => m.Skp == userSkp);

            return userModel;
        }
    }
}