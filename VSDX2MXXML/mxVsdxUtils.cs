using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxVsdxUtils
    {
        private static double screenCoordinatesPerCm = 40;

        private const double CENTIMETERS_PER_INCHES = 2.54;

        public static double conversionFactor = screenCoordinatesPerCm * CENTIMETERS_PER_INCHES;


        /**
         * Returns a collection of direct child Elements that match the specified tag name
         * @param parent the parent whose direct children will be processed
         * @param name the child tag name to match
         * @return a collection of matching Elements
         */
        public static List<XmlElement> getDirectChildNamedElements(XmlElement parent, String name)
        {
            List<XmlElement> result = new List<XmlElement>();

            for (XmlNode child = parent.FirstChild; child != null; child = child.NextSibling)
            {
                if (child is XmlElement && name.Equals(child.Name))
                {
                    result.Add((XmlElement)child);
                }
            }

            return result;
        }

        /**
         * Returns a collection of direct child Elements
         * @param parent the parent whose direct children will be processed
         * @return a collection of all child Elements
         */
        public static List<XmlElement> getDirectChildElements(XmlElement parent)
        {
            List<XmlElement> result = new List<XmlElement>();

            for (XmlNode child = parent.FirstChild; child != null; child = child.NextSibling)
            {
                if (child is XmlElement)
                {
                    result.Add((XmlElement)child);
                }
            }

            return result;
        }

        /**
         * Returns the first direct child Element
         * @param parent the parent whose direct first child will be processed
         * @return the first child Element
         */
        public static XmlElement getDirectFirstChildElement(XmlElement parent)
        {
            for (XmlNode child = parent.FirstChild; child != null; child = child.NextSibling)
            {
                if (child is XmlElement)
                {
                    return (XmlElement)child;
                }
            }

            return null;
        }

        /**
         * Return the value of an integer attribute or the default value
         * @param elem Element
         * @param attName Attribute name
         * @param defVal default value
         * @return the parsed attribute value or the default value
         */
        public static int getIntAttr(XmlElement elem, String attName, int defVal)
        {
            try
            {
                String val = elem.GetAttribute(attName);
                if (val != null)
                {
                    return int.Parse(val);
                }
            }
            catch (Exception e)
            {
                //nothing, just return the default value
            }
            return defVal;
        }

        /**
         * Return the value of an integer attribute or zero
         * @param elem Element
         * @param attName Attribute name
         * @return the parsed attribute value or zero
         */
        public static int getIntAttr(XmlElement elem, String attName)
        {
            return getIntAttr(elem, attName, 0);
        }

        /**
         * Returns the string that represents the content of a given style map.
         * @param styleMap Map with the styles values
         * @return string that represents the style.
         */
        public static String getStyleString(Dictionary<String, String> styleMap, String asig)
        {
            String style = "";


            foreach (var item in styleMap)
            {
                String key = item.Key;
                Object value = item.Value;

                if (!key.Equals(mxConstants.STYLE_SHAPE) || (!styleMap[key].StartsWith("image") && !styleMap[key].StartsWith("rounded=")))
                {
                    try
                    {
                        style = style + key + asig;
                    }
                    catch (Exception e)
                    {

                    }
                }

                style = style + value + ";";
            }

            return style;
        }

        /**
         * Returns a text surrounded by tags html.
         * @param text Text to be surrounded.
         * @param tag Name of the tag.
         * @return &lt tag &gt text &lt /tag &gt
         */
        public static String surroundByTags(String text, String tag)
        {
            return "<" + tag + ">" + text + "</" + tag + ">";
        }


        /**
         * Converts the ampersand, quote, prime, less-than and greater-than
         * characters to their corresponding HTML entities in the given string.
         * 
         * Note: this is the same method of mxUtils but we cannot use it as it is not compatible with google app engine
         */
        public static String htmlEntities(String text)
        {
            //return text.replaceAll("&", "&amp;").replaceAll("\"", "&quot;")
            //        .replaceAll("'", "&prime;").replaceAll("<", "&lt;")
            //        .replaceAll(">", "&gt;");

            return text.Replace("&", "&amp;").Replace("\"", "&quot;")
                    .Replace("'", "&prime;").Replace("<", "&lt;")
                    .Replace(">", "&gt;");
        }

        /**
         * Converts the initial letter  of each word in text to uppercase
         * @param text Text to be transformed.
         * @return Text with initial capitals.
         */
        public static String toInitialCapital(String text)
        {
            String[] words = text.Split(' ');
            String ret = "";

            foreach (String word in words)
            {
                String begin = word.Substring(0, 1);
                string word1 = word.Substring(1);
                begin = begin.ToUpper();
                ret += begin + word1;
            }

            return ret.Substring(0, ret.Length);
        }

        /**
         * Trnsforms each lower case letter in text to small capital.
         * @param text Text to be transformed.
         * @param size Size of the original text.
         * @return Text in small capitals.
         */
        public static String toSmallCaps(String text, String size)
        {
            String ret = "";

            if (!size.Equals(ret))
            {
                char a = 'a';
                char z = 'z';
                char[] letters = text.ToCharArray();

                foreach (char c in letters)
                {
                    if (c >= a && c <= z)
                    {
                        String s = (c.ToString());
                        s = s.ToUpper();
                        ret += "<font style=\"font-size:" + Double.Parse(size) / 1.28 + "px\">" + s + "</font>";
                    }
                    else
                    {
                        ret += c;
                    }
                }
            }
            else
            {
                ret = text;
            }

            return ret;
        }

        /**
         * Create a style map from a String with style definitions.
         * @param style Definition of the style.
         * @param asig Asignation simbol used in 'style'.
         * @return Map with the style properties.
         */
        public static Dictionary<String, Object> getStyleMap(String style, String asig)
        {
            Dictionary<String, Object> styleMap = new Dictionary<String, Object>();

            String[] entries = style.Split(';');

            foreach (String entry in entries)
            {
                int index = entry.IndexOf(asig);
                String key = entry.Substring(0, index);
                String value = entry.Substring(index + 1);
                styleMap.Add(key, value);
            }

            return styleMap;
        }

        public static bool isInsideTriangle(double x, double y, double ax, double ay, double bx, double by, double cx, double cy)
        {
            bx = bx - ax;
            by = by - ay;
            cx = cx - ax;
            cy = cy - ay;
            ax = 0;
            ay = 0;

            double d = bx * cy - cx * by;
            double wa = (x * (by - cy) + y * (cx - bx) + bx * cy - cx * by) / d;
            double wb = (x * cy - y * cx) / d;
            double wc = (y * bx - x * by) / d;

            if (wa > 0 && wa < 1 && wb > 0 && wb < 1 && wc > 0 && wc < 1)
            {
                return true;
            }

            return false;
        }
    }
}
