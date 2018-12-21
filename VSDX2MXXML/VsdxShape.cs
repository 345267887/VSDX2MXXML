using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace VSDX2MXXML
{
    class VsdxShape : Shape
    {
        private const String ARROW_NO_FILL_MARKER = "0";

        /**
         * Number of d.p. to round non-integers to
         */
        static public int maxDp = 2;

        // For debugging to switch off shape matching by name
        static public bool USE_SHAPE_MATCH = true;

        /**
         * Whether or not to assume HTML labels
         */
        public bool htmlLabels = true;

        /**
         * Master Shape referenced by the shape.
         */
        protected Shape masterShape;

        /**
         * Master element referenced by the shape.
         */
        protected mxVsdxMaster master;

        /**
         * If the shape is a sub shape, this is a reference to its root shape, otherwise null
         */
        protected VsdxShape rootShape { get { return rootShape==null?this: rootShape; }set { rootShape = value; } }

        public double parentHeight;

        /**
         * The prefix of the shape name
         */
        protected String shapeName = null;

        /**
         * Shape index
         */
        protected int shapeIndex = 0;

        /**
         * Whether this cell is a vertex
         */
        protected bool vertex = true;

        protected Dictionary<int, VsdxShape> childShapes = new Dictionary<int, VsdxShape>();



        public static List<String> OFFSET_ARRAY = new List<String> { "Organizational unit", "Domain 3D" };

        public const String stencilTemplate = "<shape h=\"htemplate\" w=\"wtemplate\" aspect=\"variable\" strokewidth=\"inherit\"><connections></connections><background></background><foreground></foreground></shape>";

        public static float[] arrowSizes = { 2, 3, 5, 7, 9, 22, 45 };

        public static Dictionary<int, String> arrowTypes = new Dictionary<int, String>() {
            { 0, mxConstants.NONE},
        {1, mxConstants.ARROW_OPEN},
        {2, "blockThin"},
        {3, mxConstants.ARROW_OPEN},
        {4, mxConstants.ARROW_BLOCK},
        {5, mxConstants.ARROW_CLASSIC},
        {10, mxConstants.ARROW_OVAL},
        {13, mxConstants.ARROW_BLOCK},

        {14, ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK},
        {17, ARROW_NO_FILL_MARKER + mxConstants.ARROW_CLASSIC},
        {20, ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL},
        {22, ARROW_NO_FILL_MARKER + "diamond"},

        {23, "dash"},
        {24, "ERone"},
        {25, "ERmandOne"},
        {27, "ERmany"},
        {28, "ERoneToMany"},
        {29, "ERzeroToMany"},
        {30, "ERzeroToOne"},
		
		//approximations
		{6, mxConstants.ARROW_BLOCK},
        {7, mxConstants.ARROW_OPEN},
        {8, mxConstants.ARROW_CLASSIC},

        {9, "openAsync"},
        {11, "diamond"},

        {12, mxConstants.ARROW_OPEN},

        {15, ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK},
        {16, ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK},
        {18, ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK},
        {19, ARROW_NO_FILL_MARKER + mxConstants.ARROW_CLASSIC},
        {21, ARROW_NO_FILL_MARKER + "diamond"},
        {26, "ERmandOne"},

        {31, ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL},
        {32, ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL},
        {33, ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL},
        {34, ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL},

        {35, mxConstants.ARROW_OVAL},
        {36, mxConstants.ARROW_OVAL},
        {37, mxConstants.ARROW_OVAL},
        {38, mxConstants.ARROW_OVAL},

        {39, mxConstants.ARROW_BLOCK},
        {40, ARROW_NO_FILL_MARKER + mxConstants.ARROW_BLOCK},

        {41, ARROW_NO_FILL_MARKER + mxConstants.ARROW_OVAL},
        {42, mxConstants.ARROW_OVAL},

        {43, mxConstants.ARROW_OPEN},
        {44, mxConstants.ARROW_OPEN},
        {45, mxConstants.ARROW_OPEN}
            };

        //    static
        //	{
        //		try
        //		{
        //			mxResources.add("com/mxgraph/io/vdx/resources/edgeNameU");
        //			mxResources.add("com/mxgraph/io/vdx/resources/nameU");

        //			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();

        //        dbf.setFeature("http://apache.org/xml/features/nonvalidating/load-external-dtd", false);
        //			dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
        //			dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
        //			dbf.setExpandEntityReferences(false);
        //			dbf.setXIncludeAware(false);

        //			docBuilder = dbf.newDocumentBuilder();
        //		}
        //		catch (Exception e)
        //		{
        //			// todo
        //		}

        //arrowTypes 
        //	}



        /**
         * Create a new instance of mxVdxShape.
         * This method get the references to the master element, master shape
         * and stylesheet.
         * @param shape
         */
        public VsdxShape(mxVsdxPage page, XmlElement shape, bool vertex, Dictionary<String, mxVsdxMaster> masters, mxVsdxMaster _master, mxVsdxModel model) : base(shape, model)
        {


            String masterId = this.getMasterId();
            String masterShapeLocal = this.getShapeMasterId();

            if (masterId != null)
            {
                this.master = masters[masterId];
            }
            else
            {
                this.master = _master;
            }

            if (this.master != null)
            {
                // Check if the master ID corresponds to the one passed in. If it doesn't, or doesn't
                // exist on this shape, this shape is within a group that has that master

                if (masterId == null && masterShapeLocal != null)
                {
                    this.masterShape = this.master.getSubShape(masterShapeLocal);
                }
                else
                {
                    this.masterShape = this.master.getMasterShape();
                }
            }

            if (this.debug != null && this.masterShape != null)
            {
                this.masterShape.debug = this.debug;
            }

            String name = getNameU();
            int index = name.LastIndexOf(".");

            if (index != -1)
            {
                name = name.Substring(0, index);
            }

            this.shapeName = name;

            // Get sub-shapes
            XmlNodeList shapesList = shape.GetElementsByTagName(mxVsdxConstants.SHAPES);

            if (shapesList != null && shapesList.Count > 0)
            {
                XmlElement shapesElement = (XmlElement)shapesList[0];
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

            mxVsdxTheme theme = model.getThemes()[themeIndex];
            int variant = page.getCellIntValue("VariationColorIndex", 0);

            setThemeAndVariant(theme, variant);

            foreach (var entry in childShapes)
            {
                VsdxShape childShape = entry.Value;
                childShape.setRootShape(this);

                if (childShape.theme == null)
                {
                    childShape.setThemeAndVariant(theme, variant);
                }
            }

            quickStyleVals = new QuickStyleVals(
                    int.Parse(this.getValue(this.getCellElement("QuickStyleEffectsMatrix"), "0")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleFillColor"), "1")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleFillMatrix"), "0")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleFontColor"), "1")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleFontMatrix"), "0")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleLineColor"), "1")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleLineMatrix"), "0")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleShadowColor"), "1")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleType"), "0")),
                    int.Parse(this.getValue(this.getCellElement("QuickStyleVariation"), "0")));

            //process shape geometry
            if (masterShape != null)
            {
                masterShape.processGeomList(null);
                processGeomList(masterShape.getGeomList());

                //recalculate width and height using master data
                if (this.width == 0) this.width = getScreenNumericalValue(getCellElement(mxVsdxConstants.WIDTH), 0);

                if (this.height == 0) this.height = getScreenNumericalValue(getCellElement(mxVsdxConstants.HEIGHT), 0);
            }
            else
            {
                processGeomList(null);
            }
            //several shapes have beginX/Y and also has a fill color, thus it is better to render it as a vertex
            //vsdx can have an edge as a group!
            this.vertex = vertex || (childShapes != null && childShapes.Count>0) || (geomList != null && geomList.Count>0);
        }

        /**
         * Locates the first entry for the specified attribute string in the shape hierarchy.
         * The order is to look locally, then delegate the request to the master shape
         * if it doesn't exist locally
         * @param key The key of the shape to find
         * @return the Element that first resolves to that shape key or null or none is found
         */
        public XmlElement getShapeNode(String key)
        {
            XmlElement elem = this.cellElements[key];

            if (elem == null && this.masterShape != null)
            {
                return this.masterShape.getCellElement(key);
            }

            return elem;
        }

        /**
         * Returns the value of the Text element.<br/>
         * If the shape has no text, it is obtained from the master shape.
         * @return Text label of the shape.
         */
        public String getTextLabel()
        {
            String hideText = this.getValue(this.getCellElement(mxVsdxConstants.HIDE_TEXT), "0");

            if ("1".Equals(hideText))
            {
                return null;
            }

            XmlNodeList txtChildren = getTextChildren();

            if (txtChildren == null && masterShape != null)
            {
                txtChildren = masterShape.getTextChildren();
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
                    this.styleMap.Add(mxConstants.STYLE_VERTICAL_ALIGN, getAlignVertical());
                    this.styleMap.Add(mxConstants.STYLE_ALIGN, getHorizontalAlign("0", false));

                    return getHtmlTextContent(txtChildren);
                    //				}
                }
            }
            else
            {
                String text = this.getText();

                if (text == null && masterShape != null)
                {
                    return masterShape.getText();
                }
                else
                {
                    return text;
                }
            }

            return null;
        }

        /**
         * Initialises the text labels
         * @param children the text Elements
         */
        protected void initLabels(XmlNodeList children)
        {
            // Lazy init
            paragraphs = new Dictionary<string, Paragraph>();
            String ch = null;
            String pg = null;
            String fld = null;

            for (int index = 0; index < children.Count; index++)
            {
                String value = null;
                XmlNode node = children[index];
                String nodeName = node.Name;

                switch (nodeName)
                {
                    case "cp":
                        {
                            XmlElement elem = (XmlElement)node;
                            ch = elem.GetAttribute("IX");
                        }
                        break;
                    case "tp":
                        {
                            // TODO
                            XmlElement elem = (XmlElement)node;
                            elem.GetAttribute("IX");
                        }
                        break;
                    case "pp":
                        {
                            XmlElement elem = (XmlElement)node;
                            pg = elem.GetAttribute("IX");
                        }
                        break;
                    case "fld":
                        {
                            XmlElement elem = (XmlElement)node;
                            fld = elem.GetAttribute("IX");
                            break;
                        }
                    case "#text":
                        {
                            value = StringUtils.chomp(node.InnerText);

                            // Assumes text is always last
                            // null key is allowed
                            Paragraph para = paragraphs[pg];

                            if (para == null)
                            {
                                para = new Paragraph(value, ch, pg, fld);
                                paragraphs.Add(pg, para);
                            }
                            else
                            {
                                para.addText(value, ch, fld);
                            }
                        } break;
                }
            }
        }

        /**
         * 
         * @param index
         * @return
         */
        protected String createHybridLabel(String index)
        {
            Paragraph para = this.paragraphs[index];

            // Paragraph
            this.styleMap.Add(mxConstants.STYLE_ALIGN, getHorizontalAlign(index, false));
            this.styleMap.Add(mxConstants.STYLE_SPACING_LEFT, getIndentLeft(index));
            this.styleMap.Add(mxConstants.STYLE_SPACING_RIGHT, getIndentRight(index));
            this.styleMap.Add(mxConstants.STYLE_SPACING_TOP, getSpBefore(index));
            this.styleMap.Add(mxConstants.STYLE_SPACING_BOTTOM, getSpAfter(index));
            //this.styleMap.put("text-indent", getIndentFirst(index));
            this.styleMap.Add(mxConstants.STYLE_VERTICAL_ALIGN, getAlignVertical());

            this.styleMap.Add("fontColor", getTextColor(index));
            this.styleMap.Add("fontSize", (Double.Parse(this.getTextSize(index))).ToString());
            this.styleMap.Add("fontFamily", getTextFont(index));

            // Character
            int fontStyle = isBold(index) ? mxConstants.FONT_BOLD : 0;
            fontStyle |= isItalic(index) ? mxConstants.FONT_ITALIC : 0;
            fontStyle |= isUnderline(index) ? mxConstants.FONT_UNDERLINE : 0;
            this.styleMap.Add("fontStyle",(fontStyle).ToString());

            //Commented out as the method getTextOpacity returns value between 0 and 1 instead of 0 - 100
            //		this.styleMap.put(mxConstants.STYLE_TEXT_OPACITY, getTextOpacity(index));

            int numValues = para.numValues();
            String result = null;

            for (int i = 0; i < numValues; i++)
            {
                String value = para.getValue(i);

                if (string.IsNullOrEmpty(value) && this.fields != null)
                {
                    String fieldIx = para.getField(i);

                    if (fieldIx != null)
                    {
                        value = this.fields[fieldIx];

                        if (value == null && masterShape != null && masterShape.fields != null)
                        {
                            value = masterShape.fields.get(fieldIx);
                        }
                    }
                }

                if (value != null)
                {
                    result = result == null ? value : result + value;
                }
            }

            return result;
        }


        /**
         * Returns the text contained in the shape formated with tags html.<br/>
         * @return Text content in html.
         */
        public String getHtmlTextContent(XmlNodeList txtChildren)
        {
            String ret = "";
            bool first = true;

            if (txtChildren != null && txtChildren.Count > 0)
            {
                for (int index = 0; index < txtChildren.Count; index++)
                {
                    XmlNode node = txtChildren[index];

                    if (node.Name.Equals("cp"))
                    {
                        XmlElement elem = (XmlElement)node;
                        cp = elem.GetAttribute("IX");
                    }
                    else if (node.Name.Equals("tp"))
                    {
                        XmlElement elem = (XmlElement)node;
                        tp = elem.GetAttribute("IX");
                    }
                    else if (node.Name.Equals("pp"))
                    {
                        XmlElement elem = (XmlElement)node;
                        pp = elem.getAttribute("IX");

                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            ret += "</p>";
                        }

                        String para = "<p>";
                        ret += getTextParagraphFormated(para);
                    }
                    else if (node.Name.Equals("fld"))
                    {
                        XmlElement elem = (XmlElement)node;
                        fld = elem.GetAttribute("IX");

                        String text = null;

                        if (this.fields != null)
                        {
                            text = this.fields[fld];
                        }

                        if (text == null && masterShape != null && masterShape.fields != null)
                        {
                            text = masterShape.fields[fld];
                        }

                        if (text != null)
                            ret += processLblTxt(text);
                    }
                    else if (node.Name.Equals("#text"))
                    {
                        String text = node.InnerText;

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

            String end = first ? "" : "</p>";
            ret += end;
            mxVsdxUtils.surroundByTags(ret, "div");

            return ret;
        }

        private String processLblTxt(String text)
        {
            // It's HTML text, so escape it.
            text = mxVsdxUtils.htmlEntities(text);

            text = textToList(text, pp);

            //text = text.replaceAll("\n", "<br/>").replaceAll(UNICODE_LINE_SEP, "<br/>");
            text = text.Replace("\n", "<br/>").Replace(UNICODE_LINE_SEP, "<br/>");

            return getTextCharFormated(text);
        }

        /**
         * Checks if a nameU is for big connectors.
         * @param nameU NameU attribute.
         * @return Returns <code>true</code> if a nameU is for big connectors.
         */
        public bool isConnectorBigNameU(String nameU)
        {
            return nameU.StartsWith("60 degree single")
            || nameU.StartsWith("45 degree single")
            || nameU.StartsWith("45 degree double")
            || nameU.StartsWith("60 degree double")
            || nameU.StartsWith("45 degree  tail")
            || nameU.StartsWith("60 degree  tail")
            || nameU.StartsWith("45 degree tail")
            || nameU.StartsWith("60 degree tail")
            || nameU.StartsWith("Flexi-arrow 2")
            || nameU.StartsWith("Flexi-arrow 1")
            || nameU.StartsWith("Flexi-arrow 3")
            || nameU.StartsWith("Double flexi-arrow")
            || nameU.StartsWith("Fancy arrow");
        }

        /**
         * Checks if the shape represents a vertex.
         * @return Returns <code>true</code> if the shape represents a vertex.
         */
        public new bool isVertex()
        {
            return vertex;
        }

        /**
         * Returns the coordinates of the top left corner of the Shape.
         * When a coordinate is not found, it is taken from masterShape.
         * @param parentHeight Height of the parent cell of the shape.
         * @param rotation whether to allow for cell rotation
         * @return mxPoint that represents the coordinates
         */
        public mxPoint getOriginPoint(double parentHeight, bool rotation)
        {
            double px = this.getPinX();
            double py = this.getPinY();
            double lpy = this.getLocPinY();
            double lpx = this.getLocPinX();

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

                    //double cos = Math.Cos(Math.toRadians(360 - this.rotation));
                    //double sin = Math.Sin(Math.toRadians(360 - this.rotation));

                    double cos = Math.Cos(Common.ToRadians(360 - this.rotation));
                    double sin = Math.Sin(Common.ToRadians(360 - this.rotation));

                    return new mxPoint(x + vecX - (vecX * cos - vecY * sin), (vecX * sin + vecY * cos) + y - vecY);
                }
            }

            return new mxPoint(x, y);
        }

        /**
         * Returns the width and height of the Shape expressed like an mxPoint.<br/>
         * x = width<br/>
         * y = height<br/>
         * When a dimension is not found, it is taken from masterShape.
         * @return mxPoint that represents the dimensions of the shape.
         */
        public mxPoint getDimensions()
        {
            double w = getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.WIDTH), 0);
            double h = getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.HEIGHT), 0);

            return new mxPoint(w, h);
        }

        /**
         * Returns the value of the pinX element.
         * @return The shape pinX element
         */
        public double getPinX()
        {
            return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.PIN_X), 0);
        }

        /**
         * Returns the value of the pinY element in pixels.
         * @return Numerical value of the pinY element.
         */
        public double getPinY()
        {
            return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.PIN_Y), 0);
        }

        /**
         * Returns the value of the locPinX element in pixels.
         * @return Numerical value of the pinY element.
         */
        public double getLocPinX()
        {
            return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.LOC_PIN_X), 0);
        }

        /**
         * Returns the value of the locPinY element in pixels.
         * @return Numerical value of the locPinY element.
         */
        public double getLocPinY()
        {
            return getScreenNumericalValue(this.getShapeNode(mxVsdxConstants.LOC_PIN_Y), 0);

        }

        /**
         * Returns the opacity of the Shape.<br/>
         * @return Double in the range of (transparent = 0)..(100 = opaque)
         */
        private double getOpacity(String key)
        {
            double opacity = 100;

            if (this.isGroup())
            {
                opacity = 0;
            }

            opacity = getValueAsDouble(this.getCellElement(key), 0);

            opacity = 100 - opacity * 100;
            opacity = Math.Max(opacity, 0);
            opacity = Math.Min(opacity, 100);

            return opacity;
        }

        /**
         * Returns the background color for apply in the gradient.<br/>
         * If no gradient must be applicated, returns an empty string.
         * @return hexadecimal representation of the color.
         */
        private String getGradient()
        {
            String gradient = "";
            String fillPattern = this.getValue(this.getCellElement(mxVsdxConstants.FILL_PATTERN), "0");

            //		if (fillPattern.equals("25") || fillPattern.equals("27") || fillPattern.equals("28") || fillPattern.equals("30"))
            //approximate all gradients of vsdx with mxGraph one
            if (int.Parse(fillPattern) >= 25)
            {
                gradient = this.getColor(this.getCellElement(mxVsdxConstants.FILL_BKGND));
            }
            else
            {
                mxVsdxTheme theme = getTheme();

                if (theme != null)
                {
                    Color gradColor = theme.getFillGraientColor(getQuickStyleVals());
                    if (gradColor != null) gradient = gradColor.toHexStr();
                }
            }

            return gradient;
        }

        /**
         * Returns the direction of the gradient.<br/>
         * If no gradient has to be applied, returns an empty string.
         * @return Direction.(east, west, north or south)
         */
        private String getGradientDirection()
        {
            String direction = "";
            String fillPattern = this.getValue(this.getCellElement(mxVsdxConstants.FILL_PATTERN), "0");

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

        /**
         * Returns the rotation of the shape.<br/>
         * @return Rotation of the shape in degrees.
         */
        public double calcRotation()
        {
            double rotation = Double.Parse(this.getValue(this.getCellElement(mxVsdxConstants.ANGLE), "0"));

            //rotation = Math.toDegrees(rotation);
            rotation = Common.ToRadians(rotation);
            rotation = rotation % 360;
            rotation = rotation * 100 / 100;

            return 360 - rotation;
        }

        /**
         * Used to pass in a parents rotation to the child
         * @param parentRotation the rotation of the parent
         */
        public void propagateRotation(double parentRotation)
        {
            this.rotation += parentRotation;
            this.rotation %= 360;
            this.rotation = this.rotation * 100 / 100;
        }

        /**
         * Returns the top spacing of the label in pixels.<br/>
         * The property may to be defined in master shape or text stylesheet.<br/>
         * @return Top spacing in double precision.
         */
        public double getTopSpacing()
        {
            double topMargin = this.getTextTopMargin();
            topMargin = (topMargin / 2 - 2.8) * 100 / 100;
            return topMargin;
        }

        /**
         * Returns the bottom spacing of the label in pixels.<br/>
         * The property may to be defined in master shape or text stylesheet.<br/>
         * @return Bottom spacing in double precision.
         */
        public double getBottomSpacing()
        {
            double bottomMargin = this.getTextBottomMargin();
            bottomMargin = (bottomMargin / 2 - 2.8) * 100 / 100;
            return bottomMargin;
        }

        /**
         * Returns the left spacing of the label in pixels.<br/>
         * The property may to be defined in master shape or text stylesheet.<br/>
         * @return Left spacing in double precision.
         */
        public double getLeftSpacing()
        {
            double leftMargin = this.getTextLeftMargin();
            leftMargin = (leftMargin / 2 - 2.8) * 100 / 100;
            return leftMargin;
        }

        /**
         * Returns the right spacing of the label in pixels.<br/>
         * The property may to be defined in master shape or text stylesheet.<br/>
         * @return Right spacing in double precision.
         */
        public double getRightSpacing()
        {
            double rightMargin = this.getTextRightMargin();
            rightMargin = (rightMargin / 2 - 2.8) * 100 / 100;
            return rightMargin;
        }


        /**
         * Checks if the label must be rotated.<br/>
         * The property may to be defined in master shape or text stylesheet.<br/>
         * @return Returns <code>true<code/> if the label should remain horizontal.
         */
        public bool getLabelRotation()
        {
            bool hor = true;
            //Defines rotation.
            double rotation = this.calcRotation();
            double angle = Double.Parse(this.getValue(this.getCellElement(mxVsdxConstants.TXT_ANGLE), "0"));

            //angle = Math.toDegrees(angle);
            angle = Common.ToDegrees(angle);
            angle = angle - rotation;

            if (!(Math.Abs(angle) < 45 || Math.Abs(angle) > 270))
            {
                hor = false;
            }

            return hor;
        }

        /**
         * Analyzes the shape and returns a string with the style.
         * @return style read from the shape.
         */
        public Dictionary<String, String> getStyleFromShape()
        {
            styleMap.Add(mxVsdxConstants.VSDX_ID, this.getId().ToString());

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
                styleMap.Add(mxConstants.STYLE_ROTATION, (this.rotation).ToString());
            }

            // Fill color
            String fillcolor = getFillColor();

            if (!fillcolor.Equals(""))
            {
                styleMap.Add(mxConstants.STYLE_FILLCOLOR, fillcolor);
            }
            else
            {
                styleMap.Add(mxConstants.STYLE_FILLCOLOR, "none");
            }

            int id = this.getId();

            this.styleDebug("ID = " + id + " , Fill Color = " + fillcolor);

            //Defines gradient
            String gradient = getGradient();

            if (!gradient.Equals(""))
            {
                styleMap.Add(mxConstants.STYLE_GRADIENTCOLOR, gradient);
                String gradientDirection = getGradientDirection();

                if (!gradientDirection.Equals("") && !gradientDirection.Equals(mxConstants.DIRECTION_SOUTH))
                {
                    styleMap.Add(mxConstants.STYLE_GRADIENT_DIRECTION, gradientDirection);
                }
            }
            else
            {
                styleMap.Add(mxConstants.STYLE_GRADIENTCOLOR, "none");
            }

            double opacity = this.getOpacity(mxVsdxConstants.FILL_FOREGND_TRANS);

            if (opacity < 100)
            {
                styleMap.Add(mxConstants.STYLE_FILL_OPACITY,(opacity).ToString());
            }

            opacity = this.getOpacity(mxVsdxConstants.LINE_COLOR_TRANS);

            if (opacity < 100)
            {
                styleMap.Add(mxConstants.STYLE_STROKE_OPACITY, (opacity).ToString());
            }

            Dictionary<String, String> form = getForm();

            if (form.ContainsKey(mxConstants.STYLE_SHAPE) &&
                    (form[mxConstants.STYLE_SHAPE].StartsWith("image;")))
            {
                styleMap.Add(mxConstants.STYLE_WHITE_SPACE, "wrap");
            }


            //styleMap.putAll(form);

            foreach (var item in form)
            {
                styleMap.Add(item.Key,item.Value);
            }


            //Defines line Pattern
            if (isDashed())
            {
                styleMap.Add(mxConstants.STYLE_DASHED, "1");

                String dashPattern = getDashPattern();

                if (dashPattern != null)
                {
                    styleMap.Add(mxConstants.STYLE_DASH_PATTERN, dashPattern);
                }
            }

            String color = getStrokeColor();
            double tr = this.getStrokeTransparency();

            this.styleDebug("ID = " + id + " , Color = " + color + " , stroke transparency = " + tr);

            if (!color.Equals("") && tr != 1)
            {
                styleMap.Add(mxConstants.STYLE_STROKECOLOR, color);
            }
            else
            {
                //styleMap.put(mxConstants.STYLE_STROKECOLOR, "none");
            }

            //Defines the line width
            double lWeight = getLineWidth() * 100 / 100;

            if (lWeight != 1)
            {
                styleMap.Add(mxConstants.STYLE_STROKEWIDTH, (lWeight).ToString());
            }

            /** SHADOW **/
            if (isShadow())
            {
                styleMap.Add(mxConstants.STYLE_SHADOW, mxVsdxConstants.TRUE);
            }

            //Defines label top spacing
            double topMargin = getTopSpacing() * 100 / 100;

            if (topMargin != 0)
            {
                styleMap.Add(mxConstants.STYLE_SPACING_TOP, (topMargin).ToString());
            }

            //Defines label bottom spacing
            double bottomMargin = getBottomSpacing() * 100 / 100;

            if (bottomMargin != 0)
            {
                styleMap.Add(mxConstants.STYLE_SPACING_BOTTOM, (bottomMargin).ToString());
            }

            //Defines label left spacing
            double leftMargin = getLeftSpacing() * 100 / 100;

            if (leftMargin != 0)
            {
                styleMap.Add(mxConstants.STYLE_SPACING_LEFT, (leftMargin).ToString());
            }

            //Defines label right spacing
            double rightMargin = getRightSpacing() * 100 / 100;

            if (rightMargin != 0)
            {
                styleMap.Add(mxConstants.STYLE_SPACING_RIGHT,(rightMargin).ToString());
            }

            String direction = getDirection(form);

            if (direction != mxConstants.DIRECTION_EAST)
            {
                styleMap.Add(mxConstants.STYLE_DIRECTION, direction);
            }

            String flibX = getValue(this.getCellElement(mxVsdxConstants.FLIP_X), "0");
            String flibY = getValue(this.getCellElement(mxVsdxConstants.FLIP_Y), "0");

            if ("1".Equals(flibX))
            {
                styleMap.Add(mxConstants.STYLE_FLIPH, "1");
            }

            if ("1".Equals(flibY))
            {
                styleMap.Add(mxConstants.STYLE_FLIPV, "1");
            }

            resolveCommonStyles();

            return this.styleMap;
        }

        private String getDashPattern()
        {
            List<Double> pattern = null;

            String linePattern = this.getValue(this.getCellElement(mxVsdxConstants.LINE_PATTERN), "0");

            if (linePattern.Equals("Themed"))
            {
                mxVsdxTheme theme = getTheme();

                if (theme != null)
                {
                    pattern = isVertex() ? theme.getLineDashPattern(getQuickStyleVals()) : theme.getConnLineDashPattern(getQuickStyleVals());
                }
            }
            else
            {
                pattern = getLineDashPattern(int.Parse(linePattern));
            }

            if (pattern != null && pattern.Count>0)
            {
                StringBuilder str = new StringBuilder();

                foreach (Double len in pattern)
                {
                    //string.Format("%.2f ", len)
                    str.Append(len.ToString("0.00"));
                }
                return str.ToString().Trim();
            }
            return null;
        }

        /**
         * Checks if the lines of the shape are dashed.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * @return Returns <code>true</code> if the lines of the shape are dashed.
         */
        public bool isDashed()
        {
            String linePattern = this.getValue(this.getCellElement(mxVsdxConstants.LINE_PATTERN), "0");

            if (linePattern.Equals("Themed"))
            {
                mxVsdxTheme theme = getTheme();

                if (theme != null)
                {
                    return isVertex() ? theme.isLineDashed(getQuickStyleVals()) : theme.isConnLineDashed(getQuickStyleVals());
                }
            }
            else if (!(linePattern.Equals("0") || linePattern.Equals("1")))
            {
                return true;
            }

            return false;
        }

        /**
         * Returns the line width.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * @return Line width in pixels.
         */
        public double getLineWidth()
        {
            String lineWeight = getValue(this.getCellElement(mxVsdxConstants.LINE_WEIGHT), "1");

            double lWeight = 1;
            try
            {
                if (lineWeight.Equals("Themed"))
                {
                    mxVsdxTheme theme = getTheme();

                    if (theme != null)
                    {
                        lWeight = (isVertex() ? theme.getLineWidth(getQuickStyleVals()) : theme.getConnLineWidth(getQuickStyleVals())) / 10000.0;
                    }
                }
                else
                {
                    lWeight = Double.Parse(lineWeight);
                    lWeight = getScreenNumericalValue(lWeight);
                }
            }
            catch (Exception e)
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

        /**
         * Returns the start arrow size.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * Determines the value in pixels of each arrow size category in .vdx.
         * @return Size in pixels.
         */
        public float getStartArrowSize()
        {
            String baSize = getValue(this.getCellElement(mxVsdxConstants.BEGIN_ARROW_SIZE), "4");

            try
            {
                int size = 4;

                if (baSize.Equals("Themed"))
                {
                    mxVsdxTheme theme = getTheme();

                    if (theme != null)
                    {
                        size = isVertex() ? theme.getStartSize(getQuickStyleVals()) : theme.getConnStartSize(getQuickStyleVals());
                    }
                }
                else
                {
                    size = int.Parse(baSize);
                }

                return VsdxShape.arrowSizes[size];
            }
            catch (Exception e)
            {
                // ignore
            }

            return 4;
        }

        /**
         * Returns the end arrow size.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * Determines the value in pixels of each arrow size category in .vdx.
         * @return Size in pixels.
         */
        public float getFinalArrowSize()
        {
            String eaSize = getValue(this.getCellElement(mxVsdxConstants.END_ARROW_SIZE), "4");

            try
            {
                int size = 4;

                if (eaSize.Equals("Themed"))
                {
                    mxVsdxTheme theme = getTheme();

                    if (theme != null)
                    {
                        size = isVertex() ? theme.getEndSize(getQuickStyleVals()) : theme.getConnEndSize(getQuickStyleVals());
                    }
                }
                else
                {
                    size = int.Parse(eaSize);
                }

                return VsdxShape.arrowSizes[size];
            }
            catch (Exception e)
            {
                // ignore
            }

            return 4;
        }

        /**
         * Returns whether the cell is Rounded.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * @return Returns <code>true</code> if the cell is Rounded.
         */
        public bool isRounded()
        {
            String val = getValue(this.getCellElement(mxVsdxConstants.ROUNDING), "0");

            if ("Themed".Equals(val))
            {
                //TODO add theme support 
                val = "0";
            }
            return Double.Parse(val) > 0;
        }

        /**
         * Return if the line has shadow.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * @return Returns <code>mxVdxConstants.TRUE</code> if the line has shadow.
         */
        public bool isShadow()
        {
            // https://msdn.microsoft.com/en-us/library/office/jj230454.aspx TODO
            // double shdwShow = this.getNumericalValue(this.getStyleNode(mxVdxConstants.SHDW_PATTERN), 0);

            String shdw = this.getValue(this.getCellElement(mxVsdxConstants.SHDW_PATTERN), "0");

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

        /**
         * Returns the style of the edge. (Orthogonal or straight)
         * @return Edge Style.
         */
        public Dictionary<String, String> getEdgeStyle(Dictionary<String, String> edgeShape)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            String edgeName = edgeShape[mxConstants.STYLE_SHAPE];

            if (edgeName.Equals("mxgraph.lean_mapping.electronic_info_flow_edge"))
            {
                result.Add(mxConstants.STYLE_EDGE, mxConstants.NONE);
                return result;
            }
            else
            {
                result.Add(mxConstants.STYLE_EDGE, mxConstants.EDGESTYLE_ELBOW);
                return result;
            }
            //		else
            //		{
            //			result.put(mxConstants.STYLE_EDGE, mxConstants.NONE);
            //			return result;
            //		}
        }

        /**
         * Returns the master's Id of the Shape.
         * @return Master's ID of the shape, null if has not a master.
         */
        public String getMasterId()
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

        /**
         * Returns the masterShape's Id of the shape.
         * @return Master Shape's ID of the shape, null if has not a master shape.
         */
        public String getShapeMasterId()
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

        /**
         * Checks if a shape contains other shapes inside.
         * @return Returns <code>true</code> if a shape contains other shapes inside.
         */
        public bool isGroup()
        {
            return shape.GetAttribute("Type").Equals("Group");
        }

        /**
         * Checks if a shape contains other shapes inside.
         * @return Returns <code>true</code> if a shape contains other shapes inside.
         */
        public static String getType(XmlElement shape)
        {
            return shape.GetAttribute("Type");
        }

        public mxVsdxMaster getMaster()
        {
            return master;
        }

        /**
         * Returns the NameU attribute.
         * @return Value of the NameU attribute.
         */
        public String getNameU()
        {
            String result = shape.GetAttribute(mxVsdxConstants.NAME_U);

            if ((result == null || result.Equals("")) && masterShape != null)
            {
                result = masterShape.getNameU();
            }

            return result;
        }

        /**
         * Returns the Name attribute.
         * @return Value of the Name attribute (Human readable name).
         */
        public String getName()
        {
            String result = shape.GetAttribute(mxVsdxConstants.NAME);

            if ((result == null || result.Equals("")) && masterShape != null)
            {
                result = masterShape.getName();
            }

            return result;
        }

        /**
         * Returns the master name of the shape
         * @return Master name of the shape
         */
        public String getMasterName()
        {
            return shapeName;

        }

        public void setLabelOffset(mxCell vertex, String style)
        {
            String nameU = "";
            String masterNameU = "";

            if (shape.HasAttribute(mxVsdxConstants.NAME_U))
            {
                nameU = shape.GetAttribute(mxVsdxConstants.NAME_U);
            }


            if (this.getMaster() != null && this.getMaster().getMasterElement() != null)
            {
                if (this.getMaster().getMasterElement().hasAttribute(mxVsdxConstants.NAME_U))
                {
                    masterNameU = this.getMaster().getMasterElement().getAttribute(mxVsdxConstants.NAME_U);
                }
            }

            //check for shape name/type, because of different (shape specific) treatment of each
            if (nameU.StartsWith("Organizational unit")
                    || masterNameU.StartsWith("Organizational unit"))
            {
                XmlElement control = (XmlElement)shape.GetElementsByTagName(mxVsdxConstants.CONTROL).item(0);

                XmlElement xEl = null;
                String xS = "0.0";
                XmlElement yEl = null;
                String yS = "-0.4";

                if (control != null)
                {
                    xEl = (XmlElement)control.GetElementsByTagName(mxVsdxConstants.X)[0];

                    if (xEl.HasAttribute("F"))
                    {
                        xS = xEl.GetAttribute("F");
                    }
                    else
                    {
                        xS = xEl.InnerText;
                    }

                    yEl = (XmlElement)control.GetElementsByTagName(mxVsdxConstants.Y)[0];

                    if (yEl.HasAttribute("F"))
                    {
                        yS = yEl.GetAttribute("F");
                    }
                    else
                    {
                        yS = yEl.InnerText;
                    }
                }

                mxGeometry geometry = vertex.getGeometry();

                //clean the formula strings and hope it will work with a specific algorithm
                //xS = xS.Replace("Width/2+", "");
                //xS = xS.Replace("DL", "");
                //yS = yS.Replace("Height*", "");
                xS = Regex.Replace( xS,"Width/2+", "");
                xS = Regex.Replace(xS,"DL", "");
                yS = Regex.Replace(yS,"Height*", "");

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

                String[] styleArray = style.Split(';');
                String tabHeight = "";

                for (int i = 0; i < styleArray.Length; i++)
                {
                    String currStyle = styleArray[i];
                    currStyle = currStyle.Trim();

                    if (currStyle.StartsWith("tabHeight="))
                    {
                        tabHeight = currStyle.Replace("tabHeight=", "");
                    }
                }

                if (tabHeight.Equals(""))
                {
                    tabHeight = "20";
                }

                Double tH = Double.Parse(tabHeight);

                Double x = Double.Parse(xS);
                Double y = Double.Parse(yS);
                Double h = geometry.getHeight();
                Double xFinal = geometry.getWidth() * 0.1 + x * 100;
                Double yFinal = h - h * y - tH / 2;
                mxPoint offset = new mxPoint(xFinal, yFinal);
                vertex.getGeometry().setOffset(offset);
            }
            else if (nameU.StartsWith("Domain 3D")
                    || masterNameU.StartsWith("Domain 3D"))
            {
                XmlElement control = (XmlElement)shape.GetElementsByTagName(mxVsdxConstants.CONTROL)[0];

                XmlElement xEl = null;
                String xS = "0.0";
                XmlElement yEl = null;
                String yS = "-0.4";

                if (control != null)
                {
                    xEl = (XmlElement)control.GetElementsByTagName(mxVsdxConstants.X).item(0);
                    xS = xEl.GetAttribute("F");
                    yEl = (XmlElement)control.GetElementsByTagName(mxVsdxConstants.Y).item(0);
                    yS = yEl.GetAttribute("F");
                }

                mxGeometry geometry = vertex.getGeometry();

                //clean the formula strings and hope it will work with a specific algorithm
                //xS = xS.replace("Width/2+", "");
                //xS = xS.replace("DL", "");
                //yS = yS.replace("Height*", "");
                xS = Regex.Replace(xS, "Width/2+", "");
                xS = Regex.Replace(xS, "DL", "");
                yS = Regex.Replace(yS, "Height*", "");

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

                Double x = Double.Parse(xS);
                Double y = Double.Parse(yS);
                Double h = geometry.getHeight();
                Double xFinal = geometry.getWidth() * 0.1 + x * 100;
                Double yFinal = h - h * y;
                mxPoint offset = new mxPoint(xFinal, yFinal);
                vertex.getGeometry().setOffset(offset);
            }
        }

        /**
         * Returns the constant that represents the Shape.
         * @return String that represent the form.
         */
        public Dictionary<String, String> getForm()
        {
            Dictionary<String, String> result = new Dictionary<String, String>();

            this.styleDebug("Looking to match shape = " + shapeName);

            if (shapeName != null && !shapeName.Equals("") && VsdxShape.USE_SHAPE_MATCH)
            {
                String trans = mxResources[shapeName];

                if (trans != null && !trans.Equals(""))
                {
                    this.styleDebug("Translation = " + trans);
                    result.Add(mxConstants.STYLE_SHAPE, trans);
                    return result;
                }
            }

            if (this.isVertex())
            {
                try
                {
                    String type = VsdxShape.getType(this.getShape());
                    // String foreignType = "";
                    this.styleDebug("shape type = " + type);

                    //The master may contain the foreign object data
                    if (this.imageData != null || (mxVsdxConstants.FOREIGN.Equals(type) && masterShape != null && masterShape.imageData != null))
                    {
                        Dictionary<String, String> imageData = this.imageData != null ? this.imageData : masterShape.imageData;

                        result.Add("shape", "image");
                        result.Add("aspect", "fixed");
                        String iType = imageData["iType"];
                        String iData = imageData["iData"];

                        result.Add("image", "data:image/" + iType + "," + iData);
                        return result;
                    }

                    //Shape inherit master geometry and can change some of it or override it completely. So, no need to parse the master instead of the shape itself
                    String parsedGeom = this.parseGeom();

                    if (parsedGeom.Equals(""))
                    {
                        this.styleDebug("No geom found");
                        return result;
                    }

                    String stencil = Utils.encodeURIComponent(parsedGeom, "UTF-8");

                    byte[] bytes = stencil.getBytes("UTF-8");
                    Deflater deflater = new Deflater(Deflater.BEST_COMPRESSION, true);
                    deflater.setInput(bytes);
                    ByteArrayOutputStream outputStream = new ByteArrayOutputStream();

                    deflater.finish();

                    byte[] buffer = new byte[1024];

                    while (!deflater.finished())
                    {
                        int count = deflater.deflate(buffer);
                        outputStream.write(buffer, 0, count);
                    }

                    try
                    {
                        outputStream.close();
                    }
                    catch (IOException e)
                    {
                        // TODO Auto-generated catch block
                        e.printStackTrace();
                    }

                    byte[] output = outputStream.toByteArray();
                    deflater.end();

                    byte[] encoded = Base64.encodeBase64(output);
                    String enc = new String(encoded, "UTF-8");

                    result.put(mxConstants.STYLE_SHAPE, "stencil(" + enc + ")");
                }
                //catch (UnsupportedEncodingException e)
                //{
                //    // TODO Auto-generated catch block
                //    //e.printStackTrace();
                //}
            }
            else
            {
                return getEdgeStyle();
            }

            return result;
        }

        /**
         * Checks if a shape may to be imported like an Off page reference.
         * @return Returns <code>true</code> if a shape may to be imported like an Off page reference.
         */
        public bool isOff_page_reference()
        {
            String name = getNameU();

            if (name.Equals("Off-page reference") || name.Equals("Lined/Shaded process"))
            {
                return true;
            }

            return false;
        }

        /**
         * Checks if a shape may to be imported like an External process.
         * @return Returns <code>true</code> if a shape may to be imported like an External process.
         */
        public bool isExternal_process()
        {
            return (shapeName.Equals("External process"));
        }

        /**
         * Returns the direction of the shape.
         * @param form Form of the shape.
         * @return Direction(south, north, east and south)
         */
        public String getDirection(Dictionary<String, String> form)
        {
            String offsetS = (String)mxResources["mxOffset" + shapeName];

            if (offsetS == null || offsetS.Equals("0") || offsetS.Equals(""))
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

        /**
         * Checks if a shape may to be imported like a Sub-process.
         * This method is approximated.
         * @return Returns <code>true</code> if a shape may to be imported like a
         * Sub-process.
         */
        public bool isSubproces()
        {
            return shapeName.Equals("Subproces");
        }

        /**
         * @return style map containing the proper shape and style (if needed) of a Visio "dynamic connector" edge
         */
        public Dictionary<String, String> getEdgeStyle()
        {
           Dictionary<String, String> result = new Dictionary<String, String>();

            result.Add("edgeStyle", "none");
            return result;

            //result.put("edgeStyle", "orthogonalEdgeStyle");
            //return result;

            //result.put("curved", "1");
            //return result;

            //return null;
        }

        public Dictionary<int, VsdxShape> getChildShapes()
        {
            return childShapes;
        }

        public void setChildShapes(Dictionary<int, VsdxShape> childShapes)
        {
            this.childShapes = childShapes;
        }

        public bool isDisplacedLabel()
        {
            String txtPinXF = this.getAttribute(mxVsdxConstants.TXT_PIN_X, "F", "");
            String txtPinYF = this.getAttribute(mxVsdxConstants.TXT_PIN_Y, "F", "");
            String txtWidthF = this.getAttribute(mxVsdxConstants.TXT_WIDTH, "F", "");
            String txtHeightF = this.getAttribute(mxVsdxConstants.TXT_HEIGHT, "F", "");

            if (masterShape != null)
            {
                if (txtPinXF == "" || txtPinXF.ToLower().Equals("inh"))
                {
                    txtPinXF = masterShape.getAttribute(mxVsdxConstants.TXT_PIN_X, "F", "");
                }

                if (txtPinYF == "" || txtPinYF.ToLower().Equals("inh"))
                {
                    txtPinYF = masterShape.getAttribute(mxVsdxConstants.TXT_PIN_Y, "F", "");
                }

                if (txtWidthF == "" || txtWidthF.ToLower().Equals("inh"))
                {
                    txtWidthF = masterShape.getAttribute(mxVsdxConstants.TXT_WIDTH, "F", "");
                }

                if (txtHeightF == "" || txtHeightF.ToLower().Equals("inh"))
                {
                    txtHeightF = masterShape.getAttribute(mxVsdxConstants.TXT_HEIGHT, "F", "");
                }
            }

            if (txtPinXF.ToLower().Equals("width*0.5") &&
                    txtPinYF.ToLower().Equals("height*0.5") &&
                    txtWidthF.ToLower().Equals("width*1") &&
                    txtHeightF.ToLower().Equals("height*1"))
            {
                return false;
            }
            else if (txtPinXF.ToLower().StartsWith("width*") &&
                    txtPinYF.ToLower().StartsWith("height*") &&
                    txtWidthF.ToLower().StartsWith("width*") &&
                    txtHeightF.ToLower().StartsWith("height*"))
            //		else if (txtPinXF.toLowerCase().startsWith("width*") &&
            //				txtPinYF.toLowerCase().startsWith("height*"))
            {
                return true;
            }
            else if (txtPinXF.ToLower().StartsWith("controls.row_") ||
                    txtPinYF.ToLower().StartsWith("controls.row_"))
            {
                return true;
            }

            return false;
        }

        public bool isRotatedLabel()
        {
            String txtAngleValue = this.getAttribute(mxVsdxConstants.TXT_ANGLE, "V", "");

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

        public void setRootShape(VsdxShape shape)
        {
            rootShape = shape;
        }

        public VsdxShape getRootShape()
        {
            return rootShape;
        }

        // Edge specific methods 

        /**
         * Returns the coordinates of the begin point of an Edge Shape.
         * @param parentHeight Height of the parent of the shape.
         * @return mxPoint that represents the coordinates.
         */
        public mxPoint getStartXY(double parentHeight)
        {
            double startX = getScreenNumericalValue(this.getCellElement(mxVsdxConstants.BEGIN_X), 0);
            double startY = parentHeight - getScreenNumericalValue(this.getCellElement(mxVsdxConstants.BEGIN_Y), 0);

            return new mxPoint(startX, startY);
        }

        /**
         * Returns the coordinates of the end point of an Edge Shape.
         * @param parentHeight Height of the parent of the shape.
         * @return mxPoint that represents the coordinates.
         */
        public mxPoint getEndXY(double parentHeight)
        {
            double endX = getScreenNumericalValue(this.getCellElement(mxVsdxConstants.END_X), 0);
            double endY = parentHeight - getScreenNumericalValue(this.getCellElement(mxVsdxConstants.END_Y), 0);

            return new mxPoint(endX, endY);
        }

        /**
         * Returns the list of routing points of a edge shape.
         * @param parentHeight Height of the parent of the shape.
         * @return List of mxPoint that represents the routing points.
         */
        public List<mxPoint> getRoutingPoints(double parentHeight, mxPoint startPoint, double rotation/*, boolean flipX, boolean flipY*/)
        {
            if (geomList != null)
            {
                return geomList.getRoutingPoints(parentHeight, startPoint, rotation);
            }
            return null;
        }

        /**
         * Returns the list of control points of a edge shape.
         * @param parentHeight Height of the parent of the shape.
         * @return List of mxPoint that represents the control points.
         */
        public List<mxPoint> getControlPoints(double parentHeight)
        {
            mxPoint startXY = getStartXY(parentHeight);
            mxPoint endXY = getEndXY(parentHeight);
            List<mxPoint> pointList = new List<mxPoint>();

            if (shape != null)
            {
                XmlNodeList geomList = shape.GetElementsByTagName(mxVsdxConstants.GEOM);

                if (geomList.Count> 0)
                {
                    XmlElement firstGeom = (XmlElement)geomList[0];
                    XmlElement firstNURBS = (XmlElement)firstGeom.GetElementsByTagName(mxVsdxConstants.NURBS_TO)[0];
                    XmlElement firstE = (XmlElement)firstNURBS.GetElementsByTagName("E")[0];

                    if (firstE != null)
                    {
                        String f = firstE.GetAttribute("F");
                        //f = f.replaceAll("NURBS\\(", "");
                        //f = f.replaceAll("\\)", "");
                        //f = f.replaceAll(",", " ");
                        //f = f.replaceAll("\\s\\s", " ");
                        f = f.Replace("NURBS\\(", "");
                        f = f.Replace("\\)", "");
                        f = f.Replace(",", " ");
                        f = f.Replace("\\s\\s", " ");
                        String[] pointsS = f.Split(' ');
                        double[] pointsRaw = new double[pointsS.Length];

                        for (int i = 0; i < pointsS.Length; i++)
                        {
                            pointsRaw[i] = Double.Parse(pointsS[i]);
                        }

                        for (int i = 2; i + 4 < pointsS.Length; i = i + 4)
                        {
                            mxPoint currPoint = new mxPoint();
                            double rawX = pointsRaw[i + 2];
                            double rawY = pointsRaw[i + 3];
                            double width = Math.Abs(endXY.getX() - startXY.getX());
                            double widthFixed = Math.Min(100, width);
                            double heightFixed = 100;
                            double finalX = 0;

                            finalX = startXY.getX() + widthFixed * rawX;
                            currPoint.setX(finalX);
                            currPoint.setY(startXY.getY() - heightFixed * rawY);
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

        /**
         * Analyzes a edge shape and returns a string with the style.
         * @return style read from the edge shape.
         */
        public Dictionary<String, String> getStyleFromEdgeShape(double parentHeight)
        {
            styleMap.Add(mxVsdxConstants.VSDX_ID, this.getId().HasValue? this.getId().Value.ToString():"");

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
            Dictionary<String, String> edgeShape = getForm();

            if (edgeShape != null && !edgeShape.Equals(""))
            {
                foreach (var item in edgeShape)
                {
                    styleMap.Add(item.Key, item.Value);
                }
                //styleMap.putAll();
            }

            //Defines Pattern
            if (isDashed())
            {
                styleMap.Add(mxConstants.STYLE_DASHED, "1");

                String dashPattern = getDashPattern();

                if (dashPattern != null)
                {
                    styleMap.Add(mxConstants.STYLE_DASH_PATTERN, dashPattern);
                }
            }

            //Defines Begin Arrow
            String startArrow = getEdgeMarker(true);

            if (startArrow != null)
            {
                if (startArrow.StartsWith(ARROW_NO_FILL_MARKER))
                {
                    startArrow = startArrow.Substring(ARROW_NO_FILL_MARKER.length());
                    styleMap.Add(mxConstants.STYLE_STARTFILL, "0");
                }
                styleMap.Add(mxConstants.STYLE_STARTARROW, startArrow);
            }

            //Defines End Arrow
            String endArrow = getEdgeMarker(false);

            if (endArrow != null)
            {
                if (endArrow.StartsWith(ARROW_NO_FILL_MARKER))
                {
                    endArrow = endArrow.Substring(ARROW_NO_FILL_MARKER.Length);
                    styleMap.Add(mxConstants.STYLE_ENDFILL, "0");
                }
                styleMap.Add(mxConstants.STYLE_ENDARROW, endArrow);
            }

            //Defines the start arrow size.
            float saSize = getStartArrowSize() * 100 / 100;

            if (saSize != 6)
            {
                styleMap.Add(mxConstants.STYLE_STARTSIZE, (saSize).ToString());
            }

            //Defines the end arrow size.
            float faSize = getFinalArrowSize() * 100 / 100;

            if (faSize != 6)
            {
                styleMap.Add(mxConstants.STYLE_ENDSIZE, (faSize).ToString());
            }

            //Defines the line width
            double lWeight = getLineWidth() * 100 / 100;

            if (lWeight != 1.0)
            {
                styleMap.Add(mxConstants.STYLE_STROKEWIDTH, (lWeight).ToString());
            }

            // Color
            String color = getStrokeColor();

            if (!color.Equals(""))
            {
                styleMap.Add(mxConstants.STYLE_STROKECOLOR, color);
            }

            // Shadow
            if (isShadow())
            {
                styleMap.Add(mxConstants.STYLE_SHADOW, mxVsdxConstants.TRUE);
            }

            if (isConnectorBigNameU(getNameU()))
            {
                styleMap.Add(mxConstants.STYLE_SHAPE, mxConstants.SHAPE_ARROW);
                String fillcolor = getFillColor();

                if (!fillcolor.Equals(""))
                {
                    styleMap.Add(mxConstants.STYLE_FILLCOLOR, fillcolor);
                }
            }

            //Defines label top spacing
            double topMargin = getTopSpacing() * 100 / 100;
            styleMap.Add(mxConstants.STYLE_SPACING_TOP, (topMargin).ToString());

            //Defines label bottom spacing
            double bottomMargin = getBottomSpacing() * 100 / 100;
            styleMap.Add(mxConstants.STYLE_SPACING_BOTTOM, (bottomMargin).ToString());

            //Defines label left spacing
            double leftMargin = getLeftSpacing() * 100 / 100;
            styleMap.Add(mxConstants.STYLE_SPACING_LEFT,(leftMargin).ToString());

            //Defines label right spacing
            double rightMargin = getRightSpacing() * 100 / 100;
            styleMap.Add(mxConstants.STYLE_SPACING_RIGHT, (rightMargin).ToString());

            //Defines label vertical align
            String verticalAlign = getAlignVertical();
            styleMap.Add(mxConstants.STYLE_VERTICAL_ALIGN, verticalAlign);

            //Defines Label Rotation
            //		styleMap.put(mxConstants.STYLE_HORIZONTAL, getLabelRotation());

            styleMap.Add("html", "1");

            resolveCommonStyles();
            //		System.out.println(this.getId());
            //		System.out.println(Arrays.toString(styleMap.entrySet().toArray()));

            return this.styleMap;
        }

        /**
         * Analyzes a edge shape and returns a string with the style.
         * @return style read from the edge shape.
         */
        public Dictionary<String, String> resolveCommonStyles()
        {
            /** LABEL BACKGROUND COLOR **/
            String lbkgnd = this.getTextBkgndColor(this.getCellElement(mxVsdxConstants.TEXT_BKGND));

            if (!lbkgnd.Equals(""))
            {
                this.styleMap.Add(mxConstants.STYLE_LABEL_BACKGROUNDCOLOR, lbkgnd);
            }

            /** ROUNDING **/
            this.styleMap.Add(mxConstants.STYLE_ROUNDED, isRounded() ? mxVsdxConstants.TRUE : mxVsdxConstants.FALSE);

            return styleMap;
        }

        /**
         * Returns the arrow of the line.
         * @return Type of arrow.
         */
        public String getEdgeMarker(bool start)
        {
            String marker = this.getValue(this.getCellElement(start ? mxVsdxConstants.BEGIN_ARROW : mxVsdxConstants.END_ARROW), "0");

            int val = 0;
            try
            {
                if (marker.Equals("Themed"))
                {
                    mxVsdxTheme theme = getTheme();

                    if (theme != null)
                    {
                        val = isVertex() ? theme.getEdgeMarker(start, getQuickStyleVals()) : theme.getConnEdgeMarker(start, getQuickStyleVals());

                    }
                }
                else
                {
                    val = int.Parse(marker);
                }
            }
            catch (Exception e)
            {
                // ignore
            }

            String type = VsdxShape.arrowTypes[val];

            if (val > 0 && type == null)
            {
                //if arrow  head type is not supported, use the open arrow instead
                type = VsdxShape.arrowTypes[1];
            }

            return type;
        }

        /**
         * Locates the first entry for the specified style string in the style hierarchy.
         * The order is to look locally, then delegate the request to the relevant parent style
         * if it doesn't exist locally
         * @param key The key of the style to find
         * @return the Element that first resolves to that style key or null or none is found
         */
        protected XmlElement getCellElement(String key)
        {
            XmlElement elem = base.getCellElement(key);

            if (elem == null && this.masterShape != null)
            {
                return this.masterShape.getCellElement(key);
            }

            return elem;
        }

        protected XmlElement getCellElement(String cellKey, String index, String sectKey)
        {
            XmlElement elem = base.getCellElement(cellKey, index, sectKey);

            if (elem == null && this.masterShape != null)
            {
                return this.masterShape.getCellElement(cellKey, index, sectKey);
            }

            return elem;
        }

        /**
         * Creates a sub shape for <b>shape</b> that contains the label. Used internally, when the label is positioned by an anchor.
         * @param graph
         * @param shape the shape we want to create the label for
         * @param parent
         * @param parentHeight
         * @return label sub-shape
         */
        public mxCell createLabelSubShape(mxGraph graph, mxCell parent)
        {
            double txtWV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_WIDTH), getWidth());
            double txtHV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_HEIGHT), getHeight());
            double txtLocPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_X), txtWV / 2.0);
            double txtLocPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_Y), txtHV / 2.0);
            double txtPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_X), txtLocPinXV);
            double txtPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_Y), txtLocPinYV);
            double txtAngleV = getValueAsDouble(getShapeNode(mxVsdxConstants.TXT_ANGLE), 0);

            String textLabel = getTextLabel();

            if (!string.IsNullOrEmpty(textLabel))
            {
                Dictionary<String, String> styleMap = new Dictionary<String, String>(getStyleMap());
                styleMap.Add(mxConstants.STYLE_FILLCOLOR, mxConstants.NONE);
                styleMap.Add(mxConstants.STYLE_STROKECOLOR, mxConstants.NONE);
                styleMap.Add(mxConstants.STYLE_GRADIENTCOLOR, mxConstants.NONE);

                //We don't need to override these attributes in order to properly align the text
                if (!styleMap.ContainsKey("align")) styleMap.Add("align", "center");
                if (!styleMap.ContainsKey("verticalAlign")) styleMap.Add("verticalAlign", "middle");
                if (!styleMap.ContainsKey("whiteSpace")) styleMap.Add("whiteSpace", "wrap");

                // Doesn't make sense to set a shape, it's not rendered and doesn't affect the text perimeter
                styleMap.Remove("shape");
                //image should be set for the parent shape only
                styleMap.Remove("image");
                //styleMap.put("html", "1");

                double rotation = getRotation();

                if (txtAngleV != 0)
                {
                    //double labRot = 360 - Math.toDegrees(txtAngleV);

                    double labRot = 360 - Common.ToDegrees(txtAngleV);

                    labRot = Math.Round(((labRot + rotation) % 360.0) * 100.0) / 100.0;

                    if (labRot != 0.0)
                    {
                        styleMap.Add("rotation", (labRot).ToString());
                    }
                }

                String style = "text;"
                        + mxVsdxUtils.getStyleString(styleMap, "=");

                double y = parent.getGeometry().getHeight() - (txtPinYV + txtHV - txtLocPinYV);
                double x = txtPinXV - txtLocPinXV;



                if (rotation > 0)
                {
                    mxGeometry tmpGeo = new mxGeometry(x, y, txtWV, txtHV);
                    mxGeometry pgeo = parent.getGeometry();
                    double hw = pgeo.getWidth() / 2, hh = pgeo.getHeight() / 2;
                    mxVsdxCodec.rotatedPoint(tmpGeo, rotation, hw, hh);
                    x = tmpGeo.getX();
                    y = tmpGeo.getY();
                }

                mxCell v1 = (mxCell)graph.insertVertex(parent, null, textLabel, x, y, txtWV, txtHV, style + ";html=1;");

                return v1;
            }

            return null;
        }

        public mxPoint getLblEdgeOffset(mxGraphView view, List<mxPoint> points)
        {
            if (points != null && points.size() > 1)
            {
                //find mxGraph label offset
                mxCellState state = new mxCellState();
                state.setAbsolutePoints(points);
                view.updateEdgeBounds(state);
                mxPoint mxOffset = view.getPoint(state);
                mxPoint p0 = points[0];
                mxPoint pe = points[points.Count - 1];

                //Calculate the text offset
                double txtWV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_WIDTH), getWidth());
                double txtHV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_HEIGHT), getHeight());
                double txtLocPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_X), 0);
                double txtLocPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_LOC_PIN_Y), 0);
                double txtPinXV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_X), 0);
                double txtPinYV = getScreenNumericalValue(getShapeNode(mxVsdxConstants.TXT_PIN_Y), 0);

                double y = (getHeight() - (p0.getY() - pe.getY())) / 2 + p0.getY() - mxOffset.getY() - (txtPinYV - txtLocPinYV + txtHV / 2);
                double x = txtPinXV - txtLocPinXV + txtWV / 2 + (p0.getX() - mxOffset.getX());

                //FIXME one file has txtPinX/Y values extremely high which cause draw.io to hang
                //			<Cell N='TxtPinX' V='-1.651384506429589E199' F='SETATREF(Controls.TextPosition)'/>
                //			<Cell N='TxtPinY' V='1.183491078740126E185' F='SETATREF(Controls.TextPosition.Y)'/>
                if (Math.Abs(x) > 10e10) return null;

                return new mxPoint(x, y);
            }
            else
            {
                return null;
            }
        }

        public int getShapeIndex()
        {
            return shapeIndex;
        }

        public void setShapeIndex(int shapeIndex)
        {
            this.shapeIndex = shapeIndex;
        }

	
    }
}
