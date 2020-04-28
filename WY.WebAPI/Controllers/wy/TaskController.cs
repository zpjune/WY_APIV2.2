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
    [Route("Task")]
    public class TaskController : WebApiBaseController
    {
        TaskModule TM = new TaskModule();
        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="RWBH"></param>
        /// <param name="RWMC"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetTaskInfo")]
        public IActionResult GetTaskInfo(string RWBH, string RWMC, int page, int limit) => Ok(TM.GetTaskInfo(RWBH, RWMC, page, limit));
        /// <summary>
        /// 获取任务信息和明细信息
        /// </summary>
        /// <param name="TASK_ID"></param>
        /// <returns></returns>
        [HttpGet("GetPlanCheckAndDetail")]
        public IActionResult GetPlanCheckAndDetail(string TASK_ID) => Ok(TM.GetPlanCheckAndDetail(TASK_ID));
        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("CreateTask")]
        public IActionResult CreateTask([FromBody]JObject value) => Ok(TM.CreateTask(value.ToObject<Dictionary<string, object>>()));

        /// <summary>
        /// 修改一个任务
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("UpdateTask")]
        public IActionResult UpdateTask([FromBody]JObject value) => Ok(TM.UpdateTask(value.ToObject<Dictionary<string, object>>()));
        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="TASK_ID"></param>
        /// <returns></returns>
        [HttpGet("DeleteTask")]
        public IActionResult DeleteTask(string TASK_ID) => Ok(TM.DeleteTask(TASK_ID));


        /// <summary>
        /// 推送一个任务
        /// </summary>
        /// <param name="TASK_ID"></param>
        /// <returns></returns>
        [HttpGet("PushTask")]
        public IActionResult PushTask(string TASK_ID) => Ok(TM.PushTask(TASK_ID));
    }
}