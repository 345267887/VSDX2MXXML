//using System;
//using System.Text;

///// <summary>
///// Copyright (c) 2006-2017, JGraph Ltd
///// Copyright (c) 2006-2017, Gaudenz Alder
///// </summary>
//namespace mxGraphonline
//{



//	/// <summary>
//	/// Servlet implementation ProxyServlet
//	/// </summary>
////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("serial") public class GitHubServlet extends javax.servlet.http.HttpServlet
//	public class GitHubServlet : HttpServlet
//	{

//		/// <summary>
//		/// Path component under war/ to locate iconfinder_key file.
//		/// </summary>
//		public const string DEV_CLIENT_SECRET_FILE_PATH = "/WEB-INF/github_dev_client_secret";


//		/// <summary>
//		/// Path component under war/ to locate iconfinder_key file.
//		/// </summary>
//		public const string CLIENT_SECRET_FILE_PATH = "/WEB-INF/github_client_secret";

//		/// 
//		private static string DEV_CLIENT_SECRET = "be6810bd6b8d6a47c65a8b141e0dce8aaabcd4f5";

//		/// 
//		private static string CLIENT_SECRET = "6fb1cff7690db9ac066db5bbde8e3c078efdabcf";

//		/// <seealso cref= HttpServlet#HttpServlet() </seealso>
//		public GitHubServlet() : base()
//		{
//		}

//		/// <summary>
//		/// Loads the key.
//		/// </summary>
//		protected internal virtual void updateKeys()
//		{
//			if (string.ReferenceEquals(DEV_CLIENT_SECRET, null))
//			{
//				try
//				{
//					DEV_CLIENT_SECRET = Utils.readInputStream(ServletContext.getResourceAsStream(DEV_CLIENT_SECRET_FILE_PATH)).replaceAll("\n", "");
//				}
//				catch (IOException)
//				{
//					throw new Exception("Dev client secret path invalid.");
//				}
//			}

//			if (string.ReferenceEquals(CLIENT_SECRET, null))
//			{
//				try
//				{
//					CLIENT_SECRET = Utils.readInputStream(ServletContext.getResourceAsStream(CLIENT_SECRET_FILE_PATH)).replaceAll("\n", "");
//				}
//				catch (IOException)
//				{
//					throw new Exception("Client secret path invalid.");
//				}
//			}
//		}

//		/// <seealso cref= HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response) </seealso>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void doGet(javax.servlet.http.HttpServletRequest request, javax.servlet.http.HttpServletResponse response) throws javax.servlet.ServletException, java.io.IOException
//		protected internal virtual void doGet(HttpServletRequest request, HttpServletResponse response)
//		{
//			string client = request.getParameter("client_id");
//			string code = request.getParameter("code");
//			updateKeys();

//			if (!string.ReferenceEquals(client, null) && !string.ReferenceEquals(code, null))
//			{
//				string secret = client.Equals("23bc97120b9035515661") ? DEV_CLIENT_SECRET : CLIENT_SECRET;

//				string url = "https://github.com/login/oauth/access_token";
//				URL obj = new URL(url);
//				HttpURLConnection con = (HttpURLConnection) obj.openConnection();

//				con.RequestMethod = "POST";
//				con.setRequestProperty("User-Agent", "draw.io");

//				string urlParameters = "client_id=" + client + "&client_secret=" + secret + "&code=" + code;

//				// Send post request
//				con.DoOutput = true;
//				DataOutputStream wr = new DataOutputStream(con.OutputStream);
//				wr.writeBytes(urlParameters);
//				wr.flush();
//				wr.close();

//				System.IO.StreamReader @in = new System.IO.StreamReader(con.InputStream);
//				string inputLine;
//				StringBuilder res = new StringBuilder();

//				while (!string.ReferenceEquals((inputLine = @in.ReadLine()), null))
//				{
//					res.Append(inputLine);
//				}
//				@in.Close();

//				response.Status = con.ResponseCode;

//				System.IO.Stream @out = response.OutputStream;

//				// Creates XML for stencils
//				PrintWriter writer = new PrintWriter(@out);

//				// Writes JavaScript and adds function call with
//				// stylesheet and stencils as arguments 
//				writer.println(res.ToString());

//				writer.flush();
//				writer.close();
//			}
//			else
//			{
//				response.Status = HttpServletResponse.SC_BAD_REQUEST;
//			}
//		}

//	}

//}