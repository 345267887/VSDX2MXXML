using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{


    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;
    using NodeList = System.Xml.XmlNodeList;

    using Color = mxGraph.io.vsdx.theme.Color;
    using QuickStyleVals = mxGraph.io.vsdx.theme.QuickStyleVals;
    using mxConstants = mxGraph.util.mxConstants;

    /// <summary>
    /// This class is a general wrapper for one Shape Element.<br/>
    /// Provides a set of methods for retrieving the value of different properties
    /// stored in the shape element.<br/>
    /// References to other shapes or style-sheets are not considered.
    /// </summary>
    public class Style
    {
        protected internal Element shape;

        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal int? Id_Renamed;

        // .vsdx cells elements that contain one style each
        protected internal Dictionary<string, Element> cellElements = new Dictionary<string, Element>();

        protected internal IDictionary<string, Section> sections = new Dictionary<string, Section>();

        protected internal mxPropertiesManager pm;

        /// <summary>
        /// Mapping of line,text and fill styles to the style parents
        /// </summary>
        protected internal IDictionary<string, Style> styleParents = new Dictionary<string, Style>();

        protected internal Style style;

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //private static readonly Logger LOGGER = Logger.getLogger(typeof(Style).FullName);

        public static bool vsdxStyleDebug = false;

        protected internal static IDictionary<string, string> styleTypes = new Dictionary<string, string>();

        static Style()
        {
            styleTypes[mxVsdxConstants.FILL] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.FILL_BKGND] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.FILL_BKGND_TRANS] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.FILL_FOREGND] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.FILL_FOREGND_TRANS] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.FILL_PATTERN] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.SHDW_PATTERN] = mxVsdxConstants.FILL_STYLE;
            styleTypes[mxVsdxConstants.FILL_STYLE] = mxVsdxConstants.FILL_STYLE;
            styleTypes["QuickStyleFillColor"] = mxVsdxConstants.FILL_STYLE;
            styleTypes["QuickStyleFillMatrix"] = mxVsdxConstants.FILL_STYLE;

            styleTypes[mxVsdxConstants.BEGIN_ARROW] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.END_ARROW] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.LINE_PATTERN] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.LINE_COLOR] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.LINE_COLOR_TRANS] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.LINE_WEIGHT] = mxVsdxConstants.LINE_STYLE;
            styleTypes["QuickStyleLineColor"] = mxVsdxConstants.LINE_STYLE;
            styleTypes["QuickStyleLineMatrix"] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.BEGIN_ARROW_SIZE] = mxVsdxConstants.LINE_STYLE;
            styleTypes[mxVsdxConstants.END_ARROW_SIZE] = mxVsdxConstants.LINE_STYLE;

            styleTypes[mxVsdxConstants.TEXT_BKGND] = mxVsdxConstants.TEXT_STYLE;
            styleTypes[mxVsdxConstants.BOTTOM_MARGIN] = mxVsdxConstants.TEXT_STYLE;
            styleTypes[mxVsdxConstants.LEFT_MARGIN] = mxVsdxConstants.TEXT_STYLE;
            styleTypes[mxVsdxConstants.RIGHT_MARGIN] = mxVsdxConstants.TEXT_STYLE;
            styleTypes[mxVsdxConstants.TOP_MARGIN] = mxVsdxConstants.TEXT_STYLE;
            styleTypes[mxVsdxConstants.PARAGRAPH] = mxVsdxConstants.TEXT_STYLE;
            styleTypes[mxVsdxConstants.CHARACTER] = mxVsdxConstants.TEXT_STYLE;
            styleTypes["QuickStyleFontColor"] = mxVsdxConstants.TEXT_STYLE;
            styleTypes["QuickStyleFontMatrix"] = mxVsdxConstants.TEXT_STYLE;
            //0 no pattern, 1 solid, 2 similar to mxGraph default dash 
            lineDashPatterns.Add(new List<double?>());
            lineDashPatterns.Add(new List<double?>());
            lineDashPatterns.Add(new List<double?>());
            //3
            List<double?> lineDashPattern = new List<double?>();
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //4
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //5
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //6
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //7
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //8
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //9
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //10
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //11
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //12
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //13
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //14
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //15
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPattern.Add(SHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //16
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //17
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //18
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //19
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //20
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(LONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DOT);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //21
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(XLONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //22
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(XLONG_DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPattern.Add(DASH);
            lineDashPattern.Add(LONG_SPACE);
            lineDashPatterns.Add(lineDashPattern);
            //23
            lineDashPattern = new List<double?>();
            lineDashPattern.Add(XSHORT_DASH);
            lineDashPattern.Add(SHORT_SPACE);
            lineDashPatterns.Add(lineDashPattern);
        }

        /// <summary>
        /// Create a new instance of mxGeneralShape </summary>
        /// <param name="shape"> Shape Element to be wrapped. </param>
        public Style(Element shape, mxVsdxModel model)
        {
            this.shape = shape;
            this.pm = model.PropertiesManager;

            string Id = shape.GetAttribute(mxVsdxConstants.ID);

            try
            {
                this.Id_Renamed = (!string.ReferenceEquals(Id, null) && Id.Length > 0) ? Convert.ToInt32(Id) : -1;
            }
            catch (Exception)
            {
                // TODO handle exception correctly
            }

            cacheCells(model);
            stylesheetRefs(model);
        }

        public virtual mxVsdxTheme Theme
        {
            get
            {
                return null;
            }
        }

        public virtual QuickStyleVals QuickStyleVals
        {
            get
            {
                return null;
            }
        }

        public virtual bool Vertex
        {
            get
            {
                return false;
            }
        }

        public virtual void styleDebug(string debug)
        {
            if (vsdxStyleDebug)
            {
                Console.WriteLine(debug);
            }
        }

        public virtual void stylesheetRefs(mxVsdxModel model)
        {
            if (!styleParents.ContainsKey(mxVsdxConstants.FILL_STYLE)) { styleParents.Add(mxVsdxConstants.FILL_STYLE, model.getStylesheet(shape.GetAttribute(mxVsdxConstants.FILL_STYLE))); }
            if (!styleParents.ContainsKey(mxVsdxConstants.LINE_STYLE))
            { styleParents.Add(mxVsdxConstants.LINE_STYLE, model.getStylesheet(shape.GetAttribute(mxVsdxConstants.LINE_STYLE))); }
            if (!styleParents.ContainsKey(mxVsdxConstants.TEXT_STYLE))
            { styleParents.Add(mxVsdxConstants.TEXT_STYLE, model.getStylesheet(shape.GetAttribute(mxVsdxConstants.TEXT_STYLE))); }

            Style style = model.getStylesheet("0");
            this.style = style;
        }

        /// <summary>
        /// Checks if the shape Element has a children with tag name = 'tag'. </summary>
        /// <param name="tag"> Name of the Element to be found. </param>
        /// <returns> Returns <code>true</code> if the shape Element has a children with tag name = 'tag' </returns>
        protected internal virtual void cacheCells(mxVsdxModel model)
        {
            if (shape != null)
            {
                NodeList children = shape.ChildNodes;

                if (children != null)
                {
                    Node childNode = children.Item(0);

                    while (childNode != null)
                    {
                        if (childNode is Element)
                        {
                            parseShapeElem((Element)childNode, model);
                        }

                        childNode = childNode.NextSibling;
                    }
                }
            }
        }

        /// <summary>
        /// Caches the specified element </summary>
        /// <param name="elem"> the element to cache </param>
        protected internal virtual void parseShapeElem(Element elem, mxVsdxModel model)
        {
            string childName = elem.Name;

            if (childName.Equals("Cell"))
            {
                this.cellElements.Add(elem.GetAttribute("N"), elem);
            }
            else if (childName.Equals("Section"))
            {
                this.parseSection(elem);
            }
        }

        /// <summary>
        /// Caches the specific section element </summary>
        /// <param name="elem"> the element to cache </param>
        protected internal virtual void parseSection(Element elem)
        {
            Section sect = new Section(elem);
            this.sections.Add(elem.GetAttribute("N"), sect);
        }

        /// <summary>
        /// Checks if the 'primary' Element has a child with tag name = 'tag'. </summary>
        /// <param name="tag"> Name of the Element to be found. </param>
        /// <returns> Returns <code>true</code> if the 'primary' Element has a child with tag name = 'tag'. </returns>
        protected internal virtual bool hasProperty(string nodeName, string tag)
        {
            return this.cellElements.ContainsKey(tag);
        }

        /// <summary>
        /// Returns the value of the element </summary>
        /// <param name="elem"> The element whose value is to be found </param>
        /// <param name="defaultValue"> the value to return if there is no value attribute </param>
        /// <returns> String value of the element, or the default value if no value found </returns>
        protected internal virtual string getValue(Element elem, string defaultValue)
        {
            if (elem != null)
            {
                return elem.GetAttribute("V");
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the value of the element as a double </summary>
        /// <param name="elem"> The element whose value is to be found </param>
        /// <param name="defaultValue"> the value to return if there is no value attribute </param>
        /// <returns> double value of the element, or the default value if no value found </returns>
        protected internal virtual double getValueAsDouble(Element cell, double defaultValue)
        {
            if (cell != null)
            {
                string value = cell.GetAttribute("V");

                if (!string.ReferenceEquals(value, null))
                {
                    if (value.Equals("Themed"))
                    {
                        return 0;
                    }

                    try
                    {
                        double parsedValue = double.Parse(value);

                        string units = cell.GetAttribute("U");

                        if (units.Equals("PT"))
                        {
                            // Convert from points to pixels
                            parsedValue = parsedValue * mxVsdxUtils.conversionFactor;
                        }

                        return Math.Round(parsedValue * 100.0) / 100.0;
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }
            }

            return defaultValue;
        }

        //if (!tag.equals(mxVdxConstants.FILL_BKGND_TRANS) && !tag.equals(mxVdxConstants.FILL_FOREGND_TRANS) && !tag.equals(mxVdxConstants.LINE_COLOR_TRANS) && !tag.equals(mxVdxConstants.NO_LINE))

        /// <summary>
        /// Returns the value of the element as a double </summary>
        /// <param name="elem"> The element whose value is to be found </param>
        /// <param name="defaultValue"> the value to return if there is no value attribute </param>
        /// <returns> double value of the element, or the default value if no value found </returns>
        protected internal virtual double getScreenNumericalValue(Element cell, double defaultValue)
        {
            if (cell != null)
            {
                string value = cell.GetAttribute("V");

                if (!string.ReferenceEquals(value, null))
                {
                    try
                    {
                        double parsedValue = double.Parse(value);

                        return getScreenNumericalValue(parsedValue);
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }
            }

            return defaultValue;
        }

        protected internal virtual double getScreenNumericalValue(double val)
        {
            double conVal = val * mxVsdxUtils.conversionFactor;
            return Math.Round(conVal * 100.0) / 100.0;
        }

        /// <summary>
        /// Returns the value of the attribute of the element with tag name = 'tag' in the children
        /// of the shape element<br/> </summary>
        /// <param name="tag"> Name of the Element to be found. </param>
        /// <returns> Numerical value of the element. </returns>
        public virtual string getAttribute(string tag, string attribute, string defaultValue)
        {
            string result = defaultValue;
            Element cell = this.cellElements.ContainsKey(tag)? this.cellElements[tag]:null;

            if (cell != null)
            {
                result = cell.GetAttribute(attribute);
            }

            return result;
        }

        protected internal virtual IDictionary<string, string> getChildValues(Element parent, IDictionary<string, string> requiredValues)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            Node child = parent.FirstChild;

            while (child != null)
            {
                if (child is Element)
                {
                    Element childElem = (Element)child;
                    string childName = childElem.Name;
                    string name = null;
                    string nodeValue = null;

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
                        string nodeOverride = requiredValues[name];

                        if (!string.ReferenceEquals(nodeOverride, null))
                        {
                            nodeValue = childElem.GetAttribute(nodeOverride);
                        }
                    }

                    result[name] = nodeValue;
                }

                child = child.NextSibling;
            }

            return result;
        }

        protected internal virtual Element getCellElement(string cellKey, string index, string sectKey)
        {
            Section sect = this.sections.ContainsKey(sectKey) ? this.sections[sectKey]:null;
            Element elem = null;
            bool inherit = false;

            if (sect != null)
            {
                elem = sect.getIndexedCell(index, cellKey);
            }

            if (elem != null)
            {
                string form = elem.GetAttribute("F");
                string value = elem.GetAttribute("V");

                if (!string.ReferenceEquals(form, null) && !string.ReferenceEquals(value, null))
                {
                    if (form.Equals("Inh") && value.Equals("Themed"))
                    {
                        inherit = true;
                    }
                    else if (form.Equals("THEMEVAL()") && value.Equals("Themed") && style != null)
                    {
                        //Handle theme here
                        //FIXME this is a very hacky way to test themes until fully integrating themes
                        if (mxVsdxConstants.COLOR.Equals(cellKey))
                        {
                            return elem;
                        }

                        // Use "no style" style
                        Element themeElem = style.getCellElement(cellKey, index, sectKey);

                        if (themeElem != null)
                        {
                            return themeElem;
                        }
                    }
                }
            }

            if (elem == null || inherit)
            {
                string styleType = Style.styleTypes[sectKey];
                Style parentStyle = this.styleParents[styleType];

                if (parentStyle != null)
                {
                    Element parentElem = parentStyle.getCellElement(cellKey, index, sectKey);

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

        /// <summary>
        /// Locates the first entry for the specified style string in the style hierarchy.
        /// The order is to look locally, then delegate the request to the relevant parent style
        /// if it doesn't exist locally </summary>
        /// <param name="key"> The key of the cell to find </param>
        /// <returns> the Element that first resolves to that style key or null or none is found </returns>
        protected internal virtual Element getCellElement(string key)
        {
            Element elem = this.cellElements.ContainsKey(key)? this.cellElements[key]:null;
            bool inherit = false;

            if (elem != null)
            {
                string form = elem.GetAttribute("F");
                string value = elem.GetAttribute("V");

                if (!string.ReferenceEquals(form, null) && !string.ReferenceEquals(value, null))
                {
                    if (form.Equals("Inh") && value.Equals("Themed"))
                    {
                        inherit = true;
                    }
                    else if (form.Contains("THEMEVAL()") && value.Equals("Themed") && style != null)
                    {
                        //Handle theme here
                        //FIXME this is a very hacky way to test themes until fully integrating themes
                        if ("FillForegnd".Equals(key) || mxVsdxConstants.LINE_COLOR.Equals(key) || mxVsdxConstants.LINE_PATTERN.Equals(key) || mxVsdxConstants.BEGIN_ARROW_SIZE.Equals(key) || mxVsdxConstants.END_ARROW_SIZE.Equals(key) || mxVsdxConstants.BEGIN_ARROW.Equals(key) || mxVsdxConstants.END_ARROW.Equals(key) || mxVsdxConstants.LINE_WEIGHT.Equals(key))
                        {
                            return elem;
                        }

                        // Use "no style" style
                        Element themeElem = style.getCellElement(key);

                        if (themeElem != null)
                        {
                            return themeElem;
                        }
                    }
                }
            }

            if (elem == null || inherit)
            {
                string styleType = Style.styleTypes.ContainsKey(key)? Style.styleTypes[key]:"0.0.0.00.0.";//特殊处理一下
                Style parentStyle = this.styleParents.ContainsKey(styleType)? this.styleParents[styleType]:null;

                if (parentStyle != null)
                {
                    Element parentElem = parentStyle.getCellElement(key);

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

        /// <summary>
        /// Returns the line color.<br/>
        /// The property may to be defined in master shape or line stylesheet.<br/> </summary>
        /// <returns> hexadecimal representation of the color. </returns>
        public virtual string StrokeColor
        {
            get
            {
                string color = "";

                if (this.getValue(this.getCellElement(mxVsdxConstants.LINE_PATTERN), "1").Equals("0"))
                {
                    color = "none";
                }
                else
                {
                    color = this.getColor(this.getCellElement(mxVsdxConstants.LINE_COLOR));

                    if ("Themed".Equals(color))
                    {
                        mxVsdxTheme theme = Theme;

                        if (theme != null)
                        {
                            Color colorObj = Vertex ? theme.getLineColor(QuickStyleVals) : theme.getConnLineColor(QuickStyleVals);
                            color = colorObj.toHexStr();
                        }
                        else
                        {
                            color = "";
                        }
                    }
                }

                return color;
            }
        }

        /// <summary>
        /// Returns the shape's color.
        /// The property may to be defined in master shape or fill stylesheet.
        /// If the color is the background or the fore color, it depends on the pattern.
        /// For simple gradients and solid, returns the fore color, else return the
        /// background color. </summary>
        /// <returns> hexadecimal representation of the color. </returns>
        protected internal virtual string FillColor
        {
            get
            {
                string fillForeColor = this.getColor(this.getCellElement(mxVsdxConstants.FILL_FOREGND));

                if ("Themed".Equals(fillForeColor))
                {
                    mxVsdxTheme theme = Theme;

                    if (theme != null)
                    {
                        Color color = theme.getFillColor(QuickStyleVals);
                        fillForeColor = color.toHexStr();
                    }
                    else
                    {
                        //One sample file has fill color as white when no theme is used and value is Themed!
                        fillForeColor = "#FFFFFF";
                    }
                }

                string fillPattern = this.getValue(this.getCellElement(mxVsdxConstants.FILL_PATTERN), "0");

                if (!string.ReferenceEquals(fillPattern, null) && fillPattern.Equals("0"))
                {
                    return "none";
                }
                else
                {
                    return fillForeColor;
                }
            }
        }

        protected internal virtual string getColor(Element elem)
        {
            string color = this.getValue(elem, "");

            if (!"Themed".Equals(color) && !color.StartsWith("#", StringComparison.Ordinal))
            {
                color = pm.getColor(color);
            }

            return color;
        }

        /// <summary>
        /// The TextBkgnd cell can have any value from 0 through 24, or 255. The values 0 and 255 (visTxtBlklOpaque) both indicate a transparent text background.
        /// To enter a custom color, use the RGB or HSL function plus one—for example, RGB(255,127,255)+1. The value of a custom color is its RGB color, and RGB(r, g, b)+1, 
        /// rather than a number, will be shown in the ShapeSheet window. When used in numeric operations, custom colors have values of 25 and above.
        /// You can set the transparency of the text background color in the TextBkgndTrans cell.
        /// </summary>
        protected internal virtual string getTextBkgndColor(Element elem)
        {
            string color = this.getValue(elem, "");

            if (!color.StartsWith("#", StringComparison.Ordinal))
            {
                if (color.Equals("0") || color.Equals("255"))
                {
                    return "none";
                }

                return pm.getColor((int.Parse(color) - 1).ToString());
            }

            return color;
        }

        /// <summary>
        /// Returns the line weight of the shape in pixels </summary>
        /// <returns> Numerical value of the LineWeight element. </returns>
        public virtual double LineWeight
        {
            get
            {
                return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.LINE_WEIGHT), 0);
            }
        }

        /// <summary>
        /// Returns the level of transparency of the Shape. </summary>
        /// <returns> double in range (opaque = 0)..(100 = transparent) </returns>
        public virtual double StrokeTransparency
        {
            get
            {
                return getValueAsDouble(this.getCellElement(mxVsdxConstants.LINE_COLOR_TRANS), 0);
            }
        }

        /// <summary>
        /// Returns the NameU attribute. </summary>
        /// <returns> Value of the NameU attribute. </returns>
        public virtual string NameU
        {
            get
            {
                return shape.GetAttribute(mxVsdxConstants.NAME_U);
            }
        }

        /// <summary>
        /// Returns the Name attribute. </summary>
        /// <returns> Value of the Name attribute (Human readable name). </returns>
        public virtual string Name
        {
            get
            {
                return shape.GetAttribute(mxVsdxConstants.NAME);
            }
        }

        /// <summary>
        /// Returns the UniqueID attribute. </summary>
        /// <returns> Value of the UniqueID attribute. </returns>
        public virtual string UniqueID
        {
            get
            {
                return shape.GetAttribute(mxVsdxConstants.UNIQUE_ID);
            }
        }

        /// <summary>
        /// Returns the value of the Id attribute. </summary>
        /// <returns> Value of the Id attribute. </returns>
        public virtual int? Id
        {
            get
            {
                return this.Id_Renamed;
            }
        }

        /// <summary>
        /// Returns the color of one text fragment </summary>
        /// <param name="charIX"> IX attribute of Char element </param>
        /// <returns> Text color in hexadecimal representation. </returns>
        public virtual string getTextColor(string index)
        {
            Element colorElem = getCellElement(mxVsdxConstants.COLOR, index, mxVsdxConstants.CHARACTER);
            string color = getValue(colorElem, "#000000");

            if ("Themed".Equals(color))
            {
                mxVsdxTheme theme = Theme;

                if (theme != null)
                {
                    Color colorObj = Vertex ? theme.getFontColor(QuickStyleVals) : theme.getConnFontColor(QuickStyleVals);
                    color = colorObj.toHexStr();
                }
                else
                {
                    color = "#000000";
                }
            }
            else if (!color.StartsWith("#", StringComparison.Ordinal))
            {
                color = pm.getColor(color);
            }

            return color;
        }

        /// <summary>
        /// Returns the top margin of text in pixels. </summary>
        /// <returns> Numerical value of the TopMargin element </returns>
        public virtual double TextTopMargin
        {
            get
            {
                return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.TOP_MARGIN), 0);
            }
        }

        /// <summary>
        /// Returns the bottom margin of text in pixels. </summary>
        /// <returns> Numerical value of the BottomMargin element. </returns>
        public virtual double TextBottomMargin
        {
            get
            {
                return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.BOTTOM_MARGIN), 0);
            }
        }

        /// <summary>
        /// Returns the left margin of text in pixels. </summary>
        /// <returns> Numerical value of the LeftMargin element. </returns>
        public virtual double TextLeftMargin
        {
            get
            {
                return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.LEFT_MARGIN), 0);
            }
        }

        /// <summary>
        /// Returns the right margin of text in pixels. </summary>
        /// <returns> Numerical value of the RightMargin element. </returns>
        public virtual double TextRightMargin
        {
            get
            {
                return getScreenNumericalValue(this.getCellElement(mxVsdxConstants.RIGHT_MARGIN), 0);
            }
        }

        /// <summary>
        /// Returns the style of one text fragment. </summary>
        /// <param name="charIX"> IX attribute of Char element </param>
        /// <returns> String value of the Style element. </returns>
        public virtual string getTextStyle(string index)
        {
            Element styleElem = getCellElement(mxVsdxConstants.STYLE, index, mxVsdxConstants.CHARACTER);
            return getValue(styleElem, "");
        }

        /// <summary>
        /// Returns the font of one text fragment </summary>
        /// <param name="charIX"> IX attribute of Char element </param>
        /// <returns> Name of the font. </returns>
        public virtual string getTextFont(string index)
        {
            Element fontElem = getCellElement(mxVsdxConstants.FONT, index, mxVsdxConstants.CHARACTER);
            return getValue(fontElem, "");
        }

        /// <summary>
        /// Returns the position of one text fragment </summary>
        /// <param name="charIX"> IX attribute of Char element </param>
        /// <returns> Integer value of the Pos element. </returns>
        public virtual string getTextPos(string index)
        {
            Element posElem = getCellElement(mxVsdxConstants.POS, index, mxVsdxConstants.CHARACTER);
            return getValue(posElem, "");
        }

        /// <summary>
        /// Checks if one text fragment is Strikethru </summary>
        /// <param name="charIX"> IX attribute of Char element </param>
        /// <returns> Returns <code>true</code> if one text fragment is Strikethru </returns>
        public virtual bool getTextStrike(string index)
        {
            Element strikeElem = getCellElement(mxVsdxConstants.STRIKETHRU, index, mxVsdxConstants.CHARACTER);
            return getValue(strikeElem, "").Equals("1");
        }

        /// <summary>
        /// Returns the case property of one text fragment </summary>
        /// <param name="charIX"> IX attribute of Char element </param>
        /// <returns> Integer value of the Case element </returns>
        public virtual string getTextCase(string index)
        {
            Element caseElem = getCellElement(mxVsdxConstants.CASE, index, mxVsdxConstants.CHARACTER);
            return getValue(caseElem, "");
        }

        /// <summary>
        /// Returns the horizontal align property of a paragraph </summary>
        /// <param name="index"> IX attribute of Para element </param>
        /// <param name="html"> whether to return the html values or mxGraph values </param>
        /// <returns> String value of the HorizontalAlign element. </returns>
        public virtual string getHorizontalAlign(string index, bool html)
        {
            string ret = "center";
            Element horAlign = getCellElement(mxVsdxConstants.HORIZONTAL_ALIGN, index, mxVsdxConstants.PARAGRAPH);
            string align = getValue(horAlign, "");

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

        /// <summary>
        /// Returns the first indent of one paragraph in pixels. </summary>
        /// <param name="paraIX"> IX attribute of Para element </param>
        /// <returns> String representation of the numerical value of the IndentFirst element. </returns>
        public virtual string getIndentFirst(string index)
        {
            Element indentFirstElem = getCellElement(mxVsdxConstants.INDENT_FIRST, index, mxVsdxConstants.PARAGRAPH);
            return getScreenNumericalValue(indentFirstElem, 0).ToString();
        }

        /// <summary>
        /// Returns the indent to left of one paragraph </summary>
        /// <param name="paraIX"> IX attribute of Para element </param>
        /// <returns> String representation of the numerical value of the IndentLeft element. </returns>
        public virtual string getIndentLeft(string index)
        {
            Element indentLeftElem = getCellElement(mxVsdxConstants.INDENT_LEFT, index, mxVsdxConstants.PARAGRAPH);
            return getScreenNumericalValue(indentLeftElem, 0).ToString();
        }

        /// <summary>
        /// Returns the indent to right of one paragraph </summary>
        /// <param name="paraIX"> IX attribute of Para element </param>
        /// <returns> String representation of the numerical value of the IndentRight element. </returns>
        public virtual string getIndentRight(string index)
        {
            Element indentRightElem = getCellElement(mxVsdxConstants.INDENT_RIGHT, index, mxVsdxConstants.PARAGRAPH);
            return getScreenNumericalValue(indentRightElem, 0).ToString();
        }

        /// <summary>
        /// Returns the space before one paragraph. </summary>
        /// <param name="paraIX"> IX attribute of Para element </param>
        /// <returns> String representation of the numerical value of the SpBefore element. </returns>
        public virtual string getSpBefore(string index)
        {
            Element spBeforeElem = getCellElement(mxVsdxConstants.SPACE_BEFORE, index, mxVsdxConstants.PARAGRAPH);
            return getScreenNumericalValue(spBeforeElem, 0).ToString();
        }

        /// <summary>
        /// Returns the space after one paragraph </summary>
        /// <param name="paraIX"> IX attribute of Para element </param>
        /// <returns> String representation of the numerical value of the SpAfter element. </returns>
        public virtual string getSpAfter(string index)
        {
            Element spAfterElem = getCellElement(mxVsdxConstants.SPACE_AFTER, index, mxVsdxConstants.PARAGRAPH);
            return getScreenNumericalValue(spAfterElem, 0).ToString();
        }

        /// <summary>
        /// Returns the space between lines in one paragraph. </summary>
        /// <param name="paraIX"> IX attribute of Para element. </param>
        /// <returns> Double representation of the value of the SpLine element. </returns>
        public virtual double getSpLine(string index)
        {
            Element spLineElem = getCellElement(mxVsdxConstants.SPACE_LINE, index, mxVsdxConstants.PARAGRAPH);
            string val = getValue(spLineElem, "");

            if (!val.Equals(""))
            {
                return double.Parse(val);
            }

            return 0;
        }

        /// <summary>
        /// Returns the flags of one paragraph. </summary>
        /// <param name="paraIX"> IX attribute of Para element. </param>
        /// <returns> String value of the Flags element. </returns>
        public virtual string getFlags(string index)
        {
            Element flagsElem = getCellElement(mxVsdxConstants.FLAGS, index, mxVsdxConstants.PARAGRAPH);
            return getValue(flagsElem, "0");
        }

        /// <summary>
        /// Returns the space between characters in one text fragment. </summary>
        /// <param name="paraIX"> IX attribute of Para element. </param>
        /// <returns> String representation of the numerical value of the Letterspace element. </returns>
        public virtual string getLetterSpace(string index)
        {
            Element letterSpaceElem = getCellElement(mxVsdxConstants.LETTER_SPACE, index, mxVsdxConstants.PARAGRAPH);
            return getScreenNumericalValue(letterSpaceElem, 0).ToString();
        }

        /// <summary>
        /// Returns the bullet element value. </summary>
        /// <param name="paraIX"> IX attribute of Para element. </param>
        /// <returns> String value of the Bullet element. </returns>
        public virtual string getBullet(string index)
        {
            Element bulletElem = getCellElement(mxVsdxConstants.BULLET, index, mxVsdxConstants.PARAGRAPH);
            return getValue(bulletElem, "0");
        }

        public virtual Element Shape
        {
            get
            {
                return shape;
            }
            set
            {
                this.shape = value;
            }
        }


        private const double SPACE = 4.0, SHORT_SPACE = 2.0, LONG_SPACE = 6.0, DOT = 1.0, DASH = 8.0, LONG_DASH = 12.0, SHORT_DASH = 4.0, XLONG_DASH = 20.0, XSHORT_DASH = 2.0;
        private static readonly List<List<double?>> lineDashPatterns = new List<List<double?>>();

        public static List<double?> getLineDashPattern(int pattern)
        {
            if (pattern >= 0 && pattern <= 23)
            {
                return lineDashPatterns[pattern];
            }
            else
            {
                return lineDashPatterns[0];
            }
        }
    }

}