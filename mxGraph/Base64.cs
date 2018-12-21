using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
    /// Provides Base64 encoding and decoding as defined by <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>.
    /// 
    /// <para>
    /// This class implements section <cite>6.8. Base64 Content-Transfer-Encoding</cite> from RFC 2045 <cite>Multipurpose
    /// Internet Mail Extensions (MIME) Part One: Format of Internet Message Bodies</cite> by Freed and Borenstein.
    /// </para>
    /// <para>
    /// The class can be parameterized in the following manner with various constructors:
    /// </para>
    /// <ul>
    /// <li>URL-safe mode: Default off.</li>
    /// <li>Line length: Default 76. Line length that aren't multiples of 4 will still essentially end up being multiples of
    /// 4 in the encoded data.
    /// <li>Line separator: Default is CRLF ("\r\n")</li>
    /// </ul>
    /// <para>
    /// The URL-safe parameter is only applied to encode operations. Decoding seamlessly handles both modes.
    /// </para>
    /// <para>
    /// Since this class operates directly on byte streams, and not character streams, it is hard-coded to only
    /// encode/decode character encodings which are compatible with the lower 127 ASCII chart (ISO-8859-1, Windows-1252,
    /// UTF-8, etc).
    /// </para>
    /// <para>
    /// This class is thread-safe.
    /// </para>
    /// </summary>
    /// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
    /// @since 1.0
    /// @version $Id: Base64.java 1789158 2017-03-28 15:04:58Z sebb $ </seealso>
    public class Base64 : BaseNCodec
    {

        /// <summary>
        /// BASE32 characters are 6 bits in length.
        /// They are formed by taking a block of 3 octets to form a 24-bit string,
        /// which is converted into 4 BASE64 characters.
        /// </summary>
        private const int BITS_PER_ENCODED_BYTE = 6;
        private const int BYTES_PER_UNENCODED_BLOCK = 3;
        private const int BYTES_PER_ENCODED_BLOCK = 4;

        /// <summary>
        /// Chunk separator per RFC 2045 section 2.1.
        /// 
        /// <para>
        /// N.B. The next major release may break compatibility and make this field private.
        /// </para>
        /// </summary>
        /// <seealso cref= <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045 section 2.1</a> </seealso>
        internal static readonly sbyte[] CHUNK_SEPARATOR = new sbyte[] { (sbyte)'\r', (sbyte)'\n' };

        /// <summary>
        /// This array is a lookup table that translates 6-bit positive integer index values into their "Base64 Alphabet"
        /// equivalents as specified in Table 1 of RFC 2045.
        /// 
        /// Thanks to "commons" project in ws.apache.org for this code.
        /// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        /// </summary>
        private static readonly sbyte[] STANDARD_ENCODE_TABLE = new sbyte[] { (sbyte)'A', (sbyte)'B', (sbyte)'C', (sbyte)'D', (sbyte)'E', (sbyte)'F', (sbyte)'G', (sbyte)'H', (sbyte)'I', (sbyte)'J', (sbyte)'K', (sbyte)'L', (sbyte)'M', (sbyte)'N', (sbyte)'O', (sbyte)'P', (sbyte)'Q', (sbyte)'R', (sbyte)'S', (sbyte)'T', (sbyte)'U', (sbyte)'V', (sbyte)'W', (sbyte)'X', (sbyte)'Y', (sbyte)'Z', (sbyte)'a', (sbyte)'b', (sbyte)'c', (sbyte)'d', (sbyte)'e', (sbyte)'f', (sbyte)'g', (sbyte)'h', (sbyte)'i', (sbyte)'j', (sbyte)'k', (sbyte)'l', (sbyte)'m', (sbyte)'n', (sbyte)'o', (sbyte)'p', (sbyte)'q', (sbyte)'r', (sbyte)'s', (sbyte)'t', (sbyte)'u', (sbyte)'v', (sbyte)'w', (sbyte)'x', (sbyte)'y', (sbyte)'z', (sbyte)'0', (sbyte)'1', (sbyte)'2', (sbyte)'3', (sbyte)'4', (sbyte)'5', (sbyte)'6', (sbyte)'7', (sbyte)'8', (sbyte)'9', (sbyte)'+', (sbyte)'/' };

        /// <summary>
        /// This is a copy of the STANDARD_ENCODE_TABLE above, but with + and /
        /// changed to - and _ to make the encoded Base64 results more URL-SAFE.
        /// This table is only used when the Base64's mode is set to URL-SAFE.
        /// </summary>
        private static readonly sbyte[] URL_SAFE_ENCODE_TABLE = new sbyte[] { (sbyte)'A', (sbyte)'B', (sbyte)'C', (sbyte)'D', (sbyte)'E', (sbyte)'F', (sbyte)'G', (sbyte)'H', (sbyte)'I', (sbyte)'J', (sbyte)'K', (sbyte)'L', (sbyte)'M', (sbyte)'N', (sbyte)'O', (sbyte)'P', (sbyte)'Q', (sbyte)'R', (sbyte)'S', (sbyte)'T', (sbyte)'U', (sbyte)'V', (sbyte)'W', (sbyte)'X', (sbyte)'Y', (sbyte)'Z', (sbyte)'a', (sbyte)'b', (sbyte)'c', (sbyte)'d', (sbyte)'e', (sbyte)'f', (sbyte)'g', (sbyte)'h', (sbyte)'i', (sbyte)'j', (sbyte)'k', (sbyte)'l', (sbyte)'m', (sbyte)'n', (sbyte)'o', (sbyte)'p', (sbyte)'q', (sbyte)'r', (sbyte)'s', (sbyte)'t', (sbyte)'u', (sbyte)'v', (sbyte)'w', (sbyte)'x', (sbyte)'y', (sbyte)'z', (sbyte)'0', (sbyte)'1', (sbyte)'2', (sbyte)'3', (sbyte)'4', (sbyte)'5', (sbyte)'6', (sbyte)'7', (sbyte)'8', (sbyte)'9', (sbyte)'-', (sbyte)'_' };

        /// <summary>
        /// This array is a lookup table that translates Unicode characters drawn from the "Base64 Alphabet" (as specified
        /// in Table 1 of RFC 2045) into their 6-bit positive integer equivalents. Characters that are not in the Base64
        /// alphabet but fall within the bounds of the array are translated to -1.
        /// 
        /// Note: '+' and '-' both decode to 62. '/' and '_' both decode to 63. This means decoder seamlessly handles both
        /// URL_SAFE and STANDARD base64. (The encoder, on the other hand, needs to know ahead of time what to emit).
        /// 
        /// Thanks to "commons" project in ws.apache.org for this code.
        /// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        /// </summary>
        private static readonly sbyte[] DECODE_TABLE = new sbyte[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, 62, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, 63, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51 };

        /// <summary>
        /// Base64 uses 6-bit fields.
        /// </summary>
        /// <summary>
        /// Mask used to extract 6 bits, used when encoding </summary>
        private const int MASK_6BITS = 0x3f;

        // The static final fields above are used for the original static byte[] methods on Base64.
        // The private member fields below are used with the new streaming approach, which requires
        // some state be preserved between calls of encode() and decode().

        /// <summary>
        /// Encode table to use: either STANDARD or URL_SAFE. Note: the DECODE_TABLE above remains static because it is able
        /// to decode both STANDARD and URL_SAFE streams, but the encodeTable must be a member variable so we can switch
        /// between the two modes.
        /// </summary>
        private readonly sbyte[] encodeTable;

        // Only one decode table currently; keep for consistency with Base32 code
        private readonly sbyte[] decodeTable = DECODE_TABLE;

        /// <summary>
        /// Line separator for encoding. Not used when decoding. Only used if lineLength &gt; 0.
        /// </summary>
        private readonly sbyte[] lineSeparator;

        /// <summary>
        /// Convenience variable to help us determine when our buffer is going to run out of room and needs resizing.
        /// <code>decodeSize = 3 + lineSeparator.length;</code>
        /// </summary>
        private readonly int decodeSize;

        /// <summary>
        /// Convenience variable to help us determine when our buffer is going to run out of room and needs resizing.
        /// <code>encodeSize = 4 + lineSeparator.length;</code>
        /// </summary>
        private readonly int encodeSize;

        /// <summary>
        /// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
        /// <para>
        /// When encoding the line length is 0 (no chunking), and the encoding table is STANDARD_ENCODE_TABLE.
        /// </para>
        /// 
        /// <para>
        /// When decoding all variants are supported.
        /// </para>
        /// </summary>
        public Base64() : this(0)
        {
        }

        /// <summary>
        /// Creates a Base64 codec used for decoding (all modes) and encoding in the given URL-safe mode.
        /// <para>
        /// When encoding the line length is 76, the line separator is CRLF, and the encoding table is STANDARD_ENCODE_TABLE.
        /// </para>
        /// 
        /// <para>
        /// When decoding all variants are supported.
        /// </para>
        /// </summary>
        /// <param name="urlSafe">
        ///            if <code>true</code>, URL-safe encoding is used. In most cases this should be set to
        ///            <code>false</code>.
        /// @since 1.4 </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public Base64(final boolean urlSafe)
        public Base64(bool urlSafe) : this(MIME_CHUNK_SIZE, CHUNK_SEPARATOR, urlSafe)
        {
        }

        /// <summary>
        /// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
        /// <para>
        /// When encoding the line length is given in the constructor, the line separator is CRLF, and the encoding table is
        /// STANDARD_ENCODE_TABLE.
        /// </para>
        /// <para>
        /// Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
        /// </para>
        /// <para>
        /// When decoding all variants are supported.
        /// </para>
        /// </summary>
        /// <param name="lineLength">
        ///            Each line of encoded data will be at most of the given length (rounded down to nearest multiple of
        ///            4). If lineLength &lt;= 0, then the output will not be divided into lines (chunks). Ignored when
        ///            decoding.
        /// @since 1.4 </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public Base64(final int lineLength)
        public Base64(int lineLength) : this(lineLength, CHUNK_SEPARATOR)
        {
        }

        /// <summary>
        /// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
        /// <para>
        /// When encoding the line length and line separator are given in the constructor, and the encoding table is
        /// STANDARD_ENCODE_TABLE.
        /// </para>
        /// <para>
        /// Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
        /// </para>
        /// <para>
        /// When decoding all variants are supported.
        /// </para>
        /// </summary>
        /// <param name="lineLength">
        ///            Each line of encoded data will be at most of the given length (rounded down to nearest multiple of
        ///            4). If lineLength &lt;= 0, then the output will not be divided into lines (chunks). Ignored when
        ///            decoding. </param>
        /// <param name="lineSeparator">
        ///            Each line of encoded data will end with this sequence of bytes. </param>
        /// <exception cref="IllegalArgumentException">
        ///             Thrown when the provided lineSeparator included some base64 characters.
        /// @since 1.4 </exception>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public Base64(final int lineLength, final byte[] lineSeparator)
        public Base64(int lineLength, sbyte[] lineSeparator) : this(lineLength, lineSeparator, false)
        {
        }

        /// <summary>
        /// Creates a Base64 codec used for decoding (all modes) and encoding in URL-unsafe mode.
        /// <para>
        /// When encoding the line length and line separator are given in the constructor, and the encoding table is
        /// STANDARD_ENCODE_TABLE.
        /// </para>
        /// <para>
        /// Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
        /// </para>
        /// <para>
        /// When decoding all variants are supported.
        /// </para>
        /// </summary>
        /// <param name="lineLength">
        ///            Each line of encoded data will be at most of the given length (rounded down to nearest multiple of
        ///            4). If lineLength &lt;= 0, then the output will not be divided into lines (chunks). Ignored when
        ///            decoding. </param>
        /// <param name="lineSeparator">
        ///            Each line of encoded data will end with this sequence of bytes. </param>
        /// <param name="urlSafe">
        ///            Instead of emitting '+' and '/' we emit '-' and '_' respectively. urlSafe is only applied to encode
        ///            operations. Decoding seamlessly handles both modes.
        ///            <b>Note: no padding is added when using the URL-safe alphabet.</b> </param>
        /// <exception cref="IllegalArgumentException">
        ///             The provided lineSeparator included some base64 characters. That's not going to work!
        /// @since 1.4 </exception>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public Base64(final int lineLength, final byte[] lineSeparator, final boolean urlSafe)
        public Base64(int lineLength, sbyte[] lineSeparator, bool urlSafe) : base(BYTES_PER_UNENCODED_BLOCK, BYTES_PER_ENCODED_BLOCK, lineLength, lineSeparator == null ? 0 : lineSeparator.Length)
        {
            // TODO could be simplified if there is no requirement to reject invalid line sep when length <=0
            // @see test case Base64Test.testConstructors()
            if (lineSeparator != null)
            {
                if (containsAlphabetOrPad(lineSeparator))
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final String sep = StringUtils.newStringUtf8(lineSeparator);
                    //string sep = StringUtils.newStringUtf8(lineSeparator);
                    string sep = System.Text.Encoding.UTF8.GetString(lineSeparator.ConvertBytes());
                    throw new System.ArgumentException("lineSeparator must not contain base64 characters: [" + sep + "]");
                }
                if (lineLength > 0)
                { // null line-sep forces no chunking rather than throwing IAE
                    this.encodeSize = BYTES_PER_ENCODED_BLOCK + lineSeparator.Length;
                    this.lineSeparator = new sbyte[lineSeparator.Length];
                    Array.Copy(lineSeparator, 0, this.lineSeparator, 0, lineSeparator.Length);
                }
                else
                {
                    this.encodeSize = BYTES_PER_ENCODED_BLOCK;
                    this.lineSeparator = null;
                }
            }
            else
            {
                this.encodeSize = BYTES_PER_ENCODED_BLOCK;
                this.lineSeparator = null;
            }
            this.decodeSize = this.encodeSize - 1;
            this.encodeTable = urlSafe ? URL_SAFE_ENCODE_TABLE : STANDARD_ENCODE_TABLE;
        }

        /// <summary>
        /// Returns our current encode mode. True if we're URL-SAFE, false otherwise.
        /// </summary>
        /// <returns> true if we're in URL-SAFE mode, false otherwise.
        /// @since 1.4 </returns>
        public virtual bool UrlSafe
        {
            get
            {
                return this.encodeTable == URL_SAFE_ENCODE_TABLE;
            }
        }

        /// <summary>
        /// <para>
        /// Encodes all of the provided data, starting at inPos, for inAvail bytes. Must be called at least twice: once with
        /// the data to encode, and once with inAvail set to "-1" to alert encoder that EOF has been reached, to flush last
        /// remaining bytes (if not multiple of 3).
        /// </para>
        /// <para><b>Note: no padding is added when encoding using the URL-safe alphabet.</b></para>
        /// <para>
        /// Thanks to "commons" project in ws.apache.org for the bitwise operations, and general approach.
        /// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        /// </para>
        /// </summary>
        /// <param name="in">
        ///            byte[] array of binary data to base64 encode. </param>
        /// <param name="inPos">
        ///            Position to start reading data from. </param>
        /// <param name="inAvail">
        ///            Amount of bytes available from input for encoding. </param>
        /// <param name="context">
        ///            the context to be used </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: @Override void encode(final byte[] in, int inPos, final int inAvail, final Context context)
        internal override void encode(sbyte[] @in, int inPos, int inAvail, Context context)
        {
            if (context.eof)
            {
                return;
            }
            // inAvail < 0 is how we're informed of EOF in the underlying data we're
            // encoding.
            if (inAvail < 0)
            {
                context.eof = true;
                if (0 == context.modulus && lineLength == 0)
                {
                    return; // no leftovers to process and not using chunking
                }
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final byte[] buffer = ensureBufferSize(encodeSize, context);
                sbyte[] buffer = ensureBufferSize(encodeSize, context);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int savedPos = context.pos;
                int savedPos = context.pos;
                switch (context.modulus)
                { // 0-2

                        //goto case 0;
					case 0 : // nothing to do here
						break;
					case 1 : // 8 bits = 6 + 2
						// top 6 bits:
						buffer[context.pos++] = encodeTable[(context.ibitWorkArea >> 2) & MASK_6BITS];
                // remaining 2:
                buffer[context.pos++] = encodeTable[(context.ibitWorkArea << 4) & MASK_6BITS];
                // URL-SAFE skips the padding to further reduce size.
                if (encodeTable == STANDARD_ENCODE_TABLE)
                {
                    buffer[context.pos++] = pad;
                    buffer[context.pos++] = pad;
                }
                break;

					case 2 : // 16 bits = 6 + 6 + 4
						buffer[context.pos++] = encodeTable[(context.ibitWorkArea >> 10) & MASK_6BITS];
                buffer[context.pos++] = encodeTable[(context.ibitWorkArea >> 4) & MASK_6BITS];
                buffer[context.pos++] = encodeTable[(context.ibitWorkArea << 2) & MASK_6BITS];
                // URL-SAFE skips the padding to further reduce size.
                if (encodeTable == STANDARD_ENCODE_TABLE)
                {
                    buffer[context.pos++] = pad;
                }
                break;
                default:
						throw new System.InvalidOperationException("Impossible modulus " + context.modulus);
            }
            context.currentLinePos += context.pos - savedPos; // keep track of current line position
                                                              // if currentPos == 0 we are at the start of a line, so don't add CRLF
            if (lineLength > 0 && context.currentLinePos > 0)
            {
                Array.Copy(lineSeparator, 0, buffer, context.pos, lineSeparator.Length);
                context.pos += lineSeparator.Length;
            }
        }
			else
			{
				for (int i = 0; i<inAvail; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] buffer = ensureBufferSize(encodeSize, context);
					sbyte[] buffer = ensureBufferSize(encodeSize, context);
        context.modulus = (context.modulus + 1) % BYTES_PER_UNENCODED_BLOCK;
					int b = @in[inPos++];
					if (b< 0)
					{
						b += 256;
					}
    context.ibitWorkArea = (context.ibitWorkArea << 8) + b; //  BITS_PER_BYTE
					if (0 == context.modulus)
					{ // 3 bytes = 24 bits = 4 * 6 bits to extract
						buffer[context.pos++] = encodeTable[(context.ibitWorkArea >> 18) & MASK_6BITS];
						buffer[context.pos++] = encodeTable[(context.ibitWorkArea >> 12) & MASK_6BITS];
						buffer[context.pos++] = encodeTable[(context.ibitWorkArea >> 6) & MASK_6BITS];
						buffer[context.pos++] = encodeTable[context.ibitWorkArea & MASK_6BITS];
						context.currentLinePos += BYTES_PER_ENCODED_BLOCK;
						if (lineLength > 0 && lineLength <= context.currentLinePos)
						{
							Array.Copy(lineSeparator, 0, buffer, context.pos, lineSeparator.Length);
							context.pos += lineSeparator.Length;
							context.currentLinePos = 0;
						}
					}
				}
			}
		}

		/// <summary>
		/// <para>
		/// Decodes all of the provided data, starting at inPos, for inAvail bytes. Should be called at least twice: once
		/// with the data to decode, and once with inAvail set to "-1" to alert decoder that EOF has been reached. The "-1"
		/// call is not necessary when decoding, but it doesn't hurt, either.
		/// </para>
		/// <para>
		/// Ignores all non-base64 characters. This is how chunked (e.g. 76 character) data is handled, since CR and LF are
		/// silently ignored, but has implications for other bytes, too. This method subscribes to the garbage-in,
		/// garbage-out philosophy: it will not check the provided data for validity.
		/// </para>
		/// <para>
		/// Thanks to "commons" project in ws.apache.org for the bitwise operations, and general approach.
		/// http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
		/// </para>
		/// </summary>
		/// <param name="in">
		///            byte[] array of ascii data to base64 decode. </param>
		/// <param name="inPos">
		///            Position to start reading data from. </param>
		/// <param name="inAvail">
		///            Amount of bytes available from input for encoding. </param>
		/// <param name="context">
		///            the context to be used </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override void decode(final byte[] in, int inPos, final int inAvail, final Context context)
		internal override void decode(sbyte[] @in, int inPos, int inAvail, Context context)
{
    if (context.eof)
    {
        return;
    }
    if (inAvail < 0)
    {
        context.eof = true;
    }
    for (int i = 0; i < inAvail; i++)
    {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final byte[] buffer = ensureBufferSize(decodeSize, context);
        sbyte[] buffer = ensureBufferSize(decodeSize, context);
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final byte b = in[inPos++];
        sbyte b = @in[inPos++];
        if (b == pad)
        {
            // We're done.
            context.eof = true;
            break;
        }
        if (b >= 0 && b < DECODE_TABLE.Length)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int result = DECODE_TABLE[b];
            int result = DECODE_TABLE[b];
            if (result >= 0)
            {
                context.modulus = (context.modulus + 1) % BYTES_PER_ENCODED_BLOCK;
                context.ibitWorkArea = (context.ibitWorkArea << BITS_PER_ENCODED_BYTE) + result;
                if (context.modulus == 0)
                {
                    buffer[context.pos++] = (sbyte)((context.ibitWorkArea >> 16) & MASK_8BITS);
                    buffer[context.pos++] = (sbyte)((context.ibitWorkArea >> 8) & MASK_8BITS);
                    buffer[context.pos++] = (sbyte)(context.ibitWorkArea & MASK_8BITS);
                }
            }
        }
    }

    // Two forms of EOF as far as base64 decoder is concerned: actual
    // EOF (-1) and first time '=' character is encountered in stream.
    // This approach makes the '=' padding characters completely optional.
    if (context.eof && context.modulus != 0)
    {
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final byte[] buffer = ensureBufferSize(decodeSize, context);
        sbyte[] buffer = ensureBufferSize(decodeSize, context);

        // We have some spare bits remaining
        // Output all whole multiples of 8 bits and ignore the rest
        switch (context.modulus)
        {
            //              case 0 : // impossible, as excluded above
            case 1: // 6 bits - ignore entirely
                    // TODO not currently tested; perhaps it is impossible?
                break;
            case 2: // 12 bits = 8 + 4
                context.ibitWorkArea = context.ibitWorkArea >> 4; // dump the extra 4 bits
                buffer[context.pos++] = (sbyte)((context.ibitWorkArea) & MASK_8BITS);
                break;
            case 3: // 18 bits = 8 + 8 + 2
                context.ibitWorkArea = context.ibitWorkArea >> 2; // dump 2 bits
                buffer[context.pos++] = (sbyte)((context.ibitWorkArea >> 8) & MASK_8BITS);
                buffer[context.pos++] = (sbyte)((context.ibitWorkArea) & MASK_8BITS);
                break;
            default:
                throw new System.InvalidOperationException("Impossible modulus " + context.modulus);
        }
    }
}

