using System;
using OpenNetLinkApp.Models.SGUserInfo;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGCorpIdUIService
    {
        /* To Manage Corporate Identity State */
        /// <summary>
        /// CIPath is CI Image path.
        /// </summary>
        string CIPath { get; } 
        /// <summary>
        /// CI Event Delegate, Modified by User or System.
        /// </summary>
        event Action OnChangeCI;
        /// <summary>
        /// To Set CI Image File Path
        /// </summary>
        /// <param name="ciPath"> 
        /// </param>
        /// <returns>void</returns>
        void SetCIPath(string ciPath);
    }
    internal class SGCorpIdUIService : ISGCorpIdUIService
    {
        public string CIPath { get; private set; } = String.Empty;
        public event Action OnChangeCI;
        private void NotifyStateChangedCI() => OnChangeCI?.Invoke();
        public void SetCIPath(string ciPath)
        {
            CIPath = ciPath;
            NotifyStateChangedCI();
        }
    }
}