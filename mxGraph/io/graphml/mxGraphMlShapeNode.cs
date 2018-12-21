/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	public class mxGraphMlShapeNode
	{
		private string dataHeight = "";

		private string dataWidth = "";

		private string dataX = "";

		private string dataY = "";

		private string dataLabel = "";

		private string dataStyle = "";

		/// <summary>
		/// Construct a shape Node with the given parameters </summary>
		/// <param name="dataHeight"> Node's Height </param>
		/// <param name="dataWidth"> Node's Width </param>
		/// <param name="dataX"> Node's X coordinate. </param>
		/// <param name="dataY"> Node's Y coordinate. </param>
		/// <param name="dataStyle"> Node's style. </param>
		public mxGraphMlShapeNode(string dataHeight, string dataWidth, string dataX, string dataY, string dataStyle)
		{
			this.dataHeight = dataHeight;
			this.dataWidth = dataWidth;
			this.dataX = dataX;
			this.dataY = dataY;
			this.dataStyle = dataStyle;
		}

		/// <summary>
		/// Construct an empty shape Node
		/// </summary>
		public mxGraphMlShapeNode()
		{
		}

		/// <summary>
		/// Construct a Shape Node from a xml Shape Node Element. </summary>
		/// <param name="shapeNodeElement"> Xml Shape Node Element. </param>
		public mxGraphMlShapeNode(Element shapeNodeElement)
		{
			//Defines Geometry
			Element geometryElement = mxGraphMlUtils.childsTag(shapeNodeElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.GEOMETRY);
            this.dataHeight = geometryElement.GetAttribute(mxGraphMlConstants.HEIGHT);
			this.dataWidth = geometryElement.GetAttribute(mxGraphMlConstants.WIDTH);
			this.dataX = geometryElement.GetAttribute(mxGraphMlConstants.X);
			this.dataY = geometryElement.GetAttribute(mxGraphMlConstants.Y);

			Element styleElement = mxGraphMlUtils.childsTag(shapeNodeElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.STYLE);

			if (styleElement != null)
			{
				this.dataStyle = styleElement.GetAttribute(mxGraphMlConstants.PROPERTIES);
			}
			//Defines Label
			Element labelElement = mxGraphMlUtils.childsTag(shapeNodeElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.LABEL);

			if (labelElement != null)
			{
				this.dataLabel = labelElement.GetAttribute(mxGraphMlConstants.TEXT);
			}
		}

		/// <summary>
		/// Generates a Shape Node Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
			Element dataShape = document.CreateElement(mxGraphMlConstants.JGRAPH + mxGraphMlConstants.SHAPENODE,mxGraphMlConstants.JGRAPH_URL);

			Element dataShapeGeometry = document.CreateElement(mxGraphMlConstants.JGRAPH + mxGraphMlConstants.GEOMETRY,mxGraphMlConstants.JGRAPH_URL);
            dataShapeGeometry.SetAttribute(mxGraphMlConstants.HEIGHT, dataHeight);
			dataShapeGeometry.SetAttribute(mxGraphMlConstants.WIDTH, dataWidth);
			dataShapeGeometry.SetAttribute(mxGraphMlConstants.X, dataX);
			dataShapeGeometry.SetAttribute(mxGraphMlConstants.Y, dataY);

            dataShape.AppendChild(dataShapeGeometry);

			if (!this.dataStyle.Equals(""))
			{
				Element dataShapeStyle = document.CreateElement(mxGraphMlConstants.JGRAPH + mxGraphMlConstants.STYLE,mxGraphMlConstants.JGRAPH_URL);
				dataShapeStyle.SetAttribute(mxGraphMlConstants.PROPERTIES, dataStyle);
                dataShape.AppendChild(dataShapeStyle);
			}

			//Sets Label
			if (!this.dataLabel.Equals(""))
			{

				Element dataShapeLabel = document.CreateElement(mxGraphMlConstants.JGRAPH + mxGraphMlConstants.LABEL,mxGraphMlConstants.JGRAPH_URL);
                dataShapeLabel.SetAttribute(mxGraphMlConstants.TEXT, dataLabel);

                dataShape.AppendChild(dataShapeLabel);
			}

			return dataShape;
		}

		public virtual string DataHeight
		{
			get
			{
				return dataHeight;
			}
			set
			{
				this.dataHeight = value;
			}
		}


		public virtual string DataWidth
		{
			get
			{
				return dataWidth;
			}
			set
			{
				this.dataWidth = value;
			}
		}


		public virtual string DataX
		{
			get
			{
				return dataX;
			}
			set
			{
				this.dataX = value;
			}
		}


		public virtual string DataY
		{
			get
			{
				return dataY;
			}
			set
			{
				this.dataY = value;
			}
		}


		public virtual string DataLabel
		{
			get
			{
				return dataLabel;
			}
			set
			{
				this.dataLabel = value;
			}
		}


		public virtual string DataStyle
		{
			get
			{
				return dataStyle;
			}
			set
			{
				this.dataStyle = value;
			}
		}

	}

}