using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    public class VsdxBatchConvert
    {
        public string Execute(byte[] file)
        {
            MxVsdxCodec vdxCodec = new MxVsdxCodec();
            String xml = null;
            xml = vdxCodec.decodeVsdx(file, "ISO-8859-1");

            return xml;
        }
    }
}
