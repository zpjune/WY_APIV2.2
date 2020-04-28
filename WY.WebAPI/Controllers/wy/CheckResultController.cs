﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UIDP.BIZModule.wy;

namespace WY.WebAPI.Controllers.wy
{
    [Produces("application/json")]
    [Route("CheckResult")]
    public class CheckResultController : WebApiBaseController
    {
        CheckResultModule CRM = new CheckResultModule();
        [HttpGet("GetCheckResult")]
        public IActionResult GetCheckResult(string year, string FWBH, string RWMC, int page, int limit) => Ok(CRM.GetCheckResult(year, FWBH, RWMC, page, limit));

        [HttpGet("GetTaskProcessInfo")]
        public IActionResult GetTaskProcessInfo(string year, string RWMC, string RWBH, int page, int limit) => Ok(CRM.GetTaskProcessInfo(year, RWMC, RWBH, page, limit));
    }
}