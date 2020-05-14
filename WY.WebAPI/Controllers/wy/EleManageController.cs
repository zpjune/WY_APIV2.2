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
        [HttpGet("GetWaterData")]
        public IActionResult GetWaterData(string yjstate, string month,string HouseName, string YeZhuName,string ZhuanZuName,int page, int limit) => Ok(md.GetWaterData( yjstate, month,  HouseName,  YeZhuName,  ZhuanZuName, page, limit));
        [HttpGet("ExportWaterData")]
        public IActionResult ExportWaterData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit) => Ok(md.ExportWaterData(yjstate, month, HouseName, YeZhuName, ZhuanZuName, page, limit));
        [HttpGet("GetEleData")]
        public IActionResult GetEleData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit) => Ok(md.GetEleData(yjstate, month, HouseName, YeZhuName, ZhuanZuName, page, limit));
        [HttpGet("ExportEleData")]
        public IActionResult ExportEleData(string yjstate, string month, string HouseName, string YeZhuName, string ZhuanZuName, int page, int limit) => Ok(md.ExportEleData(yjstate, month, HouseName, YeZhuName, ZhuanZuName, page, limit));
        [HttpGet("getEleWaterWarningMsg")]
        public IActionResult getEleWaterWarningMsg() => Ok(md.getEleWaterWarningMsg());

    }
}