using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// $Id: mxObjectCodec.java,v 1.1 2010-11-30 19:41:25 david Exp $
/// Copyright (c) 2006, Gaudenz Alder
/// </summary>
namespace mxGraph.io
{


	using Element = System.Xml.XmlElement;
    using NamedNodeMap = System.Xml.XmlNamedNodeMap;
    using Node = System.Xml.XmlNode;

	using mxUtils = mxGraph.util.mxUtils;

	/// <summary>
	/// Generic codec for Java objects. See below for a detailed description of
	/// the encoding/decoding scheme.
	/// 
	/// Note: Since booleans are numbers in JavaScript, all boolean values are
	/// encoded into 1 for true and 0 for false.
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public class mxObjectCodec
	public class mxObjectCodec
	{

		/// <summary>
		/// Immutable empty set.
		/// </summary>
		private static ISet<string> EMPTY_SET = new HashSet<string>();

		/// <summary>
		/// Holds the template object associated with this codec.
		/// </summary>
		protected internal object template;

		/// <summary>
		/// Array containing the variable names that should be ignored by the codec.
		/// </summary>
		protected internal ISet<string> exclude;

		/// <summary>
		/// Array containing the variable names that should be turned into or
		/// converted from references. See <mxCodec.getId> and <mxCodec.getObject>.
		/// </summary>
		protected internal ISet<string> idrefs;

		/// <summary>
		/// Maps from from fieldnames to XML attribute names.
		/// </summary>
		protected internal IDictionary<string, string> mapping;

		/// <summary>
		/// Maps from from XML attribute names to fieldnames.
		/// </summary>
		protected internal IDictionary<string, string> reverse;

		/// <summary>
		/// Constructs a new codec for the specified template object.
		/// </summary>
		public mxObjectCodec(object template) : this(template, null, null, null)
		{
		}

		/// <summary>
		/// Constructs a new codec for the specified template object. The variables
		/// in the optional exclude array are ignored by the codec. Variables in the
		/// optional idrefs array are turned into references in the XML. The
		/// optional mapping may be used to map from variable names to XML
		/// attributes. The argument is created as follows:
		/// </summary>
		/// <param name="template"> Prototypical instance of the object to be encoded/decoded. </param>
		/// <param name="exclude"> Optional array of fieldnames to be ignored. </param>
		/// <param name="idrefs"> Optional array of fieldnames to be converted to/from references. </param>
		/// <param name="mapping"> Optional mapping from field- to attributenames. </param>
		public mxObjectCodec(object template, string[] exclude, string[] idrefs, IDictionary<string, string> mapping)
		{
			this.template = template;

			if (exclude != null)
			{
				this.exclude = new HashSet<string>();

				for (int i = 0; i < exclude.Length; i++)
				{
					this.exclude.Add(exclude[i]);
				}
			}
			else
			{
				this.exclude = EMPTY_SET;
			}

			if (idrefs != null)
			{
				this.idrefs = new HashSet<string>();

				for (int i = 0; i < idrefs.Length; i++)
				{
					this.idrefs.Add(idrefs[i]);
				}
			}
			else
			{
				this.idrefs = EMPTY_SET;
			}

			if (mapping == null)
			{
				mapping = new Dictionary<string, string>();
			}

			this.mapping = mapping;

            reverse = new Dictionary<string, string>();
            //IEnumerator<KeyValuePair<string, string>> it = mapping.SetOfKeyValuePairs().GetEnumerator();

            //while (it.MoveNext())
            //{
            //	KeyValuePair<string, string> e = it.Current;
            //	reverse[e.Value] = e.Key;
            //}

            foreach (var item in mapping)
            {
                reverse.Add(item.Value, item.Key);
            }
		}

