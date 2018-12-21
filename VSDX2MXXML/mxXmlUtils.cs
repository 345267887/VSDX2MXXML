using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxXmlUtils
    {

	/**
	 * 
	 */
	private static XmlDocument documentBuilder = null;

        /**
         * 
         */
        public static XmlDocument getDocumentBuilder()
        {
            if (documentBuilder == null)
            {
                XmlDocument dbf =new XmlDocument();
                //dbf.setExpandEntityReferences(false);
                //dbf.setXIncludeAware(false);
                //dbf.setValidating(false);

                //try
                //{
                //    dbf.setFeature("http://apache.org/xml/features/nonvalidating/load-external-dtd", false);
                //    dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
                //    dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
                //}
                //catch (Exception e)
                //{

                //}

                //try
                //{
                //    documentBuilder = dbf.newDocumentBuilder();
                //}
                //catch (Exception e)
                //{
                //}

                documentBuilder = dbf;
            }

            return documentBuilder;
        }

        /**
         * Returns a new document for the given XML string. External entities and DTDs are ignored.
         * 
         * @param xml
         *            String that represents the XML data.
         * @return Returns a new XML document.
         */
        public static XmlDocument parseXml(String xml)
        {
            try
            {
                documentBuilder.LoadXml(xml);
                return documentBuilder;
                //return getDocumentBuilder().parse(new InputSource(new StringReader(xml)));
            }
            catch (Exception e)
            {
                //log.log(Level.SEVERE, "Failed to parse XML", e);
            }

            return null;
        }

        /**
         * Returns a string that represents the given node.
         * 
         * @param node
         *            Node to return the XML for.
         * @return Returns an XML string.
         */
        public static String getXml(XmlNode node)
        {
            try
            {
                //Transformer tf = TransformerFactory.newInstance().newTransformer();

                //tf.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
                //tf.setOutputProperty(OutputKeys.ENCODING, "UTF-8");

                //StreamResult dest = new StreamResult(new StringWriter());
                //tf.transform(new DOMSource(node), dest);

                //return dest.getWriter().toString();

                return node.OuterXml;
            }
            catch (Exception e)
            {
                //log.log(Level.SEVERE, "Failed to convert XML object to string", e);
            }

            return "";
        }
    }
}
