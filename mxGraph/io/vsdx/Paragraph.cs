using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{

	/// <summary>
	/// Represents a single formatted section of text
	/// 
	/// </summary>
	public class Paragraph
	{
		protected internal List<string> values;

		protected internal List<string> charIndices;

		protected internal List<string> fields;

		protected internal string paraIndex;

		public Paragraph(string val, string ch, string pg, string field)
		{
			this.values = new List<string>();
			this.values.Add(val);
			this.charIndices = new List<string>();
			this.charIndices.Add(ch);
			this.fields = new List<string>();
			this.fields.Add(field);
			this.paraIndex = pg;
		}

		public virtual void addText(string val, string ch, string field)
		{
			this.values.Add(val);
			this.charIndices.Add(ch);
			this.fields.Add(field);
		}

		public virtual string ParagraphIndex
		{
			get
			{
				return this.paraIndex;
			}
		}

		public virtual string getValue(int index)
		{
			return values[index];
		}

		public virtual int numValues()
		{
			return this.values.Count;
		}

		public virtual string getChar(int index)
		{
			return charIndices[index];
		}

		public virtual string getField(int index)
		{
			return fields[index];
		}
	}

}