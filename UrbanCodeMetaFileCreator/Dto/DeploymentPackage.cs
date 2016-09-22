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

        private string[] resolvedIssuesField;

        private string[] ichangeProjectsField;

        private string providerField;

        private string systemField;


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string provider
        {
            get
            {
                return this.providerField;
            }
            set
            {
                this.providerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string system
        {
            get
            {
                return this.systemField;
            }
            set
            {
                this.systemField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("TKey", IsNullable = false)]
        public string[] ResolvedIssues
        {
            get
            {
                return this.resolvedIssuesField;
            }
            set
            {
                this.resolvedIssuesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Pname", IsNullable = false)]
        public string[] IchangeProjects
        {
            get
            {
                return this.ichangeProjectsField;
            }
            set
            {
                this.ichangeProjectsField = value;
            }
        }

    }
}