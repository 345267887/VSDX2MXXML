using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class Style
    {
        protected XmlElement shape;

        protected int? Id;

        // .vsdx cells elements that contain one style each
        protected Dictionary<String, XmlElement> cellElements = new Dictionary<String, XmlElement>();

        protected Dictionary<String, Section> sections = new Dictionary<String, Section>();

        protected mxPropertiesManager pm;

        /**
         * Mapping of line,text and fill styles to the style parents
         */
        protected Dictionary<String, Style> styleParents = new Dictionary<String, Style>();

        protected Style style;

        public static bool vsdxStyleDebug = false;

        protected static Dictionary<String, String> styleTypes = new Dictionary<String, String>()
    {
            { mxVsdxConstants.FILL, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.FILL_BKGND, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.FILL_BKGND_TRANS, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.FILL_FOREGND, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.FILL_FOREGND_TRANS, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.FILL_PATTERN, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.SHDW_PATTERN, mxVsdxConstants.FILL_STYLE },
        {mxVsdxConstants.FILL_STYLE, mxVsdxConstants.FILL_STYLE },
        {"QuickStyleFillColor", mxVsdxConstants.FILL_STYLE },
        {"QuickStyleFillMatrix", mxVsdxConstants.FILL_STYLE },

        {mxVsdxConstants.BEGIN_ARROW, mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.END_ARROW, mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.LINE_PATTERN, mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.LINE_COLOR, mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.LINE_COLOR_TRANS, mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.LINE_WEIGHT, mxVsdxConstants.LINE_STYLE },
        {"QuickStyleLineColor", mxVsdxConstants.LINE_STYLE },
        {"QuickStyleLineMatrix", mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.BEGIN_ARROW_SIZE, mxVsdxConstants.LINE_STYLE },
        {mxVsdxConstants.END_ARROW_SIZE, mxVsdxConstants.LINE_STYLE },

        {mxVsdxConstants.TEXT_BKGND, mxVsdxConstants.TEXT_STYLE },
        {mxVsdxConstants.BOTTOM_MARGIN, mxVsdxConstants.TEXT_STYLE },
        {mxVsdxConstants.LEFT_MARGIN, mxVsdxConstants.TEXT_STYLE },
        {mxVsdxConstants.RIGHT_MARGIN, mxVsdxConstants.TEXT_STYLE },
        {mxVsdxConstants.TOP_MARGIN, mxVsdxConstants.TEXT_STYLE },
        {mxVsdxConstants.PARAGRAPH, mxVsdxConstants.TEXT_STYLE },
        {mxVsdxConstants.CHARACTER, mxVsdxConstants.TEXT_STYLE },
        {"QuickStyleFontColor", mxVsdxConstants.TEXT_STYLE },
        {"QuickStyleFontMatrix", mxVsdxConstants.TEXT_STYLE }
    };

        /**
         * Create a new instance of mxGeneralShape
         * @param shape Shape Element to be wrapped.
         */
        public Style(XmlElement shape, mxVsdxModel model)
        {
            this.shape = shape;
            this.pm = model.getPropertiesManager();

            String Id = shape.GetAttribute(mxVsdxConstants.ID);

            try
            {
                this.Id = (string.IsNullOrEmpty(Id)) ? int.Parse(Id) : -1;
            }
            catch (Exception e)
            {
                // TODO handle exception correctly
            }

            cacheCells(model);
            stylesheetRefs(model);
        }

        public mxVsdxTheme getTheme()
        {
            return null;
        }

        public QuickStyleVals getQuickStyleVals()
        {
            return null;
        }

        public bool isVertex()
        {
            return false;
        }

        public void styleDebug(String debug)
        {
            //if (vsdxStyleDebug)
            //{
            //    System.out.println(debug);
            //}
        }

        public void stylesheetRefs(mxVsdxModel model)
        {
            styleParents.Add(mxVsdxConstants.FILL_STYLE, model.getStylesheet(shape.GetAttribute(mxVsdxConstants.FILL_STYLE)));
            styleParents.Add(mxVsdxConstants.LINE_STYLE, model.getStylesheet(shape.GetAttribute(mxVsdxConstants.LINE_STYLE)));
            styleParents.Add(mxVsdxConstants.TEXT_STYLE, model.getStylesheet(shape.GetAttribute(mxVsdxConstants.TEXT_STYLE)));

            Style style = model.getStylesheet("0");
            this.style = style;
        }

        /**
         * Checks if the shape Element has a children with tag name = 'tag'.
         * @param tag Name of the Element to be found.
         * @return Returns <code>true</code> if the shape Element has a children with tag name = 'tag'
         */
        protected void cacheCells(mxVsdxModel model)
        {
            if (shape != null)
            {
                XmlNodeList children = shape.ChildNodes;

                if (children != null)
                {
                    XmlNode childNode = children[0];

                    while (childNode != null)
                    {
                        if (childNode is XmlElement)
                        {
                            parseShapeElem((XmlElement)childNode, model);
                        }

                        childNode = childNode.NextSibling;
                    }
                }
            }
        }

        /**
         * Caches the specified element
         * @param elem the element to cache
         */
        protected void parseShapeElem(XmlElement elem, mxVsdxModel model)
        {
            String childName = elem.Name;

            if (childName.Equals("Cell"))
            {
                this.cellElements.Add(elem.GetAttribute("N"), elem);
            }
            else if (childName.Equals("Section"))
            {
                this.parseSection(elem);
            }
        }

        /**
         * Caches the specific section element
         * @param elem the element to cache
         */
        protected void parseSection(XmlElement elem)
        {
            Section sect = new Section(elem);
            this.sections.Add(elem.GetAttribute("N"), sect);
        }

        /**
         * Checks if the 'primary' Element has a child with tag name = 'tag'.
         * @param tag Name of the Element to be found.
         * @return Returns <code>true</code> if the 'primary' Element has a child with tag name = 'tag'.
         */
        protected bool hasProperty(String nodeName, String tag)
        {
            return this.cellElements.ContainsKey(tag);
        }

        /**
         * Returns the value of the element
         * @param elem The element whose value is to be found
         * @param defaultValue the value to return if there is no value attribute
         * @return String value of the element, or the default value if no value found
         */
        protected String getValue(XmlElement elem, String defaultValue)
        {
            if (elem != null)
            {
                return elem.GetAttribute("V");
            }

            return defaultValue;
        }

        /**
         * Returns the value of the element as a double
         * @param elem The element whose value is to be found
         * @param defaultValue the value to return if there is no value attribute
         * @return double value of the element, or the default value if no value found
         */
        protected double getValueAsDouble(XmlElement cell, double defaultValue)
        {
            if (cell != null)
            {
                String value = cell.GetAttribute("V");

                if (value != null)
                {
                    if (value.Equals("Themed"))
                    {
                        return 0;
                    }

                    try
                    {
                        double parsedValue = Double.Parse(value);

                        String units = cell.GetAttribute("U");

                        if (units.Equals("PT"))
                        {
                            // Convert from points to pixels
                            parsedValue = parsedValue * mxVsdxUtils.conversionFactor;
                        }

                        return Math.Round(parsedValue * 100.0) / 100.0;
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return defaultValue;
        }

        //if (!tag.equals(mxVdxConstants.FILL_BKGND_TRANS) && !tag.equals(mxVdxConstants.FILL_FOREGND_TRANS) && !tag.equals(mxVdxConstants.LINE_COLOR_TRANS) && !tag.equals(mxVdxConstants.NO_LINE))

        /**
         * Returns the value of the element as a double
         * @param elem The element whose value is to be found
         * @param defaultValue the value to return if there is no value attribute
         * @return double value of the element, or the default value if no value found
         */
        protected double getScreenNumericalValue(XmlElement cell, double defaultValue)
        {
            if (cell != null)
            {
                String value = cell.GetAttribute("V");

                if (value != null)
                {
                    try
                    {
                        double parsedValue = Double.Parse(value);

                        return getScreenNumericalValue(parsedValue);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return defaultValue;
        }

        protected double getScreenNumericalValue(double val)
        {
            double conVal = val * mxVsdxUtils.conversionFactor;
            return Math.Round(conVal * 100.0) / 100.0;
        }

        /**
         * Returns the value of the attribute of the element with tag name = 'tag' in the children
         * of the shape element<br/>
         * @param tag Name of the Element to be found.
         * @return Numerical value of the element.
         */
        public String getAttribute(String tag, String attribute, String defaultValue)
        {
            String result = defaultValue;
            XmlElement cell = this.cellElements[tag];

            if (cell != null)
            {
                result = cell.GetAttribute(attribute);
            }

            return result;
        }

        protected Dictionary<String, String> getChildValues(XmlElement parent, Dictionary<String, String> requiredValues)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();

            XmlNode child = parent.FirstChild;

            while (child != null)
            {
                if (child is XmlElement)
                {
                    XmlElement childElem = (XmlElement)child;
                    String childName = childElem.Name;
                    String name = null;
                    String nodeValue = null;

                    if (childName.Equals("Cell"))
                    {
                        name = childElem.GetAttribute("N");
                        nodeValue = childElem.GetAttribute("V");
                    }
                    else
                    {
                        name = childElem.Name;
                        nodeValue = childElem.InnerText;
                    }

                    if (requiredValues != null)
                    {
                        String nodeOverride = requiredValues[name];

                        if (nodeOverride != null)
                        {
                            nodeValue = childElem.GetAttribute(nodeOverride);
                        }
                    }

                    result.Add(name, nodeValue);
                }

                child = child.NextSibling;
            }

            return result;
        }

        protected XmlElement getCellElement(String cellKey, String index, String sectKey)
        {
            Section sect = this.sections[sectKey];
            XmlElement elem = null;
            bool inherit = false;

            if (sect != null)
            {
                elem = sect.getIndexedCell(index, cellKey);
            }

            if (elem != null)
            {
                String form = elem.GetAttribute("F");
                String value = elem.GetAttribute("V");

                if (form != null && value != null)
                {
                    if (form.Equals("Inh") && value.Equals("Themed"))
                    {
                        inherit = true;
                    }
                    else if (form.Equals("THEMEVAL()") && value.Equals("Themed") && style != null)
                    {
                        //Handle theme here
                        //FIXME this is a very hacky way to test themes until fully integrating themes
                        if (mxVsdxConstants.COLOR.Equals(cellKey)) return elem;

                        // Use "no style" style
                        XmlElement themeElem = style.getCellElement(cellKey, index, sectKey);

                        if (themeElem != null)
                        {
                            return themeElem;
                        }
                    }
                }
            }

            if (elem == null || inherit)
            {
                String styleType = Style.styleTypes[sectKey];
                Style parentStyle = this.styleParents[styleType];

                if (parentStyle != null)
                {
                    XmlElement parentElem = parentStyle.getCellElement(cellKey, index, sectKey);

                    if (parentElem != null)
                    {
                        // Only return if non-null. Just in case (and not sure if that's valid) there is an
                        // inherit formula that doesn't resolve to anything
                        return parentElem;
                    }
                }
            }

            return elem;
        }

        /**
         * Locates the first entry for the specified style string in the style hierarchy.
         * The order is to look locally, then delegate the request to the relevant parent style
         * if it doesn't exist locally
         * @param key The key of the cell to find
         * @return the Element that first resolves to that style key or null or none is found
         */
        protected XmlElement getCellElement(String key)
        {
            XmlElement elem = this.cellElements[key];
            bool inherit = false;

            if (elem != null)
            {
                String form = elem.GetAttribute("F");
                String value = elem.GetAttribute("V");

                if (form != null && value != null)
                {
                    if (form.Equals("Inh") && value.Equals("Themed"))
                    {
                        inherit = true;
                    }
                    else if (form.Contains("THEMEVAL()") && value.Equals("Themed") && style != null)
                    {
                        //Handle theme here
                        //FIXME this is a very hacky way to test themes until fully integrating themes
                        if ("FillForegnd".Equals(key) || mxVsdxConstants.LINE_COLOR.Equals(key) || mxVsdxConstants.LINE_PATTERN.Equals(key)
                                || mxVsdxConstants.BEGIN_ARROW_SIZE.Equals(key) || mxVsdxConstants.END_ARROW_SIZE.Equals(key)
                                || mxVsdxConstants.BEGIN_ARROW.Equals(key) || mxVsdxConstants.END_ARROW.Equals(key)
                                || mxVsdxConstants.LINE_WEIGHT.Equals(key)) return elem;

                        // Use "no style" style
                        XmlElement themeElem = style.getCellElement(key);

                        if (themeElem != null)
                        {
                            return themeElem;
                        }
                    }
                }
            }

            if (elem == null || inherit)
            {
                String styleType = Style.styleTypes[key];
                Style parentStyle = this.styleParents[styleType];

                if (parentStyle != null)
                {
                    XmlElement parentElem = parentStyle.getCellElement(key);

                    if (parentElem != null)
                    {
                        // Only return if non-null. Just in case (and not sure if that's valid) there is an
                        // inherit formula that doesn't resolve to anything
                        return parentElem;
                    }
                }
            }

            return elem;
        }

        /**
         * Returns the line color.<br/>
         * The property may to be defined in master shape or line stylesheet.<br/>
         * @return hexadecimal representation of the color.
         */
        public String getStrokeColor()
        {
            String color = "";

            if (this.getValue(this.getCellElement(mxVsdxConstants.LINE_PATTERN), "1").Equals("0"))
            {
                color = "none";
            }
            else
            {
                color = this.getColor(this.getCellElement(mxVsdxConstants.LINE_COLOR));

                if ("Themed".Equals(color))
                {
                    mxVsdxTheme theme = getTheme();

                    if (theme != null)
                    {
                        System.Drawing.Color colorObj = isVertex() ? theme.getLineColor(getQuickStyleVals()) : theme.getConnLineColor(getQuickStyleVals());
                        color = colorObj.Name;
                    }
                    else
                    {
                        color = "";
                    }
                }
            }

            return color;
        }

        /**
         * Returns the shape's color.
         * The property may to be defined in master shape or fill stylesheet.
         * If the color is the background or the fore color, it depends on the pattern.
         * For simple gradients and solid, returns the fore color, else return the
         * background color.
         * @return hexadecimal representation of the color.
         */
        protected String getFillColor()
        {
            String fillForeColor = this.getColor(this.getCellElement(mxVsdxConstants.FILL_FOREGND));

            if ("Themed".Equals(fillForeColor))
            {
                mxVsdxTheme theme = getTheme();

                if (theme != null)
                {
                    Color color = theme.getFillColor(getQuickStyleVals());
                    fillForeColor = color.toHexStr();
                }
                else
                {
                    //One sample file has fill color as white when no theme is used and value is Themed!
                    fillForeColor = "#FFFFFF";
                }
            }

            String fillPattern = this.getValue(this.getCellElement(mxVsdxConstants.FILL_PATTERN), "0");

            if (fillPattern != null && fillPattern.Equals("0"))
            {
                return "none";
            }
            else
            {
                return fillForeColor;
            }
        }

        protected String getColor(XmlElement elem)
        {
            String color = this.getValue(elem, "");

            if (!"Themed".Equals(color) && !color.StartsWith("#"))
            {
                color = pm.getColor(color);
            }

            return color;
        }

        /**
         * The TextBkgnd cell can have any value from 0 through 24, or 255. The values 0 and 255 (visTxtBlklOpaque) both indicate a transparent text background.
         * To enter a custom color, use the RGB or HSL function plus one—for example, RGB(255,127,255)+1. The value of a custom color is its RGB color, and RGB(r, g, b)+1, 
         * rather than a number, will be shown in the ShapeSheet window. When used in numeric operations, custom colors have values of 25 and above.
         * You can set the transparency of the text background color in the TextBkgndTrans cell.
         */
        protected String getTextBkgndColor(XmlElement elem)
        {
            String color = this.getValue(elem, "");

            if (!color.StartsWith("#"))
            {
                if (color.Equals("0") || color.Equals("255"))
                {
                    return "none";
                }

                return pm.getColor((int.Parse(color) - 1).ToString());
            }

            return color;
        }

        /**
         * Returns the line weight of the shape in pixels
         * @return Numerical value of the LineWeight element.
         */
        public double getLineWeight()
        {
            return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.LINE_WEIGHT), 0);
        }

        /**
         * Returns the level of transparency of the Shape.
         * @return double in range (opaque = 0)..(100 = transparent)
         */
        public double getStrokeTransparency()
        {
            return getValueAsDouble(this.getCellElement(mxVsdxConstants.LINE_COLOR_TRANS), 0);
        }

        /**
         * Returns the NameU attribute.
         * @return Value of the NameU attribute.
         */
        public String getNameU()
        {
            return shape.GetAttribute(mxVsdxConstants.NAME_U);
        }

        /**
         * Returns the Name attribute.
         * @return Value of the Name attribute (Human readable name).
         */
        public String getName()
        {
            return shape.GetAttribute(mxVsdxConstants.NAME);
        }

        /**
         * Returns the UniqueID attribute.
         * @return Value of the UniqueID attribute.
         */
        public String getUniqueID()
        {
            return shape.GetAttribute(mxVsdxConstants.UNIQUE_ID);
        }

        /**
         * Returns the value of the Id attribute.
         * @return Value of the Id attribute.
         */
        public int? getId()
        {
            return this.Id;
        }

        /**
         * Returns the color of one text fragment
         * @param charIX IX attribute of Char element
         * @return Text color in hexadecimal representation.
         */
        public String getTextColor(String index)
        {
            XmlElement colorElem = getCellElement(mxVsdxConstants.COLOR, index, mxVsdxConstants.CHARACTER);
            String color = getValue(colorElem, "#000000");

            if ("Themed".Equals(color))
            {
                mxVsdxTheme theme = getTheme();

                if (theme != null)
                {
                    Color colorObj = isVertex() ? theme.getFontColor(getQuickStyleVals()) : theme.getConnFontColor(getQuickStyleVals());
                    color = colorObj.toHexStr();
                }
                else
                {
                    color = "#000000";
                }
            }
            else if (!color.StartsWith("#"))
            {
                color = pm.getColor(color);
            }

            return color;
        }

        /**
         * Returns the top margin of text in pixels.
         * @return Numerical value of the TopMargin element
         */
        public double getTextTopMargin()
        {
            return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.TOP_MARGIN), 0);
        }

        /**
         * Returns the bottom margin of text in pixels.
         * @return Numerical value of the BottomMargin element.
         */
        public double getTextBottomMargin()
        {
            return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.BOTTOM_MARGIN), 0);
        }

        /**
         * Returns the left margin of text in pixels.
         * @return Numerical value of the LeftMargin element.
         */
        public double getTextLeftMargin()
        {
            return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.LEFT_MARGIN), 0);
        }

        /**
         * Returns the right margin of text in pixels.
         * @return Numerical value of the RightMargin element.
         */
        public double getTextRightMargin()
        {
            return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.RIGHT_MARGIN), 0);
        }

        /**
         * Returns the style of one text fragment.
         * @param charIX IX attribute of Char element
         * @return String value of the Style element.
         */
        public String getTextStyle(String index)
        {
            XmlElement styleElem = getCellElement(mxVsdxConstants.STYLE, index, mxVsdxConstants.CHARACTER);
            return getValue(styleElem, "");
        }

        /**
         * Returns the font of one text fragment
         * @param charIX IX attribute of Char element
         * @return Name of the font.
         */
        public String getTextFont(String index)
        {
            XmlElement fontElem = getCellElement(mxVsdxConstants.FONT, index, mxVsdxConstants.CHARACTER);
            return getValue(fontElem, "");
        }

        /**
         * Returns the position of one text fragment
         * @param charIX IX attribute of Char element
         * @return Integer value of the Pos element.
         */
        public String getTextPos(String index)
        {
            XmlElement posElem = getCellElement(mxVsdxConstants.POS, index, mxVsdxConstants.CHARACTER);
            return getValue(posElem, "");
        }

        /**
         * Checks if one text fragment is Strikethru
         * @param charIX IX attribute of Char element
         * @return Returns <code>true</code> if one text fragment is Strikethru
         */
        public bool getTextStrike(String index)
        {
            XmlElement strikeElem = getCellElement(mxVsdxConstants.STRIKETHRU, index, mxVsdxConstants.CHARACTER);
            return getValue(strikeElem, "").Equals("1");
        }

        /**
         * Returns the case property of one text fragment
         * @param charIX IX attribute of Char element
         * @return Integer value of the Case element
         */
        public String getTextCase(String index)
        {
            XmlElement caseElem = getCellElement(mxVsdxConstants.CASE, index, mxVsdxConstants.CHARACTER);
            return getValue(caseElem, "");
        }

        /**
         * Returns the horizontal align property of a paragraph
         * @param index IX attribute of Para element
         * @param html whether to return the html values or mxGraph values
         * @return String value of the HorizontalAlign element.
         */
        public String getHorizontalAlign(String index, bool html)
        {
            String ret = "center";
            XmlElement horAlign = getCellElement(mxVsdxConstants.HORIZONTAL_ALIGN, index, mxVsdxConstants.PARAGRAPH);
            String align = getValue(horAlign, "");

            switch (align)
            {
                case "0":
                    ret = html ? "left" : mxConstants.ALIGN_LEFT;
                    break;
                case "2":
                    ret = html ? "right" : mxConstants.ALIGN_RIGHT;
                    break;
                case "3":
                case "4":
                    ret = html ? "justify" : mxConstants.ALIGN_CENTER;
                    break;
                default:
                    ret = html ? "center" : mxConstants.ALIGN_CENTER;
                    break;
            }

            return ret;
        }

        /**
         * Returns the first indent of one paragraph in pixels.
         * @param paraIX IX attribute of Para element
         * @return String representation of the numerical value of the IndentFirst element.
         */
        public String getIndentFirst(String index)
        {
            XmlElement indentFirstElem = getCellElement(mxVsdxConstants.INDENT_FIRST, index, mxVsdxConstants.PARAGRAPH);
            return (getScreenNumericalValue(indentFirstElem, 0)).ToString();
        }

        /**
         * Returns the indent to left of one paragraph
         * @param paraIX IX attribute of Para element
         * @return String representation of the numerical value of the IndentLeft element.
         */
        public String getIndentLeft(String index)
        {
            XmlElement indentLeftElem = getCellElement(mxVsdxConstants.INDENT_LEFT, index, mxVsdxConstants.PARAGRAPH);
            return (getScreenNumericalValue(indentLeftElem, 0)).ToString();
        }

        /**
         * Returns the indent to right of one paragraph
         * @param paraIX IX attribute of Para element
         * @return String representation of the numerical value of the IndentRight element.
         */
        public String getIndentRight(String index)
        {
            XmlElement indentRightElem = getCellElement(mxVsdxConstants.INDENT_RIGHT, index, mxVsdxConstants.PARAGRAPH);
            return (getScreenNumericalValue(indentRightElem, 0)).ToString();
        }

        /**
         * Returns the space before one paragraph.
         * @param paraIX IX attribute of Para element
         * @return String representation of the numerical value of the SpBefore element.
         */
        public String getSpBefore(String index)
        {
            XmlElement spBeforeElem = getCellElement(mxVsdxConstants.SPACE_BEFORE, index, mxVsdxConstants.PARAGRAPH);
            return (getScreenNumericalValue(spBeforeElem, 0).ToString());
        }

        /**
         * Returns the space after one paragraph
         * @param paraIX IX attribute of Para element
         * @return String representation of the numerical value of the SpAfter element.
         */
        public String getSpAfter(String index)
        {
            XmlElement spAfterElem = getCellElement(mxVsdxConstants.SPACE_AFTER, index, mxVsdxConstants.PARAGRAPH);
            return (getScreenNumericalValue(spAfterElem, 0).ToString());
        }

        /**
         * Returns the space between lines in one paragraph.
         * @param paraIX IX attribute of Para element.
         * @return Double representation of the value of the SpLine element.
         */
        public double getSpLine(String index)
        {
            XmlElement spLineElem = getCellElement(mxVsdxConstants.SPACE_LINE, index, mxVsdxConstants.PARAGRAPH);
            String val = getValue(spLineElem, "");

            if (!val.Equals(""))
            {
                return Double.Parse(val);
            }

            return 0;
        }

        /**
         * Returns the flags of one paragraph.
         * @param paraIX IX attribute of Para element.
         * @return String value of the Flags element.
         */
        public String getFlags(String index)
        {
            XmlElement flagsElem = getCellElement(mxVsdxConstants.FLAGS, index, mxVsdxConstants.PARAGRAPH);
            return getValue(flagsElem, "0");
        }

        /**
         * Returns the space between characters in one text fragment.
         * @param paraIX IX attribute of Para element.
         * @return String representation of the numerical value of the Letterspace element.
         */
        public String getLetterSpace(String index)
        {
            XmlElement letterSpaceElem = getCellElement(mxVsdxConstants.LETTER_SPACE, index, mxVsdxConstants.PARAGRAPH);
            return (getScreenNumericalValue(letterSpaceElem, 0).ToString());
        }

        /**
         * Returns the bullet element value.
         * @param paraIX IX attribute of Para element.
         * @return String value of the Bullet element.
         */
        public String getBullet(String index)
        {
            XmlElement bulletElem = getCellElement(mxVsdxConstants.BULLET, index, mxVsdxConstants.PARAGRAPH);
            return getValue(bulletElem, "0");
        }

        public XmlElement getShape()
        {
            return shape;
        }

        public void setShape(XmlElement shape)
        {
            this.shape = shape;
        }

        private static double SPACE = 4.0, SHORT_SPACE = 2.0, LONG_SPACE = 6.0, DOT = 1.0, DASH = 8.0, LONG_DASH = 12.0, SHORT_DASH = 4.0, XLONG_DASH = 20.0, XSHORT_DASH = 2.0;
        private static List<List<Double>> lineDashPatterns = new List<List<Double>>()
        {
            //0 no pattern, 1 solid, 2 similar to mxGraph default dash 
            new List<Double>(),
            new List<Double>(),
            new List<Double>(),
            //3
            new List<double>(){ DOT,SPACE},
            //4
		    new List<double>(){        (DASH),     (SPACE),        (DOT),      (SPACE)    },
            //5
		    new List<double>(){(DASH),(SPACE),(DOT),(SPACE),(DOT),(SPACE)    },
            //6
		    new List<double>(){        (DASH),      (SPACE),        (DASH),     (SPACE),        (DOT),      (SPACE)    },
            //7
		    new List<double>(){(LONG_DASH),(SPACE),(SHORT_DASH),(SPACE)    },
            //8
		    new List<double>(){(LONG_DASH),(SPACE),(SHORT_DASH),(SPACE),(SHORT_DASH),(SPACE)    },
            //9
		    new List<double>(){(SHORT_DASH),(SHORT_SPACE)    },
            //10
		    new List<double>(){(DOT),(SHORT_SPACE)    },
            //11
		    new List<double>(){(SHORT_DASH),(SHORT_SPACE),(DOT),(SHORT_SPACE)    },
            //12
		    new List<double>(){(SHORT_DASH),(SHORT_SPACE),(DOT),(SHORT_SPACE),(DOT),(SHORT_SPACE)    },
            //13
		    new List<double>(){(SHORT_DASH),(SHORT_SPACE),(SHORT_DASH),(SHORT_SPACE),(DOT),(SHORT_SPACE)    },
            //14
		    new List<double>(){(DASH),(SHORT_SPACE),(SHORT_DASH),(SHORT_SPACE)    },
            //15
		    new List<double>(){(DASH),(SHORT_SPACE),(SHORT_DASH),(SHORT_SPACE),(SHORT_DASH),(SHORT_SPACE)    },
            //16
		    new List<double>(){(LONG_DASH),(LONG_SPACE)    },
            //17
		    new List<double>(){(DOT),(LONG_SPACE)    },
            //18
		    new List<double>(){(LONG_DASH),(LONG_SPACE),(DOT),(LONG_SPACE)            },
            //19
		    new List<double>(){(LONG_DASH),(LONG_SPACE),(DOT),(LONG_SPACE),(DOT),(LONG_SPACE)    },
            //20
		    new List<double>(){(LONG_DASH),(LONG_SPACE),(LONG_DASH),(LONG_SPACE),(DOT),(LONG_SPACE)    },
            //21
		    new List<double>(){(XLONG_DASH),(LONG_SPACE),(DASH),(LONG_SPACE)    },
            //22
		    new List<double>(){(XLONG_DASH),(LONG_SPACE),(DASH),(LONG_SPACE),(DASH),(LONG_SPACE)    },
            //23
		    new List<double>(){(XSHORT_DASH),(SHORT_SPACE)    }
};


        public static List<Double> getLineDashPattern(int pattern)
        {
            if (pattern >= 0 && pattern <= 23)
                return lineDashPatterns[pattern];
            else
                return lineDashPatterns[0];
        }
    }
}
