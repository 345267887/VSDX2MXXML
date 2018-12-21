using System;
using System.Collections.Generic;

namespace mxGraph.io.vsdx.geometry
{


	using mxPoint = mxGraph.util.mxPoint;

	public class PolylineTo : Row
	{
		public PolylineTo(int index, double? x, double? y, string a) : base(index, x, y)
		{
			this.formulaA = a;
		}

		public override string handle(mxPoint p, Shape shape)
		{
			string result = "";

			if (this.x != null && this.y != null && !string.ReferenceEquals(this.formulaA, null))
			{
				double h = shape.Height;
				double w = shape.Width;
				double x = this.x.Value * mxVsdxUtils.conversionFactor;
				double y = this.y.Value * mxVsdxUtils.conversionFactor;
				x = x * 100.0 / w;
				y = y * 100.0 / h;
				y = 100 - y;
				x = Math.Round(x * 100.0) / 100.0;
				y = Math.Round(y * 100.0) / 100.0;

				string aValue = this.formulaA.Replace("\\s","").ToLower().Replace("polyline\\(","").Replace("\\)", "");

				LinkedList<string> polyEntriesList = new LinkedList<string>((aValue.Split(',')));

                //JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
                polyEntriesList.RemoveFirst();
                //JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
                polyEntriesList.RemoveFirst();
				double currX = 0;
				double currY = 0;

				while (polyEntriesList.Count > 0)
				{
                    //JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
                    currX = Convert.ToDouble(polyEntriesList.First) * mxVsdxUtils.conversionFactor;
                    polyEntriesList.RemoveFirst();
//JAVA TO C# CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the Java 'remove' method:
                    currY = Convert.ToDouble(polyEntriesList.First) * mxVsdxUtils.conversionFactor;
                    polyEntriesList.RemoveFirst();
                    currY = 100 - currY;

					currX = Math.Round(currX * 100.0) / 100.0;
					currY = Math.Round(currY * 100.0) / 100.0;

					shape.LastX = currX;
					shape.LastY = currY;

					result += "<line x=\"" + currX.ToString() + "\" y=\"" + currY.ToString() + "\"/>";
				}

				if (shape.LastMoveX == x && shape.LastMoveY == y)
				{
					result += "<close/>";
				}
			}

			return result;

		}

	}

}