		/// <summary>
		/// Returns the name used for the nodenames and lookup of the codec when
		/// classes are encoded and nodes are decoded. For classes to work with
		/// this the codec registry automatically adds an alias for the classname
		/// if that is different than what this returns. The default implementation
		/// returns the classname of the template class.
		/// 
		/// Here is an example on how to use this for renaming mxCell nodes:
		/// <code>
		/// mxCodecRegistry.register(new mxCellCodec()
		/// {
		///   public String getName()
		///   {
		///     return "anotherName";
		///   }
		/// });
		/// </code>
		/// </summary>
		public virtual string Name
		{
			get
			{
				return mxCodecRegistry.getName(Template);
			}
		}

		/// <summary>
		/// Returns the template object associated with this codec.
		/// </summary>
		/// <returns> Returns the template object. </returns>
		public virtual object Template
		{
			get
			{
				return template;
			}
		}

		/// <summary>
		/// Returns a new instance of the template object for representing the given
		/// node.
		/// </summary>
		/// <param name="node"> XML node that the object is going to represent. </param>
		/// <returns> Returns a new template instance. </returns>
		protected internal virtual object cloneTemplate(Node node)
		{
			object obj = null;

			try
			{
				if (template.GetType().IsEnum)
				{
					obj = template.GetType().GetEnumValues().GetValue(0);
				}
				else
				{
					obj = template.GetType().Assembly.CreateInstance(template.GetType().FullName);
				}

				// Special case: Check if the collection
				// should be a map. This is if the first
				// child has an "as"-attribute. This
				// assumes that all childs will have
				// as attributes in this case. This is
				// required because in JavaScript, the
				// map and array object are the same.
				if (obj is ICollection)
				{
					node = node.FirstChild;

                    if (node != null && node is Element && ((Element) node).HasAttribute("as"))
					{
						obj = new Dictionary<object, object>();
					}
				}
			}
			catch (Exception e)
			{
				// ignore
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return obj;
		}

		/// <summary>
		/// Returns true if the given attribute is to be ignored by the codec. This
		/// implementation returns true if the given fieldname is in
		/// <seealso cref="#exclude"/>.
		/// </summary>
		/// <param name="obj"> Object instance that contains the field. </param>
		/// <param name="attr"> Fieldname of the field. </param>
		/// <param name="value"> Value of the field. </param>
		/// <param name="write"> Boolean indicating if the field is being encoded or
		/// decoded. write is true if the field is being encoded, else it is
		/// being decoded. </param>
		/// <returns> Returns true if the given attribute should be ignored. </returns>
		public virtual bool isExcluded(object obj, string attr, object value, bool write)
		{
			return exclude.Contains(attr);
		}

		/// <summary>
		/// Returns true if the given fieldname is to be treated as a textual
		/// reference (ID). This implementation returns true if the given fieldname
		/// is in <seealso cref="#idrefs"/>.
		/// </summary>
		/// <param name="obj"> Object instance that contains the field. </param>
		/// <param name="attr"> Fieldname of the field. </param>
		/// <param name="value"> Value of the field. </param>
		/// <param name="isWrite"> Boolean indicating if the field is being encoded or
		/// decoded. isWrite is true if the field is being encoded, else it is being
		/// decoded. </param>
		/// <returns> Returns true if the given attribute should be handled as a
		/// reference. </returns>
		public virtual bool isReference(object obj, string attr, object value, bool isWrite)
		{
			return idrefs.Contains(attr);
		}

		/// <summary>
		/// Encodes the specified object and returns a node representing then given
		/// object. Calls beforeEncode after creating the node and afterEncode
		/// with the resulting node after processing.
		/// 
		/// Enc is a reference to the calling encoder. It is used to encode complex
		/// objects and create references.
		/// 
		/// This implementation encodes all variables of an object according to the
		/// following rules:
		/// 
		/// <ul>
		/// <li>If the variable name is in <seealso cref="#exclude"/> then it is ignored.</li>
		/// <li>If the variable name is in <seealso cref="#idrefs"/> then
		/// <seealso cref="mxCodec#getId(Object)"/> is used to replace the object with its ID.
		/// </li>
		/// <li>The variable name is mapped using <seealso cref="#mapping"/>.</li>
		/// <li>If obj is an array and the variable name is numeric (ie. an index) then it
		/// is not encoded.</li>
		/// <li>If the value is an object, then the codec is used to create a child
		/// node with the variable name encoded into the "as" attribute.</li>
		/// <li>Else, if <seealso cref="mxGraphio.mxCodec#isEncodeDefaults()"/> is true or
		/// the value differs from the template value, then ...
		/// <ul>
		/// <li>... if obj is not an array, then the value is mapped to an
		/// attribute.</li>
		/// <li>... else if obj is an array, the value is mapped to an add child
		/// with a value attribute or a text child node, if the value is a function.
		/// </li>
		/// </ul>
		/// </li>
		/// </ul>
		/// 
		/// If no ID exists for a variable in <seealso cref="#idrefs"/> or if an object cannot be
		/// encoded, a warning is printed to System.err.
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object to be encoded. </param>
		/// <returns> Returns the resulting XML node that represents the given object.  </returns>
		public virtual Node encode(mxCodec enc, object obj)
		{
            Node node = enc.document.CreateElement(Name);

			obj = beforeEncode(enc, obj, node);
			encodeObject(enc, obj, node);

			return afterEncode(enc, obj, node);
		}

		/// <summary>
		/// Encodes the value of each member in then given obj
		/// into the given node using <seealso cref="#encodeFields(mxCodec, Object, Node)"/>
		/// and <seealso cref="#encodeElements(mxCodec, Object, Node)"/>.
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object to be encoded. </param>
		/// <param name="node"> XML node that contains the encoded object. </param>
		protected internal virtual void encodeObject(mxCodec enc, object obj, Node node)
		{
			mxCodec.setAttribute(node, "id", enc.getId(obj));
			encodeFields(enc, obj, node);
			encodeElements(enc, obj, node);
		}

		/// <summary>
		/// Encodes the declared fields of the given object into the given node.
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object whose fields should be encoded. </param>
		/// <param name="node"> XML node that contains the encoded object. </param>
		protected internal virtual void encodeFields(mxCodec enc, object obj, Node node)
		{
			Type type = obj.GetType();

			while (type != null)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

				for (int i = 0; i < fields.Length; i++)
				{
                    FieldInfo f = fields[i];

					if ((!f.IsNotSerialized))
					{
						string fieldname = f.Name;
						object value = getFieldValue(obj, fieldname);
						encodeValue(enc, obj, fieldname, value, node);
					}
				}

				type = type.BaseType;
			}
		}

