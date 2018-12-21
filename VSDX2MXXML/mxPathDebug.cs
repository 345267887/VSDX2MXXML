using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    public class mxPathDebug
    {
        internal bool draw = false;
        internal double rectSize = 6;
        internal mxGraph graph = null;
        internal VsdxShape shape = null;
        internal double parentHeight;
        internal string rectStyle = "fillColor=#00ff00;strokeColor=#ff0000;gradientColor=none;verticalLabelPosition=top;labelPosition=center;align=center;verticalAlign=bottom;";
        internal string lineStyle = "strokeColor=#0000ff;endArrow=none;";

        public mxPathDebug(bool draw, mxGraph graph, VsdxShape shape, double parentHeight)
        {
            this.draw = draw;
            this.graph = graph;
            this.shape = shape;
            this.parentHeight = parentHeight;
        }

        public virtual void drawRect(double x, double y, string label)
        {
            mxPoint origin = shape.getOriginPoint(parentHeight, false);
            double x0 = origin.X + x - rectSize * 0.5;
            double y0 = origin.Y + y - rectSize * 0.5;
            graph.insertVertex(null, null, label, x0, y0, rectSize, rectSize, rectStyle);
        }

        public virtual void drawLine(double x0, double y0, double x1, double y1, string label)
        {
            mxPoint origin = shape.getOriginPoint(parentHeight, false);
            x0 = origin.X + x0;
            y0 = origin.Y + y0;
            x1 = origin.X + x1;
            y1 = origin.Y + y1;

            mxCell edge = (mxCell)graph.insertEdge(null, null, label, null, null, lineStyle);
            edge.Geometry.setTerminalPoint(new mxPoint(x0, y0), true);
            edge.Geometry.setTerminalPoint(new mxPoint(x1, y1), false);
        }
    }
}
