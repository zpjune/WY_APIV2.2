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
using System.Linq;

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

        public Dictionary<string, object> PaidFeeResult(string date,int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.PaidFeeResult(date);
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
                DateTime EndTime;
                decimal WYJZ = Convert.ToDecimal(dr["WYJZ"]);
                if (WYJZ == 0)//物业基准为0不生成通知单
                {
                    continue;
                }
                if (dr["IS_CHANGEUSER"].ToString()=="1")//已经生成过缴费确认单的商户,且上个生成的通知单通知的用户和本次查询额房屋用户是同一人
                {
                    if (dr["JFZT"].ToString() == "1")//最近一条记录为已缴费状态
                    {
                        JZR = Convert.ToDateTime(dr["YXQZ"]);

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
                                "(RECORD_ID,JFLX,FWID,JFJE,JFZT,SFTZ,JFCS,YXQS,YXQZ,CREATE_TIME,CZ_SHID,OPEN_ID,CONFIRM_RECIVEMONEY,FEE_ID,WYDJ,REMARK)VALUES(";
                            sql += GetSqlStr(Guid.NewGuid());
                            sql += GetSqlStr(0, 1);//物业费
                            sql += GetSqlStr(dr["FWID"]);
                            sql += GetSqlStr(Math.Round(WYJZ*100)/100);//保留两位小数，向上取
                            sql += GetSqlStr(0,1);//缴费状态 0 否
                            sql += GetSqlStr(0, 1);//是否通知 0 否
                            sql += GetSqlStr(0, 1);//催缴次数 0 
                            sql += GetSqlStr(Convert.ToDateTime(dr["YXQZ"]).ToString("yyyy-MM-dd HH:mm:ss"));//缴费日期有效期起 最新一条已缴费记录的有效期止就是新一条的有效期起
                            sql += GetSqlStr(EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sql += GetSqlStr(datetime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sql += GetSqlStr(dr["CZ_SHID"]);
                            sql += GetSqlStr(dr["OPEN_ID"]);
                            sql += GetSqlStr(0, 1);
                            sql += GetSqlStr(dr["FEE_ID"]);
                            sql += GetSqlStr(dr["WYDJ"],1);
                            sql += GetSqlStr(dr["REMARK"]);
                            sql = sql.TrimEnd(',') + ")";
                            list.Add(sql);
                        }
                    }
                    
                }
                else//对于首次提醒的商户和本次查询的房屋使用者和上次生成的通知单不是同一个物业合同
                {
                    JZR = Convert.ToDateTime(dr["WYJZSJ"]);
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
                               "(RECORD_ID,JFLX,FWID,JFJE,JFZT,SFTZ,JFCS,YXQS,YXQZ,CREATE_TIME,CZ_SHID,OPEN_ID,CONFIRM_RECIVEMONEY,FEE_ID,WYDJ,REMARK)VALUES(";
                        sql += GetSqlStr(Guid.NewGuid());
                        sql += GetSqlStr(0, 1);//物业费
                        sql += GetSqlStr(dr["FWID"]);
                        sql += GetSqlStr(Math.Round(WYJZ * 100) / 100);//保留两位小数，向上取
                        sql += GetSqlStr(0, 1);//缴费状态 0 否
                        sql += GetSqlStr(0, 1);//是否通知 0 否
                        sql += GetSqlStr(0, 1);//催缴次数 0 
                        sql += GetSqlStr(JZR.ToString("yyyy-MM-dd HH:mm:ss"));//首次提醒的开始时间应该是根据物业缴费方式和物业基准日期推算出的一个日子
                        sql += GetSqlStr(EndTime.ToString("yyyy-MM-dd HH:mm:ss"));//结束时间应该为根据缴费方式和开始时间推算出的一个日子
                        sql += GetSqlStr(datetime.ToString("yyyy-MM-dd HH:mm:ss"));
                        sql += GetSqlStr(dr["CZ_SHID"]);
                        sql += GetSqlStr(dr["OPEN_ID"]);
                        sql += GetSqlStr(0, 1);
                        sql += GetSqlStr(dr["FEE_ID"]);
                        sql += GetSqlStr(dr["WYDJ"], 1);
                        sql += GetSqlStr(dr["REMARK"]);
                        sql = sql.TrimEnd(',') + ")";
                        list.Add(sql);
                    }
                }
             }
            return list;
        }

        public Dictionary<string,object> DeleteRecord(string RECORD_ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteRecord(RECORD_ID);
                if (b == "")
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = b;
                }
                
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
        public Dictionary<string, object> ConfirmNotificationList(List<Dictionary<string,object>> list)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                
                //do something 需要加入缴费通知
                string b = db.ConfirmNotificationList(list);
                if (b == "")
                {
                    SendMessage(list);
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
                
                //do something 需要加入推送欠费通知
                string b = db.PushNotification(list);
                if (b == "")
                {
                    SendMessage(list);
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

        public Dictionary<string, object> ConfirmFee(string RECORD_ID, int JFLX,string JFJE,string GMSL)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.ConfirmFee(RECORD_ID,JFLX,JFJE,GMSL);
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
                
                //do something 需要加入确认收据推送功能
                string b = db.ConfirmReciveMoney(list);
                if (b == "")
                {
                    SendMessage(list);
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
                    SendMessage(list, 1);
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

        public Dictionary<string, object> GetPER_WATER_PRICE()
        {

            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetPER_WATER_PRICE();
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
        /// <summary>
        /// 发送通知方法
        /// </summary>
        /// <param name="list">数据list</param>
        /// <param name="SendType">发送类型 0是通知和催缴，1是法律诉讼</param>
        public void SendMessage(List<Dictionary<string, object>> list,int SendType=0)
        {
            try
            {
                string WhereCondition = string.Empty;
                foreach(Dictionary<string,object> d in list)
                {
                    WhereCondition += "'" + d["OPEN_ID"] + "',";
                }
                WhereCondition = WhereCondition.TrimEnd(',');
                DataTable dt = db.GetShopMobilephone(WhereCondition);
                string UserMobilephoneNO = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.IsNull("MOBILE_PHONE"))
                    {
                        continue;
                    }
                    UserMobilephoneNO += dr["MOBILE_PHONE"];
                };
                UserMobilephoneNO = UserMobilephoneNO.TrimEnd(',');
                var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json");
                Configuration = builder.Build();
                string Msgurl = Configuration.GetSection("msgUrl").GetSection("sms").Value;
                string Msgtemplateid = Configuration.GetSection("msgsmstemp").GetSection("smstemp").Value;
                Dictionary<string, string> MapDicName = new Dictionary<string, string>()
                {
                    {"0","物业费" },
                    {"1","水费" },
                    {"2","电费" }
                };
                Dictionary<string, string> MapDicUnit = new Dictionary<string, string>()
                {
                    {"0","元" },
                    {"1","吨" },
                    {"2","元" }
                };
                //string templateid = Configuration.GetSection("template").GetSection("confirm").Value;
                switch (SendType)
                {
                    case 0:
                        foreach (Dictionary<string, object> d in list)
                        {
                            if (d["JFLX"].ToString() == "0")
                            {
                                Dictionary<string, object> jsondata = new Dictionary<string, object>()
                                {
                                    {"first", "尊敬的业主" + d["ZHXM"] + "您好！您的物业费已出。" },
                                    { "userName", d["ZHXM"]},
                                    { "address",d["FWBH"] + "-" + d["FWMC"]},
                                    { "pay",d["JFJE"]},
                                    { "remark","您本次缴纳的物业费有效期为" + Convert.ToDateTime(d["YXQS"]).ToString("yyyy/MM/dd") + "到" + Convert.ToDateTime(d["YXQZ"]).ToString("yyyy/MM/dd") + "请按时缴纳物业费"}
                                };
                                if (d["OPEN_ID"] != null)
                                {
                                    //string str=MsgHelper.Msg.SendMsg(url, d["OPEN_ID"].ToString(), jsondata, Configuration.GetSection("template").GetSection("confirm").Value).Result;
                                    Task.Run(async () =>
                                    {
                                        string str=await MsgHelper.Msg.SendMsg(url, d["OPEN_ID"].ToString(), jsondata, Configuration.GetSection("template").GetSection("confirm").Value);
                                        db.InsertLog(str, "物业费请求");
                                    });
                                }
                            }
                            else
                            {

                                Dictionary<string, object> jsondata = new Dictionary<string, object>()
                                {
                                    {"first","尊敬的用户您好,您的"+MapDicName[d["JFLX"].ToString()]+"余额不足,为了避免您的使用，请尽快充值！" },
                                    {"keyword1",d["ZHXM"] },
                                    {"keyword2",d["FWBH"].ToString()+d["FWMC"].ToString() },
                                    {"keyword3",d["SURPLUSVALUE"]+ MapDicUnit[d["JFLX"].ToString()]},
                                    {"remark","您的"+MapDicName[d["JFLX"].ToString()]+"余额已经不足,请尽快充值！" }
                                 };
                                if (d["OPEN_ID"] != null)
                                {
                                    //string str=MsgHelper.Msg.SendMsg(url, d["OPEN_ID"].ToString(), jsondata, Configuration.GetSection("template").GetSection("WaterAndEleNotice").Value).Result;
                                    Task.Run(async () =>
                                    {
                                        string str=await MsgHelper.Msg.SendMsg(url, d["OPEN_ID"].ToString(), jsondata, Configuration.GetSection("template").GetSection("WaterAndEleNotice").Value);
                                        db.InsertLog(str, "水电请求");
                                    });
                                }
                            }
                        };
                        break;
                    case 1:
                        foreach (Dictionary<string, object> d in list)
                        {
                            Dictionary<string, object> jsondata = new Dictionary<string, object>()

                        {
                            {"first", "尊敬的业主" + d["ZHXM"] + "您好！您有一项新的欠费记录。" },
                            { "keyword1", d["ZHXM"]},
                            { "keyword2",d["FWBH"] + "-" + d["FWMC"]},
                            { "keyword3",MapDicName[d["JFLX"].ToString()]},
                            { "keyword4",d["JFJE"].ToString()+MapDicUnit[d["JFLX"].ToString()]},
                            { "remark","您的欠费账单已经进入法律诉讼状态！"}
                        };
                            if (d["OPEN_ID"] != null)
                            {
                                //string str= MsgHelper.Msg.SendMsg(url, d["OPEN_ID"].ToString(), jsondata, Configuration.GetSection("template").GetSection("LawyerNotice").Value).Result;
                                Task.Run(async () =>
                                {
                                    string str = await MsgHelper.Msg.SendMsg(url, d["OPEN_ID"].ToString(), jsondata, Configuration.GetSection("template").GetSection("LawyerNotice").Value);
                                    db.InsertLog(str, "法律请求");
                                });
                                
                            }
                        }
                        break;
                    default:
                        throw new Exception("未检测到的缴费类型！");
                }
                Task.Run(async () =>
                {
                    string str = await MsgHelper.Msg.SendSMS(UserMobilephoneNO, new string[2] { "", "收费消息" }, Msgurl, Msgtemplateid);
                    db.InsertLog(str, "收费消息通知请求");
                });
            }
            catch (Exception e)
            {
                db.InsertLog(e.Message,"异常插入");
            }
        }
        /// <summary>
        /// 构造sql方法
        /// </summary>
        /// <param name="t">对象值</param>
        /// <param name="type">类型，0字符串 其他为数字</param>
        /// <returns></returns>
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
                        t = Convert.ToDateTime(t).ToString("yyyyMMdd");
                    }
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
