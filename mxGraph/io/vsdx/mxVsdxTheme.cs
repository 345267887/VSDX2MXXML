using System;
using System.Collections.Generic;

namespace mxGraph.io.vsdx
{


    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;

    using Color = mxGraph.io.vsdx.theme.Color;
    using FillStyle = mxGraph.io.vsdx.theme.FillStyle;
    using FillStyleFactory = mxGraph.io.vsdx.theme.FillStyleFactory;
    using GradFill = mxGraph.io.vsdx.theme.GradFill;
    using HSLColor = mxGraph.io.vsdx.theme.HSLColor;
    using LineStyle = mxGraph.io.vsdx.theme.LineStyle;
    using LineStyleExt = mxGraph.io.vsdx.theme.LineStyleExt;
    using OoxmlColor = mxGraph.io.vsdx.theme.OoxmlColor;
    using OoxmlColorFactory = mxGraph.io.vsdx.theme.OoxmlColorFactory;
    using QuickStyleVals = mxGraph.io.vsdx.theme.QuickStyleVals;

    //Holds office 2013 theme data which applies to all office file formats
    public class mxVsdxTheme
    {
        //Theme names to ID mapping
        private static IDictionary<string, int?> themesIds = new Dictionary<string, int?>();

        //Theme index can be found deep inside the theme file, so this is faster for standard 2013 format
        static mxVsdxTheme()
        {
            themesIds["Office"] = 33;
            themesIds["Linear"] = 34;
            themesIds["Zephyr"] = 35;
            themesIds["Integral"] = 36;
            themesIds["Simple"] = 37;
            themesIds["Whisp"] = 38;
            themesIds["Daybreak"] = 39;
            themesIds["Parallel"] = 40;
            themesIds["Sequence"] = 41;
            themesIds["Slice"] = 42;
            themesIds["Ion"] = 43;
            themesIds["Retrospect"] = 44;
            themesIds["Organic"] = 45;
            themesIds["Bubble"] = 46;
            themesIds["Clouds"] = 47;
            themesIds["Gemstone"] = 48;
            themesIds["Lines"] = 49;
            themesIds["Facet"] = 50;
            themesIds["Prominence"] = 51;
            themesIds["Smoke"] = 52;
            themesIds["Radiance"] = 53;
            themesIds["Shade"] = 54;
            themesIds["Pencil"] = 55;
            themesIds["Pen"] = 56;
            themesIds["Marker"] = 57;
            themesIds["Whiteboard"] = 58;
            colorIds[0] = "dk1";
            colorIds[1] = "lt1";
            colorIds[2] = "accent1";
            colorIds[3] = "accent2";
            colorIds[4] = "accent3";
            colorIds[5] = "accent4";
            colorIds[6] = "accent5";
            colorIds[7] = "accent6";
        }

        //color id to color name
        private static IDictionary<int?, string> colorIds = new Dictionary<int?, string>();

        //https://msdn.microsoft.com/en-us/library/hh661351%28v=office.12%29.aspx
        //There are non standard values of 200 -> 206 also which are handled the same as 100 -> 106

        private Element theme;

        private int themeIndex = -1;

        private int themeVariant = 0;

        //colors handling
        private IDictionary<string, OoxmlColor> baseColors = new Dictionary<string, OoxmlColor>();

        //Dynamic background color (index 8)
        private OoxmlColor bkgndColor;

        //Variant colors
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: private OoxmlColor[][] variantsColors = new OoxmlColor[4][7];
        private OoxmlColor[,] variantsColors = new OoxmlColor[4, 7]; //RectangularArrays.ReturnRectangularOoxmlColorArray(4, 7);

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
        private List<int?> connFontStyles = new List<int?>(6);

        //font color & styles
        private List<OoxmlColor> fontColors = new List<OoxmlColor>(6);
        private List<int?> fontStyles = new List<int?>(6);

