//using System;
//using System.Collections.Generic;

//namespace mxGraphonline
//{



//	/// <summary>
//	/// Simple client-side logging servlet
//	/// </summary>
//	public class LogServlet : HttpServlet
//	{

//		/// 
//		private const long serialVersionUID = 2360583959079622105L;

////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//		private static readonly Logger log = Logger.getLogger(typeof(LogServlet).FullName);

//		internal static sbyte[] singleByteGif = new sbyte[] {0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x1, 0x0, 0x1, 0x0, unchecked((sbyte) 0x80), 0x0, 0x0, unchecked((sbyte) 0xff), unchecked((sbyte) 0xff), unchecked((sbyte) 0xff), 0x0, 0x0, 0x0, 0x2c, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x0, 0x0, 0x2, 0x2, 0x44, 0x1, 0x0, 0x3b};

//		/// <summary>
//		/// The start string of error message we want to ignore
//		/// </summary>
//		internal static string[] configArray;

//		internal static ISet<string> ignoreFilters;

//		/// <summary>
//		/// The start string of error message we want to reduce to a warning
//		/// </summary>
//		internal static string[] warningArray;

//		internal static ISet<string> warningFilters;

//		static LogServlet()
//		{
//			configArray = new string[] {"Uncaught TypeError: frames.ezLinkPreviewIFRAME.postMessage is not a function", "Uncaught TypeError: Cannot set property 'tgt' of null"};

//			ignoreFilters = new HashSet<string>(Arrays.asList(configArray));

//			warningArray = new string[] {"Uncaught Error: Client got in an error state. Call reset() to reuse it!", "Error: Client got in an error state. Call reset() to reuse it!"};

//			warningFilters = new HashSet<string>(Arrays.asList(warningArray));
//		}

//		public virtual void doGet(HttpServletRequest request, HttpServletResponse response)
//		{
//			doPost(request, response);
//		}

//		public virtual void doPost(HttpServletRequest request, HttpServletResponse response)
//		{
//			try
//			{
//				string message = request.getParameter("msg");
//				string severity = request.getParameter("severity");
//				string version = request.getParameter("v");
//				string stack = request.getParameter("stack");

//				string userAgent = request.getHeader("User-Agent");

//				if (!string.ReferenceEquals(message, null))
//				{
//					Level severityLevel = Level.CONFIG;

//					if (!string.ReferenceEquals(severity, null))
//					{
//						severityLevel = Level.parse(severity);
//					}

//					if (severityLevel.intValue() >= 1000)
//					{
//						// Tidy up severes
//						message = string.ReferenceEquals(message, null) ? message : URLDecoder.decode(message, "UTF-8");
//						version = string.ReferenceEquals(version, null) ? version : URLDecoder.decode(version, "UTF-8");
//						stack = string.ReferenceEquals(stack, null) ? stack : URLDecoder.decode(stack, "UTF-8");

//						severityLevel = filterClientErrors(message, userAgent);
//					}

//					message = !string.ReferenceEquals(version, null) ? message + "\nVERSION=" + version : message;
//					message = !string.ReferenceEquals(stack, null) ? message + "\n" + stack : message;

//					log.log(severityLevel, "CLIENT-LOG:" + message);
//				}

//				response.ContentType = "image/gif";
//				System.IO.Stream @out = response.OutputStream;
//				@out.Write(singleByteGif, 0, singleByteGif.Length);
//				@out.Flush();
//				@out.Close();

//				response.Status = HttpServletResponse.SC_OK;
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.Message);
//				Console.WriteLine(e.ToString());
//				Console.Write(e.StackTrace);
//			}
//		}

//		/// <summary>
//		/// Filter out known red herring client errors by reducing their severity </summary>
//		/// <param name="message"> the message thrown on the client </param>
//		/// <param name="userAgent"> the user agent string </param>
//		/// <returns> the severity to treat the message with </returns>
//		protected internal virtual Level filterClientErrors(string message, string userAgent)
//		{
//			try
//			{
//				string result = StringHelperClass.SubstringSpecial(message, message.IndexOf("clientError:", StringComparison.Ordinal) + 12, message.IndexOf(":url:", StringComparison.Ordinal));

//				if (!string.ReferenceEquals(result, null))
//				{
//					if (ignoreFilters.Contains(result))
//					{
//						return Level.CONFIG;
//					}
//					else if (warningFilters.Contains(result))
//					{
//						return Level.WARNING;
//					}
//				}
//			}
//			catch (Exception)
//			{

//			}

//			if (!string.ReferenceEquals(userAgent, null) && userAgent.Contains("compatible; MSIE 8.0;"))
//			{
//				return Level.WARNING;
//			}

//			return Level.SEVERE;
//		}
//	}

//}