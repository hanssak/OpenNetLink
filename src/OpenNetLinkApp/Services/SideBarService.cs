using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Routing;
using OpenNetLinkApp.Components.SideBar;

namespace OpenNetLinkApp.Services
{
    public class SideBarService
    {
        SideBarBuilder MenuBuilder = new SideBarBuilder();
        public SideBarBuilder Builder
        {
            get
            {
                return MenuBuilder;
            }
        }
        public SideBarService()
        {

            MenuBuilder.AddItem(1, "PCURL 관리", "/pcurlManage", "fas fa-tachometer-alt", "right badge badge-danger", match: NavLinkMatch.All)
                       .AddItem(2, "PCURL 결재", "/pcurlApprove", "fas fa-th", isEnabled: true)
                       .AddSubMenu(3, "Sub Menu1",
                                    new SideBarBuilder().AddItem(1, "Counter", "/MicrosoftPage/counter", "far fa-circle")
                                                        .AddItem(2, "Fetch Data", "/MicrosoftPage/fetchdata", "far fa-circle", isEnabled: true)
                                                        .AddItem(3, "You Can't See Me", "invisible", "far fa-circle", isVisible: false), "fas fa-copy", "badge badge-info right")
                       .AddSubMenu(4, "Sub Menu2",
                                    new SideBarBuilder().AddItem(1, "Counter", "counter", "far fa-circle")
                                                        .AddItem(2, "Fetch Data", "fetchdata", "far fa-circle", isEnabled: false)
                                                        .AddItem(3, "You Can See Me", "visible", "far fa-circle", isVisible: true), "fas fa-copy", "badge badge-info right")
                       .AddItem(5, "결재관리", "approveNext", "fas fa-tree")
                       .AddItem(6, "파일전송", "transfer", "fas fa-tree")
                       .AddItem(7, "전송관리", "transManage", "fas fa-tree")
                       .AddItem(8, "메일관리", "mailManage", "fas fa-tree")
                       .AddItem(9, "메일결재", "mailApprove", "fas fa-tree")
                       .AddItem(10, "DragDropList", "/MicrosoftPage/fetchdragdata", "fas fa-tree")
                       .AddItem(11, "Window", "/MicrosoftPage/window", "fas fa-tree");
                       
        }

        public IEnumerable<MenuItem> MenuItems
        {
            get
            {
                return MenuBuilder.Build(x => x.Position);
            }
        }

/*
        public IEnumerable<MenuItem> Filter(string term)
        {
            Func<string, bool> contains = value => value.Contains(term, StringComparison.OrdinalIgnoreCase);

            Func<MenuItem, bool> filter = (menuitem) => contains(menuitem.Title) || (menuitem.Tags != null && menuitem.Tags.Any(contains));

            return MenuItems.Where(category => category.Children != null && category.Children.Any(filter))
                           .Select(category => new MenuItem()
                           {
                               Name = category.Name,
                               Expanded = true,
                               Children = category.Children.Where(filter).ToArray()
                           }).ToList();
        }

        public Example FindCurrent(Uri uri)
        {
            return Examples.SelectMany(example => example.Children ?? new[] { example })
                           .FirstOrDefault(example => example.Path == uri.AbsolutePath || $"/{example.Path}" == uri.AbsolutePath);
        }

        public string TitleFor(Example example)
        {
            if (example != null && example.Name != "First Look")
            {
                return example.Title ?? $"Blazor {example.Name} | a free UI component by Radzen";
            }

            return "Free Blazor Components | 50+ controls by Radzen";
        }

        public string DescriptionFor(Example example)
        {
            return example?.Description ?? "The Radzen Blazor component library provides more than 50 UI controls for building rich ASP.NET Core web applications.";
        }
        */
    }
}