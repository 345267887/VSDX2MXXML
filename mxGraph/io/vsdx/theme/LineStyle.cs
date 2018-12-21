using System.Collections.Generic;

namespace mxGraph.io.vsdx.theme
{

	using Element = System.Xml.XmlElement;


	public class LineStyle
	{

		public enum LineCapType
		{
			ROUND,
			SQUARE,
			FLAT
		}

		public enum CompoundLineType
		{
			SINGLE,
			DOUBLE,
			THICK_THIN_DOUBLE,
			THIN_THICK_DOUBLE,
			THIN_THICK_THIN_TRIPLE
		}

		public enum LineEndType
		{
			NONE,
			TRIANGLE,
			STEALTH,
			DIAMOND,
			OVAL,
			ARROW
		}

		private int lineWidth;
		private LineCapType lineCap;
		private CompoundLineType lineComp;
		private FillStyle fillStyle;

		private bool isLineDashed = false;
		private List<double?> lineDashPattern = new List<double?>();

		private bool isRoundJoin = false, isBevelJoin = false, isMiterJoin = false;

		private LineEndType headEndType;
		private int headEndWidth, headEndLen;

		private LineEndType tailEndType;
		private int tailEndWidth, tailEndLen;

		public LineStyle()
		{

		}

		public LineStyle(Element elem)
		{
			//parse the line style xml
			lineWidth = mxVsdxUtils.getIntAttr(elem, "w");

            string lineCapAtt = elem.GetAttribute("cap");

			if (!string.ReferenceEquals(lineCapAtt, null))
			{
				switch (lineCapAtt)
				{
					case "rnd":
						lineCap = LineCapType.ROUND;
					break;
					case "sq":
						lineCap = LineCapType.SQUARE;
					break;
					case "flat":
						lineCap = LineCapType.FLAT;
					break;
				}
			}

			string lineCompAtt = elem.GetAttribute("cmpd");

			if (!string.ReferenceEquals(lineCompAtt, null))
			{
				switch (lineCompAtt)
				{
					case "sng":
						lineComp = CompoundLineType.SINGLE;
					break;
					case "dbl":
						lineComp = CompoundLineType.DOUBLE;
					break;
					case "thickThin":
						lineComp = CompoundLineType.THICK_THIN_DOUBLE;
					break;
					case "thinThick":
						lineComp = CompoundLineType.THIN_THICK_DOUBLE;
					break;
					case "tri":
						lineComp = CompoundLineType.THIN_THICK_THIN_TRIPLE;
					break;
				}
			}

			//TODO add algn (Stroke Alignment) attrinbute support [http://www.datypic.com/sc/ooxml/a-algn-4.html]

			List<Element> subElems = mxVsdxUtils.getDirectChildElements(elem);

			foreach (Element subElem in subElems)
			{
				string name = subElem.Name;

				switch (name)
				{
					case "a:noFill":
					case "a:solidFill":
					case "a:gradFill":
					case "a:pattFill":
						fillStyle = FillStyleFactory.getFillStyle(subElem);
					break;
					case "a:prstDash":
						string val = subElem.GetAttribute("val");

						isLineDashed = true;
						switch (val)
						{
							case "solid":
								isLineDashed = false;
							break;
							case "sysDot":
							case "dot":
								lineDashPattern.Add(1.0);
								lineDashPattern.Add(4.0);
							break;
							case "sysDash":
							case "dash":
								//use the default dashed pattern
							break;
							case "lgDash":
								lineDashPattern.Add(12.0);
								lineDashPattern.Add(4.0);
							break;
							case "sysDashDot":
							case "dashDot":
								lineDashPattern.Add(8.0);
								lineDashPattern.Add(4.0);
								lineDashPattern.Add(1.0);
								lineDashPattern.Add(4.0);
							break;
							case "lgDashDot":
								lineDashPattern.Add(12.0);
								lineDashPattern.Add(4.0);
								lineDashPattern.Add(1.0);
								lineDashPattern.Add(4.0);
							break;
							case "sysDashDotDot":
							case "lgDashDotDot":
								lineDashPattern.Add(12.0);
								lineDashPattern.Add(4.0);
								lineDashPattern.Add(1.0);
								lineDashPattern.Add(4.0);
								lineDashPattern.Add(1.0);
								lineDashPattern.Add(4.0);
							break;
						}
					break;
					case "a:custDash":
						isLineDashed = true;
						List<Element> dsElems = mxVsdxUtils.getDirectChildNamedElements(subElem, "a:ds");
						foreach (Element dsElem in dsElems)
						{
							int dashLen = mxVsdxUtils.getIntAttr(dsElem, "d");
							int spaceLen = mxVsdxUtils.getIntAttr(dsElem, "sp");
							//TODO find the correct conversion ratio from vsdx to mxGraph
							lineDashPattern.Add(dashLen / 10000.0);
							lineDashPattern.Add(spaceLen / 10000.0);
						}
					break;
					//https://www.w3schools.com/tags/playcanvas.asp?filename=playcanvas_lineJoin
					case "a:round": //Round Line Join
						isRoundJoin = true;
					break;
					case "a:bevel": //Bevel Line Join
						isBevelJoin = true;
					break;
					case "a:miter": //Miter Line Join
						//	Miter Join Limit
						int limit = mxVsdxUtils.getIntAttr(subElem, "lim"); //?
						isMiterJoin = true;
					break;
					case "a:headEnd": //Line Head/End Style
						headEndType = getLineEndType(subElem);
						headEndWidth = mxVsdxUtils.getIntAttr(subElem, "w");
						headEndLen = mxVsdxUtils.getIntAttr(subElem, "len");
					break;
					case "a:tailEnd": //Tail line end style
						tailEndType = getLineEndType(subElem);
						tailEndWidth = mxVsdxUtils.getIntAttr(subElem, "w");
						tailEndLen = mxVsdxUtils.getIntAttr(subElem, "len");
					break;
					case "a:extLst": //Extension List!
					break;
				}
			}
		}

		private LineEndType getLineEndType(Element subElem)
		{
            string type = subElem.GetAttribute("type");
			LineEndType endType = LineEndType.NONE;
			switch (type)
			{
				case "none":
					endType = LineEndType.NONE;
				break;
				case "triangle":
					endType = LineEndType.TRIANGLE;
				break;
				case "stealth":
					endType = LineEndType.STEALTH;
				break;
				case "diamond":
					endType = LineEndType.DIAMOND;
				break;
				case "oval":
					endType = LineEndType.OVAL;
				break;
				case "arrow":
					endType = LineEndType.ARROW;
				break;
			}
			return endType;
		}

		public virtual Color getLineColor(int lineColorStyle, mxVsdxTheme theme)
		{
			if (fillStyle != null)
			{
				return fillStyle.applyStyle(lineColorStyle, theme);
			}
			else
			{
				return theme.DefaultLineClr;
			}
		}

		public virtual bool Dashed
		{
			get
			{
				return isLineDashed;
			}
		}

		public virtual List<double?> LineDashPattern
		{
			get
			{
				return lineDashPattern;
			}
		}

		public virtual int StartSize
		{
			get
			{
				// TODO Implement this if it is needed
				return 4;
			}
		}

		public virtual int EndSize
		{
			get
			{
				// TODO Implement this if it is needed
				return 4;
			}
		}

		public virtual int Start
		{
			get
			{
				// TODO Implement this if it is needed
				return 0;
			}
		}

		public virtual int End
		{
			get
			{
				// TODO Implement this if it is needed
				return 0;
			}
		}

		public virtual int LineWidth
		{
			get
			{
				return lineWidth;
			}
		}
	}

}