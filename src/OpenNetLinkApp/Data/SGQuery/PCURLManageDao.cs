using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class PCURLManageDao
    {
        public string List(PCURLManageParam tParam)
        {
            StringBuilder sb = new StringBuilder();
			sb.Append(" select a.trans_seq ");
			sb.Append(" , cast(0 as bigint) req_seq ");
			sb.Append(" , cast(0 as character) approvepossible ");
			sb.Append(" , cast(a.d_pos as varchar) approvestauts2 ");
			sb.Append(" , cast((case when p.req_flag = '2' then '11' else p.approve_flag end) as character varying) approvestauts ");
			sb.Append(" , u.user_name ");
			sb.Append(" , p.title ");
			sb.Append(" , p.content ");
			sb.Append(" , cast(p.request_time as character varying) ");
			sb.Append(" , cast(p.approve_time as character varying) ");
			sb.Append(" from ( select * from tbl_pcurl_req_info ");
			sb.Append(" 	union all ");
			sb.Append(" 	select * from tbl_pcurl_req_his ");
			sb.Append(" 	where request_time between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" ) p ");
			sb.Append(" , ( ");
			sb.Append(" 	select trans_seq, max(d_pos) d_pos, max(approve_flag) approve_flag ");
			sb.Append(" 	from( ");
			sb.Append(" 		select 1 d_pos, * ");
			sb.Append(" 		from tbl_approve_info ");
			sb.Append(" 		union all ");
			sb.Append(" 		select 0 d_pos, * ");
			sb.Append(" 		from tbl_approve_his ");
			sb.Append(" 		where appr_req_time between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" 	) aa group by trans_seq ");
			sb.Append(" ) a ");
			sb.Append(" , tbl_user_info u ");
			sb.Append(" where p.trans_seq = a.trans_seq ");
			sb.Append(" and p.user_seq = u.user_seq ");
			sb.Append(" and u.user_seq = " + tParam.UserSeq );
			
			if( tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
            {
				if (tParam.ApprStatus == "11")
					sb.Append(" AND p.req_flag = '2' ");
				else
					sb.Append(" AND p.approve_flag = '" + tParam.ApprStatus + "'");
			}
			if (tParam.Url != null && tParam.Url.Length > 0)
				sb.Append(" AND  func_nl2_urlcount(p.trans_seq, '" + tParam.Url + "')>0 ");
			if( tParam.Title != null && tParam.Title.Length > 0)
				sb.Append(" AND  title like '%" + tParam.Title + "%'");
			
			sb.Append(" order by p.request_time desc ");
			sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);
			return sb.ToString();
        }
        public string TotalCount(PCURLManageParam tParam)
        {
            StringBuilder sb = new StringBuilder();
			sb.Append(" SELECT COUNT(a.trans_seq)  ");
			sb.Append(" from ( select * from tbl_pcurl_req_info ");
			sb.Append(" 	union all ");
			sb.Append(" 	select * from tbl_pcurl_req_his ");
			sb.Append(" 	where request_time between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" ) p ");
			sb.Append(" , ( ");
			sb.Append(" 	select trans_seq, max(d_pos) d_pos, max(approve_flag) approve_flag ");
			sb.Append(" 	from( ");
			sb.Append(" 		select 1 d_pos, * ");
			sb.Append(" 		from tbl_approve_info ");
			sb.Append(" 		union all ");
			sb.Append(" 		select 0 d_pos, * ");
			sb.Append(" 		from tbl_approve_his ");
			sb.Append(" 		where appr_req_time between '" + tParam.SearchFromDay + "' and '" + tParam.SearchToDay + "'");
			sb.Append(" 	) aa group by trans_seq ");
			sb.Append(" ) a ");
			sb.Append(" , tbl_user_info u ");
			sb.Append(" where p.trans_seq = a.trans_seq ");
			sb.Append(" and p.user_seq = u.user_seq ");
			sb.Append(" and u.user_seq = " + tParam.UserSeq);

			if (tParam.ApprStatus != null && tParam.ApprStatus.Length > 0)
			{
				if (tParam.ApprStatus == "11")
					sb.Append(" AND p.req_flag = '2' ");
				else
					sb.Append(" AND p.approve_flag = '" + tParam.ApprStatus + "'");
			}
			if (tParam.Url != null && tParam.Url.Length > 0)
				sb.Append(" AND  func_nl2_urlcount(p.trans_seq, '" + tParam.Url + "')>0 ");
			if (tParam.Title != null && tParam.Title.Length > 0)
				sb.Append(" AND  title like '%" + tParam.Title + "%'");
			return sb.ToString();
        }

    }
}
