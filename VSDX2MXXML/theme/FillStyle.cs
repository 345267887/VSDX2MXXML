using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    interface FillStyle
    {
        Color applyStyle(int styleValue, mxVsdxTheme theme);
    }
}
