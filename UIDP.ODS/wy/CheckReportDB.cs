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
        /***
         * 下面是查询任务检查计划中的检查总表功能。查询基本逻辑如下：
         * 1.任务区域字段在年度计划明细中，所以要通过检查结果关联到年度计划明细表获得检查区域
         * 2.任务结果表(获取检查结果)→任务表(为了获取任务ID)→任务表和年度计划详情映射表(获取年度检查明细ID)
         * →年度计划明细表(获取执行的年度检查明细ID)→年度计划明细检查区域映射表(最终获取到任务的检查区域)→房屋表(房屋信息)→商户表(商户信息)
         * →任务区域视图(为了合并区域字段)→区域负责人表(为了保证和其他查询的数据一样)→字典表(获取房屋所属区域的中文名)
         ***/
        public DataTable GetCheckResult(string year, string FWBH, string RWMC, string JCJG,string DLID)
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
            if (!string.IsNullOrEmpty(DLID))
            {
                //这个条件是为了查询是存在指定大类不合格的检查结果
                sql += " AND EXISTS (SELECT DISTINCT m.* FROM wy_check_result_detail k" +
                    " JOIN wy_task_detail_config l ON k.DETAIL_CODE=l.Code" +
                    " JOIN wy_task_detail_config m ON l.ParentID=m.ID " +
                    " WHERE m.ID='" + DLID + "' AND k.RESULT_ID=a.RESULT_ID)";
            }
            sql += " GROUP BY a.JCJG,a.JCSJ,i.FZR,b.RWMC,b.RWBH,f.FWBH,f.FWMC,g.ZHXM,a.RESULT_ID,j.Name,a.JCCS,a.IS_REVIEW";
            sql += " ORDER BY b.RWBH,j.Name";
            return db.GetDataTable(sql);
        }

        /***
         * 下面的查询是查询检查结果详情。日常检查的详情允许某一些项不检查，所以需要从任务关联到检查结果。如果直接用检查结果则无法获取未检查的项。
         * 1.任务表→任务明细映射表→年度检查明细→检查明细地区映射表→检查详情配置表→检查详情配置表→检查结果表→检查结果详情
         * 
         ***/
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
        /***
         * 把检查结果和检查结果详情关联起来即可
         * 
         ***/
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
        /// <summary>
        /// 日期
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="FZR">负责人姓名</param>
        /// <param name="type">报表类型 0年度 1月度</param>
        /// <returns></returns>
        public DataTable WorkloadStatistics(string date,string FZR,int type=0)
        {
            string sql = "SELECT RD_ID,FZR,count( * ) AS JCCS,MONTH ( a.JCSJ ) AS mon,YEAR ( a.JCSJ ) AS yyyy,WX_OPEN_ID," +
                //下面子查询是查询超期任务次数
                " ( SELECT count( * ) AS overdue FROM wy_check_result c " +
                " JOIN wy_check_task d ON c.TASK_ID = d.TASK_ID" +
                " JOIN wy_region_director e ON c.JCR=e.WX_OPEN_ID" +
                " WHERE !( c.JCSJ BETWEEN d.RWKSSJ AND d.RWJSSJ)" +
                //子查询结束
                " AND e.RD_ID = b.RD_ID" +
                " AND MONTH(c.JCSJ)=mon" +
                " AND YEAR(c.JCSJ)=yyyy ) AS overdue" +
                " FROM wy_check_result a" +
                " JOIN wy_region_director b ON a.JCR = b.WX_OPEN_ID" +
                " JOIN wy_houseinfo c ON c.FWID=a.FWID" +
                " JOIN wy_check_task d ON a.TASK_ID=d.TASK_ID" +
                " WHERE 1=1 " +
                " {0}" +
                " GROUP BY b.FZR,MONTH ( a.JCSJ ),YEAR ( a.JCSJ ),RD_ID,WX_OPEN_ID" +
                " ORDER BY WX_OPEN_ID,FZR,YEAR ( a.JCSJ ),MONTH ( a.JCSJ )";
            string WhereCondition = string.Empty;//查询条件
            if (type == 0)//年度查询
            {
                if (string.IsNullOrEmpty(date))//日期为空查当年的
                {
                    WhereCondition += " AND YEAR(a.JCSJ)=" + DateTime.Now.Year;
                }
                else//否则查所选时间的年度报表
                {
                    WhereCondition += " AND YEAR(a.JCSJ)=" + Convert.ToDateTime(date).Year;
                }
            }
            else//月度报表
            {
                if (string.IsNullOrEmpty(date))//日期为空查本月的
                {
                    WhereCondition += " AND YEAR(a.JCSJ)=" + DateTime.Now.Year;
                    WhereCondition += " AND MONTH(a.JCSJ)=" + DateTime.Now.Month;
                }
                else//否则查所选时间的月度
                {
                    WhereCondition += " AND YEAR(a.JCSJ)=" + Convert.ToDateTime(date).Year;
                    WhereCondition += " AND MONTH(a.JCSJ)=" + Convert.ToDateTime(date).Month;
                }
            }
            if (!string.IsNullOrEmpty(FZR))
            {
                WhereCondition += " AND b.FZR like '%" + FZR + "%'";
            }
            sql = string.Format(sql, WhereCondition);
            return db.GetDataTable(sql);
        }

        public DataTable WorkloadStatisticsDetail(string RD_ID,string yyyy,string mon)
        {
            string sql = "select d.RWBH,d.RWMC,c.FWBH,c.FWMC,a.JCSJ,b.WX_OPEN_ID," +
                " (CASE a.JCJG WHEN 0 THEN '不合格' WHEN 1 THEN '合格' WHEN 2 THEN '复查不合格' WHEN 3 THEN '复查合格' END)AS JCJG," +
                " (CASE WHEN a.JCSJ BETWEEN d.RWKSSJ AND d.RWJSSJ THEN '否' ELSE '是' END ) as  overdue" +
                " FROM wy_check_result a" +
                " JOIN wy_region_director b ON a.JCR=b.WX_OPEN_ID " +
                " JOIN wy_houseinfo c ON c.FWID=a.FWID " +
                " JOIN wy_check_task d ON a.TASK_ID=d.TASK_ID" +
                " where MONTH(a.JCSJ)=" + mon + " AND YEAR(a.JCSJ)=" + yyyy + " AND b.RD_ID='" + RD_ID + "'" +
                " ORDER BY RWBH";
            return db.GetDataTable(sql);
        }

        public DataSet ShopCheckSummary(string SSQY,string FWBH,string FWMC,string date)
        {
            string MainSql = "select a.FWBH,a.FWMC {0}" +
                " FROM wy_houseinfo a" +
                " JOIN wy_check_result b ON a.FWID = b.FWID AND b.IS_DELETE = 0" +
                " JOIN wy_region_director c ON b.JCR = c.WX_OPEN_ID AND c.IS_DELETE = 0" +
                " JOIN wy_check_result_detail d ON b.RESULT_ID=d.RESULT_ID " +
                " JOIN wy_task_detail_config e ON d.DETAIL_CODE=e.`Code`" +
                " JOIN wy_task_detail_config f ON e.ParentID=f.ID" +
                " where {1}" +
                " GROUP BY a.FWBH,a.FWMC";
            DataTable CheckMainCategorydt = GetCheckMainCategory();
            string SelectColumn = string.Empty;
            foreach(DataRow dr in CheckMainCategorydt.Rows)
            {
                SelectColumn += ",SUM(CASE WHEN f.`Code`='" + dr["Code"] + "' AND d.CHECK_DETAIL_RESULT=0 THEN 1 ELSE 0 END) AS '" +dr["Code"].ToString().Replace(" ","")+"'";
            }
            string WhereCondition = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                WhereCondition = " YEAR(b.JCSJ)=" + date;
            }
            else
            {
                WhereCondition = " YEAR(b.JCSJ)=" + DateTime.Now.Year;
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                WhereCondition += " AND a.FWBH='" + FWBH + "'";
            }
            if (!string.IsNullOrEmpty(FWMC))
            {
                WhereCondition += " AND a.FWMC like '%" + FWMC + "%'";
            }
            if (!string.IsNullOrEmpty(SSQY))
            {
                WhereCondition += " AND a.SSQY='" + SSQY + "'";
            }
            MainSql = string.Format(MainSql, SelectColumn, WhereCondition);
            DataTable data = db.GetDataTable(MainSql);
            DataSet ds = new DataSet();
            ds.Tables.Add(CheckMainCategorydt);
            ds.Tables.Add(data);
            return ds;
        }

        public DataTable GetCheckMainCategory()
        {
            string sql = "select Code,Name from wy_task_detail_config where ParentID is null or ParentID=''";
            return db.GetDataTable(sql);
        }
    }
}
