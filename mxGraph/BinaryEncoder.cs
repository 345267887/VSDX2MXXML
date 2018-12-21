using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// Defines common encoding methods for byte array encoders.
    /// 
    /// @version $Id: BinaryEncoder.java 1379145 2012-08-30 21:02:52Z tn $
    /// </summary>
    public interface BinaryEncoder : Encoder
    {

        /// <summary>
        /// Encodes a byte array and return the encoded data as a byte array.
        /// </summary>
        /// <param name="source">
        ///            Data to be encoded </param>
        /// <returns> A byte array containing the encoded data </returns>
        /// <exception cref="EncoderException">
        ///             thrown if the Encoder encounters a failure condition during the encoding process. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: byte[] encode(byte[] source) throws EncoderException;
        sbyte[] encode(sbyte[] source);
    }

}
