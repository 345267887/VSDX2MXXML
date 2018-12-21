using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class RelLineTo : Row
	{

		public RelLineTo(int index, double? x, double? y) : base(index, x, y)
		{
		}

		public override string handle(mxPoint p, Shape shape)
		{
			double x = p.X, y = p.Y;

			if (this.x != null && this.y != null)
			{
				x = this.x.Value * 100;
				y = 100 - this.y.Value * 100;
			}

			x = Math.Round(x * 100.0) / 100.0;
			y = Math.Round(y * 100.0) / 100.0;

			p.X = x;
			p.Y = y;
			shape.LastX = x;
			shape.LastY = y;

			return "<" + "line" + " x=\"" + x.ToString() + "\" y=\"" + y.ToString() + "\"/>";
		}

	}

}