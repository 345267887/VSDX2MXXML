using System.Collections.Generic;

/*
 * $Id: mxGraphSelectionModel.java,v 1.1 2010-11-30 19:41:25 david Exp $
 * Copyright (c) 2001-2005, Gaudenz Alder
 * 
 * All rights reserved.
 * 
 * See LICENSE file for license details. If you are unable to locate
 * this file please contact info (at) jgraph (dot) com.
 */
namespace mxGraph.view
{


	using mxEvent = util.mxEvent;
	using mxEventObject = util.mxEventObject;
	using mxEventSource = util.mxEventSource;
	using mxUndoableEdit = util.mxUndoableEdit;
	using mxUndoableChange = util.mxUndoableEdit.mxUndoableChange;

	/// <summary>
	/// Implements the selection model for a graph.
	/// 
	/// This class fires the following events:
	/// 
	/// mxEvent.UNDO fires after the selection was changed in changeSelection. The
	/// <code>edit</code> property contains the mxUndoableEdit which contains the
	/// mxSelectionChange.
	/// 
	/// mxEvent.CHANGE fires after the selection changes by executing an
	/// mxSelectionChange. The <code>added</code> and <code>removed</code>
	/// properties contain Collections of cells that have been added to or removed
	/// from the selection, respectively.
	/// 
	/// To add a change listener to the graph selection model:
	/// 
	/// <code>
	/// addListener(
	///   mxEvent.CHANGE, new mxIEventListener()
	///   {
	///     public void invoke(Object sender, mxEventObject evt)
	///     {
	///       mxSelectionModel model = (mxSelectionModel) sender;
	///       Collection added = (Collection) evt.getArg("added");
	///       Collection removed = (Collection) evt.getArg("removed");
	///       selectionChanged(model, added, removed);
	///     }
	///   });
	/// </code>
	/// </summary>
	public class mxGraphSelectionModel : mxEventSource
	{

		/// <summary>
		/// Reference to the enclosing graph.
		/// </summary>
		protected internal mxGraph graph;

		/// <summary>
		/// Specifies if only one selected item at a time is allowed.
		/// Default is false.
		/// </summary>
		protected internal bool singleSelection = false;

		/// <summary>
		/// Holds the selection cells.
		/// </summary>
		protected internal List<object> cells = new List<object>();

		/// <summary>
		/// Constructs a new selection model for the specified graph.
		/// </summary>
		/// <param name="graph"> </param>
		public mxGraphSelectionModel(mxGraph graph)
		{
			this.graph = graph;
		}

		/// <returns> the singleSelection </returns>
		public virtual bool SingleSelection
		{
			get
			{
				return singleSelection;
			}
			set
			{
				this.singleSelection = value;
			}
		}


		/// <summary>
		/// Returns true if the given cell is selected.
		/// </summary>
		/// <param name="cell"> </param>
		/// <returns> Returns true if the given cell is selected. </returns>
		public virtual bool isSelected(object cell)
		{
			return (cell == null) ? false : cells.Contains(cell);
		}

