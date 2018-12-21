using System.Collections.Generic;

/// <summary>
/// $Id: mxGraphLayout.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2008-2009, JGraph Ltd
/// </summary>
namespace mxGraph.layout
{


	using mxGeometry = mxGraph.model.mxGeometry;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxConstants = mxGraph.util.mxConstants;
	using mxPoint = mxGraph.util.mxPoint;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxCellState = mxGraph.view.mxCellState;
	using mxGraph = mxGraph.view.mxGraph;

	/// <summary>
	/// Abstract bass class for layouts
	/// </summary>
	public abstract class mxGraphLayout : mxIGraphLayout
	{
		public abstract void execute(object parent);

		/// <summary>
		/// Holds the enclosing graph.
		/// </summary>
		protected internal mxGraph graph;

		/// <summary>
		/// Boolean indicating if the bounding box of the label should be used if
		/// its available. Default is true.
		/// </summary>
		protected internal bool useBoundingBox = true;

		/// <summary>
		/// Constructs a new fast organic layout for the specified graph.
		/// </summary>
		public mxGraphLayout(mxGraph graph)
		{
			this.graph = graph;
		}

		/* (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#move(java.lang.Object, double, double)
		 */
		public virtual void moveCell(object cell, double x, double y)
		{
			// TODO: Map the position to a child index for
			// the cell to be placed closest to the position
		}

		/// <summary>
		/// Returns the associated graph.
		/// </summary>
		public virtual mxGraph Graph
		{
			get
			{
				return graph;
			}
		}

		/// <summary>
		/// Returns the constraint for the given key and cell. This implementation
		/// always returns the value for the given key in the style of the given
		/// cell.
		/// </summary>
		/// <param name="key"> Key of the constraint to be returned. </param>
		/// <param name="cell"> Cell whose constraint should be returned. </param>
		public virtual object getConstraint(string key, object cell)
		{
			return getConstraint(key, cell, null, false);
		}

		/// <summary>
		/// Returns the constraint for the given key and cell. The optional edge and
		/// source arguments are used to return inbound and outgoing routing-
		/// constraints for the given edge and vertex. This implementation always
		/// returns the value for the given key in the style of the given cell.
		/// </summary>
		/// <param name="key"> Key of the constraint to be returned. </param>
		/// <param name="cell"> Cell whose constraint should be returned. </param>
		/// <param name="edge"> Optional cell that represents the connection whose constraint
		/// should be returned. Default is null. </param>
		/// <param name="source"> Optional boolean that specifies if the connection is incoming
		/// or outgoing. Default is false. </param>
		public virtual object getConstraint(string key, object cell, object edge, bool source)
		{
			mxCellState state = graph.View.getState(cell);
			IDictionary<string, object> style = (state != null) ? state.Style : graph.getCellStyle(cell);

			return (style != null) ? style[key] : null;
		}

		/// <returns> the useBoundingBox </returns>
		public virtual bool UseBoundingBox
		{
			get
			{
				return useBoundingBox;
			}
			set
			{
				this.useBoundingBox = value;
			}
		}


		/// <summary>
		/// Returns true if the given vertex may be moved by the layout.
		/// </summary>
		/// <param name="vertex"> Object that represents the vertex to be tested. </param>
		/// <returns> Returns true if the vertex can be moved. </returns>
		public virtual bool isVertexMovable(object vertex)
		{
			return graph.isCellMovable(vertex);
		}

		/// <summary>
		/// Returns true if the given vertex has no connected edges.
		/// </summary>
		/// <param name="vertex"> Object that represents the vertex to be tested. </param>
		/// <returns> Returns true if the vertex should be ignored. </returns>
		public virtual bool isVertexIgnored(object vertex)
		{
			return !graph.Model.isVertex(vertex) || !graph.isCellVisible(vertex);
		}

		/// <summary>
		/// Returns true if the given edge has no source or target terminal.
		/// </summary>
		/// <param name="edge"> Object that represents the edge to be tested. </param>
		/// <returns> Returns true if the edge should be ignored. </returns>
		public virtual bool isEdgeIgnored(object edge)
		{
			mxIGraphModel model = graph.Model;

			return !model.isEdge(edge) || !graph.isCellVisible(edge) || model.getTerminal(edge, true) == null || model.getTerminal(edge, false) == null;
		}

