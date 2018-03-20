namespace Digst.OioIdws.Wsp.Wsdl.Utils.Parse
{
    using System;
    using System.Collections.Generic;

    public class Element : IDisposable
    {
        private string name;
        private string value;
        private string namespaceUri;

        private List<Attribute> attributes;
        private List<Element> children;

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

        public List<Attribute> Attributes
        {
            get
            {
                if (attributes == null) attributes =
                        new List<Attribute>();
                return attributes;
            }
        }
        public List<Element> Children
        {
            get
            {
                if (children == null) children =
                        new List<Element>();
                return children;
            }
        }

        public void Dispose()
        {
            foreach (var c in this.children)
            {
                c.Dispose();
            }
        }
    }
}