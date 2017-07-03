using Newtonsoft.Json;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Serialize;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XCT.BaseLib.API
{
    /// <summary>
    /// 
    /// </summary>
    public class XApiClient : IDisposable
    {
        private string __api_url = "";

        protected string __connect_key;
        protected string __secret_key;

        private const string __content_type = "application/json";
        private const string __user_agent = "btc-trading/5.2.2017.01";

        /// <summary>
        /// 
        /// </summary>
        public XApiClient(string api_url, string connect_key, string secret_key)
        {
            __api_url = api_url;
            __connect_key = connect_key;
            __secret_key = secret_key;
        }

        private static char[] __to_digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public byte[] EncodeHex(byte[] data)
        {
            int l = data.Length;
            byte[] _result = new byte[l << 1];

            // two characters form the hex value.
            for (int i = 0, j = 0; i < l; i++)
            {
                _result[j++] = (byte)__to_digits[(0xF0 & data[i]) >> 4];
                _result[j++] = (byte)__to_digits[0x0F & data[i]];
            }

            return _result;
        }

        public string EncodeURIComponent(Dictionary<string, object> rgData)
        {
            string _result = String.Join("&", rgData.Select((x) => String.Format("{0}={1}", x.Key, x.Value)));

            _result = System.Net.WebUtility.UrlEncode(_result)
                        .Replace("+", "%20").Replace("%21", "!")
                        .Replace("%27", "'").Replace("%28", "(")
                        .Replace("%29", ")").Replace("%26", "&")
                        .Replace("%3D", "=").Replace("%7E", "~");

            return _result;
        }

        public IRestClient CreateJsonClient(string baseurl)
        {
            var _client = new RestClient(baseurl);
            {
                _client.RemoveHandler(__content_type);
                _client.AddHandler(__content_type, new RestSharpJsonNetDeserializer());
                _client.Timeout = 10 * 1000;
                _client.UserAgent = __user_agent;
            }

            return _client;
        }

        public IRestRequest CreateJsonRequest(string resource, Method method = Method.GET)
        {
            var _request = new RestRequest(resource, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new RestSharpJsonNetSerializer()
            };

            return _request;
        }

        public Dictionary<string, object> GetHttpHeaders(string endpoint, Dictionary<string, object> rgData, string apiKey, string apiSecret)
        {
            var _nonce = CUnixTime.NowMilli.ToString();
            var _data = EncodeURIComponent(rgData);
            var _message = String.Format("{0};{1};{2}", endpoint, _data, _nonce);

            var _secretKey = Encoding.UTF8.GetBytes(apiSecret);
            var _hmac = new HMACSHA512(_secretKey);
            _hmac.Initialize();

            var _bytes = Encoding.UTF8.GetBytes(_message);
            var _rawHmac = _hmac.ComputeHash(_bytes);

            var _encoded = EncodeHex(_rawHmac);
            var _signature = Convert.ToBase64String(_encoded);

            var _headers = new Dictionary<string, object>();
            {
                _headers.Add("Api-Key", apiKey);
                _headers.Add("Api-Sign", _signature);
                _headers.Add("Api-Nonce", _nonce);
            }

            return _headers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> CallApiPostAsync<T>(string endpoint, Dictionary<string, object> args = null) where T : new()
        {
            var _request = CreateJsonRequest(endpoint, Method.POST);
            {
                var _params = new Dictionary<string, object>();
                {
                    _params.Add("endpoint", endpoint);
                    if (args != null)
                    {
                        foreach (var a in args)
                            _params.Add(a.Key, a.Value);
                    }
                }

                var _headers = GetHttpHeaders(endpoint, _params, __connect_key, __secret_key);
                foreach (var h in _headers)
                    _request.AddHeader(h.Key, h.Value.ToString());

                foreach (var a in _params)
                    _request.AddParameter(a.Key, a.Value);
            }

            var _client = CreateJsonClient(__api_url);
            {
                var tcs = new TaskCompletionSource<T>();
                _client.ExecuteAsync(_request, response =>
                {
                    tcs.SetResult(JsonConvert.DeserializeObject<T>(response.Content));
                });

                return await tcs.Task;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> CallApiGetAsync<T>(string endpoint, Dictionary<string, object> args = null) where T : new()
        {
            var _request = CreateJsonRequest(endpoint, Method.GET);

            if (args != null)
            {
                foreach (var a in args)
                    _request.AddParameter(a.Key, a.Value);
            }

            var _client = CreateJsonClient(__api_url);
            {
                var tcs = new TaskCompletionSource<T>();
                _client.ExecuteAsync(_request, response =>
                {
                    tcs.SetResult(JsonConvert.DeserializeObject<T>(response.Content));
                });

                return await tcs.Task;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }
}