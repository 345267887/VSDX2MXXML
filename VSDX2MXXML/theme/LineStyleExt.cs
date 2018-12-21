using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class LineStyleExt : LineStyle
    {
        private int rndg = 0, start = 0, startSize = 0, end = 0, endSize = 0, pattern = 0;
        private List<Double> lineDashPattern;

        public LineStyleExt(XmlElement elem)
        {
            XmlElement lineEx = mxVsdxUtils.getDirectFirstChildElement(elem); //vt:lineEx element of vt:lineStyle

            //parse the line style ext xml
            rndg = mxVsdxUtils.getIntAttr(lineEx, "rndg");
            start = mxVsdxUtils.getIntAttr(lineEx, "start");
            startSize = mxVsdxUtils.getIntAttr(lineEx, "startSize");
            end = mxVsdxUtils.getIntAttr(lineEx, "end");
            endSize = mxVsdxUtils.getIntAttr(lineEx, "endSize");
            pattern = mxVsdxUtils.getIntAttr(lineEx, "pattern");
            lineDashPattern = Style.getLineDashPattern(pattern);
        }

        public int getRndg()
        {
            return rndg;
        }

        public new int getStart()
        {
            return start;
        }

        public new int getStartSize()
        {
            return startSize;
        }

        public new int getEnd()
        {
            return end;
        }

        public new int getEndSize()
        {
            return endSize;
        }

        public new bool isDashed()
        {
            return pattern > 1;
        }

        public new List<Double> getLineDashPattern()
        {
            return lineDashPattern;
        }
    }
}
