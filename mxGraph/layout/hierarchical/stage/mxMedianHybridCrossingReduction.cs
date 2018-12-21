using mxGraph;
using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2005-2009, JGraph Ltd
 *
 * All rights reserved.
 *
 * This file is licensed under the JGraph software license, a copy of which
 * will have been provided to you in the file LICENSE at the root of your
 * installation directory. If you are unable to locate this file please
 * contact JGraph sales for another copy.
 */

namespace mxGraph.layout.hierarchical.stage
{


	using mxGraphAbstractHierarchyCell = mxGraph.layout.hierarchical.model.mxGraphAbstractHierarchyCell;
	using mxGraphHierarchyModel = mxGraph.layout.hierarchical.model.mxGraphHierarchyModel;
	using mxGraphHierarchyRank = mxGraph.layout.hierarchical.model.mxGraphHierarchyRank;

	/// <summary>
	/// Performs a vertex ordering within ranks as described by Gansner et al 1993
	/// </summary>
	public class mxMedianHybridCrossingReduction : mxHierarchicalLayoutStage //, JGraphLayout.Stoppable
	{

		/// <summary>
		/// Reference to the enclosing layout algorithm
		/// </summary>
		protected internal mxHierarchicalLayout layout;

		/// <summary>
		/// The maximum number of iterations to perform whilst reducing edge
		/// crossings
		/// </summary>
		protected internal int maxIterations = 24;

		/// <summary>
		/// Stores each rank as a collection of cells in the best order found for
		/// each layer so far
		/// </summary>
		protected internal mxGraphAbstractHierarchyCell[][] nestedBestRanks = null;

		/// <summary>
		/// The total number of crossings found in the best configuration so far
		/// </summary>
		protected internal int currentBestCrossings = 0;

		protected internal int iterationsWithoutImprovement = 0;

		protected internal int maxNoImprovementIterations = 2;

		/// <summary>
		/// Constructor that has the roots specified
		/// </summary>
		public mxMedianHybridCrossingReduction(mxHierarchicalLayout layout)
		{
			this.layout = layout;
		}

		/// <summary>
		/// Performs a vertex ordering within ranks as described by Gansner et al
		/// 1993
		/// </summary>
		public virtual void execute(object parent)
		{
			mxGraphHierarchyModel model = layout.Model;

			// Stores initial ordering as being the best one found so far
			nestedBestRanks = new mxGraphAbstractHierarchyCell[model.ranks.Count][];

			for (int i = 0; i < nestedBestRanks.Length; i++)
			{
				mxGraphHierarchyRank rank = model.ranks[new int?(i)];
				//nestedBestRanks[i] = new mxGraphAbstractHierarchyCell[rank.Count];
                nestedBestRanks[i]=rank.ToArray();
			}

			iterationsWithoutImprovement = 0;
			currentBestCrossings = calculateCrossings(model);

			for (int i = 0; i < maxIterations && iterationsWithoutImprovement < maxNoImprovementIterations; i++)
			{
				weightedMedian(i, model);
				transpose(i, model);
				int candidateCrossings = calculateCrossings(model);

				if (candidateCrossings < currentBestCrossings)
				{
					currentBestCrossings = candidateCrossings;
					iterationsWithoutImprovement = 0;

					// Store the current rankings as the best ones
					for (int j = 0; j < nestedBestRanks.Length; j++)
					{
						mxGraphHierarchyRank rank = model.ranks[new int?(j)];
						IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

						for (int k = 0; k < rank.Count; k++)
						{
                            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                            iter.MoveNext();
							mxGraphAbstractHierarchyCell cell = iter.Current;
							nestedBestRanks[j][cell.getGeneralPurposeVariable(j)] = cell;
						}
					}
				}
				else
				{
					// Increase count of iterations where we haven't improved the
					// layout
					iterationsWithoutImprovement++;

					// Restore the best values to the cells
					for (int j = 0; j < nestedBestRanks.Length; j++)
					{
						mxGraphHierarchyRank rank = model.ranks[new int?(j)];
						IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

						for (int k = 0; k < rank.Count; k++)
						{
                            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                            iter.MoveNext();
							mxGraphAbstractHierarchyCell cell = iter.Current;
							cell.setGeneralPurposeVariable(j, k);
						}
					}
				}

				if (currentBestCrossings == 0)
				{
					// Do nothing further
					break;
				}
			}

			// Store the best rankings but in the model
			IDictionary<int?, mxGraphHierarchyRank> ranks = new LinkedHashMap<int?, mxGraphHierarchyRank>(model.maxRank + 1);
			mxGraphHierarchyRank[] rankList = new mxGraphHierarchyRank[model.maxRank + 1];

			for (int i = 0; i < model.maxRank + 1; i++)
			{
				rankList[i] = new mxGraphHierarchyRank();
				ranks[new int?(i)] = rankList[i];
			}

			for (int i = 0; i < nestedBestRanks.Length; i++)
			{
				for (int j = 0; j < nestedBestRanks[i].Length; j++)
				{
                    rankList[i].Add(nestedBestRanks[i][j]);
				}
			}

			model.ranks = ranks;
		}

