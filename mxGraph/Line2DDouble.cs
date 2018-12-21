using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    [Serializable]
    public class Line2DDouble
    {
        /// <summary>
		/// The X coordinate of the start point of the line segment.
		/// @since 1.2
		/// @serial
		/// </summary>
		public double x1;

        /// <summary>
        /// The Y coordinate of the start point of the line segment.
        /// @since 1.2
        /// @serial
        /// </summary>
        public double y1;

        /// <summary>
        /// The X coordinate of the end point of the line segment.
        /// @since 1.2
        /// @serial
        /// </summary>
        public double x2;

        /// <summary>
        /// The Y coordinate of the end point of the line segment.
        /// @since 1.2
        /// @serial
        /// </summary>
        public double y2;

        /// <summary>
        /// Constructs and initializes a Line with coordinates (0, 0) -> (0, 0).
        /// @since 1.2
        /// </summary>
        public Line2DDouble()
        {
        }

        /// <summary>
        /// Constructs and initializes a <code>Line2D</code> from the
        /// specified coordinates. </summary>
        /// <param name="x1"> the X coordinate of the start point </param>
        /// <param name="y1"> the Y coordinate of the start point </param>
        /// <param name="x2"> the X coordinate of the end point </param>
        /// <param name="y2"> the Y coordinate of the end point
        /// @since 1.2 </param>
        public Line2DDouble(double x1, double y1, double x2, double y2)
        {
            setLine(x1, y1, x2, y2);
        }

        /// <summary>
        /// Constructs and initializes a <code>Line2D</code> from the
        /// specified <code>Point2D</code> objects. </summary>
        /// <param name="p1"> the start <code>Point2D</code> of this line segment </param>
        /// <param name="p2"> the end <code>Point2D</code> of this line segment
        /// @since 1.2 </param>
        public Line2DDouble(Point p1, Point p2)
        {
            setLine(p1, p2);
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual double X1
        {
            get
            {
                return x1;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual double Y1
        {
            get
            {
                return y1;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual Point P1
        {
            get
            {
                return new Point((int)x1,(int) y1);
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual double X2
        {
            get
            {
                return x2;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual double Y2
        {
            get
            {
                return y2;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual Point P2
        {
            get
            {
                return new Point((int)x2, y2);
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual void setLine(double x1, double y1, double x2, double y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public virtual Rectangle2D Bounds2D
        {
            get
            {
                double x, y, w, h;
                if (x1 < x2)
                {
                    x = x1;
                    w = x2 - x1;
                }
                else
                {
                    x = x2;
                    w = x1 - x2;
                }
                if (y1 < y2)
                {
                    y = y1;
                    h = y2 - y1;
                }
                else
                {
                    y = y2;
                    h = y1 - y2;
                }
                return new Rectangle2D.Double(x, y, w, h);
            }
        }

        /*
		 * JDK 1.6 serialVersionUID
		 */
        private const long serialVersionUID = 7979627399746467499L;

    }
}
