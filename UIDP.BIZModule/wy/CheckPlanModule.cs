using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.BIZModule.wy.Models;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class CheckPlanModule
    {
        CheckPlanDB db = new CheckPlanDB();
        public Dictionary<string,object> GetCheckPlan(string JHMC, string JHND, int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetCheckPlan(JHMC, JHND);
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
                    r["message"] = "成功，但是没有数据";
                }
            }
            catch(Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }


        public Dictionary<string, object> GetCheckPlanDetail(string PLAN_ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetCheckPlanDetail(PLAN_ID);
                if (dt.Rows.Count > 0)
                {
                    List<CheckPlanDetailModel> list = new List<CheckPlanDetailModel>();
                    foreach(DataRow dr in dt.Rows)
                    {
                        CheckPlanDetailModel item = new CheckPlanDetailModel();
                        item.PLAN_DETAIL_ID=dr["PLAN_DETAIL_ID"].ToString();
                        item.PLAN_ID = dr["PLAN_ID"].ToString();
                        item.JCQY = dr["JCQY"].ToString().TrimEnd(',').Split(',');
                        item.JCNR = dr["JCNR"].ToString();
                        item.JCLX = dr["JCLX"].ToString();
                        item.PCCS = dr["PCCS"]==null?0:Convert.ToInt32(dr["PCCS"].ToString());
                        item.CJR = dr["CJR"].ToString();
                        item.CJSJ = dr["CJSJ"].ToString();
                        item.BJR = dr["BJR"].ToString();
                        item.BJSJ = dr["BJSJ"].ToString();
                        item.IS_DELETE =Convert.ToInt32(dr["IS_DELETE"].ToString());
                        item.ALLPLACENAME = dr["NAME"].ToString();
                        item.JCNAME = dr["JCNAME"].ToString();
                        list.Add(item);
                    }
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = list;
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

        public Dictionary<string,object> CreateCheckPlan(Dictionary<string,object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.CreateCheckPlan(d);
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

        public Dictionary<string, object> UpdateCheckPlan(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UpdateCheckPlan(d);
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
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> DeleteCheckPlanDetail(string PLAN_DETAIL_ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteCheckPlanDetail(PLAN_DETAIL_ID);
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
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> DeleteCheckPlan(string PLAN_ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteCheckPlan(PLAN_ID);
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
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }
    }
}
