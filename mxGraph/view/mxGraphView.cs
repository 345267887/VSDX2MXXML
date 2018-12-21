using mxGraph;
using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxGraphView.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.view
{


    using mxGeometry = model.mxGeometry;
    using mxGraphModel = model.mxGraphModel;
    using mxIGraphModel = model.mxIGraphModel;
    using mxConstants = util.mxConstants;
    using mxEvent = util.mxEvent;
    using mxEventObject = util.mxEventObject;
    using mxEventSource = util.mxEventSource;
    using mxPoint = util.mxPoint;
    using mxRectangle = util.mxRectangle;
    using mxUndoableEdit = util.mxUndoableEdit;
    using mxUndoableChange = util.mxUndoableEdit.mxUndoableChange;
    using mxUtils = util.mxUtils;
    using mxEdgeStyleFunction = view.mxEdgeStyle.mxEdgeStyleFunction;
    using mxPerimeterFunction = view.mxPerimeter.mxPerimeterFunction;

    /// <summary>
    /// Implements a view for the graph. This class is in charge of computing the
    /// absolute coordinates for the relative child geometries, the points for
    /// perimeters and edge styles and keeping them cached in cell states for
    /// faster retrieval. The states are updated whenever the model or the view
    /// state (translate, scale) changes. The scale and translate are honoured in
    /// the bounds.
    /// 
    /// This class fires the following events:
    /// 
    /// mxEvent.UNDO fires after the root was changed in setCurrentRoot. The
    /// <code>edit</code> property contains the mxUndoableEdit which contains the
    /// mxCurrentRootChange.
    /// 
    /// mxEvent.SCALE_AND_TRANSLATE fires after the scale and transle have been
    /// changed in scaleAndTranslate. The <code>scale</code>, <code>previousScale</code>,
    /// <code>translate</code> and <code>previousTranslate</code> properties contain
    /// the new and previous scale and translate, respectively.
    /// 
    /// mxEvent.SCALE fires after the scale was changed in setScale. The
    /// <code>scale</code> and <code>previousScale</code> properties contain the
    /// new and previous scale.
    /// 
    /// mxEvent.TRANSLATE fires after the translate was changed in setTranslate. The
    /// <code>translate</code> and <code>previousTranslate</code> properties contain
    /// the new and previous value for translate.
    /// 
    /// mxEvent.UP and mxEvent.DOWN fire if the current root is changed by executing
    /// a mxCurrentRootChange. The event name depends on the location of the root
    /// in the cell hierarchy with respect to the current root. The
    /// <code>root</code> and <code>previous</code> properties contain the new and
    /// previous root, respectively.
    /// </summary>
    public class mxGraphView : mxEventSource
    {

        /// 
        private static mxPoint EMPTY_POINT = new mxPoint();

        /// <summary>
        /// Reference to the enclosing graph.
        /// </summary>
        protected internal mxGraph graph;

        /// <summary>
        /// mxCell that acts as the root of the displayed cell hierarchy.
        /// </summary>
        protected internal object currentRoot = null;

        /// <summary>
        /// Caches the current bounds of the graph.
        /// </summary>
        protected internal mxRectangle graphBounds = new mxRectangle();

        /// <summary>
        /// Specifies the scale. Default is 1 (100%).
        /// </summary>
        protected internal double scale = 1;

        /// <summary>
        /// Point that specifies the current translation. Default is a new
        /// empty point.
        /// </summary>
        protected internal mxPoint translate = new mxPoint(0, 0);

        /// <summary>
        /// Maps from cells to cell states.
        /// </summary>
        protected internal Dictionary<object, mxCellState> states = new Dictionary<object, mxCellState>();

        /// <summary>
        /// Constructs a new view for the given graph.
        /// </summary>
        /// <param name="graph"> Reference to the enclosing graph. </param>
        public mxGraphView(mxGraph graph)
        {
            this.graph = graph;
        }

        /// <summary>
        /// Returns the enclosing graph.
        /// </summary>
        /// <returns> Returns the enclosing graph. </returns>
        public virtual mxGraph Graph
        {
            get
            {
                return graph;
            }
        }

        /// <summary>
        /// Returns the dictionary that maps from cells to states.
        /// </summary>
        public virtual Dictionary<object, mxCellState> States
        {
            get
            {
                return states;
            }
            set
            {
                this.states = value;
            }
        }


        /// <summary>
        /// Returns the cached diagram bounds.
        /// </summary>
        /// <returns> Returns the diagram bounds. </returns>
        public virtual mxRectangle GraphBounds
        {
            get
            {
                return graphBounds;
            }
            set
            {
                graphBounds = value;
            }
        }


        /// <summary>
        /// Returns the current root.
        /// </summary>
        public virtual object CurrentRoot
        {
            set { currentRoot = value; }
            get
            {
                return currentRoot;
            }
        }

        /// <summary>
        /// Sets and returns the current root and fires an undo event.
        /// </summary>
        /// <param name="root"> mxCell that specifies the root of the displayed cell hierarchy. </param>
        /// <returns> Returns the object that represents the current root. </returns>
        public virtual object setCurrentRoot(object root)
        {
            if (currentRoot != root)
            {
                mxCurrentRootChange change = new mxCurrentRootChange(this, root);
                change.execute();
                mxUndoableEdit edit = new mxUndoableEdit(this, false);
                edit.add(change);
                fireEvent(new mxEventObject(mxEvent.UNDO, "edit", edit));
            }

            return root;
        }

        /// <summary>
        /// Sets the scale and translation. Fires a "scaleAndTranslate"
        /// event after calling revalidate. Revalidate is only called if
        /// isEventsEnabled.
        /// </summary>
        /// <param name="scale"> Decimal value that specifies the new scale (1 is 100%). </param>
        /// <param name="dx"> X-coordinate of the translation. </param>
        /// <param name="dy"> Y-coordinate of the translation. </param>
        public virtual void scaleAndTranslate(double scale, double dx, double dy)
        {
            double previousScale = this.scale;
            object previousTranslate = translate.clone();

            if (scale != this.scale || dx != translate.X || dy != translate.Y)
            {
                this.scale = scale;
                translate = new mxPoint(dx, dy);

                if (EventsEnabled)
                {
                    revalidate();
                }
            }

            fireEvent(new mxEventObject(mxEvent.SCALE_AND_TRANSLATE, "scale", scale, "previousScale", previousScale, "translate", translate, "previousTranslate", previousTranslate));
        }

        /// <summary>
        /// Returns the current scale.
        /// </summary>
        /// <returns> Returns the scale. </returns>
        public virtual double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                double previousScale = scale;

                if (scale != value)
                {
                    scale = value;

                    if (EventsEnabled)
                    {
                        revalidate();
                    }
                }

                fireEvent(new mxEventObject(mxEvent.SCALE, "scale", scale, "previousScale", previousScale));
            }
        }


        /// <summary>
        /// Returns the current translation.
        /// </summary>
        /// <returns> Returns the translation. </returns>
        public virtual mxPoint Translate
        {
            get
            {
                return translate;
            }
            set
            {
                object previousTranslate = translate.clone();

                if (value != null && (value.X != translate.X || value.Y != translate.Y))
                {
                    translate = value;

                    if (EventsEnabled)
                    {
                        revalidate();
                    }
                }

                fireEvent(new mxEventObject(mxEvent.TRANSLATE, "translate", translate, "previousTranslate", previousTranslate));
            }
        }


        /// <summary>
        /// Returns the bounding box for an array of cells or null, if no cells are
        /// specified.
        /// </summary>
        /// <param name="cells"> </param>
        /// <returns> Returns the bounding box for the given cells. </returns>
        public virtual mxRectangle getBounds(object[] cells)
        {
            return getBounds(cells, false);
        }

        /// <summary>
        /// Returns the bounding box for an array of cells or null, if no cells are
        /// specified.
        /// </summary>
        /// <param name="cells"> </param>
        /// <returns> Returns the bounding box for the given cells. </returns>
        public virtual mxRectangle getBoundingBox(object[] cells)
        {
            return getBounds(cells, true);
        }

        /// <summary>
        /// Returns the bounding box for an array of cells or null, if no cells are
        /// specified.
        /// </summary>
        /// <param name="cells"> </param>
        /// <returns> Returns the bounding box for the given cells. </returns>
        public virtual mxRectangle getBounds(object[] cells, bool boundingBox)
        {
            mxRectangle result = null;

            if (cells != null && cells.Length > 0)
            {
                mxIGraphModel model = graph.Model;

                for (int i = 0; i < cells.Length; i++)
                {
                    if (model.isVertex(cells[i]) || model.isEdge(cells[i]))
                    {
                        mxCellState state = getState(cells[i]);

                        if (state != null)
                        {
                            mxRectangle tmp = (boundingBox) ? state.BoundingBox : state;

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
                }
            }

            return result;
        }

        /// <summary>
        /// Removes all existing cell states and invokes validate.
        /// </summary>
        public virtual void reload()
        {
            states.Clear();
            validate();
        }

        /// 
        public virtual void revalidate()
        {
            invalidate();
            validate();
        }

        /// <summary>
        /// Invalidates all cell states.
        /// </summary>
        public virtual void invalidate()
        {
            invalidate(null);
        }

        /// <summary>
        /// Removes the state of the given cell and all descendants if the given
        /// cell is not the current root.
        /// </summary>
        /// <param name="cell"> </param>
        /// <param name="force"> </param>
        /// <param name="recurse"> </param>
        public virtual void clear(object cell, bool force, bool recurse)
        {
            removeState(cell);

            if (recurse && (force || cell != currentRoot))
            {
                mxIGraphModel model = graph.Model;
                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    clear(model.getChildAt(cell, i), force, recurse);
                }
            }
            else
            {
                invalidate(cell);
            }
        }

        /// <summary>
        /// Invalidates the state of the given cell, all its descendants and
        /// connected edges.
        /// </summary>
        public virtual void invalidate(object cell)
        {
            mxIGraphModel model = graph.Model;
            cell = (cell != null) ? cell : model.Root;
            mxCellState state = getState(cell);

            if (state == null || !state.Invalid)
            {
                if (state != null)
                {
                    state.Invalid = true;
                }

                // Recursively invalidates all descendants
                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    object child = model.getChildAt(cell, i);
                    invalidate(child);
                }

                // Propagates invalidation to all connected edges
                int edgeCount = model.getEdgeCount(cell);

                for (int i = 0; i < edgeCount; i++)
                {
                    invalidate(model.getEdgeAt(cell, i));
                }
            }
        }

        /// <summary>
        /// First validates all bounds and then validates all points recursively on
        /// all visible cells.
        /// </summary>
        public virtual void validate()
        {
            object cell = (currentRoot != null) ? currentRoot : graph.Model.Root;

            if (cell != null)
            {
                validateBounds(null, cell);
                mxRectangle bounds = validatePoints(null, cell);

                if (bounds == null)
                {
                    bounds = new mxRectangle();
                }

                GraphBounds = bounds;
            }
        }

        /// <summary>
        /// Validates the bounds of the given parent's child using the given parent
        /// state as the origin for the child. The validation is carried out
        /// recursively for all non-collapsed descendants.
        /// </summary>
        /// <param name="parentState"> Object that represents the state of the parent cell. </param>
        /// <param name="cell"> Cell for which the bounds in the state should be updated. </param>
        public virtual void validateBounds(mxCellState parentState, object cell)
        {
            mxIGraphModel model = graph.Model;
            mxCellState state = getState(cell, true);

            if (state != null && state.Invalid)
            {
                if (!graph.isCellVisible(cell))
                {
                    removeState(cell);
                }
                else if (cell != currentRoot && parentState != null)
                {
                    state.AbsoluteOffset.X = (0);
                    state.AbsoluteOffset.Y = (0);
                    state.Origin = new mxPoint(parentState.Origin);
                    mxGeometry geo = graph.getCellGeometry(cell);

                    if (geo != null)
                    {
                        if (!model.isEdge(cell))
                        {
                            mxPoint origin = state.Origin;
                            mxPoint offset = geo.Offset;

                            if (offset == null)
                            {
                                offset = EMPTY_POINT;
                            }

                            if (geo.Relative)
                            {
                                origin.X = origin.X + geo.X * parentState.Width / scale + offset.X;
                                origin.Y = origin.Y + geo.Y * parentState.Height / scale + offset.Y;
                            }
                            else
                            {
                                state.AbsoluteOffset = new mxPoint(scale * offset.X, scale * offset.Y);
                                origin.X = origin.X + geo.X;
                                origin.Y = origin.Y + geo.Y;
                            }
                        }

                        // Updates the cell state's bounds
                        state.X = scale * (translate.X + state.Origin.X);
                        state.Y = scale * (translate.Y + state.Origin.Y);
                        state.Width = scale * geo.Width;
                        state.Height = scale * geo.Height;

                        if (model.isVertex(cell))
                        {
                            updateVertexLabelOffset(state);
                        }
                    }
                }

                // Applies child offset to origin
                mxPoint offset1 = graph.getChildOffsetForCell(cell);

                if (offset1 != null)
                {
                    state.Origin.X = state.Origin.X + offset1.X;
                    state.Origin.Y = state.Origin.Y + offset1.Y;
                }
            }

            // Recursively validates the child bounds
            if (state != null && (!graph.isCellCollapsed(cell) || cell == currentRoot))
            {
                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    validateBounds(state, model.getChildAt(cell, i));
                }
            }
        }

        /// <summary>
        /// Updates the absoluteOffset of the given vertex cell state. This takes
        /// into account the label position styles.
        /// </summary>
        /// <param name="state"> Cell state whose absolute offset should be updated. </param>
        public virtual void updateVertexLabelOffset(mxCellState state)
        {
            string horizontal = mxUtils.getString(state.Style, mxConstants.STYLE_LABEL_POSITION, mxConstants.ALIGN_CENTER);

            if (horizontal.Equals(mxConstants.ALIGN_LEFT))
            {
                state.absoluteOffset.X = (state.absoluteOffset.X - state.Width);
            }
            else if (horizontal.Equals(mxConstants.ALIGN_RIGHT))
            {
                state.absoluteOffset.Y = (state.absoluteOffset.X + state.Width);
            }

            string vertical = mxUtils.getString(state.Style, mxConstants.STYLE_VERTICAL_LABEL_POSITION, mxConstants.ALIGN_MIDDLE);

            if (vertical.Equals(mxConstants.ALIGN_TOP))
            {
                state.absoluteOffset.Y = (state.absoluteOffset.Y - state.Height);
            }
            else if (vertical.Equals(mxConstants.ALIGN_BOTTOM))
            {
                state.absoluteOffset.Y = (state.absoluteOffset.Y + state.Height);
            }
        }

        /// <summary>
        /// Validates the points for the state of the given cell recursively if the
        /// cell is not collapsed and returns the bounding box of all visited states
        /// as a rectangle.
        /// </summary>
        /// <param name="parentState"> Object that represents the state of the parent cell. </param>
        /// <param name="cell"> Cell for which the points in the state should be updated. </param>
        /// <returns> Returns the bounding box for the given cell. </returns>
        public virtual mxRectangle validatePoints(mxCellState parentState, object cell)
        {
            mxIGraphModel model = graph.Model;
            mxCellState state = getState(cell);
            mxRectangle bbox = null;

            if (state != null)
            {
                if (state.Invalid)
                {
                    mxGeometry geo = graph.getCellGeometry(cell);

                    if (geo != null && model.isEdge(cell))
                    {
                        // Updates the points on the source terminal if its an edge
                        mxCellState source = getState(getVisibleTerminal(cell, true));

                        if (source != null && model.isEdge(source.Cell) && !model.isAncestor(source, cell))
                        {
                            mxCellState tmp = getState(model.getParent(source.Cell));
                            validatePoints(tmp, source);
                        }

                        // Updates the points on the source terminal if its an edge
                        mxCellState target = getState(getVisibleTerminal(cell, false));

                        if (target != null && model.isEdge(target.Cell) && !model.isAncestor(target, cell))
                        {
                            mxCellState tmp = getState(model.getParent(target.Cell));
                            validatePoints(tmp, target);
                        }

                        updateFixedTerminalPoints(state, source, target);
                        updatePoints(state, geo.Points, source, target);
                        updateFloatingTerminalPoints(state, source, target);
                        updateEdgeBounds(state);
                        state.AbsoluteOffset = getPoint(state, geo);
                    }
                    else if (geo != null && geo.Relative && parentState != null && model.isEdge(parentState.Cell))
                    {
                        mxPoint origin = getPoint(parentState, geo);

                        if (origin != null)
                        {
                            state.X = origin.X;
                            state.Y = origin.Y;

                            origin.X = (origin.X / scale) - translate.X;
                            origin.Y = (origin.Y / scale) - translate.Y;
                            state.Origin = origin;

                            childMoved(parentState, state);
                        }
                    }

                    state.Invalid = false;
                }

                if (model.isEdge(cell) || model.isVertex(cell))
                {
                    updateLabelBounds(state);
                    bbox = new mxRectangle(updateBoundingBox(state));
                }
            }

            if (state != null && (!graph.isCellCollapsed(cell) || cell == currentRoot))
            {
                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    object child = model.getChildAt(cell, i);
                    mxRectangle bounds = validatePoints(state, child);

                    if (bounds != null)
                    {
                        if (bbox == null)
                        {
                            bbox = bounds;
                        }
                        else
                        {
                            bbox.add(bounds);
                        }
                    }
                }
            }

            return bbox;
        }

        /// <summary>
        /// Invoked when a child state was moved as a result of late evaluation
        /// of its position. This is invoked for relative edge children whose
        /// position can only be determined after the points of the parent edge
        /// are updated in validatePoints, and validates the bounds of all
        /// descendants of the child using validateBounds.
        /// </summary>
        protected internal virtual void childMoved(mxCellState parent, mxCellState child)
        {
            object cell = child.Cell;

            // Children of relative edge children need to validate
            // their bounds after their parent state was updated
            if (!graph.isCellCollapsed(cell) || cell == currentRoot)
            {
                mxIGraphModel model = graph.Model;
                int childCount = model.getChildCount(cell);

                for (int i = 0; i < childCount; i++)
                {
                    validateBounds(child, model.getChildAt(cell, i));
                }
            }
        }

        /// <summary>
        /// Updates the label bounds in the given state.
        /// </summary>
        public virtual void updateLabelBounds(mxCellState state)
        {
            object cell = state.Cell;
            IDictionary<string, object> style = state.Style;

            if (mxUtils.getString(style, mxConstants.STYLE_OVERFLOW, "").Equals("fill"))
            {
                state.LabelBounds = new mxRectangle(state);
            }
            else
            {
                string label = graph.getLabel(cell);
                mxRectangle vertexBounds = (!graph.Model.isEdge(cell)) ? state : null;
                state.LabelBounds = mxUtils.getLabelPaintBounds(label, style, graph.isHtmlLabel(cell), state.AbsoluteOffset, vertexBounds, scale);
            }
        }

        /// <summary>
        /// Updates the bounding box in the given cell state.
        /// </summary>
        /// <param name="state"> Cell state whose bounding box should be
        /// updated. </param>
        public virtual mxRectangle updateBoundingBox(mxCellState state)
        {
            // Gets the cell bounds and adds shadows and markers
            mxRectangle rect = new mxRectangle(state);
            IDictionary<string, object> style = state.Style;

            // Adds extra pixels for the marker and stroke assuming
            // that the border stroke is centered around the bounds
            // and the first pixel is drawn inside the bounds
            double strokeWidth = Math.Max(1, Math.Round(mxUtils.getInt(style, mxConstants.STYLE_STROKEWIDTH, 1) * scale));
            strokeWidth -= Math.Max(1, strokeWidth / 2);

            if (graph.Model.isEdge(state.Cell))
            {
                int ms = 0;

                if (style.ContainsKey(mxConstants.STYLE_ENDARROW) || style.ContainsKey(mxConstants.STYLE_STARTARROW))
                {
                    ms = (int)Math.Round(mxConstants.DEFAULT_MARKERSIZE * scale);
                }

                // Adds the strokewidth
                rect.grow(ms + strokeWidth);

                // Adds worst case border for an arrow shape
                if (mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_ARROW))
                {
                    rect.grow(mxConstants.ARROW_WIDTH / 2);
                }
            }
            else
            {
                rect.grow(strokeWidth);
            }

            // Adds extra pixels for the shadow
            if (mxUtils.isTrue(style, mxConstants.STYLE_SHADOW))
            {
                rect.Width = rect.Width + mxConstants.SHADOW_OFFSETX;
                rect.Height = rect.Height + mxConstants.SHADOW_OFFSETY;
            }

            // Adds oversize images in labels
            if (mxUtils.getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_LABEL))
            {
                if (!string.ReferenceEquals(mxUtils.getString(style, mxConstants.STYLE_IMAGE), null))
                {
                    double w = mxUtils.getInt(style, mxConstants.STYLE_IMAGE_WIDTH, mxConstants.DEFAULT_IMAGESIZE) * scale;
                    double h = mxUtils.getInt(style, mxConstants.STYLE_IMAGE_HEIGHT, mxConstants.DEFAULT_IMAGESIZE) * scale;

                    double x = state.X;
                    double y = 0;

                    string imgAlign = mxUtils.getString(style, mxConstants.STYLE_IMAGE_ALIGN, mxConstants.ALIGN_LEFT);
                    string imgValign = mxUtils.getString(style, mxConstants.STYLE_IMAGE_VERTICAL_ALIGN, mxConstants.ALIGN_MIDDLE);

                    if (imgAlign.Equals(mxConstants.ALIGN_RIGHT))
                    {
                        x += state.Width - w;
                    }
                    else if (imgAlign.Equals(mxConstants.ALIGN_CENTER))
                    {
                        x += (state.Width - w) / 2;
                    }

                    if (imgValign.Equals(mxConstants.ALIGN_TOP))
                    {
                        y = state.Y;
                    }
                    else if (imgValign.Equals(mxConstants.ALIGN_BOTTOM))
                    {
                        y = state.Y + state.Height - h;
                    }
                    else
                    {
                        // MIDDLE
                        y = state.Y + (state.Height - h) / 2;
                    }

                    rect.add(new mxRectangle(x, y, w, h));
                }
            }

            // Adds the rotated bounds to the bounding box if the
            // shape is rotated
            double rotation = mxUtils.getDouble(style, mxConstants.STYLE_ROTATION);
            mxRectangle bbox = mxUtils.getBoundingBox(rect, rotation);

            // Add the rotated bounding box to the non-rotated so
            // that all handles are also covered
            rect.add(bbox);

            // Unifies the cell bounds and the label bounds
            if (!graph.isLabelClipped(state.Cell))
            {
                rect.add(state.LabelBounds);
            }

            state.BoundingBox = rect;

            return rect;
        }

        /// <summary>
        /// Sets the initial absolute terminal points in the given state before the edge
        /// style is computed.
        /// </summary>
        /// <param name="edge"> Cell state whose initial terminal points should be updated. </param>
        /// <param name="source"> Cell state which represents the source terminal. </param>
        /// <param name="target"> Cell state which represents the target terminal. </param>
        public virtual void updateFixedTerminalPoints(mxCellState edge, mxCellState source, mxCellState target)
        {
            updateFixedTerminalPoint(edge, source, true, graph.getConnectionConstraint(edge, source, true));
            updateFixedTerminalPoint(edge, target, false, graph.getConnectionConstraint(edge, target, false));
        }

        /// <summary>
        /// Sets the fixed source or target terminal point on the given edge.
        /// </summary>
        /// <param name="edge"> Cell state whose initial terminal points should be
        /// updated. </param>
        public virtual void updateFixedTerminalPoint(mxCellState edge, mxCellState terminal, bool source, mxConnectionConstraint constraint)
        {
            mxPoint pt = null;

            if (constraint != null)
            {
                pt = graph.getConnectionPoint(terminal, constraint);
            }

            if (pt == null && terminal == null)
            {
                mxPoint orig = edge.Origin;
                mxGeometry geo = graph.getCellGeometry(edge.cell);
                pt = geo.getTerminalPoint(source);

                if (pt != null)
                {
                    pt = new mxPoint(scale * (translate.X + pt.X + orig.X), scale * (translate.Y + pt.Y + orig.Y));
                }
            }

            edge.setAbsoluteTerminalPoint(pt, source);
        }

        /// <summary>
        /// Updates the absolute points in the given state using the specified array
        /// of points as the relative points.
        /// </summary>
        /// <param name="edge"> Cell state whose absolute points should be updated. </param>
        /// <param name="points"> Array of points that constitute the relative points. </param>
        /// <param name="source"> Cell state that represents the source terminal. </param>
        /// <param name="target"> Cell state that represents the target terminal. </param>
        public virtual void updatePoints(mxCellState edge, IList<mxPoint> points, mxCellState source, mxCellState target)
        {
            if (edge != null)
            {
                IList<mxPoint> pts = new List<mxPoint>();
                pts.Add(edge.getAbsolutePoint(0));
                mxEdgeStyleFunction edgeStyle = getEdgeStyle(edge, points, source, target);

                if (edgeStyle != null)
                {
                    mxCellState src = getTerminalPort(edge, source, true);
                    mxCellState trg = getTerminalPort(edge, target, false);

                    edgeStyle.apply(edge, src, trg, points, pts);
                }
                else if (points != null)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        pts.Add(transformControlPoint(edge, points[i]));
                    }
                }

                pts.Add(edge.getAbsolutePoint(edge.AbsolutePointCount - 1));
                edge.AbsolutePoints = pts;
            }
        }

        /// <summary>
        /// Transforms the given control point to an absolute point.
        /// </summary>
        public virtual mxPoint transformControlPoint(mxCellState state, mxPoint pt)
        {
            mxPoint origin = state.Origin;

            return new mxPoint(scale * (pt.X + translate.X + origin.X), scale * (pt.Y + translate.Y + origin.Y));
        }

        /// <summary>
        /// Returns the edge style function to be used to compute the absolute
        /// points for the given state, control points and terminals.
        /// </summary>
        public virtual mxEdgeStyleFunction getEdgeStyle(mxCellState edge, IList<mxPoint> points, object source, object target)
        {
            object edgeStyle = null;

            if (source != null && source == target)
            {
                edgeStyle = edge.Style[mxConstants.STYLE_LOOP];

                if (edgeStyle == null)
                {
                    edgeStyle = graph.DefaultLoopStyle;
                }
            }
            else if (!mxUtils.isTrue(edge.Style, mxConstants.STYLE_NOEDGESTYLE, false))
            {
                edgeStyle = edge.Style[mxConstants.STYLE_EDGE];
            }

            // Converts string values to objects
            if (edgeStyle is string)
            {
                string str = edgeStyle.ToString();
                object tmp = mxStyleRegistry.getValue(str);

                if (tmp == null)
                {
                    tmp = mxUtils.eval(str);
                }

                edgeStyle = tmp;
            }

            if (edgeStyle is mxEdgeStyleFunction)
            {
                return (mxEdgeStyleFunction)edgeStyle;
            }

            return null;
        }

        /// <summary>
        /// Updates the terminal points in the given state after the edge style was
        /// computed for the edge.
        /// </summary>
        /// <param name="state"> Cell state whose terminal points should be updated. </param>
        /// <param name="source"> Cell state that represents the source terminal. </param>
        /// <param name="target"> Cell state that represents the target terminal. </param>
        public virtual void updateFloatingTerminalPoints(mxCellState state, mxCellState source, mxCellState target)
        {
            mxPoint p0 = state.getAbsolutePoint(0);
            mxPoint pe = state.getAbsolutePoint(state.AbsolutePointCount - 1);

            if (pe == null && target != null)
            {
                updateFloatingTerminalPoint(state, target, source, false);
            }

            if (p0 == null && source != null)
            {
                updateFloatingTerminalPoint(state, source, target, true);
            }
        }

        /// <summary>
        /// Updates the absolute terminal point in the given state for the given
        /// start and end state, where start is the source if source is true.
        /// </summary>
        /// <param name="edge"> Cell state whose terminal point should be updated. </param>
        /// <param name="start"> Cell state for the terminal on "this" side of the edge. </param>
        /// <param name="end"> Cell state for the terminal on the other side of the edge. </param>
        /// <param name="source"> Boolean indicating if start is the source terminal state. </param>
        public virtual void updateFloatingTerminalPoint(mxCellState edge, mxCellState start, mxCellState end, bool source)
        {
            start = getTerminalPort(edge, start, source);
            mxPoint next = getNextPoint(edge, end, source);
            double border = mxUtils.getDouble(edge.Style, mxConstants.STYLE_PERIMETER_SPACING);
            border += mxUtils.getDouble(edge.Style, (source) ? mxConstants.STYLE_SOURCE_PERIMETER_SPACING : mxConstants.STYLE_TARGET_PERIMETER_SPACING);
            mxPoint pt = getPerimeterPoint(start, next, graph.isOrthogonal(edge), border);
            edge.setAbsoluteTerminalPoint(pt, source);
        }

        /// <summary>
        /// Returns a cell state that represents the source or target terminal or
        /// port for the given edge.
        /// </summary>
        public virtual mxCellState getTerminalPort(mxCellState state, mxCellState terminal, bool source)
        {
            string key = (source) ? mxConstants.STYLE_SOURCE_PORT : mxConstants.STYLE_TARGET_PORT;
            string id = mxUtils.getString(state.style, key);

            if (!string.ReferenceEquals(id, null) && graph.Model is mxGraphModel)
            {
                mxCellState tmp = getState(((mxGraphModel)graph.Model).getCell(id));

                // Only uses ports where a cell state exists
                if (tmp != null)
                {
                    terminal = tmp;
                }
            }

            return terminal;
        }

        /// <summary>
        /// Returns a point that defines the location of the intersection point between
        /// the perimeter and the line between the center of the shape and the given point.
        /// </summary>
        public virtual mxPoint getPerimeterPoint(mxCellState terminal, mxPoint next, bool orthogonal)
        {
            return getPerimeterPoint(terminal, next, orthogonal, 0);
        }

        /// <summary>
        /// Returns a point that defines the location of the intersection point between
        /// the perimeter and the line between the center of the shape and the given point.
        /// </summary>
        /// <param name="terminal"> Cell state for the source or target terminal. </param>
        /// <param name="next"> Point that lies outside of the given terminal. </param>
        /// <param name="orthogonal"> Boolean that specifies if the orthogonal projection onto
        /// the perimeter should be returned. If this is false then the intersection
        /// of the perimeter and the line between the next and the center point is
        /// returned. </param>
        /// <param name="border"> Optional border between the perimeter and the shape. </param>
        public virtual mxPoint getPerimeterPoint(mxCellState terminal, mxPoint next, bool orthogonal, double border)
        {
            mxPoint point = null;

            if (terminal != null)
            {
                mxPerimeterFunction perimeter = getPerimeterFunction(terminal);

                if (perimeter != null && next != null)
                {
                    mxRectangle bounds = getPerimeterBounds(terminal, border);

                    if (bounds.Width > 0 || bounds.Height > 0)
                    {
                        point = perimeter.apply(bounds, terminal, next, orthogonal);
                    }
                }

                if (point == null)
                {
                    point = getPoint(terminal);
                }
            }

            return point;
        }

        /// <summary>
        /// Returns the x-coordinate of the center point for automatic routing.
        /// </summary>
        /// <returns> Returns the x-coordinate of the routing center point. </returns>
        public virtual double getRoutingCenterX(mxCellState state)
        {
            float f = (state.Style != null) ? mxUtils.getFloat(state.Style, mxConstants.STYLE_ROUTING_CENTER_X) : 0;

            return state.CenterX + f * state.Width;
        }

        /// <summary>
        /// Returns the y-coordinate of the center point for automatic routing.
        /// </summary>
        /// <returns> Returns the y-coordinate of the routing center point. </returns>
        public virtual double getRoutingCenterY(mxCellState state)
        {
            float f = (state.Style != null) ? mxUtils.getFloat(state.Style, mxConstants.STYLE_ROUTING_CENTER_Y) : 0;

            return state.CenterY + f * state.Height;
        }

        /// <summary>
        /// Returns the perimeter bounds for the given terminal, edge pair.
        /// </summary>
        public virtual mxRectangle getPerimeterBounds(mxCellState terminal, double border)
        {
            if (terminal != null)
            {
                border += mxUtils.getDouble(terminal.Style, mxConstants.STYLE_PERIMETER_SPACING);
            }

            return terminal.getPerimeterBounds(border * scale);
        }

        /// <summary>
        /// Returns the perimeter function for the given state.
        /// </summary>
        public virtual mxPerimeterFunction getPerimeterFunction(mxCellState state)
        {
            object perimeter = state.Style[mxConstants.STYLE_PERIMETER];

            // Converts string values to objects
            if (perimeter is string)
            {
                string str = perimeter.ToString();
                object tmp = mxStyleRegistry.getValue(str);

                if (tmp == null)
                {
                    tmp = mxUtils.eval(str);
                }

                perimeter = tmp;
            }

            if (perimeter is mxPerimeterFunction)
            {
                return (mxPerimeterFunction)perimeter;
            }

            return null;
        }

        /// <summary>
        /// Returns the nearest point in the list of absolute points or the center
        /// of the opposite terminal.
        /// </summary>
        /// <param name="edge"> Cell state that represents the edge. </param>
        /// <param name="opposite"> Cell state that represents the opposite terminal. </param>
        /// <param name="source"> Boolean indicating if the next point for the source or target
        /// should be returned. </param>
        /// <returns> Returns the nearest point of the opposite side. </returns>
        public virtual mxPoint getNextPoint(mxCellState edge, mxCellState opposite, bool source)
        {
            IList<mxPoint> pts = edge.AbsolutePoints;
            mxPoint point = null;

            if (pts != null && (source || pts.Count > 2 || opposite == null))
            {
                int count = pts.Count;
                int index = (source) ? Math.Min(1, count - 1) : Math.Max(0, count - 2);
                point = pts[index];
            }

            if (point == null && opposite != null)
            {
                point = new mxPoint(opposite.CenterX, opposite.CenterY);
            }

            return point;
        }

        /// <summary>
        /// Returns the nearest ancestor terminal that is visible. The edge appears
        /// to be connected to this terminal on the display.
        /// </summary>
        /// <param name="edge"> Cell whose visible terminal should be returned. </param>
        /// <param name="source"> Boolean that specifies if the source or target terminal
        /// should be returned. </param>
        /// <returns> Returns the visible source or target terminal. </returns>
        public virtual object getVisibleTerminal(object edge, bool source)
        {
            mxIGraphModel model = graph.Model;
            object result = model.getTerminal(edge, source);
            object best = result;

            while (result != null && result != currentRoot)
            {
                if (!graph.isCellVisible(best) || graph.isCellCollapsed(result))
                {
                    best = result;
                }

                result = model.getParent(result);
            }

            // Checks if the result is not a layer
            if (model.getParent(best) == model.Root)
            {
                best = null;
            }

            return best;
        }

        /// <summary>
        /// Updates the given state using the bounding box of the absolute points.
        /// Also updates terminal distance, length and segments.
        /// </summary>
        /// <param name="state"> Cell state whose bounds should be updated. </param>
        public virtual void updateEdgeBounds(mxCellState state)
        {
            IList<mxPoint> points = state.AbsolutePoints;

            if (points != null && points.Count > 0)
            {
                mxPoint p0 = points[0];
                mxPoint pe = points[points.Count - 1];

                if (p0 == null || pe == null)
                {
                    // Note: This is an error that normally occurs
                    // if a connected edge has a null-terminal, ie.
                    // source is null and/or target is null and no
                    // additional control points are defined
                    removeState(state.Cell);
                }
                else
                {
                    if (p0.X != pe.X || p0.Y != pe.Y)
                    {
                        double dx = pe.X - p0.X;
                        double dy = pe.Y - p0.Y;
                        state.TerminalDistance = Math.Sqrt(dx * dx + dy * dy);
                    }
                    else
                    {
                        state.TerminalDistance = 0;
                    }

                    double length = 0;
                    double[] segments = new double[points.Count - 1];
                    mxPoint pt = p0;

                    double minX = pt.X;
                    double minY = pt.Y;
                    double maxX = minX;
                    double maxY = minY;

                    for (int i = 1; i < points.Count; i++)
                    {
                        mxPoint tmp = points[i];

                        if (tmp != null)
                        {
                            double dx = pt.X - tmp.X;
                            double dy = pt.Y - tmp.Y;

                            double segment = Math.Sqrt(dx * dx + dy * dy);
                            segments[i - 1] = segment;
                            length += segment;
                            pt = tmp;

                            minX = Math.Min(pt.X, minX);
                            minY = Math.Min(pt.Y, minY);
                            maxX = Math.Max(pt.X, maxX);
                            maxY = Math.Max(pt.Y, maxY);
                        }
                    }

                    state.Length = length;
                    state.Segments = segments;
                    double markerSize = 1; // TODO: include marker size

                    state.X = minX;
                    state.Y = minY;
                    state.Width = Math.Max(markerSize, maxX - minX);
                    state.Height = Math.Max(markerSize, maxY - minY);
                }
            }
        }

        /// <summary>
        /// Returns the absolute center point along the given edge.
        /// </summary>
        public virtual mxPoint getPoint(mxCellState state)
        {
            return getPoint(state, null);
        }

        /// <summary>
        /// Returns the absolute point on the edge for the given relative
        /// geometry as a point. The edge is represented by the given cell state.
        /// </summary>
        /// <param name="state"> Represents the state of the parent edge. </param>
        /// <param name="geometry"> Optional geometry that represents the relative location. </param>
        /// <returns> Returns the mxpoint that represents the absolute location
        /// of the given relative geometry. </returns>
        public virtual mxPoint getPoint(mxCellState state, mxGeometry geometry)
        {
            double x = state.CenterX;
            double y = state.CenterY;

            if (state.Segments != null && (geometry == null || geometry.Relative))
            {
                double gx = (geometry != null) ? geometry.X / 2 : 0;
                int pointCount = state.AbsolutePointCount;
                double dist = (gx + 0.5) * state.Length;
                double[] segments = state.Segments;
                double segment = segments[0];
                double length = 0;
                int index = 1;

                while (dist > length + segment && index < pointCount - 1)
                {
                    length += segment;
                    segment = segments[index++];
                }

                if (segment != 0)
                {
                    double factor = (dist - length) / segment;
                    mxPoint p0 = state.getAbsolutePoint(index - 1);
                    mxPoint pe = state.getAbsolutePoint(index);

                    if (p0 != null && pe != null)
                    {
                        double gy = 0;
                        double offsetX = 0;
                        double offsetY = 0;

                        if (geometry != null)
                        {
                            gy = geometry.Y;
                            mxPoint offset = geometry.Offset;

                            if (offset != null)
                            {
                                offsetX = offset.X;
                                offsetY = offset.Y;
                            }
                        }

                        double dx = pe.X - p0.X;
                        double dy = pe.Y - p0.Y;
                        double nx = dy / segment;
                        double ny = dx / segment;

                        x = p0.X + dx * factor + (nx * gy + offsetX) * scale;
                        y = p0.Y + dy * factor - (ny * gy - offsetY) * scale;
                    }
                }
            }
            else if (geometry != null)
            {
                mxPoint offset = geometry.Offset;

                if (offset != null)
                {
                    x += offset.X;
                    y += offset.Y;
                }
            }

            return new mxPoint(x, y);
        }

        /// <summary>
        /// Gets the relative point that describes the given, absolute label
        /// position for the given edge state.
        /// </summary>
        public virtual mxPoint getRelativePoint(mxCellState edgeState, double x, double y)
        {
            mxIGraphModel model = graph.Model;
            mxGeometry geometry = model.getGeometry(edgeState.Cell);

            if (geometry != null)
            {
                int pointCount = edgeState.AbsolutePointCount;

                if (geometry.Relative && pointCount > 1)
                {
                    double totalLength = edgeState.Length;
                    double[] segments = edgeState.Segments;

                    // Works which line segment the point of the label is closest to
                    mxPoint p0 = edgeState.getAbsolutePoint(0);
                    mxPoint pe = edgeState.getAbsolutePoint(1);
                    //Line2D line = new Line2D.Double(p0.Point, pe.Point);
                    double minDist = PointHelper.ptSegDistSq(p0.Point.X, p0.Point.Y,
                        pe.Point.X, pe.Point.Y,
                        x, y);
                    //line.ptSegDistSq(x, y);


                    int index = 0;
                    double tmp = 0;
                    double length = 0;

                    for (int i = 2; i < pointCount; i++)
                    {
                        tmp += segments[i - 2];
                        pe = edgeState.getAbsolutePoint(i);

                        //line = new Line2D.Double(p0.Point, pe.Point);
                        double dist = PointHelper.ptSegDistSq(p0.Point.X, p0.Point.Y,
                        pe.Point.X, pe.Point.Y,
                        x, y);//line.ptSegDistSq(x, y);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i - 1;
                            length = tmp;
                        }

                        p0 = pe;
                    }

                    double seg = segments[index];
                    p0 = edgeState.getAbsolutePoint(index);
                    pe = edgeState.getAbsolutePoint(index + 1);

                    double x2 = p0.X;
                    double y2 = p0.Y;

                    double x1 = pe.X;
                    double y1 = pe.Y;

                    double px = x;
                    double py = y;

                    double xSegment = x2 - x1;
                    double ySegment = y2 - y1;

                    px -= x1;
                    py -= y1;
                    double projlenSq = 0;

                    px = xSegment - px;
                    py = ySegment - py;
                    double dotprod = px * xSegment + py * ySegment;

                    if (dotprod <= 0.0)
                    {
                        projlenSq = 0;
                    }
                    else
                    {
                        projlenSq = dotprod * dotprod / (xSegment * xSegment + ySegment * ySegment);
                    }

                    double projlen = Math.Sqrt(projlenSq);

                    if (projlen > seg)
                    {
                        projlen = seg;
                    }

                    double yDistance = PointHelper.ptLineDist(p0.X, p0.Y, pe.X, pe.Y, x, y);// Line2D.ptLineDist(p0.X, p0.Y, pe.X, pe.Y, x, y);
                    int direction = PointHelper.RelativeCCW(p0.X, p0.Y, pe.X, pe.Y, x, y); //Line2D.relativeCCW(p0.X, p0.Y, pe.X, pe.Y, x, y);

                    if (direction == -1)
                    {
                        yDistance = -yDistance;
                    }

                    // Constructs the relative point for the label
                    return new mxPoint(Math.Round(((totalLength / 2 - length - projlen) / totalLength) * -2), Math.Round(yDistance / scale));
                }
            }

            return new mxPoint();
        }

        /// <summary>
        /// Returns the states for the given array of cells. The array contains all
        /// states that are not null, that is, the returned array may have less
        /// elements than the given array.
        /// </summary>
        public virtual mxCellState[] getCellStates(object[] cells)
        {
            List<mxCellState> result = new List<mxCellState>(cells.Length);

            for (int i = 0; i < cells.Length; i++)
            {
                mxCellState state = getState(cells[i]);

                if (state != null)
                {
                    result.Add(state);
                }
            }

            mxCellState[] resultArray = new mxCellState[result.Count];
            return result.ToArray();
        }

        /// <summary>
        /// Returns the state for the given cell or null if no state is defined for
        /// the cell.
        /// </summary>
        /// <param name="cell"> Cell whose state should be returned. </param>
        /// <returns> Returns the state for the given cell. </returns>
        public virtual mxCellState getState(object cell)
        {
            return getState(cell, false);
        }

        /// <summary>
        /// Returns the cell state for the given cell. If create is true, then
        /// the state is created if it does not yet exist.
        /// </summary>
        /// <param name="cell"> Cell for which a new state should be returned. </param>
        /// <param name="create"> Boolean indicating if a new state should be created if it
        /// does not yet exist. </param>
        /// <returns> Returns the state for the given cell. </returns>
        public virtual mxCellState getState(object cell, bool create)
        {
            mxCellState state = null;

            if (cell != null)
            {
                state = states.ContainsKey(cell) ? states[cell] : null;

                if (state == null && create && graph.isCellVisible(cell))
                {
                    state = createState(cell);
                    if (states.ContainsKey(cell))
                    {
                        states[cell] = state;
                    }
                    else
                    {
                        states.Add(cell, state);
                    }
                }
            }

            return state;
        }

        /// <summary>
        /// Removes and returns the mxCellState for the given cell.
        /// </summary>
        /// <param name="cell"> mxCell for which the mxCellState should be removed. </param>
        /// <returns> Returns the mxCellState that has been removed. </returns>
        public virtual mxCellState removeState(object cell)
        {
            if (cell != null)
            {
                mxCellState temp = states[cell];
                states.Remove(cell);
                return temp;


            }
            return null;
        }

        /// <summary>
        /// Creates and returns a cell state for the given cell.
        /// </summary>
        /// <param name="cell"> Cell for which a new state should be created. </param>
        /// <returns> Returns a new state for the given cell. </returns>
        public virtual mxCellState createState(object cell)
        {
            return new mxCellState(this, cell, graph.getCellStyle(cell));
        }

        /// <summary>
        /// Action to change the current root in a view.
        /// </summary>
        public class mxCurrentRootChange : mxUndoableEdit.mxUndoableChange
        {

            /// 
            protected internal mxGraphView view;

            /// 
            protected internal object root, previous;

            /// 
            protected internal bool up;

            /// <summary>
            /// Constructs a change of the current root in the given view.
            /// </summary>
            public mxCurrentRootChange(mxGraphView view, object root)
            {
                this.view = view;
                this.root = root;
                this.previous = this.root;
                this.up = (root == null);

                if (!up)
                {
                    object tmp = view.CurrentRoot;
                    mxIGraphModel model = view.graph.Model;

                    while (tmp != null)
                    {
                        if (tmp == root)
                        {
                            up = true;
                            break;
                        }

                        tmp = model.getParent(tmp);
                    }
                }
            }

            /// <summary>
            /// Returns the graph view where the change happened.
            /// </summary>
            public virtual mxGraphView View
            {
                get
                {
                    return view;
                }
            }

            /// <summary>
            /// Returns the root.
            /// </summary>
            public virtual object Root
            {
                get
                {
                    return root;
                }
            }

            /// <summary>
            /// Returns the previous root.
            /// </summary>
            public virtual object Previous
            {
                get
                {
                    return previous;
                }
            }

            /// <summary>
            /// Returns true if the drilling went upwards.
            /// </summary>
            public virtual bool Up
            {
                get
                {
                    return up;
                }
            }

            /// <summary>
            /// Changes the current root of the view.
            /// </summary>
            public virtual void execute()
            {
                object tmp = view.CurrentRoot;
                view.currentRoot = previous;
                previous = tmp;

                mxPoint translate = view.graph.getTranslateForRoot(view.CurrentRoot);

                if (translate != null)
                {
                    view.translate = new mxPoint(-translate.X, translate.Y);
                }

                // Removes all existing cell states and revalidates
                view.reload();
                up = !up;

                string eventName = (up) ? mxEvent.UP : mxEvent.DOWN;
                view.fireEvent(new mxEventObject(eventName, "root", view.currentRoot, "previous", previous));
            }

        }

    }

}