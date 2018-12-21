using System.Collections.Generic;

/// <summary>
/// $Id: mxTerminalChangeCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{

	using Node = System.Xml.XmlNode;

	using mxTerminalChange = mxGraph.model.mxGraphModel.mxTerminalChange;

	/// <summary>
	/// Codec for mxChildChanges. This class is created and registered
	/// dynamically at load time and used implicitely via mxCodec
	/// and the mxCodecRegistry.
	/// </summary>
	public class mxTerminalChangeCodec : mxObjectCodec
	{

		/// <summary>
		/// Constructs a new model codec.
		/// </summary>
		public mxTerminalChangeCodec() : this(new mxTerminalChange(), new string[] {"model", "previous"}, new string[] {"cell", "terminal"}, null)
		{
		}

		/// <summary>
		/// Constructs a new model codec for the given arguments.
		/// </summary>
		public mxTerminalChangeCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping) : base(template, exclude, idrefs, mapping)
		{
		}

		/* (non-Javadoc)
		 * @see mxGraphio.mxObjectCodec#afterDecode(mxGraphio.mxCodec, org.w3c.dom.Node, java.lang.Object)
		 */
		public override object afterDecode(mxCodec dec, Node node, object obj)
		{
			if (obj is mxTerminalChange)
			{
				mxTerminalChange change = (mxTerminalChange) obj;

				change.Previous = change.Terminal;
			}

			return obj;
		}

	}

}