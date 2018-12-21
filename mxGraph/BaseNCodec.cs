using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
	/// Abstract superclass for Base-N encoders and decoders.
	/// 
	/// <para>
	/// This class is thread-safe.
	/// </para>
	/// 
	/// @version $Id: BaseNCodec.java 1811344 2017-10-06 15:19:57Z ggregory $
	/// </summary>
	public abstract class BaseNCodec : BinaryEncoder, BinaryDecoder
    {

        /// <summary>
        /// Holds thread context so classes can be thread-safe.
        /// 
        /// This class is not itself thread-safe; each thread must allocate its own copy.
        /// 
        /// @since 1.7
        /// </summary>
        internal class Context
        {

            /// <summary>
            /// Place holder for the bytes we're dealing with for our based logic.
            /// Bitwise operations store and extract the encoding or decoding from this variable.
            /// </summary>
            internal int ibitWorkArea;

            /// <summary>
            /// Place holder for the bytes we're dealing with for our based logic.
            /// Bitwise operations store and extract the encoding or decoding from this variable.
            /// </summary>
            internal long lbitWorkArea;

            /// <summary>
            /// Buffer for streaming.
            /// </summary>
            internal sbyte[] buffer;

            /// <summary>
            /// Position where next character should be written in the buffer.
            /// </summary>
            internal int pos;

            /// <summary>
            /// Position where next character should be read from the buffer.
            /// </summary>
            internal int readPos;

            /// <summary>
            /// Boolean flag to indicate the EOF has been reached. Once EOF has been reached, this object becomes useless,
            /// and must be thrown away.
            /// </summary>
            internal bool eof;

            /// <summary>
            /// Variable tracks how many characters have been written to the current line. Only used when encoding. We use
            /// it to make sure each encoded line never goes beyond lineLength (if lineLength &gt; 0).
            /// </summary>
            internal int currentLinePos;

            /// <summary>
            /// Writes to the buffer only occur after every 3/5 reads when encoding, and every 4/8 reads when decoding. This
            /// variable helps track that.
            /// </summary>
            internal int modulus;

            internal Context()
            {
            }

            /// <summary>
            /// Returns a String useful for debugging (especially within a debugger.)
            /// </summary>
            /// <returns> a String useful for debugging. </returns>
            //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
            //ORIGINAL LINE: @SuppressWarnings("boxing") @Override public String toString()
            public override string ToString()
            { // OK to ignore boxing here
                string temp= System.Text.Encoding.Default.GetString((buffer.Cast<byte>().ToArray()));//Arrays.ToString(buffer)

                return string.Format("{0}[buffer={1}, currentLinePos={2}, eof={3}, ibitWorkArea={4}, lbitWorkArea={5}, " + "modulus={6}, pos={7}, readPos={8}]", this.GetType().Name, temp, currentLinePos, eof, ibitWorkArea, lbitWorkArea, modulus, pos, readPos);
            }
        }

        /// <summary>
        /// EOF
        /// 
        /// @since 1.7
        /// </summary>
        internal const int EOF = -1;

        /// <summary>
        ///  MIME chunk size per RFC 2045 section 6.8.
        /// 
        /// <para>
        /// The {@value} character limit does not count the trailing CRLF, but counts all other characters, including any
        /// equal signs.
        /// </para>
        /// </summary>
        /// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045 section 6.8</a> </seealso>
        public const int MIME_CHUNK_SIZE = 76;

        /// <summary>
        /// PEM chunk size per RFC 1421 section 4.3.2.4.
        /// 
        /// <para>
        /// The {@value} character limit does not count the trailing CRLF, but counts all other characters, including any
        /// equal signs.
        /// </para>
        /// </summary>
        /// <seealso cref= <a href="http://tools.ietf.org/html/rfc1421">RFC 1421 section 4.3.2.4</a> </seealso>
        public const int PEM_CHUNK_SIZE = 64;

        private const int DEFAULT_BUFFER_RESIZE_FACTOR = 2;

        /// <summary>
        /// Defines the default buffer size - currently {@value}
        /// - must be large enough for at least one encoded block+separator
        /// </summary>
        private const int DEFAULT_BUFFER_SIZE = 8192;

        /// <summary>
        /// Mask used to extract 8 bits, used in decoding bytes </summary>
        protected internal const int MASK_8BITS = 0xff;

        /// <summary>
        /// Byte used to pad output.
        /// </summary>
        protected internal const sbyte PAD_DEFAULT = (sbyte)'='; // Allow static access to default

        /// @deprecated Use <seealso cref="#pad"/>. Will be removed in 2.0. 
        [Obsolete("Use . Will be removed in 2.0.")]
		internal readonly sbyte PAD = PAD_DEFAULT; // instance variable just in case it needs to vary later

        protected internal readonly sbyte pad; // instance variable just in case it needs to vary later

        /// <summary>
        /// Number of bytes in each full block of unencoded data, e.g. 4 for Base64 and 5 for Base32 </summary>
        private readonly int unencodedBlockSize;

        /// <summary>
        /// Number of bytes in each full block of encoded data, e.g. 3 for Base64 and 8 for Base32 </summary>
        private readonly int encodedBlockSize;

        /// <summary>
        /// Chunksize for encoding. Not used when decoding.
        /// A value of zero or less implies no chunking of the encoded data.
        /// Rounded down to nearest multiple of encodedBlockSize.
        /// </summary>
        protected internal readonly int lineLength;

        /// <summary>
        /// Size of chunk separator. Not used unless <seealso cref="#lineLength"/> &gt; 0.
        /// </summary>
        private readonly int chunkSeparatorLength;

        /// <summary>
        /// Note <code>lineLength</code> is rounded down to the nearest multiple of <seealso cref="#encodedBlockSize"/>
        /// If <code>chunkSeparatorLength</code> is zero, then chunking is disabled. </summary>
        /// <param name="unencodedBlockSize"> the size of an unencoded block (e.g. Base64 = 3) </param>
        /// <param name="encodedBlockSize"> the size of an encoded block (e.g. Base64 = 4) </param>
        /// <param name="lineLength"> if &gt; 0, use chunking with a length <code>lineLength</code> </param>
        /// <param name="chunkSeparatorLength"> the chunk separator length, if relevant </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected BaseNCodec(final int unencodedBlockSize, final int encodedBlockSize, final int lineLength, final int chunkSeparatorLength)
        protected internal BaseNCodec(int unencodedBlockSize, int encodedBlockSize, int lineLength, int chunkSeparatorLength) : this(unencodedBlockSize, encodedBlockSize, lineLength, chunkSeparatorLength, PAD_DEFAULT)
        {
        }

        /// <summary>
        /// Note <code>lineLength</code> is rounded down to the nearest multiple of <seealso cref="#encodedBlockSize"/>
        /// If <code>chunkSeparatorLength</code> is zero, then chunking is disabled. </summary>
        /// <param name="unencodedBlockSize"> the size of an unencoded block (e.g. Base64 = 3) </param>
        /// <param name="encodedBlockSize"> the size of an encoded block (e.g. Base64 = 4) </param>
        /// <param name="lineLength"> if &gt; 0, use chunking with a length <code>lineLength</code> </param>
        /// <param name="chunkSeparatorLength"> the chunk separator length, if relevant </param>
        /// <param name="pad"> byte used as padding byte. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected BaseNCodec(final int unencodedBlockSize, final int encodedBlockSize, final int lineLength, final int chunkSeparatorLength, final byte pad)
        protected internal BaseNCodec(int unencodedBlockSize, int encodedBlockSize, int lineLength, int chunkSeparatorLength, sbyte pad)
        {
            this.unencodedBlockSize = unencodedBlockSize;
            this.encodedBlockSize = encodedBlockSize;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final boolean useChunking = lineLength > 0 && chunkSeparatorLength > 0;
            bool useChunking = lineLength > 0 && chunkSeparatorLength > 0;
            this.lineLength = useChunking ? (lineLength / encodedBlockSize) * encodedBlockSize : 0;
            this.chunkSeparatorLength = chunkSeparatorLength;

            this.pad = pad;
        }

        /// <summary>
        /// Returns true if this object has buffered data for reading.
        /// </summary>
        /// <param name="context"> the context to be used </param>
        /// <returns> true if there is data still available for reading. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: boolean hasData(final Context context)
        internal virtual bool hasData(Context context)
        { // package protected for access from I/O streams
            return context.buffer != null;
        }

        /// <summary>
        /// Returns the amount of buffered data available for reading.
        /// </summary>
        /// <param name="context"> the context to be used </param>
        /// <returns> The amount of buffered data available for reading. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: int available(final Context context)
        internal virtual int available(Context context)
        { // package protected for access from I/O streams
            return context.buffer != null ? context.pos - context.readPos : 0;
        }

        /// <summary>
        /// Get the default buffer size. Can be overridden.
        /// </summary>
        /// <returns> <seealso cref="#DEFAULT_BUFFER_SIZE"/> </returns>
        protected internal virtual int DefaultBufferSize
        {
            get
            {
                return DEFAULT_BUFFER_SIZE;
            }
        }

        /// <summary>
        /// Increases our buffer by the <seealso cref="#DEFAULT_BUFFER_RESIZE_FACTOR"/>. </summary>
        /// <param name="context"> the context to be used </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: private byte[] resizeBuffer(final Context context)
        private sbyte[] resizeBuffer(Context context)
        {
            if (context.buffer == null)
            {
                context.buffer = new sbyte[DefaultBufferSize];
                context.pos = 0;
                context.readPos = 0;
            }
            else
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final byte[] b = new byte[context.buffer.length * DEFAULT_BUFFER_RESIZE_FACTOR];
                sbyte[] b = new sbyte[context.buffer.Length * DEFAULT_BUFFER_RESIZE_FACTOR];
                Array.Copy(context.buffer, 0, b, 0, context.buffer.Length);
                context.buffer = b;
            }
            return context.buffer;
        }

        /// <summary>
        /// Ensure that the buffer has room for <code>size</code> bytes
        /// </summary>
        /// <param name="size"> minimum spare space required </param>
        /// <param name="context"> the context to be used </param>
        /// <returns> the buffer </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected byte[] ensureBufferSize(final int size, final Context context)
         internal virtual sbyte[] ensureBufferSize(int size, Context context)
        {
            if ((context.buffer == null) || (context.buffer.Length < context.pos + size))
            {
                return resizeBuffer(context);
            }
            return context.buffer;
        }

        /// <summary>
        /// Extracts buffered data into the provided byte[] array, starting at position bPos, up to a maximum of bAvail
        /// bytes. Returns how many bytes were actually extracted.
        /// <para>
        /// Package protected for access from I/O streams.
        /// 
        /// </para>
        /// </summary>
        /// <param name="b">
        ///            byte[] array to extract the buffered data into. </param>
        /// <param name="bPos">
        ///            position in byte[] array to start extraction at. </param>
        /// <param name="bAvail">
        ///            amount of bytes we're allowed to extract. We may extract fewer (if fewer are available). </param>
        /// <param name="context">
        ///            the context to be used </param>
        /// <returns> The number of bytes successfully extracted into the provided byte[] array. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: int readResults(final byte[] b, final int bPos, final int bAvail, final Context context)
        internal virtual int readResults(sbyte[] b, int bPos, int bAvail, Context context)
        {
            if (context.buffer != null)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int len = Math.min(available(context), bAvail);
                int len = Math.Min(available(context), bAvail);
                Array.Copy(context.buffer, context.readPos, b, bPos, len);
                context.readPos += len;
                if (context.readPos >= context.pos)
                {
                    context.buffer = null; // so hasData() will return false, and this method can return -1
                }
                return len;
            }
            return context.eof ? EOF : 0;
        }

        /// <summary>
        /// Checks if a byte value is whitespace or not.
        /// Whitespace is taken to mean: space, tab, CR, LF </summary>
        /// <param name="byteToCheck">
        ///            the byte to check </param>
        /// <returns> true if byte is whitespace, false otherwise </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected static boolean isWhiteSpace(final byte byteToCheck)
        protected internal static bool isWhiteSpace(sbyte byteToCheck)
        {
            char flg = Convert.ToChar(byteToCheck);
            switch (flg)
            {
                case ' ':
                case '\n':
                case '\r':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Encodes an Object using the Base-N algorithm. This method is provided in order to satisfy the requirements of
        /// the Encoder interface, and will throw an EncoderException if the supplied object is not of type byte[].
        /// </summary>
        /// <param name="obj">
        ///            Object to encode </param>
        /// <returns> An object (of type byte[]) containing the Base-N encoded data which corresponds to the byte[] supplied. </returns>
        /// <exception cref="EncoderException">
        ///             if the parameter supplied is not of type byte[] </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public Object encode(final Object obj) throws org.apache.commons.codec.EncoderException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        public object encode(object obj)
        {
            if (!(obj is sbyte[]))
            {
                throw new Exception("Parameter supplied to Base-N encode is not a byte[]");
            }
            return encode((sbyte[])obj);
        }

        /// <summary>
        /// Encodes a byte[] containing binary data, into a String containing characters in the Base-N alphabet.
        /// Uses UTF8 encoding.
        /// </summary>
        /// <param name="pArray">
        ///            a byte array containing binary data </param>
        /// <returns> A String containing only Base-N character data </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public String encodeToString(final byte[] pArray)
        public virtual string encodeToString(sbyte[] pArray)
        {
            return System.Text.Encoding.UTF8.GetString(encode(pArray).Cast<byte>().ToArray());
            //return StringUtils.newStringUtf8(encode(pArray));
        }

        /// <summary>
        /// Encodes a byte[] containing binary data, into a String containing characters in the appropriate alphabet.
        /// Uses UTF8 encoding.
        /// </summary>
        /// <param name="pArray"> a byte array containing binary data </param>
        /// <returns> String containing only character data in the appropriate alphabet.
        /// @since 1.5
        /// This is a duplicate of <seealso cref="#encodeToString(byte[])"/>; it was merged during refactoring. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public String encodeAsString(final byte[] pArray)
        public virtual string encodeAsString(sbyte[] pArray)
        {
            return System.Text.Encoding.UTF8.GetString(encode(pArray).Cast<byte>().ToArray());
        }

        /// <summary>
        /// Decodes an Object using the Base-N algorithm. This method is provided in order to satisfy the requirements of
        /// the Decoder interface, and will throw a DecoderException if the supplied object is not of type byte[] or String.
        /// </summary>
        /// <param name="obj">
        ///            Object to decode </param>
        /// <returns> An object (of type byte[]) containing the binary data which corresponds to the byte[] or String
        ///         supplied. </returns>
        /// <exception cref="DecoderException">
        ///             if the parameter supplied is not of type byte[] </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public Object decode(final Object obj) throws org.apache.commons.codec.DecoderException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        public object decode(object obj)
        {
            if (obj is sbyte[])
            {
                return decode((sbyte[])obj);
            }
            else if (obj is string)
            {
                return decode((string)obj);
            }
            else
            {
                throw new Exception("Parameter supplied to Base-N decode is not a byte[] or a String");
            }
        }

        /// <summary>
        /// Decodes a String containing characters in the Base-N alphabet.
        /// </summary>
        /// <param name="pArray">
        ///            A String containing Base-N character data </param>
        /// <returns> a byte array containing binary data </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public byte[] decode(final String pArray)
        public virtual sbyte[] decode(string pArray)
        {
            return decode(System.Text.Encoding.UTF8.GetBytes(pArray).ConvertSbytes());
        }

        /// <summary>
        /// Decodes a byte[] containing characters in the Base-N alphabet.
        /// </summary>
        /// <param name="pArray">
        ///            A byte array containing Base-N character data </param>
        /// <returns> a byte array containing binary data </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: @Override public byte[] decode(final byte[] pArray)
        public sbyte[] decode(sbyte[] pArray)
        {
            if (pArray == null || pArray.Length == 0)
            {
                return pArray;
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Context context = new Context();
            Context context = new Context();
            decode(pArray, 0, pArray.Length, context);
            decode(pArray, 0, EOF, context); // Notify decoder of EOF.
                                             //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                                             //ORIGINAL LINE: final byte[] result = new byte[context.pos];
            sbyte[] result = new sbyte[context.pos];
            readResults(result, 0, result.Length, context);
            return result;
        }

        /// <summary>
        /// Encodes a byte[] containing binary data, into a byte[] containing characters in the alphabet.
        /// </summary>
        /// <param name="pArray">
        ///            a byte array containing binary data </param>
        /// <returns> A byte array containing only the base N alphabetic character data </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: @Override public byte[] encode(final byte[] pArray)
        public sbyte[] encode(sbyte[] pArray)
        {
            if (pArray == null || pArray.Length == 0)
            {
                return pArray;
            }
            return encode(pArray, 0, pArray.Length);
        }

        /// <summary>
        /// Encodes a byte[] containing binary data, into a byte[] containing
        /// characters in the alphabet.
        /// </summary>
        /// <param name="pArray">
        ///            a byte array containing binary data </param>
        /// <param name="offset">
        ///            initial offset of the subarray. </param>
        /// <param name="length">
        ///            length of the subarray. </param>
        /// <returns> A byte array containing only the base N alphabetic character data
        /// @since 1.11 </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public byte[] encode(final byte[] pArray, final int offset, final int length)
        public virtual sbyte[] encode(sbyte[] pArray, int offset, int length)
        {
            if (pArray == null || pArray.Length == 0)
            {
                return pArray;
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Context context = new Context();
            Context context = new Context();
            encode(pArray, offset, length, context);
            encode(pArray, offset, EOF, context); // Notify encoder of EOF.
                                                  //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                                                  //ORIGINAL LINE: final byte[] buf = new byte[context.pos - context.readPos];
            sbyte[] buf = new sbyte[context.pos - context.readPos];
            readResults(buf, 0, buf.Length, context);
            return buf;
        }

        // package protected for access from I/O streams
        internal abstract void encode(sbyte[] pArray, int i, int length, Context context);

        // package protected for access from I/O streams
        internal abstract void decode(sbyte[] pArray, int i, int length, Context context);

        /// <summary>
        /// Returns whether or not the <code>octet</code> is in the current alphabet.
        /// Does not allow whitespace or pad.
        /// </summary>
        /// <param name="value"> The value to test
        /// </param>
        /// <returns> <code>true</code> if the value is defined in the current alphabet, <code>false</code> otherwise. </returns>
        protected internal abstract bool isInAlphabet(sbyte value);

        /// <summary>
        /// Tests a given byte array to see if it contains only valid characters within the alphabet.
        /// The method optionally treats whitespace and pad as valid.
        /// </summary>
        /// <param name="arrayOctet"> byte array to test </param>
        /// <param name="allowWSPad"> if <code>true</code>, then whitespace and PAD are also allowed
        /// </param>
        /// <returns> <code>true</code> if all bytes are valid characters in the alphabet or if the byte array is empty;
        ///         <code>false</code>, otherwise </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public boolean isInAlphabet(final byte[] arrayOctet, final boolean allowWSPad)
        public virtual bool isInAlphabet(sbyte[] arrayOctet, bool allowWSPad)
        {
            foreach (sbyte octet in arrayOctet)
            {
                if (!isInAlphabet(octet) && (!allowWSPad || (octet != pad) && !isWhiteSpace(octet)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tests a given String to see if it contains only valid characters within the alphabet.
        /// The method treats whitespace and PAD as valid.
        /// </summary>
        /// <param name="basen"> String to test </param>
        /// <returns> <code>true</code> if all characters in the String are valid characters in the alphabet or if
        ///         the String is empty; <code>false</code>, otherwise </returns>
        /// <seealso cref= #isInAlphabet(byte[], boolean) </seealso>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public boolean isInAlphabet(final String basen)
        public virtual bool isInAlphabet(string basen)
        {
            return isInAlphabet( System.Text.Encoding.UTF8.GetBytes(basen).ConvertSbytes(), true);
        }

        /// <summary>
        /// Tests a given byte array to see if it contains any characters within the alphabet or PAD.
        /// 
        /// Intended for use in checking line-ending arrays
        /// </summary>
        /// <param name="arrayOctet">
        ///            byte array to test </param>
        /// <returns> <code>true</code> if any byte is a valid character in the alphabet or PAD; <code>false</code> otherwise </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected boolean containsAlphabetOrPad(final byte[] arrayOctet)
        protected internal virtual bool containsAlphabetOrPad(sbyte[] arrayOctet)
        {
            if (arrayOctet == null)
            {
                return false;
            }
            foreach (sbyte element in arrayOctet)
            {
                if (pad == element || isInAlphabet(element))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the amount of space needed to encode the supplied array.
        /// </summary>
        /// <param name="pArray"> byte[] array which will later be encoded
        /// </param>
        /// <returns> amount of space needed to encoded the supplied array.
        /// Returns a long since a max-len array will require &gt; Integer.MAX_VALUE </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public long getEncodedLength(final byte[] pArray)
        public virtual long getEncodedLength(sbyte[] pArray)
        {
            // Calculate non-chunked size - rounded up to allow for padding
            // cast to long is needed to avoid possibility of overflow
            long len = ((pArray.Length + unencodedBlockSize - 1) / unencodedBlockSize) * (long)encodedBlockSize;
            if (lineLength > 0)
            { // We're using chunking
              // Round up to nearest multiple
                len += ((len + lineLength - 1) / lineLength) * chunkSeparatorLength;
            }
            return len;
        }
    }

}
