using mxGraph;
using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxCoordinateAssignment.java,v 1.1 2010-11-30 20:36:47 david Exp $
/// Copyright (c) 2005-2010, David Benson, Gaudenz Alder
/// </summary>

namespace mxGraph.layout.hierarchical.stage
{
    

	using mxGraphAbstractHierarchyCell = mxGraph.layout.hierarchical.model.mxGraphAbstractHierarchyCell;
	using mxGraphHierarchyEdge = mxGraph.layout.hierarchical.model.mxGraphHierarchyEdge;
	using mxGraphHierarchyModel = mxGraph.layout.hierarchical.model.mxGraphHierarchyModel;
	using mxGraphHierarchyNode = mxGraph.layout.hierarchical.model.mxGraphHierarchyNode;
	using mxGraphHierarchyRank = mxGraph.layout.hierarchical.model.mxGraphHierarchyRank;
	using mxPoint = mxGraph.util.mxPoint;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxGraph = mxGraph.view.mxGraph;



    

    /// <summary>
    /// Sets the horizontal locations of node and edge dummy nodes on each layer.
    /// Uses median down and up weighings as well as heuristics to straighten edges as
    /// far as possible.
    /// </summary>
    public class mxCoordinateAssignment : mxHierarchicalLayoutStage
	{

		public enum HierarchicalEdgeStyle
		{
			ORTHOGONAL,
			POLYLINE,
			STRAIGHT
		}

		/// <summary>
		/// Reference to the enclosing layout algorithm
		/// </summary>
		protected internal mxHierarchicalLayout layout;

		/// <summary>
		/// The minimum buffer between cells on the same rank
		/// </summary>
		protected internal double intraCellSpacing = 30.0;

		/// <summary>
		/// The minimum distance between cells on adjacent ranks
		/// </summary>
		protected internal double interRankCellSpacing = 30.0;

		/// <summary>
		/// The distance between each parallel edge on each ranks for long edges
		/// </summary>
		protected internal double parallelEdgeSpacing = 10.0;

		/// <summary>
		/// The buffer on either side of a vertex where edges must not connect.
		/// </summary>
		protected internal double vertexConnectionBuffer = 0.0;

		/// <summary>
		/// The number of heuristic iterations to run
		/// </summary>
		protected internal int maxIterations = 8;

		/// <summary>
		/// The position of the root ( start ) node(s) relative to the rest of the
		/// laid out graph
		/// </summary>
		protected internal int orientation = SwingConstants.NORTH;

		/// <summary>
		/// The minimum x position node placement starts at
		/// </summary>
		protected internal double initialX;

		/// <summary>
		/// The maximum x value this positioning lays up to
		/// </summary>
		protected internal double limitX;

		/// <summary>
		/// The sum of x-displacements for the current iteration
		/// </summary>
		protected internal double currentXDelta;

		/// <summary>
		/// The rank that has the widest x position
		/// </summary>
		protected internal int widestRank;

		/// <summary>
		/// Internal cache of top-most values of Y for each rank
		/// </summary>
		protected internal double[] rankTopY;

		/// <summary>
		/// Internal cache of bottom-most value of Y for each rank
		/// </summary>
		protected internal double[] rankBottomY;

		/// <summary>
		/// The X-coordinate of the edge of the widest rank
		/// </summary>
		protected internal double widestRankValue;

		/// <summary>
		/// The width of all the ranks
		/// </summary>
		protected internal double[] rankWidths;

		/// <summary>
		/// The Y-coordinate of all the ranks
		/// </summary>
		protected internal double[] rankY;

		/// <summary>
		/// Whether or not to perform local optimisations and iterate multiple times
		/// through the algorithm
		/// </summary>
		protected internal bool fineTuning = true;

		/// <summary>
		/// Specifies if the STYLE_NOEDGESTYLE flag should be set on edges that are
		/// modified by the result. Default is true.
		/// </summary>
		protected internal bool disableEdgeStyle = true;

		/// <summary>
		/// The style to apply between cell layers to edge segments
		/// </summary>
		public HierarchicalEdgeStyle edgeStyle = HierarchicalEdgeStyle.ORTHOGONAL;

		/// <summary>
		/// A store of connections to the layer above for speed
		/// </summary>
		protected internal mxGraphAbstractHierarchyCell[][] nextLayerConnectedCache;

		/// <summary>
		/// A store of connections to the layer below for speed
		/// </summary>
		protected internal mxGraphAbstractHierarchyCell[][] previousLayerConnectedCache;

		/// <summary>
		/// The logger for this class </summary>
		//private static Logger logger = Logger.getLogger("com.jgraph.layout.hierarchical.JGraphCoordinateAssignment");

		/// <summary>
		/// Creates a coordinate assignment.
		/// </summary>
		/// <param name="intraCellSpacing">
		///            the minimum buffer between cells on the same rank </param>
		/// <param name="interRankCellSpacing">
		///            the minimum distance between cells on adjacent ranks </param>
		/// <param name="orientation">
		///            the position of the root node(s) relative to the graph </param>
		/// <param name="initialX">
		///            the leftmost coordinate node placement starts at </param>
		public mxCoordinateAssignment(mxHierarchicalLayout layout, double intraCellSpacing, double interRankCellSpacing, int orientation, double initialX, double parallelEdgeSpacing)
		{
			this.layout = layout;
			this.intraCellSpacing = intraCellSpacing;
			this.interRankCellSpacing = interRankCellSpacing;
			this.orientation = orientation;
			this.initialX = initialX;
			this.parallelEdgeSpacing = parallelEdgeSpacing;
			//LoggerLevel = Level.OFF;
		}

