using System;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// $Id: mxCurve.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2009-2010, David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.util
{


	public class mxCurve
	{
		/// <summary>
		/// A collection of arrays of curve points
		/// </summary>
		protected internal IDictionary<string, mxPoint[]> points;

		// Rectangle just completely enclosing branch and label/
		protected internal double minXBounds = 10000000;

		protected internal double maxXBounds = 0;

		protected internal double minYBounds = 10000000;

		protected internal double maxYBounds = 0;

		/// <summary>
		/// An array of arrays of intervals. These intervals define the distance
		/// along the edge (0 to 1) that each point lies
		/// </summary>
		protected internal IDictionary<string, double[]> intervals;

		/// <summary>
		/// The curve lengths of the curves
		/// </summary>
		protected internal IDictionary<string, double?> curveLengths;

		/// <summary>
		/// Defines the key for the central curve index
		/// </summary>
		public static string CORE_CURVE = "Center_curve";

		/// <summary>
		/// Defines the key for the label curve index
		/// </summary>
		public static string LABEL_CURVE = "Label_curve";

		/// <summary>
		/// Indicates that an invalid position on a curve was requested
		/// </summary>
		public static mxLine INVALID_POSITION = new mxLine(new mxPoint(0, 0), new mxPoint(1, 0));

		/// <summary>
		/// Offset of the label curve from the curve the label curve is based on.
		/// If you wish to set this value, do so directly after creation of the curve.
		/// The first time the curve is used the label curve will be created with 
		/// whatever value is contained in this variable. Changes to it after that point 
		/// will have no effect.
		/// </summary>
		protected internal double labelBuffer = mxConstants.DEFAULT_LABEL_BUFFER;

		/// <summary>
		/// The points this curve is drawn through. These are typically control
		/// points and are at distances from each other that straight lines
		/// between them do not describe a smooth curve. This class takes
		/// these guiding points and creates a finer set of internal points
		/// that visually appears to be a curve when linked by straight lines
		/// </summary>
		public IList<mxPoint> guidePoints = new List<mxPoint>();

		/// <summary>
		/// Whether or not the curve currently holds valid values
		/// </summary>
		protected internal bool valid = false;

		/// 
		public virtual double LabelBuffer
		{
			set
			{
				labelBuffer = value;
			}
		}

		/// 
		public virtual mxRectangle Bounds
		{
			get
			{
				if (!valid)
				{
					createCoreCurve();
				}
				return new mxRectangle(minXBounds, minYBounds, maxXBounds - minXBounds, maxYBounds - minYBounds);
			}
		}

		/// 
		public mxCurve()
		{
		}

		/// 
		public mxCurve(IList<mxPoint> points)
		{
			bool nullPoints = false;

			foreach (mxPoint point in guidePoints)
			{
				if (point == null)
				{
					nullPoints = true;
					break;
				}
			}

			if (!nullPoints)
			{
				guidePoints = new List<mxPoint>(points);
			}
		}

		/// <summary>
		/// Calculates the index of the lower point on the segment
		/// that contains the point <i>distance</i> along the 
		/// </summary>
		protected internal virtual int getLowerIndexOfSegment(string index, double distance)
		{
			double[] curveIntervals = getIntervals(index);

			if (curveIntervals == null)
			{
				return 0;
			}

			int numIntervals = curveIntervals.Length;

			if (distance <= 0.0 || numIntervals < 3)
			{
				return 0;
			}

			if (distance >= 1.0)
			{
				return numIntervals - 2;
			}

			// Pick a starting index roughly where you expect the point
			// to be
			int testIndex = (int)(numIntervals * distance);

			if (testIndex >= numIntervals)
			{
				testIndex = numIntervals - 1;
			}

			// The max and min indices tested so far
			int lowerLimit = -1;
			int upperLimit = numIntervals;

			// It cannot take more than the number of intervals to find
			// the correct segment
			for (int i = 0; i < numIntervals; i++)
			{
				double segmentDistance = curveIntervals[testIndex];
				double multiplier = 0.5;

				if (distance < segmentDistance)
				{
					upperLimit = Math.Min(upperLimit, testIndex);
					multiplier = -0.5;
				}
				else if (distance > segmentDistance)
				{
					lowerLimit = Math.Max(lowerLimit, testIndex);
				}
				else
				{
					// Values equal
					if (testIndex == 0)
					{
						lowerLimit = 0;
						upperLimit = 1;
					}
					else
					{
						lowerLimit = testIndex - 1;
						upperLimit = testIndex;
					}
				}

				int indexDifference = upperLimit - lowerLimit;

				if (indexDifference == 1)
				{
					break;
				}

				testIndex = (int)(testIndex + indexDifference * multiplier);

				if (testIndex == lowerLimit)
				{
					testIndex = lowerLimit + 1;
				}

				if (testIndex == upperLimit)
				{
					testIndex = upperLimit - 1;
				}
			}

			if (lowerLimit != upperLimit - 1)
			{
				return -1;
			}

			return lowerLimit;
		}

		/// <summary>
		/// Returns a unit vector parallel to the curve at the specified
		/// distance along the curve. To obtain the angle the vector makes
		/// with (1,0) perform Math.atan(segVectorY/segVectorX). </summary>
		/// <param name="index"> the curve index specifying the curve to analyse </param>
		/// <param name="distance"> the distance from start to end of curve (0.0...1.0) </param>
		/// <returns> a unit vector at the specified point on the curve represented
		/// 		as a line, parallel with the curve. If the distance or curve is
		/// 		invalid, <code>mxCurve.INVALID_POSITION</code> is returned </returns>
		public virtual mxLine getCurveParallel(string index, double distance)
		{
			mxPoint[] pointsCurve = getCurvePoints(index);
			double[] curveIntervals = getIntervals(index);

			if (pointsCurve != null && pointsCurve.Length > 0 && curveIntervals != null && distance >= 0.0 && distance <= 1.0)
			{
				// If the curve is zero length, it will only have one point
				// We can't calculate in this case
				if (pointsCurve.Length == 1)
				{
					mxPoint point = pointsCurve[0];
					return new mxLine(new mxPoint(point.X, point.Y), new mxPoint(1, 0));
				}

				int lowerLimit = getLowerIndexOfSegment(index, distance);
				mxPoint firstPointOfSeg = pointsCurve[lowerLimit];
				double segVectorX = pointsCurve[lowerLimit + 1].X - firstPointOfSeg.X;
				double segVectorY = pointsCurve[lowerLimit + 1].Y - firstPointOfSeg.Y;
				double distanceAlongSeg = (distance - curveIntervals[lowerLimit]) / (curveIntervals[lowerLimit + 1] - curveIntervals[lowerLimit]);
				double segLength = Math.Sqrt(segVectorX * segVectorX + segVectorY * segVectorY);
				mxPoint startPoint = new mxPoint(firstPointOfSeg.X + segVectorX * distanceAlongSeg, firstPointOfSeg.Y + segVectorY * distanceAlongSeg);
				mxPoint endPoint = new mxPoint(segVectorX / segLength, segVectorY / segLength);
				return new mxLine(startPoint, endPoint);
			}
			else
			{
				return INVALID_POSITION;
			}
		}

		/// <summary>
		/// Returns a section of the curve as an array of points </summary>
		/// <param name="index"> the curve index specifying the curve to analyse </param>
		/// <param name="start"> the start position of the curve segment (0.0...1.0) </param>
		/// <param name="end"> the end position of the curve segment (0.0...1.0) </param>
		/// <returns> a sequence of point representing the curve section or null
		/// 			if it cannot be calculated </returns>
		public virtual mxPoint[] getCurveSection(string index, double start, double end)
		{
			mxPoint[] pointsCurve = getCurvePoints(index);
			double[] curveIntervals = getIntervals(index);

			if (pointsCurve != null && pointsCurve.Length > 0 && curveIntervals != null && start >= 0.0 && start <= 1.0 && end >= 0.0 && end <= 1.0)
			{
				// If the curve is zero length, it will only have one point
				// We can't calculate in this case
				if (pointsCurve.Length == 1)
				{
					mxPoint point = pointsCurve[0];
					return new mxPoint[] {new mxPoint(point.X, point.Y)};
				}

				int lowerLimit = getLowerIndexOfSegment(index, start);
				mxPoint firstPointOfSeg = pointsCurve[lowerLimit];
				double segVectorX = pointsCurve[lowerLimit + 1].X - firstPointOfSeg.X;
				double segVectorY = pointsCurve[lowerLimit + 1].Y - firstPointOfSeg.Y;
				double distanceAlongSeg = (start - curveIntervals[lowerLimit]) / (curveIntervals[lowerLimit + 1] - curveIntervals[lowerLimit]);
				mxPoint startPoint = new mxPoint(firstPointOfSeg.X + segVectorX * distanceAlongSeg, firstPointOfSeg.Y + segVectorY * distanceAlongSeg);

				List<mxPoint> result = new List<mxPoint>();
				result.Add(startPoint);

				double current = start;
				current = curveIntervals[++lowerLimit];

				while (current <= end)
				{
					mxPoint nextPointOfSeg = pointsCurve[lowerLimit];
					result.Add(nextPointOfSeg);
					current = curveIntervals[++lowerLimit];
				}

				// Add whatever proportion of the last segment has to 
				// be added to make the exactly end distance
				if (lowerLimit > 0 && lowerLimit < pointsCurve.Length && end > curveIntervals[lowerLimit - 1])
				{
					firstPointOfSeg = pointsCurve[lowerLimit - 1];
					segVectorX = pointsCurve[lowerLimit].X - firstPointOfSeg.X;
					segVectorY = pointsCurve[lowerLimit].Y - firstPointOfSeg.Y;
					distanceAlongSeg = (end - curveIntervals[lowerLimit - 1]) / (curveIntervals[lowerLimit] - curveIntervals[lowerLimit - 1]);
					mxPoint endPoint = new mxPoint(firstPointOfSeg.X + segVectorX * distanceAlongSeg, firstPointOfSeg.Y + segVectorY * distanceAlongSeg);
					result.Add(endPoint);
				}

				//mxPoint[] resultArray = new mxPoint[result.Count];
                return result.ToArray();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns whether or not the rectangle passed in hits any part of this
		/// curve. </summary>
		/// <param name="rect"> the rectangle to detect for a hit </param>
		/// <returns> whether or not the rectangle hits this curve </returns>
		public virtual bool intersectsRect(Rectangle rect)
		{
            // To save CPU, we can test if the rectangle intersects the entire
            // bounds of this curve
            if (!Bounds.Rectangle.IntersectsWith(rect))
			{
				return false;
			}

			mxPoint[] pointsCurve = getCurvePoints(mxCurve.CORE_CURVE);

			if (pointsCurve != null && pointsCurve.Length > 1)
			{
				mxRectangle mxRect = new mxRectangle(rect);
				// First check for any of the curve points lying within the 
				// rectangle, then for any of the curve segments intersecting 
				// with the rectangle sides
				for (int i = 1; i < pointsCurve.Length; i++)
				{
					if (mxRect.contains(pointsCurve[i].X, pointsCurve[i].Y) || mxRect.contains(pointsCurve[i - 1].X, pointsCurve[i - 1].Y))
					{
						return true;
					}
				}

				for (int i = 1; i < pointsCurve.Length; i++)
				{
					if (mxRect.intersectLine(pointsCurve[i].X, pointsCurve[i].Y, pointsCurve[i - 1].X, pointsCurve[i - 1].Y) != null)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Returns the point at which this curve intersects the boundary of 
		/// the given rectangle, if it does so. If it does not intersect, 
		/// null is returned. If it intersects multiple times, the first 
		/// intersection from the start end of the curve is returned.
		/// </summary>
		/// <param name="index"> the curve index specifying the curve to analyse </param>
		/// <param name="rect"> the whose boundary is to be tested for intersection
		/// with this curve </param>
		/// <returns> the point at which this curve intersects the boundary of 
		/// the given rectangle, if it does so. If it does not intersect, 
		/// null is returned. </returns>
		public virtual mxPoint intersectsRectPerimeter(string index, mxRectangle rect)
		{
			mxPoint result = null;
			mxPoint[] pointsCurve = getCurvePoints(index);

			if (pointsCurve != null && pointsCurve.Length > 1)
			{
				int crossingSeg = intersectRectPerimeterSeg(index, rect);

				if (crossingSeg != -1)
				{
					result = intersectRectPerimeterPoint(index, rect, crossingSeg);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the distance from the start of the curve at which this 
		/// curve intersects the boundary of the given rectangle, if it does 
		/// so. If it does not intersect, a negative value is returned. 
		/// If it intersects multiple times, the first intersection from 
		/// the start end of the curve is returned.
		/// </summary>
		/// <param name="index"> the curve index specifying the curve to analyse </param>
		/// <param name="rect"> the whose boundary is to be tested for intersection
		/// with this curve </param>
		/// <returns> the distance along the curve from the start at which
		/// the intersection occurs </returns>
		public virtual double intersectsRectPerimeterDist(string index, mxRectangle rect)
		{
			double result = -1;
			mxPoint[] pointsCurve = getCurvePoints(index);
			double[] curveIntervals = getIntervals(index);

			if (pointsCurve != null && pointsCurve.Length > 1)
			{
				int segIndex = intersectRectPerimeterSeg(index, rect);
				mxPoint intersectPoint = null;

				if (segIndex != -1)
				{
					intersectPoint = intersectRectPerimeterPoint(index, rect, segIndex);
				}

				if (intersectPoint != null)
				{
					double startSegX = pointsCurve[segIndex - 1].X;
					double startSegY = pointsCurve[segIndex - 1].Y;
					double distToStartSeg = curveIntervals[segIndex - 1] * getCurveLength(index);
					double intersectOffsetX = intersectPoint.X - startSegX;
					double intersectOffsetY = intersectPoint.Y - startSegY;
					double lenToIntersect = Math.Sqrt(intersectOffsetX * intersectOffsetX + intersectOffsetY * intersectOffsetY);
					result = distToStartSeg + lenToIntersect;
				}
			}

			return result;
		}

		/// <summary>
		/// Utility method to determine within which segment the specified rectangle
		/// intersects the specified curve
		/// </summary>
		/// <param name="index"> the curve index specifying the curve to analyse </param>
		/// <param name="rect"> the whose boundary is to be tested for intersection
		/// with this curve </param>
		/// <returns> the point at which this curve intersects the boundary of 
		/// the given rectangle, if it does so. If it does not intersect, 
		/// -1 is returned </returns>
		protected internal virtual int intersectRectPerimeterSeg(string index, mxRectangle rect)
		{
			mxPoint[] pointsCurve = getCurvePoints(index);

			if (pointsCurve != null && pointsCurve.Length > 1)
			{
				for (int i = 1; i < pointsCurve.Length; i++)
				{
					if (rect.intersectLine(pointsCurve[i].X, pointsCurve[i].Y, pointsCurve[i - 1].X, pointsCurve[i - 1].Y) != null)
					{
						return i;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns the point at which this curve segment intersects the boundary 
		/// of the given rectangle, if it does so. If it does not intersect, 
		/// null is returned.
		/// </summary>
		/// <param name="curveIndex"> the curve index specifying the curve to analyse </param>
		/// <param name="rect"> the whose boundary is to be tested for intersection
		/// with this curve </param>
		/// <param name="indexSeg"> the segments on this curve being checked </param>
		/// <returns> the point at which this curve segment  intersects the boundary 
		/// of the given rectangle, if it does so. If it does not intersect, 
		/// null is returned. </returns>
		protected internal virtual mxPoint intersectRectPerimeterPoint(string curveIndex, mxRectangle rect, int indexSeg)
		{
			mxPoint result = null;
			mxPoint[] pointsCurve = getCurvePoints(curveIndex);

			if (pointsCurve != null && pointsCurve.Length > 1 && indexSeg >= 0 && indexSeg < pointsCurve.Length)
			{
				double p1X = pointsCurve[indexSeg - 1].X;
				double p1Y = pointsCurve[indexSeg - 1].Y;
				double p2X = pointsCurve[indexSeg].X;
				double p2Y = pointsCurve[indexSeg].Y;

				result = rect.intersectLine(p1X, p1Y, p2X, p2Y);
			}

			return result;
		}

		/// <summary>
		/// Calculates the position of an absolute in terms relative
		/// to this curve.
		/// </summary>
		/// <param name="absPoint"> the point whose relative point is to calculated </param>
		/// <param name="index"> the index of the curve whom the relative position is to be 
		/// calculated from </param>
		/// <returns> an mxRectangle where the x is the distance along the curve 
		/// (0 to 1), y is the orthogonal offset from the closest segment on the 
		/// curve and (wdith, height) is an additional cartisian offset applied
		/// after the other calculations </returns>
		public virtual mxRectangle getRelativeFromAbsPoint(mxPoint absPoint, string index)
		{
			// Work out which segment the absolute point is closest to
			mxPoint[] currentCurve = getCurvePoints(index);
			double[] currentIntervals = getIntervals(index);
			int closestSegment = 0;
			double closestSegDistSq = 10000000;

			for (int i = 1; i < currentCurve.Length; i++)
			{
				mxLine segment = new mxLine(currentCurve[i - 1], currentCurve[i]);
				double segDistSq = segment.ptSegDistSq(absPoint);

				if (segDistSq < closestSegDistSq)
				{
					closestSegDistSq = segDistSq;
					closestSegment = i - 1;
				}
			}

			// Get the distance (squared) from the point to the
			// infinitely extrapolated line created by the closest
			// segment. If that value is the same as the distance
			// to the segment then an orthogonal offset from some
			// point on the line will intersect the point. If they
			// are not equal, an additional cartesian offset is
			// required
			mxPoint startSegPt = currentCurve[closestSegment];
			mxPoint endSegPt = currentCurve[closestSegment + 1];

			mxLine closestSeg = new mxLine(startSegPt, endSegPt);
			double lineDistSq = closestSeg.ptLineDistSq(absPoint);

			double orthogonalOffset = Math.Sqrt(Math.Min(lineDistSq, closestSegDistSq));
			double segX = endSegPt.X - startSegPt.X;
			double segY = endSegPt.Y - startSegPt.Y;
			double segDist = Math.Sqrt(segX * segX + segY * segY);
			double segNormX = segX / segDist;
			double segNormY = segY / segDist;
			// The orthogonal offset could be in one of two opposite vectors
			// Try both solutions, one will be closer to one of the segment
			// end points (unless the point is on the line)
			double candidateOffX1 = (absPoint.X - segNormY * orthogonalOffset) - endSegPt.X;
			double candidateOffY1 = (absPoint.Y + segNormX * orthogonalOffset) - endSegPt.Y;
			double candidateOffX2 = (absPoint.X + segNormY * orthogonalOffset) - endSegPt.X;
			double candidateOffY2 = (absPoint.Y - segNormX * orthogonalOffset) - endSegPt.Y;

			double candidateDist1 = (candidateOffX1 * candidateOffX1) + (candidateOffY1 * candidateOffY1);
			double candidateDist2 = (candidateOffX2 * candidateOffX2) + (candidateOffY2 * candidateOffY2);

			double orthOffsetPointX = 0;
			double orthOffsetPointY = 0;

			if (candidateDist2 < candidateDist1)
			{
				orthogonalOffset = -orthogonalOffset;
			}

			orthOffsetPointX = absPoint.X - segNormY * orthogonalOffset;
			orthOffsetPointY = absPoint.Y + segNormX * orthogonalOffset;

			double distAlongEdge = 0;
			double cartOffsetX = 0;
			double cartOffsetY = 0;

			// Don't compare for exact equality, there are often rounding errors
			if (Math.Abs(closestSegDistSq - lineDistSq) > 0.0001)
			{
				// The orthogonal offset does not move the point onto the
				// segment. Work out an additional cartesian offset that moves
				// the offset point onto the closest end point of the
				// segment

				// Not exact distances, but the equation holds
				double distToStartPoint = Math.Abs(orthOffsetPointX - startSegPt.X) + Math.Abs(orthOffsetPointY - startSegPt.Y);
				double distToEndPoint = Math.Abs(orthOffsetPointX - endSegPt.X) + Math.Abs(orthOffsetPointY - endSegPt.Y);
				if (distToStartPoint < distToEndPoint)
				{
					distAlongEdge = currentIntervals[closestSegment];
					cartOffsetX = orthOffsetPointX - startSegPt.X;
					cartOffsetY = orthOffsetPointY - startSegPt.Y;
				}
				else
				{
					distAlongEdge = currentIntervals[closestSegment + 1];
					cartOffsetX = orthOffsetPointX - endSegPt.X;
					cartOffsetY = orthOffsetPointY - endSegPt.Y;
				}
			}
			else
			{
				// The point, when orthogonally offset, lies on the segment
				// work out what proportion along the segment, and therefore
				// the entire curve, the offset point lies.
				double segmentLen = Math.Sqrt((endSegPt.X - startSegPt.X) * (endSegPt.X - startSegPt.X) + (endSegPt.Y - startSegPt.Y) * (endSegPt.Y - startSegPt.Y));
				double offsetLen = Math.Sqrt((orthOffsetPointX - startSegPt.X) * (orthOffsetPointX - startSegPt.X) + (orthOffsetPointY - startSegPt.Y) * (orthOffsetPointY - startSegPt.Y));
				double proportionAlongSeg = offsetLen / segmentLen;
				double segProportingDiff = currentIntervals[closestSegment + 1] - currentIntervals[closestSegment];
				distAlongEdge = currentIntervals[closestSegment] + segProportingDiff * proportionAlongSeg;
			}

			if (distAlongEdge > 1.0)
			{
				distAlongEdge = 1.0;
			}

			return new mxRectangle(distAlongEdge, orthogonalOffset, cartOffsetX, cartOffsetY);
		}

		/// <summary>
		/// Creates the actual 
		/// </summary>
		protected internal virtual void createCoreCurve()
		{
			// Curve is marked invalid until all of the error situations have
			// been checked
			valid = false;

			if (guidePoints == null || guidePoints.Count == 0)
			{
				return;
			}

			for (int i = 0; i < guidePoints.Count; i++)
			{
				if (guidePoints[i] == null)
				{
					return;
				}
			}

			// Reset the cached bounds value
			minXBounds = minYBounds = 10000000;
			maxXBounds = maxYBounds = 0;

			mxSpline spline = new mxSpline(guidePoints);

			// Need the rough length of the spline, so we can get
			// more samples for longer edges
			double lengthSpline = spline.Length;

			// Check for errors in the spline calculation or zero length curves
			if (double.IsNaN(lengthSpline) || !spline.checkValues() || lengthSpline < 1)
			{
				return;
			}

			mxSpline1D splineX = spline.SplineX;
			mxSpline1D splineY = spline.SplineY;
			double baseInterval = 12.0 / lengthSpline;
			double minInterval = 1.0 / lengthSpline;

			// Store the last two spline positions. If the next position is 
			// very close to where the extrapolation of the last two points 
			// then double the interval. This diviation is terms the "flatness".
			// There is a range where the interval is kept the same, any 
			// variation from this range of flatness invokes a proportional 
			// adjustment to try to reenter the range without 
			// over compensating
			double interval = baseInterval;
			// These deviations are only tested against either 
			// dimension individually, working out the correct 
			// distance is too computationally intensive
			double minDeviation = 0.15;
			double maxDeviation = 0.3;
			double preferedDeviation = (maxDeviation + minDeviation) / 2.0;

			// x1, y1 are the position two iterations ago, x2, y2
			// the position on the last iteration
			double x1 = -1.0;
			double x2 = -1.0;
			double y1 = -1.0;
			double y2 = -1.0;

			// Store the change in interval amount between iterations.
			// If it changes the extrapolation calculation must
			// take this into account.
			double intervalChange = 1;

			IList<mxPoint> coreCurve = new List<mxPoint>();
			IList<double?> coreIntervals = new List<double?>();
			bool twoLoopsComplete = false;

			for (double t = 0; t <= 1.5; t += interval)
			{
				if (t > 1.0)
				{
					// Use the point regardless of the accuracy, 
					t = 1.0001;
					mxPoint endControlPoint = guidePoints[guidePoints.Count - 1];
					mxPoint finalPoint = new mxPoint(endControlPoint.X, endControlPoint.Y);
					coreCurve.Add(finalPoint);
					coreIntervals.Add(t);
					updateBounds(endControlPoint.X, endControlPoint.Y);
					break;
				}
				// Whether or not the accuracy of the current point is acceptable
				bool currentPointAccepted = true;

				double newX = splineX.getFastValue(t);
				double newY = splineY.getFastValue(t);

				// Check if the last points are valid (indicated by
				// dissimilar values)
				// Check we're not in the first, second or last run
				if (x1 != -1.0 && twoLoopsComplete && t != 1.0001)
				{
					// Work out how far the new spline point
					// deviates from the extrapolation created 
					// by the last two points
					double diffX = Math.Abs(((x2 - x1) * intervalChange + x2) - newX);
					double diffY = Math.Abs(((y2 - y1) * intervalChange + y2) - newY);

					// If either the x or y of the straight line
					// extrapolation from the last two points
					// is more than the 1D deviation allowed
					// go back and re-calculate with a smaller interval
					// It's possible that the edge has curved too fast
					// for the algorithmn. If the interval is
					// reduced to less than the minimum permitted
					// interval, it may be that it's impossible
					// to get within the deviation because of
					// the extrapolation overshoot. The minimum 
					// interval is set to draw correctly for the
					// vast majority of cases.
					if ((diffX > maxDeviation || diffY > maxDeviation) && interval != minInterval)
					{
						double overshootProportion = maxDeviation / Math.Max(diffX, diffY);

						if (interval * overshootProportion <= minInterval)
						{
							// Set the interval 
							intervalChange = minInterval / interval;
						}
						else
						{
							// The interval can still be reduced, half 
							// the interval and go back and redo
							// this iteration
							intervalChange = overshootProportion;
						}

						t -= interval;
						interval *= intervalChange;
						currentPointAccepted = false;
					}
					else if (diffX < minDeviation && diffY < minDeviation)
					{
						intervalChange = 1.4;
						interval *= intervalChange;
					}
					else
					{
						// Try to keep the deviation around the prefered value
						double errorRatio = preferedDeviation / Math.Max(diffX, diffY);
						intervalChange = errorRatio / 4.0;
						interval *= intervalChange;
					}

					if (currentPointAccepted)
					{
						x1 = x2;
						y1 = y2;
						x2 = newX;
						y2 = newY;
					}
				}
				else if (x1 == -1.0)
				{
					x1 = x2 = newX;
					y1 = y2 = newY;
				}
				else if (x1 == x2 && y1 == y2)
				{
					x2 = newX;
					y2 = newY;
					twoLoopsComplete = true;
				}
				if (currentPointAccepted)
				{
					mxPoint newPoint = new mxPoint(newX, newY);
					coreCurve.Add(newPoint);
					coreIntervals.Add(t);
					updateBounds(newX, newY);
				}
			}

			if (coreCurve.Count < 2)
			{
				// A single point makes no sense, leave the curve as invalid
				return;
			}

			mxPoint[] corePoints = new mxPoint[coreCurve.Count];
			int count = 0;

			foreach (mxPoint point in coreCurve)
			{
				corePoints[count++] = point;
			}

			points = new Dictionary<string, mxPoint[]>();
			curveLengths = new Dictionary<string, double?>();
			points[CORE_CURVE] = corePoints;
			curveLengths[CORE_CURVE] = lengthSpline;

			double[] coreIntervalsArray = new double[coreIntervals.Count];
			count = 0;

			foreach (double? tempInterval in coreIntervals)
			{
				coreIntervalsArray[count++] = tempInterval.Value;
			}

			intervals = new Dictionary<string, double[]>();
			intervals[CORE_CURVE] = coreIntervalsArray;

			valid = true;
		}

		public virtual void configureLabelCurve()
		{

		}

		/// <summary>
		/// Whether or not the label curve starts from the end target
		///  and traces to the start of the branch </summary>
		/// <returns> whether the label curve is reversed </returns>
		public virtual bool LabelReversed
		{
			get
			{
				if (valid)
				{
					mxPoint[] centralCurve = getCurvePoints(CORE_CURVE);
    
					if (centralCurve != null)
					{
						double changeX = centralCurve[centralCurve.Length - 1].X - centralCurve[0].X;
    
						if (changeX < 0)
						{
							return true;
						}
					}
				}
    
				return false;
			}
		}

		protected internal virtual void createLabelCurve()
		{
			// Place the label on the "high" side of the vector
			// joining the start and end points of the curve
			mxPoint[] currentCurve = BaseLabelCurve;

			bool labelReversed = LabelReversed;

			List<mxPoint> labelCurvePoints = new List<mxPoint>();

			// Lower and upper curve start from the very ends
			// of their curves, so given that their middle points
			// are dervied from the center of the central points
			// they will contain one more point and both
			// side curves contain the same end point

			for (int i = 1; i < currentCurve.Length; i++)
			{
				int currentIndex = i;
				int lastIndex = i - 1;

				if (labelReversed)
				{
					currentIndex = currentCurve.Length - i - 1;
					lastIndex = currentCurve.Length - i;
				}

				mxPoint segStartPoint = currentCurve[currentIndex];
				mxPoint segEndPoint = currentCurve[lastIndex];
				double segVectorX = segEndPoint.X - segStartPoint.X;
				double segVectorY = segEndPoint.Y - segStartPoint.Y;
				double segVectorLength = Math.Sqrt(segVectorX * segVectorX + segVectorY * segVectorY);
				double normSegVectorX = segVectorX / segVectorLength;
				double normSegVectorY = segVectorY / segVectorLength;
				double centerSegX = (segEndPoint.X + segStartPoint.X) / 2.0;
				double centerSegY = (segEndPoint.Y + segStartPoint.Y) / 2.0;

				if (i == 1)
				{
					// Special case to work out the very end points at
					// the start of the curve
					mxPoint startPoint = new mxPoint(segEndPoint.X - (normSegVectorY * labelBuffer), segEndPoint.Y + (normSegVectorX * labelBuffer));
					labelCurvePoints.Add(startPoint);
					updateBounds(startPoint.X, startPoint.Y);
				}

				double pointX = centerSegX - (normSegVectorY * labelBuffer);
				double pointY = centerSegY + (normSegVectorX * labelBuffer);
				mxPoint labelCurvePoint = new mxPoint(pointX, pointY);
				updateBounds(pointX, pointY);
				labelCurvePoints.Add(labelCurvePoint);

				if (i == currentCurve.Length - 1)
				{
					// Special case to work out the very end points at
					// the start of the curve
					mxPoint endPoint = new mxPoint(segStartPoint.X - (normSegVectorY * labelBuffer), segStartPoint.Y + (normSegVectorX * labelBuffer));
					labelCurvePoints.Add(endPoint);
					updateBounds(endPoint.X, endPoint.Y);
				}
			}

			//mxPoint[] tmpPoints = new mxPoint[labelCurvePoints.Count];
			points[LABEL_CURVE] = labelCurvePoints.ToArray();
			populateIntervals(LABEL_CURVE);
		}

		/// <summary>
		/// Returns the curve the label curve is too be based on
		/// </summary>
		protected internal virtual mxPoint[] BaseLabelCurve
		{
			get
			{
				return getCurvePoints(CORE_CURVE);
			}
		}

		protected internal virtual void populateIntervals(string index)
		{
			mxPoint[] currentCurve = points[index];

			double[] newIntervals = new double[currentCurve.Length];

			double totalLength = 0.0;
			newIntervals[0] = 0;

			for (int i = 0; i < currentCurve.Length - 1; i++)
			{
				double changeX = currentCurve[i + 1].X - currentCurve[i].X;
				double changeY = currentCurve[i + 1].Y - currentCurve[i].Y;
				double segLength = Math.Sqrt(changeX * changeX + changeY * changeY);
				// We initially fill the intervals with the total distance to
				// the end of this segment then later normalize all the values
				totalLength += segLength;
				// The first index was populated before the loop (and is always 0)
				newIntervals[i + 1] = totalLength;
			}

			// Normalize the intervals
			for (int j = 0; j < newIntervals.Length; j++)
			{
				if (j == newIntervals.Length - 1)
				{
					// Make the final interval slightly over
					// 1.0 so any analysis to find the lower 
					newIntervals[j] = 1.0001;
				}
				else
				{
					newIntervals[j] = newIntervals[j] / totalLength;
				}
			}

			intervals[index] = newIntervals;
			curveLengths[index] = totalLength;
		}

		/// <summary>
		/// Updates the existing curve using the points passed in. </summary>
		/// <param name="newPoints"> the new guide points </param>
		public virtual void updateCurve(IList<mxPoint> newPoints)
		{
			bool pointsChanged = false;

			// If any of the new points are null, ignore the list
			foreach (mxPoint point in newPoints)
			{
				if (point == null)
				{
					return;
				}
			}

			if (newPoints.Count != guidePoints.Count)
			{
				pointsChanged = true;
			}
			else
			{
				// Check for a constant translation of all guide points. In that 
				// case apply the translation directly to all curves.
				// Also check whether all of the translations are trivial
				if (newPoints.Count == guidePoints.Count && newPoints.Count > 1 && guidePoints.Count > 1)
				{
					bool constantTranslation = true;
					bool trivialTranslation = true;
					mxPoint newPoint0 = newPoints[0];
					mxPoint oldPoint0 = guidePoints[0];
					double transX = newPoint0.X - oldPoint0.X;
					double transY = newPoint0.Y - oldPoint0.Y;

					if (Math.Abs(transX) > 0.01 || Math.Abs(transY) > 0.01)
					{
						trivialTranslation = false;
					}

					for (int i = 1; i < newPoints.Count; i++)
					{
						double nextTransX = newPoints[i].X - guidePoints[i].X;
						double nextTransY = newPoints[i].Y - guidePoints[i].Y;

						if (Math.Abs(transX - nextTransX) > 0.01 || Math.Abs(transY - nextTransY) > 0.01)
						{
							constantTranslation = false;
						}

						if (Math.Abs(nextTransX) > 0.01 || Math.Abs(nextTransY) > 0.01)
						{
							trivialTranslation = false;
						}
					}

					if (trivialTranslation)
					{
						pointsChanged = false;
					}
					else if (constantTranslation)
					{
						pointsChanged = false;
                        // Translate all stored points by the translation amounts
                        ICollection< mxPoint[]> curves = points.Values;

						// Update all geometry information held by the curve
						// That is, all the curve points, the guide points
						// and the cached bounds
						foreach (mxPoint[] curve in curves)
						{
							for (int i = 0; i < curve.Length; i++)
							{
								curve[i].X = curve[i].X + transX;
								curve[i].Y = curve[i].Y + transY;
							}
						}

						guidePoints = new List<mxPoint>(newPoints);
						minXBounds += transX;
						minYBounds += transY;
						maxXBounds += transX;
						maxYBounds += transY;
					}
					else
					{
						pointsChanged = true;
					}
				}
			}

			if (pointsChanged)
			{
				guidePoints = new List<mxPoint>(newPoints);
				points = new Dictionary<string, mxPoint[]>();
				valid = false;
			}
		}

		/// <summary>
		/// Obtains the points that make up the curve for the specified
		/// curve index. If that curve, or the core curve that other curves
		/// are based on have not yet been created, then they are lazily
		/// created. If creation is impossible, null is returned </summary>
		/// <param name="index"> the key specifying the curve </param>
		/// <returns> the points making up that curve, or null </returns>
		public virtual mxPoint[] getCurvePoints(string index)
		{
			if (validateCurve())
			{
				if (points[LABEL_CURVE] == null && string.ReferenceEquals(index, LABEL_CURVE))
				{
					createLabelCurve();
				}

				return points[index];
			}

			return null;
		}

		public virtual double[] getIntervals(string index)
		{
			if (validateCurve())
			{
				if (points[LABEL_CURVE] == null && string.ReferenceEquals(index, LABEL_CURVE))
				{
					createLabelCurve();
				}

				return intervals[index];
			}

			return null;
		}

		public virtual double getCurveLength(string index)
		{
			if (validateCurve())
			{
				if (intervals[index] == null)
				{
					createLabelCurve();
				}

				return curveLengths[index].Value;
			}

			return 0;
		}

		/// <summary>
		/// Method must be called before any attempt to access curve information </summary>
		/// <returns> whether or not the curve may be used </returns>
		protected internal virtual bool validateCurve()
		{
			if (!valid)
			{
				createCoreCurve();
			}

			return valid;
		}

		/// <summary>
		/// Updates the total bounds of this curve, increasing any dimensions,
		/// if necessary, to fit in the specified point
		/// </summary>
		protected internal virtual void updateBounds(double pointX, double pointY)
		{
			minXBounds = Math.Min(minXBounds, pointX);
			maxXBounds = Math.Max(maxXBounds, pointX);
			minYBounds = Math.Min(minYBounds, pointY);
			maxYBounds = Math.Max(maxYBounds, pointY);
		}

		/// <returns> the guidePoints </returns>
		public virtual IList<mxPoint> GuidePoints
		{
			get
			{
				return guidePoints;
			}
		}
	}

}