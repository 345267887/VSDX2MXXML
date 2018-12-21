using System;
using System.Collections.Generic;
using System.Drawing;

namespace mxGraph.layout
{


	using mxGeometry = mxGraph.model.mxGeometry;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxPoint = mxGraph.util.mxPoint;
	using mxCellState = mxGraph.view.mxCellState;
	using mxGraph = mxGraph.view.mxGraph;
	using mxGraphView = mxGraph.view.mxGraphView;

	public class mxEdgeLabelLayout : mxGraphLayout
	{

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxEdgeLabelLayout(mxGraph graph) : base(graph)
		{
		}

		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#execute(java.lang.Object)
		 */
		public override void execute(object parent)
		{
			mxGraphView view = graph.View;
			mxIGraphModel model = graph.Model;

			// Gets all vertices and edges inside the parent
			List<object> edges = new List<object>();
			List<object> vertices = new List<object>();
			int childCount = model.getChildCount(parent);

			for (int i = 0; i < childCount; i++)
			{
				object cell = model.getChildAt(parent, i);
				mxCellState state = view.getState(cell);

				if (state != null)
				{
					if (!isVertexIgnored(cell))
					{
						vertices.Add(state);
					}
					else if (!isEdgeIgnored(cell))
					{
						edges.Add(state);
					}
				}
			}

			placeLabels(vertices.ToArray(), edges.ToArray());
		}

		/// 
		protected internal virtual void placeLabels(object[] v, object[] e)
		{
			mxIGraphModel model = graph.Model;

			// Moves the vertices to build a circle. Makes sure the
			// radius is large enough for the vertices to not
			// overlap
			model.beginUpdate();
			try
			{
				for (int i = 0; i < e.Length; i++)
				{
					mxCellState edge = (mxCellState) e[i];

					if (edge != null && edge.LabelBounds != null)
					{
						for (int j = 0; j < v.Length; j++)
						{
							mxCellState vertex = (mxCellState) v[j];

							if (vertex != null)
							{
								avoid(edge, vertex);
							}
						}
					}
				}
			}
			finally
			{
				model.endUpdate();
			}
		}

		/// 
		protected internal virtual void avoid(mxCellState edge, mxCellState vertex)
		{
			mxIGraphModel model = graph.Model;
			Rectangle labRect = edge.LabelBounds.Rectangle;
			Rectangle vRect = vertex.Rectangle;

            if (labRect.IntersectsWith(vRect))
			{
				int dy1 = -labRect.Y - labRect.Height + vRect.Y;
                int dy2 = -labRect.Y + vRect.Y + vRect.Height;

				int dy = (Math.Abs(dy1) < Math.Abs(dy2)) ? dy1 : dy2;

                int dx1 = -labRect.X - labRect.Width + vRect.X;
                int dx2 = -labRect.X + vRect.X + vRect.Width;

				int dx = (Math.Abs(dx1) < Math.Abs(dx2)) ? dx1 : dx2;

				if (Math.Abs(dx) < Math.Abs(dy))
				{
					dy = 0;
				}
				else
				{
					dx = 0;
				}

				mxGeometry g = model.getGeometry(edge.Cell);

				if (g != null)
				{
					g = (mxGeometry) g.clone();

					if (g.Offset != null)
					{
						g.Offset.X=(g.Offset.X + dx);
						g.Offset.Y=(g.Offset.Y + dy);
					}
					else
					{
						g.Offset = new mxPoint(dx, dy);
					}

					model.setGeometry(edge.Cell, g);
				}
			}
		}

	}

}