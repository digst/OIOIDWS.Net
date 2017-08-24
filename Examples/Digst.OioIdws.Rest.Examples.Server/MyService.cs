using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Digst.OioIdws.Rest.Examples.WSP
{
    class MyService : OwinMiddleware
    {
        public MyService(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            context.Response.Write($"Requested at {context.Request.Uri}\n");

            if (context.Request.User == null || !context.Request.User.Identity.IsAuthenticated)
            {
                context.Response.Write("User is not authenticated!\n");
            }
            else
            {
                var identity = (ClaimsIdentity)context.Request.User.Identity;

                context.Response.Write(
$@"AuthenticationType: {identity.AuthenticationType}
Claims:
{string.Join("\n", identity.Claims.Select(x => $"{x.Type} = {x.Value}"))}
");
            }

            return Task.FromResult(0);
        }
    }
}
