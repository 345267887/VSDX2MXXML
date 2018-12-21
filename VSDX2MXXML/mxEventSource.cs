using System;
using System.Collections.Generic;
using System.Text;

namespace VSDX2MXXML
{
    /// <summary>
	/// Base class for objects that dispatch named events.
	/// </summary>
	public class mxEventSource
    {

        /// <summary>
        /// Defines the requirements for an object that listens to an event source.
        /// </summary>
        public interface mxIEventListener
        {

            /// <summary>
            /// Called when the graph model has changed.
            /// </summary>
            /// <param name="sender"> Reference to the source of the event. </param>
            /// <param name="evt"> Event object to be dispatched. </param>
            void invoke(object sender, mxEventObject evt);

        }

        /// <summary>
        /// Holds the event names and associated listeners in an array. The array
        /// contains the event name followed by the respective listener for each
        /// registered listener.
        /// </summary>
        [NonSerialized]
        protected internal IList<object> eventListeners = null;

        /// <summary>
        /// Holds the source object for this event source.
        /// </summary>
        protected internal object eventSource;

        /// <summary>
        /// Specifies if events can be fired. Default is true.
        /// </summary>
        protected internal bool eventsEnabled = true;

        /// <summary>
        /// Constructs a new event source using this as the source object.
        /// </summary>
        public mxEventSource() : this(null)
        {
        }

        /// <summary>
        /// Constructs a new event source for the given source object.
        /// </summary>
        public mxEventSource(object source)
        {
            EventSource = source;
        }

        /// 
        public virtual object EventSource
        {
            get
            {
                return eventSource;
            }
            set
            {
                this.eventSource = value;
            }
        }


        /// 
        public virtual bool EventsEnabled
        {
            get
            {
                return eventsEnabled;
            }
            set
            {
                this.eventsEnabled = value;
            }
        }


        /// <summary>
        /// Binds the specified function to the given event name. If no event name
        /// is given, then the listener is registered for all events.
        /// </summary>
        public virtual void addListener(string eventName, mxIEventListener listener)
        {
            if (eventListeners == null)
            {
                eventListeners = new List<object>();
            }

            eventListeners.Add(eventName);
            eventListeners.Add(listener);
        }

        /// <summary>
        /// Function: removeListener
        /// 
        /// Removes all occurances of the given listener from the list of listeners.
        /// </summary>
        public virtual void removeListener(mxIEventListener listener)
        {
            removeListener(listener, null);
        }

        /// <summary>
        /// Function: removeListener
        /// 
        /// Removes all occurances of the given listener from the list of listeners.
        /// </summary>
        public virtual void removeListener(mxIEventListener listener, string eventName)
        {
            if (eventListeners != null)
            {
                for (int i = eventListeners.Count - 2; i > 1; i -= 2)
                {
                    if (eventListeners[i + 1] == listener && (string.ReferenceEquals(eventName, null) || eventListeners[i].ToString().Equals(eventName)))
                    {
                        eventListeners.RemoveAt(i + 1);
                        eventListeners.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Dispatches the given event name with this object as the event source.
        /// <code>fireEvent(new mxEventObject("eventName", key1, val1, .., keyN, valN))</code>
        /// 
        /// </summary>
        public virtual void fireEvent(mxEventObject evt)
        {
            fireEvent(evt, null);
        }

        /// <summary>
        /// Dispatches the given event name, passing all arguments after the given
        /// name to the registered listeners for the event.
        /// </summary>
        public virtual void fireEvent(mxEventObject evt, object sender)
        {
            if (eventListeners != null && eventListeners.Count > 0 && EventsEnabled)
            {
                if (sender == null)
                {
                    sender = EventSource;
                }

                if (sender == null)
                {
                    sender = this;
                }

                for (int i = 0; i < eventListeners.Count; i += 2)
                {
                    string listen = (string)eventListeners[i];

                    if (string.ReferenceEquals(listen, null) || listen.Equals(evt.Name))
                    {
                        ((mxIEventListener)eventListeners[i + 1]).invoke(sender, evt);
                    }
                }
            }
        }

    }
}
