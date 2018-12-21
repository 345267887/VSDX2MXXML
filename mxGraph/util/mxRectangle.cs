using System;
using System.Drawing;

/// <summary>
/// $Id: mxRectangle.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{


	/// <summary>
	/// Implements a 2-dimensional rectangle with double precision coordinates.
	/// </summary>
	[Serializable]
	public class mxRectangle : mxPoint
	{

		/// 
		private const long serialVersionUID = -3793966043543578946L;

		/// <summary>
		/// Holds the width and the height. Default is 0.
		/// </summary>
		protected internal double width, height;

		/// <summary>
		/// Constructs a new rectangle at (0, 0) with the width and height set to 0.
		/// </summary>
		public mxRectangle() : this(0, 0, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a copy of the given rectangle.
		/// </summary>
		/// <param name="rect"> Rectangle to construct a copy of. </param>
		public mxRectangle(Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
		{
		}

		/// <summary>
		/// Constructs a copy of the given rectangle.
		/// </summary>
		/// <param name="rect"> Rectangle to construct a copy of. </param>
		public mxRectangle(mxRectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
		{
		}

		/// <summary>
		/// Constructs a rectangle using the given parameters.
		/// </summary>
		/// <param name="x"> X-coordinate of the new rectangle. </param>
		/// <param name="y"> Y-coordinate of the new rectangle. </param>
		/// <param name="width"> Width of the new rectangle. </param>
		/// <param name="height"> Height of the new rectangle. </param>
		public mxRectangle(double x, double y, double width, double height) : base(x, y)
		{

			Width = width;
			Height = height;
		}

		/// <summary>
		/// Returns the width of the rectangle.
		/// </summary>
		/// <returns> Returns the width. </returns>
		public virtual double Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}


		/// <summary>
		/// Returns the height of the rectangle.
		/// </summary>
		/// <returns> Returns the height. </returns>
		public virtual double Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}


		/// <summary>
		/// Adds the given rectangle to this rectangle.
		/// </summary>
		public virtual void add(mxRectangle rect)
		{
			if (rect != null)
			{
				double minX = Math.Min(x, rect.x);
				double minY = Math.Min(y, rect.y);
				double maxX = Math.Max(x + width, rect.x + rect.width);
				double maxY = Math.Max(y + height, rect.y + rect.height);

				x = minX;
				y = minY;
				width = maxX - minX;
				height = maxY - minY;
			}
		}

		/// <summary>
		/// Returns the x-coordinate of the center.
		/// </summary>
		/// <returns> Returns the x-coordinate of the center. </returns>
		public virtual double CenterX
		{
			get
			{
				return X + Width / 2;
			}
		}

		/// <summary>
		/// Returns the y-coordinate of the center.
		/// </summary>
		/// <returns> Returns the y-coordinate of the center. </returns>
		public virtual double CenterY
		{
			get
			{
				return Y + Height / 2;
			}
		}

		/// <summary>
		/// Grows the rectangle by the given amount, that is, this method subtracts
		/// the given amount from the x- and y-coordinates and adds twice the amount
		/// to the width and height.
		/// </summary>
		/// <param name="amount"> Amount by which the rectangle should be grown. </param>
		public virtual void grow(double amount)
		{
			x -= amount;
			y -= amount;
			width += 2 * amount;
			height += 2 * amount;
		}

		/// <summary>
		/// Returns true if the given point is contained in the rectangle.
		/// </summary>
		/// <param name="x"> X-coordinate of the point. </param>
		/// <param name="y"> Y-coordinate of the point. </param>
		/// <returns> Returns true if the point is contained in the rectangle. </returns>
		public virtual bool contains(double x, double y)
		{
			return (this.x <= x && this.x + width >= x && this.y <= y && this.y + height >= y);
		}

		/// <summary>
		/// Returns the point at which the specified point intersects the perimeter 
		/// of this rectangle or null if there is no intersection.
		/// </summary>
		/// <param name="x0"> the x co-ordinate of the first point of the line </param>
		/// <param name="y0"> the y co-ordinate of the first point of the line </param>
		/// <param name="x1"> the x co-ordinate of the second point of the line </param>
		/// <param name="y1"> the y co-ordinate of the second point of the line </param>
		/// <returns> the point at which the line intersects this rectangle, or null
		/// 			if there is no intersection </returns>
		public virtual mxPoint intersectLine(double x0, double y0, double x1, double y1)
		{
			mxPoint result = null;

			result = mxUtils.intersection(x, y, x + width, y, x0, y0, x1, y1);

			if (result == null)
			{
				result = mxUtils.intersection(x + width, y, x + width, y + height, x0, y0, x1, y1);
			}

			if (result == null)
			{
				result = mxUtils.intersection(x + width, y + height, x, y + height, x0, y0, x1, y1);
			}

			if (result == null)
			{
				result = mxUtils.intersection(x, y, x, y + height, x0, y0, x1, y1);
			}

			return result;
		}

		/// <summary>
		/// Returns the bounds as a new rectangle.
		/// </summary>
		/// <returns> Returns a new rectangle for the bounds. </returns>
		public virtual Rectangle Rectangle
		{
			get
			{
				int ix = (int) Math.Round(x);
				int iy = (int) Math.Round(y);
				int iw = (int) Math.Round(width - ix + x);
				int ih = (int) Math.Round(height - iy + y);
    
				return new Rectangle(ix, iy, iw, ih);
			}
		}

		/// 
		/// <summary>
		/// Returns true if the given object equals this rectangle.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is mxRectangle)
			{
				mxRectangle rect = (mxRectangle) obj;

				return rect.X == X && rect.Y == Y && rect.Width == Width && rect.Height == Height;
			}

			return false;
		}

		/// <summary>
		/// Returns a new instance of the same rectangle.
		/// </summary>
		public override object clone()
		{
			mxRectangle clone = (mxRectangle) base.clone();

			clone.Width = Width;
			clone.Height = Height;

			return clone;
		}

		/// <summary>
		/// Returns the <code>String</code> representation of this
		/// <code>mxRectangle</code>. </summary>
		/// <returns> a <code>String</code> representing this
		/// <code>mxRectangle</code>. </returns>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[x=" + x + ",y=" + y + ",w=" + width + ",h=" + height + "]";
		}
	}

}