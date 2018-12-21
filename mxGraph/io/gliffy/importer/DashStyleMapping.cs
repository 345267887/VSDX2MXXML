namespace mxGraph.io.gliffy.importer
{

	public class DashStyleMapping
	{
		public static string get(string value)
		{
			if (string.ReferenceEquals(value, null))
			{
				return "";
			}

			return "dashed=1;dashPattern=" + value.Replace(",", " ") + ";";
		}
	}

}