using WebWindows.Blazor;
using System;

namespace OpenNetLinkApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            ComponentsDesktop.Run<Startup>("OpenNetLink", "wwwroot/index.html");
        }
    }
}
