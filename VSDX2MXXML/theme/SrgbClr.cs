using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class SrgbClr: OoxmlColor
    {
        private String hexVal;
        public SrgbClr(String hexVal)
        {
            this.hexVal = hexVal;
            color = Color.decodeColorHex(hexVal);
        }
    }
}
