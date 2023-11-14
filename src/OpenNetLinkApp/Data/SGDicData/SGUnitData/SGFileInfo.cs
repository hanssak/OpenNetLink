using OpenNetLinkApp.Common;
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
        public int Type { get; set; }       //1:파일 2:디렉토리 3:상위 디렉토리버튼
        public DateTime LastModeified { get; set; }
        public long dispIndex { get; set; }

        public string getSizeStr()
        {
            string rtn = "";
            if (Size == 0)
            {
                rtn = "";
                return rtn;
            }

            rtn = CsFunction.GetSizeStr(Size);
            rtn = "(" + rtn + ")";
            return rtn;
        }
        public string getNameStr(bool longType = true)
        {
            if (longType)
            {
                if (Name.Length < 40)
                    return Name;
                else
                    return Name.Substring(0, 39);
            }
            else
            {
                if (Name.Length < 20)
                    return Name;
                else
                    return Name.Substring(0, 19);
            }
        }

        public string GetIconColor()
        {
            if (Type != 1)
                return "#f0cb00";



            switch (Ext.Replace(".", "").ToUpper())
            {
                #region Audio Type
                case "3GP":
                case "AIFF":
                case "AAC":
                case "ALAC":
                case "AMR":
                case "ATRAC":
                case "AU":
                case "AWB":
                case "DVF":
                case "FLAC":
                case "MMF":
                case "MP3":
                case "MPC":
                case "MSV":
                case "OGG":
                case "OPUS":
                case "RA&RM":
                case "TTA":
                case "VOX":
                case "WAV":
                case "WMA":
                    #endregion                    
                #region Video Type
                case "MP4":
                case "MOV":
                case "WMV":
                case "AVI":
                case "AVCHD":
                case "SWF":
                case "MKV":
                case "MPG":
                case "MP2":
                case "MPE":
                case "MPV":
                case "MPEG":
                case "WEBM":
                case "FLV":
                case "F4V":
                case "F4P":
                case "F4A":
                case "F4B":
                case "GIF":
                case "M4P":
                case "M4V":
                    #endregion
                    return "#fd7e14";
                #region Compress Type
                case "ARC":
                case "ARJ":
                case "AS":
                case "B64":
                case "BTOA":
                case "BZ":
                case "BZ2":
                case "CAB":
                case "CPT":
                case "GZ":
                case "HQX":
                case "ISO":
                case "LHA":
                case "LZH":
                case "MIM":
                case "MME":
                case "PAK":
                case "PF":
                case "RAR":
                case "RPM":
                case "SEA":
                case "SIT":
                case "SITX":
                case "TAR":
                case "TBZ":
                case "TBZ2":
                case "TGZ":
                case "UU":
                case "UUE":
                case "Z":
                case "ZIP":
                case "ZIPX":
                case "ZOO":
                    #endregion
                    return "#f0cb00";
                #region Excel Type
                case "XLSX":
                case "XLSM":
                case "XLSB":
                case "XLTX":
                case "XLTM":
                case "XLS":
                case "XLT":
                case "XLAM":
                case "XLA":
                case "XLW":
                case "XLR":
                    #endregion
                    return "#28a745";
                #region Image Type
                case "APNG":
                case "AVIF":
                // case "GIF":
                case "JPG":
                case "JPEG":
                case "JFIF":
                case "PJPEG":
                case "PJP":
                case "PNG":
                case "SVG":
                case "WEBP":
                case "BMP":
                case "ICO":
                case "CUR":
                case "TIF":
                case "TIFF":
                    #endregion
                    return "#f0cb00";
                #region PPT Type
                case "PPTX":
                case "PPTM":
                case "PPT":
                case "POTX":
                case "POTM":
                case "POT":
                case "PPSX":
                case "PPSM":
                case "PPS":
                case "PPAM":
                case "PPA":
                    #endregion
                    return "#ff0000";
                #region Word Type
                case "DOC":
                case "DOCM":
                case "DOCX":
                case "DOT":
                case "DOTM":
                case "DOTX":
                    #endregion
                    return "#007bff";
                #region Hwp Type
                case "HML":
                case "HWP":
                case "HWT":
                case "HWTX":
                case "HWPX":
                    #endregion
                    return "#00b4ff";
                case "HTML":
                case "HTM":
                    return "#ff0000";
                case "TXT":
                case "RTF":
                    return "#00b4ff";
                case "PDF":
                    return "#ff0000";
                default:
                    return "#8a8a8a";
            }
        }
        public string GetIcon()
        {
            if (Type != 1)
                return "fas fa-folder";

            switch (Ext.Replace(".", "").ToUpper())
            {
                #region Audio Type
                case "3GP":
                case "AIFF":
                case "AAC":
                case "ALAC":
                case "AMR":
                case "ATRAC":
                case "AU":
                case "AWB":
                case "DVF":
                case "FLAC":
                case "MMF":
                case "MP3":
                case "MPC":
                case "MSV":
                case "OGG":
                case "OPUS":
                case "RA&RM":
                case "TTA":
                case "VOX":
                case "WAV":
                case "WMA":
                    #endregion
                    return "fas fa-play-circle";
                #region Video Type
                case "MP4":
                case "MOV":
                case "WMV":
                case "AVI":
                case "AVCHD":
                case "SWF":
                case "MKV":
                case "MPG":
                case "MP2":
                case "MPE":
                case "MPV":
                case "MPEG":
                case "WEBM":
                case "FLV":
                case "F4V":
                case "F4P":
                case "F4A":
                case "F4B":
                case "GIF":
                case "M4P":
                case "M4V":
                    #endregion
                    return "far fa-file-video";
                #region Compress Type
                case "ARC":
                case "ARJ":
                case "AS":
                case "B64":
                case "BTOA":
                case "BZ":
                case "BZ2":
                case "CAB":
                case "CPT":
                case "GZ":
                case "HQX":
                case "ISO":
                case "LHA":
                case "LZH":
                case "MIM":
                case "MME":
                case "PAK":
                case "PF":
                case "RAR":
                case "RPM":
                case "SEA":
                case "SIT":
                case "SITX":
                case "TAR":
                case "TBZ":
                case "TBZ2":
                case "TGZ":
                case "UU":
                case "UUE":
                case "Z":
                case "ZIP":
                case "ZIPX":
                case "ZOO":
                    #endregion
                    return "far fa-file-archive";
                #region Excel Type
                case "XLSX":
                case "XLSM":
                case "XLSB":
                case "XLTX":
                case "XLTM":
                case "XLS":
                case "XLT":
                case "XLAM":
                case "XLA":
                case "XLW":
                case "XLR":
                    #endregion
                    return "fas fa-file-excel";
                #region Image Type
                case "APNG":
                case "AVIF":
                // case "GIF":
                case "JPG":
                case "JPEG":
                case "JFIF":
                case "PJPEG":
                case "PJP":
                case "PNG":
                case "SVG":
                case "WEBP":
                case "BMP":
                case "ICO":
                case "CUR":
                case "TIF":
                case "TIFF":
                    #endregion
                    return "fas fa-file-image";
                #region PPT Type
                case "PPTX":
                case "PPTM":
                case "PPT":
                case "POTX":
                case "POTM":
                case "POT":
                case "PPSX":
                case "PPSM":
                case "PPS":
                case "PPAM":
                case "PPA":
                    #endregion
                    return "fas fa-file-powerpoint";
                #region Word Type
                case "DOC":
                case "DOCM":
                case "DOCX":
                case "DOT":
                case "DOTM":
                case "DOTX":
                    #endregion
                    return "fas fa-file-word";
                #region Hwp Type
                case "HML":
                case "HWP":
                case "HWT":
                case "HWTX":
                case "HWPX":
                    #endregion
                    return "fas fa-h-square";
                case "HTML":
                case "HTM":
                    return "fab fa-html5";
                case "TXT":
                case "RTF":
                    return "fas fa-file-alt";
                case "PDF":
                    return "far fa-file-pdf";
                default:
                    return "fas fa-file";
            }
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
