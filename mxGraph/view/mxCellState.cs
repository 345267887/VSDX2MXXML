using System;
using System.Collections.Generic;

/// <summary>
/// $Id: mxCellState.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.view
{


	using mxPoint = util.mxPoint;
	using mxRectangle = util.mxRectangle;

	/// <summary>
	/// Represents the current state of a cell in a given graph view.
	/// </summary>
	[Serializable]
	public class mxCellState : mxRectangle
	{
		/// 
		private const long serialVersionUID = 7588335615324083354L;

		/// <summary>
		/// Reference to the enclosing graph view.
		/// </summary>
		protected internal mxGraphView view;

		/// <summary>
		/// Reference to the cell that is represented by this state.
		/// </summary>
		protected internal object cell;

		/// <summary>
		/// Contains an array of key, value pairs that represent the style of the
		/// cell.
		/// </summary>
		protected internal IDictionary<string, object> style;

		/// <summary>
		/// Holds the origin for all child cells.
		/// </summary>
		protected internal mxPoint origin = new mxPoint();

		/// <summary>
		/// List of mxPoints that represent the absolute points of an edge.
		/// </summary>
		protected internal IList<mxPoint> absolutePoints;

		/// <summary>
		/// Holds the absolute offset. For edges, this is the absolute coordinates
		/// of the label position. For vertices, this is the offset of the label
		/// relative to the top, left corner of the vertex.
		/// </summary>
		protected internal mxPoint absoluteOffset = new mxPoint();

		/// <summary>
		/// Caches the distance between the end points and the length of an edge.
		/// </summary>
		protected internal double terminalDistance, length;

		/// <summary>
		/// Array of numbers that represent the cached length of each segment of the
		/// edge.
		/// </summary>
		protected internal double[] segments;

		/// <summary>
		/// Holds the rectangle which contains the label.
		/// </summary>
		protected internal mxRectangle labelBounds;

		/// <summary>
		/// Holds the largest rectangle which contains all rendering for this cell.
		/// </summary>
		protected internal mxRectangle boundingBox;

		/// <summary>
		/// Specifies if the state is invalid. Default is true.
		/// </summary>
		protected internal bool invalid = true;

        /**
	 * Caches the visible source and target terminal states.
	 */
        protected mxCellState visibleSourceState, visibleTargetState;

        /// <summary>
        /// Constructs an empty cell state.
        /// </summary>
        public mxCellState() : this(null, null, null)
		{
		}

		/// <summary>
		/// Constructs a new object that represents the current state of the given
		/// cell in the specified view.
		/// </summary>
		/// <param name="view"> Graph view that contains the state. </param>
		/// <param name="cell"> Cell that this state represents. </param>
		/// <param name="style"> Array of key, value pairs that constitute the style. </param>
		public mxCellState(mxGraphView view, object cell, IDictionary<string, object> style)
		{
			View = view;
			Cell = cell;
			Style = style;
		}

		/// <summary>
		/// Returns true if the state is invalid.
		/// </summary>
		public virtual bool Invalid
		{
			get
			{
				return invalid;
			}
			set
			{
				this.invalid = value;
			}
		}


		/// <summary>
		/// Returns the enclosing graph view.
		/// </summary>
		/// <returns> the view </returns>
		public virtual mxGraphView View
		{
			get
			{
				return view;
			}
			set
			{
				this.view = value;
			}
		}


		/// <summary>
		/// Returns the cell that is represented by this state.
		/// </summary>
		/// <returns> the cell </returns>
		public virtual object Cell
		{
			get
			{
				return cell;
			}
			set
			{
				this.cell = value;
			}
		}


		/// <summary>
		/// Returns the cell style as a map of key, value pairs.
		/// </summary>
		/// <returns> the style </returns>
		public virtual IDictionary<string, object> Style
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


		/// <summary>
		/// Returns the origin for the children.
		/// </summary>
		/// <returns> the origin </returns>
		public virtual mxPoint Origin
		{
			get
			{
				return origin;
			}
			set
			{
				this.origin = value;
			}
		}


		/// <summary>
		/// Returns the absolute point at the given index.
		/// </summary>
		/// <returns> the mxPoint at the given index </returns>
		public virtual mxPoint getAbsolutePoint(int index)
		{
			return absolutePoints[index];
		}

		/// <summary>
		/// Returns the absolute point at the given index.
		/// </summary>
		/// <returns> the mxPoint at the given index </returns>
		public virtual mxPoint setAbsolutePoint(int index, mxPoint point)
		{
			return absolutePoints[index] = point;
		}

		/// <summary>
		/// Returns the number of absolute points.
		/// </summary>
		/// <returns> the absolutePoints </returns>
		public virtual int AbsolutePointCount
		{
			get
			{
				return (absolutePoints != null) ? absolutePoints.Count : 0;
			}
		}

		/// <summary>
		/// Returns the absolute points.
		/// </summary>
		/// <returns> the absolutePoints </returns>
		public virtual IList<mxPoint> AbsolutePoints
		{
			get
			{
				return absolutePoints;
			}
			set
			{
				this.absolutePoints = value;
			}
		}


		/// <summary>
		/// Returns the absolute offset.
		/// </summary>
		/// <returns> the absoluteOffset </returns>
		public virtual mxPoint AbsoluteOffset
		{
			get
			{
				return absoluteOffset;
			}
			set
			{
				this.absoluteOffset = value;
			}
		}


		/// <summary>
		/// Returns the terminal distance.
		/// </summary>
		/// <returns> the terminalDistance </returns>
		public virtual double TerminalDistance
		{
			get
			{
				return terminalDistance;
			}
			set
			{
				this.terminalDistance = value;
			}
		}


		/// <summary>
		/// Returns the length.
		/// </summary>
		/// <returns> the length </returns>
		public virtual double Length
		{
			get
			{
				return length;
			}
			set
			{
				this.length = value;
			}
		}


		/// <summary>
		/// Returns the length of the segments.
		/// </summary>
		/// <returns> the segments </returns>
		public virtual double[] Segments
		{
			get
			{
				return segments;
			}
			set
			{
				this.segments = value;
			}
		}


		/// <summary>
		/// Returns the label bounds.
		/// </summary>
		/// <returns> Returns the label bounds for this state. </returns>
		public virtual mxRectangle LabelBounds
		{
			get
			{
				return labelBounds;
			}
			set
			{
				this.labelBounds = value;
			}
		}


		/// <summary>
		/// Returns the bounding box.
		/// </summary>
		/// <returns> Returns the bounding box for this state. </returns>
		public virtual mxRectangle BoundingBox
		{
			get
			{
				return boundingBox;
			}
			set
			{
				this.boundingBox = value;
			}
		}


		/// <summary>
		/// Returns the rectangle that should be used as the perimeter of the cell.
		/// This implementation adds the perimeter spacing to the rectangle
		/// defined by this cell state.
		/// </summary>
		/// <returns> Returns the rectangle that defines the perimeter. </returns>
		public virtual mxRectangle PerimeterBounds
		{
			get
			{
				return getPerimeterBounds(0);
			}
		}

		/// <summary>
		/// Returns the rectangle that should be used as the perimeter of the cell.
		/// </summary>
		/// <returns> Returns the rectangle that defines the perimeter. </returns>
		public virtual mxRectangle getPerimeterBounds(double border)
		{
			mxRectangle bounds = new mxRectangle(Rectangle);

			if (border != 0)
			{
				bounds.grow(border);
			}

			return bounds;
		}

		/// <summary>
		/// Sets the first or last point in the list of points depending on isSource.
		/// </summary>
		/// <param name="point"> Point that represents the terminal point. </param>
		/// <param name="isSource"> Boolean that specifies if the first or last point should
		/// be assigned. </param>
		public virtual void setAbsoluteTerminalPoint(mxPoint point, bool isSource)
		{
			if (isSource)
			{
				if (absolutePoints == null)
				{
					absolutePoints = new List<mxPoint>();
				}

				if (absolutePoints.Count == 0)
				{
					absolutePoints.Add(point);
				}
				else
				{
					absolutePoints[0] = point;
				}
			}
			else
			{
				if (absolutePoints == null)
				{
					absolutePoints = new List<mxPoint>();
					absolutePoints.Add(null);
					absolutePoints.Add(point);
				}
				else if (absolutePoints.Count == 1)
				{
					absolutePoints.Add(point);
				}
				else
				{
					absolutePoints[absolutePoints.Count - 1] = point;
				}
			}
		}

		/// <summary>
		/// Returns a clone of this state where all members are deeply cloned
		/// except the view and cell references, which are copied with no
		/// cloning to the new instance.
		/// </summary>
		public override object clone()
		{
			mxCellState clone = new mxCellState(view, cell, style);

			if (absolutePoints != null)
			{
				clone.absolutePoints = new List<mxPoint>();

				for (int i = 0; i < absolutePoints.Count; i++)
				{
					clone.absolutePoints.Add((mxPoint) absolutePoints[i].clone());
				}
			}

			if (origin != null)
			{
				clone.origin = (mxPoint) origin.clone();
			}

			if (absoluteOffset != null)
			{
				clone.absoluteOffset = (mxPoint) absoluteOffset.clone();
			}

			if (labelBounds != null)
			{
				clone.labelBounds = (mxRectangle) labelBounds.clone();
			}

			if (boundingBox != null)
			{
				clone.boundingBox = (mxRectangle) boundingBox.clone();
			}

			clone.terminalDistance = terminalDistance;
			clone.segments = segments;
			clone.length = length;
			clone.x = x;
			clone.y = y;
			clone.width = width;
			clone.height = height;

			return clone;
		}

        /**
	 * Returns the visible source or target terminal cell.
	 * 
	 * @param source Boolean that specifies if the source or target cell should be
	 * returned.
	 */
        public Object getVisibleTerminal(bool source)
        {
            mxCellState tmp = getVisibleTerminalState(source);

            return (tmp != null) ? tmp.Cell : null;
        }


        /**
	 * Sets the visible source or target terminal state.
	 * 
	 * @param terminalState Cell state that represents the terminal.
	 * @param source Boolean that specifies if the source or target state should be set.
	 */
        public void setVisibleTerminalState(mxCellState terminalState,
                bool source)
        {
            if (source)
            {
                visibleSourceState = terminalState;
            }
            else
            {
                visibleTargetState = terminalState;
            }
        }

        /**
         * Returns the visible source or target terminal state.
         * 
         * @param Boolean that specifies if the source or target state should be
         * returned.
         */
        public mxCellState getVisibleTerminalState(bool source)
        {
            return (source) ? visibleSourceState : visibleTargetState;
        }

    }

}