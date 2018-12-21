using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 2005, David Benson
 *
 * All rights reserved.
 *
 * This file is licensed under the JGraph software license, a copy of which
 * will have been provided to you in the file LICENSE at the root of your
 * installation directory. If you are unable to locate this file please
 * contact JGraph sales for another copy.
 */
namespace mxGraph.layout.hierarchical.model
{


	/// <summary>
	/// An abstraction of a hierarchical edge for the hierarchy layout
	/// </summary>
	public class mxGraphHierarchyEdge : mxGraphAbstractHierarchyCell
	{

		/// <summary>
		/// The graph edge(s) this object represents. Parallel edges are all grouped
		/// together within one hierarchy edge.
		/// </summary>
		public IList<object> edges;

		/// <summary>
		/// The node this edge is sourced at
		/// </summary>
		public mxGraphHierarchyNode source;

		/// <summary>
		/// The node this edge targets
		/// </summary>
		public mxGraphHierarchyNode target;

		/// <summary>
		/// Whether or not the direction of this edge has been reversed
		/// internally to create a DAG for the hierarchical layout
		/// </summary>
		protected internal bool isReversed = false;

		/// <summary>
		/// Constructs a hierarchy edge </summary>
		/// <param name="edges"> a list of real graph edges this abstraction represents </param>
		public mxGraphHierarchyEdge(IList<object> edges)
		{
			this.edges = edges;
		}

		/// <summary>
		/// Inverts the direction of this internal edge(s)
		/// </summary>
		public virtual void invert()
		{
			mxGraphHierarchyNode temp = source;
			source = target;
			target = temp;
			isReversed = !isReversed;
		}

		/// <returns> Returns the isReversed. </returns>
		public virtual bool Reversed
		{
			get
			{
				return isReversed;
			}
			set
			{
				this.isReversed = value;
			}
		}


		/// <summary>
		/// Returns the cells this cell connects to on the next layer up </summary>
		/// <param name="layer"> the layer this cell is on </param>
		/// <returns> the cells this cell connects to on the next layer up </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<mxGraphAbstractHierarchyCell> getNextLayerConnectedCells(int layer)
		public override List<mxGraphAbstractHierarchyCell> getNextLayerConnectedCells(int layer)
		{
			if (nextLayerConnectedCells == null)
			{
                int arrLength = temp.Length;

                nextLayerConnectedCells = new List<mxGraphAbstractHierarchyCell>[arrLength];

				for (int i = 0; i < nextLayerConnectedCells.Length; i++)
				{
					nextLayerConnectedCells[i] = new List<mxGraphAbstractHierarchyCell>(2);

					if (i == nextLayerConnectedCells.Length - 1)
					{
						nextLayerConnectedCells[i].Add(source);
					}
					else
					{
						nextLayerConnectedCells[i].Add(this);
					}
				}
			}

			return nextLayerConnectedCells[layer - minRank - 1];
		}

		/// <summary>
		/// Returns the cells this cell connects to on the next layer down </summary>
		/// <param name="layer"> the layer this cell is on </param>
		/// <returns> the cells this cell connects to on the next layer down </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<mxGraphAbstractHierarchyCell> getPreviousLayerConnectedCells(int layer)
		public override List<mxGraphAbstractHierarchyCell> getPreviousLayerConnectedCells(int layer)
		{
			if (previousLayerConnectedCells == null)
			{
                int arrLength = temp.Length;
                previousLayerConnectedCells = new List<mxGraphAbstractHierarchyCell>[arrLength];

				for (int i = 0; i < previousLayerConnectedCells.Length; i++)
				{
					previousLayerConnectedCells[i] = new List<mxGraphAbstractHierarchyCell>(2);

					if (i == 0)
					{
						previousLayerConnectedCells[i].Add(target);
					}
					else
					{
						previousLayerConnectedCells[i].Add(this);
					}
				}
			}

			return previousLayerConnectedCells[layer - minRank - 1];
		}

		/// 
		/// <returns> whether or not this cell is an edge </returns>
		public override bool Edge
		{
			get
			{
				return true;
			}
		}

		/// 
		/// <returns> whether or not this cell is a node </returns>
		public override bool Vertex
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the value of temp for the specified layer
		/// </summary>
		/// <param name="layer">
		///            the layer relating to a specific entry into temp </param>
		/// <returns> the value for that layer </returns>
		public override int getGeneralPurposeVariable(int layer)
		{
			return temp[layer - minRank - 1];
		}

		/// <summary>
		/// Set the value of temp for the specified layer
		/// </summary>
		/// <param name="layer">
		///            the layer relating to a specific entry into temp </param>
		/// <param name="value">
		///            the value for that layer </param>
		public override void setGeneralPurposeVariable(int layer, int value)
		{
			temp[layer - minRank - 1] = value;
		}

	}

}