using System;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using Digst.OioIdws.Wsp.BasicPrivilegeProfile;

namespace Digst.OioIdws.WspExampleNuGet
{
    public class HelloWorld : IHelloWorld
    {
        private const string OioSaml2PrivilegeClaimType = "dk:gov:saml:attribute:Privileges_intermediate";
        private const string OioSaml3PrivilegeClaimType = "https://data.gov.dk/model/core/eid/privilegesIntermediate";
        
        public string HelloNone(string name)
        {
            return $"Hello None {name}. Your claims are:\n{GetClaims()}";
        }

        public string HelloNoneError(string name)
        {
            throw new Exception($"Hello NoneError {name}. You can read encrypted SOAP faults ... nice!");
        }

        public string HelloSign(string name)
        {
            return $"Hello Sign {name}. Your claims are:\n{GetClaims()}";
        }

        public string HelloSignError(string name)
        {
            throw new Exception($"Hello SignError {name}. You can read encrypted SOAP faults ... nice!");
        }

        public string HelloEncryptAndSign(string name)
        {
            return $"Hello EncryptAndSign {name}. Your claims are:\n{GetClaims()}";
        }

        public string HelloEncryptAndSignError(string name)
        {
            throw new Exception($"Hello SignAndEncryptError {name}. You can read encrypted SOAP faults ... nice!");
        }
    
        private string GetClaims()
        {
            var identity = (ClaimsIdentity)(OperationContext.Current.ClaimsPrincipal.Identity);

            var stringBuilder = new StringBuilder();
            foreach (var claim in identity.Claims)
            {
                stringBuilder.AppendLine($"Type: {claim.Type}, Value: {claim.Value}");

                if (claim.Type == OioSaml2PrivilegeClaimType || claim.Type == OioSaml3PrivilegeClaimType)
                {
                    var privilegeList = BasicPrivilegeProfileUtil.DeserializeBase64EncodedPrivilegeList(claim.Value);

                    foreach (var privilegeGroup in privilegeList.PrivilegeGroup)
                    {
                        stringBuilder.AppendLine($"\tPrivilege group scope: {privilegeGroup.Scope}");
                        foreach (var privilege in privilegeGroup.Privilege)
                        {
                            stringBuilder.AppendLine($"\t\tPrivilege: {privilege}");
                        }
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}
