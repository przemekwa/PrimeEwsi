using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;

namespace UrbanCodeMetaFile.Component
{
    public class SqlComponent : ComponentBase, IComponent
    {
        public SqlComponent(string path, string component) : base(path, component)
        {
            this.Name = component;
        }

        public IEnumerable<FileInfo> Files { get; private set; }

        public string Name { get; }

        public DeploymentPackageDeploymentComponent GetDeploymentComponent()
        {
            this.GetFiles();

            return new DeploymentPackageDeploymentComponent
            {
                Name = base.Component,
                Version = new DeploymentPackageDeploymentComponentVersion
                {
                    Type = "Full",
                    Property = this.ControlFileDto.DeploymentPackage.Single(d=>d.DeploymentComponentName == this.Component).Properties.ToList().Select(p => new DeploymentPackageDeploymentComponentProperty
                    {
                        Name = p.Name,
                        Value= p.Value
                   }).ToArray(),
                    FileList = this.Files.Select(f => f.Name).ToArray()
                }
            };
        }

        private void GetFiles()
        {


            this.Files = base.ControlFileDto.DeploymentPackage.Single(d => d.DeploymentComponentName == base.Component)
                    .Files.Select(sqlFile => Helper.DownloadFileUsingWebClient(sqlFile, new NetworkCredential
                    {
                        Password = base.ControlFileDto.SvnCredential.Pasword,
                        UserName = base.ControlFileDto.SvnCredential.User
                    }));
        }
    }
}