/// <summary>
/// Tests a given byte array to see if it contains only valid characters within the Base64 alphabet. Currently the
/// method treats whitespace as valid.
/// </summary>
/// <param name="arrayOctet">
///            byte array to test </param>
/// <returns> <code>true</code> if all bytes are valid characters in the Base64 alphabet or if the byte array is empty;
///         <code>false</code>, otherwise </returns>
/// @deprecated 1.5 Use <seealso cref="#isBase64(byte[])"/>, will be removed in 2.0. 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Deprecated("1.5 Use <seealso cref="#isBase64(byte[])"/>, will be removed in 2.0.") public static boolean isArrayByteBase64(final byte[] arrayOctet)
[Obsolete("1.5 will be removed in 2.0.")]
		public static bool isArrayByteBase64(sbyte[] arrayOctet)
{
    return isBase64(arrayOctet);
}

/// <summary>
/// Returns whether or not the <code>octet</code> is in the base 64 alphabet.
/// </summary>
/// <param name="octet">
///            The value to test </param>
/// <returns> <code>true</code> if the value is defined in the the base 64 alphabet, <code>false</code> otherwise.
/// @since 1.4 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static boolean isBase64(final byte octet)
public static bool isBase64(sbyte octet)
{
    return octet == PAD_DEFAULT || (octet >= 0 && octet < DECODE_TABLE.Length && DECODE_TABLE[octet] != -1);
}

