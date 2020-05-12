using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UIDP.BIZModule.wy;

namespace WY.WebAPI.Controllers.wy
{
    [Route("EleManage")]
    public class EleManageController : WebApiBaseController
    {
        EleManageModule md = new EleManageModule();
        [HttpGet("GetData")]
        public IActionResult GetData(int page, int limit) => Ok(md.GetData( page, limit));
        [HttpPost("QingLing")]
        public IActionResult QingLing([FromBody]JObject value) => Ok(md.QingLing(value.ToObject<Dictionary<string, object>>()));
        [HttpPost("KaiHu")]
        public IActionResult KaiHu([FromBody]JObject value) => Ok(md.KaiHu(value.ToObject<Dictionary<string, object>>()));
    }
}