using System.Collections.Generic;

/// <summary>
/// $Id: mxRootChangeCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{

	using Element = System.Xml.XmlDocument;
	using Node = System.Xml.XmlNode;

	using mxChildChange = mxGraph.model.mxGraphModel.mxChildChange;
	using mxRootChange = mxGraph.model.mxGraphModel.mxRootChange;
	using mxICell = mxGraph.model.mxICell;

	/// <summary>
	/// Codec for mxChildChanges. This class is created and registered
	/// dynamically at load time and used implicitely via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxRootChangeCodec : mxObjectCodec
	{

		/// <summary>
		/// Constructs a new model codec.
		/// </summary>
		public mxRootChangeCodec() : this(new mxRootChange(), new string[] {"model", "previous", "root"}, null, null)
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given arguments.
		/// </summary>
		public mxRootChangeCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping) : base(template, exclude, idrefs, mapping)
		{
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#afterEncode(mxGraphio.mxCodec, java.lang.Object, org.w3c.dom.Node)
		 */
		public override Node afterEncode(mxCodec enc, object obj, Node node)
		{
			if (obj is mxRootChange)
			{
				enc.encodeCell((mxICell)((mxRootChange) obj).Root, node, true);
			}

			return node;
		}

		/// <summary>
		/// Reads the cells into the graph model. All cells are children of the root
		/// element in the node.
		/// </summary>
		public override Node beforeDecode(mxCodec dec, Node node, object into)
		{
			if (into is mxRootChange)
			{
				mxRootChange change = (mxRootChange) into;

				if (node.FirstChild != null && node.FirstChild.NodeType ==System.Xml.XmlNodeType.Element)
				{
                    // Makes sure the original node isn't modified
                    node = node.CloneNode(true);

					Node tmp = node.FirstChild;
					change.Root = dec.decodeCell(tmp, false);

					Node tmp2 = tmp.NextSibling;
                    tmp.ParentNode.RemoveChild(tmp);
					tmp = tmp2;

					while (tmp != null)
					{
						tmp2 = tmp.NextSibling;

						if (tmp.NodeType == System.Xml.XmlNodeType.Element)
						{
							dec.decodeCell(tmp, true);
						}

                        tmp.ParentNode.RemoveChild(tmp);
						tmp = tmp2;
					}
				}
			}

			return node;
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#afterDecode(mxGraphio.mxCodec, org.w3c.dom.Node, java.lang.Object)
		 */
		public override object afterDecode(mxCodec dec, Node node, object obj)
		{
			if (obj is mxRootChange)
			{
				mxRootChange change = (mxRootChange) obj;
				change.Previous = change.Root;
			}

			return obj;
		}

	}

}