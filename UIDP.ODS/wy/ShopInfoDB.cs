using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class ShopInfoDB
    {
        DBTool db = new DBTool("");
        public DataTable GetShopInfo(string ORG_CODE,string ZHXM,string IS_PASS,string FWSX,string CZ_SHID,string FWBH)
        {
            string sql = "select a.FWID,a.FWBH,a.FWMC,b.*,c.Name,a.FWID AS OLDID from wy_houseinfo a  " +
                " join wy_shopinfo b ON a.CZ_SHID=b.CZ_SHID" +
                " left join tax_dictionary c on a.LSFGS=c.Code and c.ParentCode='LSFGS'" +
                " where a.IS_DELETE=0 AND b.IS_DELETE=0 AND a.ORG_CODE LIKE'"+ ORG_CODE+"%'";
            if (!string.IsNullOrEmpty(FWBH))
            {
                sql += " AND a.FWBH='" + FWBH+"' ";
            }
            if (!string.IsNullOrEmpty(FWSX))
            {
                sql += " AND a.FWSX=" + FWSX;
            }
            if (!string.IsNullOrWhiteSpace(IS_PASS))
            {
                sql += " AND IS_PASS='" + IS_PASS + "'";
            }
            if (!string.IsNullOrEmpty(ZHXM))
            {
                sql += " AND ZHXM='" + ZHXM + "'";
            }
            if (!string.IsNullOrEmpty(CZ_SHID))
            {
                sql += " AND b.CZ_SHID='" + CZ_SHID + "'";
            }
            //sql += " ORDER BY SHOP_ID OFFSET" + ((page - 1) * limit) + " rows fetch next " + limit + " rows only";
            return db.GetDataTable(sql);
        }

        public DataTable GetShopInfoDetail(string CZ_SHID)
        {
            string sql = "select a.FWID,a.FWBH,a.FWMC,a.JZMJ,a.ZLWZ,b.*,c.Name,a.FWID AS OLDID,d.LEASE_ID,d.ZLKSSJ,d.ZLZZSJ,d.ZLZE,d.ZLYJ,d.ZLYS,d.ZJJFFS,e.FEE_ID,e.WYJFFS,e.WYJZSJ,e.WYJZ,e.WYDJ,g.Name AS NAME1,h.Name AS NAME2," +
                " f.CZ_SHID AS CZ_SHID1,f.JYNR AS JYNR1,f.ZHXM AS ZHXM1,f.ZHXB AS ZHXB1,f.SFZH AS SFZH1,f.MOBILE_PHONE AS MOBILE_PHONE1,f.TELEPHONE AS TELEPHONE1,f.E_MAIL AS E_MAIL1,f.SHOPBH,f.SHOP_NAME,f.ZHLX" +
                " from wy_houseinfo a " +
                " join wy_shopinfo b ON a.CZ_SHID=b.CZ_SHID" +
                " left join tax_dictionary c on a.LSFGS=c.Code and c.ParentCode='LSFGS'" +
                " left join wy_Leasinginfo d on b.LEASE_ID=d.LEASE_ID and d.IS_DELETE=0" +
                " left join wy_RopertyCosts e on b.FEE_ID=e.FEE_ID AND e.IS_DELETE=0" +
                " left join wy_shopinfo f on b.SUBLET_ID=f.CZ_SHID AND b.IS_SUBLET=1 AND f.IS_DELETE=0" +
                " left join tax_dictionary g on g.Code=d.ZJJFFS AND g.ParentCode='PAY_WAY'" +
                " left join tax_dictionary h on h.Code=e.WYJFFS AND h.ParentCode='PAY_WAY'" +
                " where a.IS_DELETE=0 AND b.IS_DELETE=0 AND b.CZ_SHID='" + CZ_SHID + "'";
            return db.GetDataTable(sql);
        }

        public string DeleteShopInfo(string CZ_SHID,string FWID)
        {
            string ShopSql = " UPDATE wy_shopinfo set IS_DELETE=1 where CZ_SHID='" + CZ_SHID + "'";
            string HouseSql = " UPDATE wy_houseinfo SET CZ_SHID=NULL,FWSX=0 WHERE FWID='" + FWID + "'";
            string FeeSql = "UPDATE wy_RopertyCosts SET IS_DELETE=1 WHERE FEE_ID in(SELECT FEE_ID FROM wy_shopinfo WHERE CZ_SHID='" + CZ_SHID + "')";
            string LeaseSql = " UPDATE wy_Leasinginfo SET IS_DELETE=1 WHERE LEASE_ID in (SELECT LEASE_ID FROM wy_shopinfo WHERE CZ_SHID='" + CZ_SHID + "')";
            List<string> list = new List<string>()
            {
                { ShopSql },
                { HouseSql },
                { FeeSql},
                { LeaseSql}
            };
            return db.Executs(list); ;
        }

        public string CreateShopInfo(Dictionary<string, object> d)
        {
            List<string> list = new List<string>();
            string CZ_SHID = Guid.NewGuid().ToString();//商户唯一ID
            string FEE_ID = Guid.NewGuid().ToString();//物业费ID
            string LEASE_ID = Guid.NewGuid().ToString();//租赁信息ID
            string SUBLET_ID = Guid.NewGuid().ToString();//转租用户的ID
            string DateTime = System.DateTime.Now.ToString("yyyyMMdd");//获取时间
            string SuletSql = string.Empty;//转租语句
            string LeaseSql = string.Empty;//租赁语句
            string FeeSql = string.Empty;//物业费语句
            string ShopInfoSql = string.Empty;//商户插入语句
            string HouseUpdateSql = string.Empty;//房屋更新语句
            if (d["userType"].ToString() == "1")//出租商户
            {
                //租赁信息语句
                LeaseSql = "INSERT INTO wy_Leasinginfo(LEASE_ID,ZLKSSJ,ZLZZSJ,ZLZE,ZLYJ,ZLYS,ZJJFFS,CJR,CJSJ,IS_DELETE,CZ_SHID)VALUES(";
                LeaseSql += GetSqlStr(LEASE_ID);
                LeaseSql += GetSqlStr(d["ZLKSSJ"]);
                LeaseSql += GetSqlStr(d["ZLZZSJ"]);
                LeaseSql += GetSqlStr(d["ZLZE"], 1);
                LeaseSql += GetSqlStr(d["ZLYJ"], 1);
                LeaseSql += GetSqlStr(d["ZLYS"], 1);
                LeaseSql += GetSqlStr(d["ZJJFFS"]);
                LeaseSql += GetSqlStr(d["userId"]);
                LeaseSql += GetSqlStr(DateTime);
                LeaseSql += GetSqlStr(0, 1);
                LeaseSql+= GetSqlStr(CZ_SHID);
                LeaseSql = LeaseSql.TrimEnd(',') + ")";
                list.Add(LeaseSql);
                //租户信息插入语句
                ShopInfoSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE,IS_SUBLET,TELEPHONE,E_MAIL," +
                   "IS_PASS,CJR,CJSJ,SHOP_NAME,SHOPBH,ZHLX,LEASE_ID,FEE_ID,IS_DELETE,FWID,SHOP_STATUS," +
                   "ZXYJ,ZXYJJFSJ,ZXYJTFSJ,XFBZJ,XFBZJJFSJ,XFBZJTFSJ,OPEN_ID,WYBZJ,WYBZJJFSJ,WYBZJTFSJ)values(";
                ShopInfoSql += GetSqlStr(CZ_SHID);
                ShopInfoSql += GetSqlStr(d["JYNR"]);
                ShopInfoSql += GetSqlStr(d["ZHXM"]);
                ShopInfoSql += GetSqlStr(d["ZHXB"], 1);
                ShopInfoSql += GetSqlStr(d["SFZH"]);
                ShopInfoSql += GetSqlStr(d["MOBILE_PHONE"]);
                ShopInfoSql += GetSqlStr(0,1);
                ShopInfoSql += GetSqlStr(d["TELEPHONE"]);
                ShopInfoSql += GetSqlStr(d["E_MAIL"]);
                ShopInfoSql += GetSqlStr(0, 1);
                ShopInfoSql += GetSqlStr(d["userId"]);
                ShopInfoSql += GetSqlStr(DateTime);
                ShopInfoSql += GetSqlStr(d["SHOP_NAME"]);
                ShopInfoSql += GetSqlStr(d["SHOPBH"]);
                ShopInfoSql += GetSqlStr(d["ZHLX"], 1);
                ShopInfoSql += GetSqlStr(LEASE_ID);
                ShopInfoSql += GetSqlStr(FEE_ID);
                ShopInfoSql += GetSqlStr(0, 1);
                ShopInfoSql += GetSqlStr(d["FWID"]);
                ShopInfoSql += GetSqlStr(d["userType"],1);
                ShopInfoSql += GetSqlStr(d["ZXYJ"],1);
                ShopInfoSql += GetSqlStr(d["ZXYJJFSJ"]);
                ShopInfoSql += GetSqlStr(d["ZXYJTFSJ"]);
                ShopInfoSql += GetSqlStr(d["XFBZJ"],1);
                ShopInfoSql += GetSqlStr(d["XFBZJJFSJ"]);
                ShopInfoSql += GetSqlStr(d["XFBZJTFSJ"]);
                ShopInfoSql += "(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE"] + "' LIMIT 1)t),";
                ShopInfoSql += GetSqlStr(d["WYBZJ"], 1);
                ShopInfoSql += GetSqlStr(d["WYBZJJFSJ"]);
                ShopInfoSql += GetSqlStr(d["WYBZJTFSJ"]);
                ShopInfoSql = ShopInfoSql.TrimEnd(',') + ")";
                list.Add(ShopInfoSql);
                //房屋信息更新语句
                HouseUpdateSql = "UPDATE wy_houseinfo set FWSX=" + d["userType"] + ",CZ_SHID='" + CZ_SHID + "' WHERE FWID='" + d["FWID"] + "'";
                list.Add(HouseUpdateSql);
            }
            else if(d["userType"].ToString() == "2")//出售用户
            {
                ShopInfoSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE,IS_SUBLET,SUBLET_ID,TELEPHONE,E_MAIL," +
                   "IS_PASS,CJR,CJSJ,SHOP_NAME,SHOPBH,ZHLX,FEE_ID,IS_DELETE,FWID,SHOP_STATUS," +
                   "ZXYJ,ZXYJJFSJ,ZXYJTFSJ,XFBZJ,XFBZJJFSJ,XFBZJTFSJ,OPEN_ID,WYBZJ,WYBZJJFSJ,WYBZJTFSJ)values(";
                ShopInfoSql += GetSqlStr(CZ_SHID);
                ShopInfoSql += GetSqlStr(d["JYNR"]);
                ShopInfoSql += GetSqlStr(d["ZHXM"]);
                ShopInfoSql += GetSqlStr(d["ZHXB"], 1);
                ShopInfoSql += GetSqlStr(d["SFZH"]);
                ShopInfoSql += GetSqlStr(d["MOBILE_PHONE"]);
                ShopInfoSql += GetSqlStr(d["IS_SUBLET"]);
                if (d["IS_SUBLET"].ToString() == "1")
                {
                    ShopInfoSql += GetSqlStr(SUBLET_ID);
                    //转租语句
                    SuletSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE,IS_SUBLET,TELEPHONE,E_MAIL," +
                      "IS_PASS,CJR,CJSJ,SHOP_NAME,SHOPBH,ZHLX,IS_DELETE,FWID,OPEN_ID,SHOP_STATUS)values(";
                    SuletSql += GetSqlStr(SUBLET_ID);
                    SuletSql += GetSqlStr(d["JYNR1"]);
                    SuletSql += GetSqlStr(d["ZHXM1"]);
                    SuletSql += GetSqlStr(d["ZHXB1"], 1);
                    SuletSql += GetSqlStr(d["SFZH1"]);
                    SuletSql += GetSqlStr(d["MOBILE_PHONE1"]);
                    SuletSql += GetSqlStr(3, 1);
                    SuletSql += GetSqlStr(d["TELEPHONE1"]);
                    SuletSql += GetSqlStr(d["E_MAIL1"]);
                    SuletSql += GetSqlStr(0, 1);
                    SuletSql += GetSqlStr(d["userId"]);
                    SuletSql += GetSqlStr(DateTime);
                    SuletSql += GetSqlStr(d["SHOP_NAME1"]);
                    SuletSql += GetSqlStr(d["SHOPBH1"]);
                    SuletSql += GetSqlStr(d["ZHLX1"], 1);
                    SuletSql += GetSqlStr(0, 1);
                    SuletSql += GetSqlStr(d["FWID"]);
                    SuletSql+= "(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE1"] + "' LIMIT 1)t),";
                    SuletSql += GetSqlStr(5, 1);
                    SuletSql = SuletSql.TrimEnd(',') + ")";
                    list.Add(SuletSql);
                }
                else
                {
                    ShopInfoSql += GetSqlStr("");
                }
                ShopInfoSql += GetSqlStr(d["TELEPHONE"]);
                ShopInfoSql += GetSqlStr(d["E_MAIL"]);
                ShopInfoSql += GetSqlStr(0, 1);
                ShopInfoSql += GetSqlStr(d["userId"]);
                ShopInfoSql += GetSqlStr(DateTime);
                ShopInfoSql += GetSqlStr(d["SHOP_NAME"]);
                ShopInfoSql += GetSqlStr(d["SHOPBH"]);
                ShopInfoSql += GetSqlStr(d["ZHLX"], 1);
                //ShopInfoSql += GetSqlStr(LEASE_ID);
                ShopInfoSql += GetSqlStr(FEE_ID);
                ShopInfoSql += GetSqlStr(0, 1);
                ShopInfoSql += GetSqlStr(d["FWID"]);
                ShopInfoSql += GetSqlStr(d["userType"],1);
                ShopInfoSql += GetSqlStr(d["ZXYJ"], 1);
                ShopInfoSql += GetSqlStr(d["ZXYJJFSJ"]);
                ShopInfoSql += GetSqlStr(d["ZXYJTFSJ"]);
                ShopInfoSql += GetSqlStr(d["XFBZJ"], 1);
                ShopInfoSql += GetSqlStr(d["XFBZJJFSJ"]);
                ShopInfoSql += GetSqlStr(d["XFBZJTFSJ"]);
                ShopInfoSql += "(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE"] + "' LIMIT 1)t),";
                ShopInfoSql += GetSqlStr(d["WYBZJ"], 1);
                ShopInfoSql += GetSqlStr(d["WYBZJJFSJ"]);
                ShopInfoSql += GetSqlStr(d["WYBZJTFSJ"]);
                ShopInfoSql = ShopInfoSql.TrimEnd(',') + ")";
                list.Add(ShopInfoSql);
                HouseUpdateSql = "UPDATE wy_houseinfo set FWSX=" + d["userType"] + ",CZ_SHID='" + CZ_SHID + "' WHERE FWID='" + d["FWID"] + "'";
                list.Add(HouseUpdateSql);
            }
            else
            {
                throw new Exception("未检测到正确的用户类型！");
            }
            FeeSql = "INSERT INTO wy_RopertyCosts (FEE_ID,WYJFFS,WYJZSJ,WYJZ,IS_DELETE,WYDJ,CZ_SHID)VALUES(";
            FeeSql += GetSqlStr(FEE_ID);
            FeeSql += GetSqlStr(d["WYJFFS"]);
            FeeSql += GetSqlStr(d["WYJZSJ"]);
            FeeSql += "(SELECT JZMJ* " + d["WYDJ"] +
                " FROM wy_houseinfo where FWID='" + d["FWID"] + "'),";
            FeeSql += GetSqlStr(0, 1);
            FeeSql += GetSqlStr(d["WYDJ"], 1);
            FeeSql += GetSqlStr(CZ_SHID);
            FeeSql = FeeSql.TrimEnd(',') + ")";
            list.Add(FeeSql);

            return db.Executs(list);

        }

        public string UpdateShopInfo(Dictionary<string, object> d)
        {
            List<string> list = new List<string>();
            if (d["FWID"].ToString() != d["OLDID"].ToString())
            {
                //回滚旧房屋状态Sql
                string RollBackSql = "update wy_houseinfo set FWSX=0,CZ_SHID=null where FWID='" + d["OLDID"] + "'";
                           
                list.Add(RollBackSql);
                
            }
            string UpdateHouseSql = "update wy_houseinfo set FWSX=" + d["userType"] + ",CZ_SHID='" + d["CZ_SHID"] + "' WHERE FWID='" + d["FWID"] + "'";
            list.Add(UpdateHouseSql);
            if (d["userType"].ToString() == "1")//出租用户语句
            {
                /***
                 * 出租用户涉及到4个表，房屋表、商户表、租赁信息表以及物业表。
                 * 第一步判断房屋是否变动，生成房屋信息回滚语句和更新语句
                 * 第二步插入商户信息，租赁信息以及物业费信息。
                 * 综上所述，若更换了绑定房屋，则产生5个语句，若未更换房屋，则生成4个。
                 ***/
                //修改租赁信息
                string LeaseSql = "UPDATE wy_Leasinginfo SET ZLKSSJ=" + GetSqlStr(d["ZLKSSJ"]);
                LeaseSql += "ZLZZSJ=" + GetSqlStr(d["ZLZZSJ"]);
                LeaseSql += "ZLZE=" + GetSqlStr(d["ZLZE"], 1);
                LeaseSql += "ZLYJ=" + GetSqlStr(d["ZLYJ"], 1);
                LeaseSql += "ZLYS=" + GetSqlStr(d["ZLYS"], 1);
                LeaseSql += "ZJJFFS=" + GetSqlStr(d["ZJJFFS"]);
                LeaseSql += "BJR=" + GetSqlStr(d["userId"]);
                LeaseSql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                
                LeaseSql = LeaseSql.TrimEnd(',') + " WHERE LEASE_ID='" + d["LEASE_ID"] + "'";
                list.Add(LeaseSql);
                //修改商户信息
                string ShopInfoSql = "UPDATE wy_shopinfo SET JYNR=" + GetSqlStr(d["JYNR"]);
                ShopInfoSql += "ZHXM=" + GetSqlStr(d["ZHXM"]);
                ShopInfoSql += "ZHXB=" + GetSqlStr(d["ZHXB"], 1);
                ShopInfoSql += "SFZH=" + GetSqlStr(d["SFZH"]);
                ShopInfoSql += "MOBILE_PHONE=" + GetSqlStr(d["MOBILE_PHONE"]);
                ShopInfoSql += "IS_SUBLET=" + GetSqlStr(d["IS_SUBLET"]);
                ShopInfoSql += "TELEPHONE=" + GetSqlStr(d["TELEPHONE"]);
                ShopInfoSql += "E_MAIL=" + GetSqlStr(d["E_MAIL"]);
                ShopInfoSql += "BJR=" + GetSqlStr(d["userId"]);
                ShopInfoSql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                ShopInfoSql += "SHOP_NAME=" + GetSqlStr(d["SHOP_NAME"]);
                ShopInfoSql += "SHOPBH=" + GetSqlStr(d["SHOPBH"]);
                ShopInfoSql += "ZHLX=" + GetSqlStr(d["ZHLX"], 1);
                ShopInfoSql += "SHOP_STATUS=" + GetSqlStr(d["userType"], 1);
                ShopInfoSql += "ZXYJ="+GetSqlStr(d["ZXYJ"], 1);
                ShopInfoSql += "ZXYJJFSJ="+GetSqlStr(d["ZXYJJFSJ"]);
                ShopInfoSql += "ZXYJTFSJ="+GetSqlStr(d["ZXYJTFSJ"]);
                ShopInfoSql += "XFBZJ="+GetSqlStr(d["XFBZJ"], 1);
                ShopInfoSql += "XFBZJJFSJ="+GetSqlStr(d["XFBZJJFSJ"]);
                ShopInfoSql += "XFBZJTFSJ=" + GetSqlStr(d["XFBZJTFSJ"]);
                ShopInfoSql += "OPEN_ID=(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE"] + "' LIMIT 1)t),";
                ShopInfoSql += "WYBZJ=" + GetSqlStr(d["WYBZJ"], 1);
                ShopInfoSql += "WYBZJJFSJ=" + GetSqlStr(d["WYBZJJFSJ"]);
                ShopInfoSql += "WYBZJTFSJ=" + GetSqlStr(d["WYBZJTFSJ"]);
                ShopInfoSql = ShopInfoSql.TrimEnd(',') + " WHERE CZ_SHID='" + d["CZ_SHID"] + "'";
                list.Add(ShopInfoSql);
            }
            else if (d["userType"].ToString() == "2")
            {
                /***
                 * 出售用户涉及到3个表，房屋表、商户表以及物业表。
                 * 第一步判断房屋是否转租，可能生成两条插入商户表的语句
                 * 第二步插入物业信息 物业信息绑定在出售商户的身上。
                 * 综上所述，若更换了绑定房屋，则产生4个语句，若未更换房屋，则生成3个。
                 ***/
                string SUBLET_ID = "";
                //转租用户语句
                string SuletSql = string.Empty;
                if (d["IS_SUBLET"].ToString() == "1")
                {
                    if(d["CZ_SHID1"] != null && d["CZ_SHID1"].ToString() != "")
                    {
                        SuletSql = "UPDATE wy_shopinfo SET JYNR=" + GetSqlStr(d["JYNR1"]);
                        SuletSql += "ZHXM=" + GetSqlStr(d["ZHXM1"]);
                        SuletSql += "ZHXB=" + GetSqlStr(d["ZHXB1"], 1);
                        SuletSql += "SFZH=" + GetSqlStr(d["SFZH1"]);
                        SuletSql += "MOBILE_PHONE=" + GetSqlStr(d["MOBILE_PHONE1"]);
                        SuletSql += "IS_SUBLET=" + GetSqlStr(0,1);
                        SuletSql += "TELEPHONE=" + GetSqlStr(d["TELEPHONE1"]);
                        SuletSql += "E_MAIL=" + GetSqlStr(d["E_MAIL1"]);
                        SuletSql += "BJR=" + GetSqlStr(d["userId"]);
                        SuletSql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                        SuletSql += "SHOP_NAME=" + GetSqlStr(d["SHOP_NAME1"]);
                        SuletSql += "SHOPBH=" + GetSqlStr(d["SHOPBH1"]);
                        SuletSql += "ZHLX=" + GetSqlStr(d["ZHLX1"], 1);
                        SuletSql += "OPEN_ID=(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE1"] + "' LIMIT 1)t),";
                        SuletSql = SuletSql.TrimEnd(',') + " WHERE CZ_SHID='" + d["CZ_SHID1"] + "'";
                    }
                    else
                    {
                        SUBLET_ID = Guid.NewGuid().ToString();
                        SuletSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE,IS_SUBLET,TELEPHONE,E_MAIL," +
                  "IS_PASS,CJR,CJSJ,SHOP_NAME,SHOPBH,ZHLX,IS_DELETE,OPEN_ID)values(";
                        SuletSql += GetSqlStr(SUBLET_ID);
                        SuletSql += GetSqlStr(d["JYNR1"]);
                        SuletSql += GetSqlStr(d["ZHXM1"]);
                        SuletSql += GetSqlStr(d["ZHXB1"], 1);
                        SuletSql += GetSqlStr(d["SFZH1"]);
                        SuletSql += GetSqlStr(d["MOBILE_PHONE1"]);
                        SuletSql += GetSqlStr(3, 1);
                        SuletSql += GetSqlStr(d["TELEPHONE1"]);
                        SuletSql += GetSqlStr(d["E_MAIL1"]);
                        SuletSql += GetSqlStr(0, 1);
                        SuletSql += GetSqlStr(d["userId"]);
                        SuletSql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                        SuletSql += GetSqlStr(d["SHOP_NAME1"]);
                        SuletSql += GetSqlStr(d["SHOPBH1"]);
                        SuletSql += GetSqlStr(d["ZHLX1"], 1);
                        SuletSql += GetSqlStr(0, 1);
                        SuletSql += "(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE1"] + "' LIMIT 1)t)";
                        SuletSql = SuletSql.TrimEnd(',') + ")";
                    }
                    list.Add(SuletSql);
                }
                string ShopInfoSql = "UPDATE wy_shopinfo SET JYNR=" + GetSqlStr(d["JYNR"]);
                ShopInfoSql += "ZHXM=" + GetSqlStr(d["ZHXM"]);
                ShopInfoSql += "ZHXB=" + GetSqlStr(d["ZHXB"], 1);
                ShopInfoSql += "SFZH=" + GetSqlStr(d["SFZH"]);
                ShopInfoSql += "MOBILE_PHONE=" + GetSqlStr(d["MOBILE_PHONE"]);
                ShopInfoSql += "IS_SUBLET=" + GetSqlStr(d["IS_SUBLET"]);
                if (!string.IsNullOrEmpty(SUBLET_ID) && d["IS_SUBLET"].ToString() == "1")
                {
                    ShopInfoSql += "SUBLET_ID=" + GetSqlStr(SUBLET_ID);
                }
                else
                {
                    ShopInfoSql += "SUBLET_ID=" + GetSqlStr("");
                }
                ShopInfoSql += "TELEPHONE=" + GetSqlStr(d["TELEPHONE"]);
                ShopInfoSql += "E_MAIL=" + GetSqlStr(d["E_MAIL"]);
                ShopInfoSql += "BJR=" + GetSqlStr(d["userId"]);
                ShopInfoSql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                ShopInfoSql += "SHOP_NAME=" + GetSqlStr(d["SHOP_NAME"]);
                ShopInfoSql += "SHOPBH=" + GetSqlStr(d["SHOPBH"]);
                ShopInfoSql += "ZHLX=" + GetSqlStr(d["ZHLX"], 1);
                ShopInfoSql += "SHOP_STATUS=" + GetSqlStr(d["userType"], 1);
                ShopInfoSql += "ZXYJ=" + GetSqlStr(d["ZXYJ"], 1);
                ShopInfoSql += "ZXYJJFSJ=" + GetSqlStr(d["ZXYJJFSJ"]);
                ShopInfoSql += "ZXYJTFSJ=" + GetSqlStr(d["ZXYJTFSJ"]);
                ShopInfoSql += "XFBZJ=" + GetSqlStr(d["XFBZJ"], 1);
                ShopInfoSql += "XFBZJJFSJ=" + GetSqlStr(d["XFBZJJFSJ"]);
                ShopInfoSql += "XFBZJTFSJ=" + GetSqlStr(d["XFBZJTFSJ"]);
                ShopInfoSql += "OPEN_ID=(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE"] + "' LIMIT 1)t) ,";
                ShopInfoSql += "WYBZJ=" + GetSqlStr(d["WYBZJ"], 1);
                ShopInfoSql += "WYBZJJFSJ=" + GetSqlStr(d["WYBZJJFSJ"]);
                ShopInfoSql += "WYBZJTFSJ=" + GetSqlStr(d["WYBZJTFSJ"]);
                ShopInfoSql = ShopInfoSql.TrimEnd(',') + " WHERE CZ_SHID='" + d["CZ_SHID"] + "'";
                list.Add(ShopInfoSql);
            }
            string FeeSql = "UPDATE wy_RopertyCosts SET WYJFFS=" + GetSqlStr(d["WYJFFS"]);
            FeeSql += "WYJZSJ=" + GetSqlStr(d["WYJZSJ"]);
            //FeeSql += "WYJZ=" + GetSqlStr(d["WYJZ"], 1);
            FeeSql += "WYJZ=(SELECT JZMJ *  " + d["WYDJ"] +
                " FROM wy_houseinfo where FWID='" + d["FWID"] + "'),";
            FeeSql += "WYDJ=" + GetSqlStr(d["WYDJ"]);
            FeeSql = FeeSql.TrimEnd(',') + " WHERE FEE_ID='" + d["FEE_ID"] + "'";
            list.Add(FeeSql);

            return db.Executs(list);
        }
        public string PassInfo(string CZ_SHID)
        {
            string sql = " UPDATE wy_shopinfo set IS_PASS=1 where CZ_SHID='" + CZ_SHID + "'";
            return db.ExecutByStringResult(sql);
        }
        public string UnpassInfo(string CZ_SHID)
        {
            string sql = " UPDATE wy_shopinfo set IS_PASS=0 where CZ_SHID='" + CZ_SHID + "'";
            return db.ExecutByStringResult(sql);
        }
        public DataTable GetShopCostInfo(string FWID)
        {
            string sql = "SELECT * FROM V_pay_record WHERE FWID='" + FWID + "'";
            return db.GetDataTable(sql);
        }

        public string EndLease(string FWID, string CZ_SHID)
        {
            string HouseSql = "UPDATE wy_houseinfo SET FWSX=0,CZ_SHID=NULL WHERE FWID='" + FWID + "'";
            string ShopSql = "UPDATE wy_shopinfo SET SHOP_STATUS=4 WHERE CZ_SHID='"+CZ_SHID+"'";
            string FeeSql = "UPDATE wy_RopertyCosts SET IS_DELETE=1 WHERE FEE_ID=(SELECT FEE_ID FROM wy_shopinfo WHERE CZ_SHID='" + CZ_SHID + "')";
            string LeaseSql = " update wy_Leasinginfo SET IS_DELETE=1 WHERE LEASE_ID=(select LEASE_ID from wy_houseinfo WHERE CZ_SHID='" + CZ_SHID + "')";
            List<string> list = new List<string>()
            {
                { HouseSql },
                { FeeSql },
                { LeaseSql },
                { ShopSql }
            };
            return db.Executs(list);
            //return db.ExecutByStringResult(sql);
        }
        public string SecondHand(Dictionary<string,object> d)
        {
            /***
             *转售房屋，将房屋中的用户ID挂给新用户 老用户信息不变，是否转租状态变为2 新的购买人和房屋关联
             * 插入1条商户信息 插入一条物业费信息 修改房屋信息 修改老用户信息 共4条sql
             ***/
            string CZ_SHID = Guid.NewGuid().ToString();
            string FEE_ID = Guid.NewGuid().ToString();
            
            string NewShopInfoSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE,IS_SUBLET,TELEPHONE,E_MAIL," +
                   "IS_PASS,CJR,CJSJ,SHOP_NAME,SHOPBH,ZHLX,FEE_ID,IS_DELETE,FWID,SHOP_STATUS," +
                   "ZXYJ,ZXYJJFSJ,ZXYJTFSJ,XFBZJ,XFBZJJFSJ,XFBZJTFSJ,OPEN_ID,WYBZJ,WYBZJJFSJ,WYBZJTFSJ)values(";
            NewShopInfoSql += GetSqlStr(CZ_SHID);
            NewShopInfoSql += GetSqlStr(d["JYNR1"]);
            NewShopInfoSql += GetSqlStr(d["ZHXM1"]);
            NewShopInfoSql += GetSqlStr(d["ZHXB1"], 1);
            NewShopInfoSql += GetSqlStr(d["SFZH1"]);
            NewShopInfoSql += GetSqlStr(d["MOBILE_PHONE1"]);
            NewShopInfoSql += GetSqlStr(0,1);
            NewShopInfoSql += GetSqlStr(d["TELEPHONE1"]);
            NewShopInfoSql += GetSqlStr(d["E_MAIL1"]);
            NewShopInfoSql += GetSqlStr(0, 1);
            NewShopInfoSql += GetSqlStr(d["userId"]);
            NewShopInfoSql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            NewShopInfoSql += GetSqlStr(d["SHOP_NAME1"]);
            NewShopInfoSql += GetSqlStr(d["SHOPBH1"]);
            NewShopInfoSql += GetSqlStr(d["ZHLX1"], 1);
            NewShopInfoSql += GetSqlStr(FEE_ID);
            NewShopInfoSql += GetSqlStr(0, 1);
            NewShopInfoSql += GetSqlStr(d["FWID"]);
            NewShopInfoSql += GetSqlStr(2,1);
            NewShopInfoSql += GetSqlStr(d["ZXYJ1"], 1);
            NewShopInfoSql += GetSqlStr(d["ZXYJJFSJ1"]);
            NewShopInfoSql += GetSqlStr(d["ZXYJTFSJ1"]);
            NewShopInfoSql += GetSqlStr(d["XFBZJ1"], 1);
            NewShopInfoSql += GetSqlStr(d["XFBZJJFSJ1"]);
            NewShopInfoSql += GetSqlStr(d["XFBZJTFSJ1"]);
            NewShopInfoSql += "(select * FROM (SELECT OPENID FROM v_allusers WHERE PHONE='" + d["MOBILE_PHONE1"] + "' LIMIT 1)t),";
            NewShopInfoSql += GetSqlStr(d["WYBZJ1"], 1);
            NewShopInfoSql += GetSqlStr(d["WYBZJJFSJ1"]);
            NewShopInfoSql += GetSqlStr(d["WYBZJTFSJ1"]);
            NewShopInfoSql = NewShopInfoSql.TrimEnd(',') + ")";



            string FeeSql = "INSERT INTO wy_RopertyCosts (FEE_ID,WYJFFS,WYJZSJ,WYJZ,IS_DELETE,WYDJ)VALUES(";
            FeeSql += GetSqlStr(FEE_ID);
            FeeSql += GetSqlStr(d["WYJFFS1"]);
            FeeSql += GetSqlStr(d["WYJZSJ1"]);
            FeeSql += "(SELECT JZMJ* " + d["WYDJ1"] +
                     " FROM wy_houseinfo where FWID='" + d["FWID"] + "'),";
            FeeSql += GetSqlStr(0, 1);
            FeeSql += GetSqlStr(d["WYDJ1"], 1);
            FeeSql = FeeSql.TrimEnd(',') + ")";

            string HouseUpdateSql = "UPDATE wy_houseinfo set CZ_SHID='" + CZ_SHID + "' WHERE FWID='" + d["FWID"] + "'";

            string UpdateOldShop = "UPDATE wy_shopinfo set IS_SUBLET=2,SHOP_STATUS=3 WHERE CZ_SHID='" + d["CZ_SHID"] + "'";


            List<string> list = new List<string>()
            {
                { NewShopInfoSql},
                { FeeSql},
                { HouseUpdateSql},
                { UpdateOldShop},
            };
            return db.Executs(list);
        }

        public DataTable GetShopUserInfo(string FWBH,string ZHXM,string SFZH,string SHOPBH,int SHOP_STATUS)
        {
            string sql = "select a.*,b.FWBH,b.FWMC,c.Name from wy_shopinfo a " +
                " left join wy_houseinfo b on a.FWID=b.FWID AND b.IS_DELETE=0" +
                " left join tax_dictionary c on b.LSFGS=c.Code AND c.ParentCode='LSFGS'" +
                " where a.IS_DELETE=0";
            if (SHOP_STATUS != 0)
            {
                sql += " AND a.SHOP_STATUS=" + SHOP_STATUS;
            }
            if (!string.IsNullOrEmpty(FWBH))
            {
                sql += " AND b.FWBH='" + FWBH + "'";
            }
            if (!string.IsNullOrEmpty(ZHXM))
            {
                sql += " AND a.ZHXM like'%" + ZHXM + "%'";
            }
            if (!string.IsNullOrEmpty(SFZH))
            {
                sql += " AND a.SFZH='" + SFZH + "'";
            }
            if (!string.IsNullOrEmpty(SHOPBH))
            {
                sql += " AND a.SHOPBH='" + SHOPBH + "'";
            }
            return db.GetDataTable(sql);
        }

        public DataTable GetShopDetailUserInfo(string CZ_SHID)
        {
            string sql = "select a.*," +
                " b.*," +
                " c.*," +
                " d.JYNR AS JYNR1,d.ZHXM AS ZHXM1,d.ZHXB AS ZHXB1,d.SFZH AS SFZH1,d.MOBILE_PHONE AS MOBILE_PHONE1,d.TELEPHONE AS TELEPHONE1," +
                " d.E_MAIL AS E_MAIL1,d.SHOP_NAME AS SHOP_NAME1,d.SHOPBH AS SHOPBH1, " +
                " e.FWBH,e.FWMC,f.Name,e.JZMJ,e.ZLWZ,e.FWSX" +
                " from wy_shopinfo a " +
                " left join wy_Leasinginfo b on a.LEASE_ID=b.LEASE_ID" +
                " left join wy_RopertyCosts c on a.FEE_ID=c.FEE_ID" +
                " left join wy_shopinfo d on a.SUBLET_ID=d.CZ_SHID AND a.IS_SUBLET=1" +
                " left join wy_houseinfo e on e.FWID=a.FWID AND e.IS_DELETE=0 " +
                " left join tax_dictionary f on e.LSFGS=f.Code AND f.ParentCode='LSFGS'" +
                " where a.CZ_SHID='" + CZ_SHID + "'";
            return db.GetDataTable(sql);
        }

        public DataTable ExportShopInfo(string FWSX)
        {
            string sql = "select a.FWID,a.FWBH,a.FWMC,b.*,c.Name,a.FWID AS OLDID from wy_houseinfo a  " +
                " join wy_shopinfo b ON a.CZ_SHID=b.CZ_SHID" +
                " left join tax_dictionary c on a.LSFGS=c.Code and c.ParentCode='LSFGS'" +
                " where a.IS_DELETE=0 AND b.IS_DELETE=0" +
                " AND a.FWSX='"+FWSX+"'";
            //sql += " ORDER BY SHOP_ID OFFSET" + ((page - 1) * limit) + " rows fetch next " + limit + " rows only";
            return db.GetDataTable(sql);
        }
        /// <summary>
        /// 出租商户导入语句
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string UpLoadCZShopInfo(DataTable dt)
        {
            string ErrMes = "";//错误信息
            DataTable FWTable = db.GetDataTable("SELECT FWID,JZMJ,FWBH FROM wy_houseinfo WHERE FWSX=0 AND IS_DELETE=0");
            DataTable DicTable = db.GetDataTable("SELECT Name,Code FROM tax_dictionary WHERE ParentCode='PAY_WAY'");
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                DataRow[] FWdr = FWTable.Select("FWBH='" + dr["房屋编号"]+"'");
                if(FWdr.Length == 0)
                {
                    ErrMes += "房屋编号为" + dr["房屋编号"] + "的商铺没有找到对应的房屋，请去商铺管理功能确认此房屋存在并处于空闲状态！";
                    continue;
                }
                else if (FWdr.Length > 1)
                {
                    ErrMes += "房屋编号为" + dr["房屋编号"] + "的商铺匹配到了两个或者以上的商铺信息，请去商铺管理功能确认房屋编号是否重复！";
                    continue;
                }
                else if (ErrMes != "")
                {
                    continue;
                }
                else
                {
                    Guid CZ_SHID = Guid.NewGuid();
                    Guid FEE_ID = Guid.NewGuid();
                    Guid LEASE_ID = Guid.NewGuid();
                    string UpdateHouseInfo = "update wy_houseinfo set FWSX=1,CZ_SHID='" + CZ_SHID + "' WHERE FWID='" + FWdr[0]["FWID"] + "'";
                    list.Add(UpdateHouseInfo);
                    string ShopSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE," +
                        "IS_SUBLET,TELEPHONE,E_MAIL,IS_PASS,IS_DELETE,SHOP_NAME,FWID,SHOPBH,LEASE_ID,FEE_ID,ZHLX,SHOP_STATUS," +
                        "ZXYJ,ZXYJJFSJ,ZXYJTFSJ,XFBZJ,XFBZJJFSJ,XFBZJTFSJ,WYBZJ,WYBZJJFSJ,WYBZJTFSJ,OPEN_ID)VALUES(";
                    ShopSql += GetSqlStr(CZ_SHID);
                    ShopSql += GetSqlStr(dr["经营内容"]);
                    ShopSql += GetSqlStr(dr["租户姓名"]);
                    if (dr["租户性别"].ToString() == "女")
                    {
                        ShopSql += GetSqlStr(0, 1);
                    }
                    else
                    {
                        ShopSql += GetSqlStr(1, 1);
                    }
                    ShopSql += GetSqlStr(dr["身份证号"]);
                    ShopSql += GetSqlStr(dr["手机号码"]);
                    ShopSql += GetSqlStr(0, 1);
                    ShopSql += GetSqlStr(dr["固定电话"]);
                    ShopSql += GetSqlStr(dr["电子邮箱"]);
                    ShopSql += GetSqlStr(0, 1);
                    ShopSql += GetSqlStr(0, 1);
                    ShopSql += GetSqlStr(dr["商户名称"]);
                    ShopSql += GetSqlStr(FWdr[0]["FWID"]);
                    ShopSql += GetSqlStr(dr["商户编号"]);
                    ShopSql += GetSqlStr(LEASE_ID);
                    ShopSql += GetSqlStr(FEE_ID);
                    ShopSql += GetSqlStr(1, 1);
                    ShopSql += GetSqlStr(1, 1);
                    ShopSql += GetSqlStr(dr["装修押金"], 1);
                    ShopSql += GetSqlStr(dr["装修押金缴费时间"],2);
                    ShopSql += GetSqlStr(dr["装修押金退费时间"],2);
                    ShopSql += GetSqlStr(dr["消防保证金"], 1);
                    ShopSql += GetSqlStr(dr["消防保证金缴费时间"],2);
                    ShopSql += GetSqlStr(dr["消防保证金退费时间"],2);
                    ShopSql += GetSqlStr(dr["违约保证金"], 1);
                    ShopSql += GetSqlStr(dr["违约保证金缴费时间"],2);
                    ShopSql += GetSqlStr(dr["违约保证金退费时间"],2);
                    ShopSql += "(select * from(SELECT OPENID FROM v_allusers WHERE PHONE='" + dr["手机号码"] + "')t LIMIT 1),";
                    ShopSql = ShopSql.TrimEnd(',') + ")";
                    list.Add(ShopSql);
                    string FeeSql = "INSERT INTO wy_RopertyCosts(FEE_ID,WYJFFS,WYJZSJ,WYJZ,IS_DELETE,WYDJ)VALUES(";
                    FeeSql += GetSqlStr(FEE_ID);
                    FeeSql += GetSqlStr(DicTable.Select("Name='" + dr["物业缴纳方式"] + "'")[0]["Code"]);
                    FeeSql += GetSqlStr(dr["物业基准日期"],2);
                    FeeSql += GetSqlStr(Convert.ToDecimal(dr["物业费标准（元/平/月）"]) * Convert.ToDecimal(FWdr[0]["JZMJ"]), 1);
                    FeeSql += GetSqlStr(0, 1);
                    FeeSql += GetSqlStr(dr["物业费标准（元/平/月）"], 1);
                    FeeSql = FeeSql.TrimEnd(',') + ")";
                    list.Add(FeeSql);
                    string LeaseSql = "INSERT INTO wy_Leasinginfo(LEASE_ID,ZLKSSJ,ZLZZSJ,ZLZE,ZLYJ,ZLYS,ZJJFFS,IS_DELETE)VALUES(";
                    LeaseSql += GetSqlStr(LEASE_ID);
                    LeaseSql += GetSqlStr(dr["租赁起始日期"],2);
                    LeaseSql += GetSqlStr(dr["租赁结束日期"],2);
                    LeaseSql += GetSqlStr(dr["租赁总额"], 1);
                    LeaseSql += GetSqlStr(dr["租赁押金"], 1);
                    LeaseSql += GetSqlStr(dr["租赁月数"], 1);
                    LeaseSql += GetSqlStr(DicTable.Select("Name='" + dr["房租缴纳方式"] + "'")[0]["Code"]);
                    LeaseSql += GetSqlStr(0, 1);
                    LeaseSql = LeaseSql.TrimEnd(',') + ")";
                    list.Add(LeaseSql);
                }
                
            }
            if (ErrMes == "")
            {
                return db.Executs(list);
            }
            else
            {
                return ErrMes;
            }           
        }

        public string UpLoadCSShopInfo(DataTable dt)
        {
            string ErrMes = "";//错误信息
            DataTable FWTable = db.GetDataTable("SELECT FWID,JZMJ,FWBH FROM wy_houseinfo WHERE FWSX=0 AND IS_DELETE=0");
            DataTable DicTable = db.GetDataTable("SELECT Name,Code FROM tax_dictionary WHERE ParentCode='PAY_WAY'");
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                DataRow[] FWdr = FWTable.Select("FWBH='" + dr["房屋编号"] + "'");
                if (FWdr.Length == 0)
                {
                    ErrMes += "房屋编号为" + dr["房屋编号"] + "的商铺没有找到对应的房屋，请去商铺管理功能确认此房屋存在并处于空闲状态！";
                    continue;
                }
                else if (FWdr.Length > 1)
                {
                    ErrMes += "房屋编号为" + dr["房屋编号"] + "的商铺匹配到了两个或者以上的商铺信息，请去商铺管理功能确认房屋编号是否重复！";
                    continue;
                }
                else if (ErrMes != "")
                {
                    continue;
                }
                else
                {
                    Guid CZ_SHID = Guid.NewGuid();
                    Guid FEE_ID = Guid.NewGuid();
                    Guid SUBLET_ID = Guid.NewGuid();
                    string UpdateHouseInfo = "update wy_houseinfo set FWSX=2,CZ_SHID='" + CZ_SHID + "' WHERE FWID='" + FWdr[0]["FWID"] + "'";
                    list.Add(UpdateHouseInfo);
                    string ShopSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE," +
                        "IS_SUBLET,SUBLET_ID,TELEPHONE,E_MAIL,IS_PASS,IS_DELETE,SHOP_NAME,FWID,SHOPBH,FEE_ID,ZHLX,SHOP_STATUS," +
                        "ZXYJ,ZXYJJFSJ,ZXYJTFSJ,XFBZJ,XFBZJJFSJ,XFBZJTFSJ,WYBZJ,WYBZJJFSJ,WYBZJTFSJ,OPEN_ID)VALUES(";
                    ShopSql += GetSqlStr(CZ_SHID);
                    ShopSql += GetSqlStr(dr["经营内容"]);
                    ShopSql += GetSqlStr(dr["租户姓名"]);
                    if (dr["租户性别"].ToString() == "女")
                    {
                        ShopSql += GetSqlStr(0, 1);

                    }
                    else
                    {
                        ShopSql += GetSqlStr(1, 1);
                    }
                    ShopSql += GetSqlStr(dr["身份证号"]);
                    ShopSql += GetSqlStr(dr["手机号码"]);
                    if (dr["是否转租"].ToString() == "否")
                    {
                        ShopSql += GetSqlStr(0, 1);
                        ShopSql += GetSqlStr("");
                    }
                    else
                    {
                        ShopSql += GetSqlStr(1, 1);
                        ShopSql += GetSqlStr(SUBLET_ID);
                        string SubletSql = "INSERT INTO wy_shopinfo(CZ_SHID,JYNR,ZHXM,ZHXB,SFZH,MOBILE_PHONE," +
                        "IS_SUBLET,TELEPHONE,E_MAIL,IS_DELETE,SHOP_NAME,FWID,SHOPBH,ZHLX,SHOP_STATUS,OPEN_ID)VALUES(";
                        SubletSql += GetSqlStr(SUBLET_ID);
                        SubletSql += GetSqlStr(dr["转租经营内容"]);
                        SubletSql += GetSqlStr(dr["转租姓名"]);
                        if (dr["转租性别"].ToString() == "女")
                        {
                            SubletSql += GetSqlStr(0, 1);

                        }
                        else
                        {
                            SubletSql += GetSqlStr(1, 1);
                        }
                        SubletSql += GetSqlStr(dr["转租身份证号"]);
                        SubletSql += GetSqlStr(dr["转租手机号码"]);
                        SubletSql += GetSqlStr(0, 1);
                        SubletSql += GetSqlStr(dr["转租固定电话"]);
                        SubletSql += GetSqlStr(dr["转租电子邮箱"]);
                        SubletSql += GetSqlStr(0, 1);
                        SubletSql += GetSqlStr(dr["转租商户名称"]);
                        SubletSql += GetSqlStr(FWdr[0]["FWID"]);
                        SubletSql += GetSqlStr(dr["转租商户编号"]);
                        SubletSql += GetSqlStr(1, 1);
                        SubletSql += GetSqlStr(5, 1);
                        SubletSql += "(select * from(SELECT OPENID FROM v_allusers WHERE PHONE='" + dr["转租手机号码"] + "')t LIMIT 1),";
                        SubletSql = SubletSql.TrimEnd(',') + ")";
                        list.Add(SubletSql);

                    }
                    ShopSql += GetSqlStr(dr["固定电话"]);
                    ShopSql += GetSqlStr(dr["电子邮箱"]);
                    ShopSql += GetSqlStr(0, 1);
                    ShopSql += GetSqlStr(0, 1);
                    ShopSql += GetSqlStr(dr["商户名称"]);
                    ShopSql += GetSqlStr(FWdr[0]["FWID"]);
                    ShopSql += GetSqlStr(dr["商户编号"]);
                    ShopSql += GetSqlStr(FEE_ID);
                    ShopSql += GetSqlStr(1, 1);
                    ShopSql += GetSqlStr(2, 1);
                    ShopSql += GetSqlStr(dr["装修押金"], 1);
                    ShopSql += GetSqlStr(dr["装修押金缴费时间"],2);
                    ShopSql += GetSqlStr(dr["装修押金退费时间"],2);
                    ShopSql += GetSqlStr(dr["消防保证金"], 1);
                    ShopSql += GetSqlStr(dr["消防保证金缴费时间"],2);
                    ShopSql += GetSqlStr(dr["消防保证金退费时间"],2);
                    ShopSql += GetSqlStr(dr["违约保证金"], 1);
                    ShopSql += GetSqlStr(dr["违约保证金缴费时间"], 2);
                    ShopSql += GetSqlStr(dr["违约保证金退费时间"], 2);
                    ShopSql += "(select * from(SELECT OPENID FROM v_allusers WHERE PHONE='" + dr["手机号码"] + "')t LIMIT 1),";
                    ShopSql = ShopSql.TrimEnd(',') + ")";
                    list.Add(ShopSql);
                    string FeeSql = "INSERT INTO wy_RopertyCosts(FEE_ID,WYJFFS,WYJZSJ,WYJZ,IS_DELETE,WYDJ)VALUES(";
                    FeeSql += GetSqlStr(FEE_ID);
                    FeeSql += GetSqlStr(DicTable.Select("Name='" + dr["物业缴纳方式"] + "'")[0]["Code"]);
                    FeeSql += GetSqlStr(dr["物业基准日期"]);
                    FeeSql += GetSqlStr(Convert.ToDecimal(dr["物业费标准（元/平/月）"]) * Convert.ToDecimal(FWdr[0]["JZMJ"]), 1);
                    FeeSql += GetSqlStr(0, 1);
                    FeeSql += GetSqlStr(dr["物业费标准（元/平/月）"], 1);
                    FeeSql = FeeSql.TrimEnd(',') + ")";
                    list.Add(FeeSql);
                }
               
            }
            if (ErrMes == "")
            {
                return db.Executs(list);
            }
            else
            {
                return ErrMes;
            }
        }

        public DataTable GetLeaseTime(string CZ_SHID)
        {
            string sql = "select b.LEASE_ID,b.ZLKSSJ,b.ZLZZSJ from wy_shopinfo a" +
                " join wy_leasinginfo b on a.LEASE_ID=b.LEASE_ID " +
                " where b.CZ_SHID='" + CZ_SHID + "'";
            return db.GetDataTable(sql);
        }
        public string UpdateLeaseTime(Dictionary<string,object> d)
        {
            string sql = "UPDATE wy_leasinginfo SET ZLKSSJ='" + d["ZLKSSJ1"] + "'," +
                "ZLJSSJ='" + d["ZLJSSJ1"] + "' WHERE LEASE_ID='" + d["LEASE_ID"] + "'";
            return db.ExecutByStringResult(sql);
        }

        public string Renewal(Dictionary<string,object> d)
        {
            List<string> sqllist = new List<string>();
            Guid FEE_ID = Guid.NewGuid();
            Guid LEASE_ID = Guid.NewGuid();
            string FeeSql = "INSERT INTO wy_ropertycosts (FEE_ID,WYJFFS,WYJZSJ,WYDJ,IS_DELETE,WYJZ,CZ_SHID)VALUES(";
            FeeSql += GetSqlStr(FEE_ID);
            FeeSql += GetSqlStr(d["WYJFFS"]);
            FeeSql += GetSqlStr(d["WYJZSJ"]);
            FeeSql += GetSqlStr(d["WYDJ"],1);
            FeeSql += GetSqlStr(0,1);
            FeeSql += "(SELECT JZMJ * " + d["WYDJ"]+" FROM wy_houseinfo where FWID='" + d["FWID"] + "'),";
            FeeSql += GetSqlStr(d["CZ_SHID"]);
            //FeeSql += GetSqlStr(d["userId"]);
            //FeeSql += GetSqlStr(DateTime.Now);
            FeeSql = FeeSql.TrimEnd(',') + ")";
            sqllist.Add(FeeSql);
            string LeaseSql = "INSERT INTO wy_leasinginfo(LEASE_ID,ZLKSSJ,ZLZZSJ,ZLZE,ZLYJ,ZLYS,ZJJFFS,CJR,CJSJ,IS_DELETE,CZ_SHID)VALUES(";
            LeaseSql += GetSqlStr(LEASE_ID);
            LeaseSql += GetSqlStr(d["ZLKSSJ"]);
            LeaseSql += GetSqlStr(d["ZLZZSJ"]);
            LeaseSql += GetSqlStr(d["ZLZE"],1);
            LeaseSql += GetSqlStr(d["ZLYJ"],1);
            LeaseSql += GetSqlStr(d["ZLYS"],1);
            LeaseSql += GetSqlStr(d["ZJJFFS"]);
            LeaseSql += GetSqlStr(d["userId"]);
            LeaseSql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            LeaseSql += GetSqlStr(0,1);
            LeaseSql += GetSqlStr(d["CZ_SHID"]);
            LeaseSql = LeaseSql.TrimEnd(',') + ")";
            sqllist.Add(LeaseSql);
            string UpdateSql = "UPDATE wy_shopinfo set LEASE_ID=";
            UpdateSql += GetSqlStr(LEASE_ID);
            UpdateSql += "FEE_ID="+GetSqlStr(FEE_ID);
            UpdateSql = UpdateSql.TrimEnd(',') + "WHERE CZ_SHID='" + d["CZ_SHID"] + "'";
            sqllist.Add(UpdateSql);
            return db.Executs(sqllist);
        }
        public DataTable GetFeeResult(string CZ_SHID)
        {
            string sql = "SELECT * FROM (" +
                " SELECT b.ZHXM,b.SHOPBH,b.SHOP_NAME,a.FEE_TYPES,a.PAY_TIME,a.USER_ID AS CZ_SHID FROM wy_wx_pay a " +
                " JOIN wy_shopinfo b ON a.USER_ID = b.CZ_SHID " +
                " WHERE a.PAY_TIME is NOT NULL " +
                " UNION ALL" +
                " SELECT b.ZHXM,b.SHOPBH,b.SHOP_NAME,a.JFLX AS FEE_TYPES,a.JFRQ AS PAY_TIME,a.CZ_SHID FROM wy_pay_record a" +
                " JOIN wy_shopinfo b ON a.CZ_SHID = b.CZ_SHID" +
                " where a.PAY_WAY=0 AND a.JFZT=1)t" +
                " where t.CZ_SHID='" + CZ_SHID + "'" +
                " ORDER BY t.PAY_TIME DESC";
            return db.GetDataTable(sql);
        }
        public string GetSqlStr(object t, int type = 0)
        {
            if (t == null || t.ToString() == "")
            {
                return "null,";
            }
            else
            {
                if (type == 0)
                {
                    if (t.GetType().FullName.Trim().ToLower() == "system.datetime")
                    {
                        t = Convert.ToDateTime(t).ToString("yyyy-MM-dd");
                    }
                    return "'" + t + "',";
                }
                else if (type==2) {
                    t = Convert.ToDateTime(t).ToString("yyyy-MM-dd");
                    return "'" + t + "',";
                }
                else
                {
                    return t + ",";
                }
            }
        }
    }
}
