using System;
using System.Collections.Generic;

namespace mxGraph.io.vsdx.geometry
{


	using Element = System.Xml.XmlElement;

	public class RowFactory
	{
		private static double? getDoubleVal(string val)
		{
			try
			{
				if (!string.ReferenceEquals(val, null) && val.Length > 0)
				{
					return Convert.ToDouble(val);
				}
			}
			catch (Exception)
			{
				//nothing
			}
			return null;
		}

		public static Row getRowObj(Element elem, IList<Row> pRows)
		{
			string rowType = elem.GetAttribute("T");
			int index = int.Parse(elem.GetAttribute("IX"));
			string del = elem.GetAttribute("Del");
			if (!del.Equals("1"))
			{
				Row parentObj = null;

				if (index <= pRows.Count)
				{
					parentObj = pRows[index - 1];
				}

				double? x = null, y = null, a = null, b = null, c = null, d = null;
				string formulaE = null, formulaA = null;

				if (parentObj != null)
				{
					x = parentObj.X;
					y = parentObj.Y;
					a = parentObj.A;
					b = parentObj.B;
					c = parentObj.C;
					d = parentObj.D;
					formulaA = parentObj.FormulaA;
					formulaE = parentObj.FormulaE;
				}

				List<Element> cells = mxVsdxUtils.getDirectChildElements(elem);

				foreach (Element cell in cells)
				{
                    string name = cell.GetAttribute("N");
					string val = cell.GetAttribute("V");

					switch (name)
					{
						case "X":
							x = getDoubleVal(val);
						break;
						case "Y":
							y = getDoubleVal(val);
						break;
						case "A":
							a = getDoubleVal(val);
							//Special case for PolylineTo where we need the F attribute instead of V
							formulaA = cell.Attributes["F"].Value;
						break;
						case "B":
							b = getDoubleVal(val);
						break;
						case "C":
							c = getDoubleVal(val);
						break;
						case "D":
							d = getDoubleVal(val);
						break;
						case "E":
							formulaE = val;
						break;
					}
				}

				switch (rowType)
				{
					case "MoveTo":
						return new MoveTo(index, x, y);
					case "LineTo":
						return new LineTo(index, x, y);
					case "ArcTo":
						return new ArcTo(index, x, y, a);
					case "Ellipse":
						return new Ellipse(index, x, y, a, b, c, d);
					case "EllipticalArcTo":
						return new EllipticalArcTo(index, x, y, a, b, c, d);
					case "InfiniteLine":
						return new InfiniteLine(index, x, y, a, b);
					case "NURBSTo":
						return new NURBSTo(index, x, y, a, b, c, d, formulaE);
					case "PolylineTo":
						return new PolylineTo(index, x, y, formulaA);
					case "RelCubBezTo":
						return new RelCubBezTo(index, x, y, a, b, c, d);
					case "RelEllipticalArcTo":
						return new RelEllipticalArcTo(index, x, y, a, b, c, d);
					case "RelLineTo":
						return new RelLineTo(index, x, y);
					case "RelMoveTo":
						return new RelMoveTo(index, x, y);
					case "RelQuadBezTo":
						return new RelQuadBezTo(index, x, y, a, b);
					case "SplineKnot":
						return new SplineKnot(index, x, y, a);
					case "SplineStart":
						return new SplineStart(index, x, y, a, b, c, d);
				}
			}
			return new DelRow(index);
		}
	}

}