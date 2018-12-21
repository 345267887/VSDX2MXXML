using mxGraph;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * $Id: mxGraphHierarchyModel.java,v 1.1 2010-11-30 20:36:47 david Exp $
 * Copyright (c) 2005-2010, David Benson, Gaudenz Alder
 */
namespace mxGraph.layout.hierarchical.model
{


	using mxGraph = mxGraph.view.mxGraph;

	/// <summary>
	/// Internal model of a hierarchical graph. This model stores nodes and edges
	/// equivalent to the real graph nodes and edges, but also stores the rank of the
	/// cells, the order within the ranks and the new candidate locations of cells.
	/// The internal model also reverses edge direction were appropriate , ignores
	/// self-loop and groups parallels together under one edge object.
	/// </summary>
	public class mxGraphHierarchyModel
	{

		/// <summary>
		/// Whether the rank assignment is done from the sinks or sources.
		/// </summary>
		protected internal bool scanRanksFromSinks = true;

		/// <summary>
		/// Stores the largest rank number allocated
		/// </summary>
		public int maxRank;

		/// <summary>
		/// Map from graph vertices to internal model nodes
		/// </summary>
		protected internal IDictionary<object, mxGraphHierarchyNode> vertexMapper = null;

		/// <summary>
		/// Map from graph edges to internal model edges
		/// </summary>
		protected internal IDictionary<object, mxGraphHierarchyEdge> edgeMapper = null;

		/// <summary>
		/// Mapping from rank number to actual rank
		/// </summary>
		public IDictionary<int?, mxGraphHierarchyRank> ranks = null;

		/// <summary>
		/// Store of roots of this hierarchy model, these are real graph cells, not
		/// internal cells
		/// </summary>
		public List<object> roots;

		/// <summary>
		/// The parent cell whose children are being laid out
		/// </summary>
		public object parent = null;

		/// <summary>
		/// Count of the number of times the ancestor dfs has been used
		/// </summary>
		protected internal int dfsCount = 0;

		/// <summary>
		/// Whether or not cells are ordered according to the order in the graph
		/// model. Defaults to false since sorting usually produces quadratic
		/// performance. Note that since JGraph 6 returns edges in a deterministic
		/// order, it might be that this layout is always deterministic using that
		/// JGraph regardless of this flag setting (i.e. leave it false in that case)
		/// </summary>
		protected internal bool deterministic = false;

		/// <summary>
		/// High value to start source layering scan rank value from </summary>
		private readonly int SOURCESCANSTARTRANK = 100000000;

