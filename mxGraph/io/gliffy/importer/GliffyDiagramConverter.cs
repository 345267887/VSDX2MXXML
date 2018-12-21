using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace com.mxgraph.io.gliffy.importer
{


	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	using GsonBuilder = com.google.gson.GsonBuilder;
	using Constraint = com.mxgraph.io.gliffy.model.Constraint;
	using ConstraintData = com.mxgraph.io.gliffy.model.Constraint.ConstraintData;
	using Constraints = com.mxgraph.io.gliffy.model.Constraints;
	using Diagram = com.mxgraph.io.gliffy.model.Diagram;
	using Resource = com.mxgraph.io.gliffy.model.EmbeddedResources.Resource;
	using GliffyObject = com.mxgraph.io.gliffy.model.GliffyObject;
	using GliffyText = com.mxgraph.io.gliffy.model.GliffyText;
	using Graphic = com.mxgraph.io.gliffy.model.Graphic;
	using GliffyImage = com.mxgraph.io.gliffy.model.Graphic.GliffyImage;
	using GliffyLine = com.mxgraph.io.gliffy.model.Graphic.GliffyLine;
	using GliffyMindmap = com.mxgraph.io.gliffy.model.Graphic.GliffyMindmap;
	using GliffyShape = com.mxgraph.io.gliffy.model.Graphic.GliffyShape;
	using GliffySvg = com.mxgraph.io.gliffy.model.Graphic.GliffySvg;
	using mxCell = com.mxgraph.model.mxCell;
	using mxGeometry = com.mxgraph.model.mxGeometry;
	using Utils = com.mxgraph.online.Utils;
	using mxDomUtils = com.mxgraph.util.mxDomUtils;
	using mxPoint = com.mxgraph.util.mxPoint;
	using mxXmlUtils = com.mxgraph.util.mxXmlUtils;
	using mxGraphHeadless = com.mxgraph.view.mxGraphHeadless;

	/// <summary>
	/// Performs a conversion of a Gliffy diagram into a Draw.io diagram
	/// <para>
	/// Example :
	/// </para>
	/// <para>
	/// <code><i>
	/// GliffyDiagramConverter converter = new GliffyDiagramConverter(gliffyJsonString);<br>
	/// String drawioXml = converter.getGraphXml();</i>
	/// </code>
	/// 
	/// 
	/// </para>
	/// </summary>
	public class GliffyDiagramConverter
	{
		//internal Logger logger = Logger.getLogger("GliffyDiagramConverter");

		private string diagramString;

		private Diagram gliffyDiagram;

		private mxGraphHeadless drawioDiagram;

		private IDictionary<int?, GliffyObject> vertices;

		private Pattern rotationPattern = Pattern.compile("rotation=(\\-?\\w+)");

		/// <summary>
		/// Constructs a new converter and starts a conversion.
		/// </summary>
		/// <param name="gliffyDiagramString"> JSON string of a gliffy diagram </param>
		public GliffyDiagramConverter(string gliffyDiagramString)
		{
			vertices = new LinkedHashMap<int?, GliffyObject>();
			this.diagramString = gliffyDiagramString;
			drawioDiagram = new mxGraphHeadless();
			start();
		}

		private void start()
		{
			// creates a diagram object from the JSON string
			this.gliffyDiagram = (new GsonBuilder()).registerTypeAdapterFactory(new PostDeserializer()).create().fromJson(diagramString, typeof(Diagram));

			collectVerticesAndConvert(vertices, gliffyDiagram.stage.Objects, null);

			//sort objects by the order specified in the Gliffy diagram
			sortObjectsByOrder(gliffyDiagram.stage.Objects);

			drawioDiagram.Model.beginUpdate();

			try
			{
				foreach (GliffyObject obj in gliffyDiagram.stage.Objects)
				{
					importObject(obj, obj.parent);
				}
			}
			finally
			{
				drawioDiagram.Model.endUpdate();
			}

		}

		/// <summary>
		/// Imports the objects into the draw.io diagram. Recursively adds the children 
		/// </summary>
		private void importObject(GliffyObject obj, GliffyObject gliffyParent)
		{
			mxCell parent = gliffyParent != null ? gliffyParent.mxObject : null;

			drawioDiagram.addCell(obj.mxObject, parent);

			if (obj.hasChildren())
			{
				if (!obj.Swimlane)
				{
					// sort the children except for swimlanes
					// their order value is "auto"
					sortObjectsByOrder(obj.children);
				}

				foreach (GliffyObject child in obj.children)
				{
					importObject(child, obj);
				}
			}

			if (obj.Line)
			{
				// gets the terminal cells for the edge
				mxCell startTerminal = getTerminalCell(obj, true);
				mxCell endTerminal = getTerminalCell(obj, false);

				drawioDiagram.addCell(obj.MxObject, parent, null, startTerminal, endTerminal);

				setWaypoints(obj, startTerminal, endTerminal);
			}
		}

		private void sortObjectsByOrder(ICollection<GliffyObject> values)
		{
			IComparer<GliffyObject> c = new ComparatorAnonymousInnerClass(this);

			Collections.sort((IList<GliffyObject>) values, c);
		}

		private class ComparatorAnonymousInnerClass : IComparer<GliffyObject>
		{
			private readonly GliffyDiagramConverter outerInstance;

			public ComparatorAnonymousInnerClass(GliffyDiagramConverter outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(GliffyObject o1, GliffyObject o2)
			{
				float? o1o;
				float? o2o;
				try
				{
					if (string.ReferenceEquals(o1.order, null) || string.ReferenceEquals(o2.order, null))
					{
						return 0;
					}

					o1o = float.Parse(o1.order);
					o2o = float.Parse(o2.order);

					return o1o.Value.CompareTo(o2o);
				}
				catch (System.FormatException)
				{
					return o1.order.CompareTo(o2.order);
				}

			}
		}

		private mxCell getTerminalCell(GliffyObject gliffyEdge, bool start)
		{
			Constraints cons = gliffyEdge.Constraints;

			if (cons == null)
			{
				return null;
			}

			Constraint con = start ? cons.StartConstraint : cons.EndConstraint;

			if (con == null)
			{
				return null;
			}

			Constraint.ConstraintData cst = start ? con.StartPositionConstraint : con.EndPositionConstraint;
			int nodeId = cst.NodeId;
			GliffyObject gliffyEdgeTerminal = vertices[nodeId];

			//edge could be terminated with another edge, so import it as a dangling edge
			if (gliffyEdgeTerminal == null)
			{
				return null;
			}

			mxCell mxEdgeTerminal = gliffyEdgeTerminal.MxObject;

			return mxEdgeTerminal;
		}

		/// <summary>
		/// Sets the waypoints
		/// </summary>
		/// <param name="object"> Gliffy line </param>
		/// <param name="startTerminal"> starting point </param>
		/// <param name="endTerminal"> ending point </param>
		private void setWaypoints(GliffyObject @object, mxCell startTerminal, mxCell endTerminal)
		{
			mxCell cell = @object.MxObject;
			mxGeometry geo = drawioDiagram.Model.getGeometry(cell);
			geo.Relative = true;

			IList<float[]> points = @object.Graphic.Line.controlPath;

			if (points.Count < 2)
			{
				return;
			}

			IList<mxPoint> mxPoints = new List<mxPoint>();

			foreach (float[] point in points)
			{
				float[] pts = new float[] {point[0], point[1]};

				if (@object.rotation != 0)
				{
					pts = rotate(pts[0], pts[1], @object.rotation);
				}

				mxPoints.Add(new mxPoint(pts[0] + @object.x, pts[1] + @object.y));
			}

			if (startTerminal == null)
			{
				mxPoint first = mxPoints[0];
				geo.setTerminalPoint(first, true);
				mxPoints.Remove(first); // remove first so it doesn't become a waypoint
			}

			if (endTerminal == null)
			{
				mxPoint last = mxPoints[mxPoints.Count - 1];
				geo.setTerminalPoint(last, false);
				mxPoints.Remove(last); // remove last so it doesn't become a waypoint
			}

			if (mxPoints.Count > 0)
			{
				geo.Points = mxPoints;
			}

			drawioDiagram.Model.setGeometry(cell, geo);

		}

		private float[] rotate(float px, float py, float angle)
		{
			double angleRad = Math.toRadians(angle);
			double x = px * Math.Cos(angleRad) - py * Math.Sin(angleRad);
			double y = px * Math.Sin(angleRad) + py * Math.Cos(angleRad);

			return new float[] {(float)x, (float)y};
		}

		/// <summary>
		/// Creates a map of all vertices so they can be easily accessed when looking
		/// up terminal cells for edges
		/// </summary>
		private void collectVerticesAndConvert(IDictionary<int?, GliffyObject> vertices, ICollection<GliffyObject> objects, GliffyObject parent)
		{
			foreach (GliffyObject @object in objects)
			{
				@object.parent = parent;

				convertGliffyObject(@object, parent);

				if (!@object.Line)
				{
					vertices[@object.id] = @object;
				}

				// don't collect for swimlanes and mindmaps, their children are treated differently
				if (@object.Group || (@object.Line && @object.hasChildren()))
				{
					collectVerticesAndConvert(vertices, @object.children, @object);
				}
			}
		}

		/// <summary>
		/// Converts the mxGraph to xml string
		/// 
		/// @return </summary>
		/// <exception cref="UnsupportedEncodingException"> </exception>
		public virtual string GraphXml
		{
			get
			{
				mxCodec codec = new mxCodec();
				Element node = (Element) codec.encode(drawioDiagram.Model);
				node.setAttribute("style", "default-style2");
				node.setAttribute("background", gliffyDiagram.stage.BackgroundColor);
				node.setAttribute("grid", gliffyDiagram.stage.GridOn ? "1" : "0");
				node.setAttribute("guides", gliffyDiagram.stage.DrawingGuidesOn ? "1" : "0");
				string xml = mxXmlUtils.getXml(node);
				return xml;
			}
		}

		/// <summary>
		/// Performs the object conversion
		/// 
		/// 
		/// </summary>
		private mxCell convertGliffyObject(GliffyObject gliffyObject, GliffyObject parent)
		{
			mxCell cell = new mxCell();

			if (gliffyObject.UnrecognizedGraphicType)
			{
				logger.warning("Unrecognized graphic type for object with ID : " + gliffyObject.id);
				return cell;
			}

			StringBuilder style = new StringBuilder();

			mxGeometry geometry = new mxGeometry((int) gliffyObject.x, (int) gliffyObject.y, (int) gliffyObject.width, (int) gliffyObject.height);
			cell.Geometry = geometry;

			GliffyObject textObject = null;
			string link = null;

			Graphic graphic = null;

			if (gliffyObject.Group)
			{
				style.Append("group;");
				cell.Vertex = true;
			}
			else
			{
				// groups don't have graphic
				graphic = gliffyObject.Graphic;
				textObject = gliffyObject.TextObject;
			}

			if (graphic != null)
			{
				link = gliffyObject.Link;

				if (gliffyObject.Shape)
				{
					Graphic.GliffyShape shape = graphic.Shape_Renamed;

					cell.Vertex = true;
					style.Append("shape=" + StencilTranslator.translate(gliffyObject.uid)).Append(";");
					style.Append("shadow=" + (shape.dropShadow ? 1 : 0)).Append(";");

					if (style.lastIndexOf("strokeWidth") == -1)
					{
						style.Append("strokeWidth=" + shape.strokeWidth).Append(";");
					}

					if (style.lastIndexOf("fillColor") == -1)
					{
						style.Append("fillColor=" + shape.fillColor).Append(";");
					}
					if (style.lastIndexOf("strokeColor") == -1)
					{
						style.Append("strokeColor=" + shape.strokeColor).Append(";");
					}

					if (style.lastIndexOf("gradient") == -1 && shape.gradient && !gliffyObject.GradientIgnored)
					{
						style.Append("gradientColor=" + gliffyObject.GradientColor + ";gradientDirection=north;");
					}

					// opacity value is wrong for venn circles, so ignore it and use the one in the mapping
					if (!gliffyObject.VennCircle)
					{
						style.Append("opacity=" + shape.opacity * 100).Append(";");
						style.Append(DashStyleMapping.get(shape.dashStyle));
					}

					style.Append(DashStyleMapping.get(shape.dashStyle));

					if (gliffyObject.SubRoutine)
					{
						//Gliffy's subroutine maps to drawio process, whose inner boundary, unlike subroutine's, is relative to it's width so here we set it to 10px
						style.Append("size=" + 10 / gliffyObject.width).Append(";");
					}
				}
				else if (gliffyObject.Line)
				{
					Graphic.GliffyLine line = graphic.Line_Renamed;

					cell.Edge = true;
					style.Append("strokeWidth=" + line.strokeWidth).Append(";");
					style.Append("strokeColor=" + line.strokeColor).Append(";");
					style.Append(ArrowMapping.get(line.startArrow).ToString(true)).Append(";");
					style.Append(ArrowMapping.get(line.endArrow).ToString(false)).Append(";");
					style.Append(DashStyleMapping.get(line.dashStyle));
					style.Append(LineMapping.get(line.interpolationType));

					geometry.X = 0;
					geometry.Y = 0;
				}
				else if (gliffyObject.Text)
				{
					textObject = gliffyObject;
					cell.Vertex = true;
					style.Append("text;whiteSpace=wrap;html=1;nl2Br=0;");
					cell.Value = gliffyObject.Text;

					//if text is a child of a cell, use relative geometry and set X and Y to 0
					if (gliffyObject.parent != null && !gliffyObject.parent.Group)
					{
						mxGeometry parentGeometry = gliffyObject.parent.mxObject.Geometry;

						//if text is a child of a line, special positioning is in place
						if (gliffyObject.parent.Line)
						{
							/* Gliffy's text offset is a float in the range of [0,1]
							 * draw.io's text offset is a float in the range of [-1,-1] (while still keeping the text within the line)
							 * The equation that translates Gliffy offset to draw.io offset is : G*2 - 1 = D 
							 */
							mxGeometry mxGeo = new mxGeometry(graphic.Text_Renamed.lineTValue != null ? graphic.Text_Renamed.lineTValue * 2 - 1 : GliffyText.DEFAULT_LINE_T_VALUE.Value, 0, 0, 0);
							mxGeo.Offset = new mxPoint();
							cell.Geometry = mxGeo;

							style.Append("labelBackgroundColor=" + gliffyDiagram.stage.BackgroundColor).Append(";");
							//should we force horizontal align for text on lines?
							//style.append("align=center;");
						}
						else
						{
							cell.Geometry = new mxGeometry(0, 0, parentGeometry.Width, parentGeometry.Height);
						}

						cell.Geometry.Relative = true;
					}
				}
				else if (gliffyObject.Image)
				{
					Graphic.GliffyImage image = graphic.Image;
					cell.Vertex = true;
					style.Append("shape=" + StencilTranslator.translate(gliffyObject.uid)).Append(";");
					style.Append("image=" + image.Url).Append(";");
				}
				else if (gliffyObject.Svg)
				{
					Graphic.GliffySvg svg = graphic.Svg;
					cell.Vertex = true;
					style.Append("shape=image;aspect=fixed;");
					Resource res = gliffyDiagram.embeddedResources.get(svg.embeddedResourceId);

					style.Append("image=data:image/svg+xml,").Append(res.Base64EncodedData).Append(";");
				}
			}
			// swimlanes have children without uid so their children are converted here ad hoc
			else if (gliffyObject.Swimlane)
			{
				cell.Vertex = true;
				style.Append(StencilTranslator.translate(gliffyObject.uid)).Append(";");

				GliffyObject header = gliffyObject.children[0]; // first child is the header of the swimlane

				Graphic.GliffyShape shape = header.graphic.Shape;
				style.Append("strokeWidth=" + shape.strokeWidth).Append(";");
				style.Append("shadow=" + (shape.dropShadow ? 1 : 0)).Append(";");
				style.Append("fillColor=" + shape.fillColor).Append(";");
				style.Append("strokeColor=" + shape.strokeColor).Append(";");
				style.Append("whiteSpace=wrap;");

				double rads = Math.toRadians(gliffyObject.rotation);
				mxPoint pivot = new mxPoint(gliffyObject.width / 2, gliffyObject.height / 2);
				double cos = Math.Cos(rads);
				double sin = Math.Sin(rads);
				mxPoint baseP = Utils.getRotatedPoint(new mxPoint(0, 0), cos, sin, pivot);

				for (int i = 1; i < gliffyObject.children.Count; i++) // rest of the children are lanes
				{
					GliffyObject gLane = gliffyObject.children[i];
					gLane.parent = gliffyObject;

					Graphic.GliffyShape gs = gLane.graphic.Shape;
					StringBuilder laneStyle = new StringBuilder();
					laneStyle.Append("swimlane;swimlaneLine=0;");
					laneStyle.Append("strokeWidth=" + gs.strokeWidth).Append(";");
					laneStyle.Append("shadow=" + (gs.dropShadow ? 1 : 0)).Append(";");
					laneStyle.Append("fillColor=" + gs.fillColor).Append(";");
					laneStyle.Append("strokeColor=" + gs.strokeColor).Append(";");
					laneStyle.Append("whiteSpace=wrap;html=1;");

					mxGeometry childGeometry = null;

					if (gliffyObject.rotation != 0)
					{
						laneStyle.Append("rotation=" + gliffyObject.rotation).Append(";");
						mxPoint pointAbs = new mxPoint(gLane.x, gLane.y);
						pointAbs = Utils.getRotatedPoint(pointAbs, cos, sin, pivot);
						childGeometry = new mxGeometry(pointAbs.X - baseP.X, pointAbs.Y - baseP.Y, gLane.width, gLane.height);
					}
					else
					{
						childGeometry = new mxGeometry(gLane.x, gLane.y, gLane.width, gLane.height);
					}

					mxCell mxLane = new mxCell();
					mxLane.Vertex = true;
					cell.insert(mxLane);
					mxLane.Value = gLane.children[0].Text;
					mxLane.Style = laneStyle.ToString();

					mxLane.Geometry = childGeometry;
					gLane.mxObject = mxLane;
				}
			}
			else if (gliffyObject.Mindmap)
			{
				GliffyObject rectangle = gliffyObject.children[0];

				Graphic.GliffyMindmap mindmap = rectangle.graphic.Mindmap_Renamed;

				style.Append("shape=" + StencilTranslator.translate(gliffyObject.uid)).Append(";");
				style.Append("shadow=" + (mindmap.dropShadow ? 1 : 0)).Append(";");
				style.Append("strokeWidth=" + mindmap.strokeWidth).Append(";");
				style.Append("fillColor=" + mindmap.fillColor).Append(";");
				style.Append("strokeColor=" + mindmap.strokeColor).Append(";");
				style.Append(DashStyleMapping.get(mindmap.dashStyle));

				if (mindmap.gradient)
				{
					style.Append("gradientColor=#FFFFFF;gradientDirection=north;");
				}

				cell.Vertex = true;
			}

			if (!gliffyObject.Line && gliffyObject.rotation != 0)
			{
				//if there's a rotation by default, add to it
				if (style.lastIndexOf("rotation") != -1)
				{
					Matcher m = rotationPattern.matcher(style);
					if (m.find())
					{
						string rot = m.group(1);
						float? rotation = float.Parse(rot) + gliffyObject.rotation;

						style.Append(m.replaceFirst("rotation=" + rotation.ToString()));
					}
				}
				else
				{
					style.Append("rotation=" + gliffyObject.rotation + ";");
				}
			}

			if (textObject != null)
			{
				style.Append("html=1;nl2Br=0;whiteSpace=wrap;");

				if (!gliffyObject.Line)
				{
					cell.Value = textObject.Text;
					style.Append(textObject.graphic.Text.Style);
				}
			}

			if (!string.ReferenceEquals(link, null))
			{
				Document doc = mxDomUtils.createDocument();
				Element uo = doc.createElement("UserObject");
				uo.setAttribute("link", link);
				drawioDiagram.Model.setValue(cell, uo);

				if (textObject != null)
				{
					uo.setAttribute("label", textObject.Text);
				}
			}

			cell.Style = style.ToString();
			gliffyObject.mxObject = cell;

			return cell;
		}
	}

}