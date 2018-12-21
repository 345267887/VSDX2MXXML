/// <summary>
/// $Id: mxICell.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.model
{

	/// <summary>
	/// Defines the requirements for a cell that can be used in an model.
	/// </summary>
	public interface mxICell
	{

		/// <summary>
		/// Returns the Id of the cell as a string.
		/// </summary>
		/// <returns> Returns the Id. </returns>
		string Id {get;set;}


		/// <summary>
		/// Returns the user object of the cell.
		/// </summary>
		/// <returns> Returns the user object. </returns>
		object Value {get;set;}


		/// <summary>
		/// Returns the object that describes the geometry.
		/// </summary>
		/// <returns> Returns the cell geometry. </returns>
		mxGeometry Geometry {get;set;}


		/// <summary>
		/// Returns the string that describes the style.
		/// </summary>
		/// <returns> Returns the cell style. </returns>
		string Style {get;set;}


		/// <summary>
		/// Returns true if the cell is a vertex.
		/// </summary>
		/// <returns> Returns true if the cell is a vertex. </returns>
		bool Vertex {get;}

		/// <summary>
		/// Returns true if the cell is an edge.
		/// </summary>
		/// <returns> Returns true if the cell is an edge. </returns>
		bool Edge {get;}

		/// <summary>
		/// Returns true if the cell is connectable.
		/// </summary>
		/// <returns> Returns the connectable state. </returns>
		bool Connectable {get;}

		/// <summary>
		/// Returns true if the cell is visibile.
		/// </summary>
		/// <returns> Returns the visible state. </returns>
		bool Visible {get;set;}


		/// <summary>
		/// Returns true if the cell is collapsed.
		/// </summary>
		/// <returns> Returns the collapsed state. </returns>
		bool Collapsed {get;set;}


		/// <summary>
		/// Returns the cell's parent.
		/// </summary>
		/// <returns> Returns the parent cell. </returns>
		mxICell Parent {get;set;}


		/// <summary>
		/// Returns the source or target terminal.
		/// </summary>
		/// <param name="source"> Boolean that specifies if the source terminal should be
		/// returned. </param>
		/// <returns> Returns the source or target terminal. </returns>
		mxICell getTerminal(bool source);

		/// <summary>
		/// Sets the source or target terminal and returns the new terminal.
		/// </summary>
		/// <param name="terminal"> Cell that represents the new source or target terminal. </param>
		/// <param name="isSource"> Boolean that specifies if the source or target terminal
		/// should be set. </param>
		/// <returns> Returns the new terminal. </returns>
		mxICell setTerminal(mxICell terminal, bool isSource);

		/// <summary>
		/// Returns the number of child cells.
		/// </summary>
		/// <returns> Returns the number of children. </returns>
		int ChildCount {get;}

		/// <summary>
		/// Returns the index of the specified child in the child array.
		/// </summary>
		/// <param name="child"> Child whose index should be returned. </param>
		/// <returns> Returns the index of the given child. </returns>
		int getIndex(mxICell child);

		/// <summary>
		/// Returns the child at the specified index.
		/// </summary>
		/// <param name="index"> Integer that specifies the child to be returned. </param>
		/// <returns> Returns the child at the given index. </returns>
		mxICell getChildAt(int index);

		/// <summary>
		/// Appends the specified child into the child array and updates the parent
		/// reference of the child. Returns the appended child.
		/// </summary>
		/// <param name="child"> Cell to be appended to the child array. </param>
		/// <returns> Returns the new child. </returns>
		mxICell insert(mxICell child);

		/// <summary>
		/// Inserts the specified child into the child array at the specified index
		/// and updates the parent reference of the child. Returns the inserted child.
		/// </summary>
		/// <param name="child"> Cell to be inserted into the child array. </param>
		/// <param name="index"> Integer that specifies the index at which the child should
		/// be inserted into the child array. </param>
		/// <returns> Returns the new child. </returns>
		mxICell insert(mxICell child, int index);

		/// <summary>
		/// Removes the child at the specified index from the child array and
		/// returns the child that was removed. Will remove the parent reference of
		/// the child.
		/// </summary>
		/// <param name="index"> Integer that specifies the index of the child to be
		/// removed. </param>
		/// <returns> Returns the child that was removed. </returns>
		mxICell remove(int index);

		/// <summary>
		/// Removes the given child from the child array and returns it. Will remove
		/// the parent reference of the child.
		/// </summary>
		/// <param name="child"> Cell that represents the child to be removed. </param>
		/// <returns> Returns the child that was removed. </returns>
		mxICell remove(mxICell child);

		/// <summary>
		/// Removes the cell from its parent.
		/// </summary>
		void removeFromParent();

		/// <summary>
		/// Returns the number of edges in the edge array.
		/// </summary>
		/// <returns> Returns the number of edges. </returns>
		int EdgeCount {get;}

		/// <summary>
		/// Returns the index of the specified edge in the edge array.
		/// </summary>
		/// <param name="edge"> Cell whose index should be returned. </param>
		/// <returns> Returns the index of the given edge. </returns>
		int getEdgeIndex(mxICell edge);

		/// <summary>
		/// Returns the edge at the specified index in the edge array.
		/// </summary>
		/// <param name="index"> Integer that specifies the index of the edge to be
		/// returned. </param>
		/// <returns> Returns the edge at the given index. </returns>
		mxICell getEdgeAt(int index);

		/// <summary>
		/// Inserts the specified edge into the edge array and returns the edge.
		/// Will update the respective terminal reference of the edge.
		/// </summary>
		/// <param name="edge"> Cell to be inserted into the edge array. </param>
		/// <param name="isOutgoing"> Boolean that specifies if the edge is outgoing. </param>
		/// <returns> Returns the new edge. </returns>
		mxICell insertEdge(mxICell edge, bool isOutgoing);

		/// <summary>
		/// Removes the specified edge from the edge array and returns the edge.
		/// Will remove the respective terminal reference from the edge.
		/// </summary>
		/// <param name="edge"> Cell to be removed from the edge array. </param>
		/// <param name="isOutgoing"> Boolean that specifies if the edge is outgoing. </param>
		/// <returns> Returns the edge that was removed. </returns>
		mxICell removeEdge(mxICell edge, bool isOutgoing);

		/// <summary>
		/// Removes the edge from its source or target terminal.
		/// </summary>
		/// <param name="isSource"> Boolean that specifies if the edge should be removed
		/// from its source or target terminal. </param>
		void removeFromTerminal(bool isSource);

		/// <summary>
		/// Returns a clone of this cell.
		/// </summary>
		/// <returns> Returns a clone of this cell. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: Object clone() throws CloneNotSupportedException;
		object clone();

	}

}