using mxGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using mxGraph;

/// <summary>
/// $Id: mxOrganicLayout.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2009, JGraph Ltd
/// </summary>

namespace mxGraph.layout
{


	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxGraph = mxGraph.view.mxGraph;

	/// <summary>
	/// An implementation of a simulated annealing layout, based on "Drawing Graphs
	/// Nicely Using Simulated Annealing" by Davidson and Harel (1996). This
	/// paper describes these criteria as being favourable in a graph layout: (1)
	/// distributing nodes evenly, (2) making edge-lengths uniform, (3)
	/// minimizing cross-crossings, and (4) keeping nodes from coming too close
	/// to edges. These criteria are translated into energy cost functions in the
	/// layout. Nodes or edges breaking these criteria create a larger cost function
	/// , the total cost they contribute related to the extent that they break it.
	/// The idea of the algorithm is to minimise the total system energy. Factors
	/// are assigned to each of the criteria describing how important that
	/// criteria is. Higher factors mean that those criteria are deemed to be
	/// relatively preferable in the final layout. Most of  the criteria conflict
	/// with the others to some extent and so the setting of the factors determines
	/// the general look of the resulting graph.
	/// <para>
	/// In addition to the four aesthetic criteria the concept of a border line
	/// which induces an energy cost to nodes in proximity to the graph bounds is
	/// introduced to attempt to restrain the graph. All of the 5 factors can be
	/// switched on or off using the <code>isOptimize...</code> variables.
	/// </para>
	/// <para>
	/// Simulated Annealing is a force-directed layout and is one of the more
	/// expensive, but generally effective layouts of this type. Layouts like
	/// the spring layout only really factor in edge length and inter-node
	/// distance being the lowest CPU intensive for the most aesthetic gain. The
	/// additional factors are more expensive but can have very attractive results.
	/// </para>
	/// <para>
	/// The main loop of the algorithm consist of processing the nodes in a 
	/// deterministic order. During the processing of each node a circle of radius
	/// <code>moveRadius</code> is made around the node and split into
	/// <code>triesPerCell</code> equal segments. Each point between neighbour
	/// segments is determined and the new energy of the system if the node were
	/// moved to that position calculated. Only the necessary nodes and edges are
	/// processed new energy values resulting in quadratic performance, O(VE),
	/// whereas calculating the total system energy would be cubic. The default
	/// implementation only checks 8 points around the radius of the circle, as
	/// opposed to the suggested 30 in the paper. Doubling the number of points
	/// double the CPU load and 8 works almost as well as 30.
	/// </para>
	/// <para>
	/// The <code>moveRadius</code> replaces the temperature as the influencing
	/// factor in the way the graph settles in later iterations. If the user does
	/// not set the initial move radius it is set to half the maximum dimension
	/// of the graph. Thus, in 2 iterations a node may traverse the entire graph,
	/// and it is more sensible to find minima this way that uphill moves, which
	/// are little more than an expensive 'tilt' method. The factor by which
	/// the radius is multiplied by after each iteration is important, lowering
	/// it improves performance but raising it towards 1.0 can improve the
	/// resulting graph aesthetics. When the radius hits the minimum move radius
	/// defined, the layout terminates. The minimum move radius should be set
	/// a value where the move distance is too minor to be of interest.
	/// </para>
	/// <para>
	/// Also, the idea of a fine tuning phase is used, as described in the paper.
	/// This involves only calculating the edge to node distance energy cost
	/// at the end of the algorithm since it is an expensive calculation and
	/// it really an 'optimizating' function. <code>fineTuningRadius</code>
	/// defines the radius value that, when reached, causes the edge to node
	/// distance to be calculated.
	/// </para>
	/// <para>
	/// There are other special cases that are processed after each iteration.
	/// <code>unchangedEnergyRoundTermination</code> defines the number of
	/// iterations, after which the layout terminates. If nothing is being moved
	/// it is assumed a good layout has been found. In addition to this if
	/// no nodes are moved during an iteration the move radius is halved, presuming
	/// that a finer granularity is required.
	/// 
	/// </para>
	/// </summary>
	public class mxOrganicLayout : mxGraphLayout
	{

		/// <summary>
		/// Whether or not the distance between edge and nodes will be calculated
		/// as an energy cost function. This function is CPU intensive and is best
		/// only used in the fine tuning phase.
		/// </summary>
		protected internal bool isOptimizeEdgeDistance = true;

		/// <summary>
		/// Whether or not edges crosses will be calculated as an energy cost
		/// function. This function is CPU intensive, though if some iterations
		/// without it are required, it is best to have a few cycles at the start
		/// of the algorithm using it, then use it intermittantly through the rest
		/// of the layout.
		/// </summary>
		protected internal bool isOptimizeEdgeCrossing = true;

		/// <summary>
		/// Whether or not edge lengths will be calculated as an energy cost
		/// function. This function not CPU intensive.
		/// </summary>
		protected internal bool isOptimizeEdgeLength = true;

		/// <summary>
		/// Whether or not nodes will contribute an energy cost as they approach
		/// the bound of the graph. The cost increases to a limit close to the
		/// border and stays constant outside the bounds of the graph. This function
		/// is not CPU intensive
		/// </summary>
		protected internal bool isOptimizeBorderLine = true;

		/// <summary>
		/// Whether or not node distribute will contribute an energy cost where
		/// nodes are close together. The function is moderately CPU intensive.
		/// </summary>
		protected internal bool isOptimizeNodeDistribution = true;

		/// <summary>
		/// when <seealso cref="#moveRadius"/>reaches this value, the algorithm is terminated
		/// </summary>
		protected internal double minMoveRadius = 2.0;

		/// <summary>
		/// The current radius around each node where the next position energy
		/// values will be calculated for a possible move
		/// </summary>
		protected internal double moveRadius;

		/// <summary>
		/// The initial value of <code>moveRadius</code>. If this is set to zero
		/// the layout will automatically determine a suitable value.
		/// </summary>
		protected internal double initialMoveRadius = 0.0;

