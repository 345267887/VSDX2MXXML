using System.Collections.Generic;

namespace mxGraph.io.gliffy.model
{

	public class Stage
	{

		private string background;

		private float width;

		private float height;

		private bool autofit;

		private bool gridOn;

		private bool drawingGuidesOn;

		private IList<GliffyObject> objects;

		public Stage()
		{
		}

		public virtual string BackgroundColor
		{
			get
			{
				return background;
			}
		}

		public virtual string Background
		{
			set
			{
				this.background = value;
			}
		}

		public virtual float Width
		{
			get
			{
				return width;
			}
			set
			{
				this.width = value;
			}
		}


		public virtual float Height
		{
			get
			{
				return height;
			}
			set
			{
				this.height = value;
			}
		}


		public virtual bool Autofit
		{
			get
			{
				return autofit;
			}
			set
			{
				this.autofit = value;
			}
		}


		public virtual bool GridOn
		{
			get
			{
				return gridOn;
			}
			set
			{
				this.gridOn = value;
			}
		}


		public virtual bool DrawingGuidesOn
		{
			get
			{
				return drawingGuidesOn;
			}
			set
			{
				this.drawingGuidesOn = value;
			}
		}


		public virtual IList<GliffyObject> Objects
		{
			get
			{
				return objects;
			}
			set
			{
				this.objects = value;
			}
		}

	}
}