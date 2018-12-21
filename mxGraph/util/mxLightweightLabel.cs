using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// $Id: mxLightweightLabel.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2007, Gaudenz Alder
/// </summary>
namespace mxGraph.util
{



	/// <summary>
	/// @author Administrator
	/// 
	/// </summary>
	public class mxLightweightLabel : Label
	{

        //public string Text { get; set; }

		/// 
		private const long serialVersionUID = -6771477489533614010L;

		/// 
		protected internal static mxLightweightLabel sharedInstance;

		/// <summary>
		/// Initializes the shared instance.
		/// </summary>
		static mxLightweightLabel()
		{
			try
			{
				sharedInstance = new mxLightweightLabel();
			}
			catch (Exception)
			{
				// ignore
			}
		}

		/// 
		public static mxLightweightLabel SharedInstance
		{
			get
			{
				return sharedInstance;
			}
		}

		/// 
		/// 
		public mxLightweightLabel()
		{
			//Font = new Font(mxConstants.DEFAULT_FONTFAMILY,, 0, mxConstants.DEFAULT_FONTSIZE);
            
			//VerticalAlignment = SwingConstants.TOP;
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void validate()
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void revalidate()
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void repaint(long tm, int x, int y, int width, int height)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void repaint(Rectangle r)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		protected internal virtual void firePropertyChange(string propertyName, object oldValue, object newValue)
		{
			// Strings get interned...
			//if (string.ReferenceEquals(propertyName, "text"))
			//{
			//	base.firePropertyChange(propertyName, oldValue, newValue);
			//}
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, sbyte oldValue, sbyte newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, char oldValue, char newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, short oldValue, short newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, int oldValue, int newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, long oldValue, long newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, float oldValue, float newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, double oldValue, double newValue)
		{
		}

		/// <summary>
		/// Overridden for performance reasons.
		/// 
		/// </summary>
		public virtual void firePropertyChange(string propertyName, bool oldValue, bool newValue)
		{
		}

	}

}