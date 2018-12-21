using System;
using System.Collections;
using System.Collections.Generic;


namespace mxGraph
{
    /// <summary>
    /// This is a utility class that can be used by beans that support bound
    /// properties.  You can use an instance of this class as a member field
    /// of your bean and delegate various work to it.
    /// 
    /// This class is serializable.  When it is serialized it will save
    /// (and restore) any listeners that are themselves serializable.  Any
    /// non-serializable listeners will be skipped during serialization.
    /// </summary>
    [Serializable]
    public class PropertyChangeSupport
    {
        private PropertyChangeListenerMap map = new PropertyChangeListenerMap();

        /// <summary>
        /// Constructs a <code>PropertyChangeSupport</code> object.
        /// </summary>
        /// <param name="sourceBean">  The bean to be given as the source for any events. </param>
        public PropertyChangeSupport(object sourceBean)
        {
            if (sourceBean == null)
            {
                throw new System.NullReferenceException();
            }
            source = sourceBean;
        }

        /// <summary>
        /// Add a PropertyChangeListener to the listener list.
        /// The listener is registered for all properties.
        /// The same listener object may be added more than once, and will be called
        /// as many times as it is added.
        /// If <code>listener</code> is null, no exception is thrown and no action
        /// is taken.
        /// </summary>
        /// <param name="listener">  The PropertyChangeListener to be added </param>
        public virtual void addPropertyChangeListener(PropertyChangeListener listener)
        {
            if (listener == null)
            {
                return;
            }
            if (listener is PropertyChangeListenerProxy)
            {
                PropertyChangeListenerProxy proxy = (PropertyChangeListenerProxy)listener;
                // Call two argument add method.
                addPropertyChangeListener(proxy.PropertyName, proxy.Listener);
            }
            else
            {
                this.map.add(null, listener);
            }
        }

        /// <summary>
        /// Remove a PropertyChangeListener from the listener list.
        /// This removes a PropertyChangeListener that was registered
        /// for all properties.
        /// If <code>listener</code> was added more than once to the same event
        /// source, it will be notified one less time after being removed.
        /// If <code>listener</code> is null, or was never added, no exception is
        /// thrown and no action is taken.
        /// </summary>
        /// <param name="listener">  The PropertyChangeListener to be removed </param>
        public virtual void removePropertyChangeListener(PropertyChangeListener listener)
        {
            if (listener == null)
            {
                return;
            }
            if (listener is PropertyChangeListenerProxy)
            {
                PropertyChangeListenerProxy proxy = (PropertyChangeListenerProxy)listener;
                // Call two argument remove method.
                removePropertyChangeListener(proxy.PropertyName, proxy.Listener);
            }
            else
            {
                this.map.remove(null, listener);
            }
        }

        /// <summary>
        /// Returns an array of all the listeners that were added to the
        /// PropertyChangeSupport object with addPropertyChangeListener().
        /// <para>
        /// If some listeners have been added with a named property, then
        /// the returned array will be a mixture of PropertyChangeListeners
        /// and <code>PropertyChangeListenerProxy</code>s. If the calling
        /// method is interested in distinguishing the listeners then it must
        /// test each element to see if it's a
        /// <code>PropertyChangeListenerProxy</code>, perform the cast, and examine
        /// the parameter.
        /// 
        /// <pre>
        /// PropertyChangeListener[] listeners = bean.getPropertyChangeListeners();
        /// for (int i = 0; i < listeners.length; i++) {
        ///   if (listeners[i] instanceof PropertyChangeListenerProxy) {
        ///     PropertyChangeListenerProxy proxy =
        ///                    (PropertyChangeListenerProxy)listeners[i];
        ///     if (proxy.getPropertyName().equals("foo")) {
        ///       // proxy is a PropertyChangeListener which was associated
        ///       // with the property named "foo"
        ///     }
        ///   }
        /// }
        /// </pre>
        /// 
        /// </para>
        /// </summary>
        /// <seealso cref= PropertyChangeListenerProxy </seealso>
        /// <returns> all of the <code>PropertyChangeListeners</code> added or an
        ///         empty array if no listeners have been added
        /// @since 1.4 </returns>
        public virtual PropertyChangeListener[] PropertyChangeListeners
        {
            get
            {
                return this.map.Listeners;
            }
        }