/// <summary>
/// Tests a given String to see if it contains only valid characters within the Base64 alphabet. Currently the
/// method treats whitespace as valid.
/// </summary>
/// <param name="base64">
///            String to test </param>
/// <returns> <code>true</code> if all characters in the String are valid characters in the Base64 alphabet or if
///         the String is empty; <code>false</code>, otherwise
///  @since 1.5 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static boolean isBase64(final String base64)
public static bool isBase64(string base64)
{
    return isBase64(System.Text.Encoding.UTF8.GetBytes(base64).ConvertSbytes());
}

/// <summary>
/// Tests a given byte array to see if it contains only valid characters within the Base64 alphabet. Currently the
/// method treats whitespace as valid.
/// </summary>
/// <param name="arrayOctet">
///            byte array to test </param>
/// <returns> <code>true</code> if all bytes are valid characters in the Base64 alphabet or if the byte array is empty;
///         <code>false</code>, otherwise
/// @since 1.5 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static boolean isBase64(final byte[] arrayOctet)
public static bool isBase64(sbyte[] arrayOctet)
{
    for (int i = 0; i < arrayOctet.Length; i++)
    {
        if (!isBase64(arrayOctet[i]) && !isWhiteSpace(arrayOctet[i]))
        {
            return false;
        }
    }
    return true;
}

