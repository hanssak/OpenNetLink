using System;
using System.Collections.Generic;
using System.Text;
using OpenNetLinkApp.Data.SGDomain;
namespace OpenNetLinkApp.Data.SGQuery
{
    class MailManageDao
    {
		public string MailDetail(string seq, bool bUseSfm2 = false)
        {
			string sql = String.Empty;
			sql = $"SELECT func_email_detail_open({seq}, {(bUseSfm2?"TRUE":"FALSE")})";
			return sql;
        }
        public string List(MailParam tParam)
        {
			string mainCdSecValue = "";
			if ((tParam.SystemId?.Length ?? 0) > 0)
				mainCdSecValue = tParam.SystemId.Substring(0, 1);

			StringBuilder sb = new StringBuilder();
			sb.Append("select * from(");
			sb.Append("	SELECT tranHis.sr_type, tranHis.email_seq as emailSeq,   ");
			sb.Append("	'0' ApproveKind,");
			sb.Append("	CASE WHEN SUBSTRING(tranHis.system_id, 1, 1) = 'I' THEN  '1' ELSE '2' END as systemId ,");
			sb.Append("	tranHis.dlp_flag as dlpFalg,");
			sb.Append("	CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn,");
			sb.Append("	tranHis.trans_flag as transStat,");
			sb.Append("	tranHis.approve_flag as approveFlag,");
			sb.Append("	CASE WHEN ");
			sb.Append("	( ");
			sb.Append("	      SELECT Count(*) -1 as cnt");   
			sb.Append("	      FROM   tbl_email_receiver ");
			sb.Append("	      WHERE  email_seq=tranHis.email_seq ");
			sb.Append("	      AND    recv_type='0') = 0");
			sb.Append("	then");
			sb.Append("	rcvInfo.addr ");
			sb.Append("	else");
			sb.Append("	rcvInfo.addr || ' 외 ' ||cnt+1 || '명' end AS revcuser , ");
			sb.Append("       tranHis.title ,");
			sb.Append("       tranHis.approve_flag,");
			sb.Append("to_char(to_timestamp(substring(tranHis.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime,");
			sb.Append("to_char(to_timestamp(substring(tranHis.approve_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as apprResTime");
			sb.Append("	FROM   tbl_email_transfer_his tranHis");
			sb.Append("	LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  tranHis.email_seq  = fileInfo.email_seq ");
			sb.Append(" LEFT OUTER JOIN tbl_email_receiver rcvInfo on (tranhis.email_seq = rcvinfo.email_seq AND rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("	, tbl_user_info usInfo ");
			sb.Append("	 WHERE  usInfo.user_id ='" + tParam.UserID + "' ");
			//sb.Append("	 AND    ( ");
			//sb.Append("                              Substring(tranHis.system_id, 1, 2) ='I0' ");
			//sb.Append("                       OR     Substring(tranHis.system_id, 1, 2) ='E0' ) ");
			sb.Append("	AND    tranHis.user_seq = usInfo.user_seq ");

			if (mainCdSecValue.Length > 0)
				sb.Append("	AND  SUBSTRING(tranHis.system_id, 1, 1) ='" + mainCdSecValue + "' ");

			//날짜검색 
			sb.Append("  AND tranHis.request_time >= '" + tParam.SearchStartDate + "' AND tranHis.request_time <= '" + tParam.SearchEndDate + "'");

			if (tParam.GetTransStatusCode() != null && tParam.GetTransStatusCode().Length > 0)
			{
				sb.Append("  AND tranHis.trans_flag = '" + tParam.GetTransStatusCode() + "'");
			}
			if (tParam.GetApprStatusCode() != null && tParam.GetApprStatusCode().Length > 0)
			{
				sb.Append("  AND tranHis.approve_flag = '" + tParam.GetApprStatusCode() + "'");
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranHis.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append(" AND tranHis.sr_type ='" + tParam.TransKind + "'");
			}
			if (tParam.DlpValue != null && tParam.DlpValue.Length > 0)
			{
				if (tParam.GetDlpValueCode() == "1")
					sb.Append(" AND tranHis.dlp_flag ='1' ");
				else
					sb.Append(" AND tranHis.dlp_flag in ('0','2') ");
			}
			sb.Append("	UNION ALL ");
			sb.Append("	SELECT tranInfo.sr_type, tranInfo.email_seq as emailSeq,   ");
			sb.Append("	'0' ApproveKind,");
			sb.Append("	CASE WHEN SUBSTRING(tranInfo.system_id, 1, 1) = 'I' THEN  '1' ELSE '2' END as systemId ,");
			sb.Append("	tranInfo.dlp_flag as dlpFalg,");
			sb.Append("	CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn,");
			sb.Append("	tranInfo.trans_flag as transStat,");
			sb.Append("	tranInfo.approve_flag as approveFlag,");
			sb.Append("	CASE WHEN ");
			sb.Append("	( ");
			sb.Append("	      SELECT Count(*) -1 as cnt");
			sb.Append("	      FROM   tbl_email_receiver ");
			sb.Append("	      WHERE  email_seq=tranInfo.email_seq ");
			sb.Append("	      AND    recv_type='0') = 0");
			sb.Append("	then");
			sb.Append("	rcvInfo.addr ");
			sb.Append("	else");
			sb.Append("	rcvInfo.addr || ' 외 ' ||cnt+1 || '명' end AS revcuser , ");
			sb.Append("       tranInfo.title ,");
			sb.Append("       tranInfo.approve_flag,");
			sb.Append("to_char(to_timestamp(substring(tranInfo.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("to_char(to_timestamp(substring(tranInfo.approve_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as apprResTime");
			sb.Append("	FROM   tbl_email_transfer_info tranInfo");
			sb.Append("	LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  tranInfo.email_seq  = fileInfo.email_seq");
			sb.Append(" LEFT OUTER JOIN tbl_email_receiver rcvInfo on (traninfo.email_seq = rcvinfo.email_seq and rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("	, tbl_user_info usInfo ");
			sb.Append("	 WHERE  usInfo.user_id ='" + tParam.UserID + "' ");
			//sb.Append("	 AND    ( ");
			//sb.Append("                              Substring(tranInfo.system_id, 1, 2) ='I0' ");
			//sb.Append("                       OR     Substring(tranInfo.system_id, 1, 2) ='E0' ) ");
			sb.Append("	AND    tranInfo.user_seq = usInfo.user_seq ");

			if (mainCdSecValue.Length > 0)
				sb.Append("	AND SUBSTRING(tranInfo.system_id, 1, 1) ='" + mainCdSecValue + "' ");

			//날짜검색
			sb.Append("  AND tranInfo.request_time >= '" + tParam.SearchStartDate + "' AND tranInfo.request_time <= '" + tParam.SearchEndDate + "'");
			if (tParam.GetTransStatusCode() != null && tParam.GetTransStatusCode().Length > 0)
			{
				sb.Append("  AND tranInfo.trans_flag = '" + tParam.GetTransStatusCode() + "'");
			}
			if (tParam.GetApprStatusCode() != null && tParam.GetApprStatusCode().Length > 0)
			{
				sb.Append("  AND tranInfo.approve_flag = '" + tParam.GetApprStatusCode() + "'");
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranInfo.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append(" AND tranInfo.sr_type ='" + tParam.TransKind + "'");
			}
			if (tParam.DlpValue != null && tParam.DlpValue.Length > 0)
			{
				if (tParam.GetDlpValueCode() == "1")
					sb.Append(" AND tranInfo.dlp_flag ='1' ");
				else
					sb.Append(" AND tranInfo.dlp_flag in ('0','2') ");
			}
			sb.Append(") a");
			sb.Append(" ORDER BY a.emailSeq desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);
			return sb.ToString();
		}

        public string TotalCount(MailParam tParam)
        {
			string mainCdSecValue = "";

			if ((tParam.SystemId?.Length ?? 0) > 0)
				mainCdSecValue = tParam.SystemId.Substring(0, 1);


			StringBuilder sb = new StringBuilder();
			sb.Append("select count(*) AS count from(");
			sb.Append("	SELECT tranHis.email_seq as emailSeq,   ");
			sb.Append("	'0' ApproveKind,");
			sb.Append("	CASE WHEN SUBSTRING(tranHis.system_id, 1, 1) = 'I' THEN  '1' ELSE '2' END as systemId ,");
			sb.Append("	tranHis.dlp_flag as dlpFalg,");
			sb.Append("	CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn,");
			sb.Append("	tranHis.trans_flag as transStat,");
			sb.Append("	tranHis.approve_flag as approveFlag,");
			sb.Append("	CASE WHEN ");
			sb.Append("	( ");
			sb.Append("	      SELECT Count(*) -1 as cnt");
			sb.Append("	      FROM   tbl_email_receiver ");
			sb.Append("	      WHERE  email_seq=tranHis.email_seq ");
			sb.Append("	      AND    recv_type='0') = 0");
			sb.Append("	then");
			sb.Append("	rcvInfo.addr ");
			sb.Append("	else");
			sb.Append("	rcvInfo.addr || ' and ' ||cnt+1 || '명' end AS revcuser , ");
			sb.Append("       tranHis.title ,");
			sb.Append("       tranHis.approve_flag,");
			sb.Append("to_char(to_timestamp(substring(tranHis.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime,");
			sb.Append("to_char(to_timestamp(substring(tranHis.approve_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as apprResTime");
			sb.Append("	FROM   tbl_email_transfer_his tranHis");
			sb.Append("	LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  tranHis.email_seq  = fileInfo.email_seq ");
			sb.Append(" LEFT OUTER JOIN tbl_email_receiver rcvInfo on (tranhis.email_seq = rcvinfo.email_seq AND rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("	, tbl_user_info usInfo ");
			sb.Append("	 WHERE  usInfo.user_id ='" + tParam.UserID + "' ");
			//sb.Append("	 AND    ( ");
			//sb.Append("                              Substring(tranHis.system_id, 1, 2) ='I0' ");
			//sb.Append("                       OR     Substring(tranHis.system_id, 1, 2) ='E0' ) ");
			sb.Append("	AND    tranHis.user_seq = usInfo.user_seq ");

			if (mainCdSecValue.Length > 0)
				sb.Append("	AND  SUBSTRING(tranHis.system_id, 1, 1) ='" + mainCdSecValue + "'");

			//날짜검색 
			sb.Append("  AND tranHis.request_time >= '" + tParam.SearchStartDate + "' AND tranHis.request_time <= '" + tParam.SearchEndDate + "'");
			
			if (tParam.GetTransStatusCode() != null && tParam.GetTransStatusCode().Length > 0)
			{
				sb.Append("  AND tranHis.trans_flag = '" + tParam.GetTransStatusCode() + "'");
			}
			if (tParam.GetApprStatusCode() != null && tParam.GetApprStatusCode().Length > 0)
			{
				sb.Append("  AND tranHis.approve_flag = '" + tParam.GetApprStatusCode() + "'");
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranHis.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{       
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			if(tParam.TransKind != null && tParam.TransKind.Length > 0)
            {
				sb.Append(" AND tranHis.sr_type ='" + tParam.TransKind + "'");
            }
			if(tParam.DlpValue != null && tParam.DlpValue.Length > 0)
            {
				if(tParam.GetDlpValueCode() == "1")
					sb.Append(" AND tranHis.dlp_flag ='1' ");
				else
					sb.Append(" AND tranHis.dlp_flag in ('0','2') ");
			}

			sb.Append("	UNION ALL ");
			sb.Append("	SELECT tranInfo.email_seq as emailSeq,   ");
			sb.Append("	'0' ApproveKind,");
			sb.Append("	CASE WHEN SUBSTRING(tranInfo.system_id, 1, 1) = 'I' THEN  '1' ELSE '2' END as systemId ,");
			sb.Append("	tranInfo.dlp_flag as dlpFalg,");
			sb.Append("	CASE WHEN COALESCE(fileInfo.cnt,0)=0 THEN 'N' ELSE 'Y' END as addFileYn,");
			sb.Append("	tranInfo.trans_flag as transStat,");
			sb.Append("	tranInfo.approve_flag as approveFlag,");
			sb.Append("	CASE WHEN ");
			sb.Append("	( ");
			sb.Append("	      SELECT Count(*) -1 as cnt");
			sb.Append("	      FROM   tbl_email_receiver ");
			sb.Append("	      WHERE  email_seq=tranInfo.email_seq ");
			sb.Append("	      AND    recv_type='0') = 0");
			sb.Append("	then");
			sb.Append("	rcvInfo.addr ");
			sb.Append("	else");
			sb.Append("	rcvInfo.addr || ' 외 ' ||cnt+1 || '명' end AS revcuser , ");
			sb.Append("       tranInfo.title ,");
			sb.Append("       tranInfo.approve_flag,");
			sb.Append("to_char(to_timestamp(substring(tranInfo.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
			sb.Append("to_char(to_timestamp(substring(tranInfo.approve_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as apprResTime");
			sb.Append("	FROM   tbl_email_transfer_info tranInfo");
			sb.Append("	LEFT OUTER JOIN ( select email_seq, count(*) as cnt from tbl_email_add_file group by email_seq )fileInfo ON  tranInfo.email_seq  = fileInfo.email_seq");
			sb.Append(" LEFT OUTER JOIN tbl_email_receiver rcvInfo on (traninfo.email_seq = rcvinfo.email_seq and rcvinfo.recv_type = '0' and rcvinfo.email_no = 1) ");
			sb.Append("	, tbl_user_info usInfo ");
			sb.Append("	 WHERE  usInfo.user_id ='" + tParam.UserID + "' ");
			//sb.Append("	 AND    ( ");
			//sb.Append("                              Substring(tranInfo.system_id, 1, 2) ='I0' ");
			//sb.Append("                       OR     Substring(tranInfo.system_id, 1, 2) ='E0' ) ");
			sb.Append("	AND    tranInfo.user_seq = usInfo.user_seq ");

			if (mainCdSecValue.Length > 0)
				sb.Append("	AND  SUBSTRING(tranInfo.system_id, 1, 1) ='" + mainCdSecValue + "'" );

			//날짜검색
			sb.Append("  AND tranInfo.request_time >= '" + tParam.SearchStartDate + "' AND tranInfo.request_time <= '" + tParam.SearchEndDate + "'");
			
			if (tParam.GetTransStatusCode() != null && tParam.GetTransStatusCode().Length > 0)
			{
				sb.Append("  AND tranInfo.trans_flag = '" + tParam.GetTransStatusCode() + "'");
			}
			if (tParam.GetApprStatusCode() != null && tParam.GetApprStatusCode().Length > 0)
			{
				sb.Append("  AND tranInfo.approve_flag = '" + tParam.GetApprStatusCode() + "'");
			}
			if (tParam.Title != null && tParam.Title.Length > 0)
			{
				sb.Append("  AND tranInfo.title LIKE '%' || '" + tParam.Title + "' || '%'");
			}
			if (tParam.Receiver != null && tParam.Receiver.Length > 0)
			{
				sb.Append("  AND rcvInfo.addr LIKE '%' || '" + tParam.Receiver + "' || '%'");
			}
			if (tParam.TransKind != null && tParam.TransKind.Length > 0)
			{
				sb.Append(" AND tranInfo.sr_type ='" + tParam.TransKind + "'");
			}
			if (tParam.DlpValue != null && tParam.DlpValue.Length > 0)
			{
				if (tParam.GetDlpValueCode() == "1")
					sb.Append(" AND tranInfo.dlp_flag ='1' ");
				else
					sb.Append(" AND tranInfo.dlp_flag in ('0','2') ");
			}
			sb.Append(") a");



			return sb.ToString();
		}

		public string ListDbFunc(MailParam tParam)
        {

			// FUNC_EMAIL_TRANSFERINFO_OPEN
			// strQuery += @$"'{tParam.Receiver}', '{tParam.Title}', '0', '{tParam.PageListCount}', '{tParam.ViewPageNo}')";

			string strQuery = @$"SELECT * FROM FUNC_EMAIL_TRANSFERINFO_OPEN('{tParam.UserID}','{tParam.SearchStartDate}','{tParam.SearchEndDate}', ";
			strQuery += @$"'{tParam.GetApproveKindCode()}', '{tParam.GetTransKindCode()}','{tParam.GetApprStatusCode()}','{tParam.GetTransStatusCode(false)}', '{tParam.GetDlpValue()}',";
			strQuery += @$"'{tParam.Receiver}', '{tParam.Title}', '0', '{tParam.PageListCount}', '{tParam.ViewPageNo}')";

			return strQuery;

			/*sb.Append(" ORDER BY a.emailSeq desc");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);
			return sb.ToString();*/
		}

		public string TotalCountDbFunc(MailParam tParam)
        {
			string strQuery = @$"SELECT COUNT(*) FROM FUNC_EMAIL_TRANSFERINFO_OPEN('{tParam.UserID}','{tParam.SearchStartDate}','{tParam.SearchEndDate}', ";
			strQuery += @$"'{tParam.GetApproveKindCode()}', '{tParam.GetTransKindCode()}','{tParam.GetApprStatusCode()}','{tParam.GetTransStatusCode(false)}', '{tParam.GetDlpValue()}',";
			strQuery += @$"'{tParam.Receiver}', '{tParam.Title}', '0', '', '')";

			return strQuery;
		}

	}
}
