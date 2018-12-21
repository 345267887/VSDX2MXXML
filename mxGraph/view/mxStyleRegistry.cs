using System.Collections.Generic;

/// <summary>
/// $Id: mxStyleRegistry.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.view
{


	using mxConstants = util.mxConstants;

	/// <summary>
	/// Singleton class that acts as a global converter from string to object values
	/// in a style. This is currently only used to perimeters and edge styles.
	/// </summary>
	public class mxStyleRegistry
	{

		/// <summary>
		/// Maps from strings to objects.
		/// </summary>
		protected internal static IDictionary<string, object> values = new Dictionary<string, object>();

		// Registers the known object styles
		static mxStyleRegistry()
		{
			putValue(mxConstants.EDGESTYLE_ELBOW, mxEdgeStyle.ElbowConnector);
			putValue(mxConstants.EDGESTYLE_ENTITY_RELATION, mxEdgeStyle.EntityRelation);
			putValue(mxConstants.EDGESTYLE_LOOP, mxEdgeStyle.Loop);
			putValue(mxConstants.EDGESTYLE_SIDETOSIDE, mxEdgeStyle.SideToSide);
			putValue(mxConstants.EDGESTYLE_TOPTOBOTTOM, mxEdgeStyle.TopToBottom);

			putValue(mxConstants.PERIMETER_ELLIPSE, mxPerimeter.EllipsePerimeter);
			putValue(mxConstants.PERIMETER_RECTANGLE, mxPerimeter.RectanglePerimeter);
			putValue(mxConstants.PERIMETER_RHOMBUS, mxPerimeter.RhombusPerimeter);
			putValue(mxConstants.PERIMETER_TRIANGLE, mxPerimeter.TrianglePerimeter);
		}

		/// <summary>
		/// Puts the given object into the registry under the given name.
		/// </summary>
		public static void putValue(string name, object value)
		{
			values[name] = value;
		}

		/// <summary>
		/// Returns the value associated with the given name.
		/// </summary>
		public static object getValue(string name)
		{
			return values[name];
		}

		/// <summary>
		/// Returns the name for the given value.
		/// </summary>
		public static string getName(object value)
		{
            //IEnumerator<KeyValuePair<string, object>> it = values.SetOfKeyValuePairs().GetEnumerator();

            //while (it.MoveNext())
            //{
            //	KeyValuePair<string, object> entry = it.Current;

            //	if (entry.Value == value)
            //	{
            //		return entry.Key;
            //	}
            //}

            //return null;

            foreach (var item in values)
            {
                if (item.Value == value)
                {
                    return item.Key;
                }
            }

            return null;
		}

	}

}