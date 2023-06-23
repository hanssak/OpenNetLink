using System;
using System.Collections.Generic;
using System.Text;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGQuery
{
    class TransferDao
    {
		/// <summary>
		/// 클립보드 데이터 타입에 따른 조건절
		/// </summary>
		/// <param name="strArrClipDataType">클립보드 데이터타입</param>
		/// <returns>조건절 쿼리문</returns>
		public string GetClipDataSearch(string[] strArrClipDataType)
		{
			StringBuilder sb = new StringBuilder();

			//sb.Append("  AND a.data_type IN ('1', '2')");
			if (strArrClipDataType == null || strArrClipDataType.Length < 1)
				sb.Append("  AND a.data_type!='0'");
			else
			{
				sb.Append("  AND a.data_type IN (");
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

		/// <summary>
		/// 쿼리 결과 가져오기
		/// </summary>
		/// <param name="tParam">쿼리 매개변수</param>
		/// <param name="bNoClipboard">(true : 일반파일  false : 클립보드)</param>
		/// <param name="strArrClipDataType">클립보드 타입</param>
		/// <returns>쿼리문</returns>
		public string List(TransferParam tParam, bool bNoClipboard, string[] strArrClipDataType = null)
		{
			//단일망, 다중망일 경우 보낸 곳과 받는 곳의 정보를 받아 결재 리스트를 보여준다.
			string srcDestString = "";
			string srcDestString2 = "";
			string srcDestString3 = "";
			string srcDestString4 = "";

			if (tParam.NetWorkType == EnumNetWorkType.Single)
			{
				srcDestString = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED,
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE 
FROM TBL_TRANSFER_REQ_INFO A 
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
				srcDestString2 = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED,
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE 
FROM TBL_TRANSFER_REQ_HIS A 
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
				srcDestString3 = @"
'2' AS DATAFORWARDED, 
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
				srcDestString4 = @"
'2' AS DATAFORWARDED, 
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO_HIS B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
			}
			else
			{
				srcDestString = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM TBL_TRANSFER_REQ_INFO A
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
				srcDestString2 = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM TBL_TRANSFER_REQ_HIS A
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
				srcDestString3 = @"
'2' AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
				srcDestString4 = @"
'2' AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO_HIS B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
			}

			string dataTypeCheckString = String.Empty;
			if (bNoClipboard)
				dataTypeCheckString = "AND A.DATA_TYPE=0";
			else
				dataTypeCheckString = GetClipDataSearch(strArrClipDataType);

			string dateTimeCheckString = String.Empty;
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}'";
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}' AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}

			StringBuilder sb = new StringBuilder();
			string sql = $@"
SELECT * FROM ( 
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE,
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS, A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME,
'1' AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP(COALESCE(B.EXPIRED_DATE, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE, COALESCE(B.DOWNLOAD_COUNT, 0) AS DOWNCOUNT,
{srcDestString}
WHERE A.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{ tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
UNION ALL
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE, 
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS, A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME, 
COALESCE(B.DOWNLOAD_ALIVE, '0') AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP(COALESCE(B.EXPIRED_DATE, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE,
COALESCE(B.DOWNLOAD_COUNT, 1) AS DOWNCOUNT,
{srcDestString2}
WHERE A.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
UNION ALL
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE,
FUNC_TRANSSTATUS_FWD(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG, '1', B.DOWNLOAD_COUNT, A.APPROVE_FLAG) AS TRANSSTATUS,
A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME,
'1' AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP('00000000','YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE, 0 AS DOWNCOUNT,
{srcDestString3}
WHERE B.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
UNION ALL
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP  AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE, 
FUNC_TRANSSTATUS_FWD(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG, '2', B.DOWNLOAD_COUNT, A.APPROVE_FLAG) AS TRANSSTATUS,
A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME,
'0' AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP('00000000','YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE, 1 AS DOWNCOUNT,
{srcDestString4}
WHERE B.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
) AS X 
WHERE 1=1
";
			if ( tParam.TransKind.Equals("1") || tParam.TransKind.Equals("2") )
			{
				sql += $@" 
AND ioType = '{tParam.TransKind}' ";
			}
			if (tParam.TransStatus.Equals("W"))
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sql += @" 
AND  APPROVEFLAG != '3'";
				}
			}
			if ( tParam.TransStatus.Equals("C") )
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sql += $@"
AND (TRANSSTATUS = '{tParam.TransStatus}' OR  APPROVEFLAG = '3' )";
				}
			}
			else
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sql += $@"
AND TRANSSTATUS = '{tParam.TransStatus}'";
				}
			}
			if (tParam.ApprStatus.Equals("4"))
			{
				if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sql += @"
AND  TRANSSTATUS = 'C'
AND  APPROVEFLAG != '2'
AND  APPROVEFLAG != '3'
";
				}
			}
			else
			{
				if(tParam.ApprStatus != null && tParam.ApprStatus == "1")
                {
					sql += $@"
AND APPROVEFLAG = '{tParam.ApprStatus}' AND TRANSSTATUS != 'C' ";
				}
				else if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sql += $@"
AND APPROVEFLAG = '{tParam.ApprStatus}'";
				}
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				//sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
				sql += $@"
AND UPPER(TITLE) LIKE UPPER('%' || '{tParam.Title}' || '%')";
			}
			if(tParam.NetWorkType == EnumNetWorkType.Multiple)
            {
				if (tParam.Dest_system_id != null && tParam.Dest_system_id.Length > 0)
				{
					sql += $@" 
AND SUBSTRING(DEST_SYSTEM_ID,1,2) = '{tParam.Dest_system_id.Substring(0, 2)}'";  // 목적망:자신선택때사용
				}
			}

			sql += $@" 
ORDER BY REQUESTTIME DESC
LIMIT {tParam.PageListCount} OFFSET ({tParam.ViewPageNo } -1) * {tParam.PageListCount}
";
			return sql;
        }

		/// <summary>
		/// 특정 TransSeq만 조회
		/// </summary>
		/// <param name="tParam">쿼리 매개변수</param>
		/// <param name="transSeq">TransSeq</param>
		/// <returns>쿼리문</returns>
		public string TransSeqList(TransferParam tParam, string transSeq)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("SELECT * FROM ( ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '1' as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, coalesce(b.download_count, 0) as downCount, ");
			sb.Append("  get_forward_flag(a.trans_seq) as dataForwarded,");
			sb.Append(" (SELECT aa.user_id || '|' || aa.user_rank  || '|' || bb.dept_name FROM tbl_user_info aa, tbl_dept_info bb WHERE aa.dept_seq = bb.dept_seq AND aa.user_seq = a.user_seq) as orgUserInfo,");
			sb.Append(" a.recv_pos as recvPos, '0' as receiveType, a.data_type as dataType ");
			sb.Append("FROM tbl_transfer_req_info a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");
			
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				//sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  coalesce(b.download_alive, '0') as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, ");
			sb.Append("  coalesce(b.download_count, 1) as downCount, ");
			sb.Append("  get_forward_flag(a.trans_seq) as DataForwarded, ");
			sb.Append(" (SELECT aa.user_id || '|' || aa.user_rank  || '|' || bb.dept_name FROM tbl_user_info aa, tbl_dept_info bb WHERE aa.dept_seq = bb.dept_seq AND aa.user_seq = a.user_seq) as orgUserInfo,");
			sb.Append(" a.recv_pos as recvPos, '0' as receiveType, a.data_type as dataType ");
			sb.Append("FROM tbl_transfer_req_his a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus_fwd(a.trans_flag, a.recv_flag, a.pctrans_flag, '1', b.download_count, a.approve_flag) as transStatus, ");
			sb.Append("  a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '1' as downPossible, To_char(To_timestamp('00000000','YYYYMMDD'),'YYYY-MM-DD') as ExpiredDate, 0 as downCount, ");
			sb.Append("  '2' as dataForwarded, ");
			sb.Append(" (SELECT aa.user_id || '|' || aa.user_rank  || '|' || bb.dept_name FROM tbl_user_info aa, tbl_dept_info bb WHERE aa.dept_seq = bb.dept_seq AND aa.user_seq = a.user_seq) as orgUserInfo,");
			sb.Append(" a.recv_pos as recvPos, '1' as receiveType, a.data_type as dataType ");
			sb.Append("FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT a.trans_seq as transSeq, a.dlp  as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus_fwd(a.trans_flag, a.recv_flag, a.pctrans_flag, '2', b.download_count, a.approve_flag) as transstatus, ");
			sb.Append("  a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '0' as downPossible, To_char(To_timestamp('00000000','YYYYMMDD'),'YYYY-MM-DD') as ExpiredDate, 1 as downCount, ");
			sb.Append("  '2' as dataForwarded, ");
			sb.Append(" (SELECT aa.user_id || '|' || aa.user_rank  || '|' || bb.dept_name FROM tbl_user_info aa, tbl_dept_info bb WHERE aa.dept_seq = bb.dept_seq AND aa.user_seq = a.user_seq) as orgUserInfo,");
			sb.Append(" a.recv_pos as recvPos, '1' as receiveType, a.data_type as dataType ");
			sb.Append(" FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info_his b ON (a.trans_seq = b.trans_seq) ");
			sb.Append(" WHERE b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");
			}
			sb.Append(") as x ");
			sb.Append("where 1=1");
			if (tParam.TransKind.Equals("1") || tParam.TransKind.Equals("2"))
			{
				sb.Append("  AND ioType = '" + tParam.TransKind + "'");
			}
			if (tParam.TransStatus.Equals("W"))
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sb.Append("  AND  approveFlag != '3' ");
				}
			}
			if (tParam.TransStatus.Equals("C"))
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sb.Append("  AND (transStatus = '" + tParam.TransStatus + "' OR  approveFlag = '3' )");
				}
			}
			else
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sb.Append("  AND transStatus = '" + tParam.TransStatus + "'");
				}
			}
			if (tParam.ApprStatus.Equals("4"))
			{
				if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sb.Append("  AND  transStatus = 'C' ");
					sb.Append("  AND  approveFlag != '2' ");
					sb.Append("  AND  approveFlag != '3' ");
				}
			}
			else
			{
				if (tParam.ApprStatus != null && tParam.ApprStatus == "1")
				{
					sb.Append("  AND approveFlag = '" + tParam.ApprStatus + "' AND transStatus != 'C' ");
				}
				else if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sb.Append("  AND approveFlag = '" + tParam.ApprStatus + "'");
				}
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				//sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
				sb.Append("  AND UPPER(title) LIKE UPPER('%' || '" + tParam.Title + "' || '%')");
			}
			if (string.IsNullOrEmpty(transSeq) == false)
			{
				sb.Append($"  AND transSeq = '{transSeq}'");
			}
			sb.Append(" ORDER BY requestTime desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);

			return sb.ToString();
		}

		/// <summary>
		/// 쿼리 결과 행 수 가져오기
		/// </summary>
		/// <param name="tParam">쿼리 매개변수</param>
		/// <param name="bNoClipboard">(true : 일반파일  false : 클립보드)</param>
		/// <param name="strArrClipDataType">클립보드</param>
		/// <returns>쿼리문</returns>
		public string TotalCount(TransferParam tParam, bool bNoClipboard, string[] strArrClipDataType = null)
		{
			//단일망, 다중망일 경우 보낸 곳과 받는 곳의 정보를 받아 결재 리스트를 보여준다.
			string srcDestString = "";
			string srcDestString2 = "";
			string srcDestString3 = "";
			string srcDestString4 = "";

			if (tParam.NetWorkType == EnumNetWorkType.Single)
			{
				srcDestString = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED,
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE 
FROM TBL_TRANSFER_REQ_INFO A 
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
				srcDestString2 = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED,
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE 
FROM TBL_TRANSFER_REQ_HIS A 
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
				srcDestString3 = @"
'2' AS DATAFORWARDED, 
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
				srcDestString4 = @"
'2' AS DATAFORWARDED, 
(SELECT AA.USER_ID || '|' || AA.USER_RANK  || '|' || BB.DEPT_NAME FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB WHERE AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USER_SEQ = A.USER_SEQ) AS ORGUSERINFO,
A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE, A.DATA_TYPE AS DATATYPE
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO_HIS B ON (A.TRANS_SEQ = B.TRANS_SEQ) 
";
			}
			else
			{
				srcDestString = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM TBL_TRANSFER_REQ_INFO A
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
				srcDestString2 = @"
GET_FORWARD_FLAG(A.TRANS_SEQ) AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '0' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM TBL_TRANSFER_REQ_HIS A
LEFT OUTER JOIN VIEW_BACKUP_PERIOD B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
				srcDestString3 = @"
'2' AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
				srcDestString4 = @"
'2' AS DATAFORWARDED, GET_USER_INFO(A.USER_SEQ) AS ORGUSERINFO, A.RECV_POS AS RECVPOS, '1' AS RECEIVETYPE
,C.SRC_SYSTEM_ID, C.DEST_SYSTEM_ID
FROM VIEW_TRANSFER_ALL A
INNER JOIN TBL_FORWARD_INFO_HIS B ON (A.TRANS_SEQ = B.TRANS_SEQ)
LEFT OUTER JOIN TBL_TRANSFER_REQ_SUB_HIS C ON (A.TRANS_SEQ = C.TRANS_SEQ)
";
			}

			string dataTypeCheckString = String.Empty;
			if (bNoClipboard)
				dataTypeCheckString = "AND A.DATA_TYPE=0";
			else
				dataTypeCheckString = GetClipDataSearch(strArrClipDataType);

			string dateTimeCheckString = String.Empty;
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}'";
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				dateTimeCheckString = $"AND A.REQUEST_TIME >= '{tParam.SearchFromDay}' AND A.REQUEST_TIME <= '{tParam.SearchToDay}'";
			}

			string sql = $@"
