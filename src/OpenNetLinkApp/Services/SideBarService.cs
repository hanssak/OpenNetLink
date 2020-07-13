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

            MenuBuilder.AddItem(1, "파일전송", "transfer", "fas fa-th", isEnabled: true)
                       .AddItem(2, "전송관리", "transManage", "fas fa-tree")
                       .AddItem(3, "결재관리", "transferApprove", "fas fa-tachometer-alt", "right badge badge-danger", match: NavLinkMatch.All)
                       .AddSubMenu(4, "PC URL",
                            new SideBarBuilder().AddItem(1, "PCURL 관리", "/pcurlManage", "far fa-circle")
                                .AddItem(2, "PCURL 결재", "/pcurlApprove", "far fa-circle", isVisible: true), "fas fa-copy", "badge badge-info right")
                       .AddSubMenu(5, "메일관리",
                            new SideBarBuilder().AddItem(1, "메일관리", "mailManage", "far fa-circle")
                                .AddItem(2, "메일결재", "mailApprove", "far fa-circle", isVisible: true), "fas fa-copy", "badge badge-info right")
                       .AddSubMenu(6, "클립보드",
                            new SideBarBuilder().AddItem(1, "클립보드관리", "clipBoardManage", "far fa-circle")
                                .AddItem(2, "클립보드결재", "clipBoardApprove", "far fa-circle", isVisible: true), "fas fa-copy", "badge badge-info right")
                       .AddSubMenu(7, "데모페이지",
                            new SideBarBuilder().AddItem(1, "Window", "/MicrosoftPage/window", "far fa-circle")
                                .AddItem(2, "Hellow Files", "/MicrosoftPage/hellowfiles", "far fa-circle")
                                .AddItem(3, "Input Files", "/MicrosoftPage/indexcommon", "far fa-circle", isVisible: true), "fas fa-copy", "badge badge-info right");
                       
                                             
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