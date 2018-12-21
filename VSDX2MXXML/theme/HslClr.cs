﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class HslClr: OoxmlColor
    {
        private double hue, sat, lum;

        public HslClr(int hue, int sat, int lum)
        {
            this.hue = hue / 360.0;
            this.sat = sat / 100.0;
            this.lum = lum / 100.0;
            color = new HSLColor(hue, sat, lum).toRgb();
        }
    }
}
