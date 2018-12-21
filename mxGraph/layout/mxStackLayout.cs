using System;

namespace mxGraph.layout
{

	using mxGeometry = mxGraph.model.mxGeometry;
	using mxICell = mxGraph.model.mxICell;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxCellState = mxGraph.view.mxCellState;
	using mxGraph = mxGraph.view.mxGraph;

	public class mxStackLayout : mxGraphLayout
	{

		/// <summary>
		/// Specifies the orientation of the layout. Default is true.
		/// </summary>
		protected internal bool horizontal;

		/// <summary>
		/// Specifies the spacing between the cells. Default is 0.
		/// </summary>
		protected internal int spacing;

		/// <summary>
		/// Specifies the horizontal origin of the layout. Default is 0.
		/// </summary>
		protected internal int x0;

		/// <summary>
		/// Specifies the vertical origin of the layout. Default is 0.
		/// </summary>
		protected internal int y0;

		/// <summary>
		/// Border to be added if fill is true. Default is 0.
		/// </summary>
		protected internal int border;

		/// <summary>
		/// Boolean indicating if dimension should be changed to fill out the parent
		/// cell. Default is false.
		/// </summary>
		protected internal bool fill = false;

		/// <summary>
		/// If the parent should be resized to match the width/height of the
		/// stack. Default is false.
		/// </summary>
		protected internal bool resizeParent = false;

		/// <summary>
		/// Value at which a new column or row should be created. Default is 0.
		/// </summary>
		protected internal int wrap = 0;

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxStackLayout(mxGraph graph) : this(graph, true)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxStackLayout(mxGraph graph, bool horizontal) : this(graph, horizontal, 0)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxStackLayout(mxGraph graph, bool horizontal, int spacing) : this(graph, horizontal, spacing, 0, 0, 0)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxStackLayout(mxGraph graph, bool horizontal, int spacing, int x0, int y0, int border) : base(graph)
		{
			this.horizontal = horizontal;
			this.spacing = spacing;
			this.x0 = x0;
			this.y0 = y0;
			this.border = border;
		}

		/// 
		public virtual bool Horizontal
		{
			get
			{
				return horizontal;
			}
		}

		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxGraphLayout#move(java.lang.Object, double, double)
		 */
		public override void moveCell(object cell, double x, double y)
		{
			mxIGraphModel model = graph.Model;
			object parent = model.getParent(cell);
			bool horizontal = Horizontal;

			if (cell is mxICell && parent is mxICell)
			{
				int i = 0;
				double last = 0;
				int childCount = model.getChildCount(parent);
				double value = (horizontal) ? x : y;
				mxCellState pstate = graph.View.getState(parent);

				if (pstate != null)
				{
					value -= (horizontal) ? pstate.X : pstate.Y;
				}

				for (i = 0; i < childCount; i++)
				{
					object child = model.getChildAt(parent, i);

					if (child != cell)
					{
						mxGeometry bounds = model.getGeometry(child);

						if (bounds != null)
						{
							double tmp = (horizontal) ? bounds.X + bounds.Width / 2 : bounds.Y + bounds.Height / 2;

							if (last < value && tmp > value)
							{
								break;
							}

							last = tmp;
						}
					}
				}

				// Changes child order in parent
				int idx = ((mxICell) parent).getIndex((mxICell) cell);
				idx = Math.Max(0, i - ((i > idx) ? 1 : 0));

				model.add(parent, cell, idx);
			}
		}

		/// <summary>
		/// Hook for subclassers to return the container size.
		/// </summary>
		public virtual mxRectangle ContainerSize
		{
			get
			{
				return new mxRectangle();
			}
		}

		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#execute(java.lang.Object)
		 */
		public override void execute(object parent)
		{
			if (parent != null)
			{
				bool horizontal = Horizontal;
				mxIGraphModel model = graph.Model;
				mxGeometry pgeo = model.getGeometry(parent);

				// Handles special case where the parent is either a layer with no
				// geometry or the current root of the view in which case the size
				// of the graph's container will be used.
				if (pgeo == null && model.getParent(parent) == model.Root || parent == graph.View.CurrentRoot)
				{
					mxRectangle tmp = ContainerSize;
					pgeo = new mxGeometry(0, 0, tmp.Width, tmp.Height);
				}

				double fillValue = 0;

				if (pgeo != null)
				{
					fillValue = (horizontal) ? pgeo.Height : pgeo.Width;
				}

				fillValue -= 2 * spacing + 2 * border;

				// Handles swimlane start size
				mxRectangle size = graph.getStartSize(parent);
				fillValue -= (horizontal) ? size.Height : size.Width;
				double x0 = this.x0 + size.Width + border;
				double y0 = this.y0 + size.Height + border;

				model.beginUpdate();
				try
				{
					double tmp = 0;
					mxGeometry last = null;
					int childCount = model.getChildCount(parent);

					for (int i = 0; i < childCount; i++)
					{
						object child = model.getChildAt(parent, i);

						if (!isVertexIgnored(child) && isVertexMovable(child))
						{
							mxGeometry geo = model.getGeometry(child);

							if (geo != null)
							{
								geo = (mxGeometry) geo.clone();

								if (wrap != 0 && last != null)
								{

									if ((horizontal && last.X + last.Width + geo.Width + 2 * spacing > wrap) || (!horizontal && last.Y + last.Height + geo.Height + 2 * spacing > wrap))
									{
										last = null;

										if (horizontal)
										{
											y0 += tmp + spacing;
										}
										else
										{
											x0 += tmp + spacing;
										}

										tmp = 0;
									}
								}

								tmp = Math.Max(tmp, (horizontal) ? geo.Height : geo.Width);

								if (last != null)
								{
									if (horizontal)
									{
										geo.X = last.X + last.Width + spacing;
									}
									else
									{
										geo.Y = last.Y + last.Height + spacing;
									}
								}
								else
								{
									if (horizontal)
									{
										geo.X = x0;
									}
									else
									{
										geo.Y = y0;
									}
								}

								if (horizontal)
								{
									geo.Y = y0;
								}
								else
								{
									geo.X = x0;
								}

								if (fill && fillValue > 0)
								{
									if (horizontal)
									{
										geo.Height = fillValue;
									}
									else
									{
										geo.Width = fillValue;
									}
								}

								model.setGeometry(child, geo);
								last = geo;
							}
						}
					}

					if (resizeParent && pgeo != null && last != null && !graph.isCellCollapsed(parent))
					{
						pgeo = (mxGeometry) pgeo.clone();

						if (horizontal)
						{
							pgeo.Width = last.X + last.Width + spacing;
						}
						else
						{
							pgeo.Height = last.Y + last.Height + spacing;
						}

						model.setGeometry(parent, pgeo);
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

	}

}