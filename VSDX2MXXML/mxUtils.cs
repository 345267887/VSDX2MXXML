using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    class mxUtils
    {
        /**
	 * Returns the intersection of two lines as an mxPoint.
	 * 
	 * @param x0
	 *            X-coordinate of the first line's startpoint.
	 * @param y0
	 *            Y-coordinate of the first line's startpoint.
	 * @param x1
	 *            X-coordinate of the first line's endpoint.
	 * @param y1
	 *            Y-coordinate of the first line's endpoint.
	 * @param x2
	 *            X-coordinate of the second line's startpoint.
	 * @param y2
	 *            Y-coordinate of the second line's startpoint.
	 * @param x3
	 *            X-coordinate of the second line's endpoint.
	 * @param y3
	 *            Y-coordinate of the second line's endpoint.
	 * @return Returns the intersection between the two lines.
	 */
        public static mxPoint intersection(double x0, double y0, double x1,
                double y1, double x2, double y2, double x3, double y3)
        {
            double denom = ((y3 - y2) * (x1 - x0)) - ((x3 - x2) * (y1 - y0));
            double nume_a = ((x3 - x2) * (y0 - y2)) - ((y3 - y2) * (x0 - x2));
            double nume_b = ((x1 - x0) * (y0 - y2)) - ((y1 - y0) * (x0 - x2));

            double ua = nume_a / denom;
            double ub = nume_b / denom;

            if (ua >= 0.0 && ua <= 1.0 && ub >= 0.0 && ub <= 1.0)
            {
                // Get the intersection point
                double intersectionX = x0 + ua * (x1 - x0);
                double intersectionY = y0 + ua * (y1 - y0);

                return new mxPoint(intersectionX, intersectionY);
            }

            // No intersection
            return null;
        }
    }
}
