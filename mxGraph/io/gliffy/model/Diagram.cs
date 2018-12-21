namespace mxGraph.io.gliffy.model
{

	public class Diagram
	{

		private string version;

		public Stage stage;

		public Metadata metadata;

		public EmbeddedResources embeddedResources;

		public Diagram() : base()
		{
		}

		public virtual string Version
		{
			get
			{
				return version;
			}
			set
			{
				this.version = value;
			}
		}


	}

}