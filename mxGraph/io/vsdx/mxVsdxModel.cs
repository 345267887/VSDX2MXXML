using System.Collections.Generic;

/// <summary>
/// Copyright (c) 2006-2016, JGraph Ltd
/// Copyright (c) 2006-2016, Gaudenz Alder
/// </summary>
namespace mxGraph.io.vsdx
{


    using Document = System.Xml.XmlDocument;
    using Element = System.Xml.XmlElement;
    using Node = System.Xml.XmlNode;
    using NodeList = System.Xml.XmlNodeList;

    /// 
    /// <summary>
    /// A model representing vsdx files. As well as being a programmatic model, the XML DOMs of the unzipped
    /// files are held to enable round-tripping
    /// 
    /// </summary>
    public class mxVsdxModel
    {

        /// <summary>
        /// A map of Documents created by reading the XML files, indexed by the path to those files
        /// </summary>
        protected internal IDictionary<string, Document> xmlDocs = null;

        /// <summary>
        /// Collection of media files encoded in Base64, indexed by the path to those files
        /// </summary>
        protected internal IDictionary<string, string> media = null;

        /// <summary>
        /// The document from .../document.xml
        /// </summary>
        protected internal Element rootElement;

        /// <summary>
        /// Map of page objects indexed by their ID.
        /// </summary>
        protected internal IDictionary<int?, mxVsdxPage> pages = null;

        /// <summary>
        /// Map of master objects indexed by their ID. Before you think you're being clever by making
        /// the index an Integer as for pages, don't, there are reasons.
        /// </summary>
        protected internal IDictionary<string, mxVsdxMaster> masters = new Dictionary<string, mxVsdxMaster>();

        /// <summary>
        /// Map stylesheets indexed by their ID
        /// </summary>
        protected internal IDictionary<string, Style> stylesheets = new Dictionary<string, Style>();

        /// <summary>
        /// Map themes indexed by their index
        /// </summary>
        protected internal IDictionary<int?, mxVsdxTheme> themes = new Dictionary<int?, mxVsdxTheme>();

        internal mxPropertiesManager pm;

