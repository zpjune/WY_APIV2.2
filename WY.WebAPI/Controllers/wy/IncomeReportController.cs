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
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class IncomeReportController : WebApiBaseController
    {
        IncomeReportModule IRM = new IncomeReportModule();

        /// <summary>
        /// 获取物业日报表
        /// </summary>
        /// <param name="date">选择的日期</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("GetWYIncomeReport")]
        public IActionResult GetWYIncomeReport(string date, int page, int limit) => Ok(IRM.GetWYIncomeReport(date, page, limit));

        /// <summary>
        /// 获取普丰日报表
        /// </summary>
        /// <param name="date">选择的日期</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        [HttpGet("GetPFIncomeReport")]
        public IActionResult GetPFIncomeReport(string date, int page, int limit) => Ok(IRM.GetPFIncomeReport(date, page, limit));

        [HttpPost("ExportRecipet")]
        public IActionResult ExportRecipet([FromBody]JObject value) => Ok(IRM.ExportRecipet(value.ToObject<Dictionary<string, object>>()));
    }
}