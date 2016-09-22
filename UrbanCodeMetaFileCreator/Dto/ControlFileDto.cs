using System.Collections.Generic;

namespace UrbanCodeMetaFileCreator.Dto
{

    public class ControlFileDto
    {
        public Svncredential SvnCredential { get; set; }
        public Deploymentpackage[] DeploymentPackage { get; set; }
    }

    public class Svncredential
    {
        public string User { get; set; }
        public string Pasword { get; set; }
    }

    public class Deploymentpackage
    {
        public string DeploymentComponentName { get; set; }
        public Property[] Properties { get; set; }
        public SqlFile[] Files { get; set; }
    }
}