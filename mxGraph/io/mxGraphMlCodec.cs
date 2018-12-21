using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010-2012, JGraph Ltd
/// </summary>
namespace mxGraph.io
{

	using mxGraphMlConstants = mxGraph.io.graphml.mxGraphMlConstants;
	using mxGraphMlData = mxGraph.io.graphml.mxGraphMlData;
	using mxGraphMlEdge = mxGraph.io.graphml.mxGraphMlEdge;
	using mxGraphMlGraph = mxGraph.io.graphml.mxGraphMlGraph;
	using mxGraphMlKey = mxGraph.io.graphml.mxGraphMlKey;
	using mxGraphMlKeyManager = mxGraph.io.graphml.mxGraphMlKeyManager;
	using mxGraphMlNode = mxGraph.io.graphml.mxGraphMlNode;
	using mxGraphMlShapeEdge = mxGraph.io.graphml.mxGraphMlShapeEdge;
	using mxGraphMlShapeNode = mxGraph.io.graphml.mxGraphMlShapeNode;
	using mxGraphMlUtils = mxGraph.io.graphml.mxGraphMlUtils;
	using mxCell = mxGraph.model.mxCell;
	using mxConstants = mxGraph.util.mxConstants;
	using mxDomUtils = mxGraph.util.mxDomUtils;
	using mxPoint = mxGraph.util.mxPoint;
	using mxCellState = mxGraph.view.mxCellState;
	using mxConnectionConstraint = mxGraph.view.mxConnectionConstraint;

	using mxGraph = mxGraph.view.mxGraph;
	using mxGraphView = mxGraph.view.mxGraphView;
	using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;
	using NodeList = System.Xml.XmlNodeList;

	/// <summary>
	/// Parses a GraphML .graphml file and imports it in the given graph.<br/>
	/// 
	/// See wikipedia.org/wiki/GraphML for more on GraphML.
	/// 
	/// This class depends from the classes contained in
	/// mxGraphio.gmlImplements.
	/// </summary>
	public class mxGraphMlCodec
	{
		/// <summary>
		/// Receives a GraphMl document and parses it generating a new graph that is inserted in graph. </summary>
		/// <param name="document"> XML to be parsed </param>
		/// <param name="graph"> Graph where the parsed graph is included. </param>
		public static void decode(Document document, mxGraph graph)
		{
			object parent = graph.DefaultParent;

			graph.Model.beginUpdate();

			// Initialise the key properties.
			mxGraphMlKeyManager.Instance.initialise(document);

            NodeList graphs = document.GetElementsByTagName(mxGraphMlConstants.GRAPH);
			if (graphs.Count > 0)
			{

                Element graphElement = (Element) graphs.Item(0);

				//Create the graph model.
				mxGraphMlGraph gmlGraph = new mxGraphMlGraph(graphElement);

				gmlGraph.addGraph(graph, parent);
			}

			graph.Model.endUpdate();
			cleanMaps();
		}

		/// <summary>
		/// Remove all the elements in the Defined Maps.
		/// </summary>
		private static void cleanMaps()
		{
			mxGraphMlKeyManager.Instance.KeyMap.Clear();
		}

