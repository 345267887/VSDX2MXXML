using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxCell.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.model
{


	using Element = System.Xml.XmlElement;
	using Node = System.Xml.XmlNode;

	/// <summary>
	/// Cells are the elements of the graph model. They represent the state
	/// of the groups, vertices and edges in a graph.
	/// 
	/// <h4>Edge Labels</h4>
	/// 
	/// Using the x- and y-coordinates of a cell's geometry it is
	/// possible to position the label on edges on a specific location
	/// on the actual edge shape as it appears on the screen. The
	/// x-coordinate of an edge's geometry is used to describe the
	/// distance from the center of the edge from -1 to 1 with 0
	/// being the center of the edge and the default value. The
	/// y-coordinate of an edge's geometry is used to describe
	/// the absolute, orthogonal distance in pixels from that
	/// point. In addition, the mxGeometry.offset is used
	/// as a absolute offset vector from the resulting point.
	/// 
	/// The width and height of an edge geometry are ignored.
	/// 
	/// To add more than one edge label, add a child vertex with
	/// a relative geometry. The x- and y-coordinates of that
	/// geometry will have the same semantiv as the above for
	/// edge labels.
	/// </summary>
	[Serializable]
	public class mxCell : mxICell
	{

		/// 
		private const long serialVersionUID = 910211337632342672L;

		/// <summary>
		/// Holds the Id. Default is null.
		/// </summary>
		protected internal string id;

		/// <summary>
		/// Holds the user object. Default is null.
		/// </summary>
		protected internal object value;

		/// <summary>
		/// Holds the geometry. Default is null.
		/// </summary>
		protected internal mxGeometry geometry;

		/// <summary>
		/// Holds the style as a string of the form
		/// stylename[;key=value]. Default is null.
		/// </summary>
		protected internal string style;

		/// <summary>
		/// Specifies whether the cell is a vertex or edge and whether it is
		/// connectable, visible and collapsed. Default values are false, false,
		/// true, true and false respectively.
		/// </summary>
		protected internal bool vertex = false, edge = false, connectable = true, visible = true, collapsed = false;

		/// <summary>
		/// Reference to the parent cell and source and target terminals for edges.
		/// </summary>
		protected internal mxICell parent, source, target;

		/// <summary>
		/// Holds the child cells and connected edges.
		/// </summary>
		protected internal IList<object> children, edges;

		/// <summary>
		/// Constructs a new cell with an empty user object.
		/// </summary>
		public mxCell() : this(null)
		{
		}

		/// <summary>
		/// Constructs a new cell for the given user object.
		/// </summary>
		/// <param name="value">
		///   Object that represents the value of the cell. </param>
		public mxCell(object value) : this(value, null, null)
		{
		}

		/// <summary>
		/// Constructs a new cell for the given parameters.
		/// </summary>
		/// <param name="value"> Object that represents the value of the cell. </param>
		/// <param name="geometry"> Specifies the geometry of the cell. </param>
		/// <param name="style"> Specifies the style as a formatted string. </param>
		public mxCell(object value, mxGeometry geometry, string style)
		{
			Value = value;
			Geometry = geometry;
			Style = style;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getId()
		 */
		public virtual string Id
		{
			get
			{
				return id;
			}
			set
			{
				this.id = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#getValue()
		 */
		public virtual object Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#getGeometry()
		 */
		public virtual mxGeometry Geometry
		{
			get
			{
				return geometry;
			}
			set
			{
				this.geometry = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#getStyle()
		 */
		public virtual string Style
		{
			get
			{
				return style;
			}
			set
			{
				this.style = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#isVertex()
		 */
		public virtual bool Vertex
		{
			get
			{
				return vertex;
			}
			set
			{
				this.vertex = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#isEdge()
		 */
		public virtual bool Edge
		{
			get
			{
				return edge;
			}
			set
			{
				this.edge = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#isConnectable()
		 */
		public virtual bool Connectable
		{
			get
			{
				return connectable;
			}
			set
			{
				this.connectable = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#isVisible()
		 */
		public virtual bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				this.visible = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#isCollapsed()
		 */
		public virtual bool Collapsed
		{
			get
			{
				return collapsed;
			}
			set
			{
				this.collapsed = value;
			}
		}


		/* (non-Javadoc)
		 * @see model.mxICell#getParent()
		 */
		public virtual mxICell Parent
		{
			get
			{
				return parent;
			}
			set
			{
				this.parent = value;
			}
		}


		/// <summary>
		/// Returns the source terminal.
		/// </summary>
		public virtual mxICell Source
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


		/// <summary>
		/// Returns the target terminal.
		/// </summary>
		public virtual mxICell Target
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


		/* (non-Javadoc)
		 * @see model.mxICell#getTerminal(boolean)
		 */
		public virtual mxICell getTerminal(bool source)
		{
			return (source) ? Source : Target;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#setTerminal(model.mxICell, boolean)
		 */
		public virtual mxICell setTerminal(mxICell terminal, bool isSource)
		{
			if (isSource)
			{
				Source = terminal;
			}
			else
			{
				Target = terminal;
			}

			return terminal;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getChildCount()
		 */
		public virtual int ChildCount
		{
			get
			{
				return (children != null) ? children.Count : 0;
			}
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getIndex(model.mxICell)
		 */
		public virtual int getIndex(mxICell child)
		{
			return (children != null) ? children.IndexOf(child) : -1;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getChildAt(int)
		 */
		public virtual mxICell getChildAt(int index)
		{
			return (children != null) ? (mxICell) children[index] : null;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#insert(model.mxICell)
		 */
		public virtual mxICell insert(mxICell child)
		{
			int index = ChildCount;

			if (child.Parent == this)
			{
				index--;
			}

			return insert(child, index);
		}

		/* (non-Javadoc)
		 * @see model.mxICell#insert(model.mxICell, int)
		 */
		public virtual mxICell insert(mxICell child, int index)
		{
			if (child != null)
			{
				child.removeFromParent();
				child.Parent = this;

				if (children == null)
				{
					children = new List<object>();
					children.Add(child);
				}
				else
				{
					children.Insert(index, child);
				}
			}

			return child;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#remove(int)
		 */
		public virtual mxICell remove(int index)
		{
			mxICell child = null;

			if (children != null && index >= 0)
			{
				child = getChildAt(index);
				remove(child);
			}

			return child;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#remove(model.mxICell)
		 */
		public virtual mxICell remove(mxICell child)
		{
			if (child != null && children != null)
			{
				children.Remove(child);
				child.Parent = null;
			}

			return child;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#removeFromParent()
		 */
		public virtual void removeFromParent()
		{
			if (parent != null)
			{
				parent.remove(this);
			}
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getEdgeCount()
		 */
		public virtual int EdgeCount
		{
			get
			{
				return (edges != null) ? edges.Count : 0;
			}
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getEdgeIndex(model.mxICell)
		 */
		public virtual int getEdgeIndex(mxICell edge)
		{
			return (edges != null) ? edges.IndexOf(edge) : -1;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#getEdgeAt(int)
		 */
		public virtual mxICell getEdgeAt(int index)
		{
			return (edges != null) ? (mxICell) edges[index] : null;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#insertEdge(model.mxICell, boolean)
		 */
		public virtual mxICell insertEdge(mxICell edge, bool isOutgoing)
		{
			if (edge != null)
			{
				edge.removeFromTerminal(isOutgoing);
				edge.setTerminal(this, isOutgoing);

				if (edges == null || edge.getTerminal(!isOutgoing) != this || !edges.Contains(edge))
				{
					if (edges == null)
					{
						edges = new List<object>();
					}

					edges.Add(edge);
				}
			}

			return edge;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#removeEdge(model.mxICell, boolean)
		 */
		public virtual mxICell removeEdge(mxICell edge, bool isOutgoing)
		{
			if (edge != null)
			{
				if (edge.getTerminal(!isOutgoing) != this && edges != null)
				{
					edges.Remove(edge);
				}

				edge.setTerminal(null, isOutgoing);
			}

			return edge;
		}

		/* (non-Javadoc)
		 * @see model.mxICell#removeFromTerminal(boolean)
		 */
		public virtual void removeFromTerminal(bool isSource)
		{
			mxICell terminal = getTerminal(isSource);

			if (terminal != null)
			{
				terminal.removeEdge(this, isSource);
			}
		}

		/// <summary>
		/// Returns the specified attribute from the user object if it is an XML
		/// node.
		/// </summary>
		/// <param name="name"> Name of the attribute whose value should be returned. </param>
		/// <returns> Returns the value of the given attribute or null. </returns>
		public virtual string getAttribute(string name)
		{
			return getAttribute(name, null);
		}

		/// <summary>
		/// Returns the specified attribute from the user object if it is an XML
		/// node.
		/// </summary>
		/// <param name="name"> Name of the attribute whose value should be returned. </param>
		/// <param name="defaultValue"> Default value to use if the attribute has no value. </param>
		/// <returns> Returns the value of the given attribute or defaultValue. </returns>
		public virtual string getAttribute(string name, string defaultValue)
		{
			object userObject = Value;
			string val = null;

			if (userObject is Element)
			{
				Element element = (Element) userObject;
                val = element.GetAttribute(name);
			}

			if (string.ReferenceEquals(val, null))
			{
				val = defaultValue;
			}

			return val;
		}

		/// <summary>
		/// Sets the specified attribute on the user object if it is an XML node.
		/// </summary>
		/// <param name="name"> Name of the attribute whose value should be set. </param>
		/// <param name="value"> New value of the attribute. </param>
		public virtual void setAttribute(string name, string value)
		{
			object userObject = Value;

			if (userObject is Element)
			{
				Element element = (Element) userObject;
                element.SetAttribute(name, value);
			}
		}

		/// <summary>
		/// Returns a clone of the cell.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
            mxCell clone = new mxCell();// (mxCell) base.clone();

			clone.Value = cloneValue();
			clone.Style = Style;
			clone.Collapsed = Collapsed;
			clone.Connectable = Connectable;
			clone.Edge = Edge;
			clone.Vertex = Vertex;
			clone.Visible = Visible;
			clone.Parent = null;
			clone.Source = null;
			clone.Target = null;
			clone.children = null;
			clone.edges = null;

			mxGeometry geometry = Geometry;

			if (geometry != null)
			{
				clone.Geometry = (mxGeometry) geometry.clone();
			}

			return clone;
		}

		/// <summary>
		/// Returns a clone of the user object. This implementation clones any XML
		/// nodes or otherwise returns the same user object instance.
		/// </summary>
		protected internal virtual object cloneValue()
		{
			object value = Value;

			if (value is Node)
			{
                value = ((Node) value).CloneNode(true);
			}

			return value;
		}

	}

}