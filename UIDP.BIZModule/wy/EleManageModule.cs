using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class EleManageModule
    {
        public static IConfiguration Configuration { get; set; }
        EleManageDB db = new EleManageDB();
        public Dictionary<string, object> GetData(int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetData();
                if (dt.Rows.Count > 0)
                {

                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit); 
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = 2000;
                    r["items"] = new DataTable();
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
        /// <summary>
        /// 清零操作
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Dictionary<string, object> QingLing(Dictionary<string, object> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                Newtonsoft.Json.Linq.JArray arry = (Newtonsoft.Json.Linq.JArray)list["data"];
                if (arry.Count > 0)
                {
                    List<Dictionary<string, object>> listEle = new List<Dictionary<string, object>>();
                    foreach (var item in arry)
                    {
                        Dictionary<string, object> eleDic = new Dictionary<string, object>();
                        eleDic.Add("opr_id", item["opr_id"]?.ToString());
                        eleDic.Add("address", item["address"]?.ToString());//map.put("address", "201908290001");
                        eleDic.Add("cid", item["cid"]?.ToString());//map.put("cid", "201908290001");
                        eleDic.Add("time_out", 0);
                        eleDic.Add("must_online", true);
                        eleDic.Add("retry_time", 1);
                        listEle.Add(eleDic);
                    }
                    var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");
                    Configuration = builder.Build();
                    string EleApiIP = Configuration.GetSection("Ele").GetSection("qingling").Value;
                    string authCode = Configuration.GetSection("Ele").GetSection("authCode").Value;
                    string nonce = Configuration.GetSection("Ele").GetSection("nonce").Value;
                    TQApi tqApi = new TQApi(
                           authCode,
                           nonce,
                           EleApiIP,
                           ""
                          );// SyncMode.enable
                    string res = tqApi.readElecMeter(listEle);
                    Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
                    string status = dict["status"];
                    if ("SUCCESS".Equals(status))
                    {
                        string response_content = dict["response_content"];
                        List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response_content);
                        if (items.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < items.Count; i++)
                            {
                                Dictionary<string, string> item = items[i];
                                for (int k = 0; k < listEle.Count; k++) //最上面的查询sql list
                                {
                                    Dictionary<string, object> item2 = listEle[k];
                                    if (item["opr_id"].ToString() == item2["opr_id"].ToString())
                                    {
                                        if (item["status"].ToString() == "SUCCESS")
                                        {
                                            sb.Append("update wy_ele_recharge set QingLingMsg='',");
                                            sb.Append("QingLing=1 where opr_id='" + item2["opr_id"] + "' and address='" + item2["address"] + "' ; ");
                                        }
                                        else
                                        {
                                            sb.Append("update wy_ele_recharge set  ");
                                            sb.Append("QingLing=0,QingLingMsg='" + item["error_msg"].ToString() + "'  where opr_id='" + item2["opr_id"] + "' and address='" + item2["address"] + "' ; ");
                                        }
                                        break;
                                    }
                                }

                            }
                            string result = db.update(sb.ToString());
                            if (result == "")
                            {
                                r["message"] = "操作成功!";
                                r["code"] = 2000;
                                return r;
                            }
                            else
                            {
                                r["message"] = result;
                                r["code"] = -1;
                                return r;
                            }
                        }

                    }
                    else
                    {
                        r["code"] = -1;
                        r["message"] = res;
                    }
                    return r;
                }
                else
                {
                    r["message"] = " 请选择数据";
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
        /// 清零操作
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Dictionary<string, object> KaiHu(Dictionary<string, object> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                Newtonsoft.Json.Linq.JArray arry = (Newtonsoft.Json.Linq.JArray)list["data"];
                if (arry.Count > 0)
                {
                    List<Dictionary<string, object>> listEle = new List<Dictionary<string, object>>();
                    foreach (var item in arry)
                    {
                        Dictionary<string, object> eleDic = new Dictionary<string, object>();
                        eleDic.Add("opr_id", item["opr_id"]?.ToString());
                        eleDic.Add("address", item["address"]?.ToString());//map.put("address", "201908290001");
                        eleDic.Add("cid", item["cid"]?.ToString());//map.put("cid", "201908290001");
                        eleDic.Add("time_out", 0);
                        eleDic.Add("must_online", true);
                        eleDic.Add("retry_time", 1);
                        Dictionary<string, object> dicAccount = new Dictionary<string, object>();
                        dicAccount.Add("account_id", "1234");
                        dicAccount.Add("count", "2");
                        dicAccount.Add("money", "1");
                        //ElecMeterAccount elecMeterAccount = new ElecMeterAccount("1234", "2", row["Cost"].ToString());
                        // map.put("params", elecMeterAccount.getAccount());
                        eleDic.Add("params", dicAccount);
                        listEle.Add(eleDic);
                    }
                    var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");
                    Configuration = builder.Build();
                    string EleApiIP = Configuration.GetSection("Ele").GetSection("kaihu").Value;
                    string authCode = Configuration.GetSection("Ele").GetSection("authCode").Value;
                    string nonce = Configuration.GetSection("Ele").GetSection("nonce").Value;
                    TQApi tqApi = new TQApi(
                           authCode,
                           nonce,
                           EleApiIP,
                           ""
                          );// SyncMode.enable
                    string res = tqApi.readElecMeter(listEle);


                    Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
                    string status = dict["status"];
                    if ("SUCCESS".Equals(status))
                    {
                        JObject jObject = Tools.JsonDeSerilize(res) as JObject;
                        if (jObject != null)
                        {
                            res = jObject.Serilize2Json();
                            JToken token = null;
                            if (jObject.TryGetValue("response_content", out token) && token is JValue)
                            {
                                JArray items = Tools.JsonDeSerilize(((JValue)token).Value.ToString()) as JArray;
                                if (items != null && items.Count > 0)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    for (int i = 0; i < items.Count; i++)
                                    {
                                        var item = items[i];
                                        for (int k = 0; k < listEle.Count; k++) //最上面的查询sql list
                                        {
                                            Dictionary<string, object> item2 = listEle[k];
                                            if (item["opr_id"].ToString() == item2["opr_id"].ToString())
                                            {
                                                if (item["status"].ToString() == "SUCCESS")
                                                {
                                                    sb.Append("update wy_ele_recharge set KaiHuMsg='',");
                                                    sb.Append("KaiHu=1 where opr_id='" + item2["opr_id"] + "' and address='" + item2["address"] + "' ; ");
                                                }
                                                else
                                                {
                                                    sb.Append("update wy_ele_recharge set  ");
                                                    sb.Append("KaiHu=0,KaiHuMsg='" + item["err_msg"].ToString() + "'  where opr_id='" + item2["opr_id"] + "' and address='" + item2["address"] + "' ; ");
                                                }
                                                break;
                                            }
                                        }

                                    }
                                    string result = db.update(sb.ToString());
                                    if (result == "")
                                    {
                                        r["message"] = "操作成功!";
                                        r["code"] = 2000;
                                        return r;
                                    }
                                    else
                                    {
                                        r["message"] = result;
                                        r["code"] = -1;
                                        return r;
                                    }
                                }

                            }

                            #region MyRegion


                            //}
                            //if (items.Count > 0)
                            //{
                            //    StringBuilder sb = new StringBuilder();
                            //    for (int i = 0; i < items.Count; i++)
                            //    {
                            //        Dictionary<string, string> item = items[i];
                            //        for (int k = 0; k < listEle.Count; k++) //最上面的查询sql list
                            //        {
                            //            Dictionary<string, object> item2 = listEle[k];
                            //            if (item["opr_id"].ToString() == item2["opr_id"].ToString())
                            //            {
                            //                if (item["status"].ToString() == "SUCCESS")
                            //                {
                            //                    sb.Append("update wy_ele_recharge set ");
                            //                    sb.Append("KaiHu=1 where opr_id='" + item2["opr_id"] + "' and address='" + item2["address"] + "' ; ");
                            //                }
                            //                else
                            //                {
                            //                    sb.Append("update wy_ele_recharge set  ");
                            //                    sb.Append("KaiHu=0,KaiHuMsg='" + item["error_msg"].ToString() + "'  where opr_id='" + item2["opr_id"] + "' and address='" + item2["address"] + "' ; ");
                            //                }
                            //                break;
                            //            }
                            //        }

                            //    }
                            //    string result = db.update(sb.ToString());
                            //    if (result == "")
                            //    {
                            //        r["message"] = "操作成功!";
                            //        r["code"] = 2000;
                            //        return r;
                            //    }
                            //    else
                            //    {
                            //        r["message"] = result;
                            //        r["code"] = -1;
                            //        return r;
                            //    }
                            //}
                            #endregion
                        }
                        else
                        {
                            r["code"] = -1;
                            r["message"] = res;
                        }
                        return r;
                    }
                    else
                    {
                        r["message"] = " 请选择数据";
                        r["code"] = -1;
                    }
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }
        /// <summary>
        /// 水表报表查询
        /// </summary>
        /// <param name="yjstate">是否预警</param>
        /// <param name="month"></param>
        /// <param name="HouseName"></param>
        /// <param name="YeZhuName"></param>
        /// <param name="ZhuanZuName"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetWaterData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetWaterData( yjstate, month,  HouseName,  YeZhuName,  ZhuanZuName);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit); 
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = 2000;
                    r["items"] = new DataTable();
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
        public Dictionary<string, object> ExportWaterData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetWaterData(yjstate, month, HouseName, YeZhuName, ZhuanZuName);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = dt;
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = 2000;
                    r["items"] = new DataTable();
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
        public Dictionary<string, object> GetEleData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetEleData(yjstate, month, HouseName, YeZhuName, ZhuanZuName);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = 2000;
                    r["items"] = new DataTable();
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
        public Dictionary<string, object> ExportEleData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetEleData(yjstate, month, HouseName, YeZhuName, ZhuanZuName);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = dt;
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = 2000;
                    r["items"] = new DataTable();
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
        public Dictionary<string, object> getEleWaterWarningMsg()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dtEle = db.GetEleData("1", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"), "", "", "");
                DataTable dtWater = db.GetEleData("1", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"), "", "", "");
                int elecount = 0;
                int watercount = 0;
                if (dtEle != null&& dtEle.Rows.Count > 0)
                {
                    elecount = dtEle.Rows.Count;
                }
                if (dtWater != null && dtWater.Rows.Count > 0)
                {
                    watercount = dtWater.Rows.Count;
                }
                if (elecount>0|| watercount>0) {
                    string resl = "上个月有"+ elecount + "家商铺电表用电过高，"+ watercount+"家商铺水表用水量过高，详情请去水电报表页面查看！";
                    r["items"] = resl;
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = 2000;
                    r["items"] = "";
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
