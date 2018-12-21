using System.Collections.Generic;

/// <summary>
/// $Id: mxUndoableEdit.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{


	/// <summary>
	/// Implements a 2-dimensional rectangle with double precision coordinates.
	/// </summary>
	public class mxUndoableEdit
	{

		/// <summary>
		/// Defines the requirements for an undoable change.
		/// </summary>
		public interface mxUndoableChange
		{

			/// <summary>
			/// Undoes or redoes the change depending on its undo state.
			/// </summary>
			void execute();

		}

		/// <summary>
		/// Holds the source of the undoable edit.
		/// </summary>
		protected internal object source;

		/// <summary>
		/// Holds the list of changes that make up this undoable edit.
		/// </summary>
		protected internal IList<mxUndoableChange> changes = new List<mxUndoableChange>();

		/// <summary>
		/// Specifies this undoable edit is significant. Default is true.
		/// </summary>
		protected internal bool significant = true;

		/// <summary>
		/// Specifies the state of the undoable edit.
		/// </summary>
		protected internal bool undone, redone;

		/// <summary>
		/// Constructs a new undoable edit for the given source.
		/// </summary>
		public mxUndoableEdit(object source) : this(source, true)
		{
		}

		/// <summary>
		/// Constructs a new undoable edit for the given source.
		/// </summary>
		public mxUndoableEdit(object source, bool significant)
		{
			this.source = source;
			this.significant = significant;
		}

		/// <summary>
		/// Hook to notify any listeners of the changes after an undo or redo
		/// has been carried out. This implementation is empty.
		/// </summary>
		public virtual void dispatch()
		{
			// empty
		}

		/// <summary>
		/// Hook to free resources after the edit has been removed from the command
		/// history. This implementation is empty.
		/// </summary>
		public virtual void die()
		{
			// empty
		}

		/// <returns> the source </returns>
		public virtual object Source
		{
			get
			{
				return source;
			}
		}

		/// <returns> the changes </returns>
		public virtual IList<mxUndoableChange> Changes
		{
			get
			{
				return changes;
			}
		}

		/// <returns> the significant </returns>
		public virtual bool Significant
		{
			get
			{
				return significant;
			}
		}

		/// <returns> the undone </returns>
		public virtual bool Undone
		{
			get
			{
				return undone;
			}
		}

		/// <returns> the redone </returns>
		public virtual bool Redone
		{
			get
			{
				return redone;
			}
		}

		/// <summary>
		/// Returns true if the this edit contains no changes.
		/// </summary>
		public virtual bool Empty
		{
			get
			{
				return changes.Count == 0;
			}
		}

		/// <summary>
		/// Adds the specified change to this edit. The change is an object that is
		/// expected to either have an undo and redo, or an execute function.
		/// </summary>
		public virtual void add(mxUndoableChange change)
		{
			changes.Add(change);
		}

		/// 
		public virtual void undo()
		{
			if (!undone)
			{
				int count = changes.Count;

				for (int i = count - 1; i >= 0; i--)
				{
					mxUndoableChange change = changes[i];
					change.execute();
				}

				undone = true;
				redone = false;
			}

			dispatch();
		}

		/// 
		public virtual void redo()
		{
			if (!redone)
			{
				int count = changes.Count;

				for (int i = 0; i < count; i++)
				{
					mxUndoableChange change = changes[i];
					change.execute();
				}

				undone = false;
				redone = true;
			}

			dispatch();
		}

	}

}