using System.Collections.Generic;
using System.Drawing;

namespace mxGraph.view
{


	using mxIGraphLayout = layout.mxIGraphLayout;
	using mxGraphModel = model.mxGraphModel;
	using mxIGraphModel = model.mxIGraphModel;
	using mxChildChange = model.mxGraphModel.mxChildChange;
	using mxGeometryChange = model.mxGraphModel.mxGeometryChange;
	using mxRootChange = model.mxGraphModel.mxRootChange;
	using mxTerminalChange = model.mxGraphModel.mxTerminalChange;
	using mxEvent = util.mxEvent;
	using mxEventObject = util.mxEventObject;
	using mxEventSource = util.mxEventSource;
	using mxUndoableEdit = util.mxUndoableEdit;
	using mxUtils = util.mxUtils;
	using mxUndoableChange = util.mxUndoableEdit.mxUndoableChange;

	/// <summary>
	/// Implements a layout manager that updates the layout for a given transaction.
	/// The following example installs an automatic tree layout in a graph:
	/// 
	/// <code>
	/// new mxLayoutManager(graph) {
	/// 
	///   mxCompactTreeLayout layout = new mxCompactTreeLayout(graph);
	/// 
	///   public mxIGraphLayout getLayout(Object parent)
	///   {
	///     if (graph.getModel().getChildCount(parent) > 0) {
	///       return layout;
	///     }
	///     return null;
	///   }
	/// };
	/// </code>
	/// 
	/// This class fires the following event:
	/// 
	/// mxEvent.LAYOUT_CELLS fires between begin- and endUpdate after all cells have
	/// been layouted in layoutCells. The <code>cells</code> property contains all
	/// cells that have been passed to layoutCells.
	/// </summary>
	public class mxLayoutManager : mxEventSource
	{

		/// <summary>
		/// Defines the type of the source or target terminal. The type is a string
		/// passed to mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal mxGraph graph;

		/// <summary>
		/// Optional string that specifies the value of the attribute to be passed
		/// to mxCell.is to check if the rule applies to a cell. Default is true.
		/// </summary>
		protected internal bool enabled = true;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell. Default is true.
		/// </summary>
		protected internal bool bubbling = true;

		/// 
		protected internal mxIEventListener undoHandler = new mxIEventListenerAnonymousInnerClass();

		private class mxIEventListenerAnonymousInnerClass : mxIEventListener
		{
			public mxIEventListenerAnonymousInnerClass()
			{
			}

			public virtual void invoke(object source, mxEventObject evt)
			{
				//if (outerInstance.Enabled)
				//{
				//	outerInstance.beforeUndo((mxUndoableEdit) evt.getProperty("edit"));
				//}
			}
		}

		/// 
		protected internal mxIEventListener moveHandler = new mxIEventListenerAnonymousInnerClass2();

		private class mxIEventListenerAnonymousInnerClass2 : mxIEventListener
		{
			public mxIEventListenerAnonymousInnerClass2()
			{
			}

			public virtual void invoke(object source, mxEventObject evt)
			{
				//if (outerInstance.Enabled)
				//{
				//	outerInstance.cellsMoved((object[]) evt.getProperty("cells"), (Point) evt.getProperty("location"));
				//}
			}
		}

		/// 
		public mxLayoutManager(mxGraph graph)
		{
			Graph = graph;
		}

		/// <returns> the enabled </returns>
		public virtual bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}


		/// <returns> the bubbling </returns>
		public virtual bool Bubbling
		{
			get
			{
				return bubbling;
			}
			set
			{
				bubbling = value;
			}
		}


		/// <returns> the graph </returns>
		public virtual mxGraph Graph
		{
			get
			{
				return graph;
			}
			set
			{
				if (graph != null)
				{
					mxIGraphModel model = graph.Model;
					model.removeListener(undoHandler);
					graph.removeListener(moveHandler);
				}
    
				graph = value;
    
				if (graph != null)
				{
					mxIGraphModel model = graph.Model;
					model.addListener(mxEvent.BEFORE_UNDO, undoHandler);
					graph.addListener(mxEvent.MOVE_CELLS, moveHandler);
				}
			}
		}


