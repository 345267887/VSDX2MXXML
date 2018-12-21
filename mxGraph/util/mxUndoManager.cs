using System.Collections.Generic;

/// <summary>
/// $Id: mxUndoManager.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{


	/// <summary>
	/// Implements an undo history.
	/// 
	/// This class fires the following events:
	/// 
	/// mxEvent.CLEAR fires after clear was executed. The event has no properties.
	/// 
	/// mxEvent.UNDO fires afer a significant edit was undone in undo. The
	/// <code>edit</code> property contains the mxUndoableEdit that was undone.
	/// 
	/// mxEvent.REDO fires afer a significant edit was redone in redo. The
	/// <code>edit</code> property contains the mxUndoableEdit that was redone.
	/// 
	/// mxEvent.ADD fires after an undoable edit was added to the history. The
	/// <code>edit</code> property contains the mxUndoableEdit that was added.
	/// </summary>
	public class mxUndoManager : mxEventSource
	{

		/// <summary>
		/// Maximum command history size. 0 means unlimited history. Default is 100.
		/// </summary>
		protected internal int size;

		/// <summary>
		/// List that contains the steps of the command history.
		/// </summary>
		protected internal IList<mxUndoableEdit> history;

		/// <summary>
		/// Index of the element to be added next.
		/// </summary>
		protected internal int indexOfNextAdd;

		/// <summary>
		/// Constructs a new undo manager with a default history size.
		/// </summary>
		public mxUndoManager() : this(100)
		{
		}

		/// <summary>
		/// Constructs a new undo manager for the specified size.
		/// </summary>
		public mxUndoManager(int size)
		{
			this.size = size;
			clear();
		}

		/// 
		public virtual bool Empty
		{
			get
			{
				return history.Count == 0;
			}
		}

		/// <summary>
		/// Clears the command history.
		/// </summary>
		public virtual void clear()
		{
			history = new List<mxUndoableEdit>(size);
			indexOfNextAdd = 0;
			fireEvent(new mxEventObject(mxEvent.CLEAR));
		}

		/// <summary>
		/// Returns true if an undo is possible.
		/// </summary>
		public virtual bool canUndo()
		{
			return indexOfNextAdd > 0;
		}

		/// <summary>
		/// Undoes the last change.
		/// </summary>
		public virtual void undo()
		{
			while (indexOfNextAdd > 0)
			{
				mxUndoableEdit edit = history[--indexOfNextAdd];
				edit.undo();

				if (edit.Significant)
				{
					fireEvent(new mxEventObject(mxEvent.UNDO, "edit", edit));
					break;
				}
			}
		}

		/// <summary>
		/// Returns true if a redo is possible.
		/// </summary>
		public virtual bool canRedo()
		{
			return indexOfNextAdd < history.Count;
		}

		/// <summary>
		/// Redoes the last change.
		/// </summary>
		public virtual void redo()
		{
			int n = history.Count;

			while (indexOfNextAdd < n)
			{
				mxUndoableEdit edit = history[indexOfNextAdd++];
				edit.redo();

				if (edit.Significant)
				{
					fireEvent(new mxEventObject(mxEvent.REDO, "edit", edit));
					break;
				}
			}
		}

		/// <summary>
		/// Method to be called to add new undoable edits to the history.
		/// </summary>
		public virtual void undoableEditHappened(mxUndoableEdit undoableEdit)
		{
			trim();

			if (size > 0 && size == history.Count)
			{
				history.RemoveAt(0);
			}

			history.Add(undoableEdit);
			indexOfNextAdd = history.Count;
			fireEvent(new mxEventObject(mxEvent.ADD, "edit", undoableEdit));
		}

		/// <summary>
		/// Removes all pending steps after indexOfNextAdd from the history,
		/// invoking die on each edit. This is called from undoableEditHappened.
		/// </summary>
		protected internal virtual void trim()
		{
			while (history.Count > indexOfNextAdd)
			{

                mxUndoableEdit edit = history[indexOfNextAdd];
                history.RemoveAt(indexOfNextAdd);
				edit.die();
			}
		}

	}

}