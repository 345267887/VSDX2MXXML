using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxCell
    {
        /**
	 * 
	 */
        private const long serialVersionUID = 910211337632342672L;

        /**
         * Holds the Id. Default is null.
         */
        protected String id;

        /**
         * Holds the user object. Default is null.
         */
        protected Object value;

        /**
         * Holds the geometry. Default is null.
         */
        protected mxGeometry geometry;

        /**
         * Holds the style as a string of the form
         * stylename[;key=value]. Default is null.
         */
        protected String style;

        /**
         * Specifies whether the cell is a vertex or edge and whether it is
         * connectable, visible and collapsed. Default values are false, false,
         * true, true and false respectively.
         */
        protected bool vertex = false, edge = false, connectable = true, visible = true, collapsed = false;

        /**
         * Reference to the parent cell and source and target terminals for edges.
         */
        protected mxICell parent, source, target;

        /**
         * Holds the child cells and connected edges.
         */
        protected List<Object> children, edges;

        /**
         * Constructs a new cell with an empty user object.
         */
        public mxCell() : this(null)
        {
        }

        /**
         * Constructs a new cell for the given user object.
         * 
         * @param value
         *   Object that represents the value of the cell.
         */
        public mxCell(Object value) : this(value, null, null)
        {
        }

        /**
         * Constructs a new cell for the given parameters.
         * 
         * @param value Object that represents the value of the cell.
         * @param geometry Specifies the geometry of the cell.
         * @param style Specifies the style as a formatted string.
         */
        public mxCell(Object value, mxGeometry geometry, String style)
        {
            setValue(value);
            setGeometry(geometry);
            setStyle(style);
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getId()
         */
        public String getId()
        {
            return id;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setId(String)
         */
        public void setId(String id)
        {
            this.id = id;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getValue()
         */
        public Object getValue()
        {
            return value;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setValue(Object)
         */
        public void setValue(Object value)
        {
            this.value = value;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getGeometry()
         */
        public mxGeometry getGeometry()
        {
            return geometry;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setGeometry(com.mxgraph.model.mxGeometry)
         */
        public void setGeometry(mxGeometry geometry)
        {
            this.geometry = geometry;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getStyle()
         */
        public String getStyle()
        {
            return style;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setStyle(String)
         */
        public void setStyle(String style)
        {
            this.style = style;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#isVertex()
         */
        public bool isVertex()
        {
            return vertex;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setVertex(boolean)
         */
        public void setVertex(bool vertex)
        {
            this.vertex = vertex;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#isEdge()
         */
        public bool isEdge()
        {
            return edge;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setEdge(boolean)
         */
        public void setEdge(bool edge)
        {
            this.edge = edge;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#isConnectable()
         */
        public bool isConnectable()
        {
            return connectable;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setConnectable(boolean)
         */
        public void setConnectable(bool connectable)
        {
            this.connectable = connectable;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#isVisible()
         */
        public bool isVisible()
        {
            return visible;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setVisible(boolean)
         */
        public void setVisible(bool visible)
        {
            this.visible = visible;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#isCollapsed()
         */
        public bool isCollapsed()
        {
            return collapsed;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setCollapsed(boolean)
         */
        public void setCollapsed(bool collapsed)
        {
            this.collapsed = collapsed;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getParent()
         */
        public mxICell getParent()
        {
            return parent;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setParent(com.mxgraph.model.mxICell)
         */
        public void setParent(mxICell parent)
        {
            this.parent = parent;
        }

        /**
         * Returns the source terminal.
         */
        public mxICell getSource()
        {
            return source;
        }

        /**
         * Sets the source terminal.
         * 
         * @param source Cell that represents the new source terminal.
         */
        public void setSource(mxICell source)
        {
            this.source = source;
        }

        /**
         * Returns the target terminal.
         */
        public mxICell getTarget()
        {
            return target;
        }

        /**
         * Sets the target terminal.
         * 
         * @param target Cell that represents the new target terminal.
         */
        public void setTarget(mxICell target)
        {
            this.target = target;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getTerminal(boolean)
         */
        public mxICell getTerminal(bool source)
        {
            return (source) ? getSource() : getTarget();
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#setTerminal(com.mxgraph.model.mxICell, boolean)
         */
        public mxICell setTerminal(mxICell terminal, bool isSource)
        {
            if (isSource)
            {
                setSource(terminal);
            }
            else
            {
                setTarget(terminal);
            }

            return terminal;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getChildCount()
         */
        public int getChildCount()
        {
            return (children != null) ? children.Count : 0;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getIndex(com.mxgraph.model.mxICell)
         */
        public int getIndex(mxICell child)
        {
            return (children != null) ? children.IndexOf(child) : -1;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getChildAt(int)
         */
        public mxICell getChildAt(int index)
        {
            return (children != null) ? (mxICell)children[index] : null;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#insert(com.mxgraph.model.mxICell)
         */
        public mxICell insert(mxICell child)
        {
            int index = getChildCount();

            if (child.getParent() == this)
            {
                index--;
            }

            return insert(child, index);
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#insert(com.mxgraph.model.mxICell, int)
         */
        public mxICell insert(mxICell child, int index)
        {
            if (child != null)
            {
                child.removeFromParent();
                child.setParent(this);

                if (children == null)
                {
                    children = new List<Object>();
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
         * @see com.mxgraph.model.mxICell#remove(int)
         */
        public mxICell remove(int index)
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
         * @see com.mxgraph.model.mxICell#remove(com.mxgraph.model.mxICell)
         */
        public mxICell remove(mxICell child)
        {
            if (child != null && children != null)
            {
                children.Remove(child);
                child.setParent(null);
            }

            return child;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#removeFromParent()
         */
        public void removeFromParent()
        {
            if (parent != null)
            {
                parent.remove(this);
            }
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getEdgeCount()
         */
        public int getEdgeCount()
        {
            return (edges != null) ? edges.Count : 0;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getEdgeIndex(com.mxgraph.model.mxICell)
         */
        public int getEdgeIndex(mxICell edge)
        {
            return (edges != null) ? edges.IndexOf(edge) : -1;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#getEdgeAt(int)
         */
        public mxICell getEdgeAt(int index)
        {
            return (edges != null) ? (mxICell)edges[index] : null;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#insertEdge(com.mxgraph.model.mxICell, boolean)
         */
        public mxICell insertEdge(mxICell edge, bool isOutgoing)
        {
            if (edge != null)
            {
                edge.removeFromTerminal(isOutgoing);
                edge.setTerminal(this, isOutgoing);

                if (edges == null || edge.getTerminal(!isOutgoing) != this
                        || !edges.Contains(edge))
                {
                    if (edges == null)
                    {
                        edges = new List<Object>();
                    }

                    edges.Add(edge);
                }
            }

            return edge;
        }

        /* (non-Javadoc)
         * @see com.mxgraph.model.mxICell#removeEdge(com.mxgraph.model.mxICell, boolean)
         */
        public mxICell removeEdge(mxICell edge, bool isOutgoing)
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
         * @see com.mxgraph.model.mxICell#removeFromTerminal(boolean)
         */
        public void removeFromTerminal(bool isSource)
        {
            mxICell terminal = getTerminal(isSource);

            if (terminal != null)
            {
                terminal.removeEdge(this, isSource);
            }
        }

        /**
         * Returns the specified attribute from the user object if it is an XML
         * node.
         * 
         * @param name Name of the attribute whose value should be returned.
         * @return Returns the value of the given attribute or null.
         */
        public String getAttribute(String name)
        {
            return getAttribute(name, null);
        }

        /**
         * Returns the specified attribute from the user object if it is an XML
         * node.
         * 
         * @param name Name of the attribute whose value should be returned.
         * @param defaultValue Default value to use if the attribute has no value.
         * @return Returns the value of the given attribute or defaultValue.
         */
        public String getAttribute(String name, String defaultValue)
        {
            Object userObject = getValue();
            String val = null;

            if (userObject is XmlElement)
            {
                XmlElement element = (XmlElement)userObject;
                val = element.GetAttribute(name);
            }

            if (val == null)
            {
                val = defaultValue;
            }

            return val;
        }

        /**
         * Sets the specified attribute on the user object if it is an XML node.
         * 
         * @param name Name of the attribute whose value should be set.
         * @param value New value of the attribute.
         */
        public void setAttribute(String name, String value)
        {
            Object userObject = getValue();

            if (userObject is XmlElement)
            {
                XmlElement element = (XmlElement)userObject;
                element.SetAttribute(name, value);
            }
        }

        /**
         * Returns a clone of the cell.
         */
        public Object clone()
        {
            mxCell clone = (mxCell)base.clone();

            clone.setValue(cloneValue());
            clone.setStyle(getStyle());
            clone.setCollapsed(isCollapsed());
            clone.setConnectable(isConnectable());
            clone.setEdge(isEdge());
            clone.setVertex(isVertex());
            clone.setVisible(isVisible());
            clone.setParent(null);
            clone.setSource(null);
            clone.setTarget(null);
            clone.children = null;
            clone.edges = null;

            mxGeometry geometry = getGeometry();

            if (geometry != null)
            {
                clone.setGeometry((mxGeometry)geometry.clone());
            }

            return clone;
        }

        /**
         * Returns a clone of the user object. This implementation clones any XML
         * nodes or otherwise returns the same user object instance.
         */
        protected Object cloneValue()
        {
            Object value = getValue();

            if (value is XmlNode)
            {
                value = ((XmlNode)value).CloneNode(true);
            }

            return value;
        }
    }
}
