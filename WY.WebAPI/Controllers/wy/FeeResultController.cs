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
    [Route("FeeResult")]
    public class FeeResultController : WebApiBaseController
    {
        FeeResultModule FR = new FeeResultModule();

        /// <summary>
        /// 获取缴费信息
        /// </summary>
        /// <param name="JFLX"></param>
        /// <param name="FWMC"></param>
        /// <param name="FWBH"></param>
        /// <param name="JFSTATUS"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetFeeResult")]
        public IActionResult GetFeeResult(string JFLX, string FWMC, string FWBH, string JFSTATUS, int page, int limit) => Ok(FR.GetFeeResult(JFLX, FWMC, FWBH, JFSTATUS, page, limit));

        [HttpGet("PaidFeeResult")]
        public IActionResult PaidFeeResult(string date, int page, int limit) => Ok(FR.PaidFeeResult(date, page, limit));
        /// <summary>
        /// 获取商铺信息
        /// </summary>
        /// <param name="FWID"></param>
        /// <returns></returns>
        [HttpGet("GetShopInfo")]
        public IActionResult GetShopInfo(string FWID) => Ok(FR.GetShopInfo(FWID));

        /// <summary>
        /// 创建通知单
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("CreateNotification")]
        public IActionResult CreateNotification(string type) => Ok(FR.CreateNotification(type));
        /// <summary>
        /// 删除通知单功能 此功能是为了解除因特殊情况提前生成了部分商户的通知单信息，但是无法解除物业关系的情况
        /// </summary>
        /// <param name="RECORD_ID">通知单ID</param>
        /// <returns></returns>
        [HttpGet("DeleteRecord")]
        public IActionResult DeleteRecord(string RECORD_ID) => Ok(FR.DeleteRecord(RECORD_ID));
        /// <summary>
        /// 批量确认通知单
        /// </summary>
        /// <param name="arrList"></param>
        /// <returns></returns>
        [HttpPost("ConfirmNotificationList")]
        public IActionResult ConfirmNotificationList([FromBody]JArray value ) => Ok(FR.ConfirmNotificationList(value.ToObject<List<Dictionary<string,object>>>()));
        /// <summary>
        /// 推送催缴消息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("PushNotification")]
        public IActionResult PushNotification([FromBody]JArray value) => Ok(FR.PushNotification(value.ToObject<List<Dictionary<string, object>>>()));


        /// <summary>
        /// 获取缴费历史记录
        /// </summary>
        /// <param name="JFLX">缴费类型</param>
        /// <param name="FWMC">房屋名称</param>
        /// <param name="FWBH">房屋编号</param>
        /// <param name="JFSTATUS">缴费状态</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetHistoryFeeResult")]
        public IActionResult GetHistoryFeeResult(string JFLX, string FWMC, string FWBH, string JFSTATUS, int page, int limit) => Ok(FR.GetHistoryFeeResult(JFLX, FWMC, FWBH, JFSTATUS, page, limit));

        /// <summary>
        /// 获取欠费信息
        /// </summary>
        /// <param name="JFLX">缴费类型</param>
        /// <param name="FWMC">房屋名称</param>
        /// <param name="FWBH">房屋编号</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetBadFeeResult")]
        public IActionResult GetBadFeeResult(string JFLX, string FWMC, string FWBH, int page, int limit) => Ok(FR.GetBadFeeResult(JFLX, FWMC, FWBH, page, limit));

        /// <summary>
        /// 手动确认缴费
        /// </summary>
        /// <param name="RECORD_ID"></param>
        /// <returns></returns>
        [HttpGet("ConfirmFee")]
        public IActionResult ConfirmFee(string RECORD_ID,int JFLX,string JFJE,string GMSL) => Ok(FR.ConfirmFee(RECORD_ID,JFLX,JFJE,GMSL));

        /// <summary>
        /// 确认收据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("ConfirmReciveMoney")]
        public IActionResult ConfirmReciveMoney([FromBody]JArray value) => Ok(FR.ConfirmReciveMoney(value.ToObject<List<Dictionary<string, object>>>()));
        /// <summary>
        /// 清缴
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("PayOff")]
        public IActionResult PayOff([FromBody]JArray value) => Ok(FR.PayOff(value.ToObject<List<Dictionary<string, object>>>()));
        /// <summary>
        /// 获取水单价
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPER_WATER_PRICE")]
        public IActionResult GetPER_WATER_PRICE() => Ok(FR.GetPER_WATER_PRICE());
        /// <summary>
        /// 导出缴费通知单功能
        /// </summary>
        /// <param name="JFSTATUS"></param>
        /// <returns></returns>
        [HttpGet("ExportFeeResult")]
        public IActionResult ExportFeeResult(string JFSTATUS) => Ok(FR.ExportFeeResult(JFSTATUS));
    }
}