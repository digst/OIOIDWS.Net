using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using Digst.OioIdws.Wsp.BasicPrivilegeProfile;

namespace Digst.OioIdws.WspExampleNuGet
{
    public class HelloWorld : IHelloWorld
    {
        public string HelloNone(string name)
        {
            return string.Format("Hello None {0}. Your claims are:\n{1}", name, GetClaims());
        }

        public string HelloSign(string name)
        {
            return string.Format("Hello Sign {0}. Your claims are:\n{1}", name, GetClaims());
        }

        public string HelloEncryptAndSign(string name)
        {
            return string.Format("Hello EncryptAndSign {0}. Your claims are:\n{1}", name, GetClaims());
        }

        private string GetClaims()
        {
            var identity = (ClaimsIdentity)(OperationContext.Current.ClaimsPrincipal.Identity);

            var stringBuilder = new StringBuilder();
            foreach (var claim in identity.Claims)
            {
                stringBuilder.AppendLine(string.Format("Type: {0}, Value: {1}", claim.Type, claim.Value));

                if ("dk:gov:saml:attribute:Privileges_intermediate" == claim.Type)
                {
                    var privilegeList = BasicPrivilegeProfileUtil.DeserializeBase64EncodedPrivilegeList(claim.Value);

                    foreach (var privilegeGroup in privilegeList.PrivilegeGroup)
                    {
                        stringBuilder.AppendLine(string.Format("\tPrivilege group scope: {0}", privilegeGroup.Scope));
                        foreach (var privilege in privilegeGroup.Privilege)
                        {
                            stringBuilder.AppendLine(string.Format("\t\tPrivilege: {0}", privilege));
                        }
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}
