using System.Collections.Generic;

namespace mxGraph.io.vsdx
{


    using Element = System.Xml.XmlElement;
	using NodeList = System.Xml.XmlNodeList;

	/// <summary>
	/// This is a singleton class that stores various global properties to document.<br/>
	/// The properties are:
	/// <ul>
	/// <li>
	/// document's colors
	/// </li>
	/// <li>
	/// document's fonts
	/// </li>
	/// <li>
	/// default text style
	/// </li>
	/// <li>
	/// default line style
	/// </li>
	/// <li>
	/// default fill style
	/// </li>
	/// </ul>
	/// </summary>
	public class mxPropertiesManager
	{
		/// <summary>
		/// Map with the document's colors.<br/>
		/// The key is the index number and the value is the hex representation of the color.
		/// </summary>
		private Dictionary<string, string> colorElementMap = new Dictionary<string, string>();

		/// <summary>
		/// Map with the document's fonts.<br/>
		/// The key is the ID and the value is the name of the font.
		/// </summary>
		private Dictionary<string, string> fontElementMap = new Dictionary<string, string>();

		/// <summary>
		/// Best guess at default colors if 0-23 are missing in the document (seems to always be the case for vsdx)
		/// </summary>
		private static readonly IDictionary<string, string> defaultColors = new Dictionary<string, string>();

		static mxPropertiesManager()
		{
			defaultColors["0"] = "#000000";
			defaultColors["1"] = "#FFFFFF";
			defaultColors["2"] = "#FF0000";
			defaultColors["3"] = "#00FF00";
			defaultColors["4"] = "#0000FF";
			defaultColors["5"] = "#FFFF00";
			defaultColors["6"] = "#FF00FF";
			defaultColors["7"] = "#00FFFF";
			defaultColors["8"] = "#800000";
			defaultColors["9"] = "#008000";
			defaultColors["10"] = "#000080";
			defaultColors["11"] = "#808000";
			defaultColors["12"] = "#800080";
			defaultColors["13"] = "#008080";
			defaultColors["14"] = "#C0C0C0";
			defaultColors["15"] = "#E6E6E6";
			defaultColors["16"] = "#CDCDCD";
			defaultColors["17"] = "#B3B3B3";
			defaultColors["18"] = "#9A9A9A";
			defaultColors["19"] = "#808080";
			defaultColors["20"] = "#666666";
			defaultColors["21"] = "#4D4D4D";
			defaultColors["22"] = "#333333";
			defaultColors["23"] = "#1A1A1A";
		}

		/// <summary>
		/// Loads the properties of the document. </summary>
		/// <param name="doc"> Document with the properties. </param>
		public virtual void initialise(Element elem, mxVsdxModel model)
		{
			//Loads the colors
			if (elem != null)
			{
                NodeList vdxColors = elem.GetElementsByTagName(mxVsdxConstants.COLORS);

				if (vdxColors.Count > 0)
				{
                    Element colors = (Element) vdxColors.Item(0);
                    NodeList colorList = colors.GetElementsByTagName(mxVsdxConstants.COLOR_ENTRY);
					int colorLength = colorList.Count;

					for (int i = 0; i < colorLength; i++)
					{
                        Element color = (Element) colorList.Item(i);
                        string colorId = color.GetAttribute(mxVsdxConstants.INDEX);
                        string colorValue = color.GetAttribute(mxVsdxConstants.RGB);

                        if (colorElementMap.ContainsKey(""))
                        {
                            colorElementMap[colorId] = colorValue;
                        }
                        else
                        {
                            colorElementMap.Add(colorId, colorValue);
                        }

						
					}
				}

                //Loads the fonts
                NodeList vdxFonts = elem.GetElementsByTagName(mxVsdxConstants.FACE_NAMES);

				if (vdxFonts.Count > 0)
				{
                    Element fonts = (Element) vdxFonts.Item(0);
                    NodeList fontList = fonts.GetElementsByTagName(mxVsdxConstants.FACE_NAME);
					int fontLength = fontList.Count;

					for (int i = 0; i < fontLength; i++)
					{
                        Element font = (Element) fontList.Item(i);
                        string fontId = font.GetAttribute(mxVsdxConstants.ID);
                        string fontValue = font.GetAttribute(mxVsdxConstants.FONT_NAME);
						fontElementMap[fontId] = fontValue;
					}
				}
			}
		}

		/// <summary>
		/// Returns the color of index indicated in 'ix'. </summary>
		/// <param name="ix"> Index of the color. </param>
		/// <returns> Hexadecimal representation of the color. </returns>
		public virtual string getColor(string ix)
		{
			string color = colorElementMap.ContainsKey(ix) ? colorElementMap[ix]:null;

			if (string.ReferenceEquals(color, null))
			{
				color = mxPropertiesManager.defaultColors.ContainsKey(ix)? mxPropertiesManager.defaultColors[ix]:null;

				if (string.ReferenceEquals(color, null))
				{
					return "";
				}
			}

			return color;
		}

		/// <summary>
		/// Returns the font of id indicated in 'id' </summary>
		/// <param name="id"> font's ID </param>
		/// <returns> Name of the font. </returns>
		public virtual string getFont(string id)
		{
			string font = fontElementMap[id];

			if (string.ReferenceEquals(font, null))
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