using System.Collections.Generic;

namespace mxGraph.io.gliffy.importer
{


	public class LineMapping
	{

		private static IDictionary<string, string> mapping = new Dictionary<string, string>();

		static LineMapping()
		{
			init();
		}

		private static void init()
		{
			mapping["linear"] = "";
			mapping["orthogonal"] = "edgeStyle=orthogonal;";
			mapping["quadratic"] = "curved=1;edgeStyle=orthogonalEdgeStyle;";

		}

		public static string get(string style)
		{
			return mapping[style];
		}
	}

}