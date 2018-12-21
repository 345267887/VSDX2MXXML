using System;
using System.Collections.Generic;
using System.Text;

namespace mxGraph.io.vsdx
{


	using Element = System.Xml.XmlElement;

	using LineTo = mxGraph.io.vsdx.geometry.LineTo;
	using MoveTo = mxGraph.io.vsdx.geometry.MoveTo;
	using Row = mxGraph.io.vsdx.geometry.Row;
	using mxPoint = mxGraph.util.mxPoint;

	public class mxVsdxGeometryList
	{
		private List<mxVsdxGeometry> geomList = new List<mxVsdxGeometry>();
		private List<mxVsdxGeometry> parentGeomList = null;
		private bool sortNeeded = false;

		public mxVsdxGeometryList(mxVsdxGeometryList parentGeoList)
		{
			if (parentGeoList != null)
			{
				parentGeomList = parentGeoList.geomList;
				((List<mxVsdxGeometry>)geomList).AddRange(parentGeoList.geomList);
			}
		}

		public virtual void addGeometry(Element geoElem)
		{
			mxVsdxGeometry geo = new mxVsdxGeometry(geoElem, parentGeomList);

			if (geo.Index < geomList.Count)
			{
				geomList[geo.Index] = geo;
			}
			else
			{
				geomList.Add(geo);
				sortNeeded = true;
			}
		}

		private void sort()
		{
			if (sortNeeded)
			{
				geomList.Sort(new ComparatorAnonymousInnerClass(this));
				sortNeeded = false;
			}
		}

		private class ComparatorAnonymousInnerClass : IComparer<mxVsdxGeometry>
		{
			private readonly mxVsdxGeometryList outerInstance;

			public ComparatorAnonymousInnerClass(mxVsdxGeometryList outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(mxVsdxGeometry g1, mxVsdxGeometry g2)
			{
				return g1.Index - g2.Index;
			}
		}

		public virtual bool NoShow
		{
			get
			{
				foreach (mxVsdxGeometry geo in geomList)
				{
					if (!geo.NoShow)
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool NoFill
		{
			get
			{
				foreach (mxVsdxGeometry geo in geomList)
				{
					if (!(geo.NoShow || geo.NoFill))
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool NoLine
		{
			get
			{
				foreach (mxVsdxGeometry geo in geomList)
				{
					if (!(geo.NoShow || geo.NoLine))
					{
						return false;
					}
				}
				return true;
			}
		}

		public virtual bool hasGeom()
		{
			return geomList.Count > 0;
		}

		private void rotatedPoint(mxPoint pt, double cos, double sin)
		{
			double x1 = pt.X * cos - pt.Y * sin;
			double y1 = pt.Y * cos + pt.X * sin;

			pt.X = x1;
			pt.Y = y1;
		}

		/// <summary>
		/// Returns the list of routing points of a edge shape. </summary>
		/// <param name="parentHeight"> Height of the parent of the shape. </param>
		/// <returns> List of mxPoint that represents the routing points. </returns>
		public virtual IList<mxPoint> getRoutingPoints(double parentHeight, mxPoint startPoint, double rotation)
		{
			sort();

			IList<mxPoint> points = new List<mxPoint>();

			//Adding the starting point as a routing point instead of setting the entryX/Y
			points.Add(new mxPoint(startPoint));

			double offsetX = 0;
			double offsetY = 0;

			foreach (mxVsdxGeometry geo in geomList)
			{
				if (!geo.NoShow)
				{
					List<Row> rows = geo.Rows;

					foreach (Row row in rows)
					{
						if (row is MoveTo)
						{
							offsetX = row.X.HasValue? row.X.Value : 0;
							offsetY = row.Y.HasValue? row.Y.Value : 0;
						}
						else if (row is LineTo)
						{

							double x = row.X.HasValue? row.X.Value : 0, y = row.Y.HasValue? row.Y.Value : 0;

							mxPoint p = new mxPoint(x, y);
							if (rotation != 0)
							{
								rotation =mxGraph.Common.ToRadians(360 - rotation); //Math.toRadians(360 - rotation);
                                rotatedPoint(p, Math.Cos(rotation), Math.Sin(rotation));
							}

							x = (p.X - offsetX) * mxVsdxUtils.conversionFactor;
							x += startPoint.X;

							y = ((p.Y - offsetY) * mxVsdxUtils.conversionFactor) * -1;
							y += startPoint.Y;

							x = Math.Round(x * 100.0) / 100.0;
							y = Math.Round(y * 100.0) / 100.0;

							p.X = x;
							p.Y = y;
							points.Add(p);
						}
					}
				}
			}

			return points;
		}

		public virtual string getShapeXML(Shape shape)
		{
			mxPoint p = new mxPoint(0, 0);

			StringBuilder parsedGeom = new StringBuilder("<shape strokewidth=\"inherit\"><foreground>");
			int initSize = parsedGeom.Length;

			int lastGeoStyle = -1;

			//first all geo with fill then without
			lastGeoStyle = processGeo(shape, p, parsedGeom, lastGeoStyle, true);

			lastGeoStyle = processGeo(shape, p, parsedGeom, lastGeoStyle, false);

			if (parsedGeom.Length == initSize)
			{
				return "";
			}
			else
			{
				closePath(parsedGeom, lastGeoStyle);
			}

			//System.out.println(parsedGeom);

			parsedGeom.Append("</foreground></shape>");
			return parsedGeom.ToString();
		}

		private int processGeo(Shape shape, mxPoint p, StringBuilder parsedGeom, int lastGeoStyle, bool withFill)
		{
			foreach (mxVsdxGeometry geo in geomList)
			{

				if (withFill == geo.NoFill)
				{
					continue;
				}

				string str = geo.getPathXML(p, shape);

				if (str.Length > 0)
				{
					int geoStyle = getGeoStyle(geo);

					if (lastGeoStyle == -1) //first one
					{
						parsedGeom.Append("<path>");
						parsedGeom.Append(str);
					}
					else if (lastGeoStyle != geoStyle)
					{
						closePath(parsedGeom, lastGeoStyle);
						parsedGeom.Append("<path>");
						parsedGeom.Append(str);
					}
					else
					{
						//parsedGeom.append("<close/>");
						parsedGeom.Append(str);
					}
					lastGeoStyle = geoStyle;
				}
			}
			return lastGeoStyle;
		}

		private int getGeoStyle(mxVsdxGeometry geo)
		{
			int geoStyle = 0;
			if (!geo.NoLine && !geo.NoFill)
			{
				geoStyle = 1;
			}
			else if (!geo.NoFill)
			{
				geoStyle = 2;
			}
			else if (!geo.NoLine)
			{
				geoStyle = 3;
			}
			return geoStyle;
		}

		private void closePath(StringBuilder parsedGeom, int geoStyle)
		{
			parsedGeom.Append("</path>");
			if (geoStyle == 1)
			{
				parsedGeom.Append("<fillstroke/>");
			}
			else if (geoStyle == 2)
			{
				parsedGeom.Append("<fill/>");
			}
			else if (geoStyle == 3)
			{
				parsedGeom.Append("<stroke/>");
			}
		}

	}

}