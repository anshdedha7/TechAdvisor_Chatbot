using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;

namespace RC_Assistant_Bot.Library
{
    public class TOCGenerator
    {
        public TOCGenerator()
        {
        }

        public string GenerateTOCXml(string sXML, out string sFirstSectionId)
        {
            string sRetString = null;
            sFirstSectionId = "";
            //Initalize source xml.
            XmlDocument xd = new XmlDocument();
            if (sXML == null)
                return null;
            xd.Load(new StringReader(sXML));

            XPathNavigator xdNav = xd.CreateNavigator();
            //Initalize transformation
            XslTransform xslTr = new XslTransform(); //do not use - is deprecated.
            //XslCompiledTransform tr = new XslCompiledTransform();
            xslTr.Load(ConfigurationSettings.AppSettings["XSLT_XML2TOCNEW_PATH"].ToString().Trim());
            XsltArgumentList xslArg = new XsltArgumentList();
            //xslArg.AddParam("UID", "", a_sRefthingID);
            //Run transformation
            StringWriter sw = new StringWriter();
            xslTr.Transform(xdNav, xslArg, sw);
            sRetString = sw.ToString();
            xd.Load(new StringReader(sRetString));
            sFirstSectionId = xd.SelectSingleNode("TOC/section").Attributes["UID"].Value;
            return sRetString;
        }

        public string GenerateTOCHtml(string sXML, string sXSLT_TOC_Path, int iTocDepth, string sAticleId) //MOdified by Vivek : TocDepth from DB
        {
            //This function isCALLED FROM 2 files.Hence PASSING xslt PATH argument.
            string sRetString = null;
            //Initalize source xml.
            XmlDocument xd = new XmlDocument();
            if (sXML == null)
                return null;
            xd.Load(new StringReader(sXML));
            XPathNavigator xdNav = xd.CreateNavigator();
            //Initalize transformation
            XslTransform xslTr = new XslTransform(); //do not use - is deprecated.
            //XslCompiledTransform tr = new XslCompiledTransform();
            xslTr.Load(sXSLT_TOC_Path);
            XsltArgumentList xslArg = new XsltArgumentList();

            //int iDisplayLevel = 0;
            //Int32.TryParse(ConfigurationSettings.AppSettings[ConfigConstants.m_sXSLT_TOC_DEPTH].ToString().Trim(), out iDisplayLevel);
            int iDisplayLevel = iTocDepth; // Modified by Vivek : TocDepth from DB 

            if (iDisplayLevel <= 0)
            {
                iDisplayLevel = 0;
            }
            xslArg.AddParam("DisplayLevel", "", iDisplayLevel);
            xslArg.AddParam("ParentArticleId", "", sAticleId);
            //Run transformation
            StringWriter sw = new StringWriter();
            xslTr.Transform(xdNav, xslArg, sw);
            sRetString = sw.ToString();
            return sRetString;
        }

        /// <summary>
        /// Transform fully loaded article into HTML
        /// </summary>
        /// <param name="sXML"></param>
        /// <param name="sXSLT_TOC_Path"></param>
        /// <param name="iTocDepth"></param>
        /// <param name="a_bLoadCompleteArticle"></param>
        /// <returns></returns>
        public string GenerateTOCHtml(string sXML, string sXSLT_TOC_Path, int iTocDepth, bool a_bLoadCompleteArticle)
        {
            //This function isCALLED FROM 2 files.Hence PASSING xslt PATH argument.
            string sRetString = null;
            //Initalize source xml.
            XmlDocument xd = new XmlDocument();
            if (sXML == null)
                return null;
            xd.Load(new StringReader(sXML));
            XPathNavigator xdNav = xd.CreateNavigator();
            //Initalize transformation
            XslTransform xslTr = new XslTransform(); //do not use - is deprecated.
            //XslCompiledTransform tr = new XslCompiledTransform();
            xslTr.Load(sXSLT_TOC_Path);
            XsltArgumentList xslArg = new XsltArgumentList();

            int iDisplayLevel = iTocDepth;

            if (iDisplayLevel <= 0)
            {
                iDisplayLevel = 0;
            }
            xslArg.AddParam("DisplayLevel", "", iDisplayLevel);
            xslArg.AddParam("bLoadCompleteArticle", "", a_bLoadCompleteArticle);

            StringWriter sw = new StringWriter();
            xslTr.Transform(xdNav, xslArg, sw);
            sRetString = sw.ToString();
            return sRetString;
        }

