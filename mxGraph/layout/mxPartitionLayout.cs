using System;
using System.Collections.Generic;

namespace mxGraph.layout
{


	using mxGeometry = mxGraph.model.mxGeometry;
	using mxICell = mxGraph.model.mxICell;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxGraph = mxGraph.view.mxGraph;

	public class mxPartitionLayout : mxGraphLayout
	{

		/// <summary>
		/// Boolean indicating the direction in which the space is partitioned.
		/// Default is true.
		/// </summary>
		protected internal bool horizontal;

		/// <summary>
		/// Integer that specifies the absolute spacing in pixels between the
		/// children. Default is 0.
		/// </summary>
		protected internal int spacing;

		/// <summary>
		/// Integer that specifies the absolute inset in pixels for the parent that
		/// contains the children. Default is 0.
		/// </summary>
		protected internal int border;

		/// <summary>
		/// Boolean that specifies if vertices should be resized. Default is true.
		/// </summary>
		protected internal bool resizeVertices = true;

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxPartitionLayout(mxGraph graph) : this(graph, true)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxPartitionLayout(mxGraph graph, bool horizontal) : this(graph, horizontal, 0)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxPartitionLayout(mxGraph graph, bool horizontal, int spacing) : this(graph, horizontal, spacing, 0)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxPartitionLayout(mxGraph graph, bool horizontal, int spacing, int border) : base(graph)
		{
			this.horizontal = horizontal;
			this.spacing = spacing;
			this.border = border;
		}

		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxGraphLayout#move(java.lang.Object, double, double)
		 */
		public override void moveCell(object cell, double x, double y)
		{
			mxIGraphModel model = graph.Model;
			object parent = model.getParent(cell);

			if (cell is mxICell && parent is mxICell)
			{
				int i = 0;
				double last = 0;
				int childCount = model.getChildCount(parent);

				// Finds index of the closest swimlane
				// TODO: Take into account the orientation
				for (i = 0; i < childCount; i++)
				{
					object child = model.getChildAt(parent, i);
					mxRectangle bounds = getVertexBounds(child);

					if (bounds != null)
					{
						double tmp = bounds.X + bounds.Width / 2;

						if (last < x && tmp > x)
						{
							break;
						}

						last = tmp;
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

			if (pgeo != null)
			{
				int childCount = model.getChildCount(parent);
				IList<object> children = new List<object>(childCount);

				for (int i = 0; i < childCount; i++)
				{
					object child = model.getChildAt(parent, i);

					if (!isVertexIgnored(child) && isVertexMovable(child))
					{
						children.Add(child);
					}
				}

				int n = children.Count;

				if (n > 0)
				{
					double x0 = border;
					double y0 = border;
					double other = (horizontal) ? pgeo.Height : pgeo.Width;
					other -= 2 * border;

					mxRectangle size = graph.getStartSize(parent);

					other -= (horizontal) ? size.Height : size.Width;
					x0 = x0 + size.Width;
					y0 = y0 + size.Height;

					double tmp = border + (n - 1) * spacing;
					double value = (horizontal) ? ((pgeo.Width - x0 - tmp) / n) : ((pgeo.Height - y0 - tmp) / n);

					// Avoids negative values, that is values where the sum of the
					// spacing plus the border is larger then the available space
					if (value > 0)
					{
						model.beginUpdate();
						try
						{
							for (int i = 0; i < n; i++)
							{
								object child = children[i];
								mxGeometry geo = model.getGeometry(child);

								if (geo != null)
								{
									geo = (mxGeometry) geo.clone();
									geo.X = x0;
									geo.Y = y0;

									if (horizontal)
									{
										if (resizeVertices)
										{
											geo.Width = value;
											geo.Height = other;
										}

										x0 += value + spacing;
									}
									else
									{
										if (resizeVertices)
										{
											geo.Height = value;
											geo.Width = other;
										}

										y0 += value + spacing;
									}

									model.setGeometry(child, geo);
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
		}

	}

}