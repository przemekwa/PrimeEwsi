using System.Collections.Generic;
using System.IO;
using UrbanCodeMetaFileCreator.Dto;

namespace UrbanCodeMetaFile
{
    public interface IComponent
    {
        IEnumerable<FileInfo> Files { get; }
        string Name { get; }

        DeploymentPackageDeploymentComponent GetDeploymentComponent();
    }
}