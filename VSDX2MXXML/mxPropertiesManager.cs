using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxPropertiesManager
    {
        /**
	 * Map with the document's colors.<br/>
	 * The key is the index number and the value is the hex representation of the color.
	 */
        private Dictionary<String, String> colorElementMap = new Dictionary<String, String>();

        /**
         * Map with the document's fonts.<br/>
         * The key is the ID and the value is the name of the font.
         */
        private Dictionary<String, String> fontElementMap = new Dictionary<String, String>();

        /**
         * Best guess at default colors if 0-23 are missing in the document (seems to always be the case for vsdx)
         */
        private static Dictionary<String, String> defaultColors = new Dictionary<String, String>()
        {
            {"0", "#000000"},
            {"1", "#FFFFFF"},
            {"2", "#FF0000"},
            {"3", "#00FF00"},
            {"4", "#0000FF"},
            {"5", "#FFFF00"},
            {"6", "#FF00FF"},
            {"7", "#00FFFF"},
            {"8", "#800000"},
            {"9", "#008000"},
            {"10", "#000080"},
            {"11", "#808000"},
            {"12", "#800080"},
            {"13", "#008080"},
            {"14", "#C0C0C0"},
            {"15", "#E6E6E6"},
            {"16", "#CDCDCD"},
            {"17", "#B3B3B3"},
            {"18", "#9A9A9A"},
            {"19", "#808080"},
            {"20", "#666666"},
            {"21", "#4D4D4D"},
            {"22", "#333333"},
            {"23", "#1A1A1A"}
        };

        /**
         * Loads the properties of the document.
         * @param doc Document with the properties.
         */
        public void initialise(XmlElement elem, mxVsdxModel model)
        {
            //Loads the colors
            if (elem != null)
            {
                XmlNodeList vdxColors = elem.GetElementsByTagName(mxVsdxConstants.COLORS);

                if (vdxColors.Count > 0)
                {
                    XmlElement colors = (XmlElement)vdxColors[0];
                    XmlNodeList colorList = colors.GetElementsByTagName(mxVsdxConstants.COLOR_ENTRY);
                    int colorLength = colorList.Count;

                    for (int i = 0; i < colorLength; i++)
                    {
                        XmlElement color = (XmlElement)colorList[i];
                        String colorId = color.GetAttribute(mxVsdxConstants.INDEX);
                        String colorValue = color.GetAttribute(mxVsdxConstants.RGB);
                        colorElementMap.Add(colorId, colorValue);
                    }
                }

                //Loads the fonts
                XmlNodeList vdxFonts = elem.GetElementsByTagName(mxVsdxConstants.FACE_NAMES);

                if (vdxFonts.Count > 0)
                {
                    XmlElement fonts = (XmlElement)vdxFonts[(0)];
                    XmlNodeList fontList = fonts.GetElementsByTagName(mxVsdxConstants.FACE_NAME);
                    int fontLength = fontList.Count;

                    for (int i = 0; i < fontLength; i++)
                    {
                        XmlElement font = (XmlElement)fontList[(i)];
                        String fontId = font.GetAttribute(mxVsdxConstants.ID);
                        String fontValue = font.GetAttribute(mxVsdxConstants.FONT_NAME);
                        fontElementMap.Add(fontId, fontValue);
                    }
                }
            }
        }

        /**
         * Returns the color of index indicated in 'ix'.
         * @param ix Index of the color.
         * @return Hexadecimal representation of the color.
         */
        public String getColor(String ix)
        {
            String color = colorElementMap[(ix)];

            if (color == null)
            {
                color = mxPropertiesManager.defaultColors[(ix)];

                if (color == null)
                {
                    return "";
                }
            }

            return color;
        }

        /**
         * Returns the font of id indicated in 'id'
         * @param id font's ID
         * @return Name of the font.
         */
        public String getFont(String id)
        {
            String font = fontElementMap[(id)];

            if (font == null)
            {
                return "";
            }
            else
            {
                return font;
            }
        }
    }
}
