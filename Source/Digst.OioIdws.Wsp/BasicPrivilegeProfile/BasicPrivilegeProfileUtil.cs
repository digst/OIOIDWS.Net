using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Digst.OioIdws.Wsp.BasicPrivilegeProfile
{
    public class BasicPrivilegeProfileUtil
    {
        /// <summary>
        /// Helper method for deserializing a base64 encoded privilege assertion according to OIOSAML Basic Privilege Profile 1.0.1
        /// </summary>
        /// <param name="base64EncodedPrivilegeList"></param>
        /// <returns></returns>
        public static PrivilegeListType DeserializeBase64EncodedPrivilegeList(string base64EncodedPrivilegeList)
        {
            if (base64EncodedPrivilegeList == null) throw new ArgumentNullException(nameof(base64EncodedPrivilegeList));

            var privilegeListAsXml = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedPrivilegeList));
            XmlSerializer serializer = new XmlSerializer(typeof(PrivilegeListType));
            StringReader rdr = new StringReader(privilegeListAsXml);
            return (PrivilegeListType)serializer.Deserialize(rdr);
        }
    }
}