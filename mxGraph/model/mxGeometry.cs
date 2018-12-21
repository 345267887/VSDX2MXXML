using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxGeometry.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.model
{


	using mxPoint = mxGraph.util.mxPoint;
	using mxRectangle = mxGraph.util.mxRectangle;

	/// <summary>
	/// Represents the geometry of a cell. For vertices, the geometry consists
	/// of the x- and y-location, as well as the width and height. For edges,
	/// the geometry either defines the source- and target-terminal, or it
	/// defines the respective terminal points.
	/// 
	/// For edges, if the geometry is relative (default), then the x-coordinate
	/// is used to describe the distance from the center of the edge from -1 to 1
	/// with 0 being the center of the edge and the default value, and the
	/// y-coordinate is used to describe the absolute, orthogonal distance in
	/// pixels from that point. In addition, the offset is used as an absolute
	/// offset vector from the resulting point. 
	/// </summary>
	[Serializable]
	public class mxGeometry : mxRectangle
	{

		/// 
		private const long serialVersionUID = 2649828026610336589L;

		/// <summary>
		/// Global switch to translate the points in translate. Default is true.
		/// </summary>
		[NonSerialized]
		public static bool TRANSLATE_CONTROL_POINTS = true;

		/// <summary>
		/// Stores alternate values for x, y, width and height in a rectangle.
		/// Default is null.
		/// </summary>
		protected internal mxRectangle alternateBounds;

		/// <summary>
		/// Defines the source- and target-point of the edge. This is used if the
		/// corresponding edge does not have a source vertex. Otherwise it is
		/// ignored. Default is null.
		/// </summary>
		protected internal mxPoint sourcePoint, targetPoint;

		/// <summary>
		/// List of mxPoints which specifies the control points along the edge.
		/// These points are the intermediate points on the edge, for the endpoints
		/// use targetPoint and sourcePoint or set the terminals of the edge to
		/// a non-null value. Default is null.
		/// </summary>
		protected internal IList<mxPoint> points;

		/// <summary>
		/// Holds the offset of the label for edges. This is the absolute vector
		/// between the center of the edge and the top, left point of the label.
		/// Default is null.
		/// </summary>
		protected internal mxPoint offset;

		/// <summary>
		/// Specifies if the coordinates in the geometry are to be interpreted as
		/// relative coordinates. Default is false. This is used to mark a geometry
		/// with an x- and y-coordinate that is used to describe an edge label
		/// position, or a relative location with respect to a parent cell's
		/// width and height.
		/// </summary>
		protected internal bool relative = false;

		/// <summary>
		/// Constructs a new geometry at (0, 0) with the width and height set to 0.
		/// </summary>
		public mxGeometry() : this(0, 0, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a geometry using the given parameters.
		/// </summary>
		/// <param name="x"> X-coordinate of the new geometry. </param>
		/// <param name="y"> Y-coordinate of the new geometry. </param>
		/// <param name="width"> Width of the new geometry. </param>
		/// <param name="height"> Height of the new geometry. </param>
		public mxGeometry(double x, double y, double width, double height) : base(x, y, width, height)
		{
		}

		/// <summary>
		/// Returns the alternate bounds.
		/// </summary>
		public virtual mxRectangle AlternateBounds
		{
			get
			{
				return alternateBounds;
			}
			set
			{
				alternateBounds = value;
			}
		}


		/// <summary>
		/// Returns the source point.
		/// </summary>
		/// <returns> Returns the source point. </returns>
		public virtual mxPoint SourcePoint
		{
			get
			{
				return sourcePoint;
			}
			set
			{
				this.sourcePoint = value;
			}
		}


		/// <summary>
		/// Returns the target point.
		/// </summary>
		/// <returns> Returns the target point. </returns>
		public virtual mxPoint TargetPoint
		{
			get
			{
				return targetPoint;
			}
			set
			{
				this.targetPoint = value;
			}
		}


		/// <summary>
		/// Returns the list of control points.
		/// </summary>
		public virtual IList<mxPoint> Points
		{
			get
			{
				return points;
			}
			set
			{
				points = value;
			}
		}


		/// <summary>
		/// Returns the offset.
		/// </summary>
		public virtual mxPoint Offset
		{
			get
			{
				return offset;
			}
			set
			{
				this.offset = value;
			}
		}


		/// <summary>
		/// Returns true of the geometry is relative.
		/// </summary>
		public virtual bool Relative
		{
			get
			{
				return relative;
			}
			set
			{
				relative = value;
			}
		}


		/// <summary>
		/// Swaps the x, y, width and height with the values stored in
		/// alternateBounds and puts the previous values into alternateBounds as
		/// a rectangle. This operation is carried-out in-place, that is, using the
		/// existing geometry instance. If this operation is called during a graph
		/// model transactional change, then the geometry should be cloned before
		/// calling this method and setting the geometry of the cell using
		/// model.setGeometry.
		/// </summary>
		public virtual void swap()
		{
			if (alternateBounds != null)
			{
				mxRectangle old = new mxRectangle(X, Y, Width, Height);

				x = alternateBounds.X;
				y = alternateBounds.Y;
				width = alternateBounds.Width;
				height = alternateBounds.Height;

				alternateBounds = old;
			}
		}

		/// <summary>
		/// Returns the point representing the source or target point of this edge.
		/// This is only used if the edge has no source or target vertex.
		/// </summary>
		/// <param name="isSource"> Boolean that specifies if the source or target point
		/// should be returned. </param>
		/// <returns> Returns the source or target point. </returns>
		public virtual mxPoint getTerminalPoint(bool isSource)
		{
			return (isSource) ? sourcePoint : targetPoint;
		}

		/// <summary>
		/// Sets the sourcePoint or targetPoint to the given point and returns the
		/// new point.
		/// </summary>
		/// <param name="point"> Point to be used as the new source or target point. </param>
		/// <param name="isSource"> Boolean that specifies if the source or target point
		/// should be set. </param>
		/// <returns> Returns the new point. </returns>
		public virtual mxPoint setTerminalPoint(mxPoint point, bool isSource)
		{
			if (isSource)
			{
				sourcePoint = point;
			}
			else
			{
				targetPoint = point;
			}

			return point;
		}

		/// <summary>
		/// Translates the geometry by the specified amount. That is, x and y of the
		/// geometry, the sourcePoint, targetPoint and all elements of points are
		/// translated by the given amount. X and y are only translated if the
		/// geometry is not relative. If TRANSLATE_CONTROL_POINTS is false, then
		/// are not modified by this function.
		/// </summary>
		/// <param name="dx"> Integer that specifies the x-coordinate of the translation. </param>
		/// <param name="dy"> Integer that specifies the y-coordinate of the translation. </param>
		public virtual void translate(double dx, double dy)
		{
			// Translates the geometry
			if (!Relative)
			{
				x += dx;
				y += dy;
			}

			// Translates the source point
			if (sourcePoint != null)
			{
				sourcePoint.X = sourcePoint.X + dx;
				sourcePoint.Y = sourcePoint.Y + dy;
			}

			// Translates the target point
			if (targetPoint != null)
			{
				targetPoint.X = targetPoint.X + dx;
				targetPoint.Y = targetPoint.Y + dy;
			}

			// Translate the control points
			if (TRANSLATE_CONTROL_POINTS && points != null)
			{
				int count = points.Count;

				for (int i = 0; i < count; i++)
				{
					mxPoint pt = points[i];

					pt.X = pt.X + dx;
					pt.Y = pt.Y + dy;
				}
			}
		}

		/// <summary>
		/// Returns a clone of the cell.
		/// </summary>
		public override object clone()
		{
			mxGeometry clone = (mxGeometry) base.clone();

			clone.X = X;
			clone.Y = Y;
			clone.Width = Width;
			clone.Height = Height;
			clone.Relative = Relative;

			IList<mxPoint> pts = Points;

			if (pts != null)
			{
				clone.points = new List<mxPoint>(pts.Count);

				for (int i = 0; i < pts.Count; i++)
				{
					clone.points.Add((mxPoint) pts[i].clone());
				}
			}

			mxPoint tp = TargetPoint;

			if (tp != null)
			{
				clone.TargetPoint = (mxPoint) tp.clone();
			}

			mxPoint sp = SourcePoint;

			if (sp != null)
			{
				SourcePoint = (mxPoint) sp.clone();
			}

			mxPoint off = Offset;

			if (off != null)
			{
				clone.Offset = (mxPoint) off.clone();
			}

			mxRectangle alt = AlternateBounds;

			if (alt != null)
			{
				AlternateBounds = (mxRectangle) alt.clone();
			}

			return clone;
		}

	}

}