		/// <summary>
		/// A basic horizontal coordinate assignment algorithm
		/// </summary>
		public virtual void execute(object parent)
		{
			mxGraphHierarchyModel model = layout.Model;
			currentXDelta = 0.0;

			initialCoords(layout.Graph, model);

			if (fineTuning)
			{
				minNode(model);
			}

			double bestXDelta = 100000000.0;

			if (fineTuning)
			{
				for (int i = 0; i < maxIterations; i++)
				{
					// Median Heuristic
					if (i != 0)
					{
						medianPos(i, model);
						minNode(model);
					}

					// if the total offset is less for the current positioning,
					// there are less heavily angled edges and so the current
					// positioning is used
					if (currentXDelta < bestXDelta)
					{
						for (int j = 0; j < model.ranks.Count; j++)
						{
							mxGraphHierarchyRank rank = model.ranks[new int?(j)];
							IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

							while (iter.MoveNext())
							{
								mxGraphAbstractHierarchyCell cell = iter.Current;
								cell.setX(j, cell.getGeneralPurposeVariable(j));
							}
						}

						bestXDelta = currentXDelta;
					}
					else
					{
						// Restore the best positions
						for (int j = 0; j < model.ranks.Count; j++)
						{
							mxGraphHierarchyRank rank = model.ranks[new int?(j)];
							IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

							while (iter.MoveNext())
							{
								mxGraphAbstractHierarchyCell cell = iter.Current;
								cell.setGeneralPurposeVariable(j, (int) cell.getX(j));
							}
						}
					}

					minPath(model);

					currentXDelta = 0;
				}
			}

			setCellLocations(layout.Graph, model);
		}

		/// <summary>
		/// Performs one median positioning sweep in both directions
		/// </summary>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		private void minNode(mxGraphHierarchyModel model)
		{
			// Queue all nodes
			LinkedList<WeightedCellSorter> nodeList = new LinkedList<WeightedCellSorter>();

			// Need to be able to map from cell to cellWrapper
			IDictionary<mxGraphAbstractHierarchyCell, WeightedCellSorter> map = new Dictionary<mxGraphAbstractHierarchyCell, WeightedCellSorter>();
			mxGraphAbstractHierarchyCell[][] rank = new mxGraphAbstractHierarchyCell[model.maxRank + 1][];

			for (int i = 0; i <= model.maxRank; i++)
			{
				mxGraphHierarchyRank rankSet = model.ranks[new int?(i)];
                rank[i] = rankSet.ToArray(); //rankSet.ToArray(new mxGraphAbstractHierarchyCell[rankSet.Count]);


                for (int j = 0; j < rank[i].Length; j++)
				{
					// Use the weight to store the rank and visited to store whether
					// or not the cell is in the list
					mxGraphAbstractHierarchyCell cell = rank[i][j];
					WeightedCellSorter cellWrapper = new WeightedCellSorter(this, cell, i);
					cellWrapper.rankIndex = j;
					cellWrapper.visited = true;
					nodeList.AddLast(cellWrapper);
					map[cell] = cellWrapper;
				}
			}

			// Set a limit of the maximum number of times we will access the queue
			// in case a loop appears
			int maxTries = nodeList.Count * 10;
			int count = 0;

			// Don't move cell within this value of their median
			int tolerance = 1;

			while (nodeList.Count > 0 && count <= maxTries)
			{
				WeightedCellSorter cellWrapper = nodeList.First.Value;
				mxGraphAbstractHierarchyCell cell = cellWrapper.cell;

				int rankValue = cellWrapper.weightedValue;
				int rankIndex = cellWrapper.rankIndex;

				object[] nextLayerConnectedCells = cell.getNextLayerConnectedCells(rankValue).ToArray();
				object[] previousLayerConnectedCells = cell.getPreviousLayerConnectedCells(rankValue).ToArray();

				int numNextLayerConnected = nextLayerConnectedCells.Length;
				int numPreviousLayerConnected = previousLayerConnectedCells.Length;

				int medianNextLevel = medianXValue(nextLayerConnectedCells, rankValue + 1);
				int medianPreviousLevel = medianXValue(previousLayerConnectedCells, rankValue - 1);

				int numConnectedNeighbours = numNextLayerConnected + numPreviousLayerConnected;
				int currentPosition = cell.getGeneralPurposeVariable(rankValue);
				double cellMedian = currentPosition;

				if (numConnectedNeighbours > 0)
				{
					cellMedian = (medianNextLevel * numNextLayerConnected + medianPreviousLevel * numPreviousLayerConnected) / numConnectedNeighbours;
				}

				// Flag storing whether or not position has changed
				bool positionChanged = false;

				if (cellMedian < currentPosition - tolerance)
				{
					if (rankIndex == 0)
					{
						cell.setGeneralPurposeVariable(rankValue, (int) cellMedian);
						positionChanged = true;
					}
					else
					{
						mxGraphAbstractHierarchyCell leftCell = rank[rankValue][rankIndex - 1];
						int leftLimit = leftCell.getGeneralPurposeVariable(rankValue);
						leftLimit = leftLimit + (int) leftCell.width / 2 + (int) intraCellSpacing + (int) cell.width / 2;

						if (leftLimit < cellMedian)
						{
							cell.setGeneralPurposeVariable(rankValue, (int) cellMedian);
							positionChanged = true;
						}
						else if (leftLimit < cell.getGeneralPurposeVariable(rankValue) - tolerance)
						{
							cell.setGeneralPurposeVariable(rankValue, leftLimit);
							positionChanged = true;
						}
					}
				}
				else if (cellMedian > currentPosition + tolerance)
				{
					int rankSize = rank[rankValue].Length;

					if (rankIndex == rankSize - 1)
					{
						cell.setGeneralPurposeVariable(rankValue, (int) cellMedian);
						positionChanged = true;
					}
					else
					{
						mxGraphAbstractHierarchyCell rightCell = rank[rankValue][rankIndex + 1];
						int rightLimit = rightCell.getGeneralPurposeVariable(rankValue);
						rightLimit = rightLimit - (int) rightCell.width / 2 - (int) intraCellSpacing - (int) cell.width / 2;

						if (rightLimit > cellMedian)
						{
							cell.setGeneralPurposeVariable(rankValue, (int) cellMedian);
							positionChanged = true;
						}
						else if (rightLimit > cell.getGeneralPurposeVariable(rankValue) + tolerance)
						{
							cell.setGeneralPurposeVariable(rankValue, rightLimit);
							positionChanged = true;
						}
					}
				}

				if (positionChanged)
				{
					// Add connected nodes to map and list
					for (int i = 0; i < nextLayerConnectedCells.Length; i++)
					{
						mxGraphAbstractHierarchyCell connectedCell = (mxGraphAbstractHierarchyCell) nextLayerConnectedCells[i];
						WeightedCellSorter connectedCellWrapper = map[connectedCell];

						if (connectedCellWrapper != null)
						{
							if (connectedCellWrapper.visited == false)
							{
								connectedCellWrapper.visited = true;
								nodeList.AddLast(connectedCellWrapper);
							}
						}
					}

					// Add connected nodes to map and list
					for (int i = 0; i < previousLayerConnectedCells.Length; i++)
					{
						mxGraphAbstractHierarchyCell connectedCell = (mxGraphAbstractHierarchyCell) previousLayerConnectedCells[i];
						WeightedCellSorter connectedCellWrapper = map[connectedCell];

						if (connectedCellWrapper != null)
						{
							if (connectedCellWrapper.visited == false)
							{
								connectedCellWrapper.visited = true;
								nodeList.AddLast(connectedCellWrapper);
							}
						}
					}
				}

				nodeList.RemoveFirst();
				cellWrapper.visited = false;
				count++;
			}
		}