		/// <summary>
		/// Generates a Xml document with the gmlGraph. </summary>
		/// <param name="gmlGraph"> Graph model. </param>
		/// <returns> The Xml document generated. </returns>
		public static Document encodeXML(mxGraphMlGraph gmlGraph)
		{
			Document doc = mxDomUtils.createDocument();

            Element graphml = doc.CreateElement(mxGraphMlConstants.GRAPHML);

            graphml.SetAttribute("xmlns", "http://graphml.graphdrawing.org/xmlns");
            

            graphml.SetAttribute("http://www.w3.org/2000/xmlns/", "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			graphml.SetAttribute("http://www.w3.org/2000/xmlns/", "xmlns:jGraph", mxGraphMlConstants.JGRAPH_URL);
			graphml.SetAttribute("http://www.w3.org/2001/XMLSchema-instance", "xsi:schemaLocation", "http://graphml.graphdrawing.org/xmlns http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd");

			Dictionary<string, mxGraphMlKey> keyMap = mxGraphMlKeyManager.Instance.KeyMap;

			foreach (mxGraphMlKey key in keyMap.Values)
			{
				Element keyElement = key.generateElement(doc);
                graphml.AppendChild(keyElement);
			}

			Element graphE = gmlGraph.generateElement(doc);
            graphml.AppendChild(graphE);

            doc.AppendChild(graphml);
			cleanMaps();
			return doc;

		}

		/// <summary>
		/// Generates a Xml document with the cells in the graph. </summary>
		/// <param name="graph"> Graph with the cells. </param>
		/// <returns> The Xml document generated. </returns>
		public static Document encode(mxGraph graph)
		{
			mxGraphMlGraph gmlGraph = new mxGraphMlGraph();
			object parent = graph.DefaultParent;

			createKeyElements();

			gmlGraph = decodeGraph(graph, parent, gmlGraph);
			gmlGraph.Edgedefault = mxGraphMlConstants.EDGE_DIRECTED;

			Document document = encodeXML(gmlGraph);

			return document;
		}

		/// <summary>
		/// Creates the key elements for the encode.
		/// </summary>
		private static void createKeyElements()
		{
			Dictionary<string, mxGraphMlKey> keyMap = mxGraphMlKeyManager.Instance.KeyMap;
			mxGraphMlKey keyNode = new mxGraphMlKey(mxGraphMlConstants.KEY_NODE_ID, mxGraphMlKey.keyForValues.NODE, mxGraphMlConstants.KEY_NODE_NAME, mxGraphMlKey.keyTypeValues.STRING);
			keyMap[mxGraphMlConstants.KEY_NODE_ID] = keyNode;
			mxGraphMlKey keyEdge = new mxGraphMlKey(mxGraphMlConstants.KEY_EDGE_ID, mxGraphMlKey.keyForValues.EDGE, mxGraphMlConstants.KEY_EDGE_NAME, mxGraphMlKey.keyTypeValues.STRING);
			keyMap[mxGraphMlConstants.KEY_EDGE_ID] = keyEdge;
			mxGraphMlKeyManager.Instance.KeyMap = keyMap;
		}

		/// <summary>
		/// Returns a Gml graph with the data of the vertexes and edges in the graph. </summary>
		/// <param name="gmlGraph"> Gml document where the elements are put. </param>
		/// <param name="parent"> Parent cell of the vertexes and edges to be added. </param>
		/// <param name="graph"> Graph that contains the vertexes and edges. </param>
		/// <returns> Returns the document with the elements added. </returns>
		public static mxGraphMlGraph decodeGraph(mxGraph graph, object parent, mxGraphMlGraph gmlGraph)
		{
			object[] vertexes = graph.getChildVertices(parent);
			IList<mxGraphMlEdge> gmlEdges = gmlGraph.Edges;
			gmlEdges = encodeEdges(gmlEdges, parent, graph);
			gmlGraph.Edges = gmlEdges;

			foreach (object vertex in vertexes)
			{
				IList<mxGraphMlNode> Gmlnodes = gmlGraph.Nodes;

				mxCell v = (mxCell) vertex;
				string id = v.Id;

				mxGraphMlNode gmlNode = new mxGraphMlNode(id, null);
				addNodeData(gmlNode, v);
				Gmlnodes.Add(gmlNode);
				gmlGraph.Nodes = Gmlnodes;
				mxGraphMlGraph gmlGraphx = new mxGraphMlGraph();

				gmlGraphx = decodeGraph(graph, vertex, gmlGraphx);

				if (!gmlGraphx.Empty)
				{
					IList<mxGraphMlGraph> nodeGraphs = gmlNode.NodeGraph;
					nodeGraphs.Add(gmlGraphx);
					gmlNode.NodeGraph = nodeGraphs;
				}
			}

			return gmlGraph;
		}

		/// <summary>
		/// Add the node data in the gmlNode. </summary>
		/// <param name="gmlNode"> Gml node where the data add. </param>
		/// <param name="v"> mxCell where data are obtained. </param>
		public static void addNodeData(mxGraphMlNode gmlNode, mxCell v)
		{
			mxGraphMlData data = new mxGraphMlData();
			mxGraphMlShapeNode dataShapeNode = new mxGraphMlShapeNode();

			data.DataKey = mxGraphMlConstants.KEY_NODE_ID;
			dataShapeNode.DataHeight = (v.Geometry.Height).ToString();
			dataShapeNode.DataWidth = (v.Geometry.Width).ToString();
			dataShapeNode.DataX = (v.Geometry.X).ToString();
			dataShapeNode.DataY = (v.Geometry.Y).ToString();
			dataShapeNode.DataLabel = v.Value != null ? v.Value.ToString() : "";
			dataShapeNode.DataStyle = !string.ReferenceEquals(v.Style, null) ? v.Style : "";

			data.DataShapeNode = dataShapeNode;
			gmlNode.NodeData = data;
		}

		/// <summary>
		/// Add the edge data in the gmlEdge. </summary>
		/// <param name="gmlEdge"> Gml edge where the data add. </param>
		/// <param name="v"> mxCell where data are obtained. </param>
		public static void addEdgeData(mxGraphMlEdge gmlEdge, mxCell v)
		{
			mxGraphMlData data = new mxGraphMlData();
			mxGraphMlShapeEdge dataShapeEdge = new mxGraphMlShapeEdge();

			data.DataKey = mxGraphMlConstants.KEY_EDGE_ID;
			dataShapeEdge.Text = v.Value != null ? v.Value.ToString() : "";
			dataShapeEdge.Style = !string.ReferenceEquals(v.Style, null) ? v.Style : "";

			data.DataShapeEdge = dataShapeEdge;
			gmlEdge.EdgeData = data;
		}

		/// <summary>
		/// Converts a connection point in the string representation of a port.
		/// The specials names North, NorthWest, NorthEast, East, West, South, SouthEast and SouthWest
		/// may be returned. Else, the values returned follows the pattern "double,double"
		/// where double must be in the range 0..1 </summary>
		/// <param name="point"> mxPoint </param>
		/// <returns> Name of the port </returns>
		private static string pointToPortString(mxPoint point)
		{
			string port = "";
			if (point != null)
			{
				double x = point.X;
				double y = point.Y;

				if (x == 0 && y == 0)
				{
					port = "NorthWest";
				}
				else if (x == 0.5 && y == 0)
				{
					port = "North";
				}
				else if (x == 1 && y == 0)
				{
					port = "NorthEast";
				}
				else if (x == 1 && y == 0.5)
				{
					port = "East";
				}
				else if (x == 1 && y == 1)
				{
					port = "SouthEast";
				}
				else if (x == 0.5 && y == 1)
				{
					port = "South";
				}
				else if (x == 0 && y == 1)
				{
					port = "SouthWest";
				}
				else if (x == 0 && y == 0.5)
				{
					port = "West";
				}
				else
				{
					port = "" + x + "," + y;
				}
			}
			return port;
		}

		/// <summary>
		/// Returns a list of mxGmlEdge with the data of the edges in the graph. </summary>
		/// <param name="Gmledges"> List where the elements are put. </param>
		/// <param name="parent"> Parent cell of the edges to be added. </param>
		/// <param name="graph"> Graph that contains the edges. </param>
		/// <returns> Returns the list Gmledges with the elements added. </returns>
		private static IList<mxGraphMlEdge> encodeEdges(IList<mxGraphMlEdge> Gmledges, object parent, mxGraph graph)
		{
			object[] edges = graph.getChildEdges(parent);
			foreach (object edge in edges)
			{
				mxCell e = (mxCell) edge;
				mxCell source = (mxCell) e.Source;
				mxCell target = (mxCell) e.Target;

				string sourceName = "";
				string targetName = "";
				string sourcePort = "";
				string targetPort = "";
				sourceName = source != null ? source.Id : "";
				targetName = target != null ? target.Id : "";

				//Get the graph view that contains the states
				mxGraphView view = graph.View;
				mxPoint sourceConstraint = null;
				mxPoint targetConstraint = null;
				if (view != null)
				{
					mxCellState edgeState = view.getState(edge);
					mxCellState sourceState = view.getState(source);
					mxConnectionConstraint scc = graph.getConnectionConstraint(edgeState, sourceState, true);
					if (scc != null)
					{
						sourceConstraint = scc.Point;
					}

					mxCellState targetState = view.getState(target);
					mxConnectionConstraint tcc = graph.getConnectionConstraint(edgeState, targetState, false);
					if (tcc != null)
					{
						targetConstraint = tcc.Point;
					}
				}

				//gets the port names
				targetPort = pointToPortString(targetConstraint);
				sourcePort = pointToPortString(sourceConstraint);

				mxGraphMlEdge Gmledge = new mxGraphMlEdge(sourceName, targetName, sourcePort, targetPort);

				string style = e.Style;

				if (string.ReferenceEquals(style, null))
				{
					style = "horizontal";

				}

				Dictionary<string, object> styleMap = mxGraphMlUtils.getStyleMap(style, "=");
				string endArrow = (string) styleMap[mxConstants.STYLE_ENDARROW];
				if ((!string.ReferenceEquals(endArrow, null) && !endArrow.Equals(mxConstants.NONE)) || string.ReferenceEquals(endArrow, null))
				{
					Gmledge.EdgeDirected = "true";
				}
				else
				{
					Gmledge.EdgeDirected = "false";
				}
				addEdgeData(Gmledge, e);
				Gmledges.Add(Gmledge);
			}

			return Gmledges;
		}
	}

}