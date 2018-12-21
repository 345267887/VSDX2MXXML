/// <summary>
/// $Id: mxICanvas.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.canvas
{
    using System.Drawing;
    using mxCellState = mxGraph.view.mxCellState;

	/// <summary>
	/// Defines the requirements for a canvas that paints the vertices and edges of
	/// a graph.
	/// </summary>
	public interface mxICanvas
	{
		/// <summary>
		/// Sets the translation for the following drawing requests.
		/// </summary>
		void setTranslate(int x, int y);

		/// <summary>
		/// Returns the current translation.
		/// </summary>
		/// <returns> Returns the current translation. </returns>
		Point Translate {get;}

		/// <summary>
		/// Sets the scale for the following drawing requests.
		/// </summary>
		double Scale {set;get;}


		/// <summary>
		/// Draws the given cell.
		/// </summary>
		/// <param name="state"> State of the cell to be painted. </param>
		/// <returns> Object that represents the cell. </returns>
		object drawCell(mxCellState state);

		/// <summary>
		/// Draws the given label.
		/// </summary>
		/// <param name="text"> String that represents the label. </param>
		/// <param name="state"> State of the cell whose label is to be painted. </param>
		/// <param name="html"> Specifies if the label contains HTML markup. </param>
		/// <returns> Object that represents the label. </returns>
		object drawLabel(string text, mxCellState state, bool html);

	}

}