using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Handlers
{
    public class XSS
    {
        private HtmlSanitizer sanitizer;
        public XSS()
        {
            sanitizer = new HtmlSanitizer();
            //Sanitizer. Allowedtags. Add ("div"); // white list of tags
            sanitizer.AllowedAttributes.Add("class"); // white list of tag attributes, no class tag attribute by default      
                                                      //Sanitizer. Allowedcssproperties. Add ("font family"); // CSS property whitelist
        }

        /// <summary>
        ///XSS filtering
        /// </summary>
        ///< param name = "HTML" > HTML code < / param >
        ///< returns > filter results < / returns >
        public string Filter(string html)
        {
            string str = sanitizer.Sanitize(html);
            return str;
        }
    }
}
