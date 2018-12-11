// NOTE!!!
//
// This is a copy of WSDocInfo.java found in wss4j-ws-security-dom version 2.0.10,
// which matches CXF version 3.0.16.
//
// There is a single modification to this class (compared to the original version),
// found in the getResult() method below. It has been clearly marked as
// a workaround - and the code can likely be adapted to other versions of CXF/WSS4J
//
// The change is required when using the NemLog-in STS, as it does not return the ID
// of the actual decrypted assertion, but rather the ID of the embedded EncryptedData
// element, which does not exist on the WSP side after decryption.

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

package org.apache.wss4j.dom;

/**
 * WSDocInfo holds information about the document to process. It provides a 
 * method to store and access document information about BinarySecurityToken, 
 * used Crypto, and others.
 * 
 * Using the Document's hash a caller can identify a document and get
 * the stored information that me be necessary to process the document.
 * The main usage for this is (are) the transformation functions that
 * are called during Signature/Verification process. 
 */

import java.util.ArrayList;
import java.util.List;

import javax.xml.crypto.dom.DOMCryptoContext;

import org.apache.wss4j.common.crypto.Crypto;
import org.apache.wss4j.common.ext.WSSecurityException;
import org.apache.wss4j.dom.message.CallbackLookup;
import org.apache.wss4j.dom.util.WSSecurityUtil;
import org.w3c.dom.Document;
import org.w3c.dom.Element;

public class WSDocInfo {
    private Document doc;
    private Crypto crypto;
    private List<Element> tokenList;
    private List<WSSecurityEngineResult> resultsList;
    private CallbackLookup callbackLookup;
    private Element securityHeader;

    public WSDocInfo(Document doc) {
        //
        // This is a bit of a hack. When the Document is a SAAJ SOAPPart instance, it may
        // be that the "owner" document of any child elements is an internal Document, rather
        // than the SOAPPart. This is the case for the SUN SAAJ implementation.
        //
        if (doc != null && doc.getDocumentElement() != null) {
            this.doc = doc.getDocumentElement().getOwnerDocument();
        } else {
            this.doc = doc;
        }
    }
    
    /**
     * Clears the data stored in this object
     */
    public void clear() {
        crypto = null;
        if (tokenList != null && tokenList.size() > 0) {
            tokenList.clear();
        }
        if (resultsList != null && resultsList.size() > 0) {
            resultsList.clear();
        }
        
        tokenList = null;
        resultsList = null;
    }
    
    /**
     * Store a token element for later retrieval. Before storing the token, we check for a 
     * previously processed token with the same (wsu/SAML) Id.
     * @param element is the token element to store
     */
    public void addTokenElement(Element element) throws WSSecurityException {
        addTokenElement(element, true);
    }
    
    /**
     * Store a token element for later retrieval. Before storing the token, we check for a 
     * previously processed token with the same (wsu/SAML) Id.
     * @param element is the token element to store
     * @param checkMultipleElements check for a previously stored element with the same Id.
     */
    public void addTokenElement(Element element, boolean checkMultipleElements) throws WSSecurityException {
        if (tokenList == null) {
            tokenList = new ArrayList<Element>();
        }
        
        if (checkMultipleElements) {
            for (Element elem : tokenList) {
                if (compareElementsById(element, elem)) {
                    throw new WSSecurityException(
                        WSSecurityException.ErrorCode.INVALID_SECURITY_TOKEN, "duplicateError"
                    );
                }
            }
        }
        tokenList.add(element);
    }
    
    private boolean compareElementsById(Element firstElement, Element secondElement) {
        if (firstElement.hasAttributeNS(WSConstants.WSU_NS, "Id")
            && secondElement.hasAttributeNS(WSConstants.WSU_NS, "Id")) {
            String id = firstElement.getAttributeNS(WSConstants.WSU_NS, "Id");
            String id2 = secondElement.getAttributeNS(WSConstants.WSU_NS, "Id");
            if (id.equals(id2)) {
                return true;
            }
        }
        if (firstElement.hasAttributeNS(null, "AssertionID")
            && secondElement.hasAttributeNS(null, "AssertionID")) {
            String id = firstElement.getAttributeNS(null, "AssertionID");
            String id2 = secondElement.getAttributeNS(null, "AssertionID");
            if (id.equals(id2)) {
                return true;
            }
        }
        if (firstElement.hasAttributeNS(null, "ID") && secondElement.hasAttributeNS(null, "ID")) {
            String id = firstElement.getAttributeNS(null, "ID");
            String id2 = secondElement.getAttributeNS(null, "ID");
            if (id.equals(id2)) {
                return true;
            }
        }
        return false;
    }
    
