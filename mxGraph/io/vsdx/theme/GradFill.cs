using System.Collections.Generic;

namespace mxGraph.io.vsdx.theme
{

	using Element = System.Xml.XmlElement;


	//mxGraph doesn't support such a complex gradient fill style. So, we will approximate the gradient by the first two colors only
	public class GradFill : FillStyle
	{
		private OoxmlColor color1 = null, color2 = null;

		public GradFill(Element elem)
		{
			List<Element> gsLst = mxVsdxUtils.getDirectChildNamedElements(elem, "a:gsLst");

			if (gsLst.Count > 0)
			{
				List<Element> gs = mxVsdxUtils.getDirectChildElements(gsLst[0]);

				//approximate gradient by first and last color in the list
				if (gs.Count >= 2)
				{
					color2 = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(gs[0]));
					color1 = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(gs[gs.Count - 1]));
				}
			}

			if (color1 == null)
			{
				color1 = color2 = new SrgbClr("FFFFFF");
			}
		}

		public virtual Color applyStyle(int styleValue, mxVsdxTheme theme)
		{
			Color color = color1.getColor(styleValue, theme);
			color.GradientClr = color2.getColor(styleValue, theme);
			return color;
		}

	}

}