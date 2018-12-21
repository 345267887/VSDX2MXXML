using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace com.mxgraph.io.vsdx.export
{



    using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	using mxCell = com.mxgraph.model.mxCell;
	using mxGeometry = com.mxgraph.model.mxGeometry;
	using mxGraphModel = com.mxgraph.model.mxGraphModel;
	using Utils = com.mxgraph.online.Utils;
	using mxBase64 = com.mxgraph.online.mxBase64;
	using mxXmlUtils = com.mxgraph.util.mxXmlUtils;
	using mxGraph = com.mxgraph.view.mxGraph;
	using mxGraphHeadless = com.mxgraph.view.mxGraphHeadless;

	public class mxVsdxExporter
	{
		private const string PAGES_TYPE = "http://schemas.microsoft.com/visio/2010/relationships/page";
		private const string RELS_XMLNS = "http://schemas.openxmlformats.org/package/2006/relationships";
		private const string XML_SPACE = "preserve";
		private const string XMLNS_R = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
		private const string XMLNS = "http://schemas.microsoft.com/office/visio/2012/main";
		private const string VISIO_PAGES = "visio/pages/";
		private const string PREFEX = "com/mxgraph/io/vsdx/resources/export/";
		private const string VSDX_ENC = "ISO-8859-1";
		private const string OVERRIDE = "Override";
		private const string PART_NAME = "PartName";
		private const string DOC_PROPS_CUSTOM_XML = "docProps/custom.xml";
		private const string DOC_PROPS_CORE_XML = "docProps/core.xml";
		private const string DOC_PROPS_APP_XML = "docProps/app.xml";
		private const string _RELS_RELS_FILE = "_rels/.rels";
		private const string CONTENT_TYPES_XML = "[Content_Types].xml";
		private const string VISIO_MASTERS_RELS_MASTERS_XML_RELS = "visio/masters/_rels/masters.xml.rels";
		private const string VISIO_MASTERS_MASTERS_XML = "visio/masters/masters.xml";
		private const string VISIO_RELS_DOCUMENT_XML_RELS = "visio/_rels/document.xml.rels";
		private const string VISIO_WINDOWS_XML = "visio/windows.xml";
		private const string VISIO_DOCUMENT_XML = "visio/document.xml";
		private const string VISIO_PAGES_RELS = "visio/pages/_rels/";

		private static sbyte[] documentFile;
		private static sbyte[] windowsFile;
		private static sbyte[] visioRelsFile;
		private static sbyte[] mastersFile;
		private static sbyte[] mastersRelsFile;
		private static sbyte[] contentTypeFile;
		private static sbyte[] _relsFile;
		private static sbyte[] appFile;
		private static sbyte[] coreFile;
		private static sbyte[] customFile;
		private static sbyte[] pageRelFile;

		//Load the constant vsdx files statically so it can be used faster for all export operations 
		static mxVsdxExporter()
		{
			try
			{
				ClassLoader classLoader = typeof(mxVsdxExporter).ClassLoader;

				string file = PREFEX + VISIO_DOCUMENT_XML;
				int size = (int) (new File(classLoader.getResource(file).File)).length();
				System.IO.Stream @in = classLoader.getResourceAsStream(file);

				documentFile = new sbyte[size];
				@in.Read(documentFile, 0, documentFile.Length);

				file = PREFEX + VISIO_WINDOWS_XML;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				windowsFile = new sbyte[size];
				@in.Read(windowsFile, 0, windowsFile.Length);

				file = PREFEX + VISIO_RELS_DOCUMENT_XML_RELS;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				visioRelsFile = new sbyte[size];
				@in.Read(visioRelsFile, 0, visioRelsFile.Length);

				file = PREFEX + VISIO_MASTERS_MASTERS_XML;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				mastersFile = new sbyte[size];
				@in.Read(mastersFile, 0, mastersFile.Length);

				file = PREFEX + VISIO_MASTERS_RELS_MASTERS_XML_RELS;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				mastersRelsFile = new sbyte[size];
				@in.Read(mastersRelsFile, 0, mastersRelsFile.Length);

				file = PREFEX + CONTENT_TYPES_XML;
				size = (int) (new File(URLDecoder.decode(classLoader.getResource(file).File, "UTF8"))).length();
				@in = classLoader.getResourceAsStream(file);

				contentTypeFile = new sbyte[size];
				@in.Read(contentTypeFile, 0, contentTypeFile.Length);

				file = PREFEX + _RELS_RELS_FILE;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				_relsFile = new sbyte[size];
				@in.Read(_relsFile, 0, _relsFile.Length);

				file = PREFEX + DOC_PROPS_APP_XML;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				appFile = new sbyte[size];
				@in.Read(appFile, 0, appFile.Length);

				file = PREFEX + DOC_PROPS_CORE_XML;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				coreFile = new sbyte[size];
				@in.Read(coreFile, 0, coreFile.Length);

				file = PREFEX + DOC_PROPS_CUSTOM_XML;
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				customFile = new sbyte[size];
				@in.Read(customFile, 0, customFile.Length);

				file = PREFEX + VISIO_PAGES_RELS + "page1.xml.rels";
				size = (int) (new File(classLoader.getResource(file).File)).length();
				@in = classLoader.getResourceAsStream(file);

				pageRelFile = new sbyte[size];
				@in.Read(pageRelFile, 0, pageRelFile.Length);
			}
			catch (Exception e) //We shouldn't get any exception as long as files exist.
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/// <summary>
		/// Fill the required files in vsdx format which are constants in our exporter </summary>
		/// <param name="zip"> ZipOutputStream of vsdx file </param>
		/// <param name="pageCount"> The number of pages in the mxFile </param>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void createVsdxSkeleton(java.util.zip.ZipOutputStream zip, int pageCount) throws java.io.IOException
		private void createVsdxSkeleton(ZipOutputStream zip, int pageCount)
		{
			zip.putNextEntry(new ZipEntry(VISIO_PAGES_RELS + "page1.xml.rels"));
			zip.write(pageRelFile);

			if (pageCount == 1)
			{
				zip.putNextEntry(new ZipEntry(CONTENT_TYPES_XML));
				zip.write(contentTypeFile);
			}
			else
			{
				//Add the remaining pages
				Document doc = mxXmlUtils.parseXml(StringHelperClass.NewString(contentTypeFile));
				Element root = doc.DocumentElement;

				List<Element> overrides = mxVsdxUtils.getDirectChildNamedElements(root, OVERRIDE);
				Element page1 = null;

				foreach (Element @override in overrides)
				{
					if ("/visio/pages/page1.xml".Equals(@override.getAttribute(PART_NAME)))
					{
						page1 = @override;
					}
				}

				for (int i = 2; i <= pageCount; i++)
				{
					zip.putNextEntry(new ZipEntry(VISIO_PAGES_RELS + "page" + i + ".xml.rels"));
					zip.write(pageRelFile);

					Element newPage = (Element) page1.cloneNode(false);
					newPage.setAttribute(PART_NAME, "/visio/pages/page" + i + ".xml");
					root.appendChild(newPage);
				}

				zip.putNextEntry(new ZipEntry(CONTENT_TYPES_XML));
				zip.write(("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" + mxXmlUtils.getXml(doc)).GetBytes(VSDX_ENC));
			}



			zip.putNextEntry(new ZipEntry(_RELS_RELS_FILE));
			zip.write(_relsFile);

			zip.putNextEntry(new ZipEntry(DOC_PROPS_APP_XML));
			zip.write(appFile);

			zip.putNextEntry(new ZipEntry(DOC_PROPS_CORE_XML));
			zip.write(coreFile);

			zip.putNextEntry(new ZipEntry(DOC_PROPS_CUSTOM_XML));
			zip.write(customFile);

			zip.putNextEntry(new ZipEntry(VISIO_DOCUMENT_XML));
			zip.write(documentFile);

			zip.putNextEntry(new ZipEntry(VISIO_WINDOWS_XML));
			zip.write(windowsFile);

			zip.putNextEntry(new ZipEntry(VISIO_RELS_DOCUMENT_XML_RELS));
			zip.write(visioRelsFile);

			zip.putNextEntry(new ZipEntry(VISIO_MASTERS_MASTERS_XML));
			zip.write(mastersFile);

			zip.putNextEntry(new ZipEntry(VISIO_MASTERS_RELS_MASTERS_XML_RELS));
			zip.write(mastersRelsFile);

			zip.flush();
		}

		/// 
		/// <summary>
		/// Convert mxFile into a vdsx file </summary>
		/// <param name="mxFileXML"> mxFile xml string </param>
		/// <param name="out"> output stream where vsdx file is written to </param>
		/// <returns> true if successful, false otherwise (stream should be discarded and closed) </returns>
		public virtual bool exportMxFile(string mxFileXML, System.IO.Stream @out)
		{
			try
			{
				ZipOutputStream zip = new ZipOutputStream(@out);

				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();

				dbf.setFeature("http://apache.org/xml/features/nonvalidating/load-external-dtd", false);
				dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
				dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
				dbf.ExpandEntityReferences = false;
				dbf.XIncludeAware = false;

				DocumentBuilder docBuilder = dbf.newDocumentBuilder();

				LinkedHashMap<string, Document> pages = new LinkedHashMap<string, Document>();
				LinkedHashMap<string, ModelExtAttrib> modelsAttr = new LinkedHashMap<string, ModelExtAttrib>();

				Document doc = mxXmlUtils.parseXml(mxFileXML);
				List<Element> diagrams = mxVsdxUtils.getDirectChildNamedElements(doc.DocumentElement, "diagram");
				mxCodec codec = new mxCodec();

				foreach (Element diagram in diagrams)
				{
					string diagramName = diagram.getAttribute("name");
					string diagramB64 = diagram.FirstChild.TextContent;
					sbyte[] deflated = mxBase64.decodeFast(diagramB64);
					string uriEncoded = Utils.inflate(deflated);
					string xml = URLDecoder.decode(uriEncoded, Utils.CHARSET_FOR_URL_ENCODING);
					Element modelNode = mxXmlUtils.parseXml(xml).DocumentElement;
					ModelExtAttrib modelAttrib = getModelExtAttributes(modelNode);
					mxGraphModel model = (mxGraphModel) codec.decode(modelNode);
					pages.put(diagramName, convertMxModel2Page(docBuilder, model, diagramName, modelAttrib));
					modelsAttr.put(diagramName, modelAttrib);
				}

				createVsdxSkeleton(zip, pages.size());

				addPagesXML(docBuilder, zip, pages, modelsAttr);

				zip.close();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return false;
			}
		}

		private double? getDouble(string val)
		{
			try
			{
				if (!string.ReferenceEquals(val, null) && val.Length > 0)
				{
					return Convert.ToDouble(val);
				}
			}
			catch (Exception)
			{
				//fallback to null
			}
			return null;
		}

		private ModelExtAttrib getModelExtAttributes(Element modelNode)
		{
			ModelExtAttrib attr = new ModelExtAttrib();

			try
			{
				string grid = modelNode.getAttribute("grid");
				if (grid.Length > 0)
				{
					attr.GridEnabled = "1".Equals(grid);
				}

				double? gridSize = getDouble(modelNode.getAttribute("gridSize"));
				if (gridSize != null)
				{
					attr.GridSize = gridSize.Value;
				}

				string guides = modelNode.getAttribute("guides");
				if (guides.Length > 0)
				{
					attr.GuidesEnabled = "1".Equals(guides);
				}

				string page = modelNode.getAttribute("page");
				if (page.Length > 0)
				{
					attr.PageVisible = "1".Equals(page);
				}

				double? pageScale = getDouble(modelNode.getAttribute("pageScale"));
				if (pageScale != null)
				{
					attr.PageScale = pageScale.Value;
				}

				double? pageWidth = getDouble(modelNode.getAttribute("pageWidth"));
				if (pageWidth != null)
				{
					attr.PageWidth = pageWidth.Value;
				}

				double? pageHeight = getDouble(modelNode.getAttribute("pageHeight"));
				if (pageHeight != null)
				{
					attr.PageHeight = pageHeight.Value;
				}

				string background = modelNode.getAttribute("background");
				if (background.Length > 0)
				{
					attr.BackgroundClr = background;
				}

				string math = modelNode.getAttribute("math");
				if (math.Length > 0)
				{
					attr.MathEnabled = "1".Equals(math);
				}

				string shadow = modelNode.getAttribute("shadow");
				if (shadow.Length > 0)
				{
					attr.ShadowVisible = "1".Equals(shadow);
				}

				//these most probably not needed in vsdx
				string tooltips = modelNode.getAttribute("tooltips");
				if (tooltips.Length > 0)
				{
					attr.Tooltips = "1".Equals(tooltips);
				}

				string connect = modelNode.getAttribute("connect");
				if (connect.Length > 0)
				{
					attr.Connect = "1".Equals(connect);
				}

				string arrows = modelNode.getAttribute("arrows");
				if (arrows.Length > 0)
				{
					attr.Arrows = "1".Equals(arrows);
				}

				string fold = modelNode.getAttribute("fold");
				if (fold.Length > 0)
				{
					attr.FoldingEnabled = "1".Equals(fold);
				}
			}
			catch (Exception)
			{
				//nothing, fallback to defaults
			}
			return attr;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void addPagesXML(javax.xml.parsers.DocumentBuilder docBuilder, java.util.zip.ZipOutputStream zip, java.util.LinkedHashMap<String, org.w3c.dom.Document> pages, java.util.LinkedHashMap<String, ModelExtAttrib> modelsAttr) throws javax.xml.transform.TransformerFactoryConfigurationError, javax.xml.transform.TransformerException, java.io.IOException
		private void addPagesXML(DocumentBuilder docBuilder, ZipOutputStream zip, LinkedHashMap<string, Document> pages, LinkedHashMap<string, ModelExtAttrib> modelsAttr)
		{
			Document pagesXmlDoc = docBuilder.newDocument();
			Document pagesRelsXmlDoc = docBuilder.newDocument();

			Element pagesRoot = pagesXmlDoc.createElement("Pages");
			pagesRoot.setAttribute("xmlns", XMLNS);
			pagesRoot.setAttribute("xmlns:r", XMLNS_R);
			pagesRoot.setAttribute("xml:space", XML_SPACE);

			Element pagesRelsRoot = pagesRelsXmlDoc.createElement("Relationships");
			pagesRelsRoot.setAttribute("xmlns", RELS_XMLNS);

			int i = 1;
			foreach (KeyValuePair<string, Document> pair in pages.entrySet())
			{
				string name = pair.Key;
				string pageName = "page" + i + ".xml";

				Element pageE = pagesXmlDoc.createElement("Page");
				pageE.setAttribute("ID", (i - 1).ToString());
				pageE.setAttribute("NameU", name);
				pageE.setAttribute("Name", name);

				Element pageSheet = pagesXmlDoc.createElement("PageSheet");

				ModelExtAttrib modelAttr = modelsAttr.get(name);

				pageSheet.appendChild(createCellElemScaled("PageWidth", modelAttr.PageWidth, pagesXmlDoc));
				pageSheet.appendChild(createCellElemScaled("PageHeight", modelAttr.PageHeight, pagesXmlDoc));
				pageSheet.appendChild(createCellElem("PageScale", modelAttr.PageScale, pagesXmlDoc));
				pageSheet.appendChild(createCellElem("DrawingScale", 1, pagesXmlDoc));

				Element relE = pagesXmlDoc.createElement("Rel");
				relE.setAttribute("r:id", "rId" + i);

				pageE.appendChild(pageSheet);
				pageE.appendChild(relE);
				pagesRoot.appendChild(pageE);

				Element relationship = pagesRelsXmlDoc.createElement("Relationship");
				relationship.setAttribute("Id", "rId" + i);
				relationship.setAttribute("Type", PAGES_TYPE);
				relationship.setAttribute("Target", pageName);
				pagesRelsRoot.appendChild(relationship);

				//Note:Each page rels is created with the skeleton as they are constants

				//write the page docs
				Document xmlDoc = pair.Value;
				writeXmlDoc2Zip(zip, VISIO_PAGES + pageName, xmlDoc);
				i++;
			}

			pagesXmlDoc.appendChild(pagesRoot);
			pagesRelsXmlDoc.appendChild(pagesRelsRoot);
			writeXmlDoc2Zip(zip, VISIO_PAGES + "pages.xml", pagesXmlDoc);
			writeXmlDoc2Zip(zip, VISIO_PAGES + "_rels/pages.xml.rels", pagesRelsXmlDoc);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeXmlDoc2Zip(java.util.zip.ZipOutputStream zip, String name, org.w3c.dom.Document xmlDoc) throws java.io.IOException, java.io.UnsupportedEncodingException, javax.xml.transform.TransformerConfigurationException, javax.xml.transform.TransformerFactoryConfigurationError, javax.xml.transform.TransformerException
		private void writeXmlDoc2Zip(ZipOutputStream zip, string name, Document xmlDoc)
		{
			zip.putNextEntry(new ZipEntry(name));

			Source source = new DOMSource(xmlDoc);
			StreamResult result = new StreamResult(new System.IO.StreamWriter(zip, VSDX_ENC));
			Transformer xformer = TransformerFactory.newInstance().newTransformer();
			xformer.transform(source, result);
		}

		private Element createCellElemScaled(string name, double val, Document xmlDoc)
		{
			return createCellElem(name, val / mxVsdxUtils.conversionFactor, xmlDoc);
		}

		private Element createCellElem(string name, double val, Document xmlDoc)
		{
			Element cell = xmlDoc.createElement("Cell");
			cell.setAttribute("N", name);
			cell.setAttribute("V", val.ToString());
			return cell;
		}

		protected internal virtual Element createRow(string type, string index, double x, double y, Document xmlDoc)
		{
			Element row = xmlDoc.createElement("Row");
			row.setAttribute("T", type);
			row.setAttribute("IX", index);
			row.appendChild(createCellElem("X", x, xmlDoc));
			row.appendChild(createCellElem("Y", y, xmlDoc));
			return row;
		}

		private Element createGeoElem(mxCell cell, Document xmlDoc)
		{
			Element geoSec = xmlDoc.createElement("Section");

			geoSec.setAttribute("N", "Geometry");
			geoSec.setAttribute("IX", "0");

			//now just create a rectangle
			geoSec.appendChild(createRow("MoveTo", "1", 0, 0, xmlDoc));
			geoSec.appendChild(createRow("RelLineTo", "2", 1, 0, xmlDoc));
			geoSec.appendChild(createRow("RelLineTo", "3", 1, 1, xmlDoc));
			geoSec.appendChild(createRow("RelLineTo", "4", 0, 1, xmlDoc));
			geoSec.appendChild(createRow("RelLineTo", "5", 0, 0, xmlDoc));

			return geoSec;
		}

		private Element convertMxCell2Shape(mxCell cell, mxGraphModel model, Document xmlDoc, double parentHeight)
		{
			mxGeometry geo = model.getGeometry(cell);

			if (geo != null)
			{
				Element shape = xmlDoc.createElement("Shape");

				shape.setAttribute("ID", cell.Id);
				shape.setAttribute("NameU", "NotYet");
				shape.setAttribute("Type", "Shape");

				double hw = geo.Width / 2, hh = geo.Height / 2;

				shape.appendChild(createCellElemScaled("PinX", geo.X + hw, xmlDoc));
				shape.appendChild(createCellElemScaled("PinY", parentHeight - geo.Y - hh, xmlDoc));
				shape.appendChild(createCellElemScaled("Width", geo.Width, xmlDoc));
				shape.appendChild(createCellElemScaled("Height", geo.Height, xmlDoc));
				shape.appendChild(createCellElemScaled("LocPinX", hw, xmlDoc));
				shape.appendChild(createCellElemScaled("LocPinY", hh, xmlDoc));

				shape.appendChild(createGeoElem(cell, xmlDoc));

				return shape;
			}

			return null;
		}

		private Document convertMxModel2Page(DocumentBuilder docBuilder, mxGraphModel model, string name, ModelExtAttrib modelAttrib)
		{
			Document xmlDoc = docBuilder.newDocument();

			Element root = xmlDoc.createElement("PageContents");
			root.setAttribute("xmlns", XMLNS);
			root.setAttribute("xmlns:r", XMLNS_R);
			root.setAttribute("xml:space", XML_SPACE);

			Element shapes = xmlDoc.createElement("Shapes");
			root.appendChild(shapes);

			mxGraph graph = new mxGraphHeadless(model);
			object defParent = graph.DefaultParent;

			foreach (object c in model.Cells.Values)
			{
				//top-most cells
				if (model.getParent(c) == defParent)
				{
					Element shape = convertMxCell2Shape((mxCell) c, model, xmlDoc, modelAttrib.PageHeight);

					if (shape != null)
					{
						shapes.appendChild(shape);
					}
				}
			}

			xmlDoc.appendChild(root);
			return xmlDoc;
		}

	//	public static void main(String[] args) 
	//	{
	//		try {
	//			String mxFileXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><mxfile><diagram name=\"Page-1\">7ZZbk5owFMc/DTPtixPC/VG87G697Ky43t4CREgFwkAE3E/fIFGhzs5uO+1Lpzij8sv/5JCT849KyiCuHjKUhjPq40iCwK8kZShBKMtA5x81OTVEM60GBBnxhegGHPKGBQSCHomP846QURoxknahR5MEe6zDUJbRsivb06ibNUUBvgOOh6J7uiY+C6/rAreBR0yCUKQ2NTHgIu8QZPSYiHwSVPbnqxmO0XWuBuQh8mnZoPMUykgCfan+DiRlkFHKOugyEFcDHNU1v5SzyT/+nPi6vAwn7DfioXh4droULMiQT/hkAxrR7IwURTFN1ZUUe0+iqMVVzdV1lfMi96unIYcyv4mQiyP7Wr2LPqEJz2DzMqV1ppzhxCPRl2I1tod0Fowey/miohPzZbPmTx/Pl6vTOtqGO2Tt8Xg6cR/SiRcXnvL81lfdRRrow+EgBIN0ZPK+cRf549bRBugtf3aG2/jbqPy+nMtamRqmBystIX1jpmTqCmhbPJKg7a/mxgId1xqkQXZUx0cHLYnMVnHKk2+exjO222/lV6Iux/bhwANesgV/n9q41KitR88HDS4T6lS76fYVeX6xKGweaShF/4XrNpa10wyiFRs9n0RjL4cJ/lqvnmX0gFsl5IZyFViPpMgjSTDF+3obQQ9A0Llk8yayKWM0/lC2EF39jipkcSS2TAQsafq+/LyZuG4b3kh2GRKGHR5W72XJj47r4i4uk3sW0Di9b1DRswXOGK5a6L5hb237gGmMWXbi95cQaDTznC4HjtzTrNYl0pQ312vAbFjYMrxhGj1VnDfirAmuyd41Ut2ijZc+ZTPlQ5sJe/zkMAMgXzXaDoP/HfZrDrPOr3/RYaCnK3/TYYYs92DHYzqwuh67s1jtQv3eZFDXe8YfMtmZt35Oha71/+XMfwA=</diagram><diagram name=\"Page-2\">nZTbkqIwEIafhqrdGwuIEbwUlJldD1Mjjqe7CAGyBkKFCDhPv0HigbWmdmtzofD132nS6W4NuGn9wlGezFmIqWbqYa2BsWaahqEP5F9Dzi2B9rAFMSehEt2BTz6xgrqiJxLioiMUjFFB8i4MWJbhQHQY4pxVXVnEaDdqjmL8BPwA0We6IaFIbufS74ZXTOJEhbahMhxQcIw5O2UqnmaC6LJac4pue7WgSFDIqhZdtgATTR9pzbOuAZczJjroakhrF9Mm59d0tvG9fxPfjsdxJv7D31QfL87XhMUchURu5jLKuEQZy6TFiQilVyRTcQBRKFMBnLII6x9jCQ35QtEBU+eWuD+2kBnKmyCFwFlA6Ldy7TljNo8nr9ViWbOp/b7dyA9PF6v1eUN3yR4NI+zNpoeXfBqkZQDePkf9wzKPB+Oxm+huPrFlyRyWxevOhy76LN788S79Oal+rRYGrHLLDswaZmRkzQHvr3W4wxPNdML1wlqi0waaLOanvnfy0YoYYp3mMvj2hzcX+2hnfJD+ynOOR+nwzpfyd+bgCjJnQN+O0FxlzK/3s90HCsJyWTrS0wLl6F3qtsPhHloElttBMaVeUJgZ/t6cXnB2xA8pBBjacNhYchSQLJ7hqLlBvaebemcZ9l3kMCFY+lfZUhX0F6pEpFRdmXJYsfxr+eUycVMxsoacKiEC+9KtuctKTo3b4a4NZvSgbUGJn4tT1WuJucD1A3ou1nvJvmCWYsHP8v3qYlk9u91JjSUDmj0wfFjKXN17HkDYU8MseWh4s2/0gJo3atbEt4BfNlJTp20vddtc8odOV7qH0XrhvwE=</diagram><diagram name=\"Page-3\">7VhZc6pIFP41Vs08jNWs4qMgCmJwwS2+ITTQgjQiCvjrp1GMEuNNcm9y52EuVZb0x1n6LN9hqTHSJuvGZuQ9YRsGNRrYWY1p12iaEliG/BVIfkaaAn0G3BjZpdAVMNARliAo0T2y4a4imGAcJCiqghYOQ2glFcyMY5xWxRwcVL1GpgvvAMMyg3t0juzEK1EKgOsFBSLXK10LXHlhZVq+G+N9WPqr0YxzOs6XN+aLrTOw80wbp2foZIKRa6BVK85BjZFijJMKdLmwySQYFDm/pPPsv/Mx4ZfwYhgmP6FfFnOX5JeEubFpI2JMwgGOCRTikFwRHRQEr6DDzs7UNllTZBGYKxiILzl7JbpLYuzD12BkWih0+9Apdv4PXQdNtnlzCFcZEScJ3rwnNS7L+EOhCY4einjJJijjOUUBiyyRvImphxJoEANFklLClJeYLk1F1Rs8Ae/LUVboAOMEZjfQfXmuRepCvIFJnJN1qUIas87dbrdxNluyssHXqdtYyrKm147naFAvdbybdqd5UGe4km4l1dwX7w/7iOBlK32oy5if6TJCOAbwvMXcthr9fqt5ZlQ42SUwtFDw1/Q4o/s7Tu3L3kyfqLiPQu0I2OQI0iiftSg7kKmnXfyUAzfJUSruAkNfzOZ6aA9zRPUniMTa6wuU6pITtSf7Mh5pZMrRnXZ3LIlTw3GXrGioNVocj4zplklpNJCsYxtsMm2kzAiuAlqX2rrVOlLbp+d11JXNiewfFQpwviqrvanbUoW95+XeSBLVVbZlW3/fkYakg6ZYpmH9fubc0OIDJPoy5tC/xpySGjxXBwK4HtRHqUExoE4J300N9suowXyaGsms329jNZWHualsQjP2XBIvqUQHDccuBjN/O98e1kwnsRR52PUTet7dQMr3hC4fL8brjdhLRoqQdri9gnbdrjZfxPm2pVtzLV2uWvthDxvEmBkO+NVhTM6W5Ne0h5OQ2crrcGxIiOKDrDcf6a1+vt5nKlZkZ75Ie67U7LmtZicKPQ1trXgiA8VsRLLdWOqgZdmxRvbZcxqdwTO9MfZtNQol1ul1lCRbehvCJsjP06bRnhdVojvhYDO2nhcsyIxlkOtPPmg6WVM9tqC0h+CoUz0rRkvF97I5Bm1nciDWQ01YeAtrIg18q6kc+hPWN1WjGAKr7uk2z7T/kPSXSVqqNKk6Rd+QlLkjKcMIdZb/r25g3JexlP80S8OlMpax6spy0c2eb1miMuMTco6HvaC7lHr5DBzIHBFNzbfC+cZkomXjSE8XAfb4xmDuSl24Fhl77tOcMBEMaGlcEoN2Yyp2MlXr7RXTHTD6lIYi5/e6OmWtbDZlc07bESeaun1m9/zAeOLMLGL6sqt3aHmBls6JgiwEqUYdJW2BRlZf6SM3ygzlyDUODkmA2PQKNUXacf2Yo/S+LR/lddQZgaHZJQ2rdeJ1pBupbw9U7KwF29zr6WxaBOfM9i4eNzrPlvKHZ/8XnvHf+fzMVx6ZKZaq08JNufjvDq7xXcFRdPVtgOGZOsf8ztCEu/n4MvAapJ1hePkyQB7GHg/A6yt2QQszTlrFB4CbF06SJkRe7VsBckMCb5BtF/4K+xfRVYAt/615QTU5brW6mxegDm4JUTwgvjUwHokVu6yE9mqGPNCDtguNMlllcI/mygML1cHyeojQp6xnKFmcLZSr58pqCGNE+gDGpRHSlXF+o1Asn6vLqsqjdt7hfWzBCqNJnlyYVHhQ5ODzLR/DwEzQofp1563GfmTvYpU0jJnf6EYYhcnufc3rroaFxk+MmLcY9yEPr3l+st/8Wpun2cF+yOZpSpzT+J7g1W3phvQUuLlZChTN0tVaYsfZkY758UbKQfXSHx8fZSf85qPcxdL1K+gJ/xc=</diagram></mxfile>"; 
	//					//"<?xml version=\"1.0\" encoding=\"UTF-8\"?><mxfile><diagram name=\"Page-1\">7VhZk6JIEP41Ruw+jFEUp4+CKFfjgVf7hlAcghQiCvjrt1C7G7XdmZ7Y2d2HIcKIyqzKrDy+ryhs0dK2HGR2GrxgF8UtCPZ5FaMW3WtB6CLPPsT5t7MKEkWLllu0lGGcX0bbUkJxbRS6FwtQL4L9J7PUeRakdoaS/EcM4KPB1UcjSj+z3ZDMSzjGGVElOCEzohfG8Z3quHdLtUdkigixvUaxaDuRn+FD4t4t3ecZjtC9MrWdMPEN5NXBfINt0GE6jUf4WCPiPMfb762ahH7wXVdTnD5dEuTb+JrPOQtUFw4QqQjCHFnEQV2kgvT3PadF6OZBbdPmOaK8lPOIshyVjQpf+zFAeIvyrCJL7P213O+q2jBA1xTqbnGgTbMXfXHdhUgsBG3+or3uAKHQZpupXKeryzTPtalmnrCJETK4wuRzyNCPkIE/A5kWpGnAcQ7dxA38Pm4CO6032ecoccL4j9lpDo09qxpyMDenKjbCRD8BJj+BIq3mXcqNZepln71UwM+rsBD3sWUu5wszcUdVSBnTkOSrGQKl+mSganIk47F+QkToDSaSOLM8f8WIltqC4mRszXZ0AcOh5Jx6YFvqY2VO9CqAptQzne6J2r28btKBbE/l6KRQgI1UWdVmflcVDkFQBWNJVNfljun++cAAUg5IMTTv/Ps0aGD8Bxjx39OAokGbEv6OBlecc2wbCODjob6Cc+ZX4pz+Ms7zuWH0sFrIo8pWtomdBT4pASlrPxxNfAzm0W6xO27ofu4o8mgQ5XAx2CIqCoQBly0nm62o5WNFKPrsQQn3g4G+WGbVrms6C71YrbuHkYYt4sxOhtz6OCGjFfl13NE0oXfyJplYUkhxcaktxmbXqDaHUsWK7C2WheZLHc3vdvppEujhzsmmMlBsPpVdfmWCruNmOolT8/j+8BVurUNPTROJ8bS+kperYEuogbhF0bF6i7o1sJ8MtxPndcmA0lrFlfkSgY5XdtRTF0kHBE4mpTlZuFKioFxg0POmR+I90YVlsHSm0jByOsrRmDKRrVo1o9eDou4B3fvNuH/+xUPTQpvhbt48HapNwQbl6K9Qjv2VlOO+TLlkpUxkrPqyXEMziBxHVOZcTsZ4pMWDlaRVc3Ak54Ro65GTLLY2na74E5wtYxxw/HDhSwO0EWl3EUFWmAoWcnQ2z0CPn4n9UtW1g2L7Q9qcQSSykTYwKWftMgVTsfqebKKru1fmwA2tF9YuU9qQfbMP5WW48s58YhAodOok6ctw7BiKEfppaSknlj96pABiJ6jNFGnPGhlLmYYrn+RN2h+DkT0g6NP72SY1rSJyhyr2NoJrH8xiPquT8+YHH0/4/quj/CbN/5803NMr/E9n0bhFcjcvVIqh2lBoVJr7SqT8L4uUgnwb3ERKc3Sbpb8aKXJ9dBPYW+jC8+8kfMgcdNOKxmH1fvrwBFsoca3wVM+Qe8nz04hQzDs/Z4zaWd7NMlw0PrJIsULHjrtx6CdEvQ1dt96v9v+2dB1jJ/qMvFSHZdfrB/KCNmgCsL4rfcbeZ8vqKG9SuyP0E7u63Na1WNfknpH8iYdblt8zGp6rXob58uLhKr3eSCOUhQRRKLs6IQ3OqoZBLb7eircm556TCvgob+D8RxGcodjOw2MTdmdT0km7aZXiMMn3Dc+jWvHzZH1w8Mihs4POF43OxGPuyHZJ5s7FW2bY8/akdE0npNagcaILFGQev1HfS/sZpYn48S/KZfnHfzC0/Bc=</diagram><diagram name=\"Page-2\">jVPbcpswEP0aZtqHMiBZBj+am5P6NsHENn6TQVxqgRiQAefrKwxOaDtpqwcNe3bPrpY9K0EzaxclLpI1CwmVgFLxGyUStCQAQhLhK+Xf7hAQgARtCZolY7z/ylqT0I6Uhj1D6YKA84lXvXuVApck5/9DAH8ShhyjV8YlDlPhNxllpYBylguPEaWUPiAJwDOMwigSeF2F7bMlQFUYFJ8JNXBwiUt2zcPfUlQJLroiFSd5kNIv9d4xLLaO7adm47Zsqb8cD+Lx2cbb3w7UT054FhFntTwvimWQ1QHcvs0nZ7eIp5ZlJopZ2HpOgrNbPfk7ZOK3aruz/Oy73fzwNipqCk0PQIvydK6tYTnZK8gntgSMcL/RXHw9IMDi8jpxrjvspSrfZ4Uofnx21vwU+eprOvEc43IRhJfSFffKIA1ixpRuLwh4Odu1p5X/ioOwdmtDMDVYz19E3HE2OyEtRfVxWi2pE1QgJ1+77nnJLmT0CyFBOpp1ngIHaR6vSNQNRZEVoPxyVP0jyGCcs+yfYW4aJ39JlvCMDiMbCB4rPg+/D5N0IlKE1SQpJztB62bZCK2/N3dIQ550eWWka0jAvbhqUnLSjvQ2qHNBWEZ4eRMhuBrE9w51xIQMbXTanagy7OFmKCMsiJA87dFHAU2T9R65DQgCMpyNjj5ekk5w/Z48zI99vPtG2wztnw==</diagram><diagram name=\"Page-3\">7VVdb5swFP01SNvDkLExkMdCQtvloypJ8/VmwAEWgxE4QPrraxLaknXtPrSnaSAh7rn3+HLxObKCnLS5LkgeT3lImQJBKY6MKmioQBjSHTkw8eUEQQkoaKQgp+BcnN/SxqGsJSXhmQHaIui+k9VOWZCTgmbiVwjwLaFbo/eVUUHCROYdznhxghBClqX7CrJ3CWM9XMe+YegSr8qwuR1KUJMBIz5lNgn2UcEPWfhcn/FMdrDLmORtp1LQLEjYp2rp2kM+jUY39cxr+Ni6X6/kBOlssTyu2CbeksGOupOxf52Pg7QK0N3jle57eWQMh04MnHxkZTTwvfJmM8cOeSzv5sNN+nVUf1vMNFznphXABmfJlTlFhb4EeENHCrTD5cz0yGGFIY+Kg+4e5mSRaGKZ5rL5+tadiu1uoz0k+sK193tJuC88+ZzYtMbcNtjdHsNFxufNdrJ5IEFYeZUtmSaqru5l3Xow2GIzwdXaKMfMDUqY0c/t9KLge9r7hRBiH8E2k5MgyaIJ3bU7A1QAwcWlWa9FNheCpz8t85Io/mCxWKSs27KOsOD5++WnzaStkoCM6jgRdC5p7V7WUvAvw62SUMTtuuoAYImeBVbRQtCmp7lOodeUp1QUR1lCyk6AL1BLjGk3hQxNy1T1M1x3XWSEgaWedQ+eG0DzGTl2CNBUPOhdWt8ord7OXvmxb9Cf+KbT+3eWMQEJdbNvGfjfMr9nmcHp/hctA1QD/WXLQMNQzTeWae1gXFjG1DQVXljGAINLy3zgGBm+nmKnXO8MRKMn</diagram></mxfile>"; 
	//					//"<?xml version=\"1.0\" encoding=\"UTF-8\"?><mxfile><diagram name=\"Page-1\">3ZZZs6JGFMc/za1KHnKLfXkEAUFARAWFN5Zm33f89Gm8zuROMpOkUqk8BAW7/6fP6fX85A3flcu+85pEr0NQvGFIP6wFeMOFNwwLQeSNxfDLU8Kg8IaLb/iuq+vho1QuO1BsTmn44YFsjTDpB1b0aUUarwPV8HccsD86vGJ8GmXceWEK7bu6qDsoVXUFLXyUFsUX6Q3DSZ/1QxLqUx8uigBFFFa8Io0rWA6gP4At+cLzQcF7QR539ViFv4vZJ16z9doPoArS4qfFjvy86WfhhHhyaywH6Q3jo4Gd4JQcrc4dt91nokFjNNQnCz1eS7Ng1W52L46XYaRUQ92w4cMv2dCEv5uwjFLHshSMkQGwPz1sERaje7DcSb0h3GiPE6TQifQCZJq9wBWUcLaVSXBC/ChbgH6UpzZIEEO/tC7rOpxSs+i0GuGIM9Z0WfoKmEFWBv5B5ptGzIlcTmCvqv3YC7orH+oTjNjRiklmBiz5whH0dquu7HCSLTlUZIEJNUC21u0ORy7RdOffsQMjJOKgHsx4tTSTS82M3x1NpbG7khKQWbLqW2jVgqPiXGsJdYzySpKOTIJyTmcCru8vXNcX59jrdemguvbtyvkP/VifQqE8OiRfHw0uX4z97pAHaCCwfY6t1NX3Zt3d7bk1MClG6rFh9ru2nKvcDQrK4ps41B4VHOMycZla9L3icLs7rAOBW3NR5JMRmcXGw7RoyeZg9K4SemEjD4ujc3y1b2fQEPA4SuQSw+dcZEb/yNhjEmlV7sGtlrDHzRlb/WjpqMcYN7iK8HtDcHdiKftKc0KRnpSbJ9/4WbFPkxg+WiM0DuZhCbJhVr26uw/VDg+sjrfo6SF2F9ViuSATH3xzjgKqHU/xeTUIRqDsoxLu+KwtTgEIkGoRb6mnsIdJOsAuO31YdZXRBs0FnoY6Gv2IHCDZV7xciT5BWpk952m2nZRJ6LR5Swlc+HnLBtANaeAV3CsRyjQMi+dRH7o6B58SiH1+NkvjBWkVayDaUhJ5RzDkmwtlfmvE18NQl3/Z7JzGyZ8ES4ayeCXsy+FaNz9u/sxcsCEEgbU5SQdwgW5b4s6QdF8nd0vDIXmGoaH2wZXJK8YXV14CXCCwfGLPi1R7UJdg6FbYxOtfIPoqbY4JeE0KVgkWfUfoD31+9QprNE69Ux/qlx4o8h1hP10vp/XFQ4x5J9lP88U/4xMWXgT9Pk3xf0JTuPNU4LNR9EekRiCKnvpXpGL/HUHJPOVliWSyfUWRFTv4KLmjMlwmsA7H/Q2rJGExR+vOsCEtRghVJShkZ5af+mslE1GFa9O2dIMCH4cFOmQ+A7RHojxguVJCIVPszdYzomgZFE7eDeMZ904q6HVSDHOGNd3K5hkpPccUZlpSyYovd1WOyDTCInqwVH6fYBOmooILsCprr2N+esyBBj3RJij9DUY+3vkQUrzLXAuIUN6MU1MzzXFDqG4qhSUPvdiYYLz0ZXsuc3PirVo+88OO1/jgeJ84Xs3YM+9kieBoV3F1lNvZSseuNB+x6wcuLTvOTGl3T+cPqS7yF2LpyapymqHAsNuSuRdR46w5QWs5YqpD16t0G2YH0nAVo+Cd+hTBUTbwXp3YWvsu4ySTqRlZmXtX2guOkB6T0fekaF0Iwz01RXHGATvsQLcryzxmPSaHzhKYtz21VvlmUdtfDLhdUD8jKiwf2v4AEXqBotr5QGPtbbulc2GNM9dYZ3JHXyjT4NIDS2dBm1WXi7UL7godqGWropU7D0ihzrZBM4uqp8XJlJVFnz3rjoytM7S5aJyxIjZcC0kQZT+2FouS2gXkZ+9YC6sN9lfY3c29lg46295tZRCNhRDFXQcc7WvVzoSe4O2VPOdrw8A0kk619vP3IEkQNMFG/0dIQoBRv1Hy34AiiqBf6PeJiSiJvRPfMJFgyXf8GwwSGPlOkH+Cvu0F5utL6tP26RUXF38F</diagram></mxfile>";
	//
	//			FileOutputStream fileWriter = null;
	//		    fileWriter = new FileOutputStream("g:/work/test.vsdx");
	//			
	//		    new mxVsdxExporter().exportMxFile(mxFileXML, fileWriter);
	//		} catch (Exception e) {
	//			e.printStackTrace();
	//		}
	//	}
	}

}