using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UIDP.BIZModule.wy;

namespace WY.WebAPI.Controllers.wy
{
    [Produces("application/json")]
    [Route("CheckReport")]
    public class CheckReportController : WebApiBaseController
    {
        CheckReportModule CRM = new CheckReportModule();
        /// <summary>
        /// 检查结果查询
        /// </summary>
        /// <param name="year">当前年</param>
        /// <param name="FWBH">任务编号</param>
        /// <param name="RWMC">任务名称</param>
        /// <param name="JCJG">检查结果</param>
        /// <param name="page">当前页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("GetCheckResult")]
        public IActionResult GetCheckResult(string year, string FWBH, string RWMC, string JCJG,string DLID,int page, int limit)
            => Ok(CRM.GetCheckResult(year, FWBH, RWMC, JCJG,DLID,page, limit));
        /// <summary>
        /// 检查结果明细查询（弹窗）
        /// </summary>
        /// <param name="RESULT_ID">检查结果ID</param>
        /// <returns></returns>
        [HttpGet("GetCheckResultDetail")]
        public IActionResult GetCheckResultDetail(string RESULT_ID) => Ok(CRM.GetCheckResultDetail(RESULT_ID));
        /// <summary>
        /// 获取检查大类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetParentCheckCodeOptions")]
        public IActionResult GetParentCheckCodeOptions() => Ok(CRM.GetParentCheckCodeOptions());

        /// <summary>
        /// 检查结果导出
        /// </summary>
        /// <param name="year">当前年</param>
        /// <param name="FWBH">任务编号</param>
        /// <param name="RWMC">任务名称</param>
        /// <param name="JCJG">检查结果</param>
        /// <param name="page">当前页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("ExportTotalCheckReport")]
        public IActionResult ExportTotalCheckReport(string year, string FWBH, string RWMC, string JCJG, string DLID, int page, int limit)
            => Ok(CRM.ExportTotalCheckReport(year, FWBH, RWMC, JCJG, DLID, page, limit));

        /// <summary>
        /// 月度报表查询
        /// </summary>
        /// <param name="date">报表日期</param>
        /// <param name="FZR">负责人</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页条数</param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("WorkloadStatistics")]
        public IActionResult WorkloadStatistics(string date, string FZR, int page, int limit, int type = 0) =>
            Ok(CRM.WorkloadStatistics(date, FZR, page, limit, type));
        /// <summary>
        /// 检查人员工作量详情
        /// </summary>
        /// <param name="RD_ID">检查人ID</param>
        /// <param name="yyyy">年</param>
        /// <param name="mon">月</param>
        /// <returns></returns>
        [HttpGet("WorkloadStatisticsDetail")]
        public IActionResult WorkloadStatisticsDetail(string RD_ID, string yyyy, string mon) =>
            Ok(CRM.WorkloadStatisticsDetail(RD_ID, yyyy, mon));

        /// <summary>
        /// 商铺统计报表查询
        /// </summary>
        /// <param name="date">年度</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("ShopCheckSummary")]
        public IActionResult ShopCheckSummary(string SSQY,string FWBH,string FWMC,string date, int page, int limit) => Ok(CRM.ShopCheckSummary(SSQY,FWBH,FWMC,date, page, limit));
    }
}