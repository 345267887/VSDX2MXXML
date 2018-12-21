using System;
using System.Collections.Generic;
using System.Text;

namespace mxGraph.io
{



	using Element = System.Xml.XmlElement;
	//using SAXException = org.xml.sax.SAXException;

	using ShapePageId = mxGraph.io.vsdx.ShapePageId;
	using VsdxShape = mxGraph.io.vsdx.VsdxShape;
	using mxVsdxConstants = mxGraph.io.vsdx.mxVsdxConstants;
	using mxVsdxMaster = mxGraph.io.vsdx.mxVsdxMaster;
	using mxVsdxPage = mxGraph.io.vsdx.mxVsdxPage;
	using mxVsdxUtils = mxGraph.io.vsdx.mxVsdxUtils;
	using mxCell = mxGraph.model.mxCell;
	using mxGeometry = mxGraph.model.mxGeometry;
	using mxGraphModel = mxGraph.model.mxGraphModel;
	using mxPoint = mxGraph.util.mxPoint;
	using mxGraph = mxGraph.view.mxGraph;

	public class mxVssxCodec : mxVsdxCodec
	{
		public mxVssxCodec()
		{
			RESPONSE_END = "";
			RESPONSE_DIAGRAM_START = "";
			RESPONSE_DIAGRAM_END = "";
			RESPONSE_HEADER = "";
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String decodeVssx(byte[] data, String charset) throws java.io.IOException, javax.xml.parsers.ParserConfigurationException, org.xml.sax.SAXException, javax.xml.transform.TransformerException
		public virtual string decodeVssx(byte[] data, string charset)
		{
			StringBuilder library = new StringBuilder("<mxlibrary>[");

			//process shapes in pages
			string shapesInPages = decodeVsdx(data, charset);

			library.Append(shapesInPages);

			//process shapes in master
			IDictionary<string, mxVsdxMaster> masterShapes = vsdxModel.MasterShapes;

			//using the first page as a dummy one
			mxVsdxPage page = vsdxModel.Pages.Values.GetEnumerator().Current;

			if (masterShapes != null)
			{
				StringBuilder shapes = new StringBuilder();
				string comma = shapesInPages.Length == 0? "" : ",";
				foreach (mxVsdxMaster master in masterShapes.Values)
				{
					mxGraph shapeGraph = createMxGraph();

					Element shapeElem = master.MasterShape.Shape;
					VsdxShape shape = new VsdxShape(page, shapeElem, !page.isEdge(shapeElem), masterShapes, null, vsdxModel);
					mxCell cell = null;

					if (shape.Vertex)
					{
						edgeShapeMap.Clear();
						parentsMap.Clear();
						cell = addShape(shapeGraph, shape, shapeGraph.DefaultParent, 0, 1169); //1169 is A4 page height

						foreach (var item in edgeShapeMap)
						{
							object parent = parentsMap[item.Key];
							addUnconnectedEdge(shapeGraph, parent, item.Value, 1169); //1169 is A4 page height
						}
					}
					else
					{
						cell = (mxCell) addUnconnectedEdge(shapeGraph, null, shape, 1169); //1169 is A4 page height
					}

					if (cell != null)
					{
						shapes.Append(comma);
						shapes.Append("{\"xml\":\"");
						mxGeometry geo = normalizeGeo(cell);

						sanitiseGraph(shapeGraph);

						if (shapeGraph.Model.getChildCount(shapeGraph.DefaultParent) == 0)
						{
							continue;
						}

						string shapeXML = base.processPage(shapeGraph, null);
						shapes.Append(shapeXML);
						shapes.Append("\",\"w\":");
						shapes.Append(geo.Width);
						shapes.Append(",\"h\":");
						shapes.Append(geo.Height);
						shapes.Append(",\"title\":\"");

						string shapeName = master.Name;
						if (!string.ReferenceEquals(shapeName, null))
						{
							shapeName = mxVsdxUtils.htmlEntities(shapeName);
						}

						shapes.Append(shapeName);

						shapes.Append("\"}");
						comma = ",";
					}
				}
				library.Append(shapes);
			}
			library.Append("]</mxlibrary>");

			//TODO UTF-8 support is missing
			//
	//		System.out.println(library);

			return library.ToString();
		}

		protected internal virtual mxGeometry normalizeGeo(mxCell cell)
		{
			mxGeometry geo = cell.Geometry;
			geo.X = 0;
			geo.Y = 0;

			mxPoint srcP = geo.SourcePoint;

			if (cell.Edge && srcP != null)
			{
				transPoint(geo.TargetPoint, srcP);
				transPoint(geo.Offset, srcP);
				IList<mxPoint> points = geo.Points;

				if (points != null)
				{
					foreach (mxPoint p in points)
					{
						transPoint(p, srcP);
					}
				}
				transPoint(srcP, srcP);
			}
			return geo;
		}

		protected internal virtual void transPoint(mxPoint p, mxPoint srcP)
		{
			if (p != null)
			{
				p.X = p.X - srcP.X;
				p.Y = p.Y - srcP.Y;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected String processPage(mxGraphview.mxGraph graph, mxGraphio.vsdx.mxVsdxPage page) throws java.io.IOException
		protected internal override string processPage(mxGraph graph, mxVsdxPage page)
		{
            mxGraphModel model = (mxGraphModel)graph.Model;

			StringBuilder shapes = new StringBuilder();
			string comma = "";
			foreach (object c in model.Cells.Values)
			{
				//add top level shapes only to the library
				if (graph.DefaultParent == model.getParent(c))
				{
					shapes.Append(comma);
					shapes.Append("{\"xml\":\"");
					mxGraph shapeGraph = createMxGraph();
					shapeGraph.addCell(c);
					sanitiseGraph(shapeGraph);

					if (shapeGraph.Model.getChildCount(shapeGraph.DefaultParent) == 0)
					{
						continue;
					}

					mxGeometry geo = normalizeGeo((mxCell) c);
					string shapeXML = base.processPage(shapeGraph, null);
					shapes.Append(shapeXML);
					shapes.Append("\",\"w\":");
					shapes.Append(geo.Width);
					shapes.Append(",\"h\":");
					shapes.Append(geo.Height);
					shapes.Append(",\"title\":\"");
					string style = model.getStyle(c);

					string name = "";
					if (!string.ReferenceEquals(style, null))
					{
						int p = style.IndexOf(mxVsdxConstants.VSDX_ID, StringComparison.Ordinal);
						if (p >= 0)
						{
							p += mxVsdxConstants.VSDX_ID.Length + 1;
							int id = int.Parse(style.Substring(p, style.IndexOf(";", p, StringComparison.Ordinal) - p));
							VsdxShape vsdxShape = vertexShapeMap[new ShapePageId(page.Id.Value, id)];
							if (vsdxShape != null)
							{
								name = vsdxShape.Name;
							}
						}
					}
					shapes.Append(name);
					shapes.Append("\"}");
					comma = ",";
				}
			}

			if (shapes.Length > 0)
			{
				RESPONSE_DIAGRAM_START = ",";
			}
			else
			{
				RESPONSE_DIAGRAM_START = "";
			}

			return shapes.ToString();
		}
	}

}