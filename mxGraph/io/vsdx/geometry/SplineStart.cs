using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class SplineStart : Row
	{
		public SplineStart(int index, double? x, double? y, double? a, double? b, double? c, double? d) : base(index, x, y)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
		}

		//TODO Is this complete?
		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && this.a != null && this.b != null && this.c != null && this.d != null)
			{
				double h = shape.Height;
				double w = shape.Width;

				double x = this.x.Value * mxVsdxUtils.conversionFactor;
				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				//double a = Double.parseDouble(aValue);
				//double b = Double.parseDouble(bValue);
				double c = this.c.Value;
				double d = this.d.Value;

				//double firstKnot = b;
				//double secondKnot = a;
				double lastKnot = c;

				shape.LastKnot = lastKnot;

				double degree = d;
	//				x = x * 100.0 / w;
	//				y = y * 100.0 / h;
				y = 100 - y;
				x = Math.Round(x * 100.0) / 100.0;
				y = Math.Round(y * 100.0) / 100.0;
				lastKnot = Math.Round(lastKnot * 100.0) / 100.0;
				double x0 = shape.LastX * w / 100.0;
				double y0 = shape.LastY * h / 100.0;

				if (debug != null)
				{
					debug.drawRect(x0, y0, "0, " + Convert.ToString(degree));
					debug.drawRect(x, y, Convert.ToString(lastKnot));
					debug.drawLine(x0, y0, x, y, "");
				}

				shape.LastX = x;
				shape.LastY = y;

				return "<curve ";
			}

			return "";

		}

	}

}