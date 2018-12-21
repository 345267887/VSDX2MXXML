using System;
using System.Collections.Generic;

namespace mxGraph.layout
{


	using mxGeometry = mxGraph.model.mxGeometry;
	using mxIGraphModel = mxGraph.model.mxIGraphModel;
	using mxRectangle = mxGraph.util.mxRectangle;
	using mxGraph = mxGraph.view.mxGraph;

	public class mxCompactTreeLayout : mxGraphLayout
	{

		/// <summary>
		/// Specifies the orientation of the layout. Default is true.
		/// </summary>
		protected internal bool horizontal;

		/// <summary>
		/// Specifies if edge directions should be inverted. Default is false.
		/// </summary>
		protected internal bool invert;

		/// <summary>
		/// If the parent should be resized to match the width/height of the
		/// tree. Default is true.
		/// </summary>
		protected internal bool resizeParent = true;

		/// <summary>
		/// Specifies if the tree should be moved to the top, left corner
		/// if it is inside a top-level layer. Default is true.
		/// </summary>
		protected internal bool moveTree = true;

		/// <summary>
		/// Specifies if all edge points of traversed edges should be removed.
		/// Default is true.
		/// </summary>
		protected internal bool resetEdges = true;

		/// <summary>
		/// Holds the levelDistance. Default is 10.
		/// </summary>
		protected internal int levelDistance = 10;

		/// <summary>
		/// Holds the nodeDistance. Default is 20.
		/// </summary>
		protected internal int nodeDistance = 20;

		/// 
		/// <param name="graph"> </param>
		public mxCompactTreeLayout(mxGraph graph) : this(graph, true)
		{
		}

		/// 
		/// <param name="graph"> </param>
		/// <param name="horizontal"> </param>
		public mxCompactTreeLayout(mxGraph graph, bool horizontal) : this(graph, horizontal, false)
		{
		}