		/// <summary>
		/// Creates an internal ordered graph model using the vertices passed in. If
		/// there are any, leftward edge need to be inverted in the internal model
		/// </summary>
		/// <param name="layout">
		///            the enclosing layout object </param>
		/// <param name="vertices">
		///            the vertices for this hierarchy </param>
		/// <param name="ordered">
		///            whether or not the vertices are already ordered </param>
		/// <param name="deterministic">
		///            whether or not this layout should be deterministic on each </param>
		/// <param name="scanRanksFromSinks">
		///            Whether the rank assignment is done from the sinks or sources.
		///            usage </param>
		public mxGraphHierarchyModel(mxHierarchicalLayout layout, object[] vertices, List<object> roots, object parent, bool ordered, bool deterministic, bool scanRanksFromSinks)
		{
			mxGraph graph = layout.Graph;
			this.deterministic = deterministic;
			this.scanRanksFromSinks = scanRanksFromSinks;
			this.roots = roots;
			this.parent = parent;

			if (vertices == null)
			{
				vertices = graph.getChildVertices(parent);
			}

			if (ordered)
			{
				formOrderedHierarchy(layout, vertices, parent);
			}
			else
			{
				// map of cells to internal cell needed for second run through
				// to setup the sink of edges correctly. Guess size by number
				// of edges is roughly same as number of vertices.
				vertexMapper = new Dictionary<object, mxGraphHierarchyNode>(vertices.Length);
				edgeMapper = new Dictionary<object, mxGraphHierarchyEdge>(vertices.Length);
				if (scanRanksFromSinks)
				{
					maxRank = 0;
				}
				else
				{
					maxRank = SOURCESCANSTARTRANK;
				}
				mxGraphHierarchyNode[] internalVertices = new mxGraphHierarchyNode[vertices.Length];
				createInternalCells(layout, vertices, internalVertices);

				// Go through edges set their sink values. Also check the
				// ordering if and invert edges if necessary
				for (int i = 0; i < vertices.Length; i++)
				{
					ICollection<mxGraphHierarchyEdge> edges = internalVertices[i].connectsAsSource;
					IEnumerator<mxGraphHierarchyEdge> iter = edges.GetEnumerator();

					while (iter.MoveNext())
					{
						mxGraphHierarchyEdge internalEdge = iter.Current;
						ICollection<object> realEdges = internalEdge.edges;
						IEnumerator<object> iter2 = realEdges.GetEnumerator();
                        
						if (iter2.MoveNext())
						{
                            //object realEdge = iter2.next();
                            object realEdge = iter2.Current;
                            object targetCell = graph.View.getVisibleTerminal(realEdge, false);
							mxGraphHierarchyNode internalTargetCell = vertexMapper[targetCell];

							if (internalTargetCell != null && internalVertices[i] != internalTargetCell)
							{
								internalEdge.target = internalTargetCell;

								if (internalTargetCell.connectsAsTarget.Count == 0)
								{
									internalTargetCell.connectsAsTarget = new List<mxGraphHierarchyEdge>(4);
								}

								internalTargetCell.connectsAsTarget.Add(internalEdge);
							}
						}
					}

					// Use the temp variable in the internal nodes to mark this
					// internal vertex as having been visited.
					internalVertices[i].temp[0] = 1;
				}
			}
		}

		/// <summary>
		/// Creates an internal ordered graph model using the vertices passed in. If
		/// there are any, leftward edge need to be inverted in the internal model
		/// </summary>
		/// <param name="layout">
		///            reference to the layout algorithm </param>
		/// <param name="vertices">
		///            the vertices to be laid out </param>
		public virtual void formOrderedHierarchy(mxHierarchicalLayout layout, object[] vertices, object parent)
		{
			mxGraph graph = layout.Graph;

			// map of cells to internal cell needed for second run through
			// to setup the sink of edges correctly. Guess size by number
			// of edges is roughly same as number of vertices.
			vertexMapper = new Dictionary<object, mxGraphHierarchyNode>(vertices.Length * 2);
			edgeMapper = new Dictionary<object, mxGraphHierarchyEdge>(vertices.Length);
			maxRank = 0;
			mxGraphHierarchyNode[] internalVertices = new mxGraphHierarchyNode[vertices.Length];
			createInternalCells(layout, vertices, internalVertices);

			// Go through edges set their sink values. Also check the
			// ordering if and invert edges if necessary
			// Need a temporary list to store which of these edges have been
			// inverted in the internal model. If connectsAsSource were changed
			// in the following while loop we'd get a
			// ConcurrentModificationException
			IList<mxGraphHierarchyEdge> tempList = new List<mxGraphHierarchyEdge>();

			for (int i = 0; i < vertices.Length; i++)
			{
				ICollection<mxGraphHierarchyEdge> edges = internalVertices[i].connectsAsSource;
				IEnumerator<mxGraphHierarchyEdge> iter = edges.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphHierarchyEdge internalEdge = iter.Current;
					ICollection<object> realEdges = internalEdge.edges;
					IEnumerator<object> subiter = realEdges.GetEnumerator();
                    
					if (subiter.MoveNext())
					{
                        object realEdge = subiter.Current;//object realEdge = iter2.next();
                        object targetCell = graph.View.getVisibleTerminal(realEdge, false);
						mxGraphHierarchyNode internalTargetCell = vertexMapper[targetCell];

						if (internalTargetCell != null && internalVertices[i] != internalTargetCell)
						{
							internalEdge.target = internalTargetCell;

							if (internalTargetCell.connectsAsTarget.Count == 0)
							{
								internalTargetCell.connectsAsTarget = new List<mxGraphHierarchyEdge>(4);
							}

							// The vertices passed in were ordered, check that the
							// target cell has not already been marked as visited
							if (internalTargetCell.temp[0] == 1)
							{
								// Internal Edge is leftward, reverse it
								internalEdge.invert();
								// There must be a connectsAsSource list already
								internalTargetCell.connectsAsSource.Add(internalEdge);
								tempList.Add(internalEdge);
								internalVertices[i].connectsAsTarget.Add(internalEdge);
							}
							else
							{
								internalTargetCell.connectsAsTarget.Add(internalEdge);
							}
						}
					}
				}

				// Remove the inverted edges as sources from this node
				IEnumerator<mxGraphHierarchyEdge> iter2 = tempList.GetEnumerator();

				while (iter2.MoveNext())
				{
					internalVertices[i].connectsAsSource.Remove(iter2.Current);
				}

				tempList.Clear();

				// Use the temp variable in the internal nodes to mark this
				// internal vertex as having been visited.
				internalVertices[i].temp[0] = 1;
			}
		}

