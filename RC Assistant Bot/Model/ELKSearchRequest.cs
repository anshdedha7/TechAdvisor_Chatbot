using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RC_Assistant_Bot.Model
{
    public class ELKSearchRequest
    {
        /// <summary>
        /// Vehicle Year
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Vehicle Make
        /// </summary>
        public string Make { get; set; }

        /// <summary>
        /// Vehicle Model
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Keyword to be searched
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Page Number for required search results. Default Value is 0
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Page size for search results. Default value is 20
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// type of content to fetch
        /// </summary>
        public string ContentType { get; set; }
    }
}