/// <summary>
/// Encodes binary data using the base64 algorithm but does not chunk the output.
/// </summary>
/// <param name="binaryData">
///            binary data to encode </param>
/// <returns> byte[] containing Base64 characters in their UTF-8 representation. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeBase64(final byte[] binaryData)
public static sbyte[] encodeBase64(sbyte[] binaryData)
{
    return encodeBase64(binaryData, false);
}

/// <summary>
/// Encodes binary data using the base64 algorithm but does not chunk the output.
/// 
/// NOTE:  We changed the behaviour of this method from multi-line chunking (commons-codec-1.4) to
/// single-line non-chunking (commons-codec-1.5).
/// </summary>
/// <param name="binaryData">
///            binary data to encode </param>
/// <returns> String containing Base64 characters.
/// @since 1.4 (NOTE:  1.4 chunked the output, whereas 1.5 does not). </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static String encodeBase64String(final byte[] binaryData)
public static string encodeBase64String(sbyte[] binaryData)
{
           return System.Text.Encoding.ASCII.GetString(encodeBase64(binaryData, false).ConvertBytes());
    //return StringUtils.newStringUsAscii(encodeBase64(binaryData, false));
}

/// <summary>
/// Encodes binary data using a URL-safe variation of the base64 algorithm but does not chunk the output. The
/// url-safe variation emits - and _ instead of + and / characters.
/// <b>Note: no padding is added.</b> </summary>
/// <param name="binaryData">
///            binary data to encode </param>
/// <returns> byte[] containing Base64 characters in their UTF-8 representation.
/// @since 1.4 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeBase64URLSafe(final byte[] binaryData)
public static sbyte[] encodeBase64URLSafe(sbyte[] binaryData)
{
    return encodeBase64(binaryData, false, true);
}

