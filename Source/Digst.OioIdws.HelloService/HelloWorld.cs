using System;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;

namespace Digst.OioIdws.HelloService
{
    public class HelloWorld : IHelloWorld
    {
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

        public string HelloSignErrorNotEncrypted(string name)
        {
            throw new FaultException<string>("DetailInfo",
                $"Hello SignError {name}. You can read signed but not encrypted SOAP faults ... nice!");
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
            }

            return stringBuilder.ToString();
        }
    }
}
