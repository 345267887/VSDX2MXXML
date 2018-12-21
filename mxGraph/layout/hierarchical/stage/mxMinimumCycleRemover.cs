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
namespace mxGraph.layout.hierarchical.stage
{


	using mxGraphHierarchyEdge = mxGraph.layout.hierarchical.model.mxGraphHierarchyEdge;
	using mxGraphHierarchyModel = mxGraph.layout.hierarchical.model.mxGraphHierarchyModel;
	using mxGraphHierarchyNode = mxGraph.layout.hierarchical.model.mxGraphHierarchyNode;
	using mxGraph = mxGraph.view.mxGraph;

	/// <summary>
	/// An implementation of the first stage of the Sugiyama layout. Straightforward
	/// longest path calculation of layer assignment
	/// </summary>
	public class mxMinimumCycleRemover : mxHierarchicalLayoutStage
	{

		/// <summary>
		/// Reference to the enclosing layout algorithm
		/// </summary>
		protected internal mxHierarchicalLayout layout;

		/// <summary>
		/// Constructor that has the roots specified
		/// </summary>
		public mxMinimumCycleRemover(mxHierarchicalLayout layout)
		{
			this.layout = layout;
		}

		/// <summary>
		/// Produces the layer assignmment using the graph information specified
		/// </summary>
		public virtual void execute(object parent)
		{
			mxGraphHierarchyModel model = layout.Model;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.Set<mxGraphlayout.hierarchical.model.mxGraphHierarchyNode> seenNodes = new java.util.HashSet<mxGraphlayout.hierarchical.model.mxGraphHierarchyNode>();
            List<mxGraphHierarchyNode> seenNodes = new List<mxGraphHierarchyNode>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Set<mxGraphlayout.hierarchical.model.mxGraphHierarchyNode> unseenNodes = new java.util.HashSet<mxGraphlayout.hierarchical.model.mxGraphHierarchyNode>(model.getVertexMapper().values());
			List<mxGraphHierarchyNode> unseenNodes = new List<mxGraphHierarchyNode>(model.VertexMapper.Values);

			// Perform a dfs through the internal model. If a cycle is found,
			// reverse it.
			mxGraphHierarchyNode[] rootsArray = null;

			if (model.roots != null)
			{
				object[] modelRoots = model.roots.ToArray();
				rootsArray = new mxGraphHierarchyNode[modelRoots.Length];

				for (int i = 0; i < modelRoots.Length; i++)
				{
					object node = modelRoots[i];
					mxGraphHierarchyNode internalNode = model.VertexMapper[node];
					rootsArray[i] = internalNode;
				}
			}

			model.visit(new CellVisitorAnonymousInnerClass(this, parent, seenNodes, unseenNodes), rootsArray, true, null);

			ISet<object> possibleNewRoots = null;

			if (unseenNodes.Count > 0)
			{
				possibleNewRoots = new HashSet<object>(unseenNodes);
			}

            // If there are any nodes that should be nodes that the dfs can miss
            // these need to be processed with the dfs and the roots assigned
            // correctly to form a correct internal model
            List<mxGraphHierarchyNode> seenNodesCopy = new List<mxGraphHierarchyNode>(seenNodes);

			// Pick a random cell and dfs from it
			mxGraphHierarchyNode[] unseenNodesArray = new mxGraphHierarchyNode[1];
            unseenNodesArray=unseenNodes.ToArray();

			model.visit(new CellVisitorAnonymousInnerClass2(this, parent, seenNodes, unseenNodes), unseenNodesArray, true, seenNodesCopy);

			mxGraph graph = layout.Graph;

			if (possibleNewRoots != null && possibleNewRoots.Count > 0)
			{
				IEnumerator<object> iter = possibleNewRoots.GetEnumerator();
				IList<object> roots = model.roots;

				while (iter.MoveNext())
				{
					mxGraphHierarchyNode node = (mxGraphHierarchyNode) iter.Current;
					object realNode = node.cell;
					int numIncomingEdges = graph.getIncomingEdges(realNode).Length;

					if (numIncomingEdges == 0)
					{
						roots.Add(realNode);
					}
				}
			}
		}

		private class CellVisitorAnonymousInnerClass : mxGraphHierarchyModel.CellVisitor
		{
			private readonly mxMinimumCycleRemover outerInstance;

			private object parent;
			private List<mxGraphHierarchyNode> seenNodes;
			private List<mxGraphHierarchyNode> unseenNodes;

			public CellVisitorAnonymousInnerClass(mxMinimumCycleRemover outerInstance, object parent, List<mxGraphHierarchyNode> seenNodes, List<mxGraphHierarchyNode> unseenNodes)
			{
				this.outerInstance = outerInstance;
				this.parent = parent;
				this.seenNodes = seenNodes;
				this.unseenNodes = unseenNodes;
			}

			public virtual void visit(mxGraphHierarchyNode parent, mxGraphHierarchyNode cell, mxGraphHierarchyEdge connectingEdge, int layer, int seen)
			{
				// Check if the cell is in it's own ancestor list, if so
				// invert the connecting edge and reverse the target/source
				// relationship to that edge in the parent and the cell
				if ((cell).isAncestor(parent))
				{
					connectingEdge.invert();
					parent.connectsAsSource.Remove(connectingEdge);
					parent.connectsAsTarget.Add(connectingEdge);
					cell.connectsAsTarget.Remove(connectingEdge);
					cell.connectsAsSource.Add(connectingEdge);
				}
				seenNodes.Add(cell);
				unseenNodes.Remove(cell);
			}
		}

		private class CellVisitorAnonymousInnerClass2 : mxGraphHierarchyModel.CellVisitor
		{
			private readonly mxMinimumCycleRemover outerInstance;

			private object parent;
			private List<mxGraphHierarchyNode> seenNodes;
			private List<mxGraphHierarchyNode> unseenNodes;

			public CellVisitorAnonymousInnerClass2(mxMinimumCycleRemover outerInstance, object parent, List<mxGraphHierarchyNode> seenNodes, List<mxGraphHierarchyNode> unseenNodes)
			{
				this.outerInstance = outerInstance;
				this.parent = parent;
				this.seenNodes = seenNodes;
				this.unseenNodes = unseenNodes;
			}

			public virtual void visit(mxGraphHierarchyNode parent, mxGraphHierarchyNode cell, mxGraphHierarchyEdge connectingEdge, int layer, int seen)
			{
				// Check if the cell is in it's own ancestor list, if so
				// invert the connecting edge and reverse the target/source
				// relationship to that edge in the parent and the cell
				if ((cell).isAncestor(parent))
				{
					connectingEdge.invert();
					parent.connectsAsSource.Remove(connectingEdge);
					parent.connectsAsTarget.Add(connectingEdge);
					cell.connectsAsTarget.Remove(connectingEdge);
					cell.connectsAsSource.Add(connectingEdge);
				}
				seenNodes.Add(cell);
				unseenNodes.Remove(cell);
			}
		}
	}

}