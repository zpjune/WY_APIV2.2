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
    [Route("CheckPlan")]
    public class CheckPlanController : WebApiBaseController
    {
        CheckPlanModule CPM = new CheckPlanModule();
        /// <summary>
        /// 查询年度计划
        /// </summary>
        /// <param name="JHMC">计划名称</param>
        /// <param name="JHSJ">计划时间</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetCheckPlan")]
        public IActionResult GetCheckPlan(string JHMC, string JHND, int page, int limit) => Ok(CPM.GetCheckPlan(JHMC, JHND, page, limit));
        /// <summary>
        /// 获取年度检查计划明细
        /// </summary>
        /// <param name="PLAN_ID"></param>
        /// <returns></returns>
        [HttpGet("GetCheckPlanDetail")]
        public IActionResult GetCheckPlanDetail(string PLAN_ID) => Ok(CPM.GetCheckPlanDetail(PLAN_ID));
        /// <summary>
        /// 创建年度检查计划
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("CreateCheckPlan")]
        public IActionResult CreateCheckPlan([FromBody]JObject value) => Ok(CPM.CreateCheckPlan(value.ToObject<Dictionary<string, object>>()));


        /// <summary>
        /// 修改年度检查计划
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("UpdateCheckPlan")]
        public IActionResult UpdateCheckPlan([FromBody]JObject value) => Ok(CPM.UpdateCheckPlan(value.ToObject<Dictionary<string, object>>()));
        
        /// <summary>
        /// 删除检查计划明细
        /// </summary>
        /// <param name="PLAN_DETAIL_ID"></param>
        /// <returns></returns>
        [HttpGet("DeleteCheckPlanDetail")]
        public IActionResult DeleteCheckPlanDetail(string PLAN_DETAIL_ID) => Ok(CPM.DeleteCheckPlanDetail(PLAN_DETAIL_ID));

        /// <summary>
        /// 删除检查计划
        /// </summary>
        /// <param name="PLAN_DETAIL_ID"></param>
        /// <returns></returns>
        [HttpGet("DeleteCheckPlan")]
        public IActionResult DeleteCheckPlan(string PLAN_ID) => Ok(CPM.DeleteCheckPlan(PLAN_ID));
    }
}