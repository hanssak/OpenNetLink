using System;
using System.Collections.Generic;
using System.Text;

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
		public string List(TransferParam tParam, bool bNoClipboard, string[] strArrClipDataType=null)
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
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));

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
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));

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
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));

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
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));

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
			if ( tParam.TransKind.Equals("1") || tParam.TransKind.Equals("2") )
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
			if ( tParam.TransStatus.Equals("C") )
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
				if(tParam.ApprStatus != null && tParam.ApprStatus == "1")
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
			sb.Append(" ORDER BY requestTime desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);

			return sb.ToString();
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
			StringBuilder sb = new StringBuilder();

			sb.Append("SELECT COUNT(*) FROM ( ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '1' as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, coalesce(b.download_count, 0) as downCount, ");
			sb.Append("  get_forward_flag(a.trans_seq) as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType ");
			sb.Append("FROM tbl_transfer_req_info a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));

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
			sb.Append("UNION ALL ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  coalesce(b.download_alive, '0') as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, ");
			sb.Append("  coalesce(b.download_count, 1) as downCount, ");
			sb.Append("  get_forward_flag(a.trans_seq) as DataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType ");
			sb.Append("FROM tbl_transfer_req_his a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));


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
			sb.Append("UNION ALL ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus_fwd(a.trans_flag, a.recv_flag, a.pctrans_flag, '1', b.download_count, a.approve_flag) as transStatus, ");
			sb.Append("  a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '1' as downPossible, To_char(To_timestamp('00000000','YYYYMMDD'),'YYYY-MM-DD') as ExpiredDate, 0 as downCount, ");
			sb.Append("  '2' as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '1' as receiveType ");
			sb.Append("FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));


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
			sb.Append("UNION ALL ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp  as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus_fwd(a.trans_flag, a.recv_flag, a.pctrans_flag, '2', b.download_count, a.approve_flag) as transstatus, ");
			sb.Append("  a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '0' as downPossible, To_char(To_timestamp('00000000','YYYYMMDD'),'YYYY-MM-DD') as ExpiredDate, 1 as downCount, ");
			sb.Append("  '2' as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '1' as receiveType ");
			sb.Append("FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info_his b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");
			if (bNoClipboard)
				sb.Append("  AND a.data_type=0");
			else
				sb.Append(GetClipDataSearch(strArrClipDataType));


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
				if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
				{
					sb.Append("  AND approveFlag = '" + tParam.ApprStatus + "'");
				}
			}
			if (tParam.ApprStatus != null && tParam.ApprStatus == "1")
			{
				sb.Append("  AND approveFlag = '" + tParam.ApprStatus + "' AND transStatus != 'C' ");
			}


			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				//sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
				sb.Append("  AND UPPER(title) LIKE UPPER('%' || '" + tParam.Title + "' || '%')");
			}
			return sb.ToString();
		}
	}
}
