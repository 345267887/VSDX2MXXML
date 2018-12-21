using System.Collections.Generic;

namespace com.mxgraph.io.gliffy.importer
{


	public class StencilTranslator
	{
		//private static Logger logger = Logger.getLogger("StencilTranslator");

		private static IDictionary<string, string> translationTable = new Dictionary<string, string>();

		static StencilTranslator()
		{
			init();
		};

		private static void init()
		{
			ResourceBundle rb = PropertyResourceBundle.getBundle("com/mxgraph/io/gliffy/importer/gliffyTranslation");
			foreach (string key in rb.Keys)
			{
				translationTable[key] = rb.getString(key);
			}
		}

		public static string translate(string gliffyShapeKey)
		{
			string shape = translationTable[gliffyShapeKey];
			logger.info(gliffyShapeKey + " -> " + shape);
			return shape;
		}
	}

}