		/// <summary>
		/// Encodes the child objects of arrays, maps and collections.
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object whose child objects should be encoded. </param>
		/// <param name="node"> XML node that contains the encoded object. </param>
		protected internal virtual void encodeElements(mxCodec enc, object obj, Node node)
		{
			if (obj.GetType().IsArray)
			{
				object[] tmp = (object[]) obj;

				for (int i = 0; i < tmp.Length; i++)
				{
					encodeValue(enc, obj, null, tmp[i], node);
				}
			}
			else if (obj is IDictionary)
			{
                Dictionary<object, object> it = ((Dictionary<object,object>) obj);

                foreach (var item in it)
                {
                    encodeValue(enc, obj, item.Key.ToString(), item.Value, node);
                }

				
			}
			else if (obj is ICollection)
			{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Iterator<?> it = ((java.util.Collection<?>) obj).iterator();
				IEnumerator<object> it = ((ICollection<object>) obj).GetEnumerator();

				while (it.MoveNext())
				{
					object value = it.Current;
					encodeValue(enc, obj, null, value, node);
				}
			}
		}

		/// <summary>
		/// Converts the given value according to the mappings
		/// and id-refs in this codec and uses
		/// <seealso cref="#writeAttribute(mxCodec, Object, String, Object, Node)"/>
		/// to write the attribute into the given node.
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object whose field is going to be encoded. </param>
		/// <param name="fieldname"> Name if the field to be encoded. </param>
		/// <param name="value"> Value of the property to be encoded. </param>
		/// <param name="node"> XML node that contains the encoded object. </param>
		protected internal virtual void encodeValue(mxCodec enc, object obj, string fieldname, object value, Node node)
		{
			if (value != null && !isExcluded(obj, fieldname, value, true))
			{
				if (isReference(obj, fieldname, value, true))
				{
					object tmp = enc.getId(value);

					if (tmp == null)
					{
						Console.Error.WriteLine("mxObjectCodec.encode: No ID for " + Name + "." + fieldname + "=" + value);
						return; // exit
					}

					value = tmp;
				}

				object defaultValue = getFieldValue(template, fieldname);

				if (string.ReferenceEquals(fieldname, null) || enc.EncodeDefaults || defaultValue == null || !defaultValue.Equals(value))
				{
					writeAttribute(enc, obj, getAttributeName(fieldname), value, node);
				}
			}
		}