SELECT COUNT(*) FROM ( 
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE,
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS, A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME,
'1' AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP(COALESCE(B.EXPIRED_DATE, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE, COALESCE(B.DOWNLOAD_COUNT, 0) AS DOWNCOUNT,
{srcDestString}
WHERE A.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{ tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
UNION ALL
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE, 
FUNC_TRANSSTATUS(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSSTATUS, A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME, 
COALESCE(B.DOWNLOAD_ALIVE, '0') AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP(COALESCE(B.EXPIRED_DATE, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE,
COALESCE(B.DOWNLOAD_COUNT, 1) AS DOWNCOUNT,
{srcDestString2}
WHERE A.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
UNION ALL
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE,
FUNC_TRANSSTATUS_FWD(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG, '1', B.DOWNLOAD_COUNT, A.APPROVE_FLAG) AS TRANSSTATUS,
A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME,
'1' AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP('00000000','YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE, 0 AS DOWNCOUNT,
{srcDestString3}
WHERE B.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
UNION ALL
SELECT A.TRANS_SEQ AS TRANSSEQ, A.DLP  AS DLP, CASE WHEN SUBSTRING(A.SYSTEM_ID, 1, 1)='I' THEN '1' ELSE '2' END AS IOTYPE, 
FUNC_TRANSSTATUS_FWD(A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG, '2', B.DOWNLOAD_COUNT, A.APPROVE_FLAG) AS TRANSSTATUS,
A.APPROVE_KIND AS APPROVEKIND, A.APPROVE_FLAG AS APPROVEFLAG, A.TITLE AS TITLE, A.FILE_SIZE AS FILESIZE,
FUNC_TRANSFILEPOS(A.SYSTEM_ID, A.TRANS_FLAG, A.RECV_FLAG, A.PCTRANS_FLAG) AS TRANSPOS, TO_CHAR(TO_TIMESTAMP(SUBSTRING(A.REQUEST_TIME, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') AS REQUESTTIME,
'0' AS DOWNPOSSIBLE, TO_CHAR(TO_TIMESTAMP('00000000','YYYYMMDD'),'YYYY-MM-DD') AS EXPIREDDATE, 1 AS DOWNCOUNT,
{srcDestString4}
WHERE B.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE USER_ID = '{tParam.UserID}')
{dataTypeCheckString}
{dateTimeCheckString}
) AS X 
WHERE 1=1
";
			if (tParam.TransKind.Equals("1") || tParam.TransKind.Equals("2"))
			{
				sql += $@" 
AND ioType = '{tParam.TransKind}' ";
			}
			if (tParam.TransStatus.Equals("W"))
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sql += @" 
AND  APPROVEFLAG != '3'";
				}
			}
			if (tParam.TransStatus.Equals("C"))
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sql += $@"
AND (TRANSSTATUS = '{tParam.TransStatus}' OR  APPROVEFLAG = '3' )";
				}
			}
			else
			{
				if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
				{
					sql += $@"
AND TRANSSTATUS = '{tParam.TransStatus}'";
				}
			}
			if (tParam.ApprStatus.Equals("4"))
			{
				if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sql += @"
AND  TRANSSTATUS = 'C'
AND  APPROVEFLAG != '2'
AND  APPROVEFLAG != '3'
";
				}
			}
			else
			{
				if (tParam.ApprStatus != null && tParam.ApprStatus == "1")
				{
					sql += $@"
AND APPROVEFLAG = '{tParam.ApprStatus}' AND TRANSSTATUS != 'C' ";
				}
				else if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sql += $@"
AND APPROVEFLAG = '{tParam.ApprStatus}'";
				}
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				//sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
				sql += $@"
AND UPPER(TITLE) LIKE UPPER('%' || '{tParam.Title}' || '%')";
			}
			if (tParam.NetWorkType == EnumNetWorkType.Multiple)
			{
				if (tParam.Dest_system_id != null && tParam.Dest_system_id.Length > 0)
				{
					sql += $@" 
AND SUBSTRING(DEST_SYSTEM_ID,1,2) = '{tParam.Dest_system_id.Substring(0, 2)}'";  // 목적망:자신선택때사용
				}
			}
			return sql;
		}
	}
}
