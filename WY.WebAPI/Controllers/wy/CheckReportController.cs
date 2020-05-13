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
    }
}