using System;
using System.Collections.Generic;

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace mxGraph.io.vsdx
{

    using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;

	/// <summary>
	/// This class is a wrapper for a Master element.<br/>
	/// Contains a map with the shapes contained in the Master element
	/// and allows access these by ID.
	/// </summary>
	public class mxVsdxMaster
	{
		protected internal Element master;

		/// <summary>
		/// Unique ID of the element within its parent element
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal string Id_Renamed = null;

		protected internal Shape masterShape = null;

		/*
		 * Map that contains the shapes in Master element wrapped for instances of mxDelegateShape.
		 * The key is the shape's ID.
		 */
		protected internal Dictionary<string, Shape> childShapes = new Dictionary<string, Shape>();

		/// <summary>
		/// Create a new instance of mxMasterElement and retrieves all the shapes contained
		/// in the Master element. </summary>
		/// <param name="m"> Master Element to be wrapped. </param>
		public mxVsdxMaster(Element m, mxVsdxModel model)
		{
			this.master = m;
            this.Id_Renamed = m.GetAttribute(mxVsdxConstants.ID);
			processMasterShapes(model);
		}

		/// <summary>
		/// Retrieves and wraps all the shapes contained in the 'shape' param.<br/>
		/// This method is recursive, it retrieves the subshapes of the shapes too. </summary>
		/// <param name="shape"> Shape from which the subshapes are retrieved. </param>
		/// <returns> Map with the shapes wrapped in instances of mxMasterShape. </returns>
		protected internal virtual void processMasterShapes(mxVsdxModel model)
		{
			Node child = this.master.FirstChild;

			while (child != null)
			{
				if (child is Element && ((Element)child).Name.Equals("Rel"))
				{
                    Element relElem = model.getRelationship(((Element) child).GetAttribute("r:id"), mxVsdxCodec.vsdxPlaceholder + "/masters/" + "_rels/masters.xml.rels");
                    string target = relElem.GetAttribute("Target");
                    string type = relElem.GetAttribute("Type");
					Document masterDoc = null;

					if (!string.ReferenceEquals(type, null) && type.EndsWith("master", StringComparison.Ordinal))
					{
						masterDoc = model.getXmlDoc(mxVsdxCodec.vsdxPlaceholder + "/masters/" + target);
					}

					if (masterDoc != null)
					{
						Node masterChild = masterDoc.FirstChild;

						while (masterChild != null)
						{
							if (masterChild is Element && ((Element)masterChild).Name.Equals("MasterContents"))
							{
								processMasterShape((Element)masterChild, model);
								break;
							}

							masterChild = masterChild.NextSibling;
						}
					}
				}

				child = child.NextSibling;
			}
		}

		/// <summary>
		/// Retrieves and wraps all the shapes contained in the 'shape' param.<br/>
		/// This method is recursive, it retrieves the subshapes of the shapes too. </summary>
		/// <param name="shape"> Shape from which the subshapes are retrieved. </param>
		/// <returns> Map with the shapes wrapped in instances of mxMasterShape. </returns>
		protected internal virtual void processMasterShape(Element shapeElem, mxVsdxModel model)
		{
			Node shapeChild = shapeElem.FirstChild;

			while (shapeChild != null)
			{
				if (shapeChild is Element && ((Element)shapeChild).Name.Equals("Shapes"))
				{
					Node shapesChild = shapeChild.FirstChild;

					while (shapesChild != null)
					{
						if (shapesChild is Element && ((Element)shapesChild).Name.Equals("Shape"))
						{
							Element shape = (Element)shapesChild;
                            string shapeId = shape.GetAttribute("ID");
							Shape masterShape = new Shape(shape, model);
							this.masterShape = (this.masterShape == null) ? masterShape : this.masterShape;
							childShapes[shapeId] = masterShape;
							processMasterShape(shape, model);
						}

						shapesChild = shapesChild.NextSibling;
					}

					break;
				}

				shapeChild = shapeChild.NextSibling;
			}
		}

		/// <summary>
		/// Returns the first shape in the Master </summary>
		/// <returns> First shape in the Master wrapped in a instance of mxMasterShape </returns>
		public virtual Shape MasterShape
		{
			get
			{
				return this.masterShape;
			}
		}

		/// <summary>
		/// Returns the shape in the master element with ID = 'id'. </summary>
		/// <param name="id"> Shape's ID </param>
		/// <returns> The shape in the master element with ID = 'id' wrapped in a instance of mxMasterShape </returns>
		public virtual Shape getSubShape(string id)
		{
			return childShapes[id];
		}

		/// <summary>
		/// Returns the NameU attribute. </summary>
		/// <returns> Value of the NameU attribute. </returns>
		public virtual string NameU
		{
			get
			{
                return master.GetAttribute("NameU");
			}
		}

		/// <summary>
		/// Returns the NameU attribute. </summary>
		/// <returns> Value of the NameU attribute. </returns>
		public virtual string Name
		{
			get
			{
				return master.GetAttribute("Name");
			}
		}

		/// <summary>
		/// Returns the UniqueID attribute. </summary>
		/// <returns> Value of the UniqueID attribute. </returns>
		public virtual string UniqueID
		{
			get
			{
				string uniqueID = "";

                if (master.HasAttribute("UniqueID"))
				{
					uniqueID = master.GetAttribute("UniqueID");
				}
    
				return uniqueID;
			}
		}

		public virtual string Id
		{
			get
			{
				return this.Id_Renamed;
			}
		}

		public virtual Element MasterElement
		{
			get
			{
				return master;
			}
		}
	}

}