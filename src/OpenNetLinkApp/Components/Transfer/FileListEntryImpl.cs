using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Components.Transfer
{
    // This is public only because it's used in a JSInterop method signature,
    // but otherwise is intended as internal
    public class FileListEntryImpl : IFileListEntry
    {
        
        public event EventHandler OnDataRead;

        public int Id { get; set; }

        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }

        public string RelativePath { get; set; }

        internal void RaiseOnDataRead()
        {
            OnDataRead?.Invoke(this, null);
        }
    }
}