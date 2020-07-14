using System;
using OpenNetLinkApp.Models.SGNetwork;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGNetworkService
    {
        ISGNetworkInfo NetWorkInfo { get; }
    }
    internal class SGNetworkService : ISGNetworkService
    {
        public SGNetworkService()
        {
        }

        /* To Manage User Info State */
        public ISGNetworkInfo NetWorkInfo { get; private set; } = null;
    }
}