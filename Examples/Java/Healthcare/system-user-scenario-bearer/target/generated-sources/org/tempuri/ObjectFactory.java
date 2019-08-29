
package org.tempuri;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlElementDecl;
import javax.xml.bind.annotation.XmlRegistry;
import javax.xml.namespace.QName;


/**
 * This object contains factory methods for each 
 * Java content interface and Java element interface 
 * generated in the org.tempuri package. 
 * <p>An ObjectFactory allows you to programatically 
 * construct new instances of the Java representation 
 * for XML content. The Java representation of XML 
 * content can consist of schema derived interfaces 
 * and classes representing the binding of schema 
 * type definitions, element declarations and model 
 * groups.  Factory methods for each of these are 
 * provided in this class.
 * 
 */
@XmlRegistry
public class ObjectFactory {

    private final static QName _HelloNoneName_QNAME = new QName("http://tempuri.org/", "name");
    private final static QName _HelloNoneResponseHelloNoneResult_QNAME = new QName("http://tempuri.org/", "HelloNoneResult");
    private final static QName _HelloNoneErrorResponseHelloNoneErrorResult_QNAME = new QName("http://tempuri.org/", "HelloNoneErrorResult");
    private final static QName _HelloSignResponseHelloSignResult_QNAME = new QName("http://tempuri.org/", "HelloSignResult");
    private final static QName _HelloSignErrorResponseHelloSignErrorResult_QNAME = new QName("http://tempuri.org/", "HelloSignErrorResult");

    /**
     * Create a new ObjectFactory that can be used to create new instances of schema derived classes for package: org.tempuri
     * 
     */
    public ObjectFactory() {
    }

    /**
     * Create an instance of {@link HelloNone }
     * 
     */
    public HelloNone createHelloNone() {
        return new HelloNone();
    }

    /**
     * Create an instance of {@link HelloNoneResponse }
     * 
     */
    public HelloNoneResponse createHelloNoneResponse() {
        return new HelloNoneResponse();
    }

    /**
     * Create an instance of {@link HelloNoneError }
     * 
     */
    public HelloNoneError createHelloNoneError() {
        return new HelloNoneError();
    }

    /**
     * Create an instance of {@link HelloNoneErrorResponse }
     * 
     */
    public HelloNoneErrorResponse createHelloNoneErrorResponse() {
        return new HelloNoneErrorResponse();
    }

    /**
     * Create an instance of {@link HelloSign }
     * 
     */
    public HelloSign createHelloSign() {
        return new HelloSign();
    }

    /**
     * Create an instance of {@link HelloSignResponse }
     * 
     */
    public HelloSignResponse createHelloSignResponse() {
        return new HelloSignResponse();
    }

    /**
     * Create an instance of {@link HelloSignError }
     * 
     */
    public HelloSignError createHelloSignError() {
        return new HelloSignError();
    }

    /**
     * Create an instance of {@link HelloSignErrorResponse }
     * 
     */
    public HelloSignErrorResponse createHelloSignErrorResponse() {
        return new HelloSignErrorResponse();
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "name", scope = HelloNone.class)
    public JAXBElement<String> createHelloNoneName(String value) {
        return new JAXBElement<String>(_HelloNoneName_QNAME, String.class, HelloNone.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "HelloNoneResult", scope = HelloNoneResponse.class)
    public JAXBElement<String> createHelloNoneResponseHelloNoneResult(String value) {
        return new JAXBElement<String>(_HelloNoneResponseHelloNoneResult_QNAME, String.class, HelloNoneResponse.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "name", scope = HelloNoneError.class)
    public JAXBElement<String> createHelloNoneErrorName(String value) {
        return new JAXBElement<String>(_HelloNoneName_QNAME, String.class, HelloNoneError.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "HelloNoneErrorResult", scope = HelloNoneErrorResponse.class)
    public JAXBElement<String> createHelloNoneErrorResponseHelloNoneErrorResult(String value) {
        return new JAXBElement<String>(_HelloNoneErrorResponseHelloNoneErrorResult_QNAME, String.class, HelloNoneErrorResponse.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "name", scope = HelloSign.class)
    public JAXBElement<String> createHelloSignName(String value) {
        return new JAXBElement<String>(_HelloNoneName_QNAME, String.class, HelloSign.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "HelloSignResult", scope = HelloSignResponse.class)
    public JAXBElement<String> createHelloSignResponseHelloSignResult(String value) {
        return new JAXBElement<String>(_HelloSignResponseHelloSignResult_QNAME, String.class, HelloSignResponse.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "name", scope = HelloSignError.class)
    public JAXBElement<String> createHelloSignErrorName(String value) {
        return new JAXBElement<String>(_HelloNoneName_QNAME, String.class, HelloSignError.class, value);
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link String }{@code >}}
     * 
     */
    @XmlElementDecl(namespace = "http://tempuri.org/", name = "HelloSignErrorResult", scope = HelloSignErrorResponse.class)
    public JAXBElement<String> createHelloSignErrorResponseHelloSignErrorResult(String value) {
        return new JAXBElement<String>(_HelloSignErrorResponseHelloSignErrorResult_QNAME, String.class, HelloSignErrorResponse.class, value);
    }

}