		/// <summary>
		/// Performs one median positioning sweep in one direction
		/// </summary>
		/// <param name="i">
		///            the iteration of the whole process </param>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		private void medianPos(int i, mxGraphHierarchyModel model)
		{
			// Reverse sweep direction each time through this method
			bool downwardSweep = (i % 2 == 0);

			if (downwardSweep)
			{
				for (int j = model.maxRank; j > 0; j--)
				{
					rankMedianPosition(j - 1, model, j);
				}
			}
			else
			{
				for (int j = 0; j < model.maxRank - 1; j++)
				{
					rankMedianPosition(j + 1, model, j);
				}
			}
		}

		/// <summary>
		/// Performs median minimisation over one rank.
		/// </summary>
		/// <param name="rankValue">
		///            the layer number of this rank </param>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		/// <param name="nextRankValue">
		///            the layer number whose connected cels are to be laid out
		///            relative to </param>
		protected internal virtual void rankMedianPosition(int rankValue, mxGraphHierarchyModel model, int nextRankValue)
		{
			mxGraphHierarchyRank rankSet = model.ranks[new int?(rankValue)];
            object[] rank = rankSet.ToArray();
			// Form an array of the order in which the cells are to be processed
			// , the order is given by the weighted sum of the in or out edges,
			// depending on whether we're travelling up or down the hierarchy.
			List<WeightedCellSorter> weightedValues = new List<WeightedCellSorter>(rank.Length);
			IDictionary<mxGraphAbstractHierarchyCell, WeightedCellSorter> cellMap = new Dictionary<mxGraphAbstractHierarchyCell, WeightedCellSorter>(rank.Length);

			for (int i = 0; i < rank.Length; i++)
			{
				mxGraphAbstractHierarchyCell currentCell = (mxGraphAbstractHierarchyCell) rank[i];
				weightedValues[i] = new WeightedCellSorter(this);
				weightedValues[i].cell = currentCell;
				weightedValues[i].rankIndex = i;
				cellMap[currentCell] = weightedValues[i];
				ICollection<mxGraphAbstractHierarchyCell> nextLayerConnectedCells = null;

				if (nextRankValue < rankValue)
				{
					nextLayerConnectedCells = currentCell.getPreviousLayerConnectedCells(rankValue);
				}
				else
				{
					nextLayerConnectedCells = currentCell.getNextLayerConnectedCells(rankValue);
				}

				// Calculate the weighing based on this node type and those this
				// node is connected to on the next layer
				weightedValues[i].weightedValue = calculatedWeightedValue(currentCell, nextLayerConnectedCells);
			}

            weightedValues.Sort();

            //Arrays.sort(weightedValues);
			// Set the new position of each node within the rank using
			// its temp variable

			for (int i = 0; i < weightedValues.Count; i++)
			{
				int numConnectionsNextLevel = 0;
				mxGraphAbstractHierarchyCell cell = weightedValues[i].cell;
				object[] nextLayerConnectedCells = null;
				int medianNextLevel = 0;

				if (nextRankValue < rankValue)
				{
					nextLayerConnectedCells = cell.getPreviousLayerConnectedCells(rankValue).ToArray();
				}
				else
				{
					nextLayerConnectedCells = cell.getNextLayerConnectedCells(rankValue).ToArray();
				}

				if (nextLayerConnectedCells != null)
				{
					numConnectionsNextLevel = nextLayerConnectedCells.Length;

					if (numConnectionsNextLevel > 0)
					{
						medianNextLevel = medianXValue(nextLayerConnectedCells, nextRankValue);
					}
					else
					{
						// For case of no connections on the next level set the
						// median to be the current position and try to be
						// positioned there
						medianNextLevel = cell.getGeneralPurposeVariable(rankValue);
					}
				}

				double leftBuffer = 0.0;
				double leftLimit = -100000000.0;

				for (int j = weightedValues[i].rankIndex - 1; j >= 0;)
				{
					WeightedCellSorter weightedValue = cellMap[(mxGraphAbstractHierarchyCell)rank[j]];

					if (weightedValue != null)
					{
						mxGraphAbstractHierarchyCell leftCell = weightedValue.cell;

						if (weightedValue.visited)
						{
							// The left limit is the right hand limit of that
							// cell plus any allowance for unallocated cells
							// in-between
							leftLimit = leftCell.getGeneralPurposeVariable(rankValue) + leftCell.width / 2.0 + intraCellSpacing + leftBuffer + cell.width / 2.0;
							j = -1;
						}
						else
						{
							leftBuffer += leftCell.width + intraCellSpacing;
							j--;
						}
					}
				}

				double rightBuffer = 0.0;
				double rightLimit = 100000000.0;

				for (int j = weightedValues[i].rankIndex + 1; j < weightedValues.Count;)
				{
					WeightedCellSorter weightedValue = cellMap[(mxGraphAbstractHierarchyCell)rank[j]];

					if (weightedValue != null)
					{
						mxGraphAbstractHierarchyCell rightCell = weightedValue.cell;

						if (weightedValue.visited)
						{
							// The left limit is the right hand limit of that
							// cell plus any allowance for unallocated cells
							// in-between
							rightLimit = rightCell.getGeneralPurposeVariable(rankValue) - rightCell.width / 2.0 - intraCellSpacing - rightBuffer - cell.width / 2.0;
							j = weightedValues.Count;
						}
						else
						{
							rightBuffer += rightCell.width + intraCellSpacing;
							j++;
						}
					}
				}

				if (medianNextLevel >= leftLimit && medianNextLevel <= rightLimit)
				{
					cell.setGeneralPurposeVariable(rankValue, medianNextLevel);
				}
				else if (medianNextLevel < leftLimit)
				{
					// Couldn't place at median value, place as close to that
					// value as possible
					cell.setGeneralPurposeVariable(rankValue, (int) leftLimit);
					currentXDelta += leftLimit - medianNextLevel;
				}
				else if (medianNextLevel > rightLimit)
				{
					// Couldn't place at median value, place as close to that
					// value as possible
					cell.setGeneralPurposeVariable(rankValue, (int) rightLimit);
					currentXDelta += medianNextLevel - rightLimit;
				}

				weightedValues[i].visited = true;
			}
		}