        /// <summary>
        /// Add a PropertyChangeListener for a specific property.  The listener
        /// will be invoked only when a call on firePropertyChange names that
        /// specific property.
        /// The same listener object may be added more than once.  For each
        /// property,  the listener will be invoked the number of times it was added
        /// for that property.
        /// If <code>propertyName</code> or <code>listener</code> is null, no
        /// exception is thrown and no action is taken.
        /// </summary>
        /// <param name="propertyName">  The name of the property to listen on. </param>
        /// <param name="listener">  The PropertyChangeListener to be added </param>
        public virtual void addPropertyChangeListener(string propertyName, PropertyChangeListener listener)
        {
            if (listener == null || string.ReferenceEquals(propertyName, null))
            {
                return;
            }
            listener = this.map.extract(listener);
            if (listener != null)
            {
                this.map.add(propertyName, listener);
            }
        }

        /// <summary>
        /// Remove a PropertyChangeListener for a specific property.
        /// If <code>listener</code> was added more than once to the same event
        /// source for the specified property, it will be notified one less time
        /// after being removed.
        /// If <code>propertyName</code> is null,  no exception is thrown and no
        /// action is taken.
        /// If <code>listener</code> is null, or was never added for the specified
        /// property, no exception is thrown and no action is taken.
        /// </summary>
        /// <param name="propertyName">  The name of the property that was listened on. </param>
        /// <param name="listener">  The PropertyChangeListener to be removed </param>
        public virtual void removePropertyChangeListener(string propertyName, PropertyChangeListener listener)
        {
            if (listener == null || string.ReferenceEquals(propertyName, null))
            {
                return;
            }
            listener = this.map.extract(listener);
            if (listener != null)
            {
                this.map.remove(propertyName, listener);
            }
        }

        /// <summary>
        /// Returns an array of all the listeners which have been associated
        /// with the named property.
        /// </summary>
        /// <param name="propertyName">  The name of the property being listened to </param>
        /// <returns> all of the <code>PropertyChangeListeners</code> associated with
        ///         the named property.  If no such listeners have been added,
        ///         or if <code>propertyName</code> is null, an empty array is
        ///         returned.
        /// @since 1.4 </returns>
        public virtual PropertyChangeListener[] getPropertyChangeListeners(string propertyName)
        {
            return this.map.getListeners(propertyName);
        }

        /// <summary>
        /// Report a bound property update to any registered listeners.
        /// No event is fired if old and new are equal and non-null.
        /// 
        /// <para>
        /// This is merely a convenience wrapper around the more general
        /// firePropertyChange method that takes {@code
        /// PropertyChangeEvent} value.
        /// 
        /// </para>
        /// </summary>
        /// <param name="propertyName">  The programmatic name of the property
        ///          that was changed. </param>
        /// <param name="oldValue">  The old value of the property. </param>
        /// <param name="newValue">  The new value of the property. </param>
        public virtual void firePropertyChange(string propertyName, object oldValue, object newValue)
        {
            if (oldValue != null && newValue != null && oldValue.Equals(newValue))
            {
                return;
            }
            firePropertyChange(new PropertyChangeEvent(source, propertyName, oldValue, newValue));
        }

        /// <summary>
        /// Report an int bound property update to any registered listeners.
        /// No event is fired if old and new are equal.
        /// <para>
        /// This is merely a convenience wrapper around the more general
        /// firePropertyChange method that takes Object values.
        /// 
        /// </para>
        /// </summary>
        /// <param name="propertyName">  The programmatic name of the property
        ///          that was changed. </param>
        /// <param name="oldValue">  The old value of the property. </param>
        /// <param name="newValue">  The new value of the property. </param>
        public virtual void firePropertyChange(string propertyName, int oldValue, int newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            firePropertyChange(propertyName, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
        }

        /// <summary>
        /// Report a boolean bound property update to any registered listeners.
        /// No event is fired if old and new are equal.
        /// <para>
        /// This is merely a convenience wrapper around the more general
        /// firePropertyChange method that takes Object values.
        /// 
        /// </para>
        /// </summary>
        /// <param name="propertyName">  The programmatic name of the property
        ///          that was changed. </param>
        /// <param name="oldValue">  The old value of the property. </param>
        /// <param name="newValue">  The new value of the property. </param>
        public virtual void firePropertyChange(string propertyName, bool oldValue, bool newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            firePropertyChange(propertyName, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue));
        }

