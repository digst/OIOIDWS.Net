namespace Digst.OioIdws.TestDoubles
{
    public interface IAuthenticationAttributeProviderFactory
    {
        IAttributeProvider Create(AuthenticationDescriptor authentication);
    }
}