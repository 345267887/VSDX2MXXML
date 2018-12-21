using System.Collections.Generic;

namespace mxGraph.io.vsdx
{


	/// <summary>
	/// This class contains constants used in the Import of .vdx documents.
	/// </summary>
	public class mxVsdxConstants
	{
		public static string ANGLE = "Angle";
		public static string ARC_TO = "ArcTo";
		public static string BACKGROUND = "Background";
		public static string BACK_PAGE = "BackPage";
		public static string BEGIN_ARROW = "BeginArrow";
		public static string BEGIN_ARROW_SIZE = "BeginArrowSize";
		public static string BEGIN_X = "BeginX";
		public static string BEGIN_Y = "BeginY";
		public static string BOTTOM_MARGIN = "BottomMargin";
		public static string BULLET = "Bullet";
		public static string CASE = "Case";
		public static string CHARACTER = "Character";
		public static string COLOR = "Color";
		public static string COLOR_ENTRY = "ColorEntry";
		public static string COLORS = "Colors";

		/// <summary>
		/// Specifies the color transparency used for characters in a text run.
		/// The value is normalized such that a value of 1 corresponds to 100 percent.
		/// A value of zero specifies that the color is completely opaque;
		/// a value of one specifies that the color is completely transparent.
		/// </summary>
		public static string COLOR_TRANS = "ColorTrans";
		public static string CONNECT = "Connect";
		public static string CONNECTS = "Connects";
		public static string CONNECTION = "Connection";
		public static string CONTROL = "Control";
		public static string DELETED = "Del";
		public static string DOCUMENT_SHEET = "DocumentSheet";
		public static string ELLIPSE = "Ellipse";
		public static string ELLIPTICAL_ARC_TO = "EllipticalArcTo";
		public static string END_ARROW = "EndArrow";
		public static string END_ARROW_SIZE = "EndArrowSize";
		public static string END_X = "EndX";
		public static string END_Y = "EndY";
		public static string FACE_NAME = "FaceName";
		public static string FACE_NAMES = "FaceNames";
		public static string FALSE = "0";
		public static string FILL = "Fill";
		public static string FILL_BKGND = "FillBkgnd";
		public static string FILL_BKGND_TRANS = "FillBkgndTrans";
		public static string FILL_FOREGND = "FillForegnd";
		public static string FILL_FOREGND_TRANS = "FillForegndTrans";
		public static string FILL_PATTERN = "FillPattern";
		public static string FILL_STYLE = "FillStyle";
		public static string FLAGS = "Flags";
		public static string FLIP_X = "FlipX";
		public static string FLIP_Y = "FlipY";
		public static string FONT = "Font";
		public static string FONT_NAME = "Name";
		public static string FOREIGN = "Foreign";
		public static string FROM_CELL = "FromCell";
		public static string FROM_SHEET = "FromSheet";
		public static string GEOM = "Geom";
		public static string HEIGHT = "Height";
		public static string HORIZONTAL_ALIGN = "HorzAlign";
		public static string ID = "ID";
		public static string INDENT_FIRST = "IndFirst";
		public static string INDENT_LEFT = "IndLeft";
		public static string INDENT_RIGHT = "IndRight";
		public static string INDEX = "IX";
		public static string LEFT_MARGIN = "LeftMargin";
		public static string LETTER_SPACE = "Letterspace";
		public static string LINE = "Line";
		public static string LINE_COLOR = "LineColor";
		public static string LINE_COLOR_TRANS = "LineColorTrans";
		public static string LINE_PATTERN = "LinePattern";
		public static string LINE_STYLE = "LineStyle";
		public static string LINE_TO = "LineTo";
		public static string LINE_WEIGHT = "LineWeight";
		public static string LOC_PIN_X = "LocPinX";
		public static string LOC_PIN_Y = "LocPinY";
		public static string MASTER = "Master";
		public static string MASTER_SHAPE = "MasterShape";
		public static string MASTERS = "Masters";
		public static string MOVE_TO = "MoveTo";
		public static string NAME = "Name";
		public static string NAME_U = "NameU";
		public static string NO_LINE = "NoLine";
		public static string NURBS_TO = "NURBSTo";
		public static string PAGE = "Page";
		public static string PAGE_HEIGHT = "PageHeight";
		public static string PAGE_WIDTH = "PageWidth";
		public static string PAGES = "Pages";
		public static string PARAGRAPH = "Paragraph";
		public static string PIN_X = "PinX";
		public static string PIN_Y = "PinY";
		public static string POS = "Pos";
		public static string RGB = "RGB";
		public static string RIGHT_MARGIN = "RightMargin";
		public static string ROUNDING = "Rounding";
		public static string RTL_TEXT = "RTLText";
		public static string SIZE = "Size";
		public static string SHAPE = "Shape";
		public static string SHAPES = "Shapes";
		public static string SHAPE_SHDW_SHOW = "ShapeShdwShow";
		public static string SHDW_PATTERN = "ShdwPattern";
		public static string SPACE_AFTER = "SpAfter";
		public static string SPACE_BEFORE = "SpBefore";
		public static string SPACE_LINE = "SpLine";
		public static string STRIKETHRU = "Strikethru";
		public static string STYLE = "Style";
		public static string STYLE_SHEET = "StyleSheet";
		public static string STYLE_SHEETS = "StyleSheets";
		public static string TEXT = "Text";
		public static string TEXT_BKGND = "TextBkgnd";
		public static string TEXT_BLOCK = "TextBlock";
		public static string TEXT_STYLE = "TextStyle";
		public static string TO_PART = "ToPart";
		public static string TO_SHEET = "ToSheet";
		public static string TOP_MARGIN = "TopMargin";
		public static string TRUE = "1";
		public static string TXT_ANGLE = "TxtAngle";
		public static string TXT_HEIGHT = "TxtHeight";
		public static string TXT_LOC_PIN_X = "TxtLocPinX";
		public static string TXT_LOC_PIN_Y = "TxtLocPinY";
		public static string TXT_PIN_X = "TxtPinX";
		public static string TXT_PIN_Y = "TxtPinY";
		public static string TXT_WIDTH = "TxtWidth";
		public static string TYPE = "Type";
		public static string TYPE_GROUP = "Group";
		public static string TYPE_SHAPE = "Shape";
		public static string UNIQUE_ID = "UniqueID";
		public static string VERTICAL_ALIGN = "VerticalAlign";
		public static string WIDTH = "Width";
		public static string X_CON = "XCon";
		public static string X_DYN = "XDyn";
		public static string X = "X";
		public static string Y_CON = "YCon";
		public static string Y_DYN = "YDyn";
		public static string Y = "Y";
		public static string HIDE_TEXT = "HideText";

		public static string VSDX_ID = "vsdxID";

		public static int CONNECT_TO_PART_WHOLE_SHAPE = 3;

		public static readonly string[] SET_VALUES = new string[] {"a", "b"};
		public static readonly ISet<string> MY_SET = new HashSet<string>((SET_VALUES));
	}

}