using System.Collections.Generic;

/// <summary>
/// $Id: mxChildChangeCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{

	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;

	using mxChildChange = mxGraph.model.mxGraphModel.mxChildChange;
	using mxICell = mxGraph.model.mxICell;

	/// <summary>
	/// Codec for mxChildChanges. This class is created and registered
	/// dynamically at load time and used implicitely via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxChildChangeCodec : mxObjectCodec
	{

		/// <summary>
		/// Constructs a new model codec.
		/// </summary>
		public mxChildChangeCodec() : this(new mxChildChange(), new string[] {"model", "child", "previousIndex"}, new string[] {"parent", "previous"}, null)
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given arguments.
		/// </summary>
		public mxChildChangeCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping) : base(template, exclude, idrefs, mapping)
		{
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#isReference(java.lang.Object, java.lang.String, java.lang.Object, boolean)
		 */
		public override bool isReference(object obj, string attr, object value, bool isWrite)
		{
			if (attr.Equals("child") && obj is mxChildChange && (((mxChildChange) obj).Previous != null || !isWrite))
			{
				return true;
			}

			return idrefs.Contains(attr);
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#afterEncode(mxGraphio.mxCodec, java.lang.Object, org.w3c.dom.Node)
		 */
		public override Node afterEncode(mxCodec enc, object obj, Node node)
		{
			if (obj is mxChildChange)
			{
				mxChildChange change = (mxChildChange) obj;
				object child = change.Child;

				if (isReference(obj, "child", child, true))
				{
					// Encodes as reference (id)
					mxCodec.setAttribute(node, "child", enc.getId(child));
				}
				else
				{
					// At this point, the encoder is no longer able to know which cells
					// are new, so we have to encode the complete cell hierarchy and
					// ignore the ones that are already there at decoding time. Note:
					// This can only be resolved by moving the notify event into the
					// execute of the edit.
					enc.encodeCell((mxICell) child, node, true);
				}
			}

			return node;
		}

		/// <summary>
		/// Reads the cells into the graph model. All cells are children of the root
		/// element in the node.
		/// </summary>
		public override Node beforeDecode(mxCodec dec, Node node, object into)
		{
			if (into is mxChildChange)
			{
				mxChildChange change = (mxChildChange) into;

				if (node.FirstChild != null && node.FirstChild.NodeType ==System.Xml.XmlNodeType.Element)
				{
                    // Makes sure the original node isn't modified
                    node = node.CloneNode(true);

					Node tmp = node.FirstChild;
					change.Child = dec.decodeCell(tmp, false);

					Node tmp2 = tmp.NextSibling;
                    tmp.ParentNode.RemoveChild(tmp);
					tmp = tmp2;

					while (tmp != null)
					{
						tmp2 = tmp.NextSibling;

						if (tmp.NodeType ==System.Xml.XmlNodeType.Element)
						{
                            // Ignores all existing cells because those do not need
                            // to be re-inserted into the model. Since the encoded
                            // version of these cells contains the new parent, this
                            // would leave to an inconsistent state on the model
                            // (ie. a parent change without a call to
                            // parentForCellChanged).
                            string id = ((Element) tmp).GetAttribute("id");

							if (dec.lookup(id) == null)
							{
								dec.decodeCell(tmp, true);
							}
						}

                        tmp.ParentNode.RemoveChild(tmp);
						tmp = tmp2;
					}
				}
				else
				{
                    string childRef = ((Element) node).GetAttribute("child");
					change.Child = (mxICell) dec.getObject(childRef);
				}
			}

			return node;
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#afterDecode(mxGraphio.mxCodec, org.w3c.dom.Node, java.lang.Object)
		 */
		public override object afterDecode(mxCodec dec, Node node, object obj)
		{
			if (obj is mxChildChange)
			{
				mxChildChange change = (mxChildChange) obj;

				// Cells are encoded here after a complete transaction so the previous
				// parent must be restored on the cell for the case where the cell was
				// added. This is needed for the local model to identify the cell as a
				// new cell and register the ID.
				((mxICell) change.Child).Parent = (mxICell) change.Previous;
				change.Previous = change.Parent;
				change.PreviousIndex = change.Index;
			}

			return obj;
		}

	}

}