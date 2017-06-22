using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp;

namespace Poloniex.LIB.Serialize
{
    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class RestSharpJsonNetDeserializer : IDeserializer
    {
        /// <summary>
        /// Default deserializer
        /// </summary>
        public RestSharpJsonNetDeserializer()
        {
            DateFormat = "yyyy-MM-dd HH:mm:ss";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public T Deserialize<T>(IRestResponse response)
        {
            var _js = new JsonSerializerSettings()
            {
                DateFormatString = DateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return JsonConvert.DeserializeObject<T>(response.Content, _js);
        }

        public string RootElement
        {
            get;
            set;
        }

        public string Namespace
        {
            get;
            set;
        }

        public string DateFormat
        {
            get;
            set;
        }
    }
}