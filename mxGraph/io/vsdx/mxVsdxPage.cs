using System;
using System.Collections.Generic;

namespace mxGraph.io.vsdx
{


    using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;
    using NodeList = System.Xml.XmlNodeList;

	using mxPoint = mxGraph.util.mxPoint;

	public class mxVsdxPage
	{

		/// <summary>
		/// Unique ID of the element within its parent element
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		protected internal int? Id_Renamed = null;

		/// <summary>
		/// Name of the page taken from the "name" attribute of the page element
		/// </summary>
		protected internal string pageName = null;

		protected internal bool isBackground = false;

		protected internal int? backPageId = null;

		protected internal mxVsdxPage backPage = null;

		protected internal Element pageElement = null;

		protected internal Element pageSheet = null;

		protected internal mxVsdxModel model = null;

		protected internal IDictionary<int?, VsdxShape> shapes = new Dictionary<int?, VsdxShape>();

		protected internal IDictionary<int?, mxVsdxConnect> connects = new Dictionary<int?, mxVsdxConnect>();

		// cell in the PageSheet
		protected internal IDictionary<string, Element> cellElements = new Dictionary<string, Element>();

		public mxVsdxPage(Element pageElem, mxVsdxModel model)
		{
			this.model = model;
			this.pageElement = pageElem;

            string backGround = pageElem.GetAttribute(mxVsdxConstants.BACKGROUND);
			this.isBackground = (!string.ReferenceEquals(backGround, null) && backGround.Equals(mxVsdxConstants.TRUE)) ? true : false;
			string back = pageElem.GetAttribute(mxVsdxConstants.BACK_PAGE);

			if (!isBackground && !string.ReferenceEquals(back, null) && back.Length > 0)
			{
				this.backPageId = Convert.ToInt32(back);
			}

			this.Id_Renamed = Convert.ToInt32(pageElem.GetAttribute(mxVsdxConstants.ID));
			this.pageName = pageElem.GetAttribute(mxVsdxConstants.NAME);

			List<Element> pageSheets = mxVsdxUtils.getDirectChildNamedElements(pageElem, "PageSheet");

			if (pageSheets.Count > 0)
			{
				Element pageSheet = pageSheets[0];
				List<Element> cells = mxVsdxUtils.getDirectChildNamedElements(pageSheet, "Cell");

				foreach (Element cellElem in cells)
				{
					string n = cellElem.GetAttribute("N");
					this.cellElements[n] = cellElem;
				}
			}

			parseNodes(pageElem, model, "pages");
		}

		/// <summary>
		/// Parses the child nodes of the given element </summary>
		/// <param name="pageElem"> the parent whose children to parse </param>
		/// <param name="model"> the model of the vsdx file </param>
		/// <param name="pageName"> page information is split across pages.xml and pageX.xml where X is any number. We have to know which we're currently parsing to use the correct relationships file. </param>
		protected internal virtual void parseNodes(Node pageElem, mxVsdxModel model, string pageName)
		{
			Node pageChild = pageElem.FirstChild;

			while (pageChild != null)
			{
				if (pageChild is Element)
				{
					Element pageChildElem = (Element) pageChild;
					string childName = pageChildElem.Name;

					if (childName.Equals("Rel"))
					{
						resolveRel(pageChildElem, model, pageName);
					}
					else if (childName.Equals("Shapes"))
					{
						this.shapes = parseShapes(pageChildElem, null, false);
					}
					else if (childName.Equals("Connects"))
					{
                        NodeList connectList = pageChildElem.GetElementsByTagName(mxVsdxConstants.CONNECT);
                        Node connectNode = (connectList != null && connectList.Count > 0) ? connectList.Item(0) : null;
						//mxVdxConnect currentConnect = null;

						while (connectNode != null)
						{
							if (connectNode is Element)
							{
								Element connectElem = (Element) connectNode;
								mxVsdxConnect connect = new mxVsdxConnect(connectElem);
								int? fromSheet = connect.FromSheet;
								mxVsdxConnect previousConnect = (fromSheet != null && fromSheet > -1) ? connects[fromSheet] : null;

								if (previousConnect != null)
								{
									previousConnect.addConnect(connectElem);
								}
								else
								{
									connects[connect.FromSheet] = connect;
								}
							}

							connectNode = connectNode.NextSibling;
						}
					}
					else if (childName.Equals("PageSheet"))
					{
						this.pageSheet = pageChildElem;
					}
				}

				pageChild = pageChild.NextSibling;
			}
		}

