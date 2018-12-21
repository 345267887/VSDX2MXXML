using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// A "PropertyChange" event gets fired whenever a bean changes a "bound"
    /// property.  You can register a PropertyChangeListener with a source
    /// bean so as to be notified of any bound property updates.
    /// </summary>

    public interface PropertyChangeListener : EventListener
    {

        /// <summary>
        /// This method gets called when a bound property is changed. </summary>
        /// <param name="evt"> A PropertyChangeEvent object describing the event source
        ///          and the property that has changed. </param>

        void propertyChange(PropertyChangeEvent evt);

    }

}