    /**
     * Get a token Element for the given Id. The Id can be either a wsu:Id or a 
     * SAML AssertionID/ID. 
     * @param uri is the (relative) uri of the id
     * @return the token element or null if nothing found
     */
    public Element getTokenElement(String uri) {
        String id = uri;
        if (id == null) {
            return null;
        } else if (id.charAt(0) == '#') {
            id = id.substring(1);
        }
        if (tokenList != null) {
            for (Element elem : tokenList) {
                String cId = elem.getAttributeNS(WSConstants.WSU_NS, "Id");
                String samlId = elem.getAttributeNS(null, "AssertionID");
                String samlId2 = elem.getAttributeNS(null, "ID");
                if (elem.hasAttributeNS(WSConstants.WSU_NS, "Id") && id.equals(cId)
                    || elem.hasAttributeNS(null, "AssertionID") && id.equals(samlId)
                    || elem.hasAttributeNS(null, "ID") && id.equals(samlId2)) {
                    return elem;
                }
            }
        }
        return null;
    }

    /**
     * Set all stored tokens on the DOMCryptoContext argument
     * @param context
     */
    public void setTokensOnContext(DOMCryptoContext context) {
        if (tokenList != null) {
            for (Element elem : tokenList) {
                WSSecurityUtil.storeElementInContext(context, elem);
            }
        }
    }
    
    /**
     * Store a WSSecurityEngineResult for later retrieval. 
     * @param result is the WSSecurityEngineResult to store
     */
    public void addResult(WSSecurityEngineResult result) {
        if (resultsList == null) {
            resultsList = new ArrayList<WSSecurityEngineResult>();
        }
        resultsList.add(result);
    }
    
    /**
     * Get a WSSecurityEngineResult for the given Id.
     * @param uri is the (relative) uri of the id
     * @return the WSSecurityEngineResult or null if nothing found
     */
    public WSSecurityEngineResult getResult(String uri) {
        String id = uri;
        if (id == null) {
            return null;
        } else if (id.charAt(0) == '#') {
            id = id.substring(1);
        }
        if (resultsList != null) {
            for (WSSecurityEngineResult result : resultsList) {
                if (result != null) {
                	
                    // START DIGST WORKAROUND
                    if ("encryptedassertion".equals(id)) {
                        Object samlAssertion = result.get(WSSecurityEngineResult.TAG_SAML_ASSERTION);

                        if (samlAssertion != null) {
                            return result;
                        }
                    }
                    // END DIGST WORKAROUND
                	
                    String cId = (String)result.get(WSSecurityEngineResult.TAG_ID);
                    if (id.equals(cId)) {
                        return result;
                    }
                }
            }
        }
        return null;
    }
    
    /**
     * Get a list of WSSecurityEngineResults of the given Integer tag
     */
    public List<WSSecurityEngineResult> getResultsByTag(Integer tag) {
        List<WSSecurityEngineResult> foundResults = new ArrayList<WSSecurityEngineResult>();
        if (resultsList != null) {
            for (WSSecurityEngineResult result : resultsList) {
                if (result != null) {
                    Integer resultTag = (Integer)result.get(WSSecurityEngineResult.TAG_ACTION);
                    if (tag.intValue() == resultTag.intValue()) {
                        foundResults.add(result);
                    }
                }
            }
        }
        return foundResults;
    }
    
    /**
     * Get a WSSecurityEngineResult of the given Integer tag for the given Id
     */
    public WSSecurityEngineResult getResultByTag(Integer tag, String uri) {
        String id = uri;
        if (id == null || "".equals(uri)) {
            return null;
        } else if (id.charAt(0) == '#') {
            id = id.substring(1);
        }
        if (resultsList != null) {
            for (WSSecurityEngineResult result : resultsList) {
                if (result != null) {
                    Integer resultTag = (Integer)result.get(WSSecurityEngineResult.TAG_ACTION);
                    String cId = (String)result.get(WSSecurityEngineResult.TAG_ID);
                    if (tag.intValue() == resultTag.intValue() && id.equals(cId)) {
                        return result;
                    }
                }
            }
        }
        return null;
    }

    /**
     * @return the signature crypto class used to process
     *         the signature/verify
     */
    public Crypto getCrypto() {
        return crypto;
    }

    /**
     * @return the document
     */
    public Document getDocument() {
        return doc;
    }

    /**
     * @param crypto is the signature crypto class used to
     *               process signature/verify
     */
    public void setCrypto(Crypto crypto) {
        this.crypto = crypto;
    }
    
    /**
     * @param callbackLookup The CallbackLookup object to retrieve elements
     */
    public void setCallbackLookup(CallbackLookup callbackLookup) {
        this.callbackLookup = callbackLookup;
    }
    
    /**
     * @return the CallbackLookup object to retrieve elements
     */
    public CallbackLookup getCallbackLookup() {
        return callbackLookup;
    }

    /**
     * @return the wsse header being processed
     */
    public Element getSecurityHeader() {
        return securityHeader;
    }
    
    /**
     * Sets the wsse header being processed
     * 
     * @param securityHeader
     */
    public void setSecurityHeader(Element securityHeader) {
        this.securityHeader = securityHeader;
    }
}
