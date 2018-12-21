using System.Collections.Generic;

/// <summary>
/// $Id: mxGenericChangeCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{

	using Node = System.Xml.XmlNode;

	/// <summary>
	/// Codec for mxChildChanges. This class is created and registered
	/// dynamically at load time and used implicitely via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxGenericChangeCodec : mxObjectCodec
	{
		/// 
		protected internal string fieldname;

		/// <summary>
		/// Constructs a new model codec.
		/// </summary>
		public mxGenericChangeCodec(object template, string fieldname) : this(template, new string[] {"model", "previous"}, new string[] {"cell"}, null, fieldname)
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given arguments.
		/// </summary>
		public mxGenericChangeCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping, string fieldname) : base(template, exclude, idrefs, mapping)
		{

			this.fieldname = fieldname;
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#afterDecode(mxGraphio.mxCodec, org.w3c.dom.Node, java.lang.Object)
		 */
		public override object afterDecode(mxCodec dec, Node node, object obj)
		{
			object cell = getFieldValue(obj, "cell");

			if (cell is Node)
			{
				setFieldValue(obj, "cell", dec.decodeCell((Node) cell, false));
			}

			setFieldValue(obj, "previous", getFieldValue(obj, fieldname));

			return obj;
		}

	}

}