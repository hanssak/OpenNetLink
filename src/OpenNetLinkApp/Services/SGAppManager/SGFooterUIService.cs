using System;
using OpenNetLinkApp.Models.SGFooter;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGFooterUIService
    {
        /* To Manage Footer State */
        /// <summary>
        /// get / set FooterUI Property
        /// </summary>
        ISGFooterUI Footer { get; } 
        /// <summary>
        /// Application UI Header Event Delegate, Notified by System, Controlled by User.
        /// </summary>
        event Action OnChangeFooter;
    }
    internal class SGFooterUIService : ISGFooterUIService
    {
        public SGFooterUIService()
        {
            Footer = new SGFooterUI();
        }

        /* To Manage Header State */
        public ISGFooterUI Footer { get; private set; } = null;
        public event Action OnChangeFooter;
        private void NotifyStateChangedFooter() => OnChangeFooter?.Invoke();
    }
}