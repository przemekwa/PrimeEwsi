﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using PrimeEwsi.Models;
using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;

namespace PrimeEwsi
{
    public class PackApi : IPackApi
    {
        public IPrimeEwsiDbApi PrimeEwsiDbApi { get; set; }

        public PackApi(IPrimeEwsiDbApi primeEwsiDbApi)
        {
            PrimeEwsiDbApi = primeEwsiDbApi;
        }

        public FileInfo CreatePackFile(PackModel packModel)
        {
            var dir = CreateDirectory(packModel);

            var listOfLies = this.GetFileForSVN(packModel, dir).ToList();

            var dc = new DeploymentPackageDeploymentComponent
            {

                Name = packModel.Component,
                Version = new DeploymentPackageDeploymentComponentVersion
                {
                    Name = this.PrimeEwsiDbApi.GetConfigModelByComponent(packModel.Component).Version,
                    Type = "Incremental",
                    Property = (new List<DeploymentPackageDeploymentComponentProperty>()).ToArray(),
                    FileList = listOfLies.Select(d => d.Name).ToArray()
                }
            };


            Helper.Manifestfilename = Path.Combine(dir, "metafile.xml");

            var xmlFile = Helper.SaveXml(new DeploymentPackage
            {
                provider = "BZWBK",
                system = "PRIME",
                TestEnvironment = packModel.TestEnvironment,
                IchangeProjects = new List<string> { packModel.ProjectId }.ToArray(),
                ResolvedIssues = packModel.Teets?.Split(',').Select(s=>s.Trim()).ToArray(),
                DeploymentComponent = new DeploymentPackageDeploymentComponent[1] { dc }
            });

            listOfLies.Add(xmlFile);

            var zipFileInfo =
                new FileInfo(Path.Combine(dir, $"{packModel.Component}-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss", CultureInfo.InvariantCulture)}.zip"));

            Helper.CreateZipFile(listOfLies, zipFileInfo.FullName);


            this.UpdateVersion(packModel.Component);

            AddPackToHistory(packModel);

            return zipFileInfo;
        }

        public void AddPackToHistory(PackModel packModel)
        {
            this.PrimeEwsiDbApi.AddHistoryPack(new HistoryPackModel
            {
                Component = packModel.Component,
                Environment = packModel.TestEnvironment,
                Files = string.Join("|", packModel.Files),
                Projects = packModel.ProjectId,
                Teets = packModel.Teets,
                UserId = packModel.UserId
            });
        }

        public void UpdateVersion(string component)
        {
            var configModel = this.PrimeEwsiDbApi.GetConfigModelByComponent(component);

            configModel.Version = Infrastructure.Helper.UpdateVersion(configModel.Version);

            this.PrimeEwsiDbApi.UpdateConfig(configModel);
        }

        private IEnumerable<FileInfo> GetFileForSVN(PackModel packModel, string dirPath )
        {
            var svnUrls =
               packModel.Files.ToArray().Select(d => new SqlFile
               {
                   Name = d.Substring(d.LastIndexOf("/") + 1),
                   URL = d
               });

            var nc = new NetworkCredential
            {
                UserName = packModel.SvnUser,
                Password = packModel.SvnPassword
            };

            return svnUrls.Select(svnUrl => Helper.DownloadFileUsingSvnCheckout(svnUrl, nc, dirPath));
        }

        private string CreateDirectory(PackModel packModel)
        {
            var pathToFolderWithUserPack = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), "Pack", packModel.UserName, packModel.Component);

            if (Directory.Exists(pathToFolderWithUserPack))
            {
                Directory.Delete(pathToFolderWithUserPack, true);
            }

            Directory.CreateDirectory(pathToFolderWithUserPack);

            return pathToFolderWithUserPack;
        }

    }
}