		/// <summary>
		/// The factor by which the <code>moveRadius</code> is multiplied by after
		/// every iteration. A value of 0.75 is a good balance between performance
		/// and aesthetics. Increasing the value provides more chances to find
		/// minimum energy positions and decreasing it causes the minimum radius
		/// termination condition to occur more quickly.
		/// </summary>
		protected internal double radiusScaleFactor = 0.75;

		/// <summary>
		/// The average amount of area allocated per node. If <code> bounds</code>
		/// is not set this value mutiplied by the number of nodes to find
		/// the total graph area. The graph is assumed square.
		/// </summary>
		protected internal double averageNodeArea = 10000;

		/// <summary>
		/// The radius below which fine tuning of the layout should start
		/// This involves allowing the distance between nodes and edges to be
		/// taken into account in the total energy calculation. If this is set to
		/// zero, the layout will automatically determine a suitable value
		/// </summary>
		protected internal double fineTuningRadius = 40.0;

		/// <summary>
		/// Limit to the number of iterations that may take place. This is only
		/// reached if one of the termination conditions does not occur first.
		/// </summary>
		protected internal int maxIterations = 100;

		/// <summary>
		/// Cost factor applied to energy calculations involving the distance
		/// nodes and edges. Increasing this value tends to cause nodes to move away
		/// from edges, at the partial cost of other graph aesthetics.
		/// <code>isOptimizeEdgeDistance</code> must be true for edge to nodes
		/// distances to be taken into account.
		/// </summary>
		protected internal double edgeDistanceCostFactor = 4000;

		/// <summary>
		/// Cost factor applied to energy calculations involving edges that cross
		/// over one another. Increasing this value tends to result in fewer edge
		/// crossings, at the partial cost of other graph aesthetics.
		/// <code>isOptimizeEdgeCrossing</code> must be true for edge crossings
		/// to be taken into account.
		/// </summary>
		protected internal double edgeCrossingCostFactor = 2000;

		/// <summary>
		/// Cost factor applied to energy calculations involving the general node
		/// distribution of the graph. Increasing this value tends to result in
		/// a better distribution of nodes across the available space, at the
		/// partial cost of other graph aesthetics.
		/// <code>isOptimizeNodeDistribution</code> must be true for this general
		/// distribution to be applied.
		/// </summary>
		protected internal double nodeDistributionCostFactor = 300000;

		/// <summary>
		/// Cost factor applied to energy calculations for node promixity to the
		/// notional border of the graph. Increasing this value results in
		/// nodes tending towards the centre of the drawing space, at the
		/// partial cost of other graph aesthetics.
		/// <code>isOptimizeBorderLine</code> must be true for border
		/// repulsion to be applied.
		/// </summary>
		protected internal double borderLineCostFactor = 5;

		/// <summary>
		/// Cost factor applied to energy calculations for the edge lengths.
		/// Increasing this value results in the layout attempting to shorten all
		/// edges to the minimum edge length, at the partial cost of other graph
		/// aesthetics.
		/// <code>isOptimizeEdgeLength</code> must be true for edge length
		/// shortening to be applied.
		/// </summary>
		protected internal double edgeLengthCostFactor = 0.02;

		/// <summary>
		/// The x coordinate of the final graph
		/// </summary>
		protected internal double boundsX = 0.0;

		/// <summary>
		/// The y coordinate of the final graph
		/// </summary>
		protected internal double boundsY = 0.0;

		/// <summary>
		/// The width coordinate of the final graph
		/// </summary>
		protected internal double boundsWidth = 0.0;

		/// <summary>
		/// The height coordinate of the final graph
		/// </summary>
		protected internal double boundsHeight = 0.0;

		/// <summary>
		/// current iteration number of the layout
		/// </summary>
		protected internal int iteration;

		/// <summary>
		/// determines, in how many segments the circle around cells is divided, to
		/// find a new position for the cell. Doubling this value doubles the CPU
		/// load. Increasing it beyond 16 might mean a change to the
		/// <code>performRound</code> method might further improve accuracy for a
		/// small performance hit. The change is described in the method comment.
		/// </summary>
		protected internal int triesPerCell = 8;

		/// <summary>
		/// prevents from dividing with zero and from creating excessive energy
		/// values
		/// </summary>
		protected internal double minDistanceLimit = 2;

		/// <summary>
		/// cached version of <code>minDistanceLimit</code> squared
		/// </summary>
		protected internal double minDistanceLimitSquared;

		/// <summary>
		/// distance limit beyond which energy costs due to object repulsive is
		/// not calculated as it would be too insignificant
		/// </summary>
		protected internal double maxDistanceLimit = 100;

		/// <summary>
		/// cached version of <code>maxDistanceLimit</code> squared
		/// </summary>
		protected internal double maxDistanceLimitSquared;

		/// <summary>
		/// Keeps track of how many consecutive round have passed without any energy
		/// changes 
		/// </summary>
		protected internal int unchangedEnergyRoundCount;

		/// <summary>
		/// The number of round of no node moves taking placed that the layout
		/// terminates
		/// </summary>
		protected internal int unchangedEnergyRoundTermination = 5;

		/// <summary>
		/// Whether or not to use approximate node dimensions or not. Set to true
		/// the radius squared of the smaller dimension is used. Set to false the
		/// radiusSquared variable of the CellWrapper contains the width squared
		/// and heightSquared is used in the obvious manner.
		/// </summary>
		protected internal bool approxNodeDimensions = true;

		/// <summary>
		/// Internal models collection of nodes ( vertices ) to be laid out
		/// </summary>
		protected internal CellWrapper[] v;

		/// <summary>
		/// Internal models collection of edges to be laid out
		/// </summary>
		protected internal CellWrapper[] e;

		/// <summary>
		/// Array of the x portion of the normalised test vectors that 
		/// are tested for a lower energy around each vertex. The vector 
		/// of the combined x and y normals are multipled by the current 
		/// radius to obtain test points for each vector in the array.
		/// </summary>
		protected internal double[] xNormTry;

		/// <summary>
		/// Array of the y portion of the normalised test vectors that 
		/// are tested for a lower energy around each vertex. The vector 
		/// of the combined x and y normals are multipled by the current 
		/// radius to obtain test points for each vector in the array.
		/// </summary>
		protected internal double[] yNormTry;

