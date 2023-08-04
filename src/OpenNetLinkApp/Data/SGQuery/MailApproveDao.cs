using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class MailApproveDao
    {
		public string MailDetail(string seq)
		{
			string sql = String.Empty;
			sql = "SELECT func_email_detail(" + seq + ")";
			return sql;
		}

		public string TotalCount(MailApproveParam tParam)
        {
            string mainCdSecValue = tParam.SystemId;
            StringBuilder sb = new StringBuilder();
			sb.Append(" SELECT COUNT(*) FROM ( ");
			sb.Append("  SELECT apHis.email_seq as emailSeq,           ");
			sb.Append("			apHis.approve_user_seq as approveUserSeq,       ");
			sb.Append("			aphis.req_seq as reqSeq, ");
			sb.Append("			apHis.approve_kind as ApproveKind,                       ");
			sb.Append("			CASE WHEN SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) = 'I' THEN  'S' ELSE 'R' END as sr_type , ");
			sb.Append("			COALESCE(tranHis.dlp_flag,tranInfo.dlp_flag) as dlpFalg,  ");
			sb.Append("			CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn, ");
			sb.Append("			COALESCE(tranHis.trans_flag,tranInfo.trans_flag) as transStat, ");
			sb.Append("			apHis.approve_flag as approveFlag, ");
			sb.Append("			sendUser.user_name as sendUserNm, ");
			sb.Append("			rcvInfo.addr as revcUser,  ");//-- 0: 수신자, 1: 참조자. (수신자는 필수.)
			sb.Append("			(SELECT COUNT(*) FROM tbl_email_receiver WHERE apHis.email_seq = email_seq and recv_type ='0'), ");
			sb.Append("			COALESCE(tranHis.title,tranInfo.title) as title, ");
			sb.Append("			to_char(to_timestamp(apHis.appr_req_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("			CASE WHEN apHis.approve_flag = '1' THEN '-' ELSE to_char(to_timestamp(apHis.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') END as apprResTime, ");
			sb.Append("			'0' as ApprovePossible, ");
			sb.Append("			'2' as ApproveState ");
			sb.Append("	   FROM tbl_email_Approve_his apHis ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_info tranInfo ON apHis.email_seq = tranInfo.email_seq ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_his tranHis ON apHis.email_seq = tranHis.email_seq ");
			sb.Append("	   LEFT outer join tbl_user_info sendUser ON sendUser.user_seq = apHis.user_seq ");
			sb.Append("	   LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  apHis.email_seq  = fileInfo.email_seq ");//--email_seq에 index 존재함.
			sb.Append("	   LEFT OUTER JOIN tbl_email_receiver rcvInfo ON (aphis.email_seq = rcvinfo.email_seq and rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("		    , tbl_user_info usInfo ");
			sb.Append("	  WHERE apHis.approve_user_seq = usInfo.user_seq ");
			sb.Append("		and aphis.approve_user_seq in( SELECT user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("   and	aphis.approve_user_seq != aphis.user_seq ");        //결재 올린 자가 현재 결재라인에 포함되서 나옴, 결재 올린 자 제외
			sb.Append("		AND ( SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) ='" + mainCdSecValue + "' ) ");

			//기간 조회
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apHis.req_seq  >= '" + tParam.SearchFromDay + "0000' ");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apHis.req_seq <= '" + tParam.SearchToDay + "9999' ");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apHis.req_seq >= '" + tParam.SearchFromDay + "0000' AND apHis.req_seq <= '" + tParam.SearchToDay + "9999'");
			}
			if (tParam.ApproveKind != null && tParam.ApproveKind.Length > 0)
			{
				sb.Append("  AND apHis.approve_kind = '" + tParam.ApproveKind + "'");
			}
			if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
			{
				sb.Append("  AND COALESCE(tranhis.trans_flag,traninfo.trans_flag) = '" + tParam.TransStatus + "'");
			}
			if (tParam.Sender != null && tParam.Sender.Length > 0)
			{
				sb.Append("  AND usInfo.user_name LIKE '%' || '" + tParam.Sender + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranHis.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			sb.Append("	  UNION ALL ");
			sb.Append("	 SELECT apInfo.email_seq as emailSeq,     ");
			sb.Append("			apInfo.approve_user_seq as approveUserSeq, ");
			sb.Append("			apInfo.req_seq as reqSeq, ");
			sb.Append("			'0' ApproveKind,                  ");
			sb.Append("			CASE WHEN SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) = 'I' THEN  'S' ELSE 'R' END as sr_type , ");
			sb.Append("			COALESCE(tranHis.dlp_flag,tranInfo.dlp_flag) as dlpFalg, ");
			sb.Append("			CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn, ");
			sb.Append("			COALESCE(tranHis.trans_flag,tranInfo.trans_flag) as transStat, ");
			sb.Append("			apInfo.approve_flag as approveFlag, ");
			sb.Append("			sendUser.user_name as sendUserNm, ");
			sb.Append("			rcvInfo.addr as revcUser, "); //-- 0: 수신자, 1: 참조자. (수신자는 필수.) 
			sb.Append("			(SELECT COUNT(*) FROM tbl_email_receiver where apInfo.email_seq = email_seq and recv_type ='0'), ");
			sb.Append("			COALESCE(tranHis.title,tranInfo.title) as title, ");
			sb.Append("			to_char(to_timestamp(apInfo.appr_req_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("			CASE WHEN apInfo.approve_flag = '1' THEN '-' ELSE to_char(to_timestamp(apInfo.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') END as apprResTime, ");
			sb.Append("			(SELECT count(*)  FROM tbl_email_Approve_info ");
			sb.Append("		  	WHERE email_seq = apInfo.email_seq AND approve_user_seq = apInfo.approve_user_seq");
			sb.Append("		    AND approve_order IN (SELECT min(approve_order) FROM tbl_email_approve_info WHERE email_seq = apInfo.email_seq))as ApprovePossible, ");
			sb.Append("			'1' as ApproveState");
			sb.Append("	   FROM tbl_email_Approve_info apInfo ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_info tranInfo ON apInfo.email_seq = tranInfo.email_seq ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_his tranHis ON apInfo.email_seq = tranHis.email_seq ");
			sb.Append("	   LEFT outer join tbl_user_info sendUser ON sendUser.user_seq = apInfo.user_seq ");
			sb.Append("	   LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  apInfo.email_seq  = fileInfo.email_seq "); //--email_seq에 index 존재함.
			sb.Append("    LEFT OUTER JOIN  tbl_email_receiver rcvInfo ON (apinfo.email_seq = rcvinfo.email_seq and rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("		    , tbl_user_info usInfo ");
			sb.Append("	  WHERE apInfo.approve_user_seq = usInfo.user_seq ");
			sb.Append("		and apInfo.approve_user_seq in( SELECT user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("		AND ( SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) ='" + mainCdSecValue + "' ) ");
			// 기간 조회
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apInfo.req_seq  >= '" + tParam.SearchFromDay + "0000' ");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apInfo.req_seq <= '" + tParam.SearchToDay + "9999' ");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apInfo.req_seq >= '" + tParam.SearchFromDay + "0000' AND apInfo.req_seq <= '" + tParam.SearchToDay + "9999'");
			}
			if (tParam.ApproveKind != null && tParam.ApproveKind.Length > 0)
			{
				sb.Append("  AND apInfo.approve_kind = '" + tParam.ApproveKind + "'");
			}
			if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
			{
				sb.Append("  AND COALESCE(tranhis.trans_flag,traninfo.trans_flag) = '" + tParam.TransStatus + "'");
			}
			if (tParam.Sender != null && tParam.Sender.Length > 0)
			{
				sb.Append("  AND usInfo.user_name LIKE '%' || '" + tParam.Sender + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}

			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranInfo.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			sb.Append(" )a ");
			sb.Append(" where 1=1 ");
			if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
			{
				if (tParam.ApprStatus.Equals("1"))
				{
					sb.Append(" AND approveFlag = '1' and approveState != '2'");
				}
				else if (tParam.ApprStatus.Equals("4"))
				{       //승인불필요
					sb.Append(" AND approveFlag = '1' and approveState = '2'");
				}
				else
				{
					sb.Append("  AND approveFlag = '" + tParam.ApprStatus + "'");
				}
			}
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append(" AND sr_type ='" + tParam.TransKind + "'");
			}
			if (tParam.DlpValue != null && tParam.DlpValue.Length > 0)
				sb.Append(" AND dlpFalg = '" + tParam.DlpValue + "'");
			return sb.ToString();
        }
        public string List(MailApproveParam tParam)
        {
            string mainCdSecValue = tParam.SystemId;
            StringBuilder sb = new StringBuilder();
			sb.Append(" SELECT * FROM ( ");
			sb.Append("  SELECT apHis.email_seq as emailSeq,           ");
			sb.Append("			apHis.approve_user_seq as approveUserSeq,       ");
			sb.Append("			aphis.req_seq as reqSeq, ");
			sb.Append("			apHis.approve_kind as ApproveKind,                       ");
			sb.Append("			CASE WHEN SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) = 'I' THEN  'S' ELSE 'R' END as sr_type , ");
			sb.Append("			COALESCE(tranHis.dlp_flag,tranInfo.dlp_flag) as dlpFalg,  ");
			sb.Append("			CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn, ");
			sb.Append("			COALESCE(tranHis.trans_flag,tranInfo.trans_flag) as transStat, ");
			sb.Append("			apHis.approve_flag as approveFlag, ");
			sb.Append("			sendUser.user_name as sendUserNm, ");
			sb.Append("			rcvInfo.addr as revcUser,  ");//-- 0: 수신자, 1: 참조자. (수신자는 필수.)
			sb.Append("			(SELECT COUNT(*) FROM tbl_email_receiver WHERE apHis.email_seq = email_seq and recv_type ='0'), ");
			sb.Append("			COALESCE(tranHis.title,tranInfo.title) as title, ");
			sb.Append("			to_char(to_timestamp(apHis.appr_req_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("			CASE WHEN apHis.approve_flag = '1' THEN '-' ELSE to_char(to_timestamp(apHis.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') END as apprResTime, ");
			sb.Append("			'0' as ApprovePossible, ");
			sb.Append("			'2' as ApproveState ");
			sb.Append("	   FROM tbl_email_Approve_his apHis ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_info tranInfo ON apHis.email_seq = tranInfo.email_seq ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_his tranHis ON apHis.email_seq = tranHis.email_seq ");
			sb.Append("	   LEFT outer join tbl_user_info sendUser ON sendUser.user_seq = apHis.user_seq ");
			sb.Append("	   LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  apHis.email_seq  = fileInfo.email_seq ");//--email_seq에 index 존재함.
			sb.Append("	   LEFT OUTER JOIN tbl_email_receiver rcvInfo ON (aphis.email_seq = rcvinfo.email_seq and rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("		    , tbl_user_info usInfo ");
			sb.Append("	  WHERE apHis.approve_user_seq = usInfo.user_seq ");
			sb.Append("		and aphis.approve_user_seq in( SELECT user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("   and	aphis.approve_user_seq != aphis.user_seq ");        //결재 올린 자가 현재 결재라인에 포함되서 나옴, 결재 올린 자 제외
			sb.Append("		AND ( SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) ='" + mainCdSecValue + "' ) ");

			//기간 조회
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apHis.req_seq  >= '" + tParam.SearchFromDay + "0000' ");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apHis.req_seq <= '" + tParam.SearchToDay + "9999' ");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apHis.req_seq >= '" + tParam.SearchFromDay + "0000' AND apHis.req_seq <= '" + tParam.SearchToDay + "9999'");
			}
			if (tParam.ApproveKind != null && tParam.ApproveKind.Length > 0)
			{
				sb.Append("  AND apHis.approve_kind = '" + tParam.ApproveKind + "'");
			}
			if (tParam.TransStatus != null && tParam.TransStatus.Length > 0 )
			{
				sb.Append("  AND COALESCE(tranhis.trans_flag,traninfo.trans_flag) = '" + tParam.TransStatus + "'");
			}
			if (tParam.Sender != null && tParam.Sender.Length > 0)
			{
				sb.Append("  AND usInfo.user_name LIKE '%' || '" + tParam.Sender + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranHis.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			sb.Append("	  UNION ALL ");
			sb.Append("	 SELECT apInfo.email_seq as emailSeq,     ");
			sb.Append("			apInfo.approve_user_seq as approveUserSeq, ");
			sb.Append("			apInfo.req_seq as reqSeq, ");
			sb.Append("			'0' ApproveKind,                  ");
			sb.Append("			CASE WHEN SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) = 'I' THEN  'S' ELSE 'R' END as sr_type , ");
			sb.Append("			COALESCE(tranHis.dlp_flag,tranInfo.dlp_flag) as dlpFalg, ");
			sb.Append("			CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn, ");
			sb.Append("			COALESCE(tranHis.trans_flag,tranInfo.trans_flag) as transStat, ");
			sb.Append("			apInfo.approve_flag as approveFlag, ");
			sb.Append("			sendUser.user_name as sendUserNm, ");
			sb.Append("			rcvInfo.addr as revcUser, "); //-- 0: 수신자, 1: 참조자. (수신자는 필수.) 
			sb.Append("			(SELECT COUNT(*) FROM tbl_email_receiver where apInfo.email_seq = email_seq and recv_type ='0'), ");
			sb.Append("			COALESCE(tranHis.title,tranInfo.title) as title, ");
			sb.Append("			to_char(to_timestamp(apInfo.appr_req_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("			CASE WHEN apInfo.approve_flag = '1' THEN '-' ELSE to_char(to_timestamp(apInfo.appr_res_time,'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') END as apprResTime, ");
			sb.Append("			(SELECT count(*)  FROM tbl_email_Approve_info ");
			sb.Append("		  	WHERE email_seq = apInfo.email_seq AND approve_user_seq = apInfo.approve_user_seq");
			sb.Append("		    AND approve_order IN (SELECT min(approve_order) FROM tbl_email_approve_info WHERE email_seq = apInfo.email_seq))as ApprovePossible, ");
			sb.Append("			'1' as ApproveState");
			sb.Append("	   FROM tbl_email_Approve_info apInfo ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_info tranInfo ON apInfo.email_seq = tranInfo.email_seq ");
			sb.Append("	   LEFT OUTER JOIN tbl_email_transfer_his tranHis ON apInfo.email_seq = tranHis.email_seq ");
			sb.Append("	   LEFT outer join tbl_user_info sendUser ON sendUser.user_seq = apInfo.user_seq ");
			sb.Append("	   LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  apInfo.email_seq  = fileInfo.email_seq "); //--email_seq에 index 존재함.
			sb.Append("    LEFT OUTER JOIN  tbl_email_receiver rcvInfo ON (apinfo.email_seq = rcvinfo.email_seq and rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("		    , tbl_user_info usInfo ");
			sb.Append("	  WHERE apInfo.approve_user_seq = usInfo.user_seq ");
			sb.Append("		and apInfo.approve_user_seq in( SELECT user_seq from tbl_user_info where user_id = '" + tParam.UserID + "' ");
			if (tParam.APPROVE_TYPE_SFM.Equals("2")) //대결재,결재자기준선택(2:결재자기준이라면)
			{
				sb.Append("		  UNION SELECT B.user_seq FROM tbl_user_info A, tbl_user_sfm B WHERE A.user_id = '" + tParam.UserID + "' AND A.user_seq = B.sfm_user_seq AND to_char(now(), 'YYYYMMDD') between B.fromdate and B.todate");
			}
			sb.Append(")");
			sb.Append("		AND ( SUBSTRING(COALESCE(tranHis.system_id,tranInfo.system_id), 1, 1) ='" + mainCdSecValue + "') ");
			// 기간 조회
			if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apInfo.req_seq  >= '" + tParam.SearchFromDay + "0000' ");
			}
			else if ((tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apInfo.req_seq <= '" + tParam.SearchToDay + "9999' ");
			}
			else if (!(tParam.SearchFromDay.Equals("")) && !(tParam.SearchToDay.Equals("")))
			{
				sb.Append("  AND apInfo.req_seq >= '" + tParam.SearchFromDay + "0000' AND apInfo.req_seq <= '" + tParam.SearchToDay + "9999'");
			}
			if (tParam.ApproveKind != null && tParam.ApproveKind.Length > 0)
			{
				sb.Append("  AND apInfo.approve_kind = '" + tParam.ApproveKind + "'");
			}
			if (tParam.TransStatus != null && tParam.TransStatus.Length > 0)
			{
				sb.Append("  AND COALESCE(tranhis.trans_flag,traninfo.trans_flag) = '" + tParam.TransStatus + "'");
			}
			if (tParam.Sender != null && tParam.Sender.Length > 0)
			{
				sb.Append("  AND usInfo.user_name LIKE '%' || '" + tParam.Sender + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranInfo.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			sb.Append(" )a ");
			sb.Append(" where 1=1 ");
			if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
			{
				if (tParam.ApprStatus.Equals("1"))
				{
					sb.Append(" AND approveFlag = '1' and approveState != '2'");
				}
				else if (tParam.ApprStatus.Equals("4"))
				{       //승인불필요
					sb.Append(" AND approveFlag = '1' and approveState = '2'");
				}
				else
				{
					sb.Append("  AND approveFlag = '" + tParam.ApprStatus + "'");
				}
			}
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append(" AND sr_type ='" + tParam.TransKind + "'");
			}
			if (tParam.DlpValue != null && tParam.DlpValue.Length > 0)
				sb.Append(" AND dlpFalg = '" + tParam.DlpValue + "'");
			sb.Append(" ORDER BY emailSeq desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);
			return sb.ToString();
        }

		public string ListDbFunc(MailApproveEx1Param tParam)
		{

			string strQuery = @$"SELECT * FROM {(tParam.APPROVE_TYPE_SFM != "XXX" ? "FUNC_EMAIL_APPROVEINFO_OPEN_TEST" : "FUNC_EMAIL_APPROVEINFOTYPEFM_OPEN")}('{tParam.UserID}','{tParam.SearchStartDate}','{tParam.SearchEndDate}', ";
			strQuery += @$"'{tParam.GetApproveKindCode()}', '{tParam.GetTransKindCode()}','{tParam.GetApprStatusCode()}','{tParam.GetTransStatusCode(false)}', '{tParam.GetDlpValue()}',";
			strQuery += @$"'{tParam.Sender}', '{tParam.Receiver}', '{tParam.Title}', '0', '{(tParam.bIsDlpPrivacyApprove ? "1" : "0")}', '{tParam.PageListCount}', '{tParam.ViewPageNo}')";
			return strQuery;
		}

		public string TotalCountDbFunc(MailApproveEx1Param tParam)
		{

			string strQuery = @$"SELECT COUNT(*) FROM FUNC_EMAIL_APPROVEINFO_OPEN_TEST('{tParam.UserID}','{tParam.SearchStartDate}','{tParam.SearchEndDate}', ";
			strQuery += @$"'{tParam.GetApproveKindCode()}', '{tParam.GetTransKindCode()}','{tParam.GetApprStatusCode()}','{tParam.GetTransStatusCode(false)}', '{tParam.GetDlpValue()}',";
			strQuery += @$"'{tParam.Sender}', '{tParam.Receiver}', '{tParam.Title}', '0', '{(tParam.bIsDlpPrivacyApprove?"1":"0")}', {(tParam.APPROVE_TYPE_SFM=="2"?"TRUE":"FALSE")} ,'', '')";

			return strQuery;
		}

	}
}
