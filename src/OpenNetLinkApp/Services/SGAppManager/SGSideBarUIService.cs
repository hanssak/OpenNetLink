using System;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using OpenNetLinkApp.Models.SGSideBar;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSideBarUIService
    {
        List<ISGSideBarUI> MenuList { get; }
        ISGSideBarUIService AddMenu(int groupId, int menuId, string name, string icon, string path, 
                                  string badgeType = "", string badgeValue= "", string tooltip = "", 
                                  bool actived = false, bool expanded = false); 
        ISGSideBarUIService AddSubMenu(int groupId, int menuId, string name, string icon, string path,
                                  string badgeType = "", string badgeValue= "", string tooltip = "", 
                                  bool actived = false, bool expanded = false); 

        /* To Manage Active Menu State */
        /// <summary>
        /// Current Active Menu Info, Selected by user.
        /// </summary>
        ISGSideBarUI ActiveMenu { get; set; } 
        /// <summary>
        /// Current Active Menu Event Delegate, Selected by User.
        /// </summary>
        event Action OnChangeActMenu;
        /// <summary>
        /// Handler for the key down and moouse click events
        /// </summary>
        /// <param name="eventArgs">keyboard & mouse event</param>
        /// <param name="activeMenu"></param>
        /// <returns>void</returns>
        void ChgActiveMenu(EventArgs eventArgs, ISGSideBarUI activeMenu);
    }
    internal class SGSideBarUIService : ISGSideBarUIService
    {
        public  List<ISGSideBarUI> MenuList { get; private set; }
        public SGSideBarUIService()
        {
            MenuList = new List<ISGSideBarUI>();
        }

        public ISGSideBarUIService AddMenu(int groupId, int menuId, string name, string icon, string path,
                                        string badgeType = "", string badgeValue = "", string tooltip = "", 
                                        bool actived = false, bool expanded = false)
        {
            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                MenuId = menuId,
                Parent = null,
                Name = name,
                Icon = icon,
                Path = path,
                ToolTip = tooltip,
                BadgeType = badgeType,
                BadgeValue = badgeValue,
                Actived = actived,
                Expanded = expanded,
                IsSubMenu = false,
                //Child = new List<ISGSideBarUI>()
                Child = null
            };
            // Same: MenuList.add(menuItem);
            MenuList.Add(menuItem);

            return this;
        }
        public ISGSideBarUIService AddSubMenu(int groupId, int menuId, string name, string icon, string path, 
                                           string badgeType = "", string badgeValue = "", string tooltip = "", 
                                           bool actived = false, bool expanded = false)
        {
            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                MenuId = menuId,
                Parent = MenuList[groupId],
                Name = name,
                Icon = icon,
                Path = path,
                ToolTip = tooltip,
                BadgeType = badgeType,
                BadgeValue = badgeValue,
                Actived = actived,
                Expanded = expanded,
                IsSubMenu = true,
                //Child = new List<ISGSideBarUI>()
                Child = null
            };
            // Same: MenuList[groupId].Child.add(menuItem);
            (MenuList[groupId] as SGSideBarUI).Child ??= new List<ISGSideBarUI>();
            MenuList[groupId].Child.Add(menuItem);

            return this;
        }

        /* To Manage Active Menu State */
        public ISGSideBarUI ActiveMenu { get; set; } = null;
        public event Action OnChangeActMenu;
        private void NotifyStateChangedActMenu() => OnChangeActMenu?.Invoke();
        public void ChgActiveMenu(EventArgs eventArgs, ISGSideBarUI activeMenu)
        {   
            ISGSideBarUI        Node;
            MouseEventArgs      EventMouse;
            KeyboardEventArgs   EventKeyboard;
            if((eventArgs as KeyboardEventArgs) == null) 
            {
                EventMouse = eventArgs as MouseEventArgs;
                if(EventMouse.Button != 0) return ;
            }
            else
            {
                EventKeyboard = eventArgs as KeyboardEventArgs;
                if (EventKeyboard.Key != "Enter" && EventKeyboard.Key != " " && EventKeyboard.Key != "Spacebar") return ;
            }

            /* Initialized */
            if(ActiveMenu == null) 
            {
                foreach(var MenuItem in this.MenuList)
                {
                    (MenuItem as SGSideBarUI).Actived = false;
                    (MenuItem as SGSideBarUI).Expanded = false;
                    foreach(var SubMenuItem in MenuItem.Child)
                    {
                        (SubMenuItem as SGSideBarUI).Actived = false;
                        (SubMenuItem as SGSideBarUI).Expanded = false;
                    }
                }
            }
            
            /* If two object is not same, changed value of previous ActiveMenu with off */
            if(ActiveMenu != null && !Object.ReferenceEquals(ActiveMenu, activeMenu))
            {
                (ActiveMenu as SGSideBarUI).Actived = false;
                (ActiveMenu as SGSideBarUI).Expanded = false;

                /* Change value with off on Parents which selected node  */
                Node = ActiveMenu;
                while (Node.Parent != null)
                {
                    (Node.Parent as SGSideBarUI).Actived = false;
                    (Node.Parent as SGSideBarUI).Expanded = false;
                    Node = Node.Parent;
                }
            }

            /* 
               To Swapping Value When two object is same, 
               buf if object is submenu, change only actived value with unactive (Expected Expanded Vaule)
            */
            if(activeMenu.IsSubMenu && Object.ReferenceEquals(ActiveMenu, activeMenu))
            {
                if(activeMenu.Actived != activeMenu.Actived)
                    (activeMenu as SGSideBarUI).Actived = !activeMenu.Actived;
            }
            else
            {
                (activeMenu as SGSideBarUI).Actived = !activeMenu.Actived;
                (activeMenu as SGSideBarUI).Expanded = !activeMenu.Expanded;
            }

            /* To Change Parents's value with son's value When selected node's value is active */
            if(activeMenu.Actived)
            {
                Node = activeMenu;
                while (Node.Parent != null)
                {
                    {
                        (Node.Parent as SGSideBarUI).Actived = activeMenu.Actived; 
                        (Node.Parent as SGSideBarUI).Expanded = activeMenu.Expanded; 
                    }
                    Node = Node.Parent;
                }
            }

            /* To Save Current Active Menu */
            ActiveMenu = activeMenu;
            /* To Change State of life cycle for Rerendering of Blazor. */
            NotifyStateChangedActMenu();
        }
    }
}