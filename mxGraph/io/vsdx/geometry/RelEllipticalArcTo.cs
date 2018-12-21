namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class RelEllipticalArcTo : EllipticalArcTo
	{
		public RelEllipticalArcTo(int index, double? x, double? y, double? a, double? b, double? c, double? d) : base(index, x, y, a, b, c, d)
		{
		}

		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && this.a != null && this.b != null && this.c != null && this.d != null)
			{
				double h = shape.Height / mxVsdxUtils.conversionFactor;
				double w = shape.Width / mxVsdxUtils.conversionFactor;
				this.x *= w;
				this.y *= h;
				this.a *= w;
				this.b *= h;
			}
			return base.handle(p, shape);
		}
	}

}