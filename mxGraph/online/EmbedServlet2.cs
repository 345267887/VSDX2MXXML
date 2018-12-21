//using System;
//using System.Collections.Generic;
//using System.Text;

///// <summary>
///// $Id: EmbedServlet.java,v 1.18 2014/01/31 22:27:07 gaudenz Exp $
///// Copyright (c) 2011-2012, JGraph Ltd
///// 
///// TODO
///// 
///// We could split the static part and the stencils into two separate requests
///// in order for multiple graphs in the pages to not load the static part
///// multiple times. This is only relevant if the embed arguments are different,
///// in which case there is a problem with parsin the graph model too soon, ie.
///// before certain stencils become available.
///// 
///// Easier solution is for the user to move the embed script to after the last
///// graph in the page and merge the stencil arguments.
///// 
///// Note: The static part is roundly 105K, the stencils are much smaller in size.
///// This means if the embed function is widely used, it will make sense to factor
///// out the static part because only stencils will change between pages.
///// </summary>
//namespace mxGraphonline
//{



//	using StringEscapeUtils = org.apache.commons.lang3.StringEscapeUtils;

//	using SystemProperty = com.google.appengine.api.utils.SystemProperty;

//	/// <summary>
//	/// Servlet implementation class OpenServlet
//	/// </summary>
//	public class EmbedServlet2 : HttpServlet
//	{
//		/// 
//		private const long serialVersionUID = 1L;

//		/// 
//		protected internal static string SHAPES_PATH = "/shapes";

//		/// 
//		protected internal static string STENCIL_PATH = "/stencils";

//		/// 
//		protected internal static string lastModified = null;

//		/// 
//		protected internal Dictionary<string, string> stencils = new Dictionary<string, string>();

//		/// 
//		protected internal Dictionary<string, String[]> libraries = new Dictionary<string, String[]>();

//		/// <seealso cref= HttpServlet#HttpServlet() </seealso>
//		public EmbedServlet2()
//		{
//			if (string.ReferenceEquals(lastModified, null))
//			{
//				// Uses deployment date as lastModified header
//				string applicationVersion = SystemProperty.applicationVersion.get();
//				DateTime uploadDate = new DateTime(long.Parse(applicationVersion.Substring(applicationVersion.LastIndexOf(".", StringComparison.Ordinal) + 1)) / (2 << 27) * 1000);

//				DateFormat httpDateFormat = new SimpleDateFormat("EEE, dd MMM yyyy HH:mm:ss z", Locale.US);
//				lastModified = httpDateFormat.format(uploadDate);
//			}

//			initLibraries(libraries);
//		}

