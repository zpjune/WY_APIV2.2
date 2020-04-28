using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UIDP.BIZModule;
using UIDP.BIZModule.wy.Models;

namespace WY.WebAPI.Controllers.wy
{
   
    [Produces("application/json")]
    [Route("EleReceive")]
    public class EleReceiveController : WebApiBaseController
    {
        EleReceiveModule md = new EleReceiveModule();
        public static IConfiguration Configuration { get; set; }
        /// <summary>
        /// 批量接收余额
        /// </summary>
        /// <param name="response_content"></param>
        /// <param name="timestamp"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("getEleActiveEnergyBatch")]
        public IActionResult GetEleActiveEnergyBatch(string response_content, string timestamp, string sign) {
            if (!CheckSign(response_content, timestamp, sign))
            {
                return Ok("sign check failed");
            }
            //string res = "[{\"opr_id\": \"e3dfb115 - 2f07 - 46eb - a36f - 0b6442bb1d1e\",\"resolve_time\": \"2020-03-27 17:14:02\",\"status\": \"SUCCESS\",\"data\": [{\"type\": 3,\"value\": [\"0.00\", \"0.00\", \"0.00\", \"0.00\", \"0.00\"],\"dsp\": \"总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh\"}]}]";
            //List<EleResModle> list= Newtonsoft.Json.JsonConvert.DeserializeObject<List<EleResModle>>(res);
            //if (list!=null&&list.Count>0) {=
            //}
            return Ok(md.ReadActiveEnergyBatch(response_content));
        }
        /// <summary>
        /// 批量接收余额
        /// </summary>
        /// <param name="response_content"></param>
        /// <param name="timestamp"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("getEleRemainMoneyBatch")]
        public IActionResult GetEleRemainMoneyBatch(string response_content, string timestamp, string sign)
        {
            if (!CheckSign(response_content, timestamp, sign))
            {
                return Ok("sign check failed");
            }
            //string res = "[{\"opr_id\": \"e3dfb115 - 2f07 - 46eb - a36f - 0b6442bb1d1e\",\"resolve_time\": \"2020-03-27 17:14:02\",\"status\": \"SUCCESS\",\"data\": [{\"type\": 3,\"value\": [\"0.00\", \"0.00\", \"0.00\", \"0.00\", \"0.00\"],\"dsp\": \"总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh\"}]}]";
            //List<EleResModle> list= Newtonsoft.Json.JsonConvert.DeserializeObject<List<EleResModle>>(res);
            //if (list!=null&&list.Count>0) {=
            //}
            return Ok(md.GetEleRemainMoneyBatch(response_content));
        }
        /// <summary>
        /// 批量接收余额
        /// </summary>
        /// <param name="response_content"></param>
        /// <param name="timestamp"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        [HttpPost("getEleRechargeMoneyBatch")]
        public IActionResult GetEleRechargeMoneyBatch(string response_content, string timestamp, string sign)
        {
            if (!CheckSign(response_content, timestamp, sign))
            {
                return Ok("sign check failed");
            }
            //string res = "[{\"opr_id\": \"e3dfb115 - 2f07 - 46eb - a36f - 0b6442bb1d1e\",\"resolve_time\": \"2020-03-27 17:14:02\",\"status\": \"SUCCESS\",\"data\": [{\"type\": 3,\"value\": [\"0.00\", \"0.00\", \"0.00\", \"0.00\", \"0.00\"],\"dsp\": \"总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh\"}]}]";
            //List<EleResModle> list= Newtonsoft.Json.JsonConvert.DeserializeObject<List<EleResModle>>(res);
            //if (list!=null&&list.Count>0) {=
            //}

            return Ok(md.GetEleRechargeMoneyBatch(response_content));
        }
        #region 验证签名
        private bool CheckSign(string response_content, string timestamp, string sign)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string buf = response_content + timestamp + Configuration["nonce"];
            string encode = null;
            try
            {
                encode = CreateMD5Hash(buf);
            }
            catch (Exception e)
            {

                return false;
            }

            return encode.Equals(sign);
        }

        private string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            md5.Clear();

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        #endregion
        

        /*
        [{
       "opr_id": "e3dfb115-2f07-46eb-a36f-0b6442bb1d1e",
       "resolve_time": "2020-03-27 17:14:02",
       "status": "SUCCESS",
       "data": [{
           "type": 3,
           "value": ["0.00", "0.00", "0.00", "0.00", "0.00"],
           "dsp": "总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh"
       }]
   }]
   */

    }
}