		/// <summary>
		/// Returns true if no cells are selected.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return cells.Count == 0;
			}
		}

		/// <summary>
		/// Returns the number of selected cells.
		/// </summary>
		public virtual int size()
		{
			return cells.Count;
		}

		/// <summary>
		/// Clears the selection.
		/// </summary>
		public virtual void clear()
		{
			changeSelection(null, cells);
		}

		/// <summary>
		/// Returns the first selected cell.
		/// </summary>
		public virtual object Cell
		{
			get
			{
				return (cells.Count == 0) ? null : cells[0];
			}
			set
			{
				if (value != null)
				{
					Cells = new object[] {value};
				}
				else
				{
					clear();
				}
			}
		}

		/// <summary>
		/// Returns the selection cells.
		/// </summary>
		public virtual object[] Cells
		{
			get
			{
				return cells.ToArray();
			}
			set
			{
				if (value != null)
				{
					if (singleSelection)
					{
						value = new object[] {getFirstSelectableCell(value)};
					}
    
					IList<object> tmp = new List<object>(value.Length);
    
					for (int i = 0; i < value.Length; i++)
					{
						if (graph.isCellSelectable(value[i]))
						{
							tmp.Add(value[i]);
						}
					}
    
					changeSelection(tmp, this.cells);
				}
				else
				{
					clear();
				}
			}
		}



		/// <summary>
		/// Returns the first selectable cell in the given array of cells.
		/// </summary>
		/// <param name="cells"> Array of cells to return the first selectable cell for. </param>
		/// <returns> Returns the first cell that may be selected. </returns>
		protected internal virtual object getFirstSelectableCell(object[] cells)
		{
			if (cells != null)
			{
				for (int i = 0; i < cells.Length; i++)
				{
					if (graph.isCellSelectable(cells[i]))
					{
						return cells[i];
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Adds the given cell to the selection.
		/// </summary>
		public virtual void addCell(object cell)
		{
			if (cell != null)
			{
				addCells(new object[] {cell});
			}
		}

		/// 
		public virtual void addCells(object[] cells)
		{
			if (cells != null)
			{
				ICollection<object> remove = null;

				if (singleSelection)
				{
					remove = this.cells;
					cells = new object[] {getFirstSelectableCell(cells)};
				}

				IList<object> tmp = new List<object>(cells.Length);

				for (int i = 0; i < cells.Length; i++)
				{
					if (!isSelected(cells[i]) && graph.isCellSelectable(cells[i]))
					{
						tmp.Add(cells[i]);
					}
				}

				changeSelection(tmp, remove);
			}
		}

		/// <summary>
		/// Removes the given cell from the selection.
		/// </summary>
		public virtual void removeCell(object cell)
		{
			if (cell != null)
			{
				removeCells(new object[] {cell});
			}
		}

		/// 
		public virtual void removeCells(object[] cells)
		{
			if (cells != null)
			{
				IList<object> tmp = new List<object>(cells.Length);

				for (int i = 0; i < cells.Length; i++)
				{
					if (isSelected(cells[i]))
					{
						tmp.Add(cells[i]);
					}
				}

				changeSelection(null, tmp);
			}
		}

		/// 
		protected internal virtual void changeSelection(ICollection<object> added, ICollection<object> removed)
		{
			if ((added != null && added.Count > 0) || (removed != null && removed.Count > 0))
			{
				mxSelectionChange change = new mxSelectionChange(this, added, removed);
				change.execute();
				mxUndoableEdit edit = new mxUndoableEdit(this, false);
				edit.add(change);
				fireEvent(new mxEventObject(mxEvent.UNDO, "edit", edit));
			}
		}

		/// 
		protected internal virtual void cellAdded(object cell)
		{
			if (cell != null)
			{
				cells.Add(cell);
			}
		}

		/// 
		protected internal virtual void cellRemoved(object cell)
		{
			if (cell != null)
			{
				cells.Remove(cell);
			}
		}

		/// 
		public class mxSelectionChange : mxUndoableEdit.mxUndoableChange
		{

			/// 
			protected internal mxGraphSelectionModel model;

			/// 
			protected internal ICollection<object> added, removed;

			/// 
			/// <param name="model"> </param>
			/// <param name="added"> </param>
			/// <param name="removed"> </param>
			public mxSelectionChange(mxGraphSelectionModel model, ICollection<object> added, ICollection<object> removed)
			{
				this.model = model;
				this.added = (added != null) ? new List<object>(added) : null;
				this.removed = (removed != null) ? new List<object>(removed) : null;
			}

			/// 
			public virtual void execute()
			{
				if (removed != null)
				{
					IEnumerator<object> it = removed.GetEnumerator();

					while (it.MoveNext())
					{
						model.cellRemoved(it.Current);
					}
				}

				if (added != null)
				{
					IEnumerator<object> it = added.GetEnumerator();

					while (it.MoveNext())
					{
						model.cellAdded(it.Current);
					}
				}

				ICollection<object> tmp = added;
				added = removed;
				removed = tmp;
				model.fireEvent(new mxEventObject(mxEvent.CHANGE, "added", added, "removed", removed));
			}

		}

	}

}