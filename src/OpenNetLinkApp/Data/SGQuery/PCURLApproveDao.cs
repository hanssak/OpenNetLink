using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class PCURLApproveDao
    {
        public string List(PCURLApproveParam tParam)
        {
            StringBuilder sb = new StringBuilder();
			sb.Append(" SELECT A.TRANS_SEQ, A.REQ_SEQ, CAST((CASE WHEN GETAPPROVEPOSSIBLE2(A.APPROVE_USER_SEQ, A.TRANS_SEQ, A.APPROVE_ORDER) > 0 THEN 1 ELSE 0 END) AS CHARACTER) APPROVEPOSSIBLE ");
			sb.Append(" , CAST(A.D_POS AS VARCHAR) APPROVESTAUTS2 ");
			sb.Append(" , CAST((CASE WHEN P.REQ_FLAG = '2' THEN '11' ELSE A.APPROVE_FLAG END) AS CHARACTER VARYING) APPROVESTAUTS ");
			sb.Append(" , CAST(U.USER_NAME || (CASE WHEN COALESCE(U.USER_RANK, U.USER_POSITION, '') != '' THEN('' || COALESCE(U.USER_RANK, U.USER_POSITION)) ELSE '' END) AS VARCHAR) ");
			sb.Append(" , P.TITLE, P.CONTENT, CAST(P.REQUEST_TIME AS CHARACTER VARYING), CAST(P.APPROVE_TIME AS CHARACTER VARYING) ");
			sb.Append(" FROM ( ");
			sb.Append(" SELECT * ");
			sb.Append(" FROM TBL_PCURL_REQ_INFO ");
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT * ");
			sb.Append(" FROM TBL_PCURL_REQ_HIS ");
			sb.Append(" WHERE REQUEST_TIME between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" ) P ");
			sb.Append(" INNER JOIN( ");
			sb.Append(" SELECT 1 D_POS, * FROM TBL_APPROVE_INFO ");
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT 0 D_POS, * FROM TBL_APPROVE_HIS ");
			sb.Append(" WHERE APPR_REQ_TIME between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" ) A ON P.TRANS_SEQ = A.TRANS_SEQ  ");
			sb.Append(" LEFT OUTER JOIN (   ");
			sb.Append(" SELECT * ");
			sb.Append(" FROM TBL_USER_SFM S ");
			sb.Append(" WHERE NOW() BETWEEN TO_DATE(FROMDATE || '000000', 'YYYYMMDDHH24MISS') AND TO_DATE(TODATE || '235959', 'YYYYMMDDHH24MISS') ");
			sb.Append(" ) S ON A.APPROVE_USER_SEQ = S.USER_SEQ ");

			sb.Append(" LEFT JOIN TBL_USER_INFO U ON U.USER_SEQ = A.USER_SEQ ");
			sb.Append(" WHERE(A.APPROVE_USER_SEQ = " + tParam.UserSeq + " OR S.SFM_USER_SEQ = " + tParam.UserSeq + ") ");

			if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
			{
				if (tParam.ApprStatus == "11")
					sb.Append(" AND p.req_flag = '2' ");
				else
					sb.Append(" AND p.approve_flag = '" + tParam.ApprStatus + "'");
			}
			if (tParam.ReqUserName != null && tParam.ReqUserName.Length > 0)
				sb.Append(" AND  u.user_name like '%" + tParam.ReqUserName + "%'");

			if (tParam.Url != null && tParam.Url.Length > 0)
				sb.Append(" AND  func_nl2_urlcount(p.trans_seq, '" + tParam.Url + "')>0 ");
			if (tParam.Title != null && tParam.Title.Length > 0)
				sb.Append(" AND  title like '%" + tParam.Title + "%'");

			sb.Append(" order by p.request_time desc ");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);

			return sb.ToString();
        }
        public string TotalCount(PCURLApproveParam tParam)
        {
            StringBuilder sb = new StringBuilder();
			sb.Append(" SELECT COUNT(A.TRANS_SEQ) ");
			sb.Append(" FROM ( ");
			sb.Append(" SELECT * ");
			sb.Append(" FROM TBL_PCURL_REQ_INFO ");
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT * ");
			sb.Append(" FROM TBL_PCURL_REQ_HIS ");
			sb.Append(" WHERE REQUEST_TIME between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" ) P ");
			sb.Append(" INNER JOIN( ");
			sb.Append(" SELECT 1 D_POS, * FROM TBL_APPROVE_INFO ");
			sb.Append(" UNION ALL ");
			sb.Append(" SELECT 0 D_POS, * FROM TBL_APPROVE_HIS ");
			sb.Append(" WHERE APPR_REQ_TIME between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" ) A ON P.TRANS_SEQ = A.TRANS_SEQ  ");
			sb.Append(" LEFT OUTER JOIN (   ");
			sb.Append(" SELECT * ");
			sb.Append(" FROM TBL_USER_SFM S ");
			sb.Append(" WHERE NOW() BETWEEN TO_DATE(FROMDATE || '000000', 'YYYYMMDDHH24MISS') AND TO_DATE(TODATE || '235959', 'YYYYMMDDHH24MISS') ");
			sb.Append(" ) S ON A.APPROVE_USER_SEQ = S.USER_SEQ ");

			sb.Append(" LEFT JOIN TBL_USER_INFO U ON U.USER_SEQ = A.USER_SEQ ");
			sb.Append(" WHERE(A.APPROVE_USER_SEQ = " + tParam.UserSeq + " OR S.SFM_USER_SEQ = " + tParam.UserSeq + ") ");

			if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
			{
				if (tParam.ApprStatus == "11")
					sb.Append(" AND p.req_flag = '2' ");
				else
					sb.Append(" AND p.approve_flag = '" + tParam.ApprStatus + "'");
			}
			if (tParam.ReqUserName != null && tParam.ReqUserName.Length > 0)
				sb.Append(" AND  u.user_name like '%" + tParam.ReqUserName + "%'");

			if (tParam.Url != null && tParam.Url.Length > 0)
				sb.Append(" AND  func_nl2_urlcount(p.trans_seq, '" + tParam.Url + "')>0 ");
			if (tParam.Title != null && tParam.Title.Length > 0)
				sb.Append(" AND  title like '%" + tParam.Title + "%'");
			return sb.ToString();
        }
    }
}
