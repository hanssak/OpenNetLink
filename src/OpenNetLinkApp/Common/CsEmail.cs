using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Common
{
    public class emalApprover
    {
        public string approveOrder;
        public string approverName;
        public string approverRank;
        public string approveFlag;
        public string approveResTime;
        public string apprRejectReason;
    }
    public class emailFile
    {
        public string fileDLP;
        public string fileDLPContents;
        public string fileName;
        public string fileKind;
        public string virusDesc;
        public string fileSize;
        public string fileKey;
        public string fileSeq;
        public bool bFilePreviewPossiable;

        public emailFile()
        {
            fileDLP = "";
            fileDLPContents = "";
            fileName = "";
            fileKind = "";
            virusDesc = "";
            fileSize = "";
            fileKey = "";
            fileSeq = "";
            bFilePreviewPossiable = true;
        }
    }

}