		/// <summary>
		/// Whether or not fine tuning is on. The determines whether or not
		/// node to edge distances are calculated in the total system energy.
		/// This cost function , besides detecting line intersection, is a
		/// performance intensive component of this algorithm and best left
		/// to optimization phase. <code>isFineTuning</code> is switched to
		/// <code>true</code> if and when the <code>fineTuningRadius</code>
		/// radius is reached. Switching this variable to <code>true</code>
		/// before the algorithm runs mean the node to edge cost function
		/// is always calculated.
		/// </summary>
		protected internal bool isFineTuning = true;

		/// <summary>
		/// Constructor for mxOrganicLayout.
		/// </summary>
		public mxOrganicLayout(mxGraph graph) : base(graph)
		{
		}

		/// <summary>
		/// Constructor for mxOrganicLayout.
		/// </summary>
		public mxOrganicLayout(mxGraph graph, Rectangle bounds) : base(graph)
		{
			boundsX = bounds.X;
			boundsY = bounds.Y;
			boundsWidth = bounds.Width;
			boundsHeight = bounds.Height;
		}

		/// <summary>
		/// Returns true if the given vertex has no connected edges.
		/// </summary>
		/// <param name="vertex"> Object that represents the vertex to be tested. </param>
		/// <returns> Returns true if the vertex should be ignored. </returns>
		public override bool isVertexIgnored(object vertex)
		{
			return false;
		}

		/// <summary>
		/// Implements <mxGraphLayout.execute>.
		/// </summary>
		public override void execute(object parent)
		{
			object[] vertices = graph.getChildVertices(parent);
			object[] edges = graph.getChildEdges(parent);

			// If the bounds dimensions have not been set see if the average area
			// per node has been
			mxRectangle bounds = graph.getBoundsForCells(vertices, false, false, true);
			if (averageNodeArea == 0.0)
			{
				if (boundsWidth == 0.0 && bounds != null)
				{
					// Just use current bounds of graph
					boundsX = bounds.X;
					boundsY = bounds.Y;
					boundsWidth = bounds.Width;
					boundsHeight = bounds.Height;
				}
			}
			else
			{
				// find the centre point of the current graph
				// based the new graph bounds on the average node area set
				double newArea = averageNodeArea * vertices.Length;
				double squareLength = Math.Sqrt(newArea);
				if (bounds != null)
				{
					double centreX = bounds.X + bounds.Width / 2.0;
					double centreY = bounds.Y + bounds.Height / 2.0;
					boundsX = centreX - squareLength / 2.0;
					boundsY = centreY - squareLength / 2.0;
				}
				else
				{
					boundsX = 0;
					boundsY = 0;
				}
				boundsWidth = squareLength;
				boundsHeight = squareLength;
				// Ensure x and y are 0 or positive
				if (boundsX < 0.0 || boundsY < 0.0)
				{
					double maxNegativeAxis = Math.Min(boundsX, boundsY);
					double axisOffset = -maxNegativeAxis;
					boundsX += axisOffset;
					boundsY += axisOffset;
				}
			}

			// If the initial move radius has not been set find a suitable value.
			// A good value is half the maximum dimension of the final graph area
			if (initialMoveRadius == 0.0)
			{
				initialMoveRadius = Math.Max(boundsWidth, boundsHeight) / 2.0;
			}

			moveRadius = initialMoveRadius;

			minDistanceLimitSquared = minDistanceLimit * minDistanceLimit;
			maxDistanceLimitSquared = maxDistanceLimit * maxDistanceLimit;

			unchangedEnergyRoundCount = 0;

			// Form internal model of nodes
			IDictionary<object, int?> vertexMap = new Dictionary<object, int?>();
			v = new CellWrapper[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				v[i] = new CellWrapper(this, vertices[i]);
				vertexMap[vertices[i]] = new int?(i);
				bounds = graph.getCellBounds(vertices[i]);
				// Set the X,Y value of the internal version of the cell to
				// the center point of the vertex for better positioning
				double width = bounds.Width;
				double height = bounds.Height;
				v[i].x = bounds.X + width / 2.0;
				v[i].y = bounds.Y + height / 2.0;
				if (approxNodeDimensions)
				{
					v[i].radiusSquared = Math.Min(width, height);
					v[i].radiusSquared *= v[i].radiusSquared;
				}
				else
				{
					v[i].radiusSquared = width * width;
					v[i].heightSquared = height * height;
				}
			}

			// Form internal model of edges
			e = new CellWrapper[edges.Length];
			mxIGraphModel model = graph.Model;
			for (int i = 0; i < e.Length; i++)
			{
				e[i] = new CellWrapper(this, edges[i]);

				object sourceCell = model.getTerminal(edges[i], true);
				object targetCell = model.getTerminal(edges[i], false);
				int? source = null;
				int? target = null;
				// Check if either end of the edge is not connected
				if (sourceCell != null)
				{
					source = vertexMap[sourceCell];
				}
				if (targetCell != null)
				{
					target = vertexMap[targetCell];
				}
				if (source != null)
				{
					e[i].source = source.Value;
				}
				else
				{
					// source end is not connected
					e[i].source = -1;
				}
				if (target != null)
				{
					e[i].target = target.Value;
				}
				else
				{
					// target end is not connected
					e[i].target = -1;
				}
			}

			// Set up internal nodes with information about whether edges
			// are connected to them or not
			for (int i = 0; i < v.Length; i++)
			{
				v[i].relevantEdges = getRelevantEdges(i);
				v[i].connectedEdges = getConnectedEdges(i);
			}

			// Setup the normal vectors for the test points to move each vertex to
			xNormTry = new double[triesPerCell];
			yNormTry = new double[triesPerCell];

			for (int i = 0; i < triesPerCell; i++)
			{
				double angle = i * ((2.0 * Math.PI) / triesPerCell);
				xNormTry[i] = Math.Cos(angle);
				yNormTry[i] = Math.Sin(angle);
			}

			// The main layout loop
			for (iteration = 0; iteration < maxIterations; iteration++)
			{
				performRound();
			}

			// Obtain the final positions post them to the facade
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] result = new double[v.Length][2];
			double[][] result = RectangularArrays.ReturnRectangularDoubleArray(v.Length, 2);
			for (int i = 0; i < v.Length; i++)
			{
				vertices[i] = v[i].cell;
				bounds = graph.getCellBounds(vertices[i]);
				// Convert from vertex center points to top left points
				result[i][0] = v[i].x - bounds.Width / 2;
				result[i][1] = v[i].y - bounds.Height / 2;
			}

