namespace mxGraph.io.gliffy.model
{

	public class Metadata
	{

		private string title;

		public Metadata() : base()
		{
		}

		public virtual string Title
		{
			get
			{
				return title;
			}
			set
			{
				this.title = value;
			}
		}


	}

}