		/// <summary>
		/// Creates all edges in the internal model
		/// </summary>
		/// <param name="layout">
		///            reference to the layout algorithm </param>
		/// <param name="vertices">
		///            the vertices whom are to have an internal representation
		///            created </param>
		/// <param name="internalVertices">
		///            the blank internal vertices to have their information filled
		///            in using the real vertices </param>
		protected internal virtual void createInternalCells(mxHierarchicalLayout layout, object[] vertices, mxGraphHierarchyNode[] internalVertices)
		{
			mxGraph graph = layout.Graph;

			// Create internal edges
			for (int i = 0; i < vertices.Length; i++)
			{
				internalVertices[i] = new mxGraphHierarchyNode(vertices[i]);
				vertexMapper[vertices[i]] = internalVertices[i];

				// If the layout is deterministic, order the cells
				object[] conns = graph.getConnections(vertices[i], parent);
				IList<object> outgoingCells = new List<object>(graph.getOpposites(conns, vertices[i]));
				internalVertices[i].connectsAsSource = new List<mxGraphHierarchyEdge>(outgoingCells.Count);

				// Create internal edges, but don't do any rank assignment yet
				// First use the information from the greedy cycle remover to
				// invert the leftward edges internally
				IEnumerator<object> iter = outgoingCells.GetEnumerator();

				while (iter.MoveNext())
				{
					// Don't add self-loops
					object cell = iter.Current;

					if (cell != vertices[i] && graph.Model.isVertex(cell) && !layout.isVertexIgnored(cell))
					{
						// Allow for parallel edges
						object[] edges = graph.getEdgesBetween(vertices[i], cell, true);

						if (edges != null && edges.Length > 0)
						{
							List<object> listEdges = new List<object>(edges.Length);

							for (int j = 0; j < edges.Length; j++)
							{
								listEdges.Add(edges[j]);
							}

							mxGraphHierarchyEdge internalEdge = new mxGraphHierarchyEdge(listEdges);
							IEnumerator<object> iter2 = listEdges.GetEnumerator();

							while (iter2.MoveNext())
							{
								object edge = iter2.Current;
								edgeMapper[edge] = internalEdge;

								// Resets all point on the edge and disables the edge style
								// without deleting it from the cell style
								graph.resetEdge(edge);

								if (layout.DisableEdgeStyle)
								{
									layout.setEdgeStyleEnabled(edge, false);
									layout.setOrthogonalEdge(edge,true);
								}
							}

							internalEdge.source = internalVertices[i];
							internalVertices[i].connectsAsSource.Add(internalEdge);
						}
					}
				}

				// Ensure temp variable is cleared from any previous use
				internalVertices[i].temp[0] = 0;
			}
		}

