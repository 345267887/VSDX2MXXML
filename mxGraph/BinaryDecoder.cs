using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// Defines common decoding methods for byte array decoders.
    /// 
    /// @version $Id: BinaryDecoder.java 1379145 2012-08-30 21:02:52Z tn $
    /// </summary>
    public interface BinaryDecoder : Decoder
    {

        /// <summary>
        /// Decodes a byte array and returns the results as a byte array.
        /// </summary>
        /// <param name="source">
        ///            A byte array which has been encoded with the appropriate encoder </param>
        /// <returns> a byte array that contains decoded content </returns>
        /// <exception cref="DecoderException">
        ///             A decoder exception is thrown if a Decoder encounters a failure condition during the decode process. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: byte[] decode(byte[] source) throws DecoderException;
        sbyte[] decode(sbyte[] source);
    }

}
