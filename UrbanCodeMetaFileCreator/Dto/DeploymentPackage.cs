namespace UrbanCodeMetaFileCreator.Dto
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class DeploymentPackage
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DeploymentComponent")]
        public DeploymentPackageDeploymentComponent[] DeploymentComponent { get; set; }
    }
}