using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using mxGraph.io;
using mxGraph.model;
using mxGraph.view;
using mxGraph;

/// <summary>
/// $Id: mxUtils.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{



	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;
    using NamedNodeMap = System.Xml.XmlNamedNodeMap;//org.w3c.dom.NamedNodeMap;
	using Node = System.Xml.XmlNode;



	/// <summary>
	/// Contains various helper methods for use with mxGraph.
	/// </summary>
	public class mxUtils
	{

		/// <summary>
		/// Static Graphics used for Font Metrics.
		/// </summary>
		[NonSerialized]
		protected internal static Graphics fontGraphics;

		// Creates a renderer for HTML markup (only possible in
		// non-headless environment)
		static mxUtils()
		{
			try
			{
                //fontGraphics = (new BufferedImage(1, 1, BufferedImage.TYPE_INT_RGB)).Graphics;

                Image img = Image.FromFile("g1.jpg");//建立Image对象
                fontGraphics = Graphics.FromImage(img);//创建Graphics对象
            }
			catch (Exception)
			{
				// ignore
			}
		}

        /// <summary>
        /// Returns the size for the given label. If isHtml is true then any HTML
        /// markup in the label is computed as HTML and all newlines inside the HTML
        /// body are converted into linebreaks.
        /// </summary>
        public static mxRectangle getLabelSize(string label, IDictionary<string, object> style, bool isHtml, int width)
        {
            mxRectangle size=new mxRectangle ();

            if (isHtml)
            {
                //size = getSizeForHtml(getBodyMarkup(label, true), style);
            }
            else
            {
                //size = getSizeForString(label, getFont(style), width);
            }

            return size;
        }

        /// <summary>
        /// Returns the body part of the given HTML markup.
        /// </summary>
        public static string getBodyMarkup(string markup, bool replaceLinefeeds)
		{
			string lowerCase = markup.ToLower();
			int bodyStart = lowerCase.IndexOf("<body>", StringComparison.Ordinal);

			if (bodyStart >= 0)
			{
				bodyStart += 7;
				int bodyEnd = lowerCase.LastIndexOf("</body>", StringComparison.Ordinal);

				if (bodyEnd > bodyStart)
				{
					markup = markup.Substring(bodyStart, bodyEnd - bodyStart).Trim();
				}
			}

			if (replaceLinefeeds)
			{
                //markup = markup.replaceAll("\n", "<br>");
                markup = markup.Replace("\n", "<br>");
            }

			return markup;
		}

		/// <summary>
		/// Returns the paint bounds for the given label.
		/// </summary>
		public static mxRectangle getLabelPaintBounds(string label, IDictionary<string, object> style, bool isHtml, mxPoint offset, mxRectangle vertexBounds, double scale)
		{
			bool horizontal = mxUtils.isTrue(style, mxConstants.STYLE_HORIZONTAL, true);
			int w = 0;

			if (vertexBounds != null && getString(style, mxConstants.STYLE_WHITE_SPACE, "nowrap").Equals("wrap"))
			{
				if (horizontal)
				{
					w = (int)((vertexBounds.Width / scale) - 2 * mxConstants.LABEL_INSET - 2 * mxUtils.getDouble(style, mxConstants.STYLE_SPACING) - mxUtils.getDouble(style, mxConstants.STYLE_SPACING_LEFT) - mxUtils.getDouble(style, mxConstants.STYLE_SPACING_RIGHT));
				}
				else
				{
					w = (int)((vertexBounds.Height / scale) - 2 * mxConstants.LABEL_INSET - 2 * mxUtils.getDouble(style, mxConstants.STYLE_SPACING) - mxUtils.getDouble(style, mxConstants.STYLE_SPACING_TOP) + mxUtils.getDouble(style, mxConstants.STYLE_SPACING_BOTTOM));
				}
			}

			mxRectangle size = mxUtils.getLabelSize(label, style, isHtml, w);

			double x = offset.X;
			double y = offset.Y;
			double width = 0;
			double height = 0;

			if (vertexBounds != null)
			{
				x += vertexBounds.X;
				y += vertexBounds.Y;

				if (mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_SWIMLANE))
				{
					// Limits the label to the swimlane title
					double start = mxUtils.getDouble(style, mxConstants.STYLE_STARTSIZE, mxConstants.DEFAULT_STARTSIZE) * scale;

					if (horizontal)
					{
						width += vertexBounds.Width;
						height += start;
					}
					else
					{
						width += start;
						height += vertexBounds.Height;
					}
				}
				else
				{
					width += vertexBounds.Width;
					height += vertexBounds.Height;
				}
			}

			return mxUtils.getScaledLabelBounds(x, y, size, width, height, style, scale);
		}

		/// <summary>
		/// Returns the bounds for a label for the given location and size, taking
		/// into account the alignment and spacing in the specified style, as well as
		/// the width and height of the rectangle that contains the label. (For edge
		/// labels this width and height is 0.) The scale is used to scale the given
		/// size and the spacings in the specified style.
		/// </summary>
		public static mxRectangle getScaledLabelBounds(double x, double y, mxRectangle size, double outerWidth, double outerHeight, IDictionary<string, object> style, double scale)
		{
			double inset = mxConstants.LABEL_INSET * scale;

			// Scales the size of the label
			// FIXME: Correct rounded font size and not-rounded scale
			double width = size.Width * scale + 2 * inset;
			double height = size.Height * scale + 2 * inset;

			// Gets the global spacing and orientation
			bool horizontal = isTrue(style, mxConstants.STYLE_HORIZONTAL, true);
			int spacing = (int)(getInt(style, mxConstants.STYLE_SPACING) * scale);

			// Gets the alignment settings
			object align = getString(style, mxConstants.STYLE_ALIGN, mxConstants.ALIGN_CENTER);
			object valign = getString(style, mxConstants.STYLE_VERTICAL_ALIGN, mxConstants.ALIGN_MIDDLE);

			// Gets the vertical spacing
			int top = (int)(getInt(style, mxConstants.STYLE_SPACING_TOP) * scale);
			int bottom = (int)(getInt(style, mxConstants.STYLE_SPACING_BOTTOM) * scale);

			// Gets the horizontal spacing
			int left = (int)(getInt(style, mxConstants.STYLE_SPACING_LEFT) * scale);
			int right = (int)(getInt(style, mxConstants.STYLE_SPACING_RIGHT) * scale);

			// Applies the orientation to the spacings and dimension
			if (!horizontal)
			{
				int tmp = top;
				top = right;
				right = bottom;
				bottom = left;
				left = tmp;

				double tmp2 = width;
				width = height;
				height = tmp2;
			}

			// Computes the position of the label for the horizontal alignment
			if ((horizontal && align.Equals(mxConstants.ALIGN_CENTER)) || (!horizontal && valign.Equals(mxConstants.ALIGN_MIDDLE)))
			{
				x += (outerWidth - width) / 2 + left - right;
			}
			else if ((horizontal && align.Equals(mxConstants.ALIGN_RIGHT)) || (!horizontal && valign.Equals(mxConstants.ALIGN_BOTTOM)))
			{
				x += outerWidth - width - spacing - right;
			}
			else
			{
				x += spacing + left;
			}

			// Computes the position of the label for the vertical alignment
			if ((!horizontal && align.Equals(mxConstants.ALIGN_CENTER)) || (horizontal && valign.Equals(mxConstants.ALIGN_MIDDLE)))
			{
				y += (outerHeight - height) / 2 + top - bottom;
			}
			else if ((!horizontal && align.Equals(mxConstants.ALIGN_LEFT)) || (horizontal && valign.Equals(mxConstants.ALIGN_BOTTOM)))
			{
				y += outerHeight - height - spacing - bottom;
			}
			else
			{
				y += spacing + top;
			}

			return new mxRectangle(x, y, width, height);
		}

		/// <summary>
		/// Returns an <mxRectangle> with the size (width and height in pixels) of
		/// the given string.
		/// </summary>
		/// <param name="text">
		///            String whose size should be returned. </param>
		/// <param name="font">
		///            Font to be used for the computation. </param>
		//public static mxRectangle getSizeForString(string text, Font font, int width)
		//{
		//	FontRenderContext frc = new FontRenderContext(null, false, false);
		//	FontMetrics metrics = null;

		//	if (fontGraphics != null)
		//	{
		//		metrics = fontGraphics.getFontMetrics(font);
		//	}

		//	double lineHeight = mxConstants.LINESPACING;

		//	if (metrics != null)
		//	{
		//		lineHeight += metrics.Height;
		//	}
		//	else
		//	{
		//		lineHeight += font.Size2D * 1.27;
		//	}

		//	string[] lines;

		//	if (width > 0)
		//	{
		//		// The lines for wrapping within the given width are calcuated for no
		//		// scale. The reason for this is the granularity of actual displayed 
		//		// font can cause the displayed lines to change based on scale. A factor 
		//		// is used to allow for different overalls widths, it ensures the largest 
		//		// font size/scale factor still stays within the bounds. All this ensures
		//		// the wrapped lines are constant overing scaling, at the expense the 
		//		// label bounds will vary.
		//		lines = mxUtils.wordWrap(text, metrics, (int)(width * mxConstants.LABEL_SCALE_BUFFER));
		//	}
		//	else
		//	{
		//		lines = text.Split("\n", true);
		//	}

		//	Rectangle2D boundingBox = null;

		//	for (int i = 0; i < lines.Length; i++)
		//	{
		//		Rectangle2D bounds = font.getStringBounds(lines[i], frc);

		//		if (boundingBox == null)
		//		{
		//			boundingBox = bounds;
		//		}
		//		else
		//		{
		//			boundingBox.setFrame(0, 0, Math.Max(boundingBox.Width, bounds.Width), boundingBox.Height + lineHeight);
		//		}
		//	}

		//	return new mxRectangle(boundingBox);
		//}

		/// <summary>
		/// Returns the specified text in lines that fit within the specified
		/// width when the specified font metrics are applied to the text </summary>
		/// <param name="text"> the text to wrap </param>
		/// <param name="metrics"> the font metrics to calculate the text size for </param>
		/// <param name="width"> the width that the text must fit within </param>
		/// <returns> the input text split in lines that fit the specified width </returns>
		//public static string[] wordWrap(string text, FontMetrics metrics, int width)
		//{
		//	List<string> result = new List<string>();
		//	// First split the processing into lines already delimited by
		//	// newlines. We want the result to retain all newlines in position.
		//	string[] lines = text.Split("\n", true);

		//	for (int i = 0; i < lines.Length; i++)
		//	{
		//		int lineWidth = 0; // the display width of the current line
		//		int charCount = 0; // keeps count of current position in the line
		//		StringBuilder currentLine = new StringBuilder();

		//		// Split the words of the current line by spaces and tabs
		//		// The words are trimmed of tabs, space and newlines, therefore
		//		string[] words = lines[i].Split("\\s+", true);

		//		// Need to a form a stack of the words in reverse order
		//		// This is because if a word is split during the process 
		//		// the remainder of the word is added to the front of the 
		//		// stack and processed next
		//		Stack<string> wordStack = new Stack<string>();

		//		for (int j = words.Length - 1; j >= 0; j--)
		//		{
		//			wordStack.Push(words[j]);
		//		}

		//		while (wordStack.Count > 0)
		//		{
		//			string word = wordStack.Pop();

		//			// Work out what whitespace exists before this word.
		//			// and add the width of the whitespace to the calculation
		//			int whitespaceCount = 0;

		//			if (word.Length > 0)
		//			{
		//				// Concatenate any preceeding whitespace to the
		//				// word and calculate the number of characters of that
		//				// whitespace
		//				char firstWordLetter = word[0];
		//				int letterIndex = lines[i].IndexOf(firstWordLetter, charCount);
		//				string whitespace = lines[i].Substring(charCount, letterIndex - charCount);
		//				whitespaceCount = whitespace.Length;
		//				word = whitespace + word;
		//			}

		//			double wordLength;

		//			// If the line width is zero, we are at the start of a newline
		//			// We don't proceed preceeding whitespace in the width
		//			// calculation
		//			if (lineWidth > 0)
		//			{
		//				wordLength = metrics.stringWidth(word);
		//			}
		//			else
		//			{
		//				wordLength = metrics.stringWidth(word.Trim());
		//			}

		//			// Does the width of line so far plus the width of the 
		//			// current word exceed the allowed width?
		//			if (lineWidth + wordLength > width)
		//			{
		//				if (lineWidth > 0)
		//				{
		//					// There is already at least one word on this line
		//					// and the current word takes the overall width over
		//					// the allowed width. Because there is something on
		//					// the line, complete the current line, reset the width
		//					// counter, create a new line and put the current word
		//					// back on the stack for processing in the next round
		//					result.Add(currentLine.ToString());
		//					currentLine = new StringBuilder();
		//					wordStack.Push(word.Trim());
		//					lineWidth = 0;
		//				}
		//				else
		//				{
		//					// There are no words on the current line and the 
		//					// current word does not fit on it. Find the maximum
		//					// number of characters of this word that just fit
		//					// in the available width
		//					word = word.Trim();

		//					for (int j = 1; j <= word.Length; j++)
		//					{
		//						wordLength = metrics.stringWidth(word.Substring(0, j));

		//						if (lineWidth + wordLength > width)
		//						{
		//							// The last character took us over the allowed
		//							// width, deducted it unless there is only one
		//							// character, in which case we have to use it
		//							// since we can't split it...
		//							j = j > 1 ? j - 1 : j;
		//							string chars = word.Substring(0, j);
		//							currentLine = currentLine.Append(chars);
		//							// Return the unprocessed part of the word 
		//							// to the stack
		//							wordStack.Push(word.Substring(j, word.Length - j));
		//							result.Add(currentLine.ToString());
		//							currentLine = new StringBuilder();
		//							lineWidth = 0;
		//							// Increment char counter allowing for white 
		//							// space in the original word
		//							charCount = charCount + chars.Length + whitespaceCount;
		//							break;
		//						}
		//					}
		//				}
		//			}
		//			else
		//			{
		//				// The current word does not take the total line width
		//				// over the allowed width. Append the word, removing
		//				// preceeding whitespace if it is the first word in the
		//				// line.
		//				if (lineWidth > 0)
		//				{
		//					currentLine = currentLine.Append(word);
		//				}
		//				else
		//				{
		//					currentLine = currentLine.Append(word.Trim());
		//				}

		//				lineWidth += (int)wordLength;
		//				charCount += word.Length;
		//			}
		//		}

		//		// If there is anything in the current line after processing all of
		//		// the words in this line, add it to the result.
		//		if (currentLine.Length > 0 || result.Count == 0)
		//		{
		//			result.Add(currentLine.ToString());
		//		}
		//	}

		//	return result.ToArray();
		//}

		/// <summary>
		/// Returns an mxRectangle with the size (width and height in pixels) of the
		/// given HTML markup.
		/// </summary>
		/// <param name="markup">
		///            HTML markup whose size should be returned. </param>
		//public static mxRectangle getSizeForHtml(string markup, IDictionary<string, object> style)
		//{
		//	mxLightweightLabel textRenderer = mxLightweightLabel.SharedInstance;

		//	if (textRenderer != null)
		//	{
		//		textRenderer.Text = createHtmlDocument(style, markup);
  //              Dimension size = new Dimension(textRenderer.Size.Width, textRenderer.Size.Height);// textRenderer.Size; //textRenderer.PreferredSize;

  //              return new mxRectangle(0, 0, size.Width, size.Height);
		//	}
		//	else
		//	{
		//		return getSizeForString(markup, getFont(style), 0);
		//	}
		//}

		/// <summary>
		/// Returns the bounding box for the rotated rectangle.
		/// </summary>
		public static mxRectangle getBoundingBox(mxRectangle rect, double rotation)
		{
			mxRectangle result = null;

			if (rect != null && rotation != 0)
			{
                double rad = Common.ToRadians(rotation);//Math.toRadians(rotation);
				double cos = Math.Cos(rad);
				double sin = Math.Sin(rad);

				mxPoint cx = new mxPoint(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

				mxPoint p1 = new mxPoint(rect.X, rect.Y);
				mxPoint p2 = new mxPoint(rect.X + rect.Width, rect.Y);
				mxPoint p3 = new mxPoint(p2.X, rect.Y + rect.Height);
				mxPoint p4 = new mxPoint(rect.X, p3.Y);

				p1 = getRotatedPoint(p1, cos, sin, cx);
				p2 = getRotatedPoint(p2, cos, sin, cx);
				p3 = getRotatedPoint(p3, cos, sin, cx);
				p4 = getRotatedPoint(p4, cos, sin, cx);

				Rectangle tmp = new Rectangle((int) p1.X, (int) p1.Y, 0, 0);
                //tmp.add(p2.Point);
                //tmp.add(p3.Point);
                //tmp.add(p4.Point);
                tmp.AddPoint(p2.Point);
                tmp.AddPoint(p3.Point);
				tmp.AddPoint(p4.Point);
                result = new mxRectangle(tmp);
			}
			else if (rect != null)
			{
				result = (mxRectangle) rect.clone();
			}

			return result;
		}

		/// <summary>
		/// Rotates the given point by the given cos and sin.
		/// </summary>
		public static mxPoint getRotatedPoint(mxPoint pt, double cos, double sin)
		{
			return getRotatedPoint(pt, cos, sin, new mxPoint());
		}

		/// <summary>
		/// Finds the index of the nearest segment on the given cell state for the
		/// specified coordinate pair.
		/// </summary>
		//public static int findNearestSegment(mxCellState state, double x, double y)
		//{
		//	int index = -1;

		//	if (state.AbsolutePointCount > 0)
		//	{
		//		mxPoint last = state.getAbsolutePoint(0);
		//		double min = double.MaxValue;

		//		for (int i = 1; i < state.AbsolutePointCount; i++)
		//		{
		//			mxPoint current = state.getAbsolutePoint(i);
		//			double dist = (new Line2D.Double(last.x, last.y, current.x, current.y)).ptLineDist(x, y);

		//			if (dist < min)
		//			{
		//				min = dist;
		//				index = i - 1;
		//			}

		//			last = current;
		//		}
		//	}

		//	return index;
		//}

		/// <summary>
		/// Rotates the given point by the given cos and sin.
		/// </summary>
		public static mxPoint getRotatedPoint(mxPoint pt, double cos, double sin, mxPoint c)
		{
			double x = pt.X - c.X;
			double y = pt.Y - c.Y;

			double x1 = x * cos - y * sin;
			double y1 = y * cos + x * sin;

			return new mxPoint(x1 + c.X, y1 + c.Y);
		}

		/// <summary>
		/// Draws the image inside the clip bounds to the given graphics object.
		/// </summary>
		//public static void drawImageClip(Graphics g, BufferedImage image, ImageObserver observer)
		//{
		//	Rectangle clip = g.ClipBounds;

		//	if (clip != null)
		//	{
		//		int w = image.Width;
		//		int h = image.Height;

		//		int x = Math.Max(0, Math.Min(clip.x, w));
		//		int y = Math.Max(0, Math.Min(clip.y, h));

		//		w = Math.Min(clip.width, w - x);
		//		h = Math.Min(clip.height, h - y);

		//		if (w > 0 && h > 0)
		//		{
		//			// TODO: Support for normal images using fast subimage copies
		//			g.drawImage(image.getSubimage(x, y, w, h), clip.x, clip.y, observer);
		//		}
		//	}
		//	else
		//	{
		//		g.drawImage(image, 0, 0, observer);
		//	}
		//}

		/// 
		//public static void fillClippedRect(Graphics g, int x, int y, int width, int height)
		//{
		//	Rectangle bg = new Rectangle(x, y, width, height);

		//	try
		//	{
		//		if (g.ClipBounds != null)
		//		{
		//			bg = bg.intersection(g.ClipBounds);
		//		}
		//	}
		//	catch (Exception)
		//	{
		//		// FIXME: Getting clipbounds sometimes throws an NPE
		//	}

		//	g.fillRect(bg.x, bg.y, bg.width, bg.height);
		//}

		/// <summary>
		/// Creates a new list of new points obtained by translating the points in
		/// the given list by the given vector. Elements that are not mxPoints are
		/// added to the result as-is.
		/// </summary>
		public static IList<mxPoint> translatePoints(IList<mxPoint> pts, double dx, double dy)
		{
			IList<mxPoint> result = null;

			if (pts != null)
			{
				result = new List<mxPoint>(pts.Count);
				IEnumerator<mxPoint> it = pts.GetEnumerator();

				while (it.MoveNext())
				{
					mxPoint point = (mxPoint) it.Current.clone();

					point.X = point.X + dx;
					point.Y = point.Y + dy;

					result.Add(point);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the intersection of two lines as an mxPoint.
		/// </summary>
		/// <param name="x0">
		///            X-coordinate of the first line's startpoint. </param>
		/// <param name="y0">
		///            Y-coordinate of the first line's startpoint. </param>
		/// <param name="x1">
		///            X-coordinate of the first line's endpoint. </param>
		/// <param name="y1">
		///            Y-coordinate of the first line's endpoint. </param>
		/// <param name="x2">
		///            X-coordinate of the second line's startpoint. </param>
		/// <param name="y2">
		///            Y-coordinate of the second line's startpoint. </param>
		/// <param name="x3">
		///            X-coordinate of the second line's endpoint. </param>
		/// <param name="y3">
		///            Y-coordinate of the second line's endpoint. </param>
		/// <returns> Returns the intersection between the two lines. </returns>
		public static mxPoint intersection(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double denom = ((y3 - y2) * (x1 - x0)) - ((x3 - x2) * (y1 - y0));
			double nume_a = ((x3 - x2) * (y0 - y2)) - ((y3 - y2) * (x0 - x2));
			double nume_b = ((x1 - x0) * (y0 - y2)) - ((y1 - y0) * (x0 - x2));

			double ua = nume_a / denom;
			double ub = nume_b / denom;

			if (ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0)
			{
				// Get the intersection point
				double intersectionX = x0 + ua * (x1 - x0);
				double intersectionY = y0 + ua * (y1 - y0);

				return new mxPoint(intersectionX, intersectionY);
			}

			// No intersection
			return null;
		}

		/// <summary>
		/// Sorts the given cells according to the order in the cell hierarchy.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static Object[] sortCells(Object[] cells, final boolean ascending)
		public static object[] sortCells(object[] cells, bool ascending)
		{
            object[] temp = new object[cells.Length];
			 sortCells(cells, ascending).CopyTo(temp,0);
            return temp;

        }

		/// <summary>
		/// Sorts the given cells according to the order in the cell hierarchy.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static java.util.Collection<Object> sortCells(java.util.Collection<Object> cells, final boolean ascending)
		public static ICollection<object> sortCells(ICollection<object> cells, bool ascending)
		{
			SortedSet<object> result = new SortedSet<object>(new ComparatorAnonymousInnerClass(ascending));

            //result.addAll(cells);
            foreach (var item in cells)
            {
                result.Add(item);
            }

            return result;
		}

		private class ComparatorAnonymousInnerClass : IComparer<object>
		{
			private bool ascending;

			public ComparatorAnonymousInnerClass(bool ascending)
			{
				this.ascending = ascending;
			}

			public virtual int Compare(object o1, object o2)
			{
				int comp = mxCellPath.compare(mxCellPath.create((mxICell) o1), mxCellPath.create((mxICell) o2));

				return (comp == 0) ? 0 : (((comp > 0) == ascending) ? 1 : -1);
			}
		}

		/// <summary>
		/// Returns true if the given array contains the given object.
		/// </summary>
		public static bool contains(object[] array, object obj)
		{
			return indexOf(array, obj) >= 0;
		}

		/// <summary>
		/// Returns the index of the given object in the given array of -1 if the
		/// object is not contained in the array.
		/// </summary>
		public static int indexOf(object[] array, object obj)
		{
			if (obj != null && array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == obj)
					{
						return i;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns the stylename in a style of the form stylename[;key=value] or an
		/// empty string if the given style does not contain a stylename.
		/// </summary>
		/// <param name="style">
		///            String of the form stylename[;key=value]. </param>
		/// <returns> Returns the stylename from the given formatted string. </returns>
		public static string getStylename(string style)
		{
			if (!string.ReferenceEquals(style, null))
			{
				string[] pairs = style.Split(";", true);
				string stylename = pairs[0];

				if (stylename.IndexOf("=", StringComparison.Ordinal) < 0)
				{
					return stylename;
				}
			}

			return "";
		}

		/// <summary>
		/// Returns the stylenames in a style of the form stylename[;key=value] or an
		/// empty array if the given style does not contain any stylenames.
		/// </summary>
		/// <param name="style">
		///            String of the form stylename[;stylename][;key=value]. </param>
		/// <returns> Returns the stylename from the given formatted string. </returns>
		public static string[] getStylenames(string style)
		{
			List<string> result = new List<string>();

			if (!string.ReferenceEquals(style, null))
			{
				string[] pairs = style.Split(";", true);

				for (int i = 0; i < pairs.Length; i++)
				{
					if (pairs[i].IndexOf("=", StringComparison.Ordinal) < 0)
					{
						result.Add(pairs[i]);
					}
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Returns the index of the given stylename in the given style. This returns
		/// -1 if the given stylename does not occur (as a stylename) in the given
		/// style, otherwise it returns the index of the first character.
		/// </summary>
		public static int indexOfStylename(string style, string stylename)
		{
			if (!string.ReferenceEquals(style, null) && !string.ReferenceEquals(stylename, null))
			{
				string[] tokens = style.Split(";", true);
				int pos = 0;

				for (int i = 0; i < tokens.Length; i++)
				{
					if (tokens[i].Equals(stylename))
					{
						return pos;
					}

					pos += tokens[i].Length + 1;
				}
			}

			return -1;
		}

		/// <summary>
		/// Adds the specified stylename to the given style if it does not already
		/// contain the stylename.
		/// </summary>
		public virtual string addStylename(string style, string stylename)
		{
			if (indexOfStylename(style, stylename) < 0)
			{
				if (string.ReferenceEquals(style, null))
				{
					style = "";
				}
				else if (style.Length > 0 && style[style.Length - 1] != ';')
				{
					style += ';';
				}

				style += stylename;
			}

			return style;
		}

		/// <summary>
		/// Removes all occurrences of the specified stylename in the given style and
		/// returns the updated style. Trailing semicolons are preserved.
		/// </summary>
		public virtual string removeStylename(string style, string stylename)
		{
			StringBuilder buffer = new StringBuilder();

			if (!string.ReferenceEquals(style, null))
			{
				string[] tokens = style.Split(";", true);

				for (int i = 0; i < tokens.Length; i++)
				{
					if (!tokens[i].Equals(stylename))
					{
						buffer.Append(tokens[i] + ";");
					}
				}
			}

            return (buffer.Length > 1) ? buffer.ToString().Substring(0, buffer.Length - 1) : buffer.ToString();
		}

		/// <summary>
		/// Removes all stylenames from the given style and returns the updated
		/// style.
		/// </summary>
		public static string removeAllStylenames(string style)
		{
			StringBuilder buffer = new StringBuilder();

			if (!string.ReferenceEquals(style, null))
			{
				string[] tokens = style.Split(";", true);

				for (int i = 0; i < tokens.Length; i++)
				{
					if (tokens[i].IndexOf('=') >= 0)
					{
						buffer.Append(tokens[i] + ";");
					}
				}
			}

			return (buffer.Length > 1) ? buffer.ToString().Substring(0, buffer.Length - 1) : buffer.ToString();
		}

		/// <summary>
		/// Assigns the value for the given key in the styles of the given cells, or
		/// removes the key from the styles if the value is null.
		/// </summary>
		/// <param name="model">
		///            Model to execute the transaction in. </param>
		/// <param name="cells">
		///            Array of cells to be updated. </param>
		/// <param name="key">
		///            Key of the style to be changed. </param>
		/// <param name="value">
		///            New value for the given key. </param>
		public static void setCellStyles(mxIGraphModel model, object[] cells, string key, string value)
		{
			if (cells != null && cells.Length > 0)
			{
				model.beginUpdate();
				try
				{
					for (int i = 0; i < cells.Length; i++)
					{
						if (cells[i] != null)
						{
							string style = setStyle(model.getStyle(cells[i]), key, value);
							model.setStyle(cells[i], style);
						}
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// <summary>
		/// Adds or removes the given key, value pair to the style and returns the
		/// new style. If value is null or zero length then the key is removed from
		/// the style.
		/// </summary>
		/// <param name="style">
		///            String of the form <code>stylename[;key=value]</code>. </param>
		/// <param name="key">
		///            Key of the style to be changed. </param>
		/// <param name="value">
		///            New value for the given key. </param>
		/// <returns> Returns the new style. </returns>
		public static string setStyle(string style, string key, string value)
		{
			bool isValue = !string.ReferenceEquals(value, null) && value.Length > 0;

			if (string.ReferenceEquals(style, null) || style.Length == 0)
			{
				if (isValue)
				{
					style = key + "=" + value;
				}
			}
			else
			{
				int index = style.IndexOf(key + "=", StringComparison.Ordinal);

				if (index < 0)
				{
					if (isValue)
					{
						string sep = (style.EndsWith(";", StringComparison.Ordinal)) ? "" : ";";
						style = style + sep + key + '=' + value;
					}
				}
				else
				{
					string tmp = (isValue) ? key + "=" + value : "";
					int cont = style.IndexOf(";", index, StringComparison.Ordinal);

					if (!isValue)
					{
						cont++;
					}

					style = style.Substring(0, index) + tmp + ((cont > index) ? style.Substring(cont) : "");
				}
			}

			return style;
		}

		/// <summary>
		/// Sets or toggles the flag bit for the given key in the cell's styles. If
		/// value is null then the flag is toggled.
		/// 
		/// <code>
		/// mxUtils.setCellStyleFlags(graph.getModel(),
		/// 			cells,
		/// 			mxConstants.STYLE_FONTSTYLE,
		/// 			mxConstants.FONT_BOLD, null);
		/// </code>
		/// 
		/// Toggles the bold font style.
		/// </summary>
		/// <param name="model">
		///            Model that contains the cells. </param>
		/// <param name="cells">
		///            Array of cells to change the style for. </param>
		/// <param name="key">
		///            Key of the style to be changed. </param>
		/// <param name="flag">
		///            Integer for the bit to be changed. </param>
		/// <param name="value">
		///            Optional boolean value for the flag. </param>
		public static void setCellStyleFlags(mxIGraphModel model, object[] cells, string key, int flag, bool? value)
		{
			if (cells != null && cells.Length > 0)
			{
				model.beginUpdate();
				try
				{
					for (int i = 0; i < cells.Length; i++)
					{
						if (cells[i] != null)
						{
							string style = setStyleFlag(model.getStyle(cells[i]), key, flag, value);
							model.setStyle(cells[i], style);
						}
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// <summary>
		/// Sets or removes the given key from the specified style and returns the
		/// new style. If value is null then the flag is toggled.
		/// </summary>
		/// <param name="style">
		///            String of the form stylename[;key=value]. </param>
		/// <param name="key">
		///            Key of the style to be changed. </param>
		/// <param name="flag">
		///            Integer for the bit to be changed. </param>
		/// <param name="value">
		///            Optional boolean value for the given flag. </param>
		public static string setStyleFlag(string style, string key, int flag, bool? value)
		{
			if (string.ReferenceEquals(style, null) || style.Length == 0)
			{
				if (value == null || value.Value)
				{
					style = key + "=" + flag;
				}
				else
				{
					style = key + "=0";
				}
			}
			else
			{
				int index = style.IndexOf(key + "=", StringComparison.Ordinal);

				if (index < 0)
				{
					string sep = (style.EndsWith(";", StringComparison.Ordinal)) ? "" : ";";

					if (value == null || value.Value)
					{
						style = style + sep + key + "=" + flag;
					}
					else
					{
						style = style + sep + key + "=0";
					}
				}
				else
				{
					int cont = style.IndexOf(";", index, StringComparison.Ordinal);
					string tmp = "";
					int result = 0;

					if (cont < 0)
					{
						tmp = style.Substring(index + key.Length + 1);
					}
					else
					{
						tmp = StringHelperClass.SubstringSpecial(style, index + key.Length + 1, cont);
					}

					if (value == null)
					{
						result = int.Parse(tmp) ^ flag;
					}
					else if (value.Value)
					{
						result = int.Parse(tmp) | flag;
					}
					else
					{
						result = int.Parse(tmp) & ~flag;
					}

					style = style.Substring(0, index) + key + "=" + result + ((cont >= 0) ? style.Substring(cont) : "");
				}
			}

			return style;
		}

		public static bool intersectsHotspot(mxCellState state, int x, int y, double hotspot)
		{
			return intersectsHotspot(state, x, y, hotspot, 0, 0);
		}

		/// <summary>
		/// Returns true if the given coordinate pair intersects the hotspot of the
		/// given state.
		/// </summary>
		public static bool intersectsHotspot(mxCellState state, int x, int y, double hotspot, int min, int max)
		{
			if (hotspot > 0)
			{
				int cx = (int) Math.Round(state.CenterX);
				int cy = (int) Math.Round(state.CenterY);
				int width = (int) Math.Round(state.Width);
				int height = (int) Math.Round(state.Height);

				if (mxUtils.getString(state.Style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_SWIMLANE))
				{
					int start = mxUtils.getInt(state.Style, mxConstants.STYLE_STARTSIZE, mxConstants.DEFAULT_STARTSIZE);

					if (mxUtils.isTrue(state.Style, mxConstants.STYLE_HORIZONTAL, true))
					{
						cy = (int) Math.Round(state.Y + start / 2);
						height = start;
					}
					else
					{
						cx = (int) Math.Round(state.X + start / 2);
						width = start;
					}
				}

				int w = (int) Math.Max(min, width * hotspot);
				int h = (int) Math.Max(min, height * hotspot);

				if (max > 0)
				{
					w = Math.Min(w, max);
					h = Math.Min(h, max);
				}

				Rectangle rect = new Rectangle((int)Math.Round(cx - w / 2.0), (int)Math.Round(cy - h / 2.0), w, h);

                return rect.Contains(x, y);
			}

			return true;
		}

		/// <summary>
		/// Returns true if the dictionary contains true for the given key or false
		/// if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <returns> Returns the boolean value for key in dict. </returns>
		public static bool isTrue(IDictionary<string, object> dict, string key)
		{
			return isTrue(dict, key, false);
		}

		/// <summary>
		/// Returns true if the dictionary contains true for the given key or the
		/// given default value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the boolean value for key in dict. </returns>
		public static bool isTrue(IDictionary<string, object> dict, string key, bool defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return value.Equals("1") || value.ToString().ToLower().Equals("true");
			}
		}

		/// <summary>
		/// Returns the value for key in dictionary as an int or 0 if no value is
		/// defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <returns> Returns the integer value for key in dict. </returns>
		public static int getInt(IDictionary<string, object> dict, string key)
		{
			return getInt(dict, key, 0);
		}

		/// <summary>
		/// Returns the value for key in dictionary as an int or the given default
		/// value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the integer value for key in dict. </returns>
		public static int getInt(IDictionary<string, object> dict, string key, int defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				// Handles commas by casting them to an int
				return (int) float.Parse(value.ToString());
			}
		}

		/// <summary>
		/// Returns the value for key in dictionary as a float or 0 if no value is
		/// defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <returns> Returns the float value for key in dict. </returns>
		public static float getFloat(IDictionary<string, object> dict, string key)
		{
			return getFloat(dict, key, 0);
		}

		/// <summary>
		/// Returns the value for key in dictionary as a float or the given default
		/// value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the float value for key in dict. </returns>
		public static float getFloat(IDictionary<string, object> dict, string key, float defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return float.Parse(value.ToString());
			}
		}

		/// <summary>
		/// Returns the value for key in dictionary as a float array or the given default
		/// value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the float array value for key in dict. </returns>
		public static float[] getFloatArray(IDictionary<string, object> dict, string key, float[] defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				string[] floatChars = value.ToString().Split(",", true);
				float[] result = new float[floatChars.Length];

				for (int i = 0; i < floatChars.Length; i++)
				{
					result[i] = float.Parse(floatChars[i]);
				}

				return result;
			}
		}

		/// <summary>
		/// Returns the value for key in dictionary as a double or 0 if no value is
		/// defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <returns> Returns the double value for key in dict. </returns>
		public static double getDouble(IDictionary<string, object> dict, string key)
		{
			return getDouble(dict, key, 0);
		}

		/// <summary>
		/// Returns the value for key in dictionary as a double or the given default
		/// value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the double value for key in dict. </returns>
		public static double getDouble(IDictionary<string, object> dict, string key, double defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return double.Parse(value.ToString());
			}
		}

		/// <summary>
		/// Returns the value for key in dictionary as a string or null if no value
		/// is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <returns> Returns the string value for key in dict. </returns>
		public static string getString(IDictionary<string, object> dict, string key)
		{
			return getString(dict, key, null);
		}

		/// <summary>
		/// Returns the value for key in dictionary as a string or the given default
		/// value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the string value for key in dict. </returns>
		public static string getString(IDictionary<string, object> dict, string key, string defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return value.ToString();
			}
		}

		/// <summary>
		/// Returns the value for key in dictionary as a color or null if no value is
		/// defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <returns> Returns the color value for key in dict. </returns>
		public static Color getColor(IDictionary<string, object> dict, string key)
		{
			return getColor(dict, key, Color.Transparent);
		}

		/// <summary>
		/// Returns the value for key in dictionary as a color or the given default
		/// value if no value is defined for the key.
		/// </summary>
		/// <param name="dict">
		///            Dictionary that contains the key, value pairs. </param>
		/// <param name="key">
		///            Key whose value should be returned. </param>
		/// <param name="defaultValue">
		///            Default value to return if the key is undefined. </param>
		/// <returns> Returns the color value for key in dict. </returns>
		public static Color getColor(IDictionary<string, object> dict, string key, Color defaultValue)
		{
			object value = dict[key];

			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				return parseColor(value.ToString());
			}
		}

		/// 
		public static Font getFont(IDictionary<string, object> style)
		{
			return getFont(style, 1);
		}

		/// 
		public static Font getFont(IDictionary<string, object> style, double scale)
		{
			string fontFamily = getString(style, mxConstants.STYLE_FONTFAMILY, mxConstants.DEFAULT_FONTFAMILY);
			int fontSize = getInt(style, mxConstants.STYLE_FONTSIZE, mxConstants.DEFAULT_FONTSIZE);
			int fontStyle = getInt(style, mxConstants.STYLE_FONTSTYLE);

            FontStyle swingFontStyle = ((fontStyle & mxConstants.FONT_BOLD) == mxConstants.FONT_BOLD) ? FontStyle.Bold : FontStyle.Regular;
            //swingFontStyle += ((fontStyle & mxConstants.FONT_ITALIC) == mxConstants.FONT_ITALIC) ? FontStyle.Italic : FontStyle.Regular;

			return new Font(fontFamily, (int)(fontSize * scale), swingFontStyle);
		}

		/// 
		public static string hexString(Color color)
		{
			int r = color.R;
			int g = color.G;
			int b = color.B;

			return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
		}

		/// <summary>
		/// Convert a string representing a 24/32bit hex color value into a Color
		/// object. The following color names are also supported: white, black, red,
		/// green, blue, orange, yellow, pink, turquoise, gray and none (null).
		/// Examples of possible hex color values are: #C3D9FF, #6482B9 and #774400,
		/// but note that you do not include the "#" in the string passed in
		/// </summary>
		/// <param name="colorString">
		///            the 24/32bit hex string value (ARGB) </param>
		/// <returns> java.awt.Color (24bit RGB on JDK 1.1, 24/32bit ARGB on JDK1.2) </returns>
		/// <exception cref="NumberFormatException">
		///                if the specified string cannot be interpreted as a
		///                hexidecimal integer </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.awt.Color parseColor(String colorString) throws NumberFormatException
		public static Color parseColor(string colorString)
		{
			if (colorString.Equals("white", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.White;
			}
			else if (colorString.Equals("black", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Black;
			}
			else if (colorString.Equals("red", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Red;
			}
			else if (colorString.Equals("green", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Green;
			}
			else if (colorString.Equals("blue", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Blue;
			}
			else if (colorString.Equals("orange", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Orange;
			}
			else if (colorString.Equals("yellow", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Yellow;
			}
			else if (colorString.Equals("pink", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Pink;
			}
			else if (colorString.Equals("turqoise", StringComparison.CurrentCultureIgnoreCase))
			{
				return Color.FromArgb(0, 255, 255);
			}
			else if (colorString.Equals("gray", StringComparison.CurrentCultureIgnoreCase))
			{
                return Color.Gray;
			}
			else if (colorString.Equals("none", StringComparison.CurrentCultureIgnoreCase))
			{
				return Color.Transparent;
			}

			int value=0;
			try
			{
				value = (int) Convert.ToInt32(colorString, 16);
			}
			catch (System.FormatException)
			{
				value =Convert.ToInt32(colorString);
			}

            return Common.RgbToColor(value);
		}

		/// <summary>
		/// Returns a hex representation for the given color.
		/// </summary>
		/// <param name="color">
		///            Color to return the hex string for. </param>
		/// <returns> Returns a hex string for the given color. </returns>
		//public static string getHexColorString(Color color)
		//{
		//	return ((color.RGB & 0x00FFFFFF) | (color.Alpha << 24)).ToString("x");
		//}

		/// <summary>
		/// Reads the given filename into a string.
		/// </summary>
		/// <param name="filename">
		///            Name of the file to be read. </param>
		/// <returns> Returns a string representing the file contents. </returns>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String readFile(String filename) throws java.io.IOException
		public static string readFile(string filename)
		{
			System.IO.StreamReader reader = new System.IO.StreamReader(new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read));
			StringBuilder result = new StringBuilder();
			string tmp = reader.ReadLine();

			while (!string.ReferenceEquals(tmp, null))
			{
				result.Append(tmp + "\n");
				tmp = reader.ReadLine();
			}

			reader.Close();

			return result.ToString();
		}

		/// <summary>
		/// Writes the given string into the given file.
		/// </summary>
		/// <param name="contents">
		///            String representing the file contents. </param>
		/// <param name="filename">
		///            Name of the file to be written. </param>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void writeFile(String contents, String filename) throws java.io.IOException
		public static void writeFile(string contents, string filename)
		{
			System.IO.StreamWriter fw = new System.IO.StreamWriter(filename);
			fw.Write(contents);
			fw.Flush();
			fw.Close();
		}

		/// <summary>
		/// Returns the Md5 hash for the given text.
		/// </summary>
		/// <param name="text">
		///            String whose Md5 hash should be returned. </param>
		/// <returns> Returns the Md5 hash for the given text. </returns>
		//public static string getMd5Hash(string text)
		//{
		//	StringBuilder result = new StringBuilder(32);
		//	try
		//	{
		//		MessageDigest md5 = MessageDigest.getInstance("MD5");
		//		md5.update(text.GetBytes());
		//		Formatter f = new Formatter(result);

		//		sbyte[] digest = md5.digest();

		//		for (int i = 0; i < digest.Length; i++)
		//		{
		//			f.format("%02x", new object[] {new sbyte?(digest[i])});
		//		}
		//	}
		//	catch (NoSuchAlgorithmException ex)
		//	{
		//		Console.WriteLine(ex.ToString());
		//		Console.Write(ex.StackTrace);
		//	}

		//	return result.ToString();
		//}

		/// <summary>
		/// Returns true if the user object is an XML node with the specified type
		/// and and the optional attribute has the specified value or is not
		/// specified.
		/// </summary>
		/// <param name="value">
		///            Object that should be examined as a node. </param>
		/// <param name="nodeName">
		///            String that specifies the node name. </param>
		/// <returns> Returns true if the node name of the user object is equal to the
		///         given type. </returns>

		public static bool isNode(object value, string nodeName)
		{
			return isNode(value, nodeName, null, null);
		}

		/// <summary>
		/// Returns true if the given value is an XML node with the node name and if
		/// the optional attribute has the specified value.
		/// </summary>
		/// <param name="value">
		///            Object that should be examined as a node. </param>
		/// <param name="nodeName">
		///            String that specifies the node name. </param>
		/// <param name="attributeName">
		///            Optional attribute name to check. </param>
		/// <param name="attributeValue">
		///            Optional attribute value to check. </param>
		/// <returns> Returns true if the value matches the given conditions. </returns>
		public static bool isNode(object value, string nodeName, string attributeName, string attributeValue)
		{
			if (value is Element)
			{
				Element element = (Element) value;

				if (string.ReferenceEquals(nodeName, null) || element.Name.Equals(nodeName,StringComparison.OrdinalIgnoreCase))
				{
                    string tmp = (!string.ReferenceEquals(attributeName, null)) ? element.GetAttribute(attributeName) : null;

					return string.ReferenceEquals(attributeName, null) || (!string.ReferenceEquals(tmp, null) && tmp.Equals(attributeValue));
				}
			}

			return false;
		}

		/// 
		/// <param name="g"> </param>
		/// <param name="antiAlias"> </param>
		/// <param name="textAntiAlias"> </param>
		//public static void setAntiAlias(Graphics2D g, bool antiAlias, bool textAntiAlias)
		//{
		//	g.setRenderingHint(RenderingHints.KEY_ANTIALIASING, (antiAlias) ? RenderingHints.VALUE_ANTIALIAS_ON : RenderingHints.VALUE_ANTIALIAS_OFF);
		//	g.setRenderingHint(RenderingHints.KEY_TEXT_ANTIALIASING, (textAntiAlias) ? RenderingHints.VALUE_TEXT_ANTIALIAS_ON : RenderingHints.VALUE_TEXT_ANTIALIAS_OFF);
		//}

		/// <summary>
		/// Clears the given area of the specified graphics object with the given
		/// color or makes the region transparent.
		/// </summary>
		//public static void clearRect(Graphics g, Rectangle rect, Color background)
		//{
		//	if (background != null)
		//	{
		//		g.Color = background;
		//		g.fillRect(rect.x, rect.y, rect.width, rect.height);
		//	}
		//	else
		//	{
		//		g.Composite = AlphaComposite.getInstance(AlphaComposite.CLEAR, 0.0f);
		//		g.fillRect(rect.x, rect.y, rect.width, rect.height);
		//		g.Composite = AlphaComposite.SrcOver;
		//	}
		//}

		/// <summary>
		/// Creates a buffered image for the given parameters. If there is not enough
		/// memory to create the image then a OutOfMemoryError is thrown.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.awt.image.BufferedImage createBufferedImage(int w, int h, java.awt.Color background) throws OutOfMemoryError
		//public static BufferedImage createBufferedImage(int w, int h, Color background)
		//{
		//	BufferedImage result = null;

		//	if (w > 0 && h > 0)
		//	{
		//		// Checks if there is enough memory for allocating the buffer
		//		Runtime runtime = Runtime.Runtime;
		//		long maxMemory = runtime.maxMemory();
		//		long allocatedMemory = runtime.totalMemory();
		//		long freeMemory = runtime.freeMemory();
		//		long totalFreeMemory = (freeMemory + (maxMemory - allocatedMemory)) / 1024;

		//		int bytes = 4; // 1 if indexed
		//		long memoryRequired = w * h * bytes / 1024;

		//		if (memoryRequired <= totalFreeMemory)
		//		{
		//			int type = (background != null) ? BufferedImage.TYPE_INT_RGB : BufferedImage.TYPE_INT_ARGB;
		//			result = new BufferedImage(w, h, type);

		//			// Clears background
		//			if (background != null)
		//			{
		//				Graphics2D g2 = result.createGraphics();
		//				clearRect(g2, new Rectangle(w, h), background);
		//				g2.dispose();
		//			}
		//		}
		//		else
		//		{
		//			throw new System.OutOfMemoryException("Not enough memory for image (" + w + " x " + h + ")");
		//		}
		//	}

		//	return result;
		//}

		/// <summary>
		/// Returns a new, empty DOM document.
		/// </summary>
		/// <returns> Returns a new DOM document. </returns>
		public static Document createDocument()
		{
			Document result = null;

			try
			{
                //DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
                //DocumentBuilder parser = factory.newDocumentBuilder();

                //result = parser.newDocument();

                return result = new Document();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return result;
		}

		/// <summary>
		/// Creates a new SVG document for the given width and height.
		/// </summary>
		public static Document createSvgDocument(int width, int height)
		{
			Document document = createDocument();
            Element root = document.CreateElement("svg");

			string w = width.ToString();
			string h = height.ToString();

            root.SetAttribute("width", w);
			root.SetAttribute("height", h);
			root.SetAttribute("viewBox", "0 0 " + w + " " + h);
			root.SetAttribute("version", "1.1");
			root.SetAttribute("xmlns", mxConstants.NS_SVG);
			root.SetAttribute("xmlns:xlink", mxConstants.NS_XLINK);

            document.AppendChild(root);

			return document;
		}

		/// 
		public static Document createVmlDocument()
		{
			Document document = createDocument();

            Element root = document.CreateElement("html");
            root.SetAttribute("xmlns:v", "urn:schemas-microsoft-com:vml");
            root.SetAttribute("xmlns:o", "urn:schemas-microsoft-com:office:office");

            document.AppendChild(root);

            Element head = document.CreateElement("head");

            Element style = document.CreateElement("style");
            style.SetAttribute("type", "text/css");
            style.AppendChild(document.CreateTextNode("<!-- v\\:* {behavior: url(#default#VML);} -->"));

            head.AppendChild(style);
            root.AppendChild(head);

            Element body = document.CreateElement("body");
            root.AppendChild(body);

			return document;
		}

		/// <summary>
		/// Returns a document with a HTML node containing a HEAD and BODY node.
		/// </summary>
		public static Document createHtmlDocument()
		{
			Document document = createDocument();

            Element root = document.CreateElement("html");

            document.AppendChild(root);

            Element head = document.CreateElement("head");
            root.AppendChild(head);

            Element body = document.CreateElement("body");
            root.AppendChild(body);

			return document;
		}

		/// <summary>
		/// Returns a new, empty DOM document.
		/// </summary>
		/// <returns> Returns a new DOM document. </returns>
		public static string createHtmlDocument(IDictionary<string, object> style, string text)
		{
			return createHtmlDocument(style, text, 1);
		}

		/// <summary>
		/// Returns a new, empty DOM document.
		/// </summary>
		/// <returns> Returns a new DOM document. </returns>
		public static string createHtmlDocument(IDictionary<string, object> style, string text, double scale)
		{
			StringBuilder css = new StringBuilder();
			css.Append("font-family:" + getString(style, mxConstants.STYLE_FONTFAMILY, mxConstants.DEFAULT_FONTFAMILIES) + ";");
			css.Append("font-size:" + (int)(getInt(style, mxConstants.STYLE_FONTSIZE, mxConstants.DEFAULT_FONTSIZE) * scale) + " pt;");

			string color = mxUtils.getString(style, mxConstants.STYLE_FONTCOLOR);

			if (!string.ReferenceEquals(color, null))
			{
				css.Append("color:" + color + ";");
			}

			int fontStyle = mxUtils.getInt(style, mxConstants.STYLE_FONTSTYLE);

			if ((fontStyle & mxConstants.FONT_BOLD) == mxConstants.FONT_BOLD)
			{
				css.Append("font-weight:bold;");
			}

			if ((fontStyle & mxConstants.FONT_ITALIC) == mxConstants.FONT_ITALIC)
			{
				css.Append("font-style:italic;");
			}

			if ((fontStyle & mxConstants.FONT_UNDERLINE) == mxConstants.FONT_UNDERLINE)
			{
				css.Append("text-decoration:underline;");
			}

			string align = getString(style, mxConstants.STYLE_ALIGN, mxConstants.ALIGN_LEFT);

			if (align.Equals(mxConstants.ALIGN_CENTER))
			{
				css.Append("text-align:center;");
			}
			else if (align.Equals(mxConstants.ALIGN_RIGHT))
			{
				css.Append("text-align:right;");
			}

			return "<html><body style=\"" + css.ToString() + "\">" + text + "</body></html>";
		}

		/// <summary>
		/// Returns a new, empty DOM document.
		/// </summary>
		/// <returns> Returns a new DOM document. </returns>
		//public static HTMLDocument createHtmlDocumentObject(IDictionary<string, object> style, double scale)
		//{
		//	// Applies the font settings
		//	HTMLDocument document = new HTMLDocument();

		//	StringBuilder rule = new StringBuilder("body {");
		//	rule.Append(" font-family: " + getString(style, mxConstants.STYLE_FONTFAMILY, mxConstants.DEFAULT_FONTFAMILIES) + " ; ");
		//	rule.Append(" font-size: " + (int)(getInt(style, mxConstants.STYLE_FONTSIZE, mxConstants.DEFAULT_FONTSIZE) * scale) + " pt ;");

		//	string color = mxUtils.getString(style, mxConstants.STYLE_FONTCOLOR);

		//	if (!string.ReferenceEquals(color, null))
		//	{
		//		rule.Append("color: " + color + " ; ");
		//	}

		//	int fontStyle = mxUtils.getInt(style, mxConstants.STYLE_FONTSTYLE);

		//	if ((fontStyle & mxConstants.FONT_BOLD) == mxConstants.FONT_BOLD)
		//	{
		//		rule.Append(" font-weight: bold ; ");
		//	}

		//	if ((fontStyle & mxConstants.FONT_ITALIC) == mxConstants.FONT_ITALIC)
		//	{
		//		rule.Append(" font-style: italic ; ");
		//	}

		//	if ((fontStyle & mxConstants.FONT_UNDERLINE) == mxConstants.FONT_UNDERLINE)
		//	{
		//		rule.Append(" text-decoration: underline ; ");
		//	}

		//	string align = getString(style, mxConstants.STYLE_ALIGN, mxConstants.ALIGN_LEFT);

		//	if (align.Equals(mxConstants.ALIGN_CENTER))
		//	{
		//		rule.Append(" text-align: center ; ");
		//	}
		//	else if (align.Equals(mxConstants.ALIGN_RIGHT))
		//	{
		//		rule.Append(" text-align: right ; ");
		//	}

		//	rule.Append(" } ");
		//	document.StyleSheet.addRule(rule.ToString());

		//	return document;
		//}

		/// <summary>
		/// Creates a table for the given text using the given document to create the
		/// DOM nodes. Returns the outermost table node.
		/// </summary>
		public static Element createTable(Document document, string text, int x, int y, int w, int h, double scale, IDictionary<string, object> style)
		{
            // Does not use a textbox as this must go inside another VML shape
            Element table = document.CreateElement("table");

			if (!string.ReferenceEquals(text, null) && text.Length > 0)
			{
                Element tr = document.CreateElement("tr");
                Element td = document.CreateElement("td");

                table.SetAttribute("cellspacing", "0");
                table.SetAttribute("border", "0");
                td.SetAttribute("align", mxUtils.getString(style, mxConstants.STYLE_ALIGN, mxConstants.ALIGN_CENTER));

				string fontColor = getString(style, mxConstants.STYLE_FONTCOLOR, "black");
				string fontFamily = getString(style, mxConstants.STYLE_FONTFAMILY, mxConstants.DEFAULT_FONTFAMILIES);
				int fontSize = (int)(getInt(style, mxConstants.STYLE_FONTSIZE, mxConstants.DEFAULT_FONTSIZE) * scale);

				string s = "position:absolute;" + "left:" + x.ToString() + "px;" + "top:" + y.ToString() + "px;" + "width:" + w.ToString() + "px;" + "height:" + h.ToString() + "px;" + "font-size:" + fontSize.ToString() + "px;" + "font-family:" + fontFamily + ";" + "color:" + fontColor + ";";

				if (mxUtils.getString(style, mxConstants.STYLE_WHITE_SPACE, "nowrap").Equals("wrap"))
				{
					s += "whiteSpace:wrap;";
				}

				// Applies the background color
				string background = getString(style, mxConstants.STYLE_LABEL_BACKGROUNDCOLOR);

				if (!string.ReferenceEquals(background, null))
				{
					s += "background:" + background + ";";
				}

				// Applies the border color
				string border = getString(style, mxConstants.STYLE_LABEL_BORDERCOLOR);

				if (!string.ReferenceEquals(border, null))
				{
					s += "border:" + border + " solid 1pt;";
				}

				// Applies the opacity
				float opacity = getFloat(style, mxConstants.STYLE_TEXT_OPACITY, 100);

				if (opacity < 100)
				{
					// Adds all rules (first for IE)
					s += "filter:alpha(opacity=" + opacity + ");";
					s += "opacity:" + (opacity / 100) + ";";
				}

                td.SetAttribute("style", s);
                string[] lines = text.Split('\n');//text.Split('\n', true);


                for (int i = 0; i < lines.Length; i++)
				{
                    td.AppendChild(document.CreateTextNode(lines[i]));
                    td.AppendChild(document.CreateElement("br"));
				}

				tr.AppendChild(td);
				table.AppendChild(tr);
			}

			return table;
		}

		/// 
		public static Image loadImage(string url)
		{
			Image img = null;
			//URL realUrl = null;

			//try
			//{
			//	realUrl = new URL(url);
			//}
			//catch (Exception)
			//{
			//	realUrl = typeof(mxUtils).getResource(url);
			//}

			//if (!string.ReferenceEquals(url, null))
			//{
			//	try
			//	{
			//		img = ImageIO.read(realUrl);
			//	}
			//	catch (Exception)
			//	{
			//		// ignore
			//	}
			//}

			return img;
		}

		/// <summary>
		/// Returns a new DOM document for the given URI.
		/// </summary>
		/// <param name="uri">
		///            URI to parse into the document. </param>
		/// <returns> Returns a new DOM document for the given URI. </returns>
		public static Document loadDocument(string uri)
		{
			try
			{
                //DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
                //DocumentBuilder docBuilder = docBuilderFactory.newDocumentBuilder();
                //return docBuilder.parse(uri);
                Document doc = new Document();
                doc.Load(uri);
                return doc;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return null;
		}

		/// <summary>
		/// Returns a new document for the given XML string.
		/// </summary>
		/// <param name="xml">
		///            String that represents the XML data. </param>
		/// <returns> Returns a new XML document. </returns>
		/// @deprecated as of 31.08.2010. Use parseXML(String xml) instead 
		public static Document parse(string xml)
		{
			return mxUtils.parseXml(xml);
		}

		/// <summary>
		/// Returns a new document for the given XML string.
		/// </summary>
		/// <param name="xml">
		///            String that represents the XML data. </param>
		/// <returns> Returns a new XML document. </returns>
		public static Document parseXml(string xml)
		{
			try
			{
                //DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
                //DocumentBuilder docBuilder = docBuilderFactory.newDocumentBuilder();

                //return docBuilder.parse(new InputSource(new StringReader(xml)));
                Document doc = new Document();
                doc.LoadXml(xml);
                return doc;

			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return null;
		}

		/// <summary>
		/// Evaluates a Java expression as a class member using mxCodecRegistry. The
		/// range of supported expressions is limited to static class members such as
		/// mxEdgeStyle.ElbowConnector.
		/// </summary>
		public static object eval(string expression)
		{
			int dot = expression.LastIndexOf(".", StringComparison.Ordinal);

			if (dot > 0)
			{
				Type clazz = mxCodecRegistry.getClassForName(expression.Substring(0, dot));

				if (clazz != null)
				{
					try
					{
						return clazz.GetField(expression.Substring(dot + 1)).GetValue(null);
					}
					catch (Exception)
					{
						// ignore
					}
				}
			}

			return expression;
		}

		/// <summary>
		/// Returns the first node where attr equals value. This implementation does
		/// not use XPath.
		/// </summary>
		public static Node findNode(Node node, string attr, string value)
		{
            string tmp = (node is Element) ? ((Element) node).GetAttribute(attr) : null;

			if (!string.ReferenceEquals(tmp, null) && tmp.Equals(value))
			{
				return node;
			}

			node = node.FirstChild;

			while (node != null)
			{
				Node result = findNode(node, attr, value);

				if (result != null)
				{
					return result;
				}

				node = node.NextSibling;
			}

			return null;
		}

		/// <summary>
		/// Returns a single node that matches the given XPath expression.
		/// </summary>
		/// <param name="doc">
		///            Document that contains the nodes. </param>
		/// <param name="expression">
		///            XPath expression to be matched. </param>
		/// <returns> Returns a single node matching the given expression. </returns>
		public static Node selectSingleNode(Document doc, string expression)
		{
			try
			{
                //XPath xpath = XPathFactory.newInstance().newXPath();

                //return (Node) xpath.evaluate(expression, doc, XPathConstants.NODE);
                return doc.SelectSingleNode(expression);
			}
			catch (Exception)//XPathExpressionException
            {
				// ignore
			}

			return null;
		}

		/// <summary>
		/// Converts the ampersand, quote, prime, less-than and greater-than
		/// characters to their corresponding HTML entities in the given string.
		/// </summary>
		public static string htmlEntities(string text)
		{
            //return text.replaceAll("&", "&amp;").replaceAll("\"", "&quot;").replaceAll("'", "&prime;").replaceAll("<", "&lt;").replaceAll(">", "&gt;");
            return text.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&prime;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

		/// <summary>
		/// Returns a string that represents the given node.
		/// </summary>
		/// <param name="node">
		///            Node to return the XML for. </param>
		/// <returns> Returns an XML string. </returns>
		public static string getXml(Node node)
		{
			try
			{
                //Transformer tf = TransformerFactory.newInstance().newTransformer();

                //tf.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
                //tf.setOutputProperty(OutputKeys.ENCODING, "UTF-8");

                //StreamResult dest = new StreamResult(new StringWriter());
                //tf.transform(new DOMSource(node), dest);

                //return dest.Writer.ToString();

                return node.OuterXml;
			}
			catch (Exception)
			{
				// ignore
			}

			return "";
		}

		/// <summary>
		/// Returns a pretty-printed XML string for the given node.
		/// </summary>
		/// <param name="node">
		///            Node to return the XML for. </param>
		/// <returns> Returns a formatted XML string. </returns>
		public static string getPrettyXml(Node node)
		{
			return getPrettyXml(node, "  ", "");
		}

		/// <summary>
		/// Returns a pretty-printed XML string for the given node. Note that this
		/// string should only be used for humans to read (eg. debug output) but not
		/// for further processing as it does not use built-in mechanisms.
		/// </summary>
		/// <param name="node">
		///            Node to return the XML for. </param>
		/// <param name="tab">
		///            String to be used for indentation of inner nodes. </param>
		/// <param name="indent">
		///            Current indentation for the node. </param>
		/// <returns> Returns a formatted XML string. </returns>
		public static string getPrettyXml(Node node, string tab, string indent)
		{
			StringBuilder result = new StringBuilder();

			if (node != null)
			{
				if (node.NodeType == System.Xml.XmlNodeType.Text)
				{
					result.Append(node.Value);
				}
				else
				{
					result.Append(indent + "<" + node.Name);
					NamedNodeMap attrs = node.Attributes;

					if (attrs != null)
					{
						for (int i = 0; i < attrs.Count; i++)
						{
                            string value = attrs.Item(i).Value;
							value = mxUtils.htmlEntities(value);
                            result.Append(" " + attrs.Item(i).Name + "=\"" + value + "\"");
						}
					}
					Node tmp = node.FirstChild;

					if (tmp != null)
					{
						result.Append(">\n");

						while (tmp != null)
						{
							result.Append(getPrettyXml(tmp, tab, indent + tab));
							tmp = tmp.NextSibling;
						}

						result.Append(indent + "</" + node.Name + ">\n");
					}
					else
					{
						result.Append("/>\n");
					}
				}
			}

			return result.ToString();
		}

	}

}