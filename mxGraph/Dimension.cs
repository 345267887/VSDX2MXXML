using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
	/// The <code>Dimension</code> class encapsulates the width and
	/// height of a component (in integer precision) in a single object.
	/// The class is
	/// associated with certain properties of components. Several methods
	/// defined by the <code>Component</code> class and the
	/// <code>LayoutManager</code> interface return a
	/// <code>Dimension</code> object.
	/// <para>
	/// Normally the values of <code>width</code>
	/// and <code>height</code> are non-negative integers.
	/// The constructors that allow you to create a dimension do
	/// not prevent you from setting a negative value for these properties.
	/// If the value of <code>width</code> or <code>height</code> is
	/// negative, the behavior of some methods defined by other objects is
	/// undefined.
	/// 
	/// @author      Sami Shaio
	/// @author      Arthur van Hoff
	/// </para>
	/// </summary>
	/// <seealso cref=         java.awt.Component </seealso>
	/// <seealso cref=         java.awt.LayoutManager
	/// @since       1.0 </seealso>
	[Serializable]
    public class Dimension : Dimension2D
    {

        /// <summary>
        /// The width dimension; negative values can be used.
        /// 
        /// @serial </summary>
        /// <seealso cref= #getSize </seealso>
        /// <seealso cref= #setSize
        /// @since 1.0 </seealso>
        private double width;

        /// <summary>
        /// The height dimension; negative values can be used.
        /// 
        /// @serial </summary>
        /// <seealso cref= #getSize </seealso>
        /// <seealso cref= #setSize
        /// @since 1.0 </seealso>
        private double height;

        /*
		 * JDK 1.1 serialVersionUID
		 */
        private const long serialVersionUID = 4723952579491349524L;

        /// <summary>
        /// Initialize JNI field and method IDs
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        //[DllImport("unknown")]
        //private static extern void initIDs();

        //static Dimension()
        //{
        //    /* ensure that the necessary native libraries are loaded */
        //    Toolkit.loadLibraries();
        //    if (!GraphicsEnvironment.Headless)
        //    {
        //        initIDs();
        //    }
        //}

        /// <summary>
        /// Creates an instance of <code>Dimension</code> with a width
        /// of zero and a height of zero.
        /// </summary>
        public Dimension() : this(0, 0)
        {
        }

        /// <summary>
        /// Creates an instance of <code>Dimension</code> whose width
        /// and height are the same as for the specified dimension.
        /// </summary>
        /// <param name="d">   the specified dimension for the
        ///               <code>width</code> and
        ///               <code>height</code> values </param>
        public Dimension(Dimension d) : this(d.width, d.height)
        {
        }

        /// <summary>
        /// Constructs a <code>Dimension</code> and initializes
        /// it to the specified width and specified height.
        /// </summary>
        /// <param name="width"> the specified width </param>
        /// <param name="height"> the specified height </param>
        public Dimension(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public override double Width
        {
            set
            {
                width = value;
            }
            get
            {
                return width;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// @since 1.2
        /// </summary>
        public override double Height
        {
            set
            {
                height = value;
            }
            get
            {
                return height;
            }
        }

        /// <summary>
        /// Sets the size of this <code>Dimension</code> object to
        /// the specified width and height in double precision.
        /// Note that if <code>width</code> or <code>height</code>
        /// are larger than <code>Integer.MAX_VALUE</code>, they will
        /// be reset to <code>Integer.MAX_VALUE</code>.
        /// </summary>
        /// <param name="width">  the new width for the <code>Dimension</code> object </param>
        /// <param name="height"> the new height for the <code>Dimension</code> object
        /// @since 1.2 </param>
        public override void setSize(double width, double height)
        {
            this.width = (int)Math.Ceiling(width);
            this.height = (int)Math.Ceiling(height);
        }

        /// <summary>
        /// Gets the size of this <code>Dimension</code> object.
        /// This method is included for completeness, to parallel the
        /// <code>getSize</code> method defined by <code>Component</code>.
        /// </summary>
        /// <returns>   the size of this dimension, a new instance of
        ///           <code>Dimension</code> with the same width and height </returns>
        /// <seealso cref=      java.awt.Dimension#setSize </seealso>
        /// <seealso cref=      java.awt.Component#getSize
        /// @since    1.1 </seealso>
        public override Dimension2D Size
        {
            get
            {
                return new Dimension((int)Width,(int)Height);
            }
            set
            {
                setSize(value.Width, value.Height);
            }
        }
        


        /// <summary>
        /// Sets the size of this <code>Dimension</code> object
        /// to the specified width and height.
        /// This method is included for completeness, to parallel the
        /// <code>setSize</code> method defined by <code>Component</code>.
        /// </summary>
        /// <param name="width">   the new width for this <code>Dimension</code> object </param>
        /// <param name="height">  the new height for this <code>Dimension</code> object </param>
        /// <seealso cref=      java.awt.Dimension#getSize </seealso>
        /// <seealso cref=      java.awt.Component#setSize
        /// @since    1.1 </seealso>
        public virtual void setSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Checks whether two dimension objects have equal values.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Dimension)
            {
                Dimension d = (Dimension)obj;
                return (width == d.width) && (height == d.height);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this <code>Dimension</code>.
        /// </summary>
        /// <returns>    a hash code for this <code>Dimension</code> </returns>
        public override int GetHashCode()
        {
            double sum = width + height;
            return (int)(sum * (sum + 1) / 2 + width);
        }

        /// <summary>
        /// Returns a string representation of the values of this
        /// <code>Dimension</code> object's <code>height</code> and
        /// <code>width</code> fields. This method is intended to be used only
        /// for debugging purposes, and the content and format of the returned
        /// string may vary between implementations. The returned string may be
        /// empty but may not be <code>null</code>.
        /// </summary>
        /// <returns>  a string representation of this <code>Dimension</code>
        ///          object </returns>
        public override string ToString()
        {
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            return this.GetType().FullName + "[width=" + width + ",height=" + height + "]";
        }
    }

}
