using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.UTILITY;

namespace UIDP.ODS.wy
{
    public class EleManageDB
    {
        DBTool db = new DBTool("");
        public DataTable GetData() {
            string sql = "select * from wy_ele_recharge where CStatus='FAIL' ";
            return db.GetDataTable(sql);
        }
        public string update(string sql){
            return db.ExecutByStringResult(sql);
}
    }
}
