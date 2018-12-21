using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class ScrgbClr: OoxmlColor
    {
        private int r, g, b;


        public ScrgbClr(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            color = new Color(r, g, b);
        }
    }
}
