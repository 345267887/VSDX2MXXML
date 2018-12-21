using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    public static class PointHelper
    {
        public static double Distance(this Point p1, Point p2)
        {
            return Math.Sqrt((Math.Pow(p1.X - p2.X, 2)
                    + Math.Pow(p2.Y - p1.Y, 2)));

        }


        public static double Distance(double x1,double y1,double x2,double y2)
        {
            return Math.Sqrt((Math.Pow(x1 - x2, 2)
                    + Math.Pow(y2 - y1, 2)));
        }

        public static int RelativeCCW(double x1,double y1,
            double x2,double y2,
            double px,double py)
        {
            x2 -= x1;
            y2 -= y1;
            px -= x1;
            py -= y1;

            double ccw = px * y2 - py * x2;
            if (ccw>0.0)
            {
                px -= x2;
                py -= y2;
                ccw = px * x2 + py * y2;
                if (ccw<0.0)
                {
                    ccw = 0.0;
                }
            }

            return (ccw < 0.0) ? -1 : ((ccw > 0.0) ? 1 : 0);
        }


        public static double ptSegDistSq(double x1, double y1,
            double x2, double y2,
            double px, double py)
        {
            x2 -= x1;
            y2 -= y1;

            px -= x1;
            py -= x1;

            double dotprod = px * x2 + py * y2;
            double projlenSq;
            if (dotprod<=0.0)
            {
                projlenSq = 0.0;
            }
            else
            {
                px = x2 - px;
                py = y2 - py;
                dotprod = px * x2 + py * y2;
                if (dotprod<=0.0)
                {
                    projlenSq = 0.0;
                }
                else
                {
                    projlenSq = dotprod * dotprod / (x2 * x2 + y2 * y2);
                }
            }

            double lenSql = px * px + py * py-projlenSq;
            if (lenSql<0)
            {
                lenSql = 0;
            }

            return lenSql;
        }

        public static double ptLineDistSq(double x1, double y1,
                                      double x2, double y2,
                                      double px, double py)
        {
            // Adjust vectors relative to x1,y1
            // x2,y2 becomes relative vector from x1,y1 to end of segment
            x2 -= x1;
            y2 -= y1;
            // px,py becomes relative vector from x1,y1 to test point
            px -= x1;
            py -= y1;
            double dotprod = px * x2 + py * y2;
            // dotprod is the length of the px,py vector
            // projected on the x1,y1=>x2,y2 vector times the
            // length of the x1,y1=>x2,y2 vector
            double projlenSq = dotprod * dotprod / (x2 * x2 + y2 * y2);
            // Distance to line is now the length of the relative point
            // vector minus the length of its projection onto the line
            double lenSq = px * px + py * py - projlenSq;
            if (lenSq < 0)
            {
                lenSq = 0;
            }
            return lenSq;
        }

        public static double ptLineDist(double x1, double y1,
                                double x2, double y2,
                                double px, double py)
        {
            return Math.Sqrt(ptLineDistSq(x1, y1, x2, y2, px, py));
        }
    }
}