			model.beginUpdate();
			try
			{
				for (int i = 0; i < vertices.Length; i++)
				{
					setVertexLocation(vertices[i], result[i][0], result[i][1]);
				}
			}
			finally
			{
				model.endUpdate();
			}
		}

		/// <summary>
		/// The main round of the algorithm. Firstly, a permutation of nodes
		/// is created and worked through in that random order. Then, for each node
		/// a number of point of a circle of radius <code>moveRadius</code> are
		/// selected and the total energy of the system calculated if that node
		/// were moved to that new position. If a lower energy position is found
		/// this is accepted and the algorithm moves onto the next node. There
		/// may be a slightly lower energy value yet to be found, but forcing
		/// the loop to check all possible positions adds nearly the current
		/// processing time again, and for little benefit. Another possible
		/// strategy would be to take account of the fact that the energy values
		/// around the circle decrease for half the loop and increase for the
		/// other, as a general rule. If part of the decrease were seen, then
		/// when the energy of a node increased, the previous node position was
		/// almost always the lowest energy position. This adds about two loop
		/// iterations to the inner loop and only makes sense with 16 tries or more.
		/// </summary>
		protected internal virtual void performRound()
		{
			// sequential order cells are computed (every round the same order)

			// boolean to keep track of whether any moves were made in this round
			bool energyHasChanged = false;
			for (int i = 0; i < v.Length; i++)
			{
				int index = i;

				// Obtain the energies for the node is its current position
				double oldNodeDistribution = getNodeDistribution(index);
				double oldEdgeDistance = getEdgeDistanceFromNode(index);
				oldEdgeDistance += getEdgeDistanceAffectedNodes(index);
				double oldEdgeCrossing = getEdgeCrossingAffectedEdges(index);
				double oldBorderLine = getBorderline(index);
				double oldEdgeLength = getEdgeLengthAffectedEdges(index);
				double oldAdditionFactors = getAdditionFactorsEnergy(index);

				for (int j = 0; j < triesPerCell; j++)
				{
					double movex = moveRadius * xNormTry[j];
					double movey = moveRadius * yNormTry[j];

					// applying new move
					double oldx = v[index].x;
					double oldy = v[index].y;
					v[index].x = v[index].x + movex;
					v[index].y = v[index].y + movey;

					// calculate the energy delta from this move
					double energyDelta = calcEnergyDelta(index, oldNodeDistribution, oldEdgeDistance, oldEdgeCrossing, oldBorderLine, oldEdgeLength, oldAdditionFactors);

					if (energyDelta < 0)
					{
						// energy of moved node is lower, finish tries for this
						// node
						energyHasChanged = true;
						break; // exits loop
					}
					else
					{
						// Revert node coordinates
						v[index].x = oldx;
						v[index].y = oldy;
					}
				}
			}
			// Check if we've hit the limit number of unchanged rounds that cause
			// a termination condition
			if (energyHasChanged)
			{
				unchangedEnergyRoundCount = 0;
			}
			else
			{
				unchangedEnergyRoundCount++;
				// Half the move radius in case assuming it's set too high for
				// what might be an optimisation case
				moveRadius /= 2.0;
			}
			if (unchangedEnergyRoundCount >= unchangedEnergyRoundTermination)
			{
				iteration = maxIterations;
			}

			// decrement radius in controlled manner
			double newMoveRadius = moveRadius * radiusScaleFactor;
			// Don't waste time on tiny decrements, if the final pixel resolution
			// is 50 then there's no point doing 55,54.1, 53.2 etc
			if (moveRadius - newMoveRadius < minMoveRadius)
			{
				newMoveRadius = moveRadius - minMoveRadius;
			}
			// If the temperature reaches its minimum temperature then finish
			if (newMoveRadius <= minMoveRadius)
			{
				iteration = maxIterations;
			}
			// Switch on fine tuning below the specified temperature
			if (newMoveRadius < fineTuningRadius)
			{
				isFineTuning = true;
			}

			moveRadius = newMoveRadius;

		}

		/// <summary>
		/// Calculates the change in energy for the specified node. The new energy is
		/// calculated from the cost function methods and the old energy values for
		/// each cost function are passed in as parameters
		/// </summary>
		/// <param name="index">
		///            The index of the node in the <code>vertices</code> array </param>
		/// <param name="oldNodeDistribution">
		///            The previous node distribution energy cost of this node </param>
		/// <param name="oldEdgeDistance">
		///            The previous edge distance energy cost of this node </param>
		/// <param name="oldEdgeCrossing">
		///            The previous edge crossing energy cost for edges connected to
		///            this node </param>
		/// <param name="oldBorderLine">
		///            The previous border line energy cost for this node </param>
		/// <param name="oldEdgeLength">
		///            The previous edge length energy cost for edges connected to
		///            this node </param>
		/// <param name="oldAdditionalFactorsEnergy">
		///            The previous energy cost for additional factors from
		///            sub-classes
		/// </param>
		/// <returns> the delta of the new energy cost to the old energy cost
		///  </returns>
		protected internal virtual double calcEnergyDelta(int index, double oldNodeDistribution, double oldEdgeDistance, double oldEdgeCrossing, double oldBorderLine, double oldEdgeLength, double oldAdditionalFactorsEnergy)
		{
			double energyDelta = 0.0;
			energyDelta += getNodeDistribution(index) * 2.0;
			energyDelta -= oldNodeDistribution * 2.0;

			energyDelta += getBorderline(index);
			energyDelta -= oldBorderLine;

			energyDelta += getEdgeDistanceFromNode(index);
			energyDelta += getEdgeDistanceAffectedNodes(index);
			energyDelta -= oldEdgeDistance;

			energyDelta -= oldEdgeLength;
			energyDelta += getEdgeLengthAffectedEdges(index);

			energyDelta -= oldEdgeCrossing;
			energyDelta += getEdgeCrossingAffectedEdges(index);

			energyDelta -= oldAdditionalFactorsEnergy;
			energyDelta += getAdditionFactorsEnergy(index);

			return energyDelta;
		}

