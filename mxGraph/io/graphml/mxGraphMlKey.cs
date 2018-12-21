/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	/// <summary>
	/// Represents a Key element in the GML Structure.
	/// </summary>
	public class mxGraphMlKey
	{
		/// <summary>
		/// Possibles values for the keyFor Attribute
		/// </summary>
		public enum keyForValues
		{
			GRAPH,
			NODE,
			EDGE,
			HYPEREDGE,
			PORT,
			ENDPOINT,
			ALL
		}

		/// <summary>
		/// Possibles values for the keyType Attribute.
		/// </summary>
		public enum keyTypeValues
		{
			BOOLEAN,
			INT,
			LONG,
			FLOAT,
			DOUBLE,
			STRING
		}

		private string keyDefault;

		private string keyId;

		private keyForValues keyFor;

		private string keyName;

		private keyTypeValues keyType;

		/// <summary>
		/// Construct a key with the given parameters. </summary>
		/// <param name="keyId"> Key's ID </param>
		/// <param name="keyFor"> Scope of the key. </param>
		/// <param name="keyName"> Key Name </param>
		/// <param name="keyType"> Type of the values represented for this key. </param>
		public mxGraphMlKey(string keyId, keyForValues keyFor, string keyName, keyTypeValues keyType)
		{
			this.keyId = keyId;
			this.keyFor = keyFor;
			this.keyName = keyName;
			this.keyType = keyType;
			this.keyDefault = defaultValue();
		}

		/// <summary>
		/// Construct a key from a xml key element. </summary>
		/// <param name="keyElement"> Xml key element. </param>
		public mxGraphMlKey(Element keyElement)
		{
            this.keyId = keyElement.GetAttribute(mxGraphMlConstants.ID);
			this.keyFor = enumForValue(keyElement.GetAttribute(mxGraphMlConstants.KEY_FOR));
			this.keyName = keyElement.GetAttribute(mxGraphMlConstants.KEY_NAME);
			this.keyType = enumTypeValue(keyElement.GetAttribute(mxGraphMlConstants.KEY_TYPE));
			this.keyDefault = defaultValue();
		}

		public virtual string KeyDefault
		{
			get
			{
				return keyDefault;
			}
			set
			{
				this.keyDefault = value;
			}
		}


		public virtual keyForValues KeyFor
		{
			get
			{
				return keyFor;
			}
			set
			{
				this.keyFor = value;
			}
		}


		public virtual string KeyId
		{
			get
			{
				return keyId;
			}
			set
			{
				this.keyId = value;
			}
		}


		public virtual string KeyName
		{
			get
			{
				return keyName;
			}
			set
			{
				this.keyName = value;
			}
		}


		public virtual keyTypeValues KeyType
		{
			get
			{
				return keyType;
			}
			set
			{
				this.keyType = value;
			}
		}


		/// <summary>
		/// Returns the default value of the keyDefault attribute according
		/// the keyType.
		/// </summary>
		private string defaultValue()
		{
			string val = "";
			switch (this.keyType)
			{
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.BOOLEAN:
				{
					val = "false";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.DOUBLE:
				{
					val = "0";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.FLOAT:
				{
					val = "0";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.INT:
				{
					val = "0";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.LONG:
				{
					val = "0";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.STRING:
				{
					val = "";
					break;
				}
			}
			return val;
		}

		/// <summary>
		/// Generates a Key Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
            Element key = document.CreateElement(mxGraphMlConstants.KEY);

			if (!keyName.Equals(""))
			{
                key.SetAttribute(mxGraphMlConstants.KEY_NAME, keyName);
			}
			key.SetAttribute(mxGraphMlConstants.ID, keyId);

			if (!keyName.Equals(""))
			{
				key.SetAttribute(mxGraphMlConstants.KEY_FOR, stringForValue(keyFor));
			}

			if (!keyName.Equals(""))
			{
				key.SetAttribute(mxGraphMlConstants.KEY_TYPE, stringTypeValue(keyType));
			}

			if (!keyName.Equals(""))
			{
				key.InnerText = keyDefault;
			}

			return key;
		}

		/// <summary>
		/// Converts a String value in its corresponding enum value for the
		/// keyFor attribute. </summary>
		/// <param name="value"> Value in String representation. </param>
		/// <returns> Returns the value in its enum representation. </returns>
		public virtual keyForValues enumForValue(string value)
		{
			keyForValues enumVal = keyForValues.ALL;

			if (value.Equals(mxGraphMlConstants.GRAPH))
			{
				enumVal = keyForValues.GRAPH;
			}
			else if (value.Equals(mxGraphMlConstants.NODE))
			{
				enumVal = keyForValues.NODE;
			}
			else if (value.Equals(mxGraphMlConstants.EDGE))
			{
				enumVal = keyForValues.EDGE;
			}
			else if (value.Equals(mxGraphMlConstants.HYPEREDGE))
			{
				enumVal = keyForValues.HYPEREDGE;
			}
			else if (value.Equals(mxGraphMlConstants.PORT))
			{
				enumVal = keyForValues.PORT;
			}
			else if (value.Equals(mxGraphMlConstants.ENDPOINT))
			{
				enumVal = keyForValues.ENDPOINT;
			}
			else if (value.Equals(mxGraphMlConstants.ALL))
			{
				enumVal = keyForValues.ALL;
			}

			return enumVal;
		}

		/// <summary>
		/// Converts a enum value in its corresponding String value for the
		/// keyFor attribute. </summary>
		/// <param name="value"> Value in enum representation. </param>
		/// <returns> Returns the value in its String representation. </returns>
		public virtual string stringForValue(keyForValues value)
		{

			string val = mxGraphMlConstants.ALL;

			switch (value)
			{
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.GRAPH:
				{
					val = mxGraphMlConstants.GRAPH;
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.NODE:
				{
					val = mxGraphMlConstants.NODE;
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.EDGE:
				{
					val = mxGraphMlConstants.EDGE;
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.HYPEREDGE:
				{
					val = mxGraphMlConstants.HYPEREDGE;
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.PORT:
				{
					val = mxGraphMlConstants.PORT;
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.ENDPOINT:
				{
					val = mxGraphMlConstants.ENDPOINT;
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyForValues.ALL:
				{
					val = mxGraphMlConstants.ALL;
					break;
				}
			}

			return val;
		}

		/// <summary>
		/// Converts a String value in its corresponding enum value for the
		/// keyType attribute. </summary>
		/// <param name="value"> Value in String representation. </param>
		/// <returns> Returns the value in its enum representation. </returns>
		public virtual keyTypeValues enumTypeValue(string value)
		{
			keyTypeValues enumVal = keyTypeValues.STRING;

			if (value.Equals("boolean"))
			{
				enumVal = keyTypeValues.BOOLEAN;
			}
			else if (value.Equals("double"))
			{
				enumVal = keyTypeValues.DOUBLE;
			}
			else if (value.Equals("float"))
			{
				enumVal = keyTypeValues.FLOAT;
			}
			else if (value.Equals("int"))
			{
				enumVal = keyTypeValues.INT;
			}
			else if (value.Equals("long"))
			{
				enumVal = keyTypeValues.LONG;
			}
			else if (value.Equals("string"))
			{
				enumVal = keyTypeValues.STRING;
			}

			return enumVal;
		}

		/// <summary>
		/// Converts a enum value in its corresponding string value for the
		/// keyType attribute. </summary>
		/// <param name="value"> Value in enum representation. </param>
		/// <returns> Returns the value in its String representation. </returns>
		public virtual string stringTypeValue(keyTypeValues value)
		{
			string val = "string";

			switch (value)
			{
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.BOOLEAN:
				{
					val = "boolean";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.DOUBLE:
				{
					val = "double";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.FLOAT:
				{
					val = "float";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.INT:
				{
					val = "int";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.LONG:
				{
					val = "long";
					break;
				}
				case mxGraph.io.graphml.mxGraphMlKey.keyTypeValues.STRING:
				{
					val = "string";
					break;
				}
			}

			return val;
		}
	}

}