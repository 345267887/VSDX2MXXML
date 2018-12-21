using System;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// $Id: mxVmlCanvas.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.canvas
{


	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;


    using mxConstants = mxGraph.util.mxConstants;
	using mxPoint = mxGraph.util.mxPoint;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxUtils = mxGraph.util.mxUtils;
	using mxCellState = mxGraph.view.mxCellState;

	/// <summary>
	/// An implementation of a canvas that uses VML for painting.
	/// </summary>
	public class mxVmlCanvas : mxBasicCanvas
	{

		/// <summary>
		/// Holds the HTML document that represents the canvas.
		/// </summary>
		protected internal Document document;

		/// <summary>
		/// Constructs a new VML canvas for the specified dimension and scale.
		/// </summary>
		public mxVmlCanvas() : this(null)
		{
		}

		/// <summary>
		/// Constructs a new VML canvas for the specified bounds, scale and
		/// background color.
		/// </summary>
		public mxVmlCanvas(Document document)
		{
			Document = document;
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


		/// 
		public virtual void appendVmlElement(Element node)
		{
			if (document != null)
			{
				Node body = document.DocumentElement.FirstChild.NextSibling;

				if (body != null)
				{
                    body.AppendChild(node);
				}
			}

		}

		/* (non-Javadoc)
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
                Element strokeNode = document.CreateElement("v:stroke");

				// Draws the markers
				string start = mxUtils.getString(style, mxConstants.STYLE_STARTARROW);
				string end = mxUtils.getString(style, mxConstants.STYLE_ENDARROW);

				if (!string.ReferenceEquals(start, null) || !string.ReferenceEquals(end, null))
				{
					if (!string.ReferenceEquals(start, null))
					{
                        strokeNode.SetAttribute("startarrow", start);

						string startWidth = "medium";
						string startLength = "medium";
						double startSize = mxUtils.getFloat(style, mxConstants.STYLE_STARTSIZE, mxConstants.DEFAULT_MARKERSIZE) * scale;

						if (startSize < 6)
						{
							startWidth = "narrow";
							startLength = "short";
						}
						else if (startSize > 10)
						{
							startWidth = "wide";
							startLength = "long";
						}

                        strokeNode.SetAttribute("startarrowwidth", startWidth);
                        strokeNode.SetAttribute("startarrowlength", startLength);
					}

					if (!string.ReferenceEquals(end, null))
					{
                        strokeNode.SetAttribute("endarrow", end);

						string endWidth = "medium";
						string endLength = "medium";
						double endSize = mxUtils.getFloat(style, mxConstants.STYLE_ENDSIZE, mxConstants.DEFAULT_MARKERSIZE) * scale;

						if (endSize < 6)
						{
							endWidth = "narrow";
							endLength = "short";
						}
						else if (endSize > 10)
						{
							endWidth = "wide";
							endLength = "long";
						}

                        strokeNode.SetAttribute("endarrowwidth", endWidth);
						strokeNode.SetAttribute("endarrowlength", endLength);
					}
				}

				if (mxUtils.isTrue(style, mxConstants.STYLE_DASHED))
				{
					strokeNode.SetAttribute("dashstyle", "2 2");
				}

                elem.AppendChild(strokeNode);
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

					if (mxUtils.isTrue(style, mxConstants.STYLE_DASHED))
					{
                        Element strokeNode = document.CreateElement("v:stroke");
                        strokeNode.SetAttribute("dashstyle", "2 2");
                        elem.AppendChild(strokeNode);
					}
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

			if (drawLabels && bounds != null)
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
			string fillColor = mxUtils.getString(style, mxConstants.STYLE_FILLCOLOR);
			string strokeColor = mxUtils.getString(style, mxConstants.STYLE_STROKECOLOR);
			float strokeWidth = (float)(mxUtils.getFloat(style, mxConstants.STYLE_STROKEWIDTH, 1) * scale);

			// Draws the shape
			string shape = mxUtils.getString(style, mxConstants.STYLE_SHAPE);
			Element elem = null;

			if (shape.Equals(mxConstants.SHAPE_IMAGE))
			{
				string img = getImageForStyle(style);

				if (!string.ReferenceEquals(img, null))
				{
                    elem = document.CreateElement("v:img");
                    elem.SetAttribute("src", img);
				}
			}
			else if (shape.Equals(mxConstants.SHAPE_LINE))
			{
				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, mxConstants.DIRECTION_EAST);
				string points = null;

				if (direction.Equals(mxConstants.DIRECTION_EAST) || direction.Equals(mxConstants.DIRECTION_WEST))
				{
					int mid = (int)Math.Round(h / 2.0);
					points = "m 0 " + mid + " l " + w + " " + mid;
				}
				else
				{
					int mid = (int)Math.Round(w / 2.0);
					points = "m " + mid + " 0 L " + mid + " " + h;
				}

                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);
				elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_ELLIPSE))
			{
                elem = document.CreateElement("v:oval");
			}
			else if (shape.Equals(mxConstants.SHAPE_DOUBLE_ELLIPSE))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);
				int inset = (int)((3 + strokeWidth) * scale);

				string points = "ar 0 0 " + w + " " + h + " 0 " + (h / 2) + " " + (w / 2) + " " + (h / 2) + " e ar " + inset + " " + inset + " " + (w - inset) + " " + (h - inset) + " 0 " + (h / 2) + " " + (w / 2) + " " + (h / 2);

                elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_RHOMBUS))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);

				string points = "m " + (w / 2) + " 0 l " + w + " " + (h / 2) + " l " + (w / 2) + " " + h + " l 0 " + (h / 2);

                elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_TRIANGLE))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);

				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, "");
				string points = null;

				if (direction.Equals(mxConstants.DIRECTION_NORTH))
				{
					points = "m 0 " + h + " l " + (w / 2) + " 0 " + " l " + w + " " + h;
				}
				else if (direction.Equals(mxConstants.DIRECTION_SOUTH))
				{
					points = "m 0 0 l " + (w / 2) + " " + h + " l " + w + " 0";
				}
				else if (direction.Equals(mxConstants.DIRECTION_WEST))
				{
					points = "m " + w + " 0 l " + w + " " + (h / 2) + " l " + w + " " + h;
				}
				else
				{
				// east
					points = "m 0 0 l " + w + " " + (h / 2) + " l 0 " + h;
				}

                elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_HEXAGON))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);

				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, "");
				string points = null;

				if (direction.Equals(mxConstants.DIRECTION_NORTH) || direction.Equals(mxConstants.DIRECTION_SOUTH))
				{
					points = "m " + (int)(0.5 * w) + " 0 l " + w + " " + (int)(0.25 * h) + " l " + w + " " + (int)(0.75 * h) + " l " + (int)(0.5 * w) + " " + h + " l 0 " + (int)(0.75 * h) + " l 0 " + (int)(0.25 * h);
				}
				else
				{
					points = "m " + (int)(0.25 * w) + " 0 l " + (int)(0.75 * w) + " 0 l " + w + " " + (int)(0.5 * h) + " l " + (int)(0.75 * w) + " " + h + " l " + (int)(0.25 * w) + " " + h + " l 0 " + (int)(0.5 * h);
				}

                elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_CLOUD))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);

				string points = "m " + (int)(0.25 * w) + " " + (int)(0.25 * h) + " c " + (int)(0.05 * w) + " " + (int)(0.25 * h) + " 0 " + (int)(0.5 * h) + " " + (int)(0.16 * w) + " " + (int)(0.55 * h) + " c 0 " + (int)(0.66 * h) + " " + (int)(0.18 * w) + " " + (int)(0.9 * h) + " " + (int)(0.31 * w) + " " + (int)(0.8 * h) + " c " + (int)(0.4 * w) + " " + (h) + " " + (int)(0.7 * w) + " " + (h) + " " + (int)(0.8 * w) + " " + (int)(0.8 * h) + " c " + (w) + " " + (int)(0.8 * h) + " " + (w) + " " + (int)(0.6 * h) + " " + (int)(0.875 * w) + " " + (int)(0.5 * h) + " c " + (w) + " " + (int)(0.3 * h) + " " + (int)(0.8 * w) + " " + (int)(0.1 * h) + " " + (int)(0.625 * w) + " " + (int)(0.2 * h) + " c " + (int)(0.5 * w) + " " + (int)(0.05 * h) + " " + (int)(0.3 * w) + " " + (int)(0.05 * h) + " " + (int)(0.25 * w) + " " + (int)(0.25 * h);

                elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_ACTOR))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);

				double width3 = w / 3;
				string points = "m 0 " + (h) + " C 0 " + (3 * h / 5) + " 0 " + (2 * h / 5) + " " + (w / 2) + " " + (2 * h / 5) + " c " + (int)(w / 2 - width3) + " " + (2 * h / 5) + " " + (int)(w / 2 - width3) + " 0 " + (w / 2) + " 0 c " + (int)(w / 2 + width3) + " 0 " + (int)(w / 2 + width3) + " " + (2 * h / 5) + " " + (w / 2) + " " + (2 * h / 5) + " c " + (w) + " " + (2 * h / 5) + " " + (w) + " " + (3 * h / 5) + " " + (w) + " " + (h);

                elem.SetAttribute("path", points + " x e");
			}
			else if (shape.Equals(mxConstants.SHAPE_CYLINDER))
			{
                elem = document.CreateElement("v:shape");
                elem.SetAttribute("coordsize", w + " " + h);

				double dy = Math.Min(40, Math.Floor(h / 5.0));
				string points = "m 0 " + (int)(dy) + " C 0 " + (int)(dy / 3) + " " + (w) + " " + (int)(dy / 3) + " " + (w) + " " + (int)(dy) + " L " + (w) + " " + (int)(h - dy) + " C " + (w) + " " + (int)(h + dy / 3) + " 0 " + (int)(h + dy / 3) + " 0 " + (int)(h - dy) + " x e" + " m 0 " + (int)(dy) + " C 0 " + (int)(2 * dy) + " " + (w) + " " + (int)(2 * dy) + " " + (w) + " " + (int)(dy);

                elem.SetAttribute("path", points + " e");
			}
			else
			{
				if (mxUtils.isTrue(style, mxConstants.STYLE_ROUNDED, false))
				{
                    elem = document.CreateElement("v:roundrect");
                    elem.SetAttribute("arcsize", (mxConstants.RECTANGLE_ROUNDING_FACTOR * 100) + "%");
				}
				else
				{
                    elem = document.CreateElement("v:rect");
				}
			}

			string s = "position:absolute;left:" + x.ToString() + "px;top:" + y.ToString() + "px;width:" + w.ToString() + "px;height:" + h.ToString() + "px;";

			// Applies rotation
			double rotation = mxUtils.getDouble(style, mxConstants.STYLE_ROTATION);

			if (rotation != 0)
			{
				s += "rotation:" + rotation + ";";
			}

            elem.SetAttribute("style", s);

			// Adds the shadow element
			if (mxUtils.isTrue(style, mxConstants.STYLE_SHADOW, false) && !string.ReferenceEquals(fillColor, null))
			{
                Element shadow = document.CreateElement("v:shadow");
                shadow.SetAttribute("on", "true");
                shadow.SetAttribute("color", mxConstants.W3C_SHADOWCOLOR);
                elem.AppendChild(shadow);
			}

			float opacity = mxUtils.getFloat(style, mxConstants.STYLE_OPACITY, 100);

			// Applies opacity to fill
			if (!string.ReferenceEquals(fillColor, null))
			{
                Element fill = document.CreateElement("v:fill");
                fill.SetAttribute("color", fillColor);

				if (opacity != 100)
				{
                    fill.SetAttribute("opacity", (opacity / 100).ToString());
				}

                elem.AppendChild(fill);
			}
			else
			{
                elem.SetAttribute("filled", "false");
			}

			// Applies opacity to stroke
			if (!string.ReferenceEquals(strokeColor, null))
			{
                elem.SetAttribute("strokecolor", strokeColor);
                Element stroke = document.CreateElement("v:stroke");

				if (opacity != 100)
				{
                    stroke.SetAttribute("opacity", (opacity / 100).ToString());
				}

                elem.AppendChild(stroke);
			}
			else
			{
                elem.SetAttribute("stroked", "false");
			}

            elem.SetAttribute("strokeweight", strokeWidth.ToString() + "pt");
			appendVmlElement(elem);

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
			string strokeColor = mxUtils.getString(style, mxConstants.STYLE_STROKECOLOR);
			float strokeWidth = (float)(mxUtils.getFloat(style, mxConstants.STYLE_STROKEWIDTH, 1) * scale);

            Element elem = document.CreateElement("v:shape");

			if (!string.ReferenceEquals(strokeColor, null) && strokeWidth > 0)
			{
				mxPoint pt = pts[0];
				Rectangle r = new Rectangle(pt.Point,new Size {Width=0,Height=0 });

				string d = "m " + Math.Round(pt.X) + " " + Math.Round(pt.Y);

				for (int i = 1; i < pts.Count; i++)
				{
					pt = pts[i];
					d += " l " + Math.Round(pt.X) + " " + Math.Round(pt.Y);

					r = Rectangle.Union(r,(new Rectangle(pt.Point, new Size { Width = 0, Height = 0 })));
				}

                elem.SetAttribute("path", d);
				elem.SetAttribute("filled", "false");
				elem.SetAttribute("strokecolor", strokeColor);
				elem.SetAttribute("strokeweight", strokeWidth.ToString() + "pt");

                string s = "position:absolute;" + "left:" + r.X.ToString() + "px;" + "top:" + r.Y.ToString() + "px;" + "width:" + r.Width.ToString() + "px;" + "height:" + r.Height.ToString() + "px;";
				elem.SetAttribute("style", s);

				elem.SetAttribute("coordorigin", r.X.ToString() + " " + r.Y.ToString());
                elem.SetAttribute("coordsize", r.Width.ToString() + " " + r.Height.ToString());
			}

			appendVmlElement(elem);

			return elem;
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
		public virtual Element drawText(string text, int x, int y, int w, int h, IDictionary<string, object> style)
		{
			Element table = mxUtils.createTable(document, text, x, y, w, h, scale, style);
			appendVmlElement(table);

			return table;
		}

	}

}