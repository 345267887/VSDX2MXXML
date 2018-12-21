using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxVsdxTheme
    {
        //Theme names to ID mapping
        private static Dictionary<String, int?> themesIds = new Dictionary<String, int?>
    {
            //Theme index can be found deep inside the theme file, so this is faster for standard 2013 format
            { "Office", 33},
        {"Linear", 34},
        {"Zephyr", 35},
        {"Integral", 36},
        {"Simple", 37},
        {"Whisp", 38},
        {"Daybreak", 39},
        {"Parallel", 40},
        {"Sequence", 41},
        {"Slice", 42},
        {"Ion", 43},
        {"Retrospect", 44},
        {"Organic", 45},
        {"Bubble", 46},
        {"Clouds", 47},
        {"Gemstone", 48},
        {"Lines", 49},
        {"Facet", 50},
        {"Prominence", 51},
        {"Smoke", 52},
        {"Radiance", 53},
        {"Shade", 54},
        {"Pencil", 55},
        {"Pen", 56},
        {"Marker", 57},
        {"Whiteboard", 58}
    };

        //color id to color name
        private static Dictionary<int, String> colorIds = new Dictionary<int, String>{
        //https://msdn.microsoft.com/en-us/library/hh661351%28v=office.12%29.aspx
        //There are non standard values of 200 -> 206 also which are handled the same as 100 -> 106
        { 0, "dk1"},
        {1, "lt1"},
        {2, "accent1"},
        {3, "accent2"},
        {4, "accent3"},
        {5, "accent4"},
        {6, "accent5"},
        {7, "accent6"}
    };

        private XmlElement theme;

        private int themeIndex = -1;

        private int themeVariant = 0;

        //colors handling
        private Dictionary<String, OoxmlColor> baseColors = new Dictionary<String, OoxmlColor>();

        //Dynamic background color (index 8)
        private OoxmlColor bkgndColor;

        //Variant colors
        private OoxmlColor[,] variantsColors = new OoxmlColor[4,7];

        private bool[] isMonotoneVariant = new bool[4];

        private Color defaultClr = new Color(255, 255, 255);
        private Color defaultLineClr = new Color(0, 0, 0);

        private LineStyle defaultLineStyle = new LineStyle();

        //fill styles
        private List<FillStyle> fillStyles = new List<FillStyle>(6);

        //connector fill styles
        //TODO what is the use of it?
        private List<FillStyle> connFillStyles = new List<FillStyle>(6);

        //line styles
        private List<LineStyle> lineStyles = new List<LineStyle>(6);

        //cpnector line styles
        private List<LineStyle> connLineStyles = new List<LineStyle>(6);

        //line styles extensions
        private List<LineStyleExt> lineStylesExt = new List<LineStyleExt>(7);

        //connector line styles extensions
        private List<LineStyleExt> connLineStylesExt = new List<LineStyleExt>(7);

        //connector font color & styles
        private List<OoxmlColor> connFontColors = new List<OoxmlColor>(6);
        private List<int> connFontStyles = new List<int>(6);

        //font color & styles
        private List<OoxmlColor> fontColors = new List<OoxmlColor>(6);
        private List<int> fontStyles = new List<int>(6);

        private int[] variantEmbellishment = new int[4];
        private int[,] variantFillIdx = new int[4,4];
        private int[,] variantLineIdx = new int[4,4];
        private int[,] variantEffectIdx = new int[4,4];
        private int[,] variantFontIdx = new int[4,4];

        private bool isProcessed = false;

        //flag to indicate that some parts of the theme has different name
        private bool _isPure = true;
        private String name;

        public mxVsdxTheme(XmlElement theme)
        {
            this.theme = theme;
            this.name = theme.GetAttribute("name");

            int? themeId = themesIds[this.name];

            if (themeId.HasValue)
            {
                themeIndex = themeId.Value;
            }
        }

        public int getThemeIndex()
        {
            return themeIndex;
        }

        public void setVariant(int variant)
        {
            themeVariant = variant;
        }

        public bool isPure()
        {
            return _isPure;
        }

        public void processTheme()
        {
            if (isProcessed) return;

            try
            {
                XmlNode child = theme.FirstChild;

                while (child != null)
                {
                    if (child is XmlElement && ((XmlElement)child).Name.Equals("a:themeElements"))
				{
                        XmlNode child2 = child.FirstChild;
                        while (child2 != null)
                        {
                            if (child2 is XmlElement)
						{
                                XmlElement elem = (XmlElement)child2;
                                String nodeName = elem.Name;
                                if (nodeName.Equals("a:clrScheme"))
                                {
                                    if (!this.name.Equals(elem.GetAttribute("name")))
                                    {
                                        _isPure = false;
                                    }
                                    //Process the color scheme
                                    processColors(elem);
                                }
                                else if (nodeName.Equals("a:fontScheme"))
                                {
                                    if (!this.name.Equals(elem.GetAttribute("name")))
                                    {
                                        _isPure = false;
                                    }
                                    //Process the font scheme
                                    processFonts(elem);
                                }
                                else if (nodeName.Equals("a:fmtScheme"))
                                {
                                    if (!this.name.Equals(elem.GetAttribute("name")))
                                    {
                                        _isPure = false;
                                    }
                                    //Process the format scheme
                                    processFormats(elem);
                                }
                                else if (nodeName.Equals("a:extLst"))
                                {
                                    //Process the extra list
                                    processExtras(elem);
                                }
                            }
                            child2 = child2.NextSibling;
                        }
                    }
                    child = child.NextSibling;
                }
            }
            catch (Exception e)
            {
                //cannot parse the theme format, probably it has non-standard format
               // e.printStackTrace();
            }
            isProcessed = true;
        }

        private void processExtras(XmlElement element)
        {
            List<XmlElement> exts = mxVsdxUtils.getDirectChildElements(element);

            foreach (XmlElement ext in exts)
            {
                XmlElement vt = mxVsdxUtils.getDirectFirstChildElement(ext);
                switch (vt.Name)
                {
                    case "vt:fmtConnectorScheme":
                        if (!this.name.Equals(vt.GetAttribute("name")))
                        {
                            _isPure = false;
                        }
                        List<XmlElement> connSchemes = mxVsdxUtils.getDirectChildElements(vt);

                        foreach (XmlElement scheme in connSchemes)
                        {
                            String name = scheme.Name;

                            switch (name)
                            {
                                case "a:fillStyleLst":
                                    List<XmlElement> fillStyleElems = mxVsdxUtils.getDirectChildElements(scheme);
                                    foreach (XmlElement fillStyle in fillStyleElems)
                                    {
                                        connFillStyles.Add(FillStyleFactory.getFillStyle(fillStyle));
                                    }
                                    break;
                                case "a:lnStyleLst":
                                    List<XmlElement> lineStyleElems = mxVsdxUtils.getDirectChildElements(scheme);
                                    foreach (XmlElement lineStyle in lineStyleElems)
                                    {
                                        connLineStyles.Add(new LineStyle(lineStyle));
                                    }
                                    break;
                            }
                        }
                        break;
                    case "vt:lineStyles":
                        List<XmlElement> styles = mxVsdxUtils.getDirectChildElements(vt);

                        foreach (XmlElement style in styles)
                        {
                            String name = style.Name;

                            switch (name)
                            {
                                case "vt:fmtConnectorSchemeLineStyles":
                                    List<XmlElement> connStylesElems = mxVsdxUtils.getDirectChildElements(style);
                                    foreach (XmlElement connStyle in connStylesElems)
                                    {
                                        connLineStylesExt.Add(new LineStyleExt(connStyle));
                                    }
                                    break;
                                case "vt:fmtSchemeLineStyles":
                                    List<XmlElement> schemeStyleElems = mxVsdxUtils.getDirectChildElements(style);
                                    foreach (XmlElement schemeStyle in schemeStyleElems)
                                    {
                                        lineStylesExt.Add(new LineStyleExt(schemeStyle));
                                    }
                                    break;
                            }
                        }
                        break;
                    case "vt:fontStylesGroup":
                        List<XmlElement> fontStyleElems = mxVsdxUtils.getDirectChildElements(vt);

                        foreach (XmlElement fontStyle in fontStyleElems)
                        {
                            String name = fontStyle.Name;

                            switch (name)
                            {
                                case "vt:connectorFontStyles":
                                    fillFontStyles(fontStyle, connFontColors, connFontStyles);
                                    break;
                                case "vt:fontStyles":
                                    fillFontStyles(fontStyle, fontColors, fontStyles);
                                    break;
                            }
                        }
                        break;
                    case "vt:variationStyleSchemeLst":
                        List<XmlElement> varStyleSchemes = mxVsdxUtils.getDirectChildElements(vt);

                        int i = 0;
                        foreach (XmlElement varStyleScheme in varStyleSchemes)
                        {
                            variantEmbellishment[i] = mxVsdxUtils.getIntAttr(varStyleScheme, "embellishment");

                            List<XmlElement> varStyles = mxVsdxUtils.getDirectChildElements(varStyleScheme);
                            int j = 0;
                            foreach (XmlElement varStyle in varStyles)
                            {
                                variantFillIdx[i,j] = mxVsdxUtils.getIntAttr(varStyle, "fillIdx");
                                variantLineIdx[i,j] = mxVsdxUtils.getIntAttr(varStyle, "lineIdx");
                                variantEffectIdx[i,j] = mxVsdxUtils.getIntAttr(varStyle, "effectIdx");
                                variantFontIdx[i,j] = mxVsdxUtils.getIntAttr(varStyle, "fontIdx");
                                j++;
                            }
                            i++;
                        }
                        break;
                }
            }
        }

        private void fillFontStyles(XmlElement fontStyle, List<OoxmlColor> fontColors, List<int> fontStyles)
        {
            List<XmlElement> fontProps = mxVsdxUtils.getDirectChildElements(fontStyle);

            foreach (XmlElement fontProp in fontProps)
            {
                fontStyles.Add(mxVsdxUtils.getIntAttr(fontProp, "style"));

                XmlElement color = mxVsdxUtils.getDirectFirstChildElement(fontProp);
                if (color != null)
                    fontColors.Add(
                            OoxmlColorFactory.getOoxmlColor(
                                    mxVsdxUtils.getDirectFirstChildElement(color)));
            }
        }

        private void processFormats(XmlElement element)
        {
            List<XmlElement> styles = mxVsdxUtils.getDirectChildElements(element);
            foreach (XmlElement style in styles)
            {
                String name = style.Name;
                switch (name)
                {
                    case "a:fillStyleLst":
                        List<XmlElement> fillStyleElems = mxVsdxUtils.getDirectChildElements(style);
                        foreach (XmlElement fillStyle in fillStyleElems)
                        {
                            fillStyles.Add(FillStyleFactory.getFillStyle(fillStyle));
                        }
                        break;
                    case "a:lnStyleLst":
                        List<XmlElement> lineStyleElems = mxVsdxUtils.getDirectChildElements(style);
                        foreach (XmlElement lineStyle in lineStyleElems)
                        {
                            lineStyles.Add(new LineStyle(lineStyle));
                        }
                        break;
                    case "a:effectStyleLst":
                        //TODO effects most probably are not used by vsdx
                        break;
                    case "a:bgFillStyleLst":
                        //TODO background effects most probably are not used by vsdx
                        break;
                }
            }
        }

        private void processFonts(XmlElement element)
        {
            // TODO Fonts has only the name of the font for each language. It looks not important
        }

        private void processColors(XmlElement element)
        {
            XmlNode child = element.FirstChild;

            while (child != null)
            {
                if (child is XmlElement)
			{
                    XmlElement elem = (XmlElement)child;
                    String nodeName = elem.Name;
                    List<XmlElement> children = mxVsdxUtils.getDirectChildElements(elem);
                    if (nodeName.Equals("a:extLst"))
                    {
                        if (children.Count == 3) //the format has three a:ext nodes
                        {
                            if (themeIndex < 0)
                            {
                                extractThemeIndex(children[0]);
                            }
                            addBkgndColor(children[1]);
                            addVariantColors(children[2]);
                        }
                    }
                    else
                    {
                        String clrName = nodeName.Substring(2);

                        if (children.Count > 0)
                        {
                            addBasicColor(clrName, children[0]);
                        }
                    }
                }
                child = child.NextSibling;
            }
        }

        private void addVariantColors(XmlElement element)
        {
            XmlElement parent = mxVsdxUtils.getDirectFirstChildElement(element);

            if (parent != null)
            {
                List<XmlElement> variants = mxVsdxUtils.getDirectChildElements(parent);
                int i = 0;
                foreach (XmlElement variant in variants)
                {
                    addVariantColorsSet(i++, variant);
                }
            }
        }

        private void addVariantColorsSet(int index, XmlElement variant)
        {
            List<XmlElement> colors = mxVsdxUtils.getDirectChildElements(variant);

            isMonotoneVariant[index] = variant.HasAttribute("monotone");

            foreach (XmlElement color in colors)
            {
                String name = color.Name;
                switch (name)
                {
                    case "vt:varColor1":
                        variantsColors[index,0] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor2":
                        variantsColors[index,1] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor3":
                        variantsColors[index,2] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor4":
                        variantsColors[index,3] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor5":
                        variantsColors[index,4] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor6":
                        variantsColors[index,5] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor7":
                        variantsColors[index,6] = OoxmlColorFactory.getOoxmlColor(
                                mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                }
            }
        }

        private void addBkgndColor(XmlElement element)
        {
            XmlElement elem = mxVsdxUtils.getDirectFirstChildElement(element);

            if (elem != null)
            {
                bkgndColor = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(elem));
            }
        }

        private void extractThemeIndex(XmlElement element)
        {
            XmlElement elem = mxVsdxUtils.getDirectFirstChildElement(element);

            if (elem != null)
            {
                themeIndex = int.Parse(elem.GetAttribute("schemeEnum"));
            }
        }

        private void addBasicColor(String clrName, XmlElement element)
        {
            baseColors.Add(clrName, OoxmlColorFactory.getOoxmlColor(element));
        }

        public Color getSchemeColor(String val)
        {
            processTheme();

            OoxmlColor color = baseColors[val];

            return color != null ? color.getColor(this) : defaultClr;
        }

        //	QuickStyleFillColor
        public Color getStyleColor(int styleColor)
        {
            processTheme();

            if (styleColor < 8)
            {
                OoxmlColor color = baseColors[colorIds[styleColor]];
                if (color != null)
                {
                    return color.getColor(this);
                }
            }
            else if (styleColor == 8)
            {
                if (bkgndColor != null)
                {
                    return bkgndColor.getColor(this);
                }
            }
            else
            {
                OoxmlColor color = null;
                int clrIndex = 0;

                if (styleColor >= 200) //200-206
                {
                    clrIndex = styleColor - 200;
                }
                else if (styleColor >= 100) //100-106
                {
                    clrIndex = styleColor - 100;
                }
                if (clrIndex >= 0 && clrIndex <= 6) //0 - 6
                {
                    color = variantsColors[themeVariant,clrIndex];
                }

                if (color != null)
                {
                    return color.getColor(this);
                }
            }
            return defaultClr;
        }


        public Color getFillGraientColor(QuickStyleVals quickStyleVals)
        {
            return getFillColor(quickStyleVals, true);
        }

        public Color getFillColor(QuickStyleVals quickStyleVals)
        {
            return getFillColor(quickStyleVals, false);
        }

        //Get fill color based on QuickStyleFillColor & QuickStyleFillMatrix
        private Color getFillColor(QuickStyleVals quickStyleVals, bool getGradient)
        {
            processTheme();

            int fillColorStyle = quickStyleVals.getQuickStyleFillColor();
            FillStyle fillStyle = null;
            switch (quickStyleVals.getQuickStyleFillMatrix())
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    fillStyle = fillStyles[quickStyleVals.getQuickStyleFillMatrix() - 1];
                    break;
                case 100:
                case 101:
                case 102:
                case 103:
                    if (isMonotoneVariant[themeVariant]) fillColorStyle = 100;

                    int index = quickStyleVals.getQuickStyleFillMatrix() - 100;
                    //get style index of variants
                    fillStyle = fillStyles[variantFillIdx[themeVariant,index] - 1];
                    break;
            }

            Color retColor;
            if (fillStyle != null)
            {
                if (getGradient)
                {
                    retColor = (fillStyle is GradFill)? fillStyle.applyStyle(fillColorStyle, this).getGradientClr() : null;
                }
                else
                {
                    retColor = fillStyle.applyStyle(fillColorStyle, this);
                }
            }
            else
            {
                if (getGradient)
                {
                    retColor = null;
                }
                else
                {
                    retColor = getStyleColor(fillColorStyle);
                }
            }

            int styleVariation = quickStyleVals.getQuickStyleVariation();

            //TODO using the line color does not cover all the cases but works with most of the sample files
            if (retColor != null && (styleVariation & 8) > 0)
            {
                retColor = getLineColor(quickStyleVals);
            }

            return retColor;
        }

        //Get line style based on QuickStyleLineMatrix
        private LineStyle getLineStyle(int quickStyleLineMatrix, List<LineStyle> lineStyles)
        {
            processTheme();

            LineStyle lineStyle = null;
            switch (quickStyleLineMatrix)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    lineStyle = lineStyles[quickStyleLineMatrix - 1];
                    break;
                case 100:
                case 101:
                case 102:
                case 103:
                    int index = quickStyleLineMatrix - 100;
                    //get style index of variants
                    //Edges should not has these values
                    if (lineStyles == this.lineStyles)
                    {
                        lineStyle = this.lineStyles[variantLineIdx[themeVariant,index] - 1];
                    }
                    else
                    {
                        lineStyle = defaultLineStyle;
                    }
                    break;
            }

            return lineStyle;
        }

        private LineStyleExt getLineStyleExt(int quickStyleLineMatrix, List<LineStyleExt> lineStylesExt)
        {
            processTheme();

            LineStyleExt lineStyleExt = null;
            switch (quickStyleLineMatrix)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    lineStyleExt = lineStylesExt[quickStyleLineMatrix];
                    break;
            }

            return lineStyleExt;
        }

        //Get line color based on QuickStyleLineColor & QuickStyleLineMatrix
        private Color getLineColor(QuickStyleVals quickStyleVals, List<LineStyle> lineStyles)
        {
            processTheme();

            int lineColorStyle = quickStyleVals.getQuickStyleLineColor();
            LineStyle lineStyle = getLineStyle(quickStyleVals.getQuickStyleLineMatrix(), lineStyles);
            switch (quickStyleVals.getQuickStyleLineMatrix())
            {
                case 100:
                case 101:
                case 102:
                case 103:
                    if (isMonotoneVariant[themeVariant]) lineColorStyle = 100;
                    break;
            }

            Color lineClr;

            if (lineStyle != null)
            {
                lineClr = lineStyle.getLineColor(lineColorStyle, this);
            }
            else
            {
                lineClr = getStyleColor(lineColorStyle);
            }

            int styleVariation = quickStyleVals.getQuickStyleVariation();

            //TODO using the fill color does not cover all the cases but works with most of the sample files
            if ((styleVariation & 4) > 0)
            {
                lineClr = getFillColor(quickStyleVals);
            }
            return lineClr;
        }

        //Get line color based on QuickStyleLineColor & QuickStyleLineMatrix
        public Color getLineColor(QuickStyleVals quickStyleVals)
        {
            return getLineColor(quickStyleVals, lineStyles);
        }

        //Get connection line color based on QuickStyleLineColor & QuickStyleLineMatrix
        public Color getConnLineColor(QuickStyleVals quickStyleVals)
        {
            return getLineColor(quickStyleVals, connLineStyles);
        }


        public Color getDefaultLineClr()
        {
            return defaultLineClr;
        }

        private bool isLineDashed(QuickStyleVals quickStyleVals, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.getQuickStyleLineMatrix(), lineStylesExt);

            if (lineStyleExt != null)
            {
                return lineStyleExt.isDashed();
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.getQuickStyleLineMatrix(), lineStyles);
                return lineStyle != null ? lineStyle.isDashed() : false;
            }
        }

        public bool isLineDashed(QuickStyleVals quickStyleVals)
        {
            return isLineDashed(quickStyleVals, lineStylesExt, lineStyles);
        }

        public bool isConnLineDashed(QuickStyleVals quickStyleVals)
        {
            return isLineDashed(quickStyleVals, connLineStylesExt, connLineStyles);
        }

        private List<Double> getLineDashPattern(QuickStyleVals quickStyleVals, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.getQuickStyleLineMatrix(), lineStylesExt);

            if (lineStyleExt != null)
            {
                return lineStyleExt.getLineDashPattern();
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.getQuickStyleLineMatrix(), lineStyles);
                return lineStyle != null ? lineStyle.getLineDashPattern() : null;
            }
        }

        public List<Double> getLineDashPattern(QuickStyleVals quickStyleVals)
        {
            return getLineDashPattern(quickStyleVals, lineStylesExt, lineStyles);
        }

        public List<Double> getConnLineDashPattern(QuickStyleVals quickStyleVals)
        {
            return getLineDashPattern(quickStyleVals, connLineStylesExt, connLineStyles);
        }

        private int getArrowSize(QuickStyleVals quickStyleVals, bool isStart, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.getQuickStyleLineMatrix(), lineStylesExt);

            if (lineStyleExt != null)
            {
                return isStart ? lineStyleExt.getStartSize() : lineStyleExt.getEndSize();
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.getQuickStyleLineMatrix(), lineStyles);
                return lineStyle != null ? (isStart ? lineStyle.getStartSize() : lineStyle.getEndSize()) : 4;
            }
        }

        public int getStartSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, true, lineStylesExt, lineStyles);
        }

        public int getConnStartSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, true, connLineStylesExt, connLineStyles);
        }

        public int getEndSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, false, lineStylesExt, lineStyles);
        }

        public int getConnEndSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, false, connLineStylesExt, connLineStyles);
        }

        //Get font color based on QuickStyleFontColor & QuickStyleFontMatrix
        private Color getFontColor(QuickStyleVals quickStyleVals, List<OoxmlColor> fontColors)
        {
            processTheme();

            int fontColorStyle = quickStyleVals.getQuickStyleFontColor();
            OoxmlColor fontColor = null;
            switch (quickStyleVals.getQuickStyleFontMatrix())
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    fontColor = fontColors[quickStyleVals.getQuickStyleFontMatrix() - 1];
                    break;
                case 100:
                case 101:
                case 102:
                case 103:
                    if (isMonotoneVariant[themeVariant]) fontColorStyle = 100;

                    int index = quickStyleVals.getQuickStyleFontMatrix() - 100;
                    //get style index of variants
                    //If an edge has a non-standard value, use the dark color
                    if (fontColors != this.fontColors)
                    {
                        fontColor = this.baseColors["dk1"];
                    }
                    else
                    {
                        fontColor = fontColors[variantFontIdx[themeVariant,index] - 1];
                    }
                    break;
            }

            Color txtColor;

            if (fontColor != null)
            {
                txtColor = fontColor.getColor(fontColorStyle, this);
            }
            else
            {
                txtColor = getStyleColor(fontColorStyle);
            }

            int styleVariation = quickStyleVals.getQuickStyleVariation();

            //TODO The formula in the documentation doesn't match how vsdx viewer works. Simply using the fill/line color works!
            //		Note: Using the fill/line color does not cover all the cases but works with most of the sample files
            if ((styleVariation & 2) > 0)
            {
                Color fillColor = getFillColor(quickStyleVals);
                HSLColor fillHSLClr = fillColor.toHsl();
                //			HSLColor txtColorHSL = txtColor.toHsl();
                //			if (Math.abs(fillHSLClr.getLum() - txtColorHSL.getLum()) < 0.1616)
                //			{
                //				if (fillHSLClr.getLum() < 0.7292)
                //				{
                //					txtColor = new Color(255, 255, 255);
                //				}
                //				else
                //				{
                Color lineClr = getLineColor(quickStyleVals);
                HSLColor lineHSLClr = lineClr.toHsl();
                if (fillHSLClr.getLum() < lineHSLClr.getLum())
                {
                    txtColor = fillColor;
                }
                else
                {
                    txtColor = lineClr;
                }
                //				}
                //			}
            }

            return txtColor;
        }

        //Get font color based on QuickStyleFontColor & QuickStyleFontMatrix
        public Color getFontColor(QuickStyleVals quickStyleVals)
        {
            return getFontColor(quickStyleVals, fontColors);
        }

        //Get connection font color based on QuickStyleFontColor & QuickStyleFontMatrix
        public Color getConnFontColor(QuickStyleVals quickStyleVals)
        {
            return getFontColor(quickStyleVals, connFontColors);
        }

        private int getArrowType(QuickStyleVals quickStyleVals, bool isStart, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.getQuickStyleLineMatrix(), lineStylesExt);

            if (lineStyleExt != null)
            {
                return isStart ? lineStyleExt.getStart() : lineStyleExt.getEnd();
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.getQuickStyleLineMatrix(), lineStyles);
                return lineStyle != null ? (isStart ? lineStyle.getStart() : lineStyle.getEnd()) : 0;
            }
        }

        public int getEdgeMarker(bool isStart, QuickStyleVals quickStyleVals)
        {
            return getArrowType(quickStyleVals, isStart, lineStylesExt, lineStyles);
        }

        public int getConnEdgeMarker(bool isStart, QuickStyleVals quickStyleVals)
        {
            return getArrowType(quickStyleVals, isStart, connLineStylesExt, connLineStyles);
        }


        private int getLineWidth(QuickStyleVals quickStyleVals, List<LineStyle> lineStyles)
        {
            LineStyle lineStyle = getLineStyle(quickStyleVals.getQuickStyleLineMatrix(), lineStyles);
            return lineStyle != null ? lineStyle.getLineWidth() : 0;
        }

        public int getLineWidth(QuickStyleVals quickStyleVals)
        {
            return getLineWidth(quickStyleVals, lineStyles);
        }

        public int getConnLineWidth(QuickStyleVals quickStyleVals)
        {
            return getLineWidth(quickStyleVals, connLineStyles);
        }
    }
}
