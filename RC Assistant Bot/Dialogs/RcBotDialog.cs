using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using RC_Assistant_Bot.Utility;
using RC_Assistant_Bot.Model;
using System.Xml;
using Microsoft.Bot.Connector;
using System.Text;

namespace RC_Assistant_Bot.Dialogs
{
    [LuisModel("FILL IN YOUR APP ID", "FILL IN YOUR KEY")]
    [Serializable]
    public class RcBotDialog : LuisDialog<object>
    {
        static string year = "2008";
        static string make = "bmw";
        static string model = "128i";
        static string connectionUrl = "http://localhost:63952/";
        static string token = "50026076";
        public static string postId { get; set; }
        StringBuilder strBuilder = new StringBuilder();
        int lineCounter = 1;

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, did not understand.";
            reply(message);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Verbose")]
        public async Task Verbose(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            Random random = new Random();
            var greetings = new List<string> { "Hi!", "Hello", "Hello. How can I help you?", "Hi. How can I help you?"};
            string msg = greetings[random.Next(greetings.Count)];
            reply(msg);
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Locate")]
        public async Task Locate(IDialogContext context, LuisResult result)
        {
            EntityRecommendation component;
            if (result.TryFindEntity("component", out component))
            {
                XmlDocument contentXml = getSearchResultXmlDoc(component.Entity + " component locations", "quickinfo");
                string msg = contentXml.InnerText;
                int indexOfSee = msg.LastIndexOf("See");
                if (indexOfSee > -1)
                {
                    msg = msg.Remove(indexOfSee);
                }
                strBuilder.Append(msg);
                //reply(strBuilder.ToString());
                reply(msg);
                await context.PostAsync(msg);
            }
            else
            {
                string msg = "Sorry. Not sure which part you are talking about.";
                reply(msg);
                await context.PostAsync(msg);
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetInfo")]
        public async Task GetInfo(IDialogContext context, LuisResult result)
        {
            EntityRecommendation component;
            EntityRecommendation specType;
            if (result.TryFindEntity("component", out component))
            {
                XmlDocument contentXml;
                if (result.TryFindEntity("specType", out specType))
                {
                    if (specType.Entity.Contains("number"))
                    {
                        contentXml = getSearchResultXmlDoc(component.Entity + " parts number specification", "quickinfo");
                    }
                    else
                    {
                        contentXml = getSearchResultXmlDoc(component.Entity + " " + specType, "quickinfo");
                    }
                }
                else
                {
                    contentXml = getSearchResultXmlDoc(component.Entity, null);
                }
                string msg = contentXml.InnerText;
                int indexOfSee = msg.LastIndexOf("See");
                if (indexOfSee > -1)
                {
                    msg = msg.Remove(indexOfSee);
                }
                strBuilder.Append(msg);
                //reply(strBuilder.ToString());
                reply(msg);
                await context.PostAsync(msg);
            }
            else
            {
                string msg = "Sorry. Not sure which part you are talking about.";
                reply(msg);
                await context.PostAsync(msg);
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetProcedure")]
        public async Task GetProcedure(IDialogContext context, LuisResult result)
        {
            EntityRecommendation component;
            EntityRecommendation procedure;
            strBuilder = new StringBuilder();
            lineCounter = 1;
            if (result.TryFindEntity("component", out component))
             {
                if (result.TryFindEntity("procedure", out procedure))
                {
                    if (procedure.Entity.Contains("instal"))
                    {
                        string replyStr = CreatePara(getSearchResultXmlDoc(component.Entity + " installation installing install", null));
                        reply(replyStr);
                        await context.PostAsync(replyStr);
                    } else if (procedure.Entity.Contains("remov"))
                    {
                        string replyStr = CreatePara(getSearchResultXmlDoc(component.Entity + " removal remove removing", null));
                        reply(replyStr);
                        await context.PostAsync(replyStr);
                    } else if (procedure.Entity.Contains("locat"))
                    {
                        string replyStr = CreatePara(getSearchResultXmlDoc(component.Entity + " component locations", "quickinfo"));
                        reply(replyStr);
                        await context.PostAsync(replyStr);
                    } else if (procedure.Entity.Contains("replac"))
                    {
                        string replyStr = CreatePara(getSearchResultXmlDoc(component.Entity + " replacing replacement replace", null));
                        reply(replyStr);
                        await context.PostAsync(replyStr);
                    }
                    else
                    {
                        string query = component.Entity + " " + procedure.Entity + " ";
                        int indexOfIng = procedure.Entity.LastIndexOf("ing");
                        if (indexOfIng != -1)
                        {
                            string procName = procedure.Entity.Remove(indexOfIng);
                            query += procName + " " + procName + "e";
                        }else if (procedure.Entity[procedure.Entity.Length - 1] == 'e')
                        {
                            string procName = procedure.Entity.Remove(procedure.Entity.Length - 1);
                            query += procName + "ing";
                        }
                        else
                        {
                            query += procedure.Entity + "ing";
                        }

                        string replyStr = CreatePara(getSearchResultXmlDoc(query, null));
                        reply(replyStr);
                        await context.PostAsync(replyStr);
                    }

                }
                else
                {
                    reply("Sorry, did not understand.");
                    await context.PostAsync("Sorry, did not understand.");
                }
            }
            else if(result.TryFindEntity("procedure", out procedure))
            {
                string replyStr = CreatePara(getSearchResultXmlDoc(procedure.Entity, null));
                reply(replyStr);
                await context.PostAsync(replyStr);
            }
            else
            {
                reply("Sorry, did not understand.");
                await context.PostAsync("Sorry, did not understand.");
            }
            context.Wait(MessageReceived);
        }

        public RcBotDialog()
        {
        }
        public RcBotDialog(ILuisService service)
            : base(service)
        {
        }

        private XmlDocument getSearchResultXmlDoc(string keywords, string type)
        {
            ELKSearchResponseForBot response = ELKSearchUtil.GetSearchResultItems(connectionUrl, year, make, model, keywords, 0, 1, type);
            strBuilder.Append("Information found in ");
            foreach (string title in response.Results[0].title.Split('~'))
            {
                strBuilder.Append(" --> " + title);
            }
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(Environment.NewLine);
            XmlDocument contentXml = new XmlDocument();
            contentXml.LoadXml(response.Results[0].contentXml);
            return contentXml;
        }

        private void reply(string content)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:7801/comments");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Headers.Add("ApiKey", String.Format("API:00000378-0004-002c-090a-0b0c0d0e0f10:password"));
            httpWebRequest.Headers.Add("HMACKey", String.Format("TOKEN:" + token));
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"PostId\":\""+postId+ "\",\"AuthorId\":\"3C94B9B9-7999-42DF-903D-A6D901262226\",\"Content\":\"" + content+"\"}";
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        private string CreatePara(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.SelectSingleNode(".//messages");
            if(node != null)
            {
                buildContentString(node.ChildNodes);
                return strBuilder.ToString();
            }
            else
            {
                return "This question is incorrect.";
            }
        }

        private void buildContentString(XmlNodeList contentList)
        {
            if (contentList.Count > 0)
            {
                foreach (XmlNode node in contentList)
                {
                    buildContentString(node.ChildNodes);
                    if(node.ChildNodes.Count < 1 && node.Name.Equals("#text"))
                    {
                        strBuilder.Append(lineCounter + ". " + node.InnerText);
                        strBuilder.Append(Environment.NewLine);
                        lineCounter++;
                    }
                }
            }
        }
    }
}