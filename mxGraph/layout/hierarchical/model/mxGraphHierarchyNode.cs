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
	/// An abstraction of an internal node in the hierarchy layout
	/// </summary>
	public class mxGraphHierarchyNode : mxGraphAbstractHierarchyCell
	{

		/// <summary>
		/// Shared empty connection map to return instead of null in applyMap.
		/// </summary>
		public static List<mxGraphHierarchyEdge> emptyConnectionMap = new List<mxGraphHierarchyEdge>(0);

		/// <summary>
		/// The graph cell this object represents.
		/// </summary>
		public object cell = null;

		/// <summary>
		/// Collection of hierarchy edges that have this node as a target
		/// </summary>
		public List<mxGraphHierarchyEdge> connectsAsTarget = emptyConnectionMap;

		/// <summary>
		/// Collection of hierarchy edges that have this node as a source
		/// </summary>
		public List<mxGraphHierarchyEdge> connectsAsSource = emptyConnectionMap;

		/// <summary>
		/// Assigns a unique hashcode for each node. Used by the model dfs instead
		/// of copying HashSets
		/// </summary>
		public int[] hashCode;

		/// <summary>
		/// Constructs an internal node to represent the specified real graph cell </summary>
		/// <param name="cell"> the real graph cell this node represents </param>
		public mxGraphHierarchyNode(object cell)
		{
			this.cell = cell;
		}

		/// <summary>
		/// Returns the integer value of the layer that this node resides in </summary>
		/// <returns> the integer value of the layer that this node resides in </returns>
		public virtual int RankValue
		{
			get
			{
				return maxRank;
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
				nextLayerConnectedCells = new List<mxGraphAbstractHierarchyCell>[1];
				nextLayerConnectedCells[0] = new List<mxGraphAbstractHierarchyCell>(connectsAsTarget.Count);
				IEnumerator<mxGraphHierarchyEdge> iter = connectsAsTarget.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphHierarchyEdge edge = iter.Current;

					if (edge.maxRank == -1 || edge.maxRank == layer + 1)
					{
						// Either edge is not in any rank or
						// no dummy nodes in edge, add node of other side of edge
						nextLayerConnectedCells[0].Add(edge.source);
					}
					else
					{
						// Edge spans at least two layers, add edge
						nextLayerConnectedCells[0].Add(edge);
					}
				}
			}

			return nextLayerConnectedCells[0];
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
				previousLayerConnectedCells = new List<mxGraphAbstractHierarchyCell>[1];
				previousLayerConnectedCells[0] = new List<mxGraphAbstractHierarchyCell>(connectsAsSource.Count);
				IEnumerator<mxGraphHierarchyEdge> iter = connectsAsSource.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphHierarchyEdge edge = iter.Current;

					if (edge.minRank == -1 || edge.minRank == layer - 1)
					{
						// No dummy nodes in edge, add node of other side of edge
						previousLayerConnectedCells[0].Add(edge.target);
					}
					else
					{
						// Edge spans at least two layers, add edge
						previousLayerConnectedCells[0].Add(edge);
					}
				}
			}

			return previousLayerConnectedCells[0];
		}

		/// 
		/// <returns> whether or not this cell is an edge </returns>
		public override bool Edge
		{
			get
			{
				return false;
			}
		}

		/// 
		/// <returns> whether or not this cell is a node </returns>
		public override bool Vertex
		{
			get
			{
				return true;
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
			return temp[0];
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
			temp[0] = value;
		}

		public virtual bool isAncestor(mxGraphHierarchyNode otherNode)
		{
			// Firstly, the hash code of this node needs to be shorter than the
			// other node
			if (otherNode != null && hashCode != null && otherNode.hashCode != null && hashCode.Length < otherNode.hashCode.Length)
			{
				if (hashCode == otherNode.hashCode)
				{
					return true;
				}

				if (hashCode == null || hashCode == null)
				{
					return false;
				}

				// Secondly, this hash code must match the start of the other
				// node's hash code. Arrays.equals cannot be used here since
				// the arrays are different length, and we do not want to
				// perform another array copy.
				for (int i = 0; i < hashCode.Length; i++)
				{
					if (hashCode[i] != otherNode.hashCode[i])
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

	}

}