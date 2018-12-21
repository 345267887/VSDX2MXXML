using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class Section
    {
        /**
	 * The section element
	 */
        protected XmlElement elem = null;

        /**
         * Constructs a new Section
         * @param elem the Element to wrap
         */
        public Section(XmlElement elem)
        {
            this.elem = elem;
        }

        /**
         * Return the specified cell by key by row index, if it exists
         * @param index the row index to search
         * @param cellKey the name of the Cell to search for
         * @return the Element of the specified Cell, if null if it doesn't exist
         */
        public XmlElement getIndexedCell(String index, String cellKey)
        {
            List<XmlElement> rows = mxVsdxUtils.getDirectChildNamedElements(this.elem, "Row");

            for (int i = 0; i < rows.Count; i++)
            {
                XmlElement row = rows[i];
                String n = row.GetAttribute("IX");

                // If index is null always match. For example, you can have a shape text with no paragraph index.
                // When it checks the master shape the first paragraph should be used (or maybe the lowest index?)
                if (n.Equals(index) || index == null)
                {
                    List<XmlElement> cells = mxVsdxUtils.getDirectChildNamedElements(row, "Cell");

                    for (int j = 0; j < cells.Count; j++)
                    {
                        XmlElement cell = cells[j];
                        n = cell.GetAttribute("N");

                        if (n.Equals(cellKey))
                        {
                            return cell;
                        }
                    }
                }
            }

            return null;
        }
    }
}
