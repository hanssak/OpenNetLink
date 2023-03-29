using System;
using System.Collections.Generic;

namespace OpenNetLinkApp.Models.SGFooter
{
    internal class SGFooterUI : ISGFooterUI
    {
        public SGFooterUI()
        {
            Address = new List<string>();
            Description = new List<string>();
            Copyright = new List<string>();
            CorpName = "";
            //Address.Add("서울시 구로구 디지털로34길 27 401호-403호 (구로동, 대륭포스트타워 3차)");

            //Copyright.Add("COPYRIGHT © HANSSAK. ALL RIGHTS RESERVED");
            Copyright.Add("Korea Exchange");
            
        }
        public string       CorpName { get; private set;} = null;
        public List<string> Address { get; private set;} = null;
        public List<string> Description { get; private set;} = null;
        public List<string> Copyright { get; private set;} = null;
    }
}
