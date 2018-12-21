/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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