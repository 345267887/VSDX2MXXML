using System;
using System.Collections.Generic;

namespace mxGraph.io.gliffy.model
{


	//using SerializedName = com.google.gson.annotations.SerializedName;

	public class Graphic
	{

		public sealed class Type
		{
			public static readonly Type SVG = new Type("SVG", InnerEnum.SVG);
			public static readonly Type LINE = new Type("LINE", InnerEnum.LINE);
			public static readonly Type SHAPE = new Type("SHAPE", InnerEnum.SHAPE);
			public static readonly Type TEXT = new Type("TEXT", InnerEnum.TEXT);
			public static readonly Type IMAGE = new Type("IMAGE", InnerEnum.IMAGE);
			public static readonly Type LINK = new Type("LINK", InnerEnum.LINK);
			public static readonly Type MINDMAP = new Type("MINDMAP", InnerEnum.MINDMAP);
			public static readonly Type POPUPNOTE = new Type("POPUPNOTE", InnerEnum.POPUPNOTE);
			public static readonly Type UNKNOWN = new Type("UNKNOWN", InnerEnum.UNKNOWN);

			private static readonly IList<Type> valueList = new List<Type>();

			static Type()
			{
				valueList.Add(SVG);
				valueList.Add(LINE);
				valueList.Add(SHAPE);
				valueList.Add(TEXT);
				valueList.Add(IMAGE);
				valueList.Add(LINK);
				valueList.Add(MINDMAP);
				valueList.Add(POPUPNOTE);
				valueList.Add(UNKNOWN);
			}

			public enum InnerEnum
			{
				SVG,
				LINE,
				SHAPE,
				TEXT,
				IMAGE,
				LINK,
				MINDMAP,
				POPUPNOTE,
				UNKNOWN
			}

			private readonly string nameValue;
			private readonly int ordinalValue;
			private readonly InnerEnum innerEnumValue;
			private static int nextOrdinal = 0;

			private Type(string name, InnerEnum innerEnum)
			{
				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}


			public override string ToString()
			{
                return this.nameValue;
			}

			public static IList<Type> values()
			{
				return valueList;
			}

			public InnerEnum InnerEnumValue()
			{
				return innerEnumValue;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public static Type valueOf(string name)
			{
				foreach (Type enumInstance in Type.values())
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		public abstract class GliffyAbstractShape
		{
			public int strokeWidth;

			public string strokeColor;

			public string fillColor;

			public string dashStyle;
		}

		public class GliffyLine : GliffyAbstractShape
		{
			public int? startArrow;

			public int? endArrow;

			public string interpolationType;

			public IList<float[]> controlPath = new List<float[]>();
		}

		public class GliffyShape : GliffyAbstractShape
		{
			public string tid;

			public bool gradient;

			public bool dropShadow;

			public int state;

			public int shadowX;

			public int shadowY;

			public float opacity;

		}

		public class GliffyImage : GliffyShape
		{
			internal string url;

			public virtual string Url
			{
				get
				{
					return url.Replace(";base64", "");
				}
			}
		}

		public class GliffySvg : GliffyShape
		{
			public int? embeddedResourceId;
		}

		public class GliffyMindmap : GliffyShape
		{
		}

		public class GliffyPopupNote : GliffyShape
		{
			public string text;
		}

		public Type type;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		//public GliffyText Text_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		public GliffyLine Line_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		public GliffyShape Shape_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		public GliffyImage Image_Renamed;

		public GliffySvg Svg;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		public GliffyMindmap Mindmap_Renamed;

		public GliffyPopupNote gliffyPopupNote;

		public Graphic() : base()
		{
		}

		public virtual Type getType()
		{
			return type != null ? type : Type.UNKNOWN;
		}

		//public virtual GliffyText Text
		//{
		//	get
		//	{
		//		return Text_Renamed;
		//	}
		//}

		public virtual GliffyLine Line
		{
			get
			{
				return Line_Renamed;
			}
		}

		public virtual GliffyShape Shape
		{
			get
			{
				return Shape_Renamed;
			}
		}

		public virtual GliffyImage Image
		{
			get
			{
				return Image_Renamed;
			}
		}

		public virtual GliffyMindmap Mindmap
		{
			get
			{
				return Mindmap_Renamed;
			}
		}


	}

}