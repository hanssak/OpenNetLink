using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string Ext { get; set; }
        public int Type { get; set; }       //1:파일 2:디렉토리
        public long dispIndex { get; set; }

        public string getSizeStr()
        {
            string rtn = "";
            if (Size == 0)
            {
                rtn = "";
            }
            if (Size > 1024 * 1024 * 1024)
            {
                float nSize = (float)Size / (1024 * 1024 * 1024);
                rtn = "(" + nSize.ToString("####0.0") + "GB" + ")";
            }
            else if (Size > 1024 * 1024)
            {
                float nSize = (float)Size / (1024 * 1024);
                rtn = "(" + nSize.ToString("####0.0") + "MB" + ")";
            }
            else if (Size > 1024)
            {
                float nSize = (float)Size / (1024);
                rtn = "(" + nSize.ToString("####0.0") + "KB" + ")";
            }
            else if (Size > 0)
                rtn = "(" + Size + " Byte)";
            return rtn;
        }
        public string getNameStr()
        {
            if (Name.Length < 20)
                return Name;
            else
                return Name.Substring(0, 19);
        }

        public SGFileInfo() { }
        public SGFileInfo(string path, string name, int age, long size)
        {
            Name = name;
            Path = path;
            Size = size;
        }
    }
}