        /// <summary>
        /// Created by Gaurav K Arya for CSRL
        /// </summary>
        /// <param name="sXML">XML content in string format</param>
        /// <param name="sXSLT_TOC_Path">Path of XSLT file that will transform the XML</param>
        /// <param name="iTocDepth">Depth of sections to show</param>
        /// <param name="parentAticleId">Id of the parent Article in Left pane of CSRL screen</param>
        /// <param name="tocCheckBoxClass">State that all checkboxes in lower pane will have</param>
        /// <returns></returns>
        public string GenerateTOCHtml(string sXML, string sXSLT_TOC_Path, int iTocDepth, string parentAticleId, string tocCheckBoxClass) //MOdified by Vivek : TocDepth from DB
        {
            //This function isCALLED FROM 2 files.Hence PASSING xslt PATH argument.
            string sRetString = null;
            //Initalize source xml.
            XmlDocument xd = new XmlDocument();
            if (sXML == null)
                return null;
            xd.Load(new StringReader(sXML));
            XPathNavigator xdNav = xd.CreateNavigator();
            //Initalize transformation
            XslTransform xslTr = new XslTransform(); //do not use - is deprecated.
            //XslCompiledTransform tr = new XslCompiledTransform();
            xslTr.Load(sXSLT_TOC_Path);
            XsltArgumentList xslArg = new XsltArgumentList();

            //int iDisplayLevel = 0;
            //Int32.TryParse(ConfigurationSettings.AppSettings[ConfigConstants.m_sXSLT_TOC_DEPTH].ToString().Trim(), out iDisplayLevel);
            int iDisplayLevel = iTocDepth; // Modified by Vivek : TocDepth from DB 

            if (iDisplayLevel <= 0)
            {
                iDisplayLevel = 0;
            }
            xslArg.AddParam("DisplayLevel", "", iDisplayLevel);
            xslArg.AddParam("ParentArticleId", "", parentAticleId);
            xslArg.AddParam("CheckBoxClass", "", tocCheckBoxClass);
            //Run transformation
            StringWriter sw = new StringWriter();
            xslTr.Transform(xdNav, xslArg, sw);
            sRetString = sw.ToString();
            return sRetString;
        }

        /// <summary>
        /// Modified on 14-dec-2010 for DTC implementation for RCTA 4.0
        /// </summary>
        /// <param name="sXML"></param>
        /// <param name="sRefId"></param>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        public string GetSectionFromRefId(string sXML, string sRefId, out string bookmarkId)
        {
            XmlDocument xd = new XmlDocument();
            bookmarkId = null;
            if (sXML == null)
                return null;

            xd.Load(new StringReader(sXML));
            XmlNodeList xmlNodes = null;
            xmlNodes = xd.GetElementsByTagName("refthing");
            XmlNode section = null;
            string sSectionId = null;

            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["id"] != null)
                {
                    if (node.Attributes["id"].Value.Equals(sRefId, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (node.NextSibling != null && node.NextSibling.Name.Equals("section", StringComparison.CurrentCultureIgnoreCase))
                        {
                            section = node.NextSibling;
                            sSectionId = section.Attributes["UID"].Value;
                            bookmarkId = sSectionId;
                        }
                        else  // In case refthing is inside section tag and the immediate sibling is not a section
                        {
                            if (node.ParentNode != null && node.ParentNode.Name.Equals("section", StringComparison.CurrentCultureIgnoreCase))
                            {
                                section = node.ParentNode;
                                sSectionId = section.Attributes["UID"].Value;
                                bookmarkId = sRefId;
                            }
                        }
                        break;
                    }
                }
            }
            return sSectionId;
        }