        /// <summary>
        /// Fire an existing PropertyChangeEvent to any registered listeners.
        /// No event is fired if the given event's old and new values are
        /// equal and non-null. </summary>
        /// <param name="evt">  The PropertyChangeEvent object. </param>
        public virtual void firePropertyChange(PropertyChangeEvent evt)
        {
            object oldValue = evt.OldValue;
            object newValue = evt.NewValue;
            string propertyName = evt.PropertyName;
            if (oldValue != null && newValue != null && oldValue.Equals(newValue))
            {
                return;
            }
            PropertyChangeListener[] common = this.map.get(null);
            PropertyChangeListener[] named = (!string.ReferenceEquals(propertyName, null)) ? this.map.get(propertyName) : null;

            fire(common, evt);
            fire(named, evt);
        }

        private void fire(PropertyChangeListener[] listeners, PropertyChangeEvent @event)
        {
            if (listeners != null)
            {
                foreach (PropertyChangeListener listener in listeners)
                {
                    listener.propertyChange(@event);
                }
            }
        }

        /// <summary>
        /// Report a bound indexed property update to any registered
        /// listeners.
        /// <para>
        /// No event is fired if old and new values are equal
        /// and non-null.
        /// 
        /// </para>
        /// <para>
        /// This is merely a convenience wrapper around the more general
        /// firePropertyChange method that takes {@code PropertyChangeEvent} value.
        /// 
        /// </para>
        /// </summary>
        /// <param name="propertyName"> The programmatic name of the property that
        ///                     was changed. </param>
        /// <param name="index">        index of the property element that was changed. </param>
        /// <param name="oldValue">     The old value of the property. </param>
        /// <param name="newValue">     The new value of the property.
        /// @since 1.5 </param>
        public virtual void fireIndexedPropertyChange(string propertyName, int index, object oldValue, object newValue)
        {
            firePropertyChange(new IndexedPropertyChangeEvent(source, propertyName, oldValue, newValue, index));
        }

        /// <summary>
        /// Report an <code>int</code> bound indexed property update to any registered
        /// listeners.
        /// <para>
        /// No event is fired if old and new values are equal.
        /// </para>
        /// <para>
        /// This is merely a convenience wrapper around the more general
        /// fireIndexedPropertyChange method which takes Object values.
        /// 
        /// </para>
        /// </summary>
        /// <param name="propertyName"> The programmatic name of the property that
        ///                     was changed. </param>
        /// <param name="index">        index of the property element that was changed. </param>
        /// <param name="oldValue">     The old value of the property. </param>
        /// <param name="newValue">     The new value of the property.
        /// @since 1.5 </param>
        public virtual void fireIndexedPropertyChange(string propertyName, int index, int oldValue, int newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            fireIndexedPropertyChange(propertyName, index, Convert.ToInt32(oldValue), Convert.ToInt32(newValue));
        }

        /// <summary>
        /// Report a <code>boolean</code> bound indexed property update to any
        /// registered listeners.
        /// <para>
        /// No event is fired if old and new values are equal.
        /// </para>
        /// <para>
        /// This is merely a convenience wrapper around the more general
        /// fireIndexedPropertyChange method which takes Object values.
        /// 
        /// </para>
        /// </summary>
        /// <param name="propertyName"> The programmatic name of the property that
        ///                     was changed. </param>
        /// <param name="index">        index of the property element that was changed. </param>
        /// <param name="oldValue">     The old value of the property. </param>
        /// <param name="newValue">     The new value of the property.
        /// @since 1.5 </param>
        public virtual void fireIndexedPropertyChange(string propertyName, int index, bool oldValue, bool newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            fireIndexedPropertyChange(propertyName, index, Convert.ToBoolean(oldValue), Convert.ToBoolean(newValue));
        }

        /// <summary>
        /// Check if there are any listeners for a specific property, including
        /// those registered on all properties.  If <code>propertyName</code>
        /// is null, only check for listeners registered on all properties.
        /// </summary>
        /// <param name="propertyName">  the property name. </param>
        /// <returns> true if there are one or more listeners for the given property </returns>
        public virtual bool hasListeners(string propertyName)
        {
            return this.map.hasListeners(propertyName);
        }