		/// <summary>
		/// Calculates the energy cost of the specified node relative to all other
		/// nodes. Basically produces a higher energy the closer nodes are together.
		/// </summary>
		/// <param name="i"> the index of the node in the array <code>v</code> </param>
		/// <returns> the total node distribution energy of the specified node  </returns>
		protected internal virtual double getNodeDistribution(int i)
		{
			double energy = 0.0;

			// This check is placed outside of the inner loop for speed, even
			// though the code then has to be duplicated
			if (isOptimizeNodeDistribution == true)
			{
				if (approxNodeDimensions)
				{
					for (int j = 0; j < v.Length; j++)
					{
						if (i != j)
						{
							double vx = v[i].x - v[j].x;
							double vy = v[i].y - v[j].y;
							double distanceSquared = vx * vx + vy * vy;
							distanceSquared -= v[i].radiusSquared;
							distanceSquared -= v[j].radiusSquared;

							// prevents from dividing with Zero.
							if (distanceSquared < minDistanceLimitSquared)
							{
								distanceSquared = minDistanceLimitSquared;
							}

							energy += nodeDistributionCostFactor / distanceSquared;
						}
					}
				}
				else
				{
					for (int j = 0; j < v.Length; j++)
					{
						if (i != j)
						{
							double vx = v[i].x - v[j].x;
							double vy = v[i].y - v[j].y;
							double distanceSquared = vx * vx + vy * vy;
							distanceSquared -= v[i].radiusSquared;
							distanceSquared -= v[j].radiusSquared;
							// If the height separation indicates overlap, subtract
							// the widths from the distance. Same for width overlap
							// TODO						if ()

							// prevents from dividing with Zero.
							if (distanceSquared < minDistanceLimitSquared)
							{
								distanceSquared = minDistanceLimitSquared;
							}

							energy += nodeDistributionCostFactor / distanceSquared;
						}
					}
				}
			}
			return energy;
		}

		/// <summary>
		/// This method calculates the energy of the distance of the specified
		/// node to the notional border of the graph. The energy increases up to
		/// a limited maximum close to the border and stays at that maximum
		/// up to and over the border.
		/// </summary>
		/// <param name="i"> the index of the node in the array <code>v</code> </param>
		/// <returns> the total border line energy of the specified node  </returns>
		protected internal virtual double getBorderline(int i)
		{
			double energy = 0.0;
			if (isOptimizeBorderLine)
			{
				// Avoid very small distances and convert negative distance (i.e
				// outside the border to small positive ones )
				double l = v[i].x - boundsX;
				if (l < minDistanceLimit)
				{
					l = minDistanceLimit;
				}
				double t = v[i].y - boundsY;
				if (t < minDistanceLimit)
				{
					t = minDistanceLimit;
				}
				double r = boundsX + boundsWidth - v[i].x;
				if (r < minDistanceLimit)
				{
					r = minDistanceLimit;
				}
				double b = boundsY + boundsHeight - v[i].y;
				if (b < minDistanceLimit)
				{
					b = minDistanceLimit;
				}
				energy += borderLineCostFactor * ((1000000.0 / (t * t)) + (1000000.0 / (l * l)) + (1000000.0 / (b * b)) + (1000000.0 / (r * r)));
			}
			return energy;
		}

		/// <summary>
		/// Obtains the energy cost function for the specified node being moved.
		/// This involves calling <code>getEdgeLength</code> for all
		/// edges connected to the specified node </summary>
		/// <param name="node">
		/// 				the node whose connected edges cost functions are to be
		/// 				calculated </param>
		/// <returns> the total edge length energy of the connected edges  </returns>
		protected internal virtual double getEdgeLengthAffectedEdges(int node)
		{
			double energy = 0.0;
			for (int i = 0; i < v[node].connectedEdges.Length; i++)
			{
				energy += getEdgeLength(v[node].connectedEdges[i]);
			}
			return energy;
		}

		/// <summary>
		/// This method calculates the energy due to the length of the specified
		/// edge. The energy is proportional to the length of the edge, making
		/// shorter edges preferable in the layout.
		/// </summary>
		/// <param name="i"> the index of the edge in the array <code>e</code> </param>
		/// <returns> the total edge length energy of the specified edge  </returns>
		protected internal virtual double getEdgeLength(int i)
		{
			if (isOptimizeEdgeLength)
			{
                //double edgeLength = Point2D.distance(v[e[i].source].x, v[e[i].source].y, v[e[i].target].x, v[e[i].target].y);
                double edgeLength = PointHelper.Distance(v[e[i].source].x, v[e[i].source].y, v[e[i].target].x, v[e[i].target].y);
                return (edgeLengthCostFactor * edgeLength * edgeLength);
			}
			else
			{
				return 0.0;
			}
		}

		/// <summary>
		/// Obtains the energy cost function for the specified node being moved.
		/// This involves calling <code>getEdgeCrossing</code> for all
		/// edges connected to the specified node </summary>
		/// <param name="node">
		/// 				the node whose connected edges cost functions are to be
		/// 				calculated </param>
		/// <returns> the total edge crossing energy of the connected edges  </returns>
		protected internal virtual double getEdgeCrossingAffectedEdges(int node)
		{
			double energy = 0.0;
			for (int i = 0; i < v[node].connectedEdges.Length; i++)
			{
				energy += getEdgeCrossing(v[node].connectedEdges[i]);
			}

			return energy;
		}

