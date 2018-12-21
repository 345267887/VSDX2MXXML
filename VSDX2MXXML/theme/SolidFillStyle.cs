using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class SolidFillStyle : FillStyle
    {
        private OoxmlColor color;

        public SolidFillStyle(OoxmlColor color)
        {
            this.color = color;
        }

        public Color applyStyle(int styleValue, mxVsdxTheme theme)
        {
            return color.getColor(styleValue, theme);
        }
    }
}
