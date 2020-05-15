using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class CheckReportModule
    {
        CheckReportDB db = new CheckReportDB();
        public Dictionary<string, object> GetCheckResult(string year, string FWBH, string RWMC, string JCJG,string DLID,int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetCheckResult(year, FWBH, RWMC, JCJG,DLID);
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

        public Dictionary<string, object> GetCheckResultDetail(string RESULT_ID)
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

        public Dictionary<string,object> GetParentCheckCodeOptions()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetParentCheckCodeOptions();
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["items"] = dt;
                    r["total"] = dt.Rows.Count;
                    r["message"] = "成功";
                }
                else
                {
                    r["code"] = -1;
                    r["message"] = "成功,但是没有数据";
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }


        public Dictionary<string, object> ExportTotalCheckReport(string year, string FWBH, string RWMC, string JCJG, string DLID, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.ExportTotalCheckReport(year, FWBH, RWMC, JCJG, DLID);
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
                    r["code"] = 2001;
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

        public Dictionary<string,object> WorkloadStatistics(string date, string FZR,int page,int limit,int type = 0)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.WorkloadStatistics(date, FZR, type);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                    r["total"] = dt.Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "成功,但是没有数据！";
                    r["total"] = 0;
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

        public Dictionary<string, object> WorkloadStatisticsDetail(string RD_ID, string yyyy, string mon)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.WorkloadStatisticsDetail(RD_ID,yyyy,mon);
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
                    r["message"] = "成功,但是没有数据！";
                    r["total"] = 0;
                    r["items"] = new DataTable();
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string,object> ShopCheckSummary(string SSQY,string FWBH,string FWMC,string date,int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataSet ds = db.ShopCheckSummary(SSQY,FWBH,FWMC,date);
                if (ds.Tables[1].Rows.Count > 0)
                {
                    r["message"] = "成功!";
                    r["code"] = 2000;
                    r["ColumnInfo"] = ds.Tables[0];
                    r["DataInfo"] = KVTool.GetPagedTable(ds.Tables[1], page, limit);
                    r["total"] = ds.Tables[1].Rows.Count;
                }
                else
                {
                    r["code"] = 2000;
                    r["message"] = "成功,但是没有数据!";
                    r["ColumnInfo"] = ds.Tables[0];
                    r["DataInfo"] = KVTool.GetPagedTable(ds.Tables[1], page, limit);
                    r["total"] = ds.Tables[1].Rows.Count;
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
