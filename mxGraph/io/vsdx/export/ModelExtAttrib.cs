namespace mxGraph.io.vsdx.export
{

	public class ModelExtAttrib
	{
		private double pageScale = 1, pageWidth = 839, pageHeight = 1188, gridSize = 10; //A4 size in pixels as a default
		private bool pageVisible = true, gridEnabled = true, guidesEnabled = true, foldingEnabled = true, shadowVisible = false, tooltips = true, connect = true, arrows = true, mathEnabled = true;
		private string backgroundClr = "#FFFFFF";
		//TODO add backgroundImage support

		public virtual double PageScale
		{
			get
			{
				return pageScale;
			}
			set
			{
				this.pageScale = value;
			}
		}
		public virtual double PageWidth
		{
			get
			{
				return pageWidth;
			}
			set
			{
				this.pageWidth = value;
			}
		}
		public virtual double PageHeight
		{
			get
			{
				return pageHeight;
			}
			set
			{
				this.pageHeight = value;
			}
		}
		public virtual double GridSize
		{
			get
			{
				return gridSize;
			}
			set
			{
				this.gridSize = value;
			}
		}
		public virtual bool PageVisible
		{
			get
			{
				return pageVisible;
			}
			set
			{
				this.pageVisible = value;
			}
		}
		public virtual bool GridEnabled
		{
			get
			{
				return gridEnabled;
			}
			set
			{
				this.gridEnabled = value;
			}
		}
		public virtual bool GuidesEnabled
		{
			get
			{
				return guidesEnabled;
			}
			set
			{
				this.guidesEnabled = value;
			}
		}
		public virtual bool FoldingEnabled
		{
			get
			{
				return foldingEnabled;
			}
			set
			{
				this.foldingEnabled = value;
			}
		}
		public virtual bool ShadowVisible
		{
			get
			{
				return shadowVisible;
			}
			set
			{
				this.shadowVisible = value;
			}
		}
		public virtual bool Tooltips
		{
			get
			{
				return tooltips;
			}
			set
			{
				this.tooltips = value;
			}
		}
		public virtual bool Connect
		{
			get
			{
				return connect;
			}
			set
			{
				this.connect = value;
			}
		}
		public virtual bool Arrows
		{
			get
			{
				return arrows;
			}
			set
			{
				this.arrows = value;
			}
		}
		public virtual bool MathEnabled
		{
			get
			{
				return mathEnabled;
			}
			set
			{
				this.mathEnabled = value;
			}
		}
		public virtual string BackgroundClr
		{
			get
			{
				return backgroundClr;
			}
			set
			{
				this.backgroundClr = value;
			}
		}
	}

}