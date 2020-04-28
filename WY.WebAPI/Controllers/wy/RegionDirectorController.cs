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
    [Route("RegionDirector")]
    public class RegionDirectorController : WebApiBaseController
    {
        RegionDirectorModule RD = new RegionDirectorModule();
        [HttpGet("GetRegionDirector")]
        public IActionResult GetRegionDirector(string SSQY, string FZR, int page, int limit) => Ok(RD.GetRegionDirector(SSQY, FZR, page, limit));

        [HttpPost("CreateRegionDirector")]
        public IActionResult CreateRegionDirector([FromBody]JObject value) => Ok(RD.CreateRegionDirector(value.ToObject<Dictionary<string, object>>()));


        [HttpPost("UpdateRegionDirector")]
        public IActionResult UpdateRegionDirector([FromBody]JObject value) => Ok(RD.UpdateRegionDirector(value.ToObject<Dictionary<string, object>>()));

        [HttpGet("DeleteRegionDirector")]
        public IActionResult DeleteRegionDirector(string RD_ID) => Ok(RD.DeleteRegionDirector(RD_ID));
    }
}