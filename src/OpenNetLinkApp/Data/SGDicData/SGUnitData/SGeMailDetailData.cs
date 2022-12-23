using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Blazor.FileReader;
using System.IO;
using System.Runtime.InteropServices;
using Serilog;
using OpenNetLinkApp.Common;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{


    public class SGemailDetailData : SGDetailData
    {
        public SGemailDetailData()
        {
        }

        ~SGemailDetailData()
        {
        }
    }


    public class emailtransData : transData
    {

        public string strReceiver { get; set; }            // 수신자 email

        public emailtransData()
        {
            bCheckDisable = bCheck = false;
            TransSeq = ApvType = TransferType = TransferStatus = ApvStatus = IsFile = Title = RequestTime = "";
            strDestNet = "";
            strReceiver = "";
        }

        public emailtransData(bool bCheckDisable, bool bCheck, string seq, string apvtype, string transfertype, string transferstatus, string apvstatus, string isfile, string title, string reqtime, string strTransStatusCode, string strApprStatusCode, string strDestNetData)
        {
            TransSeq = seq;
            ApvType = apvtype;
            TransferType = transfertype;
            TransferStatus = transferstatus;
            ApvStatus = apvstatus;
            IsFile = isfile;
            Title = title;
            RequestTime = reqtime;
            TransStatusCode = strTransStatusCode;
            ApprStatusCode = strApprStatusCode;
            strDestNet = strDestNetData;
        }
    }



}
