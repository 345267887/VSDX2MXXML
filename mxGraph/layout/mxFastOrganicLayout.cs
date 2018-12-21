using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxFastOrganicLayout.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.layout
{


	using mxGeometry = mxGraph.model.mxGeometry;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxGraph = mxGraph.view.mxGraph;

	/// <summary>
	/// Fast organic layout algorithm.
	/// </summary>
	public class mxFastOrganicLayout : mxGraphLayout
	{
		/// <summary>
		/// Specifies if the top left corner of the input cells should be the origin
		/// of the layout result. Default is true.
		/// </summary>
		protected internal bool useInputOrigin = true;

		/// <summary>
		/// Specifies if all edge points of traversed edges should be removed.
		/// Default is true.
		/// </summary>
		protected internal bool resetEdges = true;

		/// <summary>
		///  Specifies if the STYLE_NOEDGESTYLE flag should be set on edges that are
		/// modified by the result. Default is true.
		/// </summary>
		protected internal bool disableEdgeStyle = true;

		/// <summary>
		/// The force constant by which the attractive forces are divided and the
		/// replusive forces are multiple by the square of. The value equates to the
		/// average radius there is of free space around each node. Default is 50.
		/// </summary>
		protected internal double forceConstant = 50;

		/// <summary>
		/// Cache of <forceConstant>^2 for performance.
		/// </summary>
		protected internal double forceConstantSquared = 0;

		/// <summary>
		/// Minimal distance limit. Default is 2. Prevents of
		/// dividing by zero.
		/// </summary>
		protected internal double minDistanceLimit = 2;

		/// <summary>
		/// Cached version of <minDistanceLimit> squared.
		/// </summary>
		protected internal double minDistanceLimitSquared = 0;

		/// <summary>
		/// Start value of temperature. Default is 200.
		/// </summary>
		protected internal double initialTemp = 200;

		/// <summary>
		/// Temperature to limit displacement at later stages of layout.
		/// </summary>
		protected internal double temperature = 0;

		/// <summary>
		/// Total number of iterations to run the layout though.
		/// </summary>
		protected internal int maxIterations = 0;

		/// <summary>
		/// Current iteration count.
		/// </summary>
		protected internal int iteration = 0;

		/// <summary>
		/// An array of all vertices to be laid out.
		/// </summary>
		protected internal object[] vertexArray;

		/// <summary>
		/// An array of locally stored X co-ordinate displacements for the vertices.
		/// </summary>
		protected internal double[] dispX;

		/// <summary>
		/// An array of locally stored Y co-ordinate displacements for the vertices.
		/// </summary>
		protected internal double[] dispY;

		/// <summary>
		/// An array of locally stored co-ordinate positions for the vertices.
		/// </summary>
		protected internal double[][] cellLocation;

		/// <summary>
		/// The approximate radius of each cell, nodes only.
		/// </summary>
		protected internal double[] radius;

		/// <summary>
		/// The approximate radius squared of each cell, nodes only.
		/// </summary>
		protected internal double[] radiusSquared;

		/// <summary>
		/// Array of booleans representing the movable states of the vertices.
		/// </summary>
		protected internal bool[] isMoveable;

		/// <summary>
		/// Local copy of cell neighbours.
		/// </summary>
		protected internal int[][] neighbours;

		/// <summary>
		/// Boolean flag that specifies if the layout is allowed to run. If this is
		/// set to false, then the layout exits in the following iteration.
		/// </summary>
		protected internal bool allowedToRun = true;

		/// <summary>
		/// Maps from vertices to indices.
		/// </summary>
		protected internal Dictionary<object, int?> indices = new Dictionary<object, int?>();

		/// <summary>
		/// Constructs a new fast organic layout for the specified graph.
		/// </summary>
		public mxFastOrganicLayout(mxGraph graph) : base(graph)
		{
		}

		/// <summary>
		/// Returns a boolean indicating if the given <mxCell> should be ignored as a
		/// vertex. This returns true if the cell has no connections.
		/// </summary>
		/// <param name="vertex"> Object that represents the vertex to be tested. </param>
		/// <returns> Returns true if the vertex should be ignored. </returns>
		public override bool isVertexIgnored(object vertex)
		{
			return base.isVertexIgnored(vertex) || graph.getConnections(vertex).Length == 0;
		}

		/// 
		public virtual bool UseInputOrigin
		{
			get
			{
				return useInputOrigin;
			}
			set
			{
				useInputOrigin = value;
			}
		}


		/// 
		public virtual bool ResetEdges
		{
			get
			{
				return resetEdges;
			}
			set
			{
				resetEdges = value;
			}
		}


		/// 
		public virtual bool DisableEdgeStyle
		{
			get
			{
				return disableEdgeStyle;
			}
			set
			{
				disableEdgeStyle = value;
			}
		}


		/// 
		public virtual int MaxIterations
		{
			get
			{
				return maxIterations;
			}
			set
			{
				maxIterations = value;
			}
		}


		/// 
		public virtual double ForceConstant
		{
			get
			{
				return forceConstant;
			}
			set
			{
				forceConstant = value;
			}
		}


		/// 
		public virtual double MinDistanceLimit
		{
			get
			{
				return minDistanceLimit;
			}
			set
			{
				minDistanceLimit = value;
			}
		}


		/// 
		public virtual double InitialTemp
		{
			get
			{
				return initialTemp;
			}
			set
			{
				initialTemp = value;
			}
		}


		/// <summary>
		/// Reduces the temperature of the layout from an initial setting in a linear
		/// fashion to zero.
		/// </summary>
		protected internal virtual void reduceTemperature()
		{
			temperature = initialTemp * (1.0 - iteration / maxIterations);
		}

		/* (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#move(java.lang.Object, double, double)
		 */
		public override void moveCell(object cell, double x, double y)
		{
			// TODO: Map the position to a child index for
			// the cell to be placed closest to the position
		}

		/* (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#execute(java.lang.Object)
		 */
		public override void execute(object parent)
		{
			mxIGraphModel model = graph.Model;

			// Finds the relevant vertices for the layout
			object[] vertices = graph.getChildVertices(parent);
			List<object> tmp = new List<object>(vertices.Length);

			for (int i = 0; i < vertices.Length; i++)
			{
				if (!isVertexIgnored(vertices[i]))
				{
					tmp.Add(vertices[i]);
				}
			}

			vertexArray = tmp.ToArray();
			mxRectangle initialBounds = (useInputOrigin) ? graph.getBoundsForCells(vertexArray, false, false, true) : null;
			int n = vertexArray.Length;

			dispX = new double[n];
			dispY = new double[n];
			cellLocation = new double[n][];
			isMoveable = new bool[n];
			neighbours = new int[n][];
			radius = new double[n];
			radiusSquared = new double[n];

			minDistanceLimitSquared = minDistanceLimit * minDistanceLimit;

			if (forceConstant < 0.001)
			{
				forceConstant = 0.001;
			}

			forceConstantSquared = forceConstant * forceConstant;

			// Create a map of vertices first. This is required for the array of
			// arrays called neighbours which holds, for each vertex, a list of
			// ints which represents the neighbours cells to that vertex as
			// the indices into vertexArray
			for (int i = 0; i < vertexArray.Length; i++)
			{
				object vertex = vertexArray[i];
				cellLocation[i] = new double[2];

				// Set up the mapping from array indices to cells
				indices[vertex] = new int?(i);
				mxRectangle bounds = getVertexBounds(vertex);

				// Set the X,Y value of the internal version of the cell to
				// the center point of the vertex for better positioning
				double width = bounds.Width;
				double height = bounds.Height;

				// Randomize (0, 0) locations
				double x = bounds.X;
				double y = bounds.Y;

				cellLocation[i][0] = x + width / 2.0;
				cellLocation[i][1] = y + height / 2.0;

				radius[i] = Math.Min(width, height);
				radiusSquared[i] = radius[i] * radius[i];
			}

			// Moves cell location back to top-left from center locations used in
			// algorithm, resetting the edge points is part of the transaction
			model.beginUpdate();
			try
			{
				for (int i = 0; i < n; i++)
				{
					dispX[i] = 0;
					dispY[i] = 0;
					isMoveable[i] = isVertexMovable(vertexArray[i]);

					// Get lists of neighbours to all vertices, translate the cells
					// obtained in indices into vertexArray and store as an array
					// against the original cell index
					object[] edges = graph.getConnections(vertexArray[i], parent);
					for (int k = 0; k < edges.Length; k++)
					{
						if (ResetEdges)
						{
							graph.resetEdge(edges[k]);
						}

						if (DisableEdgeStyle)
						{
							setEdgeStyleEnabled(edges[k], false);
						}
					}

					object[] cells = graph.getOpposites(edges, vertexArray[i]);

					neighbours[i] = new int[cells.Length];

					for (int j = 0; j < cells.Length; j++)
					{
						int? index = indices[cells[j]];

						// Check the connected cell in part of the vertex list to be
						// acted on by this layout
						if (index != null)
						{
							neighbours[i][j] = index.Value;
						}

						// Else if index of the other cell doesn't correspond to
						// any cell listed to be acted upon in this layout. Set
						// the index to the value of this vertex (a dummy self-loop)
						// so the attraction force of the edge is not calculated
						else
						{
							neighbours[i][j] = i;
						}
					}
				}

				temperature = initialTemp;

				// If max number of iterations has not been set, guess it
				if (maxIterations == 0)
				{
					maxIterations = (int)(20 * Math.Sqrt(n));
				}

				// Main iteration loop
				for (iteration = 0; iteration < maxIterations; iteration++)
				{
					if (!allowedToRun)
					{
						return;
					}

					// Calculate repulsive forces on all vertices
					calcRepulsion();

					// Calculate attractive forces through edges
					calcAttraction();

					calcPositions();
					reduceTemperature();
				}

				double? minx = null;
				double? miny = null;

				for (int i = 0; i < vertexArray.Length; i++)
				{
					object vertex = vertexArray[i];
					mxGeometry geo = model.getGeometry(vertex);

					if (geo != null)
					{
						cellLocation[i][0] -= geo.Width / 2.0;
						cellLocation[i][1] -= geo.Height / 2.0;

						double x = graph.snap(cellLocation[i][0]);
						double y = graph.snap(cellLocation[i][1]);
						setVertexLocation(vertex, x, y);

						if (minx == null)
						{
							minx = new double?(x);
						}
						else
						{
							minx = new double?(Math.Min(minx.Value, x));
						}

						if (miny == null)
						{
							miny = new double?(y);
						}
						else
						{
							miny = new double?(Math.Min(miny.Value, y));
						}
					}
				}

				// Modifies the cloned geometries in-place. Not needed
				// to clone the geometries again as we're in the same
				// undoable change.
				double dx = (minx != null) ? - minx.Value - 1 : 0;
				double dy = (miny != null) ? - miny.Value - 1 : 0;

				if (initialBounds != null)
				{
					dx += initialBounds.X;
					dy += initialBounds.Y;
				}

				graph.moveCells(vertexArray, dx, dy);
			}
			finally
			{
				model.endUpdate();
			}
		}

		/// <summary>
		/// Takes the displacements calculated for each cell and applies them to the
		/// local cache of cell positions. Limits the displacement to the current
		/// temperature.
		/// </summary>
		protected internal virtual void calcPositions()
		{
			for (int index = 0; index < vertexArray.Length; index++)
			{
				if (isMoveable[index])
				{
					// Get the distance of displacement for this node for this
					// iteration
					double deltaLength = Math.Sqrt(dispX[index] * dispX[index] + dispY[index] * dispY[index]);

					if (deltaLength < 0.001)
					{
						deltaLength = 0.001;
					}

					// Scale down by the current temperature if less than the
					// displacement distance
					double newXDisp = dispX[index] / deltaLength * Math.Min(deltaLength, temperature);
					double newYDisp = dispY[index] / deltaLength * Math.Min(deltaLength, temperature);

					// reset displacements
					dispX[index] = 0;
					dispY[index] = 0;

					// Update the cached cell locations
					cellLocation[index][0] += newXDisp;
					cellLocation[index][1] += newYDisp;
				}
			}
		}

		/// <summary>
		/// Calculates the attractive forces between all laid out nodes linked by
		/// edges
		/// </summary>
		protected internal virtual void calcAttraction()
		{
			// Check the neighbours of each vertex and calculate the attractive
			// force of the edge connecting them
			for (int i = 0; i < vertexArray.Length; i++)
			{
				for (int k = 0; k < neighbours[i].Length; k++)
				{
					// Get the index of the othe cell in the vertex array
					int j = neighbours[i][k];

					// Do not proceed self-loops
					if (i != j)
					{
						double xDelta = cellLocation[i][0] - cellLocation[j][0];
						double yDelta = cellLocation[i][1] - cellLocation[j][1];

						// The distance between the nodes
						double deltaLengthSquared = xDelta * xDelta + yDelta * yDelta - radiusSquared[i] - radiusSquared[j];

						if (deltaLengthSquared < minDistanceLimitSquared)
						{
							deltaLengthSquared = minDistanceLimitSquared;
						}

						double deltaLength = Math.Sqrt(deltaLengthSquared);
						double force = (deltaLengthSquared) / forceConstant;

						double displacementX = (xDelta / deltaLength) * force;
						double displacementY = (yDelta / deltaLength) * force;

						if (isMoveable[i])
						{
							this.dispX[i] -= displacementX;
							this.dispY[i] -= displacementY;
						}

						if (isMoveable[j])
						{
							dispX[j] += displacementX;
							dispY[j] += displacementY;
						}
					}
				}
			}
		}

		/// <summary>
		/// Calculates the repulsive forces between all laid out nodes
		/// </summary>
		protected internal virtual void calcRepulsion()
		{
			int vertexCount = vertexArray.Length;

			for (int i = 0; i < vertexCount; i++)
			{
				for (int j = i; j < vertexCount; j++)
				{
					// Exits if the layout is no longer allowed to run
					if (!allowedToRun)
					{
						return;
					}

					if (j != i)
					{
						double xDelta = cellLocation[i][0] - cellLocation[j][0];
						double yDelta = cellLocation[i][1] - cellLocation[j][1];

						if (xDelta == 0)
						{
							xDelta = 0.01 + GlobalRandom.NextDouble;
						}

						if (yDelta == 0)
						{
							yDelta = 0.01 + GlobalRandom.NextDouble;
						}

						// Distance between nodes
						double deltaLength = Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));

						double deltaLengthWithRadius = deltaLength - radius[i] - radius[j];

						if (deltaLengthWithRadius < minDistanceLimit)
						{
							deltaLengthWithRadius = minDistanceLimit;
						}

						double force = forceConstantSquared / deltaLengthWithRadius;

						double displacementX = (xDelta / deltaLength) * force;
						double displacementY = (yDelta / deltaLength) * force;

						if (isMoveable[i])
						{
							dispX[i] += displacementX;
							dispY[i] += displacementY;
						}

						if (isMoveable[j])
						{
							dispX[j] -= displacementX;
							dispY[j] -= displacementY;
						}
					}
				}
			}
		}

	}

}