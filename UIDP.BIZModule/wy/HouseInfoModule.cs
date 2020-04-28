using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UIDP.BIZModule.wy.Models;
using UIDP.ODS.wy;

namespace UIDP.BIZModule.wy
{
    public class HouseInfoModule
    {
        HouseInfoDB db = new HouseInfoDB();
        public Dictionary<string, object> GetHouseInfo(string ORG_CODE,string FWBH,string FWMC, string LSFGS, string FWSX, int limit, int page, string baseURL)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataSet ds = db.GetHouseInfo(ORG_CODE,FWBH, FWMC, LSFGS, FWSX, limit, page);
                List<HouseInfoModel> list = new List<HouseInfoModel>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        HouseInfoModel item = new HouseInfoModel();
                        item.FWID = dr["FWID"] == null ? null : dr["FWID"].ToString();
                        item.FWSX = Convert.ToInt32(dr["FWSX"]);
                        item.FWBH = dr["FWBH"] == null ? null : dr["FWBH"].ToString();
                        item.FWMC = dr["FWMC"] == null ? null : dr["FWMC"].ToString();
                        item.JZMJ = dr["JZMJ"] == null ? 0 : Convert.ToDecimal(dr["JZMJ"]);
                        item.LSFGS = dr["LSFGS"] == null ? null : dr["LSFGS"].ToString();
                        item.ZLWZ = dr["ZLWZ"] == null ? null : dr["ZLWZ"].ToString();
                        item.JGLX = dr["JGLX"] == null ? null : dr["JGLX"].ToString();
                        item.ZCYZ = dr["ZCYZ"] == null ? 0 : Convert.ToDecimal(dr["ZCYZ"]);
                        item.SSQY = dr["SSQY"] == null ? null : dr["SSQY"].ToString();
                        item.WATER_NUMBER = dr["WATER_NUMBER"] == null ? null : dr["WATER_NUMBER"].ToString();
                        item.ELE_NUMBER = dr["ELE_NUMBER"] == null ? null : dr["ELE_NUMBER"].ToString();
                        item.CZ_SHID = dr["CZ_SHID"] == null ? null : dr["CZ_SHID"].ToString();
                        //item.LEASE_ID = dr["LEASE_ID"] == null ? null : dr["LEASE_ID"].ToString();
                        //item.CZ_SHID = dr["FEE_ID"] == null ? null : dr["FEE_ID"].ToString();
                        item.CJR = dr["CJR"] == null ? null : dr["CJR"].ToString();
                        item.CJSJ = dr["CJSJ"] == null ? null : dr["CJSJ"].ToString();
                        item.BJR = dr["BJR"] == null ? null : dr["BJR"].ToString();
                        item.BJSJ = dr["BJSJ"] == null ? null : dr["BJSJ"].ToString();
                        item.IS_DELETE = Convert.ToInt32(dr["IS_DELETE"]);
                        item.ZFK = dr["ZFK"] == null ? 0 : Convert.ToDecimal(dr["ZFK"].ToString());
                        item.LS = dr["LS"] == null ? null : dr["LS"].ToString();
                        item.JG = dr["JG"] == null ? null : dr["JG"].ToString();
                        item.SS=dr["SS"] == null ? null : dr["SS"].ToString();
                        item.CID = dr["CID"] == null ? null : dr["CID"].ToString();
                        item.PMT = new List<file>();
                        if (dr["PMT"] != null && dr["PMT"].ToString() != "")
                        {
                            foreach (string path in dr["PMT"].ToString().TrimEnd(',').Split(','))
                            {
                                file f = new file()
                                {
                                    name = Guid.NewGuid().ToString(),
                                    url = baseURL + path
                                };
                                item.PMT.Add(f);
                                item.newFilePath = dr["PMT"].ToString();
                            }
                        }
                        else
                        {
                            item.newFilePath = "";
                        }
                        list.Add(item);
                    }
                    r["code"] = 2000;
                    r["total"] = ds.Tables[1].Rows[0]["TOTAL"];
                    r["items"] = list;
                    r["message"] = "成功!";
                }
                else
                {
                    r["total"] = 0;
                    r["code"] = 2000;
                    r["message"] = "成功，但是没有数据";
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> CreateHouseInfo(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.CreateHouseInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> UpdateHouseInfo(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UpdateHouseInfo(d);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> DeleteHouseInfo(string FWID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteHouseInfo(FWID);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }
        public string UploadHouseInfo(string filePath,Dictionary<string,object> userinfo)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            List<string> list = new List<string>();
            string modePath = System.IO.Directory.GetCurrentDirectory() + "\\ExcelModel\\房屋档案表模板.xls";//原始文件
            string path = filePath;//原始文件
            string mes = "";
            DataTable dt = new DataTable();

            UTILITY.ExcelTools tool = new UTILITY.ExcelTools();
            tool.GetDataTable(System.IO.File.OpenRead(path), path, modePath, ref mes, ref dt);
            if (dt == null || dt.Rows.Count == 0)
            {
                return "空数据，导入失败！";
            }
            try
            {
                string b = db.UpLoadHouseInfo(dt,userinfo);
                if (b == "")
                {
                    return "";
                }
                else
                {
                    return b;
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public Dictionary<string, object> ExportHouseInfo()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.ExportHouseInfo();
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["total"] = dt.Rows.Count;
                    r["items"] = dt;
                    r["message"] = "成功!";
                }
                else
                {
                    r["total"] = 0;
                    r["code"] = 2001;
                    r["message"] = "成功，但是没有数据";
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }
    }
}
