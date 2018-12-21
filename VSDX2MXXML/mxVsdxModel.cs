using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSDX2MXXML
{
    class mxVsdxModel
    {
        /**
	 * A map of Documents created by reading the XML files, indexed by the path to those files
	 */
        protected Dictionary<String, XmlElement> xmlDocs = null;

        /**
         * Collection of media files encoded in Base64, indexed by the path to those files
         */
        protected Dictionary<String, String> media = null;

        /**
         * The document from .../document.xml
         */
        protected XmlElement rootElement;

        /**
         * Map of page objects indexed by their ID.
         */
        protected Dictionary<int, mxVsdxPage> pages = null;

        /**
         * Map of master objects indexed by their ID. Before you think you're being clever by making
         * the index an Integer as for pages, don't, there are reasons.
         */
        protected Dictionary<String, mxVsdxMaster> masters = new Dictionary<String, mxVsdxMaster>();

        /**
         * Map stylesheets indexed by their ID
         */
        protected Dictionary<String, Style> stylesheets = new Dictionary<String, Style>();

        /**
         * Map themes indexed by their index
         */
        protected Dictionary<int, mxVsdxTheme> themes = new Dictionary<int, mxVsdxTheme>();

        mxPropertiesManager pm;

        public mxVsdxModel(XmlDocument doc, Dictionary<String, XmlDocument> docData, Dictionary<String, String> mediaData)
        {
            this.xmlDocs = docData;
            this.media = mediaData;

            XmlNode childNode = doc.FirstChild;

            while (childNode != null)
            {
                if (childNode is XmlElement && ((XmlElement)childNode).Name.ToLower().Equals(mxVsdxCodec.vsdxPlaceholder + "document"))
			{
                    this.rootElement = (XmlElement)childNode;
                    break;
                }

                childNode = childNode.NextSibling;
            }

            this.pm = new mxPropertiesManager();
            this.pm.initialise(rootElement, this);
            initStylesheets();
            initThemes();
            initMasters();
            initPages();
        }

        /**
         * Initialize theme objects from the XML files
         */
        private void initThemes()
        {
            // Lazy build up the master structure
            if (this.xmlDocs != null)
            {
                bool more = true;
                int index = 1;

                while (more)
                {
                    String path = mxVsdxCodec.vsdxPlaceholder + "/theme/theme" + index + ".xml";
                    XmlDocument themeDoc = this.xmlDocs[path];

                    if (themeDoc != null)
                    {
                        XmlNode child = themeDoc.FirstChild;

                        while (child != null)
                        {
                            if (child is XmlElement && ((XmlElement)child).Name.Equals("a:theme"))
						{
                                mxVsdxTheme theme = new mxVsdxTheme((Element)child);


                                if (theme.getThemeIndex() < 0)
                                {
                                    //theme index cannot be determined unless the theme is parsed
                                    theme.processTheme();
                                }

                                //TODO having two theme files with the same id still requires more handling
                                //		probably we need to merge the similar parts (has same theme name)
                                mxVsdxTheme existingTheme = themes[(theme.getThemeIndex())];
                                if (existingTheme == null || !existingTheme.isPure())
                                {
                                    themes.Add(theme.getThemeIndex(), theme);
                                }

                                break;
                            }

                            child = child.NextSibling;
                        }
                        index++;
                    }
                    else
                    {
                        more = false;
                    }
                }
            }
        }

        /**
         * Load the map with the stylesheets elements in the document.<br/>
         * The masters are wrapped for instances of mxStyleSheet.
         * @param doc Document with the stylesheets.
         */
        public void initStylesheets()
        {
            XmlNodeList vdxSheets = rootElement.GetElementsByTagName(mxVsdxConstants.STYLE_SHEETS);

            if (vdxSheets.Count > 0)
            {
                XmlElement sheets = (XmlElement)vdxSheets[(0)];
                XmlNodeList sheetList = sheets.GetElementsByTagName(mxVsdxConstants.STYLE_SHEET);
                int sheetLength = sheetList.Count;

                for (int i = 0; i < sheetLength; i++)
                {
                    XmlElement sheet = (XmlElement)sheetList[i];
                    String sheetId = sheet.GetAttribute(mxVsdxConstants.ID);
                    Style sheetElement = new Style(sheet, this);
                    stylesheets.Add(sheetId, sheetElement);
                }
            }


            foreach (var item in stylesheets)
            {
                Style sheet = item.Value;
                sheet.stylesheetRefs(this);
            }

            //Collection<Style> sheets = stylesheets.values();
            //Iterator<Style> iter = sheets.iterator();

            //while (iter.hasNext())
            //{
            //    Style sheet = iter.next();
            //    sheet.stylesheetRefs(this);
            //}
        }

        /**
         * Initialize master objects from the XML files
         */
        public void initMasters()
        {
            // Lazy build up the master structure
            if (this.xmlDocs != null)
            {
                String path = mxVsdxCodec.vsdxPlaceholder + "/masters/masters.xml";
                XmlElement masterDoc = this.xmlDocs[path];

                if (masterDoc != null)
                {
                    XmlNode child = masterDoc.FirstChild;

                    while (child != null)
                    {
                        if (child is XmlElement && ((XmlElement)child).Name.Equals(mxVsdxConstants.MASTERS))
					{
                            XmlNode grandChild = child.FirstChild;

                            while (grandChild != null)
                            {
                                if (grandChild is XmlElement && ((XmlElement)grandChild).Name.Equals("Master"))
							{
                                    XmlElement masterElement = (XmlElement)grandChild;
                                    mxVsdxMaster master = new mxVsdxMaster(masterElement, this);
                                    this.masters.Add(master.getId(), master);
                                }

                                grandChild = grandChild.NextSibling;
                            }

                            break;

                        }

                        child = child.NextSibling;
                    }
                }
            }
        }

        /**
         * Initialize page objects from the XML files
         */
        public void initPages()
        {
            // Lazy build up the pages structure
            if (this.xmlDocs != null)
            {
                String path = mxVsdxCodec.vsdxPlaceholder + "/pages/pages.xml";
                XmlElement pageDoc = this.xmlDocs[path];

                if (pageDoc != null)
                {
                    XmlNode child = pageDoc.FirstChild;

                    while (child != null)
                    {
                        if (child is XmlElement && ((XmlElement)child).Name.Equals(mxVsdxConstants.PAGES))
					{
                            XmlElement pages = (XmlElement)child;

                            XmlNodeList pageList = pages.GetElementsByTagName(mxVsdxConstants.PAGE);

                            if (pageList != null && pageList.Count > 0)
                            {
                                this.pages = new Dictionary<int, mxVsdxPage>();

                                Dictionary<int, mxVsdxPage> backgroundMap = new Dictionary<int, mxVsdxPage>();
                                int pageListLen = pageList.Count;

                                //Find the background pages while creating all the pages
                                for (int i = 0; i < pageListLen; i++)
                                {
                                    XmlElement pageEle = (XmlElement)pageList[i];
                                    mxVsdxPage page = createPage(pageEle);

                                    if (page.isBackground())
                                    {
                                        backgroundMap.Add(page.getId(), page);
                                    }

                                    this.pages.Add(page.getId(), page);
                                }

                                // Iterate again, assigning background pages
                                foreach (var item in this.pages)
                                {
                                    mxVsdxPage page =item.Value;

                                    if (!page.isBackground())
                                    {
                                        int backId = page.getBackPageId();

                                        if (backId != null)
                                        {
                                            //Import the background.
                                            mxVsdxPage background = backgroundMap[backId];
                                            page.setBackPage(background); ;
                                        }
                                    }
                                }
                            }

                            break; // MS defines there can only be 0 or 1 PAGES element, don't process second
                        }

                        child = child.NextSibling;
                    }
                }
            }
        }

        public Dictionary<int, mxVsdxPage> getPages()
        {
            return this.pages;
        }

        public Dictionary<int, mxVsdxTheme> getThemes()
        {
            return this.themes;
        }

        public XmlElement getRelationship(String rid, String path)
        {
            XmlElement relsDoc = this.xmlDocs[path];

            if (relsDoc == null || string.IsNullOrEmpty(rid))
            {
                // Valid to not have a rels for an XML file
                return null;
            }

            XmlNodeList rels = relsDoc.GetElementsByTagName("Relationship");

            for (int i = 0; i < rels.Count; i++)
            {
                XmlElement currElem = (XmlElement)rels[i];
                String id = currElem.GetAttribute("Id");

                if (id.Equals(rid))
                {
                    return currElem;
                }
            }

            return null;
        }

        public mxVsdxMaster getMaster(String masterId)
        {
            return this.masters[masterId];
        }

        protected mxVsdxPage createPage(XmlElement pageEle)
        {
            return new mxVsdxPage(pageEle, this);
        }

        public mxPropertiesManager getPropertiesManager()
        {
            return pm;
        }

        public void setPropertiesManager(mxPropertiesManager pm)
        {
            this.pm = pm;
        }

        public Dictionary<String, mxVsdxMaster> getMasterShapes()
        {
            return masters;
        }

        public void setMasterShapes(Dictionary<String, mxVsdxMaster> mm)
        {
            this.masters = mm;
        }

        /**
         * Returns the wrapper of the stylesheet element with id indicated by 'id'
         * @param id StyleSheet's ID.
         * @return StyleSheet element with id = 'id' wrapped in an instance of mxStyleSheet.
         */
        public Style getStylesheet(String id)
        {
            return stylesheets[ id];
        }

        public XmlElement getXmlDoc(String path)
        {
            return this.xmlDocs[path];
        }

        public String getMedia(String path)
        {
            return this.media[path];
        }
    }
}