//		/// <summary>
//		/// Sets up collection of stencils
//		/// </summary>
//		public static void initLibraries(Dictionary<string, String[]> libraries)
//		{
//			libraries["mockup"] = new string[] {SHAPES_PATH + "/mockup/mxMockupButtons.js"};
//			libraries["arrows2"] = new string[] {SHAPES_PATH + "/mxArrows.js"};
//			libraries["bpmn"] = new string[] {SHAPES_PATH + "/bpmn/mxBpmnShape2.js", STENCIL_PATH + "/bpmn.xml"};
//			libraries["er"] = new string[] {SHAPES_PATH + "/er/mxER.js"};
//			libraries["ios"] = new string[] {SHAPES_PATH + "/mockup/mxMockupiOS.js"};
//			libraries["rackGeneral"] = new string[] {SHAPES_PATH + "/rack/mxRack.js", STENCIL_PATH + "/rack/general.xml"};
//			libraries["rackF5"] = new string[] {STENCIL_PATH + "/rack/f5.xml"};
//			libraries["lean_mapping"] = new string[] {SHAPES_PATH + "/mxLeanMap.js", STENCIL_PATH + "/lean_mapping.xml"};
//			libraries["basic"] = new string[] {SHAPES_PATH + "/mxBasic.js", STENCIL_PATH + "/basic.xml"};
//			libraries["ios7icons"] = new string[] {STENCIL_PATH + "/ios7/icons.xml"};
//			libraries["ios7ui"] = new string[] {SHAPES_PATH + "/ios7/mxIOS7Ui.js", STENCIL_PATH + "/ios7/misc.xml"};
//			libraries["android"] = new string[] {SHAPES_PATH + "/mxAndroid.js", STENCIL_PATH + "electrical/transmission"};
//			libraries["electrical/transmission"] = new string[] {SHAPES_PATH + "/mxElectrical.js", STENCIL_PATH + "/electrical/transmission.xml"};
//			libraries["mockup/buttons"] = new string[] {SHAPES_PATH + "/mockup/mxMockupButtons.js"};
//			libraries["mockup/containers"] = new string[] {SHAPES_PATH + "/mockup/mxMockupContainers.js"};
//			libraries["mockup/forms"] = new string[] {SHAPES_PATH + "/mockup/mxMockupForms.js"};
//			libraries["mockup/graphics"] = new string[] {SHAPES_PATH + "/mockup/mxMockupGraphics.js", STENCIL_PATH + "/mockup/misc.xml"};
//			libraries["mockup/markup"] = new string[] {SHAPES_PATH + "/mockup/mxMockupMarkup.js"};
//			libraries["mockup/misc"] = new string[] {SHAPES_PATH + "/mockup/mxMockupMisc.js", STENCIL_PATH + "/mockup/misc.xml"};
//			libraries["mockup/navigation"] = new string[] {SHAPES_PATH + "/mockup/mxMockupNavigation.js", STENCIL_PATH + "/mockup/misc.xml"};
//			libraries["mockup/text"] = new string[] {SHAPES_PATH + "/mockup/mxMockupText.js"};
//			libraries["floorplan"] = new string[] {SHAPES_PATH + "/mxFloorplan.js", STENCIL_PATH + "/floorplan.xml"};
//			libraries["bootstrap"] = new string[] {SHAPES_PATH + "/mxBootstrap.js", STENCIL_PATH + "/bootstrap.xml"};
//			libraries["gmdl"] = new string[] {SHAPES_PATH + "/mxGmdl.js", STENCIL_PATH + "/gmdl.xml"};
//			libraries["cabinets"] = new string[] {SHAPES_PATH + "/mxCabinets.js", STENCIL_PATH + "/cabinets.xml"};
//			libraries["archimate"] = new string[] {SHAPES_PATH + "/mxArchiMate.js"};
//			libraries["archimate3"] = new string[] {SHAPES_PATH + "/mxArchiMate3.js"};
//			libraries["sysml"] = new string[] {SHAPES_PATH + "/mxSysML.js"};
//			libraries["eip"] = new string[] {SHAPES_PATH + "/mxEip.js", STENCIL_PATH + "/eip.xml"};
//			libraries["networks"] = new string[] {SHAPES_PATH + "/mxNetworks.js", STENCIL_PATH + "/networks.xml"};
//			libraries["aws3d"] = new string[] {SHAPES_PATH + "/mxAWS3D.js", STENCIL_PATH + "/aws3d.xml"};
//			libraries["pid2inst"] = new string[] {SHAPES_PATH + "/pid2/mxPidInstruments.js"};
//			libraries["pid2misc"] = new string[] {SHAPES_PATH + "/pid2/mxPidMisc.js", STENCIL_PATH + "/pid/misc.xml"};
//			libraries["pid2valves"] = new string[] {SHAPES_PATH + "/pid2/mxPidValves.js"};
//			libraries["pidFlowSensors"] = new string[] {STENCIL_PATH + "/pid/flow_sensors.xml"};
//		}

//		/// <seealso cref= HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response) </seealso>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void doGet(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		protected internal virtual void doGet(HttpServletRequest request, HttpServletResponse response)
//		{
//			try
//			{
//				string qs = request.QueryString;

