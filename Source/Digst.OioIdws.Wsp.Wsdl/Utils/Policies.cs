namespace Digst.OioIdws.Wsp.Wsdl.Utils
{
    using System.Collections.Generic;
    using WSD = System.Web.Services.Description;

    using Digst.OioIdws.Wsp.Wsdl.Bindings;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Addressing;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Common;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Algorithm;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Initiator.X509;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Layouts;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Asymmetric.Recipient;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Algorithm;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Layouts;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Security.Transport.Tokens;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.SignedSupport;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Trust;
    using Digst.OioIdws.Wsp.Wsdl.Bindings.Policies.Wss;


    // Before performing refactoring in code, please read:
    // 
    // - "Understanding Web Services Policy" > "2. Basic Concepts: Policy Expression"
    //
    // https://msdn.microsoft.com/en-us/library/ms996497.aspx#understwspol_topic2
    public static class Policies
    {
        enum Prefix
        {
            None           = 0b00000000000000000000000000000000,
            Asymmetric     = 0b00000000000000000000000000000001,
            Transport      = 0b00000000000000000000000000000010,
            InitiatorToken = 0b00000000000000000000000000000100,
            RecipientToken = 0b00000000000000000000000000001000
        }

        public static Policy Main(Parse.Element root, string name, TokenType token)
        {
            if (!root.Name.Equals("Policy"))
            {
                return null;
            }

            Policy policy = new Policy
            {
                Id = name
            };

            foreach (var c in root.Children)
            {
                if (AuxMain(c,token) is ExactlyOne exactlyOne)
                {
                    policy.ExactlyOne.Add(exactlyOne);
                }
            }

            return policy;
        }

        private static WSD.NamedItem AuxMain(
            Parse.Element child,
            TokenType token,
            Prefix prefix = Prefix.None)
        {
            var children = new List<Parse.Element>();

            switch (child.Name)
            {
                #region Common

                case "RequestSecurityTokenTemplate":
                    var initiatorTokenRequestSecurityTokenTemplate =
                        new RequestSecurityTokenTemplate();

                    return initiatorTokenRequestSecurityTokenTemplate as WSD.NamedItem;

                #endregion

                #region Addressing

                case "UsingAddressing":
                    var usingAddressing = new UsingAddressing();

                    return usingAddressing as WSD.NamedItem;

                #endregion

                #region Security

                #region Common

                case "AlgorithmSuite":

                    switch (prefix)
                    {
                        case Prefix.Asymmetric:
                            {
                                var asymmetricAlgorithmSuite =
                                    new AsymmetricAlgorithmSuite();
                                var asymmetricAlgorithmSuitePolicy =
                                    new AsymmetricAlgorithmSuitePolicy();

                                // Check if there is a nested Policy, if yes, iterate 
                                // through its children instead
                                foreach (var c in child.Children)
                                {
                                    if (c.Name.Equals("Policy"))
                                    {
                                        children.AddRange(c.Children);
                                        break;
                                    }
                                }
                                // otherwise go through the elements children
                                if (children.Count == 0)
                                {
                                    children.AddRange(child.Children);
                                }

                                foreach (var c in children)
                                {
                                    if (c.Name.Equals("Basic256") &&
                                        AuxMain(c,token,prefix) is AsymmetricBasic256 tb256)
                                    {
                                        asymmetricAlgorithmSuitePolicy.Basic256.Add(tb256);
                                    }
                                }

                                asymmetricAlgorithmSuite.NestedPolicy.Add(
                                    asymmetricAlgorithmSuitePolicy
                                );

                                return asymmetricAlgorithmSuite as WSD.NamedItem;
                            }
                        case Prefix.Transport:
                            {
                                var transportAlgorithmSuite =
                                    new TransportAlgorithmSuite();
                                var transportAlgorithmSuitePolicy =
                                    new TransportAlgorithmSuitePolicy();

                                // Check if there is a nested Policy, if yes, iterate 
                                // through its children instead
                                foreach (var c in child.Children)
                                {
                                    if (c.Name.Equals("Policy"))
                                    {
                                        children.AddRange(c.Children);
                                        break;
                                    }
                                }
                                // otherwise go through the elements children
                                if (children.Count == 0)
                                {
                                    children.AddRange(child.Children);
                                }

                                foreach (var c in children)
                                {
                                    if (c.Name.Equals("Basic256") &&
                                        AuxMain(c,token,prefix) is TransportBasic256 tb256)
                                    {
                                        transportAlgorithmSuitePolicy.Basic256.Add(tb256);
                                    }
                                }

                                transportAlgorithmSuite.NestedPolicy.Add(
                                    transportAlgorithmSuitePolicy
                                );

                                return transportAlgorithmSuite as WSD.NamedItem;
                            }
                        default:
                            return null;
                    }

                case "Basic256":

                    switch (prefix)
                    {
                        case Prefix.Asymmetric:
                            {
                                var asymmetricBasic256 = new AsymmetricBasic256();

                                return asymmetricBasic256 as WSD.NamedItem;
                            }
                        case Prefix.Transport:
                            {
                                var transportBasic256 = new TransportBasic256();

                                return transportBasic256 as WSD.NamedItem;
                            }
                        default:
                            return null;
                    }

                case "Layout":

                    switch (prefix)
                    {
                        case Prefix.Asymmetric:
                            {
                                var asymmetricLayout = new AsymmetricLayout();
                                var asymmetricLayoutPolicy = new AsymmetricLayoutPolicy();

                                // Check if there is a nested Policy, if yes, iterate 
                                // through its children instead
                                foreach (var c in child.Children)
                                {
                                    if (c.Name.Equals("Policy"))
                                    {
                                        children.AddRange(c.Children);
                                        break;
                                    }
                                }
                                // otherwise go through the elements children
                                if (children.Count == 0)
                                {
                                    children.AddRange(child.Children);
                                }

                                foreach (var c in children)
                                {
                                    if (c.Name.Equals("Strict") &&
                                        AuxMain(c,token,prefix) is AsymmetricStrict ts)
                                    {
                                        asymmetricLayoutPolicy.Strict.Add(ts);
                                    }
                                }

                                asymmetricLayout.NestedPolicy.Add(asymmetricLayoutPolicy);

                                return asymmetricLayout as WSD.NamedItem;
                            }
                        case Prefix.Transport:
                            {
                                var transportLayout = new TransportLayout();
                                var transportLayoutPolicy = new TransportLayoutPolicy();

                                // Check if there is a nested Policy, if yes, iterate 
                                // through its children instead
                                foreach (var c in child.Children)
                                {
                                    if (c.Name.Equals("Policy"))
                                    {
                                        children.AddRange(c.Children);
                                        break;
                                    }
                                }
                                // otherwise go through the elements children
                                if (children.Count == 0)
                                {
                                    children.AddRange(child.Children);
                                }

                                foreach (var c in children)
                                {
                                    if (c.Name.Equals("Strict") &&
                                        AuxMain(c,token,prefix) is TransportStrict ts)
                                    {
                                        transportLayoutPolicy.Strict.Add(ts);
                                    }
                                }

                                transportLayout.NestedPolicy.Add(transportLayoutPolicy);

                                return transportLayout as WSD.NamedItem;
                            }
                        default:
                            return null;
                    }

                case "Strict":

                    switch (prefix)
                    {
                        case Prefix.Asymmetric:
                            {
                                var asymmetricStrict = new AsymmetricStrict();

                                return asymmetricStrict as WSD.NamedItem;
                            }
                        case Prefix.Transport:
                            {
                                var transportStrict = new TransportStrict();

                                return transportStrict as WSD.NamedItem;
                            }
                        default:
                            return null;
                    }

                #endregion

                #region Asymmetric

                #region Common

                case "X509Token":

                    switch (prefix)
                    {
                        case Prefix.InitiatorToken:
                            {
                                var initiatorX509Token =
                                    new InitiatorX509Token();
                                var initiatorX509TokenPolicy =
                                    new InitiatorX509TokenPolicy();

                                // Check if there is a nested Policy, if yes,
                                // iterate through its children instead
                                foreach (var c in child.Children)
                                {
                                    if (c.Name.Equals("Policy"))
                                    {
                                        children.AddRange(c.Children);
                                        break;
                                    }
                                }
                                // otherwise go through the elements children
                                if (children.Count == 0)
                                {
                                    children.AddRange(child.Children);
                                }

                                foreach (var c in children)
                                {
                                    if (c.Name.Equals("WssX509V3Token10") &&
                                        AuxMain(c,token,prefix) is InitiatorWssX509V3Token10 wt)
                                    {
                                        initiatorX509TokenPolicy.WssX509V3Token10.Add(wt);
                                    }
                                }

                                initiatorX509Token.NestedPolicy.Add(
                                    initiatorX509TokenPolicy
                                );

                                return initiatorX509Token as WSD.NamedItem;
                            }
                        case Prefix.RecipientToken:
                            {
                                var recipientX509Token =
                                    new RecipientX509Token();
                                var recipientX509TokenPolicy =
                                    new RecipientX509TokenPolicy();

                                // Check if there is a nested Policy, if yes,
                                // iterate through its children instead
                                foreach (var c in child.Children)
                                {
                                    if (c.Name.Equals("Policy"))
                                    {
                                        children.AddRange(c.Children);
                                        break;
                                    }
                                }
                                // otherwise go through the elements children
                                if (children.Count == 0)
                                {
                                    children.AddRange(child.Children);
                                }

                                foreach (var c in children)
                                {
                                    if (c.Name.Equals("WssX509V3Token10") &&
                                        AuxMain(c,token,prefix) is RecipientWssX509V3Token10 wt)
                                    {
                                        recipientX509TokenPolicy.WssX509V3Token10.Add(wt);
                                    }
                                }

                                recipientX509Token.NestedPolicy.Add(
                                    recipientX509TokenPolicy
                                );

                                return recipientX509Token as WSD.NamedItem;
                            }
                        default:
                            return null;
                    }

                case "WssX509V3Token10":

                    switch (prefix)
                    {
                        case Prefix.InitiatorToken:
                            {
                                var initiatorWssX509V3Token10 =
                                    new InitiatorWssX509V3Token10();

                                return initiatorWssX509V3Token10 as WSD.NamedItem;
                            }
                        case Prefix.RecipientToken:
                            {
                                var recipientWssX509V3Token10 =
                                    new RecipientWssX509V3Token10();

                                return recipientWssX509V3Token10 as WSD.NamedItem;
                            }
                        default:
                            return null;
                    }

                #endregion

                case "AsymmetricBinding":
                    var asymmetricBinding = new AsymmetricBinding();
                    var asymmetricBindingPolicy = new AsymmetricBindingPolicy();

                    var asymmetricPrefix = Prefix.Asymmetric;

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("AlgorithmSuite") &&
                            AuxMain(c,token, asymmetricPrefix) is AsymmetricAlgorithmSuite tas)
                        {
                            asymmetricBindingPolicy.AlgorithmSuite.Add(tas);
                        }

                        if (c.Name.Equals("Layout") &&
                            AuxMain(c,token, asymmetricPrefix) is AsymmetricLayout tl)
                        {
                            asymmetricBindingPolicy.Layout.Add(tl);
                        }

                        if (c.Name.Equals("InitiatorToken") &&
                            AuxMain(c,token, asymmetricPrefix) is InitiatorToken it)
                        {
                            asymmetricBindingPolicy.InitiatorToken.Add(it);
                        }

                        if (c.Name.Equals("RecipientToken") &&
                            AuxMain(c,token, asymmetricPrefix) is RecipientToken rt)
                        {
                            asymmetricBindingPolicy.RecipientToken.Add(rt);
                        }

                        if (c.Name.Equals("ProtectTokens") &&
                            AuxMain(c,token, asymmetricPrefix) is ProtectTokens pt)
                        {
                            asymmetricBindingPolicy.ProtectTokens = pt;
                        }

                        if (c.Name.Equals("IncludeTimestamp") &&
                            AuxMain(c,token, asymmetricPrefix) is IncludeTimestamp its)
                        {
                            asymmetricBindingPolicy.IncludeTimestamp = its;
                        }

                        if (c.Name.Equals("OnlySignEntireHeadersAndBody") &&
                            AuxMain(c,token, asymmetricPrefix) is OnlySignEntireHeadersAndBody osehb)
                        {
                            asymmetricBindingPolicy.OnlySignEntireHeadersAndBody = osehb;
                        }
                    }

                    asymmetricBinding.NestedPolicy.Add(asymmetricBindingPolicy);

                    return asymmetricBinding as WSD.NamedItem;


                case "ProtectTokens":
                    var protectTokens = new ProtectTokens();

                    return protectTokens as WSD.NamedItem;

                case "IncludeTimestamp":
                    var includeTimestamp = new IncludeTimestamp();

                    return includeTimestamp as WSD.NamedItem;

                case "OnlySignEntireHeadersAndBody":
                    var onlySignEntireHeadersAndBody =
                        new OnlySignEntireHeadersAndBody();

                    return onlySignEntireHeadersAndBody as WSD.NamedItem;

                case "InitiatorToken":
                    var initiatorToken = new InitiatorToken();
                    var initiatorTokenPolicy = new InitiatorTokenPolicy();

                    var initiatorPrefix = Prefix.InitiatorToken;

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("X509Token") &&
                            AuxMain(c,token, initiatorPrefix) is InitiatorX509Token ixt)
                        {
                            initiatorTokenPolicy.X509Token.Add(ixt);
                        }
                    }

                    initiatorToken.NestedPolicy.Add(initiatorTokenPolicy);

                    return initiatorToken as WSD.NamedItem;

                case "RecipientToken":
                    var recipientToken = new RecipientToken();
                    var recipientTokenPolicy = new RecipientTokenPolicy();

                    var recipientPrefix = Prefix.RecipientToken;

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("X509Token") &&
                            AuxMain(c,token, recipientPrefix) is RecipientX509Token ixt)
                        {
                            recipientTokenPolicy.X509Token.Add(ixt);
                        }
                    }

                    recipientToken.NestedPolicy.Add(recipientTokenPolicy);

                    return recipientToken as WSD.NamedItem;

                #endregion

                #region Transport

                case "TransportBinding":
                    var transportBinding = new TransportBinding();
                    var transportBindingPolicy = new TransportBindingPolicy();

                    var transportPrefix = Prefix.Transport;

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("AlgorithmSuite") &&
                            AuxMain(c,token, transportPrefix) is TransportAlgorithmSuite tas)
                        {
                            transportBindingPolicy.AlgorithmSuite.Add(tas);
                        }

                        if (c.Name.Equals("Layout") &&
                            AuxMain(c,token, transportPrefix) is TransportLayout tl)
                        {
                            transportBindingPolicy.Layout.Add(tl);
                        }

                        if (c.Name.Equals("TransportToken") &&
                            AuxMain(c,token, transportPrefix) is TransportToken tt)
                        {
                            transportBindingPolicy.TransportToken.Add(tt);
                        }
                    }

                    transportBinding.NestedPolicy.Add(transportBindingPolicy);

                    return transportBinding as WSD.NamedItem;

                case "TransportToken":
                    var transportToken = new TransportToken();
                    var transportTokenPolicy = new TransportTokenPolicy();

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("HttpsToken") &&
                            AuxMain(c,token,prefix) is HttpsToken ht)
                        {
                            transportTokenPolicy.HttpsToken.Add(ht);
                        }
                    }

                    transportToken.NestedPolicy.Add(transportTokenPolicy);

                    return transportToken as WSD.NamedItem;

                case "HttpsToken":
                    var httpsToken = new HttpsToken();

                    return httpsToken as WSD.NamedItem;

                #endregion

                #endregion

                #region Trust

                case "Trust10":
                    var trust10 = new Trust13();
                    var trust10Policy = new Trust13Policy();

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("MustSupportIssuedTokens") &&
                            AuxMain(c,token,prefix) is MustSupportIssuedTokens msit)
                        {
                            trust10Policy.MustSupportIssuedTokens = msit;
                        }

                        if (c.Name.Equals("RequireClientEntropy") &&
                            AuxMain(c,token,prefix) is RequireClientEntropy rce)
                        {
                            trust10Policy.RequireClientEntropy = rce;
                        }

                        if (c.Name.Equals("RequireServerEntropy") &&
                            AuxMain(c,token,prefix) is RequireServerEntropy rse)
                        {
                            trust10Policy.RequireServerEntropy = rse;
                        }
                    }

                    trust10.NestedPolicy.Add(trust10Policy);

                    return trust10 as WSD.NamedItem;

                case "MustSupportIssuedTokens":
                    var mustSupportIssuedTokens = new MustSupportIssuedTokens();

                    return mustSupportIssuedTokens as WSD.NamedItem;

                case "RequireClientEntropy":
                    var requireClientEntropy = new RequireClientEntropy();

                    return requireClientEntropy as WSD.NamedItem;

                case "RequireServerEntropy":
                    var requireServerEntropy = new RequireServerEntropy();

                    return requireServerEntropy as WSD.NamedItem;

                #endregion

                #region Wss

                case "Wss11":
                    var wss11 = new Wss11();
                    var wss11Policy = new Wss11Policy();

                    // Check if there is a nested Policy, if yes, iterate 
                    // through its children instead
                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("Policy"))
                        {
                            children.AddRange(c.Children);
                            break;
                        }
                    }
                    // otherwise go through the elements children
                    if (children.Count == 0)
                    {
                        children.AddRange(child.Children);
                    }

                    foreach (var c in children)
                    {
                        if (c.Name.Equals("MustSupportRefIssuerSerial") &&
                            AuxMain(c,token,prefix) is MustSupportRefIssuerSerial msris)
                        {
                            wss11Policy.MustSupportRefIssuerSerial = msris;
                        }

                        if (c.Name.Equals("MustSupportRefKeyIdentifier") &&
                            AuxMain(c,token,prefix) is MustSupportRefKeyIdentifier msrki)
                        {
                            wss11Policy.MustSupportRefKeyIdentifier = msrki;
                        }

                        if (c.Name.Equals("MustSupportRefThumbprint") &&
                            AuxMain(c,token,prefix) is MustSupportRefThumbprint msrt)
                        {
                            wss11Policy.MustSupportRefThumbprint = msrt;
                        }
                    }

                    wss11.NestedPolicy.Add(wss11Policy);

                    return wss11 as WSD.NamedItem;

                case "MustSupportRefIssuerSerial":
                    var mustSupportRefIssuerSerial = new MustSupportRefIssuerSerial();

                    return mustSupportRefIssuerSerial as WSD.NamedItem;

                case "MustSupportRefKeyIdentifier":
                    var mustSupportRefKeyIdentifier = new MustSupportRefKeyIdentifier();

                    return mustSupportRefKeyIdentifier as WSD.NamedItem;

                case "MustSupportRefThumbprint":
                    var mustSupportRefThumbprint = new MustSupportRefThumbprint();

                    return mustSupportRefThumbprint as WSD.NamedItem;

                #endregion

                case "ExactlyOne":
                    var exactlyOne = new ExactlyOne();

                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("All") &&
                            AuxMain(c,token,prefix) is All all_)
                        {
                            exactlyOne.All.Add(all_);
                        }
                    }

                    return exactlyOne as WSD.NamedItem;

                case "All":
                    var all = new All();
                    var allpolicy = new SecurityPolicy();

                    foreach (var c in child.Children)
                    {
                        if (c.Name.Equals("UsingAddressing") &&
                            AuxMain(c,token,prefix) is UsingAddressing usingAddressing_)
                        {
                            all.UsingAddressing = usingAddressing_;
                        }

                        if (c.Name.Equals("Trust10") &&
                            AuxMain(c,token,prefix) is Trust13 t10_)
                        {
                            all.Trust13 = t10_;
                        }
                        if (c.Name.Equals("Wss11") &&
                            AuxMain(c,token,prefix) is Wss11 wss11_)
                        {
                            all.Wss11 = wss11_;
                        }


                        if (c.Name.Equals("TransportBinding") &&
                            AuxMain(c,token,prefix) is TransportBinding transportBinding_)
                        {
                            allpolicy.TransportBinding.Add(transportBinding_);
                        }
                        if (c.Name.Equals("AsymmetricBinding") &&
                            AuxMain(c,token,prefix) is AsymmetricBinding asymmetricBinding_)
                        {
                            allpolicy.AsymmetricBinding.Add(asymmetricBinding_);
                        }
                    }

                    all.NestedPolicy.Add(allpolicy);

                    foreach (SecurityPolicy np in all.NestedPolicy)
                    {
                        // If no Secure Tranport Binding is present in children, 
                        // add default
                        if (np.TransportBinding.Count == 0)
                        {
                            np.TransportBinding.Add(AuxTransportBinding());
                        }

                        // Clear parsed AsymmetricBinding (if any) and add 
                        // custom version that complies with [OIO IDWS SOAP]
                        np.AsymmetricBinding.Clear();
                        np.AsymmetricBinding.Add(AuxAsymmetricBinding(token));
                    }

                    // If no UsingAddressing is present in children, add default
                    if (all.UsingAddressing == null)
                    {
                        all.UsingAddressing = new UsingAddressing();
                    }

                    // Clear parsed SignedSupportingTokens (if any) and add 
                    // custom version that complies with [OIO IDWS SOAP]
                    all.SignedSupportingTokens = AuxSignedSupportingTokens(token);

                    return all as WSD.NamedItem;

                default:
                    return null;
            }
        }

        private static TransportBinding AuxTransportBinding()
        {
            var transportBinding = new TransportBinding();
            var transportBindingPolicy = new TransportBindingPolicy();

            var transportToken = new TransportToken();
            var algorithmSuite = new TransportAlgorithmSuite();
            var layout = new TransportLayout();

            {
                var transportTokenPolicy = new TransportTokenPolicy();
                var httpsToken = new HttpsToken();

                transportTokenPolicy.HttpsToken.Add(httpsToken);
                transportToken.NestedPolicy.Add(transportTokenPolicy);
            }

            {
                var algorithmSuitePolicy = new TransportAlgorithmSuitePolicy();
                var basic256 = new TransportBasic256();

                algorithmSuitePolicy.Basic256.Add(basic256);

                algorithmSuite.NestedPolicy.Add(algorithmSuitePolicy);
            }

            {
                var layoutPolicy = new TransportLayoutPolicy();
                var strict = new TransportStrict();

                layoutPolicy.Strict.Add(strict);

                layout.NestedPolicy.Add(layoutPolicy);
            }

            {
                transportBindingPolicy.TransportToken.Add(transportToken);
                transportBindingPolicy.AlgorithmSuite.Add(algorithmSuite);
                transportBindingPolicy.Layout.Add(layout);
            }

            transportBinding.NestedPolicy.Add(transportBindingPolicy);

            return transportBinding;
        }

        private static AsymmetricBinding AuxAsymmetricBinding(TokenType token)
        {
            var asymmetric = new AsymmetricBinding();
            var asymmetricPolicy = new AsymmetricBindingPolicy();

            var initiatorToken = new InitiatorToken();
            var recipientToken = new RecipientToken();
            var algorithmSuite = new AsymmetricAlgorithmSuite();
            var layout = new AsymmetricLayout();

            var protectTokens = new ProtectTokens();
            var includeTimestamp = new IncludeTimestamp();
            var onlySignEntireHeadersAndBody = new OnlySignEntireHeadersAndBody();

            {
                var initiatorTokenPolicy = new InitiatorTokenPolicy();

                switch (token)
                {
                    case TokenType.HolderOfKey:
                        var issuedToken = new InitiatorIssuedToken();

                        {
                            var requestSecurityTokenTemplate =
                                new RequestSecurityTokenTemplate
                                {
                                    KeyType = new RequestSecurityTokenTemplateKeyType(),
                                    TokenType = new RequestSecurityTokenTemplateTokenType()
                                };

                            issuedToken.RequestSecurityTokenTemplate = requestSecurityTokenTemplate;
                        }

                        initiatorTokenPolicy.IssuedToken = issuedToken;

                        break;
                    case TokenType.Bearer:
                        var initiatorX509Token = new InitiatorX509Token();
                        var initiatorX509TokenPolicy = new InitiatorX509TokenPolicy();
                        var initiatorWssX509V3Token10 = new InitiatorWssX509V3Token10();

                        initiatorX509TokenPolicy.WssX509V3Token10.Add(initiatorWssX509V3Token10);
                        initiatorX509Token.NestedPolicy.Add(initiatorX509TokenPolicy);
                        initiatorTokenPolicy.X509Token.Add(initiatorX509Token);

                        break;
                    default:
                        break;
                }

                initiatorToken.NestedPolicy.Add(initiatorTokenPolicy);
            }

            {
                var recipientTokenPolicy = new RecipientTokenPolicy();
                var recipientX509Token = new RecipientX509Token();
                var recipientX509TokenPolicy = new RecipientX509TokenPolicy();
                var recipientWssX509V3Token10 = new RecipientWssX509V3Token10();

                recipientX509TokenPolicy.WssX509V3Token10.Add(recipientWssX509V3Token10);
                recipientX509Token.NestedPolicy.Add(recipientX509TokenPolicy);
                recipientTokenPolicy.X509Token.Add(recipientX509Token);
                recipientToken.NestedPolicy.Add(recipientTokenPolicy);
            }

            {
                var algorithmSuitePolicy = new AsymmetricAlgorithmSuitePolicy();
                var basic256 = new AsymmetricBasic256();

                algorithmSuitePolicy.Basic256.Add(basic256);

                algorithmSuite.NestedPolicy.Add(algorithmSuitePolicy);
            }

            {
                var layoutPolicy = new AsymmetricLayoutPolicy();
                var strict = new AsymmetricStrict();

                layoutPolicy.Strict.Add(strict);

                layout.NestedPolicy.Add(layoutPolicy);
            }

            {
                asymmetricPolicy.InitiatorToken.Add(initiatorToken);
                asymmetricPolicy.RecipientToken.Add(recipientToken);
                asymmetricPolicy.AlgorithmSuite.Add(algorithmSuite);
                asymmetricPolicy.Layout.Add(layout);

                asymmetricPolicy.ProtectTokens = protectTokens;
                asymmetricPolicy.IncludeTimestamp = includeTimestamp;
                asymmetricPolicy.OnlySignEntireHeadersAndBody =
                    onlySignEntireHeadersAndBody;
            }

            asymmetric.NestedPolicy.Add(asymmetricPolicy);

            return asymmetric;
        }

        private static SignedSupportingTokens AuxSignedSupportingTokens(
            TokenType token)
        {
            var signedSupportingTokens = new SignedSupportingTokens();
            var signedSupportingTokensPolicy = new SignedSupportingTokensPolicy();
            var issuedToken = new SignedSupportIssuedToken();
            var requestSecurityTokenTemplate = new RequestSecurityTokenTemplate();

            {
                switch (token)
                {
                    case TokenType.HolderOfKey:
                        requestSecurityTokenTemplate.KeyType =
                            new RequestSecurityTokenTemplateKeyType();
                        requestSecurityTokenTemplate.TokenType =
                            new RequestSecurityTokenTemplateTokenType();
                        break;
                    case TokenType.Bearer:
                    default:
                        break;
                }

                issuedToken.RequestSecurityTokenTemplate = requestSecurityTokenTemplate;
            }

            {
                signedSupportingTokensPolicy.IssuedToken.Add(issuedToken);
            }

            signedSupportingTokens.NestedPolicy.Add(signedSupportingTokensPolicy);

            return signedSupportingTokens;
        }
    }
}