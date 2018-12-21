using System.Drawing;
using System.Drawing.Imaging;
/// <summary>
/// $Id: mxConstants.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007-2010, Gaudenz Alder, David Benson
/// </summary>
namespace mxGraph.util
{



    /// <summary>
    /// Contains all global constants.
    /// </summary>
    public class mxConstants
    {
        /// <summary>
        /// Defines the number of radians per degree.
        /// </summary>
        public static double RAD_PER_DEG = 0.0174532;

        /// <summary>
        /// Defines the number of degrees per radian.
        /// </summary>
        public static double DEG_PER_RAD = 57.2957795;

        /// <summary>
        /// Defines the minimum scale at which rounded polylines should be painted.
        /// Default is 0.05.
        /// </summary>
        public static double MIN_SCALE_FOR_ROUNDED_LINES = 0.05;

        /// <summary>
        /// Defines the portion of the cell which is to be used as a connectable
        /// region. Default is 0.3.
        /// </summary>
        public static double DEFAULT_HOTSPOT = 0.3;

        /// <summary>
        /// Defines the minimum size in pixels of the portion of the cell which is
        /// to be used as a connectable region. Default is 8.
        /// </summary>
        public static int MIN_HOTSPOT_SIZE = 8;

        /// <summary>
        /// Defines the maximum size in pixels of the portion of the cell which is
        /// to be used as a connectable region. Use 0 for no maximum. Default is 0.
        /// </summary>
        public static int MAX_HOTSPOT_SIZE = 0;

        /// <summary>
        /// Defines the SVG namespace.
        /// </summary>
        public static string NS_SVG = "http://www.w3.org/2000/svg";

        /// <summary>
        /// Defines the XHTML namespace.
        /// </summary>
        public static string NS_XHTML = "http://www.w3.org/1999/xhtml";

        /// <summary>
        /// Defines the XLink namespace.
        /// </summary>
        public static string NS_XLINK = "http://www.w3.org/1999/xlink";

        /// <summary>
        /// Contains an empty image of size 1, 1.
        /// </summary>
        public static Image EMPTY_IMAGE = new Bitmap(1, 1, PixelFormat.Format16bppRgb555);

        /// <summary>
        /// Comma separated list of default fonts for CSS properties.
        /// Default is Arial, Helvetica.
        /// </summary>
        public static string DEFAULT_FONTFAMILIES = "Arial,Helvetica";

        /// <summary>
        /// Defines the default font family. Default is "Dialog". (To be replaced
        /// with Font.DIALOG after EOL of Java 1.5.)
        /// </summary>
        public static string DEFAULT_FONTFAMILY = "Dialog";

        /// <summary>
        /// Defines the default font size. Default is 11.
        /// </summary>
        public static int DEFAULT_FONTSIZE = 11;

        /// <summary>
        /// Defines the default start size for swimlanes. Default is 40.
        /// </summary>
        public static int DEFAULT_STARTSIZE = 40;

        /// <summary>
        /// Specifies the line spacing. Default is 0.
        /// </summary>
        public static int LINESPACING = 0;

        /// <summary>
        /// Defines the inset in absolute pixels between the label bounding box and
        /// the label text. Default is 3.
        /// </summary>
        public static int LABEL_INSET = 3;

        /// <summary>
        /// Multiplier to the width that is passed into the word wrapping calculation
        /// See mxUtils.wordWrap for details
        /// </summary>
        public static double LABEL_SCALE_BUFFER = 0.9;

        /// <summary>
        /// Defines the default marker size. Default is 6.
        /// </summary>
        public static int DEFAULT_MARKERSIZE = 6;

        /// <summary>
        /// Defines the default image size. Default is 24.
        /// </summary>
        public static int DEFAULT_IMAGESIZE = 24;

        /// <summary>
        /// Defines the color to be used for shadows. Default is gray.
        /// </summary>
        public static Color SHADOW_COLOR = Color.Gray;

        /// <summary>
        /// Defines the x-offset to be used for shadows. Default is 2.
        /// </summary>
        public static int SHADOW_OFFSETX = 2;

        /// <summary>
        /// Defines the y-offset to be used for shadows. Default is 3.
        /// </summary>
        public static int SHADOW_OFFSETY = 3;

        /// <summary>
        /// Defines the color to be used to draw shadows in W3C standards. Default
        /// is gray.
        /// </summary>
        public static string W3C_SHADOWCOLOR = "gray";

        /// <summary>
        /// Defines the transformation used to draw shadows in SVG.
        /// </summary>
        public static string SVG_SHADOWTRANSFORM = "translate(2 3)";

        /// <summary>
        /// Specifies the default valid color. Default is green.
        /// </summary>
        public static Color DEFAULT_VALID_COLOR = Color.Green;

        /// <summary>
        /// Specifies the default invalid color. Default is red.
        /// </summary>
        public static Color DEFAULT_INVALID_COLOR = Color.Red;

        /// <summary>
        /// Specifies the default dash pattern, 3 pixels solid, 3 pixels clear.
        /// </summary>
        public static float[] DEFAULT_DASHED_PATTERN = new float[] { 3.0f, 3.0f };

        /// <summary>
        /// Specifies the default distance at 1.0 scale that the label curve is 
        /// created from its base curve
        /// </summary>
        public static double DEFAULT_LABEL_BUFFER = 12.0;

        /// <summary>
        /// Defines the rubberband border color. 
        /// </summary>
        public static Color RUBBERBAND_BORDERCOLOR = Color.FromArgb(51, 153, 255);

        /// <summary>
        /// Defines the rubberband fill color with an alpha of 80.
        /// </summary>
        public static Color RUBBERBAND_FILLCOLOR = Color.FromArgb(80, 51, 153, 255);

