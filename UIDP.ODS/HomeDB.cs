using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS
{
    public class HomeDB
    {
        DBTool db = new DBTool("MYSQL");
        /// <summary>
        /// 获取12个月中每个月的纳税人数
        /// </summary>
        /// <param name="orgcode">登录人系统编码</param>
        /// <param name="systime">当前系统时间</param>
        /// <returns></returns>
        public DataTable getMonthData(string orgcode,string systime)
        {
            int mm = Convert.ToDateTime(systime).Month;
            string sql = "SELECT  ";
            for (int i = 0; i < mm; i++)
            {
                sql += "SUM(CASE MONTH(S_WorkDate) WHEN "+(i+1)+ " THEN 1 ELSE 0 END) AS '" + (i+1) + "月'";
                if (i != mm - 1)
                {
                    sql += ",";
                }
            }
            sql += " FROM tax_salary WHERE YEAR(S_WorkDate)=YEAR('" + systime + "')";
            sql += " AND S_OrgCode LIKE'" + orgcode + "%'";
            return db.GetDataTable(sql);
        }
        /// <summary>
        /// 获取通知公告
        /// </summary>
        /// <returns></returns>
        public DataTable getNotice(string id)
        {
            string sql = "SELECT * FROM ts_store_notice WHERE IS_DELETE!=1 ";
            if (id != null && id != "")
            {
                sql += " AND NOTICE_ID='" + id + "'";
            }
            sql += " ORDER BY NOTICE_DATETIME";
            return db.GetDataTable(sql);
        }

        public DataTable getNoticeDetail(string id)
        {
            string sql= "select * from ts_store_notice_detail where NOTICE_ID='"+id + "' AND IS_DELETE!=1";
            return db.GetDataTable(sql);
        }

        public DataTable getThreshold()
        {
            string sql = "SELECT * FROM tax_subtractconfig ORDER BY TaxEnd Desc";
            return db.GetDataTable(sql);
        }
        /// <summary>
        /// 分档方法，按月查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="orgcode"></param>
        /// <param name="systime"></param>
        /// <returns></returns>
        //public DataTable getNum(DataTable dt, string orgcode, string systime)
        //{
        //    string YY = Convert.ToDateTime(systime).Year.ToString();
        //    DateTime sy = Convert.ToDateTime(systime);
        //    int mm = sy.Month;
        //    string sql = "";
        //    for (int i = 0; i < mm; i++)
        //    {
        //        string time = YY + "-" + (i + 1) + "-01";
        //        int j = 1;
        //        sql += "SELECT MONTH(S_WorkDate) AS MM, ";
        //        foreach (DataRow du in dt.Select())
        //        {
        //            if (du["TaxStart"].ToString() == "")
        //            {
        //                du["TaxStart"] = 0;
        //            }
        //            if (du["TaxEnd"].ToString() == "")
        //            {
        //                du["TaxEnd"] = 999999999999;
        //            }
        //            if (j != dt.Rows.Count)
        //            {
        //                sql += "SUM(CASE WHEN " + du["TaxStart"] + "<T_YFHJ AND T_YFHJ<=" + du["TaxEnd"] + " THEN 1 ELSE 0 END) AS '" + du["TaxRate"] + "',";
        //                j++;
        //            }
        //            else
        //            {
        //                sql += "SUM(CASE WHEN " + du["TaxStart"] + "<T_YFHJ AND T_YFHJ<=" + du["TaxEnd"] + " THEN 1 ELSE 0 END) AS '" + du["TaxRate"] + "'";
        //            }
        //        }
        //        sql += " FROM tax_salary ";
        //        sql += "WHERE S_OrgCode LIKE'" + orgcode + "%'";
        //        sql += " AND DATEDIFF(mm,S_WorkDate,'" + time + "')=0";
        //        sql += " GROUP BY MONTH(S_WorkDate) ";
        //        if (i != mm - 1)
        //        {
        //            sql += " UNION ALL ";
        //        }
        //    }
        //    return db.GetDataTable(sql);
        //}

        public DataTable getNum(string orgcode, string systime)
        {
            string YY = Convert.ToDateTime(systime).Year.ToString();
            DateTime sy = Convert.ToDateTime(systime);
            int mm = sy.Month;
            string sql = "SELECT a.TaxRate,";
            for(int i = 0; i < mm; i++)
            {
                sql += " SUM(CASE WHEN DATEDIFF(mm, S_WorkDate, '" + YY + "-" + (i + 1) + "-01')=0 THEN 1 ELSE 0 END) AS '" + (i + 1) + "月'";
                if (i != mm - 1)
                {
                    sql += ",";
                }
            }
            sql += " FROM tax_specialdeductions a";
            sql += " LEFT JOIN tax_taxpayerinfo b on a.S_WorkNumber=b.WorkerNumber";
            sql += " WHERE b.S_OrgCode LIKE'" + orgcode + "%'";
            sql += " AND DATEDIFF(YY,S_WorkDate,'" + systime + "')=0";
            sql += " GROUP BY a.TaxRate";
            return db.GetDataTable(sql);
        }

        //public DataTable CompareData(string orgcode, string systime)
        //{
        //    string YY = Convert.ToDateTime(systime).Year.ToString();
        //    DateTime sy = Convert.ToDateTime(systime);
        //    int mm = sy.Month;
        //    string sql = "";
        //    for(int i = 0; i < mm; i++)
        //    {
        //        sql += " SELECT MONTH( S_WorkDate ) AS mm,SUM ( K_KS ) AS KS,SUM ( T_DJJE ) AS DJ FROM View_MonthTaxStatistics WHERE S_WorkDate = '" + YY+"-"+(i+1)+"-01" + "'";
        //        sql += " AND S_OrgCode LIKE'" + orgcode + "%'";
        //        sql += " GROUP BY MONTH ( S_WorkDate )";
        //        if (i != mm - 1)
        //        {
        //            sql += " UNION ALL";
        //        }
        //    }
        //    return db.GetDataTable(sql);
        //}

        public DataTable CompareData(string orgcode, string systime)
        {
            string YY = Convert.ToDateTime(systime).Year.ToString();
            DateTime sy = Convert.ToDateTime(systime);
            int mm = sy.Month;
            string sql = "";
            for (int i = 0; i < mm; i++)
            {
                //sql += " SELECT MONTH( a.S_WorkDate ) AS mm,SUM ( a.AccumulatedTax) AS KS,SUM ( a.WithholdingTax) AS DJ FROM tax_specialdeductions a ";
                //sql += " LEFT JOIN tax_taxpayerinfo b on a.S_WorkNumber=b.WorkerNumber";
                //sql += " WHERE a.S_WorkDate = '" + YY + "-" + (i + 1) + "-01" + "'";
                //sql += " AND b.S_OrgCode LIKE'" + orgcode + "%'";
                //sql += " GROUP BY MONTH ( S_WorkDate )";
                sql += " SELECT MONTH ( a.S_WorkDate ) AS mm,";
                sql += " SUM(a.AccumulatedTax)-";//扣税金额，用本月累计的AccumulatedTax字段减去上个月的，1月份除外
                sql += " ( CASE MONTH('" + YY+"-"+(i+1)+"-01" + "') WHEN 1 THEN 0 ";//如果当月月份为1，则减0
                sql += " ELSE(SELECT SUM(a.AccumulatedTax) FROM tax_specialdeductions a";//如果当月月份不是1月，则查询上个月的数值，并相减
                sql += " LEFT JOIN tax_taxpayerinfo b ON a.S_WorkNumber= b.WorkerNumber ";
                sql += " WHERE DATEDIFF(mm,a.S_WorkDate,'" + YY + "-" + i + "-01" + "')=0";
                sql += " AND b.S_OrgCode LIKE'" + orgcode + "%') END) AS KS,";

                sql +=" SUM ( a.WithholdingTax ) -";//代缴金额，用本月累计的WithholdingTax字段减去上个月的，1月份除外
                sql +=" (CASE MONTH('" + YY + "-" + (i + 1) + "-01" + "') WHEN 1 THEN 0";//如果当月月份为1，则减0
                sql +=" ELSE ( SELECT SUM(a.WithholdingTax ) FROM tax_specialdeductions a";//如果当月月份不是1月，则查询上个月的数值，并相减
                sql += " LEFT JOIN tax_taxpayerinfo b ON a.S_WorkNumber= b.WorkerNumber";
                sql += " WHERE DATEDIFF(mm,a.S_WorkDate,'" + YY + "-" + i  + "-01" + "')=0";
                sql += " AND b.S_OrgCode LIKE'" + orgcode + "%') END) AS DJ";

                sql += " FROM tax_specialdeductions a";//合第一句sql为一个大整体，关联用户表查部门
                sql += " LEFT JOIN tax_taxpayerinfo b ON a.S_WorkNumber= b.WorkerNumber ";
                sql += " WHERE DATEDIFF(mm,a.S_WorkDate,'" + YY + "-" + (i + 1) + "-01" + "')=0";
                sql += " AND b.S_OrgCode LIKE'" + orgcode + "%'";
                sql += " GROUP BY MONTH(a.S_WorkDate )";
                if (i != mm - 1)
                {
                    sql += " UNION ALL";
                }
            }
            return db.GetDataTable(sql);
        }


        public DataTable GetRegionalStatistics()
        {
            string sql = "SELECT c.`Name`,count(*) AS NUM FROM wy_houseinfo a " +
                " JOIN wy_check_result b ON a.FWID=b.FWID AND a.IS_DELETE=0" +
                " JOIN tax_dictionary c ON a.SSQY=c.`Code` AND ParentCode='SSQY'" +
                " JOIN wy_region_director d ON b.JCR=d.WX_OPEN_ID AND d.IS_DELETE=0" +
                " where YEAR(b.JCSJ)=" +DateTime.Now.Year +
                " GROUP BY c.`Name`";
            return db.GetDataTable(sql);
        }

        public DataTable GetYearStatistics()
        {
            int year = DateTime.Now.Year;
            int mon = DateTime.Now.Month;
            string MainSql = "SELECT '{0}' {1} from wy_check_result a " +
                " JOIN wy_region_director b ON a.JCR=b.WX_OPEN_ID" +
                " {2}" +
                " GROUP BY '{0}'";
            string WhereCondition = " where YEAR(a.JCSJ)=" + year;
            string SelectColumn = string.Empty;
            for(int i = 1; i<= mon; i++)
            {
                SelectColumn += ",SUM(CASE WHEN MONTH(a.JCSJ)=" + i + " AND a.JCJG {0} THEN 1 ELSE 0 END) AS '" + i + "月'";
            }
            string TotalSql = string.Format(MainSql, "合格商户", string.Format(SelectColumn, "=1"), WhereCondition) +
                " UNION ALL " + string.Format(MainSql, "不合格商户", string.Format(SelectColumn, "!=1"), WhereCondition);
            return db.GetDataTable(TotalSql);
        }

        public DataTable YearHistogram()
        {
            string sql = "SELECT MONTH(PAY_TIME) AS mm," +
                " SUM(CASE WHEN TYPES_ID=0 THEN TOTAL_FEE ELSE 0 END )/100 AS WYF," +
                " SUM(CASE WHEN TYPES_ID=1 THEN TOTAL_FEE ELSE 0 END )/100 AS SF," +
                " SUM(CASE WHEN TYPES_ID=2 THEN TOTAL_FEE ELSE 0 END )/100 AS DF" +
                " FROM wy_wx_pay" +
                " WHERE YEAR(PAY_TIME)=" + DateTime.Now.Year +
                " GROUP BY MONTH(PAY_TIME)";
            return db.GetDataTable(sql);
        }
    }
}
