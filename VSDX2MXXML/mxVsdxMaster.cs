using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxVsdxMaster
    {
        protected XmlElement master;

        /**
         * Unique ID of the element within its parent element
         */
        protected String Id = null;

        protected Shape masterShape = null;

        /*
         * Map that contains the shapes in Master element wrapped for instances of mxDelegateShape.
         * The key is the shape's ID.
         */
        protected Dictionary<String, Shape> childShapes = new Dictionary<String, Shape>();

        /**
         * Create a new instance of mxMasterElement and retrieves all the shapes contained
         * in the Master element.
         * @param m Master Element to be wrapped.
         */
        public mxVsdxMaster(XmlElement m, mxVsdxModel model)
        {
            this.master = m;
            this.Id = m.GetAttribute(mxVsdxConstants.ID);
            processMasterShapes(model);
        }

        /**
         * Retrieves and wraps all the shapes contained in the 'shape' param.<br/>
         * This method is recursive, it retrieves the subshapes of the shapes too.
         * @param shape Shape from which the subshapes are retrieved.
         * @return Map with the shapes wrapped in instances of mxMasterShape.
         */
        protected void processMasterShapes(mxVsdxModel model)
        {
            XmlNode child = this.master.FirstChild;

            while (child != null)
            {
                if (child is XmlElement && ((XmlElement)child).Name.Equals("Rel"))
                {
                    XmlElement relElem = model.getRelationship(((XmlElement)child).GetAttribute("r:id"), mxVsdxCodec.vsdxPlaceholder + "/masters/" + "_rels/masters.xml.rels");
                    String target = relElem.GetAttribute("Target");
                    String type = relElem.GetAttribute("Type");
                    XmlDocument masterDoc = null;

                    if (type != null && type.EndsWith("master"))
                    {
                        masterDoc = model.getXmlDoc(mxVsdxCodec.vsdxPlaceholder + "/masters/" + target);
                    }

                    if (masterDoc != null)
                    {
                        XmlNode masterChild = masterDoc.FirstChild;

                        while (masterChild != null)
                        {
                            if (masterChild is XmlElement && ((XmlElement)masterChild).Name.Equals("MasterContents"))
                            {
                                processMasterShape((XmlElement)masterChild, model);
                                break;
                            }

                            masterChild = masterChild.NextSibling;
                        }
                    }
                }

                child = child.NextSibling;
            }
        }

        /**
         * Retrieves and wraps all the shapes contained in the 'shape' param.<br/>
         * This method is recursive, it retrieves the subshapes of the shapes too.
         * @param shape Shape from which the subshapes are retrieved.
         * @return Map with the shapes wrapped in instances of mxMasterShape.
         */
        protected void processMasterShape(XmlElement shapeElem, mxVsdxModel model)
        {
            XmlNode shapeChild = shapeElem.FirstChild;

            while (shapeChild != null)
            {
                if (shapeChild is XmlElement && ((XmlElement)shapeChild).Name.Equals("Shapes"))
			{
                    XmlNode shapesChild = shapeChild.FirstChild;

                    while (shapesChild != null)
                    {
                        if (shapesChild is XmlElement && ((XmlElement)shapesChild).Name.Equals("Shape"))
					{
                            XmlElement shape = (XmlElement)shapesChild;
                            String shapeId = shape.GetAttribute("ID");
                            Shape masterShape = new Shape(shape, model);
                            this.masterShape = (this.masterShape == null) ? masterShape : this.masterShape;
                            childShapes.Add(shapeId, masterShape);
                            processMasterShape(shape, model);
                        }

                        shapesChild = shapesChild.NextSibling;
                    }

                    break;
                }

                shapeChild = shapeChild.NextSibling;
            }
        }

        /**
         * Returns the first shape in the Master
         * @return First shape in the Master wrapped in a instance of mxMasterShape
         */
        public Shape getMasterShape()
        {
            return this.masterShape;
        }

        /**
         * Returns the shape in the master element with ID = 'id'.
         * @param id Shape's ID
         * @return The shape in the master element with ID = 'id' wrapped in a instance of mxMasterShape
         */
        public Shape getSubShape(String id)
        {
            return childShapes[id];
        }

        /**
         * Returns the NameU attribute.
         * @return Value of the NameU attribute.
         */
        public String getNameU()
        {
            return master.GetAttribute("NameU");
        }

        /**
         * Returns the NameU attribute.
         * @return Value of the NameU attribute.
         */
        public String getName()
        {
            return master.GetAttribute("Name");
        }

        /**
         * Returns the UniqueID attribute.
         * @return Value of the UniqueID attribute.
         */
        public String getUniqueID()
        {
            String uniqueID = "";

            if (master.HasAttribute("UniqueID"))
            {
                uniqueID = master.GetAttribute("UniqueID");
            }

            return uniqueID;
        }

        public String getId()
        {
            return this.Id;
        }

        public XmlElement getMasterElement()
        {
            return master;
        }
    }
}