		/// <summary>
		/// Calculates the total number of edge crossing in the current graph
		/// </summary>
		/// <param name="model">
		///            the internal model describing the hierarchy </param>
		/// <returns> the current number of edge crossings in the hierarchy graph model
		///         in the current candidate layout </returns>
		private int calculateCrossings(mxGraphHierarchyModel model)
		{
			// The intra-rank order of cells are stored within the temp variables
			// on cells
			int numRanks = model.ranks.Count;
			int totalCrossings = 0;

			for (int i = 1; i < numRanks; i++)
			{
				totalCrossings += calculateRankCrossing(i, model);
			}

			return totalCrossings;
		}

		/// <summary>
		/// Calculates the number of edges crossings between the specified rank and
		/// the rank below it
		/// </summary>
		/// <param name="i">
		///            the topmost rank of the pair ( higher rank value ) </param>
		/// <param name="model">
		///            the internal hierarchy model of the graph </param>
		/// <returns> the number of edges crossings with the rank beneath </returns>
		protected internal virtual int calculateRankCrossing(int i, mxGraphHierarchyModel model)
		{
			int totalCrossings = 0;
			mxGraphHierarchyRank rank = model.ranks[new int?(i)];
			mxGraphHierarchyRank previousRank = model.ranks[new int?(i - 1)];

			// Create an array of connections between these two levels
			int currentRankSize = rank.Count;
			int previousRankSize = previousRank.Count;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] connections = new int[currentRankSize][previousRankSize];
			int[][] connections = RectangularArrays.ReturnRectangularIntArray(currentRankSize, previousRankSize);

			// Iterate over the top rank and fill in the connection information
			IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

			while (iter.MoveNext())
			{
				mxGraphAbstractHierarchyCell cell = iter.Current;
				int rankPosition = cell.getGeneralPurposeVariable(i);
				ICollection<mxGraphAbstractHierarchyCell> connectedCells = cell.getPreviousLayerConnectedCells(i);
				IEnumerator<mxGraphAbstractHierarchyCell> iter2 = connectedCells.GetEnumerator();

				while (iter2.MoveNext())
				{
					mxGraphAbstractHierarchyCell connectedCell = iter2.Current;
					int otherCellRankPosition = connectedCell.getGeneralPurposeVariable(i - 1);
					connections[rankPosition][otherCellRankPosition] = 201207;
				}
			}

			// Iterate through the connection matrix, crossing edges are
			// indicated by other connected edges with a greater rank position
			// on one rank and lower position on the other
			for (int j = 0; j < currentRankSize; j++)
			{
				for (int k = 0; k < previousRankSize; k++)
				{
					if (connections[j][k] == 201207)
					{
						// Draw a grid of connections, crossings are top right
						// and lower left from this crossing pair
						for (int j2 = j + 1; j2 < currentRankSize; j2++)
						{
							for (int k2 = 0; k2 < k; k2++)
							{
								if (connections[j2][k2] == 201207)
								{
									totalCrossings++;
								}
							}
						}

						for (int j2 = 0; j2 < j; j2++)
						{
							for (int k2 = k + 1; k2 < previousRankSize; k2++)
							{
								if (connections[j2][k2] == 201207)
								{
									totalCrossings++;
								}
							}
						}

					}
				}
			}

