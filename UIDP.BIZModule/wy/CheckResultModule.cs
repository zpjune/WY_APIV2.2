using House.IService.Common.Message;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class CheckResultModule
    {
        CheckResultDB db = new CheckResultDB();
        public static IConfiguration Configuration { get; set; }
        public Dictionary<string,object> GetCheckResult(string year, string FWBH, string RWMC,string JCJG,int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetCheckResult(year, FWBH, RWMC,JCJG);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;                   
                }
                else
                {
                    r["message"] = "成功,但是没有数据";
                    r["code"] = 2000;
                    r["total"] = 0;
                    r["items"] = new DataTable();
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string,object> GetCheckResultDetail(string RESULT_ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetCheckResultDetail(RESULT_ID);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["message"] = "成功,但是没有数据";
                    r["code"] = 2000;
                    r["total"] = 0;
                    r["items"] = new DataTable();
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
        public Dictionary<string,object> Rectification(string[] arr)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataSet ds = db.Rectification(arr);
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                Configuration = builder.Build();
                string templateid = Configuration.GetSection("template").GetSection("Rectification").Value;
                string url= Configuration.GetSection("msgUrl").GetSection("url").Value;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string detailstr = string.Empty;
                    Dictionary<string, object> d = new Dictionary<string, object>();
                    d["first"] = "尊敬的用户" + dr["ZHXM"] + "您好,您的房屋整改情况如下:";
                    d["keyword1"] = dr["FWBH"]+"-"+dr["FWMC"];
                    d["keyword2"] = dr["JCSJ"];
                    d["keyword3"] = "总体检查不合格,检查明细如下:";
                    foreach (DataRow ddr in ds.Tables[1].Select("RESULT_ID='" + dr["RESULT_ID"] + "'"))
                    {     
                        if (ddr["CHECK_DETAIL_RESULT"].ToString().Trim() == "0")
                        {
                            detailstr += ddr["Name"] + "(不合格);";
                        }
                        else
                        {
                            detailstr += ddr["Name"] + "(合格);";
                        }
                    }
                    d["keyword3"] += detailstr;
                    Task.Run(async () =>
                    {
                        string str= await MsgHelper.Msg.SendMsg(url, dr["OPEN_ID"].ToString(), d, templateid);
                        db.InsertLog(str, "检查请求");
                    });
                    //var str= MsgHelper.Msg.SendMsg(url, dr["OPEN_ID"].ToString(), d, templateid).Result;
                }
                r["code"] = 2000;
                r["message"] = "成功!";
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }


        public Dictionary<string, object> GetTaskProcessInfo(string year, string RWMC, string RWBH, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetTaskProcessInfo(year, RWMC, RWBH);
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["message"] = "成功,但是没有数据";
                    r["code"] = 2000;
                    r["total"] = 0;
                    r["items"] = new DataTable();
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
    }
}
