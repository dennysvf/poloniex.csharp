using System;
using Newtonsoft.Json;
using Poloniex.LIB.Configuration;

namespace Poloniex.API.User
{
    public interface IDeposit
    {
        string Currency { get; }
        string Address { get; }
        double Amount { get; }

        DateTime Time { get; }
        string TransactionId { get; }
        uint Confirmations { get; }

        string Status { get; }
    }

    public class Deposit : IDeposit
    {
        [JsonProperty("currency")]
        public string Currency { get; private set; }
        [JsonProperty("address")]
        public string Address { get; private set; }
        [JsonProperty("amount")]
        public double Amount { get; private set; }

        [JsonProperty("timestamp")]
        private ulong TimeInternal
        {
            set { Time = value.UnixTimeStampToDateTime(); }
        }
            
        public DateTime Time { get; private set; }
        [JsonProperty("txid")]
        public string TransactionId { get; private set; }
        [JsonProperty("confirmations")]
        public uint Confirmations { get; private set; }

        [JsonProperty("status")]
        public string Status { get; private set; }
    }
}