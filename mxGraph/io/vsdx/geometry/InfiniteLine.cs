namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class InfiniteLine : Row
	{
		public InfiniteLine(int index, double? x, double? y, double? a, double? b) : base(index, x, y)
		{
			this.a = a;
			this.b = b;
		}

		public override string handle(mxPoint p, Shape shape)
		{
			//TODO implement this!
			return "";
		}

	}

}