namespace UrbanCodeMetaFileCreator.Dto
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DeploymentPackageDeploymentComponent
    {
        /// <remarks/>
        public DeploymentPackageDeploymentComponentVersion Version { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("name")]
        public string Name { get; set; }
    }
}