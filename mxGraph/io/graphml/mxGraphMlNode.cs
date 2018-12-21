using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Document = System.Xml.XmlDocument;
	using Element =System.Xml.XmlElement;

	/// <summary>
	/// Represents a Data element in the GML Structure.
	/// </summary>
	public class mxGraphMlNode
	{
		private string nodeId;

		private mxGraphMlData nodeData;

		private IList<mxGraphMlGraph> nodeGraphList = new List<mxGraphMlGraph>();

		private Dictionary<string, mxGraphMlData> nodeDataMap = new Dictionary<string, mxGraphMlData>();

		private Dictionary<string, mxGraphMlPort> nodePortMap = new Dictionary<string, mxGraphMlPort>();

		/// <summary>
		/// Construct a node with Id and one data element </summary>
		/// <param name="nodeId"> Node`s ID </param>
		/// <param name="nodeData"> Gml Data. </param>
		public mxGraphMlNode(string nodeId, mxGraphMlData nodeData)
		{
			this.nodeId = nodeId;
			this.nodeData = nodeData;
		}

		/// <summary>
		/// Construct a Node from a xml Node Element. </summary>
		/// <param name="nodeElement"> Xml Node Element. </param>
		public mxGraphMlNode(Element nodeElement)
		{
            this.nodeId = nodeElement.GetAttribute(mxGraphMlConstants.ID);

			//Add data elements
			IList<Element> dataList = mxGraphMlUtils.childsTags(nodeElement, mxGraphMlConstants.DATA);

			foreach (Element dataElem in dataList)
			{
				mxGraphMlData data = new mxGraphMlData(dataElem);
				string key = data.DataKey;
				nodeDataMap[key] = data;
			}

			//Add graph elements
			IList<Element> graphList = mxGraphMlUtils.childsTags(nodeElement, mxGraphMlConstants.GRAPH);

			foreach (Element graphElem in graphList)
			{
				mxGraphMlGraph graph = new mxGraphMlGraph(graphElem);
				nodeGraphList.Add(graph);
			}

			//Add port elements
			IList<Element> portList = mxGraphMlUtils.childsTags(nodeElement, mxGraphMlConstants.PORT);

			foreach (Element portElem in portList)
			{
				mxGraphMlPort port = new mxGraphMlPort(portElem);
				string name = port.Name;
				nodePortMap[name] = port;
			}
		}

		public virtual string NodeId
		{
			get
			{
				return nodeId;
			}
			set
			{
				this.nodeId = value;
			}
		}


		public virtual Dictionary<string, mxGraphMlData> NodeDataMap
		{
			get
			{
				return nodeDataMap;
			}
			set
			{
				this.nodeDataMap = value;
			}
		}


		public virtual IList<mxGraphMlGraph> NodeGraph
		{
			get
			{
				return nodeGraphList;
			}
			set
			{
				this.nodeGraphList = value;
			}
		}


		public virtual Dictionary<string, mxGraphMlPort> NodePort
		{
			get
			{
				return nodePortMap;
			}
			set
			{
				this.nodePortMap = value;
			}
		}


		/// <summary>
		/// Generates a Key Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
            Element node = document.CreateElement(mxGraphMlConstants.NODE);

            node.SetAttribute(mxGraphMlConstants.ID, nodeId);

			Element dataElement = nodeData.generateNodeElement(document);
            node.AppendChild(dataElement);

			foreach (mxGraphMlPort port in nodePortMap.Values)
			{
				Element portElement = port.generateElement(document);
                node.AppendChild(portElement);
			}

			foreach (mxGraphMlGraph graph in nodeGraphList)
			{
				Element graphElement = graph.generateElement(document);
                node.AppendChild(graphElement);
			}

			return node;
		}

		public virtual mxGraphMlData NodeData
		{
			get
			{
				return nodeData;
			}
			set
			{
				this.nodeData = value;
			}
		}


	}

}