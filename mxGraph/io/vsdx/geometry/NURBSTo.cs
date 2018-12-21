using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace mxGraph.io.vsdx.geometry
{


	using mxPoint = mxGraph.util.mxPoint;

	public class NURBSTo : Row
	{
		public NURBSTo(int index, double? x, double? y, double? a, double? b, double? c, double? d, string e) : base(index, x, y)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			this.formulaE = e;
		}

		public override string handle(mxPoint p, Shape shape)
		{
			if (this.x != null && this.y != null && !string.ReferenceEquals(this.formulaE, null))
			{
				double h = shape.Height;
				double w = shape.Width;

				double x = this.x.Value * mxVsdxUtils.conversionFactor;
				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				string eValue = this.formulaE.Replace("NURBS(", "");
				eValue = eValue.Replace(")", "");

                IList<string> nurbsValues = Regex.Split(eValue, "\\s*,\\s*", RegexOptions.None);//Arrays.asList(eValue.Split("\\s*,\\s*", true));

				if (nurbsValues.Count >= 10)
				{
					double x1 = double.Parse(nurbsValues[4]) * 100.0;
					double y1 = 100 - double.Parse(nurbsValues[5]) * 100.0;
					double x2 = double.Parse(nurbsValues[8]) * 100.0;
					double y2 = 100 - double.Parse(nurbsValues[9]) * 100.0;

					y = y * 100.0 / h;
					x = x * 100.0 / w;
					y = 100 - y;
					x = Math.Round(x * 100.0) / 100.0;
					y = Math.Round(y * 100.0) / 100.0;
					x1 = Math.Round(x1 * 100.0) / 100.0;
					y1 = Math.Round(y1 * 100.0) / 100.0;
					x2 = Math.Round(x2 * 100.0) / 100.0;
					y2 = Math.Round(y2 * 100.0) / 100.0;

					if (debug != null)
					{
						debug.drawRect(x, y, "");
						debug.drawLine(shape.LastX, shape.LastY, x, y, "");
					}

					shape.LastX = x;
					shape.LastY = y;

					return "<curve x1=\"" + x1.ToString() + "\" y1=\"" + y1.ToString() + "\" x2=\"" + x2.ToString() + "\" y2=\"" + y2.ToString() + "\" x3=\"" + x.ToString() + "\" y3=\"" + y.ToString() + "\"/>";
				}
			}

			return "";

		}

	}

}