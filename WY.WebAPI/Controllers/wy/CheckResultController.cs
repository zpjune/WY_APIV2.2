using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UIDP.BIZModule.wy;

namespace WY.WebAPI.Controllers.wy
{
    [Produces("application/json")]
    [Route("CheckResult")]
    public class CheckResultController : WebApiBaseController
    {
        CheckResultModule CRM = new CheckResultModule();
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
        public IActionResult GetCheckResult(string year, string FWBH, string RWMC,string JCJG,int page, int limit) => Ok(CRM.GetCheckResult(year, FWBH, RWMC,JCJG,page, limit));
        /// <summary>
        /// 检查结果明细查询（弹窗）
        /// </summary>
        /// <param name="RESULT_ID">检查结果ID</param>
        /// <returns></returns>
        [HttpGet("GetCheckResultDetail")]
        public IActionResult GetCheckResultDetail(string RESULT_ID) => Ok(CRM.GetCheckResultDetail(RESULT_ID));


        /// <summary>
        /// 发送整改通知
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("Rectification")]
        public IActionResult Rectification([FromBody]JArray values) => Ok(CRM.Rectification(values.ToObject<string[]>()));

        /// <summary>
        /// 检查进度查询
        /// </summary>
        /// <param name="year">当前年</param>
        /// <param name="RWMC">任务名称</param>
        /// <param name="RWBH">任务编号</param>
        /// <param name="page">当前页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("GetTaskProcessInfo")]
        public IActionResult GetTaskProcessInfo(string year, string RWMC, string RWBH, int page, int limit) => Ok(CRM.GetTaskProcessInfo(year, RWMC, RWBH, page, limit));
    }
}