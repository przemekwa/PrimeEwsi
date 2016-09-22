using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;

namespace UrbanCodeMetaFileCreatorTests
{
    public class Test1
    {
        public short Gry()
        {
            short d = 4636;
            return d;
        } 
        
    }

    public class Tests
    {
        private const string CONTROLFILEPATH= "d:\\Visual Studio 2010\\Projects\\UrbanCodeMetaFileCreator\\UrbanCodeMetaFileCreatorTests\\bin\\Debug\\controlFile.json";

        [Test]
        public void Get_Json_File_No_File_Test()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Helper.GetJsonControlFile(
                    "d:\\Visual Studio 2010\\Projects\\UrbanCodeMetaFileCreator\\UrbanCodeMetaFileCreatorTests\\bin\\Debug\\controlFile.json2");
            });
        }

        [Test]
        public void Get_Json_File_Test()
        {
            var controlFile = Helper.GetJsonControlFile(CONTROLFILEPATH);

            Assert.AreEqual(2, controlFile.DeploymentPackage.First().Files.Length);

            Assert.AreEqual("127356", controlFile.SvnCredential.User);
        }

        [Test]
        public void Save_Xml_File_Test()
        {
            Helper.SaveXml(new DeploymentPackage
            {
                DeploymentComponent = new[] { new DeploymentPackageDeploymentComponent
                {
                    Name = "dsda",
                    Version = new DeploymentPackageDeploymentComponentVersion
                    {
                        Type = "dsad",
                        FileList = new[] {"dsadasd", "asfasfa"}
                    }
                }, new DeploymentPackageDeploymentComponent
                {
                    Name = "dsda",
                    Version = new DeploymentPackageDeploymentComponentVersion
                    {
                        Type = "dsad",
                        FileList = new[] {"dsadasd"}
                    }
                }}
            });
        }
     
        [Test]
        public void Downolad_File_Using_Web_Client()
        { 
            var controlFile = Helper.GetJsonControlFile(CONTROLFILEPATH);
             
            foreach (var file in controlFile.DeploymentPackage.First().Files)
            {
                FileInfo fileInfo = Helper.DownloadFileUsingWebClient(file,
                    new NetworkCredential
                    {
                        Password = controlFile.SvnCredential.Pasword,
                        UserName = controlFile.SvnCredential.User
                    });

                Assert.AreEqual(true, fileInfo.Exists);
            }

        }

        [Test]
        public void Create_Manifest_From_Cotrol_File_Test()
        {
            var controlFile = Helper.GetJsonControlFile(CONTROLFILEPATH);

            var listOfFiles = controlFile.DeploymentPackage.First().Files.Select(sqlFile => Helper.DownloadFileUsingWebClient(sqlFile, new NetworkCredential
            {
                Password = controlFile.SvnCredential.Pasword,
                UserName = controlFile.SvnCredential.User
            })).ToList();


            var dp = Helper.GetDeploymentPackage("Addon_Live", Helper.GetDeploymentPackageDeploymentComponentVersion(listOfFiles, "Incremental"));

            Helper.SaveXml(dp);

            var xmls = new XmlSerializer(typeof(DeploymentPackage));

            using (var sr = new StreamReader(Helper.Manifestfilename))
            {
                var dp2 = (DeploymentPackage)xmls.Deserialize(sr);

                Assert.AreEqual(dp.DeploymentComponent.First().Name, dp2.DeploymentComponent.First().Name);
                Assert.AreEqual(false, string.IsNullOrEmpty(dp2.DeploymentComponent.First().Name));
                Assert.AreEqual(dp.DeploymentComponent.First().Version.Type, dp2.DeploymentComponent.First().Version.Type);
                Assert.AreEqual(false, string.IsNullOrEmpty(dp.DeploymentComponent.First().Version.Type));
                CollectionAssert.AreEqual(dp.DeploymentComponent.First().Version.FileList, dp2.DeploymentComponent.First().Version.FileList);
            }

        }

        [Test]
        public void Get_Deployment_Package_From_Control_File()
        {
            File.Delete(Helper.Manifestfilename);

            var controlFile = Helper.GetJsonControlFile(CONTROLFILEPATH);

            var listOfFiles = controlFile.DeploymentPackage.First().Files.Select(sqlFile => Helper.DownloadFileUsingWebClient(sqlFile, new NetworkCredential
            {
                Password = controlFile.SvnCredential.Pasword,
                UserName = controlFile.SvnCredential.User
            })).ToList();

            Assert.AreEqual(2, Helper.GetDeploymentPackageDeploymentComponentVersion(listOfFiles, "Incremental").FileList.Length);

        }

        [Test]
        public void Zip_List_Of_File()
        {
            Helper.CreateZipFile(Directory.GetFiles(Path.GetTempPath()).Take(4).Select(f=>new FileInfo(f)), Path.GetTempPath());

            Assert.AreEqual(true, File.Exists(Path.Combine(Path.GetTempPath(), "package.zip")));

        }

       [Test]
        public void Dev_Tests()
        {
     
        }
    }
}
