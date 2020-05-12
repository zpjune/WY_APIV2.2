using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Newtonsoft.Json;

namespace UIDP.UTILITY
{
    public static class Tools
    {

        public static string Serilize2Json(this object obj) {

            JsonSerializer serializer = new JsonSerializer();
            if(obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return "";
            }
        }

        public static object JsonDeSerilize(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            JsonTextReader jtr = new JsonTextReader(new StringReader(str));
            object obj = serializer.Deserialize(jtr);
            if(obj != null)
            {
                return obj;
            }
            else
            {
                return str;
            }
        }


        public static bool AsBool(this object obj)
        {
            if(obj == null) return false;
            if(obj is bool) return (bool)obj;
            if(obj is string)
            {
                string str = ((string)(obj)).ToLower();
                if(str == "true") return true;
                if(str == "false") return false;
            }
            return obj.AsInt() > 0;
        }

        public static int AsInt(this object obj)
        {
            if(obj == null) return 0;
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static decimal AsDecimal(this object obj)
        {
            if(obj == null) return 0;
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static float AsFloat(this object obj)
        {
            if(obj == null) return 0f;
            try
            {
                return Convert.ToSingle(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static string AsString(this object obj)
        {
            if(obj == null) return "";
            return obj.ToString();
        }


    }
}