		/// <summary>
		/// Returns true if the given object is a primitive value.
		/// </summary>
		/// <param name="value"> Object that should be checked. </param>
		/// <returns> Returns true if the given object is a primitive value. </returns>
		protected internal virtual bool isPrimitiveValue(object value)
		{
			return value is string || value is bool? || value is char? || value is sbyte? || value is short? || value is int? || value is long? || value is float? || value is double? || value.GetType().IsPrimitive;
		}

		/// <summary>
		/// Writes the given value into node using writePrimitiveAttribute
		/// or writeComplexAttribute depending on the type of the value.
		/// </summary>
		protected internal virtual void writeAttribute(mxCodec enc, object obj, string attr, object value, Node node)
		{
			value = convertValueToXml(value);

			if (isPrimitiveValue(value))
			{
				writePrimitiveAttribute(enc, obj, attr, value, node);
			}
			else
			{
				writeComplexAttribute(enc, obj, attr, value, node);
			}
		}

		/// <summary>
		/// Writes the given value as an attribute of the given node.
		/// </summary>
		protected internal virtual void writePrimitiveAttribute(mxCodec enc, object obj, string attr, object value, Node node)
		{
			if (string.ReferenceEquals(attr, null) || obj is IDictionary)
			{
                Node child = enc.document.CreateElement("add");

				if (!string.ReferenceEquals(attr, null))
				{
					mxCodec.setAttribute(child, "as", attr);
				}

				mxCodec.setAttribute(child, "value", value);
                node.AppendChild(child);
			}
			else
			{
				mxCodec.setAttribute(node, attr, value);
			}
		}

		/// <summary>
		/// Writes the given value as a child node of the given node.
		/// </summary>
		protected internal virtual void writeComplexAttribute(mxCodec enc, object obj, string attr, object value, Node node)
		{
			Node child = enc.encode(value);

			if (child != null)
			{
				if (!string.ReferenceEquals(attr, null))
				{
					mxCodec.setAttribute(child, "as", attr);
				}


                node.AppendChild(child);
			}
			else
			{
				Console.Error.WriteLine("mxObjectCodec.encode: No node for " + Name + "." + attr + ": " + value);
			}
		}

		/// <summary>
		/// Converts true to "1" and false to "0". All other values are ignored.
		/// </summary>
		protected internal virtual object convertValueToXml(object value)
		{
			if (value is bool?)
			{
				value = ((bool?) value).Value ? "1" : "0";
			}

			return value;
		}