/// <summary>
/// Encodes binary data using a URL-safe variation of the base64 algorithm but does not chunk the output. The
/// url-safe variation emits - and _ instead of + and / characters.
/// <b>Note: no padding is added.</b> </summary>
/// <param name="binaryData">
///            binary data to encode </param>
/// <returns> String containing Base64 characters
/// @since 1.4 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static String encodeBase64URLSafeString(final byte[] binaryData)
public static string encodeBase64URLSafeString(sbyte[] binaryData)
{
            return System.Text.Encoding.ASCII.GetString(encodeBase64(binaryData, false, true).ConvertBytes());
            //return StringUtils.newStringUsAscii(encodeBase64(binaryData, false, true));
        }

/// <summary>
/// Encodes binary data using the base64 algorithm and chunks the encoded output into 76 character blocks
/// </summary>
/// <param name="binaryData">
///            binary data to encode </param>
/// <returns> Base64 characters chunked in 76 character blocks </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeBase64Chunked(final byte[] binaryData)
public static sbyte[] encodeBase64Chunked(sbyte[] binaryData)
{
    return encodeBase64(binaryData, true);
}

/// <summary>
/// Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
/// </summary>
/// <param name="binaryData">
///            Array containing binary data to encode. </param>
/// <param name="isChunked">
///            if <code>true</code> this encoder will chunk the base64 output into 76 character blocks </param>
/// <returns> Base64-encoded data. </returns>
/// <exception cref="IllegalArgumentException">
///             Thrown when the input array needs an output array bigger than <seealso cref="Integer#MAX_VALUE"/> </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeBase64(final byte[] binaryData, final boolean isChunked)
public static sbyte[] encodeBase64(sbyte[] binaryData, bool isChunked)
{
    return encodeBase64(binaryData, isChunked, false);
}

