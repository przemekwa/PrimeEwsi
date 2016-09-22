using System.Collections.Generic;
using System.IO;
using System.Linq;
using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;

namespace UrbanCodeMetaFile.Component
{

    public class ComponentBase
    {
        protected ControlFileDto ControlFileDto { get; private set; }

        private readonly string path;

        protected string Component { get; }

        public ComponentBase(string path, string component)
        {
            this.path = path;
            this.Component = component;
            this.ControlFileDto = this.GetFilesFromKeyValueFile();
        }

        protected ControlFileDto GetFilesFromKeyValueFile()
        {
            var file = Helper.GetKeyValueControlFile(path);

            file.DeploymentPackage.First().DeploymentComponentName = Component;

            return file;
        }

        protected ControlFileDto GetControlFile()
        {
            return Helper.GetJsonControlFile(path);
        }
    }
}