		/// <summary>
		/// This method calculates the energy of the distance from the specified
		/// edge crossing any other edges. Each crossing add a constant factor
		/// to the total energy
		/// </summary>
		/// <param name="i"> the index of the edge in the array <code>e</code> </param>
		/// <returns> the total edge crossing energy of the specified edge  </returns>
		protected internal virtual double getEdgeCrossing(int i)
		{
			// TODO Could have a cost function per edge
			int n = 0; // counts energy of edgecrossings through edge i

			// max and min variable for minimum bounding rectangles overlapping
			// checks
			double minjX, minjY, miniX, miniY, maxjX, maxjY, maxiX, maxiY;

			if (isOptimizeEdgeCrossing)
			{
				double iP1X = v[e[i].source].x;
				double iP1Y = v[e[i].source].y;
				double iP2X = v[e[i].target].x;
				double iP2Y = v[e[i].target].y;

				for (int j = 0; j < e.Length; j++)
				{
					double jP1X = v[e[j].source].x;
					double jP1Y = v[e[j].source].y;
					double jP2X = v[e[j].target].x;
					double jP2Y = v[e[j].target].y;
					if (j != i)
					{
						// First check is to see if the minimum bounding rectangles
						// of the edges overlap at all. Since the layout tries
						// to separate nodes and shorten edges, the majority do not
						// overlap and this is a cheap way to avoid most of the
						// processing
						// Some long code to avoid a Math.max call...
						if (iP1X < iP2X)
						{
							miniX = iP1X;
							maxiX = iP2X;
						}
						else
						{
							miniX = iP2X;
							maxiX = iP1X;
						}
						if (jP1X < jP2X)
						{
							minjX = jP1X;
							maxjX = jP2X;
						}
						else
						{
							minjX = jP2X;
							maxjX = jP1X;
						}
						if (maxiX < minjX || miniX > maxjX)
						{
							continue;
						}

						if (iP1Y < iP2Y)
						{
							miniY = iP1Y;
							maxiY = iP2Y;
						}
						else
						{
							miniY = iP2Y;
							maxiY = iP1Y;
						}
						if (jP1Y < jP2Y)
						{
							minjY = jP1Y;
							maxjY = jP2Y;
						}
						else
						{
							minjY = jP2Y;
							maxjY = jP1Y;
						}
						if (maxiY < minjY || miniY > maxjY)
						{
							continue;
						}

						// Ignore if any end points are coincident
						if (((iP1X != jP1X) && (iP1Y != jP1Y)) && ((iP1X != jP2X) && (iP1Y != jP2Y)) && ((iP2X != jP1X) && (iP2Y != jP1Y)) && ((iP2X != jP2X) && (iP2Y != jP2Y)))
						{
                            // Values of zero returned from Line2D.relativeCCW are
                            // ignored because the point being exactly on the line
                            // is very rare for double and we've already checked if
                            // any end point share the same vertex. Should zero
                            // ever be returned, it would be the vertex connected
                            // to the edge that's actually on the edge and this is
                            // dealt with by the node to edge distance cost
                            // function. The worst case is that the vertex is
                            // pushed off the edge faster than it would be
                            // otherwise. Because of ignoring the zero this code
                            // below can behave like only a 1 or -1 will be
                            // returned. See Lines2D.linesIntersects().
                            bool intersects = ((PointHelper.RelativeCCW(iP1X, iP1Y, iP2X, iP2Y, jP1X, jP1Y) != PointHelper.RelativeCCW(iP1X, iP1Y, iP2X, iP2Y, jP2X, jP2Y)) && (PointHelper.RelativeCCW(jP1X, jP1Y, jP2X, jP2Y, iP1X, iP1Y) != PointHelper.RelativeCCW(jP1X, jP1Y, jP2X, jP2Y, iP2X, iP2Y)));

							if (intersects)
							{
								n++;
							}
						}
					}
				}
			}
			return edgeCrossingCostFactor * n;
		}

		/// <summary>
		/// This method calculates the energy of the distance between Cells and
		/// Edges. This version of the edge distance cost calculates the energy
		/// cost from a specified <strong>node</strong>. The distance cost to all
		/// unconnected edges is calculated and the total returned.
		/// </summary>
		/// <param name="i"> the index of the node in the array <code>v</code> </param>
		/// <returns> the total edge distance energy of the node </returns>
		protected internal virtual double getEdgeDistanceFromNode(int i)
		{
			double energy = 0.0;
			// This function is only performed during fine tuning for performance
			if (isOptimizeEdgeDistance && isFineTuning)
			{
				int[] edges = v[i].relevantEdges;
				for (int j = 0; j < edges.Length; j++)
				{
					// Note that the distance value is squared
					double distSquare = PointHelper.ptSegDistSq(v[e[edges[j]].source].x, v[e[edges[j]].source].y, v[e[edges[j]].target].x, v[e[edges[j]].target].y, v[i].x, v[i].y);

					distSquare -= v[i].radiusSquared;

					// prevents from dividing with Zero. No Math.abs() call
					// for performance
					if (distSquare < minDistanceLimitSquared)
					{
						distSquare = minDistanceLimitSquared;
					}

					// Only bother with the divide if the node and edge are
					// fairly close together
					if (distSquare < maxDistanceLimitSquared)
					{
						energy += edgeDistanceCostFactor / distSquare;
					}
				}
			}
			return energy;
		}

		/// <summary>
		/// Obtains the energy cost function for the specified node being moved.
		/// This involves calling <code>getEdgeDistanceFromEdge</code> for all
		/// edges connected to the specified node </summary>
		/// <param name="node">
		/// 				the node whose connected edges cost functions are to be
		/// 				calculated </param>
		/// <returns> the total edge distance energy of the connected edges  </returns>
		protected internal virtual double getEdgeDistanceAffectedNodes(int node)
		{
			double energy = 0.0;
			for (int i = 0; i < v[node].connectedEdges.Length; i++)
			{
				energy += getEdgeDistanceFromEdge(v[node].connectedEdges[i]);
			}

			return energy;
		}

