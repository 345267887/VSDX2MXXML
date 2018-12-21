using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class ArcTo : Row
	{
		public ArcTo(int index, double? x, double? y, double? a) : base(index, x, y)
		{
			this.a = a;
		}

		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && this.a != null)
			{
				double h = shape.Height;
				double w = shape.Width;
				double x0 = Math.Round(shape.LastX * w) / 100;
				double y0 = Math.Round(shape.LastY * h) / 100;
				double x = this.x.Value * mxVsdxUtils.conversionFactor;

				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				y = h - y;

				double a = this.a.Value * mxVsdxUtils.conversionFactor;

				double dx = Math.Abs(x - x0);
				double dy = Math.Abs(y - y0);

				double rx = (a * 0.5) + (dx * dx + dy * dy) / (8.0 * a);
				double ry = rx;
				double r0 = Math.Abs(rx);

				rx = rx * 100 / w;
				ry = ry * 100 / h;
				x = x * 100 / w;
				y = y * 100 / h;
				rx = Math.Round(rx * 100.0) / 100.0;
				ry = Math.Round(ry * 100.0) / 100.0;
				x = Math.Round(x * 100.0) / 100.0;
				y = Math.Round(y * 100.0) / 100.0;

				a = Math.Round(a * 100.0) / 100.0;
				rx = Math.Abs(rx);
				ry = Math.Abs(ry);

				//determine sweep and large-arc flag
				string sf = (a < 0) ? "1" : "0";
				string laf = (r0 < Math.Abs(a)) ? "1" : "0";

				if (debug != null)
				{
					debug.drawLine(x0, y0, x, y, "");
				}

				shape.LastX = x;
				shape.LastY = y;

				return "<arc" + " rx=\"" + rx.ToString() + "\" ry=\"" + ry.ToString() + "\" x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\" x-axis-rotation=\"0" + "\" large-arc-flag=\"" + laf + "\" sweep-flag=\"" + sf + "\"/>";
			}

			return "";

		}

	}

}