        /// <summary>
        /// Defines the handle size. Default is 7.
        /// </summary>
        public static int HANDLE_SIZE = 7;

        /// <summary>
        /// Defines the handle size. Default is 4.
        /// </summary>
        public static int LABEL_HANDLE_SIZE = 4;

        /// <summary>
        /// Defines the handle border color. Default is black.
        /// </summary>
        public static Color HANDLE_BORDERCOLOR = Color.Black;

        /// <summary>
        /// Defines the handle fill color. Default is green.
        /// </summary>
        public static Color HANDLE_FILLCOLOR = Color.Green;

        /// <summary>
        /// Defines the label handle fill color. Default is yellow.
        /// </summary>
        public static Color LABEL_HANDLE_FILLCOLOR = Color.Yellow;

        /// <summary>
        /// Defines the connect handle fill color. Default is blue.
        /// </summary>
        public static Color CONNECT_HANDLE_FILLCOLOR = Color.Blue;

        /// <summary>
        /// Defines the handle fill color for locked handles. Default is red.
        /// </summary>
        public static Color LOCKED_HANDLE_FILLCOLOR = Color.Red;

        /// <summary>
        /// Defines the default value for the connect handle. Default is false.
        /// </summary>
        public static bool CONNECT_HANDLE_ENABLED = false;

        /// <summary>
        /// Defines the connect handle size. Default is 8.
        /// </summary>
        public static int CONNECT_HANDLE_SIZE = 8;

        /// <summary>
        /// Defines the selection color for edges. Default is green.
        /// </summary>
        public static Color EDGE_SELECTION_COLOR = Color.Green;

        /// <summary>
        /// Defines the selection color for vertices. Default is green.
        /// </summary>
        public static Color VERTEX_SELECTION_COLOR = Color.Green;

        /// <summary>
        /// Defines the stroke used for painting selected edges. Default is a dashed
        /// line.
        /// </summary>
        ///public static Stroke EDGE_SELECTION_STROKE = new BasicStroke(1, BasicStroke.CAP_BUTT, BasicStroke.JOIN_MITER, 10.0f, new float[] {3, 3}, 0.0f);

        /// <summary>
        /// Defines the stroke used for painting the border of selected vertices.
        /// Default is a dashed line.
        /// </summary>
        //public static Stroke VERTEX_SELECTION_STROKE = new BasicStroke(1, BasicStroke.CAP_BUTT, BasicStroke.JOIN_MITER, 10.0f, new float[] {3, 3}, 0.0f);

        /// <summary>
        /// Defines the stroke used for painting the preview for new and existing edges
        /// that are being changed. Default is a dashed line.
        /// </summary>
        //public static Stroke PREVIEW_STROKE = new BasicStroke(1, BasicStroke.CAP_BUTT, BasicStroke.JOIN_MITER, 10.0f, new float[] {3, 3}, 0.0f);

        /// <summary>
        /// Defines the border used for painting the preview when vertices are being
        /// resized, or cells and labels are being moved.
        /// </summary>
        //public static Border PREVIEW_BORDER = new LineBorderAnonymousInnerClass(mxConstants.HANDLE_BORDERCOLOR);

        //private class LineBorderAnonymousInnerClass : LineBorder
        //{
        //	public LineBorderAnonymousInnerClass(Color HANDLE_BORDERCOLOR) : base(HANDLE_BORDERCOLOR)
        //	{
        //	}

        //		/// 
        //	private const long serialVersionUID = 1348016511717964310L;

        //	public virtual void paintBorder(Component c, Graphics g, int x, int y, int width, int height)
        //	{
        //		((Graphics2D) g).Stroke = VERTEX_SELECTION_STROKE;
        //		base.paintBorder(c, g, x, y, width, height);
        //	}
        //}



        /// <summary>
        /// Defines the length of the horizontal segment of an Entity Relation.
        /// This can be overridden using mxConstants.STYLE_SEGMENT style.
        /// Default is 30.
        /// </summary>
        public static int ENTITY_SEGMENT = 30;

        /// <summary>
        /// Defines the rounding factor for rounded rectangles in percent between
        /// 0 and 1. Values should be smaller than 0.5. Default is 0.15.
        /// </summary>
        public static double RECTANGLE_ROUNDING_FACTOR = 0.15;

        /// <summary>
        /// Defines the size of the arcs for rounded edges. Default is 10.
        /// </summary>
        public static double LINE_ARCSIZE = 10;

        /// <summary>
        /// Defines the spacing between the arrow shape and its terminals. Default
        /// is 10.
        /// </summary>
        public static int ARROW_SPACING = 10;

        /// <summary>
        /// Defines the width of the arrow shape. Default is 30.
        /// </summary>
        public static int ARROW_WIDTH = 30;

        /// <summary>
        /// Defines the size of the arrowhead in the arrow shape. Default is 30.
        /// </summary>
        public static int ARROW_SIZE = 30;

        /// <summary>
        /// Defines the value for none. Default is "none".
        /// </summary>
        public static string NONE = "none";

        /// <summary>
        /// Defines the key for the perimeter style.
        /// This is a function that defines the perimeter around a particular shape.
        /// Possible values are the functions defined in mxPerimeter that use the 
        /// <code>mxPerimeterFunction</code> interface. Alternatively, the constants
        /// in this class that start with <code>PERIMETER_</code> may be used to 
        /// access perimeter styles in <code>mxStyleRegistry</code>.
        /// </summary>
        public static string STYLE_PERIMETER = "perimeter";

        /// <summary>
        /// Defines the ID of the cell that should be used for computing the
        /// perimeter point of the source for an edge. This allows for graphically
        /// connecting to a cell while keeping the actual terminal of the edge.
        /// </summary>
        public static string STYLE_SOURCE_PORT = "sourcePort";