		/// <summary>
		/// Basic determination of minimum layer ranking by working from from sources
		/// or sinks and working through each node in the relevant edge direction.
		/// Starting at the sinks is basically a longest path layering algorithm.
		/// </summary>
		public virtual void initialRank()
		{
            //IDictionary<object, mxGraphHierarchyNode>.ValueCollection internalNodes = vertexMapper.Values;
            ICollection<mxGraphHierarchyNode> internalNodes=vertexMapper.Values;
            LinkedList<mxGraphHierarchyNode> startNodes = new LinkedList<mxGraphHierarchyNode>();
            IEnumerator<object> iter;


            if (!scanRanksFromSinks && roots != null)
			{
                iter = roots.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphHierarchyNode internalNode = vertexMapper[iter.Current];

					if (internalNode != null)
					{
						startNodes.AddLast(internalNode);
					}
				}
			}

			if (scanRanksFromSinks)
			{
				iter = internalNodes.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphHierarchyNode internalNode = (mxGraphHierarchyNode)iter.Current;

					if (internalNode.connectsAsSource == null || internalNode.connectsAsSource.Count == 0)
					{
						startNodes.AddLast(internalNode);
					}
				}
			}

			if (startNodes.Count == 0)
			{
				// Start list from sources
				 iter= internalNodes.GetEnumerator();

				while (iter.MoveNext())
				{
					mxGraphHierarchyNode internalNode = (mxGraphHierarchyNode)iter.Current;
					if (internalNode.connectsAsTarget == null || internalNode.connectsAsTarget.Count == 0)
					{
						startNodes.AddLast(internalNode);
					}
				}
			}

			iter = internalNodes.GetEnumerator();

			while (iter.MoveNext())
			{
				mxGraphHierarchyNode internalNode = (mxGraphHierarchyNode)iter.Current;
				// Mark the node as not having had a layer assigned
				internalNode.temp[0] = -1;
			}

			IList<mxGraphHierarchyNode> startNodesCopy = new List<mxGraphHierarchyNode>(startNodes);

			while (startNodes.Count > 0)
			{
				mxGraphHierarchyNode internalNode = startNodes.First.Value;
				ICollection<mxGraphHierarchyEdge> layerDeterminingEdges;
				ICollection<mxGraphHierarchyEdge> edgesToBeMarked;

				if (scanRanksFromSinks)
				{
					layerDeterminingEdges = internalNode.connectsAsSource;
					edgesToBeMarked = internalNode.connectsAsTarget;
				}
				else
				{
					layerDeterminingEdges = internalNode.connectsAsTarget;
					edgesToBeMarked = internalNode.connectsAsSource;
				}

				// flag to keep track of whether or not all layer determining
				// edges have been scanned
				bool allEdgesScanned = true;

				// Work out the layer of this node from the layer determining
				// edges
				IEnumerator<mxGraphHierarchyEdge> iter2 = layerDeterminingEdges.GetEnumerator();

				// The minimum layer number of any node connected by one of
				// the layer determining edges variable. If we are starting
				// from sources, need to start at some huge value and
				// normalise down afterwards
				int minimumLayer = 0;
				if (!scanRanksFromSinks)
				{
					minimumLayer = SOURCESCANSTARTRANK;
				}

				while (allEdgesScanned && iter2.MoveNext())
				{
					mxGraphHierarchyEdge internalEdge = iter2.Current;

					if (internalEdge.temp[0] == 5270620)
					{
						// This edge has been scanned, get the layer of the
						// node on the other end
						mxGraphHierarchyNode otherNode;

						if (scanRanksFromSinks)
						{
							otherNode = internalEdge.target;
						}
						else
						{
							otherNode = internalEdge.source;
						}

						if (scanRanksFromSinks)
						{
							minimumLayer = Math.Max(minimumLayer, otherNode.temp[0] + 1);
						}
						else
						{
							minimumLayer = Math.Min(minimumLayer, otherNode.temp[0] - 1);
						}
					}
					else
					{
						allEdgesScanned = false;
					}
				}

				// If all edge have been scanned, assign the layer, mark all
				// edges in the other direction and remove from the nodes list
				if (allEdgesScanned)
				{
					internalNode.temp[0] = minimumLayer;
					if (scanRanksFromSinks)
					{
						maxRank = Math.Max(maxRank, minimumLayer);
					}
					else
					{
						maxRank = Math.Min(maxRank, minimumLayer);
					}

					if (edgesToBeMarked != null)
					{
						IEnumerator<mxGraphHierarchyEdge> iter3 = edgesToBeMarked.GetEnumerator();

						while (iter3.MoveNext())
						{
							mxGraphHierarchyEdge internalEdge = iter3.Current;
							// Assign unique stamp ( y/m/d/h )
							internalEdge.temp[0] = 5270620;

							// Add node on other end of edge to LinkedList of
							// nodes to be analysed
							mxGraphHierarchyNode otherNode;

							if (scanRanksFromSinks)
							{
								otherNode = internalEdge.source;
							}
							else
							{
								otherNode = internalEdge.target;
							}

							// Only add node if it hasn't been assigned a layer
							if (otherNode.temp[0] == -1)
							{
								startNodes.AddLast(otherNode);

								// Mark this other node as neither being
								// unassigned nor assigned so it isn't
								// added to this list again, but it's
								// layer isn't used in any calculation.
								otherNode.temp[0] = -2;
							}
						}
					}

					startNodes.RemoveFirst();
				}
				else
				{
                    // Not all the edges have been scanned, get to the back of
                    // the class and put the dunces cap on

                    object removedCell = startNodes.First.Value;
                    startNodes.RemoveFirst();
					startNodes.AddLast(internalNode);

					if (removedCell == internalNode && startNodes.Count == 1)
					{
						// This is an error condition, we can't get out of
						// this loop. It could happen for more than one node
						// but that's a lot harder to detect. Log the error
						// TODO make log comment
						break;
					}
				}
			}

			if (scanRanksFromSinks)
			{
				// Tighten the rank 0 nodes as far as possible
				for (int i = 0; i < startNodesCopy.Count; i++)
				{
					mxGraphHierarchyNode internalNode = startNodesCopy[i];
					int currentMinLayer = 1000000;
					IEnumerator<mxGraphHierarchyEdge> iter2 = internalNode.connectsAsTarget.GetEnumerator();

					while (iter2.MoveNext())
					{
						mxGraphHierarchyEdge internalEdge = iter2.Current;
						mxGraphHierarchyNode otherNode = internalEdge.source;
						internalNode.temp[0] = Math.Min(currentMinLayer, otherNode.temp[0] - 1);
						currentMinLayer = internalNode.temp[0];
					}
				}
			}
			else
			{
				// Normalize the ranks down from their large starting value to place
				// at least 1 sink on layer 0
				iter = internalNodes.GetEnumerator();
				while (iter.MoveNext())
				{
					mxGraphHierarchyNode internalNode = (mxGraphHierarchyNode)iter.Current;
					// Mark the node as not having had a layer assigned
					internalNode.temp[0] -= maxRank;
				}
				// Reset the maxRank to that which would be expected for a from-sink
				// scan
				maxRank = SOURCESCANSTARTRANK - maxRank;
			}
		}

		/// <summary>
		/// Fixes the layer assignments to the values stored in the nodes. Also needs
		/// to create dummy nodes for edges that cross layers.
		/// </summary>
		public virtual void fixRanks()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mxGraphHierarchyRank[] rankList = new mxGraphHierarchyRank[maxRank + 1];
			mxGraphHierarchyRank[] rankList = new mxGraphHierarchyRank[maxRank + 1];
			ranks = new LinkedHashMap<int?, mxGraphHierarchyRank>(maxRank + 1);

			for (int i = 0; i < maxRank + 1; i++)
			{
				rankList[i] = new mxGraphHierarchyRank();
				ranks[new int?(i)] = rankList[i];
			}

			// Perform a DFS to obtain an initial ordering for each rank.
			// Without doing this you would end up having to process
			// crossings for a standard tree.
			mxGraphHierarchyNode[] rootsArray = null;

			if (roots != null)
			{
				object[] oldRootsArray = roots.ToArray();
				rootsArray = new mxGraphHierarchyNode[oldRootsArray.Length];

				for (int i = 0; i < oldRootsArray.Length; i++)
				{
					object node = oldRootsArray[i];
					mxGraphHierarchyNode internalNode = vertexMapper[node];
					rootsArray[i] = internalNode;
				}
			}

			visit(new CellVisitorAnonymousInnerClass(this, rankList), rootsArray, false, null);
		}

		private class CellVisitorAnonymousInnerClass : mxGraphHierarchyModel.CellVisitor
		{
			private readonly mxGraphHierarchyModel outerInstance;

			private model.mxGraphHierarchyRank[] rankList;

			public CellVisitorAnonymousInnerClass(mxGraphHierarchyModel outerInstance, model.mxGraphHierarchyRank[] rankList)
			{
				this.outerInstance = outerInstance;
				this.rankList = rankList;
			}

			public virtual void visit(mxGraphHierarchyNode parent, mxGraphHierarchyNode cell, mxGraphHierarchyEdge connectingEdge, int layer, int seen)
			{
				mxGraphHierarchyNode node = cell;

				if (seen == 0 && node.maxRank < 0 && node.minRank < 0)
				{
                    rankList[node.temp[0]].Add(cell);
					node.maxRank = node.temp[0];
					node.minRank = node.temp[0];

					// Set temp[0] to the nodes position in the rank
					node.temp[0] = rankList[node.maxRank].Count - 1;
				}

				if (parent != null && connectingEdge != null)
				{
					int parentToCellRankDifference = (parent).maxRank - node.maxRank;

					if (parentToCellRankDifference > 1)
					{
						// There are ranks in between the parent and current cell
						mxGraphHierarchyEdge edge = connectingEdge;
						edge.maxRank = (parent).maxRank;
						edge.minRank = (cell).maxRank;
						edge.temp = new int[parentToCellRankDifference - 1];
						edge.x = new double[parentToCellRankDifference - 1];
						edge.y = new double[parentToCellRankDifference - 1];

						for (int i = edge.minRank + 1; i < edge.maxRank; i++)
						{
                            // The connecting edge must be added to the
                            // appropriate ranks
                            rankList[i].Add(edge);
							edge.setGeneralPurposeVariable(i, rankList[i].Count - 1);
						}
					}
				}
			}
		}

		/// <summary>
		/// A depth first search through the internal hierarchy model
		/// </summary>
		/// <param name="visitor">
		///            the visitor pattern to be called for each node </param>
		/// <param name="trackAncestors">
		///            whether or not the search is to keep track all nodes directly
		///            above this one in the search path </param>
		public virtual void visit(CellVisitor visitor, mxGraphHierarchyNode[] dfsRoots, bool trackAncestors, List<mxGraphHierarchyNode> seenNodes)
		{
			// Run dfs through on all roots
			if (dfsRoots != null)
			{
				for (int i = 0; i < dfsRoots.Length; i++)
				{
					mxGraphHierarchyNode internalNode = dfsRoots[i];

					if (internalNode != null)
					{
						if (seenNodes == null)
						{
							seenNodes = new List<mxGraphHierarchyNode>();
						}

						if (trackAncestors)
						{
							// Set up hash code for root
							internalNode.hashCode = new int[2];
							internalNode.hashCode[0] = dfsCount;
							internalNode.hashCode[1] = i;
							dfs(null, internalNode, null, visitor, seenNodes, internalNode.hashCode, i, 0);
						}
						else
						{
							dfs(null, internalNode, null, visitor, seenNodes, 0);
						}
					}
				}

				dfsCount++;
			}
		}

		/// <summary>
		/// Performs a depth first search on the internal hierarchy model
		/// </summary>
		/// <param name="parent">
		///            the parent internal node of the current internal node </param>
		/// <param name="root">
		///            the current internal node </param>
		/// <param name="connectingEdge">
		///            the internal edge connecting the internal node and the parent
		///            internal node, if any </param>
		/// <param name="visitor">
		///            the visitor pattern to be called for each node </param>
		/// <param name="seen">
		///            a set of all nodes seen by this dfs a set of all of the
		///            ancestor node of the current node </param>
		/// <param name="layer">
		///            the layer on the dfs tree ( not the same as the model ranks ) </param>
		public virtual void dfs(mxGraphHierarchyNode parent, mxGraphHierarchyNode root, mxGraphHierarchyEdge connectingEdge, CellVisitor visitor, List<mxGraphHierarchyNode> seen, int layer)
		{
			if (root != null)
			{
				if (!seen.Contains(root))
				{
					visitor.visit(parent, root, connectingEdge, layer, 0);
					seen.Add(root);

					// Copy the connects as source list so that visitors
					// can change the original for edge direction inversions
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] outgoingEdges = root.connectsAsSource.toArray();
					object[] outgoingEdges = root.connectsAsSource.ToArray();

					for (int i = 0; i < outgoingEdges.Length; i++)
					{
						mxGraphHierarchyEdge internalEdge = (mxGraphHierarchyEdge) outgoingEdges[i];
						mxGraphHierarchyNode targetNode = internalEdge.target;

						// Root check is O(|roots|)
						dfs(root, targetNode, internalEdge, visitor, seen, layer + 1);
					}
				}
				else
				{
					// Use the int field to indicate this node has been seen
					visitor.visit(parent, root, connectingEdge, layer, 1);
				}
			}
		}

		/// <summary>
		/// Performs a depth first search on the internal hierarchy model. This dfs
		/// extends the default version by keeping track of cells ancestors, but it
		/// should be only used when necessary because of it can be computationally
		/// intensive for deep searches.
		/// </summary>
		/// <param name="parent">
		///            the parent internal node of the current internal node </param>
		/// <param name="root">
		///            the current internal node </param>
		/// <param name="connectingEdge">
		///            the internal edge connecting the internal node and the parent
		///            internal node, if any </param>
		/// <param name="visitor">
		///            the visitor pattern to be called for each node </param>
		/// <param name="seen">
		///            a set of all nodes seen by this dfs </param>
		/// <param name="ancestors">
		///            the parent hash code </param>
		/// <param name="childHash">
		///            the new hash code for this node </param>
		/// <param name="layer">
		///            the layer on the dfs tree ( not the same as the model ranks ) </param>
		public virtual void dfs(mxGraphHierarchyNode parent, mxGraphHierarchyNode root, mxGraphHierarchyEdge connectingEdge, CellVisitor visitor, List<mxGraphHierarchyNode> seen, int[] ancestors, int childHash, int layer)
		{
			// Explanation of custom hash set. Previously, the ancestors variable
			// was passed through the dfs as a HashSet. The ancestors were copied
			// into a new HashSet and when the new child was processed it was also
			// added to the set. If the current node was in its ancestor list it
			// meant there is a cycle in the graph and this information is passed
			// to the visitor.visit() in the seen parameter. The HashSet clone was
			// very expensive on CPU so a custom hash was developed using primitive
			// types. temp[] couldn't be used so hashCode[] was added to each node.
			// Each new child adds another int to the array, copying the prefix
			// from its parent. Child of the same parent add different ints (the
			// limit is therefore 2^32 children per parent...). If a node has a
			// child with the hashCode already set then the child code is compared
			// to the same portion of the current nodes array. If they match there
			// is a loop.
			// Note that the basic mechanism would only allow for 1 use of this
			// functionality, so the root nodes have two ints. The second int is
			// incremented through each node root and the first is incremented
			// through each run of the dfs algorithm (therefore the dfs is not
			// thread safe). The hash code of each node is set if not already set,
			// or if the first int does not match that of the current run.
			if (root != null)
			{
				if (parent != null)
				{
					// Form this nodes hash code if necessary, that is, if the
					// hashCode variable has not been initialized or if the
					// start of the parent hash code does not equal the start of
					// this nodes hash code, indicating the code was set on a
					// previous run of this dfs.
					if (root.hashCode == null || root.hashCode[0] != parent.hashCode[0])
					{
						int hashCodeLength = parent.hashCode.Length + 1;
						root.hashCode = new int[hashCodeLength];
						Array.Copy(parent.hashCode, 0, root.hashCode, 0, parent.hashCode.Length);
						root.hashCode[hashCodeLength - 1] = childHash;
					}
				}

				if (!seen.Contains(root))
				{
					visitor.visit(parent, root, connectingEdge, layer, 0);
					seen.Add(root);
					// Copy the connects as source list so that visitors
					// can change the original for edge direction inversions
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object[] outgoingEdges = root.connectsAsSource.toArray();
					object[] outgoingEdges = root.connectsAsSource.ToArray();

					for (int i = 0; i < outgoingEdges.Length; i++)
					{
						mxGraphHierarchyEdge internalEdge = (mxGraphHierarchyEdge) outgoingEdges[i];
						mxGraphHierarchyNode targetNode = internalEdge.target;

						// Root check is O(|roots|)
						dfs(root, targetNode, internalEdge, visitor, seen, root.hashCode, i, layer + 1);
					}
				}
				else
				{
					// Use the int field to indicate this node has been seen
					visitor.visit(parent, root, connectingEdge, layer, 1);
				}
			}
		}

		/// <summary>
		/// Defines the interface that visitors use to perform operations upon the
		/// graph information during depth first search (dfs) or other tree-traversal
		/// strategies implemented by subclassers.
		/// </summary>
		public interface CellVisitor
		{

			/// <summary>
			/// The method within which the visitor will perform operations upon the
			/// graph model
			/// </summary>
			/// <param name="parent">
			///            the parent cell the current cell </param>
			/// <param name="cell">
			///            the current cell visited </param>
			/// <param name="connectingEdge">
			///            the edge that led the last cell visited to this cell </param>
			/// <param name="layer">
			///            the current layer of the tree </param>
			/// <param name="seen">
			///            an int indicating whether this cell has been seen
			///            previously </param>
			void visit(mxGraphHierarchyNode parent, mxGraphHierarchyNode cell, mxGraphHierarchyEdge connectingEdge, int layer, int seen);
		}

		/// <returns> Returns the vertexMapping. </returns>
		public virtual IDictionary<object, mxGraphHierarchyNode> VertexMapper
		{
			get
			{
				if (vertexMapper == null)
				{
					vertexMapper = new Dictionary<object, mxGraphHierarchyNode>();
				}
				return vertexMapper;
			}
			set
			{
				this.vertexMapper = value;
			}
		}


		/// <returns> Returns the edgeMapper. </returns>
		public virtual IDictionary<object, mxGraphHierarchyEdge> EdgeMapper
		{
			get
			{
				return edgeMapper;
			}
			set
			{
				this.edgeMapper = value;
			}
		}


		/// <returns> Returns the dfsCount. </returns>
		public virtual int DfsCount
		{
			get
			{
				return dfsCount;
			}
			set
			{
				this.dfsCount = value;
			}
		}


		/// <returns> Returns the deterministic. </returns>
		public virtual bool Deterministic
		{
			get
			{
				return deterministic;
			}
			set
			{
				this.deterministic = value;
			}
		}

	}
}