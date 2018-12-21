namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public class DelRow : Row
	{

		public DelRow(int index) : base(index, null, null)
		{
		}

		public override string handle(mxPoint p, Shape shape)
		{
			//Nothing
			return "";
		}

	}

}