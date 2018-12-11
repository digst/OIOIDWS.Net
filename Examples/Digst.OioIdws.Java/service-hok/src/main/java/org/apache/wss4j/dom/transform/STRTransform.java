// NOTE!!!
//
// This is a copy of STRTransform.java found in wss4j-ws-security-dom version 2.0.10,
// which matches CXF version 3.0.16.
//
// There is a single modification to this class (compared to the original version),
// found in the transformIt() method below. It has been clearly marked as
// a workaround - and the code can likely be adapted to other versions of CXF/WSS4J
//
// The change is required when using the NemLog-in STS, as it returns an EncryptedAssertion
// element with an embedded EncryptedData element. On the WSC side, the STR-Transform
// canonization will inherit the parent namespace (from the EncryptedAssertion element),
// but this does not exist on the WSP side, so the two sides will not generate the same
// digest (resulting in a failed signature validation), unless this workaround is applied.

/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

package org.apache.wss4j.dom.transform;

import org.apache.wss4j.dom.WSConstants;
import org.apache.wss4j.dom.WSDocInfo;
import org.apache.wss4j.dom.bsp.BSPEnforcer;
import org.apache.wss4j.dom.message.token.PKIPathSecurity;
import org.apache.wss4j.dom.message.token.SecurityTokenReference;
import org.apache.wss4j.dom.message.token.X509Security;
import org.apache.wss4j.dom.util.WSSecurityUtil;

import org.apache.xml.security.c14n.Canonicalizer;
import org.apache.xml.security.signature.XMLSignatureInput;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import java.io.ByteArrayInputStream;
import java.io.OutputStream;
import java.security.InvalidAlgorithmParameterException;
import java.security.spec.AlgorithmParameterSpec;
import java.util.Iterator;

import javax.xml.crypto.Data;
import javax.xml.crypto.MarshalException;
import javax.xml.crypto.NodeSetData;
import javax.xml.crypto.OctetStreamData;
import javax.xml.crypto.XMLCryptoContext;
import javax.xml.crypto.XMLStructure;
import javax.xml.crypto.dom.DOMCryptoContext;
import javax.xml.crypto.dsig.TransformException;
import javax.xml.crypto.dsig.TransformService;
import javax.xml.crypto.dsig.spec.TransformParameterSpec;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;


/**
 * Class STRTransform.
 */
public class STRTransform extends TransformService {

    public static final String TRANSFORM_URI = 
        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform";
    
    public static final String TRANSFORM_WS_DOC_INFO = "transform_ws_doc_info";

    private TransformParameterSpec params;
    
    private Element transformElement;
    
    private static final org.slf4j.Logger LOG = 
        org.slf4j.LoggerFactory.getLogger(STRTransform.class);

    public final AlgorithmParameterSpec getParameterSpec() {
        return params;
    }
    
    public void init(TransformParameterSpec params)
        throws InvalidAlgorithmParameterException {
        this.params = params;
    }
    
    public void init(XMLStructure parent, XMLCryptoContext context)
    throws InvalidAlgorithmParameterException {
        if (context != null && !(context instanceof DOMCryptoContext)) {
            throw new ClassCastException
                ("context must be of type DOMCryptoContext");
        }
        if (!(parent instanceof javax.xml.crypto.dom.DOMStructure)) {
            throw new ClassCastException("parent must be of type DOMStructure");
        }
        transformElement = (Element) 
            ((javax.xml.crypto.dom.DOMStructure) parent).getNode();
    }

    public void marshalParams(XMLStructure parent, XMLCryptoContext context)
    throws MarshalException {
        if (context != null && !(context instanceof DOMCryptoContext)) {
            throw new ClassCastException
                ("context must be of type DOMCryptoContext");
        }
        if (!(parent instanceof javax.xml.crypto.dom.DOMStructure)) {
            throw new ClassCastException("parent must be of type DOMStructure");
        }
        Element transformElement2 = (Element) 
            ((javax.xml.crypto.dom.DOMStructure) parent).getNode();
        appendChild(transformElement2, transformElement);
        transformElement = transformElement2;
    }

    
    public Data transform(Data data, XMLCryptoContext xc) 
        throws TransformException {
        if (data == null) {
            throw new NullPointerException("data must not be null");
        }
        return transformIt(data, xc, null);
    }

