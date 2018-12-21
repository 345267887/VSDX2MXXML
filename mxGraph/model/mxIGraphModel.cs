/// <summary>
/// $Id: mxIGraphModel.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.model
{

	using mxIEventListener = mxGraph.util.mxEventSource.mxIEventListener;
	using mxUndoableChange = mxGraph.util.mxUndoableEdit.mxUndoableChange;

	/// <summary>
	/// Defines the requirements for a graph model to be used with mxGraph.
	/// </summary>
	public interface mxIGraphModel
	{

		/// <summary>
		/// Defines the interface for an atomic change of the graph model.
		/// </summary>

		/// <summary>
		/// Returns the root of the model or the topmost parent of the given cell.
		/// </summary>
		/// <returns> Returns the root cell. </returns>
		object Root {get;}

		/// <summary>
		/// Sets the root of the model and resets all structures.
		/// </summary>
		/// <param name="root"> Cell that specifies the new root. </param>
		object setRoot(object root);

		/// <summary>
		/// Returns an array of clones for the given array of cells.
		/// Depending on the value of includeChildren, a deep clone is created for
		/// each cell. Connections are restored based if the corresponding
		/// cell is contained in the passed in array.
		/// </summary>
		/// <param name="cells"> Array of cells to be cloned. </param>
		/// <param name="includeChildren"> Boolean indicating if the cells should be cloned
		/// with all descendants. </param>
		/// <returns> Returns a cloned array of cells. </returns>
		object[] cloneCells(object[] cells, bool includeChildren);

		/// <summary>
		/// Returns true if the given parent is an ancestor of the given child.
		/// </summary>
		/// <param name="parent"> Cell that specifies the parent. </param>
		/// <param name="child"> Cell that specifies the child. </param>
		/// <returns> Returns true if child is an ancestor of parent. </returns>
		bool isAncestor(object parent, object child);

		/// <summary>
		/// Returns true if the model contains the given cell.
		/// </summary>
		/// <param name="cell"> Cell to be checked. </param>
		/// <returns> Returns true if the cell is in the model. </returns>
		bool contains(object cell);

		/// <summary>
		/// Returns the parent of the given cell.
		/// </summary>
		/// <param name="child"> Cell whose parent should be returned. </param>
		/// <returns> Returns the parent of the given cell. </returns>
		object getParent(object child);

		/// <summary>
		/// Adds the specified child to the parent at the given index. If no index
		/// is specified then the child is appended to the parent's array of
		/// children.
		/// </summary>
		/// <param name="parent"> Cell that specifies the parent to contain the child. </param>
		/// <param name="child"> Cell that specifies the child to be inserted. </param>
		/// <param name="index"> Integer that specifies the index of the child. </param>
		/// <returns> Returns the inserted child. </returns>
		object add(object parent, object child, int index);

		/// <summary>
		/// Removes the specified cell from the model. This operation will remove
		/// the cell and all of its children from the model.
		/// </summary>
		/// <param name="cell"> Cell that should be removed. </param>
		/// <returns> Returns the removed cell. </returns>
		object remove(object cell);

		/// <summary>
		/// Returns the number of children in the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose number of children should be returned. </param>
		/// <returns> Returns the number of children in the given cell. </returns>
		int getChildCount(object cell);

		/// <summary>
		/// Returns the child of the given parent at the given index.
		/// </summary>
		/// <param name="parent"> Cell that represents the parent. </param>
		/// <param name="index"> Integer that specifies the index of the child to be
		/// returned. </param>
		/// <returns> Returns the child at index in parent. </returns>
		object getChildAt(object parent, int index);

		/// <summary>
		/// Returns the source or target terminal of the given edge depending on the
		/// value of the boolean parameter.
		/// </summary>
		/// <param name="edge"> Cell that specifies the edge. </param>
		/// <param name="isSource"> Boolean indicating which end of the edge should be
		/// returned. </param>
		/// <returns> Returns the source or target of the given edge. </returns>
		object getTerminal(object edge, bool isSource);

		/// <summary>
		/// Sets the source or target terminal of the given edge using.
		/// </summary>
		/// <param name="edge"> Cell that specifies the edge. </param>
		/// <param name="terminal"> Cell that specifies the new terminal. </param>
		/// <param name="isSource"> Boolean indicating if the terminal is the new source or
		/// target terminal of the edge. </param>
		/// <returns> Returns the new terminal. </returns>
		object setTerminal(object edge, object terminal, bool isSource);

		/// <summary>
		/// Returns the number of distinct edges connected to the given cell.
		/// </summary>
		/// <param name="cell"> Cell that represents the vertex. </param>
		/// <returns> Returns the number of edges connected to cell. </returns>
		int getEdgeCount(object cell);

		/// <summary>
		/// Returns the edge of cell at the given index.
		/// </summary>
		/// <param name="cell"> Cell that specifies the vertex. </param>
		/// <param name="index"> Integer that specifies the index of the edge to return. </param>
		/// <returns> Returns the edge at the given index. </returns>
		object getEdgeAt(object cell, int index);

		/// <summary>
		/// Returns true if the given cell is a vertex.
		/// </summary>
		/// <param name="cell"> Cell that represents the possible vertex. </param>
		/// <returns> Returns true if the given cell is a vertex. </returns>
		bool isVertex(object cell);

		/// <summary>
		/// Returns true if the given cell is an edge.
		/// </summary>
		/// <param name="cell"> Cell that represents the possible edge. </param>
		/// <returns> Returns true if the given cell is an edge. </returns>
		bool isEdge(object cell);

		/// <summary>
		/// Returns true if the given cell is connectable.
		/// </summary>
		/// <param name="cell"> Cell whose connectable state should be returned. </param>
		/// <returns> Returns the connectable state of the given cell. </returns>
		bool isConnectable(object cell);

		/// <summary>
		/// Returns the user object of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose user object should be returned. </param>
		/// <returns> Returns the user object of the given cell. </returns>
		object getValue(object cell);

		/// <summary>
		/// Sets the user object of then given cell.
		/// </summary>
		/// <param name="cell"> Cell whose user object should be changed. </param>
		/// <param name="value"> Object that defines the new user object. </param>
		/// <returns> Returns the new value. </returns>
		object setValue(object cell, object value);

		/// <summary>
		/// Returns the geometry of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose geometry should be returned. </param>
		/// <returns> Returns the geometry of the given cell. </returns>
		mxGeometry getGeometry(object cell);

		/// <summary>
		/// Sets the geometry of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose geometry should be changed. </param>
		/// <param name="geometry"> Object that defines the new geometry. </param>
		/// <returns> Returns the new geometry. </returns>
		mxGeometry setGeometry(object cell, mxGeometry geometry);

		/// <summary>
		/// Returns the style of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose style should be returned. </param>
		/// <returns> Returns the style of the given cell. </returns>
		string getStyle(object cell);

		/// <summary>
		/// Sets the style of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose style should be changed. </param>
		/// <param name="style"> String of the form stylename[;key=value] to specify
		/// the new cell style. </param>
		/// <returns> Returns the new style. </returns>
		string setStyle(object cell, string style);

		/// <summary>
		/// Returns true if the given cell is collapsed.
		/// </summary>
		/// <param name="cell"> Cell whose collapsed state should be returned. </param>
		/// <returns> Returns the collapsed state of the given cell. </returns>
		bool isCollapsed(object cell);

		/// <summary>
		/// Sets the collapsed state of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose collapsed state should be changed. </param>
		/// <param name="collapsed"> Boolean that specifies the new collpased state. </param>
		/// <returns> Returns the new collapsed state. </returns>
		bool setCollapsed(object cell, bool collapsed);

		/// <summary>
		/// Returns true if the given cell is visible.
		/// </summary>
		/// <param name="cell"> Cell whose visible state should be returned. </param>
		/// <returns> Returns the visible state of the given cell. </returns>
		bool isVisible(object cell);

		/// <summary>
		/// Sets the visible state of the given cell.
		/// </summary>
		/// <param name="cell"> Cell whose visible state should be changed. </param>
		/// <param name="visible"> Boolean that specifies the new visible state. </param>
		/// <returns> Returns the new visible state. </returns>
		bool setVisible(object cell, bool visible);

		/// <summary>
		/// Increments the updateLevel by one. The event notification is queued
		/// until updateLevel reaches 0 by use of endUpdate.
		/// </summary>
		void beginUpdate();

		/// <summary>
		/// Decrements the updateLevel by one and fires a notification event if the
		/// updateLevel reaches 0.
		/// </summary>
		void endUpdate();

		/// <summary>
		/// Binds the specified function to the given event name. If no event name
		/// is given, then the listener is registered for all events.
		/// </summary>
		void addListener(string eventName, mxIEventListener listener);

		/// <summary>
		/// Function: removeListener
		/// 
		/// Removes the given listener from the list of listeners.
		/// </summary>
		void removeListener(mxIEventListener listener);

		/// <summary>
		/// Function: removeListener
		/// 
		/// Removes the given listener from the list of listeners.
		/// </summary>
		void removeListener(mxIEventListener listener, string eventName);

	}

	public abstract class mxIGraphModel_mxAtomicGraphModelChange : mxUndoableChange
	{
		/// <summary>
		/// Holds the model where the change happened.
		/// </summary>
		protected internal mxIGraphModel model;

		/// <summary>
		/// Constructs an empty atomic graph model change.
		/// </summary>
		public mxIGraphModel_mxAtomicGraphModelChange() : this(null)
		{
		}

		/// <summary>
		/// Constructs an atomic graph model change for the given model.
		/// </summary>
		public mxIGraphModel_mxAtomicGraphModelChange(mxIGraphModel model)
		{
			this.model = model;
		}

		/// <summary>
		/// Returns the model where the change happened.
		/// </summary>
		public virtual mxIGraphModel Model
		{
			get
			{
				return model;
			}
			set
			{
				this.model = value;
			}
		}


		/// <summary>
		/// Executes the change on the model.
		/// </summary>
		public abstract void execute();

	}

}