        /// <summary>
        /// Defines the ID of the cell that should be used for computing the
        /// perimeter point of the target for an edge. This allows for graphically
        /// connecting to a cell while keeping the actual terminal of the edge.
        /// </summary>
        public static string STYLE_TARGET_PORT = "targetPort";

        /// <summary>
        /// Defines the key for the opacity style. The type of the value is 
        /// <code>float</code> and the possible range is 0-100.
        /// </summary>
        public static string STYLE_OPACITY = "opacity";

        /// <summary>
        /// Defines the key for the text opacity style. The type of the value is 
        /// <code>float</code> and the possible range is 0-100.
        /// </summary>
        public static string STYLE_TEXT_OPACITY = "textOpacity";

        /// <summary>
        /// Defines the key for the overflow style. Possible values are "visible",
        /// "hidden" and "fill". The default value is "visible". This value
        /// specifies how overlapping vertex labels are handles. A value of
        /// "visible" will show the complete label. A value of "hidden" will clip
        /// the label so that it does not overlap the vertex bounds. A value of
        /// "fill" will use the vertex bounds for the label.
        /// </summary>
        /// <seealso cref= mxGraphview.mxGraph#isLabelClipped(Object) </seealso>
        public static string STYLE_OVERFLOW = "overflow";

        /// <summary>
        /// Defines if the connection points on either end of the edge should be
        /// computed so that the edge is vertical or horizontal if possible and
        /// if the point is not at a fixed location. Default is false. This is
        /// used in mxGraph.isOrthogonal, which also returns true if the edgeStyle
        /// of the edge is an elbow or entity.
        /// </summary>
        public static string STYLE_ORTHOGONAL = "orthogonal";

        /// <summary>
        /// Defines the key for the horizontal relative coordinate connection point
        /// of an edge with its source terminal.
        /// </summary>
        public static string STYLE_EXIT_X = "exitX";

        /// <summary>
        /// Defines the key for the vertical relative coordinate connection point
        /// of an edge with its source terminal.
        /// </summary>
        public static string STYLE_EXIT_Y = "exitY";

        /// <summary>
        /// Defines if the perimeter should be used to find the exact entry point
        /// along the perimeter of the source. Possible values are 0 (false) and
        /// 1 (true). Default is 1 (true).
        /// </summary>
        public static string STYLE_EXIT_PERIMETER = "exitPerimeter";

        /// <summary>
        /// Defines the key for the horizontal relative coordinate connection point
        /// of an edge with its target terminal.
        /// </summary>
        public static string STYLE_ENTRY_X = "entryX";

        /// <summary>
        /// Defines the key for the vertical relative coordinate connection point
        /// of an edge with its target terminal.
        /// </summary>
        public static string STYLE_ENTRY_Y = "entryY";

        /// <summary>
        /// Defines if the perimeter should be used to find the exact entry point
        /// along the perimeter of the target. Possible values are 0 (false) and
        /// 1 (true). Default is 1 (true).
        /// </summary>
        public static string STYLE_ENTRY_PERIMETER = "entryPerimeter";

        /// <summary>
        /// Defines the key for the white-space style. Possible values are "nowrap"
        /// and "wrap". The default value is "nowrap". This value specifies how
        /// white-space inside a HTML vertex label should be handled. A value of
        /// "nowrap" means the text will never wrap to the next line until a
        /// linefeed is encountered. A value of "wrap" means text will wrap when
        /// necessary.
        /// </summary>
        public static string STYLE_WHITE_SPACE = "whiteSpace";

        /// <summary>
        /// Defines the key for the rotation style. The type of the value is 
        /// <code>double</code> and the possible range is 0-360.
        /// </summary>
        public static string STYLE_ROTATION = "rotation";

        /// <summary>
        /// Defines the key for the fillColor style. The value is a string
        /// expression supported by mxUtils.parseColor.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_FILLCOLOR = "fillColor";

        /// <summary>
        /// Defines the key for the gradientColor style. The value is a string
        /// expression supported by mxUtils.parseColor. This is ignored if no fill
        /// color is defined.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_GRADIENTCOLOR = "gradientColor";

        /// <summary>
        /// Defines the key for the gradient direction. Possible values are
        /// <code>DIRECTION_EAST</code>, <code>DIRECTION_WEST</code>,
        /// <code>DIRECTION_NORTH</code> and <code>DIRECTION_SOUTH</code>. Default
        /// is <code>DIRECTION_SOUTH</code>. Generally, and by default in mxGraph,
        /// gradient painting is done from the value of <code>STYLE_FILLCOLOR</code>
        /// to the value of <code>STYLE_GRADIENTCOLOR</code>. Taking the example of
        /// <code>DIRECTION_NORTH</code>, this means <code>STYLE_FILLCOLOR</code>
        /// color at the bottom of paint pattern and
        /// <code>STYLE_GRADIENTCOLOR</code> at top, with a gradient in-between.
        /// </summary>
        public static string STYLE_GRADIENT_DIRECTION = "gradientDirection";

        /// <summary>
        /// Defines the key for the strokeColor style. The value is a string
        /// expression supported by mxUtils.parseColor.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_STROKECOLOR = "strokeColor";

        /// <summary>
        /// Defines the key for the separatorColor style. The value is a string
        /// expression supported by mxUtils.parseColor. This style is only used
        /// for SHAPE_SWIMLANE shapes.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_SEPARATORCOLOR = "separatorColor";

        /// <summary>
        /// Defines the key for the strokeWidth style. The type of the value is
        /// <code>float</code> and the possible range is any non-negative value.
        /// The value reflects the stroke width in pixels.
        /// </summary>
        public static string STYLE_STROKEWIDTH = "strokeWidth";

