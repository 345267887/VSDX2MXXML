using System.Collections.Generic;

namespace mxGraph.view
{


    using model = model.mxGraphModel;
    using mxIGraphModel = model.mxIGraphModel;
    using mxConstants = util.mxConstants;
    using mxEventObject = util.mxEventObject;
    using mxRectangle = util.mxRectangle;
    using mxUndoableChange = util.mxUndoableEdit.mxUndoableChange;

    public class mxGraphHeadless : mxGraph
    {
        /// <summary>
        /// Constructs a new graph with an empty
        /// <seealso cref="model.model"/>.
        /// </summary>
        public mxGraphHeadless() : this(null, null)
        {
        }

        /// <summary>
        /// Constructs a new graph for the specified model. If no model is
        /// specified, then a new, empty <seealso cref="model.model"/> is
        /// used.
        /// </summary>
        /// <param name="model"> Model that contains the graph data </param>
        public mxGraphHeadless(mxIGraphModel model) : this(model, null)
        {
        }

        /// <summary>
        /// Constructs a new graph for the specified model. If no model is
        /// specified, then a new, empty <seealso cref="model.model"/> is
        /// used.
        /// </summary>
        /// <param name="stylesheet"> The stylesheet to use for the graph. </param>
        public mxGraphHeadless(mxStylesheet stylesheet) : this(null, stylesheet)
        {
        }

        /// <summary>
        /// Constructs a new graph for the specified model. If no model is
        /// specified, then a new, empty <seealso cref="model.model"/> is
        /// used.
        /// </summary>
        /// <param name="model"> Model that contains the graph data </param>
        public mxGraphHeadless(mxIGraphModel model, mxStylesheet stylesheet)
        {
            Model = (model != null) ? model : new model();
        }

        /// <summary>
        /// Constructs a new selection model to be used in this graph.
        /// </summary>
        protected internal new mxGraphSelectionModel createSelectionModel()
        {
            return null;
        }

        /// <summary>
        /// Constructs a new stylesheet to be used in this graph.
        /// </summary>
        protected internal new mxStylesheet createStylesheet()
        {
            return null;
        }

        /// <summary>
        /// Returns an array of key, value pairs representing the cell style for the
        /// given cell. If no string is defined in the model that specifies the
        /// style, then the default style for the cell is returned or <EMPTY_ARRAY>,
        /// if not style can be found.
        /// </summary>
        /// <param name="cell"> Cell whose style should be returned. </param>
        /// <returns> Returns the style of the cell. </returns>
        public new IDictionary<string, object> getCellStyle(object cell)
        {
            return mxStylesheet.EMPTY_STYLE;
        }

        /// <summary>
        /// Called when the graph model changes. Invokes processChange on each
        /// item of the given array to update the view accordingly.
        /// 
        /// Overriden to remove validation of view
        /// </summary>
        public new mxRectangle graphModelChanged(mxIGraphModel sender, IList<mxUndoableChange> changes)
        {
            return null;
        }

        /// <summary>
        /// Dispatches the given event name with this object as the event source.
        /// <code>fireEvent(new mxEventObject("eventName", key1, val1, .., keyN, valN))</code>
        /// 
        /// </summary>
        public new void fireEvent(mxEventObject evt)
        {
        }

        /// <summary>
        /// Returns true if the given cell is a swimlane. This implementation always
        /// returns false.
        /// </summary>
        /// <param name="cell"> Cell that should be checked. </param>
        /// <returns> Returns true if the cell is a swimlane. </returns>
        public new bool isSwimlane(object cell)
        {
            if (cell != null)
            {
                if (model.getParent(cell) != model.Root)
                {
                    mxCellState state = view.getState(cell);
                    IDictionary<string, object> style = (state != null) ? state.Style : getCellStyle(cell);

                    if (style != null && !model.isEdge(cell))
                    {
                        return getString(style, mxConstants.STYLE_SHAPE, "").Equals(mxConstants.SHAPE_SWIMLANE);
                    }
                }
            }

            return false;
        }

        public virtual string getString(IDictionary<string, object> dict, string key, string defaultValue)
        {
            object value = dict[key];

            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