		/// <summary>
		/// Calculates the priority the specified cell has based on the type of its
		/// cell and the cells it is connected to on the next layer
		/// </summary>
		/// <param name="currentCell">
		///            the cell whose weight is to be calculated </param>
		/// <param name="collection">
		///            the cells the specified cell is connected to </param>
		/// <returns> the total weighted of the edges between these cells </returns>
		private int calculatedWeightedValue(mxGraphAbstractHierarchyCell currentCell, ICollection<mxGraphAbstractHierarchyCell> collection)
		{
			int totalWeight = 0;
			IEnumerator<mxGraphAbstractHierarchyCell> iter = collection.GetEnumerator();

			while (iter.MoveNext())
			{
				mxGraphAbstractHierarchyCell cell = iter.Current;

				if (currentCell.Vertex && cell.Vertex)
				{
					totalWeight++;
				}
				else if (currentCell.Edge && cell.Edge)
				{
					totalWeight += 8;
				}
				else
				{
					totalWeight += 2;
				}
			}

			return totalWeight;
		}

		/// <summary>
		/// Calculates the median position of the connected cell on the specified
		/// rank
		/// </summary>
		/// <param name="connectedCells">
		///            the cells the candidate connects to on this level </param>
		/// <param name="rankValue">
		///            the layer number of this rank </param>
		/// <returns> the median rank order ( not x position ) of the connected cells </returns>
		private int medianXValue(object[] connectedCells, int rankValue)
		{
			if (connectedCells.Length == 0)
			{
				return 0;
			}

			int[] medianValues = new int[connectedCells.Length];

			for (int i = 0; i < connectedCells.Length; i++)
			{
				medianValues[i] = ((mxGraphAbstractHierarchyCell) connectedCells[i]).getGeneralPurposeVariable(rankValue);
			}

            //Arrays.sort(medianValues);
            Array.Sort(medianValues);


            if (connectedCells.Length % 2 == 1)
			{
				// For odd numbers of adjacent vertices return the median
				return medianValues[connectedCells.Length / 2];
			}
			else
			{
				int medianPoint = connectedCells.Length / 2;
				int leftMedian = medianValues[medianPoint - 1];
				int rightMedian = medianValues[medianPoint];

				return ((leftMedian + rightMedian) / 2);
			}
		}

