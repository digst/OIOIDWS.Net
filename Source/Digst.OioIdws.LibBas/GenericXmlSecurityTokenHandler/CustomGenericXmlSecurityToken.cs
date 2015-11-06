//using System;
//using System.IdentityModel.Tokens;
//using System.Xml;

//namespace Digst.OioIdws.LibBas.GenericXmlSecurityTokenHandler
//{
//    public class CustomGenericXmlSecurityTokenHandler : SecurityTokenHandler
//    {
//        private int callNumber = 0;
//        public override string[] GetTokenTypeIdentifiers()
//        {
//            return new string[] {null};
//        }

//        public override Type TokenType { get { return typeof (GenericXmlSecurityToken); } }

//        public override void WriteToken(XmlWriter writer, SecurityToken token)
//        {
//            var genericXmlSecurityToken = token as GenericXmlSecurityToken;

//            if(genericXmlSecurityToken == null)
//                throw new InvalidOperationException(string.Format("Token was of type {0}, expected type was {1}.", token.GetType().FullName, typeof(GenericXmlSecurityToken).FullName));

//            var xmlElement = genericXmlSecurityToken.TokenXml;

//            if (callNumber%2 == 0)
//            {
//                xmlElement.FirstChild.WriteTo(writer);
//            }
//            else
//            {
//                xmlElement.WriteTo(writer);
//            }
//            callNumber++;
//        }

//        public override bool CanWriteToken { get { return true; } }
//    }
//}