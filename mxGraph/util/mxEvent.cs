/// <summary>
/// $Id: mxEvent.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.util
{

	/// <summary>
	/// Contains all global constants.
	/// </summary>
	public class mxEvent
	{

		/// 
		public const string DONE = "done";

		/// 
		public const string ADD_CELLS = "addCells";

		/// 
		public const string CELLS_ADDED = "cellsAdded";

		/// 
		public const string ALIGN_CELLS = "alignCells";

		/// 
		public const string CONNECT_CELL = "connectCell";

		/// 
		public const string CELL_CONNECTED = "cellConnected";

		/// 
		public const string FLIP_EDGE = "flipEdge";

		/// 
		public const string FOLD_CELLS = "foldCells";

		/// 
		public const string CELLS_FOLDED = "cellsFolded";

		/// 
		public const string GROUP_CELLS = "groupCells";

		/// 
		public const string UNGROUP_CELLS = "ungroupCells";

		/// 
		public const string REMOVE_CELLS_FROM_PARENT = "removeCellsFromParent";

		/// 
		public const string MOVE_CELLS = "moveCells";

		/// 
		public const string CELLS_MOVED = "cellsMoved";

		/// 
		public const string ORDER_CELLS = "orderCells";

		/// 
		public const string CELLS_ORDERED = "cellsOrdered";

		/// 
		public const string REMOVE_CELLS = "removeCells";

		/// 
		public const string CELLS_REMOVED = "cellsRemoved";

		/// 
		public const string REPAINT = "repaint";

		/// 
		public const string RESIZE_CELLS = "resizeCells";

		/// 
		public const string CELLS_RESIZED = "cellsResized";

		/// 
		public const string SPLIT_EDGE = "splitEdge";

		/// 
		public const string TOGGLE_CELLS = "toggleCells";

		/// 
		public const string CELLS_TOGGLED = "cellsToggled";

		/// 
		public const string UPDATE_CELL_SIZE = "updateCellSize";

		/// 
		public const string LABEL_CHANGED = "labelChanged";

		/// 
		public const string ADD_OVERLAY = "addOverlay";

		/// 
		public const string REMOVE_OVERLAY = "removeOverlay";

		/// 
		public const string BEFORE_PAINT = "beforePaint";

		/// 
		public const string PAINT = "paint";

		/// 
		public const string AFTER_PAINT = "afterPaint";

		/// 
		public const string START_EDITING = "startEditing";

		/// 
		public const string UNDO = "undo";

		/// 
		public const string REDO = "redo";

		/// 
		public const string UP = "up";

		/// 
		public const string DOWN = "down";

		/// 
		public const string SCALE = "scale";

		/// 
		public const string TRANSLATE = "translate";

		/// 
		public const string SCALE_AND_TRANSLATE = "scaleAndTranslate";

		/// <summary>
		/// Holds the name for the change event. First and only argument in the
		/// argument array is the list of mxAtomicGraphChanges that have been
		/// executed on the model.
		/// </summary>
		public const string CHANGE = "change";

		/// <summary>
		/// Holds the name for the execute event. First and only argument in the
		/// argument array is the mxAtomicGraphChange that has been executed on the 
		/// model. This event fires before the change event.
		/// </summary>
		public const string EXECUTE = "execute";

		/// <summary>
		/// Holds the name for the beforeUndo event. First and only argument in the
		/// argument array is the current edit that is currently in progress in the 
		/// model. This event fires before notify is called on the currentEdit in
		/// the model.
		/// </summary>
		public const string BEFORE_UNDO = "beforeUndo";

		/// <summary>
		/// Holds the name for the norify event. First and only argument in the
		/// argument array is the list of mxAtomicGraphChanges that have been
		/// executed on the model. This event fires after the change event.
		/// </summary>
		public const string NOTIFY = "notify";

		/// <summary>
		/// Holds the name for the beginUpdate event. This event has no arguments and
		/// fires after the updateLevel has been changed in model.
		/// </summary>
		public const string BEGIN_UPDATE = "beginUpdate";

		/// <summary>
		/// Holds the name for the endUpdate event. This event has no arguments and fires
		/// after the updateLevel has been changed in the model. First argument is the
		/// currentEdit.
		/// </summary>
		public const string END_UPDATE = "endUpdate";

		/// 
		public const string INSERT = "insert";

		/// 
		public const string ADD = "add";

		/// 
		public const string CLEAR = "clear";

		/// 
		public const string FIRED = "fired";

		/// 
		public const string SELECT = "select";

		/// <summary>
		/// Holds the name for the mark event, which fires after a cell has been
		/// marked. First and only argument in the array is the cell state that has
		/// been marked or null, if no state has been marked.
		/// 
		/// To add a mark listener to the cell marker:
		/// 
		/// <code>
		/// addListener(
		///   mxEvent.MARK, new mxEventListener()
		///   {
		///     public void invoke(Object source, Object[] args)
		///     {
		///       cellMarked((mxCellMarker) source, (mxCellState) args[0]);
		///     }
		///   });
		/// </code>
		/// </summary>
		public static string MARK = "mark";

		/// 
		public static string ROOT = "root";

		/// 
		public static string LAYOUT_CELLS = "layoutCells";

		/// 
		public static string START = "start";

		/// 
		public static string CONTINUE = "continue";

		/// 
		public static string STOP = "stop";

	}

}