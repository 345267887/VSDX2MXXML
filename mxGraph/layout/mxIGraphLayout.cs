/// <summary>
/// $Id: mxIGraphLayout.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.layout
{

	/// <summary>
	/// Defines the requirements for an object that implements a graph layout.
	/// </summary>
	public interface mxIGraphLayout
	{

		/// <summary>
		/// Executes the layout for the children of the specified parent.
		/// </summary>
		/// <param name="parent"> Parent cell that contains the children to be layed out. </param>
		void execute(object parent);

		/// <summary>
		/// Notified when a cell is being moved in a parent that has automatic
		/// layout to update the cell state (eg. index) so that the outcome of the
		/// layout will position the vertex as close to the point (x, y) as
		/// possible.
		/// </summary>
		/// <param name="cell"> Cell which is being moved. </param>
		/// <param name="x"> X-coordinate of the new cell location. </param>
		/// <param name="y"> Y-coordinate of the new cell location. </param>
		void moveCell(object cell, double x, double y);

	}

}