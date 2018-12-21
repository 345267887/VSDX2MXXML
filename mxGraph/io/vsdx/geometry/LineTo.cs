using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class LineTo : Row
	{

		public LineTo(int index, double? x, double? y) : base(index, x, y)
		{
		}

		public override string handle(mxPoint p, Shape shape)
		{
			double x = p.X, y = p.Y;
			double h = shape.Height;
			double w = shape.Width;

			if (this.x != null && this.y != null)
			{
				x = this.x.Value * mxVsdxUtils.conversionFactor;
				y = this.y.Value * mxVsdxUtils.conversionFactor;
			}

			x = x * 100.0 / w;
			y = y * 100.0 / h;
			y = 100 - y;

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