//using System;
//using System.Collections.Generic;

///// <summary>
///// $Id: OpenServlet.java,v 1.12 2013/10/16 12:31:25 david Exp $
///// Copyright (c) 2011-2012, JGraph Ltd
///// </summary>
//namespace mxGraphonline
//{



//	using FileItemIterator = org.apache.commons.fileupload.FileItemIterator;
//	using FileItemStream = org.apache.commons.fileupload.FileItemStream;
//	using ServletFileUpload = org.apache.commons.fileupload.servlet.ServletFileUpload;
//	using Streams = org.apache.commons.fileupload.util.Streams;
//	using StringEscapeUtils = org.apache.commons.lang3.StringEscapeUtils;

//	using mxCodec = mxGraphio.mxCodec;
//	using mxGraphMlCodec = mxGraphio.mxGraphMlCodec;
//	using mxVsdxCodec = mxGraphio.mxVsdxCodec;
//	using mxVssxCodec = mxGraphio.mxVssxCodec;
//	using GliffyDiagramConverter = mxGraphio.gliffy.importer.GliffyDiagramConverter;
//	using mxXmlUtils = util.mxXmlUtils;
//	using mxGraph = mxGraphview.mxGraph;
//	using mxGraphHeadless = mxGraphview.mxGraphHeadless;

//	/// <summary>
//	/// Servlet implementation class OpenServlet
//	/// </summary>
//	public class OpenServlet : HttpServlet
//	{
//		/// 
//		private const long serialVersionUID = 1L;

//		/// <summary>
//		/// Global switch to enabled VSDX support.
//		/// </summary>
//		public static bool ENABLE_VSDX_SUPPORT = true;

//		/// <summary>
//		/// Global switch to enabled VSSX support.
//		/// </summary>
//		public static bool ENABLE_VSSX_SUPPORT = true;

//		/// <summary>
//		/// Global switch to enabled Gliffy support.
//		/// </summary>
//		public static bool ENABLE_GLIFFY_SUPPORT = true;

//		/// <summary>
//		/// Global switch to enabled GraphML support.
//		/// </summary>
//		public static bool ENABLE_GRAPHML_SUPPORT = true;

//		/// 
//		public const int PNG_CHUNK_ZTXT = 2052348020;

//		/// 
//		public const int PNG_CHUNK_IEND = 1229278788;

//		/// 
//		protected internal static string gliffyRegex = "(?s).*\"contentType\":\\s*\"application/gliffy\\+json\".*";

//		/// 
//		protected internal static string graphMlRegex = "(?s).*<graphml xmlns=\".*";

//		/// <seealso cref= HttpServlet#HttpServlet() </seealso>
//		public OpenServlet() : base()
//		{
//		}

//		/// <seealso cref= HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response) </seealso>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void doPost(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		protected internal virtual void doPost(HttpServletRequest request, HttpServletResponse response)
//		{
//			PrintWriter writer = response.Writer;

//			try
//			{
//				if (request.ContentLength < Constants.MAX_REQUEST_SIZE)
//				{
//					string filename = "";
//					string format = null;
//					string upfile = null;
//					bool vsdx = false;
//					bool vssx = false;

//					ServletFileUpload upload = new ServletFileUpload();
//					FileItemIterator iterator = upload.getItemIterator(request);

//					while (iterator.hasNext())
//					{
//						FileItemStream item = iterator.next();
//						string name = item.FieldName;
//						System.IO.Stream stream = item.openStream();

//						if (item.FormField && name.Equals("format"))
//						{
//							format = Streams.asString(stream);
//						}
//						else if (name.Equals("upfile"))
//						{
//							filename = item.Name;
//							vsdx = filename.ToLower().EndsWith(".vsdx", StringComparison.Ordinal);
//							vssx = filename.ToLower().EndsWith(".vssx", StringComparison.Ordinal);

//							if (vsdx || vssx)
//							{
//								upfile = Streams.asString(stream, "ISO-8859-1");
//							}
//							else
//							{
//								upfile = Streams.asString(stream, Utils.CHARSET_FOR_URL_ENCODING);
//							}
//						}
//					}

//					if (string.ReferenceEquals(format, null))
//					{
//						format = request.getParameter("format");
//					}

//					if (string.ReferenceEquals(format, null))
//					{
//						format = "html";
//					}

//					string xml = null;

//					if (filename.ToLower().EndsWith(".png", StringComparison.Ordinal))
//					{
//						xml = extractXmlFromPng(upfile.GetBytes(Utils.CHARSET_FOR_URL_ENCODING));
//					}
//					else if (ENABLE_GRAPHML_SUPPORT && upfile.matches(graphMlRegex))
//					{
//						// Creates a graph that contains a model but does not validate
//						// since that is not needed for the model and not allowed on GAE
//						mxGraph graph = new mxGraphHeadless();

//						mxGraphMlCodec.decode(mxXmlUtils.parseXml(upfile), graph);
//						xml = mxXmlUtils.getXml((new mxCodec()).encode(graph.Model));
//					}
//					else if (ENABLE_VSDX_SUPPORT && vsdx)
//					{
//						mxVsdxCodec vdxCodec = new mxVsdxCodec();
//						xml = vdxCodec.decodeVsdx(upfile.GetBytes("ISO-8859-1"), Utils.CHARSET_FOR_URL_ENCODING);

