using mxGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{



    //using Base64 = org.apache.commons.codec.binary.Base64;
    //using StringUtils = org.apache.commons.lang3.StringUtils;
    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;
    using NodeList = System.Xml.XmlNodeList;

    using Color = mxGraph.io.vsdx.theme.Color;
    using QuickStyleVals = mxGraph.io.vsdx.theme.QuickStyleVals;
    using mxCell = mxGraph.model.mxCell;
    using mxGeometry = mxGraph.model.mxGeometry;
    using Utils = mxGraph.online.Utils;
    using mxConstants = mxGraph.util.mxConstants;
    using mxPoint = mxGraph.util.mxPoint;
    using mxResources = mxGraph.util.mxResources;
    using mxCellState = mxGraph.view.mxCellState;
    using mxGraph = mxGraph.view.mxGraph;
    using mxGraphView = mxGraph.view.mxGraphView;

    /// <summary>
    /// This class is a wrapper for one Shape Element.<br/>
    /// This class is responsible for retrieve all the properties of the shape and add it
    /// to the graph. If a property is not found in the shape element but it is an instance
    /// of a Master, the property is taken from the masterShape element. If the property
    /// is not found neither in the masterShape , and it has a reference to a stylesheet
    /// the property is taken from there.
    /// </summary>
    public class VsdxShape : Shape
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            rootShape = this;
        }

        private const string ARROW_NO_FILL_MARKER = "0";

        /// <summary>
        /// Number of d.p. to round non-integers to
        /// </summary>
        public static int maxDp = 2;

        // For debugging to switch off shape matching by name
        public static bool USE_SHAPE_MATCH = true;

        /// <summary>
        /// Whether or not to assume HTML labels
        /// </summary>
        public bool htmlLabels = true;

        /// <summary>
        /// Master Shape referenced by the shape.
        /// </summary>
        protected internal Shape masterShape;

        /// <summary>
        /// Master element referenced by the shape.
        /// </summary>
        protected internal mxVsdxMaster master;

        /// <summary>
        /// If the shape is a sub shape, this is a reference to its root shape, otherwise null
        /// </summary>
        protected internal VsdxShape rootShape;

        public double parentHeight;

        /// <summary>
        /// The prefix of the shape name
        /// </summary>
        protected internal string shapeName = null;

        /// <summary>
        /// Shape index
        /// </summary>
        protected internal int shapeIndex = 0;

        /// <summary>
        /// Whether this cell is a vertex
        /// </summary>
        protected internal bool vertex = true;

        protected internal IDictionary<int?, VsdxShape> childShapes = new Dictionary<int?, VsdxShape>();

        //protected internal static DocumentBuilder docBuilder = null;

        public static readonly ISet<string> OFFSET_ARRAY = new HashSet<string>((new string[] { "Organizational unit", "Domain 3D" }));

        public static readonly string stencilTemplate = "<shape h=\"htemplate\" w=\"wtemplate\" aspect=\"variable\" strokewidth=\"inherit\"><connections></connections><background></background><foreground></foreground></shape>";

        public static readonly float[] arrowSizes = new float[] { 2, 3, 5, 7, 9, 22, 45 };

        public static readonly IDictionary<int?, string> arrowTypes;

        static VsdxShape()
        {
            try
            {
                //mxResources.add("com/mxgraph/io/vdx/resources/edgeNameU");
                //mxResources.add("com/mxgraph/io/vdx/resources/nameU");

                //DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();

                //dbf.setFeature("http://apache.org/xml/features/nonvalidating/load-external-dtd", false);
                //dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
                //dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
                //dbf.ExpandEntityReferences = false;
                //dbf.XIncludeAware = false;

                //docBuilder = dbf.newDocumentBuilder();
            }
            catch (Exception)
            {
                // todo
            }

            arrowTypes = new Dictionary<int?, string>();
            arrowTypes[0] = mxConstants.NONE;
            arrowTypes[1] = mxConstants.ARROW_OPEN;
            arrowTypes[2] = "blockThin";
            arrowTypes[3] = mxConstants.ARROW_OPEN;
            arrowTypes[4] = mxConstants.ARROW_BLOCK;
            arrowTypes[5] = mxConstants.ARROW_CLASSIC;
            arrowTypes[10] = mxConstants.ARROW_OVAL;
            arrowTypes[13] = mxConstants.ARROW_BLOCK;

            arrowTypes[14] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK;
            arrowTypes[17] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_CLASSIC;
            arrowTypes[20] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL;
            arrowTypes[22] = ARROW_NO_FILL_MARKER + "diamond";

            arrowTypes[23] = "dash";
            arrowTypes[24] = "ERone";
            arrowTypes[25] = "ERmandOne";
            arrowTypes[27] = "ERmany";
            arrowTypes[28] = "ERoneToMany";
            arrowTypes[29] = "ERzeroToMany";
            arrowTypes[30] = "ERzeroToOne";

            //approximations
            arrowTypes[6] = mxConstants.ARROW_BLOCK;
            arrowTypes[7] = mxConstants.ARROW_OPEN;
            arrowTypes[8] = mxConstants.ARROW_CLASSIC;

            arrowTypes[9] = "openAsync";
            arrowTypes[11] = "diamond";

            arrowTypes[12] = mxConstants.ARROW_OPEN;

            arrowTypes[15] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK;
            arrowTypes[16] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK;
            arrowTypes[18] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK;
            arrowTypes[19] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_CLASSIC;
            arrowTypes[21] = ARROW_NO_FILL_MARKER + "diamond";
            arrowTypes[26] = "ERmandOne";

            arrowTypes[31] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL;
            arrowTypes[32] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL;
            arrowTypes[33] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL;
            arrowTypes[34] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL;

            arrowTypes[35] = mxConstants.ARROW_OVAL;
            arrowTypes[36] = mxConstants.ARROW_OVAL;
            arrowTypes[37] = mxConstants.ARROW_OVAL;
            arrowTypes[38] = mxConstants.ARROW_OVAL;

            arrowTypes[39] = mxConstants.ARROW_BLOCK;
            arrowTypes[40] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK;

            arrowTypes[41] = ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL;
            arrowTypes[42] = mxConstants.ARROW_OVAL;

            arrowTypes[43] = mxConstants.ARROW_OPEN;
            arrowTypes[44] = mxConstants.ARROW_OPEN;
            arrowTypes[45] = mxConstants.ARROW_OPEN;
        }

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //private static readonly Logger LOGGER = Logger.getLogger(typeof(VsdxShape).FullName);

        /// <summary>
        /// Create a new instance of mxVdxShape.
        /// This method get the references to the master element, master shape
        /// and stylesheet. </summary>
        /// <param name="shape"> </param>
        public VsdxShape(mxVsdxPage page, Element shape, bool vertex, IDictionary<string, mxVsdxMaster> masters, mxVsdxMaster master, mxVsdxModel model) : base(shape, model)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }

            string masterId = this.MasterId;
            string masterShapeLocal = this.ShapeMasterId;

            if (!string.ReferenceEquals(masterId, null))
            {
                this.master = masters[masterId];
            }
            else
            {
                this.master = master;
            }

            if (this.master != null)
            {
                // Check if the master ID corresponds to the one passed in. If it doesn't, or doesn't
                // exist on this shape, this shape is within a group that has that master

                if (string.ReferenceEquals(masterId, null) && !string.ReferenceEquals(masterShapeLocal, null))
                {
                    this.masterShape = this.master.getSubShape(masterShapeLocal);
                }
                else
                {
                    this.masterShape = this.master.MasterShape;
                }
            }

            if (this.debug != null && this.masterShape != null)
            {
                this.masterShape.debug = this.debug;
            }

            string name = NameU;
            int index = name.LastIndexOf(".", StringComparison.Ordinal);

            if (index != -1)
            {
                name = name.Substring(0, index);
            }

            this.shapeName = name;

            // Get sub-shapes
            NodeList shapesList = shape.GetElementsByTagName(mxVsdxConstants.SHAPES);

            if (shapesList != null && shapesList.Count > 0)
            {
                Element shapesElement = (Element)shapesList.Item(0);
                this.childShapes = page.parseShapes(shapesElement, this.master, false);
            }


            double rotation = this.calcRotation();
            this.rotation = rotation * 100 / 100;
            this.rotation = this.rotation % 360.0;

            int themeIndex = page.getCellIntValue("ThemeIndex", -100);

            //sometimes theme information are at the shape level!
            if (themeIndex == -100)
            {
                themeIndex = int.Parse(this.getValue(this.getCellElement("ThemeIndex"), "0"));
            }

            mxVsdxTheme theme = model.Themes[themeIndex];
            int variant = page.getCellIntValue("VariationColorIndex", 0);

            setThemeAndVariant(theme, variant);

            foreach (var entry in childShapes)
            {
                VsdxShape childShape = entry.Value;
                childShape.RootShape = this;

                if (childShape.theme == null)
                {
                    childShape.setThemeAndVariant(theme, variant);
                }
            }

            quickStyleVals = new QuickStyleVals(int.Parse(this.getValue(this.getCellElement("QuickStyleEffectsMatrix"), "0")), int.Parse(this.getValue(this.getCellElement("QuickStyleFillColor"), "1")), int.Parse(this.getValue(this.getCellElement("QuickStyleFillMatrix"), "0")), int.Parse(this.getValue(this.getCellElement("QuickStyleFontColor"), "1")), int.Parse(this.getValue(this.getCellElement("QuickStyleFontMatrix"), "0")), int.Parse(this.getValue(this.getCellElement("QuickStyleLineColor"), "1")), int.Parse(this.getValue(this.getCellElement("QuickStyleLineMatrix"), "0")), int.Parse(this.getValue(this.getCellElement("QuickStyleShadowColor"), "1")), int.Parse(this.getValue(this.getCellElement("QuickStyleType"), "0")), int.Parse(this.getValue(this.getCellElement("QuickStyleVariation"), "0")));

            //process shape geometry
            if (masterShape != null)
            {
                masterShape.processGeomList(null);
                processGeomList(masterShape.GeomList);

                //recalculate width and height using master data
                if (this.width == 0)
                {
                    this.width = getScreenNumericalValue(getCellElement(mxVsdxConstants.WIDTH), 0);
                }

                if (this.height == 0)
                {
                    this.height = getScreenNumericalValue(getCellElement(mxVsdxConstants.HEIGHT), 0);
                }
            }
            else
            {
                processGeomList(null);
            }
            //several shapes have beginX/Y and also has a fill color, thus it is better to render it as a vertex
            //vsdx can have an edge as a group!
            this.vertex = vertex || (childShapes != null && childShapes.Count > 0) || (geomList != null && !geomList.NoFill);
        }

        /// <summary>
        /// Locates the first entry for the specified attribute string in the shape hierarchy.
        /// The order is to look locally, then delegate the request to the master shape
        /// if it doesn't exist locally </summary>
        /// <param name="key"> The key of the shape to find </param>
        /// <returns> the Element that first resolves to that shape key or null or none is found </returns>
        public virtual Element getShapeNode(string key)
        {
            Element elem = this.cellElements.ContainsKey(key)? this.cellElements[key]:null;

            if (elem == null && this.masterShape != null)
            {
                return this.masterShape.getCellElement(key);
            }

            return elem;
        }

        /// <summary>
        /// Returns the value of the Text element.<br/>
        /// If the shape has no text, it is obtained from the master shape. </summary>
        /// <returns> Text label of the shape. </returns>
        public virtual string TextLabel
        {
            get
            {
                string hideText = this.getValue(this.getCellElement(mxVsdxConstants.HIDE_TEXT), "0");

                if ("1".Equals(hideText))
                {
                    return null;
                }

                NodeList txtChildren = TextChildren;

                if (txtChildren == null && masterShape != null)
                {
                    txtChildren = masterShape.TextChildren;
                }

                if (this.htmlLabels)
                {
                    if (txtChildren != null)
                    {
                        // Collect text into same formatting paragraphs. If there's one paragraph, use the new system, otherwise
                        // leave it to the old one.
                        //				if (this.paragraphs == null)
                        //				{
                        //					initLabels(txtChildren);
                        //				}
                        //				
                        //				if (this.paragraphs.size() == 0)
                        //				{
                        //					// valid way to have an empty label override a master value "<text />"
                        //					return "";
                        //				}
                        //				else if (this.paragraphs.size() == 1)
                        //				{
                        //					return createHybridLabel(this.paragraphs.keySet().iterator().next());
                        //				}
                        //				else
                        //				{
                        //Sometimes one paragraph also contains mix of styles which are not supported by hybrid labels, so, use the old style for all html labels
                        this.styleMap[mxConstants.STYLE_VERTICAL_ALIGN] = AlignVertical;
                        this.styleMap[mxConstants.STYLE_ALIGN] = getHorizontalAlign("0", false);

                        return getHtmlTextContent(txtChildren);
                        //				}
                    }
                }
                else
                {
                    string text = this.Text;

                    if (string.ReferenceEquals(text, null) && masterShape != null)
                    {
                        return masterShape.Text;
                    }
                    else
                    {
                        return text;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Initialises the text labels </summary>
        /// <param name="children"> the text Elements </param>
        protected internal virtual void initLabels(NodeList children)
        {
            // Lazy init
            paragraphs = new LinkedHashMap<string, Paragraph>();
            string ch = null;
            string pg = null;
            string fld = null;

            for (int index = 0; index < children.Count; index++)
            {
                string value = null;
                Node node = children.Item(index);
                string nodeName = node.Name;

                switch (nodeName)
                {
                    case "cp":
                        {
                            Element elem = (Element)node;
                            ch = elem.GetAttribute("IX");
                        }
                        break;
                    case "tp":
                        {
                            // TODO
                            Element elem = (Element)node;
                            elem.GetAttribute("IX");
                        }
                        break;
                    case "pp":
                        {
                            Element elem = (Element)node;
                            pg = elem.GetAttribute("IX");
                        }
                        break;
                    case "fld":
                        {
                            Element elem = (Element)node;
                            fld = elem.GetAttribute("IX");
                            break;
                        }
                    case "#text":
                        {
                            //value = StringUtils.chomp(node.TextContent);
                            value = node.InnerText.TrimEnd((char[])"/n/r".ToCharArray());//去掉尾部的换行符
                            // Assumes text is always last
                            // null key is allowed
                            Paragraph para = paragraphs.GetValueOrNull(pg);

                            if (para == null)
                            {
                                para = new Paragraph(value, ch, pg, fld);
                                paragraphs.Add(pg, para);
                            }
                            else
                            {
                                para.addText(value, ch, fld);
                            }
                        }
                        break;
                }
            }
        }

        /// 
        /// <param name="index">
        /// @return </param>
        protected internal virtual string createHybridLabel(string index)
        {
            Paragraph para = this.paragraphs.GetValueOrNull(index);

            // Paragraph
            this.styleMap[mxConstants.STYLE_ALIGN] = getHorizontalAlign(index, false);
            this.styleMap[mxConstants.STYLE_SPACING_LEFT] = getIndentLeft(index);
            this.styleMap[mxConstants.STYLE_SPACING_RIGHT] = getIndentRight(index);
            this.styleMap[mxConstants.STYLE_SPACING_TOP] = getSpBefore(index);
            this.styleMap[mxConstants.STYLE_SPACING_BOTTOM] = getSpAfter(index);
            //this.styleMap.put("text-indent", getIndentFirst(index));
            this.styleMap[mxConstants.STYLE_VERTICAL_ALIGN] = AlignVertical;

            this.styleMap["fontColor"] = getTextColor(index);
            this.styleMap["fontSize"] = double.Parse(this.getTextSize(index)).ToString();
            this.styleMap["fontFamily"] = getTextFont(index);

            // Character
            int fontStyle = isBold(index) ? mxConstants.FONT_BOLD : 0;
            fontStyle |= isItalic(index) ? mxConstants.FONT_ITALIC : 0;
            fontStyle |= isUnderline(index) ? mxConstants.FONT_UNDERLINE : 0;
            this.styleMap["fontStyle"] = fontStyle.ToString();

            //Commented out as the method getTextOpacity returns value between 0 and 1 instead of 0 - 100
            //		this.styleMap.put(mxConstants.STYLE_TEXT_OPACITY, getTextOpacity(index));

            int numValues = para.numValues();
            string result = null;

            for (int i = 0; i < numValues; i++)
            {
                string value = para.getValue(i);

                if (value.Length == 0 && this.fields != null)
                {
                    string fieldIx = para.getField(i);

                    if (!string.ReferenceEquals(fieldIx, null))
                    {
                        value = this.fields.GetValueOrNull(fieldIx);

                        if (string.ReferenceEquals(value, null) && masterShape != null && masterShape.fields != null)
                        {
                            value = masterShape.fields.GetValueOrNull(fieldIx);
                        }
                    }
                }

                if (!string.ReferenceEquals(value, null))
                {
                    result = string.ReferenceEquals(result, null) ? value : result + value;
                }
            }

            return result;
        }


        /// <summary>
        /// Returns the text contained in the shape formated with tags html.<br/> </summary>
        /// <returns> Text content in html. </returns>
        public virtual string getHtmlTextContent(NodeList txtChildren)
        {
            string ret = "";
            bool first = true;

            if (txtChildren != null && txtChildren.Count > 0)
            {
                for (int index = 0; index < txtChildren.Count; index++)
                {
                    Node node = txtChildren.Item(index);

                    if (node.Name.Equals("cp"))
                    {
                        Element elem = (Element)node;
                        cp = elem.GetAttribute("IX");
                    }
                    else if (node.Name.Equals("tp"))
                    {
                        Element elem = (Element)node;
                        tp = elem.GetAttribute("IX");
                    }
                    else if (node.Name.Equals("pp"))
                    {
                        Element elem = (Element)node;
                        pp = elem.GetAttribute("IX");

                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            ret += "</p>";
                        }

                        string para = "<p>";
                        ret += getTextParagraphFormated(para);
                    }
                    else if (node.Name.Equals("fld"))
                    {
                        Element elem = (Element)node;
                        fld = elem.GetAttribute("IX");

                        string text = null;

                        if (this.fields != null)
                        {
                            text = this.fields.GetValueOrNull(fld);
                        }

                        if (string.ReferenceEquals(text, null) && masterShape != null && masterShape.fields != null)
                        {
                            text = masterShape.fields.GetValueOrNull(fld);
                        }

                        if (!string.ReferenceEquals(text, null))
                        {
                            ret += processLblTxt(text);
                        }
                    }
                    else if (node.Name.Equals("#text"))
                    {
                        string text = node.InnerText;

                        // There's a case in master shapes where the text element has the raw value "N".
                        // The source tool doesn't render this. Example is ALM_Information_flow.vdx, the two label
                        // edges in the center
                        //					if (!masterShapeOnly || !text.equals("N"))
                        //					{
                        ret += processLblTxt(text);
                        //					}
                    }
                }
            }

            string end = first ? "" : "</p>";
            ret += end;
            mxVsdxUtils.surroundByTags(ret, "div");

            return ret;
        }

        private string processLblTxt(string text)
        {
            // It's HTML text, so escape it.
            text = mxVsdxUtils.htmlEntities(text);

            text = textToList(text, pp);

            text = text.Replace("\n", "<br/>").Replace(UNICODE_LINE_SEP, "<br/>"); //text.replaceAll("\n", "<br/>").replaceAll(UNICODE_LINE_SEP, "<br/>");

            return getTextCharFormated(text);
        }

        /// <summary>
        /// Checks if a nameU is for big connectors. </summary>
        /// <param name="nameU"> NameU attribute. </param>
        /// <returns> Returns <code>true</code> if a nameU is for big connectors. </returns>
        public virtual bool isConnectorBigNameU(string nameU)
        {
            return nameU.StartsWith("60 degree single", StringComparison.Ordinal) || nameU.StartsWith("45 degree single", StringComparison.Ordinal) || nameU.StartsWith("45 degree double", StringComparison.Ordinal) || nameU.StartsWith("60 degree double", StringComparison.Ordinal) || nameU.StartsWith("45 degree  tail", StringComparison.Ordinal) || nameU.StartsWith("60 degree  tail", StringComparison.Ordinal) || nameU.StartsWith("45 degree tail", StringComparison.Ordinal) || nameU.StartsWith("60 degree tail", StringComparison.Ordinal) || nameU.StartsWith("Flexi-arrow 2", StringComparison.Ordinal) || nameU.StartsWith("Flexi-arrow 1", StringComparison.Ordinal) || nameU.StartsWith("Flexi-arrow 3", StringComparison.Ordinal) || nameU.StartsWith("Double flexi-arrow", StringComparison.Ordinal) || nameU.StartsWith("Fancy arrow", StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks if the shape represents a vertex. </summary>
        /// <returns> Returns <code>true</code> if the shape represents a vertex. </returns>
        public override bool Vertex
        {
            get
            {
                return vertex;
            }
        }

        /// <summary>
        /// Returns the coordinates of the top left corner of the Shape.
        /// When a coordinate is not found, it is taken from masterShape. </summary>
        /// <param name="parentHeight"> Height of the parent cell of the shape. </param>
        /// <param name="rotation"> whether to allow for cell rotation </param>
        /// <returns> mxPoint that represents the coordinates </returns>
        public virtual mxPoint getOriginPoint(double parentHeight, bool rotation)
        {
            double px = this.PinX;
            double py = this.PinY;
            double lpy = this.LocPinY;
            double lpx = this.LocPinX;

            double w = getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.WIDTH), 0);
            double h = getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.HEIGHT), 0);

            double x = px - lpx;
            double y = parentHeight - ((py) + (h - lpy));

            // If the location pins are not in the center of the vertex we
            // need to translate the origin
            if (rotation && (lpy != h / 2 || lpx != w / 2))
            {
                if (this.rotation != 0)
                {
                    double vecX = w / 2 - lpx;
                    double vecY = lpy - h / 2;

                    double cos = Math.Cos(Common.ToRadians(360 - this.rotation)); //Math.Cos(Math.toRadians(360 - this.rotation));
                    double sin = Math.Sin(Common.ToRadians(360 - this.rotation));

                    return new mxPoint(x + vecX - (vecX * cos - vecY * sin), (vecX * sin + vecY * cos) + y - vecY);
                }
            }

            return new mxPoint(x, y);
        }

        /// <summary>
        /// Returns the width and height of the Shape expressed like an mxPoint.<br/>
        /// x = width<br/>
        /// y = height<br/>
        /// When a dimension is not found, it is taken from masterShape. </summary>
        /// <returns> mxPoint that represents the dimensions of the shape. </returns>
        public virtual mxPoint Dimensions
        {
            get
            {
                double w = getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.WIDTH), 0);
                double h = getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.HEIGHT), 0);

                return new mxPoint(w, h);
            }
        }

        /// <summary>
        /// Returns the value of the pinX element. </summary>
        /// <returns> The shape pinX element </returns>
        public virtual double PinX
        {
            get
            {
                return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.PIN_X), 0);
            }
        }

        /// <summary>
        /// Returns the value of the pinY element in pixels. </summary>
        /// <returns> Numerical value of the pinY element. </returns>
        public virtual double PinY
        {
            get
            {
                return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.PIN_Y), 0);
            }
        }

        /// <summary>
        /// Returns the value of the locPinX element in pixels. </summary>
        /// <returns> Numerical value of the pinY element. </returns>
        public virtual double LocPinX
        {
            get
            {
                return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.LOC_PIN_X), 0);
            }
        }

        /// <summary>
        /// Returns the value of the locPinY element in pixels. </summary>
        /// <returns> Numerical value of the locPinY element. </returns>
        public virtual double LocPinY
        {
            get
            {
                return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.LOC_PIN_Y), 0);

            }
        }

        /// <summary>
        /// Returns the opacity of the Shape.<br/> </summary>
        /// <returns> Double in the range of (transparent = 0)..(100 = opaque) </returns>
        private double getOpacity(string key)
        {
            double opacity = 100;

            if (this.Group)
            {
                opacity = 0;
            }

            opacity = getValueAsDouble(this.getCellElement(key), 0);

            opacity = 100 - opacity * 100;
            opacity = Math.Max(opacity, 0);
            opacity = Math.Min(opacity, 100);

            return opacity;
        }

        /// <summary>
        /// Returns the background color for apply in the gradient.<br/>
        /// If no gradient must be applicated, returns an empty string. </summary>
        /// <returns> hexadecimal representation of the color. </returns>
        private string Gradient
        {
            get
            {
                string gradient = "";
                string fillPattern = this.getValue(this.getCellElement(mxVsdxConstants.FILL_PATTERN), "0");

                //		if (fillPattern.equals("25") || fillPattern.equals("27") || fillPattern.equals("28") || fillPattern.equals("30"))
                //approximate all gradients of vsdx with mxGraph one
                if (int.Parse(fillPattern) >= 25)
                {
                    gradient = this.getColor(this.getCellElement(mxVsdxConstants.FILL_BKGND));
                }
                else
                {
                    mxVsdxTheme theme = Theme;

                    if (theme != null)
                    {
                        Color gradColor = theme.getFillGraientColor(QuickStyleVals);
                        if (gradColor != null)
                        {
                            gradient = gradColor.toHexStr();
                        }
                    }
                }

                return gradient;
            }
        }

        /// <summary>
        /// Returns the direction of the gradient.<br/>
        /// If no gradient has to be applied, returns an empty string. </summary>
        /// <returns> Direction.(east, west, north or south) </returns>
        private string GradientDirection
        {
            get
            {
                string direction = "";
                string fillPattern = this.getValue(this.getCellElement(mxVsdxConstants.FILL_PATTERN), "0");

                if (fillPattern.Equals("25"))
                {
                    direction = mxConstants.DIRECTION_EAST;
                }
                else if (fillPattern.Equals("27"))
                {
                    direction = mxConstants.DIRECTION_WEST;
                }
                else if (fillPattern.Equals("28"))
                {
                    direction = mxConstants.DIRECTION_SOUTH;
                }
                else if (fillPattern.Equals("30"))
                {
                    direction = mxConstants.DIRECTION_NORTH;
                }

                return direction;
            }
        }

        /// <summary>
        /// Returns the rotation of the shape.<br/> </summary>
        /// <returns> Rotation of the shape in degrees. </returns>
        public virtual double calcRotation()
        {
            double rotation = Convert.ToDouble(this.getValue(this.getCellElement(mxVsdxConstants.ANGLE), "0"));

            rotation = Common.ToDegrees(rotation);// Math.toDegrees(rotation);
            rotation = rotation % 360;
            rotation = rotation * 100 / 100;

            return 360 - rotation;
        }

        /// <summary>
        /// Used to pass in a parents rotation to the child </summary>
        /// <param name="parentRotation"> the rotation of the parent </param>
        public virtual void propagateRotation(double parentRotation)
        {
            this.rotation += parentRotation;
            this.rotation %= 360;
            this.rotation = this.rotation * 100 / 100;
        }

        /// <summary>
        /// Returns the top spacing of the label in pixels.<br/>
        /// The property may to be defined in master shape or text stylesheet.<br/> </summary>
        /// <returns> Top spacing in double precision. </returns>
        public virtual double TopSpacing
        {
            get
            {
                double topMargin = this.TextTopMargin;
                topMargin = (topMargin / 2 - 2.8) * 100 / 100;
                return topMargin;
            }
        }

        /// <summary>
        /// Returns the bottom spacing of the label in pixels.<br/>
        /// The property may to be defined in master shape or text stylesheet.<br/> </summary>
        /// <returns> Bottom spacing in double precision. </returns>
        public virtual double BottomSpacing
        {
            get
            {
                double bottomMargin = this.TextBottomMargin;
                bottomMargin = (bottomMargin / 2 - 2.8) * 100 / 100;
                return bottomMargin;
            }
        }

        /// <summary>
        /// Returns the left spacing of the label in pixels.<br/>
        /// The property may to be defined in master shape or text stylesheet.<br/> </summary>
        /// <returns> Left spacing in double precision. </returns>
        public virtual double LeftSpacing
        {
            get
            {
                double leftMargin = this.TextLeftMargin;
                leftMargin = (leftMargin / 2 - 2.8) * 100 / 100;
                return leftMargin;
            }
        }

        /// <summary>
        /// Returns the right spacing of the label in pixels.<br/>
        /// The property may to be defined in master shape or text stylesheet.<br/> </summary>
        /// <returns> Right spacing in double precision. </returns>
        public virtual double RightSpacing
        {
            get
            {
                double rightMargin = this.TextRightMargin;
                rightMargin = (rightMargin / 2 - 2.8) * 100 / 100;
                return rightMargin;
            }
        }


        /// <summary>
        /// Checks if the label must be rotated.<br/>
        /// The property may to be defined in master shape or text stylesheet.<br/> </summary>
        /// <returns> Returns <code>true<code/> if the label should remain horizontal. </returns>
        public virtual bool LabelRotation
        {
            get
            {
                bool hor = true;
                //Defines rotation.
                double rotation = this.calcRotation();
                double angle = Convert.ToDouble(this.getValue(this.getCellElement(mxVsdxConstants.TXT_ANGLE), "0"));

                angle = Common.ToDegrees(angle);// Math.toDegrees(angle);
                angle = angle - rotation;

                if (!(Math.Abs(angle) < 45 || Math.Abs(angle) > 270))
                {
                    hor = false;
                }

                return hor;
            }
        }

        /// <summary>
        /// Analyzes the shape and returns a string with the style. </summary>
        /// <returns> style read from the shape. </returns>
        public virtual IDictionary<string, string> StyleFromShape
        {
            get
            {
                styleMap[mxVsdxConstants.VSDX_ID] = this.Id.ToString();

                // Rotation.		
                //		String labelRotation = getLabelRotation() ? "1" : "0";
                this.rotation = Math.Round(this.rotation);

                //It gives wrong results, may be it is needed in other scenarios
                //		if (!labelRotation.equals("1") && this.rotation != 90 && this.rotation != 270)
                //		{
                //			styleMap.put(mxConstants.STYLE_HORIZONTAL, labelRotation);
                //		}

                if (this.rotation != 0)
                {
                    styleMap[mxConstants.STYLE_ROTATION] = Convert.ToString(this.rotation);
                }

                // Fill color
                string fillcolor = FillColor;

                if (!fillcolor.Equals(""))
                {
                    styleMap[mxConstants.STYLE_FILLCOLOR] = fillcolor;
                }
                else
                {
                    styleMap[mxConstants.STYLE_FILLCOLOR] = "none";
                }

                int? id = this.Id;

                this.styleDebug("ID = " + id + " , Fill Color = " + fillcolor);

                //Defines gradient
                string gradient = Gradient;

                if (!gradient.Equals(""))
                {
                    styleMap[mxConstants.STYLE_GRADIENTCOLOR] = gradient;
                    string gradientDirection = GradientDirection;

                    if (!gradientDirection.Equals("") && !gradientDirection.Equals(mxConstants.DIRECTION_SOUTH))
                    {
                        styleMap[mxConstants.STYLE_GRADIENT_DIRECTION] = gradientDirection;
                    }
                }
                else
                {
                    styleMap[mxConstants.STYLE_GRADIENTCOLOR] = "none";
                }

                double opacity = this.getOpacity(mxVsdxConstants.FILL_FOREGND_TRANS);

                if (opacity < 100)
                {
                    styleMap[mxConstants.STYLE_FILL_OPACITY] = Convert.ToString(opacity);
                }

                opacity = this.getOpacity(mxVsdxConstants.LINE_COLOR_TRANS);

                if (opacity < 100)
                {
                    styleMap[mxConstants.STYLE_STROKE_OPACITY] = Convert.ToString(opacity);
                }

                IDictionary<string, string> form = Form;

                if (form.ContainsKey(mxConstants.STYLE_SHAPE) && (form[mxConstants.STYLE_SHAPE].StartsWith("image;", StringComparison.Ordinal)))
                {
                    styleMap[mxConstants.STYLE_WHITE_SPACE] = "wrap";
                }

                //JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
                //styleMap.putAll(form);
                foreach (var item in form)
                {
                    styleMap.Add(item.Key, item.Value);
                }

                //Defines line Pattern
                if (Dashed)
                {
                    styleMap[mxConstants.STYLE_DASHED] = "1";

                    string dashPattern = DashPattern;

                    if (!string.ReferenceEquals(dashPattern, null))
                    {
                        styleMap[mxConstants.STYLE_DASH_PATTERN] = dashPattern;
                    }
                }

                string color = StrokeColor;
                double tr = this.StrokeTransparency;

                this.styleDebug("ID = " + id + " , Color = " + color + " , stroke transparency = " + tr);

                if (!color.Equals("") && tr != 1)
                {
                    styleMap[mxConstants.STYLE_STROKECOLOR] = color;
                }
                else
                {
                    //styleMap.put(mxConstants.STYLE_STROKECOLOR, "none");
                }

                //Defines the line width
                double lWeight = LineWidth * 100 / 100;

                if (lWeight != 1)
                {
                    styleMap[mxConstants.STYLE_STROKEWIDTH] = Convert.ToString(lWeight);
                }

                /// <summary>
                /// SHADOW * </summary>
                if (Shadow)
                {
                    styleMap[mxConstants.STYLE_SHADOW] = mxVsdxConstants.TRUE;
                }

                //Defines label top spacing
                double topMargin = TopSpacing * 100 / 100;

                if (topMargin != 0)
                {
                    styleMap[mxConstants.STYLE_SPACING_TOP] = Convert.ToString(topMargin);
                }

                //Defines label bottom spacing
                double bottomMargin = BottomSpacing * 100 / 100;

                if (bottomMargin != 0)
                {
                    styleMap[mxConstants.STYLE_SPACING_BOTTOM] = Convert.ToString(bottomMargin);
                }

                //Defines label left spacing
                double leftMargin = LeftSpacing * 100 / 100;

                if (leftMargin != 0)
                {
                    styleMap[mxConstants.STYLE_SPACING_LEFT] = Convert.ToString(leftMargin);
                }

                //Defines label right spacing
                double rightMargin = RightSpacing * 100 / 100;

                if (rightMargin != 0)
                {
                    styleMap[mxConstants.STYLE_SPACING_RIGHT] = Convert.ToString(rightMargin);
                }

                string direction = getDirection(form);

                if (!string.ReferenceEquals(direction, mxConstants.DIRECTION_EAST))
                {
                    styleMap[mxConstants.STYLE_DIRECTION] = direction;
                }

                string flibX = getValue(this.getCellElement(mxVsdxConstants.FLIP_X), "0");
                string flibY = getValue(this.getCellElement(mxVsdxConstants.FLIP_Y), "0");

                if ("1".Equals(flibX))
                {
                    styleMap[mxConstants.STYLE_FLIPH] = "1";
                }

                if ("1".Equals(flibY))
                {
                    styleMap[mxConstants.STYLE_FLIPV] = "1";
                }

                resolveCommonStyles();

                return this.styleMap;
            }
        }

        private string DashPattern
        {
            get
            {
                List<double?> pattern = null;

                string linePattern = this.getValue(this.getCellElement(mxVsdxConstants.LINE_PATTERN), "0");

                if (linePattern.Equals("Themed"))
                {
                    mxVsdxTheme theme = Theme;

                    if (theme != null)
                    {
                        pattern = Vertex ? theme.getLineDashPattern(QuickStyleVals) : theme.getConnLineDashPattern(QuickStyleVals);
                    }
                }
                else
                {
                    pattern = getLineDashPattern(int.Parse(linePattern));
                }

                if (pattern != null && pattern.Count > 0)
                {
                    StringBuilder str = new StringBuilder();

                    foreach (double? len in pattern)
                    {
                        str.Append(string.Format("{0:F2} ", len));
                    }
                    return str.ToString().Trim();
                }
                return null;
            }
        }

        /// <summary>
        /// Checks if the lines of the shape are dashed.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/> </summary>
        /// <returns> Returns <code>true</code> if the lines of the shape are dashed. </returns>
        public virtual bool Dashed
        {
            get
            {
                string linePattern = this.getValue(this.getCellElement(mxVsdxConstants.LINE_PATTERN), "0");

                if (linePattern.Equals("Themed"))
                {
                    mxVsdxTheme theme = Theme;

                    if (theme != null)
                    {
                        return Vertex ? theme.isLineDashed(QuickStyleVals) : theme.isConnLineDashed(QuickStyleVals);
                    }
                }
                else if (!(linePattern.Equals("0") || linePattern.Equals("1")))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns the line width.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/> </summary>
        /// <returns> Line width in pixels. </returns>
        public virtual double LineWidth
        {
            get
            {
                string lineWeight = getValue(this.getCellElement(mxVsdxConstants.LINE_WEIGHT), "1");

                double lWeight = 1;
                try
                {
                    if (lineWeight.Equals("Themed"))
                    {
                        mxVsdxTheme theme = Theme;

                        if (theme != null)
                        {
                            lWeight = (Vertex ? theme.getLineWidth(QuickStyleVals) : theme.getConnLineWidth(QuickStyleVals)) / 10000.0;
                        }
                    }
                    else
                    {
                        lWeight = double.Parse(lineWeight);
                        lWeight = getScreenNumericalValue(lWeight);
                    }
                }
                catch (Exception)
                {
                    // ignore
                }

                //Value is fixed for weight < 1
                if (lWeight < 1)
                {
                    lWeight *= 2;
                }

                return lWeight;
            }
        }

        /// <summary>
        /// Returns the start arrow size.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/>
        /// Determines the value in pixels of each arrow size category in .vdx. </summary>
        /// <returns> Size in pixels. </returns>
        public virtual float StartArrowSize
        {
            get
            {
                string baSize = getValue(this.getCellElement(mxVsdxConstants.BEGIN_ARROW_SIZE), "4");

                try
                {
                    int size = 4;

                    if (baSize.Equals("Themed"))
                    {
                        mxVsdxTheme theme = Theme;

                        if (theme != null)
                        {
                            size = Vertex ? theme.getStartSize(QuickStyleVals) : theme.getConnStartSize(QuickStyleVals);
                        }
                    }
                    else
                    {
                        size = Convert.ToInt32(baSize);
                    }

                    return VsdxShape.arrowSizes[size];
                }
                catch (Exception)
                {
                    // ignore
                }

                return 4;
            }
        }

        /// <summary>
        /// Returns the end arrow size.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/>
        /// Determines the value in pixels of each arrow size category in .vdx. </summary>
        /// <returns> Size in pixels. </returns>
        public virtual float FinalArrowSize
        {
            get
            {
                string eaSize = getValue(this.getCellElement(mxVsdxConstants.END_ARROW_SIZE), "4");

                try
                {
                    int size = 4;

                    if (eaSize.Equals("Themed"))
                    {
                        mxVsdxTheme theme = Theme;

                        if (theme != null)
                        {
                            size = Vertex ? theme.getEndSize(QuickStyleVals) : theme.getConnEndSize(QuickStyleVals);
                        }
                    }
                    else
                    {
                        size = Convert.ToInt32(eaSize);
                    }

                    return VsdxShape.arrowSizes[size];
                }
                catch (Exception)
                {
                    // ignore
                }

                return 4;
            }
        }

        /// <summary>
        /// Returns whether the cell is Rounded.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/> </summary>
        /// <returns> Returns <code>true</code> if the cell is Rounded. </returns>
        public virtual bool Rounded
        {
            get
            {
                string val = getValue(this.getCellElement(mxVsdxConstants.ROUNDING), "0");

                if ("Themed".Equals(val))
                {
                    //TODO add theme support 
                    val = "0";
                }
                return Convert.ToDouble(val) > 0;
            }
        }

        /// <summary>
        /// Return if the line has shadow.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/> </summary>
        /// <returns> Returns <code>mxVdxConstants.TRUE</code> if the line has shadow. </returns>
        public virtual bool Shadow
        {
            get
            {
                // https://msdn.microsoft.com/en-us/library/office/jj230454.aspx TODO
                // double shdwShow = this.getNumericalValue(this.getStyleNode(mxVdxConstants.SHDW_PATTERN), 0);

                string shdw = this.getValue(this.getCellElement(mxVsdxConstants.SHDW_PATTERN), "0");

                if (shdw.Equals("Themed"))
                {
                    // TODO get value from theme
                }
                else if (!shdw.Equals("0"))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns the style of the edge. (Orthogonal or straight) </summary>
        /// <returns> Edge Style. </returns>
        public virtual IDictionary<string, string> getEdgeStyle(IDictionary<string, string> edgeShape)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            string edgeName = edgeShape[mxConstants.STYLE_SHAPE];

            if (edgeName.Equals("mxgraph.lean_mapping.electronic_info_flow_edge"))
            {
                result[mxConstants.STYLE_EDGE] = mxConstants.NONE;
                return result;
            }
            else
            {
                result[mxConstants.STYLE_EDGE] = mxConstants.EDGESTYLE_ELBOW;
                return result;
            }
            //		else
            //		{
            //			result.put(mxConstants.STYLE_EDGE, mxConstants.NONE);
            //			return result;
            //		}
        }

        /// <summary>
        /// Returns the master's Id of the Shape. </summary>
        /// <returns> Master's ID of the shape, null if has not a master. </returns>
        public virtual string MasterId
        {
            get
            {
                if (shape.HasAttribute(mxVsdxConstants.MASTER))
                {
                    return shape.GetAttribute(mxVsdxConstants.MASTER);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the masterShape's Id of the shape. </summary>
        /// <returns> Master Shape's ID of the shape, null if has not a master shape. </returns>
        public virtual string ShapeMasterId
        {
            get
            {
                if (shape.HasAttribute(mxVsdxConstants.MASTER_SHAPE))
                {
                    return shape.GetAttribute(mxVsdxConstants.MASTER_SHAPE);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks if a shape contains other shapes inside. </summary>
        /// <returns> Returns <code>true</code> if a shape contains other shapes inside. </returns>
        public virtual bool Group
        {
            get
            {
                return shape.GetAttribute("Type").Equals("Group");
            }
        }

        /// <summary>
        /// Checks if a shape contains other shapes inside. </summary>
        /// <returns> Returns <code>true</code> if a shape contains other shapes inside. </returns>
        public static string getType(Element shape)
        {
            return shape.GetAttribute("Type");
        }

        public virtual mxVsdxMaster Master
        {
            get
            {
                return master;
            }
        }

        /// <summary>
        /// Returns the NameU attribute. </summary>
        /// <returns> Value of the NameU attribute. </returns>
        public override string NameU
        {
            get
            {
                string result = shape.GetAttribute(mxVsdxConstants.NAME_U);

                if ((string.ReferenceEquals(result, null) || result.Equals("")) && masterShape != null)
                {
                    result = masterShape.NameU;
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the Name attribute. </summary>
        /// <returns> Value of the Name attribute (Human readable name). </returns>
        public override string Name
        {
            get
            {
                string result = shape.GetAttribute(mxVsdxConstants.NAME);

                if ((string.ReferenceEquals(result, null) || result.Equals("")) && masterShape != null)
                {
                    result = masterShape.Name;
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the master name of the shape </summary>
        /// <returns> Master name of the shape </returns>
        public virtual string MasterName
        {
            get
            {
                return shapeName;

            }
        }

        public virtual void setLabelOffset(mxCell vertex, string style)
        {
            string nameU = "";
            string masterNameU = "";

            if (shape.HasAttribute(mxVsdxConstants.NAME_U))
            {
                nameU = shape.GetAttribute(mxVsdxConstants.NAME_U);
            }


            if (this.Master != null && this.Master.MasterElement != null)
            {
                if (this.Master.MasterElement.HasAttribute(mxVsdxConstants.NAME_U))
                {
                    masterNameU = this.Master.MasterElement.GetAttribute(mxVsdxConstants.NAME_U);
                }
            }

            //check for shape name/type, because of different (shape specific) treatment of each
            if (nameU.StartsWith("Organizational unit", StringComparison.Ordinal) || masterNameU.StartsWith("Organizational unit", StringComparison.Ordinal))
            {
                Element control = (Element)shape.GetElementsByTagName(mxVsdxConstants.CONTROL).Item(0);

                Element xEl = null;
                string xS = "0.0";
                Element yEl = null;
                string yS = "-0.4";

                if (control != null)
                {
                    xEl = (Element)control.GetElementsByTagName(mxVsdxConstants.X).Item(0);

                    if (xEl.HasAttribute("F"))
                    {
                        xS = xEl.GetAttribute("F");
                    }
                    else
                    {
                        xS = xEl.InnerText;
                    }

                    yEl = (Element)control.GetElementsByTagName(mxVsdxConstants.Y).Item(0);

                    if (yEl.HasAttribute("F"))
                    {
                        yS = yEl.GetAttribute("F");
                    }
                    else
                    {
                        yS = yEl.InnerText;
                    }
                }

                mxGeometry geometry = vertex.Geometry;

                //clean the formula strings and hope it will work with a specific algorithm
                xS = xS.Replace("Width/2+", "");
                xS = xS.Replace("DL", "");
                yS = yS.Replace("Height*", "");

                if (xS.Equals("Inh"))
                {
                    xS = "0.0";
                }

                if (yS.Equals("Inh"))
                {
                    yS = "-0.4";
                }

                if (yS.Contains("txtHeight"))
                {
                    yS = "-0.4";
                }

                string[] styleArray = style.Split(";", true);
                string tabHeight = "";

                for (int i = 0; i < styleArray.Length; i++)
                {
                    string currStyle = styleArray[i];
                    currStyle = currStyle.Trim();

                    if (currStyle.StartsWith("tabHeight=", StringComparison.Ordinal))
                    {
                        tabHeight = currStyle.Replace("tabHeight=", "");
                    }
                }

                if (tabHeight.Equals(""))
                {
                    tabHeight = "20";
                }

                double? tH = Convert.ToDouble(tabHeight);

                double? x = double.Parse(xS);
                double? y = double.Parse(yS);
                double? h = geometry.Height;
                double? xFinal = geometry.Width * 0.1 + x * 100;
                double? yFinal = h - h * y - tH / 2;
                mxPoint offset = new mxPoint(xFinal.Value, yFinal.Value);
                vertex.Geometry.Offset = offset;
            }
            else if (nameU.StartsWith("Domain 3D", StringComparison.Ordinal) || masterNameU.StartsWith("Domain 3D", StringComparison.Ordinal))
            {
                Element control = (Element)shape.GetElementsByTagName(mxVsdxConstants.CONTROL).Item(0);

                Element xEl = null;
                string xS = "0.0";
                Element yEl = null;
                string yS = "-0.4";

                if (control != null)
                {
                    xEl = (Element)control.GetElementsByTagName(mxVsdxConstants.X).Item(0);
                    xS = xEl.GetAttribute("F");
                    yEl = (Element)control.GetElementsByTagName(mxVsdxConstants.Y).Item(0);
                    yS = yEl.GetAttribute("F");
                }

                mxGeometry geometry = vertex.Geometry;

                //clean the formula strings and hope it will work with a specific algorithm
                xS = xS.Replace("Width/2+", "");
                xS = xS.Replace("DL", "");
                yS = yS.Replace("Height*", "");

                if (xS.Equals("Inh") || xS.Equals(""))
                {
                    xS = "0.0";
                }

                if (yS.Equals("Inh") || yS.Equals(""))
                {
                    yS = "-0.4";
                }

                if (yS.Contains("txtHeight"))
                {
                    yS = "-0.4";
                }

                double? x = double.Parse(xS);
                double? y = double.Parse(yS);
                double? h = geometry.Height;
                double? xFinal = geometry.Width * 0.1 + x * 100;
                double? yFinal = h - h * y;
                mxPoint offset = new mxPoint(xFinal.Value, yFinal.Value);
                vertex.Geometry.Offset = offset;
            }
        }

        /// <summary>
        /// Returns the constant that represents the Shape. </summary>
        /// <returns> String that represent the form. </returns>
        public virtual IDictionary<string, string> Form
        {
            get
            {
                IDictionary<string, string> result = new Dictionary<string, string>();

                this.styleDebug("Looking to match shape = " + shapeName);

                if (!string.ReferenceEquals(shapeName, null) && !shapeName.Equals("") && VsdxShape.USE_SHAPE_MATCH)
                {
                    string trans = mxResources.get(shapeName);

                    if (!string.ReferenceEquals(trans, null) && !trans.Equals(""))
                    {
                        this.styleDebug("Translation = " + trans);
                        result[mxConstants.STYLE_SHAPE] = trans;
                        return result;
                    }
                }

                if (this.Vertex)
                {
                    try
                    {
                        string type = VsdxShape.getType(this.Shape);
                        // String foreignType = "";
                        this.styleDebug("shape type = " + type);

                        //The master may contain the foreign object data
                        if (this.imageData != null || (mxVsdxConstants.FOREIGN.Equals(type) && masterShape != null && masterShape.imageData != null))
                        {
                            IDictionary<string, string> imageData = this.imageData != null ? this.imageData : masterShape.imageData;

                            result["shape"] = "image";
                            result["aspect"] = "fixed";
                            string iType = imageData["iType"];
                            string iData = imageData["iData"];

                            result["image"] = "data:image/" + iType + "," + iData;
                            return result;
                        }

                        //Shape inherit master geometry and can change some of it or override it completely. So, no need to parse the master instead of the shape itself
                        string parsedGeom = this.parseGeom();

                        if (parsedGeom.Equals(""))
                        {
                            this.styleDebug("No geom found");
                            return result;
                        }

                        string stencil = Utils.encodeURIComponent(parsedGeom, "UTF-8");

                        //sbyte[] bytes = stencil.GetBytes(Encoding.UTF8);
                        //Deflater deflater = new Deflater(Deflater.BEST_COMPRESSION, true);
                        //deflater.Input = bytes;
                        //System.IO.MemoryStream outputStream = new System.IO.MemoryStream();

                        //deflater.finish();

                        //byte[] buffer = new byte[1024];

                        //while (!deflater.finished())
                        //{
                        //    int count = deflater.deflate(buffer);
                        //    outputStream.Write(buffer, 0, count);
                        //}

                        //try
                        //{
                        //    outputStream.Close();
                        //}
                        //catch (IOException e)
                        //{
                        //    // TODO Auto-generated catch block
                        //    Console.WriteLine(e.ToString());
                        //    Console.Write(e.StackTrace);
                        //}

                        //byte[] output = outputStream.ToArray();
                        //deflater.end();

                        //sbyte[] encoded = Base64.encodeBase64(output);
                        //string enc = StringHelperClass.NewString(encoded, "UTF-8");


                        //result[mxConstants.STYLE_SHAPE] = "stencil(" + enc + ")";

                        result[mxConstants.STYLE_SHAPE] = "stencil(" + stencil + ")";
                    }
                    catch (Exception e)
                    {
                        // TODO Auto-generated catch block
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }
                else
                {
                    return EdgeStyle;
                }

                return result;
            }
        }

        /// <summary>
        /// Checks if a shape may to be imported like an Off page reference. </summary>
        /// <returns> Returns <code>true</code> if a shape may to be imported like an Off page reference. </returns>
        public virtual bool Off_page_reference
        {
            get
            {
                string name = NameU;

                if (name.Equals("Off-page reference") || name.Equals("Lined/Shaded process"))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if a shape may to be imported like an External process. </summary>
        /// <returns> Returns <code>true</code> if a shape may to be imported like an External process. </returns>
        public virtual bool External_process
        {
            get
            {
                return (shapeName.Equals("External process"));
            }
        }

        /// <summary>
        /// Returns the direction of the shape. </summary>
        /// <param name="form"> Form of the shape. </param>
        /// <returns> Direction(south, north, east and south) </returns>
        public virtual string getDirection(IDictionary<string, string> form)
        {
            string offsetS = (string)mxResources.get("mxOffset" + shapeName);

            if (string.ReferenceEquals(offsetS, null) || offsetS.Equals("0") || offsetS.Equals(""))
            {
                return mxConstants.DIRECTION_EAST;
            }
            else if (offsetS.Equals("1"))
            {
                return mxConstants.DIRECTION_SOUTH;
            }
            else if (offsetS.Equals("2"))
            {
                return mxConstants.DIRECTION_WEST;
            }
            else if (offsetS.Equals("3"))
            {
                return mxConstants.DIRECTION_NORTH;
            }

            return mxConstants.DIRECTION_EAST;
        }

        /// <summary>
        /// Checks if a shape may to be imported like a Sub-process.
        /// This method is approximated. </summary>
        /// <returns> Returns <code>true</code> if a shape may to be imported like a
        /// Sub-process. </returns>
        public virtual bool Subproces
        {
            get
            {
                return shapeName.Equals("Subproces");
            }
        }

        /// <returns> style map containing the proper shape and style (if needed) of a Visio "dynamic connector" edge </returns>
        public virtual IDictionary<string, string> EdgeStyle
        {
            get
            {
                IDictionary<string, string> result = new Dictionary<string, string>();

                result["edgeStyle"] = "none";
                return result;

                //result.put("edgeStyle", "orthogonalEdgeStyle");
                //return result;

                //result.put("curved", "1");
                //return result;

                //return null;
            }
        }

        public virtual IDictionary<int?, VsdxShape> ChildShapes
        {
            get
            {
                return childShapes;
            }
            set
            {
                this.childShapes = value;
            }
        }


        public virtual bool DisplacedLabel
        {
            get
            {
                string txtPinXF = this.getAttribute(mxVsdxConstants.TXT_PIN_X, "F", "");
                string txtPinYF = this.getAttribute(mxVsdxConstants.TXT_PIN_Y, "F", "");
                string txtWidthF = this.getAttribute(mxVsdxConstants.TXT_WIDTH, "F", "");
                string txtHeightF = this.getAttribute(mxVsdxConstants.TXT_HEIGHT, "F", "");

                if (masterShape != null)
                {
                    if (string.ReferenceEquals(txtPinXF, "") || txtPinXF.ToLower().Equals("inh"))
                    {
                        txtPinXF = masterShape.getAttribute(mxVsdxConstants.TXT_PIN_X, "F", "");
                    }

                    if (string.ReferenceEquals(txtPinYF, "") || txtPinYF.ToLower().Equals("inh"))
                    {
                        txtPinYF = masterShape.getAttribute(mxVsdxConstants.TXT_PIN_Y, "F", "");
                    }

                    if (string.ReferenceEquals(txtWidthF, "") || txtWidthF.ToLower().Equals("inh"))
                    {
                        txtWidthF = masterShape.getAttribute(mxVsdxConstants.TXT_WIDTH, "F", "");
                    }

                    if (string.ReferenceEquals(txtHeightF, "") || txtHeightF.ToLower().Equals("inh"))
                    {
                        txtHeightF = masterShape.getAttribute(mxVsdxConstants.TXT_HEIGHT, "F", "");
                    }
                }

                if (txtPinXF.ToLower().Equals("width*0.5") && txtPinYF.ToLower().Equals("height*0.5") && txtWidthF.ToLower().Equals("width*1") && txtHeightF.ToLower().Equals("height*1"))
                {
                    return false;
                }
                else if (txtPinXF.ToLower().StartsWith("width*", StringComparison.Ordinal) && txtPinYF.ToLower().StartsWith("height*", StringComparison.Ordinal) && txtWidthF.ToLower().StartsWith("width*", StringComparison.Ordinal) && txtHeightF.ToLower().StartsWith("height*", StringComparison.Ordinal))
                //		else if (txtPinXF.toLowerCase().startsWith("width*") &&
                //				txtPinYF.toLowerCase().startsWith("height*"))
                {
                    return true;
                }
                else if (txtPinXF.ToLower().StartsWith("controls.row_", StringComparison.Ordinal) || txtPinYF.ToLower().StartsWith("controls.row_", StringComparison.Ordinal))
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool RotatedLabel
        {
            get
            {
                string txtAngleValue = this.getAttribute(mxVsdxConstants.TXT_ANGLE, "V", "");

                if (masterShape != null)
                {
                    if (txtAngleValue.Equals(""))
                    {
                        txtAngleValue = masterShape.getAttribute(mxVsdxConstants.TXT_ANGLE, "V", "");
                    }

                }

                if (!txtAngleValue.Equals("0") && !txtAngleValue.Equals("0.0") && !txtAngleValue.Equals(""))
                {
                    return true;
                }

                return false;
            }
        }

        public virtual VsdxShape RootShape
        {
            set
            {
                this.rootShape = value;
            }
            get
            {
                return this.rootShape;
            }
        }


        // Edge specific methods 

        /// <summary>
        /// Returns the coordinates of the begin point of an Edge Shape. </summary>
        /// <param name="parentHeight"> Height of the parent of the shape. </param>
        /// <returns> mxPoint that represents the coordinates. </returns>
        public virtual mxPoint getStartXY(double parentHeight)
        {
            double startX = getScreenNumericalValue(this.getCellElement(mxVsdxConstants.BEGIN_X), 0);
            double startY = parentHeight - getScreenNumericalValue(this.getCellElement(mxVsdxConstants.BEGIN_Y), 0);

            return new mxPoint(startX, startY);
        }

        /// <summary>
        /// Returns the coordinates of the end point of an Edge Shape. </summary>
        /// <param name="parentHeight"> Height of the parent of the shape. </param>
        /// <returns> mxPoint that represents the coordinates. </returns>
        public virtual mxPoint getEndXY(double parentHeight)
        {
            double endX = getScreenNumericalValue(this.getCellElement(mxVsdxConstants.END_X), 0);
            double endY = parentHeight - getScreenNumericalValue(this.getCellElement(mxVsdxConstants.END_Y), 0);

            return new mxPoint(endX, endY);
        }

        /// <summary>
        /// Returns the list of routing points of a edge shape. </summary>
        /// <param name="parentHeight"> Height of the parent of the shape. </param>
        /// <returns> List of mxPoint that represents the routing points. </returns>
        public virtual IList<mxPoint> getRoutingPoints(double parentHeight, mxPoint startPoint, double rotation) //, boolean flipX, boolean flipY
        {
            if (geomList != null)
            {
                return geomList.getRoutingPoints(parentHeight, startPoint, rotation);
            }
            return null;
        }

        /// <summary>
        /// Returns the list of control points of a edge shape. </summary>
        /// <param name="parentHeight"> Height of the parent of the shape. </param>
        /// <returns> List of mxPoint that represents the control points. </returns>
        public virtual IList<mxPoint> getControlPoints(double parentHeight)
        {
            mxPoint startXY = getStartXY(parentHeight);
            mxPoint endXY = getEndXY(parentHeight);
            List<mxPoint> pointList = new List<mxPoint>();

            if (shape != null)
            {
                NodeList geomList = shape.GetElementsByTagName(mxVsdxConstants.GEOM);

                if (geomList.Count > 0)
                {
                    Element firstGeom = (Element)geomList.Item(0);
                    Element firstNURBS = (Element)firstGeom.GetElementsByTagName(mxVsdxConstants.NURBS_TO).Item(0);
                    Element firstE = (Element)firstNURBS.GetElementsByTagName("E").Item(0);

                    if (firstE != null)
                    {
                        string f = firstE.GetAttribute("F");
                        f = f.Replace("NURBS\\(", "");
                        f = f.Replace("\\)", "");
                        f = f.Replace(",", " ");
                        f = f.Replace("\\s\\s", " ");
                        string[] pointsS = f.Split(" ", true);
                        double[] pointsRaw = new double[pointsS.Length];

                        for (int i = 0; i < pointsS.Length; i++)
                        {
                            pointsRaw[i] = double.Parse(pointsS[i]);
                        }

                        for (int i = 2; i + 4 < pointsS.Length; i = i + 4)
                        {
                            mxPoint currPoint = new mxPoint();
                            double rawX = pointsRaw[i + 2];
                            double rawY = pointsRaw[i + 3];
                            double width = Math.Abs(endXY.X - startXY.X);
                            double widthFixed = Math.Min(100, width);
                            double heightFixed = 100;
                            double finalX = 0;

                            finalX = startXY.X + widthFixed * rawX;
                            currPoint.X = finalX;
                            currPoint.Y = startXY.Y - heightFixed * rawY;
                            pointList.Add(currPoint);
                        }

                        return pointList;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Analyzes a edge shape and returns a string with the style. </summary>
        /// <returns> style read from the edge shape. </returns>
        public virtual IDictionary<string, string> getStyleFromEdgeShape(double parentHeight)
        {
            styleMap[mxVsdxConstants.VSDX_ID] = this.Id.ToString();

            // Rotation.
            //		double rotation = this.getRotation();
            //		rotation = Math.round(rotation);
            //		
            //		String rotationString = getLabelRotation() ? "1" : "0";
            //
            //		if (!rotationString.equals("1") && rotation != 90 && rotation != 270)
            //		{
            //			styleMap.put(mxConstants.STYLE_HORIZONTAL, rotationString);
            //		}
            //
            //		if (rotation != 0 && rotation != 360)
            //		{
            //			rotation = rotation * 100/100;
            //
            //			styleMap.put(mxConstants.STYLE_ROTATION, Double.toString(rotation));
            //		}

            //Defines Edge Shape
            IDictionary<string, string> edgeShape = Form;

            if (edgeShape != null && !edgeShape.Equals(""))
            {
                //JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
                foreach (var item in edgeShape)
                {
                    styleMap.Add(item.Key, item.Value);
                }
                //styleMap.putAll(edgeShape);
            }

            //Defines Pattern
            if (Dashed)
            {
                styleMap[mxConstants.STYLE_DASHED] = "1";

                string dashPattern = DashPattern;

                if (!string.ReferenceEquals(dashPattern, null))
                {
                    styleMap[mxConstants.STYLE_DASH_PATTERN] = dashPattern;
                }
            }

            //Defines Begin Arrow
            string startArrow = getEdgeMarker(true);

            if (!string.ReferenceEquals(startArrow, null))
            {
                if (startArrow.StartsWith(ARROW_NO_FILL_MARKER, StringComparison.Ordinal))
                {
                    startArrow = startArrow.Substring(ARROW_NO_FILL_MARKER.Length);
                    styleMap[mxConstants.STYLE_STARTFILL] = "0";
                }
                styleMap[mxConstants.STYLE_STARTARROW] = startArrow;
            }

            //Defines End Arrow
            string endArrow = getEdgeMarker(false);

            if (!string.ReferenceEquals(endArrow, null))
            {
                if (endArrow.StartsWith(ARROW_NO_FILL_MARKER, StringComparison.Ordinal))
                {
                    endArrow = endArrow.Substring(ARROW_NO_FILL_MARKER.Length);
                    styleMap[mxConstants.STYLE_ENDFILL] = "0";
                }
                styleMap[mxConstants.STYLE_ENDARROW] = endArrow;
            }

            //Defines the start arrow size.
            float saSize = StartArrowSize * 100 / 100;

            if (saSize != 6)
            {
                styleMap[mxConstants.STYLE_STARTSIZE] = Convert.ToString(saSize);
            }

            //Defines the end arrow size.
            float faSize = FinalArrowSize * 100 / 100;

            if (faSize != 6)
            {
                styleMap[mxConstants.STYLE_ENDSIZE] = Convert.ToString(faSize);
            }

            //Defines the line width
            double lWeight = LineWidth * 100 / 100;

            if (lWeight != 1.0)
            {
                styleMap[mxConstants.STYLE_STROKEWIDTH] = Convert.ToString(lWeight);
            }

            // Color
            string color = StrokeColor;

            if (!color.Equals(""))
            {
                styleMap[mxConstants.STYLE_STROKECOLOR] = color;
            }

            // Shadow
            if (Shadow)
            {
                styleMap[mxConstants.STYLE_SHADOW] = mxVsdxConstants.TRUE;
            }

            if (isConnectorBigNameU(NameU))
            {
                styleMap[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_ARROW;
                string fillcolor = FillColor;

                if (!fillcolor.Equals(""))
                {
                    styleMap[mxConstants.STYLE_FILLCOLOR] = fillcolor;
                }
            }

            //Defines label top spacing
            double topMargin = TopSpacing * 100 / 100;
            styleMap[mxConstants.STYLE_SPACING_TOP] = Convert.ToString(topMargin);

            //Defines label bottom spacing
            double bottomMargin = BottomSpacing * 100 / 100;
            styleMap[mxConstants.STYLE_SPACING_BOTTOM] = Convert.ToString(bottomMargin);

            //Defines label left spacing
            double leftMargin = LeftSpacing * 100 / 100;
            styleMap[mxConstants.STYLE_SPACING_LEFT] = Convert.ToString(leftMargin);

            //Defines label right spacing
            double rightMargin = RightSpacing * 100 / 100;
            styleMap[mxConstants.STYLE_SPACING_RIGHT] = Convert.ToString(rightMargin);

            //Defines label vertical align
            string verticalAlign = AlignVertical;
            styleMap[mxConstants.STYLE_VERTICAL_ALIGN] = verticalAlign;

            //Defines Label Rotation
            //		styleMap.put(mxConstants.STYLE_HORIZONTAL, getLabelRotation());

            styleMap["html"] = "1";

            resolveCommonStyles();
            //		System.out.println(this.getId());
            //		System.out.println(Arrays.toString(styleMap.entrySet().toArray()));

            return this.styleMap;
        }

        /// <summary>
        /// Analyzes a edge shape and returns a string with the style. </summary>
        /// <returns> style read from the edge shape. </returns>
        public virtual IDictionary<string, string> resolveCommonStyles()
        {
            /// <summary>
            /// LABEL BACKGROUND COLOR * </summary>
            string lbkgnd = this.getTextBkgndColor(this.getCellElement(mxVsdxConstants.TEXT_BKGND));

            if (!lbkgnd.Equals(""))
            {
                this.styleMap[mxConstants.STYLE_LABEL_BACKGROUNDCOLOR] = lbkgnd;
            }

            /// <summary>
            /// ROUNDING * </summary>
            this.styleMap[mxConstants.STYLE_ROUNDED] = Rounded ? mxVsdxConstants.TRUE : mxVsdxConstants.FALSE;

            return styleMap;
        }

        /// <summary>
        /// Returns the arrow of the line. </summary>
        /// <returns> Type of arrow. </returns>
        public virtual string getEdgeMarker(bool start)
        {
            string marker = this.getValue(this.getCellElement(start ? mxVsdxConstants.BEGIN_ARROW : mxVsdxConstants.END_ARROW), "0");

            int val = 0;
            try
            {
                if (marker.Equals("Themed"))
                {
                    mxVsdxTheme theme = Theme;

                    if (theme != null)
                    {
                        val = Vertex ? theme.getEdgeMarker(start, QuickStyleVals) : theme.getConnEdgeMarker(start, QuickStyleVals);

                    }
                }
                else
                {
                    val = int.Parse(marker);
                }
            }
            catch (Exception)
            {
                // ignore
            }

            string type = VsdxShape.arrowTypes[val];

            if (val > 0 && string.ReferenceEquals(type, null))
            {
                //if arrow  head type is not supported, use the open arrow instead
                type = VsdxShape.arrowTypes[1];
            }

            return type;
        }

        /// <summary>
        /// Locates the first entry for the specified style string in the style hierarchy.
        /// The order is to look locally, then delegate the request to the relevant parent style
        /// if it doesn't exist locally </summary>
        /// <param name="key"> The key of the style to find </param>
        /// <returns> the Element that first resolves to that style key or null or none is found </returns>
        protected internal override Element getCellElement(string key)
        {
            Element elem = base.getCellElement(key);

            if (elem == null && this.masterShape != null)
            {
                return this.masterShape.getCellElement(key);
            }

            return elem;
        }

        protected internal override Element getCellElement(string cellKey, string index, string sectKey)
        {
            Element elem = base.getCellElement(cellKey, index, sectKey);

            if (elem == null && this.masterShape != null)
            {
                return this.masterShape.getCellElement(cellKey, index, sectKey);
            }

            return elem;
        }

        /// <summary>
        /// Creates a sub shape for <b>shape</b> that contains the label. Used internally, when the label is positioned by an anchor. </summary>
        /// <param name="graph"> </param>
        /// <param name="shape"> the shape we want to create the label for </param>
        /// <param name="parent"> </param>
        /// <param name="parentHeight"> </param>
        /// <returns> label sub-shape </returns>
        public virtual mxCell createLabelSubShape(mxGraph graph, mxCell parent)
        {
            double txtWV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_WIDTH), Width);
            double txtHV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_HEIGHT), Height);
            double txtLocPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_X), txtWV / 2.0);
            double txtLocPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_Y), txtHV / 2.0);
            double txtPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_X), txtLocPinXV);
            double txtPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_Y), txtLocPinYV);
            double txtAngleV = getValueAsDouble(getShapeNode(mxVsdxConstants.TXT_ANGLE), 0);

            string textLabel = TextLabel;

            if (!string.ReferenceEquals(textLabel, null) && textLabel.Length > 0)
            {
                IDictionary<string, string> styleMap = new Dictionary<string, string>(StyleMap);
                styleMap[mxConstants.STYLE_FILLCOLOR] = mxConstants.NONE;
                styleMap[mxConstants.STYLE_STROKECOLOR] = mxConstants.NONE;
                styleMap[mxConstants.STYLE_GRADIENTCOLOR] = mxConstants.NONE;

                //We don't need to override these attributes in order to properly align the text
                if (!styleMap.ContainsKey("align"))
                {
                    styleMap["align"] = "center";
                }
                if (!styleMap.ContainsKey("verticalAlign"))
                {
                    styleMap["verticalAlign"] = "middle";
                }
                if (!styleMap.ContainsKey("whiteSpace"))
                {
                    styleMap["whiteSpace"] = "wrap";
                }

                // Doesn't make sense to set a shape, it's not rendered and doesn't affect the text perimeter
                styleMap.Remove("shape");
                //image should be set for the parent shape only
                styleMap.Remove("image");
                //styleMap.put("html", "1");

                double rotation = Rotation;

                if (txtAngleV != 0)
                {
                    double labRot = 360 -Common.ToDegrees(txtAngleV); //Math.toDegrees(txtAngleV);

                    labRot = Math.Round(((labRot + rotation) % 360.0) * 100.0) / 100.0;

                    if (labRot != 0.0)
                    {
                        styleMap["rotation"] = Convert.ToString(labRot);
                    }
                }

                string style = "text;" + mxVsdxUtils.getStyleString(styleMap, "=");

                double y = parent.Geometry.Height - (txtPinYV + txtHV - txtLocPinYV);
                double x = txtPinXV - txtLocPinXV;



                if (rotation > 0)
                {
                    mxGeometry tmpGeo = new mxGeometry(x, y, txtWV, txtHV);
                    mxGeometry pgeo = parent.Geometry;
                    double hw = pgeo.Width / 2, hh = pgeo.Height / 2;
                    mxVsdxCodec.rotatedPoint(tmpGeo, rotation, hw, hh);
                    x = tmpGeo.X;
                    y = tmpGeo.Y;
                }

                mxCell v1 = (mxCell)graph.insertVertex(parent, null, textLabel, x, y, txtWV, txtHV, style + ";html=1;");

                return v1;
            }

            return null;
        }

        public virtual mxPoint getLblEdgeOffset(mxGraphView view, IList<mxPoint> points)
        {
            if (points != null && points.Count > 1)
            {
                //find mxGraph label offset
                mxCellState state = new mxCellState();
                state.AbsolutePoints = points;
                view.updateEdgeBounds(state);
                mxPoint mxOffset = view.getPoint(state);
                mxPoint p0 = points[0];
                mxPoint pe = points[points.Count - 1];

                //Calculate the text offset
                double txtWV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_WIDTH), Width);
                double txtHV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_HEIGHT), Height);
                double txtLocPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_X), 0);
                double txtLocPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_Y), 0);
                double txtPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_X), 0);
                double txtPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_Y), 0);

                double y = (Height - (p0.Y - pe.Y)) / 2 + p0.Y - mxOffset.Y - (txtPinYV - txtLocPinYV + txtHV / 2);
                double x = txtPinXV - txtLocPinXV + txtWV / 2 + (p0.X - mxOffset.X);

                //FIXME one file has txtPinX/Y values extremely high which cause draw.io to hang
                //			<Cell N='TxtPinX' V='-1.651384506429589E199' F='SETATREF(Controls.TextPosition)'/>
                //			<Cell N='TxtPinY' V='1.183491078740126E185' F='SETATREF(Controls.TextPosition.Y)'/>
                if (Math.Abs(x) > 10e10)
                {
                    return null;
                }

                return new mxPoint(x, y);
            }
            else
            {
                return null;
            }
        }

        public virtual int ShapeIndex
        {
            get
            {
                return shapeIndex;
            }
            set
            {
                this.shapeIndex = value;
            }
        }


    }
}