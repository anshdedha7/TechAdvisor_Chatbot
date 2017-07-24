using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace RC_Assistant_Bot.Utility
{
    public class ELKSearchHelper
    {
        private string m_sELKSearchApiBaseURL = "";
        private string m_sELKSearchApiEndpoint = "";
        private string elkSearchURL = "";

        public string ELKSearchApiBaseURL
        {
            get { return this.m_sELKSearchApiBaseURL; }
            set
            {
                this.m_sELKSearchApiBaseURL = value;
            }
        }

        public string ELKSearchApiEndpoint
        {
            get { return this.m_sELKSearchApiEndpoint; }
            set
            {
                this.m_sELKSearchApiEndpoint = value;
            }
        }

        public string GetResults(string a_searchRequest)
        {
            string result = string.Empty;
            HttpWebResponse response;
            response = null;
            string requestUrl = String.Format("{0}/{1}", ELKSearchApiBaseURL, ELKSearchApiEndpoint);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

                request.Method = "POST";

                string body = a_searchRequest;
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                request.ContentType = "application/xml";
                request.Accept = "application/xml";
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                using (response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();
                        }
                        return result;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                if (response != null) response.Close();
                return result;
            }
        }
    }
}