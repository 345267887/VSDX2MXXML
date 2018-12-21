using System;
using System.Xml;

/// <summary>
/// Copyright (c) 2007-2012, JGraph Ltd
/// </summary>
namespace mxGraph.util
{



    using Document = System.Xml.XmlDocument;
    using Node = System.Xml.XmlNode;

    //using InputSource = org.xml.sax.InputSource;

    /// <summary>
    /// Contains various XML helper methods for use with mxGraph.
    /// </summary>
    public class mxXmlUtils
    {

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //private static readonly Logger log = Logger.getLogger(typeof(mxXmlUtils).FullName);

        /// 
        

        /// <summary>
        /// Returns a new document for the given XML string. External entities and DTDs are ignored.
        /// </summary>
        /// <param name="xml">
        ///            String that represents the XML data. </param>
        /// <returns> Returns a new XML document. </returns>
        public static Document parseXml(string xml)
        {
            try
            {
                //DocumentBuilder.LoadXml(xml);
                Document doc = new Document();
                doc.LoadXml(xml);
                return doc;
            }
            catch (Exception e)
            {
                //log.log(Level.SEVERE, "Failed to parse XML", e);
            }

            return null;
        }

        /// <summary>
        /// Returns a string that represents the given node.
        /// </summary>
        /// <param name="node">
        ///            Node to return the XML for. </param>
        /// <returns> Returns an XML string. </returns>
        public static string getXml(Node node)
        {
            try
            {
                //Transformer tf = TransformerFactory.newInstance().newTransformer();

                //tf.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
                //tf.setOutputProperty(OutputKeys.ENCODING, "UTF-8");

                //StreamResult dest = new StreamResult(new StringWriter());
                //tf.transform(new DOMSource(node), dest);

                //return dest.Writer.ToString();

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
