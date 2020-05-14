using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class EleManageDB
    {
        DBTool db = new DBTool("");
        public DataTable GetData()
        {
            string sql = "select * from wy_ele_recharge where CStatus='FAIL' ";
            return db.GetDataTable(sql);
        }
        public string update(string sql)
        {
            return db.ExecutByStringResult(sql);
        }
        public DataTable GetWaterData(string yjstate,string month, string HouseName, string YeZhuName, string ZhuanZuName)
        {
            string sql = @"select a.FWBH,a.FWMC,b.ZHXM,b.MOBILE_PHONE,c.ZHXM ZHXM1,c.MOBILE_PHONE MOBILE_PHONE1 ,a.WATER_NUMBER,
                            (d.maxMeterSurflow-d.minMeterSurflow) MeterFlowDiff,case when isnull(e.WaterAmount)=1 then 0 else e.WaterAmount end AmountLimit,
case when (d.maxMeterSurflow-d.minMeterSurflow)>=(case when isnull(e.WaterAmount)=1 then 0 else e.WaterAmount end) then 1 else 0 end yjstate,d.CreateMonth
                                 from wy_houseinfo a 
                            join wy_shopinfo b on a.CZ_SHID=b.CZ_SHID
                            left join wy_shopinfo c on  c.CZ_SHID=b.SUBLET_ID
                            join v_water_amount_limit d on d.MeterID=a.WATER_NUMBER 
                            left join wy_w_amount_config e on e.WaterNumber=a.WATER_NUMBER
                            where a.IS_DELETE=0 and b.IS_DELETE=0 ";
            if ("1".Equals(yjstate))
            {
                sql = sql + " and (d.maxMeterSurflow-d.minMeterSurflow)>=(case when isnull(e.WaterAmount)=1 then 0 else e.WaterAmount end) ";
            }
            else if ("0".Equals(yjstate))
            {
                sql = sql + " and (d.maxMeterSurflow-d.minMeterSurflow)<(case when isnull(e.WaterAmount)=1 then 0 else e.WaterAmount end) ";
            }
            else {

            }
            if (!string.IsNullOrWhiteSpace(month))
            {
                sql = sql + " and TIMESTAMPDIFF(month,CONCAT(d.CreateMonth,'-01'),'"+month+"')=0 ";
            }
            if (!string.IsNullOrWhiteSpace(HouseName))
            {
                sql = sql + " and  a.FWMC like '%"+HouseName+"%' ";
            }
            if (!string.IsNullOrWhiteSpace(YeZhuName))
            {
                sql = sql + " and b.ZHXM like '%"+YeZhuName+"%' ";
            }
            if (!string.IsNullOrWhiteSpace(ZhuanZuName))
            {
                sql = sql + " c.ZHXM like '%" + ZhuanZuName + "%' ";
            }
            sql = sql + "order by a.FWMC ";
            return db.GetDataTable(sql);
        }
        public DataTable GetEleData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName)
        {
            string sql = @"select a.FWBH,a.FWMC,b.ZHXM,b.MOBILE_PHONE,c.ZHXM ZHXM1,c.MOBILE_PHONE MOBILE_PHONE1 ,a.ELE_NUMBER,a.CID,
                          (d.maxTotalEle-d.minTotalEle)  eleAmountDiff,
case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end AmountLimit,
case when (d.maxTotalEle-d.minTotalEle)>=(case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end) then 1 else 0 end yjstate,
d.UpdateDateMonth
                            from wy_houseinfo a 
                            join wy_shopinfo b on a.CZ_SHID=b.CZ_SHID
                            left join wy_shopinfo c on  c.CZ_SHID=b.SUBLET_ID
                            join v_ele_amount_limitnew d on d.address=a.ELE_NUMBER and d.cid=a.cid  
                            left join wy_ele_amount_config e on e.address=a.ELE_NUMBER and e.cid=a.CID
                            where a.IS_DELETE=0 and b.IS_DELETE=0  ";
            if ("1".Equals(yjstate))
            {
                sql = sql + " and   (d.maxTotalEle-d.minTotalEle)>=(case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end) ";
            }
            else if ("0".Equals(yjstate))
            {
                sql = sql + " and   (d.maxTotalEle-d.minTotalEle)<(case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end) ";
            }
            else
            {

            }
            if (!string.IsNullOrWhiteSpace(month))
            {
                sql = sql + " and TIMESTAMPDIFF(month,CONCAT(d.UpdateDateMonth,'-01'),'" + month + "')=0 ";
            }
            if (!string.IsNullOrWhiteSpace(HouseName))
            {
                sql = sql + " and  a.FWMC like '%" + HouseName + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(YeZhuName))
            {
                sql = sql + " and b.ZHXM like '%" + YeZhuName + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(ZhuanZuName))
            {
                sql = sql + " c.ZHXM like '%" + ZhuanZuName + "%' ";
            }
            sql = sql + "order by a.FWMC ";
            return db.GetDataTable(sql);
        }
        public DataTable GetEleDatabak(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName)
        {
            string sql = @"select a.FWBH,a.FWMC,b.ZHXM,b.MOBILE_PHONE,c.ZHXM ZHXM1,c.MOBILE_PHONE MOBILE_PHONE1 ,a.ELE_NUMBER,a.CID,
                            eleAmountDiff,case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end AmountLimit,
case when d.eleAmountDiff>=(case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end) then 1 else 0 end yjstate,d.monthdate
                            from wy_houseinfo a 
                            join wy_shopinfo b on a.CZ_SHID=b.CZ_SHID
                            left join wy_shopinfo c on  c.CZ_SHID=b.SUBLET_ID
                            join v_ele_amount_limit d on d.address=a.ELE_NUMBER and d.cid=a.cid  
                            left join wy_ele_amount_config e on e.address=a.ELE_NUMBER and e.cid=a.CID
                            where a.IS_DELETE=0 and b.IS_DELETE=0  ";
            if ("1".Equals(yjstate))
            {
                sql = sql + " and  d.eleAmountDiff>=(case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end) ";
            }
            else if ("0".Equals(yjstate))
            {
                sql = sql + " and  d.eleAmountDiff<(case when isnull(e.eleAmount)=1 then 0 else e.eleAmount end) ";
            }
            else
            {

            }
            if (!string.IsNullOrWhiteSpace(month))
            {
                sql = sql + " and TIMESTAMPDIFF(month,CONCAT(d.monthdate,'-01'),'" + month + "')=0 ";
            }
            if (!string.IsNullOrWhiteSpace(HouseName))
            {
                sql = sql + " and  a.FWMC like '%" + HouseName + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(YeZhuName))
            {
                sql = sql + " and b.ZHXM like '%" + YeZhuName + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(ZhuanZuName))
            {
                sql = sql + " c.ZHXM like '%" + ZhuanZuName + "%' ";
            }
            sql = sql + "order by a.FWMC ";
            return db.GetDataTable(sql);
        }
    }
}
