using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UIDP.ODS.wy;
using UIDP.UTILITY;

namespace UIDP.BIZModule.wy
{
    public class IncomeReportModule
    {
        IncomeReportDB db = new IncomeReportDB();
        public Dictionary<string,object> GetWYIncomeReport(string date,int page,int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetWYIncomeReport(date);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                }
                else
                {
                    r["code"] = 2000;
                    r["messsage"] = "成功,但是没有数据";
                    r["total"] = 0;
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string, object> GetPFIncomeReport(string date, int page, int limit)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt = db.GetPFIncomeReport(date);
                if (dt.Rows.Count > 0)
                {
                    r["code"] = 2000;
                    r["message"] = "成功";
                    r["total"] = dt.Rows.Count;
                    r["items"] = KVTool.GetPagedTable(dt, page, limit);
                }
                else
                {
                    r["code"] = 2000;
                    r["messsage"] = "成功,但是没有数据";
                    r["total"] = 0;
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        public Dictionary<string,object> ExportRecipet(Dictionary<string,object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            int ExportType = Convert.ToInt32(d["ExportType"]);//导出类型，0为物业类型，1为普丰类型
            double Total = 0;//金额合计 默认0
            Dictionary<int, string> FileHeaderName = new Dictionary<int, string>()
            {
                {0,"物业收据" },
                {1,"普丰收据" }
            };
            string BasePath =Directory.GetCurrentDirectory()+"/";
            //FileInfo Template = new FileInfo(BasePath+ "/WY_API/ExcelModel/收据模板.xls");
            string fileName = FileHeaderName[ExportType] + "-" + d["USER_NAME"] + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            string filePath = @"WY_API/DownLoadFiles/";
            try
            {
                //商户基本信息查询
                DataSet ds = db.GetShopInfo(d["HOUSE_ID"].ToString());

                //excel插入处理
                IWorkbook workbook = null;
                FileStream fs = null;
                IRow row = null;
                ISheet sheet = null;
                //读取复制的excel模板信息
                using(fs =new FileStream(BasePath + "/WY_API/ExcelModel/收据模板.xls", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    workbook = new HSSFWorkbook(fs);//初始化
                    sheet = workbook.GetSheetAt(0);//获取第一个sheet页
                    string JZMJ = ds.Tables[0].Rows[0].IsNull("JZMJ") ? "0" : Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[0]["JZMJ"])*100)/100+"平方米";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        row = sheet.GetRow(1);
                        //商户姓名
                        row.GetCell(1).SetCellValue(d["USER_NAME"].ToString());
                        //坐落位置
                        row.GetCell(4).SetCellValue(ds.Tables[0].Rows[0].IsNull("ZLWZ") ? "" : ds.Tables[0].Rows[0]["ZLWZ"].ToString());
                        //建筑面积
                        row.GetCell(7).SetCellValue(JZMJ);
                        
                        row = sheet.GetRow(8);
                        //收款人
                        row.GetCell(1).SetCellValue("普丰物业");
                        //交款人
                        row.GetCell(3).SetCellValue(d["USER_NAME"].ToString());
                        //打印日期
                        row.GetCell(7).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));

                    }
                    int i = 3;//第四行开始是费用明细项                 
                    if (d.ContainsKey("WYF")&&d["WYF"].ToString() != "0")
                    {
                        row = sheet.GetRow(i);
                        //费用名称
                        row.GetCell(0).SetCellValue(d["REMARK"] == null || d["REMARK"].ToString() == "" ? "物业费":d["REMARK"].ToString());
                        //建筑面积
                        row.GetCell(2).SetCellValue(JZMJ);
                        //物业单价
                        row.GetCell(5).SetCellValue(d["HOUSE_SERVICE_UNITPRICE"]+"元/月/平");
                        //物业费金额
                        row.GetCell(6).SetCellValue(d["WYF"]+"元");
                        //物业费备注（起止日期）
                        row.GetCell(7).SetCellValue(d["YXQ"].ToString());
                        Total = Convert.ToDouble(d["WYF"]);
                        i++;
                    }
                    if (d.ContainsKey("DF")&&d["DF"].ToString() != "0")
                    {
                        row = sheet.GetRow(i);
                        //费用名称
                        row.GetCell(0).SetCellValue("电费");
                        //建筑面积
                        row.GetCell(2).SetCellValue(JZMJ);
                        //电费单价
                        row.GetCell(5).SetCellValue(ds.Tables[1].Select("CONF_CODE='PER_ELECTRIC_SET_PRICE'").Length > 0 ? ds.Tables[1].Select("CONF_CODE='PER_ELECTRIC_SET_PRICE'")[0]["CONF_VALUE"]+"元/度" : 0+"元/度");
                        //电费金额
                        row.GetCell(6).SetCellValue(d["DF"] + "元");
                        Total += Convert.ToDouble(d["DF"]);
                        i++;
                    }
                    if (d.ContainsKey("SF")&&d["SF"].ToString() != "0")
                    {
                        row = sheet.GetRow(i);
                        //费用名称
                        row.GetCell(0).SetCellValue("水费");
                        //建筑面积
                        row.GetCell(2).SetCellValue(JZMJ);
                        //水费单价
                        row.GetCell(5).SetCellValue(ds.Tables[1].Select("CONF_CODE='PER_WATER_PRICE'").Length > 0 ? ds.Tables[1].Select("CONF_CODE='PER_WATER_PRICE'")[0]["CONF_VALUE"]+"元/吨": 0+ "元/吨");
                        //水费金额
                        row.GetCell(6).SetCellValue(d["SF"]+"元");
                        Total += Convert.ToDouble(d["SF"]);
                        i++;
                    }
                    if (d.ContainsKey("WYBZJ")&&d["WYBZJ"].ToString() != "0")
                    {
                        row = sheet.GetRow(i);
                        //费用名称
                        row.GetCell(0).SetCellValue("违约保证金");
                        //建筑面积
                        row.GetCell(2).SetCellValue(JZMJ);
                        //row.GetCell(6).SetCellValue(ds.Tables[1].Rows[0].IsNull("PER_WATER_PRICE") ? "" : ds.Tables[1].Rows[0].IsNull("PER_WATER_PRICE").ToString());
                        //违约保证金
                        row.GetCell(6).SetCellValue(d["WYBZJ"]+"元");
                        //物业费备注（起止日期）
                        Total += Convert.ToDouble(d["WYBZJ"]);
                        i++;
                    }
                    if (d.ContainsKey("ZXYJ")&&d["ZXYJ"].ToString() != "0")
                    {
                        row = sheet.GetRow(i);
                        //费用名称
                        row.GetCell(0).SetCellValue("装修押金");
                        //建筑面积
                        row.GetCell(2).SetCellValue(JZMJ);
                        //row.GetCell(6).SetCellValue(ds.Tables[1].Rows[0].IsNull("PER_WATER_PRICE") ? "" : ds.Tables[1].Rows[0].IsNull("PER_WATER_PRICE").ToString());
                        //装修押金
                        row.GetCell(6).SetCellValue(d["ZXYJ"]+"元");
                        Total += Convert.ToDouble(d["ZXYJ"]);
                        i++;
                    }
                    if (d.ContainsKey("XFBZJ")&&d["XFBZJ"].ToString() != "0")
                    {
                        row = sheet.GetRow(i);
                        //费用名称
                        row.GetCell(0).SetCellValue("消防保证金");
                        //建筑面积
                        row.GetCell(2).SetCellValue(JZMJ);
                        //row.GetCell(6).SetCellValue(ds.Tables[1].Rows[0].IsNull("PER_WATER_PRICE") ? "" : ds.Tables[1].Rows[0].IsNull("PER_WATER_PRICE").ToString());
                        //消防保证金
                        row.GetCell(6).SetCellValue(d["XFBZJ"]+"元");
                        Total += Convert.ToDouble(d["XFBZJ"]);
                        i++;
                    }
                    //总计金额
                    string ChineseTotal = MoneyHelper.GetNumberCapitalized(Total);
                    
                    row = sheet.GetRow(7);
                    row.GetCell(2).SetCellValue(ChineseTotal);
                    row.GetCell(6).SetCellValue(Total+"元");
                    //准备生成文件
                    while (File.Exists(BasePath + filePath + fileName))
                    {
                        fileName = FileHeaderName[ExportType] +"-"+d["USER_NAME"] +"-"+DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    }
                    //将excel写入流
                    using (FileStream Reportfile = new FileStream(BasePath + filePath + fileName, FileMode.Create))
                    {
                        workbook.Write(Reportfile);
                    }
                    r["code"] = 2000;
                    r["path"] = filePath + fileName;
                }
                
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
            

        }

    }
}