		/// <summary>
		/// Sets up the layout in an initial positioning. The ranks are all centered
		/// as much as possible along the middle vertex in each rank. The other cells
		/// are then placed as close as possible on either side.
		/// </summary>
		/// <param name="facade">
		///            the facade describing the input graph </param>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		private void initialCoords(mxGraph facade, mxGraphHierarchyModel model)
		{
			calculateWidestRank(facade, model);

			// Sweep up and down from the widest rank
			for (int i = widestRank; i >= 0; i--)
			{
				if (i < model.maxRank)
				{
					rankCoordinates(i, facade, model);
				}
			}

			for (int i = widestRank + 1; i <= model.maxRank; i++)
			{
				if (i > 0)
				{
					rankCoordinates(i, facade, model);
				}
			}
		}

		/// <summary>
		/// Sets up the layout in an initial positioning. All the first cells in each
		/// rank are moved to the left and the rest of the rank inserted as close
		/// together as their size and buffering permits. This method works on just
		/// the specified rank.
		/// </summary>
		/// <param name="rankValue">
		///            the current rank being processed </param>
		/// <param name="graph">
		///            the facade describing the input graph </param>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		protected internal virtual void rankCoordinates(int rankValue, mxGraph graph, mxGraphHierarchyModel model)
		{
			mxGraphHierarchyRank rank = model.ranks[new int?(rankValue)];
			double maxY = 0.0;
			double localX = initialX + (widestRankValue - rankWidths[rankValue]) / 2;

			// Store whether or not any of the cells' bounds were unavailable so
			// to only issue the warning once for all cells
			bool boundsWarning = false;

			foreach (mxGraphAbstractHierarchyCell cell in rank)
			{
				if (cell.Vertex)
				{
					mxGraphHierarchyNode node = (mxGraphHierarchyNode) cell;
					mxRectangle bounds = layout.getVertexBounds(node.cell);

					if (bounds != null)
					{
						if (orientation == SwingConstants.NORTH || orientation == SwingConstants.SOUTH)
						{
							cell.width = bounds.Width;
							cell.height = bounds.Height;
						}
						else
						{
							cell.width = bounds.Height;
							cell.height = bounds.Width;
						}
					}
					else
					{
						boundsWarning = true;
					}

					maxY = Math.Max(maxY, cell.height);
				}
				else if (cell.Edge)
				{
					mxGraphHierarchyEdge edge = (mxGraphHierarchyEdge) cell;
					// The width is the number of additional parallel edges
					// time the parallel edge spacing
					int numEdges = 1;

					if (edge.edges != null)
					{
						numEdges = edge.edges.Count;
					}
					else
					{
						//logger.info("edge.edges is null");
					}

					cell.width = (numEdges - 1) * parallelEdgeSpacing;
				}

				// Set the initial x-value as being the best result so far
				localX += cell.width / 2.0;
				cell.setX(rankValue, localX);
				cell.setGeneralPurposeVariable(rankValue, (int) localX);
				localX += cell.width / 2.0;
				localX += intraCellSpacing;
			}

			if (boundsWarning == true)
			{
				//logger.info("At least one cell has no bounds");
			}
		}

