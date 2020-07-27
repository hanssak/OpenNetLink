using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Components.Web;
using OpenNetLinkApp.Models.SGSideBar;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSideBarUIService
    {
        List<ISGSideBarUI> MenuList { get; }
        ISGSideBarUIService AddMenu(int groupId, LSIDEBAR categoryId, string fromName, string toName, string icon, string path, 
                                  string badgeType = "", string badgeValue= "", string tooltip = "", 
                                  bool actived = false, bool expanded = false); 
        ISGSideBarUIService AddSubMenu(int groupId, LSIDEBAR categoryId, string name, string icon, string path,
                                  string badgeType = "", string badgeValue= "", string tooltip = "", 
                                  bool actived = false, bool expanded = false,bool bUse=true); 

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

        void NotifyStateChangedActMenu();

        void DeleteAllItem();
    }
    internal class SGSideBarUIService : ISGSideBarUIService
    {
        public  List<ISGSideBarUI> MenuList { get; private set; }
        public SGSideBarUIService()
        {
            MenuList = new List<ISGSideBarUI>();
        }

        public ISGSideBarUIService AddMenu(int groupId, LSIDEBAR categoryId, string fromName, string toName, string icon, string path,
                                        string badgeType = "", string badgeValue = "", string tooltip = "", 
                                        bool actived = false, bool expanded = false)
        {
            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                CategoryId = categoryId,
                Parent = null,
                FromName = fromName,
                ToName = toName,
                Icon = icon,
                Path = path,
                ToolTip = tooltip,
                BadgeType = badgeType,
                BadgeValue = badgeValue,
                Actived = actived,
                Expanded = expanded,
                IsSubMenu = false,
                DicChild = null
            };
            // Same: MenuList.add(menuItem);
            MenuList.Add(menuItem);

            return this;
        }
        public ISGSideBarUIService AddSubMenu(int groupId, LSIDEBAR categoryId, string name, string icon, string path, 
                                           string badgeType = "", string badgeValue = "", string tooltip = "", 
                                           bool actived = false, bool expanded = false, bool bUse=true)
        {
            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                CategoryId = categoryId,
                Parent = MenuList[groupId],
                FromName = "",
                ToName = name,
                Icon = icon,
                Path = path,
                ToolTip = tooltip,
                BadgeType = badgeType,
                BadgeValue = badgeValue,
                Actived = actived,
                Expanded = expanded,
                IsSubMenu = true,
                DicChild = null
            };
            // Same: MenuList[groupId].Child.add(menuItem);
            if (bUse)
            {
                (MenuList[groupId] as SGSideBarUI).DicChild ??= new Dictionary<LSIDEBAR, List<ISGSideBarUI>>();
                if (MenuList[groupId].DicChild.ContainsKey(categoryId) == false) /* Category is not exist */
                {
                    MenuList[groupId].DicChild.Add(categoryId, new List<ISGSideBarUI>());
                    MenuList[groupId].DicChild[categoryId].Add(menuItem);
                }
                else /* Category is already exist */
                    MenuList[groupId].DicChild[categoryId].Add(menuItem);
            }

            return this;
        }

        /* To Manage Active Menu State */
        public ISGSideBarUI ActiveMenu { get; set; } = null;
        public event Action OnChangeActMenu;
        public void NotifyStateChangedActMenu() => OnChangeActMenu?.Invoke();
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
                    foreach(var SubMenuDic in MenuItem.DicChild)
                    {
                        foreach(var SubMenuItem in SubMenuDic.Value)
                        {
                            (SubMenuItem as SGSideBarUI).Actived = false;
                            (SubMenuItem as SGSideBarUI).Expanded = false;
                        }
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
            if(Object.ReferenceEquals(ActiveMenu, activeMenu))
            {
                (activeMenu as SGSideBarUI).Actived = !activeMenu.Actived;
                (activeMenu as SGSideBarUI).Expanded = !activeMenu.Expanded;
            }
            /* Different from before menu */
            if(!Object.ReferenceEquals(ActiveMenu, activeMenu))
            {
                /* When Root, My son */
                if(!activeMenu.IsSubMenu && Object.ReferenceEquals(ActiveMenu.Parent, activeMenu))
                {
                    (activeMenu as SGSideBarUI).Actived = false;
                    (activeMenu as SGSideBarUI).Expanded = false;
                }
                /* When Root, Son of another Parent */
                else if(!activeMenu.IsSubMenu && !Object.ReferenceEquals(ActiveMenu.Parent, activeMenu))
                {
                    (activeMenu as SGSideBarUI).Actived = true;
                    (activeMenu as SGSideBarUI).Expanded = true;
                }
                else
                {
                    /* When SubMenu */
                    (activeMenu as SGSideBarUI).Actived = !activeMenu.Actived;
                    (activeMenu as SGSideBarUI).Expanded = !activeMenu.Expanded;
                }
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
        public void DeleteAllItem()
        {
            MenuList.Clear();
        }
    }
    public static class MenuNameMapper
    {
        public static string GetDescription(Enum value)
        {
            FieldInfo fi= value.GetType().GetField(value.ToString()); 
            DescriptionAttribute[] attributes = 
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return (attributes.Length>0)?attributes[0].Description:value.ToString();
        }
    }
}