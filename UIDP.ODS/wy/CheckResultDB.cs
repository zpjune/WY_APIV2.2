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
            string sql = "select a.RESULT_ID,a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,f.FWMC,g.ZHXM,GROUP_CONCAT(h.`Name`) AS JCQY,j.Name,a.JCCS,a.IS_REVIEW" +
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
            sql += " GROUP BY a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,f.FWMC,g.ZHXM,a.RESULT_ID,j.Name,a.JCCS,a.IS_REVIEW";
            sql += " ORDER BY b.RWBH,j.Name";
            return db.GetDataTable(sql);
        }
        public DataSet Rectification(string[] arr)
        {
            string RESULT = "SELECT b.FWBH,b.FWMC,a.JCSJ,a.RESULT_ID," +
                " ( CASE WHEN d.ZHXM IS NULL OR d.ZHXM = '' THEN c.ZHXM ELSE d.ZHXM END ) AS ZHXM," +
                " ( CASE WHEN d.OPEN_ID IS NULL OR d.OPEN_ID = '' THEN c.OPEN_ID ELSE d.OPEN_ID END ) AS OPEN_ID" +
                " FROM" +
                " wy_check_result a" +
                " JOIN wy_houseinfo b ON a.FWID = b.FWID AND b.IS_DELETE = 0" +
                " JOIN wy_shopinfo c ON b.CZ_SHID = c.CZ_SHID AND c.IS_DELETE = 0" +
                " LEFT JOIN wy_shopinfo d ON c.SUBLET_ID = d.CZ_SHID AND d.IS_SUBLET =1" +
                " where a.RESULT_ID IN {0}";
            string DEATAILRESULT = "SELECT a.RESULT_ID,a.CHECK_DETAIL_RESULT,b.`Name` FROM wy_check_result_detail a " +
                " JOIN wy_task_detail_config b ON a.DETAIL_CODE=b.`Code` AND b.ParentID is NOT NULL" +
                " where a.RESULT_ID IN {0}";
            string conditionsql = string.Empty;
            foreach(string str in arr)
            {
                conditionsql += "'" + str + "',";
            }
            conditionsql = conditionsql.TrimEnd(',');
            Dictionary<string, string> d = new Dictionary<string, string>()
            {
                {"result",string.Format(RESULT,"("+conditionsql+")") },
                {"detail",string.Format(DEATAILRESULT,"("+conditionsql+")") }
            };
            return db.GetDataSet(d); 
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
                " LEFT JOIN wy_check_result_detail h ON f.`Code`=h.DETAIL_CODE AND h.RESULT_ID=g.RESULT_ID" +
                " AND g.RESULT_ID='" + RESULT_ID + "' ORDER BY e.`Name`";
            return db.GetDataTable(sql);
        }

       public DataTable GetTaskProcessInfo(string year,string RWMC,string RWBH)
        {
            //本段查询比较复杂，若需要修改时，可以将查询内()t内的语句拿出来，同时将两个子查询先剥离出去进行修改，最后在修改子查询
            //本段查询的主要逻辑就是通过查询任务范围内应该检查的房子数量减去已经检查过的房子数量，就可以得知没有进行检查的房子数量。
            string sql= "select *,(t.total-t.complete) AS incomplete from" +
                "(select a.RWBH,a.RWMC,a.RWKSSJ,a.RWJSSJ,a.RWFW,GROUP_CONCAT( e.`Name` )," +
                //以下为子查询，查询当前任务内包含的所有房子数量，限制条件为:1.任务区域内的房子 2.房子不是空闲状态。
                "(SELECT COUNT(*)AS TOTAL FROM wy_houseinfo where SSQY IN (" +
                " SELECT REGION_CODE FROM wy_map_region f " +
                " JOIN wy_checkplan_detail g ON f.PLAN_DETAIL_ID = g.PLAN_DETAIL_ID AND g.IS_DELETE=0" +
                " JOIN wy_map_checkplandetail h ON h.PLAN_DETAIL_ID = g.PLAN_DETAIL_ID" +
                " JOIN wy_check_task i ON i.TASK_ID = h.TASK_ID AND i.IS_DELETE=0" +
                " WHERE i.TASK_ID = a.TASK_ID)" +
                " AND FWSX != 0) AS total," +
                //上述子查询到此为止
                //以下为子查询，查询当前任务内已经检查过的房子数量
                "(select COUNT(distinct FWID) FROM wy_check_result j" +
                " JOIN wy_check_task k ON j.TASK_ID = k.TASK_ID AND k.IS_DELETE=0" +
                " WHERE j.TASK_ID = a.TASK_ID) AS complete " +
                //上述子查询到此为止
                " FROM wy_check_task a" +
                " JOIN wy_map_checkplandetail b ON a.TASK_ID = b.TASK_ID " +
                " JOIN wy_checkplan_detail c ON b.PLAN_DETAIL_ID = c.PLAN_DETAIL_ID AND c.IS_DELETE = 0" +
                " JOIN wy_checkplan d ON c.PLAN_ID = d.PLAN_ID and d.IS_DELETE=0" +
                " LEFT JOIN V_TaskRegion e ON c.PLAN_DETAIL_ID = e.PLAN_DETAIL_ID " +
                " where 1=1";
            if (!string.IsNullOrEmpty(year))
            {
                sql += " AND  d.JHND=" + year;
            }
            if (!string.IsNullOrEmpty(RWMC))
            {
                sql += " AND  a.RWMC='" + RWMC + "'";
            }
            if (!string.IsNullOrEmpty(RWBH))
            {
                sql += " AND  a.RWBH='" + RWBH + "'";
            }
            sql += " GROUP BY a.RWBH,a.RWMC,a.RWKSSJ,a.RWJSSJ,a.RWFW,a.TASK_ID,a.PLAN_DETAIL_ID)t";
            return db.GetDataTable(sql);
        }

        public string InsertLog(string mes, string remark)
        {
            string sql = "INSERT INTO ts_uidp_loginfo (ACCESS_TIME,LOG_CONTENT,REMARK)values('" + DateTime.Now.ToString("yyyyMMdd") + "','" + mes +
                " ','" + remark + "')";
            return db.ExecutByStringResult(sql);
        }
    }
}
