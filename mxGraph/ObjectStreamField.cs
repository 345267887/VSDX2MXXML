using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    /// <summary>
	/// A description of a Serializable field from a Serializable class.  An array
	/// of ObjectStreamFields is used to declare the Serializable fields of a class.
	/// 
	/// @author      Mike Warres
	/// @author      Roger Riggs </summary>
	/// <seealso cref= ObjectStreamClass
	/// @since 1.2 </seealso>
	public class ObjectStreamField : IComparable<object>
    {

        /// <summary>
        /// field name </summary>
        private readonly string name;
        /// <summary>
        /// canonical JVM signature of field type </summary>
        private readonly string signature;
        /// <summary>
        /// field type (Object.class if unknown non-primitive type) </summary>
        private readonly Type type;
        /// <summary>
        /// whether or not to (de)serialize field values as unshared </summary>
        private readonly bool unshared;
        /// <summary>
        /// corresponding reflective field object, if any </summary>
        private readonly Field field;
        /// <summary>
        /// offset of field value in enclosing field group </summary>
        private int offset = 0;

        /// <summary>
        /// Create a Serializable field with the specified type.  This field should
        /// be documented with a <code>serialField</code> tag.
        /// </summary>
        /// <param name="name"> the name of the serializable field </param>
        /// <param name="type"> the <code>Class</code> object of the serializable field </param>
        public ObjectStreamField(string name, Type type) : this(name, type, false)
        {
        }

        /// <summary>
        /// Creates an ObjectStreamField representing a serializable field with the
        /// given name and type.  If unshared is false, values of the represented
        /// field are serialized and deserialized in the default manner--if the
        /// field is non-primitive, object values are serialized and deserialized as
        /// if they had been written and read by calls to writeObject and
        /// readObject.  If unshared is true, values of the represented field are
        /// serialized and deserialized as if they had been written and read by
        /// calls to writeUnshared and readUnshared.
        /// </summary>
        /// <param name="name"> field name </param>
        /// <param name="type"> field type </param>
        /// <param name="unshared"> if false, write/read field values in the same manner
        ///          as writeObject/readObject; if true, write/read in the same
        ///          manner as writeUnshared/readUnshared
        /// @since   1.4 </param>
        public ObjectStreamField(string name, Type type, bool unshared)
        {
            if (string.ReferenceEquals(name, null))
            {
                throw new System.NullReferenceException();
            }
            this.name = name;
            this.type = type;
            this.unshared = unshared;
            signature = ObjectStreamClass.getClassSignature(type).intern();
            field = null;
        }

        /// <summary>
        /// Creates an ObjectStreamField representing a field with the given name,
        /// signature and unshared setting.
        /// </summary>
        internal ObjectStreamField(string name, string signature, bool unshared)
        {
            if (string.ReferenceEquals(name, null))
            {
                throw new System.NullReferenceException();
            }
            this.name = name;
            this.signature = signature.intern();
            this.unshared = unshared;
            field = null;

            switch (signature[0])
            {
                case 'Z':
                    type = Boolean.TYPE;
                    break;
                case 'B':
                    type = Byte.TYPE;
                    break;
                case 'C':
                    type = Character.TYPE;
                    break;
                case 'S':
                    type = Short.TYPE;
                    break;
                case 'I':
                    type = Integer.TYPE;
                    break;
                case 'J':
                    type = Long.TYPE;
                    break;
                case 'F':
                    type = Float.TYPE;
                    break;
                case 'D':
                    type = Double.TYPE;
                    break;
                case 'L':
                case '[':
                    type = typeof(object);
                    break;
                default:
                    throw new System.ArgumentException("illegal signature");
            }
        }

        /// <summary>
        /// Creates an ObjectStreamField representing the given field with the
        /// specified unshared setting.  For compatibility with the behavior of
        /// earlier serialization implementations, a "showType" parameter is
        /// necessary to govern whether or not a getType() call on this
        /// ObjectStreamField (if non-primitive) will return Object.class (as
        /// opposed to a more specific reference type).
        /// </summary>
        internal ObjectStreamField(Field field, bool unshared, bool showType)
        {
            this.field = field;
            this.unshared = unshared;
            name = field.Name;
            Type ftype = field.Type;
            type = (showType || ftype.IsPrimitive) ? ftype : typeof(object);
            signature = ObjectStreamClass.getClassSignature(ftype).intern();
        }

        /// <summary>
        /// Get the name of this field.
        /// </summary>
        /// <returns>  a <code>String</code> representing the name of the serializable
        ///          field </returns>
        public virtual string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Get the type of the field.  If the type is non-primitive and this
        /// <code>ObjectStreamField</code> was obtained from a deserialized {@link
        /// ObjectStreamClass} instance, then <code>Object.class</code> is returned.
        /// Otherwise, the <code>Class</code> object for the type of the field is
        /// returned.
        /// </summary>
        /// <returns>  a <code>Class</code> object representing the type of the
        ///          serializable field </returns>
        public virtual Type Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Returns character encoding of field type.  The encoding is as follows:
        /// <blockquote><pre>
        /// B            byte
        /// C            char
        /// D            double
        /// F            float
        /// I            int
        /// J            long
        /// L            class or interface
        /// S            short
        /// Z            boolean
        /// [            array
        /// </pre></blockquote>
        /// </summary>
        /// <returns>  the typecode of the serializable field </returns>
        // REMIND: deprecate?
        public virtual char TypeCode
        {
            get
            {
                return signature[0];
            }
        }

        /// <summary>
        /// Return the JVM type signature.
        /// </summary>
        /// <returns>  null if this field has a primitive type. </returns>
        // REMIND: deprecate?
        public virtual string TypeString
        {
            get
            {
                return Primitive ? null : signature;
            }
        }

        /// <summary>
        /// Offset of field within instance data.
        /// </summary>
        /// <returns>  the offset of this field </returns>
        /// <seealso cref= #setOffset </seealso>
        // REMIND: deprecate?
        public virtual int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                this.offset = value;
            }
        }


        /// <summary>
        /// Return true if this field has a primitive type.
        /// </summary>
        /// <returns>  true if and only if this field corresponds to a primitive type </returns>
        // REMIND: deprecate?
        public virtual bool Primitive
        {
            get
            {
                char tcode = signature[0];
                return ((tcode != 'L') && (tcode != '['));
            }
        }

        /// <summary>
        /// Returns boolean value indicating whether or not the serializable field
        /// represented by this ObjectStreamField instance is unshared.
        /// 
        /// @since 1.4
        /// </summary>
        public virtual bool Unshared
        {
            get
            {
                return unshared;
            }
        }

        /// <summary>
        /// Compare this field with another <code>ObjectStreamField</code>.  Return
        /// -1 if this is smaller, 0 if equal, 1 if greater.  Types that are
        /// primitives are "smaller" than object types.  If equal, the field names
        /// are compared.
        /// </summary>
        // REMIND: deprecate?
        public virtual int CompareTo(object obj)
        {
            ObjectStreamField other = (ObjectStreamField)obj;
            bool isPrim = Primitive;
            if (isPrim != other.Primitive)
            {
                return isPrim ? -1 : 1;
            }
            return name.CompareTo(other.name);
        }

        /// <summary>
        /// Return a string that describes this field.
        /// </summary>
        public override string ToString()
        {
            return signature + ' ' + name;
        }

        /// <summary>
        /// Returns field represented by this ObjectStreamField, or null if
        /// ObjectStreamField is not associated with an actual field.
        /// </summary>
        internal virtual Field Field
        {
            get
            {
                return field;
            }
        }

        /// <summary>
        /// Returns JVM type signature of field (similar to getTypeString, except
        /// that signature strings are returned for primitive fields as well).
        /// </summary>
        internal virtual string Signature
        {
            get
            {
                return signature;
            }
        }
    }

}
