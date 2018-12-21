using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    using Element = System.Xml.XmlDocument;
    

    /// <summary>
    /// Implements a graph object that allows to create diagrams from a graph model
    /// and stylesheet.
    /// 
    /// <h3>Images</h3>
    /// To create an image from a graph, use the following code for a given
    /// XML document (doc) and File (file):
    /// 
    /// <code>
    /// Image img = mxCellRenderer.createBufferedImage(
    /// 		graph, null, 1, Color.WHITE, false, null);
    /// ImageIO.write(img, "png", file);
    /// </code>
    /// 
    /// If the XML is given as a string rather than a document, the document can
    /// be obtained using mxUtils.parse.
    /// 
    /// This class fires the following events:
    /// 
    /// mxEvent.ROOT fires if the root in the model has changed. This event has no
    /// properties.
    /// 
    /// mxEvent.ALIGN_CELLS fires between begin- and endUpdate in alignCells. The
    /// <code>cells</code> and <code>align</code> properties contain the respective
    /// arguments that were passed to alignCells.
    /// 
    /// mxEvent.FLIP_EDGE fires between begin- and endUpdate in flipEdge. The
    /// <code>edge</code> property contains the edge passed to flipEdge.
    /// 
    /// mxEvent.ORDER_CELLS fires between begin- and endUpdate in orderCells. The
    /// <code>cells</code> and <code>back</code> properties contain the respective
    /// arguments that were passed to orderCells.
    /// 
    /// mxEvent.CELLS_ORDERED fires between begin- and endUpdate in cellsOrdered.
    /// The <code>cells</code> and <code>back</code> arguments contain the
    /// respective arguments that were passed to cellsOrdered.
    /// 
    /// mxEvent.GROUP_CELLS fires between begin- and endUpdate in groupCells. The
    /// <code>group</code>, <code>cells</code> and <code>border</code> arguments
    /// contain the respective arguments that were passed to groupCells.
    /// 
    /// mxEvent.UNGROUP_CELLS fires between begin- and endUpdate in ungroupCells.
    /// The <code>cells</code> property contains the array of cells that was passed
    /// to ungroupCells.
    /// 
    /// mxEvent.REMOVE_CELLS_FROM_PARENT fires between begin- and endUpdate in
    /// removeCellsFromParent. The <code>cells</code> property contains the array of
    /// cells that was passed to removeCellsFromParent.
    /// 
    /// mxEvent.ADD_CELLS fires between begin- and endUpdate in addCells. The
    /// <code>cells</code>, <code>parent</code>, <code>index</code>,
    /// <code>source</code> and <code>target</code> properties contain the
    /// respective arguments that were passed to addCells.
    /// 
    /// mxEvent.CELLS_ADDED fires between begin- and endUpdate in cellsAdded. The
    /// <code>cells</code>, <code>parent</code>, <code>index</code>,
    /// <code>source</code>, <code>target</code> and <code>absolute</code>
    /// properties contain the respective arguments that were passed to cellsAdded.
    /// 
    /// mxEvent.REMOVE_CELLS fires between begin- and endUpdate in removeCells. The
    /// <code>cells</code> and <code>includeEdges</code> arguments contain the
    /// respective arguments that were passed to removeCells.
    /// 
    /// mxEvent.CELLS_REMOVED fires between begin- and endUpdate in cellsRemoved.
    /// The <code>cells</code> argument contains the array of cells that was
    /// removed.
    /// 
    /// mxEvent.SPLIT_EDGE fires between begin- and endUpdate in splitEdge. The
    /// <code>edge</code> property contains the edge to be splitted, the
    /// <code>cells</code>, <code>newEdge</code>, <code>dx</code> and
    /// <code>dy</code> properties contain the respective arguments that were passed
    /// to splitEdge.
    /// 
    /// mxEvent.TOGGLE_CELLS fires between begin- and endUpdate in toggleCells. The
    /// <code>show</code>, <code>cells</code> and <code>includeEdges</code>
    /// properties contain the respective arguments that were passed to toggleCells.
    /// 
    /// mxEvent.FOLD_CELLS fires between begin- and endUpdate in foldCells. The
    /// <code>collapse</code>, <code>cells</code> and <code>recurse</code>
    /// properties contain the respective arguments that were passed to foldCells.
    /// 
    /// mxEvent.CELLS_FOLDED fires between begin- and endUpdate in cellsFolded. The
    /// <code>collapse</code>, <code>cells</code> and <code>recurse</code>
    /// properties contain the respective arguments that were passed to cellsFolded.
    /// 
    /// mxEvent.UPDATE_CELL_SIZE fires between begin- and endUpdate in
    /// updateCellSize. The <code>cell</code> and <code>ignoreChildren</code>
    /// properties contain the respective arguments that were passed to
    /// updateCellSize.
    /// 
    /// mxEvent.RESIZE_CELLS fires between begin- and endUpdate in resizeCells. The
    /// <code>cells</code> and <code>bounds</code> properties contain the respective
    /// arguments that were passed to resizeCells.
    /// 
    /// mxEvent.CELLS_RESIZED fires between begin- and endUpdate in cellsResized.
    /// The <code>cells</code> and <code>bounds</code> properties contain the
    /// respective arguments that were passed to cellsResized.
    /// 
    /// mxEvent.MOVE_CELLS fires between begin- and endUpdate in moveCells. The
    /// <code>cells</code>, <code>dx</code>, <code>dy</code>, <code>clone</code>,
    /// <code>target</code> and <code>location</code> properties contain the
    /// respective arguments that were passed to moveCells.
    /// 
    /// mxEvent.CELLS_MOVED fires between begin- and endUpdate in cellsMoved. The
    /// <code>cells</code>, <code>dx</code>, <code>dy</code> and
    /// <code>disconnect</code> properties contain the respective arguments that
    /// were passed to cellsMoved.
    /// 
    /// mxEvent.CONNECT_CELL fires between begin- and endUpdate in connectCell. The
    /// <code>edge</code>, <code>terminal</code> and <code>source</code> properties
    /// contain the respective arguments that were passed to connectCell.
    /// 
    /// mxEvent.CELL_CONNECTED fires between begin- and endUpdate in cellConnected.
    /// The <code>edge</code>, <code>terminal</code> and <code>source</code>
    /// properties contain the respective arguments that were passed to
    /// cellConnected.
    /// 
    /// mxEvent.REPAINT fires if a repaint was requested by calling repaint. The
    /// <code>region</code> property contains the optional mxRectangle that was
    /// passed to repaint to define the dirty region.
    /// </summary>
    public class mxGraph : mxEventSource
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            changeSupport = new PropertyChangeSupport(this);
        }


        /// <summary>
        /// Adds required resources.
        /// </summary>
        static mxGraph()
        {
            try
            {
                //mxResources.add("com.mxgraph.resources.graph");
            }
            catch (Exception)
            {
                // ignore
            }
        }

        /// <summary>
        /// Holds the version number of this release. Current version
        /// is @MXGRAPH-VERSION@.
        /// </summary>
        public const string VERSION = "@MXGRAPH-VERSION@";

        /// 
        public interface mxICellVisitor
        {

            /// 
            /// <param name="vertex"> </param>
            /// <param name="edge"> </param>
            bool visit(object vertex, object edge);

        }

        /// <summary>
        /// Property change event handling.
        /// </summary>
        protected internal PropertyChangeSupport changeSupport;

        /// <summary>
        /// Holds the model that contains the cells to be displayed.
        /// </summary>
        protected internal mxIGraphModel model;

        /// <summary>
        /// Holds the view that caches the cell states.
        /// </summary>
        protected internal mxGraphView view;

        /// <summary>
        /// Holds the stylesheet that defines the appearance of the cells.
        /// </summary>
        protected internal mxStylesheet stylesheet;

        /// <summary>
        /// Holds the <mxGraphSelection> that models the current selection.
        /// </summary>
        protected internal mxGraphSelectionModel selectionModel;

        /// <summary>
        /// Specifies the grid size. Default is 10.
        /// </summary>
        protected internal int gridSize = 10;

        /// <summary>
        /// Specifies if the grid is enabled. Default is true.
        /// </summary>
        protected internal bool gridEnabled = true;

        /// <summary>
        /// Value returned by getOverlap if isAllowOverlapParent returns
        /// true for the given cell. getOverlap is used in keepInside if
        /// isKeepInsideParentOnMove returns true. The value specifies the
        /// portion of the child which is allowed to overlap the parent.
        /// </summary>
        protected internal double defaultOverlap = 0.5;

        /// <summary>
        /// Specifies the default parent to be used to insert new cells.
        /// This is used in getDefaultParent. Default is null.
        /// </summary>
        protected internal object defaultParent;

        /// <summary>
        /// Specifies the alternate edge style to be used if the main control point
        /// on an edge is being doubleclicked. Default is null.
        /// </summary>
        protected internal string alternateEdgeStyle;

        /// <summary>
        /// Specifies the return value for isEnabled. Default is true.
        /// </summary>
        protected internal bool enabled = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Locked. Default is false.
        /// </summary>
        protected internal bool cellsLocked = false;

        /// <summary>
        /// Specifies the return value for isCell(s)Editable. Default is true.
        /// </summary>
        protected internal bool cellsEditable = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Sizable. Default is true.
        /// </summary>
        protected internal bool cellsResizable = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Movable. Default is true.
        /// </summary>
        protected internal bool cellsMovable = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Bendable. Default is true.
        /// </summary>
        protected internal bool cellsBendable = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Selectable. Default is true.
        /// </summary>
        protected internal bool cellsSelectable = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Deletable. Default is true.
        /// </summary>
        protected internal bool cellsDeletable = true;

        /// <summary>
        /// Specifies the return value for isCell(s)Cloneable. Default is true.
        /// </summary>
        protected internal bool cellsCloneable = true;

        /// <summary>
        /// Specifies the return value for isCellDisconntableFromTerminal. Default
        /// is true.
        /// </summary>
        protected internal bool cellsDisconnectable = true;

        /// <summary>
        /// Specifies the return value for isLabel(s)Clipped. Default is false.
        /// </summary>
        protected internal bool labelsClipped = false;

        /// <summary>
        /// Specifies the return value for edges in isLabelMovable. Default is true.
        /// </summary>
        protected internal bool edgeLabelsMovable = true;

        /// <summary>
        /// Specifies the return value for vertices in isLabelMovable. Default is false.
        /// </summary>
        protected internal bool vertexLabelsMovable = false;

        /// <summary>
        /// Specifies the return value for isDropEnabled. Default is true.
        /// </summary>
        protected internal bool dropEnabled = true;

        /// <summary>
        /// Specifies if dropping onto edges should be enabled. Default is true.
        /// </summary>
        protected internal bool splitEnabled = true;

        /// <summary>
        /// Specifies if the graph should automatically update the cell size
        /// after an edit. This is used in isAutoSizeCell. Default is false.
        /// </summary>
        protected internal bool autoSizeCells = false;

        /// <summary>
        /// <mxRectangle> that specifies the area in which all cells in the
        /// diagram should be placed. Uses in getMaximumGraphBounds. Use a width
        /// or height of 0 if you only want to give a upper, left corner.
        /// </summary>
        protected internal mxRectangle maximumGraphBounds = null;

        /// <summary>
        /// mxRectangle that specifies the minimum size of the graph canvas inside
        /// the scrollpane.
        /// </summary>
        protected internal mxRectangle minimumGraphSize = null;

        /// <summary>
        /// Border to be added to the bottom and right side when the container is
        /// being resized after the graph has been changed. Default is 0.
        /// </summary>
        protected internal int border = 0;

        /// <summary>
        /// Specifies if edges should appear in the foreground regardless of their
        /// order in the model. This has precendence over keepEdgeInBackground
        /// Default is false.
        /// </summary>
        protected internal bool keepEdgesInForeground = false;

        /// <summary>
        /// Specifies if edges should appear in the background regardless of their
        /// order in the model. Default is false.
        /// </summary>
        protected internal bool keepEdgesInBackground = false;

        /// <summary>
        /// Specifies if the cell size should be changed to the preferred size when
        /// a cell is first collapsed. Default is true.
        /// </summary>
        protected internal bool collapseToPreferredSize = true;

        /// <summary>
        /// Specifies if negative coordinates for vertices are allowed. Default is true.
        /// </summary>
        protected internal bool allowNegativeCoordinates = true;

        /// <summary>
        /// Specifies the return value for isConstrainChildren. Default is true.
        /// </summary>
        protected internal bool constrainChildren = true;

        /// <summary>
        /// Specifies if a parent should contain the child bounds after a resize of
        /// the child. Default is true.
        /// </summary>
        protected internal bool extendParents = true;

        /// <summary>
        /// Specifies if parents should be extended according to the <extendParents>
        /// switch if cells are added. Default is true.
        /// </summary>
        protected internal bool extendParentsOnAdd = true;

        /// <summary>
        /// Specifies if the scale and translate should be reset if
        /// the root changes in the model. Default is true.
        /// </summary>
        protected internal bool resetViewOnRootChange = true;

        /// <summary>
        /// Specifies if loops (aka self-references) are allowed.
        /// Default is false.
        /// </summary>
        protected internal bool resetEdgesOnResize = false;

        /// <summary>
        /// Specifies if edge control points should be reset after
        /// the move of a connected cell. Default is false.
        /// </summary>
        protected internal bool resetEdgesOnMove = false;

        /// <summary>
        /// Specifies if edge control points should be reset after
        /// the the edge has been reconnected. Default is true.
        /// </summary>
        protected internal bool resetEdgesOnConnect = true;

        /// <summary>
        /// Specifies if loops (aka self-references) are allowed.
        /// Default is false.
        /// </summary>
        protected internal bool allowLoops = false;

        /// <summary>
        /// Specifies the multiplicities to be used for validation of the graph.
        /// </summary>
        protected internal mxMultiplicity[] multiplicities;

        /// <summary>
        /// Specifies the default style for loops.
        /// </summary>
        protected internal mxEdgeStyle.mxEdgeStyleFunction defaultLoopStyle = mxEdgeStyle.Loop;

        /// <summary>
        /// Specifies if multiple edges in the same direction between
        /// the same pair of vertices are allowed. Default is true.
        /// </summary>
        protected internal bool multigraph = true;

        /// <summary>
        /// Specifies if edges are connectable. Default is false.
        /// This overrides the connectable field in edges.
        /// </summary>
        protected internal bool connectableEdges = false;

        /// <summary>
        /// Specifies if edges with disconnected terminals are
        /// allowed in the graph. Default is false.
        /// </summary>
        protected internal bool allowDanglingEdges = true;

        /// <summary>
        /// Specifies if edges that are cloned should be validated and only inserted
        /// if they are valid. Default is true.
        /// </summary>
        protected internal bool cloneInvalidEdges = false;

        /// <summary>
        /// Specifies if edges should be disconnected from their terminals when they
        /// are moved. Default is true.
        /// </summary>
        protected internal bool disconnectOnMove = true;

        /// <summary>
        /// Specifies if labels should be visible. This is used in
        /// getLabel. Default is true.
        /// </summary>
        protected internal bool labelsVisible = true;

        /// <summary>
        /// Specifies the return value for isHtmlLabel. Default is false.
        /// </summary>
        protected internal bool htmlLabels = false;

        /// <summary>
        /// Specifies if nesting of swimlanes is allowed. Default is true.
        /// </summary>
        protected internal bool swimlaneNesting = true;

        /// <summary>
        /// Specifies the maximum number of changes that should be processed to find
        /// the dirty region. If the number of changes is larger, then the complete
        /// grah is repainted. A value of zero will always compute the dirty region
        /// for any number of changes. Default is 1000.
        /// </summary>
        protected internal int changesRepaintThreshold = 1000;

        /// <summary>
        /// Specifies if the origin should be automatically updated. 
        /// </summary>
        protected internal bool autoOrigin = false;

        /// <summary>
        /// Holds the current automatic origin.
        /// </summary>
        protected internal mxPoint origin = new mxPoint();

        /// <summary>
        /// Fires repaint events for full repaints.
        /// </summary>
        protected internal mxIEventListener fullRepaintHandler = new mxIEventListenerAnonymousInnerClass();

        private class mxIEventListenerAnonymousInnerClass : mxIEventListener
        {
            public mxIEventListenerAnonymousInnerClass()
            {
            }

            public virtual void invoke(object sender, mxEventObject evt)
            {
                outerInstance.repaint();
            }
        }

        /// <summary>
        /// Fires repaint events for model changes.
        /// </summary>
        protected internal mxIEventListener graphModelChangeHandler = new mxIEventListenerAnonymousInnerClass2();

        private class mxIEventListenerAnonymousInnerClass2 : mxIEventListener
        {
            public mxIEventListenerAnonymousInnerClass2()
            {
            }

            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: @SuppressWarnings("unchecked") public void invoke(Object sender, com.mxgraph.util.mxEventObject evt)
            public virtual void invoke(object sender, mxEventObject evt)
            {
                mxRectangle dirty = outerInstance.graphModelChanged((mxIGraphModel)sender, (IList<mxUndoableEdit.mxUndoableChange>)((mxUndoableEdit)evt.getProperty("edit")).Changes);
                outerInstance.repaint(dirty);
            }
        }

        /// <summary>
        /// Constructs a new graph with an empty
        /// <seealso cref="com.mxgraph.model.mxGraphModel"/>.
        /// </summary>
        public mxGraph() : this(null, null)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Constructs a new graph for the specified model. If no model is
        /// specified, then a new, empty <seealso cref="com.mxgraph.model.mxGraphModel"/> is
        /// used.
        /// </summary>
        /// <param name="model"> Model that contains the graph data </param>
        public mxGraph(mxIGraphModel model) : this(model, null)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Constructs a new graph for the specified model. If no model is
        /// specified, then a new, empty <seealso cref="com.mxgraph.model.mxGraphModel"/> is
        /// used.
        /// </summary>
        /// <param name="stylesheet"> The stylesheet to use for the graph. </param>
        public mxGraph(mxStylesheet stylesheet) : this(null, stylesheet)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Constructs a new graph for the specified model. If no model is
        /// specified, then a new, empty <seealso cref="com.mxgraph.model.mxGraphModel"/> is
        /// used.
        /// </summary>
        /// <param name="model"> Model that contains the graph data </param>
        public mxGraph(mxIGraphModel model, mxStylesheet stylesheet)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            selectionModel = createSelectionModel();
            Model = (model != null) ? model : new mxGraphModel();
            Stylesheet = (stylesheet != null) ? stylesheet : createStylesheet();
            View = createGraphView();
        }

        /// <summary>
        /// Constructs a new selection model to be used in this graph.
        /// </summary>
        protected internal virtual mxGraphSelectionModel createSelectionModel()
        {
            return new mxGraphSelectionModel(this);
        }

        /// <summary>
        /// Constructs a new stylesheet to be used in this graph.
        /// </summary>
        protected internal virtual mxStylesheet createStylesheet()
        {
            return new mxStylesheet();
        }

        /// <summary>
        /// Constructs a new view to be used in this graph.
        /// </summary>
        protected internal virtual mxGraphView createGraphView()
        {
            return new mxGraphView(this);
        }

        /// <summary>
        /// Returns the graph model that contains the graph data.
        /// </summary>
        /// <returns> Returns the model that contains the graph data </returns>
        public virtual mxIGraphModel Model
        {
            get
            {
                return model;
            }
            set
            {
                if (model != null)
                {
                    model.removeListener(graphModelChangeHandler);
                }

                object oldModel = model;
                model = value;

                if (view != null)
                {
                    view.revalidate();
                }

                model.addListener(mxEvent.CHANGE, graphModelChangeHandler);
                changeSupport.firePropertyChange("model", oldModel, model);
                repaint();
            }
        }


        /// <summary>
        /// Returns the view that contains the cell states.
        /// </summary>
        /// <returns> Returns the view that contains the cell states </returns>
        public virtual mxGraphView View
        {
            get
            {
                return view;
            }
            set
            {
                if (view != null)
                {
                    view.removeListener(fullRepaintHandler);
                }

                object oldView = view;
                view = value;

                if (view != null)
                {
                    view.revalidate();
                }

                // Listens to changes in the view
                view.addListener(mxEvent.SCALE, fullRepaintHandler);
                view.addListener(mxEvent.TRANSLATE, fullRepaintHandler);
                view.addListener(mxEvent.SCALE_AND_TRANSLATE, fullRepaintHandler);
                view.addListener(mxEvent.UP, fullRepaintHandler);
                view.addListener(mxEvent.DOWN, fullRepaintHandler);

                changeSupport.firePropertyChange("view", oldView, view);
            }
        }


        /// <summary>
        /// Returns the stylesheet that provides the style.
        /// </summary>
        /// <returns> Returns the stylesheet that provides the style. </returns>
        public virtual mxStylesheet Stylesheet
        {
            get
            {
                return stylesheet;
            }
            set
            {
                mxStylesheet oldValue = stylesheet;
                stylesheet = value;

                changeSupport.firePropertyChange("stylesheet", oldValue, stylesheet);
            }
        }


        /// <summary>
        /// Returns the cells to be selected for the given list of changes.
        /// </summary>
        public virtual object[] getSelectionCellsForChanges(IList<mxUndoableEdit.mxUndoableChange> changes)
        {
            IList<object> cells = new List<object>();
            IEnumerator<mxUndoableEdit.mxUndoableChange> it = changes.GetEnumerator();

            while (it.MoveNext())
            {
                object change = it.Current;

                if (change is mxGraphModel.mxChildChange)
                {
                    cells.Add(((mxGraphModel.mxChildChange)change).Child);
                }
                else if (change is mxGraphModel.mxTerminalChange)
                {
                    cells.Add(((mxGraphModel.mxTerminalChange)change).Cell);
                }
                else if (change is mxGraphModel.mxValueChange)
                {
                    cells.Add(((mxGraphModel.mxValueChange)change).Cell);
                }
                else if (change is mxGraphModel.mxStyleChange)
                {
                    cells.Add(((mxGraphModel.mxStyleChange)change).Cell);
                }
                else if (change is mxGraphModel.mxGeometryChange)
                {
                    cells.Add(((mxGraphModel.mxGeometryChange)change).Cell);
                }
                else if (change is mxGraphModel.mxCollapseChange)
                {
                    cells.Add(((mxGraphModel.mxCollapseChange)change).Cell);
                }
                else if (change is mxGraphModel.mxVisibleChange)
                {
                    mxGraphModel.mxVisibleChange vc = (mxGraphModel.mxVisibleChange)change;

                    if (vc.Visible)
                    {
                        cells.Add(((mxGraphModel.mxVisibleChange)change).Cell);
                    }
                }
            }

            return mxGraphModel.getTopmostCells(model, cells.ToArray());
        }

        /// <summary>
        /// Called when the graph model changes. Invokes processChange on each
        /// item of the given array to update the view accordingly.
        /// </summary>
        public virtual mxRectangle graphModelChanged(mxIGraphModel sender, IList<mxUndoableEdit.mxUndoableChange> changes)
        {
            int thresh = ChangesRepaintThreshold;
            bool ignoreDirty = thresh > 0 && changes.Count > thresh;

            // Ignores dirty rectangle if there was a root change
            if (!ignoreDirty)
            {
                IEnumerator<mxUndoableEdit.mxUndoableChange> it = changes.GetEnumerator();

                while (it.MoveNext())
                {
                    if (it.Current is mxGraphModel.mxRootChange)
                    {
                        ignoreDirty = true;
                        break;
                    }
                }
            }

            mxRectangle dirty = processChanges(changes, true, ignoreDirty);
            view.validate();

            if (AutoOrigin)
            {
                updateOrigin();
            }

            if (!ignoreDirty)
            {
                mxRectangle tmp = processChanges(changes, false, ignoreDirty);

                if (tmp != null)
                {
                    if (dirty == null)
                    {
                        dirty = tmp;
                    }
                    else
                    {
                        dirty.add(tmp);
                    }
                }
            }

            removeSelectionCells(getRemovedCellsForChanges(changes));

            return dirty;
        }

        /// <summary>
        /// Extends the canvas by doing another validation with a shifted
        /// global translation if the bounds of the graph are below (0,0).
        /// 
        /// The first validation is required to compute the bounds of the graph
        /// while the second validation is required to apply the new translate.
        /// </summary>
        protected internal virtual void updateOrigin()
        {
            mxRectangle bounds = GraphBounds;

            if (bounds != null)
            {
                double scale = View.Scale;
                double x = bounds.X / scale - Border;
                double y = bounds.Y / scale - Border;

                if (x < 0 || y < 0)
                {
                    double x0 = Math.Min(0, x);
                    double y0 = Math.Min(0, y);

                    origin.X = origin.X + x0;
                    origin.Y = origin.Y + y0;

                    mxPoint t = View.Translate;
                    View.Translate = new mxPoint(t.X - x0, t.Y - y0);
                }
                else if ((x > 0 || y > 0) && (origin.X < 0 || origin.Y < 0))
                {
                    double dx = Math.Min(-origin.X, x);
                    double dy = Math.Min(-origin.Y, y);

                    origin.X = origin.X + dx;
                    origin.Y = origin.Y + dy;

                    mxPoint t = View.Translate;
                    View.Translate = new mxPoint(t.X - dx, t.Y - dy);
                }
            }
        }

        /// <summary>
        /// Returns the cells that have been removed from the model.
        /// </summary>
        public virtual object[] getRemovedCellsForChanges(IList<mxUndoableEdit.mxUndoableChange> changes)
        {
            IList<object> result = new List<object>();
            IEnumerator<mxUndoableEdit.mxUndoableChange> it = changes.GetEnumerator();

            while (it.MoveNext())
            {
                object change = it.Current;

                if (change is mxGraphModel.mxRootChange)
                {
                    break;
                }
                else if (change is mxGraphModel.mxChildChange)
                {
                    mxGraphModel.mxChildChange cc = (mxGraphModel.mxChildChange)change;

                    if (cc.Parent == null)
                    {
                        ((List<object>)result).AddRange(mxGraphModel.getDescendants(model, cc.Child));
                    }
                }
                else if (change is mxGraphModel.mxVisibleChange)
                {
                    object cell = ((mxGraphModel.mxVisibleChange)change).Cell;
                    ((List<object>)result).AddRange(mxGraphModel.getDescendants(model, cell));
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Processes the changes and returns the minimal rectangle to be
        /// repainted in the buffer. A return value of null means no repaint
        /// is required.
        /// </summary>
        public virtual mxRectangle processChanges(IList<mxUndoableEdit.mxUndoableChange> changes, bool invalidate, bool ignoreDirty)
        {
            mxRectangle bounds = null;
            IEnumerator<mxUndoableEdit.mxUndoableChange> it = changes.GetEnumerator();

            while (it.MoveNext())
            {
                mxRectangle rect = processChange(it.Current, invalidate, ignoreDirty);

                if (bounds == null)
                {
                    bounds = rect;
                }
                else
                {
                    bounds.add(rect);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Processes the given change and invalidates the respective cached data
        /// in <view>. This fires a <root> event if the root has changed in the
        /// model.
        /// </summary>
        public virtual mxRectangle processChange(mxUndoableEdit.mxUndoableChange change, bool invalidate, bool ignoreDirty)
        {
            mxRectangle result = null;

            if (change is mxGraphModel.mxRootChange)
            {
                result = (ignoreDirty) ? null : GraphBounds;

                if (invalidate)
                {
                    clearSelection();
                    removeStateForCell(((mxGraphModel.mxRootChange)change).Previous);

                    if (ResetViewOnRootChange)
                    {
                        view.EventsEnabled = false;

                        try
                        {
                            view.scaleAndTranslate(1, 0, 0);
                        }
                        finally
                        {
                            view.EventsEnabled = true;
                        }
                    }

                }

                fireEvent(new mxEventObject(mxEvent.ROOT));
            }
            else if (change is mxGraphModel.mxChildChange)
            {
                mxGraphModel.mxChildChange cc = (mxGraphModel.mxChildChange)change;

                // Repaints the parent area if it is a rendered cell (vertex or
                // edge) otherwise only the child area is repainted, same holds
                // if the parent and previous are the same object, in which case
                // only the child area needs to be repainted (change of order)
                if (!ignoreDirty)
                {
                    if (cc.Parent != cc.Previous)
                    {
                        if (model.isVertex(cc.Parent) || model.isEdge(cc.Parent))
                        {
                            result = getBoundingBox(cc.Parent, true, true);
                        }

                        if (model.isVertex(cc.Previous) || model.isEdge(cc.Previous))
                        {
                            if (result != null)
                            {
                                result.add(getBoundingBox(cc.Previous, true, true));
                            }
                            else
                            {
                                result = getBoundingBox(cc.Previous, true, true);
                            }
                        }
                    }

                    if (result == null)
                    {
                        result = getBoundingBox(cc.Child, true, true);
                    }
                }

                if (invalidate)
                {
                    if (cc.Parent != null)
                    {
                        view.clear(cc.Child, false, true);
                    }
                    else
                    {
                        removeStateForCell(cc.Child);
                    }
                }
            }
            else if (change is mxGraphModel.mxTerminalChange)
            {
                object cell = ((mxGraphModel.mxTerminalChange)change).Cell;

                if (!ignoreDirty)
                {
                    result = getBoundingBox(cell, true);
                }

                if (invalidate)
                {
                    view.invalidate(cell);
                }
            }
            else if (change is mxGraphModel.mxValueChange)
            {
                object cell = ((mxGraphModel.mxValueChange)change).Cell;

                if (!ignoreDirty)
                {
                    result = getBoundingBox(cell);
                }

                if (invalidate)
                {
                    view.clear(cell, false, false);
                }
            }
            else if (change is mxGraphModel.mxStyleChange)
            {
                object cell = ((mxGraphModel.mxStyleChange)change).Cell;

                if (!ignoreDirty)
                {
                    result = getBoundingBox(cell, true);
                }

                if (invalidate)
                {
                    // TODO: Add includeEdges argument to clear method for
                    // not having to call invalidate in this case (where it
                    // is possible that the perimeter has changed, which
                    // means the connected edges need to be invalidated)
                    view.clear(cell, false, false);
                    view.invalidate(cell);
                }
            }
            else if (change is mxGraphModel.mxGeometryChange)
            {
                object cell = ((mxGraphModel.mxGeometryChange)change).Cell;

                if (!ignoreDirty)
                {
                    result = getBoundingBox(cell, true, true);
                }

                if (invalidate)
                {
                    view.invalidate(cell);
                }
            }
            else if (change is mxGraphModel.mxCollapseChange)
            {
                object cell = ((mxGraphModel.mxCollapseChange)change).Cell;

                if (!ignoreDirty)
                {
                    result = getBoundingBox(((mxGraphModel.mxCollapseChange)change).Cell, true, true);
                }

                if (invalidate)
                {
                    removeStateForCell(cell);
                }
            }
            else if (change is mxGraphModel.mxVisibleChange)
            {
                object cell = ((mxGraphModel.mxVisibleChange)change).Cell;

                if (!ignoreDirty)
                {
                    result = getBoundingBox(((mxGraphModel.mxVisibleChange)change).Cell, true, true);
                }

                if (invalidate)
                {
                    removeStateForCell(cell);
                }
            }

            return result;
        }

        /// <summary>
        /// Removes all cached information for the given cell and its descendants.
        /// This is called when a cell was removed from the model.
        /// </summary>
        /// <param name="cell"> Cell that was removed from the model. </param>
        protected internal virtual void removeStateForCell(object cell)
        {
            int childCount = model.getChildCount(cell);

            for (int i = 0; i < childCount; i++)
            {
                removeStateForCell(model.getChildAt(cell, i));
            }

            view.removeState(cell);
        }

        //
        // Cell styles
        //

        /// <summary>
        /// Returns an array of key, value pairs representing the cell style for the
        /// given cell. If no string is defined in the model that specifies the
        /// style, then the default style for the cell is returned or <EMPTY_ARRAY>,
        /// if not style can be found.
        /// </summary>
        /// <param name="cell"> Cell whose style should be returned. </param>
        /// <returns> Returns the style of the cell. </returns>
        public virtual IDictionary<string, object> getCellStyle(object cell)
        {
            IDictionary<string, object> style = (model.isEdge(cell)) ? stylesheet.DefaultEdgeStyle : stylesheet.DefaultVertexStyle;

            string name = model.getStyle(cell);

            if (!string.ReferenceEquals(name, null))
            {
                style = stylesheet.getCellStyle(name, style);
            }

            if (style == null)
            {
                style = mxStylesheet.EMPTY_STYLE;
            }

            return style;
        }

        /// <summary>
        /// Sets the style of the selection cells to the given value.
        /// </summary>
        /// <param name="style"> String representing the new style of the cells. </param>
        public virtual object[] setCellStyle(string style)
        {
            return setCellStyle(style, null);
        }

        /// <summary>
        /// Sets the style of the specified cells. If no cells are given, then the
        /// selection cells are changed.
        /// </summary>
        /// <param name="style"> String representing the new style of the cells. </param>
        /// <param name="cells"> Optional array of <mxCells> to set the style for. Default is the
        /// selection cells. </param>
        public virtual object[] setCellStyle(string style, object[] cells)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            if (cells != null)
            {
                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        model.setStyle(cells[i], style);
                    }
                }
                finally
                {
                    model.endUpdate();
                }
            }

            return cells;
        }

        /// <summary>
        /// Toggles the boolean value for the given key in the style of the
        /// given cell. If no cell is specified then the selection cell is
        /// used.
        /// </summary>
        /// <param name="key"> Key for the boolean value to be toggled. </param>
        /// <param name="defaultValue"> Default boolean value if no value is defined. </param>
        /// <param name="cell"> Cell whose style should be modified. </param>
        public virtual object toggleCellStyle(string key, bool defaultValue, object cell)
        {
            return toggleCellStyles(key, defaultValue, new object[] { cell })[0];
        }

        /// <summary>
        /// Toggles the boolean value for the given key in the style of the
        /// selection cells.
        /// </summary>
        /// <param name="key"> Key for the boolean value to be toggled. </param>
        /// <param name="defaultValue"> Default boolean value if no value is defined. </param>
        public virtual object[] toggleCellStyles(string key, bool defaultValue)
        {
            return toggleCellStyles(key, defaultValue, null);
        }

        /// <summary>
        /// Toggles the boolean value for the given key in the style of the given
        /// cells. If no cells are specified, then the selection cells are used. For
        /// example, this can be used to toggle mxConstants.STYLE_ROUNDED or any
        /// other style with a boolean value.
        /// </summary>
        /// <param name="key"> String representing the key of the boolean style to be toggled. </param>
        /// <param name="defaultValue"> Default boolean value if no value is defined. </param>
        /// <param name="cells"> Cells whose styles should be modified. </param>
        public virtual object[] toggleCellStyles(string key, bool defaultValue, object[] cells)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            if (cells != null && cells.Length > 0)
            {
                mxCellState state = view.getState(cells[0]);
                IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cells[0]);

                if (style != null)
                {
                    string value = (mxUtils.isTrue(style, key, defaultValue)) ? "0" : "1";
                    setCellStyles(key, value, cells);
                }
            }

            return cells;
        }

        /// <summary>
        /// Sets the key to value in the styles of the selection cells.
        /// </summary>
        /// <param name="key"> String representing the key to be assigned. </param>
        /// <param name="value"> String representing the new value for the key. </param>
        public virtual object[] setCellStyles(string key, string value)
        {
            return setCellStyles(key, value, null);
        }

        /// <summary>
        /// Sets the key to value in the styles of the given cells. This will modify
        /// the existing cell styles in-place and override any existing assignment
        /// for the given key. If no cells are specified, then the selection cells
        /// are changed. If no value is specified, then the respective key is
        /// removed from the styles.
        /// </summary>
        /// <param name="key"> String representing the key to be assigned. </param>
        /// <param name="value"> String representing the new value for the key. </param>
        /// <param name="cells"> Array of cells to change the style for. </param>
        public virtual object[] setCellStyles(string key, string value, object[] cells)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            mxUtils.setCellStyles(model, cells, key, value);

            return cells;
        }

        /// <summary>
        /// Toggles the given bit for the given key in the styles of the selection
        /// cells.
        /// </summary>
        /// <param name="key"> String representing the key to toggle the flag in. </param>
        /// <param name="flag"> Integer that represents the bit to be toggled. </param>
        public virtual object[] toggleCellStyleFlags(string key, int flag)
        {
            return toggleCellStyleFlags(key, flag, null);
        }

        /// <summary>
        /// Toggles the given bit for the given key in the styles of the specified
        /// cells.
        /// </summary>
        /// <param name="key"> String representing the key to toggle the flag in. </param>
        /// <param name="flag"> Integer that represents the bit to be toggled. </param>
        /// <param name="cells"> Optional array of <mxCells> to change the style for. Default is
        /// the selection cells. </param>
        public virtual object[] toggleCellStyleFlags(string key, int flag, object[] cells)
        {
            return setCellStyleFlags(key, flag, null, cells);
        }

        /// <summary>
        /// Sets or toggles the given bit for the given key in the styles of the
        /// selection cells.
        /// </summary>
        /// <param name="key"> String representing the key to toggle the flag in. </param>
        /// <param name="flag"> Integer that represents the bit to be toggled. </param>
        /// <param name="value"> Boolean value to be used or null if the value should be
        /// toggled. </param>
        public virtual object[] setCellStyleFlags(string key, int flag, bool value)
        {
            return setCellStyleFlags(key, flag, value, null);
        }

        /// <summary>
        /// Sets or toggles the given bit for the given key in the styles of the
        /// specified cells.
        /// </summary>
        /// <param name="key"> String representing the key to toggle the flag in. </param>
        /// <param name="flag"> Integer that represents the bit to be toggled. </param>
        /// <param name="value"> Boolean value to be used or null if the value should be
        /// toggled. </param>
        /// <param name="cells"> Optional array of cells to change the style for. If no
        /// cells are specified then the selection cells are used. </param>
        public virtual object[] setCellStyleFlags(string key, int flag, bool? value, object[] cells)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            if (cells != null && cells.Length > 0)
            {
                if (value == null)
                {
                    mxCellState state = view.getState(cells[0]);
                    IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cells[0]);

                    if (style != null)
                    {
                        int current = mxUtils.getInt(style, key);
                        value = !((current & flag) == flag);
                    }
                }

                mxUtils.setCellStyleFlags(model, cells, key, flag, value);
            }

            return cells;
        }

        //
        // Cell alignment and orientation
        //

        /// <summary>
        /// Aligns the selection cells vertically or horizontally according to the
        /// given alignment.
        /// </summary>
        /// <param name="align"> Specifies the alignment. Possible values are all constants
        /// in mxConstants with an ALIGN prefix. </param>
        public virtual object[] alignCells(string align)
        {
            return alignCells(align, null);
        }

        /// <summary>
        /// Aligns the given cells vertically or horizontally according to the given
        /// alignment.
        /// </summary>
        /// <param name="align"> Specifies the alignment. Possible values are all constants
        /// in mxConstants with an ALIGN prefix. </param>
        /// <param name="cells"> Array of cells to be aligned. </param>
        public virtual object[] alignCells(string align, object[] cells)
        {
            return alignCells(align, cells, null);
        }

        /// <summary>
        /// Aligns the given cells vertically or horizontally according to the given
        /// alignment using the optional parameter as the coordinate.
        /// </summary>
        /// <param name="align"> Specifies the alignment. Possible values are all constants
        /// in mxConstants with an ALIGN prefix. </param>
        /// <param name="cells"> Array of cells to be aligned. </param>
        /// <param name="param"> Optional coordinate for the alignment. </param>
        public virtual object[] alignCells(string align, object[] cells, object param)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            if (cells != null && cells.Length > 1)
            {
                // Finds the required coordinate for the alignment
                if (param == null)
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        mxGeometry geo = getCellGeometry(cells[i]);

                        if (geo != null && !model.isEdge(cells[i]))
                        {
                            if (param == null)
                            {
                                if (string.ReferenceEquals(align, null) || align.Equals(mxConstants.ALIGN_LEFT))
                                {
                                    param = geo.X;
                                }
                                else if (align.Equals(mxConstants.ALIGN_CENTER))
                                {
                                    param = geo.X + geo.Width / 2;
                                    break;
                                }
                                else if (align.Equals(mxConstants.ALIGN_RIGHT))
                                {
                                    param = geo.X + geo.Width;
                                }
                                else if (align.Equals(mxConstants.ALIGN_TOP))
                                {
                                    param = geo.Y;
                                }
                                else if (align.Equals(mxConstants.ALIGN_MIDDLE))
                                {
                                    param = geo.Y + geo.Height / 2;
                                    break;
                                }
                                else if (align.Equals(mxConstants.ALIGN_BOTTOM))
                                {
                                    param = geo.Y + geo.Height;
                                }
                            }
                            else
                            {
                                double tmp = double.Parse(param.ToString());

                                if (string.ReferenceEquals(align, null) || align.Equals(mxConstants.ALIGN_LEFT))
                                {
                                    param = Math.Min(tmp, geo.X);
                                }
                                else if (align.Equals(mxConstants.ALIGN_RIGHT))
                                {
                                    param = Math.Max(tmp, geo.X + geo.Width);
                                }
                                else if (align.Equals(mxConstants.ALIGN_TOP))
                                {
                                    param = Math.Min(tmp, geo.Y);
                                }
                                else if (align.Equals(mxConstants.ALIGN_BOTTOM))
                                {
                                    param = Math.Max(tmp, geo.Y + geo.Height);
                                }
                            }
                        }
                    }
                }

                // Aligns the cells to the coordinate
                model.beginUpdate();
                try
                {
                    double tmp = double.Parse(param.ToString());

                    for (int i = 0; i < cells.Length; i++)
                    {
                        mxGeometry geo = getCellGeometry(cells[i]);

                        if (geo != null && !model.isEdge(cells[i]))
                        {
                            geo = (mxGeometry)geo.clone();

                            if (string.ReferenceEquals(align, null) || align.Equals(mxConstants.ALIGN_LEFT))
                            {
                                geo.X = tmp;
                            }
                            else if (align.Equals(mxConstants.ALIGN_CENTER))
                            {
                                geo.X = tmp - geo.Width / 2;
                            }
                            else if (align.Equals(mxConstants.ALIGN_RIGHT))
                            {
                                geo.X = tmp - geo.Width;
                            }
                            else if (align.Equals(mxConstants.ALIGN_TOP))
                            {
                                geo.Y = tmp;
                            }
                            else if (align.Equals(mxConstants.ALIGN_MIDDLE))
                            {
                                geo.Y = tmp - geo.Height / 2;
                            }
                            else if (align.Equals(mxConstants.ALIGN_BOTTOM))
                            {
                                geo.Y = tmp - geo.Height;
                            }

                            model.setGeometry(cells[i], geo);

                            if (ResetEdgesOnMove)
                            {
                                resetEdges(new object[] { cells[i] });
                            }
                        }
                    }

                    fireEvent(new mxEventObject(mxEvent.ALIGN_CELLS, "cells", cells, "align", align));
                }
                finally
                {
                    model.endUpdate();
                }
            }

            return cells;
        }

        /// <summary>
        /// Called when the main control point of the edge is double-clicked. This
        /// implementation switches between null (default) and alternateEdgeStyle
        /// and resets the edges control points. Finally, a flip event is fired
        /// before endUpdate is called on the model.
        /// </summary>
        /// <param name="edge"> Cell that represents the edge to be flipped. </param>
        /// <returns> Returns the edge that has been flipped. </returns>
        public virtual object flipEdge(object edge)
        {
            if (edge != null && !string.ReferenceEquals(alternateEdgeStyle, null))
            {
                model.beginUpdate();
                try
                {
                    string style = model.getStyle(edge);

                    if (string.ReferenceEquals(style, null) || style.Length == 0)
                    {
                        model.setStyle(edge, alternateEdgeStyle);
                    }
                    else
                    {
                        model.setStyle(edge, null);
                    }

                    // Removes all existing control points
                    resetEdge(edge);
                    fireEvent(new mxEventObject(mxEvent.FLIP_EDGE, "edge", edge));
                }
                finally
                {
                    model.endUpdate();
                }
            }

            return edge;
        }

        //
        // Order
        //

        /// <summary>
        /// Moves the selection cells to the front or back. This is a shortcut method.
        /// </summary>
        /// <param name="back"> Specifies if the cells should be moved to back. </param>
        public virtual object[] orderCells(bool back)
        {
            return orderCells(back, null);
        }

        /// <summary>
        /// Moves the given cells to the front or back. The change is carried out
        /// using cellsOrdered. This method fires mxEvent.ORDER_CELLS while the
        /// transaction is in progress.
        /// </summary>
        /// <param name="back"> Specifies if the cells should be moved to back. </param>
        /// <param name="cells"> Array of cells whose order should be changed. If null is
        /// specified then the selection cells are used. </param>
        public virtual object[] orderCells(bool back, object[] cells)
        {
            if (cells == null)
            {
                cells = mxUtils.sortCells(getSelectionCells(), true);
            }

            model.beginUpdate();
            try
            {
                cellsOrdered(cells, back);
                fireEvent(new mxEventObject(mxEvent.ORDER_CELLS, "cells", cells, "back", back));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Moves the given cells to the front or back. This method fires
        /// mxEvent.CELLS_ORDERED while the transaction is in progress.
        /// </summary>
        /// <param name="cells"> Array of cells whose order should be changed. </param>
        /// <param name="back"> Specifies if the cells should be moved to back. </param>
        public virtual void cellsOrdered(object[] cells, bool back)
        {
            if (cells != null)
            {
                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        object parent = model.getParent(cells[i]);

                        if (back)
                        {
                            model.add(parent, cells[i], i);
                        }
                        else
                        {
                            model.add(parent, cells[i], model.getChildCount(parent) - 1);
                        }
                    }

                    fireEvent(new mxEventObject(mxEvent.CELLS_ORDERED, "cells", cells, "back", back));
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        //
        // Grouping
        //

        /// <summary>
        /// Groups the selection cells. This is a shortcut method.
        /// </summary>
        /// <returns> Returns the new group. </returns>
        public virtual object groupCells()
        {
            return groupCells(null);
        }

        /// <summary>
        /// Groups the selection cells and adds them to the given group. This is a
        /// shortcut method.
        /// </summary>
        /// <returns> Returns the new group. </returns>
        public virtual object groupCells(object group)
        {
            return groupCells(group, 0);
        }

        /// <summary>
        /// Groups the selection cells and adds them to the given group. This is a
        /// shortcut method.
        /// </summary>
        /// <returns> Returns the new group. </returns>
        public virtual object groupCells(object group, double border)
        {
            return groupCells(group, border, null);
        }

        /// <summary>
        /// Adds the cells into the given group. The change is carried out using
        /// cellsAdded, cellsMoved and cellsResized. This method fires
        /// mxEvent.GROUP_CELLS while the transaction is in progress. Returns the
        /// new group. A group is only created if there is at least one entry in the
        /// given array of cells.
        /// </summary>
        /// <param name="group"> Cell that represents the target group. If null is specified
        /// then a new group is created using createGroupCell. </param>
        /// <param name="border"> Integer that specifies the border between the child area
        /// and the group bounds. </param>
        /// <param name="cells"> Optional array of cells to be grouped. If null is specified
        /// then the selection cells are used. </param>
        public virtual object groupCells(object group, double border, object[] cells)
        {
            if (cells == null)
            {
                cells = mxUtils.sortCells(getSelectionCells(), true);
            }

            cells = getCellsForGroup(cells);

            if (group == null)
            {
                group = createGroupCell(cells);
            }

            mxRectangle bounds = getBoundsForGroup(group, cells, border);

            if (cells.Length > 0 && bounds != null)
            {
                object parent = model.getParent(cells[0]);

                model.beginUpdate();
                try
                {
                    // Checks if the group has a geometry and
                    // creates one if one does not exist
                    if (getCellGeometry(group) == null)
                    {
                        model.setGeometry(group, new mxGeometry());
                    }

                    // Adds the children into the group and moves
                    int index = model.getChildCount(group);
                    cellsAdded(cells, group, index, null, null, false);
                    cellsMoved(cells, -bounds.X, -bounds.Y, false, true);

                    // Adds the group into the parent and resizes
                    index = model.getChildCount(parent);
                    cellsAdded(new object[] { group }, parent, index, null, null, false);
                    cellsResized(new object[] { group }, new mxRectangle[] { bounds });

                    fireEvent(new mxEventObject(mxEvent.GROUP_CELLS, "group", group, "cells", cells, "border", border));
                }
                finally
                {
                    model.endUpdate();
                }
            }

            return group;
        }

        /// <summary>
        /// Returns the cells with the same parent as the first cell
        /// in the given array.
        /// </summary>
        public virtual object[] getCellsForGroup(object[] cells)
        {
            IList<object> result = new List<object>(cells.Length);

            if (cells != null && cells.Length > 0)
            {
                object parent = model.getParent(cells[0]);
                result.Add(cells[0]);

                // Filters selection cells with the same parent
                for (int i = 1; i < cells.Length; i++)
                {
                    if (model.getParent(cells[i]) == parent)
                    {
                        result.Add(cells[i]);
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns the bounds to be used for the given group and children. This
        /// implementation computes the bounding box of the geometries of all
        /// vertices in the given children array. Edges are ignored. If the group
        /// cell is a swimlane the title region is added to the bounds.
        /// </summary>
        public virtual mxRectangle getBoundsForGroup(object group, object[] children, double border)
        {
            mxRectangle result = getBoundingBoxFromGeometry(children);

            if (result != null)
            {
                if (isSwimlane(group))
                {
                    mxRectangle size = getStartSize(group);

                    result.X = result.X - size.Width;
                    result.Y = result.Y - size.Height;
                    result.Width = result.Width + size.Width;
                    result.Height = result.Height + size.Height;
                }

                // Adds the border
                result.X = result.X - border;
                result.Y = result.Y - border;
                result.Width = result.Width + 2 * border;
                result.Height = result.Height + 2 * border;
            }

            return result;
        }

        /// <summary>
        /// Hook for creating the group cell to hold the given array of <mxCells> if
        /// no group cell was given to the <group> function. The children are just
        /// for informational purpose, they will be added to the returned group
        /// later. Note that the returned group should have a geometry. The
        /// coordinates of which are later overridden.
        /// </summary>
        /// <param name="cells"> </param>
        /// <returns> Returns a new group cell. </returns>
        public virtual object createGroupCell(object[] cells)
        {
            mxCell group = new mxCell("", new mxGeometry(), null);
            group.Vertex = true;
            group.Connectable = false;

            return group;
        }

        /// <summary>
        /// Ungroups the selection cells. This is a shortcut method.
        /// </summary>
        public virtual object[] ungroupCells()
        {
            return ungroupCells(null);
        }

        /// <summary>
        /// Ungroups the given cells by moving the children the children to their
        /// parents parent and removing the empty groups.
        /// </summary>
        /// <param name="cells"> Array of cells to be ungrouped. If null is specified then
        /// the selection cells are used. </param>
        /// <returns> Returns the children that have been removed from the groups. </returns>
        public virtual object[] ungroupCells(object[] cells)
        {
            IList<object> result = new List<object>();

            if (cells == null)
            {
                cells = getSelectionCells();

                // Finds the cells with children
                IList<object> tmp = new List<object>(cells.Length);

                for (int i = 0; i < cells.Length; i++)
                {
                    if (model.getChildCount(cells[i]) > 0)
                    {
                        tmp.Add(cells[i]);
                    }
                }

                cells = tmp.ToArray();
            }

            if (cells != null && cells.Length > 0)
            {
                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        object[] children = mxGraphModel.getChildren(model, cells[i]);

                        if (children != null && children.Length > 0)
                        {
                            object parent = model.getParent(cells[i]);
                            int index = model.getChildCount(parent);

                            cellsAdded(children, parent, index, null, null, true);
                            ((List<object>)result).AddRange(Arrays.asList(children));
                        }
                    }

                    cellsRemoved(addAllEdges(cells));
                    fireEvent(new mxEventObject(mxEvent.UNGROUP_CELLS, "cells", cells));
                }
                finally
                {
                    model.endUpdate();
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Removes the selection cells from their parents and adds them to the
        /// default parent returned by getDefaultParent.
        /// </summary>
        public virtual object[] removeCellsFromParent()
        {
            return removeCellsFromParent(null);
        }

        /// <summary>
        /// Removes the specified cells from their parents and adds them to the
        /// default parent.
        /// </summary>
        /// <param name="cells"> Array of cells to be removed from their parents. </param>
        /// <returns> Returns the cells that were removed from their parents. </returns>
        public virtual object[] removeCellsFromParent(object[] cells)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            model.beginUpdate();
            try
            {
                object parent = DefaultParent;
                int index = model.getChildCount(parent);

                cellsAdded(cells, parent, index, null, null, true);
                fireEvent(new mxEventObject(mxEvent.REMOVE_CELLS_FROM_PARENT, "cells", cells));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Updates the bounds of the given array of groups so that it includes
        /// all child vertices.
        /// </summary>
        public virtual object[] updateGroupBounds()
        {
            return updateGroupBounds(null);
        }

        /// <summary>
        /// Updates the bounds of the given array of groups so that it includes
        /// all child vertices.
        /// </summary>
        /// <param name="cells"> The groups whose bounds should be updated. </param>
        public virtual object[] updateGroupBounds(object[] cells)
        {
            return updateGroupBounds(cells, 0);
        }

        /// <summary>
        /// Updates the bounds of the given array of groups so that it includes
        /// all child vertices.
        /// </summary>
        /// <param name="cells"> The groups whose bounds should be updated. </param>
        /// <param name="border"> The border to be added in the group. </param>
        public virtual object[] updateGroupBounds(object[] cells, int border)
        {
            return updateGroupBounds(cells, border, false);
        }

        /// <summary>
        /// Updates the bounds of the given array of groups so that it includes
        /// all child vertices.
        /// </summary>
        /// <param name="cells"> The groups whose bounds should be updated. </param>
        /// <param name="border"> The border to be added in the group. </param>
        /// <param name="moveParent"> Specifies if the group should be moved. </param>
        public virtual object[] updateGroupBounds(object[] cells, int border, bool moveParent)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            model.beginUpdate();
            try
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    mxGeometry geo = getCellGeometry(cells[i]);

                    if (geo != null)
                    {
                        object[] children = getChildCells(cells[i]);

                        if (children != null && children.Length > 0)
                        {
                            mxRectangle childBounds = getBoundingBoxFromGeometry(children);

                            if (childBounds.Width > 0 && childBounds.Height > 0)
                            {
                                mxRectangle size = (isSwimlane(cells[i])) ? getStartSize(cells[i]) : new mxRectangle();

                                geo = (mxGeometry)geo.clone();

                                if (moveParent)
                                {
                                    geo.X = geo.X + childBounds.X - size.Width - border;
                                    geo.Y = geo.Y + childBounds.Y - size.Height - border;
                                }

                                geo.Width = childBounds.Width + size.Width + 2 * border;
                                geo.Height = childBounds.Height + size.Height + 2 * border;

                                model.setGeometry(cells[i], geo);
                                moveCells(children, -childBounds.X + size.Width + border, -childBounds.Y + size.Height + border);
                            }
                        }
                    }
                }
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        //
        // Cell cloning, insertion and removal
        //

        /// <summary>
        /// Clones all cells in the given array. To clone all children in a cell and
        /// add them to another graph:
        /// 
        /// <code>
        /// graph2.addCells(graph.cloneCells(new Object[] { parent }));
        /// </code>
        /// </summary>
        public virtual object[] cloneCells(object[] cells)
        {
            return cloneCells(cells, true);
        }

        /// <summary>
        /// Returns the clones for the given cells. If the terminal of an edge is
        /// not in the given array, then the respective end is assigned a terminal
        /// point and the terminal is removed. If a cloned edge is invalid and
        /// allowInvalidEdges is false, then a null pointer will be at this position
        /// in the returned array. Use getCloneableCells on the input array to only
        /// clone the cells where isCellCloneable returns true.
        /// </summary>
        /// <param name="cells"> Array of mxCells to be cloned. </param>
        /// <returns> Returns the clones of the given cells. </returns>
        public virtual object[] cloneCells(object[] cells, bool allowInvalidEdges)
        {
            object[] clones = null;

            if (cells != null)
            {
                ICollection<object> tmp = new LinkedHashSet<object>(cells.Length);
                tmp.addAll(Arrays.asList(cells));

                if (tmp.Count > 0)
                {
                    double scale = view.Scale;
                    mxPoint trans = view.Translate;
                    clones = model.cloneCells(cells, true);

                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (!allowInvalidEdges && model.isEdge(clones[i]) && !string.ReferenceEquals(getEdgeValidationError(clones[i], model.getTerminal(clones[i], true), model.getTerminal(clones[i], false)), null))
                        {
                            clones[i] = null;
                        }
                        else
                        {
                            mxGeometry g = model.getGeometry(clones[i]);

                            if (g != null)
                            {
                                mxCellState state = view.getState(cells[i]);
                                mxCellState pstate = view.getState(model.getParent(cells[i]));

                                if (state != null && pstate != null)
                                {
                                    double dx = pstate.Origin.X;
                                    double dy = pstate.Origin.Y;

                                    if (model.isEdge(clones[i]))
                                    {
                                        // Checks if the source is cloned or sets the terminal point
                                        object src = model.getTerminal(cells[i], true);

                                        while (src != null && !tmp.Contains(src))
                                        {
                                            src = model.getParent(src);
                                        }

                                        if (src == null)
                                        {
                                            mxPoint pt = state.getAbsolutePoint(0);
                                            g.setTerminalPoint(new mxPoint(pt.X / scale - trans.X, pt.Y / scale - trans.Y), true);
                                        }

                                        // Checks if the target is cloned or sets the terminal point
                                        object trg = model.getTerminal(cells[i], false);

                                        while (trg != null && !tmp.Contains(trg))
                                        {
                                            trg = model.getParent(trg);
                                        }

                                        if (trg == null)
                                        {
                                            mxPoint pt = state.getAbsolutePoint(state.AbsolutePointCount - 1);
                                            g.setTerminalPoint(new mxPoint(pt.X / scale - trans.X, pt.Y / scale - trans.Y), false);
                                        }

                                        // Translates the control points
                                        IList<mxPoint> points = g.Points;

                                        if (points != null)
                                        {
                                            IEnumerator<mxPoint> it = points.GetEnumerator();

                                            while (it.MoveNext())
                                            {
                                                mxPoint pt = it.Current;
                                                pt.X = pt.X + dx;
                                                pt.Y = pt.Y + dy;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        g.X = g.X + dx;
                                        g.Y = g.Y + dy;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    clones = new object[] { };
                }
            }

            return clones;
        }

        /// <summary>
        /// Creates and adds a new vertex with an empty style.
        /// </summary>
        public virtual object insertVertex(object parent, string id, object value, double x, double y, double width, double height)
        {
            return insertVertex(parent, id, value, x, y, width, height, null);
        }

        /// <summary>
        /// Adds a new vertex into the given parent using value as the user object
        /// and the given coordinates as the geometry of the new vertex. The id and
        /// style are used for the respective properties of the new cell, which is
        /// returned.
        /// </summary>
        /// <param name="parent"> Cell that specifies the parent of the new vertex. </param>
        /// <param name="id"> Optional string that defines the Id of the new vertex. </param>
        /// <param name="value"> Object to be used as the user object. </param>
        /// <param name="x"> Integer that defines the x coordinate of the vertex. </param>
        /// <param name="y"> Integer that defines the y coordinate of the vertex. </param>
        /// <param name="width"> Integer that defines the width of the vertex. </param>
        /// <param name="height"> Integer that defines the height of the vertex. </param>
        /// <param name="style"> Optional string that defines the cell style. </param>
        /// <returns> Returns the new vertex that has been inserted. </returns>
        public virtual object insertVertex(object parent, string id, object value, double x, double y, double width, double height, string style)
        {
            object vertex = createVertex(parent, id, value, x, y, width, height, style);

            return addCell(vertex, parent);
        }

        /// <summary>
        /// Hook method that creates the new vertex for insertVertex.
        /// </summary>
        /// <param name="parent"> Cell that specifies the parent of the new vertex. </param>
        /// <param name="id"> Optional string that defines the Id of the new vertex. </param>
        /// <param name="value"> Object to be used as the user object. </param>
        /// <param name="x"> Integer that defines the x coordinate of the vertex. </param>
        /// <param name="y"> Integer that defines the y coordinate of the vertex. </param>
        /// <param name="width"> Integer that defines the width of the vertex. </param>
        /// <param name="height"> Integer that defines the height of the vertex. </param>
        /// <param name="style"> Optional string that defines the cell style. </param>
        /// <returns> Returns the new vertex to be inserted. </returns>
        public virtual object createVertex(object parent, string id, object value, double x, double y, double width, double height, string style)
        {
            mxGeometry geometry = new mxGeometry(x, y, width, height);
            mxCell vertex = new mxCell(value, geometry, style);

            vertex.Id = id;
            vertex.Vertex = true;
            vertex.Connectable = true;

            return vertex;
        }

        /// <summary>
        /// Creates and adds a new edge with an empty style.
        /// </summary>
        public virtual object insertEdge(object parent, string id, object value, object source, object target)
        {
            return insertEdge(parent, id, value, source, target, null);
        }

        /// <summary>
        /// Adds a new edge into the given parent using value as the user object and
        /// the given source and target as the terminals of the new edge. The Id and
        /// style are used for the respective properties of the new cell, which is
        /// returned.
        /// </summary>
        /// <param name="parent"> Cell that specifies the parent of the new edge. </param>
        /// <param name="id"> Optional string that defines the Id of the new edge. </param>
        /// <param name="value"> Object to be used as the user object. </param>
        /// <param name="source"> Cell that defines the source of the edge. </param>
        /// <param name="target"> Cell that defines the target of the edge. </param>
        /// <param name="style"> Optional string that defines the cell style. </param>
        /// <returns> Returns the new edge that has been inserted. </returns>
        public virtual object insertEdge(object parent, string id, object value, object source, object target, string style)
        {
            object edge = createEdge(parent, id, value, source, target, style);

            return addEdge(edge, parent, source, target, null);
        }

        /// <summary>
        /// Hook method that creates the new edge for insertEdge. This
        /// implementation does not set the source and target of the edge, these
        /// are set when the edge is added to the model.
        /// </summary>
        /// <param name="parent"> Cell that specifies the parent of the new edge. </param>
        /// <param name="id"> Optional string that defines the Id of the new edge. </param>
        /// <param name="value"> Object to be used as the user object. </param>
        /// <param name="source"> Cell that defines the source of the edge. </param>
        /// <param name="target"> Cell that defines the target of the edge. </param>
        /// <param name="style"> Optional string that defines the cell style. </param>
        /// <returns> Returns the new edge to be inserted. </returns>
        public virtual object createEdge(object parent, string id, object value, object source, object target, string style)
        {
            mxCell edge = new mxCell(value, new mxGeometry(), style);

            edge.Id = id;
            edge.Edge = true;
            edge.Geometry.Relative = true;

            return edge;
        }

        /// <summary>
        /// Adds the edge to the parent and connects it to the given source and
        /// target terminals. This is a shortcut method.
        /// </summary>
        /// <param name="edge"> Edge to be inserted into the given parent. </param>
        /// <param name="parent"> Object that represents the new parent. If no parent is
        /// given then the default parent is used. </param>
        /// <param name="source"> Optional cell that represents the source terminal. </param>
        /// <param name="target"> Optional cell that represents the target terminal. </param>
        /// <param name="index"> Optional index to insert the cells at. Default is to append. </param>
        /// <returns> Returns the edge that was added. </returns>
        public virtual object addEdge(object edge, object parent, object source, object target, int? index)
        {
            return addCell(edge, parent, index, source, target);
        }

        /// <summary>
        /// Adds the cell to the default parent. This is a shortcut method.
        /// </summary>
        /// <param name="cell"> Cell to be inserted. </param>
        /// <returns> Returns the cell that was added. </returns>
        public virtual object addCell(object cell)
        {
            return addCell(cell, null);
        }

        /// <summary>
        /// Adds the cell to the parent. This is a shortcut method.
        /// </summary>
        /// <param name="cell"> Cell tobe inserted. </param>
        /// <param name="parent"> Object that represents the new parent. If no parent is
        /// given then the default parent is used. </param>
        /// <returns> Returns the cell that was added. </returns>
        public virtual object addCell(object cell, object parent)
        {
            return addCell(cell, parent, null, null, null);
        }

        /// <summary>
        /// Adds the cell to the parent and connects it to the given source and
        /// target terminals. This is a shortcut method.
        /// </summary>
        /// <param name="cell"> Cell to be inserted into the given parent. </param>
        /// <param name="parent"> Object that represents the new parent. If no parent is
        /// given then the default parent is used. </param>
        /// <param name="index"> Optional index to insert the cells at. Default is to append. </param>
        /// <param name="source"> Optional cell that represents the source terminal. </param>
        /// <param name="target"> Optional cell that represents the target terminal. </param>
        /// <returns> Returns the cell that was added. </returns>
        public virtual object addCell(object cell, object parent, int? index, object source, object target)
        {
            return addCells(new object[] { cell }, parent, index, source, target)[0];
        }

        /// <summary>
        /// Adds the cells to the default parent. This is a shortcut method.
        /// </summary>
        /// <param name="cells"> Array of cells to be inserted. </param>
        /// <returns> Returns the cells that were added. </returns>
        public virtual object[] addCells(object[] cells)
        {
            return addCells(cells, null);
        }

        /// <summary>
        /// Adds the cells to the parent. This is a shortcut method.
        /// </summary>
        /// <param name="cells"> Array of cells to be inserted. </param>
        /// <param name="parent"> Optional cell that represents the new parent. If no parent
        /// is specified then the default parent is used. </param>
        /// <returns> Returns the cells that were added. </returns>
        public virtual object[] addCells(object[] cells, object parent)
        {
            return addCells(cells, parent, null);
        }

        /// <summary>
        /// Adds the cells to the parent at the given index. This is a shortcut method.
        /// </summary>
        /// <param name="cells"> Array of cells to be inserted. </param>
        /// <param name="parent"> Optional cell that represents the new parent. If no parent
        /// is specified then the default parent is used. </param>
        /// <param name="index"> Optional index to insert the cells at. Default is to append. </param>
        /// <returns> Returns the cells that were added. </returns>
        public virtual object[] addCells(object[] cells, object parent, int? index)
        {
            return addCells(cells, parent, index, null, null);
        }

        /// <summary>
        /// Adds the cells to the parent at the given index, connecting each cell to
        /// the optional source and target terminal. The change is carried out using
        /// cellsAdded. This method fires mxEvent.ADD_CELLS while the transaction
        /// is in progress.
        /// </summary>
        /// <param name="cells"> Array of cells to be added. </param>
        /// <param name="parent"> Optional cell that represents the new parent. If no parent
        /// is specified then the default parent is used. </param>
        /// <param name="index"> Optional index to insert the cells at. Default is to append. </param>
        /// <param name="source"> Optional source terminal for all inserted cells. </param>
        /// <param name="target"> Optional target terminal for all inserted cells. </param>
        /// <returns> Returns the cells that were added. </returns>
        public virtual object[] addCells(object[] cells, object parent, int? index, object source, object target)
        {
            if (parent == null)
            {
                parent = DefaultParent;
            }

            if (index == null)
            {
                index = model.getChildCount(parent);
            }

            model.beginUpdate();
            try
            {
                cellsAdded(cells, parent, index, source, target, false);
                fireEvent(new mxEventObject(mxEvent.ADD_CELLS, "cells", cells, "parent", parent, "index", index, "source", source, "target", target));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Adds the specified cells to the given parent. This method fires
        /// mxEvent.CELLS_ADDED while the transaction is in progress.
        /// </summary>
        public virtual void cellsAdded(object[] cells, object parent, int? index, object source, object target, bool absolute)
        {
            if (cells != null && parent != null && index != null)
            {
                model.beginUpdate();
                try
                {
                    mxCellState parentState = (absolute) ? view.getState(parent) : null;
                    mxPoint o1 = (parentState != null) ? parentState.Origin : null;
                    mxPoint zero = new mxPoint(0, 0);

                    for (int i = 0; i < cells.Length; i++)
                    {
                        object previous = model.getParent(cells[i]);

                        // Keeps the cell at its absolute location
                        if (o1 != null && cells[i] != parent && parent != previous)
                        {
                            mxCellState oldState = view.getState(previous);
                            mxPoint o2 = (oldState != null) ? oldState.Origin : zero;
                            mxGeometry geo = model.getGeometry(cells[i]);

                            if (geo != null)
                            {
                                double dx = o2.X - o1.X;
                                double dy = o2.Y - o1.Y;

                                geo = (mxGeometry)geo.clone();
                                geo.translate(dx, dy);

                                if (!geo.Relative && model.isVertex(cells[i]) && !AllowNegativeCoordinates)
                                {
                                    geo.X = Math.Max(0, geo.X);
                                    geo.Y = Math.Max(0, geo.Y);
                                }

                                model.setGeometry(cells[i], geo);
                            }
                        }

                        // Decrements all following indices
                        // if cell is already in parent
                        if (parent == previous)
                        {
                            index--;
                        }

                        model.add(parent, cells[i], index + i);

                        // Extends the parent
                        if (ExtendParentsOnAdd && isExtendParent(cells[i]))
                        {
                            extendParent(cells[i]);
                        }

                        // Constrains the child
                        constrainChild(cells[i]);

                        // Sets the source terminal
                        if (source != null)
                        {
                            cellConnected(cells[i], source, true, null);
                        }

                        // Sets the target terminal
                        if (target != null)
                        {
                            cellConnected(cells[i], target, false, null);
                        }
                    }

                    fireEvent(new mxEventObject(mxEvent.CELLS_ADDED, "cells", cells, "parent", parent, "index", index, "source", source, "target", target, "absolute", absolute));

                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Removes the selection cells from the graph.
        /// </summary>
        /// <returns> Returns the cells that have been removed. </returns>
        public virtual object[] removeCells()
        {
            return removeCells(null);
        }

        /// <summary>
        /// Removes the given cells from the graph.
        /// </summary>
        /// <param name="cells"> Array of cells to remove. </param>
        /// <returns> Returns the cells that have been removed. </returns>
        public virtual object[] removeCells(object[] cells)
        {
            return removeCells(cells, true);
        }

        /// <summary>
        /// Removes the given cells from the graph including all connected edges if
        /// includeEdges is true. The change is carried out using cellsRemoved. This
        /// method fires mxEvent.REMOVE_CELLS while the transaction is in progress.
        /// </summary>
        /// <param name="cells"> Array of cells to remove. If null is specified then the
        /// selection cells which are deletable are used. </param>
        /// <param name="includeEdges"> Specifies if all connected edges should be removed as
        /// well. </param>
        public virtual object[] removeCells(object[] cells, bool includeEdges)
        {
            if (cells == null)
            {
                cells = getDeletableCells(getSelectionCells());
            }

            // Adds all edges to the cells
            if (includeEdges)
            {
                cells = getDeletableCells(addAllEdges(cells));
            }

            model.beginUpdate();
            try
            {
                cellsRemoved(cells);
                fireEvent(new mxEventObject(mxEvent.REMOVE_CELLS, "cells", cells, "includeEdges", includeEdges));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Removes the given cells from the model. This method fires
        /// mxEvent.CELLS_REMOVED while the transaction is in progress.
        /// </summary>
        /// <param name="cells"> Array of cells to remove. </param>
        public virtual void cellsRemoved(object[] cells)
        {
            if (cells != null && cells.Length > 0)
            {
                double scale = view.Scale;
                mxPoint tr = view.Translate;

                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        // Disconnects edges which are not in cells
                        ICollection<object> cellSet = new HashSet<object>();
                        cellSet.addAll(Arrays.asList(cells));
                        object[] edges = getConnections(cells[i]);

                        for (int j = 0; j < edges.Length; j++)
                        {
                            if (!cellSet.Contains(edges[j]))
                            {
                                mxGeometry geo = model.getGeometry(edges[j]);

                                if (geo != null)
                                {
                                    mxCellState state = view.getState(edges[j]);

                                    if (state != null)
                                    {
                                        geo = (mxGeometry)geo.clone();
                                        bool source = view.getVisibleTerminal(edges[j], true) == cells[i];
                                        int n = (source) ? 0 : state.AbsolutePointCount - 1;
                                        mxPoint pt = state.getAbsolutePoint(n);

                                        geo.setTerminalPoint(new mxPoint(pt.X / scale - tr.X, pt.Y / scale - tr.Y), source);
                                        model.setTerminal(edges[j], null, source);
                                        model.setGeometry(edges[j], geo);
                                    }
                                }
                            }
                        }

                        model.remove(cells[i]);
                    }

                    fireEvent(new mxEventObject(mxEvent.CELLS_REMOVED, "cells", cells));
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// 
        public virtual object splitEdge(object edge, object[] cells)
        {
            return splitEdge(edge, cells, null, 0, 0);
        }

        /// 
        public virtual object splitEdge(object edge, object[] cells, double dx, double dy)
        {
            return splitEdge(edge, cells, null, dx, dy);
        }

        /// <summary>
        /// Splits the given edge by adding a newEdge between the previous source
        /// and the given cell and reconnecting the source of the given edge to the
        /// given cell. Fires mxEvent.SPLIT_EDGE while the transaction is in
        /// progress.
        /// </summary>
        /// <param name="edge"> Object that represents the edge to be splitted. </param>
        /// <param name="cells"> Array that contains the cells to insert into the edge. </param>
        /// <param name="newEdge"> Object that represents the edge to be inserted. </param>
        /// <returns> Returns the new edge that has been inserted. </returns>
        public virtual object splitEdge(object edge, object[] cells, object newEdge, double dx, double dy)
        {
            if (newEdge == null)
            {
                newEdge = cloneCells(new object[] { edge })[0];
            }

            object parent = model.getParent(edge);
            object source = model.getTerminal(edge, true);

            model.beginUpdate();
            try
            {
                cellsMoved(cells, dx, dy, false, false);
                cellsAdded(cells, parent, model.getChildCount(parent), null, null, true);
                cellsAdded(new object[] { newEdge }, parent, model.getChildCount(parent), source, cells[0], false);
                cellConnected(edge, cells[0], true, null);
                fireEvent(new mxEventObject(mxEvent.SPLIT_EDGE, "edge", edge, "cells", cells, "newEdge", newEdge, "dx", dx, "dy", dy));
            }
            finally
            {
                model.endUpdate();
            }

            return newEdge;
        }

        //
        // Cell visibility
        //

        /// <summary>
        /// Sets the visible state of the selection cells. This is a shortcut
        /// method.
        /// </summary>
        /// <param name="show"> Boolean that specifies the visible state to be assigned. </param>
        /// <returns> Returns the cells whose visible state was changed. </returns>
        public virtual object[] toggleCells(bool show)
        {
            return toggleCells(show, null);
        }

        /// <summary>
        /// Sets the visible state of the specified cells. This is a shortcut
        /// method.
        /// </summary>
        /// <param name="show"> Boolean that specifies the visible state to be assigned. </param>
        /// <param name="cells"> Array of cells whose visible state should be changed. </param>
        /// <returns> Returns the cells whose visible state was changed. </returns>
        public virtual object[] toggleCells(bool show, object[] cells)
        {
            return toggleCells(show, cells, true);
        }

        /// <summary>
        /// Sets the visible state of the specified cells and all connected edges
        /// if includeEdges is true. The change is carried out using cellsToggled.
        /// This method fires mxEvent.TOGGLE_CELLS while the transaction is in
        /// progress.
        /// </summary>
        /// <param name="show"> Boolean that specifies the visible state to be assigned. </param>
        /// <param name="cells"> Array of cells whose visible state should be changed. If
        /// null is specified then the selection cells are used. </param>
        /// <returns> Returns the cells whose visible state was changed. </returns>
        public virtual object[] toggleCells(bool show, object[] cells, bool includeEdges)
        {
            if (cells == null)
            {
                cells = getSelectionCells();
            }

            // Adds all connected edges recursively
            if (includeEdges)
            {
                cells = addAllEdges(cells);
            }

            model.beginUpdate();
            try
            {
                cellsToggled(cells, show);
                fireEvent(new mxEventObject(mxEvent.TOGGLE_CELLS, "show", show, "cells", cells, "includeEdges", includeEdges));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Sets the visible state of the specified cells.
        /// </summary>
        /// <param name="cells"> Array of cells whose visible state should be changed. </param>
        /// <param name="show"> Boolean that specifies the visible state to be assigned. </param>
        public virtual void cellsToggled(object[] cells, bool show)
        {
            if (cells != null && cells.Length > 0)
            {
                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        model.setVisible(cells[i], show);
                    }
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        //
        // Folding
        //

        /// <summary>
        /// Sets the collapsed state of the selection cells without recursion.
        /// This is a shortcut method.
        /// </summary>
        /// <param name="collapse"> Boolean that specifies the collapsed state to be
        /// assigned. </param>
        /// <returns> Returns the cells whose collapsed state was changed. </returns>
        public virtual object[] foldCells(bool collapse)
        {
            return foldCells(collapse, false);
        }

        /// <summary>
        /// Sets the collapsed state of the selection cells. This is a shortcut
        /// method.
        /// </summary>
        /// <param name="collapse"> Boolean that specifies the collapsed state to be
        /// assigned. </param>
        /// <param name="recurse"> Boolean that specifies if the collapsed state should
        /// be assigned to all descendants. </param>
        /// <returns> Returns the cells whose collapsed state was changed. </returns>
        public virtual object[] foldCells(bool collapse, bool recurse)
        {
            return foldCells(collapse, recurse, null);
        }

        /// <summary>
        /// Sets the collapsed state of the specified cells and all descendants
        /// if recurse is true. The change is carried out using cellsFolded.
        /// This method fires mxEvent.FOLD_CELLS while the transaction is in
        /// progress. Returns the cells whose collapsed state was changed.
        /// </summary>
        /// <param name="collapse"> Boolean indicating the collapsed state to be assigned. </param>
        /// <param name="recurse"> Boolean indicating if the collapsed state of all
        /// descendants should be set. </param>
        /// <param name="cells"> Array of cells whose collapsed state should be set. If
        /// null is specified then the foldable selection cells are used. </param>
        public virtual object[] foldCells(bool collapse, bool recurse, object[] cells)
        {
            if (cells == null)
            {
                cells = getFoldableCells(getSelectionCells(), collapse);
            }

            model.beginUpdate();
            try
            {
                cellsFolded(cells, collapse, recurse);
                fireEvent(new mxEventObject(mxEvent.FOLD_CELLS, "cells", cells, "collapse", collapse, "recurse", recurse));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Sets the collapsed state of the specified cells. This method fires
        /// mxEvent.CELLS_FOLDED while the transaction is in progress. Returns the
        /// cells whose collapsed state was changed.
        /// </summary>
        /// <param name="cells"> Array of cells whose collapsed state should be set. </param>
        /// <param name="collapse"> Boolean indicating the collapsed state to be assigned. </param>
        /// <param name="recurse"> Boolean indicating if the collapsed state of all
        /// descendants should be set. </param>
        public virtual void cellsFolded(object[] cells, bool collapse, bool recurse)
        {
            if (cells != null && cells.Length > 0)
            {
                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (collapse != isCellCollapsed(cells[i]))
                        {
                            model.setCollapsed(cells[i], collapse);
                            swapBounds(cells[i], collapse);

                            if (isExtendParent(cells[i]))
                            {
                                extendParent(cells[i]);
                            }

                            if (recurse)
                            {
                                object[] children = mxGraphModel.getChildren(model, cells[i]);
                                cellsFolded(children, collapse, recurse);
                            }
                        }
                    }

                    fireEvent(new mxEventObject(mxEvent.CELLS_FOLDED, "cells", cells, "collapse", collapse, "recurse", recurse));
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Swaps the alternate and the actual bounds in the geometry of the given
        /// cell invoking updateAlternateBounds before carrying out the swap.
        /// </summary>
        /// <param name="cell"> Cell for which the bounds should be swapped. </param>
        /// <param name="willCollapse"> Boolean indicating if the cell is going to be collapsed. </param>
        public virtual void swapBounds(object cell, bool willCollapse)
        {
            if (cell != null)
            {
                mxGeometry geo = model.getGeometry(cell);

                if (geo != null)
                {
                    geo = (mxGeometry)geo.clone();

                    updateAlternateBounds(cell, geo, willCollapse);
                    geo.swap();

                    model.setGeometry(cell, geo);
                }
            }
        }

        /// <summary>
        /// Updates or sets the alternate bounds in the given geometry for the given
        /// cell depending on whether the cell is going to be collapsed. If no
        /// alternate bounds are defined in the geometry and
        /// collapseToPreferredSize is true, then the preferred size is used for
        /// the alternate bounds. The top, left corner is always kept at the same
        /// location.
        /// </summary>
        /// <param name="cell"> Cell for which the geometry is being udpated. </param>
        /// <param name="geo"> Geometry for which the alternate bounds should be updated. </param>
        /// <param name="willCollapse"> Boolean indicating if the cell is going to be collapsed. </param>
        public virtual void updateAlternateBounds(object cell, mxGeometry geo, bool willCollapse)
        {
            if (cell != null && geo != null)
            {
                if (geo.AlternateBounds == null)
                {
                    mxRectangle bounds = null;

                    if (CollapseToPreferredSize)
                    {
                        bounds = getPreferredSizeForCell(cell);

                        if (isSwimlane(cell))
                        {
                            mxRectangle size = getStartSize(cell);

                            bounds.Height = Math.Max(bounds.Height, size.Height);
                            bounds.Width = Math.Max(bounds.Width, size.Width);
                        }
                    }

                    if (bounds == null)
                    {
                        bounds = geo;
                    }

                    geo.AlternateBounds = new mxRectangle(geo.X, geo.Y, bounds.Width, bounds.Height);
                }
                else
                {
                    geo.AlternateBounds.X = geo.X;
                    geo.AlternateBounds.Y = geo.Y;
                }
            }
        }

        /// <summary>
        /// Returns an array with the given cells and all edges that are connected
        /// to a cell or one of its descendants.
        /// </summary>
        public virtual object[] addAllEdges(object[] cells)
        {
            IList<object> allCells = new List<object>(cells.Length);
            ((List<object>)allCells).AddRange(Arrays.asList(cells));
            ((List<object>)allCells).AddRange(Arrays.asList(getAllEdges(cells)));

            return allCells.ToArray();
        }

        /// <summary>
        /// Returns all edges connected to the given cells or their descendants.
        /// </summary>
        public virtual object[] getAllEdges(object[] cells)
        {
            IList<object> edges = new List<object>();

            if (cells != null)
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    int edgeCount = model.getEdgeCount(cells[i]);

                    for (int j = 0; j < edgeCount; j++)
                    {
                        edges.Add(model.getEdgeAt(cells[i], j));
                    }

                    // Recurses
                    object[] children = mxGraphModel.getChildren(model, cells[i]);
                    ((List<object>)edges).AddRange(Arrays.asList(getAllEdges(children)));
                }
            }

            return edges.ToArray();
        }

        //
        // Cell sizing
        //

        /// <summary>
        /// Updates the size of the given cell in the model using
        /// getPreferredSizeForCell to get the new size. This function
        /// fires beforeUpdateSize and afterUpdateSize events.
        /// </summary>
        /// <param name="cell"> <mxCell> for which the size should be changed. </param>
        public virtual object updateCellSize(object cell)
        {
            return updateCellSize(cell, false);
        }

        /// <summary>
        /// Updates the size of the given cell in the model using
        /// getPreferredSizeForCell to get the new size. This function
        /// fires mxEvent.UPDATE_CELL_SIZE.
        /// </summary>
        /// <param name="cell"> Cell for which the size should be changed. </param>
        public virtual object updateCellSize(object cell, bool ignoreChildren)
        {
            model.beginUpdate();
            try
            {
                cellSizeUpdated(cell, ignoreChildren);
                fireEvent(new mxEventObject(mxEvent.UPDATE_CELL_SIZE, "cell", cell, "ignoreChildren", ignoreChildren));
            }
            finally
            {
                model.endUpdate();
            }

            return cell;
        }

        /// <summary>
        /// Updates the size of the given cell in the model using
        /// getPreferredSizeForCell to get the new size.
        /// </summary>
        /// <param name="cell"> Cell for which the size should be changed. </param>
        public virtual void cellSizeUpdated(object cell, bool ignoreChildren)
        {
            if (cell != null)
            {
                model.beginUpdate();
                try
                {
                    mxRectangle size = getPreferredSizeForCell(cell);
                    mxGeometry geo = model.getGeometry(cell);

                    if (size != null && geo != null)
                    {
                        bool collapsed = isCellCollapsed(cell);
                        geo = (mxGeometry)geo.clone();

                        if (isSwimlane(cell))
                        {
                            mxCellState state = view.getState(cell);
                            IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cell);
                            string cellStyle = model.getStyle(cell);

                            if (string.ReferenceEquals(cellStyle, null))
                            {
                                cellStyle = "";
                            }

                            if (mxUtils.isTrue(style, mxConstants.STYLE_HORIZONTAL, true))
                            {
                                cellStyle = mxUtils.setStyle(cellStyle, mxConstants.STYLE_STARTSIZE, (size.Height + 8).ToString());

                                if (collapsed)
                                {
                                    geo.Height = size.Height + 8;
                                }

                                geo.Width = size.Width;
                            }
                            else
                            {
                                cellStyle = mxUtils.setStyle(cellStyle, mxConstants.STYLE_STARTSIZE, (size.Width + 8).ToString());

                                if (collapsed)
                                {
                                    geo.Width = size.Width + 8;
                                }

                                geo.Height = size.Height;
                            }

                            model.setStyle(cell, cellStyle);
                        }
                        else
                        {
                            geo.Width = size.Width;
                            geo.Height = size.Height;
                        }

                        if (!ignoreChildren && !collapsed)
                        {
                            mxRectangle bounds = view.getBounds(mxGraphModel.getChildren(model, cell));

                            if (bounds != null)
                            {
                                mxPoint tr = view.Translate;
                                double scale = view.Scale;

                                double width = (bounds.X + bounds.Width) / scale - geo.X - tr.X;
                                double height = (bounds.Y + bounds.Height) / scale - geo.Y - tr.Y;

                                geo.Width = Math.Max(geo.Width, width);
                                geo.Height = Math.Max(geo.Height, height);
                            }
                        }

                        cellsResized(new object[] { cell }, new mxRectangle[] { geo });
                    }
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Returns the preferred width and height of the given <mxCell> as an
        /// <mxRectangle>.
        /// </summary>
        /// <param name="cell"> <mxCell> for which the preferred size should be returned. </param>
        public virtual mxRectangle getPreferredSizeForCell(object cell)
        {
            mxRectangle result = null;

            if (cell != null)
            {
                mxCellState state = view.getState(cell);
                IDictionary<string, object> style = (state != null) ? state.style : getCellStyle(cell);

                if (style != null && !model.isEdge(cell))
                {
                    double dx = 0;
                    double dy = 0;

                    // Adds dimension of image if shape is a label
                    if (!string.ReferenceEquals(getImage(state), null) || !string.ReferenceEquals(mxUtils.getString(style, mxConstants.STYLE_IMAGE), null))
                    {
                        if (mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_LABEL))
                        {
                            if (mxUtils.getString(style, mxConstants.STYLE_VERTICAL_ALIGN, "").Equals(mxConstants.ALIGN_MIDDLE))
                            {
                                dx += mxUtils.getDouble(style, mxConstants.STYLE_IMAGE_WIDTH, mxConstants.DEFAULT_IMAGESIZE);
                            }

                            if (mxUtils.getString(style, mxConstants.STYLE_ALIGN, "").Equals(mxConstants.ALIGN_CENTER))
                            {
                                dy += mxUtils.getDouble(style, mxConstants.STYLE_IMAGE_HEIGHT, mxConstants.DEFAULT_IMAGESIZE);
                            }
                        }
                    }

                    // Adds spacings
                    double spacing = mxUtils.getDouble(style, mxConstants.STYLE_SPACING);
                    dx += 2 * spacing;
                    dx += mxUtils.getDouble(style, mxConstants.STYLE_SPACING_LEFT);
                    dx += mxUtils.getDouble(style, mxConstants.STYLE_SPACING_RIGHT);

                    dy += 2 * spacing;
                    dy += mxUtils.getDouble(style, mxConstants.STYLE_SPACING_TOP);
                    dy += mxUtils.getDouble(style, mxConstants.STYLE_SPACING_BOTTOM);

                    // LATER: Add space for collapse/expand icon if applicable

                    // Adds space for label
                    string value = getLabel(cell);

                    if (!string.ReferenceEquals(value, null) && value.Length > 0)
                    {
                        // FIXME: Check word-wrap style and balance width/height
                        mxRectangle size = mxUtils.getLabelSize(value, style, isHtmlLabel(cell), 0);
                        double width = size.Width + dx;
                        double height = size.Height + dy;

                        if (!mxUtils.isTrue(style, mxConstants.STYLE_HORIZONTAL, true))
                        {
                            double tmp = height;

                            height = width;
                            width = tmp;
                        }

                        if (gridEnabled)
                        {
                            width = snap(width + gridSize / 2);
                            height = snap(height + gridSize / 2);
                        }

                        result = new mxRectangle(0, 0, width, height);
                    }
                    else
                    {
                        double gs2 = 4 * gridSize;
                        result = new mxRectangle(0, 0, gs2, gs2);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the bounds of the given cell using resizeCells. Returns the
        /// cell which was passed to the function.
        /// </summary>
        /// <param name="cell"> <mxCell> whose bounds should be changed. </param>
        /// <param name="bounds"> <mxRectangle> that represents the new bounds. </param>
        public virtual object resizeCell(object cell, mxRectangle bounds)
        {
            return resizeCells(new object[] { cell }, new mxRectangle[] { bounds })[0];
        }

        /// <summary>
        /// Sets the bounds of the given cells and fires a mxEvent.RESIZE_CELLS
        /// event. while the transaction is in progress. Returns the cells which
        /// have been passed to the function.
        /// </summary>
        /// <param name="cells"> Array of cells whose bounds should be changed. </param>
        /// <param name="bounds"> Array of rectangles that represents the new bounds. </param>
        public virtual object[] resizeCells(object[] cells, mxRectangle[] bounds)
        {
            model.beginUpdate();
            try
            {
                cellsResized(cells, bounds);
                fireEvent(new mxEventObject(mxEvent.RESIZE_CELLS, "cells", cells, "bounds", bounds));
            }
            finally
            {
                model.endUpdate();
            }

            return cells;
        }

        /// <summary>
        /// Sets the bounds of the given cells and fires a <mxEvent.CELLS_RESIZED>
        /// event. If extendParents is true, then the parent is extended if a child
        /// size is changed so that it overlaps with the parent.
        /// </summary>
        /// <param name="cells"> Array of <mxCells> whose bounds should be changed. </param>
        /// <param name="bounds"> Array of <mxRectangles> that represents the new bounds. </param>
        public virtual void cellsResized(object[] cells, mxRectangle[] bounds)
        {
            if (cells != null && bounds != null && cells.Length == bounds.Length)
            {
                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        mxRectangle tmp = bounds[i];
                        mxGeometry geo = model.getGeometry(cells[i]);

                        if (geo != null && (geo.X != tmp.X || geo.Y != tmp.Y || geo.Width != tmp.Width || geo.Height != tmp.Height))
                        {
                            geo = (mxGeometry)geo.clone();

                            if (geo.Relative)
                            {
                                mxPoint offset = geo.Offset;

                                if (offset != null)
                                {
                                    offset.setX(offset.X + tmp.X - geo.X);
                                    offset.setY(offset.Y + tmp.Y - geo.Y);
                                }
                            }
                            else
                            {
                                geo.X = tmp.X;
                                geo.Y = tmp.Y;
                            }

                            geo.Width = tmp.Width;
                            geo.Height = tmp.Height;

                            if (!geo.Relative && model.isVertex(cells[i]) && !AllowNegativeCoordinates)
                            {
                                geo.X = Math.Max(0, geo.X);
                                geo.Y = Math.Max(0, geo.Y);
                            }

                            model.setGeometry(cells[i], geo);

                            if (isExtendParent(cells[i]))
                            {
                                extendParent(cells[i]);
                            }
                        }
                    }

                    if (ResetEdgesOnResize)
                    {
                        resetEdges(cells);
                    }

                    // RENAME BOUNDSARRAY TO BOUNDS
                    fireEvent(new mxEventObject(mxEvent.CELLS_RESIZED, "cells", cells, "bounds", bounds));
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Resizes the parents recursively so that they contain the complete area
        /// of the resized child cell.
        /// </summary>
        /// <param name="cell"> <mxCell> that has been resized. </param>
        public virtual void extendParent(object cell)
        {
            if (cell != null)
            {
                object parent = model.getParent(cell);
                mxGeometry p = model.getGeometry(parent);

                if (parent != null && p != null && !isCellCollapsed(parent))
                {
                    mxGeometry geo = model.getGeometry(cell);

                    if (geo != null && (p.Width < geo.X + geo.Width || p.Height < geo.Y + geo.Height))
                    {
                        p = (mxGeometry)p.clone();

                        p.Width = Math.Max(p.Width, geo.X + geo.Width);
                        p.Height = Math.Max(p.Height, geo.Y + geo.Height);

                        cellsResized(new object[] { parent }, new mxRectangle[] { p });
                    }
                }
            }
        }

        //
        // Cell moving
        //

        /// <summary>
        /// Moves the cells by the given amount. This is a shortcut method.
        /// </summary>
        public virtual object[] moveCells(object[] cells, double dx, double dy)
        {
            return moveCells(cells, dx, dy, false);
        }

        /// <summary>
        /// Moves or clones the cells and moves the cells or clones by the given
        /// amount. This is a shortcut method.
        /// </summary>
        public virtual object[] moveCells(object[] cells, double dx, double dy, bool clone)
        {
            return moveCells(cells, dx, dy, clone, null, null);
        }

        /// <summary>
        /// Moves or clones the specified cells and moves the cells or clones by the
        /// given amount, adding them to the optional target cell. The location is
        /// the position of the mouse pointer as the mouse was released. The change
        /// is carried out using cellsMoved. This method fires mxEvent.MOVE_CELLS
        /// while the transaction is in progress.
        /// </summary>
        /// <param name="cells"> Array of cells to be moved, cloned or added to the target. </param>
        /// <param name="dx"> Integer that specifies the x-coordinate of the vector. </param>
        /// <param name="dy"> Integer that specifies the y-coordinate of the vector. </param>
        /// <param name="clone"> Boolean indicating if the cells should be cloned. </param>
        /// <param name="target"> Cell that represents the new parent of the cells. </param>
        /// <param name="location"> Location where the mouse was released. </param>
        /// <returns> Returns the cells that were moved. </returns>
        public virtual object[] moveCells(object[] cells, double dx, double dy, bool clone, object target, Point location)
        {
            if (cells != null && (dx != 0 || dy != 0 || clone || target != null))
            {
                model.beginUpdate();
                try
                {
                    if (clone)
                    {
                        cells = cloneCells(cells, CloneInvalidEdges);

                        if (target == null)
                        {
                            target = DefaultParent;
                        }
                    }

                    cellsMoved(cells, dx, dy, !clone && DisconnectOnMove && AllowDanglingEdges, target == null);

                    if (target != null)
                    {
                        int? index = model.getChildCount(target);
                        cellsAdded(cells, target, index, null, null, true);
                    }

                    fireEvent(new mxEventObject(mxEvent.MOVE_CELLS, "cells", cells, "dx", dx, "dy", dy, "clone", clone, "target", target, "location", location));
                }
                finally
                {
                    model.endUpdate();
                }
            }

            return cells;
        }

        /// <summary>
        /// Moves the specified cells by the given vector, disconnecting the cells
        /// using disconnectGraph if disconnect is true. This method fires
        /// mxEvent.CELLS_MOVED while the transaction is in progress.
        /// </summary>
        public virtual void cellsMoved(object[] cells, double dx, double dy, bool disconnect, bool constrain)
        {
            if (cells != null && (dx != 0 || dy != 0))
            {
                model.beginUpdate();
                try
                {
                    if (disconnect)
                    {
                        disconnectGraph(cells);
                    }

                    for (int i = 0; i < cells.Length; i++)
                    {
                        translateCell(cells[i], dx, dy);

                        if (constrain)
                        {
                            constrainChild(cells[i]);
                        }
                    }

                    if (ResetEdgesOnMove)
                    {
                        resetEdges(cells);
                    }

                    fireEvent(new mxEventObject(mxEvent.CELLS_MOVED, "cells", cells, "dx", dx, "dy", dy, "disconnect", disconnect));
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Translates the geometry of the given cell and stores the new,
        /// translated geometry in the model as an atomic change.
        /// </summary>
        public virtual void translateCell(object cell, double dx, double dy)
        {
            mxGeometry geo = model.getGeometry(cell);

            if (geo != null)
            {
                geo = (mxGeometry)geo.clone();
                geo.translate(dx, dy);

                if (!geo.Relative && model.isVertex(cell) && !AllowNegativeCoordinates)
                {
                    geo.X = Math.Max(0, geo.X);
                    geo.Y = Math.Max(0, geo.Y);
                }

                if (geo.Relative && !model.isEdge(cell))
                {
                    if (geo.Offset == null)
                    {
                        geo.Offset = new mxPoint(dx, dy);
                    }
                    else
                    {
                        mxPoint offset = geo.Offset;

                        offset.setX(offset.X + dx);
                        offset.setY(offset.Y + dy);
                    }
                }

                model.setGeometry(cell, geo);
            }
        }

        /// <summary>
        /// Returns the mxRectangle inside which a cell is to be kept.
        /// </summary>
        public virtual mxRectangle getCellContainmentArea(object cell)
        {
            if (cell != null && !model.isEdge(cell))
            {
                object parent = model.getParent(cell);

                if (parent == DefaultParent || parent == CurrentRoot)
                {
                    return MaximumGraphBounds;
                }
                else if (parent != null && parent != DefaultParent)
                {
                    mxGeometry g = model.getGeometry(parent);

                    if (g != null)
                    {
                        double x = 0;
                        double y = 0;
                        double w = g.Width;
                        double h = g.Height;

                        if (isSwimlane(parent))
                        {
                            mxRectangle size = getStartSize(parent);

                            x = size.Width;
                            w -= size.Width;
                            y = size.Height;
                            h -= size.Height;
                        }

                        return new mxRectangle(x, y, w, h);
                    }
                }
            }

            return null;
        }

        /// <returns> the maximumGraphBounds </returns>
        public virtual mxRectangle MaximumGraphBounds
        {
            get
            {
                return maximumGraphBounds;
            }
            set
            {
                mxRectangle oldValue = maximumGraphBounds;
                maximumGraphBounds = value;

                changeSupport.firePropertyChange("maximumGraphBounds", oldValue, maximumGraphBounds);
            }
        }


        /// <summary>
        /// Keeps the given cell inside the bounds returned by
        /// getCellContainmentArea for its parent, according to the rules defined by
        /// getOverlap and isConstrainChild. This modifies the cell's geometry
        /// in-place and does not clone it.
        /// </summary>
        /// <param name="cell"> Cell which should be constrained. </param>
        public virtual void constrainChild(object cell)
        {
            if (cell != null)
            {
                mxGeometry geo = model.getGeometry(cell);
                mxRectangle area = (isConstrainChild(cell)) ? getCellContainmentArea(cell) : MaximumGraphBounds;

                if (geo != null && area != null)
                {
                    // Keeps child within the content area of the parent
                    if (!geo.Relative && (geo.X < area.X || geo.Y < area.Y || area.Width < geo.X + geo.Width || area.Height < geo.Y + geo.Height))
                    {
                        double overlap = getOverlap(cell);

                        if (area.Width > 0)
                        {
                            geo.X = Math.Min(geo.X, area.X + area.Width - (1 - overlap) * geo.Width);
                        }

                        if (area.Height > 0)
                        {
                            geo.Y = Math.Min(geo.Y, area.Y + area.Height - (1 - overlap) * geo.Height);
                        }

                        geo.X = Math.Max(geo.X, area.X - geo.Width * overlap);
                        geo.Y = Math.Max(geo.Y, area.Y - geo.Height * overlap);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the control points of the edges that are connected to the given
        /// cells if not both ends of the edge are in the given cells array.
        /// </summary>
        /// <param name="cells"> Array of mxCells for which the connected edges should be
        /// reset. </param>
        public virtual void resetEdges(object[] cells)
        {
            if (cells != null)
            {
                // Prepares a hashtable for faster cell lookups
                HashSet<object> set = new HashSet<object>(Arrays.asList(cells));

                model.beginUpdate();
                try
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        object[] edges = mxGraphModel.getEdges(model, cells[i]);

                        if (edges != null)
                        {
                            for (int j = 0; j < edges.Length; j++)
                            {
                                object source = view.getVisibleTerminal(edges[j], true);
                                object target = view.getVisibleTerminal(edges[j], false);

                                // Checks if one of the terminals is not in the given array
                                if (!set.Contains(source) || !set.Contains(target))
                                {
                                    resetEdge(edges[j]);
                                }
                            }
                        }

                        resetEdges(mxGraphModel.getChildren(model, cells[i]));
                    }
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Resets the control points of the given edge.
        /// </summary>
        public virtual object resetEdge(object edge)
        {
            mxGeometry geo = model.getGeometry(edge);

            if (geo != null)
            {
                // Resets the control points
                IList<mxPoint> points = geo.Points;

                if (points != null && points.Count > 0)
                {
                    geo = (mxGeometry)geo.clone();
                    geo.Points = null;
                    model.setGeometry(edge, geo);
                }
            }

            return edge;
        }

        //
        // Cell connecting and connection constraints
        //

        /// <summary>
        /// Returns an array of all constraints for the given terminal.
        /// </summary>
        /// <param name="terminal"> Cell state that represents the terminal. </param>
        /// <param name="source"> Specifies if the terminal is the source or target. </param>
        public virtual mxConnectionConstraint[] getAllConnectionConstraints(mxCellState terminal, bool source)
        {
            return null;
        }

        /// <summary>
        /// Returns an connection constraint that describes the given connection
        /// point. This result can then be passed to getConnectionPoint.
        /// </summary>
        /// <param name="edge"> Cell state that represents the edge. </param>
        /// <param name="terminal"> Cell state that represents the terminal. </param>
        /// <param name="source"> Boolean indicating if the terminal is the source or target. </param>
        public virtual mxConnectionConstraint getConnectionConstraint(mxCellState edge, mxCellState terminal, bool source)
        {
            mxPoint point = null;
            object x = edge.Style[(source) ? mxConstants.STYLE_EXIT_X : mxConstants.STYLE_ENTRY_X];

            if (x != null)
            {
                object y = edge.Style[(source) ? mxConstants.STYLE_EXIT_Y : mxConstants.STYLE_ENTRY_Y];

                if (y != null)
                {
                    point = new mxPoint(double.Parse(x.ToString()), double.Parse(y.ToString()));
                }
            }

            bool perimeter = false;

            if (point != null)
            {
                perimeter = mxUtils.isTrue(edge.style, (source) ? mxConstants.STYLE_EXIT_PERIMETER : mxConstants.STYLE_ENTRY_PERIMETER, true);
            }

            return new mxConnectionConstraint(point, perimeter);
        }

        /// <summary>
        /// Sets the connection constraint that describes the given connection point.
        /// If no constraint is given then nothing is changed. To remove an existing
        /// constraint from the given edge, use an empty constraint instead.
        /// </summary>
        /// <param name="edge"> Cell that represents the edge. </param>
        /// <param name="terminal"> Cell that represents the terminal. </param>
        /// <param name="source"> Boolean indicating if the terminal is the source or target. </param>
        /// <param name="constraint"> Optional connection constraint to be used for this connection. </param>
        public virtual void setConnectionConstraint(object edge, object terminal, bool source, mxConnectionConstraint constraint)
        {
            if (constraint != null)
            {
                model.beginUpdate();
                try
                {
                    object[] cells = new object[] { edge };

                    // FIXME, constraint can't be null, we've checked that above
                    if (constraint == null || constraint.point == null)
                    {
                        setCellStyles((source) ? mxConstants.STYLE_EXIT_X : mxConstants.STYLE_ENTRY_X, null, cells);
                        setCellStyles((source) ? mxConstants.STYLE_EXIT_Y : mxConstants.STYLE_ENTRY_Y, null, cells);
                        setCellStyles((source) ? mxConstants.STYLE_EXIT_PERIMETER : mxConstants.STYLE_ENTRY_PERIMETER, null, cells);
                    }
                    else if (constraint.point != null)
                    {
                        setCellStyles((source) ? mxConstants.STYLE_EXIT_X : mxConstants.STYLE_ENTRY_X, constraint.point.X.ToString(), cells);
                        setCellStyles((source) ? mxConstants.STYLE_EXIT_Y : mxConstants.STYLE_ENTRY_Y, constraint.point.Y.ToString(), cells);

                        // Only writes 0 since 1 is default
                        if (!constraint.perimeter)
                        {
                            setCellStyles((source) ? mxConstants.STYLE_EXIT_PERIMETER : mxConstants.STYLE_ENTRY_PERIMETER, "0", cells);
                        }
                        else
                        {
                            setCellStyles((source) ? mxConstants.STYLE_EXIT_PERIMETER : mxConstants.STYLE_ENTRY_PERIMETER, null, cells);
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
        /// Sets the connection constraint that describes the given connection point.
        /// If no constraint is given then nothing is changed. To remove an existing
        /// constraint from the given edge, use an empty constraint instead.
        /// </summary>
        /// <param name="vertex"> Cell state that represents the vertex. </param>
        /// <param name="constraint"> Connection constraint that represents the connection point
        /// constraint as returned by getConnectionConstraint. </param>
        public virtual mxPoint getConnectionPoint(mxCellState vertex, mxConnectionConstraint constraint)
        {
            mxPoint point = null;

            if (vertex != null && constraint.point != null)
            {
                point = new mxPoint(vertex.X + constraint.Point.X * vertex.Width, vertex.Y + constraint.Point.Y * vertex.Height);
            }

            if (point != null && constraint.perimeter)
            {
                point = view.getPerimeterPoint(vertex, point, false);
            }

            return point;
        }

        /// <summary>
        /// Connects the specified end of the given edge to the given terminal
        /// using cellConnected and fires mxEvent.CONNECT_CELL while the transaction
        /// is in progress.
        /// </summary>
        public virtual object connectCell(object edge, object terminal, bool source)
        {
            return connectCell(edge, terminal, source, null);
        }

        /// <summary>
        /// Connects the specified end of the given edge to the given terminal
        /// using cellConnected and fires mxEvent.CONNECT_CELL while the transaction
        /// is in progress.
        /// </summary>
        /// <param name="edge"> Edge whose terminal should be updated. </param>
        /// <param name="terminal"> New terminal to be used. </param>
        /// <param name="source"> Specifies if the new terminal is the source or target. </param>
        /// <param name="constraint"> Optional constraint to be used for this connection. </param>
        /// <returns> Returns the update edge. </returns>
        public virtual object connectCell(object edge, object terminal, bool source, mxConnectionConstraint constraint)
        {
            model.beginUpdate();
            try
            {
                object previous = model.getTerminal(edge, source);
                cellConnected(edge, terminal, source, constraint);
                fireEvent(new mxEventObject(mxEvent.CONNECT_CELL, "edge", edge, "terminal", terminal, "source", source, "previous", previous));
            }
            finally
            {
                model.endUpdate();
            }

            return edge;
        }

        /// <summary>
        /// Sets the new terminal for the given edge and resets the edge points if
        /// isResetEdgesOnConnect returns true. This method fires
        /// <mxEvent.CELL_CONNECTED> while the transaction is in progress.
        /// </summary>
        /// <param name="edge"> Edge whose terminal should be updated. </param>
        /// <param name="terminal"> New terminal to be used. </param>
        /// <param name="source"> Specifies if the new terminal is the source or target. </param>
        /// <param name="constraint"> Constraint to be used for this connection. </param>
        public virtual void cellConnected(object edge, object terminal, bool source, mxConnectionConstraint constraint)
        {
            if (edge != null)
            {
                model.beginUpdate();
                try
                {
                    object previous = model.getTerminal(edge, source);

                    // Updates the constraint
                    setConnectionConstraint(edge, terminal, source, constraint);

                    // Checks if the new terminal is a port
                    string id = null;

                    if (isPort(terminal) && terminal is mxICell)
                    {
                        id = ((mxICell)terminal).Id;
                        terminal = getTerminalForPort(terminal, source);
                    }

                    // Sets or resets all previous information for connecting to a child port
                    string key = (source) ? mxConstants.STYLE_SOURCE_PORT : mxConstants.STYLE_TARGET_PORT;
                    setCellStyles(key, id, new object[] { edge });
                    model.setTerminal(edge, terminal, source);

                    if (ResetEdgesOnConnect)
                    {
                        resetEdge(edge);
                    }

                    fireEvent(new mxEventObject(mxEvent.CELL_CONNECTED, "edge", edge, "terminal", terminal, "source", source, "previous", previous));
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        /// <summary>
        /// Disconnects the given edges from the terminals which are not in the
        /// given array.
        /// </summary>
        /// <param name="cells"> Array of <mxCells> to be disconnected. </param>
        public virtual void disconnectGraph(object[] cells)
        {
            if (cells != null)
            {
                model.beginUpdate();
                try
                {
                    double scale = view.Scale;
                    mxPoint tr = view.Translate;

                    // Prepares a hashtable for faster cell lookups
                    ISet<object> hash = new HashSet<object>();

                    for (int i = 0; i < cells.Length; i++)
                    {
                        hash.Add(cells[i]);
                    }

                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (model.isEdge(cells[i]))
                        {
                            mxGeometry geo = model.getGeometry(cells[i]);

                            if (geo != null)
                            {
                                mxCellState state = view.getState(cells[i]);
                                mxCellState pstate = view.getState(model.getParent(cells[i]));

                                if (state != null && pstate != null)
                                {
                                    geo = (mxGeometry)geo.clone();

                                    double dx = -pstate.Origin.X;
                                    double dy = -pstate.Origin.Y;

                                    object src = model.getTerminal(cells[i], true);

                                    if (src != null && isCellDisconnectable(cells[i], src, true))
                                    {
                                        while (src != null && !hash.Contains(src))
                                        {
                                            src = model.getParent(src);
                                        }

                                        if (src == null)
                                        {
                                            mxPoint pt = state.getAbsolutePoint(0);
                                            geo.setTerminalPoint(new mxPoint(pt.X / scale - tr.X + dx, pt.Y / scale - tr.Y + dy), true);
                                            model.setTerminal(cells[i], null, true);
                                        }
                                    }

                                    object trg = model.getTerminal(cells[i], false);

                                    if (trg != null && isCellDisconnectable(cells[i], trg, false))
                                    {
                                        while (trg != null && !hash.Contains(trg))
                                        {
                                            trg = model.getParent(trg);
                                        }

                                        if (trg == null)
                                        {
                                            int n = state.AbsolutePointCount - 1;
                                            mxPoint pt = state.getAbsolutePoint(n);
                                            geo.setTerminalPoint(new mxPoint(pt.X / scale - tr.X + dx, pt.Y / scale - tr.Y + dy), false);
                                            model.setTerminal(cells[i], null, false);
                                        }
                                    }
                                }

                                model.setGeometry(cells[i], geo);
                            }
                        }
                    }
                }
                finally
                {
                    model.endUpdate();
                }
            }
        }

        //
        // Drilldown
        //

        /// <summary>
        /// Returns the current root of the displayed cell hierarchy. This is a
        /// shortcut to <mxGraphView.currentRoot> in <view>.
        /// </summary>
        /// <returns> Returns the current root in the view. </returns>
        public virtual object CurrentRoot
        {
            get
            {
                return view.CurrentRoot;
            }
        }

        /// <summary>
        /// Returns the translation to be used if the given cell is the root cell as
        /// an <mxPoint>. This implementation returns null.
        /// </summary>
        /// <param name="cell"> Cell that represents the root of the view. </param>
        /// <returns> Returns the translation of the graph for the given root cell. </returns>
        public virtual mxPoint getTranslateForRoot(object cell)
        {
            return null;
        }

        /// <summary>
        /// Returns true if the given cell is a "port", that is, when connecting to
        /// it, the cell returned by getTerminalForPort should be used as the
        /// terminal and the port should be referenced by the ID in either the
        /// mxConstants.STYLE_SOURCE_PORT or the or the
        /// mxConstants.STYLE_TARGET_PORT. Note that a port should not be movable.
        /// This implementation always returns false.
        /// 
        /// A typical implementation of this method looks as follows:
        /// 
        /// <code>
        /// public boolean isPort(Object cell)
        /// {
        ///   mxGeometry geo = getCellGeometry(cell);
        /// 
        ///   return (geo != null) ? geo.isRelative() : false;
        /// }
        /// </code>
        /// </summary>
        /// <param name="cell"> Cell that represents the port. </param>
        /// <returns> Returns true if the cell is a port. </returns>
        public virtual bool isPort(object cell)
        {
            return false;
        }

        /// <summary>
        /// Returns the terminal to be used for a given port. This implementation
        /// always returns the parent cell.
        /// </summary>
        /// <param name="cell"> Cell that represents the port. </param>
        /// <param name="source"> If the cell is the source or target port. </param>
        /// <returns> Returns the terminal to be used for the given port. </returns>
        public virtual object getTerminalForPort(object cell, bool source)
        {
            return Model.getParent(cell);
        }

        /// <summary>
        /// Returns the offset to be used for the cells inside the given cell. The
        /// root and layer cells may be identified using mxGraphModel.isRoot and
        /// mxGraphModel.isLayer. This implementation returns null.
        /// </summary>
        /// <param name="cell"> Cell whose offset should be returned. </param>
        /// <returns> Returns the child offset for the given cell. </returns>
        public virtual mxPoint getChildOffsetForCell(object cell)
        {
            return null;
        }

        /// 
        public virtual void enterGroup()
        {
            enterGroup(null);
        }

        /// <summary>
        /// Uses the given cell as the root of the displayed cell hierarchy. If no
        /// cell is specified then the selection cell is used. The cell is only used
        /// if <isValidRoot> returns true.
        /// </summary>
        /// <param name="cell"> </param>
        public virtual void enterGroup(object cell)
        {
            if (cell == null)
            {
                cell = SelectionCell;
            }

            if (cell != null && isValidRoot(cell))
            {
                view.CurrentRoot = cell;
                clearSelection();
            }
        }

        /// <summary>
        /// Changes the current root to the next valid root in the displayed cell
        /// hierarchy.
        /// </summary>
        public virtual void exitGroup()
        {
            object root = model.Root;
            object current = CurrentRoot;

            if (current != null)
            {
                object next = model.getParent(current);

                // Finds the next valid root in the hierarchy
                while (next != root && !isValidRoot(next) && model.getParent(next) != root)
                {
                    next = model.getParent(next);
                }

                // Clears the current root if the new root is
                // the model's root or one of the layers.
                if (next == root || model.getParent(next) == root)
                {
                    view.CurrentRoot = null;
                }
                else
                {
                    view.CurrentRoot = next;
                }

                mxCellState state = view.getState(current);

                // Selects the previous root in the graph
                if (state != null)
                {
                    SelectionCell = current;
                }
            }
        }

        /// <summary>
        /// Uses the root of the model as the root of the displayed cell hierarchy
        /// and selects the previous root.
        /// </summary>
        public virtual void home()
        {
            object current = CurrentRoot;

            if (current != null)
            {
                view.CurrentRoot = null;
                mxCellState state = view.getState(current);

                if (state != null)
                {
                    SelectionCell = current;
                }
            }
        }

        /// <summary>
        /// Returns true if the given cell is a valid root for the cell display
        /// hierarchy. This implementation returns true for all non-null values.
        /// </summary>
        /// <param name="cell"> <mxCell> which should be checked as a possible root. </param>
        /// <returns> Returns true if the given cell is a valid root. </returns>
        public virtual bool isValidRoot(object cell)
        {
            return (cell != null);
        }

        //
        // Graph display
        //

        /// <summary>
        /// Returns the bounds of the visible graph.
        /// </summary>
        public virtual mxRectangle GraphBounds
        {
            get
            {
                return view.GraphBounds;
            }
        }

        /// <summary>
        /// Returns the bounds of the given cell.
        /// </summary>
        public virtual mxRectangle getCellBounds(object cell)
        {
            return getCellBounds(cell, false);
        }

        /// <summary>
        /// Returns the bounds of the given cell including all connected edges
        /// if includeEdge is true.
        /// </summary>
        public virtual mxRectangle getCellBounds(object cell, bool includeEdges)
        {
            return getCellBounds(cell, includeEdges, false);
        }

        /// <summary>
        /// Returns the bounds of the given cell including all connected edges
        /// if includeEdge is true.
        /// </summary>
        public virtual mxRectangle getCellBounds(object cell, bool includeEdges, bool includeDescendants)
        {
            return getCellBounds(cell, includeEdges, includeDescendants, false);
        }

        /// <summary>
        /// Returns the bounding box for the geometries of the vertices in the
        /// given array of cells.
        /// </summary>
        public virtual mxRectangle getBoundingBoxFromGeometry(object[] cells)
        {
            mxRectangle result = null;

            if (cells != null)
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    if (Model.isVertex(cells[i]))
                    {
                        mxGeometry geo = getCellGeometry(cells[i]);

                        if (result == null)
                        {
                            result = new mxRectangle(geo);
                        }
                        else
                        {
                            result.add(geo);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the bounds of the given cell.
        /// </summary>
        public virtual mxRectangle getBoundingBox(object cell)
        {
            return getBoundingBox(cell, false);
        }

        /// <summary>
        /// Returns the bounding box of the given cell including all connected edges
        /// if includeEdge is true.
        /// </summary>
        public virtual mxRectangle getBoundingBox(object cell, bool includeEdges)
        {
            return getBoundingBox(cell, includeEdges, false);
        }

        /// <summary>
        /// Returns the bounding box of the given cell including all connected edges
        /// if includeEdge is true.
        /// </summary>
        public virtual mxRectangle getBoundingBox(object cell, bool includeEdges, bool includeDescendants)
        {
            return getCellBounds(cell, includeEdges, includeDescendants, true);
        }

        /// <summary>
        /// Returns the bounding box of the given cells and their descendants.
        /// </summary>
        public virtual mxRectangle getPaintBounds(object[] cells)
        {
            return getBoundsForCells(cells, false, true, true);
        }

        /// <summary>
        /// Returns the bounds for the given cells.
        /// </summary>
        public virtual mxRectangle getBoundsForCells(object[] cells, bool includeEdges, bool includeDescendants, bool boundingBox)
        {
            mxRectangle result = null;

            if (cells != null && cells.Length > 0)
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    mxRectangle tmp = getCellBounds(cells[i], includeEdges, includeDescendants, boundingBox);

                    if (tmp != null)
                    {
                        if (result == null)
                        {
                            result = new mxRectangle(tmp);
                        }
                        else
                        {
                            result.add(tmp);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the bounds of the given cell including all connected edges
        /// if includeEdge is true.
        /// </summary>
        public virtual mxRectangle getCellBounds(object cell, bool includeEdges, bool includeDescendants, bool boundingBox)
        {
            mxRectangle result = null;
            object[] cells;

            // Recursively includes connected edges
            if (includeEdges)
            {
                ISet<object> allCells = new HashSet<object>();
                allCells.Add(cell);

                ISet<object> edges = new HashSet<object>(Arrays.asList(getEdges(cell)));

                //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
                while (edges.Count > 0 && !allCells.containsAll(edges))
                {
                    allCells.addAll(edges);

                    ISet<object> tmp = new HashSet<object>();
                    IEnumerator<object> it = edges.GetEnumerator();

                    while (it.MoveNext())
                    {
                        object edge = it.Current;
                        tmp.addAll(Arrays.asList(getEdges(edge)));
                    }

                    edges = tmp;
                }

                cells = allCells.ToArray();
            }
            else
            {
                cells = new object[] { cell };
            }

            if (boundingBox)
            {
                result = view.getBoundingBox(cells);
            }
            else
            {
                result = view.getBounds(cells);
            }

            // Recursively includes the bounds of the children
            if (includeDescendants)
            {
                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    mxRectangle tmp = getCellBounds(model.getChildAt(cell, i), includeEdges, true, boundingBox);

                    if (result != null)
                    {
                        result.add(tmp);
                    }
                    else
                    {
                        result = tmp;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Clears all cell states or the states for the hierarchy starting at the
        /// given cell and validates the graph.
        /// </summary>
        public virtual void refresh()
        {
            view.reload();
            repaint();
        }

        /// <summary>
        /// Fires a repaint event.
        /// </summary>
        public virtual void repaint()
        {
            repaint(null);
        }

        /// <summary>
        /// Fires a repaint event. The optional region is the rectangle that needs
        /// to be repainted.
        /// </summary>
        public virtual void repaint(mxRectangle region)
        {
            fireEvent(new mxEventObject(mxEvent.REPAINT, "region", region));
        }

        /// <summary>
        /// Snaps the given numeric value to the grid if <gridEnabled> is true.
        /// </summary>
        /// <param name="value"> Numeric value to be snapped to the grid. </param>
        /// <returns> Returns the value aligned to the grid. </returns>
        public virtual double snap(double value)
        {
            if (gridEnabled)
            {
                value = Math.Round(value / gridSize) * gridSize;
            }

            return value;
        }

        /// <summary>
        /// Returns the geometry for the given cell.
        /// </summary>
        /// <param name="cell"> Cell whose geometry should be returned. </param>
        /// <returns> Returns the geometry of the cell. </returns>
        public virtual mxGeometry getCellGeometry(object cell)
        {
            return model.getGeometry(cell);
        }

        /// <summary>
        /// Returns true if the given cell is visible in this graph. This
        /// implementation uses <mxGraphModel.isVisible>. Subclassers can override
        /// this to implement specific visibility for cells in only one graph, that
        /// is, without affecting the visible state of the cell.
        /// 
        /// When using dynamic filter expressions for cell visibility, then the
        /// graph should be revalidated after the filter expression has changed.
        /// </summary>
        /// <param name="cell"> Cell whose visible state should be returned. </param>
        /// <returns> Returns the visible state of the cell. </returns>
        public virtual bool isCellVisible(object cell)
        {
            return model.isVisible(cell);
        }

        /// <summary>
        /// Returns true if the given cell is collapsed in this graph. This
        /// implementation uses <mxGraphModel.isCollapsed>. Subclassers can override
        /// this to implement specific collapsed states for cells in only one graph,
        /// that is, without affecting the collapsed state of the cell.
        /// 
        /// When using dynamic filter expressions for the collapsed state, then the
        /// graph should be revalidated after the filter expression has changed.
        /// </summary>
        /// <param name="cell"> Cell whose collapsed state should be returned. </param>
        /// <returns> Returns the collapsed state of the cell. </returns>
        public virtual bool isCellCollapsed(object cell)
        {
            return model.isCollapsed(cell);
        }

        /// <summary>
        /// Returns true if the given cell is connectable in this graph. This
        /// implementation uses <mxGraphModel.isConnectable>. Subclassers can override
        /// this to implement specific connectable states for cells in only one graph,
        /// that is, without affecting the connectable state of the cell in the model.
        /// </summary>
        /// <param name="cell"> Cell whose connectable state should be returned. </param>
        /// <returns> Returns the connectable state of the cell. </returns>
        public virtual bool isCellConnectable(object cell)
        {
            return model.isConnectable(cell);
        }

        /// <summary>
        /// Returns true if perimeter points should be computed such that the
        /// resulting edge has only horizontal or vertical segments.
        /// </summary>
        /// <param name="edge"> Cell state that represents the edge. </param>
        public virtual bool isOrthogonal(mxCellState edge)
        {
            if (edge.Style.ContainsKey(mxConstants.STYLE_ORTHOGONAL))
            {
                return mxUtils.isTrue(edge.Style, mxConstants.STYLE_ORTHOGONAL);
            }

            mxEdgeStyle.mxEdgeStyleFunction tmp = view.getEdgeStyle(edge, null, null, null);

            return tmp == mxEdgeStyle.ElbowConnector || tmp == mxEdgeStyle.SideToSide || tmp == mxEdgeStyle.TopToBottom || tmp == mxEdgeStyle.EntityRelation;
        }

        /// <summary>
        /// Returns true if the given cell state is a loop.
        /// </summary>
        /// <param name="state"> <mxCellState> that represents a potential loop. </param>
        /// <returns> Returns true if the given cell is a loop. </returns>
        public virtual bool isLoop(mxCellState state)
        {
            object src = view.getVisibleTerminal(state.Cell, true);
            object trg = view.getVisibleTerminal(state.Cell, false);

            return (src != null && src == trg);
        }

        //
        // Cell validation
        //

        /// 
        public virtual mxMultiplicity[] Multiplicities
        {
            set
            {
                mxMultiplicity[] oldValue = multiplicities;
                multiplicities = value;

                changeSupport.firePropertyChange("multiplicities", oldValue, multiplicities);
            }
            get
            {
                return multiplicities;
            }
        }


        /// <summary>
        /// Checks if the return value of getEdgeValidationError for the given
        /// arguments is null.
        /// </summary>
        /// <param name="edge"> Cell that represents the edge to validate. </param>
        /// <param name="source"> Cell that represents the source terminal. </param>
        /// <param name="target"> Cell that represents the target terminal. </param>
        public virtual bool isEdgeValid(object edge, object source, object target)
        {
            return string.ReferenceEquals(getEdgeValidationError(edge, source, target), null);
        }

        /// <summary>
        /// Returns the validation error message to be displayed when inserting or
        /// changing an edges' connectivity. A return value of null means the edge
        /// is valid, a return value of '' means it's not valid, but do not display
        /// an error message. Any other (non-empty) string returned from this method
        /// is displayed as an error message when trying to connect an edge to a
        /// source and target. This implementation uses the multiplicities, as
        /// well as multigraph and allowDanglingEdges to generate validation
        /// errors.
        /// </summary>
        /// <param name="edge"> Cell that represents the edge to validate. </param>
        /// <param name="source"> Cell that represents the source terminal. </param>
        /// <param name="target"> Cell that represents the target terminal. </param>
        public virtual string getEdgeValidationError(object edge, object source, object target)
        {
            if (edge != null && model.getTerminal(edge, true) == null && model.getTerminal(edge, false) == null)
            {
                return null;
            }

            // Checks if we're dealing with a loop
            if (!AllowLoops && source == target && source != null)
            {
                return "";
            }

            // Checks if the connection is generally allowed
            if (!isValidConnection(source, target))
            {
                return "";
            }

            if (source != null && target != null)
            {
                StringBuilder error = new StringBuilder();

                // Checks if the cells are already connected
                // and adds an error message if required			
                if (!multigraph)
                {
                    object[] tmp = mxGraphModel.getEdgesBetween(model, source, target, true);

                    // Checks if the source and target are not connected by another edge
                    if (tmp.Length > 1 || (tmp.Length == 1 && tmp[0] != edge))
                    {
                        error.Append(mxResources.get("alreadyConnected", "Already Connected") + "\n");
                    }
                }

                // Gets the number of outgoing edges from the source
                // and the number of incoming edges from the target
                // without counting the edge being currently changed.
                int sourceOut = mxGraphModel.getDirectedEdgeCount(model, source, true, edge);
                int targetIn = mxGraphModel.getDirectedEdgeCount(model, target, false, edge);

                // Checks the change against each multiplicity rule
                if (multiplicities != null)
                {
                    for (int i = 0; i < multiplicities.Length; i++)
                    {
                        string err = multiplicities[i].check(this, edge, source, target, sourceOut, targetIn);

                        if (!string.ReferenceEquals(err, null))
                        {
                            error.Append(err);
                        }
                    }
                }

                // Validates the source and target terminals independently
                string err = validateEdge(edge, source, target);

                if (!string.ReferenceEquals(err, null))
                {
                    error.Append(err);
                }

                return (error.Length > 0) ? error.ToString() : null;
            }

            return (allowDanglingEdges) ? null : "";
        }

        /// <summary>
        /// Hook method for subclassers to return an error message for the given
        /// edge and terminals. This implementation returns null.
        /// </summary>
        /// <param name="edge"> Cell that represents the edge to validate. </param>
        /// <param name="source"> Cell that represents the source terminal. </param>
        /// <param name="target"> Cell that represents the target terminal. </param>
        public virtual string validateEdge(object edge, object source, object target)
        {
            return null;
        }

        /// <summary>
        /// Checks all multiplicities that cannot be enforced while the graph is
        /// being modified, namely, all multiplicities that require a minimum of
        /// 1 edge.
        /// </summary>
        /// <param name="cell"> Cell for which the multiplicities should be checked. </param>
        public virtual string getCellValidationError(object cell)
        {
            int outCount = mxGraphModel.getDirectedEdgeCount(model, cell, true);
            int inCount = mxGraphModel.getDirectedEdgeCount(model, cell, false);
            StringBuilder error = new StringBuilder();
            object value = model.getValue(cell);

            for (int i = 0; i < multiplicities.Length; i++)
            {
                mxMultiplicity rule = multiplicities[i];
                int max = rule.MaxValue;

                if (rule.source && mxUtils.isNode(value, rule.type, rule.attr, rule.value) && ((max == 0 && outCount > 0) || (rule.min == 1 && outCount == 0) || (max == 1 && outCount > 1)))
                {
                    error.Append(rule.countError + '\n');
                }
                else if (!rule.source && mxUtils.isNode(value, rule.type, rule.attr, rule.value) && ((max == 0 && inCount > 0) || (rule.min == 1 && inCount == 0) || (max == 1 && inCount > 1)))
                {
                    error.Append(rule.countError + '\n');
                }
            }

            return (error.Length > 0) ? error.ToString() : null;
        }

        /// <summary>
        /// Hook method for subclassers to return an error message for the given
        /// cell and validation context. This implementation returns null.
        /// </summary>
        /// <param name="cell"> Cell that represents the cell to validate. </param>
        /// <param name="context"> Hashtable that represents the global validation state. </param>
        public virtual string validateCell(object cell, Dictionary<object, object> context)
        {
            return null;
        }

        //
        // Graph appearance
        //

        /// <returns> the labelsVisible </returns>
        public virtual bool LabelsVisible
        {
            get
            {
                return labelsVisible;
            }
            set
            {
                bool oldValue = labelsVisible;
                labelsVisible = value;

                changeSupport.firePropertyChange("labelsVisible", oldValue, labelsVisible);
            }
        }


        /// <param name="value"> the htmlLabels to set </param>
        public virtual bool HtmlLabels
        {
            set
            {
                bool oldValue = htmlLabels;
                htmlLabels = value;

                changeSupport.firePropertyChange("htmlLabels", oldValue, htmlLabels);
            }
            get
            {
                return htmlLabels;
            }
        }


        /// <summary>
        /// Returns the textual representation for the given cell.
        /// </summary>
        /// <param name="cell"> Cell to be converted to a string. </param>
        /// <returns> Returns the textual representation of the cell. </returns>
        public virtual string convertValueToString(object cell)
        {
            object result = model.getValue(cell);

            return (result != null) ? result.ToString() : "";
        }

        /// <summary>
        /// Returns a string or DOM node that represents the label for the given
        /// cell. This implementation uses <convertValueToString> if <labelsVisible>
        /// is true. Otherwise it returns an empty string.
        /// </summary>
        /// <param name="cell"> <mxCell> whose label should be returned. </param>
        /// <returns> Returns the label for the given cell. </returns>
        public virtual string getLabel(object cell)
        {
            string result = "";

            if (cell != null)
            {
                mxCellState state = view.getState(cell);
                IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cell);

                if (labelsVisible && !mxUtils.isTrue(style, mxConstants.STYLE_NOLABEL, false))
                {
                    result = convertValueToString(cell);
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the new label for a cell. If autoSize is true then
        /// <cellSizeUpdated> will be called.
        /// </summary>
        /// <param name="cell"> Cell whose label should be changed. </param>
        /// <param name="value"> New label to be assigned. </param>
        /// <param name="autoSize"> Specifies if cellSizeUpdated should be called. </param>
        public virtual void cellLabelChanged(object cell, object value, bool autoSize)
        {
            model.beginUpdate();
            try
            {
                Model.setValue(cell, value);

                if (autoSize)
                {
                    cellSizeUpdated(cell, false);
                }
            }
            finally
            {
                model.endUpdate();
            }
        }

        /// <summary>
        /// Returns true if the label must be rendered as HTML markup. The default
        /// implementation returns <htmlLabels>.
        /// </summary>
        /// <param name="cell"> <mxCell> whose label should be displayed as HTML markup. </param>
        /// <returns> Returns true if the given cell label is HTML markup. </returns>
        public virtual bool isHtmlLabel(object cell)
        {
            return HtmlLabels;
        }

        /// <summary>
        /// Returns the tooltip to be used for the given cell.
        /// </summary>
        public virtual string getToolTipForCell(object cell)
        {
            return convertValueToString(cell);
        }

        /// <summary>
        /// Returns the start size of the given swimlane, that is, the width or
        /// height of the part that contains the title, depending on the
        /// horizontal style. The return value is an <mxRectangle> with either
        /// width or height set as appropriate.
        /// </summary>
        /// <param name="swimlane"> <mxCell> whose start size should be returned. </param>
        /// <returns> Returns the startsize for the given swimlane. </returns>
        public virtual mxRectangle getStartSize(object swimlane)
        {
            mxRectangle result = new mxRectangle();
            mxCellState state = view.getState(swimlane);
            IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(swimlane);

            if (style != null)
            {
                double size = mxUtils.getDouble(style, mxConstants.STYLE_STARTSIZE, mxConstants.DEFAULT_STARTSIZE);

                if (mxUtils.isTrue(style, mxConstants.STYLE_HORIZONTAL, true))
                {
                    result.Height = size;
                }
                else
                {
                    result.Width = size;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the image URL for the given cell state. This implementation
        /// returns the value stored under <mxConstants.STYLE_IMAGE> in the cell
        /// style.
        /// </summary>
        /// <param name="state"> </param>
        /// <returns> Returns the image associated with the given cell state. </returns>
        public virtual string getImage(mxCellState state)
        {
            return (state != null && state.Style != null) ? mxUtils.getString(state.Style, mxConstants.STYLE_IMAGE) : null;
        }

        /// <summary>
        /// Returns the value of <border>.
        /// </summary>
        /// <returns> Returns the border. </returns>
        public virtual int Border
        {
            get
            {
                return border;
            }
            set
            {
                border = value;
            }
        }


        /// <summary>
        /// Returns the default edge style used for loops.
        /// </summary>
        /// <returns> Returns the default loop style. </returns>
        public virtual mxEdgeStyle.mxEdgeStyleFunction DefaultLoopStyle
        {
            get
            {
                return defaultLoopStyle;
            }
            set
            {
                mxEdgeStyle.mxEdgeStyleFunction oldValue = defaultLoopStyle;
                defaultLoopStyle = value;

                changeSupport.firePropertyChange("defaultLoopStyle", oldValue, defaultLoopStyle);
            }
        }


        /// <summary>
        /// Returns true if the given cell is a swimlane. This implementation always
        /// returns false.
        /// </summary>
        /// <param name="cell"> Cell that should be checked. </param>
        /// <returns> Returns true if the cell is a swimlane. </returns>
        public virtual bool isSwimlane(object cell)
        {
            if (cell != null)
            {
                if (model.getParent(cell) != model.Root)
                {
                    mxCellState state = view.getState(cell);
                    IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cell);

                    if (style != null && !model.isEdge(cell))
                    {
                        return mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_SWIMLANE);
                    }
                }
            }

            return false;
        }

        //
        // Cells and labels control options
        //

        /// <summary>
        /// Returns true if the given cell may not be moved, sized, bended,
        /// disconnected, edited or selected. This implementation returns true for
        /// all vertices with a relative geometry if cellsLocked is false.
        /// </summary>
        /// <param name="cell"> Cell whose locked state should be returned. </param>
        /// <returns> Returns true if the given cell is locked. </returns>
        public virtual bool isCellLocked(object cell)
        {
            mxGeometry geometry = model.getGeometry(cell);

            return CellsLocked || (geometry != null && model.isVertex(cell) && geometry.Relative);
        }

        /// <summary>
        /// Returns cellsLocked, the default return value for isCellLocked.
        /// </summary>
        public virtual bool CellsLocked
        {
            get
            {
                return cellsLocked;
            }
            set
            {
                bool oldValue = cellsLocked;
                cellsLocked = value;

                changeSupport.firePropertyChange("cellsLocked", oldValue, cellsLocked);
            }
        }


        /// <summary>
        /// Returns true if the given cell is movable. This implementation returns editable.
        /// </summary>
        /// <param name="cell"> Cell whose editable state should be returned. </param>
        /// <returns> Returns true if the cell is editable. </returns>
        public virtual bool isCellEditable(object cell)
        {
            return CellsEditable && !isCellLocked(cell);
        }

        /// <summary>
        /// Returns true if editing is allowed in this graph.
        /// </summary>
        /// <returns> Returns true if the graph is editable. </returns>
        public virtual bool CellsEditable
        {
            get
            {
                return cellsEditable;
            }
            set
            {
                bool oldValue = cellsEditable;
                cellsEditable = value;

                changeSupport.firePropertyChange("cellsEditable", oldValue, cellsEditable);
            }
        }


        /// <summary>
        /// Returns true if the given cell is resizable. This implementation returns
        /// cellsSizable for all cells.
        /// </summary>
        /// <param name="cell"> Cell whose resizable state should be returned. </param>
        /// <returns> Returns true if the cell is sizable. </returns>
        public virtual bool isCellResizable(object cell)
        {
            return CellsResizable && !isCellLocked(cell);
        }

        /// <summary>
        /// Returns true if the given cell is resizable. This implementation return sizable.
        /// </summary>
        public virtual bool CellsResizable
        {
            get
            {
                return cellsResizable;
            }
            set
            {
                bool oldValue = cellsResizable;
                cellsResizable = value;

                changeSupport.firePropertyChange("cellsResizable", oldValue, cellsResizable);
            }
        }


        /// <summary>
        /// Returns the cells which are movable in the given array of cells.
        /// </summary>
        public virtual object[] getMovableCells(object[] cells)
        {
            return mxGraphModel.filterCells(cells, new FilterAnonymousInnerClass(this));
        }

        private class FilterAnonymousInnerClass : mxGraphModel.Filter
        {
            private readonly mxGraph outerInstance;

            public FilterAnonymousInnerClass(mxGraph outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual bool filter(object cell)
            {
                return outerInstance.isCellMovable(cell);
            }
        }

        /// <summary>
        /// Returns true if the given cell is movable. This implementation
        /// returns movable.
        /// </summary>
        /// <param name="cell"> Cell whose movable state should be returned. </param>
        /// <returns> Returns true if the cell is movable. </returns>
        public virtual bool isCellMovable(object cell)
        {
            return CellsMovable && !isCellLocked(cell);
        }

        /// <summary>
        /// Returns cellsMovable.
        /// </summary>
        public virtual bool CellsMovable
        {
            get
            {
                return cellsMovable;
            }
            set
            {
                bool oldValue = cellsMovable;
                cellsMovable = value;

                changeSupport.firePropertyChange("cellsMovable", oldValue, cellsMovable);
            }
        }


        /// <summary>
        /// Returns true if the given cell is bendable. This implementation returns
        /// bendable. This is used in mxElbowEdgeHandler to determine if the middle
        /// handle should be shown.
        /// </summary>
        /// <param name="cell"> Cell whose bendable state should be returned. </param>
        /// <returns> Returns true if the cell is bendable. </returns>
        public virtual bool isCellBendable(object cell)
        {
            return CellsBendable && !isCellLocked(cell);
        }

        /// <summary>
        /// Returns cellsBendable.
        /// </summary>
        public virtual bool CellsBendable
        {
            get
            {
                return cellsBendable;
            }
            set
            {
                bool oldValue = cellsBendable;
                cellsBendable = value;

                changeSupport.firePropertyChange("cellsBendable", oldValue, cellsBendable);
            }
        }


        /// <summary>
        /// Returns true if the given cell is selectable. This implementation returns
        /// <selectable>.
        /// </summary>
        /// <param name="cell"> <mxCell> whose selectable state should be returned. </param>
        /// <returns> Returns true if the given cell is selectable. </returns>
        public virtual bool isCellSelectable(object cell)
        {
            return CellsSelectable;
        }

        /// <summary>
        /// Returns cellsSelectable.
        /// </summary>
        public virtual bool CellsSelectable
        {
            get
            {
                return cellsSelectable;
            }
            set
            {
                bool oldValue = cellsSelectable;
                cellsSelectable = value;

                changeSupport.firePropertyChange("cellsSelectable", oldValue, cellsSelectable);
            }
        }


        /// <summary>
        /// Returns the cells which are movable in the given array of cells.
        /// </summary>
        public virtual object[] getDeletableCells(object[] cells)
        {
            return mxGraphModel.filterCells(cells, new FilterAnonymousInnerClass2(this));
        }

        private class FilterAnonymousInnerClass2 : mxGraphModel.Filter
        {
            private readonly mxGraph outerInstance;

            public FilterAnonymousInnerClass2(mxGraph outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual bool filter(object cell)
            {
                return outerInstance.isCellDeletable(cell);
            }
        }

        /// <summary>
        /// Returns true if the given cell is movable. This implementation always
        /// returns true.
        /// </summary>
        /// <param name="cell"> Cell whose movable state should be returned. </param>
        /// <returns> Returns true if the cell is movable. </returns>
        public virtual bool isCellDeletable(object cell)
        {
            return CellsDeletable;
        }

        /// <summary>
        /// Returns cellsDeletable.
        /// </summary>
        public virtual bool CellsDeletable
        {
            get
            {
                return cellsDeletable;
            }
            set
            {
                bool oldValue = cellsDeletable;
                cellsDeletable = value;

                changeSupport.firePropertyChange("cellsDeletable", oldValue, cellsDeletable);
            }
        }


        /// <summary>
        /// Returns the cells which are movable in the given array of cells.
        /// </summary>
        public virtual object[] getCloneableCells(object[] cells)
        {
            return mxGraphModel.filterCells(cells, new FilterAnonymousInnerClass3(this));
        }

        private class FilterAnonymousInnerClass3 : mxGraphModel.Filter
        {
            private readonly mxGraph outerInstance;

            public FilterAnonymousInnerClass3(mxGraph outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual bool filter(object cell)
            {
                return outerInstance.isCellCloneable(cell);
            }
        }

        /// <summary>
        /// Returns the constant true. This does not use the cloneable field to
        /// return a value for a given cell, it is simply a hook for subclassers
        /// to disallow cloning of individual cells.
        /// </summary>
        public virtual bool isCellCloneable(object cell)
        {
            return CellsCloneable;
        }

        /// <summary>
        /// Returns cellsCloneable.
        /// </summary>
        public virtual bool CellsCloneable
        {
            get
            {
                return cellsCloneable;
            }
            set
            {
                bool oldValue = cellsCloneable;
                cellsCloneable = value;

                changeSupport.firePropertyChange("cellsCloneable", oldValue, cellsCloneable);
            }
        }


        /// <summary>
        /// Returns true if the given cell is disconnectable from the source or
        /// target terminal. This returns <disconnectable> for all given cells if
        /// <isLocked> does not return true for the given cell.
        /// </summary>
        /// <param name="cell"> <mxCell> whose disconnectable state should be returned. </param>
        /// <param name="terminal"> <mxCell> that represents the source or target terminal. </param>
        /// <param name="source"> Boolean indicating if the source or target terminal is to be
        /// disconnected. </param>
        /// <returns> Returns true if the given edge can be disconnected from the given
        /// terminal. </returns>
        public virtual bool isCellDisconnectable(object cell, object terminal, bool source)
        {
            return CellsDisconnectable && !isCellLocked(cell);
        }

        /// <summary>
        /// Returns cellsDisconnectable.
        /// </summary>
        public virtual bool CellsDisconnectable
        {
            get
            {
                return cellsDisconnectable;
            }
            set
            {
                bool oldValue = cellsDisconnectable;
                cellsDisconnectable = value;

                changeSupport.firePropertyChange("cellsDisconnectable", oldValue, cellsDisconnectable);
            }
        }


        /// <summary>
        /// Returns true if the overflow portion of labels should be hidden. If this
        /// returns true then vertex labels will be clipped to the size of the vertices.
        /// This implementation returns true if <mxConstants.STYLE_OVERFLOW> in the
        /// style of the given cell is "hidden".
        /// </summary>
        /// <param name="cell"> Cell whose label should be clipped. </param>
        /// <returns> Returns true if the cell label should be clipped. </returns>
        public virtual bool isLabelClipped(object cell)
        {
            if (!LabelsClipped)
            {
                mxCellState state = view.getState(cell);
                IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cell);

                return (style != null) ? mxUtils.getString(style, mxConstants.STYLE_OVERFLOW, "").Equals("hidden") : false;
            }

            return LabelsClipped;
        }

        /// <summary>
        /// Returns labelsClipped.
        /// </summary>
        public virtual bool LabelsClipped
        {
            get
            {
                return labelsClipped;
            }
            set
            {
                bool oldValue = labelsClipped;
                labelsClipped = value;

                changeSupport.firePropertyChange("labelsClipped", oldValue, labelsClipped);
            }
        }


        /// <summary>
        /// Returns true if the given edges's label is moveable. This returns
        /// <movable> for all given cells if <isLocked> does not return true
        /// for the given cell.
        /// </summary>
        /// <param name="cell"> <mxCell> whose label should be moved. </param>
        /// <returns> Returns true if the label of the given cell is movable. </returns>
        public virtual bool isLabelMovable(object cell)
        {
            return !isCellLocked(cell) && ((model.isEdge(cell) && EdgeLabelsMovable) || (model.isVertex(cell) && VertexLabelsMovable));
        }

        /// <summary>
        /// Returns vertexLabelsMovable.
        /// </summary>
        public virtual bool VertexLabelsMovable
        {
            get
            {
                return vertexLabelsMovable;
            }
            set
            {
                bool oldValue = vertexLabelsMovable;
                vertexLabelsMovable = value;

                changeSupport.firePropertyChange("vertexLabelsMovable", oldValue, vertexLabelsMovable);
            }
        }


        /// <summary>
        /// Returns edgeLabelsMovable.
        /// </summary>
        public virtual bool EdgeLabelsMovable
        {
            get
            {
                return edgeLabelsMovable;
            }
            set
            {
                bool oldValue = edgeLabelsMovable;
                edgeLabelsMovable = value;

                changeSupport.firePropertyChange("edgeLabelsMovable", oldValue, edgeLabelsMovable);
            }
        }


        //
        // Graph control options
        //

        /// <summary>
        /// Returns true if the graph is <enabled>.
        /// </summary>
        /// <returns> Returns true if the graph is enabled. </returns>
        public virtual bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                bool oldValue = enabled;
                enabled = value;

                changeSupport.firePropertyChange("enabled", oldValue, enabled);
            }
        }


        /// <summary>
        /// Returns true if the graph allows drop into other cells.
        /// </summary>
        public virtual bool DropEnabled
        {
            get
            {
                return dropEnabled;
            }
            set
            {
                bool oldValue = dropEnabled;
                dropEnabled = value;

                changeSupport.firePropertyChange("dropEnabled", oldValue, dropEnabled);
            }
        }


        /// <summary>
        /// Affects the return values of isValidDropTarget to allow for edges as
        /// drop targets. The splitEdge method is called in mxGraphHandler if
        /// mxGraphComponent.isSplitEvent returns true for a given configuration.
        /// </summary>
        public virtual bool SplitEnabled
        {
            get
            {
                return splitEnabled;
            }
            set
            {
                splitEnabled = value;
            }
        }


        /// <summary>
        /// Returns multigraph.
        /// </summary>
        public virtual bool Multigraph
        {
            get
            {
                return multigraph;
            }
            set
            {
                bool oldValue = multigraph;
                multigraph = value;

                changeSupport.firePropertyChange("multigraph", oldValue, multigraph);
            }
        }


        /// <summary>
        /// Returns swimlaneNesting.
        /// </summary>
        public virtual bool SwimlaneNesting
        {
            get
            {
                return swimlaneNesting;
            }
            set
            {
                bool oldValue = swimlaneNesting;
                swimlaneNesting = value;

                changeSupport.firePropertyChange("swimlaneNesting", oldValue, swimlaneNesting);
            }
        }


        /// <summary>
        /// Returns allowDanglingEdges
        /// </summary>
        public virtual bool AllowDanglingEdges
        {
            get
            {
                return allowDanglingEdges;
            }
            set
            {
                bool oldValue = allowDanglingEdges;
                allowDanglingEdges = value;

                changeSupport.firePropertyChange("allowDanglingEdges", oldValue, allowDanglingEdges);
            }
        }


        /// <summary>
        /// Returns cloneInvalidEdges.
        /// </summary>
        public virtual bool CloneInvalidEdges
        {
            get
            {
                return cloneInvalidEdges;
            }
            set
            {
                bool oldValue = cloneInvalidEdges;
                cloneInvalidEdges = value;

                changeSupport.firePropertyChange("cloneInvalidEdges", oldValue, cloneInvalidEdges);
            }
        }


        /// <summary>
        /// Returns disconnectOnMove
        /// </summary>
        public virtual bool DisconnectOnMove
        {
            get
            {
                return disconnectOnMove;
            }
            set
            {
                bool oldValue = disconnectOnMove;
                disconnectOnMove = value;

                changeSupport.firePropertyChange("disconnectOnMove", oldValue, disconnectOnMove);

            }
        }


        /// <summary>
        /// Returns allowLoops.
        /// </summary>
        public virtual bool AllowLoops
        {
            get
            {
                return allowLoops;
            }
            set
            {
                bool oldValue = allowLoops;
                allowLoops = value;

                changeSupport.firePropertyChange("allowLoops", oldValue, allowLoops);
            }
        }


        /// <summary>
        /// Returns connectableEdges.
        /// </summary>
        public virtual bool ConnectableEdges
        {
            get
            {
                return connectableEdges;
            }
            set
            {
                bool oldValue = connectableEdges;
                connectableEdges = value;

                changeSupport.firePropertyChange("connectableEdges", oldValue, connectableEdges);

            }
        }


        /// <summary>
        /// Returns resetEdgesOnMove.
        /// </summary>
        public virtual bool ResetEdgesOnMove
        {
            get
            {
                return resetEdgesOnMove;
            }
            set
            {
                bool oldValue = resetEdgesOnMove;
                resetEdgesOnMove = value;

                changeSupport.firePropertyChange("resetEdgesOnMove", oldValue, resetEdgesOnMove);
            }
        }


        /// <summary>
        /// Returns resetViewOnRootChange.
        /// </summary>
        public virtual bool ResetViewOnRootChange
        {
            get
            {
                return resetViewOnRootChange;
            }
            set
            {
                bool oldValue = resetViewOnRootChange;
                resetViewOnRootChange = value;

                changeSupport.firePropertyChange("resetViewOnRootChange", oldValue, resetViewOnRootChange);
            }
        }


        /// <summary>
        /// Returns resetEdgesOnResize.
        /// </summary>
        public virtual bool ResetEdgesOnResize
        {
            get
            {
                return resetEdgesOnResize;
            }
            set
            {
                bool oldValue = resetEdgesOnResize;
                resetEdgesOnResize = value;

                changeSupport.firePropertyChange("resetEdgesOnResize", oldValue, resetEdgesOnResize);
            }
        }


        /// <summary>
        /// Returns resetEdgesOnConnect.
        /// </summary>
        public virtual bool ResetEdgesOnConnect
        {
            get
            {
                return resetEdgesOnConnect;
            }
            set
            {
                bool oldValue = resetEdgesOnConnect;
                resetEdgesOnConnect = value;

                changeSupport.firePropertyChange("resetEdgesOnConnect", oldValue, resetEdgesOnResize);
            }
        }


        /// <summary>
        /// Returns true if the size of the given cell should automatically be
        /// updated after a change of the label. This implementation returns
        /// autoSize for all given cells.
        /// </summary>
        /// <param name="cell"> Cell that should be resized. </param>
        /// <returns> Returns true if the size of the given cell should be updated. </returns>
        public virtual bool isAutoSizeCell(object cell)
        {
            return AutoSizeCells;
        }

        /// <summary>
        /// Returns true if the size of the given cell should automatically be
        /// updated after a change of the label. This implementation returns
        /// autoSize for all given cells.
        /// </summary>
        public virtual bool AutoSizeCells
        {
            get
            {
                return autoSizeCells;
            }
            set
            {
                bool oldValue = autoSizeCells;
                autoSizeCells = value;

                changeSupport.firePropertyChange("autoSizeCells", oldValue, autoSizeCells);
            }
        }


        /// <summary>
        /// Returns true if the parent of the given cell should be extended if the
        /// child has been resized so that it overlaps the parent. This
        /// implementation returns ExtendParents if cell is not an edge.
        /// </summary>
        /// <param name="cell"> Cell that has been resized. </param>
        public virtual bool isExtendParent(object cell)
        {
            return !Model.isEdge(cell) && ExtendParents;
        }

        /// <summary>
        /// Returns extendParents.
        /// </summary>
        public virtual bool ExtendParents
        {
            get
            {
                return extendParents;
            }
            set
            {
                bool oldValue = extendParents;
                extendParents = value;

                changeSupport.firePropertyChange("extendParents", oldValue, extendParents);
            }
        }


        /// <summary>
        /// Returns extendParentsOnAdd.
        /// </summary>
        public virtual bool ExtendParentsOnAdd
        {
            get
            {
                return extendParentsOnAdd;
            }
            set
            {
                bool oldValue = extendParentsOnAdd;
                extendParentsOnAdd = value;

                changeSupport.firePropertyChange("extendParentsOnAdd", oldValue, extendParentsOnAdd);
            }
        }


        /// <summary>
        /// Returns true if the given cell should be kept inside the bounds of its
        /// parent according to the rules defined by getOverlap and
        /// isAllowOverlapParent. This implementation returns
        /// isConstrainChildren() for all given cells.
        /// </summary>
        public virtual bool isConstrainChild(object cell)
        {
            return ConstrainChildren;
        }

        /// <summary>
        /// Returns constrainChildren.
        /// </summary>
        /// <returns> the keepInsideParentOnMove </returns>
        public virtual bool ConstrainChildren
        {
            get
            {
                return constrainChildren;
            }
            set
            {
                bool oldValue = constrainChildren;
                constrainChildren = value;

                changeSupport.firePropertyChange("constrainChildren", oldValue, constrainChildren);
            }
        }


        /// <summary>
        /// Returns autoOrigin.
        /// </summary>
        public virtual bool AutoOrigin
        {
            get
            {
                return autoOrigin;
            }
            set
            {
                bool oldValue = autoOrigin;
                autoOrigin = value;

                changeSupport.firePropertyChange("autoOrigin", oldValue, autoOrigin);
            }
        }


        /// <summary>
        /// Returns origin.
        /// </summary>
        public virtual mxPoint Origin
        {
            get
            {
                return origin;
            }
            set
            {
                mxPoint oldValue = origin;
                origin = value;

                changeSupport.firePropertyChange("origin", oldValue, origin);
            }
        }


        /// <returns> Returns changesRepaintThreshold. </returns>
        public virtual int ChangesRepaintThreshold
        {
            get
            {
                return changesRepaintThreshold;
            }
            set
            {
                int oldValue = changesRepaintThreshold;
                changesRepaintThreshold = value;

                changeSupport.firePropertyChange("changesRepaintThreshold", oldValue, changesRepaintThreshold);
            }
        }


        /// <summary>
        /// Returns isAllowNegativeCoordinates.
        /// </summary>
        /// <returns> the allowNegativeCoordinates </returns>
        public virtual bool AllowNegativeCoordinates
        {
            get
            {
                return allowNegativeCoordinates;
            }
            set
            {
                bool oldValue = allowNegativeCoordinates;
                allowNegativeCoordinates = value;

                changeSupport.firePropertyChange("allowNegativeCoordinates", oldValue, allowNegativeCoordinates);
            }
        }


        /// <summary>
        /// Returns collapseToPreferredSize.
        /// </summary>
        /// <returns> the collapseToPreferredSize </returns>
        public virtual bool CollapseToPreferredSize
        {
            get
            {
                return collapseToPreferredSize;
            }
            set
            {
                bool oldValue = collapseToPreferredSize;
                collapseToPreferredSize = value;

                changeSupport.firePropertyChange("collapseToPreferredSize", oldValue, collapseToPreferredSize);
            }
        }


        /// <returns> Returns true if edges are rendered in the foreground. </returns>
        public virtual bool KeepEdgesInForeground
        {
            get
            {
                return keepEdgesInForeground;
            }
            set
            {
                bool oldValue = keepEdgesInForeground;
                keepEdgesInForeground = value;

                changeSupport.firePropertyChange("keepEdgesInForeground", oldValue, keepEdgesInForeground);
            }
        }


        /// <returns> Returns true if edges are rendered in the background. </returns>
        public virtual bool KeepEdgesInBackground
        {
            get
            {
                return keepEdgesInBackground;
            }
            set
            {
                bool oldValue = keepEdgesInBackground;
                keepEdgesInBackground = value;

                changeSupport.firePropertyChange("keepEdgesInBackground", oldValue, keepEdgesInBackground);
            }
        }


        /// <summary>
        /// Returns true if the given cell is a valid source for new connections.
        /// This implementation returns true for all non-null values and is
        /// called by is called by <isValidConnection>.
        /// </summary>
        /// <param name="cell"> Object that represents a possible source or null. </param>
        /// <returns> Returns true if the given cell is a valid source terminal. </returns>
        public virtual bool isValidSource(object cell)
        {
            return (cell == null && allowDanglingEdges) || (cell != null && (!model.isEdge(cell) || ConnectableEdges) && isCellConnectable(cell));
        }

        /// <summary>
        /// Returns isValidSource for the given cell. This is called by
        /// isValidConnection.
        /// </summary>
        /// <param name="cell"> Object that represents a possible target or null. </param>
        /// <returns> Returns true if the given cell is a valid target. </returns>
        public virtual bool isValidTarget(object cell)
        {
            return isValidSource(cell);
        }

        /// <summary>
        /// Returns true if the given target cell is a valid target for source.
        /// This is a boolean implementation for not allowing connections between
        /// certain pairs of vertices and is called by <getEdgeValidationError>.
        /// This implementation returns true if <isValidSource> returns true for
        /// the source and <isValidTarget> returns true for the target.
        /// </summary>
        /// <param name="source"> Object that represents the source cell. </param>
        /// <param name="target"> Object that represents the target cell. </param>
        /// <returns> Returns true if the the connection between the given terminals
        /// is valid. </returns>
        public virtual bool isValidConnection(object source, object target)
        {
            return isValidSource(source) && isValidTarget(target) && (AllowLoops || source != target);
        }

        /// <summary>
        /// Returns the minimum size of the diagram.
        /// </summary>
        /// <returns> Returns the minimum container size. </returns>
        public virtual mxRectangle MinimumGraphSize
        {
            get
            {
                return minimumGraphSize;
            }
            set
            {
                mxRectangle oldValue = minimumGraphSize;
                minimumGraphSize = value;

                changeSupport.firePropertyChange("minimumGraphSize", oldValue, value);
            }
        }


        /// <summary>
        /// Returns a decimal number representing the amount of the width and height
        /// of the given cell that is allowed to overlap its parent. A value of 0
        /// means all children must stay inside the parent, 1 means the child is
        /// allowed to be placed outside of the parent such that it touches one of
        /// the parents sides. If <isAllowOverlapParent> returns false for the given
        /// cell, then this method returns 0.
        /// </summary>
        /// <param name="cell"> </param>
        /// <returns> Returns the overlapping value for the given cell inside its
        /// parent. </returns>
        public virtual double getOverlap(object cell)
        {
            return (isAllowOverlapParent(cell)) ? DefaultOverlap : 0;
        }

        /// <summary>
        /// Gets defaultOverlap.
        /// </summary>
        public virtual double DefaultOverlap
        {
            get
            {
                return defaultOverlap;
            }
            set
            {
                double oldValue = defaultOverlap;
                defaultOverlap = value;

                changeSupport.firePropertyChange("defaultOverlap", oldValue, value);
            }
        }


        /// <summary>
        /// Returns true if the given cell is allowed to be placed outside of the
        /// parents area.
        /// </summary>
        /// <param name="cell"> </param>
        /// <returns> Returns true if the given cell may overlap its parent. </returns>
        public virtual bool isAllowOverlapParent(object cell)
        {
            return false;
        }

        /// <summary>
        /// Returns the cells which are movable in the given array of cells.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public Object[] getFoldableCells(Object[] cells, final boolean collapse)
        public virtual object[] getFoldableCells(object[] cells, bool collapse)
        {
            return mxGraphModel.filterCells(cells, new FilterAnonymousInnerClass4(this, collapse));
        }

        private class FilterAnonymousInnerClass4 : mxGraphModel.Filter
        {
            private readonly mxGraph outerInstance;

            private bool collapse;

            public FilterAnonymousInnerClass4(mxGraph outerInstance, bool collapse)
            {
                this.outerInstance = outerInstance;
                this.collapse = collapse;
            }

            public virtual bool filter(object cell)
            {
                return outerInstance.isCellFoldable(cell, collapse);
            }
        }

        /// <summary>
        /// Returns true if the given cell is expandable. This implementation
        /// returns true if the cell has at least one child.
        /// </summary>
        /// <param name="cell"> <mxCell> whose expandable state should be returned. </param>
        /// <returns> Returns true if the given cell is expandable. </returns>
        public virtual bool isCellFoldable(object cell, bool collapse)
        {
            return model.getChildCount(cell) > 0;
        }

        /// <summary>
        /// Returns true if the grid is enabled.
        /// </summary>
        /// <returns> Returns the enabled state of the grid. </returns>
        public virtual bool GridEnabled
        {
            get
            {
                return gridEnabled;
            }
            set
            {
                bool oldValue = gridEnabled;
                gridEnabled = value;

                changeSupport.firePropertyChange("gridEnabled", oldValue, gridSize);
            }
        }


        /// <summary>
        /// Returns the grid size.
        /// </summary>
        /// <returns> Returns the grid size </returns>
        public virtual int GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                int oldValue = gridSize;
                gridSize = value;

                changeSupport.firePropertyChange("gridSize", oldValue, gridSize);
            }
        }


        /// <summary>
        /// Returns alternateEdgeStyle.
        /// </summary>
        public virtual string AlternateEdgeStyle
        {
            get
            {
                return alternateEdgeStyle;
            }
            set
            {
                string oldValue = alternateEdgeStyle;
                alternateEdgeStyle = value;

                changeSupport.firePropertyChange("alternateEdgeStyle", oldValue, alternateEdgeStyle);
            }
        }


        /// <summary>
        /// Returns true if the given cell is a valid drop target for the specified
        /// cells. This returns true if the cell is a swimlane, has children and is
        /// not collapsed, or if splitEnabled is true and isSplitTarget returns
        /// true for the given arguments
        /// </summary>
        /// <param name="cell"> Object that represents the possible drop target. </param>
        /// <param name="cells"> Objects that are going to be dropped. </param>
        /// <returns> Returns true if the cell is a valid drop target for the given
        /// cells. </returns>
        public virtual bool isValidDropTarget(object cell, object[] cells)
        {
            return cell != null && ((SplitEnabled && isSplitTarget(cell, cells)) || (!model.isEdge(cell) && (isSwimlane(cell) || (model.getChildCount(cell) > 0 && !isCellCollapsed(cell)))));
        }

        /// <summary>
        /// Returns true if split is enabled and the given edge may be splitted into
        /// two edges with the given cell as a new terminal between the two.
        /// </summary>
        /// <param name="target"> Object that represents the edge to be splitted. </param>
        /// <param name="cells"> Array of cells to add into the given edge. </param>
        /// <returns> Returns true if the given edge may be splitted by the given
        /// cell. </returns>
        public virtual bool isSplitTarget(object target, object[] cells)
        {
            if (target != null && cells != null && cells.Length == 1)
            {
                object src = model.getTerminal(target, true);
                object trg = model.getTerminal(target, false);

                return (model.isEdge(target) && isCellConnectable(cells[0]) && string.ReferenceEquals(getEdgeValidationError(target, model.getTerminal(target, true), cells[0]), null) && !model.isAncestor(cells[0], src) && !model.isAncestor(cells[0], trg));
            }

            return false;
        }

        /// <summary>
        /// Returns the given cell if it is a drop target for the given cells or the
        /// nearest ancestor that may be used as a drop target for the given cells.
        /// If the given array contains a swimlane and swimlaneNesting is false
        /// then this always returns null. If no cell is given, then the bottommost
        /// swimlane at the location of the given event is returned.
        /// 
        /// This function should only be used if isDropEnabled returns true.
        /// </summary>
        public virtual object getDropTarget(object[] cells, Point pt, object cell)
        {
            if (!SwimlaneNesting)
            {
                for (int i = 0; i < cells.Length; i++)
                {
                    if (isSwimlane(cells[i]))
                    {
                        return null;
                    }
                }
            }

            // FIXME the else below does nothing if swimlane is null
            object swimlane = null; //getSwimlaneAt(pt.x, pt.y);

            if (cell == null)
            {
                cell = swimlane;
            }
            else if (swimlane != null)
            {
                // Checks if the cell is an ancestor of the swimlane
                // under the mouse and uses the swimlane in that case
                object tmp = model.getParent(swimlane);

                while (tmp != null && isSwimlane(tmp) && tmp != cell)
                {
                    tmp = model.getParent(tmp);
                }

                if (tmp == cell)
                {
                    cell = swimlane;
                }
            }

            while (cell != null && !isValidDropTarget(cell, cells) && model.getParent(cell) != model.Root)
            {
                cell = model.getParent(cell);
            }

            return (model.getParent(cell) != model.Root && !mxUtils.contains(cells, cell)) ? cell : null;
        };

        //
        // Cell retrieval
        //

        /// <summary>
        /// Returns the first child of the root in the model, that is, the first or
        /// default layer of the diagram. 
        /// </summary>
        /// <returns> Returns the default parent for new cells. </returns>
        public virtual object DefaultParent
        {
            get
            {
                object parent = defaultParent;

                if (parent == null)
                {
                    parent = view.CurrentRoot;

                    if (parent == null)
                    {
                        object root = model.Root;
                        parent = model.getChildAt(root, 0);
                    }
                }

                return parent;
            }
            set
            {
                defaultParent = value;
            }
        }


        /// <summary>
        /// Returns the visible child vertices of the given parent.
        /// </summary>
        /// <param name="parent"> Cell whose children should be returned. </param>
        public virtual object[] getChildVertices(object parent)
        {
            return getChildCells(parent, true, false);
        }

        /// <summary>
        /// Returns the visible child edges of the given parent.
        /// </summary>
        /// <param name="parent"> Cell whose children should be returned. </param>
        public virtual object[] getChildEdges(object parent)
        {
            return getChildCells(parent, false, true);
        }

        /// <summary>
        /// Returns the visible children of the given parent.
        /// </summary>
        /// <param name="parent"> Cell whose children should be returned. </param>
        public virtual object[] getChildCells(object parent)
        {
            return getChildCells(parent, false, false);
        }

        /// <summary>
        /// Returns the visible child vertices or edges in the given parent. If
        /// vertices and edges is false, then all children are returned.
        /// </summary>
        /// <param name="parent"> Cell whose children should be returned. </param>
        /// <param name="vertices"> Specifies if child vertices should be returned. </param>
        /// <param name="edges"> Specifies if child edges should be returned. </param>
        /// <returns> Returns the child vertices and edges. </returns>
        public virtual object[] getChildCells(object parent, bool vertices, bool edges)
        {
            object[] cells = mxGraphModel.getChildCells(model, parent, vertices, edges);
            IList<object> result = new List<object>(cells.Length);

            // Filters out the non-visible child cells
            for (int i = 0; i < cells.Length; i++)
            {
                if (isCellVisible(cells[i]))
                {
                    result.Add(cells[i]);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns all visible edges connected to the given cell without loops.
        /// </summary>
        /// <param name="cell"> Cell whose connections should be returned. </param>
        /// <returns> Returns the connected edges for the given cell. </returns>
        public virtual object[] getConnections(object cell)
        {
            return getConnections(cell, null);
        }

        /// <summary>
        /// Returns all visible edges connected to the given cell without loops.
        /// If the optional parent argument is specified, then only child
        /// edges of the given parent are returned.
        /// </summary>
        /// <param name="cell"> Cell whose connections should be returned. </param>
        /// <param name="parent"> Optional parent of the opposite end for a connection
        /// to be returned. </param>
        /// <returns> Returns the connected edges for the given cell. </returns>
        public virtual object[] getConnections(object cell, object parent)
        {
            return getEdges(cell, parent, true, true, false);
        }

        /// <summary>
        /// Returns all incoming visible edges connected to the given cell without
        /// loops.
        /// </summary>
        /// <param name="cell"> Cell whose incoming edges should be returned. </param>
        /// <returns> Returns the incoming edges of the given cell. </returns>
        public virtual object[] getIncomingEdges(object cell)
        {
            return getIncomingEdges(cell, null);
        }

        /// <summary>
        /// Returns the visible incoming edges for the given cell. If the optional
        /// parent argument is specified, then only child edges of the given parent
        /// are returned.
        /// </summary>
        /// <param name="cell"> Cell whose incoming edges should be returned. </param>
        /// <param name="parent"> Optional parent of the opposite end for an edge
        /// to be returned. </param>
        /// <returns> Returns the incoming edges of the given cell. </returns>
        public virtual object[] getIncomingEdges(object cell, object parent)
        {
            return getEdges(cell, parent, true, false, false);
        }

        /// <summary>
        /// Returns all outgoing visible edges connected to the given cell without
        /// loops.
        /// </summary>
        /// <param name="cell"> Cell whose outgoing edges should be returned. </param>
        /// <returns> Returns the outgoing edges of the given cell. </returns>
        public virtual object[] getOutgoingEdges(object cell)
        {
            return getOutgoingEdges(cell, null);
        }

        /// <summary>
        /// Returns the visible outgoing edges for the given cell. If the optional
        /// parent argument is specified, then only child edges of the given parent
        /// are returned.
        /// </summary>
        /// <param name="cell"> Cell whose outgoing edges should be returned. </param>
        /// <param name="parent"> Optional parent of the opposite end for an edge
        /// to be returned. </param>
        /// <returns> Returns the outgoing edges of the given cell. </returns>
        public virtual object[] getOutgoingEdges(object cell, object parent)
        {
            return getEdges(cell, parent, false, true, false);
        }

        /// <summary>
        /// Returns all visible edges connected to the given cell including loops.
        /// </summary>
        /// <param name="cell"> Cell whose edges should be returned. </param>
        /// <returns> Returns the edges of the given cell. </returns>
        public virtual object[] getEdges(object cell)
        {
            return getEdges(cell, null);
        }

        /// <summary>
        /// Returns all visible edges connected to the given cell including loops.
        /// </summary>
        /// <param name="cell"> Cell whose edges should be returned. </param>
        /// <param name="parent"> Optional parent of the opposite end for an edge
        /// to be returned. </param>
        /// <returns> Returns the edges of the given cell. </returns>
        public virtual object[] getEdges(object cell, object parent)
        {
            return getEdges(cell, parent, true, true, true);
        }

        /// <summary>
        /// Returns the incoming and/or outgoing edges for the given cell.
        /// If the optional parent argument is specified, then only edges are returned
        /// where the opposite is in the given parent cell.
        /// </summary>
        /// <param name="cell"> Cell whose edges should be returned. </param>
        /// <param name="parent"> Optional parent of the opposite end for an edge to be
        /// returned. </param>
        /// <param name="incoming"> Specifies if incoming edges should be included in the
        /// result. </param>
        /// <param name="outgoing"> Specifies if outgoing edges should be included in the
        /// result. </param>
        /// <param name="includeLoops"> Specifies if loops should be included in the result. </param>
        /// <returns> Returns the edges connected to the given cell. </returns>
        public virtual object[] getEdges(object cell, object parent, bool incoming, bool outgoing, bool includeLoops)
        {
            bool isCollapsed = isCellCollapsed(cell);
            IList<object> edges = new List<object>();
            int childCount = model.getChildCount(cell);

            for (int i = 0; i < childCount; i++)
            {
                object child = model.getChildAt(cell, i);

                if (isCollapsed || !isCellVisible(child))
                {
                    ((List<object>)edges).AddRange(Arrays.asList(mxGraphModel.getEdges(model, child, incoming, outgoing, includeLoops)));
                }
            }

            ((List<object>)edges).AddRange(Arrays.asList(mxGraphModel.getEdges(model, cell, incoming, outgoing, includeLoops)));
            IList<object> result = new List<object>(edges.Count);
            IEnumerator<object> it = edges.GetEnumerator();

            while (it.MoveNext())
            {
                object edge = it.Current;
                object source = view.getVisibleTerminal(edge, true);
                object target = view.getVisibleTerminal(edge, false);

                if (includeLoops || ((source != target) && ((incoming && target == cell && (parent == null || model.getParent(source) == parent)) || (outgoing && source == cell && (parent == null || model.getParent(target) == parent)))))
                {
                    result.Add(edge);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns all distinct visible opposite cells of the terminal on the given
        /// edges.
        /// </summary>
        /// <param name="edges"> </param>
        /// <param name="terminal"> </param>
        /// <returns> Returns the terminals at the opposite ends of the given edges. </returns>
        public virtual object[] getOpposites(object[] edges, object terminal)
        {
            return getOpposites(edges, terminal, true, true);
        }

        /// <summary>
        /// Returns all distincts visible opposite cells for the specified terminal
        /// on the given edges.
        /// </summary>
        /// <param name="edges"> Edges whose opposite terminals should be returned. </param>
        /// <param name="terminal"> Terminal that specifies the end whose opposite should be
        /// returned. </param>
        /// <param name="sources"> Specifies if source terminals should be included in the
        /// result. </param>
        /// <param name="targets"> Specifies if targer terminals should be included in the
        /// result. </param>
        /// <returns> Returns the cells at the oppsite ends of the given edges. </returns>
        public virtual object[] getOpposites(object[] edges, object terminal, bool sources, bool targets)
        {
            ICollection<object> terminals = new LinkedHashSet<object>();

            if (edges != null)
            {
                for (int i = 0; i < edges.Length; i++)
                {
                    object source = view.getVisibleTerminal(edges[i], true);
                    object target = view.getVisibleTerminal(edges[i], false);

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
        /// Returns the edges between the given source and target. This takes into
        /// account collapsed and invisible cells and returns the connected edges
        /// as displayed on the screen.
        /// </summary>
        /// <param name="source"> </param>
        /// <param name="target"> </param>
        /// <returns> Returns all edges between the given terminals. </returns>
        public virtual object[] getEdgesBetween(object source, object target)
        {
            return getEdgesBetween(source, target, false);
        }

        /// <summary>
        /// Returns the edges between the given source and target. This takes into
        /// account collapsed and invisible cells and returns the connected edges
        /// as displayed on the screen.
        /// </summary>
        /// <param name="source"> </param>
        /// <param name="target"> </param>
        /// <param name="directed"> </param>
        /// <returns> Returns all edges between the given terminals. </returns>
        public virtual object[] getEdgesBetween(object source, object target, bool directed)
        {
            object[] edges = getEdges(source);
            IList<object> result = new List<object>(edges.Length);

            // Checks if the edge is connected to the correct
            // cell and adds any match to the result
            for (int i = 0; i < edges.Length; i++)
            {
                object src = view.getVisibleTerminal(edges[i], true);
                object trg = view.getVisibleTerminal(edges[i], false);

                if ((src == source && trg == target) || (!directed && src == target && trg == source))
                {
                    result.Add(edges[i]);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns the children of the given parent that are contained in the
        /// halfpane from the given point (x0, y0) rightwards and downwards
        /// depending on rightHalfpane and bottomHalfpane.
        /// </summary>
        /// <param name="x0"> X-coordinate of the origin. </param>
        /// <param name="y0"> Y-coordinate of the origin. </param>
        /// <param name="parent"> <mxCell> whose children should be checked. </param>
        /// <param name="rightHalfpane"> Boolean indicating if the cells in the right halfpane
        /// from the origin should be returned. </param>
        /// <param name="bottomHalfpane"> Boolean indicating if the cells in the bottom halfpane
        /// from the origin should be returned. </param>
        /// <returns> Returns the cells beyond the given halfpane. </returns>
        public virtual object[] getCellsBeyond(double x0, double y0, object parent, bool rightHalfpane, bool bottomHalfpane)
        {
            if (parent == null)
            {
                parent = DefaultParent;
            }

            int childCount = model.getChildCount(parent);
            IList<object> result = new List<object>(childCount);

            if (rightHalfpane || bottomHalfpane)
            {

                if (parent != null)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        object child = model.getChildAt(parent, i);
                        mxCellState state = view.getState(child);

                        if (isCellVisible(child) && state != null)
                        {
                            if ((!rightHalfpane || state.X >= x0) && (!bottomHalfpane || state.Y >= y0))
                            {
                                result.Add(child);
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns all visible children in the given parent which do not have
        /// incoming edges. If the result is empty then the with the greatest
        /// difference between incoming and outgoing edges is returned. This
        /// takes into account edges that are being promoted to the given
        /// root due to invisible children or collapsed cells.
        /// </summary>
        /// <param name="parent"> Cell whose children should be checked. </param>
        /// <returns> List of tree roots in parent. </returns>
        public virtual IList<object> findTreeRoots(object parent)
        {
            return findTreeRoots(parent, false);
        }

        /// <summary>
        /// Returns all visible children in the given parent which do not have
        /// incoming edges. If the result is empty then the children with the
        /// maximum difference between incoming and outgoing edges are returned.
        /// This takes into account edges that are being promoted to the given
        /// root due to invisible children or collapsed cells.
        /// </summary>
        /// <param name="parent"> Cell whose children should be checked. </param>
        /// <param name="isolate"> Specifies if edges should be ignored if the opposite
        /// end is not a child of the given parent cell. </param>
        /// <returns> List of tree roots in parent. </returns>
        public virtual IList<object> findTreeRoots(object parent, bool isolate)
        {
            return findTreeRoots(parent, isolate, false);
        }

        /// <summary>
        /// Returns all visible children in the given parent which do not have
        /// incoming edges. If the result is empty then the children with the
        /// maximum difference between incoming and outgoing edges are returned.
        /// This takes into account edges that are being promoted to the given
        /// root due to invisible children or collapsed cells.
        /// </summary>
        /// <param name="parent"> Cell whose children should be checked. </param>
        /// <param name="isolate"> Specifies if edges should be ignored if the opposite
        /// end is not a child of the given parent cell. </param>
        /// <param name="invert"> Specifies if outgoing or incoming edges should be counted
        /// for a tree root. If false then outgoing edges will be counted. </param>
        /// <returns> List of tree roots in parent. </returns>
        public virtual IList<object> findTreeRoots(object parent, bool isolate, bool invert)
        {
            IList<object> roots = new List<object>();

            if (parent != null)
            {
                int childCount = model.getChildCount(parent);
                object best = null;
                int maxDiff = 0;

                for (int i = 0; i < childCount; i++)
                {
                    object cell = model.getChildAt(parent, i);

                    if (model.isVertex(cell) && isCellVisible(cell))
                    {
                        object[] conns = getConnections(cell, (isolate) ? parent : null);
                        int fanOut = 0;
                        int fanIn = 0;

                        for (int j = 0; j < conns.Length; j++)
                        {
                            object src = view.getVisibleTerminal(conns[j], true);

                            if (src == cell)
                            {
                                fanOut++;
                            }
                            else
                            {
                                fanIn++;
                            }
                        }

                        if ((invert && fanOut == 0 && fanIn > 0) || (!invert && fanIn == 0 && fanOut > 0))
                        {
                            roots.Add(cell);
                        }

                        int diff = (invert) ? fanIn - fanOut : fanOut - fanIn;

                        if (diff > maxDiff)
                        {
                            maxDiff = diff;
                            best = cell;
                        }
                    }
                }

                if (roots.Count == 0 && best != null)
                {
                    roots.Add(best);
                }
            }

            return roots;
        }

        /// <summary>
        /// Traverses the tree starting at the given vertex. Here is how to use this
        /// method for a given vertex (root) which is typically the root of a tree:
        /// <code>
        /// graph.traverse(root, true, new mxICellVisitor()
        /// {
        ///   public boolean visit(Object vertex, Object edge)
        ///   {
        ///     System.out.println("edge="+graph.convertValueToString(edge)+
        ///       " vertex="+graph.convertValueToString(vertex));
        /// 
        ///     return true;
        ///   }
        /// });
        /// </code>
        /// </summary>
        /// <param name="vertex"> </param>
        /// <param name="directed"> </param>
        /// <param name="visitor"> </param>
        public virtual void traverse(object vertex, bool directed, mxICellVisitor visitor)
        {
            traverse(vertex, directed, visitor, null, null);
        }

        /// <summary>
        /// Traverses the (directed) graph invoking the given function for each
        /// visited vertex and edge. The function is invoked with the current vertex
        /// and the incoming edge as a parameter. This implementation makes sure
        /// each vertex is only visited once. The function may return false if the
        /// traversal should stop at the given vertex.
        /// </summary>
        /// <param name="vertex"> <mxCell> that represents the vertex where the traversal starts. </param>
        /// <param name="directed"> Optional boolean indicating if edges should only be traversed
        /// from source to target. Default is true. </param>
        /// <param name="visitor"> Visitor that takes the current vertex and the incoming edge.
        /// The traversal stops if the function returns false. </param>
        /// <param name="edge"> Optional <mxCell> that represents the incoming edge. This is
        /// null for the first step of the traversal. </param>
        /// <param name="visited"> Optional array of cell paths for the visited cells. </param>
        public virtual void traverse(object vertex, bool directed, mxICellVisitor visitor, object edge, ISet<object> visited)
        {
            if (vertex != null && visitor != null)
            {
                if (visited == null)
                {
                    visited = new HashSet<object>();
                }

                if (!visited.Contains(vertex))
                {
                    visited.Add(vertex);

                    if (visitor.visit(vertex, edge))
                    {
                        int edgeCount = model.getEdgeCount(vertex);

                        if (edgeCount > 0)
                        {
                            for (int i = 0; i < edgeCount; i++)
                            {
                                object e = model.getEdgeAt(vertex, i);
                                bool isSource = model.getTerminal(e, true) == vertex;

                                if (!directed || isSource)
                                {
                                    object next = model.getTerminal(e, !isSource);
                                    traverse(next, directed, visitor, e, visited);
                                }
                            }
                        }
                    }
                }
            }
        }

        //
        // Selection
        //

        /// 
        public virtual mxGraphSelectionModel SelectionModel
        {
            get
            {
                return selectionModel;
            }
        }

        /// 
        public virtual int SelectionCount
        {
            get
            {
                return selectionModel.size();
            }
        }

        /// 
        /// <param name="cell"> </param>
        /// <returns> Returns true if the given cell is selected. </returns>
        public virtual bool isCellSelected(object cell)
        {
            return selectionModel.isSelected(cell);
        }

        /// 
        /// <returns> Returns true if the selection is empty. </returns>
        public virtual bool SelectionEmpty
        {
            get
            {
                return selectionModel.Empty;
            }
        }

        /// 
        public virtual void clearSelection()
        {
            selectionModel.clear();
        }

        /// 
        /// <returns> Returns the selection cell. </returns>
        public virtual object SelectionCell
        {
            get
            {
                return selectionModel.Cell;
            }
            set
            {
                selectionModel.Cell = value;
            }
        }


        /// 
        /// <returns> Returns the selection cells. </returns>
        public virtual object[] getSelectionCells()
        {
            return selectionModel.Cells;
        }

        /// 
        public virtual void setSelectionCells(object[] cells)
        {
            selectionModel.Cells = cells;
        }

        /// 
        /// <param name="cells"> </param>
        public virtual void setSelectionCells(ICollection<object> cells)
        {
            if (cells != null)
            {
                setSelectionCells(cells.ToArray());
            }
        }

        /// 
        public virtual void addSelectionCell(object cell)
        {
            selectionModel.addCell(cell);
        }

        /// 
        public virtual void addSelectionCells(object[] cells)
        {
            selectionModel.addCells(cells);
        }

        /// 
        public virtual void removeSelectionCell(object cell)
        {
            selectionModel.removeCell(cell);
        }

        /// 
        public virtual void removeSelectionCells(object[] cells)
        {
            selectionModel.removeCells(cells);
        }

        /// <summary>
        /// Selects the next cell.
        /// </summary>
        public virtual void selectNextCell()
        {
            selectCell(true, false, false);
        }

        /// <summary>
        /// Selects the previous cell.
        /// </summary>
        public virtual void selectPreviousCell()
        {
            selectCell(false, false, false);
        }

        /// <summary>
        /// Selects the parent cell.
        /// </summary>
        public virtual void selectParentCell()
        {
            selectCell(false, true, false);
        }

        /// <summary>
        /// Selects the first child cell.
        /// </summary>
        public virtual void selectChildCell()
        {
            selectCell(false, false, true);
        }

        /// <summary>
        /// Selects the next, parent, first child or previous cell, if all arguments
        /// are false.
        /// </summary>
        /// <param name="isNext"> </param>
        /// <param name="isParent"> </param>
        /// <param name="isChild"> </param>
        public virtual void selectCell(bool isNext, bool isParent, bool isChild)
        {
            object cell = SelectionCell;

            if (SelectionCount > 1)
            {
                clearSelection();
            }

            object parent = (cell != null) ? model.getParent(cell) : DefaultParent;
            int childCount = model.getChildCount(parent);

            if (cell == null && childCount > 0)
            {
                object child = model.getChildAt(parent, 0);
                SelectionCell = child;
            }
            else if ((cell == null || isParent) && view.getState(parent) != null && model.getGeometry(parent) != null)
            {
                if (CurrentRoot != parent)
                {
                    SelectionCell = parent;
                }
            }
            else if (cell != null && isChild)
            {
                int tmp = model.getChildCount(cell);

                if (tmp > 0)
                {
                    object child = model.getChildAt(cell, 0);
                    SelectionCell = child;
                }
            }
            else if (childCount > 0)
            {
                int i = ((mxICell)parent).getIndex((mxICell)cell);

                if (isNext)
                {
                    i++;
                    SelectionCell = model.getChildAt(parent, i % childCount);
                }
                else
                {
                    i--;
                    int index = (i < 0) ? childCount - 1 : i;
                    SelectionCell = model.getChildAt(parent, index);
                }
            }
        }

        /// <summary>
        /// Selects all vertices inside the default parent.
        /// </summary>
        public virtual void selectVertices()
        {
            selectVertices(null);
        }

        /// <summary>
        /// Selects all vertices inside the given parent or the default parent
        /// if no parent is given.
        /// </summary>
        public virtual void selectVertices(object parent)
        {
            selectCells(true, false, parent);
        }

        /// <summary>
        /// Selects all vertices inside the default parent.
        /// </summary>
        public virtual void selectEdges()
        {
            selectEdges(null);
        }

        /// <summary>
        /// Selects all vertices inside the given parent or the default parent
        /// if no parent is given.
        /// </summary>
        public virtual void selectEdges(object parent)
        {
            selectCells(false, true, parent);
        }

        /// <summary>
        /// Selects all vertices and/or edges depending on the given boolean
        /// arguments recursively, starting at the default parent. Use
        /// <code>selectAll</code> to select all cells.
        /// </summary>
        /// <param name="vertices"> Boolean indicating if vertices should be selected. </param>
        /// <param name="edges"> Boolean indicating if edges should be selected. </param>
        public virtual void selectCells(bool vertices, bool edges)
        {
            selectCells(vertices, edges, null);
        }

        /// <summary>
        /// Selects all vertices and/or edges depending on the given boolean
        /// arguments recursively, starting at the given parent or the default
        /// parent if no parent is specified. Use <code>selectAll</code> to select
        /// all cells.
        /// </summary>
        /// <param name="vertices"> Boolean indicating if vertices should be selected. </param>
        /// <param name="edges"> Boolean indicating if edges should be selected. </param>
        /// <param name="parent"> Optional cell that acts as the root of the recursion.
        /// Default is <code>defaultParent</code>. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void selectCells(final boolean vertices, final boolean edges, Object parent)
        public virtual void selectCells(bool vertices, bool edges, object parent)
        {
            if (parent == null)
            {
                parent = DefaultParent;
            }

            ICollection<object> cells = mxGraphModel.filterDescendants(Model, new FilterAnonymousInnerClass5(this, vertices, edges));
            setSelectionCells(cells);
        }

        private class FilterAnonymousInnerClass5 : mxGraphModel.Filter
        {
            private readonly mxGraph outerInstance;

            private bool vertices;
            private bool edges;

            public FilterAnonymousInnerClass5(mxGraph outerInstance, bool vertices, bool edges)
            {
                this.outerInstance = outerInstance;
                this.vertices = vertices;
                this.edges = edges;
            }


            /// 
            public virtual bool filter(object cell)
            {
                return outerInstance.view.getState(cell) != null && outerInstance.model.getChildCount(cell) == 0 && ((outerInstance.model.isVertex(cell) && vertices) || (outerInstance.model.isEdge(cell) && edges));
            }

        }

        /// 
        public virtual void selectAll()
        {
            selectAll(null);
        }

        /// <summary>
        /// Selects all children of the given parent cell or the children of the
        /// default parent if no parent is specified. To select leaf vertices and/or
        /// edges use <selectCells>.
        /// </summary>
        /// <param name="parent">  Optional <mxCell> whose children should be selected.
        /// Default is <defaultParent>. </param>
        public virtual void selectAll(object parent)
        {
            if (parent == null)
            {
                parent = DefaultParent;
            }

            object[] children = mxGraphModel.getChildren(model, parent);

            if (children != null)
            {
                setSelectionCells(children);
            }
        }

        //
        // Images and drawing
        //

        /// <summary>
        /// Draws the graph onto the given canvas.
        /// </summary>
        /// <param name="canvas"> Canvas onto which the graph should be drawn. </param>
        public virtual void drawGraph(mxICanvas canvas)
        {
            drawCell(canvas, Model.Root);
        }

        /// <summary>
        /// Draws the given cell and its descendants onto the specified canvas.
        /// </summary>
        /// <param name="canvas"> Canvas onto which the cell should be drawn. </param>
        /// <param name="cell"> Cell that should be drawn onto the canvas. </param>
        public virtual void drawCell(mxICanvas canvas, object cell)
        {
            drawState(canvas, View.getState(cell), getLabel(cell));

            // Draws the children on top of their parent
            int childCount = model.getChildCount(cell);

            for (int i = 0; i < childCount; i++)
            {
                object child = model.getChildAt(cell, i);
                drawCell(canvas, child);
            }
        }

        /// <summary>
        /// Draws the cell state with the given label onto the canvas. No
        /// children or descendants are painted here. This method invokes
        /// cellDrawn after the cell, but not its descendants have been
        /// painted.
        /// </summary>
        /// <param name="canvas"> Canvas onto which the cell should be drawn. </param>
        /// <param name="state"> State of the cell to be drawn. </param>
        /// <param name="label"> Label of the cell to be dranw. </param>
        public virtual void drawState(mxICanvas canvas, mxCellState state, string label)
        {

        }

        /// <summary>
        /// Called when a cell has been painted as the specified object, typically a
        /// DOM node that represents the given cell graphically in a document.
        /// </summary>
        protected internal virtual void cellDrawn(mxICanvas canvas, mxCellState state, object element, object labelElement)
        {
            if (element is Element)
            {
                string link = getLinkForCell(state.Cell);

                if (!string.ReferenceEquals(link, null))
                {
                    string title = getToolTipForCell(state.Cell);
                    Element elem = (Element)element;

                    if (elem.NodeName.StartsWith("v:"))
                    {
                        elem.setAttribute("href", link.ToString());

                        if (!string.ReferenceEquals(title, null))
                        {
                            elem.setAttribute("title", title);
                        }
                    }
                    else if (elem.OwnerDocument.getElementsByTagName("svg").Length > 0)
                    {
                        Element xlink = elem.OwnerDocument.createElement("a");
                        xlink.setAttribute("xlink:href", link.ToString());

                        elem.ParentNode.replaceChild(xlink, elem);
                        xlink.appendChild(elem);

                        if (!string.ReferenceEquals(title, null))
                        {
                            xlink.setAttribute("xlink:title", title);
                        }

                        elem = xlink;
                    }
                    else
                    {
                        Element a = elem.OwnerDocument.createElement("a");
                        a.setAttribute("href", link.ToString());
                        a.setAttribute("style", "text-decoration:none;");

                        elem.ParentNode.replaceChild(a, elem);
                        a.appendChild(elem);

                        if (!string.ReferenceEquals(title, null))
                        {
                            a.setAttribute("title", title);
                        }

                        elem = a;
                    }

                    string target = getTargetForCell(state.Cell);

                    if (!string.ReferenceEquals(target, null))
                    {
                        elem.setAttribute("target", target);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the hyperlink to be used for the given cell.
        /// </summary>
        protected internal virtual string getLinkForCell(object cell)
        {
            return null;
        }

        /// <summary>
        /// Returns the hyperlink to be used for the given cell.
        /// </summary>
        protected internal virtual string getTargetForCell(object cell)
        {
            return null;
        }

        //
        // Redirected to change support
        //

        /// <param name="listener"> </param>
        /// <seealso cref= java.beans.PropertyChangeSupport#addPropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
        public virtual void addPropertyChangeListener(PropertyChangeListener listener)
        {
            changeSupport.addPropertyChangeListener(listener);
        }

        /// <param name="propertyName"> </param>
        /// <param name="listener"> </param>
        /// <seealso cref= java.beans.PropertyChangeSupport#addPropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        public virtual void addPropertyChangeListener(string propertyName, PropertyChangeListener listener)
        {
            changeSupport.addPropertyChangeListener(propertyName, listener);
        }

        /// <param name="listener"> </param>
        /// <seealso cref= java.beans.PropertyChangeSupport#removePropertyChangeListener(java.beans.PropertyChangeListener) </seealso>
        public virtual void removePropertyChangeListener(PropertyChangeListener listener)
        {
            changeSupport.removePropertyChangeListener(listener);
        }

        /// <param name="propertyName"> </param>
        /// <param name="listener"> </param>
        /// <seealso cref= java.beans.PropertyChangeSupport#removePropertyChangeListener(java.lang.String, java.beans.PropertyChangeListener) </seealso>
        public virtual void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
        {
            changeSupport.removePropertyChangeListener(propertyName, listener);
        }

        /// <summary>
        /// Prints the version number on the console. 
        /// </summary>
        public static void Main(string[] args)
        {
            Console.WriteLine("mxGraph version \"" + VERSION + "\"");
        }

    }
    }
}
