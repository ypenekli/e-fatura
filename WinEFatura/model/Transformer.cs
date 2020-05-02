using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.IO;

namespace com.yp.efatura.model {
    class Transformer {


        /// <summary>
        /// Xml'i Html koduna çevirir.
        /// </summary>
        /// <param name="xslEncoded"></param>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        /// stash çalışması 1
        /// stash çalışma 2
        public static string XmlToHtml(string xslEncoded, string inputXml) {
            byte[] data = Convert.FromBase64String(xslEncoded);
            string decodedXslt = Encoding.UTF8.GetString(data);
            //string decodedXslt = (new UTF8Encoding(false)).GetString(data);
            string xmlFile = File.ReadAllText(inputXml);
            return TransformXMLToHTML(xmlFile, decodedXslt);
        }

        public static string TransformXMLToHTML(string inputXml, string xsltString) {
            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(new StringReader(xsltString))) {
                transform.Load(reader);
            }
            StringWriter results = new StringWriter();
            using (XmlReader reader = XmlReader.Create(
                new StringReader(inputXml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse })) {
                transform.Transform(reader, null, results);
            }
            String str = results.ToString();
            results.Close();
            return str;
        }

        public static String findXsltString(String pUrlXml) {
            XmlDocument xmldoc = new XmlDocument();
            FileStream fs = new FileStream(pUrlXml, FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            XmlNode x = find(xmldoc);
            if (x != null) {
                foreach (XmlNode n1 in x.ChildNodes) {
                    if (n1.Name == "cac:Attachment") {
                        //return n1.ChildNodes.Item(0).InnerText;
                        return n1.FirstChild.InnerText;
                    }
                }
            }
            return "";
            XmlNode find(XmlDocument xmldoc) {
                XmlNodeList xmlnodeList = xmldoc.GetElementsByTagName("cac:AdditionalDocumentReference");
                foreach (XmlNode x in xmlnodeList) {
                    foreach (XmlNode n1 in x.ChildNodes) {
                        if (n1.Name == "cbc:DocumentType" && !String.IsNullOrEmpty(n1.InnerText) && n1.InnerText.ToLower() == "xslt")
                            return x;
                    }
                }
                return null;
            }
        }





        public static void ExtractXsltFile(string xmlFilePath, string xsltFilePath) {
            XmlNamespaceManager GetXmlNsManager(XmlDocument doc) {
                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                ns.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
                ns.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                ns.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                ns.AddNamespace("ubltr", "urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents");
                ns.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                ns.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                ns.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                ns.AddNamespace("ef", "http://www.efatura.gov.tr/package-namespace");
                ns.AddNamespace("sh", "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader");
                ns.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                ns.AddNamespace("sch", "http://purl.oclc.org/dsdl/schematron");
                ns.AddNamespace("urn", "urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2");
                ns.AddNamespace("urn1", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                ns.AddNamespace("urn2", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                return ns;
            }

            var document = new XmlDocument();
            document.Load(xmlFilePath);
            foreach (XmlNode selectNode in document.DocumentElement.SelectNodes("//cac:AdditionalDocumentReference/cac:Attachment/cbc:EmbeddedDocumentBinaryObject", GetXmlNsManager(document))) {
                foreach (XmlAttribute attribute in (XmlNamedNodeMap)selectNode.Attributes) {
                    if (attribute.Name != "filename" || string.IsNullOrEmpty(attribute.Value) ||
                        !attribute.Value.ToLower().Contains("xslt")) continue;
                    if (selectNode.FirstChild == null) {
                        // int num = (int) MessageBox.Show("Xslt verisi okunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    var utF8Encoding = new UTF8Encoding(false);
                    var bytes = Convert.FromBase64String(selectNode.FirstChild.Value);
                    var str = utF8Encoding.GetString(bytes);
                    if (string.IsNullOrEmpty(str)) continue;
                    using (var streamWriter = new StreamWriter(xsltFilePath, false, (Encoding)utF8Encoding)) {
                        streamWriter.Write(str);
                        streamWriter.Close();
                        break;
                    }
                }
            }
        }
    }
}
