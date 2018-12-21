using System;
using System.Collections.Generic;

namespace mxGraph.layout
{


	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxGraph = mxGraph.view.mxGraph;

	public class mxCircleLayout : mxGraphLayout
	{

		/// <summary>
		/// Integer specifying the size of the radius. Default is 100.
		/// </summary>
		protected internal double radius;

		/// <summary>
		/// Boolean specifying if the circle should be moved to the top,
		/// left corner specified by x0 and y0. Default is false.
		/// </summary>
		protected internal bool moveCircle = true;

		/// <summary>
		/// Integer specifying the left coordinate of the circle.
		/// Default is 0.
		/// </summary>
		protected internal double x0 = 0;

		/// <summary>
		/// Integer specifying the top coordinate of the circle.
		/// Default is 0.
		/// </summary>
		protected internal double y0 = 0;

		/// <summary>
		/// Specifies if all edge points of traversed edges should be removed.
		/// Default is true.
		/// </summary>
		protected internal bool resetEdges = false;

		/// <summary>
		///  Specifies if the STYLE_NOEDGESTYLE flag should be set on edges that are
		/// modified by the result. Default is true.
		/// </summary>
		protected internal bool disableEdgeStyle = true;

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxCircleLayout(mxGraph graph) : this(graph, 100)
		{
		}

		/// <summary>
		/// Constructs a new stack layout layout for the specified graph,
		/// spacing, orientation and offset.
		/// </summary>
		public mxCircleLayout(mxGraph graph, double radius) : base(graph)
		{
			this.radius = radius;
		}

		/// <returns> the radius </returns>
		public virtual double Radius
		{
			get
			{
				return radius;
			}
			set
			{
				this.radius = value;
			}
		}


		/// <returns> the moveCircle </returns>
		public virtual bool MoveCircle
		{
			get
			{
				return moveCircle;
			}
			set
			{
				this.moveCircle = value;
			}
		}


		/// <returns> the x0 </returns>
		public virtual double X0
		{
			get
			{
				return x0;
			}
			set
			{
				this.x0 = value;
			}
		}


		/// <returns> the y0 </returns>
		public virtual double Y0
		{
			get
			{
				return y0;
			}
			set
			{
				this.y0 = value;
			}
		}


		/// <returns> the resetEdges </returns>
		public virtual bool ResetEdges
		{
			get
			{
				return resetEdges;
			}
			set
			{
				this.resetEdges = value;
			}
		}


		/// <returns> the disableEdgeStyle </returns>
		public virtual bool DisableEdgeStyle
		{
			get
			{
				return disableEdgeStyle;
			}
			set
			{
				this.disableEdgeStyle = value;
			}
		}


		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#execute(java.lang.Object)
		 */
		public override void execute(object parent)
		{
			mxIGraphModel model = graph.Model;

			// Moves the vertices to build a circle. Makes sure the
			// radius is large enough for the vertices to not
			// overlap
			model.beginUpdate();
			try
			{
				// Gets all vertices inside the parent and finds
				// the maximum dimension of the largest vertex
				double max = 0;
				double? top = null;
				double? left = null;
				List<object> vertices = new List<object>();
				int childCount = model.getChildCount(parent);

				for (int i = 0; i < childCount; i++)
				{
					object cell = model.getChildAt(parent, i);

					if (!isVertexIgnored(cell))
					{
						vertices.Add(cell);
						mxRectangle bounds = getVertexBounds(cell);

						if (top == null)
						{
							top = bounds.Y;
						}
						else
						{
							top = Math.Min(top.Value, bounds.Y);
						}

						if (left == null)
						{
							left = bounds.X;
						}
						else
						{
							left = Math.Min(left.Value, bounds.X);
						}

						max = Math.Max(max, Math.Max(bounds.Width, bounds.Height));
					}
					else if (!isEdgeIgnored(cell))
					{
						if (ResetEdges)
						{
							graph.resetEdge(cell);
						}

						if (DisableEdgeStyle)
						{
							setEdgeStyleEnabled(cell, false);
						}
					}
				}

				int vertexCount = vertices.Count;
				double r = Math.Max(vertexCount * max / Math.PI, radius);

				// Moves the circle to the specified origin
				if (moveCircle)
				{
					top = x0;
					left = y0;
				}

				circle(vertices.ToArray(), r, left.Value, top.Value);
			}
			finally
			{
				model.endUpdate();
			}
		}

		/// <summary>
		/// Executes the circular layout for the specified array
		/// of vertices and the given radius.
		/// </summary>
		public virtual void circle(object[] vertices, double r, double left, double top)
		{
			int vertexCount = vertices.Length;
			double phi = 2 * Math.PI / vertexCount;

			for (int i = 0; i < vertexCount; i++)
			{
				if (isVertexMovable(vertices[i]))
				{
					setVertexLocation(vertices[i], left + r + r * Math.Sin(i * phi), top + r + r * Math.Cos(i * phi));
				}
			}
		}

	}

}