/// <summary>
/// Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
/// </summary>
/// <param name="binaryData">
///            Array containing binary data to encode. </param>
/// <param name="isChunked">
///            if <code>true</code> this encoder will chunk the base64 output into 76 character blocks </param>
/// <param name="urlSafe">
///            if <code>true</code> this encoder will emit - and _ instead of the usual + and / characters.
///            <b>Note: no padding is added when encoding using the URL-safe alphabet.</b> </param>
/// <returns> Base64-encoded data. </returns>
/// <exception cref="IllegalArgumentException">
///             Thrown when the input array needs an output array bigger than <seealso cref="Integer#MAX_VALUE"/>
/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeBase64(final byte[] binaryData, final boolean isChunked, final boolean urlSafe)
public static sbyte[] encodeBase64(sbyte[] binaryData, bool isChunked, bool urlSafe)
{
    return encodeBase64(binaryData, isChunked, urlSafe, int.MaxValue);
}

/// <summary>
/// Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
/// </summary>
/// <param name="binaryData">
///            Array containing binary data to encode. </param>
/// <param name="isChunked">
///            if <code>true</code> this encoder will chunk the base64 output into 76 character blocks </param>
/// <param name="urlSafe">
///            if <code>true</code> this encoder will emit - and _ instead of the usual + and / characters.
///            <b>Note: no padding is added when encoding using the URL-safe alphabet.</b> </param>
/// <param name="maxResultSize">
///            The maximum result size to accept. </param>
/// <returns> Base64-encoded data. </returns>
/// <exception cref="IllegalArgumentException">
///             Thrown when the input array needs an output array bigger than maxResultSize
/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeBase64(final byte[] binaryData, final boolean isChunked, final boolean urlSafe, final int maxResultSize)
public static sbyte[] encodeBase64(sbyte[] binaryData, bool isChunked, bool urlSafe, int maxResultSize)
{
    if (binaryData == null || binaryData.Length == 0)
    {
        return binaryData;
    }

    // Create this so can use the super-class method
    // Also ensures that the same roundings are performed by the ctor and the code
    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final Base64 b64 = isChunked ? new Base64(urlSafe) : new Base64(0, CHUNK_SEPARATOR, urlSafe);
    Base64 b64 = isChunked ? new Base64(urlSafe) : new Base64(0, CHUNK_SEPARATOR, urlSafe);
    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final long len = b64.getEncodedLength(binaryData);
    long len = b64.getEncodedLength(binaryData);
    if (len > maxResultSize)
    {
        throw new System.ArgumentException("Input array too big, the output array would be bigger (" + len + ") than the specified maximum size of " + maxResultSize);
    }

    return b64.encode(binaryData);
}

