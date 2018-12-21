using mxGraph;
using System;

/// <summary>
/// $Id: mxCellPath.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.model
{


	/// <summary>
	/// Implements a mechanism for temporary cell Ids.
	/// </summary>
	public class mxCellPath
	{

		/// <summary>
		/// Defines the separator between the path components. Default is
		/// <code>.</code>.
		/// </summary>
		public static string PATH_SEPARATOR = ".";

		/// <summary>
		/// Creates the cell path for the given cell. The cell path is a
		/// concatenation of the indices of all cells on the (finite) path to
		/// the root, eg. "0.0.0.1".
		/// </summary>
		/// <param name="cell"> Cell whose path should be returned. </param>
		/// <returns> Returns the string that represents the path. </returns>
		public static string create(mxICell cell)
		{
			string result = "";

			if (cell != null)
			{
				mxICell parent = cell.Parent;

				while (parent != null)
				{
					int index = parent.getIndex(cell);
					result = index + mxCellPath.PATH_SEPARATOR + result;

					cell = parent;
					parent = cell.Parent;
				}
			}

			return (result.Length > 1) ? result.Substring(0, result.Length - 1) : "";
		}

		/// <summary>
		/// Returns the path for the parent of the cell represented by the given
		/// path. Returns null if the given path has no parent.
		/// </summary>
		/// <param name="path"> Path whose parent path should be returned. </param>
		public static string getParentPath(string path)
		{
			if (!string.ReferenceEquals(path, null))
			{
				int index = path.LastIndexOf(mxCellPath.PATH_SEPARATOR, StringComparison.Ordinal);

				if (index >= 0)
				{
					return path.Substring(0, index);
				}
				else if (path.Length > 0)
				{
					return "";
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the cell for the specified cell path using the given root as the
		/// root of the path.
		/// </summary>
		/// <param name="root"> Root cell of the path to be resolved. </param>
		/// <param name="path"> String that defines the path. </param>
		/// <returns> Returns the cell that is defined by the path. </returns>
		public static mxICell resolve(mxICell root, string path)
		{
			mxICell parent = root;
            string[] tokens = path.Split(Common.quote(PATH_SEPARATOR),true); //path.Split(Pattern.quote(PATH_SEPARATOR), true);

            

            for (int i = 0; i < tokens.Length; i++)
			{
				parent = parent.getChildAt(int.Parse(tokens[i]));
			}

			return parent;
		}

		/// <summary>
		/// Compares the given cell paths and returns -1 if cp1 is smaller, 0 if
		/// cp1 is equal and 1 if cp1 is greater than cp2.
		/// </summary>
		public static int compare(string cp1, string cp2)
		{
			StringTokenizer p1 = new StringTokenizer(cp1, mxCellPath.PATH_SEPARATOR);
			StringTokenizer p2 = new StringTokenizer(cp2, mxCellPath.PATH_SEPARATOR);
			int comp = 0;
			while (p1.hasMoreTokens() && p2.hasMoreTokens())
			{
				string t1 = p1.nextToken();
				string t2 = p2.nextToken();

				if (!t1.Equals(t2))
				{
					if (t1.Length == 0 || t2.Length == 0)
					{
						comp = t1.CompareTo(t2);
					}
					else
					{
                        comp = Convert.ToInt32(t1).CompareTo(Convert.ToInt32(t2));
					}

					break;
				}
			}

			// Compares path length if both paths are equal to this point
			if (comp == 0)
			{
				int t1 = p1.countTokens();
				int t2 = p2.countTokens();

				if (t1 != t2)
				{
					comp = (t1 > t2) ? 1 : -1;
				}
			}

			return comp;
		}

	}

}