		/// 
		/// <param name="relNode"> </param>
		/// <param name="model"> </param>
		/// <param name="pageName"> </param>
		protected internal virtual void resolveRel(Element relNode, mxVsdxModel model, string pageName)
		{
			Element relElem = model.getRelationship(relNode.GetAttribute("r:id"), mxVsdxCodec.vsdxPlaceholder + "/pages/" + "_rels/" + pageName + ".xml.rels");

			string target = relElem.GetAttribute("Target");
			string type = relElem.GetAttribute("Type");

			if (type.ToString().EndsWith("page", StringComparison.Ordinal))
			{
				Document pageDoc = null;

				if (!string.ReferenceEquals(type, null) && type.EndsWith("page", StringComparison.Ordinal))
				{
					pageDoc = model.getXmlDoc(mxVsdxCodec.vsdxPlaceholder + "/pages/" + target);
				}

				if (pageDoc != null)
				{
					Node child = pageDoc.FirstChild;

					while (child != null)
					{
						if (child is Element && ((Element)child).Name.Equals("PageContents"))
						{
							int index = target.IndexOf('.');

							if (index != -1)
							{
								parseNodes(child, model, target.Substring(0, index));
							}

							break;
						}

						child = child.NextSibling;
					}
				}
			}
		}

		public virtual IDictionary<int?, VsdxShape> parseShapes(Element shapesElement, mxVsdxMaster master, bool recurse)
		{
			IDictionary<int?, VsdxShape> shapes = new Dictionary<int?, VsdxShape>();
            NodeList shapeList = shapesElement.GetElementsByTagName(mxVsdxConstants.SHAPE);

            Node shapeNode = (shapeList != null && shapeList.Count > 0) ? shapeList.Item(0) : null;

			while (shapeNode != null)
			{
				if (shapeNode is Element)
				{
					Element shapeElem = (Element) shapeNode;
					mxVsdxMaster masterTmp = master;

					// Work out node type
					if (masterTmp == null)
					{
						//If the shape has the Master attribute the master shape is the first
						//shape of the master element.
						string masterId = shapeElem.GetAttribute(mxVsdxConstants.MASTER);

						if (!string.ReferenceEquals(masterId, null) && !masterId.Equals(""))
						{
							masterTmp = model.getMaster(masterId);
						}
					}

					bool _isEdge = isEdge(shapeElem);

					// If the master of the shape has an xform1D, it's an edge
					if (!_isEdge && masterTmp != null)
					{
                        string masterId = shapeElem.GetAttribute(mxVsdxConstants.MASTER_SHAPE);

						Element elem = masterTmp.MasterElement;
						if (!string.ReferenceEquals(masterId, null) && !masterId.Equals(""))
						{
							elem = masterTmp.getSubShape(masterId).Shape;
						}
                        _isEdge = isEdge(elem);
					}

					//String type = mxVdxShape.getType(shapeElem);

					VsdxShape shape = this.createCell(shapeElem, !_isEdge, masterTmp);

					shapes[shape.Id] = shape;
				}

				shapeNode = shapeNode.NextSibling;
			}

			return shapes;
		}

		protected internal virtual VsdxShape createCell(Element shapeElem, bool vertex, mxVsdxMaster masterTmp)
		{
			return new VsdxShape(this, shapeElem, vertex, this.model.MasterShapes, masterTmp, this.model);
		}

