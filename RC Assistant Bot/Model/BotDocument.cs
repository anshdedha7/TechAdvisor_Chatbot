using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RC_Assistant_Bot.Model
{
    public class BotDocument
    {
        /// <summary>
        /// Article Title
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Article Url Path
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// Article type
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Article keywords 
        /// </summary>
        public string keywords { get; set; }

        /// <summary>
        /// Article keywords 
        /// </summary>
        public string imageCaptions { get; set; }

        /// <summary>
        /// Article keywords 
        /// </summary>
        public string contentXml { get; set; }
    }
}