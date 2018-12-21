using System;
using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{

	using mxConstants = mxGraph.util.mxConstants;


	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;

	/// <summary>
	/// General utilities for .vdx format support
	/// </summary>
	public class mxVsdxUtils
	{
		private static double screenCoordinatesPerCm = 40;

		private const double CENTIMETERS_PER_INCHES = 2.54;

		public static readonly double conversionFactor = screenCoordinatesPerCm * CENTIMETERS_PER_INCHES;

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		//private static readonly Logger log = Logger.getLogger(typeof(mxVsdxUtils).FullName);

		/// <summary>
		/// Returns a collection of direct child Elements that match the specified tag name </summary>
		/// <param name="parent"> the parent whose direct children will be processed </param>
		/// <param name="name"> the child tag name to match </param>
		/// <returns> a collection of matching Elements </returns>
		public static List<Element> getDirectChildNamedElements(Element parent, string name)
		{
			List<Element> result = new List<Element>();

			for (Node child = parent.FirstChild; child != null; child = child.NextSibling)
			{
				if (child is Element && name.Equals(child.Name))
				{
					result.Add((Element)child);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a collection of direct child Elements </summary>
		/// <param name="parent"> the parent whose direct children will be processed </param>
		/// <returns> a collection of all child Elements </returns>
		public static List<Element> getDirectChildElements(Element parent)
		{
			List<Element> result = new List<Element>();

			for (Node child = parent.FirstChild; child != null; child = child.NextSibling)
			{
				if (child is Element)
				{
					result.Add((Element)child);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the first direct child Element </summary>
		/// <param name="parent"> the parent whose direct first child will be processed </param>
		/// <returns> the first child Element </returns>
		public static Element getDirectFirstChildElement(Element parent)
		{
			for (Node child = parent.FirstChild; child != null; child = child.NextSibling)
			{
				if (child is Element)
				{
					return (Element)child;
				}
			}

			return null;
		}

		/// <summary>
		/// Return the value of an integer attribute or the default value </summary>
		/// <param name="elem"> Element </param>
		/// <param name="attName"> Attribute name </param>
		/// <param name="defVal"> default value </param>
		/// <returns> the parsed attribute value or the default value </returns>
		public static int getIntAttr(Element elem, string attName, int defVal)
		{
			try
			{
                string val = elem.Attributes[attName].Value;
				if (!string.ReferenceEquals(val, null))
				{
					return int.Parse(val);
				}
			}
			catch (System.FormatException)
			{
				//nothing, just return the default value
			}
			return defVal;
		}

		/// <summary>
		/// Return the value of an integer attribute or zero </summary>
		/// <param name="elem"> Element </param>
		/// <param name="attName"> Attribute name </param>
		/// <returns> the parsed attribute value or zero </returns>
		public static int getIntAttr(Element elem, string attName)
		{
			return getIntAttr(elem, attName, 0);
		}

		/// <summary>
		/// Returns the string that represents the content of a given style map. </summary>
		/// <param name="styleMap"> Map with the styles values </param>
		/// <returns> string that represents the style. </returns>
		public static string getStyleString(IDictionary<string, string> styleMap, string asig)
		{
			string style = "";
            foreach (var item in styleMap)
            {
                string key = item.Key;
                string value = item.Value;
                if (!key.Equals(mxConstants.STYLE_SHAPE) || (!styleMap[key].StartsWith("image", StringComparison.Ordinal) && !styleMap[key].StartsWith("rounded=", StringComparison.Ordinal)))
                {
                    try
                    {
                        style = style + key + asig;
                    }
                    catch (Exception e)
                    {
                        //log.log(Level.SEVERE, "mxVsdxUtils.getStyleString," + e.ToString() + ",style.length=" + style.Length + ",key.length=" + key.Length + ",asig.length=" + asig.Length);
                    }
                }

                style = style + value + ";";
            }
//			IEnumerator<string> it = styleMap.Values.GetEnumerator();
//			IEnumerator<string> kit = styleMap.Keys.GetEnumerator();

//			while (kit.MoveNext())
//			{
//				string key = kit.Current;
////JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//				object value = it.next();

//				if (!key.Equals(mxConstants.STYLE_SHAPE) || (!styleMap[key].StartsWith("image", StringComparison.Ordinal) && !styleMap[key].StartsWith("rounded=", StringComparison.Ordinal)))
//				{
//					try
//					{
//						style = style + key + asig;
//					}
//					catch (Exception e)
//					{
//						log.log(Level.SEVERE, "mxVsdxUtils.getStyleString," + e.ToString() + ",style.length=" + style.Length + ",key.length=" + key.Length + ",asig.length=" + asig.Length);
//					}
//				}

//				style = style + value + ";";
//			}

			return style;
		}

		/// <summary>
		/// Returns a text surrounded by tags html. </summary>
		/// <param name="text"> Text to be surrounded. </param>
		/// <param name="tag"> Name of the tag. </param>
		/// <returns> &lt tag &gt text &lt /tag &gt </returns>
		public static string surroundByTags(string text, string tag)
		{
			return "<" + tag + ">" + text + "</" + tag + ">";
		}


		/// <summary>
		/// Converts the ampersand, quote, prime, less-than and greater-than
		/// characters to their corresponding HTML entities in the given string.
		/// 
		/// Note: this is the same method of mxUtils but we cannot use it as it is not compatible with google app engine
		/// </summary>
		public static string htmlEntities(string text)
		{
            string temp= text.Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&prime;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
            return temp;//text.replaceAll("&", "&amp;").replaceAll("\"", "&quot;").replaceAll("'", "&prime;").replaceAll("<", "&lt;").replaceAll(">", "&gt;");
		}

		/// <summary>
		/// Converts the initial letter  of each word in text to uppercase </summary>
		/// <param name="text"> Text to be transformed. </param>
		/// <returns> Text with initial capitals. </returns>
		public static string toInitialCapital(string text)
		{
            string[] words = text.Split(' '); //text.Split(" ", true);

            string ret = "";

			foreach (string word in words)
			{
				string begin = word.Substring(0, 1);
				string word1 = word.Substring(1);
				begin = begin.ToUpper();
				ret += begin + word1;
			}

			return ret.Substring(0, ret.Length);
		}

		/// <summary>
		/// Trnsforms each lower case letter in text to small capital. </summary>
		/// <param name="text"> Text to be transformed. </param>
		/// <param name="size"> Size of the original text. </param>
		/// <returns> Text in small capitals. </returns>
		public static string toSmallCaps(string text, string size)
		{
			string ret = "";

			if (!size.Equals(ret))
			{
				char a = 'a';
				char z = 'z';
				char[] letters = text.ToCharArray();

				foreach (char c in letters)
				{
					if (c >= a && c <= z)
					{
						string s = c.ToString();
						s = s.ToUpper();
						ret += "<font style=\"font-size:" + Convert.ToDouble(size) / 1.28 + "px\">" + s + "</font>";
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

		/// <summary>
		/// Create a style map from a String with style definitions. </summary>
		/// <param name="style"> Definition of the style. </param>
		/// <param name="asig"> Asignation simbol used in 'style'. </param>
		/// <returns> Map with the style properties. </returns>
		public static Dictionary<string, object> getStyleMap(string style, string asig)
		{
			Dictionary<string, object> styleMap = new Dictionary<string, object>();

            string[] entries = style.Split(';');//style.Split(";", true);


            foreach (string entry in entries)
			{
				int index = entry.IndexOf(asig, StringComparison.Ordinal);
				string key = entry.Substring(0, index);
				string value = entry.Substring(index + 1);
				styleMap[key] = value;
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