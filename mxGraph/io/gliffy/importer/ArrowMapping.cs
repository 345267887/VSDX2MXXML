using System.Collections.Generic;

namespace mxGraph.io.gliffy.importer
{


	using mxConstants = mxGraph.util.mxConstants;

	public class ArrowMapping
	{

		static ArrowMapping()
		{
			init();
		}

		public class ArrowStyle
		{

			public string name;

			public bool? fill;

			public ArrowStyle(string name, bool fill) : base()
			{
				this.name = name;
				this.fill = fill;
			}

			public virtual string ToString(bool start)
			{
				int intFill = fill.HasValue&&fill.Value ? 1 : 0;
				return start ? "startArrow=" + name + ";startFill=" + intFill : "endArrow=" + name + ";endFill=" + intFill;
			}

		}

		private static IDictionary<int?, ArrowStyle> mapping;

		private static void init()
		{
			mapping = new Dictionary<int?, ArrowStyle>();
			mapping[0] = new ArrowStyle("none", false);
			mapping[1] = new ArrowStyle(mxConstants.ARROW_OPEN, false);
			mapping[2] = new ArrowStyle(mxConstants.ARROW_BLOCK, true);
			mapping[3] = new ArrowStyle(mxConstants.ARROW_BLOCK, false);
			mapping[4] = new ArrowStyle(mxConstants.ARROW_BLOCK, false);
			mapping[5] = new ArrowStyle(mxConstants.ARROW_DIAMOND, false);
			mapping[6] = new ArrowStyle(mxConstants.ARROW_CLASSIC, false);
			mapping[7] = new ArrowStyle(mxConstants.ARROW_DIAMOND, true);
			mapping[8] = new ArrowStyle(mxConstants.ARROW_CLASSIC, true);
			mapping[9] = new ArrowStyle("ERzeroToMany", true);
			mapping[10] = new ArrowStyle("ERoneToMany", true);
			mapping[11] = new ArrowStyle("ERmandOne", true);
			mapping[12] = new ArrowStyle("ERone", true);
			mapping[13] = new ArrowStyle("ERzeroToOne", true);
			mapping[14] = new ArrowStyle("ERmany", true);
			mapping[15] = new ArrowStyle(mxConstants.ARROW_OVAL, false);
			mapping[16] = new ArrowStyle(mxConstants.ARROW_OVAL, true);
			mapping[17] = new ArrowStyle(mxConstants.ARROW_BLOCK, true);
			mapping[18] = new ArrowStyle(mxConstants.ARROW_CLASSIC, true);
			mapping[19] = new ArrowStyle(mxConstants.ARROW_CLASSIC, true);
		}

		public static ArrowStyle get(int? gliffyId)
		{
			return mapping[gliffyId];
		}
	}

}