using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGQuery
{
    class ApproveDao
    {
		public string GetClipDataSearch(string[] strArrClipDataType)
		{
			StringBuilder sb = new StringBuilder();

			// a.data_type=0 AND

			//sb.Append("  AND a.data_type IN ('1', '2')");
			if (strArrClipDataType == null || strArrClipDataType.Length < 1)
				sb.Append("AND a.data_type!='0'");
			else
			{
				sb.Append("AND a.data_type IN (");
				int nIdx = 0;
				for (; nIdx < strArrClipDataType.Length; nIdx++)
				{
					sb.Append("'");
					sb.Append(strArrClipDataType[nIdx]);
					sb.Append("'");
					if (nIdx < strArrClipDataType.Length - 1)
						sb.Append(",");
				}
				sb.Append(")");
			}

			return sb.ToString();
		}

		public string List(ApproveParam tParam, bool bNoClipboard, string[] strArrClipDataType = null)
		{
			string mainCdSecValue = tParam.SystemId.Substring(1, 1);

			//날짜 Serach
			string apprString = "";

			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				apprString = $"  AND APPR_REQ_TIME >= '{tParam.SearchFromDay}'";
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				apprString = $"  AND APPR_REQ_TIME <= '{tParam.SearchToDay}'";
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				apprString = $"  AND APPR_REQ_TIME >= '{tParam.SearchFromDay}' AND APPR_REQ_TIME <= '{tParam.SearchToDay}'";

			string requestString = "";
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				requestString = $"  AND A.REQUEST_TIME >= '{tParam.SearchFromDay}'";
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				requestString = $"  AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				requestString = $"  AND A.REQUEST_TIME >= '{tParam.SearchFromDay}' AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";

			//단일망, 다중망일 경우 보낸 곳과 받는 곳의 정보를 받아 결재 리스트를 보여준다.
			string srcDestString = "";
			string srcDestTableString = "";
			if(tParam.NetWorkType == EnumNetWorkType.Single)
            {
				srcDestString = "0 AS SRC_SYSTEM_ID, 0 AS DEST_SYSTEM_ID";
				srcDestTableString = @"TBL_USER_INFO C , TBL_USER_INFO E";
			}
			else
            {
				srcDestString = "D.SRC_SYSTEM_ID, D.DEST_SYSTEM_ID";
				srcDestTableString = "TBL_USER_INFO C, TBL_TRANSFER_REQ_SUB_HIS D, TBL_USER_INFO E";
			}
			string securityOrderNum = "";
			if(!tParam.IsSecurity)
            {
				securityOrderNum = "(APPROVE_ORDER > 100 AND APPROVE_ORDER < 200)";
			}
			else
            {
				securityOrderNum = "(APPROVE_ORDER < 101 OR APPROVE_ORDER > 199)";
			}
			string dataTypeString = "";
			if (bNoClipboard)
				dataTypeString  = " AND A.DATA_TYPE=0";
			else
				dataTypeString = GetClipDataSearch(strArrClipDataType);

            Dictionary<string, string> param = SQLXmlService.Instanse.ConvertClassToDictionary(tParam);
            param.Add("ApprString", apprString);
            param.Add("RequestString", requestString);
			param.Add("SecurityOrderNum", securityOrderNum);
			param.Add("DataTypeString", dataTypeString);

			StringBuilder sb = new StringBuilder();
            SQLXmlService.Instanse.GetSqlQuery("ApproveDaoList", param, ref sb);

			return sb.ToString();
		}
		public string TotalCount(ApproveParam tParam, bool bNoClipboard, string[] strArrClipDataType = null)
        {
			string mainCdSecValue = tParam.SystemId.Substring(1, 1);

			//SFM TYPE이 2일 경우 대결재자도 결재가 가능하게 리스트를 보여준다.
			string sfmString = $@"
		UNION 
		SELECT B.USER_SEQ FROM TBL_USER_INFO A, TBL_USER_SFM B WHERE A.USER_ID = '{tParam.UserID}' AND A.USER_SEQ = B.SFM_USER_SEQ AND TO_CHAR(NOW(), 'YYYYMMDD') BETWEEN B.FROMDATE AND B.TODATE";

			//날짜 Serach
			string apprString = "";
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				apprString = $"  AND APPR_REQ_TIME >= '{tParam.SearchFromDay}'";
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				apprString = $"  AND APPR_REQ_TIME <= '{tParam.SearchToDay}'";
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				apprString = $"  AND APPR_REQ_TIME >= '{tParam.SearchFromDay}' AND APPR_REQ_TIME <= '{tParam.SearchToDay}'";

			string requestString = "";
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				requestString = $"  AND A.REQUEST_TIME >= '{tParam.SearchFromDay}'";
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				requestString = $"  AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				requestString = $"  AND A.REQUEST_TIME >= '{tParam.SearchFromDay}' AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";

			//단일망, 다중망일 경우 보낸 곳과 받는 곳의 정보를 받아 결재 리스트를 보여준다.
			string srcDestString = "";
			string srcDestTableString = "";
			if (tParam.NetWorkType == EnumNetWorkType.Single)
			{
				srcDestString = "0 AS SRC_SYSTEM_ID, 0 AS DEST_SYSTEM_ID";
				srcDestTableString = @"TBL_USER_INFO C , TBL_USER_INFO E";
			}
			else
			{
				srcDestString = "D.SRC_SYSTEM_ID, D.DEST_SYSTEM_ID";
				srcDestTableString = "TBL_USER_INFO C, TBL_TRANSFER_REQ_SUB_HIS D, TBL_USER_INFO E";
			}
			string securityOrderNum = "";
			if (!tParam.IsSecurity)
			{
				securityOrderNum = "(APPROVE_ORDER > 100 AND APPROVE_ORDER < 200)";
			}
			else
			{
				securityOrderNum = "(APPROVE_ORDER < 101 OR APPROVE_ORDER > 199)";
			}
			string dataTypeString = "";
			if (bNoClipboard)
				dataTypeString = " AND A.DATA_TYPE=0";
			else
				dataTypeString = GetClipDataSearch(strArrClipDataType);
			
			Dictionary<string, string> param = SQLXmlService.Instanse.ConvertClassToDictionary(tParam);
			param.Add("ApprString", apprString);
			param.Add("RequestString", requestString);
			param.Add("SecurityOrderNum", securityOrderNum);
			param.Add("DataTypeString", dataTypeString);

			StringBuilder sb = new StringBuilder();
			SQLXmlService.Instanse.GetSqlQuery("ApproveDaoCount", param, ref sb);

			return sb.ToString();
		}
    }
}
