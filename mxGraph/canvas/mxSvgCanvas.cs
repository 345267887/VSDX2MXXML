using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxSvgCanvas.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.canvas
{


	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	using mxConstants = mxGraph.util.mxConstants;
	using mxPoint = mxGraph.util.mxPoint;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxUtils = mxGraph.util.mxUtils;
	using mxCellState = mxGraph.view.mxCellState;

	/// <summary>
	/// An implementation of a canvas that uses SVG for painting. This canvas
	/// ignores the STYLE_LABEL_BACKGROUNDCOLOR and
	/// STYLE_LABEL_BORDERCOLOR styles due to limitations of SVG.
	/// </summary>
	public class mxSvgCanvas : mxBasicCanvas
	{

		/// <summary>
		/// Holds the HTML document that represents the canvas.
		/// </summary>
		protected internal Document document;

		/// <summary>
		/// Constructs a new SVG canvas for the specified dimension and scale.
		/// </summary>
		public mxSvgCanvas() : this(null)
		{
		}

		/// <summary>
		/// Constructs a new SVG canvas for the specified bounds, scale and
		/// background color.
		/// </summary>
		public mxSvgCanvas(Document document)
		{
			Document = document;
		}

		/// 
		public virtual void appendSvgElement(Element node)
		{
			if (document != null)
			{
                document.DocumentElement.AppendChild(node);
			}
		}

		/// 
		public virtual Document Document
		{
			set
			{
				this.document = value;
			}
			get
			{
				return document;
			}
		}


		/*
		 * (non-Javadoc)
		 * @see mxGraphcanvas.mxICanvas#drawCell()
		 */
		public override object drawCell(mxCellState state)
		{
			IDictionary<string, object> style = state.Style;
			Element elem = null;

			if (state.AbsolutePointCount > 1)
			{
				IList<mxPoint> pts = state.AbsolutePoints;

				// Transpose all points by cloning into a new array
				pts = mxUtils.translatePoints(pts, translate.X, translate.Y);

				// Draws the line
				elem = drawLine(pts, style);

				// Applies opacity
				float opacity = mxUtils.getFloat(style, mxConstants.STYLE_OPACITY, 100);

				if (opacity != 100)
				{
					string value = (opacity / 100).ToString();
                    elem.SetAttribute("fill-opacity", value);
					elem.SetAttribute("stroke-opacity", value);
				}
			}
			else
			{
                int x = (int) state.X + translate.X;
				int y = (int) state.Y + translate.Y;
				int w = (int) state.Width;
				int h = (int) state.Height;

				if (!mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_SWIMLANE))
				{
					elem = drawShape(x, y, w, h, style);
				}
				else
				{
					int start = (int) Math.Round(mxUtils.getInt(style, mxConstants.STYLE_STARTSIZE, mxConstants.DEFAULT_STARTSIZE) * scale);

					// Removes some styles to draw the content area
					IDictionary<string, object> cloned = new Dictionary<string, object>(style);
					cloned.Remove(mxConstants.STYLE_FILLCOLOR);
					cloned.Remove(mxConstants.STYLE_ROUNDED);

					if (mxUtils.isTrue(style, mxConstants.STYLE_HORIZONTAL, true))
					{
						elem = drawShape(x, y, w, start, style);
						drawShape(x, y + start, w, h - start, cloned);
					}
					else
					{
						elem = drawShape(x, y, start, h, style);
						drawShape(x + start, y, w - start, h, cloned);
					}
				}
			}

			return elem;
		}

		/*
		 * (non-Javadoc)
		 * @see mxGraphcanvas.mxICanvas#drawLabel()
		 */
		public override object drawLabel(string label, mxCellState state, bool html)
		{
			mxRectangle bounds = state.LabelBounds;

			if (drawLabels)
			{
				int x = (int) bounds.X + translate.X;
				int y = (int) bounds.Y + translate.Y;
				int w = (int) bounds.Width;
				int h = (int) bounds.Height;
				IDictionary<string, object> style = state.Style;

				return drawText(label, x, y, w, h, style);
			}

			return null;
		}

		/// <summary>
		/// Draws the shape specified with the STYLE_SHAPE key in the given style.
		/// </summary>
		/// <param name="x"> X-coordinate of the shape. </param>
		/// <param name="y"> Y-coordinate of the shape. </param>
		/// <param name="w"> Width of the shape. </param>
		/// <param name="h"> Height of the shape. </param>
		/// <param name="style"> Style of the the shape. </param>
		public virtual Element drawShape(int x, int y, int w, int h, IDictionary<string, object> style)
		{
			string fillColor = mxUtils.getString(style, mxConstants.STYLE_FILLCOLOR, "none");
			string strokeColor = mxUtils.getString(style, mxConstants.STYLE_STROKECOLOR);
			float strokeWidth = (float)(mxUtils.getFloat(style, mxConstants.STYLE_STROKEWIDTH, 1) * scale);

			// Draws the shape
			string shape = mxUtils.getString(style, mxConstants.STYLE_SHAPE);
			Element elem = null;
			Element background = null;

			if (shape.Equals(mxConstants.SHAPE_IMAGE))
			{
				string img = getImageForStyle(style);

				if (!string.ReferenceEquals(img, null))
				{
                    elem = document.CreateElement("image");

                    elem.SetAttribute("x", x.ToString());
                    elem.SetAttribute("y", y.ToString());
                    elem.SetAttribute("width", w.ToString());
                    elem.SetAttribute("height", h.ToString());

					elem.SetAttribute(mxConstants.NS_XLINK, "xlink:href", img);
				}
			}
			else if (shape.Equals(mxConstants.SHAPE_LINE))
			{
				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, mxConstants.DIRECTION_EAST);
				string d = null;

				if (direction.Equals(mxConstants.DIRECTION_EAST) || direction.Equals(mxConstants.DIRECTION_WEST))
				{
					int mid = (y + h / 2);
					d = "M " + x + " " + mid + " L " + (x + w) + " " + mid;
				}
				else
				{
					int mid = (x + w / 2);
					d = "M " + mid + " " + y + " L " + mid + " " + (y + h);
				}

                elem = document.CreateElement("path");
                elem.SetAttribute("d", d + " Z");
			}
			else if (shape.Equals(mxConstants.SHAPE_ELLIPSE))
			{
                elem = document.CreateElement("ellipse");

                elem.SetAttribute("cx", (x + w / 2).ToString());
				elem.SetAttribute("cy", (y + h / 2).ToString());
				elem.SetAttribute("rx", (w / 2).ToString());
				elem.SetAttribute("ry", (h / 2).ToString());
			}
			else if (shape.Equals(mxConstants.SHAPE_DOUBLE_ELLIPSE))
			{
                elem = document.CreateElement("g");
				background = document.CreateElement("ellipse");
                background.SetAttribute("cx", (x + w / 2).ToString());
				background.SetAttribute("cy", (y + h / 2).ToString());
				background.SetAttribute("rx", (w / 2).ToString());
				background.SetAttribute("ry", (h / 2).ToString());
                elem.AppendChild(background);

				int inset = (int)((3 + strokeWidth) * scale);

                Element foreground = document.CreateElement("ellipse");
				foreground.SetAttribute("fill", "none");
				foreground.SetAttribute("stroke", strokeColor);
				foreground.SetAttribute("stroke-width", strokeWidth.ToString());

				foreground.SetAttribute("cx", (x + w / 2).ToString());
				foreground.SetAttribute("cy", (y + h / 2).ToString());
				foreground.SetAttribute("rx", (w / 2 - inset).ToString());
				foreground.SetAttribute("ry", (h / 2 - inset).ToString());
                elem.AppendChild(foreground);
			}
			else if (shape.Equals(mxConstants.SHAPE_RHOMBUS))
			{
                elem = document.CreateElement("path");

				string d = "M " + (x + w / 2) + " " + y + " L " + (x + w) + " " + (y + h / 2) + " L " + (x + w / 2) + " " + (y + h) + " L " + x + " " + (y + h / 2);

                elem.SetAttribute("d", d + " Z");
			}
			else if (shape.Equals(mxConstants.SHAPE_TRIANGLE))
			{
                elem = document.CreateElement("path");
				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, "");
				string d = null;

				if (direction.Equals(mxConstants.DIRECTION_NORTH))
				{
					d = "M " + x + " " + (y + h) + " L " + (x + w / 2) + " " + y + " L " + (x + w) + " " + (y + h);
				}
				else if (direction.Equals(mxConstants.DIRECTION_SOUTH))
				{
					d = "M " + x + " " + y + " L " + (x + w / 2) + " " + (y + h) + " L " + (x + w) + " " + y;
				}
				else if (direction.Equals(mxConstants.DIRECTION_WEST))
				{
					d = "M " + (x + w) + " " + y + " L " + x + " " + (y + h / 2) + " L " + (x + w) + " " + (y + h);
				}
				else
				{
				// east
					d = "M " + x + " " + y + " L " + (x + w) + " " + (y + h / 2) + " L " + x + " " + (y + h);
				}

                elem.SetAttribute("d", d + " Z");
			}
			else if (shape.Equals(mxConstants.SHAPE_HEXAGON))
			{
                elem = document.CreateElement("path");
				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, "");
				string d = null;

				if (direction.Equals(mxConstants.DIRECTION_NORTH) || direction.Equals(mxConstants.DIRECTION_SOUTH))
				{
					d = "M " + (x + 0.5 * w) + " " + y + " L " + (x + w) + " " + (y + 0.25 * h) + " L " + (x + w) + " " + (y + 0.75 * h) + " L " + (x + 0.5 * w) + " " + (y + h) + " L " + x + " " + (y + 0.75 * h) + " L " + x + " " + (y + 0.25 * h);
				}
				else
				{
					d = "M " + (x + 0.25 * w) + " " + y + " L " + (x + 0.75 * w) + " " + y + " L " + (x + w) + " " + (y + 0.5 * h) + " L " + (x + 0.75 * w) + " " + (y + h) + " L " + (x + 0.25 * w) + " " + (y + h) + " L " + x + " " + (y + 0.5 * h);
				}

                elem.SetAttribute("d", d + " Z");
			}
			else if (shape.Equals(mxConstants.SHAPE_CLOUD))
			{
                elem = document.CreateElement("path");

				string d = "M " + (x + 0.25 * w) + " " + (y + 0.25 * h) + " C " + (x + 0.05 * w) + " " + (y + 0.25 * h) + " " + x + " " + (y + 0.5 * h) + " " + (x + 0.16 * w) + " " + (y + 0.55 * h) + " C " + x + " " + (y + 0.66 * h) + " " + (x + 0.18 * w) + " " + (y + 0.9 * h) + " " + (x + 0.31 * w) + " " + (y + 0.8 * h) + " C " + (x + 0.4 * w) + " " + (y + h) + " " + (x + 0.7 * w) + " " + (y + h) + " " + (x + 0.8 * w) + " " + (y + 0.8 * h) + " C " + (x + w) + " " + (y + 0.8 * h) + " " + (x + w) + " " + (y + 0.6 * h) + " " + (x + 0.875 * w) + " " + (y + 0.5 * h) + " C " + (x + w) + " " + (y + 0.3 * h) + " " + (x + 0.8 * w) + " " + (y + 0.1 * h) + " " + (x + 0.625 * w) + " " + (y + 0.2 * h) + " C " + (x + 0.5 * w) + " " + (y + 0.05 * h) + " " + (x + 0.3 * w) + " " + (y + 0.05 * h) + " " + (x + 0.25 * w) + " " + (y + 0.25 * h);

                elem.SetAttribute("d", d + " Z");
			}
			else if (shape.Equals(mxConstants.SHAPE_ACTOR))
			{
                elem = document.CreateElement("path");
				double width3 = w / 3;

				string d = " M " + x + " " + (y + h) + " C " + x + " " + (y + 3 * h / 5) + " " + x + " " + (y + 2 * h / 5) + " " + (x + w / 2) + " " + (y + 2 * h / 5) + " C " + (x + w / 2 - width3) + " " + (y + 2 * h / 5) + " " + (x + w / 2 - width3) + " " + y + " " + (x + w / 2) + " " + y + " C " + (x + w / 2 + width3) + " " + y + " " + (x + w / 2 + width3) + " " + (y + 2 * h / 5) + " " + (x + w / 2) + " " + (y + 2 * h / 5) + " C " + (x + w) + " " + (y + 2 * h / 5) + " " + (x + w) + " " + (y + 3 * h / 5) + " " + (x + w) + " " + (y + h);

                elem.SetAttribute("d", d + " Z");
			}
			else if (shape.Equals(mxConstants.SHAPE_CYLINDER))
			{
                elem = document.CreateElement("g");
                background = document.CreateElement("path");

				double dy = Math.Min(40, Math.Floor(h / 5.0));
				string d = " M " + x + " " + (y + dy) + " C " + x + " " + (y - dy / 3) + " " + (x + w) + " " + (y - dy / 3) + " " + (x + w) + " " + (y + dy) + " L " + (x + w) + " " + (y + h - dy) + " C " + (x + w) + " " + (y + h + dy / 3) + " " + x + " " + (y + h + dy / 3) + " " + x + " " + (y + h - dy);
                background.SetAttribute("d", d + " Z");
                elem.AppendChild(background);

                Element foreground = document.CreateElement("path");
				d = "M " + x + " " + (y + dy) + " C " + x + " " + (y + 2 * dy) + " " + (x + w) + " " + (y + 2 * dy) + " " + (x + w) + " " + (y + dy);

                foreground.SetAttribute("d", d);
				foreground.SetAttribute("fill", "none");
				foreground.SetAttribute("stroke", strokeColor);
				foreground.SetAttribute("stroke-width", strokeWidth.ToString());

                elem.AppendChild(foreground);
			}
			else
			{
                elem = document.CreateElement("rect");

				elem.SetAttribute("x", x.ToString());
				elem.SetAttribute("y", y.ToString());
				elem.SetAttribute("width", w.ToString());
				elem.SetAttribute("height", h.ToString());

				if (mxUtils.isTrue(style, mxConstants.STYLE_ROUNDED, false))
				{
					elem.SetAttribute("rx", (w * mxConstants.RECTANGLE_ROUNDING_FACTOR).ToString());
					elem.SetAttribute("ry", (h * mxConstants.RECTANGLE_ROUNDING_FACTOR).ToString());
				}
			}

			Element bg = background;

			if (bg == null)
			{
				bg = elem;
			}

			bg.SetAttribute("fill", fillColor);
			bg.SetAttribute("stroke", strokeColor);
			bg.SetAttribute("stroke-width", strokeWidth.ToString());

			// Adds the shadow element
			Element shadowElement = null;

			if (mxUtils.isTrue(style, mxConstants.STYLE_SHADOW, false) && !fillColor.Equals("none"))
			{
                shadowElement = (Element) bg.CloneNode(true);

				shadowElement.SetAttribute("transform", mxConstants.SVG_SHADOWTRANSFORM);
				shadowElement.SetAttribute("fill", mxConstants.W3C_SHADOWCOLOR);
				shadowElement.SetAttribute("stroke", mxConstants.W3C_SHADOWCOLOR);
				shadowElement.SetAttribute("stroke-width", strokeWidth.ToString());

				appendSvgElement(shadowElement);
			}

			// Applies rotation
			double rotation = mxUtils.getDouble(style, mxConstants.STYLE_ROTATION);

			if (rotation != 0)
			{
				int cx = x + w / 2;
				int cy = y + h / 2;

				elem.SetAttribute("transform", "rotate(" + rotation + "," + cx + "," + cy + ")");

				if (shadowElement != null)
				{
					shadowElement.SetAttribute("transform", "rotate(" + rotation + "," + cx + "," + cy + ") " + mxConstants.SVG_SHADOWTRANSFORM);
				}
			}

			// Applies opacity
			float opacity = mxUtils.getFloat(style, mxConstants.STYLE_OPACITY, 100);

			if (opacity != 100)
			{
				string value = (opacity / 100).ToString();
				elem.SetAttribute("fill-opacity", value);
				elem.SetAttribute("stroke-opacity", value);

				if (shadowElement != null)
				{
					shadowElement.SetAttribute("fill-opacity", value);
					shadowElement.SetAttribute("stroke-opacity", value);
				}
			}

			if (mxUtils.isTrue(style, mxConstants.STYLE_DASHED))
			{
				elem.SetAttribute("stroke-dasharray", "3, 3");
			}

			appendSvgElement(elem);

			return elem;
		}

		/// <summary>
		/// Draws the given lines as segments between all points of the given list
		/// of mxPoints.
		/// </summary>
		/// <param name="pts"> List of points that define the line. </param>
		/// <param name="style"> Style to be used for painting the line. </param>
		public virtual Element drawLine(IList<mxPoint> pts, IDictionary<string, object> style)
		{
            Element group = document.CreateElement("g");
            Element path = document.CreateElement("path");

			string strokeColor = mxUtils.getString(style, mxConstants.STYLE_STROKECOLOR);
			float tmpStroke = (mxUtils.getFloat(style, mxConstants.STYLE_STROKEWIDTH, 1));
			float strokeWidth = (float)(tmpStroke * scale);

			if (!string.ReferenceEquals(strokeColor, null) && strokeWidth > 0)
			{
				// Draws the start marker
				object marker = style[mxConstants.STYLE_STARTARROW];

				mxPoint pt = pts[1];
				mxPoint p0 = pts[0];
				mxPoint offset = null;

				if (marker != null)
				{
					float size = (mxUtils.getFloat(style, mxConstants.STYLE_STARTSIZE, mxConstants.DEFAULT_MARKERSIZE));
					offset = drawMarker(group, marker, pt, p0, size, tmpStroke, strokeColor);
				}
				else
				{
					double dx = pt.X - p0.X;
					double dy = pt.Y - p0.Y;

					double dist = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));
					double nx = dx * strokeWidth / dist;
					double ny = dy * strokeWidth / dist;

					offset = new mxPoint(nx / 2, ny / 2);
				}

				// Applies offset to the point
				if (offset != null)
				{
					p0 = (mxPoint) p0.clone();
					p0.X = p0.X + offset.X;
					p0.Y = p0.Y + offset.Y;

					offset = null;
				}

				// Draws the end marker
				marker = style[mxConstants.STYLE_ENDARROW];

				pt = pts[pts.Count - 2];
				mxPoint pe = pts[pts.Count - 1];

				if (marker != null)
				{
					float size = (mxUtils.getFloat(style, mxConstants.STYLE_ENDSIZE, mxConstants.DEFAULT_MARKERSIZE));
					offset = drawMarker(group, marker, pt, pe, size, tmpStroke, strokeColor);
				}
				else
				{
					double dx = pt.X - p0.X;
					double dy = pt.Y - p0.Y;

					double dist = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));
					double nx = dx * strokeWidth / dist;
					double ny = dy * strokeWidth / dist;

					offset = new mxPoint(nx / 2, ny / 2);
				}

				// Applies offset to the point
				if (offset != null)
				{
					pe = (mxPoint) pe.clone();
					pe.X = pe.X + offset.X;
					pe.Y = pe.Y + offset.Y;

					offset = null;
				}

				// Draws the line segments
				pt = p0;
				string d = "M " + pt.X + " " + pt.Y;

				for (int i = 1; i < pts.Count - 1; i++)
				{
					pt = pts[i];
					d += " L " + pt.X + " " + pt.Y;
				}

				d += " L " + pe.X + " " + pe.Y;

				path.SetAttribute("d", d);
				path.SetAttribute("stroke", strokeColor);
				path.SetAttribute("fill", "none");
				path.SetAttribute("stroke-width", strokeWidth.ToString());

				if (mxUtils.isTrue(style, mxConstants.STYLE_DASHED))
				{
					path.SetAttribute("stroke-dasharray", "3, 3");
				}

                group.AppendChild(path);
				appendSvgElement(group);
			}

			return group;
		}

		/// <summary>
		/// Draws the specified marker as a child path in the given parent.
		/// </summary>
		public virtual mxPoint drawMarker(Element parent, object type, mxPoint p0, mxPoint pe, float size, float strokeWidth, string color)
		{
			mxPoint offset = null;

			// Computes the norm and the inverse norm
			double dx = pe.X - p0.X;
			double dy = pe.Y - p0.Y;

			double dist = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));
			double absSize = size * scale;
			double nx = dx * absSize / dist;
			double ny = dy * absSize / dist;

			pe = (mxPoint) pe.clone();
			pe.X = pe.X - nx * strokeWidth / (2 * size);
			pe.Y = pe.Y - ny * strokeWidth / (2 * size);

			nx *= 0.5 + strokeWidth / 2;
			ny *= 0.5 + strokeWidth / 2;

            Element path = document.CreateElement("path");
			path.SetAttribute("stroke-width", (strokeWidth * scale).ToString());
			path.SetAttribute("stroke", color);
			path.SetAttribute("fill", color);

			string d = null;

			if (type.Equals(mxConstants.ARROW_CLASSIC) || type.Equals(mxConstants.ARROW_BLOCK))
			{
				d = "M " + pe.X + " " + pe.Y + " L " + (pe.X - nx - ny / 2) + " " + (pe.Y - ny + nx / 2) + ((!type.Equals(mxConstants.ARROW_CLASSIC)) ? "" : " L " + (pe.X - nx * 3 / 4) + " " + (pe.Y - ny * 3 / 4)) + " L " + (pe.X + ny / 2 - nx) + " " + (pe.Y - ny - nx / 2) + " z";
			}
			else if (type.Equals(mxConstants.ARROW_OPEN))
			{
				nx *= 1.2;
				ny *= 1.2;

				d = "M " + (pe.X - nx - ny / 2) + " " + (pe.Y - ny + nx / 2) + " L " + (pe.X - nx / 6) + " " + (pe.Y - ny / 6) + " L " + (pe.X + ny / 2 - nx) + " " + (pe.Y - ny - nx / 2) + " M " + pe.X + " " + pe.Y;
				path.SetAttribute("fill", "none");
			}
			else if (type.Equals(mxConstants.ARROW_OVAL))
			{
				nx *= 1.2;
				ny *= 1.2;
				absSize *= 1.2;

				d = "M " + (pe.X - ny / 2) + " " + (pe.Y + nx / 2) + " a " + (absSize / 2) + " " + (absSize / 2) + " 0  1,1 " + (nx / 8) + " " + (ny / 8) + " z";
			}
			else if (type.Equals(mxConstants.ARROW_DIAMOND))
			{
				d = "M " + (pe.X + nx / 2) + " " + (pe.Y + ny / 2) + " L " + (pe.X - ny / 2) + " " + (pe.Y + nx / 2) + " L " + (pe.X - nx / 2) + " " + (pe.Y - ny / 2) + " L " + (pe.X + ny / 2) + " " + (pe.Y - nx / 2) + " z";
			}

			if (!string.ReferenceEquals(d, null))
			{
				path.SetAttribute("d", d);

                parent.AppendChild(path);
			}

			return offset;
		}

		/// <summary>
		/// Draws the specified text either using drawHtmlString or using drawString.
		/// </summary>
		/// <param name="text"> Text to be painted. </param>
		/// <param name="x"> X-coordinate of the text. </param>
		/// <param name="y"> Y-coordinate of the text. </param>
		/// <param name="w"> Width of the text. </param>
		/// <param name="h"> Height of the text. </param>
		/// <param name="style"> Style to be used for painting the text. </param>
		public virtual object drawText(string text, int x, int y, int w, int h, IDictionary<string, object> style)
		{
			Element elem = null;
			string fontColor = mxUtils.getString(style, mxConstants.STYLE_FONTCOLOR, "black");
			string fontFamily = mxUtils.getString(style, mxConstants.STYLE_FONTFAMILY, mxConstants.DEFAULT_FONTFAMILIES);
			int fontSize = (int)(mxUtils.getInt(style, mxConstants.STYLE_FONTSIZE, mxConstants.DEFAULT_FONTSIZE) * scale);

			if (!string.ReferenceEquals(text, null) && text.Length > 0)
			{
                elem = document.CreateElement("text");

				// Applies the opacity
				float opacity = mxUtils.getFloat(style, mxConstants.STYLE_TEXT_OPACITY, 100);

				if (opacity != 100)
				{
					string value = (opacity / 100).ToString();
					elem.SetAttribute("fill-opacity", value);
					elem.SetAttribute("stroke-opacity", value);
				}

				elem.SetAttribute("text-anchor", "middle");
				elem.SetAttribute("font-weight", "normal");
				elem.SetAttribute("font-decoration", "none");

				elem.SetAttribute("font-size", fontSize.ToString());
				elem.SetAttribute("font-family", fontFamily);
				elem.SetAttribute("fill", fontColor);

                string[] lines = text.Split('\n');//text.Split("\n", true);

                y += fontSize + (h - lines.Length * (fontSize + mxConstants.LINESPACING)) / 2 - 2;

				for (int i = 0; i < lines.Length; i++)
				{
                    Element tspan = document.CreateElement("tspan");

                    tspan.SetAttribute("x", (x + w / 2).ToString());
                    tspan.SetAttribute("y", y.ToString());

                    tspan.AppendChild(document.CreateTextNode(lines[i]));
                    elem.AppendChild(tspan);

					y += fontSize + mxConstants.LINESPACING;
				}

				appendSvgElement(elem);
			}

			return elem;
		}

	}

}