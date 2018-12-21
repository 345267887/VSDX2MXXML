namespace mxGraph.io.vsdx.geometry
{

	using mxPoint = mxGraph.util.mxPoint;

	public abstract class Row
	{
		protected internal double? x, y, a, b, c, d;
		protected internal string formulaA, formulaE;
		protected internal int index;

		public mxPathDebug debug = null;

		public Row(int index, double? x, double? y)
		{
			this.index = index;
			this.x = x;
			this.y = y;
		}

		//TODO probably point p is not needed as the point from previous step is stored in lastP?
		public abstract string handle(mxPoint p, Shape shape);

		public virtual double? X
		{
			get
			{
				return x;
			}
		}

		public virtual double? Y
		{
			get
			{
				return y;
			}
		}

		public virtual double? A
		{
			get
			{
				return a;
			}
		}

		public virtual double? B
		{
			get
			{
				return b;
			}
		}

		public virtual double? C
		{
			get
			{
				return c;
			}
		}

		public virtual double? D
		{
			get
			{
				return d;
			}
		}

		public virtual string FormulaA
		{
			get
			{
				return formulaA;
			}
		}

		public virtual string FormulaE
		{
			get
			{
				return formulaE;
			}
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
		}
	}

}