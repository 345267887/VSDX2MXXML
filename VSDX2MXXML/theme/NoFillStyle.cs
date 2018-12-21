using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class NoFillStyle: FillStyle
    {
        public Color applyStyle(int styleValue, mxVsdxTheme theme)
        {
            return Color.NONE;
        }
    }
}
