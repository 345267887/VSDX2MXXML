using System.Text;

namespace com.mxgraph.io.gliffy.model
{


	//using PostDeserializer = com.mxgraph.io.gliffy.importer.PostDeserializer;

	public class GliffyText //: PostDeserializer.PostDeserializable
	{
		//places the text in the middle of the line
		public static double? DEFAULT_LINE_T_VALUE = 0.5;

		private string html;

		private string valign;

		//extracted from html
		private string halign;

		private string vposition;

		private string hposition;

		private int? paddingLeft;

		private int? paddingRight;

		private int? paddingBottom;

		private int? paddingTop;

		public double? lineTValue = DEFAULT_LINE_T_VALUE;

		public int? linePerpValue;

		private static Pattern pattern = Pattern.compile("<p(.*?)<\\/p>");

		private static Pattern textAlign = Pattern.compile(".*(text-align: ?(left|center|right);).*", Pattern.DOTALL);

		public GliffyText()
		{
		}

		public virtual void postDeserialize()
		{
			halign = HorizontalTextAlignment;
			html = replaceParagraphWithDiv(html);
		}

		public virtual string Html
		{
			get
			{
				return html;
			}
			set
			{
			}
		}


		public virtual string Style
		{
			get
			{
				StringBuilder sb = new StringBuilder();
    
				//vertical label position
				if (vposition.Equals("above"))
				{
					sb.Append("verticalLabelPosition=top;").Append("verticalAlign=bottom;");
				}
				else if (vposition.Equals("below"))
				{
					sb.Append("verticalLabelPosition=bottom;").Append("verticalAlign=top;");
				}
				else if (vposition.Equals("none"))
				{
					sb.Append("verticalAlign=").Append(valign).Append(";");
				}
    
				if (hposition.Equals("left"))
				{
					sb.Append("labelPosition=left;").Append("align=right;");
				}
				else if (hposition.Equals("right"))
				{
					sb.Append("labelPosition=right;").Append("align=left;");
				}
				else if (hposition.Equals("none"))
				{
					if (!string.ReferenceEquals(halign, null))
					{
						sb.Append("align=").Append(halign).Append(";");
					}
					else
					{
						sb.Append("align=center;");
					}
				}
    
				sb.Append("spacingLeft=").Append(paddingLeft).Append(";");
				sb.Append("spacingRight=").Append(paddingRight).Append(";");
				sb.Append("spacingTop=").Append(paddingTop).Append(";");
				sb.Append("spacingBottom=").Append(paddingBottom).Append(";");
    
				return sb.ToString();
			}
		}

		private string replaceParagraphWithDiv(string html)
		{
			Matcher m = pattern.matcher(html);
			StringBuilder sb = new StringBuilder();
			while (m.find())
			{
				sb.Append("<div" + m.group(1) + "</div>");
			}

			return sb.Length > 0 ? sb.ToString() : html;
		}

		/// <summary>
		/// Extracts horizontal text alignment from html and removes it
		/// so it does not interfere with alignment set in mxCell style </summary>
		/// <returns> horizontal text alignment or null if there is none </returns>
		private string HorizontalTextAlignment
		{
			get
			{
				Matcher m = textAlign.matcher(html);
    
				if (m.matches())
				{
					html = html.replaceAll("text-align: ?\\w*;", "");
					return m.group(2);
				}
    
				return null;
			}
		}

	}

}