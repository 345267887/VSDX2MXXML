/// <summary>
/// Copyright (c) 2007-2012, JGraph Ltd
/// </summary>
namespace mxGraph.util
{

    using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;

    /// <summary>
    /// Contains various DOM API helper methods for use with mxGraph.
    /// </summary>
    public class mxDomUtils
    {

        /// <summary>
        /// Returns a new, empty DOM document.
        /// </summary>
        /// <returns> Returns a new DOM document. </returns>
        public static Document createDocument()
        {
            return new Document();
        }

        /// <summary>
        /// Creates a new SVG document for the given width and height.
        /// </summary>
        public static Document createSvgDocument(int width, int height)
        {
            Document document = createDocument();
            Element root = document.CreateElement("svg");

            string w = width.ToString();
            string h = height.ToString();

            root.SetAttribute("width", w);
            root.SetAttribute("height", h);
            root.SetAttribute("viewBox", "0 0 " + w + " " + h);
            root.SetAttribute("version", "1.1");
            root.SetAttribute("xmlns", mxConstants.NS_SVG);
            root.SetAttribute("xmlns:xlink", mxConstants.NS_XLINK);

            document.AppendChild(root);

            return document;
        }

        /// 
        public static Document createVmlDocument()
        {
            Document document = createDocument();

            Element root = document.CreateElement("html");
            root.SetAttribute("xmlns:v", "urn:schemas-microsoft-com:vml");
            root.SetAttribute("xmlns:o", "urn:schemas-microsoft-com:office:office");

            document.AppendChild(root);

            Element head = document.CreateElement("head");

            Element style = document.CreateElement("style");
            style.SetAttribute("type", "text/css");
            style.AppendChild(document.CreateTextNode("<!-- v\\:* {behavior: url(#default#VML);} -->"));

            head.AppendChild(style);
            root.AppendChild(head);

            Element body = document.CreateElement("body");
            root.AppendChild(body);

            return document;
        }

        /// <summary>
        /// Returns a document with a HTML node containing a HEAD and BODY node.
        /// </summary>
        public static Document createHtmlDocument()
        {
            Document document = createDocument();

            Element root = document.CreateElement("html");

            document.AppendChild(root);

            Element head = document.CreateElement("head");
            root.AppendChild(head);

            Element body = document.CreateElement("body");
            root.AppendChild(body);

            return document;
        }
    }
}
