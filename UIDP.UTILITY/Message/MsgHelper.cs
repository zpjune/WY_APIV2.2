
using House.IService.Common.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIDP.UTILITY.JWTHelper;

namespace House.IService.Common.Message
{
    public class MsgHelper
    {
        private string url;
        private string token;
        private JWTHelper jwt => new JWTHelper();
        private static MsgHelper msg;

        public static MsgHelper Msg
        {
            get
            {
                if (msg == null)
                {
                    msg = new MsgHelper();
                }
                return msg;
            }
        }

        /// <summary>
        /// 消息推送接口
        /// </summary>
        /// <param name="url">推送接口的url</param>
        /// <param name="openId">接收者openid</param>
        /// <param name="data">模板数据</param>
        /// <param name="templaterId">模板ID</param>
        /// <param name="color">模板内容字体颜色，不填默认为黑色，只是针对赋值更改颜色</param>
        /// <param name="appurl">模板跳转链接（海外帐号没有跳转能力）</param>
        /// <param name="appId">跳小程序所需数据，不需跳小程序可不用传该数据</param>
        /// <param name="pagepath">所需跳转到小程序的具体页面路径，支持带参数,（示例index?foo=bar），要求该小程序已发布，暂不支持小游戏</param>
        /// <returns></returns>
        public async Task<string> SendMsg(string url, string openId, Dictionary<string, object> data
            , string templateId = null, string color = "#173177", string appurl = null,
            string appId = null, string pagepath = null)
        {
            string jsonData = JsonConvert.SerializeObject(new
            { openId, data, templateId, color, appurl, appId, pagepath });
            return await this.SendMsg(url, jsonData);
        }

        public async Task<string> SendMsg(string url, string jsonData)
        {
            return await this.In(url)
                .SendMsg(jsonData);
        }

        /// <summary>
        /// reload 为重试推送次数，因为token在请求的一秒钟可能会出现同步的失效问题，
        /// </summary>
        public async Task<string> SendMsg(string jsonData, int reload = 2)
        {
            string content = await HttpHelper.PostJsonSync(url, jsonData, this.Token());
            if (!string.IsNullOrEmpty(content))
            {
                dynamic Content = JsonConvert.DeserializeObject(content);
                if (Content.code != 1000 && reload != 0)
                {
                    reload--;
                    return await SendMsg(jsonData, reload);
                }
            }
            return content;
        }

        public async Task<string> SendSMS(string to,string[] datas, string url,
            string templateId = null, string appId = null)
        {
            string jsonData = JsonConvert.SerializeObject(new
            {
                to,
                datas,
                templateId,
                appId
            });
            return await SendMsg(url, jsonData);
        }

        public MsgHelper In(string u)
        {
            url = u;
            this.Token();
            return this;
        }

        public string Token()
        {
            if (string.IsNullOrEmpty(token) || !jwt.Validate(token))
            {
                token = jwt.CreateToken();
            }
            return this.token;
        }

    }
}
