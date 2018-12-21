using System;
using System.Collections.Generic;
using System.Drawing;

namespace mxGraph.canvas
{


	using mxConstants = mxGraph.util.mxConstants;
	using mxUtils = mxGraph.util.mxUtils;

	public abstract class mxBasicCanvas : mxICanvas
	{
		public abstract object drawLabel(string text, view.mxCellState state, bool html);
		public abstract object drawCell(view.mxCellState state);

		/// <summary>
		/// Defines the default value for the imageBasePath in all GDI canvases.
		/// Default is an empty string.
		/// </summary>
		public static string DEFAULT_IMAGEBASEPATH = "";

		/// <summary>
		/// Defines the base path for images with relative paths. Trailing slash
		/// is required. Default value is DEFAULT_IMAGEBASEPATH.
		/// </summary>
		protected internal string imageBasePath = DEFAULT_IMAGEBASEPATH;

		/// <summary>
		/// Specifies the current translation. Default is (0,0).
		/// </summary>
		protected internal Point translate = new Point();

		/// <summary>
		/// Specifies the current scale. Default is 1.
		/// </summary>
		protected internal double scale = 1;

		/// <summary>
		/// Specifies whether labels should be painted. Default is true.
		/// </summary>
		protected internal bool drawLabels = true;

		/// <summary>
		/// Sets the current translate.
		/// </summary>
		public virtual void setTranslate(int dx, int dy)
		{
			translate = new Point(dx, dy);
		}

		/// <summary>
		/// Returns the current translate.
		/// </summary>
		public virtual Point Translate
		{
			get
			{
				return translate;
			}
		}

		/// 
		public virtual double Scale
		{
			set
			{
				this.scale = value;
			}
			get
			{
				return scale;
			}
		}


		/// 
		public virtual bool DrawLabels
		{
			set
			{
				this.drawLabels = value;
			}
			get
			{
				return drawLabels;
			}
		}

		/// 
		public virtual string ImageBasePath
		{
			get
			{
				return imageBasePath;
			}
			set
			{
				this.imageBasePath = value;
			}
		}
        



        /// <summary>
        /// Gets the image path from the given style. If the path is relative (does
        /// not start with a slash) then it is appended to the imageBasePath.
        /// </summary>
        public virtual string getImageForStyle(IDictionary<string, object> style)
		{
			string filename = mxUtils.getString(style, mxConstants.STYLE_IMAGE);

			if (!string.ReferenceEquals(filename, null) && !filename.StartsWith("/", StringComparison.Ordinal))
			{
				filename = imageBasePath + filename;
			}

			return filename;
		}

	}

}