//				if (!string.ReferenceEquals(qs, null) && qs.Equals("stats"))
//				{
//					writeStats(response);
//				}
//				else
//				{
//					// Checks or sets last modified date of delivered content.
//					// Date comparison not needed. Only return 304 if
//					// delivered by this servlet instance.
//					string modSince = request.getHeader("If-Modified-Since");

//					if (!string.ReferenceEquals(modSince, null) && modSince.Equals(lastModified) && request.getParameter("fetch") == null)
//					{
//						response.Status = HttpServletResponse.SC_NOT_MODIFIED;
//					}
//					else
//					{
//						writeEmbedResponse(request, response);
//					}
//				}
//			}
//			catch (Exception)
//			{
//				response.Status = HttpServletResponse.SC_BAD_REQUEST;
//			}
//		}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void writeEmbedResponse(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws java.io.IOException
//		public virtual void writeEmbedResponse(HttpServletRequest request, HttpServletResponse response)
//		{
//			response.CharacterEncoding = "UTF-8";
//			response.ContentType = "application/javascript; charset=UTF-8";
//			response.setHeader("Last-Modified", lastModified);

//			System.IO.Stream @out = response.OutputStream;

//			// Creates XML for stencils
//			PrintWriter writer = new PrintWriter(@out);

//			// Writes JavaScript and adds function call with
//			// stylesheet and stencils as arguments 
//			writer.println(createEmbedJavaScript(request));
//			response.Status = HttpServletResponse.SC_OK;

//			writer.flush();
//			writer.close();
//		}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public String createEmbedJavaScript(javax.servlet.http.HttpServletRequest request) throws java.io.IOException
//		public virtual string createEmbedJavaScript(HttpServletRequest request)
//		{
//			string sparam = request.getParameter("s");
//			string dev = request.getParameter("dev");
//			StringBuilder result = new StringBuilder("[");
//			StringBuilder js = new StringBuilder("");

//			// Processes each stencil only once
//			HashSet<string> done = new HashSet<string>();

//			// Processes each lib only once
//			HashSet<string> libsLoaded = new HashSet<string>();

//			if (!string.ReferenceEquals(sparam, null))
//			{
//				string[] names = sparam.Split(";", true);

//				for (int i = 0; i < names.Length; i++)
//				{
//					if (names[i].IndexOf("..", StringComparison.Ordinal) < 0 && !done.Contains(names[i]))
//					{
//						if (names[i].Equals("*"))
//						{
//							js.Append(readXmlFile("/js/shapes.min.js", false));
//							result.Append("'" + readXmlFile("/stencils.xml", true) + "'");
//						}
//						else
//						{
//							// Checks if any JS files are associated with the library
//							// name and injects the JS into the page
//							string[] libs = libraries[names[i]];

//							if (libs != null)
//							{
//								for (int j = 0; j < libs.Length; j++)
//								{
//									if (!libsLoaded.Contains(libs[j]))
//									{
//										string tmp = stencils[libs[j]];
//										libsLoaded.Add(libs[j]);

//										if (string.ReferenceEquals(tmp, null))
//										{
//											try
//											{
//												tmp = readXmlFile(libs[j], !libs[j].ToLower().EndsWith(".js", StringComparison.Ordinal));

//												// Cache for later use
//												if (!string.ReferenceEquals(tmp, null))
//												{
//													stencils[libs[j]] = tmp;
//												}
//											}
//											catch (System.NullReferenceException)
//											{
//												// This seems possible according to access log so ignore stencil
//											}
//										}

//										if (!string.ReferenceEquals(tmp, null))
//										{
//											// TODO: Add JS to Javascript code inline. This had to be done to quickly
//											// add JS-based dynamic loading to the existing embed setup where everything
//											// dynamic is passed via function call, so an indirection via eval must be
//											// used even though the JS could be parsed directly by adding it to JS.
//											if (libs[j].ToLower().EndsWith(".js", StringComparison.Ordinal))
//											{
//												js.Append(tmp);
//											}
//											else
//											{
//												if (result.Length > 1)
//												{
//													result.Append(",");
//												}

