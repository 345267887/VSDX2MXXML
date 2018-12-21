using System;
using System.Text;
using System.Web;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.online
{


	using mxPoint = mxGraph.util.mxPoint;

	/// 
	/// <summary>
	/// String/byte array encoding/manipulation utilities
	/// 
	/// </summary>
	public class Utils
	{

		/// 
		public static string CHARSET_FOR_URL_ENCODING = "ISO-8859-1";

		/// 
		protected internal const int IO_BUFFER_SIZE = 4 * 1024;

		/// <summary>
		/// Applies a standard inflate algo to the input byte array </summary>
		/// <param name="binary"> the byte array to inflate </param>
		/// <returns> the inflated String
		///  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String inflate(byte[] binary) throws java.io.IOException
		public static string inflate(byte[] binary)
		{
            //StringBuilder result = new StringBuilder();
            //System.IO.Stream @in = new InflaterInputStream(new System.IO.MemoryStream(binary), new Inflater(true));

            //while (@in.available() != 0)
            //{
            //	sbyte[] buffer = new sbyte[IO_BUFFER_SIZE];
            //	int len = @in.Read(buffer, 0, IO_BUFFER_SIZE);

            //	if (len <= 0)
            //	{
            //		break;
            //	}

            //	result.Append(StringHelperClass.NewString(buffer, 0, len));
            //}

            //@in.Close();

            return System.Text.Encoding.UTF8.GetString(binary);
		}

		/// <summary>
		/// Applies a standard deflate algo to the input String </summary>
		/// <param name="inString"> the String to deflate </param>
		/// <returns> the deflated byte array
		///  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static byte[] deflate(String inString) throws java.io.IOException
		public static byte[] deflate(string inString)
		{
			//Deflater deflater = new Deflater(Deflater.DEFAULT_COMPRESSION, true);
			//byte[] inBytes = inString.GetBytes(Encoding.UTF8);
			//deflater.Input = inBytes;

			//System.IO.MemoryStream outputStream = new System.IO.MemoryStream(inBytes.Length);
			//deflater.finish();
			//sbyte[] buffer = new sbyte[IO_BUFFER_SIZE];

			//while (!deflater.finished())
			//{
			//	int count = deflater.deflate(buffer); // returns the generated code... index
			//	outputStream.Write(buffer, 0, count);
			//}

			//outputStream.Close();
			//sbyte[] output = outputStream.toByteArray();

			return System.Text.Encoding.UTF8.GetBytes(inString);
		}

		/// <summary>
		/// Copies the input stream to the output stream using the default buffer size </summary>
		/// <param name="in"> the input stream </param>
		/// <param name="out"> the output stream </param>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(java.io.InputStream in, java.io.OutputStream out) throws java.io.IOException
		public static void copy(System.IO.Stream @in, System.IO.Stream @out)
		{
			copy(@in, @out, IO_BUFFER_SIZE);
		}

		/// <summary>
		/// Copies the input stream to the output stream using the specified buffer size </summary>
		/// <param name="in"> the input stream </param>
		/// <param name="out"> the output stream </param>
		/// <param name="bufferSize"> the buffer size to use when copying </param>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(java.io.InputStream in, java.io.OutputStream out, int bufferSize) throws java.io.IOException
		public static void copy(System.IO.Stream @in, System.IO.Stream @out, int bufferSize)
		{
			byte[] b = new byte[bufferSize];
			int read;

			while ((read = @in.Read(b, 0, b.Length))>0)
			{
				@out.Write(b, 0, read);
			}
		}

		/// <summary>
		/// Reads an input stream and returns the result as a String </summary>
		/// <param name="stream"> the input stream to read </param>
		/// <returns> a String representation of the input stream </returns>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String readInputStream(java.io.InputStream stream) throws java.io.IOException
		public static string readInputStream(System.IO.Stream stream)
		{
			System.IO.StreamReader reader = new System.IO.StreamReader(stream);
			StringBuilder result = new StringBuilder();
			string tmp = reader.ReadLine();

			while (!string.ReferenceEquals(tmp, null))
			{
				result.Append(tmp + "\n");
				tmp = reader.ReadLine();
			}

			reader.Close();

			return result.ToString();
		}

		/// <summary>
		/// Encodes the passed String as UTF-8 using an algorithm that's compatible
		/// with JavaScript's <code>encodeURIComponent</code> function. Returns
		/// <code>null</code> if the String is <code>null</code>.
		/// </summary>
		/// <param name="s"> The String to be encoded </param>
		/// <param name="charset"> the character set to base the encoding on </param>
		/// <returns> the encoded String </returns>
		public static string encodeURIComponent(string s, string charset)
		{
			if (string.ReferenceEquals(s, null))
			{
				return null;
			}
			else
			{
				string result;

				try
				{
                    //result = URLEncoder.encode(s, charset).replaceAll("\\+", "%20").replaceAll("\\%21", "!").replaceAll("\\%27", "'").replaceAll("\\%28", "(").replaceAll("\\%29", ")").replaceAll("\\%7E", "~");

                    
                    result = HttpUtility.UrlEncode(s,System.Text.Encoding.GetEncoding(charset)).Replace("\\+", "%20").Replace("\\%21", "!").Replace("\\%27", "'").Replace("\\%28", "(").Replace("\\%29", ")").Replace("\\%7E", "~");
                }
				catch (Exception)
				{
					// This exception should never occur
					result = s;
				}

				return result;
			}
		}

		/// <summary>
		/// Rotates the given point by the given cos and sin.
		/// </summary>
		public static mxPoint getRotatedPoint(mxPoint pt, double cos, double sin, mxPoint c)
		{
			double x = pt.X - c.X;
			double y = pt.Y - c.Y;

			double x1 = x * cos - y * sin;
			double y1 = y * cos + x * sin;

			return new mxPoint(x1 + c.X, y1 + c.Y);
		}

	}

}