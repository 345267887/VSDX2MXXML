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
////ORIGINAL LINE: @SuppressWarnings("serial") public class ImgurRedirectServlet extends javax.servlet.http.HttpServlet
//	public class ImgurRedirectServlet : HttpServlet
//	{

//		/// <seealso cref= HttpServlet#HttpServlet() </seealso>
//		public ImgurRedirectServlet() : base()
//		{
//		}

//		/// <seealso cref= HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response) </seealso>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void doGet(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		protected internal virtual void doGet(HttpServletRequest request, HttpServletResponse response)
//		{
//			string uri = request.RequestURI;
//			int last = uri.LastIndexOf("/", StringComparison.Ordinal);

//			if (last > 0)
//			{
//				string id = uri.Substring(last + 1);
//				response.setHeader("Location", "https://www.draw.io/?chrome=0&lightbox=1&layers=1&url=http%3A%2F%2Fi.imgur.com%2F" + id + ".png" + "&edit=https%3A%2F%2Fwww.draw.io%2F%3Furl%3Dhttp%253A%252F%252Fi.imgur.com%252F" + id + ".png");
//				response.Status = HttpServletResponse.SC_TEMPORARY_REDIRECT;
//			}
//			else
//			{
//				response.Status = HttpServletResponse.SC_BAD_REQUEST;
//			}
//		}

//	}

//}