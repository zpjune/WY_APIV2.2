using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class TaskDB
    {
        DBTool db = new DBTool("");
        public DataTable GetTaskInfo(string RWBH,string RWMC)
        {
            string sql = "select  a.*,group_concat(c.Name separator '-') AS NAME  from wy_check_task a" +
                " join wy_map_checkplandetail b on a.TASK_ID=b.TASK_ID " +
                " JOIN v_taskregion c ON b.PLAN_DETAIL_ID=c.PLAN_DETAIL_ID" +
                " where IS_DELETE=0";
            if (!string.IsNullOrEmpty(RWBH))
            {
                sql += " AND RWBH='" + RWBH + "'";
            }
            if (!string.IsNullOrEmpty(RWMC))
            {
                sql += " AND RWMC like'%" + RWMC + "%'";
            }
            sql += " GROUP BY a.TASK_ID";
            sql += " ORDER BY a.RWKSSJ DESC";
            return db.GetDataTable(sql);
        }
        public string getRWBH() {
            string rwbh = "";
            try
            {
                DateTime d = DateTime.Now;
                 rwbh = d.ToString("yyyyMMdd");
                string dtnow = d.ToString("yyyy-MM-dd");
                string sql = "select count(*)+1 from wy_check_task WHERE TIMESTAMPDIFF(MONTH,CJSJ,'" + dtnow + "')=0";
                DataTable dt = db.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    rwbh = rwbh + (dt.Rows[0][0].ToString().PadLeft(3, '0'));
                }
                else
                {
                    rwbh = rwbh + "00001";
                }
            }
            catch (Exception ex)
            {
                
            }
            return rwbh;
        }
        public string CreateTask(Dictionary<string,object>d)
        {
            string rwbh = getRWBH();
            if (rwbh=="") {
                return "任务编号生成失败";
            }
            Guid TASK_ID = Guid.NewGuid();
            List<string> list = new List<string>();
            string sql = "INSERT INTO wy_check_task (TASK_ID,PLAN_DETAIL_ID,RWBH,RWMC,RWKSSJ,RWJSSJ,RWNR,RWFW,REMARK,CJR,CJSJ,IS_DELETE,IS_PUSH)VALUES(";
            sql += GetSqlStr(TASK_ID);
            sql += GetSqlStr(d["PLAN_DETAIL_ID"]);
            sql += GetSqlStr(rwbh);
            sql += GetSqlStr(d["RWMC"]);
            sql += GetSqlStr(d["RWKSSJ"]);
            sql += GetSqlStr(d["RWJSSJ"]);
            sql += GetSqlStr(d["RWNR"]);
            sql += GetSqlStr(d["RWFW"]);
            sql += GetSqlStr(d["REMARK"]);
            sql += GetSqlStr(d["userId"]);
            sql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            sql += GetSqlStr(0,1);
            sql += GetSqlStr(0, 1);
            sql = sql.TrimEnd(',') + ")";
            list.Add(sql);
            string[] arr = JArray.FromObject(d["PLAN_DETAIL_ARR"]).ToObject<string[]>();
            foreach (string str in arr)
            {
                string mapsql = "INSERT INTO wy_map_checkplandetail(TASK_ID,PLAN_DETAIL_ID)values(";
                mapsql += GetSqlStr(TASK_ID);
                mapsql += GetSqlStr(str);
                mapsql = mapsql.TrimEnd(',') + ")";
                list.Add(mapsql);
            }
            return db.Executs(list);
        }

        public string UpdateTask(Dictionary<string,object>d)
        {
            List<string> sqllist = new List<string>();
            string Updatesql = "UPDATE wy_check_task SET PLAN_DETAIL_ID=" + GetSqlStr(d["PLAN_DETAIL_ID"]);
            Updatesql += "RWBH=" + GetSqlStr(d["RWBH"]);
            Updatesql += "RWMC=" + GetSqlStr(d["RWMC"]);
            Updatesql += "RWKSSJ=" + GetSqlStr(d["RWKSSJ"]);
            Updatesql += "RWJSSJ=" + GetSqlStr(d["RWJSSJ"]);
            Updatesql += "RWNR=" + GetSqlStr(d["RWNR"]);
            Updatesql += "RWFW=" + GetSqlStr(d["RWFW"]);
            Updatesql += "REMARK=" + GetSqlStr(d["REMARK"]);
            Updatesql += "BJR=" + GetSqlStr(d["userId"]);
            Updatesql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            Updatesql = Updatesql.TrimEnd(',');
            Updatesql += " WHERE TASK_ID='" + d["TASK_ID"] + "'";
            sqllist.Add(Updatesql);
            string DelSql = "DELETE FROM wy_map_checkplandetail WHERE TASK_ID='" + d["TASK_ID"] + "'";
            sqllist.Add(DelSql);
            string[] arr = JArray.FromObject(d["PLAN_DETAIL_ARR"]).ToObject<string[]>();
            foreach (string str in arr)
            {
                string mapsql = "INSERT INTO wy_map_checkplandetail(TASK_ID,PLAN_DETAIL_ID)values(";
                mapsql += GetSqlStr(d["TASK_ID"]);
                mapsql += GetSqlStr(str);
                mapsql = mapsql.TrimEnd(',') + ")";
                sqllist.Add(mapsql);
            }
            return db.Executs(sqllist);
        }

        public string DeleteTask(string TASK_ID)
        {
            string sql = "UPDATE wy_check_task SET IS_DELETE=1 WHERE TASK_ID='" + TASK_ID + "'";
            return db.ExecutByStringResult(sql);
        }

        public string PushTask(string TASK_ID)
        {
            string sql = "UPDATE wy_check_task SET IS_PUSH=1 WHERE TASK_ID='" + TASK_ID + "'";
            return db.ExecutByStringResult(sql);
        }

        public DataSet GetPlanCheckAndDetail(string TASK_ID)
        {
            string CheckPlanDetailSql = "select a.*,b.NAME AS ALLPLACENAME,c.NAME AS JCNAME, " +
                " (SELECT count(*) FROM wy_map_checkplandetail WHERE PLAN_DETAIL_ID=a.PLAN_DETAIL_ID )AS ZXCS " +
                " from wy_checkPlan_detail a" +
                " JOIN wy_map_checkplandetail d ON a.PLAN_DETAIL_ID=d.PLAN_DETAIL_ID" +
                " left join V_TaskRegion b on a.PLAN_DETAIL_ID= b.PLAN_DETAIL_ID " +
                " left join wy_task_detail_config c on c.Code=a.JCLX" +
                " where d.TASK_ID='" + TASK_ID + "'";
            string CheckPlanSql = "select * from wy_checkPlan where PLAN_ID=(" +
                " SELECT DISTINCT PLAN_ID FROM wy_checkPlan_detail WHERE PLAN_DETAIL_ID =(" +
                " SELECT PLAN_DETAIL_ID FROM wy_map_checkplandetail WHERE TASK_ID ='" + TASK_ID + "' limit 1))";
            Dictionary<string, string> list = new Dictionary<string, string>()
            {
                {"CheckPlanDetailSql",CheckPlanDetailSql},
                {"CheckPlanSql",CheckPlanSql }
            };
            return db.GetDataSet(list);
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
                    if (t.GetType().FullName.Trim().ToLower() == "system.datetime")
                    {
                        t = Convert.ToDateTime(t).ToString("yyyy-MM-dd");
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
