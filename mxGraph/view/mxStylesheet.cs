using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxStylesheet.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.view
{


	using mxConstants = util.mxConstants;

	/// <summary>
	/// Defines the appearance of the cells in a graph. The following example
	/// changes the font size for all vertices by changing the default vertex
	/// style in-place:
	/// <code>
	/// getDefaultVertexStyle().put(mxConstants.STYLE_FONTSIZE, 16);
	/// </code>
	/// 
	/// To change the default font size for all cells, set
	/// mxConstants.DEFAULT_FONTSIZE.
	/// </summary>
	public class mxStylesheet
	{

		/// <summary>
		/// Shared immutable empty hashtable (for undefined cell styles).
		/// </summary>
		public static readonly IDictionary<string, object> EMPTY_STYLE = new Dictionary<string, object>();

		/// <summary>
		/// Maps from names to styles.
		/// </summary>
		protected internal IDictionary<string, IDictionary<string, object>> styles = new Dictionary<string, IDictionary<string, object>>();

		/// <summary>
		/// Constructs a new stylesheet and assigns default styles.
		/// </summary>
		public mxStylesheet()
		{
			DefaultVertexStyle = createDefaultVertexStyle();
			DefaultEdgeStyle = createDefaultEdgeStyle();
		}

		/// <summary>
		/// Returns all styles as map of name, hashtable pairs.
		/// </summary>
		/// <returns> All styles in this stylesheet. </returns>
		public virtual IDictionary<string, IDictionary<string, object>> Styles
		{
			get
			{
				return styles;
			}
			set
			{
				this.styles = value;
			}
		}


		/// <summary>
		/// Creates and returns the default vertex style.
		/// </summary>
		/// <returns> Returns the default vertex style. </returns>
		protected internal virtual IDictionary<string, object> createDefaultVertexStyle()
		{
			IDictionary<string, object> style = new Dictionary<string, object>();

			style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_RECTANGLE;
			style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
			style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
			style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
			style[mxConstants.STYLE_FILLCOLOR] = "#C3D9FF";
			style[mxConstants.STYLE_STROKECOLOR] = "#6482B9";
			style[mxConstants.STYLE_FONTCOLOR] = "#774400";

			return style;
		}

		/// <summary>
		/// Creates and returns the default edge style.
		/// </summary>
		/// <returns> Returns the default edge style. </returns>
		protected internal virtual IDictionary<string, object> createDefaultEdgeStyle()
		{
			IDictionary<string, object> style = new Dictionary<string, object>();

			style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_CONNECTOR;
			style[mxConstants.STYLE_ENDARROW] = mxConstants.ARROW_CLASSIC;
			style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
			style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
			style[mxConstants.STYLE_STROKECOLOR] = "#6482B9";
			style[mxConstants.STYLE_FONTCOLOR] = "#446299";

			return style;
		}

		/// <summary>
		/// Returns the default style for vertices.
		/// </summary>
		/// <returns> Returns the default vertex style. </returns>
		public virtual IDictionary<string, object> DefaultVertexStyle
		{
			get
			{
				return styles["defaultVertex"];
			}
			set
			{
				putCellStyle("defaultVertex", value);
			}
		}


		/// <summary>
		/// Returns the default style for edges.
		/// </summary>
		/// <returns> Returns the default edge style. </returns>
		public virtual IDictionary<string, object> DefaultEdgeStyle
		{
			get
			{
				return styles["defaultEdge"];
			}
			set
			{
				putCellStyle("defaultEdge", value);
			}
		}


		/// <summary>
		/// Stores the specified style under the given name.
		/// </summary>
		/// <param name="name"> Name for the style to be stored. </param>
		/// <param name="style"> Key, value pairs that define the style. </param>
		public virtual void putCellStyle(string name, IDictionary<string, object> style)
		{
			styles[name] = style;
		}

		/// <summary>
		/// Returns the cell style for the specified cell or the given defaultStyle
		/// if no style can be found for the given stylename.
		/// </summary>
		/// <param name="name"> String of the form [(stylename|key=value);] that represents the
		/// style. </param>
		/// <param name="defaultStyle"> Default style to be returned if no style can be found. </param>
		/// <returns> Returns the style for the given formatted cell style. </returns>
		public virtual IDictionary<string, object> getCellStyle(string name, IDictionary<string, object> defaultStyle)
		{
			IDictionary<string, object> style = defaultStyle;

			if (!string.ReferenceEquals(name, null) && name.Length > 0)
			{
                string[] pairs = name.Split(';');// name.Split(";", true);

				if (style != null && !name.StartsWith(";", StringComparison.Ordinal))
				{
					style = new Dictionary<string, object>(style);
				}
				else
				{
					style = new Dictionary<string, object>();
				}

				for (int i = 0; i < pairs.Length; i++)
				{
					string tmp = pairs[i];
					int c = tmp.IndexOf('=');

					if (c >= 0)
					{
						string key = tmp.Substring(0, c);
						string value = tmp.Substring(c + 1);

						if (value.Equals(mxConstants.NONE))
						{
							style.Remove(key);
						}
						else
						{
							style[key] = value;
						}
					}
					else
					{
						IDictionary<string, object> tmpStyle = styles[tmp];

						if (tmpStyle != null)
						{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
							//style.putAll(tmpStyle);

                            foreach (var item in tmpStyle)
                            {
                                style.Add(item.Key, item.Value);
                            }
						}
					}
				}
			}

			return style;
		}

	}

}