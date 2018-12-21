using System;

namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class SplineKnot : Row
	{
		public SplineKnot(int index, double? x, double? y, double? a) : base(index, x, y)
		{
			this.a = a;
		}

		//TODO Is this complete?
		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && this.a != null)
			{
				//double h = this.getHeight();
				//double w = this.getWidth();
				double x = this.x.Value * mxVsdxUtils.conversionFactor;
				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				double a = this.a.Value;

				double knot = a;
	//				x = x * 100.0 / w;
	//				y = y * 100.0 / h;
				y = 100 - y;
				x = Math.Round(x * 100.0) / 100.0;
				y = Math.Round(y * 100.0) / 100.0;
				knot = Math.Round(knot * 100.0) / 100.0;


				if (debug != null)
				{
					debug.drawRect(x, y, Convert.ToString(knot));
					debug.drawLine(shape.LastX, shape.LastY, x, y, "");
				}

				shape.LastX = x;
				shape.LastY = y;
			}

			return "";

		}

	}

}