using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS
{
    public class TaxConfigDB
    {
        DBTool db = new DBTool("MYSQL");
        public DataTable getData()
        {
            string sql = "SELECT * FROM tax_dictionary ORDER BY Code,SortNo";
            return db.GetDataTable(sql);
        }

        public string editNode(Dictionary<string,object> d)
        {
            string sql = "UPDATE tax_dictionary SET ParentCode='" + d["ParentCode"] + "',";
            sql += "Code='" + d["Code"] + "',";
            sql += "Name='" + d["Name"] + "',";
            sql += "S_UpdateBy='" + d["S_UpdateBy"]+"',";
            sql += "S_UpdateDate='" + d["S_UpdateDate"] + "'";
            if (d["EnglishCode"]!=null&& d["EnglishCode"].ToString() != "")
            {
                sql += ",EnglishCode='" + d["EnglishCode"] + "'";
            }
            if(d["SortNo"]!=null&& d["SortNo"].ToString() != "")
            {
                sql += ",SortNo=" + d["SortNo"] + "";
            }
            sql += " WHERE S_Id='" + d["S_Id"] + "'";
            return db.ExecutByStringResult(sql);
        }

        public string createNode(Dictionary<string, object> d)
        {
            StringBuilder sql = new StringBuilder();
            //string sql = "INSERT INTO tax_dictionary(S_Id,S_CreateDate,S_CreateBy,ParentCode,Code,Name,EnglishCode,SortNo)VALUES(";
            sql.Append("INSERT INTO tax_dictionary(S_Id,S_CreateDate,S_CreateBy,ParentCode,Code,Name,EnglishCode,SortNo)VALUES(");
            sql.Append(GetSqlStr(d["S_Id"]));
            sql.Append(GetSqlStr(d["S_CreateDate"]));
            sql.Append(GetSqlStr(d["S_CreateBy"]));
            if (d.ContainsKey("ParentCode"))
            {
                sql.Append(GetSqlStr(d["ParentCode"]));
            }
            else
            {
                sql.Append(GetSqlStr(""));
            }
            sql.Append(GetSqlStr(d["Code"]));
            sql.Append(GetSqlStr(d["Name"]));
            sql.Append(GetSqlStr(d["EnglishCode"]));
            sql.Append(GetSqlStr(d["SortNo"],1));
            return db.ExecutByStringResult(sql.ToString().TrimEnd(',') + ")");
        }

        public DataTable getRepeatInfo(Dictionary<string, object> d)
        {
            string sql = "SELECT * FROM  tax_dictionary WHERE 1=1";
            sql += " AND Code='" + d["Code"] + "'";
            if (d.ContainsKey("ParentCode"))
            {
                sql += " AND ParentCode='" + d["ParentCode"] + "'";
            }
            
            //sql+=" OR Name='" + d["Name"] + "'";
            return db.GetDataTable(sql);
        }

        public string delNode(Dictionary<string,object> d)
        {
            string sql = "DELETE FROM tax_dictionary WHERE S_Id='" + d["S_Id"] + "'";
            return db.ExecutByStringResult(sql);
        }

        public DataTable search(string param)
        {
            string sql = "SELECT * FROM tax_dictionary WHERE Code='" + param + "'" + " OR Name='" + param + "'" + " OR EnglishCode='" + param + "'";
            return db.GetDataTable(sql);
        }

        public DataTable GetOptions(string ParentCode)
        {
            string sql = "select Code,Name from tax_dictionary where ParentCode='" + ParentCode + "' ORDER BY SortNo";
            return db.GetDataTable(sql);
        }
        /// <summary>
        /// 构造sql字符串方法
        /// </summary>
        /// <param name="t">data</param>
        /// <param name="type">类型，0为字符类型</param>
        /// <returns></returns>
        public string GetSqlStr(object t,int type=0)
        {
            if (t==null||t.ToString()=="")
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
