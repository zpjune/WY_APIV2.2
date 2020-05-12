using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace UIDP.UTILITY
{
   public class TQApi
    {
        private string _authCode;
        private string _nonce;
        private string _url;
        private string _notyUrl;
        public TQApi(string authCode,string nonce,string url, string notyUrl) {
            _authCode = authCode;
            _nonce = nonce;
            _url = url;
            _notyUrl = notyUrl;
        }
        public string readElecMeter(List<Dictionary<string, Object>>list) {
            String request_content = JsonConvert.SerializeObject(list);
             return  testApiAsync(_url, request_content);
        }
        public  string testApiAsync(string url, string request_content)
        {
            //StackTrace st = new StackTrace(true);
            //Console.WriteLine(st.GetFrame(1).GetMethod().Name.ToString());
            string response = requestAsync(url, request_content);
            //printResponse(response);
           
            return response;
        }
        private static void printResponse(string response)
        {
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            string status = dict["status"];
            if (!"SUCCESS".Equals(status))
            {
                Console.WriteLine(dict["error_msg"]);
            }
            else
            {
                string response_content = dict["response_content"];

                try
                {
                    List<Dictionary<string, string>> items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response_content);
                    int i = 1;
                    Console.WriteLine("返回结果：");
                    foreach (Dictionary<string, string> item in items)
                    {
                        Console.WriteLine("[" + i++ + "]");
                        foreach (KeyValuePair<string, string> kvp in item)
                        {
                            Console.WriteLine(kvp.Key + ": " + kvp.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                }

            }
            Console.WriteLine();
        }
        public  string generateOperateId()
        {
            string id = System.Guid.NewGuid().ToString("N");
            return id;
        }
        private  string requestAsync(string url, string request_content)
        {
            // 时间戳
            string timestamp = getTimestamp();


            // 用于签名的内容
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("auth_code", _authCode);
            data.Add("timestamp", timestamp);
            data.Add("request_content", request_content);
            data.Add("notify_url", _notyUrl);

            // 获取签名
            string sign = getSign(data);

            data.Add("sign", sign);
            string postData = getPostData(data);

            // 请求接口，获取数据
            string ret = sendHttpRequest(url, postData);

            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);

            if (!checkSign(response))
            {
                //throw new Exception("签名验证失败");
               return("电接口签名验证失败");
            }

            return ret;
        }
        // 检验签名
        private  bool checkSign(Dictionary<string, string> response)
        {
            string sign = response["sign"];
            response.Remove("sign");
            return sign.Equals(getSign(response));
        }
        // 获得时间戳
        private  string getTimestamp()
        {
            long currentTicks = DateTime.Now.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long current = (currentTicks - dtFrom.Ticks) / 10000000;
            return current.ToString();
        }
        private  string getSign(Dictionary<string, string> data)
        {
            // 获取关键字列表
            List<string> keys = new List<string>();
            foreach (string key in data.Keys)
            {
                keys.Add(key);
            }
            // 关键字列表排序
            keys.Sort();
            StringBuilder sb = new StringBuilder();
            foreach (string key in keys)
            {
                // 取各个字段内容拼接字符串
                sb.Append(data[key]);
            }
            // 加上双方约定随机字符串
            string txt = sb.ToString() + _nonce;

            // 计算哈希值
            return getMD5(txt);
        }
        // md5加密
        private static string getMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.ASCII.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        private  string getPostData(Dictionary<string, string> data)
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in data.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, data[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, data[key]);
                }
                i++;
            }
            return buffer.ToString();
        }

        // 发送http请求
        private static string sendHttpRequest(string url, string postData)
        {
            //Console.WriteLine("请求地址：" + url);
            //Console.WriteLine("发送参数：" + postData);

            //定义request并设置request的路径
            WebRequest request = WebRequest.Create(url);

            //定义请求的方式
            request.Method = "POST";

            //设置参数的编码格式，解决中文乱码
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //设置request的MIME类型及内容长度
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            //打开request字符流
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //定义response为前面的request响应
            WebResponse response = request.GetResponse();

            //定义response字符流
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();//读取所有

            //Console.WriteLine("接口返回：" + responseFromServer);

            //关闭资源
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

    }
   
}
