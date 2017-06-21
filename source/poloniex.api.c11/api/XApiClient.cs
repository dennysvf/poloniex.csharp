using Newtonsoft.Json;
using Poloniex.LIB.Configuration;
using Poloniex.LIB.Serialize;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poloniex.API
{
    /// <summary>
    /// 
    /// </summary>
    public class XApiClient : IDisposable
    {
        private const string __api_url = "https://poloniex.com";

        private string __connect_key;
        private string __secret_key;

        private const string __content_type = "application/x-www-form-urlencoded";
        private const string __user_agent = "btc-trading/5.2.2017.01";

        private static char[] __to_digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        private HMACSHA512 __encryptor = null;
        public HMACSHA512 Encryptor
        {
            get
            {
                if (__encryptor == null)
                    __encryptor = new HMACSHA512(Encoding.UTF8.GetBytes(__secret_key));

                return __encryptor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XApiClient()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public XApiClient(string connect_key, string secret_key)
        {
            __connect_key = connect_key;
            __secret_key = secret_key;

            Encryptor.Key = Encoding.UTF8.GetBytes(__secret_key);
        }

        protected byte[] EncodeHex(byte[] data)
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

        protected string EncodeURIComponent(Dictionary<string, object> rgData)
        {
            string _result = String.Join("&", rgData.Select((x) => String.Format("{0}={1}", x.Key, x.Value)));

            _result = System.Net.WebUtility.UrlEncode(_result)
                        .Replace("+", "%20").Replace("%21", "!")
                        .Replace("%27", "'").Replace("%28", "(")
                        .Replace("%29", ")").Replace("%26", "&")
                        .Replace("%3D", "=").Replace("%7E", "~");

            return _result;
        }

        protected IRestClient CreateJsonClient(string baseurl)
        {
            var _client = new RestClient(baseurl);
            {
                _client.RemoveHandler(__content_type);
                _client.AddHandler(__content_type, new RestSharpJsonNetDeserializer());

                _client.Timeout = Timeout.Infinite;
                _client.UserAgent = __user_agent;
            }

            return _client;
        }

        protected IRestRequest CreateJsonRequest(string resource, Method method = Method.GET)
        {
            var _request = new RestRequest(resource, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new RestSharpJsonNetSerializer()
            };

            return _request;
        }

        private BigInteger CurrentHttpPostNonce
        {
            get;
            set;
        }

        private string GetCurrentHttpPostNonce()
        {
            var _ne_nonce = new BigInteger(
                                    Math.Round(
                                        DateTime.UtcNow.Subtract(
                                            UnixTime.DateTimeUnixEpochStart
                                        )
                                        .TotalMilliseconds * 1000, 
                                        MidpointRounding.AwayFromZero
                                    )
                                );

            if (_ne_nonce > CurrentHttpPostNonce)
            {
                CurrentHttpPostNonce = _ne_nonce;
            }
            else
            {
                CurrentHttpPostNonce += 1;
            }

            return CurrentHttpPostNonce.ToString(CultureInfo.InvariantCulture);
        }

        private string HttpPostString(List<Parameter> dictionary)
        {
            var _result = "";

            foreach (var _entry in dictionary)
            {
                var _value = _entry.Value as string;
                if (_value == null)
                    _result += "&" + _entry.Name+ "=" + _entry.Value;
                else
                    _result += "&" + _entry.Name + "=" + _value.Replace(' ', '+');
            }

            return _result.Substring(1);
        }

        private string ConvertHexString(byte[] value)
        {
            var _result = "";

            for (var i = 0; i < value.Length; i++)
                _result += value[i].ToString("x2", CultureInfo.InvariantCulture);

            return _result;
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
                    _params.Add("nonce", GetCurrentHttpPostNonce());

                    if (args != null)
                    {
                        foreach (var a in args)
                            _params.Add(a.Key, a.Value);
                    }
                }

                foreach (var _p in _params)
                    _request.AddParameter(_p.Key, _p.Value);

                var _post_data = HttpPostString(_request.Parameters);
                var _post_bytes = Encoding.UTF8.GetBytes(_post_data);
                var _post_hash = Encryptor.ComputeHash(_post_bytes);

                var _signature = ConvertHexString(_post_hash);
                {
                    _request.AddHeader("Key", __connect_key);
                    _request.AddHeader("Sign", _signature);
                }
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