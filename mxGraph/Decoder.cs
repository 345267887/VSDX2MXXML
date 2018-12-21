using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// Provides the highest level of abstraction for Decoders.
    /// <para>
    /// This is the sister interface of <seealso cref="Encoder"/>. All Decoders implement this common generic interface.
    /// Allows a user to pass a generic Object to any Decoder implementation in the codec package.
    /// </para>
    /// <para>
    /// One of the two interfaces at the center of the codec package.
    /// 
    /// @version $Id: Decoder.java 1379145 2012-08-30 21:02:52Z tn $
    /// </para>
    /// </summary>
    public interface Decoder
    {

        /// <summary>
        /// Decodes an "encoded" Object and returns a "decoded" Object. Note that the implementation of this interface will
        /// try to cast the Object parameter to the specific type expected by a particular Decoder implementation. If a
        /// <seealso cref="ClassCastException"/> occurs this decode method will throw a DecoderException.
        /// </summary>
        /// <param name="source">
        ///            the object to decode </param>
        /// <returns> a 'decoded" object </returns>
        /// <exception cref="DecoderException">
        ///             a decoder exception can be thrown for any number of reasons. Some good candidates are that the
        ///             parameter passed to this method is null, a param cannot be cast to the appropriate type for a
        ///             specific encoder. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: Object decode(Object source) throws DecoderException;
        object decode(object source);
    }

}