		/// <summary>
		/// Converts XML attribute values to object of the given type.
		/// </summary>
		protected internal virtual object convertValueFromXml(Type type, object value)
		{
			if (value is string)
			{
				string tmp = (string) value;

				if (type.Equals(typeof(bool)) || type == typeof(Boolean))
				{
					if (tmp.Equals("1") || tmp.Equals("0"))
					{
						tmp = (tmp.Equals("1")) ? "true" : "false";
					}

					value = Convert.ToBoolean(tmp);
				}
				else if (type.Equals(typeof(char)) || type == typeof(Char))
				{
					value = Convert.ToChar(tmp[0]);
				}
				else if (type.Equals(typeof(sbyte)) || type == typeof(SByte))
				{
					value = Convert.ToSByte(tmp);
				}
				else if (type.Equals(typeof(short)) || type == typeof(Int16))
				{
					value = Convert.ToInt16(tmp);
				}
				else if (type.Equals(typeof(int)) || type == typeof(Int32))
				{
					value = Convert.ToInt32(tmp);
				}
				else if (type.Equals(typeof(long)) || type == typeof(Int64))
				{
					value = Convert.ToInt64(tmp);
				}
				else if (type.Equals(typeof(float)) || type == typeof(Single))
				{
					value = Convert.ToSingle(tmp);
				}
				else if (type.Equals(typeof(double)) || type == typeof(Double))
				{
					value = Convert.ToDouble(tmp);
				}
			}

			return value;
		}

		/// <summary>
		/// Returns the XML node attribute name for the given Java field name. That
		/// is, it returns the mapping of the field name.
		/// </summary>
		protected internal virtual string getAttributeName(string fieldname)
		{
			if (!string.ReferenceEquals(fieldname, null))
			{
				object mapped = mapping[fieldname];

				if (mapped != null)
				{
					fieldname = mapped.ToString();
				}
			}

			return fieldname;
		}

		/// <summary>
		/// Returns the Java field name for the given XML attribute name. That is, it
		/// returns the reverse mapping of the attribute name.
		/// </summary>
		/// <param name="attributename">
		///            The attribute name to be mapped. </param>
		/// <returns> String that represents the mapped field name. </returns>
		protected internal virtual string getFieldName(string attributename)
		{
			if (!string.ReferenceEquals(attributename, null))
			{
				object mapped = reverse[attributename];

				if (mapped != null)
				{
					attributename = mapped.ToString();
				}
			}

			return attributename;
		}

		/// <summary>
		/// Returns the field with the specified name.
		/// </summary>
		protected internal virtual FieldInfo getField(object obj, string fieldname)
		{
			Type type = obj.GetType();

			while (type != null)
			{
				try
				{
					FieldInfo field = type.GetField(fieldname);

					if (field != null)
					{
						return field;
					}
				}
				catch (Exception)
				{
					// ignore
				}

				type = type.BaseType;
			}

			return null;
		}

		/// <summary>
		/// Returns the accessor (getter, setter) for the specified field.
		/// </summary>
		protected internal virtual MethodInfo getAccessor(object obj, FieldInfo field, bool isGetter)
		{
			string name = field.Name;
			name = name.Substring(0, 1).ToUpper() + name.Substring(1);

			if (!isGetter)
			{
				name = "set" + name;
			}
			else if (field.GetType().IsSubclassOf(typeof(bool)))
			{
				name = "is" + name;
			}
			else
			{
				name = "get" + name;
			}

			try
			{
				if (isGetter)
				{
					return getMethod(obj, name, null);
				}
				else
				{
					return getMethod(obj, name, new Type[] {field.GetType()});
				}
			}
			catch (Exception)
			{
				// ignore
			}

			return null;
		}

		/// <summary>
		/// Returns the method with the specified signature.
		/// </summary>
		protected internal virtual MethodInfo getMethod(object obj, string methodname, Type[] @params)
		{
			Type type = obj.GetType();

			while (type != null)
			{
				try
				{
					MethodInfo method = type.GetMethod(methodname, @params);

					if (method != null)
					{
						return method;
					}
				}
				catch (Exception)
				{
					// ignore
				}

				type = type.BaseType;
			}
			return null;
		}