//												result.Append("'" + tmp + "'");
//											}
//										}
//									}
//								}
//							}
//							else
//							{
//								string tmp = stencils[names[i]];

//								if (string.ReferenceEquals(tmp, null))
//								{
//									try
//									{
//										tmp = readXmlFile("/stencils/" + names[i] + ".xml", true);

//										// Cache for later use
//										if (!string.ReferenceEquals(tmp, null))
//										{
//											stencils[names[i]] = tmp;
//										}
//									}
//									catch (System.NullReferenceException)
//									{
//										// This seems possible according to access log so ignore stencil
//									}
//								}

//								if (!string.ReferenceEquals(tmp, null))
//								{
//									if (result.Length > 1)
//									{
//										result.Append(",");
//									}

//									result.Append("'" + tmp + "'");
//								}
//							}
//						}

//						done.Add(names[i]);
//					}
//				}
//			}

//			result.Append("]");

//			// LATER: Detect protocol of request in dev
//			// mode to avoid security errors
//			string proto = "https://";

//			string setCachedUrls = "";
//			string[] urls = request.getParameterValues("fetch");

//			if (urls != null)
//			{
//				HashSet<string> completed = new HashSet<string>();

//				for (int i = 0; i < urls.Length; i++)
//				{
//					try
//					{
//						// Checks if URL already fetched to avoid duplicates
//						if (!completed.Contains(urls[i]))
//						{
//							completed.Add(urls[i]);
//							URL url = new URL(urls[i]);
//							URLConnection connection = url.openConnection();
//							System.IO.MemoryStream stream = new System.IO.MemoryStream();
//							Utils.copy(connection.InputStream, stream);
//							setCachedUrls += "GraphViewer.cachedUrls['" + StringEscapeUtils.escapeEcmaScript(urls[i]) + "'] = decodeURIComponent('" + StringEscapeUtils.escapeEcmaScript(Utils.encodeURIComponent(stream.ToString("UTF-8"), Utils.CHARSET_FOR_URL_ENCODING)) + "');";
//						}
//					}
//					catch (Exception)
//					{
//						// ignore
//					}
//				}
//			}

//			// Installs a callback to load the stencils after the viewer was injected
//			return "window.onDrawioViewerLoad = function() {" + setCachedUrls + "mxStencilRegistry.parseStencilSets(" + result.ToString() + ");" + js + "GraphViewer.processElements(); };" + "var t = document.getElementsByTagName('script');" + "if (t != null && t.length > 0) {" + "var script = document.createElement('script');" + "script.type = 'text/javascript';" + "script.src = '" + proto + ((!string.ReferenceEquals(dev, null) && dev.Equals("1")) ? "test" : "www") + ".draw.io/js/viewer.min.js';" + "t[0].parentNode.appendChild(script);}";
//		}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void writeStats(javax.servlet.http.HttpServletResponse response) throws java.io.IOException
//		public virtual void writeStats(HttpServletResponse response)
//		{
//			PrintWriter writer = new PrintWriter(response.OutputStream);
//			writer.println("<html>");
//			writer.println("<body>");
//			writer.println("Deployed: " + lastModified);
//			writer.println("</body>");
//			writer.println("</html>");
//			writer.flush();
//		}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public String readXmlFile(String filename, boolean xmlContent) throws java.io.IOException
//		public virtual string readXmlFile(string filename, bool xmlContent)
//		{
//			string result = readFile(filename);

//			if (xmlContent)
//			{
//				result = result.replaceAll("'", "\\\\'").replaceAll("\t", "").replaceAll("\n", "");
//			}

//			return result;
//		}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public String readFile(String filename) throws java.io.IOException
//		public virtual string readFile(string filename)
//		{
//			System.IO.Stream @is = ServletContext.getResourceAsStream(filename);

//			return Utils.readInputStream(@is);
//		}

//	}

//}