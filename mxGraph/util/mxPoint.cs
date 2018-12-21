using System;
using System.Drawing;

/// <summary>
/// $Id: mxPoint.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{


	/// <summary>
	/// Implements a 2-dimensional point with double precision coordinates.
	/// </summary>
	[Serializable]
	public class mxPoint 
	{

		/// 
		private const long serialVersionUID = 6554231393215892186L;

		/// <summary>
		/// Holds the x- and y-coordinates of the point. Default is 0.
		/// </summary>
		protected internal double x, y;

		/// <summary>
		/// Constructs a new point at (0, 0).
		/// </summary>
		public mxPoint() : this(0, 0)
		{
		}

		/// <summary>
		/// Constructs a new point at the location of the given point.
		/// </summary>
		/// <param name="point"> Point that specifies the location. </param>
		public mxPoint(Point point) : this(point.X, point.Y)
		{
		}

		/// <summary>
		/// Constructs a new point at the location of the given point.
		/// </summary>
		/// <param name="point"> Point that specifies the location. </param>
		public mxPoint(mxPoint point) : this(point.X, point.Y)
		{
		}

		/// <summary>
		/// Constructs a new point at (x, y).
		/// </summary>
		/// <param name="x"> X-coordinate of the point to be created. </param>
		/// <param name="y"> Y-coordinate of the point to be created. </param>
		public mxPoint(double x, double y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Returns the x-coordinate of the point.
		/// </summary>
		/// <returns> Returns the x-coordinate. </returns>
		public virtual double X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}


		/// <summary>
		/// Returns the x-coordinate of the point.
		/// </summary>
		/// <returns> Returns the x-coordinate. </returns>
		public virtual double Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}


		/// <summary>
		/// Returns the coordinates as a new point.
		/// </summary>
		/// <returns> Returns a new point for the location. </returns>
		public virtual Point Point
		{
			get
			{
				return new Point((int) Math.Round(x), (int) Math.Round(y));
			}
		}

		/// 
		/// <summary>
		/// Returns true if the given object equals this rectangle.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is mxPoint)
			{
				mxPoint pt = (mxPoint) obj;

				return pt.X == X && pt.Y == Y;
			}

			return false;
		}

		/// <summary>
		/// Returns a new instance of the same point.
		/// </summary>
		public virtual object clone()
		{
			mxPoint clone=new mxPoint ();

			//try
			//{
			//	clone = (mxPoint) base.clone();
			//}
			//catch (Exception)
			//{
			//	clone = new mxPoint();
			//}

			clone.X = X;
			clone.Y = Y;

			return clone;
		}

		/// <summary>
		/// Returns a <code>String</code> that represents the value
		/// of this <code>mxPoint</code>. </summary>
		/// <returns> a string representation of this <code>mxPoint</code>. </returns>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return this.GetType().FullName + "[" + x + ", " + y + "]";
		}
	}

}