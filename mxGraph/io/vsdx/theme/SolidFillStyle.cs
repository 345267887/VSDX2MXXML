namespace mxGraph.io.vsdx.theme
{

	public class SolidFillStyle : FillStyle
	{

		private OoxmlColor color;

		public SolidFillStyle(OoxmlColor color)
		{
			this.color = color;
		}

		public virtual Color applyStyle(int styleValue, mxVsdxTheme theme)
		{
			return color.getColor(styleValue, theme);
		}
	}

}