using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Components.Web;
using OpenNetLinkApp.Services.SGAppManager;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGCtrlSideUIService
    {
        /* To Manage ControlSide State */
        /// <summary>
        /// Control Right SideBar Menu Delegate, Related App Environment Setting.
        /// </summary>
        event Action OnChangeCtrlSide;
    }
    internal class SGCtrlSideUIService : ISGCtrlSideUIService
    {
        public SGCtrlSideUIService()
        {
        }

        /* To Manage ControlSide State */
        public event Action OnChangeCtrlSide;
        private void NotifyStateChangedCtrlSide() => OnChangeCtrlSide?.Invoke();
    }
}