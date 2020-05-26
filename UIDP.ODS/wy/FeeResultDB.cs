using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class FeeResultDB
    {
        DBTool db = new DBTool("");
        public DataTable GetFeeResult(string JFLX, string FWMC,string FWBH,string JFSTATUS)
        {
            //string sql = "select a.*,b.FWMC,b.FWBH,c.SHOP_NAME,c.ZHXM,c.MOBILE_PHONE from wy_pay_record a " +
            //    " join wy_houseinfo b on a.FWID=b.FWID AND b.IS_DELETE=0" +
            //    " join wy_shopinfo c on b.CZ_SHID=c.CZ_SHID AND c.IS_DELETE=0" +
            //    " WHERE a.IS_DELETE=0 AND datediff(mm,'"+DateTime.Now+ "',a.CJSJ)=0";
            string sql = "SELECT a.*,b.FWMC,b.FWBH,c.ZHXM,c.MOBILE_PHONE FROM V_pay_record a " +
                " join wy_houseinfo b on a.FWID=b.FWID AND b.IS_DELETE=0" +
                " join wy_shopinfo c on b.CZ_SHID=c.CZ_SHID AND c.IS_DELETE=0" +
                " WHERE 1=1";
            if (!string.IsNullOrEmpty(JFSTATUS))
            {
                sql += " AND a.JFZT=" + JFSTATUS;
            }
            if (!string.IsNullOrEmpty(JFLX))
            {
                sql += " AND a.JFLX='" + JFLX + "'";
            }
            if (!string.IsNullOrEmpty(FWMC))
            {
                sql += " AND b.FWMC like'%" + FWMC + "%'";
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                sql += " AND b.FWBH='" + FWBH + "'";
            }
            return db.GetDataTable(sql);
        }
        public DataTable PaidFeeResult(string date)
        {
            string sql = "SELECT a.*,b.ORDER_ID,c.FWBH,c.FWMC,d.ZHXM,d.ZHXB,d.MOBILE_PHONE FROM wy_pay_record a" +
                " LEFT JOIN wy_wx_pay b ON a.RECORD_ID = b.RECORD_ID OR a.RECORD_ID = b.ID" +
                " JOIN wy_houseinfo c ON a.FWID=c.FWID AND c.IS_DELETE=0" +
                " JOIN wy_shopinfo d ON a.CZ_SHID=d.CZ_SHID AND d.IS_DELETE=0" +
                " where JFZT=1" +
                " {0}" +
                " ORDER BY JFRQ DESC";
            if (!string.IsNullOrEmpty(date))
            {
                sql = string.Format(sql, " AND DATE_FORMAT(a.JFRQ,'%Y-%m-%d')=DATE_FORMAT('" + date + "','%Y-%m-%d')");
            }
            else
            {
                sql = string.Format(sql, "");
            }
            return db.GetDataTable(sql);
        }
        public DataTable GetShopInfo(string FWID)
        {
            string sql = "select a.FWID,a.FWBH,a.FWMC,b.*,c.Name,a.FWID AS OLDID from wy_houseinfo a  " +
                " join wy_shopinfo b ON a.CZ_SHID=b.CZ_SHID" +
                " left join tax_dictionary c on a.LSFGS=c.Code and c.ParentCode='LSFGS'" +
                " where a.IS_DELETE=0 AND b.IS_DELETE=0 AND a.FWID='"+FWID+"'";
            //sql += " ORDER BY SHOP_ID OFFSET" + ((page - 1) * limit) + " rows fetch next " + limit + " rows only";
            return db.GetDataTable(sql);
        }

        public DataTable GetFeeInfoAndNotification()
        {
            //List<string> list = new List<string>();
            //string sql = "SELECT a.FWID," +
            //    " (CASE WHEN d.CZ_SHID is not null THEN d.CZ_SHID ELSE b.CZ_SHID END ) AS CZ_SHID," +
            //    " (CASE WHEN d.OPEN_ID is not null THEN d.OPEN_ID ELSE b.OPEN_ID END ) AS OPEN_ID," +
            //    " c.*,e.* FROM wy_houseinfo a" +
            //    " JOIN wy_shopinfo b on a.CZ_SHID=b.CZ_SHID AND b.IS_DELETE=0" +
            //    " JOIN wy_RopertyCosts c ON b.FEE_ID=c.FEE_ID AND c.IS_DELETE=0" +
            //    " LEFT JOIN wy_shopinfo d ON b.SUBLET_ID=d.CZ_SHID AND d.IS_DELETE=0 AND b.IS_SUBLET=1" +
            //    " LEFT JOIN V_pay_record e ON a.FWID= e.FWID AND e.JFLX=0";
            string sql = "SELECT FWID,CZ_SHID,OPEN_ID,FEE_ID,WYJFFS,WYJZSJ,WYJZ,JFZT,YXQS,YXQZ,recordFEE_ID,WYDJ," +
                " (CASE WHEN FEE_ID=recordFEE_ID THEN 1 ELSE 0 END ) AS  IS_CHANGEUSER" +
                " FROM " +
                " (SELECT a.FWID," +
                " ( CASE WHEN d.CZ_SHID IS NOT NULL THEN d.CZ_SHID ELSE b.CZ_SHID END ) AS CZ_SHID," +
                " ( CASE WHEN d.OPEN_ID IS NOT NULL THEN d.OPEN_ID ELSE b.OPEN_ID END ) AS OPEN_ID," +
                " c.FEE_ID,c.WYJFFS,c.WYJZSJ,c.WYJZ,e.JFZT,e.YXQS,e.YXQZ,e.FEE_ID AS recordFEE_ID,c.WYDJ " +
                " FROM wy_houseinfo a " +
                " JOIN wy_shopinfo b ON a.CZ_SHID= b.CZ_SHID AND b.IS_DELETE= 0 AND b.IS_PASS=1" +
                " JOIN wy_RopertyCosts c ON b.FEE_ID= c.FEE_ID AND c.IS_DELETE= 0" +
                " LEFT JOIN wy_shopinfo d ON b.SUBLET_ID= d.CZ_SHID AND d.IS_DELETE= 0 AND b.IS_SUBLET= 1" +
                " LEFT JOIN V_pay_record e ON a.FWID= e.FWID AND e.JFLX= 0 )t";
            return db.GetDataTable(sql);
        }

        public string CreateNotification(List<string> list)
        {
            return db.Executs(list);
        }
        public string DeleteRecord(string RECORD_ID)
        {
            return db.ExecutByStringResult("DELETE FROM wy_pay_record where RECORD_ID='" + RECORD_ID + "'");
        }
        public string ConfirmNotificationList(List<Dictionary<string,object>> datalist)
        {
            List<string> list = new List<string>();
            foreach(Dictionary<string,object> d in datalist)
            {
                string sql = "UPDATE wy_pay_record SET SFTZ=1 WHERE RECORD_ID='" + d["RECORD_ID"] + "'";
                list.Add(sql);
            }
            return db.Executs(list);
        }

        public string PushNotification(List<Dictionary<string, object>> datalist)
        {
            List<string> list = new List<string>();
            foreach (Dictionary<string, object> d in datalist)
            {
                string sql = "UPDATE wy_pay_record SET JFCS=(JFCS+1) " +
               " WHERE RECORD_ID='" + d["RECORD_ID"] + "'";
                list.Add(sql);
            }
            return db.Executs(list);
        }

        public DataTable GetHistoryFeeResult(string JFLX, string FWMC, string FWBH,string JFSTATUS)
        {
            string sql = "select a.*,b.FWMC,b.FWBH,c.SHOP_NAME,c.ZHXM,c.MOBILE_PHONE,d.ORDER_ID from wy_pay_record a" +
                " join wy_houseinfo b on a.FWID=b.FWID and b.IS_DELETE=0" +
                " join wy_shopinfo c on a.CZ_SHID=c.CZ_SHID AND c.IS_DELETE=0" +
                " LEFT JOIN wy_wx_pay d ON a.RECORD_ID = d.RECORD_ID OR a.RECORD_ID = d.ID" +
                " where 1=1";
            if (!string.IsNullOrEmpty(JFSTATUS))
            {
                sql += " AND a.JFZT=" + JFSTATUS + "";
            }
            if (!string.IsNullOrEmpty(JFLX))
            {
                sql += " AND a.JFLX='" + JFLX + "'";
            }
            if (!string.IsNullOrEmpty(FWMC))
            {
                sql += " AND b.FWMC like'%" + FWMC + "%'";
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                sql += " AND b.FWBH='" + FWBH + "'";
            }
            return db.GetDataTable(sql);
        }

        public DataTable GetBadFeeResult(string JFLX, string FWMC, string FWBH)
        {
            string sql = "select a.*,b.FWMC,b.FWBH,c.SHOP_NAME,c.ZHXM,c.MOBILE_PHONE from V_pay_record a" +
                " join wy_houseinfo b on a.FWID=b.FWID AND b.FWSX!=0" +
                " join wy_shopinfo c on a.CZ_SHID=c.CZ_SHID" +
                //" where a.JFZT=0 AND datediff(dd,GETDATE(),YXQS)<0 ";//SqlSever版本
                " where a.JFZT=0 AND datediff(sysdate(),YXQS)<0 ";//MySql版本
            if (!string.IsNullOrEmpty(JFLX))
            {
                sql += " AND a.JFLX='" + JFLX + "'";
            }
            if (!string.IsNullOrEmpty(FWMC))
            {
                sql += " AND b.FWMC like'%" + FWMC + "%'";
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                sql += " AND b.FWBH='" + FWBH + "'";
            }
            return db.GetDataTable(sql);
        }

        public string ConfirmFee(string RECORD_ID, int JFLX,string JFJE)
        {
            string sql = "UPDATE wy_pay_record SET JFZT=1,JFRQ='"+DateTime.Now.ToString("yyyyMMdd")+ "',PAY_WAY=0 {0}" +
                " WHERE RECORD_ID='" + RECORD_ID + "'";
            if (JFLX != 0)
            {
                sql = string.Format(sql, ",JFJE=" + JFJE);
            }
            else
            {
                sql = string.Format(sql, "");
            }
            return db.ExecutByStringResult(sql);
        }

        public string ConfirmReciveMoney(List<Dictionary<string, object>> datalist)
        {
            List<string> list = new List<string>();
            foreach (Dictionary<string, object> d in datalist)
            {
                string sql = "UPDATE wy_pay_record SET CONFIRM_RECIVEMONEY=1 WHERE RECORD_ID='" + d["RECORD_ID"] + "'";
                list.Add(sql);
            }
            return db.Executs(list);
        }

        public string PayOff(List<Dictionary<string, object>> datalist)
        {
            List<string> list = new List<string>();
            foreach (Dictionary<string, object> d in datalist)
            {
                string sql = "UPDATE wy_pay_record SET SFTZ=2 WHERE RECORD_ID='" + d["RECORD_ID"] + "'";
                list.Add(sql);
            }
            return db.Executs(list);
        }
        public DataTable GetShopMobilephone(string whereCondition)
        {
            string sql = "select distinct MOBILE_PHONE from wy_shopinfo where OPEN_ID in(" + whereCondition + ")";
            return db.GetDataTable(sql);
        }

        public string InsertLog(string mes,string remark)
        {
            string sql = "INSERT INTO ts_uidp_loginfo (ACCESS_TIME,LOG_CONTENT,REMARK)values('" + DateTime.Now.ToString("yyyyMMdd") + "','" + mes +
                " ','" + remark + "')";
            return db.ExecutByStringResult(sql);
        }

    }
}
