using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UIDP.BIZModule.wy;

namespace WY.WebAPI.Controllers.wy
{
    [Route("EleManage")]
    public class EleManageController : WebApiBaseController
    {
        EleManageModule md = new EleManageModule();
        [HttpGet("GetData")]
        public IActionResult GetData(int page, int limit) => Ok(md.GetData( page, limit));
    }
}