using System.Collections.Generic;

namespace mxGraph.io.vsdx.theme
{

	using Element = System.Xml.XmlElement;


	public class LineStyleExt
	{

		private int rndg = 0, start = 0, startSize = 0, end = 0, endSize = 0, pattern = 0;
		private List<double?> lineDashPattern;

		public LineStyleExt(Element elem)
		{
			Element lineEx = mxVsdxUtils.getDirectFirstChildElement(elem); //vt:lineEx element of vt:lineStyle

			//parse the line style ext xml
			rndg = mxVsdxUtils.getIntAttr(lineEx, "rndg");
			start = mxVsdxUtils.getIntAttr(lineEx, "start");
			startSize = mxVsdxUtils.getIntAttr(lineEx, "startSize");
			end = mxVsdxUtils.getIntAttr(lineEx, "end");
			endSize = mxVsdxUtils.getIntAttr(lineEx, "endSize");
			pattern = mxVsdxUtils.getIntAttr(lineEx, "pattern");
			lineDashPattern = Style.getLineDashPattern(pattern);
		}

		public virtual int Rndg
		{
			get
			{
				return rndg;
			}
		}

		public virtual int Start
		{
			get
			{
				return start;
			}
		}

		public virtual int StartSize
		{
			get
			{
				return startSize;
			}
		}

		public virtual int End
		{
			get
			{
				return end;
			}
		}

		public virtual int EndSize
		{
			get
			{
				return endSize;
			}
		}

		public virtual bool Dashed
		{
			get
			{
				return pattern > 1;
			}
		}

		public virtual List<double?> LineDashPattern
		{
			get
			{
				return lineDashPattern;
			}
		}
	}

}