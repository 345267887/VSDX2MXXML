using System;
using System.Collections.Generic;
using System.Text;

namespace mxGraph.util
{


	public class mxResources
	{

		/// <summary>
		/// Ordered list of the inserted resource bundles.
		/// </summary>
		protected internal static LinkedList<KeyValuePair<string,string>> bundles = new LinkedList<KeyValuePair<string,string>>();

		/// <summary>
		/// Returns the bundles.
		/// </summary>
		/// <returns> Returns the bundles. </returns>
		public static LinkedList<KeyValuePair<string, string>> Bundles
		{
			get
			{
				return bundles;
			}
			set
			{
				bundles = value;
			}
		}


		/// <summary>
		/// Adds a resource bundle. This may throw a MissingResourceException that
		/// should be handled in the calling code.
		/// </summary>
		/// <param name="basename">
		///            The basename of the resource bundle to add. </param>
		public static void add(string basename)
		{
			//bundles.AddFirst();
		}

		
		/// 
		public static string get(string key)
		{
			return get(key, null, null);
		}

		/// 
		public static string get(string key, string defaultValue)
		{
			return get(key, null, defaultValue);
		}

		/// <summary>
		/// Returns the value for the specified resource key.
		/// </summary>
		public static string get(string key, string[] @params)
		{
			return get(key, @params, null);
		}

		/// <summary>
		/// Returns the value for the specified resource key.
		/// </summary>
		public static string get(string key, string[] @params, string defaultValue)
		{
			string value = getResource(key);

			// Applies default value if required
			if (string.ReferenceEquals(value, null))
			{
				value = defaultValue;
			}

			// Replaces the placeholders with the values in the array
			if (!string.ReferenceEquals(value, null) && @params != null)
			{
				StringBuilder result = new StringBuilder();
				string index = null;

				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];

					if (c == '{')
					{
						index = "";
					}
					else if (!string.ReferenceEquals(index, null) && c == '}')
					{
						int tmp = int.Parse(index) - 1;

						if (tmp >= 0 && tmp < @params.Length)
						{
							result.Append(@params[tmp]);
						}

						index = null;
					}
					else if (!string.ReferenceEquals(index, null))
					{
						index += c;
					}
					else
					{
						result.Append(c);
					}
				}

				value = result.ToString();
			}

			return value;
		}

		/// <summary>
		/// Returns the value for <code>key</code> by searching the resource
		/// bundles in inverse order or <code>null</code> if no value can be found
		/// for <code>key</code>.
		/// </summary>
		protected internal static string getResource(string key)
		{
			IEnumerator<KeyValuePair<string, string>> it = bundles.GetEnumerator();

			while (it.MoveNext())
			{
				try
				{
					return it.Current.Value;
				}
				catch (Exception)
				{
					// continue
				}
			}

			return null;
		}

	}

}