        private int[] variantEmbellishment = new int[4];
        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: private int[][] variantFillIdx = new int[4][4];
        private int[,] variantFillIdx = new int[4, 4]; //RectangularArrays.ReturnRectangularIntArray(4, 4);
                                                       //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                                                       //ORIGINAL LINE: private int[][] variantLineIdx = new int[4][4];
        private int[,] variantLineIdx = new int[4, 4];//RectangularArrays.ReturnRectangularIntArray(4, 4);
                                                      //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                                                      //ORIGINAL LINE: private int[][] variantEffectIdx = new int[4][4];
        private int[,] variantEffectIdx = new int[4, 4];//RectangularArrays.ReturnRectangularIntArray(4, 4);
                                                        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
                                                        //ORIGINAL LINE: private int[][] variantFontIdx = new int[4][4];
        private int[,] variantFontIdx = new int[4, 4];// RectangularArrays.ReturnRectangularIntArray(4, 4);

        private bool isProcessed = false;

        //flag to indicate that some parts of the theme has different name
        private bool isPure = true;
        private string name;

        public mxVsdxTheme(Element theme)
        {
            this.theme = theme;
            this.name = theme.GetAttribute("name");

            int? themeId = themesIds[this.name];

            if (themeId != null)
            {
                themeIndex = themeId.Value;
            }
        }

        public virtual int ThemeIndex
        {
            get
            {
                return themeIndex;
            }
        }

        public virtual int Variant
        {
            set
            {
                themeVariant = value;
            }
        }

        public virtual bool Pure
        {
            get
            {
                return isPure;
            }
        }

