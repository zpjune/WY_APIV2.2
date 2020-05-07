using House.IService.Common.Message;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIDP.ODS.wy;
using UIDP.UTILITY;
using UIDP.UTILITY.JWTHelper;

namespace UIDP.BIZModule.wy
{
    public class FeeResultModule
    {
        FeeResultDB db = new FeeResultDB();

        public static IConfiguration Configuration { get; set; }
        public string url;
        public FeeResultModule()
        {
            url = GetSendUrl();
        }

        public string GetSendUrl()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            return Configuration.GetSection("msgUrl").GetSection("url").Value;
        }

        public Dictionary<string,object> GetFeeResult(string JFLX, string FWMC, string FWBH, string JFSTATUS,int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetFeeResult(JFLX, FWMC, FWBH, JFSTATUS);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = "成功！,但是没有数据!";
                    r["total"] = 0;
                    r["code"] = 2000;
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }


        public Dictionary<string, object> GetShopInfo(string FWID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetShopInfo(FWID);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["total"] = dt.Rows.Count;
                    r["items"] = dt;
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = "成功！,但是没有数据!";
                    r["total"] = 0;
                    r["code"] = 2000;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string,object> CreateNotification(string type)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                int MonthNum;
                if (type == "0")//按月催缴
                {
                    MonthNum = 1;
                }
                else
                {
                    MonthNum = 3;
                }
                string b = db.CreateNotification(CreateNotification(db.GetFeeInfoAndNotification(),MonthNum));
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = b;
                }

            }
            catch(Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }
        public List<string> CreateNotification(DataTable dt,int MonthNum)
        {
            DateTime datetime = DateTime.Now;
            List<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                DateTime JZR;
                if (dr["IS_CHANGEUSER"].ToString()=="1")//已经生成过缴费确认单的商户,且上个生成的通知单通知的用户和本次查询额房屋用户是同一人
                {
                    if (dr["JFZT"].ToString() == "1")//最近一条记录为已缴费状态
                    {
                        JZR = Convert.ToDateTime(dr["YXQZ"]);
                        DateTime EndTime;
                        decimal WYJZ = Convert.ToDecimal(dr["WYJZ"]);
                        switch (dr["WYJFFS"].ToString().Trim())
                        {
                            case "PerMonth":
                                EndTime = JZR.AddMonths(1);
                                WYJZ = WYJZ * 1;
                                break;
                            case "PerHalfYear":
                                EndTime = JZR.AddMonths(6);
                                WYJZ = WYJZ * 6;
                                break;
                            case "PerThreeMonth":
                                EndTime = JZR.AddMonths(3);
                                WYJZ = WYJZ * 3;
                                break;
                            case "PerYear":
                                EndTime = JZR.AddYears(1);
                                WYJZ = WYJZ * 12;
                                break;
                            default:
                                throw new Exception("参数不正确!");
                        }
                        if (datetime.AddMonths(MonthNum) > JZR)
                        {
                            string sql = "INSERT INTO wy_pay_record" +
                                "(RECORD_ID,JFLX,FWID,JFJE,JFZT,SFTZ,JFCS,YXQS,YXQZ,CREATE_TIME,CZ_SHID,OPEN_ID,CONFIRM_RECIVEMONEY,FEE_ID)VALUES(";
                            sql += GetSqlStr(Guid.NewGuid());
                            sql += GetSqlStr(0, 1);//物业费
                            sql += GetSqlStr(dr["FWID"]);
                            sql += GetSqlStr(WYJZ);
                            sql += GetSqlStr(0,1);//缴费状态 0 否
                            sql += GetSqlStr(0, 1);//是否通知 0 否
                            sql += GetSqlStr(0, 1);//催缴次数 0 
                            sql += GetSqlStr(dr["YXQZ"]);//缴费日期有效期起 最新一条已缴费记录的有效期止就是新一条的有效期起
                            sql += GetSqlStr(EndTime.ToString("yyyyMMdd"));
                            sql += GetSqlStr(datetime.ToString("yyyyMMdd"));
                            sql += GetSqlStr(dr["CZ_SHID"]);
                            sql += GetSqlStr(dr["OPEN_ID"]);
                            sql += GetSqlStr(0, 1);
                            sql += GetSqlStr(dr["FEE_ID"]);
                            sql = sql.TrimEnd(',') + ")";
                            list.Add(sql);
                        }
                    }
                    
                }
                else//对于首次提醒的商户和本次查询的房屋使用者和上次生成的通知单不是一个人
                {
                    DateTime EndTime;
                    JZR = Convert.ToDateTime(dr["WYJZSJ"]);
                    decimal WYJZ = Convert.ToDecimal(dr["WYJZ"]);
                    switch (dr["WYJFFS"].ToString().Trim())
                    {
                        case "PerMonth":
                            EndTime = JZR.AddMonths(1);
                            WYJZ = WYJZ * 1;
                            break;
                        case "PerHalfYear":
                            EndTime = JZR.AddMonths(6);
                            WYJZ = WYJZ * 6;
                            break;
                        case "PerThreeMonth":
                            EndTime = JZR.AddMonths(3);
                            WYJZ = WYJZ * 3;
                            break;
                        case "PerYear":
                            EndTime = JZR.AddYears(1);
                            WYJZ = WYJZ * 12;
                            break;
                        default:
                            throw new Exception("参数不正确!");
                    }
                    if (datetime.AddMonths(MonthNum) > JZR)
                    {
                        string sql = "INSERT INTO wy_pay_record" +
                               "(RECORD_ID,JFLX,FWID,JFJE,JFZT,SFTZ,JFCS,YXQS,YXQZ,CREATE_TIME,CZ_SHID,OPEN_ID,CONFIRM_RECIVEMONEY,FEE_ID)VALUES(";
                        sql += GetSqlStr(Guid.NewGuid());
                        sql += GetSqlStr(0, 1);//物业费
                        sql += GetSqlStr(dr["FWID"]);
                        sql += GetSqlStr(WYJZ);
                        sql += GetSqlStr(0, 1);//缴费状态 0 否
                        sql += GetSqlStr(0, 1);//是否通知 0 否
                        sql += GetSqlStr(0, 1);//催缴次数 0 
                        sql += GetSqlStr(JZR.ToString("yyyyMMdd"));//首次提醒的开始时间应该是根据物业缴费方式和物业基准日期推算出的一个日子
                        sql += GetSqlStr(EndTime.ToString("yyyyMMdd"));//结束时间应该为根据缴费方式和开始时间推算出的一个日子
                        sql += GetSqlStr(datetime.ToString("yyyyMMdd"));
                        sql += GetSqlStr(dr["CZ_SHID"]);
                        sql += GetSqlStr(dr["OPEN_ID"]);
                        sql += GetSqlStr(0, 1);
                        sql += GetSqlStr(dr["FEE_ID"]);
                        sql = sql.TrimEnd(',') + ")";
                        list.Add(sql);
                    }
                }
             }
            return list;
        }

        public Dictionary<string, object> ConfirmNotificationList(List<Dictionary<string,object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                MsgHelper mh = new MsgHelper();
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                Configuration = builder.Build();
                string templateid = Configuration.GetSection("template").GetSection("confirm").Value;
                foreach (Dictionary<string, object> d in list)
                {
                    if (d["OPEN_ID"]!= null)
                    {
                        Task.Run(async () =>
                        {
                           string str=await mh.SendMsg(url, d["OPEN_ID"].ToString(), d, templateid);
                        });
                    }
                    
                };       
                //do something 需要加入缴费通知
                string b = db.ConfirmNotificationList(list);
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

        public Dictionary<string, object> PushNotification(List<Dictionary<string,object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                MsgHelper mh = new MsgHelper();
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                Configuration = builder.Build();
                string templateid = Configuration.GetSection("template").GetSection("PushNotification").Value;
                foreach (Dictionary<string, object> d in list)
                {
                    if (d["OPEN_ID"].ToString() != "")
                    {
                        Task.Run(async () =>
                        {
                            await mh.SendMsg(url, d["OPEN_ID"].ToString(), d, templateid);
                        });
                    }

                };
                //do something 需要加入推送欠费通知
                string b = db.PushNotification(list);
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
        public Dictionary<string, object> GetHistoryFeeResult(string JFLX, string FWMC, string FWBH, string JFSTATUS,int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetHistoryFeeResult(JFLX, FWMC, FWBH, JFSTATUS);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = "成功！,但是没有数据!";
                    r["total"] = 0;
                    r["code"] = 2000;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string, object> GetBadFeeResult(string JFLX, string FWMC, string FWBH,int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetBadFeeResult(JFLX, FWMC, FWBH);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = "成功！,但是没有数据!";
                    r["total"] = 0;
                    r["code"] = 2000;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string, object> ConfirmFee(string RECORD_ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.ConfirmFee(RECORD_ID);
                if (b=="")
                {
                    r["message"] = "成功！";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = 2000;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string, object> ConfirmReciveMoney(List<Dictionary<string, object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                MsgHelper mh = new MsgHelper();
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                Configuration = builder.Build();
                string templateid = Configuration.GetSection("template").GetSection("ConfirmReciveMoney").Value;
                foreach (Dictionary<string, object> d in list)
                {
                    if (d["OPEN_ID"].ToString() != "")
                    {
                        Task.Run(async () =>
                        {
                            await mh.SendMsg(url, d["OPEN_ID"].ToString(), d, templateid);
                        });
                    }

                };
                //do something 需要加入确认收据推送功能
                string b = db.ConfirmReciveMoney(list);
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

        public Dictionary<string, object> PayOff(List<Dictionary<string, object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                //do something 需要加入确认收据推送功能
                string b = db.PayOff(list);
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



        public Dictionary<string,object> ExportFeeResult(string JFSTATUS)
        {

            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetFeeResult("", "", "", JFSTATUS);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功！";
                    r["total"] = dt.Rows.Count;
                    r["items"] = dt;
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = "成功！,但是没有数据!";
                    r["total"] = 0;
                    r["code"] = 2000;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
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
