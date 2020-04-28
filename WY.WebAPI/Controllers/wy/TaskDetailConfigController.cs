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
    [Route("TaskDetailConfig")]
    public class TaskDetailConfigController : WebApiBaseController
    {
        TaskDetailConfigModule TDC = new TaskDetailConfigModule();

        [HttpGet("GetTaskDetailConfig")]
        public IActionResult GetTaskDetailConfig() => Ok(TDC.GetTaskDetailConfig());

        [HttpPost("CreateTaskDetailConfig")]
        public IActionResult CreateTaskDetailConfig([FromBody]JObject value) => Ok(TDC.CreateTaskDetailConfig(value.ToObject<Dictionary<string, object>>()));


        [HttpPost("UpdateTaskDetailConfig")]
        public IActionResult UpdateTaskDetailConfig([FromBody]JObject value) => Ok(TDC.UpdateTaskDetailConfig(value.ToObject<Dictionary<string, object>>()));

        [HttpGet("DeleteTaskDetailConfig")]
        public IActionResult DeleteTaskDetailConfig(string ID) => Ok(TDC.DeleteTaskDetailConfig(ID));

        [HttpGet("GetParentCodeConfig")]
        public IActionResult GetParentCodeConfig() => Ok(TDC.GetParentCodeConfig());
    }
}