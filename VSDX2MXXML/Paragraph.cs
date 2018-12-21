using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class Paragraph
    {
        protected List<String> values;

        protected List<String> charIndices;

        protected List<String> fields;

        protected String paraIndex;

        public Paragraph(String val, String ch, String pg, String field)
        {
            this.values = new List<String>();
            this.values.Add(val);
            this.charIndices = new List<String>();
            this.charIndices.Add(ch);
            this.fields = new List<String>();
            this.fields.Add(field);
            this.paraIndex = pg;
        }

        public void addText(String val, String ch, String field)
        {
            this.values.Add(val);
            this.charIndices.Add(ch);
            this.fields.Add(field);
        }

        public String getParagraphIndex()
        {
            return this.paraIndex;
        }

        public String getValue(int index)
        {
            return values[index];
        }

        public int numValues()
        {
            return this.values.Count;
        }

        public String getChar(int index)
        {
            return charIndices[index];
        }

        public String getField(int index)
        {
            return fields[index];
        }
    }
}
