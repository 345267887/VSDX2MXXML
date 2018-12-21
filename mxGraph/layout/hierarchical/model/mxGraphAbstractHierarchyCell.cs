using System.Collections.Generic;

/// <summary>
/// $Id: mxGraphAbstractHierarchyCell.java,v 1.1 2010-11-30 20:36:47 david Exp $
/// Copyright (c) 2005-2010, David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.layout.hierarchical.model
{

	/// <summary>
	/// An abstraction of an internal hierarchy node or edge
	/// </summary>
	public abstract class mxGraphAbstractHierarchyCell
	{

		/// <summary>
		/// The maximum rank this cell occupies
		/// </summary>
		public int maxRank = -1;

		/// <summary>
		/// The minimum rank this cell occupies
		/// </summary>
		public int minRank = -1;

		/// <summary>
		/// The x position of this cell for each layer it occupies
		/// </summary>
		public double[] x = new double[1];

		/// <summary>
		/// The y position of this cell for each layer it occupies
		/// </summary>
		public double[] y = new double[1];

		/// <summary>
		/// The width of this cell
		/// </summary>
		public double width = 0.0;

		/// <summary>
		/// The height of this cell
		/// </summary>
		public double height = 0.0;

		/// <summary>
		/// A cached version of the cells this cell connects to on the next layer up
		/// </summary>
		protected internal List<mxGraphAbstractHierarchyCell>[] nextLayerConnectedCells = null;

		/// <summary>
		/// A cached version of the cells this cell connects to on the next layer down
		/// </summary>
		protected internal List<mxGraphAbstractHierarchyCell>[] previousLayerConnectedCells = null;

		/// <summary>
		/// Temporary variable for general use. Generally, try to avoid
		/// carrying information between stages. Currently, the longest
		/// path layering sets temp to the rank position in fixRanks()
		/// and the crossing reduction uses this. This meant temp couldn't
		/// be used for hashing the nodes in the model dfs and so hashCode
		/// was created
		/// </summary>
		public int[] temp = new int[1];

		/// <summary>
		/// Returns the cells this cell connects to on the next layer up </summary>
		/// <param name="layer"> the layer this cell is on </param>
		/// <returns> the cells this cell connects to on the next layer up </returns>
		public abstract List<mxGraphAbstractHierarchyCell> getNextLayerConnectedCells(int layer);

		/// <summary>
		/// Returns the cells this cell connects to on the next layer down </summary>
		/// <param name="layer"> the layer this cell is on </param>
		/// <returns> the cells this cell connects to on the next layer down </returns>
		public abstract List<mxGraphAbstractHierarchyCell> getPreviousLayerConnectedCells(int layer);

		/// 
		/// <returns> whether or not this cell is an edge </returns>
		public abstract bool Edge {get;}

		/// 
		/// <returns> whether or not this cell is a node </returns>
		public abstract bool Vertex {get;}

		/// <summary>
		/// Gets the value of temp for the specified layer
		/// </summary>
		/// <param name="layer">
		///            the layer relating to a specific entry into temp </param>
		/// <returns> the value for that layer </returns>
		public abstract int getGeneralPurposeVariable(int layer);

		/// <summary>
		/// Set the value of temp for the specified layer
		/// </summary>
		/// <param name="layer">
		///            the layer relating to a specific entry into temp </param>
		/// <param name="value">
		///            the value for that layer </param>
		public abstract void setGeneralPurposeVariable(int layer, int value);

		/// <summary>
		/// Set the value of x for the specified layer
		/// </summary>
		/// <param name="layer">
		///            the layer relating to a specific entry into x[] </param>
		/// <param name="value">
		///            the x value for that layer </param>
		public virtual void setX(int layer, double value)
		{
			if (Vertex)
			{
				x[0] = value;
			}
			else if (Edge)
			{
				x[layer - minRank - 1] = value;
			}
		}

		/// <summary>
		/// Gets the value of x on the specified layer </summary>
		/// <param name="layer"> the layer to obtain x for </param>
		/// <returns> the value of x on the specified layer </returns>
		public virtual double getX(int layer)
		{
			if (Vertex)
			{
				return x[0];
			}
			else if (Edge)
			{
				return x[layer - minRank - 1];
			}

			return 0.0;
		}

		/// <summary>
		/// Set the value of y for the specified layer
		/// </summary>
		/// <param name="layer">
		///            the layer relating to a specific entry into y[] </param>
		/// <param name="value">
		///            the y value for that layer </param>
		public virtual void setY(int layer, double value)
		{
			if (Vertex)
			{
				y[0] = value;
			}
			else if (Edge)
			{
				y[layer - minRank - 1] = value;
			}
		}

	}
}