        /// <summary>
        /// @serialData Null terminated list of <code>PropertyChangeListeners</code>.
        /// <para>
        /// At serialization time we skip non-serializable listeners and
        /// only serialize the serializable listeners.
        /// </para>
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
        private void writeObject(ObjectOutputStream s)
        {
            Dictionary<string, PropertyChangeSupport> children = null;
            PropertyChangeListener[] listeners = null;
            lock (this.map)
            {
                foreach (KeyValuePair<string, PropertyChangeListener[]> entry in this.map.Entries)
                {
                    string property = entry.Key;
                    if (string.ReferenceEquals(property, null))
                    {
                        listeners = entry.Value;
                    }
                    else
                    {
                        if (children == null)
                        {
                            children = new Dictionary<string, PropertyChangeSupport>();
                        }
                        PropertyChangeSupport pcs = new PropertyChangeSupport(this.source);
                        pcs.map.set(null, entry.Value);
                        children[property] = pcs;
                    }
                }
            }
            ObjectOutputStream.PutField fields = s.putFields();
            fields.put("children", children);
            fields.put("source", this.source);
            fields.put("propertyChangeSupportSerializedDataVersion", 2);
            s.writeFields();

            if (listeners != null)
            {
                foreach (PropertyChangeListener l in listeners)
                {
                    if (l is Serializable)
                    {
                        s.writeObject(l);
                    }
                }
            }
            s.writeObject(null);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void readObject(java.io.ObjectInputStream s) throws ClassNotFoundException, java.io.IOException
        private void readObject(ObjectInputStream s)
        {
            this.map = new PropertyChangeListenerMap();

            ObjectInputStream.GetField fields = s.readFields();

            Dictionary<string, PropertyChangeSupport> children = (Dictionary<string, PropertyChangeSupport>)fields.get("children", null);
            this.source = fields.get("source", null);
            fields.get("propertyChangeSupportSerializedDataVersion", 2);

            object listenerOrNull;
            while (null != (listenerOrNull = s.readObject()))
            {
                this.map.add(null, (PropertyChangeListener)listenerOrNull);
            }
            if (children != null)
            {
                foreach (KeyValuePair<string, PropertyChangeSupport> entry in children.SetOfKeyValuePairs())
                {
                    foreach (PropertyChangeListener listener in entry.Value.PropertyChangeListeners)
                    {
                        this.map.add(entry.Key, listener);
                    }
                }
            }
        }

        /// <summary>
        /// The object to be provided as the "source" for any generated events.
        /// </summary>
        private object source;

        /// <summary>
        /// @serialField children                                   Hashtable
        /// @serialField source                                     Object
        /// @serialField propertyChangeSupportSerializedDataVersion int
        /// </summary>
        private static readonly ObjectStreamField[] serialPersistentFields = new ObjectStreamField[]
        {
            new ObjectStreamField("children", typeof(Hashtable)),
            new ObjectStreamField("source", typeof(object)),
            new ObjectStreamField("propertyChangeSupportSerializedDataVersion", Integer.TYPE)
        };

        /// <summary>
        /// Serialization version ID, so we're compatible with JDK 1.1
        /// </summary>
        internal const long serialVersionUID = 6401253773779951803L;

        /// <summary>
        /// This is a <seealso cref="ChangeListenerMap ChangeListenerMap"/> implementation
        /// that works with <seealso cref="PropertyChangeListener PropertyChangeListener"/> objects.
        /// </summary>
        private sealed class PropertyChangeListenerMap : ChangeListenerMap<PropertyChangeListener>
        {
            internal static readonly PropertyChangeListener[] EMPTY = new PropertyChangeListener[] { };

            /// <summary>
            /// Creates an array of <seealso cref="PropertyChangeListener PropertyChangeListener"/> objects.
            /// This method uses the same instance of the empty array
            /// when {@code length} equals {@code 0}.
            /// </summary>
            /// <param name="length">  the array length </param>
            /// <returns>        an array with specified length </returns>
            protected internal override PropertyChangeListener[] newArray(int length)
            {
                return (0 < length) ? new PropertyChangeListener[length] : EMPTY;
            }

            /// <summary>
            /// Creates a <seealso cref="PropertyChangeListenerProxy PropertyChangeListenerProxy"/>
            /// object for the specified property.
            /// </summary>
            /// <param name="name">      the name of the property to listen on </param>
            /// <param name="listener">  the listener to process events </param>
            /// <returns>          a {@code PropertyChangeListenerProxy} object </returns>
            protected internal override PropertyChangeListener newProxy(string name, PropertyChangeListener listener)
            {
                return new PropertyChangeListenerProxy(name, listener);
            }
        }
    }

}
