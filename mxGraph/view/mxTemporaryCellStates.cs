using System.Collections.Generic;

namespace mxGraph.view
{

	using mxCell = model.mxCell;
	using mxRectangle = util.mxRectangle;

	public class mxTemporaryCellStates
	{
		/// 
		protected internal mxGraphView view;

		/// 
		protected internal Dictionary<object, mxCellState> oldStates;

		/// 
		protected internal mxRectangle oldBounds;

		/// 
		protected internal double oldScale;

		/// <summary>
		/// Constructs a new temporary cell states instance.
		/// </summary>
		public mxTemporaryCellStates(mxGraphView view) : this(view, 1, null)
		{
		}

		/// <summary>
		/// Constructs a new temporary cell states instance.
		/// </summary>
		public mxTemporaryCellStates(mxGraphView view, double scale) : this(view, scale, null)
		{
		}

		/// <summary>
		/// Constructs a new temporary cell states instance.
		/// </summary>
		public mxTemporaryCellStates(mxGraphView view, double scale, object[] cells)
		{
			this.view = view;

			// Stores the previous state
			oldBounds = view.GraphBounds;
			oldStates = view.States;
			oldScale = view.Scale;

			// Creates space for the new states
			view.States = new Dictionary<object, mxCellState>();
			view.Scale = scale;

			if (cells != null)
			{
				// Creates virtual parent state for validation
				mxCellState state = view.createState(new mxCell());

				// Validates the vertices and edges without adding them to
				// the model so that the original cells are not modified
				for (int i = 0; i < cells.Length; i++)
				{
					view.validateBounds(state, cells[i]);
				}

				mxRectangle bbox = null;

				for (int i = 0; i < cells.Length; i++)
				{
					mxRectangle bounds = view.validatePoints(state, cells[i]);

					if (bounds != null)
					{
						if (bbox == null)
						{
							bbox = bounds;
						}
						else
						{
							bbox.add(bounds);
						}
					}
				}

				if (bbox == null)
				{
					bbox = new mxRectangle();
				}

				view.GraphBounds = bbox;
			}
		}

		/// <summary>
		/// Destroys the cell states and restores the state of the graph view.
		/// </summary>
		public virtual void destroy()
		{
			view.Scale = oldScale;
			view.States = oldStates;
			view.GraphBounds = oldBounds;
		}

	}

}