		public virtual bool isEdge(Element shape)
		{
			if (shape != null)
			{
				NodeList children = shape.ChildNodes;

				if (children != null)
				{
                    Node childNode = children.Item(0);

					while (childNode != null)
					{
						if (childNode is Element)
						{
							Element childElem = (Element) childNode;

							if (childElem.Name.Equals("Cell"))
							{
                                string n = childElem.GetAttribute("N");

								if (n.Equals("BeginX") || n.Equals("BeginY") || n.Equals("EndY") || n.Equals("EndX"))
								{
									return true;
								}
							}
						}

						childNode = childNode.NextSibling;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Returns the width and height of a Page expressed as an mxPoint. </summary>
		/// <returns> mxPoint that represents the dimensions of the page </returns>
		public virtual mxPoint PageDimensions
		{
			get
			{
				double pageH = 0;
				double pageW = 0;
    
				Element height = this.cellElements.ContainsKey("PageHeight") ? this.cellElements["PageHeight"]:null;
				Element width = this.cellElements.ContainsKey("PageWidth") ? this.cellElements["PageWidth"]:null;
    
				if (height != null)
				{
                    pageH = Convert.ToDouble(height.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
					pageH = Math.Round(pageH * 100.0) / 100.0;
				}
    
				if (width != null)
				{
                    pageW = Convert.ToDouble(width.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
					pageW = Math.Round(pageW * 100.0) / 100.0;
				}
    
				return new mxPoint(pageW, pageH);
			}
		}

		/// <summary>
		/// Returns the drawing scale attribute of this page </summary>
		/// <returns> the DrawingScale </returns>
		public virtual double DrawingScale
		{
			get
			{
				Element scale = this.cellElements.ContainsKey("DrawingScale") ? this.cellElements["DrawingScale"]:null; 
    
				if (scale != null)
				{
                    return Convert.ToDouble(scale.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
				}
    
				return 1;
			}
		}


		/// <summary>
		/// Returns the page scale attribute of this page </summary>
		/// <returns> the PageScale </returns>
		public virtual double PageScale
		{
			get
			{
				Element scale = this.cellElements.ContainsKey("PageScale")? this.cellElements["PageScale"]:null;
    
				if (scale != null)
				{
                    return Convert.ToDouble(scale.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
				}
    
				return 1;
			}
		}

		public virtual string getCellValue(string cellName)
		{
			Element cell = this.cellElements.ContainsKey(cellName)? this.cellElements[cellName]:null;

			if (cell != null)
			{
                return cell.GetAttribute("V");
			}

			return null;
		}

		public virtual int getCellIntValue(string cellName, int defVal)
		{
			string val = getCellValue(cellName);

			if (!string.ReferenceEquals(val, null))
			{
				return int.Parse(val);
			}

			return defVal;
		}

		/// <summary>
		/// Returns the ID of the page </summary>
		/// <returns> the ID of the page </returns>
		public virtual int? Id
		{
			get
			{
				return this.Id_Renamed;
			}
		}
		public virtual string PageName
		{
			get
			{
				return this.pageName;
			}
		}

		public virtual IDictionary<int?, VsdxShape> Shapes
		{
			get
			{
				return this.shapes;
			}
		}

		public virtual IDictionary<int?, mxVsdxConnect> Connects
		{
			get
			{
				return this.connects;
			}
		}

		public virtual bool Background
		{
			get
			{
				return this.isBackground;
			}
		}

		/// <summary>
		/// Returns the background page ID, if any </summary>
		/// <returns> the ID of any background page or null for no background page </returns>
		public virtual int? BackPageId
		{
			get
			{
				return this.backPageId;
			}
		}

		public virtual mxVsdxPage BackPage
		{
			set
			{
				this.backPage = value;
			}
			get
			{
				return this.backPage;
			}
		}

	}

}