//						// Replaces VSDX extension
//						int dot = filename.LastIndexOf('.');
//						filename = filename.Substring(0, dot + 1) + "xml";
//					}
//					else if (ENABLE_VSSX_SUPPORT && vssx)
//					{
//						mxVssxCodec vssxCodec = new mxVssxCodec();
//						xml = vssxCodec.decodeVssx(upfile.GetBytes("ISO-8859-1"), Utils.CHARSET_FOR_URL_ENCODING);

//						// Replaces VSDX extension
//						int dot = filename.LastIndexOf('.');
//						filename = filename.Substring(0, dot + 1) + "xml";
//					}
//					else if (ENABLE_GLIFFY_SUPPORT && upfile.matches(gliffyRegex))
//					{
//						GliffyDiagramConverter converter = new GliffyDiagramConverter(upfile);
//						xml = converter.GraphXml;
//					}

//					// Fallback to old data parameter
//					if (string.ReferenceEquals(xml, null))
//					{
//						xml = (string.ReferenceEquals(upfile, null)) ? request.getParameter("data") : upfile;
//					}

//					if (!format.Equals("xml"))
//					{
//						if (string.ReferenceEquals(xml, null) || xml.Length == 0)
//						{
//							writeScript(writer, "window.parent.showOpenAlert({message:window.parent.mxResources.get('invalidOrMissingFile')});");
//						}
//						else
//						{
//							// Workaround for replacement char and null byte in IE9 request
//							xml = xml.replaceAll("[\\uFFFD\\u0000]*", "");
//							writeScript(writer, "try{window.parent.setCurrentXml(decodeURIComponent('" + encodeString(xml) + "'), decodeURIComponent('" + encodeString(filename) + "'));}catch(e){window.parent.showOpenAlert({message:window.parent.mxResources.get('notAUtf8File')});}");
//						}
//					}
//					else
//					{
//						writer.println(xml);
//					}
//				}
//				else
//				{
//					response.Status = HttpServletResponse.SC_REQUEST_ENTITY_TOO_LARGE;
//					writeScript(writer, "window.parent.showOpenAlert(window.parent.mxResources.get('drawingTooLarge'));");
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//				Console.Write(e.StackTrace);
//				response.Status = HttpServletResponse.SC_NOT_FOUND;
//				writeScript(writer, "window.parent.showOpenAlert(window.parent.mxResources.get('invalidOrMissingFile'));");
//			}

//			writer.flush();
//			writer.close();
//		}

//		/// <summary>
//		/// URI encodes the given string for JavaScript.
//		/// </summary>
//		protected internal virtual string encodeString(string s)
//		{
//			return StringEscapeUtils.escapeEcmaScript(Utils.encodeURIComponent(s, Utils.CHARSET_FOR_URL_ENCODING));
//		};

//		/// <summary>
//		/// Writes the given string as a script in a HTML page to the given print writer.
//		/// </summary>
//		protected internal virtual void writeScript(PrintWriter writer, string js)
//		{
//			writer.println("<html>");
//			writer.println("<body>");
//			writer.println("<script type=\"text/javascript\">");
//			writer.println(js);
//			writer.println("</script>");
//			writer.println("</body>");
//			writer.println("</html>");
//		}

//		// NOTE: Key length must not be longer than 79 bytes (not checked)
//		protected internal virtual string extractXmlFromPng(sbyte[] data)
//		{
//			IDictionary<string, string> textChunks = decodeCompressedText(new System.IO.MemoryStream(data));

//			return (textChunks != null) ? textChunks["model"] : null;
//		}

//		/// <summary>
//		/// Decodes the zTXt chunk of the given PNG image stream.
//		/// </summary>
//		public static IDictionary<string, string> decodeCompressedText(System.IO.Stream stream)
//		{
//			IDictionary<string, string> result = new Dictionary<string, string>();

//			if (!stream.markSupported())
//			{
//				stream = new BufferedInputStream(stream);
//			}
//			DataInputStream distream = new DataInputStream(stream);

//			try
//			{
//				long magic = distream.readLong();

//				if (magic != 0x89504e470d0a1a0aL)
//				{
//					throw new Exception("PNGImageDecoder0");
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//				Console.Write(e.StackTrace);
//				throw new Exception("PNGImageDecoder1");
//			}

//			try
//			{
//				while (distream.available() > 0)
//				{
//					int length = distream.readInt();
//					int type = distream.readInt();
//					sbyte[] data = new sbyte[length];
//					distream.readFully(data);
//					distream.readInt(); // Move past the crc

//					if (type == PNG_CHUNK_IEND)
//					{
//						return null;
//					}
//					else if (type == PNG_CHUNK_ZTXT)
//					{
//						int currentIndex = 0;
//						while ((data[currentIndex++]) != 0)
//						{
//						}

//						string key = StringHelperClass.NewString(data, 0, currentIndex - 1);

//						try
//						{
//							sbyte[] bytes = Arrays.copyOfRange(data, currentIndex + 1, data.Length);
//							string value = URLDecoder.decode(Utils.inflate(bytes), Utils.CHARSET_FOR_URL_ENCODING);
//							result[key] = value;
//						}
//						catch (Exception e)
//						{
//							Console.WriteLine(e.ToString());
//							Console.Write(e.StackTrace);
//						}

//						// No need to parse the rest of the PNG
//						return result;
//					}
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//				Console.Write(e.StackTrace);
//			}

//			return null;
//		}
//	}

//}