using System;
using System.Collections.Generic;
using System.Text;
using UIDP.BIZModule.wy.Models;
using UIDP.ODS.wy;

namespace UIDP.BIZModule
{
    public class EleReceiveModule
    {
        EleReceiveDB db = new EleReceiveDB();
        public string ReadActiveEnergyBatch(string res) {
            string result = "";
            try
            {
                List<EleResModle> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EleResModle>>(res);
                if (list != null && list.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string dtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    int total = 0;
                    int count = 0;
                    foreach (EleResModle m in list)
                    {
                        total++;
                        sb.Append("update wy_ele_Energy set Ustatus='" + m.status + "',Umessage='" + m.err_msg?.ToString() + "',UpdateDate='" + dtNow + "' ");
                        if (m.data!=null&& m.data.Count>0) {
                            sb.Append(" ,TotalEle=" + m.data[0].value[0]);
                        }
                        sb.Append("  where opr_id='" + m.opr_id + "';");
                        if (total == 998)
                        {
                            db.UpdateEle(sb.ToString());
                            sb.Clear();
                            sb.Length = 0;
                            total = 0;
                            count++;
                        }
                    }
                    if (count == 0 || (count > 0 && total > 0 && total != 998))
                    {
                        db.UpdateEle(sb.ToString());
                    }
                    result= "SUCCESS";
                    db.AddLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "readActiveEnergyBatch()", "10", "chenggong");
                }
            }
            catch (Exception ex)
            {
                db.AddLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "readActiveEnergyBatch()", "10", ex.ToString().Substring(0, ex.ToString().Length > 200 ? 200 : ex.ToString().Length));
                result= "FALSE";
            }
            return result;
            
        }
        public string GetEleRemainMoneyBatch(string res)
        {
            string result = "";
            try
            {
                List<EleResModle> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EleResModle>>(res);
                if (list != null && list.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string dtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    int total = 0;
                    int count = 0;
                    foreach (EleResModle m in list)
                    {
                        total++;
                        sb.Append("update wy_ele_Balance set Ustatus='" + m.status + "',Umessage='" + m.err_msg?.ToString() + "',UpdateDate='" + dtNow + "' ");
                        if (m.data != null && m.data.Count > 0)
                        {
                            sb.Append(" ,EleBalance=" + m.data[0].value[0]);
                        }
                        sb.Append("  where opr_id='" + m.opr_id + "';");
                        if (total == 998)
                        {
                            db.UpdateEle(sb.ToString());
                            sb.Clear();
                            sb.Length = 0;
                            total = 0;
                            count++;
                        }
                    }
                    if (count == 0 || (count > 0 && total > 0 && total != 998))
                    {
                        db.UpdateEle(sb.ToString());
                    }
                    result = "SUCCESS";
                    db.AddLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "getEleRemainMoneyBatch()", "10", "chenggong");
                }
            }
            catch (Exception ex)
            {
                db.AddLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "getEleRemainMoneyBatch()", "10", ex.ToString().Substring(0, ex.ToString().Length > 200 ? 200 : ex.ToString().Length));
                result = "FALSE";
            }
            return result;

        }
        public string GetEleRechargeMoneyBatch(string res)
        {
            string result = "";
            try
            {
                List<EleResModle> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EleResModle>>(res);
                if (list != null && list.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string dtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    int total = 0;
                    int count = 0;
                    foreach (EleResModle m in list)
                    {
                        total++;
                        sb.Append("update wy_ele_recharge set Pstatus='" + m.status + "',Pmessage='" + m.err_msg?.ToString() + "',PUpdateDate='" + dtNow + "' ");
                        sb.Append("  where opr_id='" + m.opr_id + "';");
                        if (total == 998)
                        {
                            db.UpdateEle(sb.ToString());
                            sb.Clear();
                            sb.Length = 0;
                            total = 0;
                            count++;
                        }
                    }
                    if (count == 0 || (count > 0 && total > 0 && total != 998))
                    {
                        db.UpdateEle(sb.ToString());
                    }
                    result = "SUCCESS";
                    db.AddLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "getEleRemainMoneyBatch()", "10", "chenggong");
                }
            }
            catch (Exception ex)
            {
                db.AddLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "getEleRemainMoneyBatch()", "10", ex.ToString().Substring(0, ex.ToString().Length > 200 ? 200 : ex.ToString().Length));
                result = "FALSE";
            }
            return result;

        }
        /*
        [{
       "opr_id": "e3dfb115-2f07-46eb-a36f-0b6442bb1d1e",
       "resolve_time": "2020-03-27 17:14:02",
       "status": "SUCCESS",
       "data": [{
           "type": 3,
           "value": ["0.00", "0.00", "0.00", "0.00", "0.00"],
           "dsp": "总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh"
       }]
   }]
   */
    }
}