			return totalCrossings / 2;
		}

		/// <summary>
		/// Takes each possible adjacent cell pair on each rank and checks if
		/// swapping them around reduces the number of crossing
		/// </summary>
		/// <param name="mainLoopIteration">
		///            the iteration number of the main loop </param>
		/// <param name="model">
		///            the internal model describing the hierarchy </param>
		private void transpose(int mainLoopIteration, mxGraphHierarchyModel model)
		{
			bool improved = true;

			// Track the number of iterations in case of looping
			int count = 0;
			int maxCount = 10;

			while (improved && count++ < maxCount)
			{
				// On certain iterations allow allow swapping of cell pairs with
				// equal edge crossings switched or not switched. This help to
				// nudge a stuck layout into a lower crossing total.
				bool nudge = mainLoopIteration % 2 == 1 && count % 2 == 1;
				improved = false;

				for (int i = 0; i < model.ranks.Count; i++)
				{
					mxGraphHierarchyRank rank = model.ranks[new int?(i)];
					mxGraphAbstractHierarchyCell[] orderedCells = new mxGraphAbstractHierarchyCell[rank.Count];
					IEnumerator<mxGraphAbstractHierarchyCell> iter = rank.GetEnumerator();

					for (int j = 0; j < orderedCells.Length; j++)
					{
                        //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                        iter.MoveNext();

						mxGraphAbstractHierarchyCell cell = iter.Current;
						orderedCells[cell.getGeneralPurposeVariable(i)] = cell;
					}

					IList<mxGraphAbstractHierarchyCell> leftCellAboveConnections = null;
					IList<mxGraphAbstractHierarchyCell> leftCellBelowConnections = null;
					IList<mxGraphAbstractHierarchyCell> rightCellAboveConnections = null;
					IList<mxGraphAbstractHierarchyCell> rightCellBelowConnections = null;

					int[] leftAbovePositions = null;
					int[] leftBelowPositions = null;
					int[] rightAbovePositions = null;
					int[] rightBelowPositions = null;

					mxGraphAbstractHierarchyCell leftCell = null;
					mxGraphAbstractHierarchyCell rightCell = null;

					for (int j = 0; j < (rank.Count - 1); j++)
					{
						// For each intra-rank adjacent pair of cells
						// see if swapping them around would reduce the
						// number of edges crossing they cause in total
						// On every cell pair except the first on each rank, we
						// can save processing using the previous values for the
						// right cell on the new left cell
						if (j == 0)
						{
							leftCell = orderedCells[j];
							leftCellAboveConnections = leftCell.getNextLayerConnectedCells(i);
							leftCellBelowConnections = leftCell.getPreviousLayerConnectedCells(i);

							leftAbovePositions = new int[leftCellAboveConnections.Count];
							leftBelowPositions = new int[leftCellBelowConnections.Count];

							for (int k = 0; k < leftAbovePositions.Length; k++)
							{
								leftAbovePositions[k] = leftCellAboveConnections[k].getGeneralPurposeVariable(i + 1);
							}

							for (int k = 0; k < leftBelowPositions.Length; k++)
							{
								leftBelowPositions[k] = (leftCellBelowConnections[k]).getGeneralPurposeVariable(i - 1);
							}
						}
						else
						{
							leftCellAboveConnections = rightCellAboveConnections;
							leftCellBelowConnections = rightCellBelowConnections;
							leftAbovePositions = rightAbovePositions;
							leftBelowPositions = rightBelowPositions;
							leftCell = rightCell;
						}

						rightCell = orderedCells[j + 1];
						rightCellAboveConnections = rightCell.getNextLayerConnectedCells(i);
						rightCellBelowConnections = rightCell.getPreviousLayerConnectedCells(i);

						rightAbovePositions = new int[rightCellAboveConnections.Count];
						rightBelowPositions = new int[rightCellBelowConnections.Count];

						for (int k = 0; k < rightAbovePositions.Length; k++)
						{
							rightAbovePositions[k] = (rightCellAboveConnections[k]).getGeneralPurposeVariable(i + 1);
						}

						for (int k = 0; k < rightBelowPositions.Length; k++)
						{
							rightBelowPositions[k] = (rightCellBelowConnections[k]).getGeneralPurposeVariable(i - 1);
						}

						int totalCurrentCrossings = 0;
						int totalSwitchedCrossings = 0;

						for (int k = 0; k < leftAbovePositions.Length; k++)
						{
							for (int ik = 0; ik < rightAbovePositions.Length; ik++)
							{
								if (leftAbovePositions[k] > rightAbovePositions[ik])
								{
									totalCurrentCrossings++;
								}

								if (leftAbovePositions[k] < rightAbovePositions[ik])
								{
									totalSwitchedCrossings++;
								}
							}
						}

						for (int k = 0; k < leftBelowPositions.Length; k++)
						{
							for (int ik = 0; ik < rightBelowPositions.Length; ik++)
							{
								if (leftBelowPositions[k] > rightBelowPositions[ik])
								{
									totalCurrentCrossings++;
								}

								if (leftBelowPositions[k] < rightBelowPositions[ik])
								{
									totalSwitchedCrossings++;
								}
							}
						}

						if ((totalSwitchedCrossings < totalCurrentCrossings) || (totalSwitchedCrossings == totalCurrentCrossings && nudge))
						{
							int temp = leftCell.getGeneralPurposeVariable(i);
							leftCell.setGeneralPurposeVariable(i, rightCell.getGeneralPurposeVariable(i));
							rightCell.setGeneralPurposeVariable(i, temp);
							// With this pair exchanged we have to switch all of
							// values for the left cell to the right cell so the
							// next iteration for this rank uses it as the left
							// cell again
							rightCellAboveConnections = leftCellAboveConnections;
							rightCellBelowConnections = leftCellBelowConnections;
							rightAbovePositions = leftAbovePositions;
							rightBelowPositions = leftBelowPositions;
							rightCell = leftCell;

							if (!nudge)
							{
								// Don't count nudges as improvement or we'll end
								// up stuck in two combinations and not finishing
								// as early as we should
								improved = true;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Sweeps up or down the layout attempting to minimise the median placement
		/// of connected cells on adjacent ranks
		/// </summary>
		/// <param name="iteration">
		///            the iteration number of the main loop </param>
		/// <param name="model">
		///            the internal model describing the hierarchy </param>
		private void weightedMedian(int iteration, mxGraphHierarchyModel model)
		{
			// Reverse sweep direction each time through this method
			bool downwardSweep = (iteration % 2 == 0);

			if (downwardSweep)
			{
				for (int j = model.maxRank - 1; j >= 0; j--)
				{
					medianRank(j, downwardSweep);
				}
			}
			else
			{
				for (int j = 1; j < model.maxRank; j++)
				{
					medianRank(j, downwardSweep);
				}
			}
		}

		/// <summary>
		/// Attempts to minimise the median placement of connected cells on this rank
		/// and one of the adjacent ranks
		/// </summary>
		/// <param name="rankValue">
		///            the layer number of this rank </param>
		/// <param name="downwardSweep">
		///            whether or not this is a downward sweep through the graph </param>
		private void medianRank(int rankValue, bool downwardSweep)
		{
			int numCellsForRank = nestedBestRanks[rankValue].Length;
			MedianCellSorter[] medianValues = new MedianCellSorter[numCellsForRank];

			for (int i = 0; i < numCellsForRank; i++)
			{
				mxGraphAbstractHierarchyCell cell = nestedBestRanks[rankValue][i];
				medianValues[i] = new MedianCellSorter(this);
				medianValues[i].cell = cell;

				// Flip whether or not equal medians are flipped on up and down
				// sweeps
				medianValues[i].nudge = !downwardSweep;
				ICollection<mxGraphAbstractHierarchyCell> nextLevelConnectedCells;

				if (downwardSweep)
				{
					nextLevelConnectedCells = cell.getNextLayerConnectedCells(rankValue);
				}
				else
				{
					nextLevelConnectedCells = cell.getPreviousLayerConnectedCells(rankValue);
				}

				int nextRankValue;

				if (downwardSweep)
				{
					nextRankValue = rankValue + 1;
				}
				else
				{
					nextRankValue = rankValue - 1;
				}

				if (nextLevelConnectedCells != null && nextLevelConnectedCells.Count != 0)
				{
					medianValues[i].medianValue = medianValue(nextLevelConnectedCells, nextRankValue);
				}
				else
				{
					// Nodes with no adjacent vertices are given a median value of
					// -1 to indicate to the median function that they should be
					// left of their current position if possible.
					medianValues[i].medianValue = -1.0; // TODO needs to account for
					// both layers
				}
			}

             List<MedianCellSorter> tem=new List<MedianCellSorter>( medianValues);
            tem.Sort();
            medianValues = tem.ToArray();

            // Set the new position of each node within the rank using
            // its temp variable
            for (int i = 0; i < numCellsForRank; i++)
			{
				medianValues[i].cell.setGeneralPurposeVariable(rankValue, i);
			}
		}

		/// <summary>
		/// Calculates the median rank order positioning for the specified cell using
		/// the connected cells on the specified rank
		/// </summary>
		/// <param name="connectedCells">
		///            the cells on the specified rank connected to the specified
		///            cell </param>
		/// <param name="rankValue">
		///            the rank that the connected cell lie upon </param>
		/// <returns> the median rank ordering value of the connected cells </returns>
		private double medianValue(ICollection<mxGraphAbstractHierarchyCell> connectedCells, int rankValue)
		{
			double[] medianValues = new double[connectedCells.Count];
			int arrayCount = 0;
			IEnumerator<mxGraphAbstractHierarchyCell> iter = connectedCells.GetEnumerator();

			while (iter.MoveNext())
			{
				medianValues[arrayCount++] = (iter.Current).getGeneralPurposeVariable(rankValue);
			}

			//Arrays.sort(medianValues);

			if (arrayCount % 2 == 1)
			{
				// For odd numbers of adjacent vertices return the median
				return medianValues[arrayCount / 2];
			}
			else if (arrayCount == 2)
			{
				return ((medianValues[0] + medianValues[1]) / 2.0);
			}
			else
			{
				int medianPoint = arrayCount / 2;
				double leftMedian = medianValues[medianPoint - 1] - medianValues[0];
				double rightMedian = medianValues[arrayCount - 1] - medianValues[medianPoint];

				return (medianValues[medianPoint - 1] * rightMedian + medianValues[medianPoint] * leftMedian) / (leftMedian + rightMedian);
			}
		}

		/// <summary>
		/// A utility class used to track cells whilst sorting occurs on the median
		/// values. Does not violate (x.compareTo(y)==0) == (x.equals(y))
		/// </summary>
		protected internal class MedianCellSorter : IComparable<object>
		{
			private readonly mxMedianHybridCrossingReduction outerInstance;

			public MedianCellSorter(mxMedianHybridCrossingReduction outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			/// <summary>
			/// The median value of the cell stored
			/// </summary>
			public double medianValue = 0.0;

			/// <summary>
			/// Whether or not to flip equal median values.
			/// </summary>
			public bool nudge = false;

			/// <summary>
			/// The cell whose median value is being calculated
			/// </summary>
			internal mxGraphAbstractHierarchyCell cell = null;

			/// <summary>
			/// comparator on the medianValue
			/// </summary>
			/// <param name="arg0">
			///            the object to be compared to </param>
			/// <returns> the standard return you would expect when comparing two
			///         double </returns>
			public virtual int CompareTo(object arg0)
			{
				if (arg0 is MedianCellSorter)
				{
					if (medianValue < ((MedianCellSorter) arg0).medianValue)
					{
						return -1;
					}
					else if (medianValue > ((MedianCellSorter) arg0).medianValue)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					return 0;
				}
			}
		}
	}
}