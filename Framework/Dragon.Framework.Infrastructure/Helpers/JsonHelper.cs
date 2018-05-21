using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 序列化对象为规范的Json格式
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="withType">是否包含类型</param>
        /// <returns></returns>
        public static string SerializeObject(object data, bool withType = false)
        {
            return JsonConvert.SerializeObject(data,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    TypeNameHandling = withType ? TypeNameHandling.All : TypeNameHandling.None
                });
        }

        /// <summary>
        /// 反序列化Json为对象
        /// </summary>
        /// <param name="json">Json</param>
        /// <param name="withType">是否包含类型</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json, bool withType = false)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        DateFormatString = "yyyy-MM-dd HH:mm:ss",
                        TypeNameHandling = withType ? TypeNameHandling.All : TypeNameHandling.None
                    });
            }
            catch (JsonSerializationException jsEx)
            {
                throw new JsonSerializationException(jsEx.Message, jsEx);
            }
            catch (JsonReaderException jrEx)
            {
                throw new JsonReaderException(jrEx.Message, jrEx);
            }
        }

        /// <summary>
        /// 扩展方法，将对象转换为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return SerializeObject(obj);
        }
    }
}