/// <summary>
/// Decodes a Base64 String into octets.
/// <para>
/// <b>Note:</b> this method seamlessly handles data encoded in URL-safe or normal mode.
/// </para>
/// </summary>
/// <param name="base64String">
///            String containing Base64 data </param>
/// <returns> Array containing decoded data.
/// @since 1.4 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] decodeBase64(final String base64String)
public static sbyte[] decodeBase64(string base64String)
{
    return (new Base64()).decode(base64String);
}

/// <summary>
/// Decodes Base64 data into octets.
/// <para>
/// <b>Note:</b> this method seamlessly handles data encoded in URL-safe or normal mode.
/// </para>
/// </summary>
/// <param name="base64Data">
///            Byte array containing Base64 data </param>
/// <returns> Array containing decoded data. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] decodeBase64(final byte[] base64Data)
public static sbyte[] decodeBase64(sbyte[] base64Data)
{
    return (new Base64()).decode(base64Data);
}

// Implementation of the Encoder Interface

// Implementation of integer encoding used for crypto
/// <summary>
/// Decodes a byte64-encoded integer according to crypto standards such as W3C's XML-Signature.
/// </summary>
/// <param name="pArray">
///            a byte array containing base64 character data </param>
/// <returns> A BigInteger
/// @since 1.4 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static java.math.BigInteger decodeInteger(final byte[] pArray)
//public static System.Numerics.BigInteger decodeInteger(sbyte[] pArray)
//{
//            Convert.
//    return new System.Numerics.BigInteger(1, decodeBase64(pArray));
//}

