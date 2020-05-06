using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class RegionDirectorDB
    {
        DBTool db = new DBTool("");

        public DataTable GetRegionDirector(string SSQY,string FZR)
        {
            string sql = "SELECT a.*,b.Name from wy_region_director a " +
                " left join tax_dictionary b on a.SSQY=b.Code AND b.ParentCode='SSQY'" +
                " where a.IS_DELETE=0";
            if (!string.IsNullOrEmpty(SSQY))
            {
                sql += " AND a.SSQY='" + SSQY + "'";
            }
            if (!string.IsNullOrEmpty(FZR))
            {
                sql += " AND a.FZR like'%" + FZR + "%'";
            }
            return db.GetDataTable(sql);
        }

        public string CreateRegionDirector(Dictionary<string,object> d)
        {
            string sql = "INSERT INTO wy_region_director(RD_ID,SSQY,FZR,MOBILE,CJR,CJSJ,IS_DELETE)VALUES(";
            sql += GetSqlStr(Guid.NewGuid());
            sql += GetSqlStr(d["SSQY"]);
            sql += GetSqlStr(d["FZR"]);
            sql += GetSqlStr(d["MOBILE"]);
            sql += GetSqlStr(d["userId"]);
            sql += GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            sql += GetSqlStr(0,1);
            sql = sql.TrimEnd(',') + ")";
            return db.ExecutByStringResult(sql);
        }

        public string UpdateRegionDirector(Dictionary<string,object> d)
        {
            string sql = " UPDATE wy_region_director set SSQY=" + GetSqlStr(d["SSQY"]);
            sql += "FZR=" + GetSqlStr(d["FZR"]);
            sql += "MOBILE=" + GetSqlStr(d["MOBILE"]);
            sql += "BJR=" + GetSqlStr(d["userId"]);
            sql += "BJSJ=" + GetSqlStr(DateTime.Now.ToString("yyyyMMdd"));
            sql = sql.TrimEnd(',');
            sql += " WHERE RD_ID='" + d["RD_ID"] + "'";
            return db.ExecutByStringResult(sql);
        }

        public string DeleteRegionDirector(string RD_ID)
        {
            return db.ExecutByStringResult("UPDATE wy_region_director SET IS_DELETE=1 WHERE RD_ID='" + RD_ID + "'");
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
