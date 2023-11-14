using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Serilog;
using AgLogManager;
using Microsoft.AspNetCore.Components.Web;
using OpenNetLinkApp.Models.SGSideBar;
using OpenNetLinkApp.Services.SGAppManager;


namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSideBarUIService
    {
        List<ISGSideBarUI> MenuList { get; }
        ISGSideBarUIService AddRoot(int groupId, LSIDEBAR categoryId, string fromName, string toName, string icon, string path,
                                string badgeType = "", string badgeValue = "", string tooltip = "",
                                bool actived = false, bool expanded = false, string strUserSeq = "");
        ISGSideBarUIService AddMenu(int groupId, int Id, LSIDEBAR categoryId, string name, string icon, string path,
                                string badgeType = "", string badgeValue = "", string tooltip = "",
                                bool actived = false, bool expanded = false, bool bUse = true, string strUserSeq = "");
        ISGSideBarUI AddSubMenu(int groupId, int Id, int parentId, LSIDEBAR categoryId, string name, string icon, string path,
                                string badgeType = "", string badgeValue = "", string tooltip = "",
                                bool actived = false, bool expanded = false, bool bUse = true, string strUserSeq = "");

        ISGSideBarUI FindSubMenu(int groupId, int parentId, int Id);

        ISGSideBarUI FindMenu(int groupId, int parentId);
        ISGSideBarUI FindSubMenu(int groupId, string path);

        ISGSideBarUI FindRootMenu(int groupId);

        bool DeleteMenu(int groupId, int parentId, int Id);


        bool DeleteMenuAllButRoot(int groupId);


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

        void EmitNotifyStateChangedActMenu();

        void DeleteAllItem();
    }
    internal class SGSideBarUIService : ISGSideBarUIService
    {

        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGSideBarUIService>();

        public List<ISGSideBarUI> MenuList { get; private set; }
        public SGSideBarUIService()
        {
            try
            {
                MenuList = new List<ISGSideBarUI>();
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"SGSideBarUIService Exception :{ex.ToString()}");
            }
        }

        public ISGSideBarUIService AddRoot(int groupId, LSIDEBAR categoryId, string fromName, string toName, string icon, string path,
                                        string badgeType = "", string badgeValue = "", string tooltip = "",
                                        bool actived = false, bool expanded = false, string strUserSeq = "")
        {

            foreach (ISGSideBarUI Item in MenuList)
            {
                if (Item.GroupId == groupId && Item.CategoryId == categoryId)
                {
                    ISGSideBarUI menuItem2 = null;
                    return this;
                }
            }

            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                Idx = -1,
                ParentId = -1,
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
                Child = null,
                strItemUserSeq = strUserSeq
            };
            // Same: MenuList.add(menuItem);

            MenuList.Add(menuItem);

            return this;
        }
        public ISGSideBarUIService AddMenu(int groupId, int Id, LSIDEBAR categoryId, string name, string icon, string path,
                                           string badgeType = "", string badgeValue = "", string tooltip = "",
                                           bool actived = false, bool expanded = false, bool bUse = true, string strUserSeq = "")
        {
            if (!bUse)
                return this;
            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                Idx = Id,
                ParentId = Id,
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
                Child = null,
                strItemUserSeq = strUserSeq
            };
            // Same: MenuList[groupId].Child.add(menuItem);

            (MenuList[groupId] as SGSideBarUI).Child ??= new List<ISGSideBarUI>();
            MenuList[groupId].Child?.Add(menuItem);

            return this;
        }
        public ISGSideBarUI AddSubMenu(int groupId, int Id, int parentId, LSIDEBAR categoryId, string name, string icon, string path,
                                           string badgeType = "", string badgeValue = "", string tooltip = "",
                                           bool actived = false, bool expanded = false, bool bUse = true, string strUserSeq = "")
        {
            if (!bUse)
                return null;

            ISGSideBarUI parent = MenuList[groupId]?.Child?.Find(child => child.Idx == parentId);
            ISGSideBarUI menuItem = new SGSideBarUI
            {
                GroupId = groupId,
                Idx = Id,
                ParentId = parentId,
                CategoryId = categoryId,
                Parent = parent,
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
                Child = null,
                strItemUserSeq = strUserSeq
            };
            // Same: MenuList[groupId].Child.add(menuItem);

            (parent as SGSideBarUI).Child ??= new List<ISGSideBarUI>();
            parent.Child?.Add(menuItem);

            return menuItem;
        }

        public ISGSideBarUI FindSubMenu(int groupId, int parentId, int Id)
        {
            try
            {
                if (MenuList[groupId] == null ||
                    MenuList[groupId].Child[parentId] == null ||
                    MenuList[groupId].Child[parentId].Child == null)
                    return null;

                foreach (var item in MenuList[groupId].Child[parentId].Child)
                {
                    if (item.Idx == Id)
                    {
                        return item;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                CLog.Here().Information($"FindSubMenu-Exception(Msg) : {e.Message}");
                return null;
            }
        }

        public ISGSideBarUI FindMenu(int groupId, int parentId)
        {
            try
            {
                if (MenuList[groupId] == null ||
                    MenuList[groupId].Child == null ||
                    MenuList[groupId].Child.Count < 1)
                    return null;

                foreach (var item in MenuList[groupId].Child)
                {
                    if (item.Idx == parentId)
                    {
                        return item;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                CLog.Here().Information($"FindSubMenu-Exception(Msg) : {e.Message}");
                return null;
            }
        }

        public ISGSideBarUI FindSubMenu(int groupId, string path)
        {
            try
            {
                if (MenuList[groupId] == null ||
                    MenuList[groupId].Child == null ||
                    MenuList[groupId].Child.Count < 1)
                    return null;

                foreach (ISGSideBarUI ui in MenuList[groupId].Child)
                {
                    foreach (ISGSideBarUI childUi in ui.Child)
                    {
                        if (childUi.Path.ToUpper().Equals(path.ToUpper()))
                            return childUi;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                CLog.Here().Information($"FindSubMenu-Exception(Msg) : {e.Message}");
                return null;
            }
        }

        public ISGSideBarUI FindRootMenu(int groupId)
        {
            try
            {
                return MenuList[groupId];
            }
            catch (Exception e)
            {
                CLog.Here().Information($"FindRootMenu-Exception(Msg) : {e.Message}");
                return null;
            }
        }


        public bool DeleteMenu(int groupId, int parentId, int Id)
        {
            try
            {
                if (MenuList[groupId] == null ||
                    MenuList[groupId].Child[parentId] == null ||
                    MenuList[groupId].Child[parentId].Child == null)
                    return false;

                int nIdx = 0;
                int nCount = MenuList[groupId].Child[parentId].Child.Count;

                for (; nIdx < nCount; nIdx++)
                {
                    if (MenuList[groupId].Child[parentId].Child[nIdx].Idx == Id)
                    {
                        MenuList[groupId].Child[parentId].Child.RemoveAt(nIdx);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                CLog.Here().Information($"DeleteMenu-Exception(Msg) : {e.Message}");
            }

            return false;

        }


        public bool DeleteMenuAllButRoot(int groupId)
        {
            try
            {
                if (MenuList[groupId] == null)
                    return false;

                MenuList[groupId].Child?.Clear();
                /*                if (MenuList[groupId].Child != null)
                                    MenuList[groupId].Child.Clear();
                */
                NotifyStateChangedActMenu();

                return true;
            }
            catch (Exception e)
            {
                CLog.Here().Information($"DeleteMenuAllButRoot-Exception(Msg) : {e.Message}");
            }

            return false;
        }


        /* To Manage Active Menu State */
        public ISGSideBarUI ActiveMenu { get; set; } = null;
        public event Action OnChangeActMenu;
        private void NotifyStateChangedActMenu() => OnChangeActMenu?.Invoke();

        public void ChgActiveMenu(EventArgs eventArgs, ISGSideBarUI activeMenu)
        {
            ISGSideBarUI Node;
            MouseEventArgs EventMouse;
            KeyboardEventArgs EventKeyboard;
            if ((eventArgs as KeyboardEventArgs) == null)
            {
                EventMouse = eventArgs as MouseEventArgs;
                if (EventMouse.Button != 0) return;
            }
            else
            {
                EventKeyboard = eventArgs as KeyboardEventArgs;
                if (EventKeyboard.Key != "Enter" && EventKeyboard.Key != " " && EventKeyboard.Key != "Spacebar") return;
            }

            /* Initialized */
            if (ActiveMenu == null)
            {
                foreach (var RootItem in this.MenuList)
                {
                    (RootItem as SGSideBarUI).Actived = false;
                    (RootItem as SGSideBarUI).Expanded = false;
                    foreach (var MenuItem in RootItem.Child)
                    {
                        (MenuItem as SGSideBarUI).Actived = false;
                        (MenuItem as SGSideBarUI).Expanded = false;
                        foreach (var SubMenuItem in MenuItem.Child)
                        {
                            (SubMenuItem as SGSideBarUI).Actived = false;
                            (SubMenuItem as SGSideBarUI).Expanded = false;
                        }
                    }
                }
            }

            /* If two object is not same, changed value of previous ActiveMenu with off */
            if (ActiveMenu != null && !Object.ReferenceEquals(ActiveMenu, activeMenu))
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
            if (Object.ReferenceEquals(ActiveMenu, activeMenu))
            {
                (activeMenu as SGSideBarUI).Actived = !activeMenu.Actived;
                (activeMenu as SGSideBarUI).Expanded = !activeMenu.Expanded;
            }
            /* Different from before menu */
            if (!Object.ReferenceEquals(ActiveMenu, activeMenu))
            {
                /* When Root, My son */
                if (!activeMenu.IsSubMenu && Object.ReferenceEquals(ActiveMenu.Parent, activeMenu))
                {
                    (activeMenu as SGSideBarUI).Actived = false;
                    (activeMenu as SGSideBarUI).Expanded = false;
                }
                /* When Root, Son of another Parent */
                else if (!activeMenu.IsSubMenu && !Object.ReferenceEquals(ActiveMenu.Parent, activeMenu))
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
            if (activeMenu.Actived)
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
        public void EmitNotifyStateChangedActMenu() => NotifyStateChangedActMenu();
        public void DeleteAllItem()
        {
            MenuList.Clear();
        }

        public void DeleteItem()
        {
            //MenuList.RemoveAt();
        }

        public void DeleteItemRange()
        {
            //MenuList.RemoveRange();
        }

    }
    public static class MenuNameMapper
    {
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}