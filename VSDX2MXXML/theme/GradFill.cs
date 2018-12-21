using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class GradFill : FillStyle
    {
        private OoxmlColor color1 = null, color2 = null;

        public GradFill(XmlElement elem)
        {
            List<XmlElement> gsLst = mxVsdxUtils.getDirectChildNamedElements(elem, "a:gsLst");

            if (gsLst.Count > 0)
            {
                List<XmlElement> gs = mxVsdxUtils.getDirectChildElements(gsLst[0]);

                //approximate gradient by first and last color in the list
                if (gs.Count >= 2)
                {
                    color2 = OoxmlColorFactory.getOoxmlColor(
                            mxVsdxUtils.getDirectFirstChildElement(gs[0]));
                    color1 = OoxmlColorFactory.getOoxmlColor(
                            mxVsdxUtils.getDirectFirstChildElement(gs[gs.Count - 1]));
                }
            }

            if (color1 == null)
            {
                color1 = color2 = new SrgbClr("FFFFFF");
            }
        }


        public Color applyStyle(int styleValue, mxVsdxTheme theme)
        {
            Color color = color1.getColor(styleValue, theme);
            color.setGradientClr(color2.getColor(styleValue, theme));
            return color;
        }
    }
}