		/// <summary>
		/// Returns the value of the field with the specified name in the specified
		/// object instance.
		/// </summary>
		protected internal virtual object getFieldValue(object obj, string fieldname)
		{
			object value = null;

			if (obj != null && !string.ReferenceEquals(fieldname, null))
			{
				FieldInfo field = getField(obj, fieldname);

				try
				{
					if (field != null)
					{
						value = field.GetValue(obj);
					}
				}
				catch (Exception)
				{
					if (field != null)
					{
						try
						{
							MethodInfo method = getAccessor(obj, field, true);
                            value = method.Invoke(obj, (object[]) null);
						}
						catch (Exception)
						{
							// ignore
						}
					}
				}
			}

			return value;
		}

		/// <summary>
		/// Sets the value of the field with the specified name
		/// in the specified object instance.
		/// </summary>
		protected internal virtual void setFieldValue(object obj, string fieldname, object value)
		{
			FieldInfo field = null;

			try
			{
				field = getField(obj, fieldname);

				if (field.GetType() == typeof(Boolean))
				{
					value = new bool?(value.Equals("1") || value.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase));
				}

				field.SetValue(obj, value);
			}
			catch (Exception)
			{
				if (field != null)
				{
					try
					{
						MethodInfo method = getAccessor(obj, field, false);
						Type type = method.GetParameters()[0].GetType();
						value = convertValueFromXml(type, value);

						// Converts collection to a typed array before setting
						if (type.IsArray && value is ICollection)
						{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.Collection<?> coll = (java.util.Collection<?>) value;
							ICollection<object> coll = (ICollection<object>) value;

                            object[] temp= (object[])Array.CreateInstance(type.GetElementType(), coll.Count);
                            coll.CopyTo(temp, 0);
                            value = temp;

                            //value = coll.toArray();
                        }

                        method.Invoke(obj, new object[] {value});
					}
					catch (Exception e2)
					{
						Console.Error.WriteLine("setFieldValue: " + e2 + " on " + obj.GetType().Name + "." + fieldname + " (" + field.GetType().Name + ") = " + value + " (" + value.GetType().Name + ")");
					}
				}
			}
		}

		/// <summary>
		/// Hook for subclassers to pre-process the object before encoding. This
		/// returns the input object. The return value of this function is used in
		/// encode to perform the default encoding into the given node.
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object to be encoded. </param>
		/// <param name="node"> XML node to encode the object into. </param>
		/// <returns> Returns the object to be encoded by the default encoding. </returns>
		public virtual object beforeEncode(mxCodec enc, object obj, Node node)
		{
			return obj;
		}

		/// <summary>
		/// Hook for subclassers to post-process the node for the given object after
		/// encoding and return the post-processed node. This implementation returns
		/// the input node. The return value of this method is returned to the
		/// encoder from <encode>.
		/// 
		/// Parameters:
		/// </summary>
		/// <param name="enc"> Codec that controls the encoding process. </param>
		/// <param name="obj"> Object to be encoded. </param>
		/// <param name="node"> XML node that represents the default encoding. </param>
		/// <returns> Returns the resulting node of the encoding. </returns>
		public virtual Node afterEncode(mxCodec enc, object obj, Node node)
		{
			return node;
		}

		/// <summary>
		/// Parses the given node into the object or returns a new object
		/// representing the given node.
		/// </summary>
		/// <param name="dec"> Codec that controls the encoding process. </param>
		/// <param name="node"> XML node to be decoded. </param>
		/// <returns> Returns the resulting object that represents the given XML node. </returns>
		public virtual object decode(mxCodec dec, Node node)
		{
			return decode(dec, node, null);
		}

