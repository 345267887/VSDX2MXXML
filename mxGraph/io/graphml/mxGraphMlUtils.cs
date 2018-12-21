using System;
using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;
	using NodeList = System.Xml.XmlNodeList;

	/// <summary>
	/// This class implements several GML utility methods.
	/// </summary>
	public class mxGraphMlUtils
	{
		/// <summary>
		/// Checks if the NodeList has a Node with name = tag. </summary>
		/// <param name="nl"> NodeList </param>
		/// <param name="tag"> Name of the node. </param>
		/// <returns> Returns <code>true</code> if the Node List has a Node with name = tag. </returns>
		public static bool nodeListHasTag(NodeList nl, string tag)
		{
			bool has = false;

			if (nl != null)
			{
                int length = nl.Count;

				for (int i = 0; (i < length) && !has; i++)
				{
                    has = (nl.Item(i)).Name.Equals(tag);
				}
			}

			return has;
		}

		/// <summary>
		/// Returns the first Element that has name = tag in Node List. </summary>
		/// <param name="nl"> NodeList </param>
		/// <param name="tag"> Name of the Element </param>
		/// <returns> Element with name = 'tag'. </returns>
		public static Element nodeListTag(NodeList nl, string tag)
		{
			if (nl != null)
			{
				int length = nl.Count;
				bool has = false;

				for (int i = 0; (i < length) && !has; i++)
				{
                    has = (nl.Item(i)).Name.Equals(tag);

					if (has)
					{
						return (Element) nl.Item(i);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Returns a list with the elements included in the Node List that have name = tag. </summary>
		/// <param name="nl"> NodeList </param>
		/// <param name="tag"> name of the Element. </param>
		/// <returns> List with the indicated elements. </returns>
		public static IList<Element> nodeListTags(NodeList nl, string tag)
		{
			List<Element> ret = new List<Element>();

			if (nl != null)
			{
				int length = nl.Count;

				for (int i = 0; i < length; i++)
				{
                    if (tag.Equals((nl.Item(i)).Name))
					{
                        ret.Add((Element) nl.Item(i));
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Checks if the childrens of element has a Node with name = tag. </summary>
		/// <param name="element"> Element </param>
		/// <param name="tag"> Name of the node. </param>
		/// <returns> Returns <code>true</code> if the childrens of element has a Node with name = tag. </returns>
		public static bool childsHasTag(Element element, string tag)
		{
			NodeList nl = element.ChildNodes;

			bool has = false;

			if (nl != null)
			{
				int length = nl.Count;

				for (int i = 0; (i < length) && !has; i++)
				{
                    has = (nl.Item(i)).Name.Equals(tag);
				}
			}
			return has;
		}

		/// <summary>
		/// Returns the first Element that has name = tag in the childrens of element. </summary>
		/// <param name="element"> Element </param>
		/// <param name="tag"> Name of the Element </param>
		/// <returns> Element with name = 'tag'. </returns>
		public static Element childsTag(Element element, string tag)
		{
			NodeList nl = element.ChildNodes;

			if (nl != null)
			{
				int length = nl.Count;
				bool has = false;

				for (int i = 0; (i < length) && !has; i++)
				{
                    has = (nl.Item(i)).Name.Equals(tag);

					if (has)
					{
                        return (Element) nl.Item(i);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Returns a list with the elements included in the childrens of element
		/// that have name = tag. </summary>
		/// <param name="element"> Element </param>
		/// <param name="tag"> name of the Element. </param>
		/// <returns> List with the indicated elements. </returns>
		public static IList<Element> childsTags(Element element, string tag)
		{
			NodeList nl = element.ChildNodes;

			List<Element> ret = new List<Element>();

			if (nl != null)
			{
				int length = nl.Count;

				for (int i = 0; i < length; i++)
				{
                    if (tag.Equals((nl.Item(i)).Name))
					{
                        ret.Add((Element) nl.Item(i));
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Copy a given NodeList into a List<Element> </summary>
		/// <param name="nodeList"> Node List. </param>
		/// <returns> List with the elements of nodeList. </returns>
		public static IList<Node> copyNodeList(NodeList nodeList)
		{
			List<Node> copy = new List<Node>();
			int length = nodeList.Count;

			for (int i = 0; i < length; i++)
			{
                copy.Add((Node) nodeList.Item(i));
			}

			return copy;
		}

		/// <summary>
		/// Create a style map from a String with style definitions. </summary>
		/// <param name="style"> Definition of the style. </param>
		/// <param name="asig"> Asignation simbol used in 'style'. </param>
		/// <returns> Map with the style properties. </returns>
		public static Dictionary<string, object> getStyleMap(string style, string asig)
		{
			Dictionary<string, object> styleMap = new Dictionary<string, object>();
			string key = "";
			string value = "";
			int index = 0;

			if (!style.Equals(""))
			{
				string[] entries = style.Split(';');//style.Split(";", true);

                foreach (string entry in entries)
				{
					index = entry.IndexOf(asig, StringComparison.Ordinal);

					if (index == -1)
					{
						key = "";
						value = entry;
						styleMap[key] = value;
					}
					else
					{
						key = entry.Substring(0, index);
						value = entry.Substring(index + 1);
						styleMap[key] = value;
					}
				}
			}
			return styleMap;
		}

		/// <summary>
		/// Returns the string that represents the content of a given style map. </summary>
		/// <param name="styleMap"> Map with the styles values </param>
		/// <returns> string that represents the style. </returns>
		public static string getStyleString(IDictionary<string, object> styleMap, string asig)
		{
			string style = "";
            foreach (var item in styleMap)
            {
                string key = item.Key;
                object value = item.Value;
                style = style + key + asig + value + ";";
            }
//			IEnumerator<object> it = styleMap.Values.GetEnumerator();
//			IEnumerator<string> kit = styleMap.Keys.GetEnumerator();

//			while (kit.MoveNext())
//			{
//				string key = kit.Current;
////JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//				object value = it.next();
//				style = style + key + asig + value + ";";
//			}
			return style;
		}
	}

}