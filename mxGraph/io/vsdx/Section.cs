using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{

    using Element = System.Xml.XmlElement;

	/// <summary>
	/// Wrapper for a Section element https://msdn.microsoft.com/en-us/library/office/jj684189.aspx
	/// 
	/// </summary>
	public class Section
	{
		/// <summary>
		/// The section element
		/// </summary>
		protected internal Element elem = null;

		/// <summary>
		/// Constructs a new Section </summary>
		/// <param name="elem"> the Element to wrap </param>
		public Section(Element elem)
		{
			this.elem = elem;
		}

		/// <summary>
		/// Return the specified cell by key by row index, if it exists </summary>
		/// <param name="index"> the row index to search </param>
		/// <param name="cellKey"> the name of the Cell to search for </param>
		/// <returns> the Element of the specified Cell, if null if it doesn't exist </returns>
		public virtual Element getIndexedCell(string index, string cellKey)
		{
			List<Element> rows = mxVsdxUtils.getDirectChildNamedElements(this.elem, "Row");

			for (int i = 0; i < rows.Count; i++)
			{
				Element row = rows[i];
                string n = row.GetAttribute("IX");

				// If index is null always match. For example, you can have a shape text with no paragraph index.
				// When it checks the master shape the first paragraph should be used (or maybe the lowest index?)
				if (n.Equals(index) || string.ReferenceEquals(index, null))
				{
					List<Element> cells = mxVsdxUtils.getDirectChildNamedElements(row, "Cell");

					for (int j = 0; j < cells.Count; j++)
					{
						Element cell = cells[j];
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