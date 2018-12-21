using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Document =System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;
    using NodeList = System.Xml.XmlNodeList;

	/// <summary>
	/// This is a singleton class that contains a map with the key elements of the
	/// document. The key elements are wrapped in instances of mxGmlKey and
	/// may to be access by ID.
	/// </summary>
	public class mxGraphMlKeyManager
	{
		/// <summary>
		/// Map with the key elements of the document.<br/>
		/// The key is the key's ID.
		/// </summary>
		private Dictionary<string, mxGraphMlKey> keyMap = new Dictionary<string, mxGraphMlKey>();

		private static mxGraphMlKeyManager keyManager = null;

		/// <summary>
		/// Singleton pattern requires private constructor.
		/// </summary>
		private mxGraphMlKeyManager()
		{
		}

		/// <summary>
		/// Returns the instance of mxGmlKeyManager.
		/// If no instance has been created until the moment, a new instance is
		/// returned.
		/// This method don't load the map. </summary>
		/// <returns> An instance of mxGmlKeyManager. </returns>
		public static mxGraphMlKeyManager Instance
		{
			get
			{
				if (keyManager == null)
				{
					keyManager = new mxGraphMlKeyManager();
				}
				return keyManager;
			}
		}

		/// <summary>
		/// Load the map with the key elements in the document.<br/>
		/// The keys are wrapped for instances of mxGmlKey. </summary>
		/// <param name="doc"> Document with the keys. </param>
		public virtual void initialise(Document doc)
		{
            NodeList gmlKeys = doc.GetElementsByTagName(mxGraphMlConstants.KEY);

			int keyLength = gmlKeys.Count;

			for (int i = 0; i < keyLength; i++)
			{
                Element key = (Element) gmlKeys.Item(i);
                string keyId = key.GetAttribute(mxGraphMlConstants.ID);
				mxGraphMlKey keyElement = new mxGraphMlKey(key);
				keyMap[keyId] = keyElement;
			}
		}

		public virtual Dictionary<string, mxGraphMlKey> KeyMap
		{
			get
			{
				return keyMap;
			}
			set
			{
				this.keyMap = value;
			}
		}

	}

}