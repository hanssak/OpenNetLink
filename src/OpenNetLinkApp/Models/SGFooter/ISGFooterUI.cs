using System;
using System.Collections.Generic;

namespace OpenNetLinkApp.Models.SGFooter
{
    public interface ISGFooterUI
    {
        string       CorpName { get; }
        List<string> Address { get; }
        List<string> Description { get; }
        List<string> Copyright { get; }
    }
}