		/// <summary>
		/// Parses the given node into the object or returns a new object
		/// representing the given node.
		/// 
		/// Dec is a reference to the calling decoder. It is used to decode complex
		/// objects and resolve references.
		/// 
		/// If a node has an id attribute then the object cache is checked for the
		/// object. If the object is not yet in the cache then it is constructed
		/// using the constructor of <template> and cached in <mxCodec.objects>.
		/// 
		/// This implementation decodes all attributes and childs of a node according
		/// to the following rules:
		///  - If the variable name is in <exclude> or if the attribute name is "id"
		/// or "as" then it is ignored. - If the variable name is in <idrefs> then
		/// <mxCodec.getObject> is used to replace the reference with an object. -
		/// The variable name is mapped using a reverse <mapping>. - If the value has
		/// a child node, then the codec is used to create a child object with the
		/// variable name taken from the "as" attribute. - If the object is an array
		/// and the variable name is empty then the value or child object is appended
		/// to the array. - If an add child has no value or the object is not an
		/// array then the child text content is evaluated using <mxUtils.eval>.
		/// 
		/// If no object exists for an ID in <idrefs> a warning is issued in
		/// System.err.
		/// </summary>
		/// <param name="dec"> Codec that controls the encoding process. </param>
		/// <param name="node"> XML node to be decoded. </param>
		/// <param name="into"> Optional object to encode the node into. </param>
		/// <returns> Returns the resulting object that represents the given XML node
		/// or the object given to the method as the into parameter. </returns>
		public virtual object decode(mxCodec dec, Node node, object into)
		{
			object obj = null;

			if (node is Element)
			{
                string id = ((Element) node).GetAttribute("id");
				obj = dec.objects[id];

				if (obj == null)
				{
					obj = into;

					if (obj == null)
					{
						obj = cloneTemplate(node);
					}

					if (!string.ReferenceEquals(id, null) && id.Length > 0)
					{
						dec.putObject(id, obj);
					}
				}

				node = beforeDecode(dec, node, obj);
				decodeNode(dec, node, obj);
				obj = afterDecode(dec, node, obj);
			}

			return obj;
		}

		/// <summary>
		/// Calls decodeAttributes and decodeChildren for the given node.
		/// </summary>
		protected internal virtual void decodeNode(mxCodec dec, Node node, object obj)
		{
			if (node != null)
			{
				decodeAttributes(dec, node, obj);
				decodeChildren(dec, node, obj);
			}
		}

		/// <summary>
		/// Decodes all attributes of the given node using decodeAttribute.
		/// </summary>
		protected internal virtual void decodeAttributes(mxCodec dec, Node node, object obj)
		{
			NamedNodeMap attrs = node.Attributes;

			if (attrs != null)
			{
				for (int i = 0; i < attrs.Count; i++)
				{
					Node attr = attrs.Item(i);
					decodeAttribute(dec, attr, obj);
				}
			}
		}

		/// <summary>
		/// Reads the given attribute into the specified object.
		/// </summary>
		protected internal virtual void decodeAttribute(mxCodec dec, Node attr, object obj)
		{
			string name = attr.Name;

			if (!name.Equals("as", StringComparison.CurrentCultureIgnoreCase) && !name.Equals("id", StringComparison.CurrentCultureIgnoreCase))
			{
				object value = attr.Value;
				string fieldname = getFieldName(name);

				if (isReference(obj, fieldname, value, false))
				{
					object tmp = dec.getObject(value.ToString());

					if (tmp == null)
					{
						Console.Error.WriteLine("mxObjectCodec.decode: No object for " + Name + "." + fieldname + "=" + value);
						return; // exit
					}

					value = tmp;
				}

				if (!isExcluded(obj, fieldname, value, false))
				{
					setFieldValue(obj, fieldname, value);
				}
			}
		}

		/// <summary>
		/// Decodec all children of the given node using decodeChild.
		/// </summary>
		protected internal virtual void decodeChildren(mxCodec dec, Node node, object obj)
		{
			Node child = node.FirstChild;

			while (child != null)
			{
				if (child.NodeType == System.Xml.XmlNodeType.Element && !processInclude(dec, child, obj))
				{
					decodeChild(dec, child, obj);
				}

				child = child.NextSibling;
			}
		}

