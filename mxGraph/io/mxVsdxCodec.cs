using ICSharpCode.SharpZipLib.Zip;
using mxGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Copyright (c) 2006-2017, JGraph Ltd
/// Copyright (c) 2006-2017, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{



    //using Base64 = org.apache.commons.codec.binary.Base64;
    //using StringUtils = org.apache.commons.codec.binary.StringUtils;
    //using StringEscapeUtils = org.apache.commons.lang3.StringEscapeUtils;
    using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;
    using NodeList = System.Xml.XmlNodeList;
    //using SAXException = org.xml.sax.SAXException;

    //using Image = com.google.appengine.api.images.Image;
    //using ImagesService = com.google.appengine.api.images.ImagesService;
    //using OutputEncoding = com.google.appengine.api.images.ImagesService.OutputEncoding;
    //using ImagesServiceFactory = com.google.appengine.api.images.ImagesServiceFactory;
    //using Transform = com.google.appengine.api.images.Transform;
    //using SystemProperty = com.google.appengine.api.utils.SystemProperty;
    using ShapePageId = mxGraph.io.vsdx.ShapePageId;
    using VsdxShape = mxGraph.io.vsdx.VsdxShape;
    using mxPathDebug = mxGraph.io.vsdx.mxPathDebug;
    using mxVsdxConnect = mxGraph.io.vsdx.mxVsdxConnect;
    using mxVsdxConstants = mxGraph.io.vsdx.mxVsdxConstants;
    using mxVsdxGeometryList = mxGraph.io.vsdx.mxVsdxGeometryList;
    using mxVsdxMaster = mxGraph.io.vsdx.mxVsdxMaster;
    using mxVsdxModel = vsdx.mxVsdxModel;
    using mxVsdxPage = mxGraph.io.vsdx.mxVsdxPage;
    using mxVsdxUtils = mxGraph.io.vsdx.mxVsdxUtils;
    using mxCell = mxGraph.model.mxCell;
    using mxGeometry = mxGraph.model.mxGeometry;
    using mxGraphModel = mxGraph.model.mxGraphModel;
    using mxICell = mxGraph.model.mxICell;
    using mxIGraphModel = mxGraph.model.mxIGraphModel;
    using Utils = mxGraph.online.Utils;
    using mxBase64 = mxGraph.online.mxBase64;
    using mxConstants = mxGraph.util.mxConstants;
    using mxPoint = mxGraph.util.mxPoint;
    using mxRectangle = mxGraph.util.mxRectangle;
    using mxXmlUtils = mxGraph.util.mxXmlUtils;
    using mxConnectionConstraint = mxGraph.view.mxConnectionConstraint;
    using mxGraph = mxGraph.view.mxGraph;
    using mxGraphHeadless = mxGraph.view.mxGraphHeadless;

    /// <summary>
    /// Parses a .vsdx XML diagram file and imports it in the given graph.<br/>
    /// </summary>
    public class mxVsdxCodec
    {
        protected internal string RESPONSE_END = "</mxfile>";

        protected internal string RESPONSE_DIAGRAM_START = "";
        protected internal string RESPONSE_DIAGRAM_END = "</diagram>";

        protected internal string RESPONSE_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><mxfile>";

        /// <summary>
        /// Stores the vertexes imported.
        /// </summary>
        protected internal Dictionary<ShapePageId, mxCell> vertexMap = new Dictionary<ShapePageId, mxCell>();

        /// <summary>
        /// Stores the shapes that represent Edges.
        /// </summary>
        protected internal Dictionary<ShapePageId, VsdxShape> edgeShapeMap = new Dictionary<ShapePageId, VsdxShape>();

        /// <summary>
        /// Stores the shapes that represent Vertexes.
        /// </summary>
        protected internal Dictionary<ShapePageId, VsdxShape> vertexShapeMap = new Dictionary<ShapePageId, VsdxShape>();

        /// <summary>
        /// Stores the parents of the shapes imported.
        /// </summary>
        protected internal Dictionary<ShapePageId, object> parentsMap = new Dictionary<ShapePageId, object>();

        /// <summary>
        /// Set to true if you want to display spline debug data
        /// </summary>
        protected internal bool debugPaths = false;

        /// <summary>
        /// Do not remove, ask David
        /// </summary>
        public static string vsdxPlaceholder = new string(Base64.decodeBase64("dmlzaW8=").ConvertChars());

        protected internal mxVsdxModel vsdxModel;

        public mxVsdxCodec()
        {
        }

        /// <summary>
        /// Calculate the absolute coordinates of a cell's point. </summary>
        /// <param name="cellParent"> Cell that contains the point. </param>
        /// <param name="graph"> Graph where the parsed graph is included. </param>
        /// <param name="point"> Point to which coordinates are calculated. </param>
        /// <returns> The point in absolute coordinates. </returns>
        private static mxPoint calculateAbsolutePoint(object cellParent, mxGraph graph, mxPoint point)
        {
            if (cellParent != null)
            {
                mxGeometry geo = graph.Model.getGeometry(cellParent);

                if (geo != null)
                {
                    point.X = point.X + geo.X;
                    point.Y = point.Y + geo.Y;
                }
            }

            return point;
        }

        /// <summary>
        /// Parses the input VSDX format and uses the information to populate 
        /// the specified graph. </summary>
        /// <param name="docs"> All XML documents contained in the VSDX source file </param>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="ParserConfigurationException"> </exception>
        /// <exception cref="SAXException"> </exception>
        /// <exception cref="TransformerException">  </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String decodeVsdx(byte[] data, String charset) throws java.io.IOException, javax.xml.parsers.ParserConfigurationException, org.xml.sax.SAXException, javax.xml.transform.TransformerException
        public virtual string decodeVsdx(byte[] data, string charset)
        {
            ZipInputStream zis = new ZipInputStream(new System.IO.MemoryStream(data));
            ZipEntry ze = null;
            IDictionary<string, Document> docData = new Dictionary<string, Document>();
            IDictionary<string, string> mediaData = new Dictionary<string, string>();

            while ((ze = zis.GetNextEntry()) != null)
            {
                string filename = ze.Name;


                if (ze.IsFile)
                {
                    System.IO.MemoryStream @out = new System.IO.MemoryStream();


                    Utils.copy(zis, @out);
                    @out.Close();



                    if (filename.ToLower().EndsWith(".xml", StringComparison.Ordinal) | filename.ToLower().EndsWith(".xml.rels", StringComparison.Ordinal))
                    {
                        //string str = @out.ToString(charset);
                        byte[] b = @out.ToArray();
                        string str = System.Text.Encoding.UTF8.GetString(b);

                        if (str.Length > 0)
                        {
                            Document doc = mxXmlUtils.parseXml(str);
                            // Hack to be able to find the filename from an element in the XML
                            //doc.BaseURI = filename;
                            docData.Add(filename, doc);

                            //Console.Error.WriteLine(filename);
                            //Console.Error.WriteLine(doc.OuterXml);

                        }
                    }
                    #region
                    /*
					else if (filename.ToLower().StartsWith(mxVsdxCodec.vsdxPlaceholder + "/media", StringComparison.Ordinal))
					{
						string base64Str = "";
						//Some BMP images are huge and doesn't show up in the browser, so, it is better to compress it as PNG 
						if (filename.ToLower().EndsWith(".bmp", StringComparison.Ordinal))
						{
							try
							{
								string environ = SystemProperty.environment.get();

								if (environ.Equals("Production") || environ.Equals("Development"))
								{
									ImagesService imagesService = ImagesServiceFactory.ImagesService;

									Image image = ImagesServiceFactory.makeImage(@out.toByteArray());

									//dummy transform
									Transform transform = ImagesServiceFactory.makeCrop(0.0, 0.0, 1.0, 1.0);

									//Use PNG format as it is lossless similar to BMP but compressed
									Image newImage = imagesService.applyTransform(transform, image, ImagesService.OutputEncoding.PNG);

									base64Str = StringUtils.newStringUtf8(Base64.encodeBase64(newImage.ImageData, false));
								}
								else
								{
									//Use ImageIO as it is normally available in other servlet containers (e.g.; Tomcat)
									System.IO.MemoryStream bis = new System.IO.MemoryStream(@out.toByteArray());
									System.IO.MemoryStream bos = new System.IO.MemoryStream();

									BufferedImage image = ImageIO.read(bis);
									ImageIO.write(image, "PNG", bos);

									base64Str = StringUtils.newStringUtf8(Base64.encodeBase64(bos.toByteArray(), false));
								}
							}
							catch (Exception)
							{
								//conversion failed, nothing we can do!
								base64Str = StringUtils.newStringUtf8(Base64.encodeBase64(@out.toByteArray(), false));
							}
						}
						else if (filename.ToLower().EndsWith(".emf", StringComparison.Ordinal)) //extract jpg or png images from emf file
						{
							sbyte[] emfData = @out.toByteArray();
							bool imageFound = false;
							//search for jpg or png header
							for (int i = 0; i < emfData.Length - 8; i++) //we subtract 8 from the length to be safe when testing image headers
							{
								if (isPng(emfData, i) || isJpg(emfData, i)) //png or jpg?
								{
									//although the resulting file is larger than the actual image but any extra bytes after the image are ignored
									base64Str = StringUtils.newStringUtf8(Base64.encodeBase64(Arrays.copyOfRange(emfData, i, emfData.Length), false));
									imageFound = true;
									break;
								}

							}
							if (!imageFound)
							{
								base64Str = StringUtils.newStringUtf8(Base64.encodeBase64(@out.toByteArray(), false));
							}
						}
						else
						{
							base64Str = StringUtils.newStringUtf8(Base64.encodeBase64(@out.toByteArray(), false));
						}

						mediaData[filename] = base64Str;
					}
                    */
                    #endregion

                }
            }

            zis.Close();

            string path = mxVsdxCodec.vsdxPlaceholder + "/document.xml";
            Document rootDoc = docData[path];
            Node rootChild = rootDoc.FirstChild;

            while (rootChild != null && !(rootChild is Element))
            {
                rootChild = rootChild.NextSibling;
            }

            if (rootChild != null && rootChild is Element)
            {
                importNodes(rootDoc, (Element)rootChild, path, docData);
            }
            else
            {
                // TODO log error
                return null;
            }

            vsdxModel = new mxVsdxModel(rootDoc, docData, mediaData);

            //Imports each page of the document.
            IDictionary<int?, mxVsdxPage> pages = vsdxModel.Pages;

            StringBuilder xmlBuilder = new StringBuilder(RESPONSE_HEADER);

            foreach (var entry in pages)
            {
                mxVsdxPage page = entry.Value;

                if (!page.Background)
                {
                    mxGraph graph = createMxGraph();

                    graph.Model.beginUpdate();
                    importPage(page, graph, graph.DefaultParent);

                    mxVsdxPage backPage = page.BackPage;

                    if (backPage != null)
                    {
                        graph.Model.setValue(graph.DefaultParent, page.PageName);
                        object backCell = new mxCell(backPage.PageName);
                        graph.addCell(backCell, graph.Model.Root, 0, null, null);
                        importPage(backPage, graph, graph.DefaultParent);
                    }

                    //scale page 
                    double scale = page.PageScale / page.DrawingScale;

                    if (scale != 1)
                    {
                        mxGraphModel model = (mxGraphModel)graph.Model;

                        foreach (object c in model.Cells.Values)
                        {
                            mxGeometry geo = model.getGeometry(c);

                            if (geo != null)
                            {
                                scaleRect(geo, scale);
                                scaleRect(geo.AlternateBounds, scale);

                                if (model.isEdge(c))
                                {
                                    //scale edge waypoints, offset, ...
                                    scalePoint(geo.SourcePoint, scale);
                                    scalePoint(geo.TargetPoint, scale);
                                    scalePoint(geo.Offset, scale);
                                    IList<mxPoint> points = geo.Points;

                                    if (points != null)
                                    {
                                        foreach (mxPoint p in points)
                                        {
                                            scalePoint(p, scale);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    graph.Model.endUpdate();

                    xmlBuilder.Append(RESPONSE_DIAGRAM_START);
                    xmlBuilder.Append(processPage(graph, page));
                    xmlBuilder.Append(RESPONSE_DIAGRAM_END);
                }
            }

            xmlBuilder.Append(RESPONSE_END);

            return xmlBuilder.ToString();
        }

        /// <summary>
        /// ZIP压缩单个文件
        /// </summary>
        /// <param name="sFileToZip">需要压缩的文件（绝对路径）</param>
        /// <param name="sZippedPath">压缩后的文件路径（绝对路径）</param>
        /// <param name="sZippedFileName">压缩后的文件名称（文件名，默认 同源文件同名）</param>
        /// <param name="nCompressionLevel">压缩等级（0 无 - 9 最高，默认 5）</param>
        /// <param name="nBufferSize">缓存大小（每次写入文件大小，默认 2048）</param>
        /// <param name="bEncrypt">是否加密（默认 加密）</param>
        /// <param name="sPassword">密码（设置加密时生效。默认密码为"123"）</param>
        public static string ZipFile(string sFileToZip, string sZippedPath, string sZippedFileName = "", int nCompressionLevel = 5, int nBufferSize = 2048, bool bEncrypt = true, string sPassword = "123")
        {
            if (!File.Exists(sFileToZip))
            {
                return null;
            }
            string sZipFileName = string.IsNullOrEmpty(sZippedFileName) ? sZippedPath + "\\" + new FileInfo(sFileToZip).Name.Substring(0, new FileInfo(sFileToZip).Name.LastIndexOf('.')) + ".zip" : sZippedPath + "\\" + sZippedFileName + ".zip";
            using (FileStream aZipFile = File.Create(sZipFileName))
            {
                using (ZipOutputStream aZipStream = new ZipOutputStream(aZipFile))
                {
                    using (FileStream aStreamToZip = new FileStream(sFileToZip, FileMode.Open, FileAccess.Read))
                    {
                        string sFileName = sFileToZip.Substring(sFileToZip.LastIndexOf("\\") + 1);
                        ZipEntry ZipEntry = new ZipEntry(sFileName);
                        if (bEncrypt)
                        {
                            aZipStream.Password = sPassword;
                        }
                        aZipStream.PutNextEntry(ZipEntry);
                        aZipStream.SetLevel(nCompressionLevel);
                        byte[] buffer = new byte[nBufferSize];
                        int sizeRead = 0;
                        try
                        {
                            do
                            {
                                sizeRead = aStreamToZip.Read(buffer, 0, buffer.Length);
                                aZipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        aStreamToZip.Close();
                    }
                    aZipStream.Finish();
                    aZipStream.Close();
                }
                aZipFile.Close();
            }
            return sZipFileName;
        }
        /// <summary>
        /// ZIP:解压一个zip文件
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="ZipFile">需要解压的Zip文件（绝对路径）</param>
        /// <param name="TargetDirectory">解压到的目录</param>
        /// <param name="Password">解压密码</param>
        /// <param name="OverWrite">是否覆盖已存在的文件</param>
        public static void UnZip(string ZipFile, string TargetDirectory, string Password, bool OverWrite = true)
        {
            //如果解压到的目录不存在，则报错
            if (!System.IO.Directory.Exists(TargetDirectory))
            {
                throw new System.IO.FileNotFoundException("指定的目录: " + TargetDirectory + " 不存在!");
            }
            //目录结尾
            if (!TargetDirectory.EndsWith("\\")) { TargetDirectory = TargetDirectory + "\\"; }

            using (ZipInputStream zipfiles = new ZipInputStream(File.OpenRead(ZipFile)))
            {
                zipfiles.Password = Password;
                ZipEntry theEntry;

                while ((theEntry = zipfiles.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(TargetDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(TargetDirectory + directoryName + fileName) && OverWrite) || (!File.Exists(TargetDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(TargetDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipfiles.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                zipfiles.Close();
            }
        }

        protected internal virtual mxGraph createMxGraph()
        {
            mxGraph graph = new mxGraphHeadless();
            //Disable parent (groups) auto extend feature as it miss with the coordinates of vsdx format
            graph.ExtendParents = false;
            graph.ExtendParentsOnAdd = false;

            graph.ConstrainChildren = false;
            graph.HtmlLabels = true;
            //Prevent change of edge parent as it misses with the routing points
            ((mxGraphModel)graph.Model).MaintainEdgeParent = false;
            return graph;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected String processPage(mxGraphview.mxGraph graph, mxGraphio.vsdx.mxVsdxPage page) throws java.io.IOException
        protected internal virtual string processPage(mxGraph graph, mxVsdxPage page)
        {
            mxCodec codec = new mxCodec();
            Node node = codec.encode(graph.Model);
            ((Element)node).SetAttribute("style", "default-style2");
            string modelString = mxXmlUtils.getXml(node);
            string modelAscii = Utils.encodeURIComponent(modelString, Utils.CHARSET_FOR_URL_ENCODING);
            byte[] modelBytes = Utils.deflate(modelAscii);

            StringBuilder output = new StringBuilder();

            if (page != null)
            {
                string pageName = page.PageName;// StringEscapeUtils.escapeXml11(page.PageName);
                output.Append("<diagram name=\"");
                output.Append(pageName);
                output.Append("\">");
            }
            sbyte[] mySByte = new sbyte[modelBytes.Length];

            for (int i = 0; i < modelBytes.Length; i++)
            {
                if (modelBytes[i] > 127)
                    mySByte[i] = (sbyte)(modelBytes[i] - 256);
                else
                    mySByte[i] = (sbyte)modelBytes[i];
            }


            output.Append(mxBase64.encodeToString(mySByte, false));

            return output.ToString();
        }

        private bool isJpg(sbyte[] emfData, int i)
        {
            //the loop calling this function make sure that we still have 3 bytes in the buffer
            return emfData[i] == unchecked((sbyte)0xFF) && emfData[i + 1] == unchecked((sbyte)0xD8) && emfData[i + 2] == unchecked((sbyte)0xFF);
        }

        private bool isPng(sbyte[] emfData, int i)
        {
            //the loop calling this function make sure that we still have 8 bytes in the buffer
            return emfData[i] == unchecked((sbyte)0x89) && emfData[i + 1] == (sbyte)0x50 && emfData[i + 2] == (sbyte)0x4E && emfData[i + 3] == (sbyte)0x47 && emfData[i + 4] == (sbyte)0x0D && emfData[i + 5] == (sbyte)0x0A && emfData[i + 6] == (sbyte)0x1A && emfData[i + 7] == (sbyte)0x0A;
        }

        /// <summary>
        /// Scale a point in place
        /// </summary>
        /// <param name="p"> point to scale in place </param>
        /// <param name="scale"> scale </param>
        /// <returns> scaled point </returns>
        private mxPoint scalePoint(mxPoint p, double scale)
        {
            if (p != null)
            {
                p.X = p.X * scale;
                p.Y = p.Y * scale;
            }

            return p;
        }

        /// <summary>
        /// Scale a rectangle in place
        /// </summary>
        /// <param name="rect"> rectangle to scale in place </param>
        /// <param name="scale"> scale </param>
        /// <returns> scaled rectangle </returns>
        private mxRectangle scaleRect(mxRectangle rect, double scale)
        {
            if (rect != null)
            {
                rect.X = rect.X * scale;
                rect.Y = rect.Y * scale;
                rect.Height = rect.Height * scale;
                rect.Width = rect.Width * scale;
            }

            return rect;
        }

        /// 
        /// <param name="rootDoc"> </param>
        /// <param name="currentNode"> </param>
        /// <param name="path"> </param>
        /// <param name="docData"> </param>
        private void importNodes(Document rootDoc, Element currentNode, string path, IDictionary<string, Document> docData)
        {
            int lastSlash = path.LastIndexOf("/", StringComparison.Ordinal);

            string dir = path;
            string fileName = path;

            if (lastSlash != -1)
            {
                dir = path.Substring(0, lastSlash);
                fileName = path.Substring(lastSlash + 1, path.Length - (lastSlash + 1));
            }
            else
            {
                // Can't handle this case
                return;
            }

            string relsPath = dir + "/_rels/" + fileName + ".rels";
            Document relsDoc = docData[relsPath];

            if (relsDoc == null)
            {
                // Valid to not have a rels for an XML file
                return;
            }

            NodeList rels = relsDoc.GetElementsByTagName("Relationship");
            IDictionary<string, string> relMap = new Dictionary<string, string>();

            for (int i = 0; i < rels.Count; i++)
            {
                Element currElem = (Element)rels.Item(i);
                string id = currElem.GetAttribute("Id");
                string target = currElem.GetAttribute("Target");
                if (relMap.ContainsKey(id))
                {
                    relMap[id] = target;
                }
                else
                {
                    relMap.Add(id, target);
                }
            }

            NodeList relList = currentNode.GetElementsByTagName("Rel");

            for (int i = 0; i < relList.Count; i++)
            {
                Element rel = (Element)relList.Item(i);
                string pathSuffix = relMap[rel.GetAttribute("r:id")];
                string target = dir + "/" + pathSuffix;

                if (!string.ReferenceEquals(target, null))
                {
                    Document childDoc = docData[target];

                    if (childDoc != null)
                    {
                        Node parent = rel.ParentNode;
                        Node rootChild = childDoc.FirstChild;

                        while (rootChild != null && !(rootChild is Element))
                        {
                            rootChild = rootChild.NextSibling;
                        }

                        if (rootChild != null && rootChild is Element)
                        {
                            Node importNode = rootChild.FirstChild;

                            while (importNode != null)
                            {
                                if (importNode is Element)
                                {
                                    Node newNode = parent.AppendChild(rootDoc.ImportNode(importNode, true));
                                    string pathTmp = target;
                                    importNodes(rootDoc, (Element)newNode, pathTmp, docData);
                                }

                                importNode = importNode.NextSibling;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Imports a page of the document with the actual pageHeight.<br/>
        /// In .vdx, the Y-coordinate grows upward from the bottom of the page.<br/>
        /// The page height is used for calculating the correct position in mxGraph using
        /// this formula: mxGraph_Y_Coord = PageHeight - VSDX_Y_Coord. </summary>
        /// <param name="page"> Actual page Element to be imported </param>
        /// <param name="graph"> Graph where the parsed graph is included. </param>
        /// <param name="parent"> The parent of the elements to be imported. </param>
        protected internal virtual double importPage(mxVsdxPage page, mxGraph graph, object parent)
        {
            IDictionary<int?, VsdxShape> shapes = page.Shapes;
            //IEnumerator<KeyValuePair<int?, VsdxShape>> entries = shapes.SetOfKeyValuePairs().GetEnumerator();

            double pageHeight = page.PageDimensions.Y;
            int? pageId = page.Id;

            foreach (var entry in shapes)
            {
                //KeyValuePair<int?, VsdxShape> entry = entries.Current;

                //if (this.debugPaths)
                //{
                //	mxPathDebug debug = new mxPathDebug(true, graph, entry.Value, pageHeight);
                //	entry.Value.debug = debug;
                //}

                addShape(graph, entry.Value, parent, pageId, pageHeight);
            }

            IDictionary<int?, mxVsdxConnect> connects = page.Connects;
            //IEnumerator<KeyValuePair<int?, mxVsdxConnect>> entries2 = connects.SetOfKeyValuePairs().GetEnumerator();

            foreach (var entry in connects)
            {
                //KeyValuePair<int?, mxVsdxConnect> entry = entries2.Current;
                ShapePageId edgeId = addConnectedEdge(graph, entry.Value, pageId, pageHeight);

                if (edgeId != null)
                {
                    edgeShapeMap.Remove(edgeId); // ensure not processed twice
                }
            }

            //Process unconnected edges.
            //IEnumerator<KeyValuePair<ShapePageId, VsdxShape>> it = edgeShapeMap.SetOfKeyValuePairs().GetEnumerator();

            foreach (var edgeShapeEntry in edgeShapeMap)
            {
                //KeyValuePair<ShapePageId, VsdxShape> edgeShapeEntry = it.Current;

                //Only this page unconnected edges
                if (edgeShapeEntry.Key.PageNumber == pageId)
                {
                    addUnconnectedEdge(graph, parentsMap[edgeShapeEntry.Key], edgeShapeEntry.Value, pageHeight);
                }
            }

            sanitiseGraph(graph);

            return pageHeight;
        }

        /// <summary>
        /// Adds a vertex to the graph if 'shape' is a vertex or add the shape to edgeShapeMap if it is an edge.
        /// This method doesn't import sub-shapes of 'shape'. </summary>
        /// <param name="graph"> Graph where the parsed graph is included. </param>
        /// <param name="shp"> Shape to be imported. </param>
        /// <param name="parentHeight"> Height of the parent cell. </param>
        /// <returns> the new vertex added. null if 'shape' is not a vertex. </returns>
        protected internal virtual mxCell addShape(mxGraph graph, VsdxShape shape, object parent, int? pageId, double parentHeight)
        {
            shape.parentHeight = parentHeight;

            string type = VsdxShape.getType(shape.Shape);

            //If is a Shape or a Group add the vertex to the graph.
            if (!string.ReferenceEquals(type, null) && (type.Equals(mxVsdxConstants.TYPE_SHAPE) || type.Equals(mxVsdxConstants.TYPE_GROUP) || type.Equals(mxVsdxConstants.FOREIGN)))
            {
                int id = shape.Id.Value;

                if (shape.Vertex)
                {
                    mxCell v1 = null;

                    if (shape.Group)
                    {
                        v1 = addGroup(graph, shape, parent, pageId, parentHeight);
                    }
                    else
                    {
                        v1 = addVertex(graph, shape, parent, pageId, parentHeight);
                    }

                    vertexShapeMap[new ShapePageId(pageId.Value, id)] = shape;
                    return v1;
                }
                else
                {
                    //remember the edge order to maintain the shapes order (back to front)
                    shape.ShapeIndex = graph.Model.getChildCount(parent);
                    edgeShapeMap[new ShapePageId(pageId.Value, id)] = shape;
                    parentsMap[new ShapePageId(pageId.Value, id)] = parent;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a group to the graph.
        /// The sub-shapes of a complex shape are processed like part of the shape. </summary>
        /// <param name="graph"> Graph where the parsed graph is included. </param>
        /// <param name="parent"> Parent cell of the shape. </param>
        /// <param name="parentHeight"> Height of the parent cell of the shape. </param>
        /// <returns> Cell added to the graph. </returns>
        public virtual mxCell addGroup(mxGraph graph, VsdxShape shape, object parent, int? pageId, double parentHeight)
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
            mxPoint d = shape.Dimensions;
            mxVsdxMaster master = shape.Master;
            //Define style
            IDictionary<string, string> styleMap = shape.StyleFromShape;

            //Shape inherit its master geometry, so we don't need to check its master
            mxVsdxGeometryList geomList = shape.GeomList;

            if (geomList.NoFill)
            {
                styleMap[mxConstants.STYLE_FILLCOLOR] = "none";
                styleMap[mxConstants.STYLE_GRADIENTCOLOR] = "none";
            }

            if (geomList.NoLine)
            {
                styleMap[mxConstants.STYLE_STROKECOLOR] = "none";
            }

            styleMap["html"] = "1";
            styleMap[mxConstants.STYLE_WHITE_SPACE] = "wrap";
            //TODO need to check if "shape=" should be added before the shape name (for "image", it should be skipped for example)
            string style = mxVsdxUtils.getStyleString(styleMap, "=");

            mxCell group = null;
            IDictionary<int?, VsdxShape> children = shape.ChildShapes;
            bool hasChildren = children != null && children.Count > 0;
            bool subLabel = shape.DisplacedLabel || shape.RotatedLabel || hasChildren;
            mxPoint o = shape.getOriginPoint(parentHeight, true);

            if (subLabel)
            {
                group = (mxCell)graph.insertVertex(parent, null, null, o.X, o.Y, d.X, d.Y, style);
            }
            else
            {
                string textLabel = shape.TextLabel;
                group = (mxCell)graph.insertVertex(parent, null, textLabel, o.X, o.Y, d.X, d.Y, style);
            }

            //IEnumerator<KeyValuePair<int?, VsdxShape>> entries = children.SetOfKeyValuePairs().GetEnumerator();

            foreach (var entry in children)
            {
                //KeyValuePair<int?, VsdxShape> entry = entries.Current;
                VsdxShape subShape = entry.Value;
                int? Id = subShape.Id;

                if (subShape.Vertex)
                {
                    if (this.debugPaths)
                    {
                        mxPathDebug debug = new mxPathDebug(true, graph, subShape, parentHeight);
                        subShape.debug = debug;
                    }

                    string type = VsdxShape.getType(subShape.Shape);

                    //If is a Shape or a Group add the vertex to the graph.
                    if (!string.ReferenceEquals(type, null) && (type.Equals(mxVsdxConstants.TYPE_SHAPE) || type.Equals(mxVsdxConstants.TYPE_GROUP) || type.Equals(mxVsdxConstants.FOREIGN)))
                    {
                        if (subShape.Vertex)
                        {
                            subShape.propagateRotation(shape.Rotation);

                            if (subShape.Group)
                            {
                                addGroup(graph, subShape, group, pageId, d.Y);
                            }
                            else
                            {
                                addVertex(graph, subShape, group, pageId, d.Y);
                            }
                        }
                    }

                    if (master == null)
                    {
                        // If the group doesn't have a master, sub vertices are instances of document masters
                        vertexShapeMap[new ShapePageId(pageId.Value, Id.Value)] = subShape;
                    }
                }
                else
                {
                    if (master == null)
                    {
                        // If the group doesn't have a master, sub edges are instances of document masters
                        edgeShapeMap[new ShapePageId(pageId.Value, Id.Value)] = subShape;
                        parentsMap[new ShapePageId(pageId.Value, Id.Value)] = group;
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
            double rotation = shape.Rotation;
            if (rotation != 0)
            {
                mxGeometry pgeo = group.Geometry;
                double hw = pgeo.Width / 2, hh = pgeo.Height / 2;
                for (int i = 0; i < group.ChildCount; i++)
                {
                    mxICell child = group.getChildAt(i);
                    rotatedPoint(child.Geometry, rotation, hw, hh);
                }
            }
            return group;
        }

        public static void rotatedPoint(mxGeometry geo, double rotation, double cx, double cy)
        {
            rotation = Common.ToRadians(rotation); //Math.toRadians(rotation);
            double cos = Math.Cos(rotation), sin = Math.Sin(rotation);

            double x = geo.CenterX - cx;
            double y = geo.CenterY - cy;

            double x1 = x * cos - y * sin;
            double y1 = y * cos + x * sin;

            geo.X = Math.Round(x1 + cx - geo.Width / 2);
            geo.Y = Math.Round(y1 + cy - geo.Height / 2);
        }

        public static void rotatedEdgePoint(mxPoint pt, double rotation, double cx, double cy)
        {
            rotation = Common.ToRadians(rotation); //Math.toRadians(rotation);
            double cos = Math.Cos(rotation), sin = Math.Sin(rotation);

            double x = pt.X - cx;
            double y = pt.Y - cy;

            double x1 = x * cos - y * sin;
            double y1 = y * cos + x * sin;

            pt.X = Math.Round(x1 + cx);
            pt.Y = Math.Round(y1 + cy);
        }

        /// <summary>
        /// Adds a simple shape to the graph </summary>
        /// <param name="graph"> Graph where the parsed graph is included. </param>
        /// <param name="parent"> Parent cell of the shape. </param>
        /// <param name="parentHeight"> Height of the parent cell of the shape. </param>
        /// <returns> Cell added to the graph. </returns>
        public virtual mxCell addVertex(mxGraph graph, VsdxShape shape, object parent, int? pageId, double parentHeight)
        {
            //Defines Text Label.
            string textLabel = "";

            bool hasSubLabel = shape.DisplacedLabel || shape.RotatedLabel; // || shape.getRotation() != 0;

            if (!hasSubLabel)
            {
                textLabel = shape.TextLabel;
            }

            mxPoint dimensions = shape.Dimensions;

            IDictionary<string, string> styleMap = shape.StyleFromShape;

            //if (textLabel != null && (textLabel.startsWith("<p>") || textLabel.startsWith("<p ")
            //		|| textLabel.startsWith("<font")))
            //{
            styleMap["html"] = "1";
            //}

            bool geomExists = styleMap.ContainsKey(mxConstants.STYLE_SHAPE) || styleMap.ContainsKey("stencil");

            if (!styleMap.ContainsKey(mxConstants.STYLE_FILLCOLOR) || !geomExists)
            {
                styleMap[mxConstants.STYLE_FILLCOLOR] = "none";
            }

            if (!geomExists)
            {
                styleMap[mxConstants.STYLE_STROKECOLOR] = "none";
            }

            if (!styleMap.ContainsKey(mxConstants.STYLE_GRADIENTCOLOR) || !geomExists)
            {
                styleMap[mxConstants.STYLE_GRADIENTCOLOR] = "none";
            }

            styleMap[mxConstants.STYLE_WHITE_SPACE] = "wrap";

            mxPoint coordinates = shape.getOriginPoint(parentHeight, true);

            if (geomExists || !string.ReferenceEquals(textLabel, null))
            {
                string style = mxVsdxUtils.getStyleString(styleMap, "=");

                mxCell v1 = null;

                if (hasSubLabel)
                {
                    v1 = (mxCell)graph.insertVertex(parent, null, null, coordinates.X, coordinates.Y, dimensions.X, dimensions.Y, style);
                }
                else
                {
                    v1 = (mxCell)graph.insertVertex(parent, null, textLabel, coordinates.X, coordinates.Y, dimensions.X, dimensions.Y, style);
                }

                vertexMap[new ShapePageId(pageId.Value, shape.Id.Value)] = v1;
                shape.setLabelOffset(v1, style);

                if (hasSubLabel)
                {
                    shape.createLabelSubShape(graph, v1);
                }

                return v1;
            }

            return null;
        }

        /// <summary>
        /// Adds a connected edge to the graph.
        /// These edged are the referenced in one Connect element at least. </summary>
        /// <param name="graph"> graph Graph where the parsed graph is included. </param>
        /// <param name="connect"> Connect Element that references an edge shape and the source vertex. </param>
        protected internal virtual ShapePageId addConnectedEdge(mxGraph graph, mxVsdxConnect connect, int? pageId, double pageHeight)
        {
            int? fromSheet = connect.FromSheet;
            ShapePageId edgeId = new ShapePageId(pageId.Value, fromSheet.Value);
            VsdxShape edgeShape = edgeShapeMap[edgeId];

            if (edgeShape == null)
            {
                return null;
            }

            object parent = parentsMap[new ShapePageId(pageId.Value, edgeShape.Id.Value)];
            double parentHeight = pageHeight;

            if (parent != null)
            {
                mxGeometry parentGeo = graph.Model.getGeometry(parent);

                if (parentGeo != null)
                {
                    parentHeight = parentGeo.Height;
                }
            }

            //Get beginXY and endXY coordinates.
            mxPoint beginXY = edgeShape.getStartXY(parentHeight);
            mxPoint endXY = edgeShape.getEndXY(parentHeight);
            IList<mxPoint> points = edgeShape.getRoutingPoints(parentHeight, beginXY, edgeShape.Rotation);

            rotateChildEdge(graph.Model, parent, beginXY, endXY, points);

            int? sourceSheet = connect.SourceToSheet;

            mxCell source = sourceSheet != null ? vertexMap[new ShapePageId(pageId.Value, sourceSheet.Value)] : null;

            if (source == null)
            {
                // Source is dangling
                source = (mxCell)graph.insertVertex(parent, null, null, beginXY.X, beginXY.Y, 0, 0);
            }
            //Else: Routing points will contain the exit/entry points, so no need to set the to/from constraint 

            int? toSheet = connect.TargetToSheet;

            mxCell target = toSheet != null ? vertexMap[new ShapePageId(pageId.Value, toSheet.Value)] : null;

            if (target == null)
            {
                // Target is dangling
                target = (mxCell)graph.insertVertex(parent, null, null, endXY.X, endXY.Y, 0, 0);
            }
            //Else: Routing points will contain the exit/entry points, so no need to set the to/from constraint 

            //Defines the style of the edge.
            IDictionary<string, string> styleMap = edgeShape.getStyleFromEdgeShape(parentHeight);
            //Insert new edge and set constraints.
            object edge;
            double rotation = edgeShape.Rotation;
            if (rotation != 0)
            {
                edge = graph.insertEdge(parent, null, null, source, target, mxVsdxUtils.getStyleString(styleMap, "="));

                mxCell label = edgeShape.createLabelSubShape(graph, (mxCell)edge);
                if (label != null)
                {
                    label.Style = label.Style + ";rotation=" + (rotation > 60 && rotation < 240 ? (rotation + 180) % 360 : rotation);

                    mxGeometry geo = label.Geometry;
                    geo.X = 0;
                    geo.Y = 0;
                    geo.Relative = true;
                    geo.Offset = new mxPoint(-geo.Width / 2, -geo.Height / 2);
                }
            }
            else
            {
                edge = graph.insertEdge(parent, null, edgeShape.TextLabel, source, target, mxVsdxUtils.getStyleString(styleMap, "="));

                mxPoint lblOffset = edgeShape.getLblEdgeOffset(graph.View, points);
                ((mxCell)edge).Geometry.Offset = lblOffset;
            }

            mxGeometry edgeGeometry = graph.Model.getGeometry(edge);
            edgeGeometry.Points = points;

            //Gets and sets routing points of the edge.
            if (styleMap.ContainsKey("curved") && styleMap["curved"].Equals("1"))
            {
                edgeGeometry = graph.Model.getGeometry(edge);
                IList<mxPoint> pointList = edgeShape.getControlPoints(parentHeight);
                edgeGeometry.Points = pointList;
            }

            return edgeId;
        }

        /// <summary>
        /// Adds a new edge not connected to any vertex to the graph. </summary>
        /// <param name="graph"> Graph where the parsed graph is included. </param>
        /// <param name="parent"> Parent cell of the edge to be imported. </param>
        /// <param name="edgeShape"> Shape Element that represents an edge. </param>
        /// <returns> The new edge added. </returns>
        protected internal virtual object addUnconnectedEdge(mxGraph graph, object parent, VsdxShape edgeShape, double pageHeight)
        {
            double parentHeight = pageHeight;

            if (parent != null)
            {
                mxGeometry parentGeometry = graph.Model.getGeometry(parent);

                if (parentGeometry != null)
                {
                    parentHeight = parentGeometry.Height;
                }
            }

            mxPoint beginXY = edgeShape.getStartXY(parentHeight);
            mxPoint endXY = edgeShape.getEndXY(parentHeight);

            //Define style of the edge
            IDictionary<string, string> styleMap = edgeShape.getStyleFromEdgeShape(parentHeight);

            //TODO add style numeric entries rounding option

            //Insert new edge and set constraints.
            object edge;
            IList<mxPoint> points = edgeShape.getRoutingPoints(parentHeight, beginXY, edgeShape.Rotation);
            double rotation = edgeShape.Rotation;
            if (rotation != 0)
            {
                if (edgeShape.ShapeIndex == 0)
                {
                    edge = graph.insertEdge(parent, null, null, null, null, mxVsdxUtils.getStyleString(styleMap, "="));
                }
                else
                {
                    edge = graph.createEdge(parent, null, null, null, null, mxVsdxUtils.getStyleString(styleMap, "="));
                    edge = graph.addEdge(edge, parent, null, null, edgeShape.ShapeIndex);
                }
                mxCell label = edgeShape.createLabelSubShape(graph, (mxCell)edge);
                if (label != null)
                {
                    label.Style = label.Style + ";rotation=" + (rotation > 60 && rotation < 240 ? (rotation + 180) % 360 : rotation);

                    mxGeometry geo = label.Geometry;
                    geo.X = 0;
                    geo.Y = 0;
                    geo.Relative = true;
                    geo.Offset = new mxPoint(-geo.Width / 2, -geo.Height / 2);
                }
            }
            else
            {
                if (edgeShape.ShapeIndex == 0)
                {
                    edge = graph.insertEdge(parent, null, edgeShape.TextLabel, null, null, mxVsdxUtils.getStyleString(styleMap, "="));
                }
                else
                {
                    edge = graph.createEdge(parent, null, edgeShape.TextLabel, null, null, mxVsdxUtils.getStyleString(styleMap, "="));
                    edge = graph.addEdge(edge, parent, null, null, edgeShape.ShapeIndex);
                }

                mxPoint lblOffset = edgeShape.getLblEdgeOffset(graph.View, points);
                ((mxCell)edge).Geometry.Offset = lblOffset;
            }

            rotateChildEdge(graph.Model, parent, beginXY, endXY, points);

            mxGeometry edgeGeometry = graph.Model.getGeometry(edge);
            edgeGeometry.Points = points;

            edgeGeometry.setTerminalPoint(beginXY, true);
            edgeGeometry.setTerminalPoint(endXY, false);

            //Gets and sets routing points of the edge.
            if (styleMap.ContainsKey("curved") && styleMap["curved"].Equals("1"))
            {
                edgeGeometry = graph.Model.getGeometry(edge);
                IList<mxPoint> pointList = edgeShape.getControlPoints(parentHeight);
                edgeGeometry.Points = pointList;
            }

            return edge;
        }

        protected internal virtual void rotateChildEdge(mxIGraphModel model, object parent, mxPoint beginXY, mxPoint endXY, IList<mxPoint> points)
        {
            //Rotate all points based on parent rotation
            //Must get parent rotation and apply it similar to what we did in group rotation of all children
            if (parent != null)
            {
                mxGeometry pgeo = model.getGeometry(parent);
                string pStyle = model.getStyle(parent);

                if (pgeo != null && !string.ReferenceEquals(pStyle, null))
                {
                    int pos = pStyle.IndexOf("rotation=", StringComparison.Ordinal);

                    if (pos > -1)
                    {
                        double pRotation = double.Parse(pStyle.Substring(pos + 9, pStyle.IndexOf(';', pos) - (pos + 9))); //9 is the length of "rotation="

                        double hw = pgeo.Width / 2, hh = pgeo.Height / 2;

                        rotatedEdgePoint(beginXY, pRotation, hw, hh);
                        rotatedEdgePoint(endXY, pRotation, hw, hh);

                        foreach (mxPoint p in points)
                        {
                            rotatedEdgePoint(p, pRotation, hw, hh);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Post processes groups to remove leaf vertices that render nothing </summary>
        /// <param name="group"> </param>
        protected internal virtual void sanitiseGraph(mxGraph graph)
        {
            object root = graph.Model.Root;
            sanitiseCell(graph, root);
        }

        private bool sanitiseCell(mxGraph graph, object cell)
        {
            mxIGraphModel model = graph.Model;
            int childCount = model.getChildCount(cell);
            List<object> removeList = new List<object>();

            for (int i = 0; i < childCount; i++)
            {
                object child = model.getChildAt(cell, i);
                bool remove = sanitiseCell(graph, child);

                // Can't remove during loop or indexing is messed up
                if (remove)
                {
                    removeList.Add(child);
                }
            }

            foreach (object removeChild in removeList)
            {
                model.remove(removeChild);
            }

            if (childCount > 0)
            {
                // children may have been removed above
                childCount = model.getChildCount(cell);
            }

            string value = model.getValue(cell) == null ? null : model.getValue(cell).ToString();
            string style = model.getStyle(cell);

            if (childCount == 0 && model.isVertex(cell))
            {
                if ((model.getValue(cell) == null || value.Length == 0) && (!string.ReferenceEquals(style, null)) && (style.Contains(mxConstants.STYLE_FILLCOLOR + "=none")) && (style.Contains(mxConstants.STYLE_STROKECOLOR + "=none")) && (!style.Contains("image=")))
                {
                    // Leaf vertex, nothing rendered, no label, remove it

                    return true;
                }
            }

            return false;
        }
    }

}