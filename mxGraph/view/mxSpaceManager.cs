using System;

namespace mxGraph.view
{

	using mxGeometry = model.mxGeometry;
	using mxIGraphModel = model.mxIGraphModel;
	using mxEvent = util.mxEvent;
	using mxEventObject = util.mxEventObject;
	using mxEventSource = util.mxEventSource;
	using mxPoint = util.mxPoint;

	public class mxSpaceManager : mxEventSource
	{

		/// <summary>
		/// Defines the type of the source or target terminal. The type is a string
		/// passed to mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal mxGraph graph;

		/// <summary>
		/// Optional string that specifies the value of the attribute to be passed
		/// to mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool enabled;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool shiftRightwards;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool shiftDownwards;

		/// <summary>
		/// Optional string that specifies the attributename to be passed to
		/// mxCell.is to check if the rule applies to a cell.
		/// </summary>
		protected internal bool extendParents;

		/// 
		protected internal mxIEventListener resizeHandler = new mxIEventListenerAnonymousInnerClass();

		private class mxIEventListenerAnonymousInnerClass : mxIEventListener
		{
			public mxIEventListenerAnonymousInnerClass()
			{
			}

			public virtual void invoke(object source, mxEventObject evt)
			{
				//if (outerInstance.Enabled)
				//{
				//	outerInstance.cellsResized((object[]) evt.getProperty("cells"));
				//}
			}
		}

		/// 
		public mxSpaceManager(mxGraph graph)
		{
			Graph = graph;
		}

		/// 
		public virtual bool isCellIgnored(object cell)
		{
			return !Graph.Model.isVertex(cell);
		}

		/// 
		public virtual bool isCellShiftable(object cell)
		{
			return Graph.Model.isVertex(cell) && Graph.isCellMovable(cell);
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


		/// <returns> the shiftRightwards </returns>
		public virtual bool ShiftRightwards
		{
			get
			{
				return shiftRightwards;
			}
			set
			{
				this.shiftRightwards = value;
			}
		}


		/// <returns> the shiftDownwards </returns>
		public virtual bool ShiftDownwards
		{
			get
			{
				return shiftDownwards;
			}
			set
			{
				this.shiftDownwards = value;
			}
		}


		/// <returns> the extendParents </returns>
		public virtual bool ExtendParents
		{
			get
			{
				return extendParents;
			}
			set
			{
				this.extendParents = value;
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
				if (this.graph != null)
				{
					this.graph.removeListener(resizeHandler);
				}
    
				this.graph = value;
    
				if (this.graph != null)
				{
					this.graph.addListener(mxEvent.RESIZE_CELLS, resizeHandler);
					this.graph.addListener(mxEvent.FOLD_CELLS, resizeHandler);
				}
			}
		}


		/// 
		protected internal virtual void cellsResized(object[] cells)
		{
			if (cells != null)
			{
				mxIGraphModel model = Graph.Model;

				model.beginUpdate();
				try
				{
					for (int i = 0; i < cells.Length; i++)
					{
						if (!isCellIgnored(cells[i]))
						{
							cellResized(cells[i]);
							break;
						}
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// 
		protected internal virtual void cellResized(object cell)
		{
			mxGraph graph = Graph;
			mxGraphView view = graph.View;
			mxIGraphModel model = graph.Model;

			mxCellState state = view.getState(cell);
			mxCellState pstate = view.getState(model.getParent(cell));

			if (state != null && pstate != null)
			{
				object[] cells = getCellsToShift(state);
				mxGeometry geo = model.getGeometry(cell);

				if (cells != null && geo != null)
				{
					mxPoint tr = view.Translate;
					double scale = view.Scale;

					double x0 = state.X - pstate.Origin.X - tr.X * scale;
					double y0 = state.Y - pstate.Origin.Y - tr.Y * scale;
					double right = state.X + state.Width;
					double bottom = state.Y + state.Height;

					double dx = state.Width - geo.Width * scale + x0 - geo.X * scale;
					double dy = state.Height - geo.Height * scale + y0 - geo.Y * scale;

					double fx = 1 - geo.Width * scale / state.Width;
					double fy = 1 - geo.Height * scale / state.Height;

					model.beginUpdate();
					try
					{
						for (int i = 0; i < cells.Length; i++)
						{
							if (cells[i] != cell && isCellShiftable(cells[i]))
							{
								shiftCell(cells[i], dx, dy, x0, y0, right, bottom, fx, fy, ExtendParents && graph.isExtendParent(cells[i]));
							}
						}
					}
					finally
					{
						model.endUpdate();
					}
				}
			}
		}

		/// 
		protected internal virtual void shiftCell(object cell, double dx, double dy, double x0, double y0, double right, double bottom, double fx, double fy, bool extendParent)
		{
			mxGraph graph = Graph;
			mxCellState state = graph.View.getState(cell);

			if (state != null)
			{
				mxIGraphModel model = graph.Model;
				mxGeometry geo = model.getGeometry(cell);

				if (geo != null)
				{
					model.beginUpdate();
					try
					{
						if (ShiftRightwards)
						{
							if (state.X >= right)
							{
								geo = (mxGeometry) geo.clone();
								geo.translate(-dx, 0);
							}
							else
							{
								double tmpDx = Math.Max(0, state.X - x0);
								geo = (mxGeometry) geo.clone();
								geo.translate(-fx * tmpDx, 0);
							}
						}

						if (ShiftDownwards)
						{
							if (state.Y >= bottom)
							{
								geo = (mxGeometry) geo.clone();
								geo.translate(0, -dy);
							}
							else
							{
								double tmpDy = Math.Max(0, state.Y - y0);
								geo = (mxGeometry) geo.clone();
								geo.translate(0, -fy * tmpDy);
							}

							if (geo != model.getGeometry(cell))
							{
								model.setGeometry(cell, geo);

								// Parent size might need to be updated if this
								// is seen as part of the resize
								if (extendParent)
								{
									graph.extendParent(cell);
								}
							}
						}
					}
					finally
					{
						model.endUpdate();
					}
				}
			}
		}

		/// 
		protected internal virtual object[] getCellsToShift(mxCellState state)
		{
			mxGraph graph = this.Graph;
			object parent = graph.Model.getParent(state.Cell);
			bool down = ShiftDownwards;
			bool right = ShiftRightwards;

			return graph.getCellsBeyond(state.X + ((down) ? 0 : state.Width), state.Y + ((down && right) ? 0 : state.Height), parent, right, down);
		}

		/// 
		public virtual void destroy()
		{
			Graph = null;
		}

	}

}