using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2010 David Benson, Gaudenz Alder
/// </summary>
namespace mxGraph.io.graphml
{

	using Document = System.Xml.XmlDocument;
	using Element = System.Xml.XmlElement;

	/// <summary>
	/// Represents a Port element in the GML Structure.
	/// </summary>
	public class mxGraphMlPort
	{
		private string name;

		private Dictionary<string, mxGraphMlData> portDataMap = new Dictionary<string, mxGraphMlData>();

		/// <summary>
		/// Construct a Port with name. </summary>
		/// <param name="name"> Port Name </param>
		public mxGraphMlPort(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Construct a Port from a xml port Element. </summary>
		/// <param name="portElement"> Xml port Element. </param>
		public mxGraphMlPort(Element portElement)
		{
            this.name = portElement.GetAttribute(mxGraphMlConstants.PORT_NAME);

			//Add data elements
			IList<Element> dataList = mxGraphMlUtils.childsTags(portElement, mxGraphMlConstants.DATA);

			foreach (Element dataElem in dataList)
			{
				mxGraphMlData data = new mxGraphMlData(dataElem);
				string key = data.DataKey;
				portDataMap[key] = data;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}


		public virtual Dictionary<string, mxGraphMlData> PortDataMap
		{
			get
			{
				return portDataMap;
			}
			set
			{
				this.portDataMap = value;
			}
		}


		/// <summary>
		/// Generates a Key Element from this class. </summary>
		/// <param name="document"> Document where the key Element will be inserted. </param>
		/// <returns> Returns the generated Elements. </returns>
		public virtual Element generateElement(Document document)
		{
            Element node = document.CreateElement(mxGraphMlConstants.PORT);

            node.SetAttribute(mxGraphMlConstants.PORT_NAME, name);

			foreach (mxGraphMlData data in portDataMap.Values)
			{
				Element dataElement = data.generateNodeElement(document);
                node.AppendChild(dataElement);
			}

			return node;
		}
	}

}