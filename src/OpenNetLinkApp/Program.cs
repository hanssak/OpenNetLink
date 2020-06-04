using WebWindows.Blazor;
using System;

namespace OpenNetLinkApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            ComponentsDesktop.Run<Startup>("Secure Gate App", "wwwroot/index.html");
        }
    }
}
