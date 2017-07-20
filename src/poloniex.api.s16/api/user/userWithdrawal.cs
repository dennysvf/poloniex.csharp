using XCT.BaseLib.Configuration;
using Newtonsoft.Json;
using System;

namespace XCT.BaseLib.API.Poloniex.User
{
    public interface IWithdrawal
    {
        ulong Id { get; }

        string Currency { get; }
        string Address { get; }
        decimal Amount { get; }

        DateTime Time { get; }
        string IpAddress { get; }

        string Status { get; }
    }

    public class Withdrawal : IWithdrawal
    {
        [JsonProperty("withdrawalNumber")]
        public ulong Id { get; private set; }

        [JsonProperty("currency")]
        public string Currency { get; private set; }
        [JsonProperty("address")]
        public string Address { get; private set; }
        [JsonProperty("amount")]
        public decimal Amount { get; private set; }

        [JsonProperty("timestamp")]
        private ulong TimeInternal
        {
            set { Time = value.UnixTimeStampToDateTime(); }
        }
        public DateTime Time { get; private set; }
        [JsonProperty("ipAddress")]
        public string IpAddress { get; private set; }

        [JsonProperty("status")]
        public string Status { get; private set; }
    }
}