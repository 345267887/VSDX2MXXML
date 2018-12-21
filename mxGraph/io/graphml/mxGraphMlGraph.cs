using System;
using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using mxCell = mxGraph.model.mxCell;
	using mxPoint = mxGraph.util.mxPoint;
	using mxConnectionConstraint = mxGraph.view.mxConnectionConstraint;
	using mxGraph = mxGraph.view.mxGraph;
	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	/// <summary>
	/// Represents a Graph element in the GML Structure.
	/// </summary>
	public class mxGraphMlGraph
	{
		/// <summary>
		/// Map with the vertex cells added in the addNode method.
		/// </summary>
		private static Dictionary<string, object> cellsMap = new Dictionary<string, object>();

		private string id = "";

		private string edgedefault = "";

		private IList<mxGraphMlNode> nodes = new List<mxGraphMlNode>();

		private IList<mxGraphMlEdge> edges = new List<mxGraphMlEdge>();

		/// <summary>
		/// Constructs a graph with id and edge default direction. </summary>
		/// <param name="id"> Graph's ID </param>
		/// <param name="edgedefault"> Edge Default direction.("directed" or "undirected") </param>
		public mxGraphMlGraph(string id, string edgedefault)
		{
			this.id = id;
			this.edgedefault = edgedefault;
		}

		/// <summary>
		/// Constructs an empty graph.
		/// </summary>
		public mxGraphMlGraph()
		{
		}

		/// <summary>
		/// Constructs a graph from a xml graph element. </summary>
		/// <param name="graphElement"> Xml graph element. </param>
		public mxGraphMlGraph(Element graphElement)
		{
            this.id = graphElement.GetAttribute(mxGraphMlConstants.ID);
            this.edgedefault = graphElement.GetAttribute(mxGraphMlConstants.EDGE_DEFAULT);

			//Adds node elements
			IList<Element> nodeElements = mxGraphMlUtils.childsTags(graphElement, mxGraphMlConstants.NODE);

			foreach (Element nodeElem in nodeElements)
			{
				mxGraphMlNode node = new mxGraphMlNode(nodeElem);

				nodes.Add(node);
			}

			//Adds edge elements
			IList<Element> edgeElements = mxGraphMlUtils.childsTags(graphElement, mxGraphMlConstants.EDGE);

			foreach (Element edgeElem in edgeElements)
			{
				mxGraphMlEdge edge = new mxGraphMlEdge(edgeElem);

				if (edge.EdgeDirected.Equals(""))
				{
					if (edgedefault.Equals(mxGraphMlConstants.EDGE_DIRECTED))
					{
						edge.EdgeDirected = "true";
					}
					else if (edgedefault.Equals(mxGraphMlConstants.EDGE_UNDIRECTED))
					{
						edge.EdgeDirected = "false";
					}
				}

				edges.Add(edge);
			}
		}

		/// <summary>
		/// Adds the elements represented for this graph model into the given graph. </summary>
		/// <param name="graph"> Graph where the elements will be located </param>
		/// <param name="parent"> Parent of the cells to be added. </param>
		public virtual void addGraph(mxGraph graph, object parent)
		{
			IList<mxGraphMlNode> nodeList = Nodes;

			foreach (mxGraphMlNode node in nodeList)
			{
				addNode(graph, parent, node);
			}
			IList<mxGraphMlEdge> edgeList = Edges;

			foreach (mxGraphMlEdge edge in edgeList)
			{
				addEdge(graph, parent, edge);
			}
		}

		/// <summary>
		/// Checks if the node has data elements inside. </summary>
		/// <param name="node"> Gml node element. </param>
		/// <returns> Returns <code>true</code> if the node has data elements inside. </returns>
		public static bool hasData(mxGraphMlNode node)
		{
			bool ret = false;
			if (node.NodeDataMap == null)
			{
				ret = false;
			}
			else
			{
				ret = true;
			}
			return ret;
		}

		/// <summary>
		/// Returns the data element inside the node that references to the key element
		/// with name = KEY_NODE_NAME. </summary>
		/// <param name="node"> Gml Node element. </param>
		/// <returns> The required data. null if not found. </returns>
		public static mxGraphMlData dataNodeKey(mxGraphMlNode node)
		{
			string keyId = "";
			Dictionary<string, mxGraphMlKey> keyMap = mxGraphMlKeyManager.Instance.KeyMap;

			foreach (mxGraphMlKey key in keyMap.Values)
			{
				if (key.KeyName.Equals(mxGraphMlConstants.KEY_NODE_NAME))
				{
					keyId = key.KeyId;
				}
			}

			mxGraphMlData data = null;
			Dictionary<string, mxGraphMlData> nodeDataMap = node.NodeDataMap;
			data = nodeDataMap[keyId];

			return data;
		}

		/// <summary>
		/// Returns the data element inside the edge that references to the key element
		/// with name = KEY_EDGE_NAME. </summary>
		/// <param name="edge"> Gml Edge element. </param>
		/// <returns> The required data. null if not found. </returns>
		public static mxGraphMlData dataEdgeKey(mxGraphMlEdge edge)
		{
			string keyId = "";
			Dictionary<string, mxGraphMlKey> keyMap = mxGraphMlKeyManager.Instance.KeyMap;
			foreach (mxGraphMlKey key in keyMap.Values)
			{
				if (key.KeyName.Equals(mxGraphMlConstants.KEY_EDGE_NAME))
				{
					keyId = key.KeyId;
				}
			}

			mxGraphMlData data = null;
			Dictionary<string, mxGraphMlData> nodeDataMap = edge.EdgeDataMap;
			data = nodeDataMap[keyId];

			return data;
		}

		/// <summary>
		/// Adds the vertex represented for the gml node into the graph with the given parent. </summary>
		/// <param name="graph"> Graph where the vertex will be added. </param>
		/// <param name="parent"> Parent's cell. </param>
		/// <param name="node"> Gml Node </param>
		/// <returns> The inserted Vertex cell. </returns>
		private mxCell addNode(mxGraph graph, object parent, mxGraphMlNode node)
		{
			mxCell v1;
			string id = node.NodeId;

			mxGraphMlData data = dataNodeKey(node);

			if (data != null && data.DataShapeNode != null)
			{
				double? x = Convert.ToDouble(data.DataShapeNode.DataX);
				double? y = Convert.ToDouble(data.DataShapeNode.DataY);
				double? h = Convert.ToDouble(data.DataShapeNode.DataHeight);
				double? w = Convert.ToDouble(data.DataShapeNode.DataWidth);
				string label = data.DataShapeNode.DataLabel;
				string style = data.DataShapeNode.DataStyle;
				v1 = (mxCell) graph.insertVertex(parent, id, label, x.Value, y.Value, w.Value, h.Value, style);
			}
			else
			{
				v1 = (mxCell) graph.insertVertex(parent, id, "", 0, 0, 100, 100);
			}

			cellsMap[id] = v1;
			IList<mxGraphMlGraph> graphs = node.NodeGraph;

			foreach (mxGraphMlGraph gmlGraph in graphs)
			{
				gmlGraph.addGraph(graph, v1);
			}
			return v1;
		}

		/// <summary>
		/// Returns the point represented for the port name.
		/// The specials names North, NorthWest, NorthEast, East, West, South, SouthEast and SouthWest.
		/// are accepted. Else, the values acepted follow the pattern "double,double".
		/// where double must be in the range 0..1 </summary>
		/// <param name="source"> Port Name. </param>
		/// <returns> point that represent the port value. </returns>
		private static mxPoint portValue(string source)
		{
			mxPoint fromConstraint = null;

			if (!string.ReferenceEquals(source, null) && !source.Equals(""))
			{

				if (source.Equals("North"))
				{
					fromConstraint = new mxPoint(0.5, 0);
				}
				else if (source.Equals("South"))
				{
					fromConstraint = new mxPoint(0.5, 1);

				}
				else if (source.Equals("East"))
				{
					fromConstraint = new mxPoint(1, 0.5);

				}
				else if (source.Equals("West"))
				{
					fromConstraint = new mxPoint(0, 0.5);

				}
				else if (source.Equals("NorthWest"))
				{
					fromConstraint = new mxPoint(0, 0);
				}
				else if (source.Equals("SouthWest"))
				{
					fromConstraint = new mxPoint(0, 1);
				}
				else if (source.Equals("SouthEast"))
				{
					fromConstraint = new mxPoint(1, 1);
				}
				else if (source.Equals("NorthEast"))
				{
					fromConstraint = new mxPoint(1, 0);
				}
				else
				{
					try
					{
						string[] s = source.Split(',');//source.Split(",", true);
                        double? x = Convert.ToDouble(s[0]);
						double? y = Convert.ToDouble(s[1]);
						fromConstraint = new mxPoint(x.Value, y.Value);
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
			}
			return fromConstraint;
		}

		/// <summary>
		/// Adds the edge represented for the gml edge into the graph with the given parent. </summary>
		/// <param name="graph"> Graph where the vertex will be added. </param>
		/// <param name="parent"> Parent's cell. </param>
		/// <param name="edge"> Gml Edge </param>
		/// <returns> The inserted edge cell. </returns>
		private static mxCell addEdge(mxGraph graph, object parent, mxGraphMlEdge edge)
		{
			//Get source and target vertex
			mxPoint fromConstraint = null;
			mxPoint toConstraint = null;
			object source = cellsMap[edge.EdgeSource];
			object target = cellsMap[edge.EdgeTarget];
			string sourcePort = edge.EdgeSourcePort;
			string targetPort = edge.EdgeTargetPort;

			fromConstraint = portValue(sourcePort);

			toConstraint = portValue(targetPort);

			mxGraphMlData data = dataEdgeKey(edge);

			string style = "";
			string label = "";

			if (data != null)
			{
				mxGraphMlShapeEdge shEdge = data.DataShapeEdge;
				style = shEdge.Style;
				label = shEdge.Text;
			}
			else
			{
				style = edge.EdgeStyle;
			}

			//Insert new edge.
			mxCell e = (mxCell) graph.insertEdge(parent, null, label, source, target, style);
			graph.setConnectionConstraint(e, source, true, new mxConnectionConstraint(fromConstraint, false));
			graph.setConnectionConstraint(e, target, false, new mxConnectionConstraint(toConstraint, false));
			return e;
		}

		public virtual string Edgedefault
		{
			get
			{
				return edgedefault;
			}
			set
			{
				this.edgedefault = value;
			}
		}


		public virtual string Id
		{
			get
			{
				return id;
			}
			set
			{
				this.id = value;
			}
		}


		public virtual IList<mxGraphMlNode> Nodes
		{
			get
			{
				return nodes;
			}
			set
			{
				this.nodes = value;
			}
		}


		public virtual IList<mxGraphMlEdge> Edges
		{
			get
			{
				return edges;
			}
			set
			{
				this.edges = value;
			}
		}


		/// <summary>
		/// Checks if the graph has child nodes or edges. </summary>
		/// <returns> Returns <code>true</code> if the graph hasn't child nodes or edges. </returns>
		public virtual bool Empty
		{
			get
			{
				return nodes.Count == 0 && edges.Count == 0;
			}
		}

		/// <summary>
		/// Generates a Key Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
            Element graph = document.CreateElement(mxGraphMlConstants.GRAPH);

			if (!id.Equals(""))
			{
                graph.SetAttribute(mxGraphMlConstants.ID, id);
			}
			if (!edgedefault.Equals(""))
			{
                graph.SetAttribute(mxGraphMlConstants.EDGE_DEFAULT, edgedefault);
			}

			foreach (mxGraphMlNode node in nodes)
			{
				Element nodeElement = node.generateElement(document);
                graph.AppendChild(nodeElement);
			}

			foreach (mxGraphMlEdge edge in edges)
			{
				Element edgeElement = edge.generateElement(document);
                graph.AppendChild(edgeElement);
			}

			return graph;
		}
	}

}