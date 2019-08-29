
package org.tempuri;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Java class for anonymous complex type.
 * 
 * <p>The following schema fragment specifies the expected content contained within this class.
 * 
 * <pre>
 * &lt;complexType&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="HelloSignErrorNotEncryptedResult" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "", propOrder = {
    "helloSignErrorNotEncryptedResult"
})
@XmlRootElement(name = "HelloSignErrorNotEncryptedResponse")
public class HelloSignErrorNotEncryptedResponse {

    @XmlElementRef(name = "HelloSignErrorNotEncryptedResult", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<String> helloSignErrorNotEncryptedResult;

    /**
     * Gets the value of the helloSignErrorNotEncryptedResult property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getHelloSignErrorNotEncryptedResult() {
        return helloSignErrorNotEncryptedResult;
    }

    /**
     * Sets the value of the helloSignErrorNotEncryptedResult property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setHelloSignErrorNotEncryptedResult(JAXBElement<String> value) {
        this.helloSignErrorNotEncryptedResult = value;
    }

}
