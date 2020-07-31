using System;
using System.Collections.Generic;

namespace OpenNetLinkApp.Models.SGFooter
{
    internal class SGFooterUI : ISGFooterUI
    {
        public SGFooterUI()
        {
            Description = new List<string>();
            Description.Add("(주) 한싹시스템");
            Description.Add("서울시 구로구 디지털로34길 27 401호-403호 (구로동, 대륭포스트타워 3차)");
            Description.Add("COPYRIGHT 2020 HANSSAK. ALL RIGHTS RESERVED");
        }
        public List<string> Description { get; private set;} = null;
    }
}
