using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace VSDX2MXXML
{
    class mxVsdxCodec
    {
        protected String RESPONSE_END = "</mxfile>";

        protected String RESPONSE_DIAGRAM_START = "";
        protected String RESPONSE_DIAGRAM_END = "</diagram>";

        protected String RESPONSE_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><mxfile>";

        /**
         * Stores the vertexes imported.
         */
        protected Dictionary<ShapePageId, mxCell> vertexMap = new Dictionary<ShapePageId, mxCell>();

        /**
         * Stores the shapes that represent Edges.
         */
        protected Dictionary<ShapePageId, VsdxShape> edgeShapeMap = new Dictionary<ShapePageId, VsdxShape>();

        /**
         * Stores the shapes that represent Vertexes.
         */
        protected Dictionary<ShapePageId, VsdxShape> vertexShapeMap = new Dictionary<ShapePageId, VsdxShape>();

        /**
         * Stores the parents of the shapes imported.
         */
        protected Dictionary<ShapePageId, Object> parentsMap = new Dictionary<ShapePageId, Object>();

        /**
         * Set to true if you want to display spline debug data
         */
        protected bool debugPaths = false;

        /**
         * Do not remove, ask David
         */
        //public static String vsdxPlaceholder = new String(Base64.decodeBase64("dmlzaW8="));
        public static String vsdxPlaceholder = new String(Common.DecodeBase64(Encoding.UTF8, "dmlzaW8=").ToCharArray());



        protected mxVsdxModel vsdxModel;


        /**
         * Calculate the absolute coordinates of a cell's point.
         * @param cellParent Cell that contains the point.
         * @param graph Graph where the parsed graph is included.
         * @param point Point to which coordinates are calculated.
         * @return The point in absolute coordinates.
         */
        private static mxPoint calculateAbsolutePoint(Object cellParent,
                mxGraph graph, mxPoint point)
        {
            if (cellParent != null)
            {
                mxGeometry geo = graph.getModel().getGeometry(cellParent);

                if (geo != null)
                {
                    point.setX(point.getX() + geo.getX());
                    point.setY(point.getY() + geo.getY());
                }
            }


            return point;
        }

        /**
         * Parses the input VSDX format and uses the information to populate 
         * the specified graph.
         * @param docs All XML documents contained in the VSDX source file
         * @throws IOException 
         * @throws ParserConfigurationException 
         * @throws SAXException 
         * @throws TransformerException 
         */
        public String decodeVsdx(byte[] data, String charset)
        {

            ZipInputStream zis = new ZipInputStream(new MemoryStream(data));
            ZipEntry ze = null;
            Dictionary<String, XmlDocument> docData = new Dictionary<String, XmlDocument>();
            Dictionary<String, String> mediaData = new Dictionary<String, String>();

            while ((ze = zis.GetNextEntry()) != null)
            {
                String filename = ze.Name;

                if (!ze.IsDirectory)
                {
                    byte[] xmlData = new byte[zis.Length];
                    zis.Read(xmlData, 0, xmlData.Length);

                    if (filename.ToLower().EndsWith(".xml") | filename.ToLower().EndsWith(".xml.rels"))
                    {
                        String str = System.Text.Encoding.Default.GetString(xmlData);
                        if (!string.IsNullOrEmpty(str)) ;
                        {
                            XmlDocument doc = mxXmlUtils.parseXml(str);
                            // Hack to be able to find the filename from an element in the XML
                            //doc.setDocumentURI(filename);
                            docData.Add(filename, doc);
                        }
                    }

                }
            }

            zis.Close();

            String path = mxVsdxCodec.vsdxPlaceholder + "/document.xml";
            XmlDocument rootDoc = docData[path];
            XmlNode rootChild = rootDoc.FirstChild;

            while (rootChild != null && !(rootChild is XmlElement))
            {
                rootChild = rootChild.NextSibling;
            }

            if (rootChild != null && rootChild is XmlElement)
            {
                importNodes(rootDoc, (XmlElement)rootChild, path, docData);
            }
            else
            {
                // TODO log error
                return null;
            }

            vsdxModel = new mxVsdxModel(rootDoc, docData, mediaData);

            //Imports each page of the document.
            Dictionary<int, mxVsdxPage> pages = vsdxModel.getPages();

            StringBuilder xmlBuilder = new StringBuilder(RESPONSE_HEADER);

            foreach (var entry in pages)
            {
                mxVsdxPage page = entry.Value;

                if (!page.isBackground())
                {
                    mxGraph graph = createMxGraph();

                    graph.getModel().beginUpdate();
                    importPage(page, graph, graph.getDefaultParent());

                    mxVsdxPage backPage = page.getBackPage();

                    if (backPage != null)
                    {
                        graph.getModel().setValue(graph.getDefaultParent(), page.getPageName());
                        Object backCell = new mxCell(backPage.getPageName());
                        graph.addCell(backCell, graph.getModel().getRoot(), 0, null, null);
                        importPage(backPage, graph, graph.getDefaultParent());
                    }

                    //scale page 
                    double scale = page.getPageScale() / page.getDrawingScale();

                    if (scale != 1)
                    {
                        mxGraphModel model = (mxGraphModel)graph.getModel();

                        for (Object c : model.getCells().values())
                        {
                            mxGeometry geo = model.getGeometry(c);

                            if (geo != null)
                            {
                                scaleRect(geo, scale);
                                scaleRect(geo.getAlternateBounds(), scale);

                                if (model.isEdge(c))
                                {
                                    //scale edge waypoints, offset, ...
                                    scalePoint(geo.getSourcePoint(), scale);
                                    scalePoint(geo.getTargetPoint(), scale);
                                    scalePoint(geo.getOffset(), scale);
                                    List<mxPoint> points = geo.getPoints();

                                    if (points != null)
                                    {
                                        for (mxPoint p : points)
                                        {
                                            scalePoint(p, scale);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    graph.getModel().endUpdate();

                    xmlBuilder.Append(RESPONSE_DIAGRAM_START);
                    xmlBuilder.Append(processPage(graph, page));
                    xmlBuilder.Append(RESPONSE_DIAGRAM_END);
                }
            }

            xmlBuilder.Append(RESPONSE_END);

            return xmlBuilder.ToString();
        }

        protected mxGraph createMxGraph()
        {
            mxGraph graph = new mxGraphHeadless();
            //Disable parent (groups) auto extend feature as it miss with the coordinates of vsdx format
            graph.setExtendParents(false);
            graph.setExtendParentsOnAdd(false);

            graph.setConstrainChildren(false);
            graph.setHtmlLabels(true);
            //Prevent change of edge parent as it misses with the routing points
            ((mxGraphModel)graph.getModel()).setMaintainEdgeParent(false);
            return graph;
        }

        protected String processPage(mxGraph graph, mxVsdxPage page)
        {
            mxCodec codec = new mxCodec();
            Node node = codec.encode(graph.getModel());
            ((Element)node).setAttribute("style", "default-style2");
            String modelString = mxXmlUtils.getXml(node);
            String modelAscii = Utils.encodeURIComponent(modelString, Utils.CHARSET_FOR_URL_ENCODING);
            byte[] modelBytes = Utils.deflate(modelAscii);

            StringBuilder output = new StringBuilder();

            if (page != null)
            {
                String pageName = StringEscapeUtils.escapeXml11(page.getPageName());
                output.append("<diagram name=\"");
                output.append(pageName);
                output.append("\">");
            }
            output.append(mxBase64.encodeToString(modelBytes, false));

            return output.toString();
        }

        private boolean isJpg(byte[] emfData, int i)
        {
            //the loop calling this function make sure that we still have 3 bytes in the buffer
            return emfData[i] == (byte)0xFF && emfData[i + 1] == (byte)0xD8 &&
                   emfData[i + 2] == (byte)0xFF;
        }

        private boolean isPng(byte[] emfData, int i)
        {
            //the loop calling this function make sure that we still have 8 bytes in the buffer
            return emfData[i] == (byte)0x89 && emfData[i + 1] == (byte)0x50 &&
                   emfData[i + 2] == (byte)0x4E && emfData[i + 3] == (byte)0x47 &&
                   emfData[i + 4] == (byte)0x0D && emfData[i + 5] == (byte)0x0A &&
                   emfData[i + 6] == (byte)0x1A && emfData[i + 7] == (byte)0x0A;
        }

        /**
         * Scale a point in place
         * 
         * @param p point to scale in place 
         * @param scale scale
         * @return scaled point
         */
        private mxPoint scalePoint(mxPoint p, double scale)
        {
            if (p != null)
            {
                p.setX(p.getX() * scale);
                p.setY(p.getY() * scale);
            }

            return p;
        }

        /**
         * Scale a rectangle in place
         * 
         * @param rect rectangle to scale in place
         * @param scale scale
         * @return scaled rectangle
         */
        private mxRectangle scaleRect(mxRectangle rect, double scale)
        {
            if (rect != null)
            {
                rect.setX(rect.getX() * scale);
                rect.setY(rect.getY() * scale);
                rect.setHeight(rect.getHeight() * scale);
                rect.setWidth(rect.getWidth() * scale);
            }

            return rect;
        }

        /**
         * 
         * @param rootDoc
         * @param currentNode
         * @param path
         * @param docData
         */
        private void importNodes(Document rootDoc, Element currentNode,
                String path, Map<String, Document> docData)
        {
            int lastSlash = path.lastIndexOf("/");

            String dir = path;
            String fileName = path;

            if (lastSlash != -1)
            {
                dir = path.substring(0, lastSlash);
                fileName = path.substring(lastSlash + 1, path.length());
            }
            else
            {
                // Can't handle this case
                return;
            }

            String relsPath = dir + "/_rels/" + fileName + ".rels";
            Document relsDoc = docData.get(relsPath);

            if (relsDoc == null)
            {
                // Valid to not have a rels for an XML file
                return;
            }

            NodeList rels = relsDoc.getElementsByTagName("Relationship");
            Map<String, String> relMap = new HashMap<String, String>();

            for (int i = 0; i < rels.getLength(); i++)
            {
                Element currElem = (Element)rels.item(i);
                String id = currElem.getAttribute("Id");
                String target = currElem.getAttribute("Target");
                relMap.put(id, target);
            }

            NodeList relList = currentNode.getElementsByTagName("Rel");

            for (int i = 0; i < relList.getLength(); i++)
            {
                Element rel = (Element)relList.item(i);
                String pathSuffix = relMap.get(rel.getAttribute("r:id"));
                String target = dir + "/" + pathSuffix;

                if (target != null)
                {
                    Document childDoc = docData.get(target);

                    if (childDoc != null)
                    {
                        Node parent = rel.getParentNode();
                        Node rootChild = childDoc.getFirstChild();

                        while (rootChild != null && !(rootChild instanceof Element))
					{
                rootChild.getNextSibling();
            }

            if (rootChild != null && rootChild instanceof Element)
					{
                Node importNode = rootChild.getFirstChild();

                while (importNode != null)
                {
                    if (importNode instanceof Element)
							{
                        Node newNode = parent.appendChild(rootDoc
                                .importNode(importNode, true));
                        String pathTmp = target;
                        importNodes(rootDoc, (Element)newNode,
                                pathTmp, docData);
                    }

                    importNode = importNode.getNextSibling();
                }
            }
        }
    }
}
	}

	/**
	 * Imports a page of the document with the actual pageHeight.<br/>
	 * In .vdx, the Y-coordinate grows upward from the bottom of the page.<br/>
	 * The page height is used for calculating the correct position in mxGraph using
	 * this formula: mxGraph_Y_Coord = PageHeight - VSDX_Y_Coord.
	 * @param page Actual page Element to be imported
	 * @param graph Graph where the parsed graph is included.
	 * @param parent The parent of the elements to be imported.
	 */
	protected double importPage(mxVsdxPage page, mxGraph graph, Object parent)
{
    Map<Integer, VsdxShape> shapes = page.getShapes();
    Iterator<Map.Entry<Integer, VsdxShape>> entries = shapes.entrySet()
            .iterator();

    double pageHeight = page.getPageDimensions().getY();
    Integer pageId = page.getId();

    while (entries.hasNext())
    {
        Map.Entry<Integer, VsdxShape> entry = entries.next();

        if (this.debugPaths)
        {
            mxPathDebug debug = new mxPathDebug(true, graph,
                    entry.getValue(), pageHeight);
            entry.getValue().debug = debug;
        }

        addShape(graph, entry.getValue(), parent,
                pageId, pageHeight);
    }

    Map<Integer, mxVsdxConnect> connects = page.getConnects();
    Iterator<Map.Entry<Integer, mxVsdxConnect>> entries2 = connects
            .entrySet().iterator();

    while (entries2.hasNext())
    {
        Map.Entry<Integer, mxVsdxConnect> entry = entries2.next();
        ShapePageId edgeId = addConnectedEdge(graph, entry.getValue(), pageId, pageHeight);

        if (edgeId != null)
        {
            edgeShapeMap.remove(edgeId); // ensure not processed twice
        }
    }

    //Process unconnected edges.
    Iterator<Entry<ShapePageId, VsdxShape>> it = edgeShapeMap.entrySet().iterator();

    while (it.hasNext())
    {
        Entry<ShapePageId, VsdxShape> edgeShapeEntry = it.next();

        //Only this page unconnected edges
        if (edgeShapeEntry.getKey().getPageNumber() == pageId)
        {
            addUnconnectedEdge(graph, parentsMap.get(edgeShapeEntry.getKey()), edgeShapeEntry.getValue(), pageHeight);
        }
    }

    sanitiseGraph(graph);

    return pageHeight;
}

/**
 * Adds a vertex to the graph if 'shape' is a vertex or add the shape to edgeShapeMap if it is an edge.
 * This method doesn't import sub-shapes of 'shape'.
 * @param graph Graph where the parsed graph is included.
 * @param shp Shape to be imported.
 * @param parentHeight Height of the parent cell.
 * @return the new vertex added. null if 'shape' is not a vertex.
 */
protected mxCell addShape(mxGraph graph, VsdxShape shape, Object parent, Integer pageId, double parentHeight)
{
    shape.parentHeight = parentHeight;

    String type = VsdxShape.getType(shape.getShape());

    //If is a Shape or a Group add the vertex to the graph.
    if (type != null
            && (type.equals(mxVsdxConstants.TYPE_SHAPE)
                    || type.equals(mxVsdxConstants.TYPE_GROUP) || type
                        .equals(mxVsdxConstants.FOREIGN)))
    {
        int id = shape.getId();

        if (shape.isVertex())
        {
            mxCell v1 = null;

            if (shape.isGroup())
            {
                v1 = addGroup(graph, shape, parent, pageId, parentHeight);
            }
            else
            {
                v1 = addVertex(graph, shape, parent, pageId, parentHeight);
            }

            vertexShapeMap.put(new ShapePageId(pageId, id), shape);
            return v1;
        }
        else
        {
            //remember the edge order to maintain the shapes order (back to front)
            shape.setShapeIndex(graph.getModel().getChildCount(parent));
            edgeShapeMap.put(new ShapePageId(pageId, id), shape);
            parentsMap.put(new ShapePageId(pageId, id), parent);
        }
    }

    return null;
}

/**
 * Adds a group to the graph.
 * The sub-shapes of a complex shape are processed like part of the shape.
 * @param graph Graph where the parsed graph is included.
 * @param parent Parent cell of the shape.
 * @param parentHeight Height of the parent cell of the shape.
 * @return Cell added to the graph.
 */
public mxCell addGroup(mxGraph graph, VsdxShape shape, Object parent, Integer pageId, double parentHeight)
{
    //Set title
    //		String t = "";
    //		Element shapeElem = shape.getShape();
    //		Element text = (Element) shapeElem.getElementsByTagName("Text").item(0);
    //
    //		if (text != null)
    //		{
    //			t = (text.getTextContent());
    //		}

    //Define dimensions
    mxPoint d = shape.getDimensions();
    mxVsdxMaster master = shape.getMaster();
    //Define style
    Map<String, String> styleMap = shape.getStyleFromShape();

    //Shape inherit its master geometry, so we don't need to check its master
    mxVsdxGeometryList geomList = shape.getGeomList();

    if (geomList.isNoFill())
    {
        styleMap.put(mxConstants.STYLE_FILLCOLOR, "none");
        styleMap.put(mxConstants.STYLE_GRADIENTCOLOR, "none");
    }

    if (geomList.isNoLine())
    {
        styleMap.put(mxConstants.STYLE_STROKECOLOR, "none");
    }

    styleMap.put("html", "1");
    styleMap.put(mxConstants.STYLE_WHITE_SPACE, "wrap");
    //TODO need to check if "shape=" should be added before the shape name (for "image", it should be skipped for example)
    String style = mxVsdxUtils.getStyleString(styleMap, "=");

    mxCell group = null;
    Map<Integer, VsdxShape> children = shape.getChildShapes();
    boolean hasChildren = children != null && children.size() > 0;
    boolean subLabel = shape.isDisplacedLabel() || shape.isRotatedLabel() || hasChildren;
    mxPoint o = shape.getOriginPoint(parentHeight, true);

    if (subLabel)
    {
        group = (mxCell)graph.insertVertex(parent, null, null,
                o.getX(), o.getY(), d.getX(), d.getY(), style);
    }
    else
    {
        String textLabel = shape.getTextLabel();
        group = (mxCell)graph.insertVertex(parent, null, textLabel,
                o.getX(), o.getY(), d.getX(), d.getY(), style);
    }

    Iterator<Map.Entry<Integer, VsdxShape>> entries = children.entrySet()
            .iterator();

    while (entries.hasNext())
    {
        Map.Entry<Integer, VsdxShape> entry = entries.next();
        VsdxShape subShape = entry.getValue();
        Integer Id = subShape.getId();

        if (subShape.isVertex())
        {
            if (this.debugPaths)
            {
                mxPathDebug debug = new mxPathDebug(true, graph, subShape,
                        parentHeight);
                subShape.debug = debug;
            }

            String type = VsdxShape.getType(subShape.getShape());

            //If is a Shape or a Group add the vertex to the graph.
            if (type != null
                    && (type.equals(mxVsdxConstants.TYPE_SHAPE)
                            || type.equals(mxVsdxConstants.TYPE_GROUP) || type
                                .equals(mxVsdxConstants.FOREIGN)))
            {
                if (subShape.isVertex())
                {
                    subShape.propagateRotation(shape.getRotation());

                    if (subShape.isGroup())
                    {
                        addGroup(graph, subShape, group, pageId, d.getY());
                    }
                    else
                    {
                        addVertex(graph, subShape, group, pageId, d.getY());
                    }
                }
            }

            if (master == null)
            {
                // If the group doesn't have a master, sub vertices are instances of document masters
                vertexShapeMap.put(new ShapePageId(pageId, Id),
                        subShape);
            }
        }
        else
        {
            if (master == null)
            {
                // If the group doesn't have a master, sub edges are instances of document masters
                edgeShapeMap.put(new ShapePageId(pageId, Id),
                        subShape);
                parentsMap.put(new ShapePageId(pageId, Id), group);
            }
            else
            {
                addUnconnectedEdge(graph, group, subShape, parentHeight);
            }
        }
    }

    if (subLabel)
    {
        shape.createLabelSubShape(graph, group);
    }

    //rotate sub vertices coordinates based on parent rotation. It should be done here after the group size if determined
    double rotation = shape.getRotation();
    if (rotation != 0)
    {
        mxGeometry pgeo = group.getGeometry();
        double hw = pgeo.getWidth() / 2, hh = pgeo.getHeight() / 2;
        for (int i = 0; i < group.getChildCount(); i++)
        {
            mxICell child = group.getChildAt(i);
            rotatedPoint(child.getGeometry(), rotation, hw, hh);
        }
    }
    return group;
}

public static void rotatedPoint(mxGeometry geo, double rotation,
        double cx, double cy)
{
    rotation = Math.toRadians(rotation);
    double cos = Math.cos(rotation), sin = Math.sin(rotation);

    double x = geo.getCenterX() - cx;
    double y = geo.getCenterY() - cy;

    double x1 = x * cos - y * sin;
    double y1 = y * cos + x * sin;

    geo.setX(Math.round(x1 + cx - geo.getWidth() / 2));
    geo.setY(Math.round(y1 + cy - geo.getHeight() / 2));
}

public static void rotatedEdgePoint(mxPoint pt, double rotation,
        double cx, double cy)
{
    rotation = Math.toRadians(rotation);
    double cos = Math.cos(rotation), sin = Math.sin(rotation);

    double x = pt.getX() - cx;
    double y = pt.getY() - cy;

    double x1 = x * cos - y * sin;
    double y1 = y * cos + x * sin;

    pt.setX(Math.round(x1 + cx));
    pt.setY(Math.round(y1 + cy));
}

/**
 * Adds a simple shape to the graph
 * @param graph Graph where the parsed graph is included.
 * @param parent Parent cell of the shape.
 * @param parentHeight Height of the parent cell of the shape.
 * @return Cell added to the graph.
 */
public mxCell addVertex(mxGraph graph, VsdxShape shape, Object parent, Integer pageId, double parentHeight)
{
    //Defines Text Label.
    String textLabel = "";

    boolean hasSubLabel = shape.isDisplacedLabel() || shape.isRotatedLabel();// || shape.getRotation() != 0;

    if (!hasSubLabel)
    {
        textLabel = shape.getTextLabel();
    }

    mxPoint dimensions = shape.getDimensions();

    Map<String, String> styleMap = shape.getStyleFromShape();

    //if (textLabel != null && (textLabel.startsWith("<p>") || textLabel.startsWith("<p ")
    //		|| textLabel.startsWith("<font")))
    //{
    styleMap.put("html", "1");
    //}

    boolean geomExists = styleMap.containsKey(mxConstants.STYLE_SHAPE)
            || styleMap.containsKey("stencil");

    if (!styleMap.containsKey(mxConstants.STYLE_FILLCOLOR) || !geomExists)
    {
        styleMap.put(mxConstants.STYLE_FILLCOLOR, "none");
    }

    if (!geomExists)
    {
        styleMap.put(mxConstants.STYLE_STROKECOLOR, "none");
    }

    if (!styleMap.containsKey(mxConstants.STYLE_GRADIENTCOLOR)
            || !geomExists)
    {
        styleMap.put(mxConstants.STYLE_GRADIENTCOLOR, "none");
    }

    styleMap.put(mxConstants.STYLE_WHITE_SPACE, "wrap");

    mxPoint coordinates = shape.getOriginPoint(parentHeight, true);

    if (geomExists || textLabel != null)
    {
        String style = mxVsdxUtils.getStyleString(styleMap, "=");

        mxCell v1 = null;

        if (hasSubLabel)
        {
            v1 = (mxCell)graph.insertVertex(parent, null, null,
                    coordinates.getX(), coordinates.getY(), dimensions.getX(),
                    dimensions.getY(), style);
        }
        else
        {
            v1 = (mxCell)graph.insertVertex(parent, null, textLabel,
                    coordinates.getX(), coordinates.getY(), dimensions.getX(),
                    dimensions.getY(), style);
        }

        vertexMap.put(new ShapePageId(pageId, shape.getId()), v1);
        shape.setLabelOffset(v1, style);

        if (hasSubLabel)
        {
            shape.createLabelSubShape(graph, v1);
        }

        return v1;
    }

    return null;
}

/**
 * Adds a connected edge to the graph.
 * These edged are the referenced in one Connect element at least.
 * @param graph graph Graph where the parsed graph is included.
 * @param connect Connect Element that references an edge shape and the source vertex.
 */
protected ShapePageId addConnectedEdge(mxGraph graph, mxVsdxConnect connect, Integer pageId, double pageHeight)
{
    Integer fromSheet = connect.getFromSheet();
    ShapePageId edgeId = new ShapePageId(pageId, fromSheet);
    VsdxShape edgeShape = edgeShapeMap.get(edgeId);

    if (edgeShape == null)
    {
        return null;
    }

    Object parent = parentsMap.get(new ShapePageId(pageId,
            edgeShape.getId()));
    double parentHeight = pageHeight;

    if (parent != null)
    {
        mxGeometry parentGeo = graph.getModel().getGeometry(parent);

        if (parentGeo != null)
        {
            parentHeight = parentGeo.getHeight();
        }
    }

    //Get beginXY and endXY coordinates.
    mxPoint beginXY = edgeShape.getStartXY(parentHeight);
    mxPoint endXY = edgeShape.getEndXY(parentHeight);
    List<mxPoint> points = edgeShape.getRoutingPoints(parentHeight, beginXY, edgeShape.getRotation());

    rotateChildEdge(graph.getModel(), parent, beginXY, endXY, points);

    Integer sourceSheet = connect.getSourceToSheet();

    mxCell source = sourceSheet != null ? vertexMap
            .get(new ShapePageId(pageId, sourceSheet)) : null;

    if (source == null)
    {
        // Source is dangling
        source = (mxCell)graph.insertVertex(parent, null, null,
                beginXY.getX(), beginXY.getY(), 0, 0);
    }
    //Else: Routing points will contain the exit/entry points, so no need to set the to/from constraint 

    Integer toSheet = connect.getTargetToSheet();

    mxCell target = toSheet != null ? vertexMap.get(new ShapePageId(
            pageId, toSheet)) : null;

    if (target == null)
    {
        // Target is dangling
        target = (mxCell)graph.insertVertex(parent, null, null,
                endXY.getX(), endXY.getY(), 0, 0);
    }
    //Else: Routing points will contain the exit/entry points, so no need to set the to/from constraint 

    //Defines the style of the edge.
    Map<String, String> styleMap = edgeShape
            .getStyleFromEdgeShape(parentHeight);
    //Insert new edge and set constraints.
    Object edge;
    double rotation = edgeShape.getRotation();
    if (rotation != 0)
    {
        edge = graph.insertEdge(parent, null, null, source,
                target, mxVsdxUtils.getStyleString(styleMap, "="));

        mxCell label = edgeShape.createLabelSubShape(graph, (mxCell)edge);
        if (label != null)
        {
            label.setStyle(label.getStyle() + ";rotation=" + (rotation > 60 && rotation < 240 ? (rotation + 180) % 360 : rotation));

            mxGeometry geo = label.getGeometry();
            geo.setX(0);
            geo.setY(0);
            geo.setRelative(true);
            geo.setOffset(new mxPoint(-geo.getWidth() / 2, -geo.getHeight() / 2));
        }
    }
    else
    {
        edge = graph.insertEdge(parent, null, edgeShape.getTextLabel(), source,
                target, mxVsdxUtils.getStyleString(styleMap, "="));

        mxPoint lblOffset = edgeShape.getLblEdgeOffset(graph.getView(), points);
        ((mxCell)edge).getGeometry().setOffset(lblOffset);
    }

    mxGeometry edgeGeometry = graph.getModel().getGeometry(edge);
    edgeGeometry.setPoints(points);

    //Gets and sets routing points of the edge.
    if (styleMap.containsKey("curved")
            && styleMap.get("curved").equals("1"))
    {
        edgeGeometry = graph.getModel().getGeometry(edge);
        List<mxPoint> pointList = edgeShape
                .getControlPoints(parentHeight);
        edgeGeometry.setPoints(pointList);
    }

    return edgeId;
}

/**
 * Adds a new edge not connected to any vertex to the graph.
 * @param graph Graph where the parsed graph is included.
 * @param parent Parent cell of the edge to be imported.
 * @param edgeShape Shape Element that represents an edge.
 * @return The new edge added.
 */
protected Object addUnconnectedEdge(mxGraph graph, Object parent, VsdxShape edgeShape, double pageHeight)
{
    double parentHeight = pageHeight;

    if (parent != null)
    {
        mxGeometry parentGeometry = graph.getModel().getGeometry(parent);

        if (parentGeometry != null)
        {
            parentHeight = parentGeometry.getHeight();
        }
    }

    mxPoint beginXY = edgeShape.getStartXY(parentHeight);
    mxPoint endXY = edgeShape.getEndXY(parentHeight);

    //Define style of the edge
    Map<String, String> styleMap = edgeShape.getStyleFromEdgeShape(parentHeight);

    //TODO add style numeric entries rounding option

    //Insert new edge and set constraints.
    Object edge;
    List<mxPoint> points = edgeShape.getRoutingPoints(parentHeight, beginXY, edgeShape.getRotation());
    double rotation = edgeShape.getRotation();
    if (rotation != 0)
    {
        if (edgeShape.getShapeIndex() == 0)
        {
            edge = graph.insertEdge(parent, null, null, null, null, mxVsdxUtils.getStyleString(styleMap, "="));
        }
        else
        {
            edge = graph.createEdge(parent, null, null, null, null, mxVsdxUtils.getStyleString(styleMap, "="));
            edge = graph.addEdge(edge, parent, null, null, edgeShape.getShapeIndex());
        }
        mxCell label = edgeShape.createLabelSubShape(graph, (mxCell)edge);
        if (label != null)
        {
            label.setStyle(label.getStyle() + ";rotation=" + (rotation > 60 && rotation < 240 ? (rotation + 180) % 360 : rotation));

            mxGeometry geo = label.getGeometry();
            geo.setX(0);
            geo.setY(0);
            geo.setRelative(true);
            geo.setOffset(new mxPoint(-geo.getWidth() / 2, -geo.getHeight() / 2));
        }
    }
    else
    {
        if (edgeShape.getShapeIndex() == 0)
        {
            edge = graph.insertEdge(parent, null, edgeShape.getTextLabel(), null, null, mxVsdxUtils.getStyleString(styleMap, "="));
        }
        else
        {
            edge = graph.createEdge(parent, null, edgeShape.getTextLabel(), null, null, mxVsdxUtils.getStyleString(styleMap, "="));
            edge = graph.addEdge(edge, parent, null, null, edgeShape.getShapeIndex());
        }

        mxPoint lblOffset = edgeShape.getLblEdgeOffset(graph.getView(), points);
        ((mxCell)edge).getGeometry().setOffset(lblOffset);
    }

    rotateChildEdge(graph.getModel(), parent, beginXY, endXY, points);

    mxGeometry edgeGeometry = graph.getModel().getGeometry(edge);
    edgeGeometry.setPoints(points);

    edgeGeometry.setTerminalPoint(beginXY, true);
    edgeGeometry.setTerminalPoint(endXY, false);

    //Gets and sets routing points of the edge.
    if (styleMap.containsKey("curved")
            && styleMap.get("curved").equals("1"))
    {
        edgeGeometry = graph.getModel().getGeometry(edge);
        List<mxPoint> pointList = edgeShape
                .getControlPoints(parentHeight);
        edgeGeometry.setPoints(pointList);
    }

    return edge;
}

protected void rotateChildEdge(mxIGraphModel model, Object parent, mxPoint beginXY, mxPoint endXY, List<mxPoint> points)
{
    //Rotate all points based on parent rotation
    //Must get parent rotation and apply it similar to what we did in group rotation of all children
    if (parent != null)
    {
        mxGeometry pgeo = model.getGeometry(parent);
        String pStyle = model.getStyle(parent);

        if (pgeo != null && pStyle != null)
        {
            int pos = pStyle.indexOf("rotation=");

            if (pos > -1)
            {
                double pRotation = Double.parseDouble(pStyle.substring(pos + 9, pStyle.indexOf(';', pos))); //9 is the length of "rotation="

                double hw = pgeo.getWidth() / 2, hh = pgeo.getHeight() / 2;

                rotatedEdgePoint(beginXY, pRotation, hw, hh);
                rotatedEdgePoint(endXY, pRotation, hw, hh);

                for (mxPoint p : points)
                {
                    rotatedEdgePoint(p, pRotation, hw, hh);
                }
            }
        }
    }
}

/**
 * Post processes groups to remove leaf vertices that render nothing
 * @param group
 */
protected void sanitiseGraph(mxGraph graph)
{
    Object root = graph.getModel().getRoot();
    sanitiseCell(graph, root);
}

private boolean sanitiseCell(mxGraph graph, Object cell)
{
    mxIGraphModel model = graph.getModel();
    int childCount = model.getChildCount(cell);
    ArrayList<Object> removeList = new ArrayList<Object>();

    for (int i = 0; i < childCount; i++)
    {
        Object child = model.getChildAt(cell, i);
        boolean remove = sanitiseCell(graph, child);

        // Can't remove during loop or indexing is messed up
        if (remove)
        {
            removeList.add(child);
        }
    }

    for (Object removeChild : removeList)
    {
        model.remove(removeChild);
    }

    if (childCount > 0)
    {
        // children may have been removed above
        childCount = model.getChildCount(cell);
    }

    String value = String.valueOf(model.getValue(cell));
    String style = model.getStyle(cell);

    if (childCount == 0 && model.isVertex(cell))
    {
        if ((model.getValue(cell) == null || value.isEmpty()) &&
                (style != null) &&
                (style.contains(mxConstants.STYLE_FILLCOLOR + "=none")) &&
                (style.contains(mxConstants.STYLE_STROKECOLOR + "=none")) &&
                (!style.contains("image=")))
        {
            // Leaf vertex, nothing rendered, no label, remove it

            return true;
        }
    }

    return false;
}
    }
}
