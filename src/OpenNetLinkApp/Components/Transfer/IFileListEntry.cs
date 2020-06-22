using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Components.Transfer
{
    public interface IFileListEntry
    {
        DateTime LastModified { get; }

        string Name { get; }

        long Size { get; }

        string Type { get; }

        public string RelativePath { get; set; }
                               
        event EventHandler OnDataRead;
    }
}