        /// <summary>
        /// Defines the key for the align style. Possible values are
        /// <code>ALIGN_LEFT</code>, <code>ALIGN_CENTER</code> and
        /// <code>ALIGN_RIGHT</code>. This value defines how the lines of the label
        /// are horizontally aligned. <code>ALIGN_LEFT</code> mean label text lines
        /// are aligned to left of the label bounds, <code>ALIGN_RIGHT</code> to the
        /// right of the label bounds and <code>ALIGN_CENTER</code> means the
        /// center of the text lines are aligned in the center of the label bounds.
        /// Note this value doesn't affect the positioning of the overall label
        /// bounds relative to the vertex, to move the label bounds horizontally, use
        /// <code>STYLE_LABEL_POSITION</code>.
        /// </summary>
        public static string STYLE_ALIGN = "align";

        /// <summary>
        /// Defines the key for the verticalAlign style. Possible values are
        /// <code>ALIGN_TOP</code>, <code>ALIGN_MIDDLE</code> and
        /// <code>ALIGN_BOTTOM</code>. This value defines how the lines of the label
        /// are vertically aligned. <code>ALIGN_TOP</code> means the topmost label
        /// text line is aligned against the top of the label bounds,
        /// <code>ALIGN_BOTTOM</code> means the bottom-most label text line is
        /// aligned against the bottom of the label bounds and
        /// <code>ALIGN_MIDDLE</code> means there is equal spacing between the
        /// topmost text label line and the top of the label bounds and the
        /// bottom-most text label line and the bottom of the label bounds. Note
        /// this value doesn't affect the positioning of the overall label bounds
        /// relative to the vertex, to move the label bounds vertically, use
        /// <code>STYLE_VERTICAL_LABEL_POSITION</code>.
        /// </summary>
        public static string STYLE_VERTICAL_ALIGN = "verticalAlign";

        /// <summary>
        /// Defines the key for the horizontal label position of vertices. Possible
        /// values are <code>ALIGN_LEFT</code>, <code>ALIGN_CENTER</code> and
        /// <code>ALIGN_RIGHT</code>. Default is <code>ALIGN_CENTER</code>. The
        /// label align defines the position of the label relative to the cell.
        /// <code>ALIGN_LEFT</code> means the entire label bounds is placed
        /// completely just to the left of the vertex, <code>ALIGN_RIGHT</code>
        /// means adjust to the right and <code>ALIGN_CENTER</code> means the label
        /// bounds are vertically aligned with the bounds of the vertex. Note this
        /// value doesn't affect the positioning of label within the label bounds,
        /// to move the label horizontally within the label bounds, use
        /// <code>STYLE_ALIGN</code>.
        /// </summary>
        public static string STYLE_LABEL_POSITION = "labelPosition";

        /// <summary>
        /// Defines the key for the vertical label position of vertices. Possible
        /// values are <code>ALIGN_TOP</code>, <code>ALIGN_BOTTOM</code> and
        /// <code>ALIGN_MIDDLE</code>. Default is <code>ALIGN_MIDDLE</code>. The
        /// label align defines the position of the label relative to the cell.
        /// <code>ALIGN_TOP</code> means the entire label bounds is placed
        /// completely just on the top of the vertex, <code>ALIGN_BOTTOM</code>
        /// means adjust on the bottom and <code>ALIGN_MIDDLE</code> means the label
        /// bounds are horizontally aligned with the bounds of the vertex. Note
        /// this value doesn't affect the positioning of label within the label
        /// bounds, to move the label vertically within the label bounds, use
        /// <code>STYLE_VERTICAL_ALIGN</code>.
        /// </summary>
        public static string STYLE_VERTICAL_LABEL_POSITION = "verticalLabelPosition";

        /// <summary>
        /// Defines the key for the align style. Possible values are
        /// <code>ALIGN_LEFT</code>, <code>ALIGN_CENTER</code> and
        /// <code>ALIGN_RIGHT</code>. The value defines how any image in the vertex
        /// label is aligned horizontally within the label bounds of a SHAPE_LABEL
        /// shape.
        /// </summary>
        public static string STYLE_IMAGE_ALIGN = "imageAlign";

        /// <summary>
        /// Defines the key for the verticalAlign style. Possible values are
        /// <code>ALIGN_TOP</code>, <code>ALIGN_MIDDLE</code> and
        /// <code>ALIGN_BOTTOM</code>. The value defines how any image in the vertex
        /// label is aligned vertically within the label bounds of a SHAPE_LABEL
        /// shape.
        /// </summary>
        public static string STYLE_IMAGE_VERTICAL_ALIGN = "imageVerticalAlign";

        /// <summary>
        /// Defines the key for the glass style. Possible values are 0 (disabled) and
        /// 1(enabled). The default value is 0. This is used in mxLabel.
        /// </summary>
        public static string STYLE_GLASS = "glass";

        /// <summary>
        /// Defines the key for the image style. Possible values are any image URL,
        /// the type of the value is <code>String</code>. This is the path to the
        /// image to image that is to be displayed within the label of a vertex. See
        /// mxGraphics2DCanvas.getImageForStyle, loadImage and setImageBasePath on
        /// how the image URL is resolved. Finally, mxUtils.loadImage is used for
        /// loading the image for a given URL.
        /// </summary>
        public static string STYLE_IMAGE = "image";

        /// <summary>
        /// Defines the key for the imageWidth style. The type of this value is
        /// <code>int</code>, the value is the image width in pixels and must be
        /// greated than 0.
        /// </summary>
        public static string STYLE_IMAGE_WIDTH = "imageWidth";

