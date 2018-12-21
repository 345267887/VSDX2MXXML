using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// Provides the highest level of abstraction for Encoders.
    /// <para>
    /// This is the sister interface of <seealso cref="Decoder"/>.  Every implementation of Encoder provides this
    /// common generic interface which allows a user to pass a generic Object to any Encoder implementation
    /// in the codec package.
    /// 
    /// @version $Id: Encoder.java 1379145 2012-08-30 21:02:52Z tn $
    /// </para>
    /// </summary>
    public interface Encoder
    {

        /// <summary>
        /// Encodes an "Object" and returns the encoded content as an Object. The Objects here may just be
        /// <code>byte[]</code> or <code>String</code>s depending on the implementation used.
        /// </summary>
        /// <param name="source">
        ///            An object to encode </param>
        /// <returns> An "encoded" Object </returns>
        /// <exception cref="EncoderException">
        ///             An encoder exception is thrown if the encoder experiences a failure condition during the encoding
        ///             process. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: Object encode(Object source) throws EncoderException;
        object encode(object source);
    }

}