		/// <summary>
		/// This method calculates the energy of the distance between Cells and
		/// Edges. This version of the edge distance cost calculates the energy
		/// cost from a specified <strong>edge</strong>. The distance cost to all
		/// unconnected nodes is calculated and the total returned.
		/// </summary>
		/// <param name="i"> the index of the edge in the array <code>e</code> </param>
		/// <returns> the total edge distance energy of the edge </returns>
		protected internal virtual double getEdgeDistanceFromEdge(int i)
		{
			double energy = 0.0;
			// This function is only performed during fine tuning for performance
			if (isOptimizeEdgeDistance && isFineTuning)
			{
				for (int j = 0; j < v.Length; j++)
				{
					// Don't calculate for connected nodes
					if (e[i].source != j && e[i].target != j)
					{
						double distSquare = PointHelper.ptSegDistSq(v[e[i].source].x, v[e[i].source].y, v[e[i].target].x, v[e[i].target].y, v[j].x, v[j].y);

						distSquare -= v[j].radiusSquared;

						// prevents from dividing with Zero. No Math.abs() call
						// for performance
						if (distSquare < minDistanceLimitSquared)
						{
							distSquare = minDistanceLimitSquared;
						}

						// Only bother with the divide if the node and edge are
						// fairly close together
						if (distSquare < maxDistanceLimitSquared)
						{
							energy += edgeDistanceCostFactor / distSquare;
						}
					}
				}
			}
			return energy;
		}

		/// <summary>
		/// Hook method to adding additional energy factors into the layout.
		/// Calculates the energy just for the specified node. </summary>
		/// <param name="i"> the nodes whose energy is being calculated </param>
		/// <returns> the energy of this node caused by the additional factors </returns>
		protected internal virtual double getAdditionFactorsEnergy(int i)
		{
			return 0.0;
		}

		/// <summary>
		/// Returns all Edges that are not connected to the specifed cell
		/// </summary>
		/// <param name="cellIndex">
		///            the cell index to which the edges are not connected </param>
		/// <returns> Array of all interesting Edges </returns>
		protected internal virtual int[] getRelevantEdges(int cellIndex)
		{
			List<int?> relevantEdgeList = new List<int?>(e.Length);

			for (int i = 0; i < e.Length; i++)
			{
				if (e[i].source != cellIndex && e[i].target != cellIndex)
				{
					// Add non-connected edges
					relevantEdgeList.Add(new int?(i));
				}
			}

			int[] relevantEdgeArray = new int[relevantEdgeList.Count];
			IEnumerator<int?> iter = relevantEdgeList.GetEnumerator();

			//Reform the list into an array but replace Integer values with ints
			for (int i = 0; i < relevantEdgeArray.Length; i++)
			{
                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                if (iter.MoveNext())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					relevantEdgeArray[i] = iter.Current.Value;
				}
			}