        /// <summary>
        /// Defines the key for the imageHeight style The type of this value is
        /// <code>int</code>, the value is the image height in pixels and must be
        /// greater than 0.
        /// </summary>
        public static string STYLE_IMAGE_HEIGHT = "imageHeight";

        /// <summary>
        /// Defines the key for the image background color. This style is only used
        /// for image shapes. Possible values are all HTML color names or HEX codes.
        /// </summary>
        public static string STYLE_IMAGE_BACKGROUND = "imageBackground";

        /// <summary>
        /// Defines the key for the image border color. This style is only used for
        /// image shapes. Possible values are all HTML color names or HEX codes.
        /// </summary>
        public static string STYLE_IMAGE_BORDER = "imageBorder";

        /// <summary>
        /// Defines the key for the noLabel style. If this is
        /// true then no label is visible for a given cell.
        /// Possible values are true or false (1 or 0).
        /// Default is false.
        /// </summary>
        public static string STYLE_NOLABEL = "noLabel";

        /// <summary>
        /// Defines the key for the noEdgeStyle style. If this is
        /// true then no edge style is applied for a given edge.
        /// Possible values are true or false (1 or 0).
        /// Default is false.
        /// </summary>
        public static string STYLE_NOEDGESTYLE = "noEdgeStyle";

        /// <summary>
        /// Defines the key for the label background color. The value is a string
        /// expression supported by mxUtils.parseColor.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_LABEL_BACKGROUNDCOLOR = "labelBackgroundColor";

        /// <summary>
        /// Defines the key for the label border color. The value is a string
        /// expression supported by mxUtils.parseColor.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_LABEL_BORDERCOLOR = "labelBorderColor";

        /// <summary>
        /// Defines the key for the indicatorShape style.
        /// Possible values are any of the SHAPE_*
        /// constants.
        /// </summary>
        public static string STYLE_INDICATOR_SHAPE = "indicatorShape";

        /// <summary>
        /// Defines the key for the indicatorImage style.
        /// Possible values are any image URL, the type of the value is
        /// <code>String</code>.
        /// </summary>
        public static string STYLE_INDICATOR_IMAGE = "indicatorImage";

        /// <summary>
        /// Defines the key for the indicatorColor style. The value is a string
        /// expression supported by mxUtils.parseColor.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_INDICATOR_COLOR = "indicatorColor";

        /// <summary>
        /// Defines the key for the indicatorGradientColor style. The value is a
        /// string expression supported by mxUtils.parseColor. This style is only
        /// supported in SHAPE_LABEL shapes.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_INDICATOR_GRADIENTCOLOR = "indicatorGradientColor";

        /// <summary>
        /// Defines the key for the indicatorSpacing style (in px).
        /// </summary>
        public static string STYLE_INDICATOR_SPACING = "indicatorSpacing";

        /// <summary>
        /// Defines the key for the indicatorWidth style (in px).
        /// </summary>
        public static string STYLE_INDICATOR_WIDTH = "indicatorWidth";

        /// <summary>
        /// Defines the key for the indicatorHeight style (in px).
        /// </summary>
        public static string STYLE_INDICATOR_HEIGHT = "indicatorHeight";

        /// <summary>
        /// Defines the key for the shadow style. The type of the value is
        /// <code>boolean</code>. This style applies to vertices and arrow style
        /// edges.
        /// </summary>
        public static string STYLE_SHADOW = "shadow";

        /// <summary>
        /// Defines the key for the segment style. The type of this value is
        /// <code>float</code> and the value represents the size of the horizontal
        /// segment of the entity relation style. Default is ENTITY_SEGMENT.
        /// </summary>
        public static string STYLE_SEGMENT = "segment";

        /// <summary>
        /// Defines the key for the endArrow style.
        /// Possible values are all constants in this
        /// class that start with ARROW_. This style is
        /// supported in the <code>mxConnector</code> shape.
        /// </summary>
        public static string STYLE_ENDARROW = "endArrow";

        /// <summary>
        /// Defines the key for the startArrow style.
        /// Possible values are all constants in this
        /// class that start with ARROW_.
        /// See STYLE_ENDARROW.
        /// This style is supported in the mxConnector shape.
        /// </summary>
        public static string STYLE_STARTARROW = "startArrow";

        /// <summary>
        /// Defines the key for the endSize style. The type of this value is 
        /// <code>float</code> and the value represents the size of the end 
        /// marker in pixels.
        /// </summary>
        public static string STYLE_ENDSIZE = "endSize";

        /// <summary>
        /// Defines the key for the startSize style. The type of this value is
        /// <code>float</code> and the value represents the size of the start marker
        /// or the size of the swimlane title region depending on the shape it is
        /// used for.
        /// </summary>
        public static string STYLE_STARTSIZE = "startSize";

        /// <summary>
        /// Defines the key for the dashed style. The type of this value is
        /// <code>boolean</code> and the value determines whether or not an edge or
        /// border is drawn with a dashed pattern along the line.
        /// </summary>
        public static string STYLE_DASHED = "dashed";

        /// <summary>
        /// Defines the key for the dashed pattern style. The type of this value
        /// is <code>float[]</code> and the value specifies the dashed pattern 
        /// to apply to edges drawn with this style.
        /// </summary>
        public static string STYLE_DASH_PATTERN = "dashPattern";

        /// <summary>
        /// Defines the key for the rounded style. The type of this value is
        /// <code>boolean</code>. For edges this determines whether or not joins
        /// between edges segments are smoothed to a rounded finish. For vertices
        /// that have the rectangle shape, this determines whether or not the
        /// rectangle is rounded.
        /// </summary>
        public static string STYLE_ROUNDED = "rounded";

