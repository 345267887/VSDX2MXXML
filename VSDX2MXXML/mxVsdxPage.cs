using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxVsdxPage
    {
        /**
	 * Unique ID of the element within its parent element
	 */
        protected int? Id = null;

        /**
         * Name of the page taken from the "name" attribute of the page element
         */
        protected String pageName = null;

        protected bool _isBackground = false;

        protected int? backPageId = null;

        protected mxVsdxPage backPage = null;

        protected XmlElement pageElement = null;

        protected XmlElement pageSheet = null;

        protected mxVsdxModel model = null;

        protected Dictionary<int, VsdxShape> shapes = new Dictionary<Integer, VsdxShape>();

        protected Dictionary<int, mxVsdxConnect> connects = new Dictionary<int, mxVsdxConnect>();

        // cell in the PageSheet
        protected Dictionary<String, XmlElement> cellElements = new Dictionary<String, XmlElement>();

        public mxVsdxPage(XmlElement pageElem, mxVsdxModel model)
        {
            this.model = model;
            this.pageElement = pageElem;

            String backGround = pageElem.GetAttribute(mxVsdxConstants.BACKGROUND);
            this._isBackground = (backGround != null && backGround.Equals(mxVsdxConstants.TRUE)) ? true : false;
            String back = pageElem.GetAttribute(mxVsdxConstants.BACK_PAGE);

            if (!_isBackground && back != null && back.Length > 0)
            {
                this.backPageId = int.Parse(back);
            }

            this.Id = int.Parse(pageElem.GetAttribute(mxVsdxConstants.ID));
            this.pageName = pageElem.GetAttribute(mxVsdxConstants.NAME);

            List<XmlElement> pageSheets = mxVsdxUtils.getDirectChildNamedElements(pageElem, "PageSheet");

            if (pageSheets.Count > 0)
            {
                XmlElement pageSheet = pageSheets[0];
                List<XmlElement> cells = mxVsdxUtils.getDirectChildNamedElements(pageSheet, "Cell");

                foreach (XmlElement cellElem in cells)
                {
                    String n = cellElem.GetAttribute("N");
                    this.cellElements.Add(n, cellElem);
                }
            }

            parseNodes(pageElem, model, "pages");
        }

        /**
         * Parses the child nodes of the given element
         * @param pageElem the parent whose children to parse
         * @param model the model of the vsdx file
         * @param pageName page information is split across pages.xml and pageX.xml where X is any number. We have to know which we're currently parsing to use the correct relationships file.
         */
        protected void parseNodes(XmlNode pageElem, mxVsdxModel model, String pageName)
        {
            XmlNode pageChild = pageElem.FirstChild;

            while (pageChild != null)
            {
                if (pageChild is XmlElement)
			{
                    XmlElement pageChildElem = (XmlElement)pageChild;
                    String childName = pageChildElem.Name;

                    if (childName.Equals("Rel"))
                    {
                        resolveRel(pageChildElem, model, pageName);
                    }
                    else if (childName.Equals("Shapes"))
                    {
                        this.shapes = parseShapes(pageChildElem, null, false);
                    }
                    else if (childName.Equals("Connects"))
                    {
                        XmlNodeList connectList = pageChildElem.GetElementsByTagName(mxVsdxConstants.CONNECT);
                        XmlNode connectNode = (connectList != null && connectList.Count > 0) ? connectList[0] : null;
                        //mxVdxConnect currentConnect = null;

                        while (connectNode != null)
                        {
                            if (connectNode is XmlElement)
						{
                                XmlElement connectElem = (XmlElement)connectNode;
                                mxVsdxConnect connect = new mxVsdxConnect(connectElem);
                                int? fromSheet = connect.getFromSheet();
                                mxVsdxConnect previousConnect = (fromSheet.HasValue && fromSheet.Value > -1) ? connects.get(fromSheet) : null;

                                if (previousConnect != null)
                                {
                                    previousConnect.addConnect(connectElem);
                                }
                                else
                                {
                                    connects.put(connect.getFromSheet(), connect);
                                }
                            }

                            connectNode = connectNode.NextSibling;
                        }
                    }
                    else if (childName.Equals("PageSheet"))
                    {
                        this.pageSheet = pageChildElem;
                    }
                }

                pageChild = pageChild.NextSibling;
            }
        }

        /**
         * 
         * @param relNode
         * @param model
         * @param pageName
         */
        protected void resolveRel(XmlElement relNode, mxVsdxModel model, String pageName)
        {
            XmlElement relElem = model.getRelationship(relNode.GetAttribute("r:id"), mxVsdxCodec.vsdxPlaceholder + "/pages/" + "_rels/" + pageName + ".xml.rels");

            String target = relElem.GetAttribute("Target");
            String type = relElem.GetAttribute("Type");

            if (type.EndsWith("page"))
            {
                XmlDocument pageDoc = null;

                if (type != null && type.EndsWith("page"))
                {
                    pageDoc = model.getXmlDoc(mxVsdxCodec.vsdxPlaceholder + "/pages/" + target);
                }

                if (pageDoc != null)
                {
                    XmlNode child = pageDoc.FirstChild;

                    while (child != null)
                    {
                        if (child is XmlElement && ((XmlElement)child).Name.Equals("PageContents"))
					{
                            int index = target.IndexOf('.');

                            if (index != -1)
                            {
                                parseNodes(child, model, target.Substring(0, index));
                            }

                            break;
                        }

                        child = child.NextSibling;
                    }
                }
            }
        }

        public Dictionary<int, VsdxShape> parseShapes(XmlElement shapesElement, mxVsdxMaster master, bool recurse)
        {
            Dictionary<int, VsdxShape> shapes = new Dictionary<int, VsdxShape>();
            XmlNodeList shapeList = shapesElement.GetElementsByTagName(mxVsdxConstants.SHAPE);

            XmlNode shapeNode = (shapeList != null && shapeList.Count > 0) ? shapeList[0] : null;

            while (shapeNode != null)
            {
                if (shapeNode is XmlElement)
			{
                    XmlElement shapeElem = (XmlElement)shapeNode;
                    mxVsdxMaster masterTmp = master;

                    // Work out node type
                    if (masterTmp == null)
                    {
                        //If the shape has the Master attribute the master shape is the first
                        //shape of the master element.
                        String masterId = shapeElem.GetAttribute(mxVsdxConstants.MASTER);

                        if (masterId != null && !masterId.Equals(""))
                        {
                            masterTmp = model.getMaster(masterId);
                        }
                    }

                    bool _isEdge = isEdge(shapeElem);

                    // If the master of the shape has an xform1D, it's an edge
                    if (!_isEdge && masterTmp != null)
                    {
                        String masterId = shapeElem.GetAttribute(mxVsdxConstants.MASTER_SHAPE);

                        XmlElement elem = masterTmp.getMasterElement();
                        if (masterId != null && !masterId.Equals(""))
                        {
                            elem = masterTmp.getSubShape(masterId).getShape();
                        }
                        _isEdge = isEdge(elem);
                    }

                    //String type = mxVdxShape.getType(shapeElem);

                    VsdxShape shape = this.createCell(shapeElem, !_isEdge, masterTmp);

                    shapes.Add(shape.getId(), shape);
                }

                shapeNode = shapeNode.NextSibling;
            }

            return shapes;
        }

        protected VsdxShape createCell(XmlElement shapeElem, bool vertex, mxVsdxMaster masterTmp)
        {
            return new VsdxShape(this, shapeElem, vertex, this.model.getMasterShapes(), masterTmp, this.model);
        }

        public bool isEdge(XmlElement shape)
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
                            XmlElement childElem = (XmlElement)childNode;

                            if (childElem.Name.Equals("Cell"))
                            {
                                String n = childElem.GetAttribute("N");

                                if (n.Equals("BeginX") || n.Equals("BeginY") || n.Equals("EndY") || n.Equals("EndX"))
                                {
                                    return true;
                                }
                            }
                        }

                        childNode = childNode.NextSibling;
                    }
                }
            }

            return false;
        }

        /**
         * Returns the width and height of a Page expressed as an mxPoint.
         * @return mxPoint that represents the dimensions of the page
         */
        public mxPoint getPageDimensions()
        {
            double pageH = 0;
            double pageW = 0;

            XmlElement height = this.cellElements["PageHeight"];
            XmlElement width = this.cellElements["PageWidth"];

            if (height != null)
            {
                pageH = Double.Parse(height.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
                pageH = Math.Round(pageH * 100.0) / 100.0;
            }

            if (width != null)
            {
                pageW = Double.Parse(width.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
                pageW = Math.Round(pageW * 100.0) / 100.0;
            }

            return new mxPoint(pageW, pageH);
        }

        /**
         * Returns the drawing scale attribute of this page
         * @return the DrawingScale
         */
        public double getDrawingScale()
        {
            XmlElement scale = this.cellElements["DrawingScale"];

            if (scale != null)
            {
                return Double.Parse(scale.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
            }

            return 1;
        }


        /**
         * Returns the page scale attribute of this page
         * @return the PageScale
         */
        public double getPageScale()
        {
            XmlElement scale = this.cellElements["PageScale"];

            if (scale != null)
            {
                return Double.Parse(scale.GetAttribute("V")) * mxVsdxUtils.conversionFactor;
            }

            return 1;
        }

        public String getCellValue(String cellName)
        {
            XmlElement cell = this.cellElements[cellName];

            if (cell != null)
            {
                return cell.GetAttribute("V");
            }

            return null;
        }

        public int getCellIntValue(String cellName, int defVal)
        {
            String val = getCellValue(cellName);

            if (val != null)
            {
                return int.Parse(val);
            }

            return defVal;
        }

        /**
         * Returns the ID of the page
         * @return the ID of the page
         */
        public int? getId()
        {
            return this.Id;
        }
        public String getPageName()
        {
            return this.pageName;
        }

        public Dictionary<int, VsdxShape> getShapes()
        {
            return this.shapes;
        }

        public Dictionary<int, mxVsdxConnect> getConnects()
        {
            return this.connects;
        }

        public bool isBackground()
        {
            return this._isBackground;
        }

        /**
         * Returns the background page ID, if any
         * @return the ID of any background page or null for no background page
         */
        public int? getBackPageId()
        {
            return this.backPageId;
        }

        public void setBackPage(mxVsdxPage page)
        {
            this.backPage = page;
        }

        public mxVsdxPage getBackPage()
        {
            return this.backPage;
        }
    }
}
