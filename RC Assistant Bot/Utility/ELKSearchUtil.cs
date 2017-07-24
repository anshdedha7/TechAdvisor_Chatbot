using RC_Assistant_Bot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace RC_Assistant_Bot.Utility
{
    public class ELKSearchUtil
    {
        private const string DefaultAPIEndpoint = "GetSearchResultsForBot";

        public static ELKSearchResponseForBot GetSearchResultItems(string a_sSearchApiBaseURL, string a_sVehicleYear, string a_sVehicleMake, string a_sVehicleModel, string a_sSearchKey, int a_iStartIndex, int a_iCount, string a_contentType)
        {
            string APIEndpoint = string.Empty;
            ELKSearchHelper m_objWSSearchHelper = new ELKSearchHelper();
            m_objWSSearchHelper.ELKSearchApiBaseURL = a_sSearchApiBaseURL;

            string queryPacket = GetSearchRequest(a_sVehicleYear, a_sVehicleMake, a_sVehicleModel, a_sSearchKey, a_iStartIndex, a_iCount, a_contentType, out APIEndpoint);
            m_objWSSearchHelper.ELKSearchApiEndpoint = APIEndpoint;

            string response = m_objWSSearchHelper.GetResults(queryPacket);
            if (!string.IsNullOrEmpty(response))
            {
                return parseReponsePacket(response);
            }
            else
            {
                return null;
            }
        }

        private static string GetSearchRequest(string a_sVehicleYear, string a_sVehicleMake, string a_sVehicleModel, string a_sSearchKey, int a_iStartIndex, int a_iCount, string a_contentType, out string APIEndpoint, int a_iTopicGroupId = -1, int a_iVehicleId = -1)
        {
            string searchRequest = string.Empty;

            
                var request = new ELKSearchRequest()
                {
                    Year = a_sVehicleYear,
                    Make = a_sVehicleMake,
                    Model = a_sVehicleModel,
                    Keyword = a_sSearchKey,
                    PageIndex = a_iStartIndex - 1,
                    PageSize = a_iCount,
                    ContentType = a_contentType
                };

                searchRequest = SerializeUtils.serialize(typeof(ELKSearchRequest), request);
                APIEndpoint = DefaultAPIEndpoint;
            
            return searchRequest;
        }

        private static ELKSearchResponseForBot parseReponsePacket(string response)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            ELKSearchResponseForBot obj = null;
            try
            {
                strReader = new StringReader(response);
                serializer = new XmlSerializer(typeof(ELKSearchResponseForBot));
                xmlReader = new XmlTextReader(strReader);
                obj = (ELKSearchResponseForBot)serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }
    }
}