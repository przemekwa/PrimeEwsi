using System.Xml.Serialization;

namespace UrbanCodeMetaFileCreator.Dto
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DeploymentPackageDeploymentComponentVersion
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("FileName", IsNullable = false)]
        public string[] FileList { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Property", IsNullable = false)]
        public DeploymentPackageDeploymentComponentProperty[] Property { get; set; }
    }

    public class DeploymentPackageDeploymentComponentProperty
    {
        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public string Value { get; set; }
    }


}