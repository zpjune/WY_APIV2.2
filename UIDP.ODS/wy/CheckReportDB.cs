using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class CheckReportDB
    {
        DBTool db = new DBTool("");
        public DataTable GetCheckResult(string year, string FWBH, string RWMC, string JCJG,string DLID)
        {
            string sql = "select a.RESULT_ID,a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,g.ZHXM,GROUP_CONCAT(h.`Name`) AS JCQY,j.Name,a.JCCS,a.IS_REVIEW" +
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
            if (!string.IsNullOrEmpty(DLID))
            {
                sql += " AND EXISTS (SELECT DISTINCT m.* FROM wy_check_result_detail k" +
                    " JOIN wy_task_detail_config l ON k.DETAIL_CODE=l.Code" +
                    " JOIN wy_task_detail_config m ON l.ParentID=m.ID " +
                    " WHERE m.ID='" + DLID + "' AND k.RESULT_ID=a.RESULT_ID)";
            }
            sql += " GROUP BY a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,g.ZHXM,a.RESULT_ID,j.Name,a.JCCS,a.IS_REVIEW";
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
                " LEFT JOIN wy_check_result_detail h ON f.`Code`=h.DETAIL_CODE AND h.RESULT_ID=g.RESULT_ID" +
                " AND g.RESULT_ID='" + RESULT_ID + "' ORDER BY e.`Name`";
            return db.GetDataTable(sql);
        }

        public DataTable GetParentCheckCodeOptions()
        {
            return db.GetDataTable("select ID,Name from wy_task_detail_config where ParentID IS NULL OR ParentID=''");
        }

        public DataTable ExportTotalCheckReport(string year, string FWBH, string RWMC, string JCJG, string DLID)
        {
            string TotalSql = "SELECT * FROM ({0})t " +
                " join ({1})tt on t.RESULT_ID=tt.RESULT_ID" +
                " ORDER BY t.RWMC,t.FWBH,tt.DL";
            string CheckResult = "select a.RESULT_ID,a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,g.ZHXM,GROUP_CONCAT(h.`Name`) AS JCQY,j.Name AS FWQY,a.JCCS,a.IS_REVIEW" +
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
                CheckResult += " AND e.JHND='" + year + "'";
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                CheckResult += " AND e.FWBH='" + FWBH + "'";
            }
            if (!string.IsNullOrEmpty(RWMC))
            {
                CheckResult += " AND b.RWMC like'%" + RWMC + "%'";
            }
            if (!string.IsNullOrEmpty(JCJG))
            {
                CheckResult += " AND a.JCJG=" + JCJG;
            }
            if (!string.IsNullOrEmpty(DLID))
            {
                CheckResult += " AND EXISTS (SELECT DISTINCT m.* FROM wy_check_result_detail k" +
                    " JOIN wy_task_detail_config l ON k.DETAIL_CODE=l.Code" +
                    " JOIN wy_task_detail_config m ON l.ParentID=m.ID " +
                    " WHERE m.ID='" + DLID + "' AND k.RESULT_ID=a.RESULT_ID)";
            }
            CheckResult += " GROUP BY a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,g.ZHXM,a.RESULT_ID,j.Name,a.JCCS,a.IS_REVIEW";
            // CheckResult += " ORDER BY b.RWBH,j.Name";

            string CheckDetail = "SELECT DISTINCT h.RESULT_ID,e.`Name` AS DL,f.`Name` AS XL," +
                " (CASE h.CHECK_DETAIL_RESULT WHEN 0 THEN '不合格' WHEN  1 THEN '合格' ELSE '未检查'END ) AS result" +
                " FROM wy_check_task a " +
                " JOIN wy_map_checkplandetail b ON a.TASK_ID=b.TASK_ID" +
                " JOIN wy_checkplan_detail c ON b.PLAN_DETAIL_ID=c.PLAN_DETAIL_ID" +
                " JOIN wy_map_region d ON c.PLAN_DETAIL_ID=d.PLAN_DETAIL_ID" +
                " JOIN wy_task_detail_config e ON e.`Code`=c.JCLX AND e.ParentID is NULL" +
                " JOIN wy_task_detail_config f ON f.ParentID=e.ID" +
                " JOIN wy_check_result g ON a.TASK_ID=g.TASK_ID" +
                " LEFT JOIN wy_check_result_detail h ON f.`Code`=h.DETAIL_CODE AND h.RESULT_ID=g.RESULT_ID";
            TotalSql = string.Format(TotalSql, CheckResult, CheckDetail);
            return db.GetDataTable(TotalSql);
        }
    }
}
