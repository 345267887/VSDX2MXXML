using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using mxConstants = mxGraph.util.mxConstants;
	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	/// <summary>
	/// Represents a Data element in the GML Structure.
	/// </summary>
	public class mxGraphMlEdge
	{
		private string edgeId;

		private string edgeSource;

		private string edgeSourcePort;

		private string edgeTarget;

		private string edgeTargetPort;

		private string edgeDirected;

		private mxGraphMlData edgeData;

		/// <summary>
		/// Map with the data. The key is the key attribute
		/// </summary>
		private Dictionary<string, mxGraphMlData> edgeDataMap = new Dictionary<string, mxGraphMlData>();

		/// <summary>
		/// Construct an edge with source and target. </summary>
		/// <param name="edgeSource"> Source Node's ID. </param>
		/// <param name="edgeTarget"> Target Node's ID. </param>
		public mxGraphMlEdge(string edgeSource, string edgeTarget, string edgeSourcePort, string edgeTargetPort)
		{
			this.edgeId = "";
			this.edgeSource = edgeSource;
			this.edgeSourcePort = edgeSourcePort;
			this.edgeTarget = edgeTarget;
			this.edgeTargetPort = edgeTargetPort;
			this.edgeDirected = "";
		}

		/// <summary>
		/// Construct an edge from a xml edge element. </summary>
		/// <param name="edgeElement"> Xml edge element. </param>
		public mxGraphMlEdge(Element edgeElement)
		{
            this.edgeId = edgeElement.GetAttribute(mxGraphMlConstants.ID);
			this.edgeSource = edgeElement.GetAttribute(mxGraphMlConstants.EDGE_SOURCE);
			this.edgeSourcePort = edgeElement.GetAttribute(mxGraphMlConstants.EDGE_SOURCE_PORT);
			this.edgeTarget = edgeElement.GetAttribute(mxGraphMlConstants.EDGE_TARGET);
			this.edgeTargetPort = edgeElement.GetAttribute(mxGraphMlConstants.EDGE_TARGET_PORT);
			this.edgeDirected = edgeElement.GetAttribute(mxGraphMlConstants.EDGE_DIRECTED);

			IList<Element> dataList = mxGraphMlUtils.childsTags(edgeElement, mxGraphMlConstants.DATA);

			foreach (Element dataElem in dataList)
			{
				mxGraphMlData data = new mxGraphMlData(dataElem);
				string key = data.DataKey;
				edgeDataMap[key] = data;
			}
		}

		public virtual string EdgeDirected
		{
			get
			{
				return edgeDirected;
			}
			set
			{
				this.edgeDirected = value;
			}
		}


		public virtual string EdgeId
		{
			get
			{
				return edgeId;
			}
			set
			{
				this.edgeId = value;
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


		public virtual string EdgeSourcePort
		{
			get
			{
				return edgeSourcePort;
			}
			set
			{
				this.edgeSourcePort = value;
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


		public virtual string EdgeTargetPort
		{
			get
			{
				return edgeTargetPort;
			}
			set
			{
				this.edgeTargetPort = value;
			}
		}


		public virtual Dictionary<string, mxGraphMlData> EdgeDataMap
		{
			get
			{
				return edgeDataMap;
			}
			set
			{
				this.edgeDataMap = value;
			}
		}


		public virtual mxGraphMlData EdgeData
		{
			get
			{
				return edgeData;
			}
			set
			{
				this.edgeData = value;
			}
		}


		/// <summary>
		/// Generates a Edge Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
            Element edge = document.CreateElement(mxGraphMlConstants.EDGE);

			if (!edgeId.Equals(""))
			{
                edge.SetAttribute(mxGraphMlConstants.ID, edgeId);
			}
			edge.SetAttribute(mxGraphMlConstants.EDGE_SOURCE, edgeSource);
			edge.SetAttribute(mxGraphMlConstants.EDGE_TARGET, edgeTarget);

			if (!edgeSourcePort.Equals(""))
			{
				edge.SetAttribute(mxGraphMlConstants.EDGE_SOURCE_PORT, edgeSourcePort);
			}

			if (!edgeTargetPort.Equals(""))
			{
				edge.SetAttribute(mxGraphMlConstants.EDGE_TARGET_PORT, edgeTargetPort);
			}

			if (!edgeDirected.Equals(""))
			{
				edge.SetAttribute(mxGraphMlConstants.EDGE_DIRECTED, edgeDirected);
			}

			Element dataElement = edgeData.generateEdgeElement(document);
            edge.AppendChild(dataElement);

			return edge;
		}

		/// <summary>
		/// Returns if the edge has end arrow. </summary>
		/// <returns> style that indicates the end arrow type(CLASSIC or NONE). </returns>
		public virtual string EdgeStyle
		{
			get
			{
				string style = "";
				Dictionary<string, object> styleMap = new Dictionary<string, object>();
    
				//Defines style of the edge.
				if (edgeDirected.Equals("true"))
				{
					styleMap[mxConstants.STYLE_ENDARROW] = mxConstants.ARROW_CLASSIC;
    
					style = mxGraphMlUtils.getStyleString(styleMap, "=");
				}
				else if (edgeDirected.Equals("false"))
				{
					styleMap[mxConstants.STYLE_ENDARROW] = mxConstants.NONE;
    
					style = mxGraphMlUtils.getStyleString(styleMap, "=");
				}
    
				return style;
			}
		}
	}

}