using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class ShopInfoModule
    {
        ShopInfoDB db = new ShopInfoDB();
        public Dictionary<string,object> GetShopInfo(string ORG_CODE,string ZHXM, string IS_PASS, string FWSX, string FWID,int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetShopInfo(ORG_CODE,ZHXM, IS_PASS,FWSX, FWID);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["code"] = 2000;
                    r["items"] = KVTool.GetPagedTable(dt,page,limit);
                    r["total"] = dt.Rows.Count;

                }
                else
                {
                    r["message"] = "成功，但是没有数据";
                    r["code"] = 2000;
                    r["items"] = new DataTable();
                    r["total"] = 0;
                }
            }
            catch(Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> GetShopInfoDetail(string CZ_SHID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetShopInfoDetail(CZ_SHID);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["code"] = 2000;
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;

                }
                else
                {
                    r["message"] = "成功，但是没有数据";
                    r["code"] = 2000;
                    r["items"] = new DataTable();
                    r["total"] = 0;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string,object> DeleteShopInfo(string CZ_SHID,string FWID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteShopInfo(CZ_SHID,FWID);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
        public Dictionary<string,object> CreateShopInfo(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.CreateShopInfo(d);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch(Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string,object> UpdateShopInfo(Dictionary<string,object> d)
        {
            DateTime dt = DateTime.Now;
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UpdateShopInfo(d);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
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

        public Dictionary<string, object> PassInfo(string CZ_SHID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.PassInfo(CZ_SHID);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
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

        public Dictionary<string, object> UnpassInfo(string CZ_SHID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UnpassInfo(CZ_SHID);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
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

        public Dictionary<string, object> EndLease(string FWID,string CZ_SHID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string mes = JudgeArrears(FWID);
                if (mes == "")
                {
                    string b = db.EndLease(FWID, CZ_SHID);
                    if (b == "")
                    {
                        r["message"] = "成功";
                        r["code"] = 2000;
                    }
                    else
                    {
                        r["message"] = b;
                        r["code"] = -1;
                    }
                }
                else
                {
                    r["message"] = mes;
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

        public Dictionary<string, object> GetShopUserInfo(string FWBH, string ZHXM, string SFZH, string SHOPBH, int SHOP_STATUS,int limit,int page)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetShopUserInfo(FWBH, ZHXM, SFZH, SHOPBH, SHOP_STATUS);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2001;
                    r["message"] = "成功,但是没有数据,租户可能已经解除了物业关系";
                    r["items"] = dt;
                    r["total"] = 0;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }


        public Dictionary<string, object> GetShopDetailUserInfo(string CZ_SHID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetShopDetailUserInfo(CZ_SHID);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "成功,但是没有数据";
                    r["items"] = dt;
                    r["total"] = 0;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> SecondHand(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string mes = JudgeArrears(d["FWID"].ToString());
                if (mes == "")
                {
                    string b = db.SecondHand(d);
                    if (b == "")
                    {
                        r["message"] = "成功";
                        r["code"] = 2000;
                    }
                    else
                    {
                        r["message"] = b;
                        r["code"] = -1;
                    }
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = mes;
                }

            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> ExportShopInfo(string FWSX)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.ExportShopInfo(FWSX);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["code"] = 2000;
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;

                }
                else
                {
                    r["message"] = "成功，但是没有数据";
                    r["code"] = 2001;
                    r["items"] = new DataTable();
                    r["total"] = 0;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public string JudgeArrears(string FWID)
        {
            string mes = "";
            DataTable InfoDic = db.GetShopCostInfo(FWID);
            if (InfoDic.Rows.Count > 0)
            {
                foreach (DataRow dr in InfoDic.Rows)
                {
                    if (dr["JFLX"].ToString() == "0")//物业费
                    {
                        if (dr["JFZT"].ToString() == "0")
                        {
                            if (!(Convert.ToDateTime(dr["YXQS"]) > DateTime.Now))
                            {
                                mes += "当前商户存在物业费欠缴情况!";
                            }
                        }
                        if (dr["JFZT"].ToString() == "1")
                        {
                            if (!(Convert.ToDateTime(dr["YXQZ"]) > DateTime.Now))
                            {
                                mes += "虽然当前商户已经清缴上一季物业费,新的物业通知单还未生成,但是本季已属于欠费状态,请提示商户清缴本季物业费后再来解除物业关系！";
                            }
                        }
                    }
                    else if (dr["JFLX"].ToString() == "1")//水费
                    {
                        if (Convert.ToDecimal(dr["SURPLUSVALUE"]) < 0)
                        {
                            mes += "当前商户存在水费欠缴情况!";
                        }
                    }
                    else if (dr["JFLX"].ToString() == "2")
                    {
                        if (Convert.ToDecimal(dr["SURPLUSVALUE"]) < 0)
                        {
                            mes += "当前商户存在电费欠缴情况!";
                        }
                    }
                }
            }
            return mes;
        }

        public string uploadCZSHOPInfo(string filePath)
        {
            string modePath = System.IO.Directory.GetCurrentDirectory() + "\\ExcelModel\\出租商户模板.xls";//原始文件
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
                string b = db.UpLoadCZShopInfo(dt);
                if (b == "")
                {
                    return "";
                }
                else
                {
                    return b;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string uploadCSSHOPInfo(string filePath)
        {
            string modePath = System.IO.Directory.GetCurrentDirectory() + "\\ExcelModel\\出售商户模板.xls";//原始文件
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
                string b = db.UpLoadCSShopInfo(dt);
                if (b == "")
                {
                    return "";
                }
                else
                {
                    return b;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public Dictionary<string,object> GetLeaseTime(string CZ_SHID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetLeaseTime(CZ_SHID);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功!";
                    r["code"] = 2000;
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["message"] = "成功,但是没有数据";
                    r["code"] = 2000;
                    r["items"] = new DataTable();
                }
            }
            catch(Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }


        public Dictionary<string, object> UpdateLeaseTime(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UpdateLeaseTime(d);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
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

        public Dictionary<string,object> Renewal(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.Renewal(d);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
    }
}
