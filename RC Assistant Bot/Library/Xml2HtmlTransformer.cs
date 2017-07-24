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
using com.mitchell.crs.web.lib;
using com.mitchell.crs.web.lib.Transformation;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;

namespace RC_Assistant_Bot.Library
{
    public class Xml2HtmlTransformer
    {
        #region Private Variables
        private int a_iArticleId = -1;
        private int a_iVehicleId = -1;
        private int a_iTopicID = -1;

        private string a_sXmlContent = null;
        private string a_sXslFilePath = null;
        private string a_sYear = null;
        private string a_sMake = null;
        private string a_sModel = null;
        private string a_sArticlePath = null;
        private string a_sectionUID = null;
        private string a_sMultipleUIDs = null;//refers to Filter params
        private string a_sParentArticleUniqueId = null;
        private string a_sArticleHeading = null;
        private string a_sShowBackLink = "0"; //Default value is zero to hide back link
        private string a_sBackLinkOnClickFunction = null;

        private bool a_bIsBookmark = false;
        private bool a_bDisplayExternalLink = false;
        private bool a_bLoadCompleteArticle = false;
        private bool a_bCSRLPDF = false;//Used for CSRL Print preview and PDF
        private bool a_bCSRLBackToTop = true; //Used to decide whether to display 'Back to top' link or not
        private bool a_bICarEmbedded = false;
        private DateTime a_dCurrdate;
        #endregion

        #region Properties
        public int ArticleId { set { a_iArticleId = value; } }
        public int VehicleId { set { a_iVehicleId = value; } }
        public int TopicID { set { a_iTopicID = value; } }

        public string XmlContent { set { a_sXmlContent = value; } }
        public string XslFilePath { set { a_sXslFilePath = value; } }
        public string VehicleYear { set { a_sYear = value; } }
        public string VehicleMake { set { a_sMake = value; } }
        public string VehicleModel { set { a_sModel = value; } }
        public string ArticlePath { set { a_sArticlePath = value; } }
        public string SectionUID { set { a_sectionUID = value; } }
        public string ParentArticleUniqueId { set { a_sParentArticleUniqueId = value; } }
        public string MultipleUIDs { set { a_sMultipleUIDs = value; } }
        public string ArticleHeading { set { a_sArticleHeading = value; } }
        public string ShowBackLink { set { a_sShowBackLink = value; } }
        public string BackLinkOnClickFunction { set { a_sBackLinkOnClickFunction = value; } }

        public bool IsBookmark { set { a_bIsBookmark = value; } }
        public bool DisplayExternalLink { set { a_bDisplayExternalLink = value; } }
        public bool LoadCompleteArticle { set { a_bLoadCompleteArticle = value; } }
        public bool CSRLPDF { set { a_bCSRLPDF = value; } } //Used for CSRL Print preview and PDF
        public bool CSRLBackToTop { set { a_bCSRLBackToTop = value; } } //Used to decide whether to display 'Back to top' link or not
        /// <summary>
        /// Set true if article is ICar advantage and in embedded mode
        /// Added by Gaurav for seperate Print link in ICar in embedded mode
        /// </summary>
        public bool IsICarEmbedded { set { a_bICarEmbedded = value; } }
        public DateTime Currdate { set { a_dCurrdate = value; } }
        #endregion

        #region Public Constructor
        public Xml2HtmlTransformer()
        {
        }
        #endregion Public Constructor

        #region Data Members

