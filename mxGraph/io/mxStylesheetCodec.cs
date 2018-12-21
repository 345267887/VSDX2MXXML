using System.Collections.Generic;

/// <summary>
/// $Id: mxStylesheetCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006-2010, JGraph Ltd
/// </summary>
namespace mxGraph.io
{


	using Element = System.Xml.XmlElement;
	using Node =System.Xml.XmlNode;

	using mxUtils = mxGraph.util.mxUtils;
	using mxStylesheet = mxGraph.view.mxStylesheet;

	/// <summary>
	/// Codec for mxStylesheets. This class is created and registered
	/// dynamically at load time and used implicitely via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxStylesheetCodec : mxObjectCodec
	{

		/// <summary>
		/// Constructs a new model codec.
		/// </summary>
		public mxStylesheetCodec() : this(new mxStylesheet())
		{
		}

		/// <summary>
		/// Constructs a new stylesheet codec for the given template.
		/// </summary>
		public mxStylesheetCodec(object template) : this(template, null, null, null)
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given arguments.
		/// </summary>
		public mxStylesheetCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping) : base(template, exclude, idrefs, mapping)
		{
		}

		/// <summary>
		/// Encodes the given mxStylesheet.
		/// </summary>
		public override Node encode(mxCodec enc, object obj)
		{
            Element node = enc.document.CreateElement(Name);

			if (obj is mxStylesheet)
			{
				mxStylesheet stylesheet = (mxStylesheet) obj;
				//IEnumerator<KeyValuePair<string, IDictionary<string, object>>> it = stylesheet.Styles.SetOfKeyValuePairs().GetEnumerator();

				foreach (var item1 in stylesheet.Styles)
				{
					//KeyValuePair<string, IDictionary<string, object>> entry = it.Current;

                    Element styleNode = enc.document.CreateElement("add");
					string stylename = item1.Key;
                    styleNode.SetAttribute("as", stylename);


                    foreach (var item in item1.Value)
                    {
                        Element entryNode = enc.document.CreateElement("add");
                        entryNode.SetAttribute("as", item.Key.ToString());
                        entryNode.SetAttribute("value", item.Value.ToString());
                        styleNode.AppendChild(entryNode);
                    }
					//IDictionary<string, object> style = entry.Value;
					//IEnumerator<KeyValuePair<string, object>> it2 = style.SetOfKeyValuePairs().GetEnumerator();

					//while (it2.MoveNext())
					//{
					//	KeyValuePair<string, object> entry2 = it2.Current;
     //                   Element entryNode = enc.document.CreateElement("add");
     //                   entryNode.SetAttribute("as", entry2.Key.ToString());
     //                   entryNode.SetAttribute("value", entry2.Value.ToString());
     //                   styleNode.AppendChild(entryNode);
					//}

					if (styleNode.ChildNodes.Count > 0)
					{
                        node.AppendChild(styleNode);
					}
				}
			}

			return node;
		}

		/// <summary>
		/// Decodes the given mxStylesheet.
		/// </summary>
		public override object decode(mxCodec dec, Node node, object into)
		{
			object obj = null;

			if (node is Element)
			{
                string id = ((Element) node).GetAttribute("id");
				obj = dec.objects[id];

				if (obj == null)
				{
					obj = into;

					if (obj == null)
					{
						obj = cloneTemplate(node);
					}

					if (!string.ReferenceEquals(id, null) && id.Length > 0)
					{
						dec.putObject(id, obj);
					}
				}

				node = node.FirstChild;

				while (node != null)
				{
					if (!processInclude(dec, node, obj) && node.Name.Equals("add") && node is Element)
					{
                        string @as = ((Element) node).GetAttribute("as");

						if (!string.ReferenceEquals(@as, null) && @as.Length > 0)
						{
							string extend = ((Element) node).GetAttribute("extend");
							IDictionary<string, object> style = (!string.ReferenceEquals(extend, null)) ? ((mxStylesheet) obj).Styles[extend] : null;

							if (style == null)
							{
								style = new Dictionary<string, object>();
							}
							else
							{
								style = new Dictionary<string, object>(style);
							}

							Node entry = node.FirstChild;

							while (entry != null)
							{
								if (entry is Element)
								{
									Element entryElement = (Element) entry;
                                    string key = entryElement.GetAttribute("as");

									if (entry.Name.Equals("add"))
									{
										string text = entry.InnerText;
										object value = null;

										if (!string.ReferenceEquals(text, null) && text.Length > 0)
										{
											value = mxUtils.eval(text);
										}
										else
										{
                                            value = entryElement.GetAttribute("value");

										}

										if (value != null)
										{
											style[key] = value;
										}
									}
									else if (entry.Name.Equals("remove"))
									{
										style.Remove(key);
									}
								}

								entry = entry.NextSibling;
							}

							((mxStylesheet) obj).putCellStyle(@as, style);
						}
					}

					node = node.NextSibling;
				}
			}

			return obj;
		}

	}

}