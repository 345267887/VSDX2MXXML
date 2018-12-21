using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
	/// This is an abstract class that provides base functionality
	/// for the <seealso cref="PropertyChangeSupport PropertyChangeSupport"/> class
	/// and the <seealso cref="VetoableChangeSupport VetoableChangeSupport"/> class.
	/// </summary>
	/// <seealso cref= PropertyChangeListenerMap </seealso>
	/// <seealso cref= VetoableChangeListenerMap
	/// 
	/// @author Sergey A. Malenkov </seealso>
	internal abstract class ChangeListenerMap<L> where L : EventListener
    {
        private IDictionary<string, L[]> map;

        /// <summary>
        /// Creates an array of listeners.
        /// This method can be optimized by using
        /// the same instance of the empty array
        /// when {@code length} is equal to {@code 0}.
        /// </summary>
        /// <param name="length">  the array length </param>
        /// <returns>        an array with specified length </returns>
        protected internal abstract L[] newArray(int length);

        /// <summary>
        /// Creates a proxy listener for the specified property.
        /// </summary>
        /// <param name="name">      the name of the property to listen on </param>
        /// <param name="listener">  the listener to process events </param>
        /// <returns>          a proxy listener </returns>
        protected internal abstract L newProxy(string name, L listener);

        /// <summary>
        /// Adds a listener to the list of listeners for the specified property.
        /// This listener is called as many times as it was added.
        /// </summary>
        /// <param name="name">      the name of the property to listen on </param>
        /// <param name="listener">  the listener to process events </param>
        public void add(string name, L listener)
        {
            lock (this)
            {
                if (this.map == null)
                {
                    this.map = new Dictionary<string, L[]>();
                }
                L[] array = this.map[name];
                int size = (array != null) ? array.Length : 0;

                L[] clone = newArray(size + 1);
                clone[size] = listener;
                if (array != null)
                {
                    Array.Copy(array, 0, clone, 0, size);
                }
                this.map[name] = clone;
            }
        }

        /// <summary>
        /// Removes a listener from the list of listeners for the specified property.
        /// If the listener was added more than once to the same event source,
        /// this listener will be notified one less time after being removed.
        /// </summary>
        /// <param name="name">      the name of the property to listen on </param>
        /// <param name="listener">  the listener to process events </param>
        public void remove(string name, L listener)
        {
            lock (this)
            {
                if (this.map != null)
                {
                    L[] array = this.map[name];
                    if (array != null)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (listener.Equals(array[i]))
                            {
                                int size = array.Length - 1;
                                if (size > 0)
                                {
                                    L[] clone = newArray(size);
                                    Array.Copy(array, 0, clone, 0, i);
                                    Array.Copy(array, i + 1, clone, i, size - i);
                                    this.map[name] = clone;
                                }
                                else
                                {
                                    this.map.Remove(name);
                                    if (this.map.Count == 0)
                                    {
                                        this.map = null;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the list of listeners for the specified property.
        /// </summary>
        /// <param name="name">  the name of the property </param>
        /// <returns>      the corresponding list of listeners </returns>
        public L[] get(string name)
        {
            lock (this)
            {
                return (this.map != null) ? this.map[name] : null;
            }
        }

        /// <summary>
        /// Sets new list of listeners for the specified property.
        /// </summary>
        /// <param name="name">       the name of the property </param>
        /// <param name="listeners">  new list of listeners </param>
        public void set(string name, L[] listeners)
        {
            if (listeners != null)
            {
                if (this.map == null)
                {
                    this.map = new Dictionary<string, L[]>();
                }
                this.map[name] = listeners;
            }
            else if (this.map != null)
            {
                this.map.Remove(name);
                if (this.map.Count == 0)
                {
                    this.map = null;
                }
            }
        }

        /// <summary>
        /// Returns all listeners in the map.
        /// </summary>
        /// <returns> an array of all listeners </returns>
        public L[] Listeners
        {
            get
            {
                lock (this)
                {
                    if (this.map == null)
                    {
                        return newArray(0);
                    }
                    IList<L> list = new List<L>();

                    L[] listeners = this.map[null];
                    if (listeners != null)
                    {
                        foreach (L listener in listeners)
                        {
                            list.Add(listener);
                        }
                    }
                    foreach (KeyValuePair<string, L[]> entry in this.map.SetOfKeyValuePairs())
                    {
                        string name = entry.Key;
                        if (!string.ReferenceEquals(name, null))
                        {
                            foreach (L listener in entry.Value)
                            {
                                list.Add(newProxy(name, listener));
                            }
                        }
                    }
                    //return list.toArray(newArray(list.Count));
                    return list.ToArray();
                }
            }
        }

        /// <summary>
        /// Returns listeners that have been associated with the named property.
        /// </summary>
        /// <param name="name">  the name of the property </param>
        /// <returns> an array of listeners for the named property </returns>
        public L[] getListeners(string name)
        {
            if (!string.ReferenceEquals(name, null))
            {
                L[] listeners = get(name);
                if (listeners != null)
                {
                    return (L[])listeners.Clone();
                }
            }
            return newArray(0);
        }

        /// <summary>
        /// Indicates whether the map contains
        /// at least one listener to be notified.
        /// </summary>
        /// <param name="name">  the name of the property </param>
        /// <returns>      {@code true} if at least one listener exists or
        ///              {@code false} otherwise </returns>
        public bool hasListeners(string name)
        {
            lock (this)
            {
                if (this.map == null)
                {
                    return false;
                }
                L[] array = this.map[null];
                return (array != null) || ((!string.ReferenceEquals(name, null)) && (null != this.map[name]));
            }
        }

        /// <summary>
        /// Returns a set of entries from the map.
        /// Each entry is a pair consisted of the property name
        /// and the corresponding list of listeners.
        /// </summary>
        /// <returns> a set of entries from the map </returns>
        public ISet<KeyValuePair<string, L[]>> Entries
        {
            get
            {
                //return (this.map != null) ? this.map.SetOfKeyValuePairs() : System.Linq.Enumerable.Empty<KeyValuePair<string, L[]>>();

                return (this.map != null) ? this.map.SetOfKeyValuePairs() : new HashSet<KeyValuePair<string, L[]>>();
            }
        }

        /// <summary>
        /// Extracts a real listener from the proxy listener.
        /// It is necessary because default proxy class is not serializable.
        /// </summary>
        /// <returns> a real listener </returns>
        public L extract(L listener)
        {
            //while (listener is EventListenerProxy<L>)
            //{
            //    EventListenerProxy<L> proxy = (EventListenerProxy<L>)listener;
            //    listener = proxy.Listener;
            //}

            return listener;
        }
    }

}
