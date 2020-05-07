using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class CheckResultModule
    {
        CheckResultDB db = new CheckResultDB();

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
