﻿using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Digst.OioIdws.Common.Logging;
using Digst.OioIdws.OioWsTrust.ProtocolChannel;

namespace Digst.OioIdws.OioWsTrust
{
    /// <summary>
    /// <see cref="IStsTokenService"/>
    /// </summary>
    public class StsTokenService : StsTokenServiceBase
    {
        private readonly StsTokenServiceConfiguration _config;

        public StsTokenService(StsTokenServiceConfiguration config)
        {
            // Check input arguments
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrEmpty(config.WspEndpointId)) throw new ArgumentException("WspEndpointId");
            if (string.IsNullOrEmpty(config.StsEndpointAddress)) throw new ArgumentException("StsEndpointAddress");
            // X509FindType cannot be tested below because default value is FindByThumbprint
            if (config.ClientCertificate == null) throw new ArgumentException("ClientCertificate");
            if (config.StsCertificate == null) throw new ArgumentException("StsCertificate");

            // New object is created to ensure that configuration does not change. The alternative would be immutable objects.
            _config = new StsTokenServiceConfiguration
            {
                ClientCertificate = config.ClientCertificate,
                StsCertificate = config.StsCertificate,
                SendTimeout = config.SendTimeout,
                StsEndpointAddress = config.StsEndpointAddress,
                TokenLifeTimeInMinutes = config.TokenLifeTimeInMinutes,
                WspEndpointId = config.WspEndpointId
            };
        }


        protected internal override SecurityToken GetToken(StsAuthenticationCase stsAuthenticationCase, SecurityToken authenticationToken)
        {
            Logger.Instance.Trace(
                $@"RequestToken called with the client certificate: {_config.ClientCertificate.SubjectName.Name} ({
                        _config.ClientCertificate.Thumbprint
                    })");
            Logger.Instance.Trace(
                $@"RequestToken called with the STS certificate: {_config.StsCertificate.SubjectName.Name} ({
                        _config.StsCertificate.Thumbprint
                    })");

            try
            {
                // Create custom binding
                var stsBinding = new CustomBinding();
                if (_config.SendTimeout.HasValue)
                {
                    Logger.Instance.Warning($"RequestToken send timeout set to {_config.SendTimeout.Value}");
                    stsBinding.SendTimeout = _config.SendTimeout.Value;
                }
                stsBinding.Elements.Add(new OioWsTrustBindingElement(_config, stsAuthenticationCase));
                stsBinding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10,
                    Encoding.UTF8));
                // ManualAddressing must be true in order to make sure that wsa header elements are not altered in the HttpsTransportChannel which happens after xml elements have been digitally signed.
                stsBinding.Elements.Add(new HttpsTransportBindingElement() {ManualAddressing = true});

                // Setup channel factory and apply client credentials
                var factory = new WSTrustChannelFactory(stsBinding, new EndpointAddress(_config.StsEndpointAddress));
                factory.TrustVersion = TrustVersion.WSTrust13;
                factory.Credentials.ClientCertificate.Certificate = _config.ClientCertificate;

                // Create token request
                // UseKey and KeyType are only set in order for WCF to know which proof token to use when signing the request sent from WSC to WSP. Hence, UseKey and KeyType are actually sent to the NemLogin-STS but not used for anything by the STS.
                var requestSecurityToken = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = new EndpointReference(_config.WspEndpointId),
                    // TokenType is optional according to [NEMLOGIN-STSRULES]. If specified it must contain the value http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0 which is the only type NemLogin STS supports.
                    // We specify it in case that NemLogin STS supports other token types in the future.
                    // Currently if TokenType is not specified ... then TokenType is also not specified in the RequestSecurityTokenResponse (RSTR). According to spec it should always be specified in the RSTR. Specifying TokenType in the RST triggers the TokenType to be specified in the RSTR.
                    TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",
                    UseKey =
                        new UseKey(new X509SecurityToken(_config
                            .ClientCertificate)), // The UseKey must be set in order for WCF to know which certificate must be used for signing the request from WSC to WSP. Thus, the usekey is actually the same as the proof key in the holder-of-key scenario.
                    KeyType =
                        KeyTypes
                            .Asymmetric // The KeyType must be set to Asymmetric in order for WCF to know that it must use the UseKey as proof token.
                };
                // Lifetime is only specified if it has been configured. Should result in a default life time (8 hours) on issued token if not specified. If specified, STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
                var currentTimeUtc = DateTime.UtcNow;
                if (_config.TokenLifeTimeInMinutes.HasValue)
                {
                    requestSecurityToken.Lifetime = new Lifetime(null,
                        currentTimeUtc.AddMinutes(_config.TokenLifeTimeInMinutes.Value));
                }

                // ActAs is only set if a bootstrap token or local token is supplied.
                switch (stsAuthenticationCase)
                {
                    case StsAuthenticationCase.SignatureCase:
                        break;
                    case StsAuthenticationCase.BootstrapTokenCase:
                    case StsAuthenticationCase.LocalTokenCase:
                        // First check validity before using it.
                        if (authenticationToken.ValidTo < currentTimeUtc)
                            throw new ArgumentException(
                                $"Bootstrap token life time has expired. Please renew before trying again. Bootstrap token ID: {authenticationToken.Id}, Valid to: {authenticationToken.ValidTo}, Current time: {currentTimeUtc}");
                        requestSecurityToken.ActAs = new SecurityTokenElement(authenticationToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(stsAuthenticationCase), stsAuthenticationCase, null);
                }

                // Request token and return
                var wsTrustChannelContract = factory.CreateChannel();
                var securityToken = wsTrustChannelContract.Issue(requestSecurityToken);

                // Tried making this method async with the following statement ... but an error "Manual addressing is enabled on this factory, so all messages sent must be pre-addressed." occurs when the async pattern is used
                // var securityToken = await Task.Factory.FromAsync((callback, stateObject) => wsTrustChannelContract.BeginIssue(requestSecurityToken, callback, stateObject), result => wsTrustChannelContract.EndIssue(result, out RequestSecurityTokenResponse requestSecurityTokenResponse), null);
                // also tried the following approach which gives the same result:
                //SecurityToken securityToken = null;
                //wsTrustChannelContract.BeginIssue(requestSecurityToken, callback =>
                //{
                //    securityToken = wsTrustChannelContract.EndIssue(callback, out RequestSecurityTokenResponse requestSecurityTokenResponse);
                //    Console.WriteLine("sdfd");
                //}, null);

                return securityToken;
            }
            // Log all errors and rethrow
            catch (Exception e)
            {
                Logger.Instance.Error("Error occured while requesting token. See exception details!", e);
                throw;
            }
        }
    }
}