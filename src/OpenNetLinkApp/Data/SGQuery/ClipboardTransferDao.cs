using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class ClipboardTransferDao
    {
		public string List(TransferParam tParam)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("SELECT * FROM ( ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '1' as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, coalesce(b.download_count, 0) as downCount, ");
			sb.Append("  get_forward_flag(a.trans_seq) as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType, a.data_type AS dataType ");
			sb.Append("FROM tbl_transfer_req_info a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
			sb.Append("  get_forward_flag(a.trans_seq) as DataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType, a.data_type AS dataType ");
			sb.Append("FROM tbl_transfer_req_his a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
			sb.Append("  '2' as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '1' as receiveType, a.data_type AS dataType ");
			sb.Append("FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
			sb.Append("  '2' as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '1' as receiveType, a.data_type AS dataType  ");
			sb.Append(" FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info_his b ON (a.trans_seq = b.trans_seq) ");
			sb.Append(" WHERE a.data_type != 0 AND b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
				sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if( tParam.DataType != 0)
            {
				if( tParam.DataType == 3)
					sb.Append("  AND (dataType = 1 OR  + dataType = 2) ");
				else
					sb.Append("  AND dataType = " + tParam.DataType);
			}
			sb.Append(" ORDER BY requestTime desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);

			return sb.ToString();
		}

		public string TotalCount(TransferParam tParam)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("SELECT COUNT(*) FROM ( ");
			sb.Append("SELECT a.trans_seq as transSeq, a.dlp as dlp, CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as ioType, ");
			sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
			sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("  '1' as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, coalesce(b.download_count, 0) as downCount, ");
			sb.Append("  get_forward_flag(a.trans_seq) as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType, a.data_type AS dataType  ");
			sb.Append("FROM tbl_transfer_req_info a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
			sb.Append("  get_forward_flag(a.trans_seq) as DataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType, a.data_type AS dataType ");
			sb.Append("FROM tbl_transfer_req_his a ");
			sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
			sb.Append("  '2' as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '1' as receiveType, a.data_type AS dataType ");
			sb.Append("FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
			sb.Append("  '2' as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '1' as receiveType, a.data_type AS dataType ");
			sb.Append("FROM view_transfer_all a ");
			sb.Append("  INNER JOIN tbl_forward_info_his b ON (a.trans_seq = b.trans_seq) ");
			sb.Append("WHERE a.data_type != 0 AND b.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

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
				sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.DataType != 0)
			{
				if (tParam.DataType == 3)
					sb.Append("  AND (dataType = 1 OR  + dataType = 2) ");
				else
					sb.Append("  AND dataType = " + tParam.DataType);
			}
			return sb.ToString();
		}
	}
}
