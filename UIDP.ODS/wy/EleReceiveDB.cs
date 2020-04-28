using System;
using System.Collections.Generic;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class EleReceiveDB
    {
        DBTool db = new DBTool("");
        public void UpdateEle(string sql){
            db.ExecutByStringResult(sql);
        }
        public void AddLog(string ACCESS_TIME,string USER_NAME,string LOG_TYPE,string LOG_CONTENT) {
            string sql = "insert into ts_uidp_loginfo(ACCESS_TIME,USER_NAME,LOG_TYPE,LOG_CONTENT)values ('";
            sql += ACCESS_TIME;
            sql += "','";
            sql += USER_NAME;
            sql += "',";
            sql += LOG_TYPE;
            sql += ",'";
            sql += LOG_CONTENT;
            sql += "')";
            db.Execut(sql);
        }
    }
}
