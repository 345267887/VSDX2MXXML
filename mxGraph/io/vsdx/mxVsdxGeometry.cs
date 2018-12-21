using System.Collections.Generic;
using System.Text;

namespace mxGraph.io.vsdx
{


    using Element = System.Xml.XmlElement;

	using Row = mxGraph.io.vsdx.geometry.Row;
	using RowFactory = mxGraph.io.vsdx.geometry.RowFactory;
	using mxPoint = mxGraph.util.mxPoint;

	public class mxVsdxGeometry
	{
		private int index;

		private bool noFill = false, noLine = false, noShow = false, noSnap = false, noQuickDrag = false;

		private List<Row> rows = null;

		public mxVsdxGeometry(Element elem)
		{
            index = int.Parse(elem.GetAttribute("IX"));
			processGeoElem(elem);
		}

		public mxVsdxGeometry(Element elem, IList<mxVsdxGeometry> parentGeo)
		{
			index = int.Parse(elem.GetAttribute("IX"));
			if (parentGeo != null && index < parentGeo.Count)
			{
				//inherit all parent values including 
				this.inheritGeo(parentGeo[index]);
			}
			processGeoElem(elem);
		}

		private void processGeoElem(Element elem)
		{
			List<Element> cellElems = mxVsdxUtils.getDirectChildNamedElements(elem, "Cell");
			List<Element> rowElems = mxVsdxUtils.getDirectChildNamedElements(elem, "Row");

			if (rows == null)
			{
				rows = new List<Row>(rowElems.Count);

				//set the list size to row size
				for (int i = 0; i < rowElems.Count; i++)
				{
					rows.Add(null);
				}
			}

			foreach (Element cellElem in cellElems)
			{
				string name = cellElem.GetAttribute("N");
				string val = cellElem.GetAttribute("V");
				switch (name)
				{
					case "NoFill":
						noFill = "1".Equals(val);
					break;
					case "NoLine":
						noLine = "1".Equals(val);
					break;
					case "NoShow":
						noShow = "1".Equals(val);
					break;
					case "NoSnap":
						noSnap = "1".Equals(val);
					break;
					case "NoQuickDrag":
						noQuickDrag = "1".Equals(val);
					break;
				}
			}

			int rowsLen = rows.Count;
			bool sortNeeded = false;

			foreach (Element rowElem in rowElems)
			{
				Row row = RowFactory.getRowObj(rowElem, rows);

				//this can happen when child geo has more rows than parent
				if (row.Index > rowsLen)
				{
					rows.Add(row);
					sortNeeded = true;
				}
				else
				{
					rows[row.Index - 1] = row;
				}
			}

			if (sortNeeded)
			{
				rows.Sort(new ComparatorAnonymousInnerClass(this));
			}
		}

		private class ComparatorAnonymousInnerClass : IComparer<Row>
		{
			private readonly mxVsdxGeometry outerInstance;

			public ComparatorAnonymousInnerClass(mxVsdxGeometry outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(Row r1, Row r2)
			{
				return r1.Index - r2.Index;
			}
		}

		private void inheritGeo(mxVsdxGeometry parent)
		{
			this.noFill = parent.noFill;
			this.noLine = parent.noLine;
			this.noShow = parent.noShow;
			this.noSnap = parent.noSnap;
			this.noQuickDrag = parent.noQuickDrag;
			rows = new List<Row>();
			this.rows.AddRange(parent.rows);
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}

		public virtual bool NoFill
		{
			get
			{
				return noFill;
			}
		}

		public virtual bool NoLine
		{
			get
			{
				return noLine;
			}
		}

		public virtual bool NoShow
		{
			get
			{
				return noShow;
			}
		}

		public virtual bool NoSnap
		{
			get
			{
				return noSnap;
			}
		}

		public virtual bool NoQuickDrag
		{
			get
			{
				return noQuickDrag;
			}
		}

		public virtual List<Row> Rows
		{
			get
			{
				return rows;
			}
		}

		public virtual string getPathXML(mxPoint p, Shape shape)
		{
			if (noShow)
			{
				return "";
			}

			StringBuilder geomElemParsed = new StringBuilder();

			foreach (Row row in rows)
			{
				geomElemParsed.Append(row.handle(p, shape));
			}

			return geomElemParsed.ToString();
		}
	}

}