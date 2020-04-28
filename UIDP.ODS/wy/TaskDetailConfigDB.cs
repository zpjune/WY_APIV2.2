using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class TaskDetailConfigDB
    {
        DBTool db = new DBTool("");
        public DataTable GetTaskDetailConfig()
        {
            return db.GetDataTable("select * from wy_task_detail_config");
        }
        public string CreateTaskDetailConfig(Dictionary<string,object> d)
        {
            string sql = "INSERT INTO wy_task_detail_config(ID,ParentID,Code,Name)VALUES(";
            sql += GetSqlStr(Guid.NewGuid());
            sql += GetSqlStr(d["ParentID"]);
            sql += GetSqlStr(d["Code"]);
            sql += GetSqlStr(d["Name"]);
            sql = sql.TrimEnd(',') + ")";
            return db.ExecutByStringResult(sql);
        }

        public string UpdateTaskDetailConfig(Dictionary<string,object> d)
        {
            string sql = "UPDATE wy_task_detail_config SET ParentID=" + GetSqlStr(d["ParentID"]);
            sql += " Code=" + GetSqlStr(d["Code"]);
            sql += " Name=" + GetSqlStr(d["Name"]);
            sql = sql.TrimEnd(',') + " WHERE ID='" + d["ID"] + "'";
            return db.ExecutByStringResult(sql);
        }

        public string DeleteTaskDetailConfig(string ID)
        {
            return db.ExecutByStringResult("delete from wy_task_detail_config where ID='" + ID + "'");
        }

        public DataTable GetParentCodeConfig()
        {
            return db.GetDataTable("select ID,Name,Code from wy_task_detail_config where ParentID IS NULL OR ParentID=''");
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