        /// <summary>
        /// Defines the key for the source perimeter spacing. The type of this value
        /// is <code>double</code>. This is the distance between the source
        /// connection point of an edge and the perimeter of the source vertex in
        /// pixels. This style only applies to edges.
        /// </summary>
        public static string STYLE_SOURCE_PERIMETER_SPACING = "sourcePerimeterSpacing";

        /// <summary>
        /// Defines the key for the target perimeter spacing. The type of this value
        /// is <code>double</code>. This is the distance between the target
        /// connection point of an edge and the perimeter of the target vertex in
        /// pixels. This style only applies to edges.
        /// </summary>
        public static string STYLE_TARGET_PERIMETER_SPACING = "targetPerimeterSpacing";

        /// <summary>
        /// Defines the key for the perimeter spacing. This is the distance between
        /// the connection point and the perimeter in pixels. When used in a vertex
        /// style, this applies to all incoming edges to floating ports (edges that
        /// terminate on the perimeter of the vertex). When used in an edge style,
        /// this spacing applies to the source and target separately, if they
        /// terminate in floating ports (on the perimeter of the vertex).
        /// </summary>
        public static string STYLE_PERIMETER_SPACING = "perimeterSpacing";

        /// <summary>
        /// Defines the key for the spacing. The value represents the spacing, in
        /// pixels, added to each side of a label in a vertex (style applies to
        /// vertices only).
        /// </summary>
        public static string STYLE_SPACING = "spacing";

        /// <summary>
        /// Defines the key for the spacingTop style. The value represents the
        /// spacing, in pixels, added to the top side of a label in a vertex (style
        /// applies to vertices only).
        /// </summary>
        public static string STYLE_SPACING_TOP = "spacingTop";

        /// <summary>
        /// Defines the key for the spacingLeft style. The value represents the
        /// spacing, in pixels, added to the left side of a label in a vertex (style
        /// applies to vertices only).
        /// </summary>
        public static string STYLE_SPACING_LEFT = "spacingLeft";

        /// <summary>
        /// Defines the key for the spacingBottom style The value represents the
        /// spacing, in pixels, added to the bottom side of a label in a vertex
        /// (style applies to vertices only).
        /// </summary>
        public static string STYLE_SPACING_BOTTOM = "spacingBottom";

        /// <summary>
        /// Defines the key for the spacingRight style The value represents the
        /// spacing, in pixels, added to the right side of a label in a vertex (style
        /// applies to vertices only).
        /// </summary>
        public static string STYLE_SPACING_RIGHT = "spacingRight";

        /// <summary>
        /// Defines the key for the horizontal style. Possible values are
        /// <code>true</code> or <code>false</code>. This value only applies to
        /// vertices. If the <code>STYLE_SHAPE</code> is <code>SHAPE_SWIMLANE</code>
        /// a value of <code>false</code> indicates that the swimlane should be drawn
        /// vertically, <code>true</code> indicates to draw it horizontally. If the
        /// shape style does not indicate that this vertex is a swimlane, this value
        /// affects only whether the label is drawn horizontally or vertically.
        /// </summary>
        public static string STYLE_HORIZONTAL = "horizontal";

        /// <summary>
        /// Defines the key for the direction style. The direction style is used to
        /// specify the direction of certain shapes (eg. <code>mxTriangle</code>).
        /// Possible values are <code>DIRECTION_EAST</code> (default),
        /// <code>DIRECTION_WEST</code>, <code>DIRECTION_NORTH</code> and
        /// <code>DIRECTION_SOUTH</code>. This value only applies to vertices.
        /// </summary>
        public static string STYLE_DIRECTION = "direction";

        /// <summary>
        /// Defines the key for the elbow style. Possible values are
        /// <code>ELBOW_HORIZONTAL</code> and <code>ELBOW_VERTICAL</code>. Default is
        /// <code>ELBOW_HORIZONTAL</code>. This defines how the three segment
        /// orthogonal edge style leaves its terminal vertices. The vertical style
        /// leaves the terminal vertices at the top and bottom sides.
        /// </summary>
        public static string STYLE_ELBOW = "elbow";

        /// <summary>
        /// Defines the key for the fontColor style. The value is type
        /// <code>String</code> and of the expression supported by
        /// mxUtils.parseColor.
        /// </summary>
        /// <seealso cref= util.mxUtils#parseColor(String) </seealso>
        public static string STYLE_FONTCOLOR = "fontColor";

        /// <summary>
        /// Defines the key for the fontFamily style. Possible values are names such
        /// as Arial; Dialog; Verdana; Times New Roman. The value is of type
        /// <code>String</code>.
        /// </summary>
        public static string STYLE_FONTFAMILY = "fontFamily";

        /// <summary>
        /// Defines the key for the fontSize style (in points). The type of the value
        /// is <code>int</code>.
        /// </summary>
        public static string STYLE_FONTSIZE = "fontSize";

        /// <summary>
        /// Defines the key for the fontStyle style. Values may be any logical AND
        /// (sum) of FONT_BOLD, FONT_ITALIC, FONT_UNDERLINE and FONT_SHADOW. The type
        /// of the value is <code>int</code>.
        /// </summary>
        public static string STYLE_FONTSTYLE = "fontStyle";

        /// <summary>
        /// Defines the key for the shape style.
        /// Possible values are any of the SHAPE_*
        /// constants.
        /// </summary>
        public static string STYLE_SHAPE = "shape";

        /// <summary>
        /// Takes a function that creates points. Possible values are the
        /// functions defined in mxEdgeStyle.
        /// </summary>
        public static string STYLE_EDGE = "edgeStyle";

        /// <summary>
        /// Defines the key for the loop style. Possible values are the
        /// functions defined in mxEdgeStyle.
        /// </summary>
        public static string STYLE_LOOP = "loopStyle";

