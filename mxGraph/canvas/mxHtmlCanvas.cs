using System;
using System.Collections.Generic;

using mxGraph.util;
using mxGraph.view;

/// <summary>
/// $Id: mxHtmlCanvas.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.canvas
{


	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;

	

	/// <summary>
	/// An implementation of a canvas that uses HTML for painting.
	/// </summary>
	public class mxHtmlCanvas : mxBasicCanvas
	{

		/// <summary>
		/// Holds the HTML document that represents the canvas.
		/// </summary>
		protected internal Document document;

		/// <summary>
		/// Constructs a new HTML canvas for the specified dimension and scale.
		/// </summary>
		public mxHtmlCanvas() : this(null)
		{
		}

		/// <summary>
		/// Constructs a new HTML canvas for the specified bounds, scale and
		/// background color.
		/// </summary>
		public mxHtmlCanvas(Document document)
		{
			Document = document;
		}

		/// 
		public virtual void appendHtmlElement(Element node)
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

			if (state.AbsolutePointCount > 1)
			{
				IList<mxPoint> pts = state.AbsolutePoints;

                // Transpose all points by cloning into a new array
                pts = mxUtils.translatePoints(pts, translate.X, translate.Y);
				drawLine(pts, style);
			}
			else
			{
				int x = (int) state.X + translate.X;
				int y = (int) state.Y + translate.Y;
				int w = (int) state.Width;
				int h = (int) state.Height;

				if (!mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_SWIMLANE))
				{
					drawShape(x, y, w, h, style);
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
						drawShape(x, y, w, start, style);
						drawShape(x, y + start, w, h - start, cloned);
					}
					else
					{
						drawShape(x, y, start, h, style);
						drawShape(x + start, y, w - start, h, cloned);
					}
				}
			}

			return null;
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

            Element elem = document.CreateElement("div");

			if (shape.Equals(mxConstants.SHAPE_LINE))
			{
				string direction = mxUtils.getString(style, mxConstants.STYLE_DIRECTION, mxConstants.DIRECTION_EAST);

				if (direction.Equals(mxConstants.DIRECTION_EAST) || direction.Equals(mxConstants.DIRECTION_WEST))
				{
					y = (int)Math.Round(y + h / 2.0);
					h = 1;
				}
				else
				{
					x = (int)Math.Round(y + w / 2.0);
					w = 1;
				}
			}

			if (mxUtils.isTrue(style, mxConstants.STYLE_SHADOW, false) && !string.ReferenceEquals(fillColor, null))
			{
                Element shadow = (Element) elem.CloneNode(true);

				string styleValue = "overflow:hidden;position:absolute;" + "left:" + (x + mxConstants.SHADOW_OFFSETX).ToString() + "px;" + "top:" + (y + mxConstants.SHADOW_OFFSETY).ToString() + "px;" + "width:" + w.ToString() + "px;" + "height:" + h.ToString() + "px;background:" + mxConstants.W3C_SHADOWCOLOR + ";border-style:solid;border-color:" + mxConstants.W3C_SHADOWCOLOR + ";border-width:" + Math.Round(strokeWidth).ToString() + ";";
                shadow.SetAttribute("style", styleValue);

				appendHtmlElement(shadow);
			}

			if (shape.Equals(mxConstants.SHAPE_IMAGE))
			{
				string img = getImageForStyle(style);

				if (!string.ReferenceEquals(img, null))
				{
                    elem = document.CreateElement("img");
                    elem.SetAttribute("border", "0");
                    elem.SetAttribute("src", img);
				}
			}

			// TODO: Draw other shapes. eg. SHAPE_LINE here

			string s = "overflow:hidden;position:absolute;" + "left:" + x.ToString() + "px;" + "top:" + y.ToString() + "px;" + "width:" + w.ToString() + "px;" + "height:" + h.ToString() + "px;background:" + fillColor + ";" + ";border-style:solid;border-color:" + strokeColor + ";border-width:" + Math.Round(strokeWidth).ToString() + ";";
            elem.SetAttribute("style", s);

			appendHtmlElement(elem);

			return elem;
		}

		/// <summary>
		/// Draws the given lines as segments between all points of the given list
		/// of mxPoints.
		/// </summary>
		/// <param name="pts"> List of points that define the line. </param>
		/// <param name="style"> Style to be used for painting the line. </param>
		public virtual void drawLine(IList<mxPoint> pts, IDictionary<string, object> style)
		{
			string strokeColor = mxUtils.getString(style, mxConstants.STYLE_STROKECOLOR);
			int strokeWidth = (int)(mxUtils.getInt(style, mxConstants.STYLE_STROKEWIDTH, 1) * scale);

			if (!string.ReferenceEquals(strokeColor, null) && strokeWidth > 0)
			{

				mxPoint last = pts[0];

				for (int i = 1; i < pts.Count; i++)
				{
					mxPoint pt = pts[i];

					drawSegment((int) last.X, (int) last.Y, (int) pt.X, (int) pt.Y, strokeColor, strokeWidth);

					last = pt;
				}
			}
		}

		/// <summary>
		/// Draws the specified segment of a line.
		/// </summary>
		/// <param name="x0"> X-coordinate of the start point. </param>
		/// <param name="y0"> Y-coordinate of the start point. </param>
		/// <param name="x1"> X-coordinate of the end point. </param>
		/// <param name="y1"> Y-coordinate of the end point. </param>
		/// <param name="strokeColor"> Color of the stroke to be painted. </param>
		/// <param name="strokeWidth"> Width of the stroke to be painted. </param>
		protected internal virtual void drawSegment(int x0, int y0, int x1, int y1, string strokeColor, int strokeWidth)
		{
			int tmpX = Math.Min(x0, x1);
			int tmpY = Math.Min(y0, y1);

			int width = Math.Max(x0, x1) - tmpX;
			int height = Math.Max(y0, y1) - tmpY;

			x0 = tmpX;
			y0 = tmpY;

			if (width == 0 || height == 0)
			{
				string s = "overflow:hidden;position:absolute;" + "left:" + x0.ToString() + "px;" + "top:" + y0.ToString() + "px;" + "width:" + width.ToString() + "px;" + "height:" + height.ToString() + "px;" + "border-color:" + strokeColor + ";" + "border-style:solid;" + "border-width:1 1 0 0px;";

                Element elem = document.CreateElement("div");
                elem.SetAttribute("style", s);

				appendHtmlElement(elem);
			}
			else
			{
				int x = x0 + (x1 - x0) / 2;

				drawSegment(x0, y0, x, y0, strokeColor, strokeWidth);
				drawSegment(x, y0, x, y1, strokeColor, strokeWidth);
				drawSegment(x, y1, x1, y1, strokeColor, strokeWidth);
			}
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
			appendHtmlElement(table);

			return table;
		}

	}

}