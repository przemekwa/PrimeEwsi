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

using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;


namespace PrimeEwsi.Controllers
{
    using Infrastructure;

    [HandleErrorException]
    public class PackController : Controller
    {
        public PrimeEwsiContext PrimeEwsiContext { get; set; } 

        public PackApi PackApi { get; set; }

#if DEBUG
        private const string SERVERURL = "https://wro2096v.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/";
#else
        private const string SERVERURL = "https://ewsi.centrala.bzwbk:9999/artifactory/bzwbk-tmp/BZWBK/PRIME/";
#endif

        public PackController()
        {
            this.PrimeEwsiContext = new PrimeEwsiContext();
            this.PackApi = new PackApi(new PrimeEwsiContext(), new UrbanCodeMetaFIleApi());
        }

        public ActionResult Create()
        {
            var userModel = Helper.GetUserModel();

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
            packModel.InitUser(Helper.GetUserModel());

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
            packModel.InitUser(Helper.GetUserModel());

            packModel.HistoryPackCollection = this.PrimeEwsiContext.PackCollection.Where(p => p.UserId == packModel.Id);
            
            if (Validate(packModel))
            {
                return View("Create", packModel);
            }

            var packFile = this.PackApi.CreatePackFile(packModel);

            using (var client = new WebClient())
            {
#if DEBUG
                client.Credentials = new NetworkCredential(packModel.Skp.Substring(9), "AP6sYG9ktmWsTVcSp5roxfFytckrqyFXvxx6hN");
#else
                client.Credentials = new NetworkCredential(packModel.Skp.Substring(9), packModel.ApiKey);
#endif
                var resultByte = client.UploadFile(new Uri($"{SERVERURL}{packFile.Name}"), "PUT", packFile.FullName);
                  
                packModel.SendModel = new SendModel
                {
                    Result = Helper.FormatJson(Encoding.UTF8.GetString(resultByte))
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