        public virtual void processTheme()
        {
            if (isProcessed)
            {
                return;
            }

            try
            {
                Node child = theme.FirstChild;

                while (child != null)
                {
                    if (child is Element && ((Element)child).Name.Equals("a:themeElements"))
                    {
                        Node child2 = child.FirstChild;
                        while (child2 != null)
                        {
                            if (child2 is Element)
                            {
                                Element elem = (Element)child2;
                                string nodeName = elem.Name;
                                if (nodeName.Equals("a:clrScheme"))
                                {
                                    if (!this.name.Equals(elem.GetAttribute("name")))
                                    {
                                        isPure = false;
                                    }
                                    //Process the color scheme
                                    processColors(elem);
                                }
                                else if (nodeName.Equals("a:fontScheme"))
                                {
                                    if (!this.name.Equals(elem.GetAttribute("name")))
                                    {
                                        isPure = false;
                                    }
                                    //Process the font scheme
                                    processFonts(elem);
                                }
                                else if (nodeName.Equals("a:fmtScheme"))
                                {
                                    if (!this.name.Equals(elem.GetAttribute("name")))
                                    {
                                        isPure = false;
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
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            isProcessed = true;
        }

        private void processExtras(Element element)
        {
            List<Element> exts = mxVsdxUtils.getDirectChildElements(element);

            foreach (Element ext in exts)
            {
                Element vt = mxVsdxUtils.getDirectFirstChildElement(ext);
                switch (vt.Name)
                {
                    case "vt:fmtConnectorScheme":
                        if (!this.name.Equals(vt.GetAttribute("name")))
                        {
                            isPure = false;
                        }
                        List<Element> connSchemes = mxVsdxUtils.getDirectChildElements(vt);

                        foreach (Element scheme in connSchemes)
                        {
                            string name = scheme.Name;

                            switch (name)
                            {
                                case "a:fillStyleLst":
                                    List<Element> fillStyleElems = mxVsdxUtils.getDirectChildElements(scheme);
                                    foreach (Element fillStyle in fillStyleElems)
                                    {
                                        connFillStyles.Add(FillStyleFactory.getFillStyle(fillStyle));
                                    }
                                    break;
                                case "a:lnStyleLst":
                                    List<Element> lineStyleElems = mxVsdxUtils.getDirectChildElements(scheme);
                                    foreach (Element lineStyle in lineStyleElems)
                                    {
                                        connLineStyles.Add(new LineStyle(lineStyle));
                                    }
                                    break;
                            }
                        }
                        break;
                    case "vt:lineStyles":
                        List<Element> styles = mxVsdxUtils.getDirectChildElements(vt);

                        foreach (Element style in styles)
                        {
                            string name = style.Name;

                            switch (name)
                            {
                                case "vt:fmtConnectorSchemeLineStyles":
                                    List<Element> connStylesElems = mxVsdxUtils.getDirectChildElements(style);
                                    foreach (Element connStyle in connStylesElems)
                                    {
                                        connLineStylesExt.Add(new LineStyleExt(connStyle));
                                    }
                                    break;
                                case "vt:fmtSchemeLineStyles":
                                    List<Element> schemeStyleElems = mxVsdxUtils.getDirectChildElements(style);
                                    foreach (Element schemeStyle in schemeStyleElems)
                                    {
                                        lineStylesExt.Add(new LineStyleExt(schemeStyle));
                                    }
                                    break;
                            }
                        }
                        break;
                    case "vt:fontStylesGroup":
                        List<Element> fontStyleElems = mxVsdxUtils.getDirectChildElements(vt);

                        foreach (Element fontStyle in fontStyleElems)
                        {
                            string name = fontStyle.Name;

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
                        List<Element> varStyleSchemes = mxVsdxUtils.getDirectChildElements(vt);

                        int i = 0;
                        foreach (Element varStyleScheme in varStyleSchemes)
                        {
                            variantEmbellishment[i] = mxVsdxUtils.getIntAttr(varStyleScheme, "embellishment");

                            List<Element> varStyles = mxVsdxUtils.getDirectChildElements(varStyleScheme);
                            int j = 0;
                            foreach (Element varStyle in varStyles)
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

        private void fillFontStyles(Element fontStyle, List<OoxmlColor> fontColors, List<int?> fontStyles)
        {
            List<Element> fontProps = mxVsdxUtils.getDirectChildElements(fontStyle);

            foreach (Element fontProp in fontProps)
            {
                fontStyles.Add(mxVsdxUtils.getIntAttr(fontProp, "style"));

                Element color = mxVsdxUtils.getDirectFirstChildElement(fontProp);
                if (color != null)
                {
                    fontColors.Add(OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color)));
                }
            }
        }

        private void processFormats(Element element)
        {
            List<Element> styles = mxVsdxUtils.getDirectChildElements(element);
            foreach (Element style in styles)
            {
                string name = style.Name;
                switch (name)
                {
                    case "a:fillStyleLst":
                        List<Element> fillStyleElems = mxVsdxUtils.getDirectChildElements(style);
                        foreach (Element fillStyle in fillStyleElems)
                        {
                            fillStyles.Add(FillStyleFactory.getFillStyle(fillStyle));
                        }
                        break;
                    case "a:lnStyleLst":
                        List<Element> lineStyleElems = mxVsdxUtils.getDirectChildElements(style);
                        foreach (Element lineStyle in lineStyleElems)
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

        private void processFonts(Element element)
        {
            // TODO Fonts has only the name of the font for each language. It looks not important
        }

        private void processColors(Element element)
        {
            Node child = element.FirstChild;

            while (child != null)
            {
                if (child is Element)
                {
                    Element elem = (Element)child;
                    string nodeName = elem.Name;
                    List<Element> children = mxVsdxUtils.getDirectChildElements(elem);
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
                        string clrName = nodeName.Substring(2);

                        if (children.Count > 0)
                        {
                            addBasicColor(clrName, children[0]);
                        }
                    }
                }
                child = child.NextSibling;
            }
        }

        private void addVariantColors(Element element)
        {
            Element parent = mxVsdxUtils.getDirectFirstChildElement(element);

            if (parent != null)
            {
                List<Element> variants = mxVsdxUtils.getDirectChildElements(parent);
                int i = 0;
                foreach (Element variant in variants)
                {
                    addVariantColorsSet(i++, variant);
                }
            }
        }

        private void addVariantColorsSet(int index, Element variant)
        {
            List<Element> colors = mxVsdxUtils.getDirectChildElements(variant);

            isMonotoneVariant[index] = variant.HasAttribute("monotone");

            foreach (Element color in colors)
            {
                string name = color.Name;
                switch (name)
                {
                    case "vt:varColor1":
                        variantsColors[index,0] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor2":
                        variantsColors[index,1] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor3":
                        variantsColors[index,2] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor4":
                        variantsColors[index,3] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor5":
                        variantsColors[index,4] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor6":
                        variantsColors[index,5] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                    case "vt:varColor7":
                        variantsColors[index,6] = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(color));
                        break;
                }
            }
        }

        private void addBkgndColor(Element element)
        {
            Element elem = mxVsdxUtils.getDirectFirstChildElement(element);

            if (elem != null)
            {
                bkgndColor = OoxmlColorFactory.getOoxmlColor(mxVsdxUtils.getDirectFirstChildElement(elem));
            }
        }

        private void extractThemeIndex(Element element)
        {
            Element elem = mxVsdxUtils.getDirectFirstChildElement(element);

            if (elem != null)
            {
                themeIndex = int.Parse(elem.GetAttribute("schemeEnum"));
            }
        }

        private void addBasicColor(string clrName, Element element)
        {
            baseColors[clrName] = OoxmlColorFactory.getOoxmlColor(element);
        }

        public virtual Color getSchemeColor(string val)
        {
            processTheme();

            OoxmlColor color = baseColors[val];

            return color != null ? color.getColor(this) : defaultClr;
        }

        //	QuickStyleFillColor
        public virtual Color getStyleColor(int styleColor)
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


        public virtual Color getFillGraientColor(QuickStyleVals quickStyleVals)
        {
            return getFillColor(quickStyleVals, true);
        }

        public virtual Color getFillColor(QuickStyleVals quickStyleVals)
        {
            return getFillColor(quickStyleVals, false);
        }

        //Get fill color based on QuickStyleFillColor & QuickStyleFillMatrix
        private Color getFillColor(QuickStyleVals quickStyleVals, bool getGradient)
        {
            processTheme();

            int fillColorStyle = quickStyleVals.QuickStyleFillColor;
            FillStyle fillStyle = null;
            switch (quickStyleVals.QuickStyleFillMatrix)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    fillStyle = fillStyles[quickStyleVals.QuickStyleFillMatrix - 1];
                    break;
                case 100:
                case 101:
                case 102:
                case 103:
                    if (isMonotoneVariant[themeVariant])
                    {
                        fillColorStyle = 100;
                    }

                    int index = quickStyleVals.QuickStyleFillMatrix - 100;
                    //get style index of variants
                    fillStyle = fillStyles[variantFillIdx[themeVariant,index] - 1];
                    break;
            }

            Color retColor;
            if (fillStyle != null)
            {
                if (getGradient)
                {
                    retColor = (fillStyle is GradFill) ? fillStyle.applyStyle(fillColorStyle, this).GradientClr : null;
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

            int styleVariation = quickStyleVals.QuickStyleVariation;

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

            int lineColorStyle = quickStyleVals.QuickStyleLineColor;
            LineStyle lineStyle = getLineStyle(quickStyleVals.QuickStyleLineMatrix, lineStyles);
            switch (quickStyleVals.QuickStyleLineMatrix)
            {
                case 100:
                case 101:
                case 102:
                case 103:
                    if (isMonotoneVariant[themeVariant])
                    {
                        lineColorStyle = 100;
                    }
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

            int styleVariation = quickStyleVals.QuickStyleVariation;

            //TODO using the fill color does not cover all the cases but works with most of the sample files
            if ((styleVariation & 4) > 0)
            {
                lineClr = getFillColor(quickStyleVals);
            }
            return lineClr;
        }

        //Get line color based on QuickStyleLineColor & QuickStyleLineMatrix
        public virtual Color getLineColor(QuickStyleVals quickStyleVals)
        {
            return getLineColor(quickStyleVals, lineStyles);
        }

        //Get connection line color based on QuickStyleLineColor & QuickStyleLineMatrix
        public virtual Color getConnLineColor(QuickStyleVals quickStyleVals)
        {
            return getLineColor(quickStyleVals, connLineStyles);
        }


        public virtual Color DefaultLineClr
        {
            get
            {
                return defaultLineClr;
            }
        }

        private bool isLineDashed(QuickStyleVals quickStyleVals, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.QuickStyleLineMatrix, lineStylesExt);

            if (lineStyleExt != null)
            {
                return lineStyleExt.Dashed;
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.QuickStyleLineMatrix, lineStyles);
                return lineStyle != null ? lineStyle.Dashed : false;
            }
        }

        public virtual bool isLineDashed(QuickStyleVals quickStyleVals)
        {
            return isLineDashed(quickStyleVals, lineStylesExt, lineStyles);
        }

        public virtual bool isConnLineDashed(QuickStyleVals quickStyleVals)
        {
            return isLineDashed(quickStyleVals, connLineStylesExt, connLineStyles);
        }

        private List<double?> getLineDashPattern(QuickStyleVals quickStyleVals, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.QuickStyleLineMatrix, lineStylesExt);

            if (lineStyleExt != null)
            {
                return lineStyleExt.LineDashPattern;
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.QuickStyleLineMatrix, lineStyles);
                return lineStyle != null ? lineStyle.LineDashPattern : null;
            }
        }

        public virtual List<double?> getLineDashPattern(QuickStyleVals quickStyleVals)
        {
            return getLineDashPattern(quickStyleVals, lineStylesExt, lineStyles);
        }

        public virtual List<double?> getConnLineDashPattern(QuickStyleVals quickStyleVals)
        {
            return getLineDashPattern(quickStyleVals, connLineStylesExt, connLineStyles);
        }

        private int getArrowSize(QuickStyleVals quickStyleVals, bool isStart, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.QuickStyleLineMatrix, lineStylesExt);

            if (lineStyleExt != null)
            {
                return isStart ? lineStyleExt.StartSize : lineStyleExt.EndSize;
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.QuickStyleLineMatrix, lineStyles);
                return lineStyle != null ? (isStart ? lineStyle.StartSize : lineStyle.EndSize) : 4;
            }
        }

        public virtual int getStartSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, true, lineStylesExt, lineStyles);
        }

        public virtual int getConnStartSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, true, connLineStylesExt, connLineStyles);
        }

        public virtual int getEndSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, false, lineStylesExt, lineStyles);
        }

        public virtual int getConnEndSize(QuickStyleVals quickStyleVals)
        {
            return getArrowSize(quickStyleVals, false, connLineStylesExt, connLineStyles);
        }

        //Get font color based on QuickStyleFontColor & QuickStyleFontMatrix
        private Color getFontColor(QuickStyleVals quickStyleVals, List<OoxmlColor> fontColors)
        {
            processTheme();

            int fontColorStyle = quickStyleVals.QuickStyleFontColor;
            OoxmlColor fontColor = null;
            switch (quickStyleVals.QuickStyleFontMatrix)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    fontColor = fontColors[quickStyleVals.QuickStyleFontMatrix - 1];
                    break;
                case 100:
                case 101:
                case 102:
                case 103:
                    if (isMonotoneVariant[themeVariant])
                    {
                        fontColorStyle = 100;
                    }

                    int index = quickStyleVals.QuickStyleFontMatrix - 100;
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

            int styleVariation = quickStyleVals.QuickStyleVariation;

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
                if (fillHSLClr.Lum < lineHSLClr.Lum)
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
        public virtual Color getFontColor(QuickStyleVals quickStyleVals)
        {
            return getFontColor(quickStyleVals, fontColors);
        }

        //Get connection font color based on QuickStyleFontColor & QuickStyleFontMatrix
        public virtual Color getConnFontColor(QuickStyleVals quickStyleVals)
        {
            return getFontColor(quickStyleVals, connFontColors);
        }

        private int getArrowType(QuickStyleVals quickStyleVals, bool isStart, List<LineStyleExt> lineStylesExt, List<LineStyle> lineStyles)
        {
            LineStyleExt lineStyleExt = getLineStyleExt(quickStyleVals.QuickStyleLineMatrix, lineStylesExt);

            if (lineStyleExt != null)
            {
                return isStart ? lineStyleExt.Start : lineStyleExt.End;
            }
            else
            {
                LineStyle lineStyle = getLineStyle(quickStyleVals.QuickStyleLineMatrix, lineStyles);
                return lineStyle != null ? (isStart ? lineStyle.Start : lineStyle.End) : 0;
            }
        }

        public virtual int getEdgeMarker(bool isStart, QuickStyleVals quickStyleVals)
        {
            return getArrowType(quickStyleVals, isStart, lineStylesExt, lineStyles);
        }

        public virtual int getConnEdgeMarker(bool isStart, QuickStyleVals quickStyleVals)
        {
            return getArrowType(quickStyleVals, isStart, connLineStylesExt, connLineStyles);
        }


        private int getLineWidth(QuickStyleVals quickStyleVals, List<LineStyle> lineStyles)
        {
            LineStyle lineStyle = getLineStyle(quickStyleVals.QuickStyleLineMatrix, lineStyles);
            return lineStyle != null ? lineStyle.LineWidth : 0;
        }

        public virtual int getLineWidth(QuickStyleVals quickStyleVals)
        {
            return getLineWidth(quickStyleVals, lineStyles);
        }

        public virtual int getConnLineWidth(QuickStyleVals quickStyleVals)
        {
            return getLineWidth(quickStyleVals, connLineStyles);
        }
    }

}