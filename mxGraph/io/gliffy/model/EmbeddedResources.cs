using mxGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace mxGraph.io.gliffy.model
{


	

	public class EmbeddedResources
	{

		public class Resource
		{
			public int? id;

			public string mimeType;

			public string data;

			public Resource()
			{
			}

			public virtual string Base64EncodedData
			{
				get
				{
					try
					{
						return Base64.encodeBase64String(data.GetBytes(Encoding.UTF8));
					}
					catch (Exception e)
					{
						throw e;
					}
				}
			}
		}

		public IList<Resource> resources;

		public IDictionary<int?, Resource> resourceMap;

		public virtual IList<Resource> Resources
		{
			set
			{
				this.resources = value;
			}
		}

		public virtual Resource get(int? id)
		{
			if (resourceMap == null)
			{
				resourceMap = new Dictionary<int?, Resource>();
				foreach (Resource r in resources)
				{
					resourceMap[r.id] = r;
				}
			}

			return resourceMap[id];
		}

	}

}