		/// <summary>
		/// Disables or enables the edge style of the given edge.
		/// </summary>
		public virtual void setEdgeStyleEnabled(object edge, bool value)
		{
			graph.setCellStyles(mxConstants.STYLE_NOEDGESTYLE, (value) ? "0" : "1", new object[] {edge});
		}

		/// <summary>
		/// Disables or enables orthogonal end segments of the given edge
		/// </summary>
		public virtual void setOrthogonalEdge(object edge, bool value)
		{
			graph.setCellStyles(mxConstants.STYLE_ORTHOGONAL, (value) ? "1" : "0", new object[] {edge});
		}

		/// <summary>
		/// Sets the control points of the given edge to the given
		/// list of mxPoints. Set the points to null to remove all
		/// existing points for an edge.
		/// </summary>
		public virtual void setEdgePoints(object edge, IList<mxPoint> points)
		{
			mxIGraphModel model = graph.Model;
			mxGeometry geometry = model.getGeometry(edge);

			if (geometry == null)
			{
				geometry = new mxGeometry();
				geometry.Relative = true;
			}
			else
			{
				geometry = (mxGeometry) geometry.clone();
			}

			geometry.Points = points;
			model.setGeometry(edge, geometry);
		}

		/// <summary>
		/// Returns an <mxRectangle> that defines the bounds of the given cell
		/// or the bounding box if <useBoundingBox> is true.
		/// </summary>
		public virtual mxRectangle getVertexBounds(object vertex)
		{
			mxRectangle geo = graph.Model.getGeometry(vertex);

			// Checks for oversize label bounding box and corrects
			// the return value accordingly
			if (useBoundingBox)
			{
				mxCellState state = graph.View.getState(vertex);

				if (state != null)
				{
					double scale = graph.View.Scale;
					mxRectangle tmp = state.BoundingBox;

					double dx0 = (tmp.X - state.X) / scale;
					double dy0 = (tmp.Y - state.Y) / scale;
					double dx1 = (tmp.X + tmp.Width - state.X - state.Width) / scale;
					double dy1 = (tmp.Y + tmp.Height - state.Y - state.Height) / scale;

					geo = new mxRectangle(geo.X + dx0, geo.Y + dy0, geo.Width - dx0 + dx1, geo.Height + -dy0 + dy1);
				}
			}

			return new mxRectangle(geo);
		}

		/// <summary>
		/// Sets the new position of the given cell taking into account the size of
		/// the bounding box if <useBoundingBox> is true. The change is only carried
		/// out if the new location is not equal to the existing location, otherwise
		/// the geometry is not replaced with an updated instance. The new or old
		/// bounds are returned (including overlapping labels).
		/// 
		/// Parameters:
		/// 
		/// cell - <mxCell> whose geometry is to be set.
		/// x - Integer that defines the x-coordinate of the new location.
		/// y - Integer that defines the y-coordinate of the new location.
		/// </summary>
		public virtual mxRectangle setVertexLocation(object vertex, double x, double y)
		{
			mxIGraphModel model = graph.Model;
			mxGeometry geometry = model.getGeometry(vertex);
			mxRectangle result = null;

			if (geometry != null)
			{
				result = new mxRectangle(x, y, geometry.Width, geometry.Height);

				// Checks for oversize labels and offset the result
				if (useBoundingBox)
				{
					mxCellState state = graph.View.getState(vertex);

					if (state != null)
					{
						double scale = graph.View.Scale;
						mxRectangle box = state.BoundingBox;

						if (state.BoundingBox.X < state.X)
						{
							x += (state.X - box.X) / scale;
							result.Width = box.Width;
						}
						if (state.BoundingBox.Y < state.Y)
						{
							y += (state.Y - box.Y) / scale;
							result.Height = box.Height;
						}
					}
				}

				if (geometry.X != x || geometry.Y != y)
				{
					geometry = (mxGeometry) geometry.clone();
					geometry.X = x;
					geometry.Y = y;

					model.setGeometry(vertex, geometry);
				}
			}

			return result;
		}

	}

}