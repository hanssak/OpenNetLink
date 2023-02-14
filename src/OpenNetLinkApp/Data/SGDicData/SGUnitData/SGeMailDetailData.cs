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

    public class emailApproveData : SGApproveData
    {

        public string strIsFileAdd { get; set; }            // 파일첨부

        public string strSender { get; set; }            // 발신자

        public string strReceiver { get; set; }            // 수신자 email

        public bool bCanApproveReject { get; set; }      // 승인 / 반려를 할 수 있는지 유무

        public bool bIsAfterApprove { get; set; } = false;    // 승인 / 반려를 할 수 있는지 유무

        public bool bIsCanFilePreview { get; set; } = false;    // 파일 미리보기가 가능한지 유무

        public emailApproveData()
        {
            bCheckDisable = bCheck = false;
            TransSeq = ApvType = TransferType = TransStatusCode = ApvStatus = IsFile = Title = RequestTime = "";
            strDestNet = "";
            strSender = "";
            strReceiver = "";
        }

        public emailApproveData(bool bCheckDisable, bool bCheck, string seq, string apvtype, string transfertype, string transferstatus, string apvstatus, string isfile, string title, string reqtime, string strTransStatusCode, string strApprStatusCode, string strDestNetData)
        {
            TransSeq = seq;
            ApvType = apvtype;
            TransferType = transfertype;
            TransStatusCode = transferstatus;
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
