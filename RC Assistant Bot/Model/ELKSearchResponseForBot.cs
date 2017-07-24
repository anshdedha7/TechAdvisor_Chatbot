using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RC_Assistant_Bot.Model
{
    public class ELKSearchResponseForBot
    {
        /// <summary>
        /// A collection of Search Document
        /// </summary>
        public List<BotDocument> Results { get; set; }
    }
}