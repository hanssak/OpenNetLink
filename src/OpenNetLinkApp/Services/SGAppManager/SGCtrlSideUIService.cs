using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Components.Web;
using OpenNetLinkApp.Models.SGCtrlSide;
using OpenNetLinkApp.Services.SGAppManager;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGCtrlSideUIService
    {
        /* To Manage ControlSide State */
        /// <summary>
        /// Control Right SideBar Menu, Related App Environment Setting.
        /// </summary>
        ISGCtrlSideUI CtrlSide { get; } 
        /// <summary>
        /// Control Right SideBar Menu Delegate, Related App Environment Setting.
        /// </summary>
        event Action OnChangeCtrlSide;
        /// <summary>
        /// Setting Control Side Object
        /// </summary>
        /// <param name="ControlSize Object"></param>
        /// <returns>void</returns>
        void SetCtrlSide(ISGCtrlSideUI ctrlSide);
    }
    internal class SGCtrlSideUIService : ISGCtrlSideUIService
    {
        public SGCtrlSideUIService()
        {
            CtrlSide = new SGCtrlSideUI();
        }

        /* To Manage ControlSide State */
        public ISGCtrlSideUI CtrlSide { get; private set; } = null; 
        public event Action OnChangeCtrlSide;
        private void NotifyStateChangedCtrlSide() => OnChangeCtrlSide?.Invoke();
        public void SetCtrlSide(ISGCtrlSideUI ctrlSide)
        {
            CtrlSide = ctrlSide;
            NotifyStateChangedCtrlSide();
        }
    }
}