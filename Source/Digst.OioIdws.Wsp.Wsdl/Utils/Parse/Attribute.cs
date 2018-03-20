namespace Digst.OioIdws.Wsp.Wsdl.Utils.Parse
{
    public class Attribute
    {
        private string name;
        private string value;

        private string namespaceUri;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string NamespaceUri
        {
            get { return this.namespaceUri; }
            set { this.namespaceUri = value; }
        }
    }
}