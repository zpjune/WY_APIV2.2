using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class CheckResultDB
    {
        DBTool db = new DBTool("");

        public DataTable GetCheckResult(string year,string FWBH,string RWMC,string JCJG)
        {
            string sql = "select a.RESULT_ID,a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,g.ZHXM,GROUP_CONCAT(h.`Name`) AS JCQY,j.Name" +
                " from wy_check_result a" +
                " join wy_check_task b on a.TASK_ID=b.TASK_ID AND b.IS_DELETE=0" +
                " join wy_map_checkplandetail c ON b.TASK_ID=c.TASK_ID" +
                " JOIN wy_checkPlan_detail d ON c.PLAN_DETAIL_ID = d.PLAN_DETAIL_ID AND d.IS_DELETE = 0" +
                " JOIN wy_checkPlan e ON d.PLAN_ID = e.PLAN_ID AND e.IS_DELETE = 0" +
                " JOIN wy_houseinfo f ON a.FWID = f.FWID AND f.IS_DELETE = 0" +
                " JOIN wy_shopinfo g ON f.CZ_SHID = g.CZ_SHID AND g.IS_DELETE = 0 " +
                " JOIN v_taskregion h ON h.PLAN_DETAIL_ID = d.PLAN_DETAIL_ID" +
                " JOIN wy_region_director i ON a.JCR = i.WX_OPEN_ID" +
                " JOIN tax_dictionary j on f.SSQY=j.Code AND j.ParentCode='SSQY'" +
                " where a.IS_DELETE=0";
            if (!string.IsNullOrEmpty(year))
            {
                sql += " AND e.JHND='" + year + "'";
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                sql += " AND e.FWBH='" + FWBH + "'";
            }
            if (!string.IsNullOrEmpty(RWMC))
            {
                sql += " AND b.RWMC like'%" + RWMC + "%'";
            }
            if (!string.IsNullOrEmpty(JCJG))
            {
                sql += " AND a.JCJG=" + JCJG;
            }
            sql += " GROUP BY a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,g.ZHXM,a.RESULT_ID,j.Name";
            sql += " ORDER BY b.RWBH,j.Name";
            return db.GetDataTable(sql);
        }

        public DataTable GetCheckResultDetail(string RESULT_ID)
        {
            string sql = "SELECT DISTINCT e.`Name` AS DL,f.`Name` AS XL," +
                " (CASE h.CHECK_DETAIL_RESULT WHEN 0 THEN '不合格' WHEN  1 THEN '合格' ELSE '未检查'END ) AS result" +
                " FROM wy_check_task a " +
                " JOIN wy_map_checkplandetail b ON a.TASK_ID=b.TASK_ID" +
                " JOIN wy_checkplan_detail c ON b.PLAN_DETAIL_ID=c.PLAN_DETAIL_ID" +
                " JOIN wy_map_region d ON c.PLAN_DETAIL_ID=d.PLAN_DETAIL_ID" +
                " JOIN wy_task_detail_config e ON e.`Code`=c.JCLX AND e.ParentID is NULL" +
                " JOIN wy_task_detail_config f ON f.ParentID=e.ID" +
                " JOIN wy_check_result g ON a.TASK_ID=g.TASK_ID" +
                " JOIN wy_check_result_detail h ON f.`Code`=h.DETAIL_CODE AND h.RESULT_ID=g.RESULT_ID" +
                " AND g.RESULT_ID='" + RESULT_ID + "' ORDER BY e.`Name`";
            return db.GetDataTable(sql);
        }

       public DataTable GetTaskProcessInfo(string year,string RWMC,string RWBH)
        {
            string sql= "select *,(t.total-t.complete) AS incomplete from" +
                "(select a.RWBH,a.RWMC,a.RWKSSJ,a.RWJSSJ,a.RWFW,d.NAME," +
                "(SELECT COUNT(*)AS TOTAL FROM wy_houseinfo where SSQY IN (SELECT REGION_CODE FROM wy_map_region WHERE PLAN_DETAIL_ID=a.PLAN_DETAIL_ID AND IS_DELETE=0))AS total," +
                "(select COUNT(distinct FWID) FROM wy_check_result  where a.PLAN_DETAIL_ID=TASK_ID AND IS_DELETE=0) AS complete" +
                " FROM wy_check_task a" +
                " join wy_checkPlan_detail b on a.PLAN_DETAIL_ID=b.PLAN_DETAIL_ID AND b.IS_DELETE=0 " +
                " join wy_checkPlan c on b.PLAN_ID=c.PLAN_ID AND c.IS_DELETE=0" +
                //" left join tax_dictionary d on a.RWFW=d.Code AND d.ParentCode='SSQY'" +
                " left join V_TaskRegion d on a.PLAN_DETAIL_ID=d.PLAN_DETAIL_ID" +
                " where 1=1";
            if (!string.IsNullOrEmpty(year))
            {
                sql += " AND  c.JHND=" + year;
            }
            if (!string.IsNullOrEmpty(RWMC))
            {
                sql += " AND  a.RWMC='" + RWMC + "'";
            }
            if (!string.IsNullOrEmpty(RWBH))
            {
                sql += " AND  a.RWBH='" + RWBH + "'";
            }
            sql += " GROUP BY a.RWBH,a.RWMC,a.RWKSSJ,a.RWJSSJ,a.RWFW,a.TASK_ID,a.PLAN_DETAIL_ID,d.Name)t";
            return db.GetDataTable(sql);
        }
    }
}
