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
    [Route("HouseInfo")]
    public class HouseInfoController : WebApiBaseController
    {
        HouseInfoModule HM = new HouseInfoModule();
        /// <summary>
        /// 上传平面图
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="noticeId"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        [HttpPost("uploadHouseImg")]
        public IActionResult uploadHouseImg([FromForm]IFormCollection formCollection)
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
                    string suffix = name.Substring(name.LastIndexOf("."), (name.Length - name.LastIndexOf("."))); //扩展名
                    //double filesize = Math.Round(Convert.ToDouble(file.Length / 1024.00 / 1024.00), 2);
                    string GUID = Guid.NewGuid().ToString();
                    string filepath = @"/WY_API/UploadFiles/HouseImg/" + GUID + suffix;
                    string filename = (Directory.GetCurrentDirectory() + filepath).Replace("\\","/");
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
                    r["message"] = 2000;
                    r["fileName"] = GUID+suffix;
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
        /// 获取房屋信息
        /// </summary>
        /// <param name="FWMC"></param>
        /// <param name="LSFGS"></param>
        /// <param name="FWSX"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("GetHouseInfo")]
        public IActionResult GetHouseInfo(string ORG_CODE,string FWBH,string FWMC, string LSFGS, string FWSX, int limit, int page,string baseURL) => Ok(HM.GetHouseInfo(ORG_CODE,FWBH, FWMC, LSFGS, FWSX, limit, page, baseURL));

        /// <summary>
        /// 新建房屋信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("CreateHouseInfo")]
        public IActionResult CreateHouseInfo([FromBody]JObject value) => Ok(HM.CreateHouseInfo(value.ToObject<Dictionary<string, object>>()));

        /// <summary>
        /// 修改房屋信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("UpdateHouseInfo")]
        public IActionResult UpdateHouseInfo([FromBody]JObject value) => Ok(HM.UpdateHouseInfo(value.ToObject<Dictionary<string, object>>()));

        /// <summary>
        /// 修改房屋信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("DeleteHouseInfo")]
        public IActionResult DeleteHouseInfo(string FWID) => Ok(HM.DeleteHouseInfo(FWID));

        /// <summary>
        /// 房屋信息导入
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("uploadHouseInfo")]
        public IActionResult uploadHouseInfo([FromForm] IFormCollection formCollection)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            var form = Request.Form;
            try
            {
                FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
                Dictionary<string, object> userinfo = new Dictionary<string, object>()
                {
                    {"userId",formCollection["userId"] },
                    {"ORG_CODE",formCollection["ORG_CODE"] }
                };
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
                    r["message"] = HM.UploadHouseInfo(filename, userinfo);
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
        [HttpGet("ExportHouseInfo")]
        public IActionResult ExportHouseInfo() => Ok(HM.ExportHouseInfo());

    }
}