		/// <summary>
		/// Reads the specified child into the given object.
		/// </summary>
		protected internal virtual void decodeChild(mxCodec dec, Node child, object obj)
		{
            string fieldname = getFieldName(((Element) child).GetAttribute("as"));

			if (string.ReferenceEquals(fieldname, null) || !isExcluded(obj, fieldname, child, false))
			{
				object value = null;
				object template = getFieldValue(obj, fieldname);

				if (child.Name.Equals("add"))
				{
                    value = ((Element) child).GetAttribute("value");

					if (value == null)
					{
						value = child.InnerText;
					}
				}
				else
				{
					// Arrays are replaced completely
					if (template != null && template.GetType().IsArray)
					{
						template = null;
					}
					// Collections are cleared
					else if (template is ICollection)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: ((java.util.Collection<?>) template).clear();
						((ICollection<object>) template).Clear();
					}

					value = dec.decode(child, template);
					// System.out.println("Decoded " + child.getNodeName() + "."
					// + fieldname + "=" + value);
				}

				if (value != null && !value.Equals(template))
				{
					if (!string.ReferenceEquals(fieldname, null) && obj is IDictionary)
					{
						((IDictionary) obj)[fieldname] = value;
					}
					else if (!string.ReferenceEquals(fieldname, null) && fieldname.Length > 0)
					{
						setFieldValue(obj, fieldname, value);
					}
					// Arrays are treated as collections and
					// converted in setFieldValue
					else if (obj is ICollection)
					{
						((ICollection<object>) obj).Add(value);
					}
				}
			}
		}

		/// <summary>
		/// Returns true if the given node is an include directive and executes the
		/// include by decoding the XML document. Returns false if the given node is
		/// not an include directive.
		/// </summary>
		/// <param name="dec"> Codec that controls the encoding/decoding process. </param>
		/// <param name="node"> XML node to be checked. </param>
		/// <param name="into"> Optional object to pass-thru to the codec. </param>
		/// <returns> Returns true if the given node was processed as an include. </returns>
		public virtual bool processInclude(mxCodec dec, Node node, object into)
		{
            if (node.NodeType == System.Xml.XmlNodeType.Element && node.Name.Equals("include",StringComparison.OrdinalIgnoreCase))
			{
                string name = ((Element) node).GetAttribute("name");

				if (!string.ReferenceEquals(name, null))
				{
					try
					{
						Node xml = mxUtils.loadDocument(typeof(mxObjectCodec).FullName+name).DocumentElement;

						if (xml != null)
						{
							dec.decode(xml, into);
						}
					}
					catch (Exception)
					{
						Console.Error.WriteLine("Cannot process include: " + name);
					}
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Hook for subclassers to pre-process the node for the specified object
		/// and return the node to be used for further processing by
		/// <seealso cref="#decode(mxCodec, Node)"/>. The object is created based on the
		/// template in the calling method and is never null.
		/// 
		/// This implementation returns the input node. The return value of this
		/// function is used in <seealso cref="#decode(mxCodec, Node)"/> to perform the
		/// default decoding into the given object.
		/// </summary>
		/// <param name="dec"> Codec that controls the decoding process. </param>
		/// <param name="node"> XML node to be decoded. </param>
		/// <param name="obj"> Object to encode the node into. </param>
		/// <returns> Returns the node used for the default decoding. </returns>
		public virtual Node beforeDecode(mxCodec dec, Node node, object obj)
		{
			return node;
		}

		/// <summary>
		/// Hook for subclassers to post-process the object after decoding. This
		/// implementation returns the given object without any changes. The return
		/// value of this method is returned to the decoder from
		/// <seealso cref="#decode(mxCodec, Node)"/>.
		/// </summary>
		/// <param name="dec"> Codec that controls the decoding process. </param>
		/// <param name="node"> XML node to be decoded. </param>
		/// <param name="obj"> Object that represents the default decoding. </param>
		/// <returns> Returns the result of the decoding process. </returns>
		public virtual object afterDecode(mxCodec dec, Node node, object obj)
		{
			return obj;
		}

	}

}