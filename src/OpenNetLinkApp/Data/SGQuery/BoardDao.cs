using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class BoardDao
    {
        public string List(BoardParam tParam)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select (ROW_NUMBER() OVER()) AS rownum, Z.*                                                                              ");
            sb.Append(" FROM(                                                                                                                   ");
            sb.Append("  select A.idx, A.title, C.admnm,                                                                                        ");
            sb.Append("  substring(A.dtinsert, 1, 4) || '-' || substring(A.dtinsert, 5, 2) || '-' || substring(A.dtinsert, 7, 2)  AS dtinsert   ");
            sb.Append(" from tbl_board_info A, tbl_admmgr C,                                                                                    ");
            sb.Append("   (select * from tbl_board_dept a left join tbl_user_info b ON a.dept_seq = b.dept_seq and b.user_id = '" + tParam.UserID + "') B ");
            sb.Append("    WHERE A.idx = B.idx AND A.loc in ('0', '1')    AND A.useyn = '1'                                                     ");
            sb.Append("    AND A.dtstart <= to_char(NOW(), 'yyyyMMdd010101') AND A.dtend >= to_char(NOW(), 'yyyyMMdd235959')                    ");
            sb.Append("    AND A.writer = C.admcd                                                                                               ");
            sb.Append("     UNION                                                                                                               ");
            sb.Append("  select A.idx, A.title, C.admnm,                                                                                        ");
            sb.Append("  substring(A.dtinsert, 1, 4) || '-' || substring(A.dtinsert, 5, 2) || '-' || substring(A.dtinsert, 7, 2)  AS dtinsert   ");
            sb.Append("  from tbl_board_info A, tbl_admmgr C, tbl_board_dept B                                                                  ");
            sb.Append("  WHERE A.loc in ('0', '1') AND A.useyn = '1' AND A.idx = B.idx                                                          ");
            sb.Append("  AND A.dtstart <= to_char(NOW(), 'yyyyMMdd010101') AND A.dtend >= to_char(NOW(), 'yyyyMMdd235959')                      ");
            sb.Append("  AND B.dept_seq = 0                                                                                                     ");
            sb.Append("  AND A.writer = C.admcd                                                                                                 ");
            sb.Append(" ) Z                                                                                                                     ");
            sb.Append(" ORDER BY Z.dtinsert DESC                                                                                                ");
            sb.Append(" limit " + tParam.PageListCount + " offset (" + tParam.ViewPageNo + "-1) * " + tParam.PageListCount);
            return sb.ToString();
        }

        public string TotalCount(BoardParam tParam)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select COUNT(Z.*) AS totalCount                                                                              ");
            sb.Append(" FROM(                                                                                                                   ");
            sb.Append("  select A.idx, A.title, C.admnm,                                                                                        ");
            sb.Append("  substring(A.dtinsert, 1, 4) || '-' || substring(A.dtinsert, 5, 2) || '-' || substring(A.dtinsert, 7, 2)  AS dtinsert   ");
            sb.Append(" from tbl_board_info A, tbl_admmgr C,                                                                                    ");
            sb.Append("   (select * from tbl_board_dept a left join tbl_user_info b ON a.dept_seq = b.dept_seq and b.user_id = '" + tParam.UserID + "') B ");
            sb.Append("    WHERE A.idx = B.idx AND A.loc in ('0', '1')    AND A.useyn = '1'                                                     ");
            sb.Append("    AND A.dtstart <= to_char(NOW(), 'yyyyMMdd010101') AND A.dtend >= to_char(NOW(), 'yyyyMMdd235959')                    ");
            sb.Append("    AND A.writer = C.admcd                                                                                               ");
            sb.Append("     UNION                                                                                                               ");
            sb.Append("  select A.idx, A.title, C.admnm,                                                                                        ");
            sb.Append("  substring(A.dtinsert, 1, 4) || '-' || substring(A.dtinsert, 5, 2) || '-' || substring(A.dtinsert, 7, 2)  AS dtinsert   ");
            sb.Append("  from tbl_board_info A, tbl_admmgr C, tbl_board_dept B                                                                  ");
            sb.Append("  WHERE A.loc in ('0', '1') AND A.useyn = '1' AND A.idx = B.idx                                                          ");
            sb.Append("  AND A.dtstart <= to_char(NOW(), 'yyyyMMdd010101') AND A.dtend >= to_char(NOW(), 'yyyyMMdd235959')                      ");
            sb.Append("  AND B.dept_seq = 0                                                                                                     ");
            sb.Append("  AND A.writer = C.admcd                                                                                                 ");
            sb.Append(" ) Z                                                                                                                     ");
            return sb.ToString();
        }
    }
}
