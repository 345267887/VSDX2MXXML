using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
	/// A class which extends the {@code EventListenerProxy}
	/// specifically for adding a {@code PropertyChangeListener}
	/// with a "bound" property.
	/// Instances of this class can be added
	/// as {@code PropertyChangeListener}s to a bean
	/// which supports firing property change events.
	/// <para>
	/// If the object has a {@code getPropertyChangeListeners} method
	/// then the array returned could be a mixture of {@code PropertyChangeListener}
	/// and {@code PropertyChangeListenerProxy} objects.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= java.util.EventListenerProxy </seealso>
	/// <seealso cref= PropertyChangeSupport#getPropertyChangeListeners
	/// @since 1.4 </seealso>
	public class PropertyChangeListenerProxy : EventListenerProxy<PropertyChangeListener>, PropertyChangeListener
    {

        private readonly string propertyName;

        /// <summary>
        /// Constructor which binds the {@code PropertyChangeListener}
        /// to a specific property.
        /// </summary>
        /// <param name="propertyName">  the name of the property to listen on </param>
        /// <param name="listener">      the listener object </param>
        public PropertyChangeListenerProxy(string propertyName, PropertyChangeListener listener) : base(listener)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Forwards the property change event to the listener delegate.
        /// </summary>
        /// <param name="event">  the property change event </param>
        public virtual void propertyChange(PropertyChangeEvent @event)
        {
            Listener.propertyChange(@event);
        }

        /// <summary>
        /// Returns the name of the named property associated with the listener.
        /// </summary>
        /// <returns> the name of the named property associated with the listener </returns>
        public virtual string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }
    }

}