			return relevantEdgeArray;
		}

		/// <summary>
		/// Returns all Edges that are connected with the specified cell
		/// </summary>
		/// <param name="cellIndex">
		///            the cell index to which the edges are connected </param>
		/// <returns> Array of all connected Edges </returns>
		protected internal virtual int[] getConnectedEdges(int cellIndex)
		{
			List<int?> connectedEdgeList = new List<int?>(e.Length);

			for (int i = 0; i < e.Length; i++)
			{
				if (e[i].source == cellIndex || e[i].target == cellIndex)
				{
					// Add connected edges to list by their index number
					connectedEdgeList.Add(new int?(i));
				}
			}

			int[] connectedEdgeArray = new int[connectedEdgeList.Count];
			IEnumerator<int?> iter = connectedEdgeList.GetEnumerator();

			// Reform the list into an array but replace Integer values with ints
			for (int i = 0; i < connectedEdgeArray.Length; i++)
			{
                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                if (iter.MoveNext())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					connectedEdgeArray[i] = iter.Current.Value;
					;
				}
			}

			return connectedEdgeArray;
		}

		/// <summary>
		/// Returns <code>Organic</code>, the name of this algorithm.
		/// </summary>
		public override string ToString()
		{
			return "Organic";
		}

		/// <summary>
		/// Internal representation of a node or edge that holds cached information
		/// to enable the layout to perform more quickly and to simplify the code
		/// </summary>
		public class CellWrapper
		{
			private readonly mxOrganicLayout outerInstance;


			/// <summary>
			/// The actual graph cell this wrapper represents
			/// </summary>
			protected internal object cell;

			/// <summary>
			/// All edge that repel this cell, only used for nodes. This array
			/// is equivalent to all edges unconnected to this node
			/// </summary>
			protected internal int[] relevantEdges = null;

			/// <summary>
			/// the index of all connected edges in the <code>e</code> array
			/// to this node. This is only used for nodes.
			/// </summary>
			protected internal int[] connectedEdges = null;

			/// <summary>
			/// The x-coordinate position of this cell, nodes only
			/// </summary>
			protected internal double x;

			/// <summary>
			/// The y-coordinate position of this cell, nodes only
			/// </summary>
			protected internal double y;

			/// <summary>
			/// The approximate radius squared of this cell, nodes only. If
			/// approxNodeDimensions is true on the layout this value holds the
			/// width of the node squared
			/// </summary>
			protected internal double radiusSquared;

			/// <summary>
			/// The height of the node squared, only used if approxNodeDimensions
			/// is set to true.
			/// </summary>
			protected internal double heightSquared;

			/// <summary>
			/// The index of the node attached to this edge as source, edges only
			/// </summary>
			protected internal int source;

			/// <summary>
			/// The index of the node attached to this edge as target, edges only
			/// </summary>
			protected internal int target;

			/// <summary>
			/// Constructs a new CellWrapper </summary>
			/// <param name="cell"> the graph cell this wrapper represents </param>
			public CellWrapper(mxOrganicLayout outerInstance, object cell)
			{
				this.outerInstance = outerInstance;
				this.cell = cell;
			}

			/// <returns> the relevantEdges </returns>
			public virtual int[] RelevantEdges
			{
				get
				{
					return relevantEdges;
				}
				set
				{
					this.relevantEdges = value;
				}
			}


			/// <returns> the connectedEdges </returns>
			public virtual int[] ConnectedEdges
			{
				get
				{
					return connectedEdges;
				}
				set
				{
					this.connectedEdges = value;
				}
			}


			/// <returns> the x </returns>
			public virtual double X
			{
				get
				{
					return x;
				}
				set
				{
					this.x = value;
				}
			}


			/// <returns> the y </returns>
			public virtual double Y
			{
				get
				{
					return y;
				}
				set
				{
					this.y = value;
				}
			}


			/// <returns> the radiusSquared </returns>
			public virtual double RadiusSquared
			{
				get
				{
					return radiusSquared;
				}
				set
				{
					this.radiusSquared = value;
				}
			}


			/// <returns> the heightSquared </returns>
			public virtual double HeightSquared
			{
				get
				{
					return heightSquared;
				}
				set
				{
					this.heightSquared = value;
				}
			}


			/// <returns> the source </returns>
			public virtual int Source
			{
				get
				{
					return source;
				}
				set
				{
					this.source = value;
				}
			}


			/// <returns> the target </returns>
			public virtual int Target
			{
				get
				{
					return target;
				}
				set
				{
					this.target = value;
				}
			}


			/// <returns> the cell </returns>
			public virtual object Cell
			{
				get
				{
					return cell;
				}
			}
		}

		/// <returns> Returns the averageNodeArea. </returns>
		public virtual double AverageNodeArea
		{
			get
			{
				return averageNodeArea;
			}
			set
			{
				this.averageNodeArea = value;
			}
		}


		/// <returns> Returns the borderLineCostFactor. </returns>
		public virtual double BorderLineCostFactor
		{
			get
			{
				return borderLineCostFactor;
			}
			set
			{
				this.borderLineCostFactor = value;
			}
		}


		/// <returns> Returns the edgeCrossingCostFactor. </returns>
		public virtual double EdgeCrossingCostFactor
		{
			get
			{
				return edgeCrossingCostFactor;
			}
			set
			{
				this.edgeCrossingCostFactor = value;
			}
		}


		/// <returns> Returns the edgeDistanceCostFactor. </returns>
		public virtual double EdgeDistanceCostFactor
		{
			get
			{
				return edgeDistanceCostFactor;
			}
			set
			{
				this.edgeDistanceCostFactor = value;
			}
		}


		/// <returns> Returns the edgeLengthCostFactor. </returns>
		public virtual double EdgeLengthCostFactor
		{
			get
			{
				return edgeLengthCostFactor;
			}
			set
			{
				this.edgeLengthCostFactor = value;
			}
		}


		/// <returns> Returns the fineTuningRadius. </returns>
		public virtual double FineTuningRadius
		{
			get
			{
				return fineTuningRadius;
			}
			set
			{
				this.fineTuningRadius = value;
			}
		}


		/// <returns> Returns the initialMoveRadius. </returns>
		public virtual double InitialMoveRadius
		{
			get
			{
				return initialMoveRadius;
			}
			set
			{
				this.initialMoveRadius = value;
			}
		}


		/// <returns> Returns the isFineTuning. </returns>
		public virtual bool FineTuning
		{
			get
			{
				return isFineTuning;
			}
			set
			{
				this.isFineTuning = value;
			}
		}


		/// <returns> Returns the isOptimizeBorderLine. </returns>
		public virtual bool OptimizeBorderLine
		{
			get
			{
				return isOptimizeBorderLine;
			}
			set
			{
				this.isOptimizeBorderLine = value;
			}
		}


		/// <returns> Returns the isOptimizeEdgeCrossing. </returns>
		public virtual bool OptimizeEdgeCrossing
		{
			get
			{
				return isOptimizeEdgeCrossing;
			}
			set
			{
				this.isOptimizeEdgeCrossing = value;
			}
		}


		/// <returns> Returns the isOptimizeEdgeDistance. </returns>
		public virtual bool OptimizeEdgeDistance
		{
			get
			{
				return isOptimizeEdgeDistance;
			}
			set
			{
				this.isOptimizeEdgeDistance = value;
			}
		}


		/// <returns> Returns the isOptimizeEdgeLength. </returns>
		public virtual bool OptimizeEdgeLength
		{
			get
			{
				return isOptimizeEdgeLength;
			}
			set
			{
				this.isOptimizeEdgeLength = value;
			}
		}


		/// <returns> Returns the isOptimizeNodeDistribution. </returns>
		public virtual bool OptimizeNodeDistribution
		{
			get
			{
				return isOptimizeNodeDistribution;
			}
			set
			{
				this.isOptimizeNodeDistribution = value;
			}
		}


		/// <returns> Returns the maxIterations. </returns>
		public virtual int MaxIterations
		{
			get
			{
				return maxIterations;
			}
			set
			{
				this.maxIterations = value;
			}
		}


		/// <returns> Returns the minDistanceLimit. </returns>
		public virtual double MinDistanceLimit
		{
			get
			{
				return minDistanceLimit;
			}
			set
			{
				this.minDistanceLimit = value;
			}
		}


		/// <returns> Returns the minMoveRadius. </returns>
		public virtual double MinMoveRadius
		{
			get
			{
				return minMoveRadius;
			}
			set
			{
				this.minMoveRadius = value;
			}
		}


		/// <returns> Returns the nodeDistributionCostFactor. </returns>
		public virtual double NodeDistributionCostFactor
		{
			get
			{
				return nodeDistributionCostFactor;
			}
			set
			{
				this.nodeDistributionCostFactor = value;
			}
		}


		/// <returns> Returns the radiusScaleFactor. </returns>
		public virtual double RadiusScaleFactor
		{
			get
			{
				return radiusScaleFactor;
			}
			set
			{
				this.radiusScaleFactor = value;
			}
		}


		/// <returns> Returns the triesPerCell. </returns>
		public virtual int TriesPerCell
		{
			get
			{
				return triesPerCell;
			}
			set
			{
				this.triesPerCell = value;
			}
		}


		/// <returns> Returns the unchangedEnergyRoundTermination. </returns>
		public virtual int UnchangedEnergyRoundTermination
		{
			get
			{
				return unchangedEnergyRoundTermination;
			}
			set
			{
				this.unchangedEnergyRoundTermination = value;
			}
		}


		/// <returns> Returns the maxDistanceLimit. </returns>
		public virtual double MaxDistanceLimit
		{
			get
			{
				return maxDistanceLimit;
			}
			set
			{
				this.maxDistanceLimit = value;
			}
		}


		/// <returns> the approxNodeDimensions </returns>
		public virtual bool ApproxNodeDimensions
		{
			get
			{
				return approxNodeDimensions;
			}
			set
			{
				this.approxNodeDimensions = value;
			}
		}

	}

}