		/// 
		/// <param name="graph"> </param>
		/// <param name="horizontal"> </param>
		/// <param name="invert"> </param>
		public mxCompactTreeLayout(mxGraph graph, bool horizontal, bool invert) : base(graph)
		{
			this.horizontal = horizontal;
			this.invert = invert;
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

		/// <returns> the horizontal </returns>
		public virtual bool Horizontal
		{
			get
			{
				return horizontal;
			}
			set
			{
				this.horizontal = value;
			}
		}


		/// <returns> the invert </returns>
		public virtual bool Invert
		{
			get
			{
				return invert;
			}
			set
			{
				this.invert = value;
			}
		}


		/// <returns> the resizeParent </returns>
		public virtual bool ResizeParent
		{
			get
			{
				return resizeParent;
			}
			set
			{
				this.resizeParent = value;
			}
		}


		/// <returns> the moveTree </returns>
		public virtual bool MoveTree
		{
			get
			{
				return moveTree;
			}
			set
			{
				this.moveTree = value;
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


		/// <returns> the levelDistance </returns>
		public virtual int LevelDistance
		{
			get
			{
				return levelDistance;
			}
			set
			{
				this.levelDistance = value;
			}
		}


		/// <returns> the nodeDistance </returns>
		public virtual int NodeDistance
		{
			get
			{
				return nodeDistance;
			}
			set
			{
				this.nodeDistance = value;
			}
		}


		/*
		 * (non-Javadoc)
		 * @see mxGraphlayout.mxIGraphLayout#execute(java.lang.Object)
		 */
		public override void execute(object parent)
		{
			execute(parent, null);
		}

		/// <summary>
		/// Implements <mxGraphLayout.execute>.
		/// 
		/// If the parent has any connected edges, then it is used as the root of
		/// the tree. Else, <mxGraph.findTreeRoots> will be used to find a suitable
		/// root node within the set of children of the given parent.
		/// </summary>
		public virtual void execute(object parent, object root)
		{
			mxIGraphModel model = graph.Model;

			if (root == null)
			{
				// Takes the parent as the root if it has outgoing edges
				if (graph.getEdges(parent, model.getParent(parent), invert, !invert, false).Length > 0)
				{
					root = parent;
				}

				// Tries to find a suitable root in the parent's
				// children
				else
				{
					IList<object> roots = graph.findTreeRoots(parent, true, invert);

					if (roots != null && roots.Count > 0)
					{
						for (int i = 0; i < roots.Count; i++)
						{
							if (!isVertexIgnored(roots[i]) && graph.getEdges(roots[i], null, invert, !invert, false).Length > 0)
							{
								root = roots[i];
								break;
							}
						}
					}
				}
			}

			if (root != null)
			{
				parent = model.getParent(root);
				model.beginUpdate();

				try
				{
					TreeNode node = dfs(root, parent, null);

					if (node != null)
					{
						layout(node);

						double x0 = graph.GridSize;
						double y0 = x0;

						if (!moveTree || model.getParent(parent) == model.Root)
						{
							mxGeometry g = model.getGeometry(root);

							if (g != null)
							{
								x0 = g.X;
								y0 = g.Y;
							}
						}

						mxRectangle bounds = null;

						if (horizontal)
						{
							bounds = horizontalLayout(node, x0, y0, null);
						}
						else
						{
							bounds = verticalLayout(node, null, x0, y0, null);
						}

						if (bounds != null)
						{
							double dx = 0;
							double dy = 0;

							if (bounds.X < 0)
							{
								dx = Math.Abs(x0 - bounds.X);
							}

							if (bounds.Y < 0)
							{
								dy = Math.Abs(y0 - bounds.Y);
							}

							if (parent != null)
							{
								mxRectangle size = graph.getStartSize(parent);
								dx += size.Width;
								dy += size.Height;

								// Resize parent swimlane
								if (resizeParent && !graph.isCellCollapsed(parent))
								{
									mxGeometry g = model.getGeometry(parent);

									if (g != null)
									{
										double width = bounds.Width + size.Width - bounds.X + 2 * x0;
										double height = bounds.Height + size.Height - bounds.Y + 2 * y0;

										g = (mxGeometry) g.clone();

										if (g.Width > width)
										{
											dx += (g.Width - width) / 2;
										}
										else
										{
											g.Width = width;
										}

										if (g.Height > height)
										{
											if (horizontal)
											{
												dy += (g.Height - height) / 2;
											}
										}
										else
										{
											g.Height = height;
										}

										model.setGeometry(parent, g);
									}
								}
							}

							moveNode(node, dx, dy);
						}
					}
				}
				finally
				{
					model.endUpdate();
				}
			}
		}

		/// <summary>
		/// Moves the specified node and all of its children by the given amount.
		/// </summary>
		protected internal virtual void moveNode(TreeNode node, double dx, double dy)
		{
			node.x += dx;
			node.y += dy;
			apply(node, null);

			TreeNode child = node.child;

			while (child != null)
			{
				moveNode(child, dx, dy);
				child = child.next;
			}
		}

		/// <summary>
		/// Does a depth first search starting at the specified cell.
		/// Makes sure the specified swimlane is never left by the
		/// algorithm.
		/// </summary>
		protected internal virtual TreeNode dfs(object cell, object parent, ISet<object> visited)
		{
			if (visited == null)
			{
				visited = new HashSet<object>();
			}

			TreeNode node = null;

			if (cell != null && !visited.Contains(cell) && !isVertexIgnored(cell))
			{
				visited.Add(cell);
				node = createNode(cell);

				mxIGraphModel model = graph.Model;
				TreeNode prev = null;
				object[] @out = graph.getEdges(cell, parent, invert, !invert, false);

				for (int i = 0; i < @out.Length; i++)
				{
					object edge = @out[i];

					if (!isEdgeIgnored(edge))
					{
						// Resets the points on the traversed edge
						if (resetEdges)
						{
							setEdgePoints(edge, null);
						}

						// Checks if terminal in same swimlane
						object target = graph.View.getVisibleTerminal(edge, invert);
						TreeNode tmp = dfs(target, parent, visited);

						if (tmp != null && model.getGeometry(target) != null)
						{
							if (prev == null)
							{
								node.child = tmp;
							}
							else
							{
								prev.next = tmp;
							}

							prev = tmp;
						}
					}
				}
			}

			return node;
		}

		/// <summary>
		/// Starts the actual compact tree layout algorithm
		/// at the given node.
		/// </summary>
		protected internal virtual void layout(TreeNode node)
		{
			if (node != null)
			{
				TreeNode child = node.child;

				while (child != null)
				{
					layout(child);
					child = child.next;
				}

				if (node.child != null)
				{
					attachParent(node, join(node));
				}
				else
				{
					layoutLeaf(node);
				}
			}
		}

		/// 
		protected internal virtual mxRectangle horizontalLayout(TreeNode node, double x0, double y0, mxRectangle bounds)
		{
			node.x += x0 + node.offsetX;
			node.y += y0 + node.offsetY;
			bounds = apply(node, bounds);
			TreeNode child = node.child;

			if (child != null)
			{
				bounds = horizontalLayout(child, node.x, node.y, bounds);
				double siblingOffset = node.y + child.offsetY;
				TreeNode s = child.next;

				while (s != null)
				{
					bounds = horizontalLayout(s, node.x + child.offsetX, siblingOffset, bounds);
					siblingOffset += s.offsetY;
					s = s.next;
				}
			}

			return bounds;
		}

		/// 
		protected internal virtual mxRectangle verticalLayout(TreeNode node, object parent, double x0, double y0, mxRectangle bounds)
		{
			node.x += x0 + node.offsetY;
			node.y += y0 + node.offsetX;
			bounds = apply(node, bounds);
			TreeNode child = node.child;

			if (child != null)
			{
				bounds = verticalLayout(child, node, node.x, node.y, bounds);
				double siblingOffset = node.x + child.offsetY;
				TreeNode s = child.next;

				while (s != null)
				{
					bounds = verticalLayout(s, node, siblingOffset, node.y + child.offsetX, bounds);
					siblingOffset += s.offsetY;
					s = s.next;
				}
			}

			return bounds;
		}

		/// 
		protected internal virtual void attachParent(TreeNode node, double height)
		{
			double x = nodeDistance + levelDistance;
			double y2 = (height - node.width) / 2 - nodeDistance;
			double y1 = y2 + node.width + 2 * nodeDistance - height;

			node.child.offsetX = x + node.height;
			node.child.offsetY = y1;

			node.contour.upperHead = createLine(node.height, 0, createLine(x, y1, node.contour.upperHead));
			node.contour.lowerHead = createLine(node.height, 0, createLine(x, y2, node.contour.lowerHead));
		}

		/// 
		protected internal virtual void layoutLeaf(TreeNode node)
		{
			double dist = 2 * nodeDistance;

			node.contour.upperTail = createLine(node.height + dist, 0, null);
			node.contour.upperHead = node.contour.upperTail;
			node.contour.lowerTail = createLine(0, -node.width - dist, null);
			node.contour.lowerHead = createLine(node.height + dist, 0, node.contour.lowerTail);
		}

		/// 
		protected internal virtual double join(TreeNode node)
		{
			double dist = 2 * nodeDistance;

			TreeNode child = node.child;
			node.contour = child.contour;
			double h = child.width + dist;
			double sum = h;
			child = child.next;

			while (child != null)
			{
				double d = merge(node.contour, child.contour);
				child.offsetY = d + h;
				child.offsetX = 0;
				h = child.width + dist;
				sum += d + h;
				child = child.next;
			}

			return sum;
		}

		/// 
		protected internal virtual double merge(Polygon p1, Polygon p2)
		{
			double x = 0;
			double y = 0;
			double total = 0;

			Polyline upper = p1.lowerHead;
			Polyline lower = p2.upperHead;

			while (lower != null && upper != null)
			{
				double d = offset(x, y, lower.dx, lower.dy, upper.dx, upper.dy);
				y += d;
				total += d;

				if (x + lower.dx <= upper.dx)
				{
					x += lower.dx;
					y += lower.dy;
					lower = lower.next;
				}
				else
				{
					x -= upper.dx;
					y -= upper.dy;
					upper = upper.next;
				}
			}

			if (lower != null)
			{
				Polyline b = bridge(p1.upperTail, 0, 0, lower, x, y);
				p1.upperTail = (b.next != null) ? p2.upperTail : b;
				p1.lowerTail = p2.lowerTail;
			}
			else
			{
				Polyline b = bridge(p2.lowerTail, x, y, upper, 0, 0);

				if (b.next == null)
				{
					p1.lowerTail = b;
				}
			}

			p1.lowerHead = p2.lowerHead;

			return total;
		}

		/// 
		protected internal virtual double offset(double p1, double p2, double a1, double a2, double b1, double b2)
		{
			double d = 0;

			if (b1 <= p1 || p1 + a1 <= 0)
			{
				return 0;
			}

			double t = b1 * a2 - a1 * b2;

			if (t > 0)
			{
				if (p1 < 0)
				{
					double s = p1 * a2;
					d = s / a1 - p2;
				}
				else if (p1 > 0)
				{
					double s = p1 * b2;
					d = s / b1 - p2;
				}
				else
				{
					d = -p2;
				}
			}
			else if (b1 < p1 + a1)
			{
				double s = (b1 - p1) * a2;
				d = b2 - (p2 + s / a1);
			}
			else if (b1 > p1 + a1)
			{
				double s = (a1 + p1) * b2;
				d = s / b1 - (p2 + a2);
			}
			else
			{
				d = b2 - (p2 + a2);
			}

			if (d > 0)
			{
				return d;
			}

			return 0;
		}

		/// 
		protected internal virtual Polyline bridge(Polyline line1, double x1, double y1, Polyline line2, double x2, double y2)
		{
			double dx = x2 + line2.dx - x1;
			double dy = 0;
			double s = 0;

			if (line2.dx == 0)
			{
				dy = line2.dy;
			}
			else
			{
				s = dx * line2.dy;
				dy = s / line2.dx;
			}

			Polyline r = createLine(dx, dy, line2.next);
			line1.next = createLine(0, y2 + line2.dy - dy - y1, r);

			return r;
		}

		/// 
		protected internal virtual TreeNode createNode(object cell)
		{
			TreeNode node = new TreeNode(cell);

			mxRectangle geo = getVertexBounds(cell);

			if (geo != null)
			{
				if (horizontal)
				{
					node.width = geo.Height;
					node.height = geo.Width;
				}
				else
				{
					node.width = geo.Width;
					node.height = geo.Height;
				}
			}

			return node;
		}

		/// 
		protected internal virtual mxRectangle apply(TreeNode node, mxRectangle bounds)
		{
			mxRectangle g = graph.Model.getGeometry(node.cell);

			if (node.cell != null && g != null)
			{
				if (isVertexMovable(node.cell))
				{
					g = setVertexLocation(node.cell, node.x, node.y);
				}

				if (bounds == null)
				{
					bounds = new mxRectangle(g.X, g.Y, g.Width, g.Height);
				}
				else
				{
					bounds = new mxRectangle(Math.Min(bounds.X, g.X), Math.Min(bounds.Y, g.Y), Math.Max(bounds.X + bounds.Width, g.X + g.Width), Math.Max(bounds.Y + bounds.Height, g.Y + g.Height));
				}
			}

			return bounds;
		}

		/// 
		protected internal virtual Polyline createLine(double dx, double dy, Polyline next)
		{
			return new Polyline(dx, dy, next);
		}

		/// 
		protected internal class TreeNode
		{
			/// 
			protected internal object cell;

			/// 
			protected internal double x, y, width, height, offsetX, offsetY;

			/// 
			protected internal TreeNode child, next; // parent, sibling

			/// 
			protected internal Polygon contour = new Polygon();

			/// 
			public TreeNode(object cell)
			{
				this.cell = cell;
			}

		}

		/// 
		protected internal class Polygon
		{

			/// 
			protected internal Polyline lowerHead, lowerTail, upperHead, upperTail;

		}

		/// 
		protected internal class Polyline
		{

			/// 
			protected internal double dx, dy;

			/// 
			protected internal Polyline next;

			/// 
			protected internal Polyline(double dx, double dy, Polyline next)
			{
				this.dx = dx;
				this.dy = dy;
				this.next = next;
			}

		}

	}

}