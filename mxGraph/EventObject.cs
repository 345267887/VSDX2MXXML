using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// <para>
    /// The root class from which all event state objects shall be derived.
    /// </para>
    /// <para>
    /// All Events are constructed with a reference to the object, the "source",
    /// that is logically deemed to be the object upon which the Event in question
    /// initially occurred upon.
    /// 
    /// @since JDK1.1
    /// </para>
    /// </summary>

    [Serializable]
    public class EventObject
    {

        private const long serialVersionUID = 5516075349620653480L;

        /// <summary>
        /// The object on which the Event initially occurred.
        /// </summary>
        [NonSerialized]
        protected internal object source;

        /// <summary>
        /// Constructs a prototypical Event.
        /// </summary>
        /// <param name="source">    The object on which the Event initially occurred. </param>
        /// <exception cref="IllegalArgumentException">  if source is null. </exception>
        public EventObject(object source)
        {
            if (source == null)
            {
                throw new System.ArgumentException("null source");
            }

            this.source = source;
        }

        /// <summary>
        /// The object on which the Event initially occurred.
        /// </summary>
        /// <returns>   The object on which the Event initially occurred. </returns>
        public virtual object Source
        {
            get
            {
                return source;
            }
        }

        /// <summary>
        /// Returns a String representation of this EventObject.
        /// </summary>
        /// <returns>  A a String representation of this EventObject. </returns>
        public override string ToString()
        {
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            return this.GetType().FullName + "[source=" + source + "]";
        }
    }

}
