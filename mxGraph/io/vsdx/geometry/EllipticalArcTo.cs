using mxGraph;
using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class EllipticalArcTo : Row
	{
		public EllipticalArcTo(int index, double? x, double? y, double? a, double? b, double? c, double? d) : base(index, x, y)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
		}

		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && this.a != null && this.b != null && this.c != null && this.d != null)
			{
				double h = shape.Height;
				double w = shape.Width;

				double x = this.x.Value * mxVsdxUtils.conversionFactor;
				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				y = h - y;
				double a = this.a.Value * mxVsdxUtils.conversionFactor;
				double b = this.b.Value * mxVsdxUtils.conversionFactor;
				double c = this.c.Value;
				double d = this.d.Value;

				x = x * 100.0 / w;
				y = y * 100.0 / h;

				double x1 = shape.LastX * w / 100.0;
				double y1 = shape.LastY * h / 100.0;

				double x2 = x * w / 100.0;
				double y2 = y * h / 100.0;

				double x3 = a;
				double y3 = h - b;

				double ang = -c;

				double p1x = Math.Sqrt(x1 * x1 + y1 * y1) * Math.Cos(Math.Atan2(y1, x1) - ang);
				double p1y = Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sin(Math.Atan2(y1, x1) - ang);

				double p2x = Math.Sqrt(x2 * x2 + y2 * y2) * Math.Cos(Math.Atan2(y2, x2) - ang);
				double p2y = Math.Sqrt(x2 * x2 + y2 * y2) * Math.Sin(Math.Atan2(y2, x2) - ang);

				double p3x = Math.Sqrt(x3 * x3 + y3 * y3) * Math.Cos(Math.Atan2(y3, x3) - ang);
				double p3y = Math.Sqrt(x3 * x3 + y3 * y3) * Math.Sin(Math.Atan2(y3, x3) - ang);

				double p0x = ((p1x - p2x) * (p1x + p2x) * (p2y - p3y) - (p2x - p3x) * (p2x + p3x) * (p1y - p2y) + d * d * (p1y - p2y) * (p2y - p3y) * (p1y - p3y)) / (2 * ((p1x - p2x) * (p2y - p3y) - (p2x - p3x) * (p1y - p2y)));
				double p0y = ((p1x - p2x) * (p2x - p3x) * (p1x - p3x) / (d * d) + (p2x - p3x) * (p1y - p2y) * (p1y + p2y) - (p1x - p2x) * (p2y - p3y) * (p2y + p3y)) / (2 * ((p2x - p3x) * (p1y - p2y) - (p1x - p2x) * (p2y - p3y)));

				double newX = Math.Sqrt(p0x * p0x + p0y * p0y) * Math.Cos(Math.Atan2(p0y, p0x) + ang);
				double newY = Math.Sqrt(p0x * p0x + p0y * p0y) * Math.Sin(Math.Atan2(p0y, p0x) + ang);

				newX = newX * w / 100.0;
				newY = newY * h / 100.0;

				double dx = p1x - p0x;
				double dy = p1y - p0y;
				double rx = Math.Sqrt(dx * dx + dy * dy * d * d);
				double ry = rx / d;
                double rot = Common.ToDegrees(ang); //Math.toDegrees(ang);


                rx = rx * 100.0 / w;
				ry = ry * 100.0 / h;

				x = Math.Round(x * 100.0) / 100.0;
				y = Math.Round(y * 100.0) / 100.0;
				rx = Math.Round(rx * 100.0) / 100.0;
				ry = Math.Round(ry * 100.0) / 100.0;
				rot = Math.Round(rot * 100.0) / 100.0;

				//determine sweep
				//TODO fix rare error (file "1 Supported Forms" shape "storeddata" on page 5)
				double sweep = (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
				string sf = (sweep > 0) ? "0" : "1";

				//determine large arc flag
				string laf = "0";

				if (mxVsdxUtils.isInsideTriangle(p0x, p0y, p1x, p1y, p2x, p2y, p3x, p3y) && isReflexAngle(p0x, p0y, p1x, p1y, p2x, p2y, p3x, p3y))
				{
					laf = "1";
				}

				if (debug != null)
				{
					debug.drawRect(p0x, p0y, "P0");
					debug.drawRect(p1x, p1y, "P1");
					debug.drawRect(p2x, p2y, "P2");
					debug.drawRect(p3x, p3y, "P3");
					debug.drawRect(newX, newY, "X");
					debug.drawRect(x3, y3, "CP");
					debug.drawLine(x1, y1, x2, y2, "");
				}

				shape.LastX = x;
				shape.LastY = y;

				return "<arc" + " rx=\"" + rx.ToString() + "\" ry=\"" + ry.ToString() + "\" x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\" x-axis-rotation=\"" + rot.ToString() + "\" large-arc-flag=\"" + laf + "\" sweep-flag=\"" + sf + "\"/>";
			}

			return "";
		}

		/// <param name="x0"> y0 center point of ellipse containing the arc </param>
		/// <param name="x1"> y1 starting point of the arc </param>
		/// <param name="x2"> y2 endpoint of the arc </param>
		/// <param name="x3"> y3 control point </param>
		/// <returns> true if the start to end angle that contains the control point is a reflex angle  </returns>
		protected internal virtual bool isReflexAngle(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3)
		{
			x1 = x1 - x0;
			y1 = y1 - y0;
			x2 = x2 - x0;
			y2 = y2 - y0;
			x2 = x3 - x0;
			y3 = y3 - y0;
			x0 = 0;
			y0 = 0;

            double aStart = Common.ToDegrees(Math.Atan2(y1, x1) - Math.Atan2(y0, x0)); //Math.toDegrees(Math.Atan2(y1, x1) - Math.Atan2(y0, x0));
			double aEnd = Common.ToDegrees(Math.Atan2(y2, x2) - Math.Atan2(y0, x0)); //Math.toDegrees(Math.Atan2(y2, x2) - Math.Atan2(y0, x0));
            double aCP = Common.ToDegrees(Math.Atan2(y3, x3) - Math.Atan2(y0, x0)); //Math.toDegrees(Math.Atan2(y3, x3) - Math.Atan2(y0, x0));

            aStart = (aStart - aCP) % 360;
			aEnd = (aEnd - aCP) % 360;

			if (aStart > 180)
			{
				aStart = aStart - 360;
			}
			else if (aStart < -180)
			{
				aStart = aStart + 360;
			}

			if (aEnd > 180)
			{
				aEnd = aEnd - 360;
			}
			else if (aEnd < -180)
			{
				aEnd = aEnd + 360;
			}

			if ((aStart > 0 && aEnd < 0) || (aStart < 0 && aEnd > 0))
			{
				if (Math.Abs(aStart - aEnd) > 180)
				{
					return true;
				}
			}

			return false;
		}

	}

}