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


			string sql = $@"
SELECT * FROM (
SELECT A.TRANS_SEQ, B.REQ_SEQ, A.DLP, C.USER_ID, C.USER_NAME, C.USER_RANK,
CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IO_TYPE,
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS,
A.APPROVE_KIND, B.APPROVE_FLAG, A.TITLE,  TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUEST_TIME,
TO_CHAR(TO_TIMESTAMP(B.APPR_RES_TIME,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'C' AS APPROVETABLEPOS, 
GETAPPROVEPOSSIBLE(B.APPROVE_USER_SEQ, A.TRANS_SEQ) AS APPROVEPOSSIBLE, B.APPROVESTATE,
CASE WHEN ((SELECT COUNT(*) FROM TBL_FORWARD_INFO WHERE TRANS_SEQ = A.TRANS_SEQ) + (SELECT COUNT(*) FROM TBL_FORWARD_INFO_HIS WHERE TRANS_SEQ = A.TRANS_SEQ)) > 0  THEN '1' 
ELSE '0' END AS FORWARD_TYPE,
{srcDestString},
COALESCE((SELECT APPROVE_REAL_NAME || ' ' || APPROVE_REAL_RANK FROM TBL_APPROVE_REAL_HIS WHERE REQ_SEQ = B.REQ_SEQ), E.USER_NAME || ' ' || E.USER_RANK) AS APPROVE_REAL_USER_NAME
FROM TBL_TRANSFER_REQ_INFO A, 
( 
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '2' AS 
	APPROVESTATE FROM TBL_APPROVE_HIS 
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	{apprString}
	AND APPROVE_USER_SEQ <> USER_SEQ
	UNION ALL 
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_INFO
	WHERE {securityOrderNum}                                                
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	AND APPROVE_USER_SEQ <> USER_SEQ
	UNION ALL
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_AFTER
	WHERE {securityOrderNum}
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	AND APPROVE_USER_SEQ <> USER_SEQ ) B,
	{srcDestTableString}
	WHERE A.TRANS_SEQ = B.TRANS_SEQ AND A.USER_SEQ = C.USER_SEQ AND B.APPROVE_USER_SEQ = E.USER_SEQ AND A.USER_SEQ <> B.APPROVE_USER_SEQ ";
			if (tParam.NetWorkType == EnumNetWorkType.Multiple)
				sql += "AND A.TRANS_SEQ = D.TRANS_SEQ";
			if (!tParam.IsSecurity)
			{
				if (bNoClipboard)
					sql += "AND A.DATA_TYPE=0 ";
				else
					sql += GetClipDataSearch(strArrClipDataType);
			}
			sql += $@"
	{requestString}
	AND A.TRANS_FLAG <> '6'
	AND  ( SUBSTRING(A.SYSTEM_ID, 1, 1) ='I' OR  SUBSTRING(A.SYSTEM_ID, 1, 1) ='E' )
UNION ALL 
SELECT A.TRANS_SEQ, B.REQ_SEQ, A.DLP, C.USER_ID, C.USER_NAME, C.USER_RANK, 
CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IO_TYPE, 
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS, A.APPROVE_KIND,
B.APPROVE_FLAG, A.TITLE,  
TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUEST_TIME,
TO_CHAR(TO_TIMESTAMP(B.APPR_RES_TIME,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'H' AS APPROVETABLEPOS, GETAPPROVEPOSSIBLE(B.APPROVE_USER_SEQ, A.TRANS_SEQ) AS APPROVEPOSSIBLE, B.APPROVESTATE,
CASE WHEN ((SELECT COUNT(*) FROM TBL_FORWARD_INFO WHERE TRANS_SEQ = A.TRANS_SEQ) + (SELECT COUNT(*) FROM TBL_FORWARD_INFO_HIS WHERE TRANS_SEQ = A.TRANS_SEQ)) > 0  THEN '1' 
ELSE '0' END AS FORWARD_TYPE, 
{srcDestString},
COALESCE((SELECT APPROVE_REAL_NAME || ' ' || APPROVE_REAL_RANK FROM TBL_APPROVE_REAL_HIS WHERE REQ_SEQ = B.REQ_SEQ), E.USER_NAME || ' ' || E.USER_RANK) AS APPROVE_REAL_USER_NAME 
FROM TBL_TRANSFER_REQ_HIS A, ( 
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '2' AS APPROVESTATE 
	FROM TBL_APPROVE_HIS 
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	{apprString}
	AND APPROVE_USER_SEQ <> USER_SEQ 
	UNION ALL
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_INFO
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@" )
	AND APPROVE_USER_SEQ <> USER_SEQ
	UNION ALL
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_AFTER
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	AND APPROVE_USER_SEQ <> USER_SEQ) B, 
	{srcDestTableString}
	WHERE A.TRANS_SEQ = B.TRANS_SEQ AND A.USER_SEQ = C.USER_SEQ AND B.APPROVE_USER_SEQ = E.USER_SEQ AND A.USER_SEQ <> B.APPROVE_USER_SEQ 
";
			if (tParam.NetWorkType == EnumNetWorkType.Multiple)
				sql += " AND A.TRANS_SEQ = D.TRANS_SEQ";

			if (!tParam.IsSecurity)
			{
				if (bNoClipboard)
					sql += " AND A.DATA_TYPE=0";
				else
					sql += GetClipDataSearch(strArrClipDataType);
			}

			sql += $@"
	{requestString}			
	AND  ( SUBSTRING(A.SYSTEM_ID, 1, 1) ='I' OR  SUBSTRING(A.SYSTEM_ID, 1, 1) ='E' )
) AS X
WHERE 1=1";
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sql += $"  AND IO_TYPE = '{tParam.TransKind}'";
			}
			//*****************************************************************************
			//여기 맞춰야함 WEBLINK기준이라 넷링크랑 다를수 있음 2020/07/27 YKH
			//*****************************************************************************
			if (!tParam.ApprStatus.Equals("5"))
			{
				if (tParam.ApprStatus.Equals("1"))
				{
					sql += $" AND APPROVE_FLAG = '1' AND APPROVESTATE != '2'";
				}
				else if (tParam.ApprStatus.Equals("4"))
				{       //승인불필요
					sql += $" AND APPROVE_FLAG = '1' AND APPROVESTATE = '2'";
				}
				else
				{
					if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
						sql += $"  AND APPROVE_FLAG = '{tParam.ApprStatus}'";
				}
			}
			if (tParam.ApprKind != null && tParam.ApprKind.Length > 0)
			{
				sql += $"  AND APPROVE_KIND = '{tParam.ApprKind}'";
			}
			
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sql += $"  AND UPPER(TITLE) LIKE UPPER('%' || '{tParam.Title}' || '%')";
			}
			if (tParam.ReqUserName != null && tParam.ReqUserName.Length > 0)
			{
				sql += $" AND USER_NAME LIKE '%' || '{tParam.ReqUserName}' || '%'";
			}
			// 변경 : 송신기준
			if (tParam.Src_system_id != null && tParam.Src_system_id.Length > 0)
			{
				sql += $" AND (SUBSTRING(SRC_SYSTEM_ID,1,1) = '{tParam.Src_system_id.Substring(0, 1)}')";
			}
			if (tParam.Dest_system_id != null && tParam.Dest_system_id.Length > 0)
			{
				sql += $" AND (SUBSTRING(DEST_SYSTEM_ID,1,2) = '{tParam.Dest_system_id.Substring(0, 2)}')";
			}

			sql += $@" 
ORDER BY REQUEST_TIME DESC
LIMIT {tParam.PageListCount} OFFSET ({tParam.ViewPageNo} -1) * {tParam.PageListCount}";

			return sql;

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


			string sql = $@"
SELECT COUNT(*) FROM (
SELECT A.TRANS_SEQ, B.REQ_SEQ, A.DLP, C.USER_ID, C.USER_NAME, C.USER_RANK,
CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IO_TYPE,
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS,
A.APPROVE_KIND, B.APPROVE_FLAG, A.TITLE,  TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUEST_TIME,
TO_CHAR(TO_TIMESTAMP(B.APPR_RES_TIME,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'C' AS APPROVETABLEPOS, 
GETAPPROVEPOSSIBLE(B.APPROVE_USER_SEQ, A.TRANS_SEQ) AS APPROVEPOSSIBLE, B.APPROVESTATE,
CASE WHEN ((SELECT COUNT(*) FROM TBL_FORWARD_INFO WHERE TRANS_SEQ = A.TRANS_SEQ) + (SELECT COUNT(*) FROM TBL_FORWARD_INFO_HIS WHERE TRANS_SEQ = A.TRANS_SEQ)) > 0  THEN '1' 
ELSE '0' END AS FORWARD_TYPE,
{srcDestString},
COALESCE((SELECT APPROVE_REAL_NAME || ' ' || APPROVE_REAL_RANK FROM TBL_APPROVE_REAL_HIS WHERE REQ_SEQ = B.REQ_SEQ), E.USER_NAME || ' ' || E.USER_RANK) AS APPROVE_REAL_USER_NAME
FROM TBL_TRANSFER_REQ_INFO A, 
( 
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '2' AS 
	APPROVESTATE FROM TBL_APPROVE_HIS 
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	{apprString}
	AND APPROVE_USER_SEQ <> USER_SEQ
	UNION ALL 
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_INFO
	WHERE {securityOrderNum}                                                
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	AND APPROVE_USER_SEQ <> USER_SEQ
	UNION ALL
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_AFTER
	WHERE {securityOrderNum}
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	AND APPROVE_USER_SEQ <> USER_SEQ ) B,
	{srcDestTableString}
	WHERE A.TRANS_SEQ = B.TRANS_SEQ AND A.USER_SEQ = C.USER_SEQ AND B.APPROVE_USER_SEQ = E.USER_SEQ AND A.USER_SEQ <> B.APPROVE_USER_SEQ ";
			if (tParam.NetWorkType == EnumNetWorkType.Multiple)
				sql += "AND A.TRANS_SEQ = D.TRANS_SEQ";
			if (!tParam.IsSecurity)
			{
				if (bNoClipboard)
					sql += "AND A.DATA_TYPE=0 ";
				else
					sql += GetClipDataSearch(strArrClipDataType);
			}
			sql += $@"
	{requestString}
	AND A.TRANS_FLAG <> '6'
	AND  ( SUBSTRING(A.SYSTEM_ID, 1, 1) ='I' OR  SUBSTRING(A.SYSTEM_ID, 1, 1) ='E' )
UNION ALL 
SELECT A.TRANS_SEQ, B.REQ_SEQ, A.DLP, C.USER_ID, C.USER_NAME, C.USER_RANK, 
CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IO_TYPE, 
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS, A.APPROVE_KIND,
B.APPROVE_FLAG, A.TITLE,  
TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUEST_TIME,
TO_CHAR(TO_TIMESTAMP(B.APPR_RES_TIME,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'H' AS APPROVETABLEPOS, GETAPPROVEPOSSIBLE(B.APPROVE_USER_SEQ, A.TRANS_SEQ) AS APPROVEPOSSIBLE, B.APPROVESTATE,
CASE WHEN ((SELECT COUNT(*) FROM TBL_FORWARD_INFO WHERE TRANS_SEQ = A.TRANS_SEQ) + (SELECT COUNT(*) FROM TBL_FORWARD_INFO_HIS WHERE TRANS_SEQ = A.TRANS_SEQ)) > 0  THEN '1' 
ELSE '0' END AS FORWARD_TYPE, 
{srcDestString},
COALESCE((SELECT APPROVE_REAL_NAME || ' ' || APPROVE_REAL_RANK FROM TBL_APPROVE_REAL_HIS WHERE REQ_SEQ = B.REQ_SEQ), E.USER_NAME || ' ' || E.USER_RANK) AS APPROVE_REAL_USER_NAME 
FROM TBL_TRANSFER_REQ_HIS A, ( 
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '2' AS APPROVESTATE 
	FROM TBL_APPROVE_HIS 
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	{apprString}
	AND APPROVE_USER_SEQ <> USER_SEQ 
	UNION ALL
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_INFO
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@" )
	AND APPROVE_USER_SEQ <> USER_SEQ
	UNION ALL
	SELECT REQ_SEQ, TRANS_SEQ, APPROVE_USER_SEQ, APPROVE_FLAG, APPR_RES_TIME, '1' AS APPROVESTATE 
	FROM TBL_APPROVE_AFTER
	WHERE {securityOrderNum}                                               
	AND APPROVE_USER_SEQ IN (
		SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}'";
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sql += sfmString;
			}
			sql += $@")
	AND APPROVE_USER_SEQ <> USER_SEQ) B, 
	{srcDestTableString}
	WHERE A.TRANS_SEQ = B.TRANS_SEQ AND A.USER_SEQ = C.USER_SEQ AND B.APPROVE_USER_SEQ = E.USER_SEQ AND A.USER_SEQ <> B.APPROVE_USER_SEQ 
";
			if (tParam.NetWorkType == EnumNetWorkType.Multiple)
				sql += " AND A.TRANS_SEQ = D.TRANS_SEQ";

			if (!tParam.IsSecurity)
			{
				if (bNoClipboard)
					sql += " AND A.DATA_TYPE=0";
				else
					sql += GetClipDataSearch(strArrClipDataType);
			}

			sql += $@"
	{requestString}			
	AND  ( SUBSTRING(A.SYSTEM_ID, 1, 1) ='I' OR  SUBSTRING(A.SYSTEM_ID, 1, 1) ='E' )
) AS X
WHERE 1=1";
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sql += $"  AND IO_TYPE = '{tParam.TransKind}'";
			}
			//*****************************************************************************
			//여기 맞춰야함 WEBLINK기준이라 넷링크랑 다를수 있음 2020/07/27 YKH
			//*****************************************************************************
			if (!tParam.ApprStatus.Equals("5"))
			{
				if (tParam.ApprStatus.Equals("1"))
				{
					sql += $" AND APPROVE_FLAG = '1' AND APPROVESTATE != '2'";
				}
				else if (tParam.ApprStatus.Equals("4"))
				{       //승인불필요
					sql += $" AND APPROVE_FLAG = '1' AND APPROVESTATE = '2'";
				}
				else
				{
					if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
						sql += $"  AND APPROVE_FLAG = '{tParam.ApprStatus}'";
				}
			}
			if (tParam.ApprKind != null && tParam.ApprKind.Length > 0)
			{
				sql += $"  AND APPROVE_KIND = '{tParam.ApprKind}'";
			}

			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sql += $"  AND UPPER(TITLE) LIKE UPPER('%' || '{tParam.Title}' || '%')";
			}
			if (tParam.ReqUserName != null && tParam.ReqUserName.Length > 0)
			{
				sql += $" AND USER_NAME LIKE '%' || '{tParam.ReqUserName}' || '%'";
			}
			// 변경 : 송신기준
			if (tParam.Src_system_id != null && tParam.Src_system_id.Length > 0)
			{
				sql += $" AND (SUBSTRING(SRC_SYSTEM_ID,1,1) = '{tParam.Src_system_id.Substring(0, 1)}')";
			}
			if (tParam.Dest_system_id != null && tParam.Dest_system_id.Length > 0)
			{
				sql += $" AND (SUBSTRING(DEST_SYSTEM_ID,1,2) = '{tParam.Dest_system_id.Substring(0, 2)}')";
			}
			return sql;
		}
    }
}