		/// 
		protected internal virtual mxIGraphLayout getLayout(object parent)
		{
			return null;
		}

		/// 
		protected internal virtual void cellsMoved(object[] cells, Point location)
		{
			if (cells != null && location != null)
			{
				mxIGraphModel model = Graph.Model;

				// Checks if a layout exists to take care of the moving
				for (int i = 0; i < cells.Length; i++)
				{
					mxIGraphLayout layout = getLayout(model.getParent(cells[i]));

					if (layout != null)
					{
                        layout.moveCell(cells[i], location.X, location.Y);
					}
				}
			}
		}

		/// 
		protected internal virtual void beforeUndo(mxUndoableEdit edit)
		{
			ICollection<object> cells = getCellsForChanges(edit.Changes);
			mxIGraphModel model = Graph.Model;

			if (Bubbling)
			{
				object[] tmp = mxGraphModel.getParents(model, ((List<object>)cells).ToArray());

				while (tmp.Length > 0)
				{
                    foreach (var item in tmp)
                    {
                        cells.Add(item);
                    }

					tmp = mxGraphModel.getParents(model, tmp);
				}
			}

			layoutCells(((List<object>)mxUtils.sortCells(cells, false)).ToArray());
		}

		/// 
		protected internal virtual ICollection<object> getCellsForChanges(IList<mxUndoableEdit.mxUndoableChange> changes)
		{
			ISet<object> result = new HashSet<object>();
			IEnumerator<mxUndoableEdit.mxUndoableChange> it = changes.GetEnumerator();

			while (it.MoveNext())
			{
				mxUndoableEdit.mxUndoableChange change = it.Current;

				if (change is mxGraphModel.mxRootChange)
				{
					return new HashSet<object>();
				}
				else
				{
                    List<object> tmp = (List<object>)getCellsForChange(change);
                    foreach (var item in tmp)
                    {
                        result.Add(item);
                    }
                    
				}
			}

			return result;
		}

		/// 
		protected internal virtual ICollection<object> getCellsForChange(mxUndoableEdit.mxUndoableChange change)
		{
			mxIGraphModel model = Graph.Model;
			ISet<object> result = new HashSet<object>();

			if (change is mxGraphModel.mxChildChange)
			{
				mxGraphModel.mxChildChange cc = (mxGraphModel.mxChildChange) change;
				object parent = model.getParent(cc.Child);

				if (cc.Child != null)
				{
					result.Add(cc.Child);
				}

				if (parent != null)
				{
					result.Add(parent);
				}

				if (cc.Previous != null)
				{
					result.Add(cc.Previous);
				}
			}
			else if (change is mxGraphModel.mxTerminalChange || change is mxGraphModel.mxGeometryChange)
			{
				object cell = (change is mxGraphModel.mxTerminalChange) ? ((mxGraphModel.mxTerminalChange) change).Cell : ((mxGraphModel.mxGeometryChange) change).Cell;

				if (cell != null)
				{
					result.Add(cell);
					object parent = model.getParent(cell);

					if (parent != null)
					{
						result.Add(parent);
					}
				}
			}

			return result;
		}

		/// 
		protected internal virtual void layoutCells(object[] cells)
		{
			if (cells.Length > 0)
			{
				// Invokes the layouts while removing duplicates
				mxIGraphModel model = Graph.Model;

				model.beginUpdate();
				try
				{
					for (int i = 0; i < cells.Length; i++)
					{
						if (cells[i] != model.Root)
						{
							executeLayout(getLayout(cells[i]), cells[i]);
						}
					}

					fireEvent(new mxEventObject(mxEvent.LAYOUT_CELLS, "cells", cells));
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// 
		protected internal virtual void executeLayout(mxIGraphLayout layout, object parent)
		{
			if (layout != null && parent != null)
			{
				layout.execute(parent);
			}
		}

		/// 
		public virtual void destroy()
		{
			Graph = null;
		}

	}

}