using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    /// <summary>
    /// 보안결재용 클래스, ApproveDao 로 대체.... 사용안함.....
    /// </summary>
    class SecurityApproveDao
    {
		public string List(ApproveParam tParam)
		{
			string mainCdSecValue = tParam.SystemId.Substring(1, 1);

			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT * FROM ( ");
			sb.Append("  SELECT a.trans_seq, b.req_seq, a.dlp, c.user_id, c.user_name, c.user_rank, ");
			sb.Append("    CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as io_type, ");
			sb.Append("    func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transstatus,  ");
			sb.Append("    a.approve_kind,   ");
			sb.Append("    b.approve_flag, a.title,  to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as request_time, ");
			sb.Append("    to_char(to_timestamp(b.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'C' as ApproveTablePos, GetApprovePossible(b.approve_user_seq, a.trans_seq) as ApprovePossible, b.ApproveState, ");
			sb.Append("    CASE WHEN ((select count(*) from tbl_forward_info where trans_seq = a.trans_seq) + ");
			sb.Append("     (select count(*) from tbl_forward_info_his where trans_seq = a.trans_seq)) > 0  THEN '1' ELSE '0' END as forward_type ");
			sb.Append("  FROM tbl_transfer_req_info a, ( ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '2' as ApproveState FROM tbl_approve_his ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                                ");
			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "' AND appr_req_time <= '" + tParam.SearchToDay + "'");

			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_info ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                             ");

			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_after ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                               ");
			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("  ) b, tbl_user_info c ");
			sb.Append("  WHERE a.trans_seq = b.trans_seq AND a.user_seq = c.user_seq AND a.user_seq <> b.approve_user_seq ");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");

			sb.Append("    AND a.trans_flag <> '6' ");
			// sb.Append("   AND  ( Substring(a.system_id, 1, 2) ='I" + mainCdSecValue + "' OR  Substring(a.system_id, 1, 2) ='E" + mainCdSecValue + "' ) ");
			sb.Append("   AND  ( Substring(a.system_id, 1, 1) ='I' OR  Substring(a.system_id, 1, 1) ='E' ) ");
			sb.Append("  UNION ALL ");
			sb.Append("  SELECT a.trans_seq, b.req_seq, a.dlp, c.user_id, c.user_name, c.user_rank, ");
			sb.Append("    CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as io_type, ");
			sb.Append("    func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transstatus,  ");
			sb.Append("    a.approve_kind,   ");
			sb.Append("    b.approve_flag, a.title,  to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as request_time, ");
			sb.Append("    to_char(to_timestamp(b.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'H' as ApproveTablePos, GetApprovePossible(b.approve_user_seq, a.trans_seq) as ApprovePossible, b.ApproveState, ");
			sb.Append("    CASE WHEN ((select count(*) from tbl_forward_info where trans_seq = a.trans_seq) + ");
			sb.Append("     (select count(*) from tbl_forward_info_his where trans_seq = a.trans_seq)) > 0  THEN '1' ELSE '0' END as forward_type ");
			sb.Append("  FROM tbl_transfer_req_his a, ( ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '2' as ApproveState FROM tbl_approve_his ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                                ");
			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "' AND appr_req_time <= '" + tParam.SearchToDay + "'");

			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_info ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                            ");

			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_after ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                               ");

			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("  ) b, tbl_user_info c ");
			sb.Append("  WHERE a.trans_seq = b.trans_seq AND a.user_seq = c.user_seq  AND a.user_seq <> b.approve_user_seq ");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");

			// sb.Append("   AND  ( Substring(a.system_id, 1, 2) ='I" + mainCdSecValue + "' OR  Substring(a.system_id, 1, 2) ='E" + mainCdSecValue + "' ) ");
			sb.Append("   AND  ( Substring(a.system_id, 1, 1) ='I' OR  Substring(a.system_id, 1, 1) ='E' ) ");
			sb.Append(") as x ");
			sb.Append("where 1=1");
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append("  AND io_type = '" + tParam.TransKind + "'");
			}
			//*****************************************************************************
			//여기 맞춰야함 WEBLINK기준이라 넷링크랑 다를수 있음 2020/07/27 YKH
			//*****************************************************************************
			if (!tParam.ApprStatus.Equals("5"))
			{
				if (tParam.ApprStatus.Equals("1"))
				{
					sb.Append(" AND approve_flag = '1' and approveState != '2'");
				}
				else if (tParam.ApprStatus.Equals("4"))
				{       //승인불필요
					sb.Append(" AND approve_flag = '1' and approveState = '2'");
				}
				else
				{
					if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
						sb.Append("  AND approve_flag = '" + tParam.ApprStatus + "'");
				}
			}
			if (tParam.ApprKind != null && tParam.ApprKind.Length > 0)
			{
				sb.Append("  AND approve_kind = '" + tParam.ApprKind + "'");
			}

			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.ReqUserName != null && tParam.ReqUserName.Length > 0)
			{
				sb.Append(" AND user_name like '%' || '" + tParam.ReqUserName + "' || '%'");
			}
			sb.Append(" ORDER BY request_time desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);

			return sb.ToString();

		}
		public string TotalCount(ApproveParam tParam)
		{
			StringBuilder sb = new StringBuilder();
			string mainCdSecValue = tParam.SystemId.Substring(1, 1);

			sb.Append("SELECT COUNT(*) FROM ( ");
			sb.Append("  SELECT a.trans_seq, b.req_seq, a.dlp, c.user_id, c.user_name, c.user_rank, ");
			sb.Append("    CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as io_type, ");
			sb.Append("    func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transstatus,  ");
			sb.Append("    a.approve_kind,   ");
			sb.Append("    b.approve_flag, a.title,  to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as request_time, ");
			sb.Append("    to_char(to_timestamp(b.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'C' as ApproveTablePos, GetApprovePossible(b.approve_user_seq, a.trans_seq) as ApprovePossible, b.ApproveState, ");
			sb.Append("    CASE WHEN ((select count(*) from tbl_forward_info where trans_seq = a.trans_seq) + ");
			sb.Append("     (select count(*) from tbl_forward_info_his where trans_seq = a.trans_seq)) > 0  THEN '1' ELSE '0' END as forward_type ");
			sb.Append("  FROM tbl_transfer_req_info a, ( ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '2' as ApproveState FROM tbl_approve_his ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                         ");
			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "' AND appr_req_time <= '" + tParam.SearchToDay + "'");

			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_info ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                               ");

			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_after ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                            ");
			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("  ) b, tbl_user_info c ");
			sb.Append("  WHERE a.trans_seq = b.trans_seq AND a.user_seq = c.user_seq AND a.user_seq <> b.approve_user_seq ");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");

			sb.Append("    AND a.trans_flag <> '6' ");
			// sb.Append("   AND  ( Substring(a.system_id, 1, 2) ='I" + mainCdSecValue + "' OR  Substring(a.system_id, 1, 2) ='E" + mainCdSecValue + "' ) ");
			sb.Append("   AND  ( Substring(a.system_id, 1, 1) ='I' OR  Substring(a.system_id, 1, 1) ='E' ) ");
			sb.Append("  UNION ALL ");
			sb.Append("  SELECT a.trans_seq, b.req_seq, a.dlp, c.user_id, c.user_name, c.user_rank, ");
			sb.Append("    CASE WHEN substring(a.system_id, 1, 1)='I' THEN '1' ELSE '2' END as io_type, ");
			sb.Append("    func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transstatus,  ");
			sb.Append("    a.approve_kind,   ");
			sb.Append("    b.approve_flag, a.title,  to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as request_time, ");
			sb.Append("    to_char(to_timestamp(b.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS'), 'H' as ApproveTablePos, GetApprovePossible(b.approve_user_seq, a.trans_seq) as ApprovePossible, b.ApproveState, ");
			sb.Append("    CASE WHEN ((select count(*) from tbl_forward_info where trans_seq = a.trans_seq) + ");
			sb.Append("     (select count(*) from tbl_forward_info_his where trans_seq = a.trans_seq)) > 0  THEN '1' ELSE '0' END as forward_type ");
			sb.Append("  FROM tbl_transfer_req_his a, ( ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '2' as ApproveState FROM tbl_approve_his ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                               ");
			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND appr_req_time >= '" + tParam.SearchFromDay + "' AND appr_req_time <= '" + tParam.SearchToDay + "'");

			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_info ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                             ");

			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("    UNION ALL ");
			sb.Append("    SELECT req_seq, trans_seq, approve_user_seq, approve_flag, appr_res_time, '1' as ApproveState FROM tbl_approve_after ");
			sb.Append("       WHERE (approve_order < 101 OR approve_order > 199)                                              ");

			sb.Append("      and approve_user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("        AND approve_user_seq <> user_seq ");
			sb.Append("  ) b, tbl_user_info c ");
			sb.Append("  WHERE a.trans_seq = b.trans_seq AND a.user_seq = c.user_seq  AND a.user_seq <> b.approve_user_seq ");
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time <= '" + tParam.SearchToDay + "'");
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
				sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "' AND a.request_time <= '" + tParam.SearchToDay + "'");

			// sb.Append("   AND  ( Substring(a.system_id, 1, 2) ='I" + mainCdSecValue + "' OR  Substring(a.system_id, 1, 2) ='E" + mainCdSecValue + "' ) ");
			sb.Append("   AND  ( Substring(a.system_id, 1, 1) ='I' OR  Substring(a.system_id, 1, 1) ='E' ) ");
			sb.Append(") as x ");
			sb.Append("where 1=1");
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append("  AND io_type = '" + tParam.TransKind + "'");
			}
			//*****************************************************************************
			//여기 맞춰야함 WEBLINK기준이라 넷링크랑 다를수 있음 2020/07/27 YKH
			//*****************************************************************************
			if (!tParam.ApprStatus.Equals("5"))
			{
				if (tParam.ApprStatus.Equals("1"))
				{
					sb.Append(" AND approve_flag = '1' and approveState != '2'");
				}
				else if (tParam.ApprStatus.Equals("4"))
				{       //승인불필요
					sb.Append(" AND approve_flag = '1' and approveState = '2'");
				}
				else
				{
					if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
						sb.Append("  AND approve_flag = '" + tParam.ApprStatus + "'");
				}
			}
			if (tParam.ApprKind != null && tParam.ApprKind.Length > 0)
			{
				sb.Append("  AND approve_kind = '" + tParam.ApprKind + "'");
			}

			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.ReqUserName != null && tParam.ReqUserName.Length > 0)
			{
				sb.Append(" AND user_name like '%' || '" + tParam.ReqUserName + "' || '%'");
			}

			return sb.ToString();
		}
	}
}
