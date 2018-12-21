using mxGraph;
using System;
using System.Collections.Generic;
using System.Drawing;

/*
 * $Id: mxHierarchicalLayout.java,v 1.2 2010-12-01 17:59:41 david Exp $
 * Copyright (c) 2005-2010, David Benson, Gaudenz Alder
 */
namespace mxGraph.layout.hierarchical
{


    using mxGraphHierarchyModel = mxGraph.layout.hierarchical.model.mxGraphHierarchyModel;
    using mxCoordinateAssignment = mxGraph.layout.hierarchical.stage.mxCoordinateAssignment;
    using mxHierarchicalLayoutStage = mxGraph.layout.hierarchical.stage.mxHierarchicalLayoutStage;
    using mxMedianHybridCrossingReduction = mxGraph.layout.hierarchical.stage.mxMedianHybridCrossingReduction;
    using mxMinimumCycleRemover = mxGraph.layout.hierarchical.stage.mxMinimumCycleRemover;
    using mxIGraphModel = mxGraph.model.mxIGraphModel;
    using mxGraph = mxGraph.view.mxGraph;

    /// <summary>
    /// The top level compound layout of the hierarchical layout. The individual
    /// elements of the layout are called in sequence.
    /// </summary>
    public class mxHierarchicalLayout : mxGraphLayout /*
							        * ,
							        * JGraphLayout.Stoppable
							        */
    {
        /// <summary>
        /// The root nodes of the layout </summary>
        protected internal List<object> roots = null;

        /// <summary>
        /// Specifies if the parent should be resized after the layout so that it
        /// contains all the child cells. Default is false. @See parentBorder.
        /// </summary>
        protected internal bool resizeParent = false;

        /// <summary>
        /// Specifies if the parnent should be moved if resizeParent is enabled.
        /// Default is false. @See resizeParent.
        /// </summary>
        protected internal bool moveParent = false;

        /// <summary>
        /// The border to be added around the children if the parent is to be resized
        /// using resizeParent. Default is 0. @See resizeParent.
        /// </summary>
        protected internal int parentBorder = 0;

        /// <summary>
        /// The spacing buffer added between cells on the same layer
        /// </summary>
        protected internal double intraCellSpacing = 30.0;

        /// <summary>
        /// The spacing buffer added between cell on adjacent layers
        /// </summary>
        protected internal double interRankCellSpacing = 50.0;

        /// <summary>
        /// The spacing buffer between unconnected hierarchies
        /// </summary>
        protected internal double interHierarchySpacing = 60.0;

        /// <summary>
        /// The distance between each parallel edge on each ranks for long edges
        /// </summary>
        protected internal double parallelEdgeSpacing = 10.0;

        /// <summary>
        /// The position of the root node(s) relative to the laid out graph in.
        /// Default is <code>SwingConstants.NORTH</code>, i.e. top-down.
        /// </summary>
        protected internal int orientation = SwingConstants.NORTH;

        /// <summary>
        /// Specifies if the STYLE_NOEDGESTYLE flag should be set on edges that are
        /// modified by the result. Default is true.
        /// </summary>
        protected internal bool disableEdgeStyle = true;

        /// <summary>
        /// Whether or not to perform local optimisations and iterate multiple times
        /// through the algorithm
        /// </summary>
        protected internal bool fineTuning = true;

        /// <summary>
        /// Whether or not cells are ordered according to the order in the graph
        /// model. Defaults to false since sorting usually produces quadratic
        /// performance. Note that since mxGraph returns edges in a deterministic
        /// order, it might be that this layout is always deterministic using that
        /// JGraph regardless of this flag setting (i.e. leave it false in that
        /// case). Default is true.
        /// </summary>
        protected internal bool deterministic;

        /// <summary>
        /// Whether or not to fix the position of the root cells. Keep in mind to
        /// turn off features such as move to origin when fixing the roots, move to
        /// origin usually overrides this flag (in JGraph it does).
        /// </summary>
        protected internal bool fixRoots = false;

        /// <summary>
        /// Whether or not the initial scan of the graph to determine the layer
        /// assigned to each vertex starts from the sinks or source (the sinks being
        /// vertices with the fewest, preferable zero, outgoing edges and sources
        /// same with incoming edges). Starting from either direction can tight the
        /// layout up and also produce better results for certain types of graphs. If
        /// the result for the default is not good enough try a few sample layouts
        /// with the value false to see if they improve
        /// </summary>
        protected internal bool layoutFromSinks = true;

        /// <summary>
        /// The internal model formed of the layout
        /// </summary>
        protected internal mxGraphHierarchyModel model = null;

        /// <summary>
        /// The layout progress bar
        /// </summary>
        // protected JGraphLayoutProgress progress = new JGraphLayoutProgress();
        /// <summary>
        /// The logger for this class </summary>
        //private static Logger logger = Logger.getLogger("com.jgraph.layout.hierarchical.JGraphHierarchicalLayout");

        /// <summary>
        /// Constructs a hierarchical layout
        /// </summary>
        /// <param name="graph">
        ///            the graph to lay out
        ///  </param>
        public mxHierarchicalLayout(mxGraph graph) : this(graph, SwingConstants.NORTH)
        {
        }

        /// <summary>
        /// Constructs a hierarchical layout
        /// </summary>
        /// <param name="graph">
        ///            the graph to lay out </param>
        /// <param name="orientation">
        ///            <code>SwingConstants.NORTH, SwingConstants.EAST, SwingConstants.SOUTH</code>
        ///            or <code> SwingConstants.WEST</code>
        ///  </param>
        public mxHierarchicalLayout(mxGraph graph, int orientation) : base(graph)
        {
            this.orientation = orientation;
        }

        /// <summary>
        /// Returns the model for this layout algorithm.
        /// </summary>
        public virtual mxGraphHierarchyModel Model
        {
            get
            {
                return model;
            }
        }

        /// <summary>
        /// Executes the layout for the children of the specified parent.
        /// </summary>
        /// <param name="parent">
        ///            Parent cell that contains the children to be laid out. </param>
        public override void execute(object parent)
        {
            execute(parent, null);
        }

        /// <summary>
        /// Executes the layout for the children of the specified parent.
        /// </summary>
        /// <param name="parent">
        ///            Parent cell that contains the children to be laid out. </param>
        /// <param name="roots">
        ///            the starting roots of the layout </param>
        public virtual void execute(object parent, List<object> roots)
        {
            if (roots == null || roots.Count == 0)
            {
                roots = (List<object>)graph.findTreeRoots(parent);
            }

            this.roots = roots;
            mxIGraphModel model = graph.Model;

            model.beginUpdate();
            try
            {
                run(parent);

                if (ResizeParent && !graph.isCellCollapsed(parent))
                {
                    graph.updateGroupBounds(new object[] { parent }, ParentBorder, MoveParent);
                }
            }
            finally
            {
                model.endUpdate();
            }
        }

        /// <summary>
        /// The API method used to exercise the layout upon the graph description and
        /// produce a separate description of the vertex position and edge routing
        /// changes made.
        /// </summary>
        public virtual void run(object parent)
        {
            // Separate out unconnected hierarchies
            IList<HashSet<object>> hierarchyVertices = new List<HashSet<object>>();

            // Keep track of one root in each hierarchy in case it's fixed position
            IList<object> fixedRoots = null;
            IList<Point> rootLocations = null;
            IList<HashSet<object>> affectedEdges = null;

            if (fixRoots)
            {
                fixedRoots = new List<object>();
                rootLocations = new List<Point>();
                affectedEdges = new List<HashSet<object>>();
            }
            IEnumerator<HashSet<object>> iter;
            for (int n = 0; n < roots.Count; n++)
            {
                
                // First check if this root appears in any of the previous vertex
                // sets
                bool newHierarchy = true;
                iter = hierarchyVertices.GetEnumerator();

                while (newHierarchy && iter.MoveNext())
                {
                    if (iter.Current.Contains(roots[n]))
                    {
                        newHierarchy = false;
                    }
                }

                if (newHierarchy)
                {
                    // Obtains set of vertices connected to this root
                    Stack<object> cellsStack = new Stack<object>();
                    cellsStack.Push(roots[n]);
                    HashSet<object> edgeSet = null;

                    if (fixRoots)
                    {
                        fixedRoots.Add(roots[n]);
                        Point location = getVertexBounds(roots[n]).Point;
                        rootLocations.Add(location);
                        edgeSet = new HashSet<object>();
                    }

                    HashSet<object> vertexSet = new HashSet<object>();

                    while (cellsStack.Count > 0)
                    {
                        object cell = cellsStack.Pop();

                        if (!vertexSet.Contains(cell))
                        {
                            vertexSet.Add(cell);

                            if (fixRoots)
                            {
                                Array.ForEach(graph.getIncomingEdges(cell, parent), x => { edgeSet.Add(x); });


                            }

                            object[] conns = graph.getConnections(cell, parent);
                            object[] cells = graph.getOpposites(conns, cell);

                            for (int j = 0; j < cells.Length; j++)
                            {
                                if (!vertexSet.Contains(cells[j]))
                                {
                                    cellsStack.Push(cells[j]);
                                }
                            }
                        }
                    }

                    hierarchyVertices.Add(vertexSet);

                    if (fixRoots)
                    {
                        affectedEdges.Add(edgeSet);
                    }
                }
            }

            // Perform a layout for each seperate hierarchy
            // Track initial coordinate x-positioning
            double initialX = 0;
            iter = hierarchyVertices.GetEnumerator();
            int i = 0;

            while (iter.MoveNext())
            {
                object[] vertexSet =new object[iter.Current.Count];
                    iter.Current.CopyTo(vertexSet);
                
                model = new mxGraphHierarchyModel(this, vertexSet, roots, parent, false, deterministic, layoutFromSinks);

                cycleStage(parent);
                layeringStage();
                crossingStage(parent);
                initialX = placementStage(initialX, parent);

                if (fixRoots)
                {
                    // Reposition roots and their hierarchies using their bounds
                    // stored earlier
                    object root = fixedRoots[i];
                    Point oldLocation = rootLocations[i];
                    Point newLocation = graph.Model.getGeometry(root).Point;

                    double diffX = oldLocation.X - newLocation.X;
                    double diffY = oldLocation.Y - newLocation.Y;
                    graph.moveCells(vertexSet, diffX, diffY);

                    // Also translate connected edges
                    HashSet<object> connectedEdges = affectedEdges[i++];

                    object[] cells = new object[connectedEdges.Count];
                    connectedEdges.CopyTo(cells);

                    graph.moveCells(cells, diffX, diffY);
                }
            }
        }

        /// <summary>
        /// Executes the cycle stage. This implementation uses the
        /// mxMinimumCycleRemover.
        /// </summary>
        public virtual void cycleStage(object parent)
        {
            mxHierarchicalLayoutStage cycleStage = new mxMinimumCycleRemover(this);
            cycleStage.execute(parent);
        }

        /// <summary>
        /// Implements first stage of a Sugiyama layout.
        /// </summary>
        public virtual void layeringStage()
        {
            model.initialRank();
            model.fixRanks();
        }

        /// <summary>
        /// Executes the crossing stage using mxMedianHybridCrossingReduction.
        /// </summary>
        public virtual void crossingStage(object parent)
        {
            mxHierarchicalLayoutStage crossingStage = new mxMedianHybridCrossingReduction(this);
            crossingStage.execute(parent);
        }

        /// <summary>
        /// Executes the placement stage using mxCoordinateAssignment.
        /// </summary>
        public virtual double placementStage(double initialX, object parent)
        {
            mxCoordinateAssignment placementStage = new mxCoordinateAssignment(this, intraCellSpacing, interRankCellSpacing, orientation, initialX, parallelEdgeSpacing);
            placementStage.FineTuning = fineTuning;
            placementStage.execute(parent);

            return placementStage.LimitX + interHierarchySpacing;
        }

        /// <summary>
        /// Returns the resizeParent flag.
        /// </summary>
        public virtual bool ResizeParent
        {
            get
            {
                return resizeParent;
            }
            set
            {
                resizeParent = value;
            }
        }


        /// <summary>
        /// Returns the moveParent flag.
        /// </summary>
        public virtual bool MoveParent
        {
            get
            {
                return moveParent;
            }
            set
            {
                moveParent = value;
            }
        }


        /// <summary>
        /// Returns parentBorder.
        /// </summary>
        public virtual int ParentBorder
        {
            get
            {
                return parentBorder;
            }
            set
            {
                parentBorder = value;
            }
        }


        /// <returns> Returns the intraCellSpacing. </returns>
        public virtual double IntraCellSpacing
        {
            get
            {
                return intraCellSpacing;
            }
            set
            {
                this.intraCellSpacing = value;
            }
        }


        /// <returns> Returns the interRankCellSpacing. </returns>
        public virtual double InterRankCellSpacing
        {
            get
            {
                return interRankCellSpacing;
            }
            set
            {
                this.interRankCellSpacing = value;
            }
        }


        /// <returns> Returns the orientation. </returns>
        public virtual int Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                this.orientation = value;
            }
        }


        /// <returns> Returns the interHierarchySpacing. </returns>
        public virtual double InterHierarchySpacing
        {
            get
            {
                return interHierarchySpacing;
            }
            set
            {
                this.interHierarchySpacing = value;
            }
        }


        public virtual double ParallelEdgeSpacing
        {
            get
            {
                return parallelEdgeSpacing;
            }
            set
            {
                this.parallelEdgeSpacing = value;
            }
        }


        /// <returns> Returns the fineTuning. </returns>
        public virtual bool FineTuning
        {
            get
            {
                return fineTuning;
            }
            set
            {
                this.fineTuning = value;
            }
        }


        /// 
        public virtual bool DisableEdgeStyle
        {
            get
            {
                return disableEdgeStyle;
            }
            set
            {
                this.disableEdgeStyle = value;
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


        /// <returns> Returns the fixRoots. </returns>
        public virtual bool FixRoots
        {
            get
            {
                return fixRoots;
            }
            set
            {
                this.fixRoots = value;
            }
        }


        public virtual bool LayoutFromSinks
        {
            get
            {
                return layoutFromSinks;
            }
            set
            {
                this.layoutFromSinks = value;
            }
        }


        /// <summary>
        /// Sets the logging level of this class
        /// </summary>
        /// <param name="level">
        ///            the logging level to set </param>
        //public virtual Level LoggerLevel
        //{
        //    set
        //    {
        //        try
        //        {
        //            logger.Level = value;
        //        }
        //        catch (SecurityException)
        //        {
        //            // Probably running in an applet
        //        }
        //    }
        //}

        /// <summary>
        /// Returns <code>Hierarchical</code>, the name of this algorithm.
        /// </summary>
        public override string ToString()
        {
            return "Hierarchical";
        }

    }

}