		/// <summary>
		/// Calculates the width rank in the hierarchy. Also set the y value of each
		/// rank whilst performing the calculation
		/// </summary>
		/// <param name="graph">
		///            the facade describing the input graph </param>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		protected internal virtual void calculateWidestRank(mxGraph graph, mxGraphHierarchyModel model)
		{
			// Starting y co-ordinate
			double y = -interRankCellSpacing;

			// Track the widest cell on the last rank since the y
			// difference depends on it
			double lastRankMaxCellHeight = 0.0;
			rankWidths = new double[model.maxRank + 1];
			rankY = new double[model.maxRank + 1];

			for (int rankValue = model.maxRank; rankValue >= 0; rankValue--)
			{
				// Keep track of the widest cell on this rank
				double maxCellHeight = 0.0;
				mxGraphHierarchyRank rank = model.ranks[new int?(rankValue)];
				double localX = initialX;

				// Store whether or not any of the cells' bounds were unavailable so
				// to only issue the warning once for all cells
				bool boundsWarning = false;
				IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphAbstractHierarchyCell cell = iter.Current;

					if (cell.Vertex)
					{
						mxGraphHierarchyNode node = (mxGraphHierarchyNode) cell;
						mxRectangle bounds = layout.getVertexBounds(node.cell);

						if (bounds != null)
						{
							if (orientation == SwingConstants.NORTH || orientation == SwingConstants.SOUTH)
							{
								cell.width = bounds.Width;
								cell.height = bounds.Height;
							}
							else
							{
								cell.width = bounds.Height;
								cell.height = bounds.Width;
							}
						}
						else
						{
							boundsWarning = true;
						}

						maxCellHeight = Math.Max(maxCellHeight, cell.height);
					}
					else if (cell.Edge)
					{
						mxGraphHierarchyEdge edge = (mxGraphHierarchyEdge) cell;
						// The width is the number of additional parallel edges
						// time the parallel edge spacing
						int numEdges = 1;

						if (edge.edges != null)
						{
							numEdges = edge.edges.Count;
						}
						else
						{
							//logger.info("edge.edges is null");
						}

						cell.width = (numEdges - 1) * parallelEdgeSpacing;
					}

					// Set the initial x-value as being the best result so far
					localX += cell.width / 2.0;
					cell.setX(rankValue, localX);
					cell.setGeneralPurposeVariable(rankValue, (int) localX);
					localX += cell.width / 2.0;
					localX += intraCellSpacing;

					if (localX > widestRankValue)
					{
						widestRankValue = localX;
						widestRank = rankValue;
					}

					rankWidths[rankValue] = localX;
				}

				//if (boundsWarning == true)
				//{
				//	logger.info("At least one cell has no bounds");
				//}

				rankY[rankValue] = y;
				double distanceToNextRank = maxCellHeight / 2.0 + lastRankMaxCellHeight / 2.0 + interRankCellSpacing;
				lastRankMaxCellHeight = maxCellHeight;

				if (orientation == SwingConstants.NORTH || orientation == SwingConstants.WEST)
				{
					y += distanceToNextRank;
				}
				else
				{
					y -= distanceToNextRank;
				}

				iter = rank.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphAbstractHierarchyCell cell = iter.Current;
					cell.setY(rankValue, y);
				}
			}
		}

		/// <summary>
		/// Straightens out chains of virtual nodes where possible
		/// </summary>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		protected internal virtual void minPath(mxGraphHierarchyModel model)
		{
			// Work down and up each edge with at least 2 control points
			// trying to straighten each one out. If the same number of
			// straight segments are formed in both directions, the 
			// preferred direction used is the one where the final
			// control points have the least offset from the connectable 
			// region of the terminating vertices
			IDictionary<object, mxGraphHierarchyEdge> edges = model.EdgeMapper;

			foreach (mxGraphAbstractHierarchyCell cell in edges.Values)
			{
				if (cell.maxRank > cell.minRank + 2)
				{
					int numEdgeLayers = cell.maxRank - cell.minRank - 1;
					// At least two virtual nodes in the edge
					// Check first whether the edge is already straight
					int referenceX = cell.getGeneralPurposeVariable(cell.minRank + 1);
					bool edgeStraight = true;
					int refSegCount = 0;

					for (int i = cell.minRank + 2; i < cell.maxRank; i++)
					{
						int x = cell.getGeneralPurposeVariable(i);

						if (referenceX != x)
						{
							edgeStraight = false;
							referenceX = x;
						}
						else
						{
							refSegCount++;
						}
					}

					if (edgeStraight)
					{
						continue;
					}

					int upSegCount = 0;
					int downSegCount = 0;
					double[] upXPositions = new double[numEdgeLayers - 1];
					double[] downXPositions = new double[numEdgeLayers - 1];

					double currentX = cell.getX(cell.minRank + 1);

					for (int i = cell.minRank + 1; i < cell.maxRank - 1; i++)
					{
						// Attempt to straight out the control point on the
						// next segment up with the current control point.
						double nextX = cell.getX(i + 1);

						if (currentX == nextX)
						{
							upSegCount++;
						}
						else if (repositionValid(model, cell, i + 1, currentX))
						{
							upXPositions[i - cell.minRank - 1] = currentX;
							upSegCount++;
							// Leave currentX at same value
						}
						else
						{
							currentX = nextX;
						}
					}

					currentX = cell.getX(cell.maxRank - 1);

					for (int i = cell.maxRank - 1; i > cell.minRank + 1; i--)
					{
						// Attempt to straight out the control point on the
						// next segment down with the current control point.
						double nextX = cell.getX(i - 1);

						if (currentX == nextX)
						{
							downSegCount++;
						}
						else if (repositionValid(model, cell, i - 1, currentX))
						{
							downXPositions[i - cell.minRank - 2] = currentX;
							downSegCount++;
							// Leave currentX at same value
						}
						else
						{
							currentX = nextX;
						}
					}

					if (downSegCount <= refSegCount && upSegCount <= refSegCount)
					{
						// Neither of the new calculation provide a straighter edge
						continue;
					}

					if (downSegCount > upSegCount)
					{
						// Apply down calculation values
						for (int i = cell.maxRank - 2; i > cell.minRank; i--)
						{
							cell.setX(i, (int)downXPositions[i - cell.minRank - 1]);
						}
					}
					else if (upSegCount > downSegCount)
					{
						// Apply up calculation values
						for (int i = cell.minRank + 2; i < cell.maxRank; i++)
						{
							cell.setX(i, (int)upXPositions[i - cell.minRank - 2]);
						}
					}
					else
					{
						// Neither direction provided a favourable result
						// But both calculations are better than the
						// existing solution, so apply the one with minimal
						// offset to attached vertices at either end.

					}
				}
			}
		}

		/// <summary>
		/// Determines whether or not a node may be moved to the specified x 
		/// position on the specified rank
		/// @model the layout model </summary>
		/// <param name="cell"> the cell being analysed </param>
		/// <param name="rank"> the layer of the cell </param>
		/// <param name="position"> the x position being sought </param>
		/// <returns> whether or not the virtual node can be moved to this position </returns>
		protected internal virtual bool repositionValid(mxGraphHierarchyModel model, mxGraphAbstractHierarchyCell cell, int rank, double position)
		{
			mxGraphHierarchyRank rankSet = model.ranks[new int?(rank)];
            mxGraphAbstractHierarchyCell[] rankArray = rankSet.ToArray(); //rankSet.toArray(new mxGraphAbstractHierarchyCell[rankSet.size()]);

            int rankIndex = -1;

			for (int i = 0; i < rankArray.Length; i++)
			{
				if (cell == rankArray[i])
				{
					rankIndex = i;
					break;
				}
			}

			if (rankIndex < 0)
			{
				return false;
			}

			int currentX = cell.getGeneralPurposeVariable(rank);

			if (position < currentX)
			{
				// Trying to move node to the left.
				if (rankIndex == 0)
				{
					// Left-most node, can move anywhere
					return true;
				}

				mxGraphAbstractHierarchyCell leftCell = rankArray[rankIndex - 1];
				int leftLimit = leftCell.getGeneralPurposeVariable(rank);
				leftLimit = leftLimit + (int) leftCell.width / 2 + (int) intraCellSpacing + (int) cell.width / 2;

				if (leftLimit <= position)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (position > currentX)
			{
				// Trying to move node to the right.
				if (rankIndex == rankArray.Length - 1)
				{
					// Right-most node, can move anywhere
					return true;
				}

				mxGraphAbstractHierarchyCell rightCell = rankArray[rankIndex + 1];
				int rightLimit = rightCell.getGeneralPurposeVariable(rank);
				rightLimit = rightLimit - (int) rightCell.width / 2 - (int) intraCellSpacing - (int) cell.width / 2;

				if (rightLimit >= position)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Sets the cell locations in the facade to those stored after this layout
		/// processing step has completed.
		/// </summary>
		/// <param name="graph">
		///            the facade describing the input graph </param>
		/// <param name="model">
		///            an internal model of the hierarchical layout </param>
		protected internal virtual void setCellLocations(mxGraph graph, mxGraphHierarchyModel model)
		{
			rankTopY = new double[model.ranks.Count];
			rankBottomY = new double[model.ranks.Count];

			for (int i = 0; i < model.ranks.Count; i++)
			{
				rankTopY[i] = double.MaxValue;
				rankBottomY[i] = 0.0;
			}

			IDictionary<object, mxGraphHierarchyEdge> edges = model.EdgeMapper;
			IDictionary<object, mxGraphHierarchyNode> vertices = model.VertexMapper;

			// Process vertices all first, since they define the lower and 
			// limits of each rank. Between these limits lie the channels
			// where the edges can be routed across the graph

			foreach (mxGraphAbstractHierarchyCell cell in vertices.Values)
			{
				VertexLocation = cell;
			}

			foreach (mxGraphAbstractHierarchyCell cell in edges.Values)
			{
				EdgePosition = cell;
			}

			// Post process edge styles
			if (this.edgeStyle == HierarchicalEdgeStyle.ORTHOGONAL)
			{

			}
		}

		/// <summary>
		/// Fixes the control points </summary>
		/// <param name="cell"> </param>
		protected internal virtual mxGraphAbstractHierarchyCell EdgePosition
		{
			set
			{
				// Do not process single rank length edges
				if (value.minRank == value.maxRank)
				{
					return;
				}
    
				mxGraphHierarchyEdge edge = (mxGraphHierarchyEdge) value;
				// For parallel edges we need to seperate out the points a
				// little
				double offsetX = 0.0;
				// Only set the edge control points once
    
				if (edge.temp[0] != 101207)
				{
					IEnumerator<object> parallelEdges = edge.edges.GetEnumerator();
    
					while (parallelEdges.MoveNext())
					{
						object realEdge = parallelEdges.Current;
						//List oldPoints = graph.getPoints(realEdge);
						IList<mxPoint> newPoints = new List<mxPoint>(edge.x.Length);
    
						// Declare variables to define loop through edge points and 
						// change direction if edge is reversed
    
						int loopStart = edge.x.Length - 1;
						int loopLimit = -1;
						int loopDelta = -1;
						int currentRank = edge.maxRank - 1;
    
						if (edge.Reversed)
						{
							loopStart = 0;
							loopLimit = edge.x.Length;
							loopDelta = 1;
							currentRank = edge.minRank + 1;
						}
						// Reversed edges need the points inserted in
						// reverse order
						for (int j = loopStart; j != loopLimit; j += loopDelta)
						{
							// The horizontal position in a vertical layout
							double positionX = edge.x[j] + offsetX;
    
							// Work out the vertical positions in a vertical layout
							// in the edge buffer channels above and below this rank
							double topChannelY = (rankTopY[currentRank] + rankBottomY[currentRank + 1]) / 2.0;
							double bottomChannelY = (rankTopY[currentRank - 1] + rankBottomY[currentRank]) / 2.0;
    
							if (edge.Reversed)
							{
								double tmp = topChannelY;
								topChannelY = bottomChannelY;
								bottomChannelY = tmp;
							}
    
							if (orientation == SwingConstants.NORTH || orientation == SwingConstants.SOUTH)
							{
								newPoints.Add(new mxPoint(positionX, topChannelY));
								newPoints.Add(new mxPoint(positionX, bottomChannelY));
							}
							else
							{
								newPoints.Add(new mxPoint(topChannelY, positionX));
								newPoints.Add(new mxPoint(bottomChannelY, positionX));
							}
    
							limitX = Math.Max(limitX, positionX);
    
							//					double currentY = (rankTopY[currentRank] + rankBottomY[currentRank]) / 2.0;
							//					System.out.println("topChannelY = " + topChannelY + " , "
							//							+ "exact Y = " + edge.y[j]);
							currentRank += loopDelta;
						}
    
						if (edge.Reversed)
						{
							processReversedEdge(edge, realEdge);
						}
    
						layout.setEdgePoints(realEdge, newPoints);
    
						// Increase offset so next edge is drawn next to
						// this one
						if (offsetX == 0.0)
						{
							offsetX = parallelEdgeSpacing;
						}
						else if (offsetX > 0)
						{
							offsetX = -offsetX;
						}
						else
						{
							offsetX = -offsetX + parallelEdgeSpacing;
						}
					}
    
					edge.temp[0] = 101207;
				}
			}
		}

		/// <summary>
		/// Fixes the position of the specified vertex </summary>
		/// <param name="cell"> the vertex to position </param>
		protected internal virtual mxGraphAbstractHierarchyCell VertexLocation
		{
			set
			{
				mxGraphHierarchyNode node = (mxGraphHierarchyNode) value;
				object realCell = node.cell;
				double positionX = node.x[0] - node.width / 2;
				double positionY = node.y[0] - node.height / 2;
    
				rankTopY[value.minRank] = Math.Min(rankTopY[value.minRank], positionY);
				rankBottomY[value.minRank] = Math.Max(rankBottomY[value.minRank], positionY + node.height);
    
				if (orientation == SwingConstants.NORTH || orientation == SwingConstants.SOUTH)
				{
					layout.setVertexLocation(realCell, positionX, positionY);
				}
				else
				{
					layout.setVertexLocation(realCell, positionY, positionX);
				}
    
				limitX = Math.Max(limitX, positionX + node.width);
			}
		}

		/// <summary>
		/// Hook to add additional processing
		/// </summary>
		/// <param name="edge">
		///            The hierarchical model edge </param>
		/// <param name="realEdge">
		///            The real edge in the graph </param>
		protected internal virtual void processReversedEdge(mxGraphHierarchyEdge edge, object realEdge)
		{
			// Added as hook for customer
		}

		/// <summary>
		/// A utility class used to track cells whilst sorting occurs on the weighted
		/// sum of their connected edges. Does not violate (x.compareTo(y)==0) ==
		/// (x.equals(y))
		/// </summary>
		protected internal class WeightedCellSorter : IComparable<object>
		{
			private readonly mxCoordinateAssignment outerInstance;


			/// <summary>
			/// The weighted value of the cell stored
			/// </summary>
			public int weightedValue = 0;

			/// <summary>
			/// Whether or not to flip equal weight values.
			/// </summary>
			public bool nudge = false;

			/// <summary>
			/// Whether or not this cell has been visited in the current assignment
			/// </summary>
			public bool visited = false;

			/// <summary>
			/// The index this cell is in the model rank
			/// </summary>
			public int rankIndex;

			/// <summary>
			/// The cell whose median value is being calculated
			/// </summary>
			public mxGraphAbstractHierarchyCell cell = null;

			public WeightedCellSorter(mxCoordinateAssignment outerInstance) : this(outerInstance, null, 0)
			{
				this.outerInstance = outerInstance;
			}

			public WeightedCellSorter(mxCoordinateAssignment outerInstance, mxGraphAbstractHierarchyCell cell, int weightedValue)
			{
				this.outerInstance = outerInstance;
				this.cell = cell;
				this.weightedValue = weightedValue;
			}

			/// <summary>
			/// comparator on the medianValue
			/// </summary>
			/// <param name="arg0">
			///            the object to be compared to </param>
			/// <returns> the standard return you would expect when comparing two
			///         double </returns>
			public virtual int CompareTo(object arg0)
			{
				if (arg0 is WeightedCellSorter)
				{
					if (weightedValue > ((WeightedCellSorter) arg0).weightedValue)
					{
						return -1;
					}
					else if (weightedValue < ((WeightedCellSorter) arg0).weightedValue)
					{
						return 1;
					}
				}

				return 0;
			}
		}

		/// <summary>
		/// Utility class that stores a collection of vertices and edge points within
		/// a certain area. This area includes the buffer lengths of cells.
		/// </summary>
		protected internal class AreaSpatialCache //: Rectangle2D.Double
		{
			private readonly mxCoordinateAssignment outerInstance;

			public AreaSpatialCache(mxCoordinateAssignment outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public ISet<object> cells = new HashSet<object>();
		}

		/// <returns> Returns the interRankCellSpacing. </returns>
		public virtual double InterRankCellSpacing
		{
			get
			{
				return interRankCellSpacing;
			}
			set
			{
				this.interRankCellSpacing = value;
			}
		}


		/// <returns> Returns the intraCellSpacing. </returns>
		public virtual double IntraCellSpacing
		{
			get
			{
				return intraCellSpacing;
			}
			set
			{
				this.intraCellSpacing = value;
			}
		}


		/// <returns> Returns the orientation. </returns>
		public virtual int Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				this.orientation = value;
			}
		}


		/// <returns> Returns the limitX. </returns>
		public virtual double LimitX
		{
			get
			{
				return limitX;
			}
			set
			{
				this.limitX = value;
			}
		}


		/// <returns> Returns the fineTuning. </returns>
		public virtual bool FineTuning
		{
			get
			{
				return fineTuning;
			}
			set
			{
				this.fineTuning = value;
			}
		}


		/// <summary>
		/// Sets the logging level of this class
		/// </summary>
		/// <param name="level">
		///            the logging level to set </param>
		//public virtual Level LoggerLevel
		//{
		//	set
		//	{
		//		try
		//		{
		//			logger.Level = value;
		//		}
		//		catch (SecurityException)
		//		{
		//			// Probably running in an applet
		//		}
		//	}
		//}
	}

}