        /// <summary>
        /// Purpose	:Builds Argument List based on properties set from outside of the class i.e. from calling function
        /// </summary>
        /// <returns></returns>
        private XsltArgumentList ReturnXSLArgumentList()
        {

            XsltArgumentList xslArg = new XsltArgumentList();

            if (!string.IsNullOrEmpty(a_sYear))
            {
                xslArg.AddParam("year", "", a_sYear.ToLower());
            }
            if (!string.IsNullOrEmpty(a_sMake))
            {
                xslArg.AddParam("make", "", a_sMake.ToLower());
            }
            if (!string.IsNullOrEmpty(a_sModel))
            {
                xslArg.AddParam("model", "", a_sModel.ToLower());
            }
            if (!string.IsNullOrEmpty(a_sArticlePath))
            {
                xslArg.AddParam("articlepath", "", a_sArticlePath.ToLower());
            }
            if (!string.IsNullOrEmpty(a_sectionUID))
            {
                xslArg.AddParam("secuid", "", a_sectionUID.ToLower());
            }
            if (!string.IsNullOrEmpty(a_sArticleHeading))
            {
                xslArg.AddParam("articleheading", "", a_sArticleHeading);
            }
            if (!string.IsNullOrEmpty(a_sShowBackLink))
            {
                xslArg.AddParam("showBackLink", "", a_sShowBackLink);
            }
            if (!string.IsNullOrEmpty(a_sBackLinkOnClickFunction))
            {
                xslArg.AddParam("backLinkOnClickFunction", "", a_sBackLinkOnClickFunction);
            }

            if (a_iTopicID > -1)
            {
                xslArg.AddParam("topicid", "", a_iTopicID);
            }

            if (a_iArticleId > -1)
            {
                xslArg.AddParam("articleid", "", a_iArticleId);
            }
            if (a_iVehicleId > -1)
            {
                xslArg.AddParam("vehicleid", "", a_iVehicleId);
            }
            if (!string.IsNullOrEmpty(a_sParentArticleUniqueId))
            {
                xslArg.AddParam("parentArticleUniqueId", "", a_sParentArticleUniqueId);
            }

            if (!string.IsNullOrEmpty(a_sMultipleUIDs))
            {
                xslArg.AddParam("filter", "", a_sMultipleUIDs);
            }

            // Need to further verify this parameter in which scenario it is required; for now, we overwrite the passed value with current date time
            xslArg.AddParam("currdate", "", DateTime.Now.ToString("yyyyMMdd"));
            xslArg.AddParam("bIsBookmark", "", a_bIsBookmark);
            xslArg.AddParam("extlinks", "", a_bDisplayExternalLink);
            xslArg.AddParam("bLoadCompleteArticle", "", a_bLoadCompleteArticle);
            xslArg.AddParam("bCSRL_PDF", "", a_bCSRLPDF);
            xslArg.AddParam("bCSRL_BackToTop", "", a_bCSRLBackToTop);
            xslArg.AddParam("bIsICarEmbedded", "", a_bICarEmbedded);//Added by Gaurav for seperate Print link in ICar in embedded mode

            return xslArg;
        }

        /// <summary>
        /// Purpose	:A Generic method to transform XML Content of RCTA to its corresponding HTML and display it in different formats like TOC/ Article section at UI
        /// </summary>
        /// <returns></returns>
        public string Transform()
        {
            string sRetString = null;

            try
            {
                //Initalize source xml.
                XmlDocument xd = new XmlDocument();

                if (a_sXmlContent == null)
                    return null;

                xd.Load(new StringReader(a_sXmlContent));

                XPathNavigator xdNav = xd.CreateNavigator();

                //Initalize transformation
                XslTransform xslTr = new XslTransform(); //do not use - is deprecated.
                //XslCompiledTransform tr = new XslCompiledTransform();
                xslTr.Load(a_sXslFilePath);

                //Build parameter list.
                XsltArgumentList xslArg = ReturnXSLArgumentList();

                //Run transformation
                StringWriter sw = new StringWriter();
                xslTr.Transform(xdNav, xslArg, sw);
                sRetString = sw.ToString();
                sw.Close();
            }
            catch (XsltException ex)
            {
            }

            return sRetString;
        }


        /// <summary>
        /// Obsolete now.In use in RCTA 3.1; Generate TOC from XML
        /// </summary>
        /// <param name="a_sTOCXmlStr">XML string</param>
        /// <returns></returns>
        public toc generateXMLtoTOC(string a_sTOCXmlStr)
        {
            toc toc = null;
            toc = (toc)deserialize(typeof(toc), a_sTOCXmlStr);
            return toc;
        }

        private object deserialize(Type t, string xmlData)
        {
            XmlSerializer xs = new XmlSerializer(t);
            UnicodeEncoding encoding = new UnicodeEncoding();
            MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(xmlData.Trim()));
            object o = xs.Deserialize(memoryStream);
            memoryStream.Close();
            return o;
        }

        #endregion Data Members
    }
}