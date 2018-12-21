using System.Collections.Generic;

/// <summary>
/// $Id: mxEventObject.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.util
{


	/// <summary>
	/// Base class for objects that dispatch named events.
	/// </summary>
	public class mxEventObject
	{

		/// <summary>
		/// Holds the name of the event.
		/// </summary>
		protected internal string name;

		/// <summary>
		/// Holds the properties of the event.
		/// </summary>
		protected internal IDictionary<string, object> properties;

		/// <summary>
		/// Holds the consumed state of the event. Default is false.
		/// </summary>
		protected internal bool consumed = false;

		/// <summary>
		/// Constructs a new event for the given name.
		/// </summary>
		public mxEventObject(string name) : this(name, (object[]) null)
		{
		}

		/// <summary>
		/// Constructs a new event for the given name and properties. The optional
		/// properties are specified using a sequence of keys and values, eg.
		/// <code>new mxEventObject("eventName", key1, val1, .., keyN, valN))</code>
		/// </summary>
		public mxEventObject(string name, params object[] args)
		{
			this.name = name;
			properties = new Dictionary<string, object>();

			if (args != null)
			{
				for (int i = 0; i < args.Length; i += 2)
				{
					if (args[i + 1] != null)
					{
						properties[args[i].ToString()] = args[i + 1];
					}
				}
			}
		}

		/// <summary>
		/// Returns the name of the event.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		/// 
		public virtual IDictionary<string, object> Properties
		{
			get
			{
				return properties;
			}
		}

		/// 
		public virtual object getProperty(string key)
		{
			return properties[key];
		}

		/// <summary>
		/// Returns true if the event has been consumed.
		/// </summary>
		public virtual bool Consumed
		{
			get
			{
				return consumed;
			}
		}

		/// <summary>
		/// Consumes the event.
		/// </summary>
		public virtual void consume()
		{
			consumed = true;
		}

	}

}