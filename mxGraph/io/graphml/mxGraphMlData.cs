using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;
	using NodeList = System.Xml.XmlNodeList;

	/// <summary>
	/// Represents a Data element in the GML Structure.
	/// </summary>
	public class mxGraphMlData
	{
		private string dataId = "";

		private string dataKey = "";

		private string dataValue = ""; //not using

		private mxGraphMlShapeNode dataShapeNode;

		private mxGraphMlShapeEdge dataShapeEdge;

		/// <summary>
		/// Construct a data with the params values. </summary>
		/// <param name="dataId"> Data's ID </param>
		/// <param name="dataKey"> Reference to a Key Element ID </param>
		/// <param name="dataValue"> Value of the data Element </param>
		/// <param name="dataShapeEdge"> JGraph specific edge properties. </param>
		/// <param name="dataShapeNode"> JGraph specific node properties. </param>
		public mxGraphMlData(string dataId, string dataKey, string dataValue, mxGraphMlShapeEdge dataShapeEdge, mxGraphMlShapeNode dataShapeNode)
		{
			this.dataId = dataId;
			this.dataKey = dataKey;
			this.dataValue = dataValue;
			this.dataShapeNode = dataShapeNode;
			this.dataShapeEdge = dataShapeEdge;
		}

		/// <summary>
		/// Construct a data from one xml data element. </summary>
		/// <param name="dataElement"> Xml Data Element. </param>
		public mxGraphMlData(Element dataElement)
		{
            this.dataId = dataElement.GetAttribute(mxGraphMlConstants.ID);
			this.dataKey = dataElement.GetAttribute(mxGraphMlConstants.KEY);

			this.dataValue = "";

			Element shapeNodeElement = mxGraphMlUtils.childsTag(dataElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.SHAPENODE);
			Element shapeEdgeElement = mxGraphMlUtils.childsTag(dataElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.SHAPEEDGE);

			if (shapeNodeElement != null)
			{
				this.dataShapeNode = new mxGraphMlShapeNode(shapeNodeElement);
			}
			else if (shapeEdgeElement != null)
			{
				this.dataShapeEdge = new mxGraphMlShapeEdge(shapeEdgeElement);
			}
			else
			{
				NodeList childs = dataElement.ChildNodes;
				IList<Node> childrens = mxGraphMlUtils.copyNodeList(childs);

				foreach (Node n in childrens)
				{
					if (n.Name.Equals("#text"))
					{

						this.dataValue += n.Value;
					}
				}
				this.dataValue = this.dataValue.Trim();
			}
		}

		/// <summary>
		/// Construct an empty data.
		/// </summary>
		public mxGraphMlData()
		{
		}

		public virtual string DataId
		{
			get
			{
				return dataId;
			}
			set
			{
				this.dataId = value;
			}
		}


		public virtual string DataKey
		{
			get
			{
				return dataKey;
			}
			set
			{
				this.dataKey = value;
			}
		}


		public virtual string DataValue
		{
			get
			{
				return dataValue;
			}
			set
			{
				this.dataValue = value;
			}
		}


		public virtual mxGraphMlShapeNode DataShapeNode
		{
			get
			{
				return dataShapeNode;
			}
			set
			{
				this.dataShapeNode = value;
			}
		}


		public virtual mxGraphMlShapeEdge DataShapeEdge
		{
			get
			{
				return dataShapeEdge;
			}
			set
			{
				this.dataShapeEdge = value;
			}
		}


		/// <summary>
		/// Generates an Node Data Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateNodeElement(Document document)
		{
            Element data = document.CreateElement(mxGraphMlConstants.DATA);
            data.SetAttribute(mxGraphMlConstants.KEY, dataKey);

			Element shapeNodeElement = dataShapeNode.generateElement(document);
            data.AppendChild(shapeNodeElement);

			return data;
		}

		/// <summary>
		/// Generates an Edge Data Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateEdgeElement(Document document)
		{
            Element data = document.CreateElement(mxGraphMlConstants.DATA);
            data.SetAttribute(mxGraphMlConstants.KEY, dataKey);

			Element shapeEdgeElement = dataShapeEdge.generateElement(document);
            data.AppendChild(shapeEdgeElement);

			return data;
		}
	}

}