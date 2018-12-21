//using System;

///// <summary>
///// $Id: ProxyServlet.java,v 1.4 2013/12/13 13:18:11 david Exp $
///// Copyright (c) 2011-2012, JGraph Ltd
///// </summary>
//namespace mxGraphonline
//{



//	/// <summary>
//	/// Servlet implementation ProxyServlet
//	/// </summary>
////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("serial") public class ProxyServlet extends javax.servlet.http.HttpServlet
//	public class ProxyServlet : HttpServlet
//	{

//		/// <seealso cref= HttpServlet#HttpServlet() </seealso>
//		public ProxyServlet() : base()
//		{
//		}

//		/// <seealso cref= HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response) </seealso>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void doGet(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		protected internal virtual void doGet(HttpServletRequest request, HttpServletResponse response)
//		{
//			string urlParam = request.getParameter("url");

//			if (!string.ReferenceEquals(urlParam, null))
//			{
//				request.CharacterEncoding = "UTF-8";
//				response.CharacterEncoding = "UTF-8";

//				System.IO.Stream @out = response.OutputStream;

//				try
//				{
//					URL url = new URL(urlParam);
//					URLConnection connection = url.openConnection();

//					// Status code pass-through
//					if (connection is HttpURLConnection)
//					{
//						response.Status = ((HttpURLConnection) connection).ResponseCode;
//					}

//					if (connection != null)
//					{
//						response.ContentType = connection.ContentType;
//						Utils.copy(connection.InputStream, @out);
//					}

//					@out.Flush();
//					@out.Close();
//				}
//				catch (Exception e)
//				{
//					response.Status = HttpServletResponse.SC_INTERNAL_SERVER_ERROR;
//					Console.WriteLine(e.ToString());
//					Console.Write(e.StackTrace);
//				}
//			}
//			else
//			{
//				response.Status = HttpServletResponse.SC_BAD_REQUEST;
//			}
//		}

//	}

//}