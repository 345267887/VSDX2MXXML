/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{
    using System.Xml;
    using Document =System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	/// <summary>
	/// This class represents the properties of a JGraph edge.
	/// </summary>
	public class mxGraphMlShapeEdge
	{
		private string text = "";

		private string style = "";

		private string edgeSource;

		private string edgeTarget;

		/// <summary>
		/// Construct a Shape Edge with text and style. </summary>
		/// <param name="text"> </param>
		/// <param name="style"> </param>
		public mxGraphMlShapeEdge(string text, string style)
		{
			this.text = text;
			this.style = style;
		}

		/// <summary>
		/// Constructs a ShapeEdge from a xml shapeEdgeElement. </summary>
		/// <param name="shapeEdgeElement"> </param>
		public mxGraphMlShapeEdge(Element shapeEdgeElement)
		{
			Element labelElement = mxGraphMlUtils.childsTag(shapeEdgeElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.LABEL);

			if (labelElement != null)
			{
                this.text = labelElement.GetAttribute(mxGraphMlConstants.TEXT);
			}

			Element styleElement = mxGraphMlUtils.childsTag(shapeEdgeElement, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.STYLE);

			if (styleElement != null)
			{
                this.style = styleElement.GetAttribute(mxGraphMlConstants.PROPERTIES);

			}
		}

		/// <summary>
		/// Construct an empty Shape Edge Element.
		/// </summary>
		public mxGraphMlShapeEdge()
		{
		}

		/// <summary>
		/// Generates a ShapeEdge Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
            Element dataEdge = document.CreateElement( mxGraphMlConstants.JGRAPH + mxGraphMlConstants.SHAPEEDGE, mxGraphMlConstants.JGRAPH_URL);//document.createElementNS(mxGraphMlConstants.JGRAPH_URL, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.SHAPEEDGE);



            if (!this.text.Equals(""))
			{
				Element dataEdgeLabel = document.CreateElement(mxGraphMlConstants.JGRAPH + mxGraphMlConstants.LABEL,mxGraphMlConstants.JGRAPH_URL); ;//document.createElementNS(mxGraphMlConstants.JGRAPH_URL, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.LABEL);
                dataEdgeLabel.SetAttribute(mxGraphMlConstants.TEXT, this.text);
                dataEdge.AppendChild(dataEdgeLabel);
			}

			if (!this.style.Equals(""))
			{
				Element dataEdgeStyle = document.CreateElement(mxGraphMlConstants.JGRAPH + mxGraphMlConstants.STYLE,mxGraphMlConstants.JGRAPH_URL); ;//document.createElementNS(mxGraphMlConstants.JGRAPH_URL, mxGraphMlConstants.JGRAPH + mxGraphMlConstants.STYLE);

                dataEdgeStyle.SetAttribute(mxGraphMlConstants.PROPERTIES, this.style);
                dataEdge.AppendChild(dataEdgeStyle);
			}

			return dataEdge;
		}

		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				this.text = value;
			}
		}


		public virtual string Style
		{
			get
			{
				return style;
			}
			set
			{
				this.style = value;
			}
		}


		public virtual string EdgeSource
		{
			get
			{
				return edgeSource;
			}
			set
			{
				this.edgeSource = value;
			}
		}


		public virtual string EdgeTarget
		{
			get
			{
				return edgeTarget;
			}
			set
			{
				this.edgeTarget = value;
			}
		}

	}

}