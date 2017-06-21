using Newtonsoft.Json;
using RestSharp.Serializers;

namespace Poloniex.LIB.Serialize
{
    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class RestSharpJsonNetSerializer : ISerializer
    {
        /// <summary>
        /// Default serializer
        /// </summary>
        public RestSharpJsonNetSerializer()
        {
            ContentType = "application/json";
            DateFormat = "yyyy-MM-dd HH:mm:ss";
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize
        /// <returns>JSON as String</returns>
        public string Serialize(object obj)
        {
            var _js = new JsonSerializerSettings()
            {
                DateFormatString = DateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return JsonConvert.SerializeObject(obj, _js);
        }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat
        {
            get;
            set;
        }
        
        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement
        {
            get;
            set;
        }
        
        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace
        {
            get;
            set;
        }
 
        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }
    }
}