        public mxVsdxModel(Document doc, IDictionary<string, Document> docData, IDictionary<string, string> mediaData)
        {
            this.xmlDocs = docData;
            this.media = mediaData;

            Node childNode = doc.FirstChild;

            while (childNode != null)
            {
                if (childNode is Element && ((Element)childNode).Name.ToLower().Equals(mxVsdxCodec.vsdxPlaceholder + "document"))
                {
                    this.rootElement = (Element)childNode;
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

        /// <summary>
        /// Initialize theme objects from the XML files
        /// </summary>
        private void initThemes()
        {
            // Lazy build up the master structure
            if (this.xmlDocs != null)
            {
                bool more = true;
                int index = 1;

                while (more)
                {
                    string path = mxVsdxCodec.vsdxPlaceholder + "/theme/theme" + index + ".xml";
                    Document themeDoc = this.xmlDocs.ContainsKey(path)? this.xmlDocs[path]:null;

                    if (themeDoc != null)
                    {
                        Node child = themeDoc.FirstChild;

                        while (child != null)
                        {
                            if (child is Element && ((Element)child).Name.Equals("a:theme"))
                            {
                                mxVsdxTheme theme = new mxVsdxTheme((Element)child);


                                if (theme.ThemeIndex < 0)
                                {
                                    //theme index cannot be determined unless the theme is parsed
                                    theme.processTheme();
                                }

                                //TODO having two theme files with the same id still requires more handling
                                //		probably we need to merge the similar parts (has same theme name)
                                mxVsdxTheme existingTheme = themes.ContainsKey(theme.ThemeIndex)? themes[theme.ThemeIndex]:null;
                                if (existingTheme == null || !existingTheme.Pure)
                                {
                                    if (themes.ContainsKey(theme.ThemeIndex))
                                    {
                                        themes[theme.ThemeIndex] = theme;
                                    }
                                    else
                                    {
                                        themes.Add(theme.ThemeIndex, theme);
                                    }
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

        /// <summary>
        /// Load the map with the stylesheets elements in the document.<br/>
        /// The masters are wrapped for instances of mxStyleSheet. </summary>
        /// <param name="doc"> Document with the stylesheets. </param>
        public virtual void initStylesheets()
        {
            if (rootElement == null) return;
            NodeList vdxSheets = rootElement.GetElementsByTagName(mxVsdxConstants.STYLE_SHEETS);

            if (vdxSheets.Count > 0)
            {
                Element sheets1 = (Element)vdxSheets.Item(0);
                NodeList sheetList = sheets1.GetElementsByTagName(mxVsdxConstants.STYLE_SHEET);
                int sheetLength = sheetList.Count;

                for (int i = 0; i < sheetLength; i++)
                {
                    Element sheet1 = (Element)sheetList.Item(i);
                    string sheetId = sheet1.GetAttribute(mxVsdxConstants.ID);
                    Style sheetElement = new Style(sheet1, this);

                    if (stylesheets.ContainsKey(sheetId))
                    {
                        stylesheets[sheetId] = sheetElement;
                    }
                    else
                    {

                        stylesheets.Add(sheetId, sheetElement);
                    }
                }
            }


            foreach (var item in stylesheets)
            {
                Style sheet = item.Value;
                sheet.stylesheetRefs(this);
            }

            //IDictionary<string, Style>.ValueCollection sheets = stylesheets.Values;
            //IEnumerator<Style> iter = sheets.GetEnumerator();

            //while (iter.MoveNext())
            //{
            //	Style sheet = iter.Current;
            //	sheet.stylesheetRefs(this);
            //}
        }

        /// <summary>
        /// Initialize master objects from the XML files
        /// </summary>
        public virtual void initMasters()
        {
            // Lazy build up the master structure
            if (this.xmlDocs != null)
            {
                string path = mxVsdxCodec.vsdxPlaceholder + "/masters/masters.xml";
                Document masterDoc = this.xmlDocs.ContainsKey(path)? this.xmlDocs[path]:null;

                if (masterDoc != null)
                {
                    Node child = masterDoc.FirstChild;

                    while (child != null)
                    {
                        if (child is Element && ((Element)child).Name.Equals(mxVsdxConstants.MASTERS))
                        {
                            Node grandChild = child.FirstChild;

                            while (grandChild != null)
                            {
                                if (grandChild is Element && ((Element)grandChild).Name.Equals("Master"))
                                {
                                    Element masterElement = (Element)grandChild;
                                    mxVsdxMaster master = new mxVsdxMaster(masterElement, this);
                                    if (this.masters.ContainsKey(master.Id))
                                    {
                                        this.masters.Add(master.Id,master);
                                    }
                                    else
                                    {
                                        this.masters[master.Id] = master;
                                    }
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

        /// <summary>
        /// Initialize page objects from the XML files
        /// </summary>
        public virtual void initPages()
        {
            // Lazy build up the pages structure
            if (this.xmlDocs != null)
            {
                string path = mxVsdxCodec.vsdxPlaceholder + "/pages/pages.xml";
                Document pageDoc =this.xmlDocs.ContainsKey(path)? this.xmlDocs[path]:null;

                if (pageDoc != null)
                {
                    Node child = pageDoc.FirstChild;

                    while (child != null)
                    {
                        if (child is Element && ((Element)child).Name.Equals(mxVsdxConstants.PAGES))
                        {
                            Element pages = (Element)child;

                            NodeList pageList = pages.GetElementsByTagName(mxVsdxConstants.PAGE);

                            if (pageList != null && pageList.Count > 0)
                            {
                                this.pages = new Dictionary<int?, mxVsdxPage>();

                                Dictionary<int?, mxVsdxPage> backgroundMap = new Dictionary<int?, mxVsdxPage>();
                                int pageListLen = pageList.Count;

                                //Find the background pages while creating all the pages
                                for (int i = 0; i < pageListLen; i++)
                                {
                                    Element pageEle = (Element)pageList.Item(i);
                                    mxVsdxPage page = createPage(pageEle);

                                    if (page.Background)
                                    {
                                        backgroundMap[page.Id] = page;
                                    }

                                    this.pages[page.Id] = page;
                                }

                                // Iterate again, assigning background pages
                                foreach (var entry in this.pages)
                                {
                                    mxVsdxPage page = entry.Value;

                                    if (!page.Background)
                                    {
                                        int? backId = page.BackPageId;

                                        if (backId != null)
                                        {
                                            //Import the background.
                                            mxVsdxPage background = backgroundMap[backId];
                                            page.BackPage = background;
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

        public virtual IDictionary<int?, mxVsdxPage> Pages
        {
            get
            {
                return this.pages;
            }
        }

        public virtual IDictionary<int?, mxVsdxTheme> Themes
        {
            get
            {
                return this.themes;
            }
        }

        protected internal virtual Element getRelationship(string rid, string path)
        {
            Document relsDoc = this.xmlDocs.ContainsKey(path)? this.xmlDocs[path]:null;

            if (relsDoc == null || string.ReferenceEquals(rid, null) || rid.Length == 0)
            {
                // Valid to not have a rels for an XML file
                return null;
            }

            NodeList rels = relsDoc.GetElementsByTagName("Relationship");

            for (int i = 0; i < rels.Count; i++)
            {
                Element currElem = (Element)rels.Item(i);
                string id = currElem.GetAttribute("Id");

                if (id.Equals(rid))
                {
                    return currElem;
                }
            }

            return null;
        }

        public virtual mxVsdxMaster getMaster(string masterId)
        {
            return this.masters[masterId];
        }

        protected internal virtual mxVsdxPage createPage(Element pageEle)
        {
            return new mxVsdxPage(pageEle, this);
        }

        public virtual mxPropertiesManager PropertiesManager
        {
            get
            {
                return pm;
            }
            set
            {
                this.pm = value;
            }
        }


        public virtual IDictionary<string, mxVsdxMaster> MasterShapes
        {
            get
            {
                return masters;
            }
            set
            {
                this.masters = value;
            }
        }


        /// <summary>
        /// Returns the wrapper of the stylesheet element with id indicated by 'id' </summary>
        /// <param name="id"> StyleSheet's ID. </param>
        /// <returns> StyleSheet element with id = 'id' wrapped in an instance of mxStyleSheet. </returns>
        public virtual Style getStylesheet(string id)
        {
            if (stylesheets.ContainsKey(id))
            {
                return stylesheets[id];
            }
            return null;
        }

        public virtual Document getXmlDoc(string path)
        {
            if (xmlDocs.ContainsKey(path))
            {
                return this.xmlDocs[path];
            }
            return null;
        }

        public virtual string getMedia(string path)
        {
            if (media.ContainsKey(path))
            {
                return this.media[path];
            }
            return null;
        }
    }

}