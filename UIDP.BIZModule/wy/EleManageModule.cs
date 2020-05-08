using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
   public class EleManageModule
    {
        EleManageDB db = new EleManageDB();
        public Dictionary<string, object> GetData(int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetData();
                if (dt.Rows.Count > 0)
                {
                    
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit); ;
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
    }
}
