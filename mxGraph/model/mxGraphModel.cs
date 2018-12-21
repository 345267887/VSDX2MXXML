using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxGraphModel.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.model
{


    using mxEvent = mxGraph.util.mxEvent;
    using mxEventObject = mxGraph.util.mxEventObject;
    using mxEventSource = mxGraph.util.mxEventSource;
    using mxPoint = mxGraph.util.mxPoint;
    using mxUndoableEdit = mxGraph.util.mxUndoableEdit;

    /// <summary>
    /// Extends mxEventSource to implement a graph model. The graph model acts as
    /// a wrapper around the cells which are in charge of storing the actual graph
    /// datastructure. The model acts as a transactional wrapper with event
    /// notification for all changes, whereas the cells contain the atomic
    /// operations for updating the actual datastructure.
    /// 
    /// Layers:
    /// 
    /// The cell hierarchy in the model must have a top-level root cell which
    /// contains the layers (typically one default layer), which in turn contain the
    /// top-level cells of the layers. This means each cell is contained in a layer.
    /// If no layers are required, then all new cells should be added to the default
    /// layer.
    /// 
    /// Layers are useful for hiding and showing groups of cells, or for placing
    /// groups of cells on top of other cells in the display. To identify a layer,
    /// the <isLayer> function is used. It returns true if the parent of the given
    /// cell is the root of the model.
    /// 
    /// This class fires the following events:
    /// 
    /// mxEvent.CHANGE fires when an undoable edit is dispatched. The <code>edit</code>
    /// property contains the mxUndoableEdit. The <code>changes</code> property
    /// contains the list of undoable changes inside the undoable edit. The changes
    /// property is deprecated, please use edit.getChanges() instead.
    /// 
    /// mxEvent.EXECUTE fires between begin- and endUpdate and after an atomic
    /// change was executed in the model. The <code>change</code> property contains
    /// the atomic change that was executed.
    /// 
    /// mxEvent.BEGIN_UPDATE fires after the updateLevel was incremented in
    /// beginUpdate. This event contains no properties.
    /// 
    /// mxEvent.END_UPDATE fires after the updateLevel was decreased in endUpdate
    /// but before any notification or change dispatching. The <code>edit</code>
    /// property contains the current mxUndoableEdit.
    /// 
    /// mxEvent.BEFORE_UNDO fires before the change is dispatched after the update
    /// level has reached 0 in endUpdate. The <code>edit</code> property contains
    /// the current mxUndoableEdit.
    /// 
    /// mxEvent.UNDO fires after the change was dispatched in endUpdate. The
    /// <code>edit</code> property contains the current mxUndoableEdit.
    /// </summary>
    public class mxGraphModel : mxEventSource, mxIGraphModel
    {

        /// <summary>
        /// Holds the root cell, which in turn contains the cells that represent the
        /// layers of the diagram as child cells. That is, the actual element of the
        /// diagram are supposed to live in the third generation of cells and below.
        /// </summary>
        protected internal mxICell root;

        /// <summary>
        /// Maps from Ids to cells.
        /// </summary>
        protected internal IDictionary<string, object> cells;

        /// <summary>
        /// Specifies if edges should automatically be moved into the nearest common
        /// ancestor of their terminals. Default is true.
        /// </summary>
        protected internal bool maintainEdgeParent = true;

        /// <summary>
        /// Specifies if the model should automatically create Ids for new cells.
        /// Default is true.
        /// </summary>
        protected internal bool createIds = true;

        /// <summary>
        /// Specifies the next Id to be created. Initial value is 0.
        /// </summary>
        protected internal int nextId = 0;

        /// <summary>
        /// Holds the changes for the current transaction. If the transaction is
        /// closed then a new object is created for this variable using
        /// createUndoableEdit.
        /// </summary>
        [NonSerialized]
        protected internal mxUndoableEdit currentEdit;

        /// <summary>
        /// Counter for the depth of nested transactions. Each call to beginUpdate
        /// increments this counter and each call to endUpdate decrements it. When
        /// the counter reaches 0, the transaction is closed and the respective
        /// events are fired. Initial value is 0.
        /// </summary>
        [NonSerialized]
        protected internal int updateLevel = 0;

        /// 
        [NonSerialized]
        protected internal bool endingUpdate = false;

        /// <summary>
        /// Constructs a new empty graph model.
        /// </summary>
        public mxGraphModel() : this(null)
        {
        }

        /// <summary>
        /// Constructs a new graph model. If no root is specified
        /// then a new root mxCell with a default layer is created.
        /// </summary>
        /// <param name="root"> Cell that represents the root cell. </param>
        public mxGraphModel(object root)
        {
            currentEdit = createUndoableEdit();

            if (root != null)
            {
                Root = root;
            }
            else
            {
                clear();
            }
        }

        /// <summary>
        /// Sets a new root using createRoot.
        /// </summary>
        public virtual void clear()
        {
            Root = createRoot();
        }

        /// 
        public virtual int UpdateLevel
        {
            get
            {
                return updateLevel;
            }
        }

        /// <summary>
        /// Creates a new root cell with a default layer (child 0).
        /// </summary>
        public virtual object createRoot()
        {
            mxCell root = new mxCell();
            root.insert(new mxCell());

            return root;
        }

        /// <summary>
        /// Returns the internal lookup table that is used to map from Ids to cells.
        /// </summary>
        public virtual IDictionary<string, object> Cells
        {
            get
            {
                return cells;
            }
        }

        /// <summary>
        /// Returns the cell for the specified Id or null if no cell can be
        /// found for the given Id.
        /// </summary>
        /// <param name="id"> A string representing the Id of the cell. </param>
        /// <returns> Returns the cell for the given Id. </returns>
        public virtual object getCell(string id)
        {
            object result = null;

            if (cells != null)
            {
                result = cells.ContainsKey(id) ? cells[id] : null;
            }
            return result;
        }

        /// <summary>
        /// Returns true if the model automatically update parents of edges so that
        /// the edge is contained in the nearest-common-ancestor of its terminals.
        /// </summary>
        /// <returns> Returns true if the model maintains edge parents. </returns>
        public virtual bool MaintainEdgeParent
        {
            get
            {
                return maintainEdgeParent;
            }
            set
            {
                this.maintainEdgeParent = value;
            }
        }


        /// <summary>
        /// Returns true if the model automatically creates Ids and resolves Id
        /// collisions.
        /// </summary>
        /// <returns> Returns true if the model creates Ids. </returns>
        public virtual bool CreateIds
        {
            get
            {
                return createIds;
            }
            set
            {
                createIds = value;
            }
        }


        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getRoot()
		 */
        public virtual object Root
        {
            get
            {
                return root;
            }
            set
            {
                root = (mxICell)value;
            }
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setRoot(Object)
		 */
        public virtual object setRoot(object root)
        {
            execute(new mxRootChange(this, root));

            return root;
        }

        /// <summary>
        /// Inner callback to change the root of the model and update the internal
        /// datastructures, such as cells and nextId. Returns the previous root.
        /// </summary>
        protected internal virtual object rootChanged(object root)
        {
            object oldRoot = this.root;
            this.root = (mxICell)root;

            // Resets counters and datastructures
            nextId = 0;
            cells = null;
            cellAdded(root);

            return oldRoot;
        }

        /// <summary>
        /// Creates a new undoable edit.
        /// </summary>
        protected internal virtual mxUndoableEdit createUndoableEdit()
        {
            return new mxUndoableEditAnonymousInnerClass(this);
        }

        private class mxUndoableEditAnonymousInnerClass : mxUndoableEdit
        {
            private readonly mxGraphModel outerInstance;

            public mxUndoableEditAnonymousInnerClass(mxGraphModel outerInstance) : base(outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public override void dispatch()
            {
                // LATER: Remove changes property (deprecated)
                ((mxGraphModel)source).fireEvent(new mxEventObject(mxEvent.CHANGE, "edit", this, "changes", changes));
            }
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#cloneCells(Object[], boolean)
		 */
        public virtual object[] cloneCells(object[] cells, bool includeChildren)
        {
            IDictionary<object, object> mapping = new Dictionary<object, object>();
            object[] clones = new object[cells.Length];

            for (int i = 0; i < cells.Length; i++)
            {
                try
                {
                    clones[i] = cloneCell(cells[i], mapping, includeChildren);
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            for (int i = 0; i < cells.Length; i++)
            {
                restoreClone(clones[i], cells[i], mapping);
            }

            return clones;
        }

        /// <summary>
        /// Inner helper method for cloning cells recursively.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected Object cloneCell(Object cell, java.util.Map<Object, Object> mapping, boolean includeChildren) throws CloneNotSupportedException
        protected internal virtual object cloneCell(object cell, IDictionary<object, object> mapping, bool includeChildren)
        {
            if (cell is mxICell)
            {
                mxICell mxc = (mxICell)((mxICell)cell).clone();
                mapping[cell] = mxc;

                if (includeChildren)
                {
                    int childCount = getChildCount(cell);

                    for (int i = 0; i < childCount; i++)
                    {
                        object clone = cloneCell(getChildAt(cell, i), mapping, true);
                        mxc.insert((mxICell)clone);
                    }
                }

                return mxc;
            }

            return null;
        }

        /// <summary>
        /// Inner helper method for restoring the connections in
        /// a network of cloned cells.
        /// </summary>
        protected internal virtual void restoreClone(object clone, object cell, IDictionary<object, object> mapping)
        {
            if (clone is mxICell)
            {
                mxICell mxc = (mxICell)clone;
                object source = getTerminal(cell, true);

                if (source is mxICell)
                {
                    mxICell tmp = (mxICell)mapping[source];

                    if (tmp != null)
                    {
                        tmp.insertEdge(mxc, true);
                    }
                }

                object target = getTerminal(cell, false);

                if (target is mxICell)
                {
                    mxICell tmp = (mxICell)mapping[target];

                    if (tmp != null)
                    {
                        tmp.insertEdge(mxc, false);
                    }
                }
            }

            int childCount = getChildCount(clone);

            for (int i = 0; i < childCount; i++)
            {
                restoreClone(getChildAt(clone, i), getChildAt(cell, i), mapping);
            }
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#isAncestor(Object, Object)
		 */
        public virtual bool isAncestor(object parent, object child)
        {
            while (child != null && child != parent)
            {
                child = getParent(child);
            }

            return child == parent;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#contains(Object)
		 */
        public virtual bool contains(object cell)
        {
            return isAncestor(Root, cell);
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getParent(Object)
		 */
        public virtual object getParent(object child)
        {
            return (child is mxICell) ? ((mxICell)child).Parent : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#add(Object, Object, int)
		 */
        public virtual object add(object parent, object child, int index)
        {
            if (child != parent && parent != null && child != null)
            {
                bool parentChanged = parent != getParent(child);
                execute(new mxChildChange(this, parent, child, index));

                // Maintains the edges parents by moving the edges
                // into the nearest common ancestor of its
                // terminals
                if (maintainEdgeParent && parentChanged)
                {
                    updateEdgeParents(child);
                }
            }

            return child;
        }

        /// <summary>
        /// Invoked after a cell has been added to a parent. This recursively
        /// creates an Id for the new cell and/or resolves Id collisions.
        /// </summary>
        /// <param name="cell"> Cell that has been added. </param>
        protected internal virtual void cellAdded(object cell)
        {
            if (cell is mxICell)
            {
                mxICell mxc = (mxICell)cell;

                if (string.ReferenceEquals(mxc.Id, null) && CreateIds)
                {
                    mxc.Id = createId(cell);
                }

                if (!string.ReferenceEquals(mxc.Id, null))
                {
                    object collision = getCell(mxc.Id);

                    if (collision != cell)
                    {
                        while (collision != null)
                        {
                            mxc.Id = createId(cell);
                            collision = getCell(mxc.Id);
                        }

                        if (cells == null)
                        {
                            cells = new Dictionary<string, object>();
                        }


                        if (cells.ContainsKey(mxc.Id))
                        {
                            cells[mxc.Id] = cell;
                        }
                        else
                        {
                            cells.Add(mxc.Id, cell);
                        }
                    }
                }

                // Makes sure IDs of deleted cells are not reused
                try
                {
                    int id = int.Parse(mxc.Id);
                    nextId = Math.Max(nextId, id + 1);
                }
                catch (System.FormatException)
                {
                    // ignore
                }

                int childCount = mxc.ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    cellAdded(mxc.getChildAt(i));
                }
            }
        }

        /// <summary>
        /// Creates a new Id for the given cell and increments the global counter
        /// for creating new Ids.
        /// </summary>
        /// <param name="cell"> Cell for which a new Id should be created. </param>
        /// <returns> Returns a new Id for the given cell. </returns>
        public virtual string createId(object cell)
        {
            string id = nextId.ToString();
            nextId++;

            return id;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#remove(Object)
		 */
        public virtual object remove(object cell)
        {
            if (cell == root)
            {
                Root = null;
            }
            else if (getParent(cell) != null)
            {
                execute(new mxChildChange(this, null, cell));
            }

            return cell;
        }

        /// <summary>
        /// Invoked after a cell has been removed from the model. This recursively
        /// removes the cell from its terminals and removes the mapping from the Id
        /// to the cell.
        /// </summary>
        /// <param name="cell"> Cell that has been removed. </param>
        protected internal virtual void cellRemoved(object cell)
        {
            if (cell is mxICell)
            {
                mxICell mxc = (mxICell)cell;
                int childCount = mxc.ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    cellRemoved(mxc.getChildAt(i));
                }

                if (cells != null && !string.ReferenceEquals(mxc.Id, null))
                {
                    cells.Remove(mxc.Id);
                }
            }
        }

        /// <summary>
        /// Inner callback to update the parent of a cell using mxCell.insert
        /// on the parent and return the previous parent.
        /// </summary>
        protected internal virtual object parentForCellChanged(object cell, object parent, int index)
        {
            mxICell child = (mxICell)cell;
            mxICell previous = (mxICell)getParent(cell);

            if (parent != null)
            {
                if (parent != previous || previous.getIndex(child) != index)
                {
                    ((mxICell)parent).insert(child, index);
                }
            }
            else if (previous != null)
            {
                int oldIndex = previous.getIndex(child);
                previous.remove(oldIndex);
            }

            // Checks if the previous parent was already in the
            // model and avoids calling cellAdded if it was.
            if (!contains(previous) && parent != null)
            {
                cellAdded(cell);
            }
            else if (parent == null)
            {
                cellRemoved(cell);
            }

            return previous;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getChildCount(Object)
		 */
        public virtual int getChildCount(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).ChildCount : 0;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getChildAt(Object, int)
		 */
        public virtual object getChildAt(object parent, int index)
        {
            return (parent is mxICell) ? ((mxICell)parent).getChildAt(index) : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getTerminal(Object, boolean)
		 */
        public virtual object getTerminal(object edge, bool isSource)
        {
            return (edge is mxICell) ? ((mxICell)edge).getTerminal(isSource) : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setTerminal(Object, Object, boolean)
		 */
        public virtual object setTerminal(object edge, object terminal, bool isSource)
        {
            bool terminalChanged = terminal != getTerminal(edge, isSource);
            execute(new mxTerminalChange(this, edge, terminal, isSource));

            if (maintainEdgeParent && terminalChanged)
            {
                updateEdgeParent(edge, Root);
            }

            return terminal;
        }

        /// <summary>
        /// Inner helper function to update the terminal of the edge using
        /// mxCell.insertEdge and return the previous terminal.
        /// </summary>
        protected internal virtual object terminalForCellChanged(object edge, object terminal, bool isSource)
        {
            mxICell previous = (mxICell)getTerminal(edge, isSource);

            if (terminal != null)
            {
                ((mxICell)terminal).insertEdge((mxICell)edge, isSource);
            }
            else if (previous != null)
            {
                previous.removeEdge((mxICell)edge, isSource);
            }

            return previous;
        }

        /// <summary>
        /// Updates the parents of the edges connected to the given cell and all its
        /// descendants so that each edge is contained in the nearest common
        /// ancestor.
        /// </summary>
        /// <param name="cell"> Cell whose edges should be checked and updated. </param>
        public virtual void updateEdgeParents(object cell)
        {
            updateEdgeParents(cell, Root);
        }

        /// <summary>
        /// Updates the parents of the edges connected to the given cell and all its
        /// descendants so that the edge is contained in the nearest-common-ancestor.
        /// </summary>
        /// <param name="cell"> Cell whose edges should be checked and updated. </param>
        /// <param name="root"> Root of the cell hierarchy that contains all cells. </param>
        public virtual void updateEdgeParents(object cell, object root)
        {
            // Updates edges on children first
            int childCount = getChildCount(cell);

            for (int i = 0; i < childCount; i++)
            {
                object child = getChildAt(cell, i);
                updateEdgeParents(child, root);
            }

            // Updates the parents of all connected edges
            int edgeCount = getEdgeCount(cell);
            IList<object> edges = new List<object>(edgeCount);

            for (int i = 0; i < edgeCount; i++)
            {
                edges.Add(getEdgeAt(cell, i));
            }

            IEnumerator<object> it = edges.GetEnumerator();

            while (it.MoveNext())
            {
                object edge = it.Current;

                // Updates edge parent if edge and child have
                // a common root node (does not need to be the
                // model root node)
                if (isAncestor(root, edge))
                {
                    updateEdgeParent(edge, root);
                }
            }
        }

        /// <summary>
        /// Inner helper method to update the parent of the specified edge to the
        /// nearest-common-ancestor of its two terminals.
        /// </summary>
        /// <param name="edge"> Specifies the edge to be updated. </param>
        /// <param name="root"> Current root of the model. </param>
        public virtual void updateEdgeParent(object edge, object root)
        {
            object source = getTerminal(edge, true);
            object target = getTerminal(edge, false);
            object cell = null;

            if (isAncestor(root, source) && isAncestor(root, target))
            {
                if (source == target)
                {
                    cell = getParent(source);
                }
                else
                {
                    cell = getNearestCommonAncestor(source, target);
                }

                // Keeps the edge in the same layer
                if (cell != null && (getParent(cell) != root || isAncestor(cell, edge)) && getParent(edge) != cell)
                {
                    mxGeometry geo = getGeometry(edge);

                    if (geo != null)
                    {
                        mxPoint origin1 = getOrigin(getParent(edge));
                        mxPoint origin2 = getOrigin(cell);

                        double dx = origin2.X - origin1.X;
                        double dy = origin2.Y - origin1.Y;

                        geo = (mxGeometry)geo.clone();
                        geo.translate(-dx, -dy);
                        setGeometry(edge, geo);
                    }

                    add(cell, edge, getChildCount(cell));
                }
            }
        }

        /// <summary>
        /// Returns the absolute, accumulated origin for the children inside the
        /// given parent. 
        /// </summary>
        public virtual mxPoint getOrigin(object cell)
        {
            mxPoint result = null;

            if (cell != null)
            {
                result = getOrigin(getParent(cell));

                if (!isEdge(cell))
                {
                    mxGeometry geo = getGeometry(cell);

                    if (geo != null)
                    {
                        result.X = result.X + geo.X;
                        result.Y = result.Y + geo.Y;
                    }
                }
            }
            else
            {
                result = new mxPoint();
            }

            return result;
        }

        /// <summary>
        /// Returns the nearest common ancestor for the specified cells.
        /// </summary>
        /// <param name="cell1"> Cell that specifies the first cell in the tree. </param>
        /// <param name="cell2"> Cell that specifies the second cell in the tree. </param>
        /// <returns> Returns the nearest common ancestor of the given cells. </returns>
        public virtual object getNearestCommonAncestor(object cell1, object cell2)
        {
            if (cell1 != null && cell2 != null)
            {
                // Creates the cell path for the second cell
                string path = mxCellPath.create((mxICell)cell2);

                if (!string.ReferenceEquals(path, null) && path.Length > 0)
                {
                    // Bubbles through the ancestors of the first
                    // cell to find the nearest common ancestor.
                    object cell = cell1;
                    string current = mxCellPath.create((mxICell)cell);

                    while (cell != null)
                    {
                        object parent = getParent(cell);

                        // Checks if the cell path is equal to the beginning
                        // of the given cell path
                        if (path.IndexOf(current + mxCellPath.PATH_SEPARATOR, StringComparison.Ordinal) == 0 && parent != null)
                        {
                            return cell;
                        }

                        current = mxCellPath.getParentPath(current);
                        cell = parent;
                    }
                }
            }

            return null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getEdgeCount(Object)
		 */
        public virtual int getEdgeCount(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).EdgeCount : 0;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getEdgeAt(Object, int)
		 */
        public virtual object getEdgeAt(object parent, int index)
        {
            return (parent is mxICell) ? ((mxICell)parent).getEdgeAt(index) : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#isVertex(Object)
		 */
        public virtual bool isVertex(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Vertex : false;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#isEdge(Object)
		 */
        public virtual bool isEdge(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Edge : false;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#isConnectable(Object)
		 */
        public virtual bool isConnectable(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Connectable : true;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getValue(Object)
		 */
        public virtual object getValue(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Value : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setValue(Object, Object)
		 */
        public virtual object setValue(object cell, object value)
        {
            execute(new mxValueChange(this, cell, value));

            return value;
        }

        /// <summary>
        /// Inner callback to update the user object of the given mxCell
        /// using mxCell.setValue and return the previous value,
        /// that is, the return value of mxCell.getValue.
        /// </summary>
        protected internal virtual object valueForCellChanged(object cell, object value)
        {
            object oldValue = ((mxICell)cell).Value;
            ((mxICell)cell).Value = value;

            return oldValue;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getGeometry(Object)
		 */
        public virtual mxGeometry getGeometry(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Geometry : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setGeometry(Object, mxGeometry)
		 */
        public virtual mxGeometry setGeometry(object cell, mxGeometry geometry)
        {
            if (geometry != getGeometry(cell))
            {
                execute(new mxGeometryChange(this, cell, geometry));
            }

            return geometry;
        }

        /// <summary>
        /// Inner callback to update the mxGeometry of the given mxCell using
        /// mxCell.setGeometry and return the previous mxGeometry.
        /// </summary>
        protected internal virtual mxGeometry geometryForCellChanged(object cell, mxGeometry geometry)
        {
            mxGeometry previous = getGeometry(cell);
            ((mxICell)cell).Geometry = geometry;

            return previous;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#getStyle(Object)
		 */
        public virtual string getStyle(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Style : null;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setStyle(Object, String)
		 */
        public virtual string setStyle(object cell, string style)
        {
            if (!string.ReferenceEquals(style, getStyle(cell)))
            {
                execute(new mxStyleChange(this, cell, style));
            }

            return style;
        }

        /// <summary>
        /// Inner callback to update the style of the given mxCell
        /// using mxCell.setStyle and return the previous style.
        /// </summary>
        protected internal virtual string styleForCellChanged(object cell, string style)
        {
            string previous = getStyle(cell);
            ((mxICell)cell).Style = style;

            return previous;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#isCollapsed(Object)
		 */
        public virtual bool isCollapsed(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Collapsed : false;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setCollapsed(Object, boolean)
		 */
        public virtual bool setCollapsed(object cell, bool collapsed)
        {
            if (collapsed != isCollapsed(cell))
            {
                execute(new mxCollapseChange(this, cell, collapsed));
            }

            return collapsed;
        }

        /// <summary>
        /// Inner callback to update the collapsed state of the
        /// given mxCell using mxCell.setCollapsed and return
        /// the previous collapsed state.
        /// </summary>
        protected internal virtual bool collapsedStateForCellChanged(object cell, bool collapsed)
        {
            bool previous = isCollapsed(cell);
            ((mxICell)cell).Collapsed = collapsed;

            return previous;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#isVisible(Object)
		 */
        public virtual bool isVisible(object cell)
        {
            return (cell is mxICell) ? ((mxICell)cell).Visible : false;
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#setVisible(Object, boolean)
		 */
        public virtual bool setVisible(object cell, bool visible)
        {
            if (visible != isVisible(cell))
            {
                execute(new mxVisibleChange(this, cell, visible));
            }

            return visible;
        }

        /// <summary>
        /// Sets the visible state of the given mxCell using mxVisibleChange and
        /// adds the change to the current transaction.
        /// </summary>
        protected internal virtual bool visibleStateForCellChanged(object cell, bool visible)
        {
            bool previous = isVisible(cell);
            ((mxICell)cell).Visible = visible;

            return previous;
        }

        /// <summary>
        /// Executes the given atomic change and adds it to the current edit.
        /// </summary>
        /// <param name="change"> Atomic change to be executed. </param>
        public virtual void execute(mxIGraphModel_mxAtomicGraphModelChange change)
        {
            change.execute();
            beginUpdate();
            currentEdit.add(change);
            fireEvent(new mxEventObject(mxEvent.EXECUTE, "change", change));
            endUpdate();
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#beginUpdate()
		 */
        public virtual void beginUpdate()
        {
            updateLevel++;
            fireEvent(new mxEventObject(mxEvent.BEGIN_UPDATE));
        }

        /* (non-Javadoc)
		 * @see com.mxgraph.model.mxIGraphModel#endUpdate()
		 */
        public virtual void endUpdate()
        {
            updateLevel--;

            if (!endingUpdate)
            {
                endingUpdate = updateLevel == 0;
                fireEvent(new mxEventObject(mxEvent.END_UPDATE, "edit", currentEdit));

                try
                {
                    if (endingUpdate && !currentEdit.Empty)
                    {
                        fireEvent(new mxEventObject(mxEvent.BEFORE_UNDO, "edit", currentEdit));
                        mxUndoableEdit tmp = currentEdit;
                        currentEdit = createUndoableEdit();
                        tmp.dispatch();
                        fireEvent(new mxEventObject(mxEvent.UNDO, "edit", tmp));
                    }
                }
                finally
                {
                    endingUpdate = false;
                }
            }
        }

        /// <summary>
        /// Merges the children of the given cell into the given target cell inside
        /// this model. All cells are cloned unless there is a corresponding cell in
        /// the model with the same id, in which case the source cell is ignored and
        /// all edges are connected to the corresponding cell in this model. Edges
        /// are considered to have no identity and are always cloned unless the
        /// cloneAllEdges flag is set to false, in which case edges with the same
        /// id in the target model are reconnected to reflect the terminals of the
        /// source edges.
        /// </summary>
        /// <param name="from"> </param>
        /// <param name="to"> </param>
        /// <param name="cloneAllEdges"> </param>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void mergeChildren(mxICell from, mxICell to, boolean cloneAllEdges) throws CloneNotSupportedException
        public virtual void mergeChildren(mxICell from, mxICell to, bool cloneAllEdges)
        {
            beginUpdate();
            try
            {
                Dictionary<object, object> mapping = new Dictionary<object, object>();
                mergeChildrenImpl(from, to, cloneAllEdges, mapping);

                // Post-processes all edges in the mapping and
                // reconnects the terminals to the corresponding
                // cells in the target model
                IEnumerator<object> it = mapping.Keys.GetEnumerator();

                while (it.MoveNext())
                {
                    object edge = it.Current;
                    object cell = mapping[edge];
                    object terminal = getTerminal(edge, true);

                    if (terminal != null)
                    {
                        terminal = mapping[terminal];
                        setTerminal(cell, terminal, true);
                    }

                    terminal = getTerminal(edge, false);

                    if (terminal != null)
                    {
                        terminal = mapping[terminal];
                        setTerminal(cell, terminal, false);
                    }
                }
            }
            finally
            {
                endUpdate();
            }
        }

        /// <summary>
        /// Clones the children of the source cell into the given target cell in
        /// this model and adds an entry to the mapping that maps from the source
        /// cell to the target cell with the same id or the clone of the source cell
        /// that was inserted into this model.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void mergeChildrenImpl(mxICell from, mxICell to, boolean cloneAllEdges, java.util.Hashtable<Object, Object> mapping) throws CloneNotSupportedException
        protected internal virtual void mergeChildrenImpl(mxICell from, mxICell to, bool cloneAllEdges, Dictionary<object, object> mapping)
        {
            beginUpdate();
            try
            {
                int childCount = from.ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    object child = from.getChildAt(i);

                    if (child is mxICell)
                    {
                        mxICell cell = (mxICell)child;
                        string id = cell.Id;
                        mxICell target = (mxICell)((!string.ReferenceEquals(id, null) && (!isEdge(cell) || !cloneAllEdges)) ? getCell(id) : null);

                        // Clones and adds the child if no cell exists for the id
                        if (target == null)
                        {
                            mxCell clone = (mxCell)cell.clone();
                            clone.Id = id;

                            // Do *NOT* use model.add as this will move the edge away
                            // from the parent in updateEdgeParent if maintainEdgeParent
                            // is enabled in the target model
                            target = to.insert(clone);
                            cellAdded(target);
                        }

                        // Stores the mapping for later reconnecting edges
                        mapping[cell] = target;

                        // Recurses
                        mergeChildrenImpl(cell, target, cloneAllEdges, mapping);
                    }
                }
            }
            finally
            {
                endUpdate();
            }
        }

        /// <summary>
        /// Returns the number of incoming or outgoing edges.
        /// </summary>
        /// <param name="model"> Graph model that contains the connection data. </param>
        /// <param name="cell"> Cell whose edges should be counted. </param>
        /// <param name="outgoing"> Boolean that specifies if the number of outgoing or
        /// incoming edges should be returned. </param>
        /// <returns> Returns the number of incoming or outgoing edges. </returns>
        public static int getDirectedEdgeCount(mxIGraphModel model, object cell, bool outgoing)
        {
            return getDirectedEdgeCount(model, cell, outgoing, null);
        }

        /// <summary>
        /// Returns the number of incoming or outgoing edges, ignoring the given
        /// edge.
        /// </summary>
        /// <param name="model"> Graph model that contains the connection data. </param>
        /// <param name="cell"> Cell whose edges should be counted. </param>
        /// <param name="outgoing"> Boolean that specifies if the number of outgoing or
        /// incoming edges should be returned. </param>
        /// <param name="ignoredEdge"> Object that represents an edge to be ignored. </param>
        /// <returns> Returns the number of incoming or outgoing edges. </returns>
        public static int getDirectedEdgeCount(mxIGraphModel model, object cell, bool outgoing, object ignoredEdge)
        {
            int count = 0;
            int edgeCount = model.getEdgeCount(cell);

            for (int i = 0; i < edgeCount; i++)
            {
                object edge = model.getEdgeAt(cell, i);

                if (edge != ignoredEdge && model.getTerminal(edge, outgoing) == cell)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns all edges connected to this cell including loops.
        /// </summary>
        /// <param name="model"> Model that contains the connection information. </param>
        /// <param name="cell"> Cell whose connections should be returned. </param>
        /// <returns> Returns the array of connected edges for the given cell. </returns>
        public static object[] getEdges(mxIGraphModel model, object cell)
        {
            return getEdges(model, cell, true, true, true);
        }

        /// <summary>
        /// Returns all edges connected to this cell without loops.
        /// </summary>
        /// <param name="model"> Model that contains the connection information. </param>
        /// <param name="cell"> Cell whose connections should be returned. </param>
        /// <returns> Returns the connected edges for the given cell. </returns>
        public static object[] getConnections(mxIGraphModel model, object cell)
        {
            return getEdges(model, cell, true, true, false);
        }

        /// <summary>
        /// Returns the incoming edges of the given cell without loops.
        /// </summary>
        /// <param name="model"> Graphmodel that contains the edges. </param>
        /// <param name="cell"> Cell whose incoming edges should be returned. </param>
        /// <returns> Returns the incoming edges for the given cell. </returns>
        public static object[] getIncomingEdges(mxIGraphModel model, object cell)
        {
            return getEdges(model, cell, true, false, false);
        }

        /// <summary>
        /// Returns the outgoing edges of the given cell without loops.
        /// </summary>
        /// <param name="model"> Graphmodel that contains the edges. </param>
        /// <param name="cell"> Cell whose outgoing edges should be returned. </param>
        /// <returns> Returns the outgoing edges for the given cell. </returns>
        public static object[] getOutgoingEdges(mxIGraphModel model, object cell)
        {
            return getEdges(model, cell, false, true, false);
        }

        /// <summary>
        /// Returns all distinct edges connected to this cell.
        /// </summary>
        /// <param name="model"> Model that contains the connection information. </param>
        /// <param name="cell"> Cell whose connections should be returned. </param>
        /// <param name="incoming"> Specifies if incoming edges should be returned. </param>
        /// <param name="outgoing"> Specifies if outgoing edges should be returned. </param>
        /// <param name="includeLoops"> Specifies if loops should be returned. </param>
        /// <returns> Returns the array of connected edges for the given cell. </returns>
        public static object[] getEdges(mxIGraphModel model, object cell, bool incoming, bool outgoing, bool includeLoops)
        {
            int edgeCount = model.getEdgeCount(cell);
            List<object> result = new List<object>(edgeCount);

            for (int i = 0; i < edgeCount; i++)
            {
                object edge = model.getEdgeAt(cell, i);
                object source = model.getTerminal(edge, true);
                object target = model.getTerminal(edge, false);

                if (includeLoops || ((source != target) && ((incoming && target == cell) || (outgoing && source == cell))))
                {
                    result.Add(edge);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns all edges from the given source to the given target.
        /// </summary>
        /// <param name="model"> The graph model that contains the graph. </param>
        /// <param name="source"> Object that defines the source cell. </param>
        /// <param name="target"> Object that defines the target cell. </param>
        /// <returns> Returns all edges from source to target. </returns>
        public static object[] getEdgesBetween(mxIGraphModel model, object source, object target)
        {
            return getEdgesBetween(model, source, target, false);
        }

        /// <summary>
        /// Returns all edges between the given source and target pair. If directed
        /// is true, then only edges from the source to the target are returned,
        /// otherwise, all edges between the two cells are returned.
        /// </summary>
        /// <param name="model"> The graph model that contains the graph. </param>
        /// <param name="source"> Object that defines the source cell. </param>
        /// <param name="target"> Object that defines the target cell. </param>
        /// <param name="directed"> Boolean that specifies if the direction of the edge
        /// should be taken into account. </param>
        /// <returns> Returns all edges between the given source and target. </returns>
        public static object[] getEdgesBetween(mxIGraphModel model, object source, object target, bool directed)
        {
            int tmp1 = model.getEdgeCount(source);
            int tmp2 = model.getEdgeCount(target);

            // Assumes the source has less connected edges
            object terminal = source;
            int edgeCount = tmp1;

            // Uses the smaller array of connected edges
            // for searching the edge
            if (tmp2 < tmp1)
            {
                edgeCount = tmp2;
                terminal = target;
            }

            List<object> result = new List<object>(edgeCount);

            // Checks if the edge is connected to the correct
            // cell and returns the first match
            for (int i = 0; i < edgeCount; i++)
            {
                object edge = model.getEdgeAt(terminal, i);
                object src = model.getTerminal(edge, true);
                object trg = model.getTerminal(edge, false);
                bool isSource = src == source;

                if (isSource && trg == target || (!directed && model.getTerminal(edge, !isSource) == target))
                {
                    result.Add(edge);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns all opposite cells of terminal for the given edges.
        /// </summary>
        /// <param name="model"> Model that contains the connection information. </param>
        /// <param name="edges"> Array of edges to be examined. </param>
        /// <param name="terminal"> Cell that specifies the known end of the edges. </param>
        /// <returns> Returns the opposite cells of the given terminal. </returns>
        public static object[] getOpposites(mxIGraphModel model, object[] edges, object terminal)
        {
            return getOpposites(model, edges, terminal, true, true);
        }

        /// <summary>
        /// Returns all opposite vertices wrt terminal for the given edges, only
        /// returning sources and/or targets as specified. The result is returned as
        /// an array of mxCells.
        /// </summary>
        /// <param name="model"> Model that contains the connection information. </param>
        /// <param name="edges"> Array of edges to be examined. </param>
        /// <param name="terminal"> Cell that specifies the known end of the edges. </param>
        /// <param name="sources"> Boolean that specifies if source terminals should
        /// be contained in the result. Default is true. </param>
        /// <param name="targets"> Boolean that specifies if target terminals should
        /// be contained in the result. Default is true. </param>
        /// <returns> Returns the array of opposite terminals for the given edges. </returns>
        public static object[] getOpposites(mxIGraphModel model, object[] edges, object terminal, bool sources, bool targets)
        {
            List<object> terminals = new List<object>();

            if (edges != null)
            {
                for (int i = 0; i < edges.Length; i++)
                {
                    object source = model.getTerminal(edges[i], true);
                    object target = model.getTerminal(edges[i], false);

                    // Checks if the terminal is the source of
                    // the edge and if the target should be
                    // stored in the result
                    if (targets && source == terminal && target != null && target != terminal)
                    {
                        terminals.Add(target);
                    }

                    // Checks if the terminal is the taget of
                    // the edge and if the source should be
                    // stored in the result
                    else if (sources && target == terminal && source != null && source != terminal)
                    {
                        terminals.Add(source);
                    }
                }
            }

            return terminals.ToArray();
        }

        /// <summary>
        /// Sets the source and target of the given edge in a single atomic change.
        /// </summary>
        /// <param name="edge"> Cell that specifies the edge. </param>
        /// <param name="source"> Cell that specifies the new source terminal. </param>
        /// <param name="target"> Cell that specifies the new target terminal. </param>
        public static void setTerminals(mxIGraphModel model, object edge, object source, object target)
        {
            model.beginUpdate();
            try
            {
                model.setTerminal(edge, source, true);
                model.setTerminal(edge, target, false);
            }
            finally
            {
                model.endUpdate();
            }
        }

        /// <summary>
        /// Returns all children of the given cell regardless of their type.
        /// </summary>
        /// <param name="model"> Model that contains the hierarchical information. </param>
        /// <param name="parent"> Cell whose child vertices or edges should be returned. </param>
        /// <returns> Returns the child vertices and/or edges of the given parent. </returns>
        public static object[] getChildren(mxIGraphModel model, object parent)
        {
            return getChildCells(model, parent, false, false);
        }

        /// <summary>
        /// Returns the child vertices of the given parent.
        /// </summary>
        /// <param name="model"> Model that contains the hierarchical information. </param>
        /// <param name="parent"> Cell whose child vertices should be returned. </param>
        /// <returns> Returns the child vertices of the given parent. </returns>
        public static object[] getChildVertices(mxIGraphModel model, object parent)
        {
            return getChildCells(model, parent, true, false);
        }

        /// <summary>
        /// Returns the child edges of the given parent.
        /// </summary>
        /// <param name="model"> Model that contains the hierarchical information. </param>
        /// <param name="parent"> Cell whose child edges should be returned. </param>
        /// <returns> Returns the child edges of the given parent. </returns>
        public static object[] getChildEdges(mxIGraphModel model, object parent)
        {
            return getChildCells(model, parent, false, true);
        }

        /// <summary>
        /// Returns the children of the given cell that are vertices and/or edges
        /// depending on the arguments. If both arguments are false then all
        /// children are returned regardless of their type.
        /// </summary>
        /// <param name="model"> Model that contains the hierarchical information. </param>
        /// <param name="parent"> Cell whose child vertices or edges should be returned. </param>
        /// <param name="vertices"> Boolean indicating if child vertices should be returned. </param>
        /// <param name="edges"> Boolean indicating if child edges should be returned. </param>
        /// <returns> Returns the child vertices and/or edges of the given parent. </returns>
        public static object[] getChildCells(mxIGraphModel model, object parent, bool vertices, bool edges)
        {
            int childCount = model.getChildCount(parent);
            List<object> result = new List<object>(childCount);

            for (int i = 0; i < childCount; i++)
            {
                object child = model.getChildAt(parent, i);

                if ((!edges && !vertices) || (edges && model.isEdge(child)) || (vertices && model.isVertex(child)))
                {
                    result.Add(child);
                }
            }

            return result.ToArray();
        }

        /// 
        public static object[] getParents(mxIGraphModel model, object[] cells)
        {
            List<object> parents = new List<object>();

            if (cells != null)
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    object parent = model.getParent(cells[i]);

                    if (parent != null)
                    {
                        parents.Add(parent);
                    }
                }
            }

            return parents.ToArray();
        }

        /// 
        public static object[] filterCells(object[] cells, Filter filter)
        {
            List<object> result = null;

            if (cells != null)
            {
                result = new List<object>(cells.Length);

                for (int i = 0; i < cells.Length; i++)
                {
                    if (filter.filter(cells[i]))
                    {
                        result.Add(cells[i]);
                    }
                }
            }

            return (result != null) ? result.ToArray() : null;
        }

        /// <summary>
        /// Returns a all descendants of the given cell and the cell itself
        /// as a collection.
        /// </summary>
        public static ICollection<object> getDescendants(mxIGraphModel model, object parent)
        {
            return filterDescendants(model, null, parent);
        }

        /// <summary>
        /// Creates a collection of cells using the visitor pattern.
        /// </summary>
        public static ICollection<object> filterDescendants(mxIGraphModel model, Filter filter)
        {
            return filterDescendants(model, filter, model.Root);
        }

        /// <summary>
        /// Creates a collection of cells using the visitor pattern.
        /// </summary>
        public static ICollection<object> filterDescendants(mxIGraphModel model, Filter filter, object parent)
        {
            IList<object> result = new List<object>();

            if (filter == null || filter.filter(parent))
            {
                result.Add(parent);
            }

            int childCount = model.getChildCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                object child = model.getChildAt(parent, i);
                ((List<object>)result).AddRange(filterDescendants(model, filter, child));
            }

            return result;
        }

        /// <summary>
        /// Function: getTopmostCells
        /// 
        /// Returns the topmost cells of the hierarchy in an array that contains no
        /// desceandants for each <mxCell> that it contains. Duplicates should be
        /// removed in the cells array to improve performance.
        /// 
        /// Parameters:
        /// 
        /// cells - Array of <mxCells> whose topmost ancestors should be returned.
        /// </summary>
        public static object[] getTopmostCells(mxIGraphModel model, object[] cells)
        {
            ISet<object> hash = new HashSet<object>();
            foreach (var item in cells)
            {
                hash.Add(item);
            }

            List<object> result = new List<object>(cells.Length);

            for (int i = 0; i < cells.Length; i++)
            {
                object cell = cells[i];
                bool topmost = true;
                object parent = model.getParent(cell);

                while (parent != null)
                {
                    if (hash.Contains(parent))
                    {
                        topmost = false;
                        break;
                    }

                    parent = model.getParent(parent);
                }

                if (topmost)
                {
                    result.Add(cell);
                }
            }

            return result.ToArray();
        }

        //
        // Visitor patterns
        //

        /// 
        public interface Filter
        {

            /// 
            bool filter(object cell);
        }

        //
        // Atomic changes
        //

        public class mxRootChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// <summary>
            /// Holds the new and previous root cell.
            /// </summary>
            protected internal object root, previous;

            /// 
            public mxRootChange() : this(null, null)
            {
            }

            /// 
            public mxRootChange(mxGraphModel model, object root) : base(model)
            {
                this.root = root;
                previous = root;
            }


            /// <returns> the root </returns>
            public virtual object Root
            {
                set
                {
                    root = value;
                }
                get
                {
                    return root;
                }
            }

            /// 
            public virtual object Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                root = previous;
                previous = ((mxGraphModel)model).rootChanged(previous);
            }

        }

        public class mxChildChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object parent, previous, child;

            /// 
            protected internal int index, previousIndex;

            /// 
            public mxChildChange() : this(null, null, null, 0)
            {
            }

            /// 
            public mxChildChange(mxGraphModel model, object parent, object child) : this(model, parent, child, 0)
            {
            }

            /// 
            public mxChildChange(mxGraphModel model, object parent, object child, int index) : base(model)
            {
                this.parent = parent;
                previous = this.parent;
                this.child = child;
                this.index = index;
                previousIndex = index;
            }

            /// 
            public virtual object Parent
            {
                set
                {
                    parent = value;
                }
                get
                {
                    return parent;
                }
            }


            /// 
            public virtual object Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// 
            public virtual object Child
            {
                set
                {
                    child = value;
                }
                get
                {
                    return child;
                }
            }


            /// 
            public virtual int Index
            {
                set
                {
                    index = value;
                }
                get
                {
                    return index;
                }
            }


            /// 
            public virtual int PreviousIndex
            {
                set
                {
                    previousIndex = value;
                }
                get
                {
                    return previousIndex;
                }
            }


            /// <summary>
            /// Gets the source or target terminal field for the given
            /// edge even if the edge is not stored as an incoming or
            /// outgoing edge in the respective terminal.
            /// </summary>
            protected internal virtual object getTerminal(object edge, bool source)
            {
                return model.getTerminal(edge, source);
            }

            /// <summary>
            /// Sets the source or target terminal field for the given edge
            /// without inserting an incoming or outgoing edge in the
            /// respective terminal.
            /// </summary>
            protected internal virtual void setTerminal(object edge, object terminal, bool source)
            {
                ((mxICell)edge).setTerminal((mxICell)terminal, source);
            }

            /// 
            protected internal virtual void connect(object cell, bool isConnect)
            {
                object source = getTerminal(cell, true);
                object target = getTerminal(cell, false);

                if (source != null)
                {
                    if (isConnect)
                    {
                        ((mxGraphModel)model).terminalForCellChanged(cell, source, true);
                    }
                    else
                    {
                        ((mxGraphModel)model).terminalForCellChanged(cell, null, true);
                    }
                }

                if (target != null)
                {
                    if (isConnect)
                    {
                        ((mxGraphModel)model).terminalForCellChanged(cell, target, false);
                    }
                    else
                    {
                        ((mxGraphModel)model).terminalForCellChanged(cell, null, false);
                    }
                }

                // Stores the previous terminals in the edge
                setTerminal(cell, source, true);
                setTerminal(cell, target, false);

                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    connect(model.getChildAt(cell, i), isConnect);
                }
            }

            /// <summary>
            /// Returns the index of the given child inside the given parent.
            /// </summary>
            protected internal virtual int getChildIndex(object parent, object child)
            {
                return (parent is mxICell && child is mxICell) ? ((mxICell)parent).getIndex((mxICell)child) : 0;
            }

            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                object tmp = model.getParent(child);
                int tmp2 = getChildIndex(tmp, child);

                if (previous == null)
                {
                    connect(child, false);
                }

                tmp = ((mxGraphModel)model).parentForCellChanged(child, previous, previousIndex);

                if (previous != null)
                {
                    connect(child, true);
                }

                parent = previous;
                previous = tmp;
                index = previousIndex;
                previousIndex = tmp2;
            }

        }

        public class mxTerminalChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object cell, terminal, previous;

            /// 
            protected internal bool source;

            /// 
            public mxTerminalChange() : this(null, null, null, false)
            {
            }

            /// 
            public mxTerminalChange(mxGraphModel model, object cell, object terminal, bool source) : base(model)
            {
                this.cell = cell;
                this.terminal = terminal;
                this.previous = this.terminal;
                this.source = source;
            }

            /// 
            public virtual object Cell
            {
                set
                {
                    cell = value;
                }
                get
                {
                    return cell;
                }
            }


            /// 
            public virtual object Terminal
            {
                set
                {
                    terminal = value;
                }
                get
                {
                    return terminal;
                }
            }


            /// 
            public virtual object Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// 
            public virtual bool Source
            {
                set
                {
                    source = value;
                }
                get
                {
                    return source;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                terminal = previous;
                previous = ((mxGraphModel)model).terminalForCellChanged(cell, previous, source);
            }

        }

        public class mxValueChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object cell, value, previous;

            /// 
            public mxValueChange() : this(null, null, null)
            {
            }

            /// 
            public mxValueChange(mxGraphModel model, object cell, object value) : base(model)
            {
                this.cell = cell;
                this.value = value;
                this.previous = this.value;
            }

            /// 
            public virtual object Cell
            {
                set
                {
                    cell = value;
                }
                get
                {
                    return cell;
                }
            }


            /// 
            public virtual object Value
            {
                set
                {
                    this.value = value;
                }
                get
                {
                    return value;
                }
            }


            /// 
            public virtual object Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                value = previous;
                previous = ((mxGraphModel)model).valueForCellChanged(cell, previous);
            }

        }

        public class mxStyleChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object cell;

            /// 
            protected internal string style, previous;

            /// 
            public mxStyleChange() : this(null, null, null)
            {
            }

            /// 
            public mxStyleChange(mxGraphModel model, object cell, string style) : base(model)
            {
                this.cell = cell;
                this.style = style;
                this.previous = this.style;
            }

            /// 
            public virtual object Cell
            {
                set
                {
                    cell = value;
                }
                get
                {
                    return cell;
                }
            }


            /// 
            public virtual string Style
            {
                set
                {
                    style = value;
                }
                get
                {
                    return style;
                }
            }


            /// 
            public virtual string Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                style = previous;
                previous = ((mxGraphModel)model).styleForCellChanged(cell, previous);
            }

        }

        public class mxGeometryChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object cell;

            /// 
            protected internal mxGeometry geometry, previous;

            /// 
            public mxGeometryChange() : this(null, null, null)
            {
            }

            /// 
            public mxGeometryChange(mxGraphModel model, object cell, mxGeometry geometry) : base(model)
            {
                this.cell = cell;
                this.geometry = geometry;
                this.previous = this.geometry;
            }

            /// 
            public virtual object Cell
            {
                set
                {
                    cell = value;
                }
                get
                {
                    return cell;
                }
            }


            /// 
            public virtual mxGeometry Geometry
            {
                set
                {
                    geometry = value;
                }
                get
                {
                    return geometry;
                }
            }


            /// 
            public virtual mxGeometry Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                geometry = previous;
                previous = ((mxGraphModel)model).geometryForCellChanged(cell, previous);
            }

        }

        public class mxCollapseChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object cell;

            /// 
            protected internal bool collapsed, previous;

            /// 
            public mxCollapseChange() : this(null, null, false)
            {
            }

            /// 
            public mxCollapseChange(mxGraphModel model, object cell, bool collapsed) : base(model)
            {
                this.cell = cell;
                this.collapsed = collapsed;
                this.previous = this.collapsed;
            }

            /// 
            public virtual object Cell
            {
                set
                {
                    cell = value;
                }
                get
                {
                    return cell;
                }
            }


            /// 
            public virtual bool Collapsed
            {
                set
                {
                    collapsed = value;
                }
                get
                {
                    return collapsed;
                }
            }


            /// 
            public virtual bool Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                collapsed = previous;
                previous = ((mxGraphModel)model).collapsedStateForCellChanged(cell, previous);
            }

        }

        public class mxVisibleChange : mxIGraphModel_mxAtomicGraphModelChange
        {

            /// 
            protected internal object cell;

            /// 
            protected internal bool visible, previous;

            /// 
            public mxVisibleChange() : this(null, null, false)
            {
            }

            /// 
            public mxVisibleChange(mxGraphModel model, object cell, bool visible) : base(model)
            {
                this.cell = cell;
                this.visible = visible;
                this.previous = this.visible;
            }

            /// 
            public virtual object Cell
            {
                set
                {
                    cell = value;
                }
                get
                {
                    return cell;
                }
            }


            /// 
            public virtual bool Visible
            {
                set
                {
                    visible = value;
                }
                get
                {
                    return visible;
                }
            }


            /// 
            public virtual bool Previous
            {
                set
                {
                    previous = value;
                }
                get
                {
                    return previous;
                }
            }


            /// <summary>
            /// Changes the root of the model.
            /// </summary>
            public override void execute()
            {
                visible = previous;
                previous = ((mxGraphModel)model).visibleStateForCellChanged(cell, previous);
            }

        }

    }

}