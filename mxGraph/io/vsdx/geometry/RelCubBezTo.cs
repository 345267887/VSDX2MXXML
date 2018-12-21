using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class RelCubBezTo : Row
	{
		public RelCubBezTo(int index, double? x, double? y, double? a, double? b, double? c, double? d) : base(index, x, y)
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
				double x = this.x.Value * 100;
				double y = 100 - this.y.Value * 100;
				double x1 = this.a.Value * 100.0;
				double y1 = 100 - this.b.Value * 100.0;
				double x2 = this.c.Value * 100.0;
				double y2 = 100 - this.d.Value * 100.0;

				x = Math.Round(x * 100.0) / 100.0;
				y = Math.Round(y * 100.0) / 100.0;
				x1 = Math.Round(x1 * 100.0) / 100.0;
				y1 = Math.Round(y1 * 100.0) / 100.0;
				x2 = Math.Round(x2 * 100.0) / 100.0;
				y2 = Math.Round(y2 * 100.0) / 100.0;

				shape.LastX = x;
				shape.LastY = y;

				return "<curve x1=\"" + x1.ToString() + "\" y1=\"" + y1.ToString() + "\" x2=\"" + x2.ToString() + "\" y2=\"" + y2.ToString() + "\" x3=\"" + x.ToString() + "\" y3=\"" + y.ToString() + "\"/>";
			}

			return "";
		}

	}

}