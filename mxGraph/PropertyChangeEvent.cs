using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// A "PropertyChange" event gets delivered whenever a bean changes a "bound"
    /// or "constrained" property.  A PropertyChangeEvent object is sent as an
    /// argument to the PropertyChangeListener and VetoableChangeListener methods.
    /// <P>
    /// Normally PropertyChangeEvents are accompanied by the name and the old
    /// and new value of the changed property.  If the new value is a primitive
    /// type (such as int or boolean) it must be wrapped as the
    /// corresponding java.lang.* Object type (such as Integer or Boolean).
    /// <P>
    /// Null values may be provided for the old and the new values if their
    /// true values are not known.
    /// <P>
    /// An event source may send a null object as the name to indicate that an
    /// arbitrary set of if its properties have changed.  In this case the
    /// old and new values should also be null.
    /// </summary>

    public class PropertyChangeEvent : EventObject
    {

        /// <summary>
        /// Constructs a new <code>PropertyChangeEvent</code>.
        /// </summary>
        /// <param name="source">  The bean that fired the event. </param>
        /// <param name="propertyName">  The programmatic name of the property
        ///          that was changed. </param>
        /// <param name="oldValue">  The old value of the property. </param>
        /// <param name="newValue">  The new value of the property. </param>
        public PropertyChangeEvent(object source, string propertyName, object oldValue, object newValue) : base(source)
        {
            this.propertyName = propertyName;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        /// <summary>
        /// Gets the programmatic name of the property that was changed.
        /// </summary>
        /// <returns>  The programmatic name of the property that was changed.
        ///          May be null if multiple properties have changed. </returns>
        public virtual string PropertyName
        {
            get
            {
                return propertyName;
            }
        }

        /// <summary>
        /// Gets the new value for the property, expressed as an Object.
        /// </summary>
        /// <returns>  The new value for the property, expressed as an Object.
        ///          May be null if multiple properties have changed. </returns>
        public virtual object NewValue
        {
            get
            {
                return newValue;
            }
        }

        /// <summary>
        /// Gets the old value for the property, expressed as an Object.
        /// </summary>
        /// <returns>  The old value for the property, expressed as an Object.
        ///          May be null if multiple properties have changed. </returns>
        public virtual object OldValue
        {
            get
            {
                return oldValue;
            }
        }

        /// <summary>
        /// Sets the propagationId object for the event.
        /// </summary>
        /// <param name="propagationId">  The propagationId object for the event. </param>
        public virtual object PropagationId
        {
            set
            {
                this.propagationId = value;
            }
            get
            {
                return propagationId;
            }
        }


        /// <summary>
        /// name of the property that changed.  May be null, if not known.
        /// @serial
        /// </summary>
        private string propertyName;

        /// <summary>
        /// New value for property.  May be null if not known.
        /// @serial
        /// </summary>
        private object newValue;

        /// <summary>
        /// Previous value for property.  May be null if not known.
        /// @serial
        /// </summary>
        private object oldValue;

        /// <summary>
        /// Propagation ID.  May be null.
        /// @serial </summary>
        /// <seealso cref= #getPropagationId </seealso>
        private object propagationId;
    }

}