/// <summary>
/// Encodes to a byte64-encoded integer according to crypto standards such as W3C's XML-Signature.
/// </summary>
/// <param name="bigInt">
///            a BigInteger </param>
/// <returns> A byte array containing base64 character data </returns>
/// <exception cref="NullPointerException">
///             if null is passed in
/// @since 1.4 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static byte[] encodeInteger(final java.math.BigInteger bigInt)
//public static sbyte[] encodeInteger(System.Numerics.BigInteger bigInt)
//{
//    if (bigInt == null)
//    {
//        throw new System.NullReferenceException("encodeInteger called with null parameter");
//    }
//    return encodeBase64(toIntegerBytes(bigInt), false);
//}

/// <summary>
/// Returns a byte-array representation of a <code>BigInteger</code> without sign bit.
/// </summary>
/// <param name="bigInt">
///            <code>BigInteger</code> to be converted </param>
/// <returns> a byte array representation of the BigInteger parameter </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static byte[] toIntegerBytes(final java.math.BigInteger bigInt)
//internal static sbyte[] toIntegerBytes(System.Numerics.BigInteger bigInt)
//{
//    int bitlen = bigInt.bitLength();
//    // round bitlen
//    bitlen = ((bitlen + 7) >> 3) << 3;
//    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//    //ORIGINAL LINE: final byte[] bigBytes = bigInt.toByteArray();
//    sbyte[] bigBytes = bigInt.toByteArray();

//    if (((bigInt.bitLength() % 8) != 0) && (((bigInt.bitLength() / 8) + 1) == (bitlen / 8)))
//    {
//        return bigBytes;
//    }
//    // set up params for copying everything but sign bit
//    int startSrc = 0;
//    int len = bigBytes.Length;

//    // if bigInt is exactly byte-aligned, just skip signbit in copy
//    if ((bigInt.bitLength() % 8) == 0)
//    {
//        startSrc = 1;
//        len--;
//    }
//    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//    //ORIGINAL LINE: final int startDst = bitlen / 8 - len;
//    int startDst = bitlen / 8 - len; // to pad w/ nulls as per spec
//                                     //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                                     //ORIGINAL LINE: final byte[] resizedBytes = new byte[bitlen / 8];
//    sbyte[] resizedBytes = new sbyte[bitlen / 8];
//    Array.Copy(bigBytes, startSrc, resizedBytes, startDst, len);
//    return resizedBytes;
//}

/// <summary>
/// Returns whether or not the <code>octet</code> is in the Base64 alphabet.
/// </summary>
/// <param name="octet">
///            The value to test </param>
/// <returns> <code>true</code> if the value is defined in the the Base64 alphabet <code>false</code> otherwise. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected boolean isInAlphabet(final byte octet)
protected internal override bool isInAlphabet(sbyte octet)
{
    return octet >= 0 && octet < decodeTable.Length && decodeTable[octet] != -1;
}
}
}
