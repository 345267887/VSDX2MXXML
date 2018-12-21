using System.Collections.Generic;

/// <summary>
/// $Id: mxModelCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.io
{

	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;

	using model = mxGraph.model.mxGraphModel;
	using mxICell = mxGraph.model.mxICell;

	/// <summary>
	/// Codec for models. This class is created and registered
	/// dynamically at load time and used implicitly via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxModelCodec : mxObjectCodec
	{

		/// <summary>
		/// Constructs a new model codec.
		/// </summary>
		public mxModelCodec() : this(new model())
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given template.
		/// </summary>
		public mxModelCodec(object template) : this(template, null, null, null)
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given arguments.
		/// </summary>
		public mxModelCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping) : base(template, exclude, idrefs, mapping)
		{
		}

		/// <summary>
		/// Encode the given model by writing a (flat) XML sequence
		/// of cell nodes as produced by the mxCellCodec. The sequence is
		/// wrapped-up in a node with the name root.
		/// </summary>
		public override Node encode(mxCodec enc, object obj)
		{
			Node node = null;

			if (obj is model)
			{
				model model = (model) obj;

				node = enc.document.CreateElement(Name);
                Node rootNode = enc.document.CreateElement("root");

				enc.encodeCell((mxICell) model.Root, rootNode, true);
                node.AppendChild(rootNode);
			}

			return node;
		}

		/// <summary>
		/// Reads the cells into the graph model. All cells are children of the root
		/// element in the node.
		/// </summary>
		public override Node beforeDecode(mxCodec dec, Node node, object into)
		{
			if (node is Element)
			{
				Element elt = (Element) node;
				model model = null;

				if (into is model)
				{
					model = (model) into;
				}
				else
				{
					model = new model();
				}

                // Reads the cells into the graph model. All cells
                // are children of the root element in the node.
                Node root = elt.GetElementsByTagName("root").Item(0);
				mxICell rootCell = null;

				if (root != null)
				{
					Node tmp = root.FirstChild;

					while (tmp != null)
					{
						mxICell cell = dec.decodeCell(tmp, true);

						if (cell != null && cell.Parent == null)
						{
							rootCell = cell;
						}

						tmp = tmp.NextSibling;
					}

                    root.ParentNode.RemoveChild(root);
				}

				// Sets the root on the model if one has been decoded
				if (rootCell != null)
				{
					model.Root = rootCell;
				}
			}

			return node;
		}

	}

}