        /// <summary>
        /// Searches content sequentially in Article XML.-By Tanima
        /// </summary>
        /// <param name="sXML"></param>
        /// <param name="sKeyWord"></param>
        /// <param name="currSecUID"></param>
        /// <returns></returns>
        public string GetSectionFromKeyWord(string sXML, string sKeyWord, string currSecUID)
        {

            if (sXML == null)
                return null;

            string sectionIDofKeyWord = null;
            XmlDocument xd = new XmlDocument();
            xd.Load(new StringReader(sXML));

            //**If currSecUID belongs to a child section, get the parent section based on the child's UID
            //**Child could be a 'table' or a 'section' tag, hence the 'or' condition
            string xpath = "//section[descendant::section[@UID='" + currSecUID + "'] or descendant::table[@UID='" + currSecUID + "']]";

            XmlNodeList nodelist = xd.SelectNodes(xpath);
            XmlNode currentParentNode = null;

            if (nodelist.Count > 0)
            {
                currentParentNode = nodelist[0];
            }
            else
            {
                xpath = "//section[@UID='" + currSecUID + "']"; //**If currSecUID is a parent section
                currentParentNode = xd.SelectSingleNode(xpath);
            }

            xpath = "following-sibling::section";//**To search content sequentially, get the sibling nodes just after current node.
            sectionIDofKeyWord = IterateNodesGetID(sKeyWord, currentParentNode.SelectNodes(xpath));

            if (string.IsNullOrEmpty(sectionIDofKeyWord))//**If keyword not found , get the sibling nodes just before current node.
            {
                xpath = "preceding-sibling::section";
                sectionIDofKeyWord = IterateNodesGetID(sKeyWord, currentParentNode.SelectNodes(xpath));
            }

            return sectionIDofKeyWord;
        }

        /// <summary>
        /// Helper function to loop through sibling nodes of currently selected node
        /// </summary>
        /// <param name="sKeyWord"></param>
        /// <param name="NodeList"></param>
        /// <returns></returns>
        private string IterateNodesGetID(string sKeyWord, XmlNodeList NodeList)
        {
            string sectionIDofKeyWord = null;
            sKeyWord = sKeyWord.ToLower();//** perform case insensitive search- Tanima
            foreach (XmlNode node in NodeList)
            {
                if (node.InnerText.ToLower().Contains(sKeyWord)) //** perform case insensitive search-Tanima
                {
                    if (node.Attributes["UID"] != null)//if attribute is null, sectionIDofKeyWord will be null, so no need for else condition
                    {
                        sectionIDofKeyWord = node.Attributes["UID"].Value;
                        break;
                    }
                }
            }
            return sectionIDofKeyWord;
        }


        /// <summary>
        /// Searches content in Entire Article XML.
        /// </summary>
        /// <param name="sXML"></param>
        /// <param name="sPartNum"></param>
        /// <returns></returns>
        public string GetSectionFromPartNum(string sXML, string sPartNum)
        {
            XmlDocument xd = new XmlDocument();
            if (sXML == null)
                return null;
            xd.Load(new StringReader(sXML));
            string sectionId = null;
            XmlNodeList xmlNodes = xd.SelectNodes("document/documentbody/section");
            XmlNode section = null;
            sPartNum = sPartNum.ToLower();//** perform case insensitive search- Tanima
            foreach (XmlNode node in xmlNodes)
            {
                if (node.InnerText.ToLower().Contains(sPartNum))//** perform case insensitive search-Tanima
                {
                    section = node;
                    break;
                }
            }
            if (section != null)
            {
                if (section.Attributes["UID"] != null)
                {
                    return section.Attributes["UID"].Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}