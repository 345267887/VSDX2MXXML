using System.Collections.Generic;

/// <summary>
/// $Id: mxCellCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{


	using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;

	using mxCell = mxGraph.model.mxCell;

	/// <summary>
	/// Codec for mxCells. This class is created and registered
	/// dynamically at load time and used implicitely via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxCellCodec : mxObjectCodec
	{

		/// <summary>
		/// Constructs a new cell codec.
		/// </summary>
		public mxCellCodec() : this(new mxCell(), null, new string[] {"parent", "source", "target"}, null)
		{
		}

		/// <summary>
		/// Constructs a new cell codec for the given template.
		/// </summary>
		public mxCellCodec(object template) : this(template, null, null, null)
		{
		}

		/// <summary>
		/// Constructs a new cell codec for the given arguments.
		/// </summary>
		public mxCellCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping) : base(template, exclude, idrefs, mapping)
		{
		}

		/// <summary>
		/// Excludes user objects that are XML nodes.
		/// </summary>
		public override bool isExcluded(object obj, string attr, object value, bool write)
		{
			return exclude.Contains(attr) || (write && attr.Equals("value") && value is Node && ((Node) value).NodeType ==System.Xml.XmlNodeType.None);
		}

		/// <summary>
		/// Encodes an mxCell and wraps the XML up inside the
		/// XML of the user object (inversion).
		/// </summary>
		public new Node afterEncode(mxCodec enc, object obj, Node node)
		{
			if (obj is mxCell)
			{
				mxCell cell = (mxCell) obj;

				if (cell.Value is Node)
				{
					// Wraps the graphical annotation up in the
					// user object (inversion) by putting the
					// result of the default encoding into
					// a clone of the user object (node type 1)
					// and returning this cloned user object.
					Element tmp = (Element) node;
                    node = enc.Document.ImportNode((Node) cell.Value, true);
                    node.AppendChild(tmp);

                    // Moves the id attribute to the outermost
                    // XML node, namely the node which denotes
                    // the object boundaries in the file.
                    string id = tmp.GetAttribute("id");
                    ((Element) node).SetAttribute("id", id);
                    tmp.RemoveAttribute("id");
				}
			}

			return node;
		}

		/// <summary>
		/// Decodes an mxCell and uses the enclosing XML node as
		/// the user object for the cell (inversion).
		/// </summary>
		public new Node beforeDecode(mxCodec dec, Node node, object obj)
		{
			Element inner = (Element) node;

			if (obj is mxCell)
			{
				mxCell cell = (mxCell) obj;
				string classname = Name;

				if (!node.Name.Equals(classname))
				{
                    // Passes the inner graphical annotation node to the
                    // object codec for further processing of the cell.
                    Node tmp = inner.GetElementsByTagName(classname).Item(0);

					if (tmp != null && tmp.ParentNode == node)
					{
						inner = (Element) tmp;

						// Removes annotation and whitespace from node
						Node tmp2 = tmp.PreviousSibling;

						while (tmp2 != null && tmp2.NodeType ==System.Xml.XmlNodeType.Text)
						{
							Node tmp3 = tmp2.PreviousSibling;

							if (tmp2.InnerText.Trim().Length == 0)
							{
                                tmp2.ParentNode.RemoveChild(tmp2);
							}

							tmp2 = tmp3;
						}

						// Removes more whitespace
						tmp2 = tmp.NextSibling;

						while (tmp2 != null && tmp2.NodeType == System.Xml.XmlNodeType.Text)
						{
							Node tmp3 = tmp2.PreviousSibling;

							if (tmp2.InnerText.Trim().Length == 0)
							{
                                tmp2.ParentNode.RemoveChild(tmp2);
							}

							tmp2 = tmp3;
						}

						tmp.ParentNode.RemoveChild(tmp);
					}
					else
					{
						inner = null;
					}

                    // Creates the user object out of the XML node
                    Element value = (Element) node.CloneNode(true);
					cell.Value = value;
                    string id = value.GetAttribute("id");

					if (!string.ReferenceEquals(id, null))
					{
						cell.Id = id;
                        value.RemoveAttribute("id");
					}
				}
				else
				{
                    cell.Id = ((Element) node).GetAttribute("id");
				}

				// Preprocesses and removes all Id-references
				// in order to use the correct encoder (this)
				// for the known references to cells (all).
				if (inner != null && idrefs != null)
				{
					IEnumerator<string> it = idrefs.GetEnumerator();

					while (it.MoveNext())
					{
						string attr = it.Current;
                        string @ref = inner.GetAttribute(attr);

						if (!string.ReferenceEquals(@ref, null) && @ref.Length > 0)
						{
                            inner.RemoveAttribute(attr);
							object @object = dec.objects[@ref];

							if (@object == null)
							{
								@object = dec.lookup(@ref);
							}

							if (@object == null)
							{
								// Needs to decode forward reference
								Node element = dec.getElementById(@ref);

								if (element != null)
								{
									mxObjectCodec decoder = mxCodecRegistry.getCodec(element.Name);

									if (decoder == null)
									{
										decoder = this;
									}

									@object = decoder.decode(dec, element);
								}
							}

							setFieldValue(obj, attr, @object);
						}
					}
				}
			}

			return inner;
		}

	}

}