        /// <summary>
        /// Defines the key for the horizontal routing center. Possible values are
        /// between -0.5 and 0.5. This is the relative offset from the center used
        /// for connecting edges. The type of this value is <code>float</code>.
        /// </summary>
        public static string STYLE_ROUTING_CENTER_X = "routingCenterX";

        /// <summary>
        /// Defines the key for the vertical routing center. Possible values are
        /// between -0.5 and 0.5. This is the relative offset from the center used
        /// for connecting edges. The type of this value is <code>float</code>.
        /// </summary>
        public static string STYLE_ROUTING_CENTER_Y = "routingCenterY";

        /// <summary>
        /// FONT_BOLD
        /// </summary>
        public const int FONT_BOLD = 1;

        /// <summary>
        /// FONT_ITALIC
        /// </summary>
        public const int FONT_ITALIC = 2;

        /// <summary>
        /// FONT_UNDERLINE
        /// </summary>
        public const int FONT_UNDERLINE = 4;

        /// <summary>
        /// FONT_SHADOW
        /// </summary>
        public const int FONT_SHADOW = 8;

        /// <summary>
        /// SHAPE_RECTANGLE
        /// </summary>
        public const string SHAPE_RECTANGLE = "rectangle";

        /// <summary>
        /// SHAPE_ELLIPSE
        /// </summary>
        public const string SHAPE_ELLIPSE = "ellipse";

        /// <summary>
        /// SHAPE_DOUBLE_ELLIPSE
        /// </summary>
        public const string SHAPE_DOUBLE_ELLIPSE = "doubleEllipse";

        /// <summary>
        /// SHAPE_RHOMBUS
        /// </summary>
        public const string SHAPE_RHOMBUS = "rhombus";

        /// <summary>
        /// SHAPE_LINE
        /// </summary>
        public const string SHAPE_LINE = "line";

        /// <summary>
        /// SHAPE_IMAGE
        /// </summary>
        public const string SHAPE_IMAGE = "image";

        /// <summary>
        /// SHAPE_ARROW
        /// </summary>
        public const string SHAPE_ARROW = "arrow";

        /// <summary>
        /// SHAPE_ARROW
        /// </summary>
        public const string SHAPE_CURVE = "curve";

        /// <summary>
        /// SHAPE_LABEL
        /// </summary>
        public const string SHAPE_LABEL = "label";

        /// <summary>
        /// SHAPE_CYLINDER
        /// </summary>
        public const string SHAPE_CYLINDER = "cylinder";

        /// <summary>
        /// SHAPE_SWIMLANE
        /// </summary>
        public const string SHAPE_SWIMLANE = "swimlane";

        /// <summary>
        /// SHAPE_CONNECTOR
        /// </summary>
        public const string SHAPE_CONNECTOR = "connector";

        /// <summary>
        /// SHAPE_ACTOR
        /// </summary>
        public const string SHAPE_ACTOR = "actor";

        /// <summary>
        /// SHAPE_CLOUD
        /// </summary>
        public const string SHAPE_CLOUD = "cloud";

        /// <summary>
        /// SHAPE_TRIANGLE
        /// </summary>
        public const string SHAPE_TRIANGLE = "triangle";

        /// <summary>
        /// SHAPE_HEXAGON
        /// </summary>
        public const string SHAPE_HEXAGON = "hexagon";

        /// <summary>
        /// ARROW_CLASSIC
        /// </summary>
        public const string ARROW_CLASSIC = "classic";

        /// <summary>
        /// ARROW_BLOCK
        /// </summary>
        public const string ARROW_BLOCK = "block";

        /// <summary>
        /// ARROW_OPEN
        /// </summary>
        public const string ARROW_OPEN = "open";

        /// <summary>
        /// ARROW_BLOCK
        /// </summary>
        public const string ARROW_OVAL = "oval";

        /// <summary>
        /// ARROW_OPEN
        /// </summary>
        public const string ARROW_DIAMOND = "diamond";

        /// <summary>
        /// ALIGN_LEFT
        /// </summary>
        public const string ALIGN_LEFT = "left";

        /// <summary>
        /// ALIGN_CENTER
        /// </summary>
        public const string ALIGN_CENTER = "center";

        /// <summary>
        /// ALIGN_RIGHT
        /// </summary>
        public const string ALIGN_RIGHT = "right";

        /// <summary>
        /// ALIGN_TOP
        /// </summary>
        public const string ALIGN_TOP = "top";

        /// <summary>
        /// ALIGN_MIDDLE
        /// </summary>
        public const string ALIGN_MIDDLE = "middle";

        /// <summary>
        /// ALIGN_BOTTOM
        /// </summary>
        public const string ALIGN_BOTTOM = "bottom";

        /// <summary>
        /// DIRECTION_NORTH
        /// </summary>
        public const string DIRECTION_NORTH = "north";

        /// <summary>
        /// DIRECTION_SOUTH
        /// </summary>
        public const string DIRECTION_SOUTH = "south";

        /// <summary>
        /// DIRECTION_EAST
        /// </summary>
        public const string DIRECTION_EAST = "east";

        /// <summary>
        /// DIRECTION_WEST
        /// </summary>
        public const string DIRECTION_WEST = "west";

        /// <summary>
        /// ELBOW_VERTICAL
        /// </summary>
        public const string ELBOW_VERTICAL = "vertical";

        /// <summary>
        /// ELBOW_HORIZONTAL
        /// </summary>
        public const string ELBOW_HORIZONTAL = "horizontal";

        /// <summary>
        /// Name of the elbow edge style. Can be used as a string value
        /// for the STYLE_EDGE style.
        /// </summary>
        public const string EDGESTYLE_ELBOW = "elbowEdgeStyle";

