namespace mxGraph.io.vsdx.theme
{

	public class NoFillStyle : FillStyle
	{

		public virtual Color applyStyle(int styleValue, mxVsdxTheme theme)
		{
			return Color.NONE;
		}

	}

}