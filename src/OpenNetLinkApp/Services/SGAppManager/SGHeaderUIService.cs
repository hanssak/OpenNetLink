using System;
using OpenNetLinkApp.Models.SGHeader;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGHeaderUIService
    {
        /* To Manage Header State */
        /// <summary>
        /// get / set HeaderUI Property
        /// </summary>
        ISGHeaderUI Header { get; } 
        /// <summary>
        /// Application UI Header Event Delegate, Notified by System, Controlled by User.
        /// </summary>
        event Action OnChangeHeader;
        /// <summary>
        /// </summary>
        /// <param name="header"> 
        /// </param>
        /// <returns>void</returns>
        void SetHeaderUI(ISGHeaderUI header);
    }
    internal class SGHeaderUIService : ISGHeaderUIService
    {
        /* To Manage Header State */
        public ISGHeaderUI Header { get; private set; } = null;
        public event Action OnChangeHeader;
        private void NotifyStateChangedHeader() => OnChangeHeader?.Invoke();
        public void SetHeaderUI(ISGHeaderUI header)
        {
            Header = header;
            NotifyStateChangedHeader();
        }
    }
}