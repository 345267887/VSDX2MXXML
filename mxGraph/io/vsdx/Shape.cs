using mxGraph;
using System;
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

    using QuickStyleVals = mxGraph.io.vsdx.theme.QuickStyleVals;
    using mxConstants = mxGraph.util.mxConstants;

    public class Shape : Style
    {
        public const long VSDX_START_TIME = -2209168800000L; //new Date("12/30/1899").getTime();
                                                             /// <summary>
                                                             /// The text element of the shape, if any
                                                             /// </summary>
        protected internal Element text;

        /// <summary>
        /// The text fields of the shape, if any
        /// </summary>
        protected internal LinkedHashMap<string, string> fields;

        /// <summary>
        /// List of paragraphs in this shape
        /// </summary>
        protected internal LinkedHashMap<string, Paragraph> paragraphs = null;

        /// <summary>
        /// mxGraph cell style map
        /// </summary>
        protected internal IDictionary<string, string> styleMap = new Dictionary<string, string>();

        /// <summary>
        /// Width of shape
        /// </summary>
        protected internal double width = 0;

        /// <summary>
        /// Height of shape
        /// </summary>
        protected internal double height = 0;

        /// <summary>
        /// Cumulative rotation of shape, including parents
        /// </summary>
        protected internal double rotation = 0;

        protected internal double lastX = 0;

        protected internal double lastY = 0;

        protected internal double lastMoveX = 0;

        protected internal double lastMoveY = 0;

        protected internal double lastKnot = -1;

        protected internal IList<Element> geom;

        protected internal mxVsdxGeometryList geomList = null;
        protected internal bool geomListProcessed = false;

        protected internal IDictionary<string, string> imageData;

        protected internal mxVsdxTheme theme;

        protected internal int themeVariant = 0;

        protected internal QuickStyleVals quickStyleVals;

        protected internal static readonly string UNICODE_LINE_SEP = new string(new char[] { (char)226, (char)128, (char)168 });

        public mxPathDebug debug = null;

        public Shape(Element shape, mxVsdxModel model) : base(shape, model)
        {
            this.width = getScreenNumericalValue(
                (this.cellElements.ContainsKey(mxVsdxConstants.WIDTH) ? this.cellElements[mxVsdxConstants.WIDTH] : null)
                , 0);


            this.height = getScreenNumericalValue(
                (this.cellElements.ContainsKey(mxVsdxConstants.HEIGHT) ? this.cellElements[mxVsdxConstants.HEIGHT] : null),
                0);
        }

        public virtual void setThemeAndVariant(mxVsdxTheme theme, int themeVariant)
        {
            this.theme = theme;
            this.themeVariant = themeVariant;
        }

        public override mxVsdxTheme Theme
        {
            get
            {
                if (theme != null)
                {
                    theme.Variant = themeVariant;
                }
                return theme;
            }
        }

        public override QuickStyleVals QuickStyleVals
        {
            get
            {
                return quickStyleVals;
            }
        }

        protected internal virtual void processGeomList(mxVsdxGeometryList parentGeoList)
        {
            if (!geomListProcessed)
            {
                geomList = new mxVsdxGeometryList(parentGeoList);

                if (geom != null)
                {
                    foreach (Element geoElem in geom)
                    {
                        geomList.addGeometry(geoElem);
                    }
                }

                geomListProcessed = true;
            }
        }

        /// <summary>
        /// Caches the specified element </summary>
        /// <param name="elem"> the element to cache </param>
        protected internal override void parseShapeElem(Element elem, mxVsdxModel model)
        {
            base.parseShapeElem(elem, model);

            string childName = elem.Name;

            if (childName.Equals("ForeignData"))
            {
                string filename = elem.OwnerDocument.BaseURI;//elem.OwnerDocument.DocumentURI;
                string iType = elem.GetAttribute("ForeignType");
                string compression = elem.GetAttribute("CompressionType");

                if (iType.Equals("Bitmap"))
                {
                    compression = compression.ToLower();
                }
                else if (iType.Equals("MetaFile"))
                {
                    compression = "x-wmf";
                }
                else if (iType.Equals("Enhanced Metafile") || iType.Equals("EnhMetaFile"))
                {
                    compression = "x-emf";
                }
                else
                {
                    //TODO log and unsupported type
                    return;
                }

                Node fdChild = elem.FirstChild;

                if (fdChild != null)
                {
                    if (fdChild is Element)
                    {
                        Element fdElem = (Element)fdChild;
                        string grandchildName = fdElem.Name;

                        if (grandchildName.ToLower().Equals("rel"))
                        {
                            string rid = fdElem.GetAttribute("r:id");

                            if (!string.ReferenceEquals(rid, null) && rid.Length > 0)
                            {
                                // insert "_rel" into the path
                                int index = filename.LastIndexOf('/');
                                string pre = "";
                                string post = "";

                                try
                                {
                                    pre = filename.Substring(0, index);
                                    post = filename.Substring(index, filename.Length - index);
                                }
                                catch (System.IndexOutOfRangeException)
                                {
                                    return;
                                }

                                Element relElem = model.getRelationship(rid, pre + "/_rels" + post + ".rels");

                                if (relElem != null)
                                {
                                    string target = relElem.GetAttribute("Target");
                                    string type = relElem.GetAttribute("Type");
                                    index = target.LastIndexOf('/');

                                    try
                                    {
                                        target = target.Substring(index + 1, target.Length - (index + 1));
                                    }
                                    catch (System.IndexOutOfRangeException)
                                    {
                                        return;
                                    }

                                    if (!string.ReferenceEquals(type, null) && type.EndsWith("image", StringComparison.Ordinal))
                                    {
                                        this.imageData = new Dictionary<string, string>();
                                        string iData = model.getMedia(mxVsdxCodec.vsdxPlaceholder + "/media/" + target);
                                        this.imageData["iData"] = iData;

                                        //since we convert BMP files to PNG, we set the compression to PNG
                                        if (target.ToLower().EndsWith(".bmp", StringComparison.Ordinal))
                                        {
                                            compression = "png";
                                        }
                                        else if (target.ToLower().EndsWith(".emf", StringComparison.Ordinal))
                                        {
                                            //emf can be a png or jpg or vector (which is not supported yet)
                                            //We use a number of bytes equal to file header length (which is safe as header in base64 requires 4n/3 bytes
                                            compression = iData.StartsWith("iVBORw0K", StringComparison.Ordinal) ? "png" : (iData.StartsWith("/9j/", StringComparison.Ordinal) ? "jpg" : compression);
                                        }

                                        this.imageData["iType"] = compression;
                                    }
                                }
                                else
                                {
                                    //TODO log path issue
                                }

                                // more than one rel would break things
                                return;
                            }


                        }
                    }

                    fdChild = fdChild.NextSibling;
                }
            }
            else if (childName.Equals(mxVsdxConstants.TEXT))
            {
                this.text = elem;
            }
        }

        /// <summary>
        /// Caches the specific section element </summary>
        /// <param name="elem"> the element to cache </param>
        protected internal override void parseSection(Element elem)
        {
            string n = elem.GetAttribute("N");

            if (n.Equals("Geometry"))
            {
                if (geom == null)
                {
                    geom = new List<Element>();
                }

                this.geom.Add(elem);
            }
            else if (n.Equals("Field"))
            {
                List<Element> rows = mxVsdxUtils.getDirectChildNamedElements(elem, "Row");

                foreach (Element row in rows)
                {
                    string ix = row.GetAttribute("IX");

                    if (ix.Length > 0)
                    {
                        if (this.fields == null)
                        {
                            fields = new LinkedHashMap<string, string>();
                        }

                        string del = row.GetAttribute("Del");

                        //supporting deletion of a field by adding an empty string such that master field is not used
                        if ("1".Equals(del))
                        {
                            this.fields.Add(ix, "");
                            continue;
                        }

                        List<Element> cells = mxVsdxUtils.getDirectChildNamedElements(row, "Cell");

                        string value = "", format = "", calendar = "", type = "";
                        foreach (Element cell in cells)
                        {
                            n = cell.GetAttribute("N");
                            string v = cell.GetAttribute("V");

                            switch (n)
                            {
                                case "Value":
                                    value = v;
                                    break;
                                case "Format":
                                    format = v;
                                    break;
                                case "Calendar":
                                    calendar = v;
                                    break;
                                case "Type":
                                    type = v;
                                    break;
                            }
                        }

                        if (value.Length > 0)
                        {

                            try
                            {
                                //TODO support other formats and calendars
                                if (format.StartsWith("{{", StringComparison.Ordinal))
                                {
                                    value = new DateTime(VSDX_START_TIME + (long)(double.Parse(value) * 24 * 60 * 60 * 1000)).ToShortDateString();//(new SimpleDateFormat(format.replaceAll("\\{|\\}", ""))).format(new DateTime(VSDX_START_TIME + (long)(double.Parse(value) * 24 * 60 * 60 * 1000)));
                                                                                                                                                  //the value is the number of days after 30/12/1899 (VSDX_START_TIME)
                                }
                            }
                            catch (Exception)
                            {
                                //						System.out.println("Vsdx import: Unkown text format " + format + ". Error: " + e.getMessage());
                            }
                            this.fields.Add(ix, value);
                        }
                    }
                }
            }
            else
            {
                base.parseSection(elem);
            }
        }

        /// 
        /// <returns> mxGraph stencil XML or null or there is no displayed geometry </returns>
        protected internal virtual string parseGeom()
        {
            if (!hasGeomList())
            {
                return "";
            }

            return geomList.getShapeXML(this);
        }

        /// <summary>
        /// Returns the value of the Text element. </summary>
        /// <returns> Value of the Text element. </returns>
        public virtual string Text
        {
            get
            {
                return this.text != null ? text.InnerText : null;
            }
        }

        /// <summary>
        /// Returns the children Nodes of Text. </summary>
        /// <returns> List with the children of the Text element. </returns>
        public virtual NodeList TextChildren
        {
            get
            {
                return this.text != null ? text.ChildNodes : null;
            }
        }

        /// <summary>
        /// Returns the value of the width element in pixels. </summary>
        /// <returns> Numerical value of the width element. </returns>
        public virtual double Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        /// Returns the value of the height element in pixels. </summary>
        /// <returns> Numerical value of the height element. </returns>
        public virtual double Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// Returns the value of the rotation. </summary>
        /// <returns> Numerical value of the rotation </returns>
        public virtual double Rotation
        {
            get
            {
                return this.rotation;
            }
        }

        /// <summary>
        /// Returns the style map of this shape </summary>
        /// <returns> the style map </returns>
        public virtual IDictionary<string, string> StyleMap
        {
            get
            {
                return this.styleMap;
            }
        }

        /// <summary>
        /// Returns whether or not this shape has a geometry defined, locally
        /// or inherited </summary>
        /// <returns> whether the shape has a geometry </returns>
        public virtual bool hasGeom()
        {
            return !(this.geom == null || this.geom.Count == 0);
        }

        /// <summary>
        /// Returns whether or not this shape or its master has a geometry defined </summary>
        /// <returns> whether the shape has a geometry </returns>
        public virtual bool hasGeomList()
        {
            return this.geomList != null && this.geomList.hasGeom();
        }

        /// <summary>
        /// Last cp IX referenced in the Text Element.
        /// </summary>
        internal string cp = "0";

        /// <summary>
        /// Last pp IX referenced in the Text Element.
        /// </summary>
        internal string pp = "0";

        /// <summary>
        /// Last tp IX referenced in the Text Element.
        /// </summary>
        internal string tp = "0";

        /// <summary>
        /// Last fld IX referenced in the Text Element.
        /// </summary>
        internal string fld = "0";







        /// <summary>
        /// Transform plain text into a HTML list if the Para element referenced by
        /// pp indicates it. </summary>
        /// <param name="text"> Text to be transformed. </param>
        /// <param name="pp"> Reference to a Para element. </param>
        /// <returns> Text like a HTML list. </returns>
        public virtual string textToList(string text, string pp)
        {
            if (!pp.Equals(""))
            {
                string bullet = getBullet(pp);

                if (!bullet.Equals("0"))
                {
                    string[] entries = text.Split("\n", true);
                    string ret = "";

                    foreach (string entry in entries)
                    {
                        ret += mxVsdxUtils.surroundByTags(entry, "li");
                    }

                    ret = mxVsdxUtils.surroundByTags(ret, "ul");
                    Dictionary<string, string> styleMap = new Dictionary<string, string>();

                    if (bullet.Equals("4"))
                    {
                        styleMap["list-style-type"] = "square";
                    }
                    else
                    {
                        styleMap["list-style-type"] = "disc";
                    }

                    ret = this.insertAttributes(ret, styleMap);

                    return ret;
                }
            }

            return text;
        }

        /// <summary>
        /// Returns the paragraph formated according the properties in the last
        /// Para element referenced. </summary>
        /// <param name="para"> Paragraph to be formated </param>
        /// <returns> Formated paragraph. </returns>
        public virtual string getTextParagraphFormated(string para)
        {
            string ret = "";
            Dictionary<string, string> styleMap = new Dictionary<string, string>();
            styleMap["align"] = getHorizontalAlign(pp, true);
            styleMap["margin-left"] = getIndentLeft(pp);
            styleMap["margin-right"] = getIndentRight(pp);
            styleMap["margin-top"] = getSpBefore(pp) + "px";
            styleMap["margin-bottom"] = getSpAfter(pp) + "px";
            styleMap["text-indent"] = getIndentFirst(pp);
            styleMap["valign"] = AlignVertical;
            //		String spc = getSpcLine(pp);
            //TODO dividing by 0.71 gives very large line height in most of the cases. Probably we don't need it?
            //		String spcNum = spc.replaceAll("[^\\d.]", "");
            //		String postFix = spc.substring(spcNum.length(),spc.length());
            //double lineH = (Double.parseDouble(spcNum) / 0.71);
            //		spc = Double.toString(lineH);
            //		
            //		if (spc.contains("."))
            //		{
            //			spc = spc.substring(0, spc.lastIndexOf(".") + 3);
            //		}
            //		
            //		spc = spc + postFix;
            //		styleMap.put("line-height", spc);
            styleMap["direction"] = getTextDirection(pp);
            ret += insertAttributes(para, styleMap);
            return ret;
        }

        /// <summary>
        /// Returns the text formated according the properties in the last
        /// Char element referenced. </summary>
        /// <param name="text"> Text to be formated </param>
        /// <returns> Formated text. </returns>
        public virtual string getTextCharFormated(string text)
        {
            string ret = "";
            string color = "color:" + getTextColor(cp) + ";";
            string size = "font-size:" + (double.Parse(this.getTextSize(cp))) + "px;";
            string font = "font-family:" + this.getTextFont(cp) + ";";
            string direction = "direction:" + this.getRtlText(cp) + ";";
            string space = "letter-spacing:" + (double.Parse(this.getLetterSpace(cp)) / 0.71) + "px;";
            string lineHeight = "line-height:" + getSpcLine(pp);
            string opacity = ";opacity:" + getTextOpacity(cp);
            string pos = this.getTextPos(cp);
            string tCase = getTextCase(cp);

            if (tCase.Equals("1"))
            {
                text = text.ToUpper();
            }
            else if (tCase.Equals("2"))
            {
                text = mxVsdxUtils.toInitialCapital(text);
            }

            if (pos.Equals("1"))
            {
                text = mxVsdxUtils.surroundByTags(text, "sup");
            }
            else if (pos.Equals("2"))
            {
                text = mxVsdxUtils.surroundByTags(text, "sub");
            }

            text = this.isBold(cp) ? mxVsdxUtils.surroundByTags(text, "b") : text;
            text = this.isItalic(cp) ? mxVsdxUtils.surroundByTags(text, "i") : text;
            text = this.isUnderline(cp) ? mxVsdxUtils.surroundByTags(text, "u") : text;
            text = this.getTextStrike(cp) ? mxVsdxUtils.surroundByTags(text, "s") : text;
            text = this.isSmallCaps(cp) ? mxVsdxUtils.toSmallCaps(text, this.getTextSize(cp)) : text;

            ret += "<font style=\"" + size + font + color + direction + space + lineHeight + opacity + "\">" + text + "</font>";
            return ret;
        }

        /// <summary>
        /// Returns the direction of the text. It may be right to left or left to right.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default style-sheet. </summary>
        /// <param name="index"> Index of the Para element that contains the Flags element. </param>
        /// <returns> The direction of the text. </returns>
        public virtual string getTextDirection(string index)
        {
            string direction = getFlags(index);

            if (direction.Equals("0"))
            {
                direction = "ltr";
            }
            else if (direction.Equals("1"))
            {
                direction = "rtl";
            }

            return direction;
        }

        /// <summary>
        /// Returns the space between lines in a paragraph.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default style-sheet. </summary>
        /// <param name="index"> Index of the Para element that contains the SpLine element. </param>
        /// <returns> The space between lines n pixels. </returns>
        public virtual string getSpcLine(string index)
        {
            string ret = "0";
            bool isPercent = false;
            double space = getSpLine(index);

            if (space > 0)
            {
                space = space * mxVsdxUtils.conversionFactor;
            }
            else if (space == 0)
            {
                space = 100;
                isPercent = true;
            }
            else
            {
                space = Math.Abs(space) * 100;
                isPercent = true;
            }

            ret = space.ToString();
            ret += isPercent ? "%" : "px";

            return ret;
        }

        /// <summary>
        /// Returns the space before a paragraph.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default style-sheet. </summary>
        /// <param name="index"> Index of the Para element that contains the SpBefore element. </param>
        /// <returns> The space before the paragraph in pixels. </returns>
        public virtual string getSpcBefore(string index)
        {
            return getSpBefore(index);
        }

        /// <summary>
        /// Inserts the style attributes contained in attr into the text.<br/>
        /// The text must be surrounded by tags html. </summary>
        /// <param name="text"> Text where the attributes must be inserted. </param>
        /// <param name="attr"> Map with the attributes. </param>
        /// <returns> Text with the attributes applied like style. </returns>
        public virtual string insertAttributes(string text, Dictionary<string, string> attr)
        {
            if (text.Contains(">"))
            {
                int i = text.IndexOf(">", StringComparison.Ordinal);
                string tail = text.Substring(i);
                string head = text.Substring(0, i);

                string style = " style=\"" + mxVsdxUtils.getStyleString(attr, ":") + "\"";
                return head + style + tail;
            }

            return text;
        }

        /// <summary>
        /// Returns the direction of the text. It may be right to left or left to right.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default stylesheet. </summary>
        /// <param name="index"> Index of the Char element that contains the RTLText element. </param>
        /// <returns> Direction of the text. </returns>
        public virtual string getRtlText(string index)
        {
            Element rtlElem = getCellElement(mxVsdxConstants.RTL_TEXT, index, mxVsdxConstants.PARAGRAPH);
            string direction = getValue(rtlElem, "ltr");


            if (direction.Equals("0"))
            {
                direction = "ltr";
            }
            else if (direction.Equals("1"))
            {
                direction = "rtl";
            }

            return direction;
        }

        /// <summary>
        /// Checks if the style property of the Char element of index = 'index' 
        /// indicates bold.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default stylesheet. </summary>
        /// <param name="index"> Index of the Char element that contains the Style element. </param>
        /// <returns> Returns <code>true</code> if the style property of the Char element of 
        /// index = 'index' indicates bold. </returns>
        public virtual bool isBold(string index)
        {
            bool isBold = false;
            string style = getTextStyle(index);

            if (!style.Equals(""))
            {
                if (style.ToLower().Equals("themed"))
                {
                    // TODO theme support
                }
                else
                {
                    int value = int.Parse(style);
                    isBold = ((value & 1) == 1);
                }
            }

            return isBold;
        }

        /// <summary>
        /// Checks if the style property of the Char element of index = 'index' 
        /// indicates italic.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default stylesheet. </summary>
        /// <param name="index"> Index of the Char element that contains the Style element. </param>
        /// <returns> Returns <code>true</code> if the style property of the Char element of 
        /// index = 'index' indicates italic. </returns>
        public virtual bool isItalic(string index)
        {
            bool isItalic = false;
            string style = getTextStyle(index);

            if (!style.Equals(""))
            {
                if (style.ToLower().Equals("themed"))
                {
                    // TODO theme support
                }
                else
                {
                    int value = int.Parse(style);
                    isItalic = ((value & 2) == 2);
                }
            }

            return isItalic;
        }

        /// <summary>
        /// Checks if the style property of the Char element of index = 'index' 
        /// indicates underline.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default stylesheet. </summary>
        /// <param name="index"> Index of the Char element that contains the Style element. </param>
        /// <returns> Returns <code>true</code> if the style property of the Char element of 
        /// index = 'index' indicates underline. </returns>
        public virtual bool isUnderline(string index)
        {
            bool isUnderline = false;
            string style = getTextStyle(index);

            if (!style.Equals(""))
            {
                if (style.ToLower().Equals("themed"))
                {
                    // TODO theme support
                }
                else
                {
                    int value = int.Parse(style);
                    isUnderline = ((value & 4) == 4);
                }
            }

            return isUnderline;
        }

        /// <summary>
        /// Checks if the style property of the Char element of index = 'index'
        /// indicates small caps.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default stylesheet. </summary>
        /// <param name="index"> Index of the Char element that contains the Style element. </param>
        /// <returns> Returns <code>true</code> if the style property of the Char element of
        /// index = 'index' indicates small caps. </returns>
        public virtual bool isSmallCaps(string index)
        {
            bool isSmallCaps = false;
            string style = getTextStyle(index);

            if (!style.Equals(""))
            {
                if (style.ToLower().Equals("themed"))
                {
                    // TODO theme support
                }
                else
                {
                    int value = int.Parse(style);
                    isSmallCaps = ((value & 8) == 8);
                }
            }

            return isSmallCaps;
        }

        public virtual string getTextOpacity(string index)
        {
            Element colorTrans = getCellElement(mxVsdxConstants.COLOR_TRANS, index, mxVsdxConstants.CHARACTER);
            string trans = getValue(colorTrans, "1");
            string result = "1";

            if (!string.ReferenceEquals(trans, null) && trans.Length > 0)
            {
                double tmp = 1.0 - Convert.ToDouble(trans);
                result = tmp.ToString();
            }

            return result;
        }

        /// <summary>
        /// Returns the actual text size defined by the Char element referenced in cp.<br/>
        /// This property may to be founded in the shape, master shape, stylesheet or
        /// default stylesheet. </summary>
        /// <param name="index"> Index of the Char element that contains the Size element. </param>
        /// <returns> Returns the size of the font in pixels. </returns>
        public virtual string getTextSize(string index)
        {
            Element sizeElem = getCellElement(mxVsdxConstants.SIZE, index, mxVsdxConstants.CHARACTER);
            double size = getScreenNumericalValue(sizeElem, 12);

            return size.ToString();
        }

        /// <summary>
        /// Returns the vertical align of the label.<br/>
        /// The property may to be defined in master shape or text stylesheet.<br/> </summary>
        /// <returns> Vertical align (bottom, middle and top) </returns>
        public virtual string AlignVertical
        {
            get
            {
                string vertical = mxConstants.ALIGN_MIDDLE;

                int align = int.Parse(getValue(this.getCellElement(mxVsdxConstants.VERTICAL_ALIGN), "1"));

                if (align == 0)
                {
                    vertical = mxConstants.ALIGN_TOP;
                }
                else if (align == 2)
                {
                    vertical = mxConstants.ALIGN_BOTTOM;
                }

                return vertical;
            }
        }

        public virtual mxVsdxGeometryList GeomList
        {
            get
            {
                return geomList;
            }
        }

        public virtual double LastX
        {
            get
            {
                return lastX;
            }
            set
            {
                this.lastX = value;
            }
        }

        public virtual double LastY
        {
            get
            {
                return lastY;
            }
            set
            {
                this.lastY = value;
            }
        }

        public virtual double LastMoveX
        {
            get
            {
                return lastMoveX;
            }
            set
            {
                this.lastMoveX = value;
            }
        }

        public virtual double LastMoveY
        {
            get
            {
                return lastMoveY;
            }
            set
            {
                this.lastMoveY = value;
            }
        }

        public virtual double LastKnot
        {
            get
            {
                return lastKnot;
            }
            set
            {
                this.lastKnot = value;
            }
        }





    }

}