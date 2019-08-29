namespace Digst.OioIdws.SamlAttributes
{

    /// <summary>
    /// The AssuranceLevel attribute which provides the Service Provider an indication
    /// of how strongly the user was authenticated
    /// </summary>
    public enum AssuranceLevel
    {

        /// <summary>
        /// Assurance Level 1:
        /// No authentication. There is no evidence supporting that the actual user is a claimed user
        /// </summary>
        [SamlAttributeValue("1")]
        Level1 = 1,

        /// <summary>
        /// Assurance Level 2:
        /// The user has supported the claim about the user identity by providing a matching password
        /// </summary>
        [SamlAttributeValue("2")]
        Level2 = 2,

        /// <summary>
        /// Assurance Level 3:
        /// The user has supported the claim about the user identity through digital signature
        /// </summary>
        [SamlAttributeValue("3")]
        Level3 = 3,

        /// <summary>
        /// Assurance Level 4
        /// </summary>
        [SamlAttributeValue("4")]
        Level4 = 4,

        /// <summary>
        /// Test assurance level
        /// </summary>
        [SamlAttributeValue("test")]
        Test = -1,
    }



}