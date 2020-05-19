using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using UIDP.BIZModule.wy;

namespace WY.WebAPI.Controllers.wy
{
    [Produces("application/json")]
    [Route("ShopInfo")]
    public class ShopInfoController : WebApiBaseController
    {
        ShopInfoModule SM = new ShopInfoModule();
        /// <summary>
        /// 查询列表中的商铺信息
        /// </summary>
        /// <param name="ZHXM"></param>
        /// <param name="FWSX"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("GetShopInfo")]
        public IActionResult GetShopInfo(string ORG_CODE,string ZHXM, string IS_PASS, string FWSX, string FWID, string FWBH, int page, int limit) => Ok(SM.GetShopInfo(ORG_CODE,ZHXM, IS_PASS,FWSX,FWID,  FWBH, page, limit));
        /// <summary>
        /// 获取商户详情 包括房屋信息，商户信息，租赁信息，物业信息等
        /// </summary>
        /// <param name="FWID"></param>
        /// <returns></returns>
        [HttpGet("GetShopInfoDetail")]
        public IActionResult GetShopInfoDetail(string CZ_SHID) => Ok(SM.GetShopInfoDetail(CZ_SHID));
        /// <summary>
        /// 删除列表中的商户信息
        /// </summary>
        /// <param name="CZ_SHID"></param>
        /// <param name="FWID"></param>
        /// <returns></returns>
        [HttpGet("DeleteShopInfo")]
        public IActionResult DeleteShopInfo(string CZ_SHID, string FWID) => Ok(SM.DeleteShopInfo(CZ_SHID, FWID));
        /// <summary>
        /// 创建商户信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("CreateShopInfo")]
        public IActionResult CreateShopInfo([FromBody]JObject value) => Ok(SM.CreateShopInfo(value.ToObject<Dictionary<string,object>>()));

        /// <summary>
        /// 修改商户信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("UpdateShopInfo")]
        public IActionResult UpdateShopInfo([FromBody]JObject value) => Ok(SM.UpdateShopInfo(value.ToObject<Dictionary<string, object>>()));
        /// <summary>
        /// 通过审核
        /// </summary>
        /// <param name="CZ_SHID">商户ID</param>
        /// <returns></returns>
        [HttpGet("PassInfo")]
        public IActionResult PassInfo(string CZ_SHID) => Ok(SM.PassInfo(CZ_SHID));
        /// <summary>
        /// 不通过审核
        /// </summary>
        /// <param name="CZ_SHID"></param>
        /// <returns></returns>
        [HttpGet("UnpassInfo")]
        public IActionResult UnpassInfo(string CZ_SHID) => Ok(SM.UnpassInfo(CZ_SHID));

        /// <summary>
        /// 终止租赁
        /// </summary>
        /// <param name="FWID"></param>
        /// <param name="CZ_SHID"></param>
        /// <returns></returns>
        [HttpGet("EndLease")]
        public IActionResult EndLease(string FWID,string CZ_SHID) => Ok(SM.EndLease(FWID,CZ_SHID));

        /// <summary>
        /// 转售房屋
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("SecondHand")]
        public IActionResult SecondHand([FromBody]JObject value) => Ok(SM.SecondHand(value.ToObject<Dictionary<string, object>>()));

        /// <summary>
        /// 获取商户使用者详情
        /// </summary>
        /// <param name="CZ_SHID"></param>
        /// <returns></returns>
        [HttpGet("GetShopDetailUserInfo")]
        public IActionResult GetShopDetailUserInfo(string CZ_SHID) => Ok(SM.GetShopDetailUserInfo(CZ_SHID));

        /// <summary>
        /// 获取商户查询列表
        /// </summary>
        /// <param name="FWBH"></param>
        /// <param name="ZHXM"></param>
        /// <param name="SFZH"></param>
        /// <param name="SHOPBH"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("GetShopUserInfo")]
        public IActionResult GetShopUserInfo(string FWBH, string ZHXM, string SFZH, string SHOPBH,int SHOP_STATUS,int limit, int page) 
            => Ok(SM.GetShopUserInfo(FWBH, ZHXM, SFZH, SHOPBH, SHOP_STATUS,limit, page));


        [HttpGet("ExportShopInfo")]
        public IActionResult ExportShopInfo(string FWSX) => Ok(SM.ExportShopInfo(FWSX));
        /// <summary>
        /// 出租商户导入
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("uploadCZSHOPInfo")]
        public IActionResult uploadCZSHOPInfo([FromForm] IFormCollection formCollection)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
                foreach (IFormFile file in fileCollection)
                {
                    StreamReader reader = new StreamReader(file.OpenReadStream());
                    String content = reader.ReadToEnd();
                    String name = file.FileName;
                    Random ran = new Random();
                    String filename = System.IO.Directory.GetCurrentDirectory() + "/WY_API/Files/" + DateTime.Now.ToString("yyyyMMddhhmmss") + ran.Next(100, 999).ToString() + name;
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        // 复制文件
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                    }
                    r["message"] = SM.uploadCZSHOPInfo(filename);
                    if (r["message"].ToString() == "")
                    {
                        r["code"] = 2000;
                    }
                    else
                    {
                        r["code"] = -1;
                    }
                    Json(r);
                }
            }
            catch (Exception ex)
            {
                r["code"] = -1;
                r["message"] = ex.Message;
            }

            return Json(r);
        }

        /// <summary>
        /// 出售商户导入
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("uploadCSSHOPInfo")]
        public IActionResult uploadCSSHOPInfo([FromForm] IFormCollection formCollection)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
                foreach (IFormFile file in fileCollection)
                {
                    StreamReader reader = new StreamReader(file.OpenReadStream());
                    String content = reader.ReadToEnd();
                    String name = file.FileName;
                    Random ran = new Random();
                    String filename = System.IO.Directory.GetCurrentDirectory() + "/WY_API/Files/" + DateTime.Now.ToString("yyyyMMddhhmmss") + ran.Next(100, 999).ToString() + name;
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        // 复制文件
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                    }
                    r["message"] = SM.uploadCSSHOPInfo(filename);
                    if (r["message"].ToString() == "")
                    {
                        r["code"] = 2000;
                    }
                    else
                    {
                        r["code"] = -1;
                    }
                    Json(r);
                }
            }
            catch (Exception ex)
            {
                r["code"] = -1;
                r["message"] = ex.Message;
            }

            return Json(r);
        }
        /// <summary>
        /// 续租功能
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Renewal")]
        public IActionResult Renewal([FromBody]JObject value) => Ok(SM.Renewal(value.ToObject<Dictionary<string,object>>()));

        #region 根据需求 下面两个接口暂未启用 更换另一种方案
        /// <summary>
        /// 获取原租赁信息的租赁日期
        /// </summary>
        /// <param name="CZ_SHID"></param>
        /// <returns></returns>
        [HttpGet("GetLeaseTime")]
        public IActionResult GetLeaseTime(string CZ_SHID) => Ok(SM.GetLeaseTime(CZ_SHID));
        /// <summary>
        /// 修改租赁信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("UpdateLeaseTime")]
        public IActionResult UpdateLeaseTime([FromBody]JObject value) => Ok(SM.UpdateLeaseTime(value.ToObject<Dictionary<string,object>>()));
        #endregion

        [HttpGet("GetFeeResult")]
        public IActionResult GetFeeResult(string CZ_SHID,int page,int limit) => Ok(SM.GetFeeResult(CZ_SHID,page,limit));
    }
}