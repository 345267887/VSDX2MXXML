//using System;

//namespace mxGraphonline
//{



//	using mxBase64 = util.mxBase64;

//	/// <summary>
//	/// Servlet implementation class SaveServlet
//	/// </summary>
//	public class SaveServlet : HttpServlet
//	{
//		/// 
//		private const long serialVersionUID = 1L;

//		/// 
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//		private static readonly Logger log = Logger.getLogger(typeof(SaveServlet).FullName);

//		/// <seealso cref= HttpServlet#HttpServlet() </seealso>
//		public SaveServlet() : base()
//		{
//		}

//		/// <seealso cref= HttpServlet#doPost(HttpServletRequest request, HttpServletResponse
//		///      response) </seealso>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void doPost(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		protected internal virtual void doPost(HttpServletRequest request, HttpServletResponse response)
//		{
//			handlePost(request, response);
//		}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public static void handlePost(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		public static void handlePost(HttpServletRequest request, HttpServletResponse response)
//		{
//			if (request.ContentLength < Constants.MAX_REQUEST_SIZE)
//			{
//				long t0 = DateTimeHelperClass.CurrentUnixTimeMillis();
//				string mime = request.getParameter("mime");
//				string filename = request.getParameter("filename");
//				sbyte[] data = null;

//				// Data in data param is base64 encoded and deflated
//				string enc = request.getParameter("data");
//				string xml = null;

//				try
//				{
//					if (!string.ReferenceEquals(enc, null) && enc.Length > 0)
//					{
//						// NOTE: Simulate is used on client-side so the value is double-encoded
//						xml = Utils.inflate(mxBase64.decode(URLDecoder.decode(enc, Utils.CHARSET_FOR_URL_ENCODING).Bytes));
//					}
//					else
//					{
//						xml = request.getParameter("xml");
//					}

//					// Decoding is optional (no plain text values allowed here so %3C means encoded)
//					if (!string.ReferenceEquals(xml, null) && xml.StartsWith("%3C", StringComparison.Ordinal))
//					{
//						xml = URLDecoder.decode(xml, Utils.CHARSET_FOR_URL_ENCODING);
//					}

//					string binary = request.getParameter("binary");

//					if (!string.ReferenceEquals(binary, null) && binary.Equals("1") && !string.ReferenceEquals(xml, null) && !string.ReferenceEquals(mime, null))
//					{
//						response.Status = HttpServletResponse.SC_OK;

//						if (!string.ReferenceEquals(filename, null))
//						{
//							response.ContentType = "application/x-unknown";
//							response.setHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"; filename*=UTF-8''" + filename);
//						}
//						else if (!string.ReferenceEquals(mime, null))
//						{
//							response.ContentType = mime;
//						}

//						response.OutputStream.write(mxBase64.decodeFast(URLDecoder.decode(xml, Utils.CHARSET_FOR_URL_ENCODING)));
//					}
//					else if (!string.ReferenceEquals(mime, null) && !string.ReferenceEquals(xml, null))
//					{
//						if (!string.ReferenceEquals(xml, null))
//						{
//							data = xml.GetBytes(Utils.CHARSET_FOR_URL_ENCODING);
//						}

//						string format = request.getParameter("format");

//						if (string.ReferenceEquals(format, null))
//						{
//							format = "xml";
//						}

//						if (!string.ReferenceEquals(filename, null) && filename.Length > 0 && !filename.ToLower().EndsWith(".svg", StringComparison.Ordinal) && !filename.ToLower().EndsWith(".html", StringComparison.Ordinal) && !filename.ToLower().EndsWith(".png", StringComparison.Ordinal) && !filename.ToLower().EndsWith("." + format, StringComparison.Ordinal))
//						{
//							filename += "." + format;
//						}

//						response.Status = HttpServletResponse.SC_OK;

//						if (!string.ReferenceEquals(filename, null))
//						{
//							response.ContentType = mime;
//							response.setHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"; filename*=UTF-8''" + filename);
//						}
//						else if (mime.Equals("image/svg+xml"))
//						{
//							response.ContentType = "image/svg+xml";
//						}
//						else
//						{
//							// Required to avoid download of file
//							response.ContentType = "text/plain";
//						}

//						System.IO.Stream @out = response.OutputStream;
//						@out.Write(data, 0, data.Length);
//						@out.Close();
//					}
//					else
//					{
//						response.Status = HttpServletResponse.SC_BAD_REQUEST;
//					}
//				}
//				catch (System.ArgumentException e)
//				{
//					log.warning("Error parsing xml contents : " + xml + System.getProperty("line.separator") + "Original stack trace : " + e.Message);
//				}
//				long mem = Runtime.Runtime.totalMemory() - Runtime.Runtime.freeMemory();

//				log.fine("save: ip=" + request.RemoteAddr + " ref=\"" + request.getHeader("Referer") + "\" in=" + request.ContentLength + " enc=" + ((!string.ReferenceEquals(enc, null)) ? enc.Length : "[none]") + " xml=" + ((!string.ReferenceEquals(xml, null)) ? xml.Length : "[none]") + " dt=" + request.ContentLength + " mem=" + mem + " dt=" + (DateTimeHelperClass.CurrentUnixTimeMillis() - t0));
//			}
//			else
//			{
//				response.Status = HttpServletResponse.SC_REQUEST_ENTITY_TOO_LARGE;
//			}
//		}

//	}

//}