    public Data transform(Data data, XMLCryptoContext xc, OutputStream os) 
        throws TransformException {
        if (data == null) {
            throw new NullPointerException("data must not be null");
        }
        if (os == null) {
            throw new NullPointerException("output stream must not be null");
        }
        return transformIt(data, xc, os);
    }
    
    
    private Data transformIt(Data data, XMLCryptoContext xc, OutputStream os) 
        throws TransformException {

        //
        // First step: Get the required c14n argument and get the specified
        // Canonicalizer
        //
        String canonAlgo = null;
        Element transformParams = WSSecurityUtil.getDirectChildElement(
            transformElement, "TransformationParameters", WSConstants.WSSE_NS
        );
        if (transformParams != null) {
            Element canonElem = 
                WSSecurityUtil.getDirectChildElement(
                    transformParams, "CanonicalizationMethod", WSConstants.SIG_NS
                );
            canonAlgo = canonElem.getAttributeNS(null, "Algorithm");
        }
        try {
            //
            // Get the input (node) to transform. 
            //
            Element str = null;
            if (data instanceof NodeSetData) {
                NodeSetData nodeSetData = (NodeSetData)data;
                Iterator<?> iterator = nodeSetData.iterator();
                while (iterator.hasNext()) {
                    Node node = (Node)iterator.next();
                    if (node instanceof Element && "SecurityTokenReference".equals(node.getLocalName())) {
                        str = (Element)node;
                        break;
                    }
                }
            } else {
                try {
                    XMLSignatureInput xmlSignatureInput = 
                        new XMLSignatureInput(((OctetStreamData)data).getOctetStream());
                    str = (Element)xmlSignatureInput.getSubNode();
                } catch (Exception ex) {
                    throw new TransformException(ex);
                }
            }
            if (str == null) {
                throw new TransformException("No SecurityTokenReference found");
            }
            //
            // The element to transform MUST be a SecurityTokenReference
            // element.
            //
            SecurityTokenReference secRef = new SecurityTokenReference(str, new BSPEnforcer());
            
            Canonicalizer canon = Canonicalizer.getInstance(canonAlgo);

            byte[] buf = null;
            
            //
            // Third and fourth step are performed by dereferenceSTR()
            //
            Object wsDocInfoObject = xc.getProperty(TRANSFORM_WS_DOC_INFO);
            WSDocInfo wsDocInfo = null;
            if (wsDocInfoObject instanceof WSDocInfo) {
                wsDocInfo = (WSDocInfo)wsDocInfoObject;
            }
            if (wsDocInfo == null) {
                LOG.debug("STRTransform: no WSDocInfo found");
            }

            Document doc = str.getOwnerDocument();
            Element dereferencedToken = 
                STRTransformUtil.dereferenceSTR(doc, secRef, wsDocInfo);
            
            if (dereferencedToken != null) {
                String type = dereferencedToken.getAttributeNS(null, "ValueType");
                if (X509Security.X509_V3_TYPE.equals(type) 
                    || PKIPathSecurity.getType().equals(type)) {
                    //
                    // Add the WSSE/WSU namespaces to the element for C14n
                    //
                    WSSecurityUtil.setNamespace(
                        dereferencedToken, WSConstants.WSSE_NS, WSConstants.WSSE_PREFIX
                    );
                    WSSecurityUtil.setNamespace(
                        dereferencedToken, WSConstants.WSU_NS, WSConstants.WSU_PREFIX
                    );
                }
            }
            
			// BEGIN DIGST WORKAROUND
			boolean parentNodeIsNull = (dereferencedToken.getParentNode() == null);
			boolean encryptedAssertionId = (dereferencedToken.getAttribute("wsu:Id") != null && dereferencedToken.getAttribute("wsu:Id").equals("encryptedassertion"));

			if (parentNodeIsNull && encryptedAssertionId) {
				DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
				DocumentBuilder documentBuilder = factory.newDocumentBuilder();
				Document document = documentBuilder.newDocument();
				Element encryptedAssertion = document.createElement("EncryptedAssertion");
				encryptedAssertion.setAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns", "urn:oasis:names:tc:SAML:2.0:assertion");
				dereferencedToken = (Element) document.importNode(dereferencedToken, true);
				encryptedAssertion.appendChild(dereferencedToken);

				dereferencedToken = encryptedAssertion;

				NodeList securityTokenReferences = dereferencedToken.getElementsByTagName("o:SecurityTokenReference");
				if ((securityTokenReferences.getLength() > 0) && ((securityTokenReferences.item(0) instanceof Element))) {
					Element securityTokenReference = (Element) securityTokenReferences.item(0);
					securityTokenReference.setAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns:o", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
				}
			}
			// END DIGST WORKAROUND

            //
            // C14n with specified algorithm. According to WSS Specification.
            //
            buf = canon.canonicalizeSubtree(dereferencedToken, "#default", true);
            if (LOG.isDebugEnabled()) {
                LOG.debug("after c14n: " + new String(buf, "UTF-8"));
            }

            if (os != null) {
                os.write(buf);
                return null;
            }
            return new OctetStreamData(new ByteArrayInputStream(buf));
        } catch (Exception ex) {
            throw new TransformException(ex);
        }
    }
    
    
    public final boolean isFeatureSupported(String feature) {
        if (feature == null) {
            throw new NullPointerException();
        } else {
            return false;
        }
    }
    
    private static void appendChild(Node parent, Node child) {
        Document ownerDoc = null;
        if (parent.getNodeType() == Node.DOCUMENT_NODE) {
            ownerDoc = (Document)parent;
        } else {
            ownerDoc = parent.getOwnerDocument();
        }
        if (child.getOwnerDocument() != ownerDoc) {
            parent.appendChild(ownerDoc.importNode(child, true));
        } else {
            parent.appendChild(child);
        }
    }

}