        /// <summary>
        /// Name of the entity relation edge style. Can be used as a string value
        /// for the STYLE_EDGE style.
        /// </summary>
        public const string EDGESTYLE_ENTITY_RELATION = "entityRelationEdgeStyle";

        /// <summary>
        /// Name of the loop edge style. Can be used as a string value
        /// for the STYLE_EDGE style.
        /// </summary>
        public const string EDGESTYLE_LOOP = "loopEdgeStyle";

        /// <summary>
        /// Name of the side to side edge style. Can be used as a string value
        /// for the STYLE_EDGE style.
        /// </summary>
        public const string EDGESTYLE_SIDETOSIDE = "sideToSideEdgeStyle";

        /// <summary>
        /// Name of the top to bottom edge style. Can be used as a string value
        /// for the STYLE_EDGE style.
        /// </summary>
        public const string EDGESTYLE_TOPTOBOTTOM = "topToBottomEdgeStyle";

        /// <summary>
        /// Name of the ellipse perimeter. Can be used as a string value
        /// for the STYLE_PERIMETER style.
        /// </summary>
        public const string PERIMETER_ELLIPSE = "ellipsePerimeter";

        /// <summary>
        /// Name of the rectangle perimeter. Can be used as a string value
        /// for the STYLE_PERIMETER style.
        /// </summary>
        public const string PERIMETER_RECTANGLE = "rectanglePerimeter";

        /// <summary>
        /// Name of the rhombus perimeter. Can be used as a string value
        /// for the STYLE_PERIMETER style.
        /// </summary>
        public const string PERIMETER_RHOMBUS = "rhombusPerimeter";

        /// <summary>
        /// Name of the triangle perimeter. Can be used as a string value
        /// for the STYLE_PERIMETER style.
        /// </summary>
        public const string PERIMETER_TRIANGLE = "trianglePerimeter";

        /// <summary>
        /// Name of the hexagon perimeter. Can be used as a string value
        /// for the STYLE_PERIMETER style.
        /// </summary>
        public const string PERIMETER_HEXAGON = "hexagonPerimeter";


        /// <summary>
        /// Defines the key for the fill opacity style. The type of the value is 
        /// <code>float</code> and the possible range is 0-100.
        /// </summary>
        public static string STYLE_FILL_OPACITY = "fillOpacity";

        /// <summary>
        /// Defines the key for the stroke opacity style. The type of the value is
        /// <code>float</code> and the possible range is 0-100.
        /// </summary>
        public static string STYLE_STROKE_OPACITY = "strokeOpacity";

        /// <summary>
        /// Defines the key for the horizontal image flip. This style is only used
        /// in <mxImageShape>. Possible values are 0 and 1. Default is 0.
        /// </summary>
        public static string STYLE_FLIPH = "flipH";

        /// <summary>
        /// Variable: STYLE_FLIPV
        /// 
        /// Defines the key for the vertical flip. Possible values are 0 and 1.
        /// Default is 0.
        /// </summary>
        public static string STYLE_FLIPV = "flipV";

        /// <summary>
        /// Defines the key for the startFill style. Use 0 for no fill or 1
        /// (default) for fill. (This style is only exported via <mxImageExport>.)
        /// </summary>
        public static string STYLE_STARTFILL = "startFill";


        /// <summary>
        /// Defines the key for the endFill style. Use 0 for no fill or 1
        /// (default) for fill. (This style is only exported via <mxImageExport>.)
        /// </summary>
        public static string STYLE_ENDFILL = "endFill";

        /// <summary>
        ///  Defines the key for the editable style. This specifies if the value of
        /// a cell can be edited using the in-place editor.Possible values are 0 or
        /// </summary>
        public static string STYLE_EDITABLE = "editable";

        /// <summary>
        /// Defines the key for the movable style. This specifies if a cell can
        /// be moved. Possible values are 0 or 1. Default is 1. See
        /// mxGraph.isCellMovable.
        /// </summary>
        public static string STYLE_MOVABLE = "movable";


        /// <summary>
        /// Defines the key for the bendable style. This specifies if the control
        /// points of an edge can be moved. Possible values are 0 or 1. Default is
        /// 1. See mxGraph.isCellBendable.
        /// 
        /// </summary>
        public static string STYLE_BENDABLE = "bendable";


        /// <summary>
        /// Defines the key for the resizable style. This specifies if a cell can
        /// be resized. Possible values are 0 or 1. Default is 1. See
        /// mxGraph.isCellResizable.
        /// 
        /// </summary>
        public static string STYLE_RESIZABLE = "resizable";

        /// <summary>
        /// 
        /// Defines the key for the deletable style. This specifies if a cell can be
        /// deleted. Possible values are 0 or 1. Default is 1. See
        /// mxGraph.isCellDeletable.
        /// </summary>
        public static string STYLE_DELETABLE = "deletable";

        /// <summary>
        /// Defines the key for the cloneable style. This specifies if a cell can
        /// be cloned. Possible values are 0 or 1. Default is 1. See
        /// mxGraph.isCellCloneable.
        /// </summary>
        public static string STYLE_CLONEABLE = "cloneable";/// <summary>
		/// Defines the key for the autosize style. This specifies if a cell should be
		/// resized automatically if the value has changed. Possible values are 0 or 1.
		/// Default is 0. See mxGraph.isAutoSizeCell. This is normally combined with
		/// STYLE_RESIZABLE to disable manual sizing.
		/// </summary>
		public static string STYLE_AUTOSIZE = "autosize";

        /// <summary>
        /// Defines the key for the foldable style. This specifies if a cell is foldable
        /// using a folding icon. Possible values are 0 or 1. Default is 1. See
        /// mxGraph.isCellFoldable.
        /// </summary>
        public static string STYLE_FOLDABLE = "foldable";
    }

}