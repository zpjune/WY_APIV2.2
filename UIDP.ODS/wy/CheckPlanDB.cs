using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class CheckPlanDB
    {
        DBTool db = new DBTool("");

        public DataTable GetCheckPlan(string JHMC,string JHND)
        {
            string sql = " SELECT * from wy_checkPlan where IS_DELETE=0";
            if (!string.IsNullOrEmpty(JHMC))
            {
                sql += " and JHMC='" + JHMC + "'";
            }
            if (!string.IsNullOrEmpty(JHND))
            {
                sql += " and JHND=" + JHND;
            }
            return db.GetDataTable(sql);
        }

        public DataTable GetCheckPlanDetail(string PLAN_ID)
        {
            return db.GetDataTable("SELECT a.*,b.NAME,c.Name AS JCNAME," +
                " (SELECT count(*) FROM wy_map_checkplandetail WHERE PLAN_DETAIL_ID=a.PLAN_DETAIL_ID )	AS ZXCS" +
                " FROM wy_checkPlan_detail a " +
                " left join V_TaskRegion b on a.PLAN_DETAIL_ID=b.PLAN_DETAIL_ID" +
                " left join wy_task_detail_config c on c.Code=a.JCLX" +
                " WHERE PLAN_ID='" + PLAN_ID + "'AND IS_DELETE=0");
        }

        public string CreateCheckPlan(Dictionary<string,object> d)
        {
            List<string> SqlList = new List<string>();
            string PLAN_ID = Guid.NewGuid().ToString();
            string CheckPlanSql = "INSERT INTO wy_checkPlan (PLAN_ID,JHND,JHMC,JHSM,JHSJ,REMARK,CJR,CJSJ,IS_DELETE)values(";
            CheckPlanSql += GetSqlStr(PLAN_ID);
            CheckPlanSql += GetSqlStr(d["JHND"]);
            CheckPlanSql += GetSqlStr(d["JHMC"]);
            CheckPlanSql += GetSqlStr(d["JHSM"]);
            CheckPlanSql += GetSqlStr(d["JHSJ"]);
            CheckPlanSql += GetSqlStr(d["REMARK"]);
            CheckPlanSql += GetSqlStr(d["userId"]);
            CheckPlanSql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            CheckPlanSql += GetSqlStr(0,1);
            CheckPlanSql = CheckPlanSql.TrimEnd(',') + ")";
            SqlList.Add(CheckPlanSql);
            foreach(Dictionary<string,object> CheckPlanDetail in JArray.FromObject(d["CheckPlanDetail"]).ToObject<List<Dictionary<string, object>>>())
            {
                if (CheckPlanDetail.ContainsValue(""))
                {
                    return "您输入的详细计划内有空值！请仔细修改后再提交表单";
                }
                else
                {
                    string PLAN_DETAIL_ID = Guid.NewGuid().ToString();
                    string DetailSql = "INSERT INTO wy_checkPlan_detail(PLAN_DETAIL_ID,PLAN_ID,JCQY,JCNR,JCLX,PCCS,CJR,CJSJ,IS_DELETE)VALUES(";
                    string JHQYstr = "";
                    foreach(string jhqy in JArray.FromObject(CheckPlanDetail["JCQY"]).ToObject<string[]>())
                    {
                        JHQYstr += jhqy + ",";
                        string mapSql = "INSERT INTO wy_map_region(PLAN_DETAIL_ID,REGION_CODE)values(";
                        mapSql += GetSqlStr(PLAN_DETAIL_ID);
                        mapSql += GetSqlStr(jhqy);
                        mapSql = mapSql.TrimEnd(',') + ")";
                        SqlList.Add(mapSql);
                    }
                    DetailSql += GetSqlStr(PLAN_DETAIL_ID);
                    DetailSql += GetSqlStr(PLAN_ID);
                    DetailSql += GetSqlStr(JHQYstr);
                    DetailSql += GetSqlStr(CheckPlanDetail["JCNR"]);
                    DetailSql += GetSqlStr(CheckPlanDetail["JCLX"]);
                    DetailSql += GetSqlStr(CheckPlanDetail["PCCS"], 1);
                    DetailSql += GetSqlStr(d["userId"]);
                    DetailSql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                    DetailSql += GetSqlStr(0, 1);
                    DetailSql = DetailSql.TrimEnd(',') + ")";
                    SqlList.Add(DetailSql);
                }
                
            }
            return db.Executs(SqlList);
        }


        public string UpdateCheckPlan(Dictionary<string,object> d)
        {
            List<string> SqlList = new List<string>();
            string CheckPlanSql = "UPDATE wy_checkPlan SET JHND="+GetSqlStr(d["JHND"]);
            CheckPlanSql += "JHMC=" + GetSqlStr(d["JHMC"]);
            CheckPlanSql += "JHSM=" + GetSqlStr(d["JHSM"]);
            CheckPlanSql += "JHSJ=" + GetSqlStr(d["JHSJ"]);
            CheckPlanSql += "REMARK=" + GetSqlStr(d["REMARK"]);
            CheckPlanSql += "BJR=" + GetSqlStr(d["userId"]);
            CheckPlanSql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            CheckPlanSql += "IS_DELETE=" + GetSqlStr(0,1);
            CheckPlanSql = CheckPlanSql.TrimEnd(',');
            CheckPlanSql += " WHERE PLAN_ID='" + d["PLAN_ID"] + "'";
            SqlList.Add(CheckPlanSql);
            foreach (Dictionary<string, object> CheckPlanDetail in JArray.FromObject(d["CheckPlanDetail"]).ToObject<List<Dictionary<string, object>>>())
            {
                if(!CheckPlanDetail.ContainsKey("PLAN_DETAIL_ID"))
                {
                    if (CheckPlanDetail.ContainsValue(""))
                    {
                        return "您输入的详细计划内有空值！请仔细修改后再提交表单";
                    }
                    else
                    {
                        string PLAN_DETAIL_ID = Guid.NewGuid().ToString();
                        string DetailSql = "INSERT INTO wy_checkPlan_detail(PLAN_DETAIL_ID,PLAN_ID,JCQY,JCNR,JCLX,PCCS,CJR,CJSJ,IS_DELETE)VALUES(";
                        string JHQYstr = "";
                        foreach (string jhqy in JArray.FromObject(CheckPlanDetail["JCQY"]).ToObject<string[]>())
                        {
                            JHQYstr += jhqy + ",";
                            string mapSql = "INSERT INTO wy_map_region(PLAN_DETAIL_ID,REGION_CODE)values(";
                            mapSql += GetSqlStr(PLAN_DETAIL_ID);
                            mapSql += GetSqlStr(jhqy);
                            mapSql = mapSql.TrimEnd(',') + ")";
                            SqlList.Add(mapSql);
                        }
                        DetailSql += GetSqlStr(PLAN_DETAIL_ID);
                        DetailSql += GetSqlStr(d["PLAN_ID"]);
                        DetailSql += GetSqlStr(JHQYstr);
                        DetailSql += GetSqlStr(CheckPlanDetail["JCNR"]);
                        DetailSql += GetSqlStr(CheckPlanDetail["JCLX"]);
                        DetailSql += GetSqlStr(CheckPlanDetail["PCCS"], 1);
                        DetailSql += GetSqlStr(d["userId"]);
                        DetailSql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                        DetailSql += GetSqlStr(0, 1);
                        DetailSql = DetailSql.TrimEnd(',') + ")";
                        SqlList.Add(DetailSql);
                    }                       
                }
                else
                {
                    //if (CheckPlanDetail.ContainsValue(""))
                    //{
                    //    return "您输入的详细计划内有空值！请仔细修改后再提交表单";
                    //}
                    //else
                    //{
                        if (CheckPlanDetail["IS_DELETE"].ToString() == "0")
                        {
                            string DelMapSql = "DELETE FROM wy_map_region WHERE PLAN_DETAIL_ID='" + CheckPlanDetail["PLAN_DETAIL_ID"] + "'";
                            SqlList.Add(DelMapSql);
                            string JHQYstr = "";
                            foreach (string jhqy in JArray.FromObject(CheckPlanDetail["JCQY"]).ToObject<string[]>())
                            {
                                JHQYstr += jhqy + ",";
                                string mapSql = "INSERT INTO wy_map_region(PLAN_DETAIL_ID,REGION_CODE)values(";
                                mapSql += GetSqlStr(CheckPlanDetail["PLAN_DETAIL_ID"]);
                                mapSql += GetSqlStr(jhqy);
                                mapSql = mapSql.TrimEnd(',') + ")";
                                SqlList.Add(mapSql);
                            }
                        string DetailSql = "UPDATE wy_checkPlan_detail SET JCQY=" + GetSqlStr(JHQYstr);
                        DetailSql += " JCNR=" + GetSqlStr(CheckPlanDetail["JCNR"]);
                        DetailSql += " JCLX=" + GetSqlStr(CheckPlanDetail["JCLX"]);
                        DetailSql += " PCCS=" + GetSqlStr(CheckPlanDetail["PCCS"], 1);
                        DetailSql += " BJR=" + GetSqlStr(d["userId"]);
                        DetailSql += " BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
                        DetailSql = DetailSql.TrimEnd(',') + " where PLAN_DETAIL_ID='" + CheckPlanDetail["PLAN_DETAIL_ID"] + "'";
                        SqlList.Add(DetailSql);
                    }
                        else
                        {

                            string DetailSql = "UPDATE wy_checkPlan_detail SET IS_DELETE=1 " +
                                " where PLAN_DETAIL_ID='" + CheckPlanDetail["PLAN_DETAIL_ID"] + "'";
                            SqlList.Add(DetailSql);
                            string DelMapSql = "DELETE FROM wy_map_region WHERE PLAN_DETAIL_ID='" + CheckPlanDetail["PLAN_DETAIL_ID"] + "'";
                            SqlList.Add(DelMapSql);
                        }
                        
                    //}
                        
                } 
            }
            return db.Executs(SqlList);
        }

        public string DeleteCheckPlanDetail(string PLAN_DETAIL_ID)
        {
            return db.ExecutByStringResult("UPDATE wy_checkPlan_detail SET IS_DELETE=1 WHERE PLAN_DETAIL_ID='" + PLAN_DETAIL_ID + "'");
        }

        public string DeleteCheckPlan(string PLAN_ID)
        {
            return db.Executs(new List<string>()
            {
                { "update wy_checkPlan set IS_DELETE=1 WHERE PLAN_ID='" + PLAN_ID + "'"},
                { "update wy_checkPlan_detail set IS_DELETE=1 WHERE PLAN_ID='" + PLAN_ID + "'"}
            });
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
