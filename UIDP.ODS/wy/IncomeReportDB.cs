using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class IncomeReportDB
    {
        DBTool db = new DBTool("");
        /***
         * 下面的方法是查询收入日报，查询逻辑较为复杂，逻辑如下：
         * 1.从微信支付凭证表取出各个商户交款总额，分成物业费、水费、电费、消防保证金、违约金、装修保证金等，后三个费用直接指定为0(后三个费用微信表没有)
         * 2.从记录表取出各个商户的交款总额，查询时只查询pay_way=0的即线下缴费总额，也是上述几个费用。后三个交款的日期就是商户表里面对应的费用的收款日期。
         * 3.将上述两个查询通过union all合成一个表。
         * 4.将合成的表看做一个表，SUM各个费用。
         * 5.将SUM成的表再看成一个表，做子查询查询物业有效期，先从微信标查有效起止日期，若为空，再查记录表的起止日期。关联条件是用户ID相等，付费时间相等。
         * tip:若有需求查询电费，将下面方法注释查询的列放出即可。
         ***/
        public DataTable GetWYIncomeReport(string date)
        {
            //微信收入查询sql(注释中的第一步)
            string WXIncomeSql = "SELECT " +
                " a.USER_ID," +
                " a.USER_NAME," +
                " a.SHOP_NAME," +
                " b.SHOPBH," +
                " SUM( CASE FEE_TYPES WHEN 0 THEN TOTAL_FEE ELSE 0 END ) / 100 AS WYF," +
                " SUM( CASE FEE_TYPES WHEN 1 THEN TOTAL_FEE ELSE 0 END ) / 100 AS SF," +
                //" SUM( CASE FEE_TYPES WHEN 2 THEN TOTAL_FEE ELSE 0 END ) / 100 AS DF," +
                " 0 AS ZXYJ," +
                " 0 AS XFBZJ," +
                " 0 AS WYBZJ," +
                " DATE_FORMAT( PAY_TIME, '%Y-%m-%d' ) AS payday" +
                " FROM wy_wx_pay a " +
                " join wy_shopinfo b on a.USER_ID=b.CZ_SHID AND b.IS_DELETE=0" +
                " WHERE PAY_TIME IS NOT NULL " +
                " GROUP BY a.USER_ID,a.USER_NAME,a.SHOP_NAME,DATE_FORMAT( PAY_TIME, '%Y-%m-%d' ),b.SHOPBH";
            //记录表查询sql(注释中的第二步)
            string RecordIncomeSql = "SELECT " +
                " a.CZ_SHID as USER_ID," +
                " b.ZHXM AS USER_NAME," +
                " b.SHOP_NAME," +
                " b.SHOPBH," +
                " SUM( CASE WHEN JFLX = 0 AND PAY_WAY = 0 THEN JFJE ELSE 0 END ) AS WYF," +
                " SUM( CASE WHEN JFLX = 1 AND PAY_WAY = 0 THEN JFJE ELSE 0 END ) AS SF," +
                //" SUM( CASE WHEN JFLX = 2 AND PAY_WAY = 0 THEN JFJE ELSE 0 END ) AS DF," +
                " MAX( ( CASE WHEN DATE_FORMAT( b.ZXYJJFSJ, '%Y-%m-%d' ) =DATE_FORMAT('" + date + "','%Y-%m-%d')  THEN ZXYJ ELSE 0 END ) ) AS ZXYJ," +
                " MAX( ( CASE WHEN DATE_FORMAT( b.XFBZJJFSJ, '%Y-%m-%d' ) =DATE_FORMAT('" + date + "','%Y-%m-%d')  THEN XFBZJJFSJ ELSE 0 END ) ) AS XFBZ," +
                " MAX( ( CASE WHEN DATE_FORMAT( b.WYBZJJFSJ, '%Y-%m-%d' ) =DATE_FORMAT('" + date + "','%Y-%m-%d')  THEN WYBZJJFSJ ELSE 0 END ) ) AS WYBZ," +
                " DATE_FORMAT( a.JFRQ, '%Y-%m-%d' ) AS payday" +
                " FROM wy_pay_record a" +
                " JOIN wy_shopinfo b ON a.CZ_SHID = b.CZ_SHID AND b.IS_DELETE = 0" +
                " WHERE PAY_WAY = 0 " +
                " GROUP BY a.CZ_SHID,b.ZHXM,b.SHOP_NAME,DATE_FORMAT( a.JFRQ, '%Y-%m-%d' ),b.SHOPBH";
            //上述的第三步,将大表sum
            string SUMSql = "SELECT " +
                " t.USER_ID," +
                " t.USER_NAME," +
                " t.SHOP_NAME," +
                " t.SHOPBH," +
                " SUM( t.WYF ) AS WYF," +
                " SUM( t.SF ) AS SF," +
                //" SUM( t.DF ) AS DF," +
                " SUM( t.ZXYJ ) AS ZXYJ," +
                " SUM( XFBZJ ) AS XFBZJ," +
                " SUM( WYBZJ ) AS WYBZJ," +
                " t.payday" +
                " FROM({0})t" +
                " GROUP BY t.USER_ID,t.USER_ID,t.USER_NAME,t.SHOP_NAME,t.payday,t.SHOPBH";
            SUMSql = string.Format(SUMSql, WXIncomeSql + " UNION ALL " + RecordIncomeSql);
            //最后一步，将物业费有有效起止日期查出来
            string TotalSql = "SELECT a.*,a.WYF+a.SF+a.ZXYJ+a.XFBZJ+a.WYBZJ as WYTotal," +
                //先从线上微信表查物业有效起止时间，若为空格去记录表查，都没有就是空
                " (CASE WHEN (SELECT HOUSE_SERVICEEFFCTIVE FROM wy_wx_pay " +
                " WHERE USER_ID = a.USER_ID AND DATE_FORMAT( PAY_TIME, '%Y-%m-%d' ) = DATE_FORMAT( a.payday, '%Y-%m-%d' )LIMIT 1)!=''" +
                " THEN (SELECT HOUSE_SERVICEEFFCTIVE FROM wy_wx_pay" +
                " WHERE USER_ID = a.USER_ID AND DATE_FORMAT( PAY_TIME, '%Y-%m-%d' ) = DATE_FORMAT( a.payday, '%Y-%m-%d' )LIMIT 1)" +
                " ELSE (SELECT CONCAT( DATE_FORMAT( YXQS, '%Y/%m/%d' ), '-', DATE_FORMAT( YXQZ, '%Y/%m/%d' ) ) FROM wy_pay_record" +
                " WHERE CZ_SHID = A.USER_ID AND DATE_FORMAT( JFRQ, '%Y-%m-%d' ) = DATE_FORMAT( a.payday, '%Y-%m-%d' ) LIMIT 1)" +
                " END ) AS YXQ FROM({0})a where a.payday=DATE_FORMAT('" + date + "','%Y-%m-%d')" +
                " AND(a.WYF!=0 OR a.SF!=0 OR a.ZXYJ!=0 OR a.XFBZJ!=0 OR WYBZJ!=0)";//这个条件是为了避免当天某个商户就交了电费导致普丰报表出现一条0的数据
            TotalSql = string.Format(TotalSql, SUMSql);
            return db.GetDataTable(TotalSql);
        }
        /***
         * 大体流程和上面的查询一样，只有违约保证金和电费，同时因为不需要查询物业有效期，可以少查询一层。
         ***/
        public DataTable GetPFIncomeReport(string date)
        {
            string WXIncomeSql = "SELECT " +
                " a.USER_ID," +
                " a.USER_NAME," +
                " a.SHOP_NAME," +
                " b.SHOPBH," +
                " SUM( CASE FEE_TYPES WHEN 2 THEN TOTAL_FEE ELSE 0 END ) / 100 AS DF," +
                " 0 as WYBZJ," +
                " DATE_FORMAT( PAY_TIME, '%Y-%m-%d' ) AS payday" +
                " FROM wy_wx_pay a " +
                " join wy_shopinfo b on a.USER_ID=b.CZ_SHID AND b.IS_DELETE=0" +
                " WHERE PAY_TIME IS NOT NULL " +
                " GROUP BY a.USER_ID,a.USER_NAME,a.SHOP_NAME,DATE_FORMAT( PAY_TIME, '%Y-%m-%d' ),b.SHOPBH";
            string RecordIncomeSql = "SELECT " +
               " a.CZ_SHID as USER_ID," +
               " b.ZHXM AS USER_NAME," +
               " b.SHOP_NAME," +
               " b.SHOPBH," +
               " SUM( CASE WHEN JFLX = 2 AND PAY_WAY = 0 THEN JFJE ELSE 0 END ) AS DF," +
               " MAX( ( CASE WHEN DATE_FORMAT( b.WYBZJJFSJ, '%Y-%m-%d' ) =DATE_FORMAT('" + date + "','%Y-%m-%d')  THEN WYBZJJFSJ ELSE 0 END ) ) AS WYBZJ," +
               " DATE_FORMAT( a.JFRQ, '%Y-%m-%d' ) AS payday" +
               " FROM wy_pay_record a" +
               " JOIN wy_shopinfo b ON a.CZ_SHID = b.CZ_SHID AND b.IS_DELETE = 0" +
               " WHERE PAY_WAY = 0 " +
               " GROUP BY a.CZ_SHID,b.ZHXM,b.SHOP_NAME,DATE_FORMAT( a.JFRQ, '%Y-%m-%d' ),b.SHOPBH";
            string SUMSql = "SELECT " +
                " t.USER_ID," +
                " t.USER_NAME," +
                " t.SHOP_NAME," +
                " t.SHOPBH," +
                " SUM( t.DF ) AS DF," +
                " SUM( t.WYBZJ ) AS WYBZJ," +
                " SUM( t.DF )+SUM( t.WYBZJ ) AS PFTotal," +
                " t.payday" +
                " FROM({0})t" +
                " WHERE t.payday=DATE_FORMAT('" +date+ "','%Y-%m-%d') AND (t.DF!=0 OR t.WYBZJ!=0)" +//这个条件是为了避免当天某个商户没交电费交的别的钱导致出现了0的数据
                " GROUP BY t.USER_ID,t.USER_ID,t.USER_NAME,t.SHOP_NAME,t.payday,t.SHOPBH";
            SUMSql = string.Format(SUMSql, WXIncomeSql + " UNION ALL " + RecordIncomeSql);
            return db.GetDataTable(SUMSql);
        }
    }
}
