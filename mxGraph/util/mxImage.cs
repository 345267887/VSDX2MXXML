using System;

/// <summary>
/// $Id: mxImage.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.util
{

	/// <summary>
	/// Implements a 2-dimensional point with double precision coordinates.
	/// </summary>
	[Serializable]
	public class mxImage : ICloneable
	{

		/// 
		private const long serialVersionUID = 8541229679513497585L;

		/// <summary>
		/// Holds the path or URL for the image.
		/// </summary>
		protected internal string src;

		/// <summary>
		/// Holds the image width and height.
		/// </summary>
		protected internal int width, height;

		/// <summary>
		/// Constructs a new point at (0, 0).
		/// </summary>
		public mxImage(string src, int width, int height)
		{
			this.src = src;
			this.width = width;
			this.height = height;
		}

		/// <returns> the src </returns>
		public virtual string Src
		{
			get
			{
				return src;
			}
			set
			{
				this.src = value;
			}
		}


		/// <returns> the width </returns>
		public virtual int Width
		{
			get
			{
				return width;
			}
			set
			{
				this.width = value;
			}
		}


		/// <returns> the height </returns>
		public virtual int Height
		{
			get
			{
				return height;
			}
			set
			{
				this.height = value;
			}
		}

        public object Clone()
        {
            mxImage img = new mxImage(this.Src, this.Width, this.Height);

            return img;
        }


    }

}