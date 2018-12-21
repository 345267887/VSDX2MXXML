using mxGraph;
using System;

/// <summary>
/// $Id: mxLine.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{

	/// <summary>
	/// Implements a line with double precision coordinates.
	/// </summary>

	[Serializable]
	public class mxLine : mxPoint
	{
		/// 
		private const long serialVersionUID = -4730972599169158546L;
		/// <summary>
		/// The end point of the line
		/// </summary>
		protected internal mxPoint endPoint;

		/// <summary>
		/// Creates a new line
		/// </summary>
		public mxLine(mxPoint startPt, mxPoint endPt)
		{
			this.X = startPt.X;
			this.Y = startPt.Y;
			this.endPoint = endPt;
		}

		/// <summary>
		/// Returns the end point of the line.
		/// </summary>
		/// <returns> Returns the end point of the line. </returns>
		public virtual mxPoint EndPoint
		{
			get
			{
				return this.endPoint;
			}
			set
			{
				this.endPoint = value;
			}
		}


		/// <summary>
		/// Returns the square of the shortest distance from a point to this line.
		/// The line is considered extrapolated infinitely in both directions for 
		/// the purposes of the calculation.
		/// </summary>
		/// <param name="pt"> the point whose distance is being measured </param>
		/// <returns> the square of the distance from the specified point to this line. </returns>
		public virtual double ptLineDistSq(mxPoint pt)
		{
            //return (new Line2DDouble(X, Y, endPoint.X, endPoint.Y)).ptLineDistSq(pt.X, pt.Y);

            return PointHelper.ptLineDistSq(X, Y, endPoint.X, endPoint.Y, pt.X, pt.Y);
        }

		/// <summary>
		/// Returns the square of the shortest distance from a point to this 
		/// line segment.
		/// </summary>
		/// <param name="pt"> the point whose distance is being measured </param>
		/// <returns> the square of the distance from the specified point to this segment. </returns>
		public virtual double ptSegDistSq(mxPoint pt)
		{
            //return (new Line2D.Double(X, Y, endPoint.X, endPoint.Y)).ptSegDistSq(pt.X, pt.Y);

            return PointHelper.ptSegDistSq(X, Y, endPoint.X, endPoint.Y, pt.X, pt.Y);
		}

	}

}