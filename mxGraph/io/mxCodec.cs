using System;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// $Id: mxCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{


    using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;

	using mxCell = mxGraph.model.mxCell;
	using mxCellPath = mxGraph.model.mxCellPath;
	using mxICell = mxGraph.model.mxICell;
	using mxUtils = mxGraph.util.mxUtils;

	/// <summary>
	/// XML codec for Java object graphs. In order to resolve forward references
	/// when reading files the XML document that contains the data must be passed
	/// to the constructor.
	/// </summary>
	public class mxCodec
	{

		/// <summary>
		/// Holds the owner document of the codec.
		/// </summary>
		protected internal Document document;

		/// <summary>
		/// Maps from IDs to objects.
		/// </summary>
		protected internal IDictionary<string, object> objects = new Dictionary<string, object>();

		/// <summary>
		/// Specifies if default values should be encoded. Default is false.
		/// </summary>
		protected internal bool encodeDefaults = false;

		/// <summary>
		/// Constructs an XML encoder/decoder with a new owner document.
		/// </summary>
		public mxCodec() : this(mxUtils.createDocument())
		{
		}

		/// <summary>
		/// Constructs an XML encoder/decoder for the specified owner document.
		/// </summary>
		/// <param name="document"> Optional XML document that contains the data. If no document
		/// is specified then a new document is created using mxUtils.createDocument </param>
		public mxCodec(Document document)
		{
			if (document == null)
			{
				document = mxUtils.createDocument();
			}

			this.document = document;
		}

		/// <summary>
		/// Returns the owner document of the codec.
		/// </summary>
		/// <returns> Returns the owner document. </returns>
		public virtual Document Document
		{
			get
			{
				return document;
			}
			set
			{
				document = value;
			}
		}


		/// <summary>
		/// Returns if default values of member variables should be encoded.
		/// </summary>
		public virtual bool EncodeDefaults
		{
			get
			{
				return encodeDefaults;
			}
			set
			{
				this.encodeDefaults = value;
			}
		}


		/// <summary>
		/// Returns the object lookup table.
		/// </summary>
		public virtual IDictionary<string, object> Objects
		{
			get
			{
				return objects;
			}
		}

		/// <summary>
		/// Assoiates the given object with the given ID.
		/// </summary>
		/// <param name="id"> ID for the object to be associated with. </param>
		/// <param name="object"> Object to be associated with the ID. </param>
		/// <returns> Returns the given object. </returns>
		public virtual object putObject(string id, object @object)
		{
			return objects[id] = @object;
		}

		/// <summary>
		/// Returns the decoded object for the element with the specified ID in
		/// <seealso cref="#document"/>. If the object is not known then <seealso cref="#lookup(String)"/>
		/// is used to find an object. If no object is found, then the element with
		/// the respective ID from the document is parsed using <seealso cref="#decode(Node)"/>.
		/// </summary>
		/// <param name="id"> ID of the object to be returned. </param>
		/// <returns> Returns the object for the given ID. </returns>
		public virtual object getObject(string id)
		{
			object obj = null;

			if (!string.ReferenceEquals(id, null))
			{
				obj = objects[id];

				if (obj == null)
				{
					obj = lookup(id);

					if (obj == null)
					{
						Node node = getElementById(id);

						if (node != null)
						{
							obj = decode(node);
						}
					}
				}
			}

			return obj;
		}

		/// <summary>
		/// Hook for subclassers to implement a custom lookup mechanism for cell IDs.
		/// This implementation always returns null.
		/// </summary>
		/// <param name="id"> ID of the object to be returned. </param>
		/// <returns> Returns the object for the given ID. </returns>
		public virtual object lookup(string id)
		{
			return null;
		}

		/// <summary>
		/// Returns the element with the given ID from the document.
		/// </summary>
		/// <param name="id"> ID of the element to be returned. </param>
		/// <returns> Returns the element for the given ID. </returns>
		public virtual Node getElementById(string id)
		{
			return getElementById(id, null);
		}

		/// <summary>
		/// Returns the element with the given ID from document. The optional attr
		/// argument specifies the name of the ID attribute. Default is "id". The
		/// XPath expression used to find the element is //*[@attr='arg'] where attr
		/// is the name of the ID attribute and arg is the given id.
		/// 
		/// Parameters:
		/// 
		/// id - String that contains the ID.
		/// attr - Optional string for the attributename. Default is id.
		/// </summary>
		public virtual Node getElementById(string id, string attr)
		{
			if (string.ReferenceEquals(attr, null))
			{
				attr = "id";
			}

			string expr = "//*[@" + attr + "='" + id + "']";

			return mxUtils.selectSingleNode(document, expr);
		}

		/// <summary>
		/// Returns the ID of the specified object. This implementation calls
		/// reference first and if that returns null handles the object as an
		/// mxCell by returning their IDs using mxCell.getId. If no ID exists for
		/// the given cell, then an on-the-fly ID is generated using
		/// mxCellPath.create.
		/// </summary>
		/// <param name="obj"> Object to return the ID for. </param>
		/// <returns> Returns the ID for the given object. </returns>
		public virtual string getId(object obj)
		{
			string id = null;

			if (obj != null)
			{
				id = reference(obj);

				if (string.ReferenceEquals(id, null) && obj is mxICell)
				{
					id = ((mxICell) obj).Id;

					if (string.ReferenceEquals(id, null))
					{
						// Uses an on-the-fly Id
						id = mxCellPath.create((mxICell) obj);

						if (id.Length == 0)
						{
							id = "root";
						}
					}
				}
			}

			return id;
		}

		/// <summary>
		/// Hook for subclassers to implement a custom method for retrieving IDs from
		/// objects. This implementation always returns null.
		/// </summary>
		/// <param name="obj"> Object whose ID should be returned. </param>
		/// <returns> Returns the ID for the given object. </returns>
		public virtual string reference(object obj)
		{
			return null;
		}

		/// <summary>
		/// Encodes the specified object and returns the resulting XML node.
		/// </summary>
		/// <param name="obj"> Object to be encoded. </param>
		/// <returns> Returns an XML node that represents the given object. </returns>
		public virtual Node encode(object obj)
		{
			Node node = null;

			if (obj != null)
			{
				string name = mxCodecRegistry.getName(obj);
				mxObjectCodec enc = mxCodecRegistry.getCodec(name);

				if (enc != null)
				{
					node = enc.encode(this, obj);
				}
				else
				{
					if (obj is Node)
					{
                        node = ((Node) obj).CloneNode(true);
					}
					else
					{
						Console.Error.WriteLine("No codec for " + name);
					}
				}
			}

			return node;
		}

		/// <summary>
		/// Decodes the given XML node using <seealso cref="#decode(Node, Object)"/>.
		/// </summary>
		/// <param name="node"> XML node to be decoded. </param>
		/// <returns> Returns an object that represents the given node. </returns>
		public virtual object decode(Node node)
		{
			return decode(node, null);
		}

		/// <summary>
		/// Decodes the given XML node. The optional "into" argument specifies an
		/// existing object to be used. If no object is given, then a new
		/// instance is created using the constructor from the codec.
		/// 
		/// The function returns the passed in object or the new instance if no
		/// object was given.
		/// </summary>
		/// <param name="node"> XML node to be decoded. </param>
		/// <param name="into"> Optional object to be decodec into. </param>
		/// <returns> Returns an object that represents the given node. </returns>
		public virtual object decode(Node node, object into)
		{
			object obj = null;

			if (node != null && node.NodeType == XmlNodeType.Element)
			{
				mxObjectCodec codec = mxCodecRegistry.getCodec(node.Name);

				try
				{
					if (codec != null)
					{
						obj = codec.decode(this, node, into);
					}
					else
					{
                        obj = node.CloneNode(true);
                        ((Element) obj).RemoveAttribute("as");
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("Cannot decode " + node.Name + ": " + e.Message);
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}

			return obj;
		}

		/// <summary>
		/// Encoding of cell hierarchies is built-into the core, but is a
		/// higher-level function that needs to be explicitely used by the
		/// respective object encoders (eg. mxModelCodec, mxChildChangeCodec
		/// and mxRootChangeCodec). This implementation writes the given cell
		/// and its children as a (flat) sequence into the given node. The
		/// children are not encoded if the optional includeChildren is false.
		/// The function is in charge of adding the result into the given node
		/// and has no return value.
		/// </summary>
		/// <param name="cell"> mxCell to be encoded. </param>
		/// <param name="node"> Parent XML node to add the encoded cell into. </param>
		/// <param name="includeChildren"> Boolean indicating if the method
		/// should include all descendents. </param>
		public virtual void encodeCell(mxICell cell, Node node, bool includeChildren)
		{
            node.AppendChild(encode(cell));

			if (includeChildren)
			{
				int childCount = cell.ChildCount;

				for (int i = 0; i < childCount; i++)
				{
					encodeCell(cell.getChildAt(i), node, includeChildren);
				}
			}
		}

		/// <summary>
		/// Decodes cells that have been encoded using inversion, ie. where the
		/// user object is the enclosing node in the XML, and restores the group
		/// and graph structure in the cells. Returns a new <mxCell> instance
		/// that represents the given node.
		/// </summary>
		/// <param name="node"> XML node that contains the cell data. </param>
		/// <param name="restoreStructures"> Boolean indicating whether the graph
		/// structure should be restored by calling insert and insertEdge on the
		/// parent and terminals, respectively. </param>
		/// <returns> Graph cell that represents the given node. </returns>
		public virtual mxICell decodeCell(Node node, bool restoreStructures)
		{
			mxICell cell = null;

			if (node != null && node.NodeType ==XmlNodeType.Element)
			{
				// Tries to find a codec for the given node name. If that does
				// not return a codec then the node is the user object (an XML node
				// that contains the mxCell, aka inversion).
				mxObjectCodec decoder = mxCodecRegistry.getCodec(node.Name);

				// Tries to find the codec for the cell inside the user object.
				// This assumes all node names inside the user object are either
				// not registered or they correspond to a class for cells.
				if (!(decoder is mxCellCodec))
				{
					Node child = node.FirstChild;

					while (child != null && !(decoder is mxCellCodec))
					{
						decoder = mxCodecRegistry.getCodec(child.Name);
						child = child.NextSibling;
					}

					string name = typeof(mxCell).Name;
					decoder = mxCodecRegistry.getCodec(name);
				}

				if (!(decoder is mxCellCodec))
				{
					string name = typeof(mxCell).Name;
					decoder = mxCodecRegistry.getCodec(name);
				}

				cell = (mxICell) decoder.decode(this, node);

				if (restoreStructures)
				{
					insertIntoGraph(cell);
				}
			}

			return cell;
		}

		/// <summary>
		/// Inserts the given cell into its parent and terminal cells.
		/// </summary>
		public virtual void insertIntoGraph(mxICell cell)
		{
			mxICell parent = cell.Parent;
			mxICell source = cell.getTerminal(true);
			mxICell target = cell.getTerminal(false);

			// Fixes possible inconsistencies during insert into graph
			cell.setTerminal(null, false);
			cell.setTerminal(null, true);
			cell.Parent = null;

			if (parent != null)
			{
				parent.insert(cell);
			}

			if (source != null)
			{
				source.insertEdge(cell, true);
			}

			if (target != null)
			{
				target.insertEdge(cell, false);
			}
		}

		/// <summary>
		/// Sets the attribute on the specified node to value. This is a
		/// helper method that makes sure the attribute and value arguments
		/// are not null.
		/// </summary>
		/// <param name="node"> XML node to set the attribute for. </param>
		/// <param name="attribute"> Name of the attribute whose value should be set. </param>
		/// <param name="value"> New value of the attribute. </param>
		public static void setAttribute(Node node, string attribute, object value)
		{
			if (node.NodeType == XmlNodeType.Element && !string.ReferenceEquals(attribute, null) && value != null)
			{
                ((Element) node).SetAttribute(attribute, value.ToString());
			}
		}

	}

}