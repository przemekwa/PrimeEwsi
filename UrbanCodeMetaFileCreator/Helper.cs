using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using Newtonsoft.Json;
//using SharpShell.Diagnostics;
//using SharpSvn;
using UrbanCodeMetaFileCreator.Dto;
using File = System.IO.File;

namespace UrbanCodeMetaFileCreator
{
    public static class Helper
    {
        public static string Manifestfilename = Path.Combine(Path.GetTempPath(), "manifest.xml");

        public static ControlFileDto GetJsonControlFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new  ArgumentException(path);
            }

            return JsonConvert.DeserializeObject<ControlFileDto>(File.ReadAllText(path));
        }

        public static ControlFileDto GetKeyValueControlFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException(path);
            }

            var files = new List<SqlFile>();
            var property = new List<Property>();

            using (var sr = new StreamReader(path))
            {
                var line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    var split = line.Split('=');

                    if (split[0].Equals("property", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var prop = split[1].Split(';');

                        property.Add(new Property
                        {
                            Name = prop[0],
                            Value = prop[1]
                        });

                        continue;
                    }

                    files.Add(new SqlFile
                    {
                        Name = split[0],
                        URL = split[1]
                    });
                }
            }

            return new ControlFileDto
            {
                SvnCredential = new Svncredential
                {
                    User = ConfigurationManager.AppSettings["SvnUser"],
                    Pasword = ConfigurationManager.AppSettings["SvnPassword"]
                },
                DeploymentPackage = new[]
                {
                    new Deploymentpackage()
                    {
                        Files = files.ToArray(),
                        Properties = property.ToArray()
                    }
                }
            };

        }

        public static FileInfo SaveXml(DeploymentPackage deploymentPackage)
        {
            if (deploymentPackage == null)
            {
                throw new ArgumentException(nameof(deploymentPackage));
            }

            foreach (var component in deploymentPackage.DeploymentComponent)
            {
               component.Version.Name = Guid.NewGuid().ToString();
            }

            var xmlSerializer = new XmlSerializer(typeof(DeploymentPackage));

            using (var sw = new StreamWriter(Manifestfilename))
            {
                xmlSerializer.Serialize(sw, deploymentPackage);
            }

            return new FileInfo(Manifestfilename);
        }

        //public static void DownloadFileUsingSvnCheckout(Uri adress)
        //{
        //    using (var client = new SvnClient())
        //    {
        //        client.CheckOut(adress, "d:\\sharpsvn\\Exensions.cs");
        //    }
        //}

        public static DeploymentPackage GetDeploymentPackage(string name, DeploymentPackageDeploymentComponentVersion version)
        {
            return new DeploymentPackage
            {
                DeploymentComponent = new[] { new DeploymentPackageDeploymentComponent
                {

                    Name = name,
                    Version = version

                } }
            };
        }

        public static DeploymentPackageDeploymentComponentVersion GetDeploymentPackageDeploymentComponentVersion(IEnumerable<FileInfo> listOfFiles, string type)
        {
            return new DeploymentPackageDeploymentComponentVersion
            {
                Type = type,
                FileList = listOfFiles.Select(f => f.Name).ToArray()
            };
        }

        public static FileInfo DownloadFileUsingWebClient(Dto.SqlFile sqlFile, NetworkCredential networkCredential, string serverPath)
        {
            var path = Path.Combine(serverPath, sqlFile.Name);

            using (var webClient = new WebClient())
            {
                webClient.Credentials = networkCredential;

                webClient.DownloadFile(sqlFile.URL, path);

                return new FileInfo(path);
            }
        }

        public static void CreateZipFile(IEnumerable<FileInfo> listOfFiles, string pathToFile)
        {
            using (FileStream zipToOpen = new FileStream(pathToFile , FileMode.OpenOrCreate))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (var fileInfo in listOfFiles)
                    {
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                }
            }
        }
    }
}
