namespace Digst.OioIdws.Common.Constants
{
    // Liberty Basic Soap Binding
    public class OioWsTrust
    {
        // Always set to encryptedassertion by NemLog-in STS. If this value becomes dynamic then the KeyIdentfier value must be compared to the id of the encrypted assertion.
        public const string EncryptedAssertionId = "encryptedassertion";
    }
}
