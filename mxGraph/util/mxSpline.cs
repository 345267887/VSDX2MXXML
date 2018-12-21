using System;
using System.Collections.Generic;

namespace mxGraph.util
{

	public class mxSpline
	{
		/// <summary>
		/// Array representing the relative proportion of the total distance
		/// of each point in the line ( i.e. first point is 0.0, end point is
		/// 1.0, a point halfway on line is 0.5 ).
		/// </summary>
		private double[] t;
		private mxSpline1D splineX;
		private mxSpline1D splineY;

		/// <summary>
		/// Total length tracing the points on the spline
		/// </summary>
		private double length;

		public mxSpline(IList<mxPoint> points)
		{
			if (points != null)
			{
				double[] x = new double[points.Count];
				double[] y = new double[points.Count];
				int i = 0;

				foreach (mxPoint point in points)
				{
					x[i] = point.X;
					y[i++] = point.Y;
				}

				init(x, y);
			}
		}

		/// <summary>
		/// Creates a new mxSpline. </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		public virtual void Spline2D(double[] x, double[] y)
		{
			init(x, y);
		}

		protected internal virtual void init(double[] x, double[] y)
		{
			if (x.Length != y.Length)
			{
				// Arrays must have the same length
				// TODO log something
				return;
			}

			if (x.Length < 2)
			{
				// Spline edges must have at least two points
				// TODO log something
				return;
			}

			t = new double[x.Length];
			t[0] = 0.0; // start point is always 0.0
			length = 0.0;

			// Calculate the partial proportions of each section between each set
			// of points and the total length of sum of all sections
			for (int i = 1; i < t.Length; i++)
			{
				double lx = x[i] - x[i - 1];
				double ly = y[i] - y[i - 1];

				// If either diff is zero there is no point performing the square root
				if (0.0 == lx)
				{
					t[i] = Math.Abs(ly);
				}
				else if (0.0 == ly)
				{
					t[i] = Math.Abs(lx);
				}
				else
				{
					t[i] = Math.Sqrt(lx * lx + ly * ly);
				}

				length += t[i];
				t[i] += t[i - 1];
			}

			for (int i2 = 1; i2 < (t.Length) - 1; i2++)
			{
				t[i2] = t[i2] / length;
			}

			t[(t.Length) - 1] = 1.0; // end point is always 1.0

			splineX = new mxSpline1D(t, x);
			splineY = new mxSpline1D(t, y);
		}

		/// <param name="t"> 0 <= t <= 1 </param>
		public virtual mxPoint getPoint(double t)
		{
			mxPoint result = new mxPoint(splineX.getValue(t), splineY.getValue(t));

			return result;
		}

		/// <summary>
		/// Used to check the correctness of this spline
		/// </summary>
		public virtual bool checkValues()
		{
			return (splineX.checkValues() && splineY.checkValues());
		}

		public virtual double getDx(double t)
		{
			return splineX.getDx(t);
		}

		public virtual double getDy(double t)
		{
			return splineY.getDx(t);
		}

		public virtual mxSpline1D SplineX
		{
			get
			{
				return splineX;
			}
		}

		public virtual mxSpline1D SplineY
		{
			get
			{
				return splineY;
			}
		}

		public virtual double Length
		{
			get
			{
				return length;
			}
		}
	}

}