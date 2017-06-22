using Newtonsoft.Json;
using Poloniex.LIB.Configuration;
using Poloniex.LIB.Types;
using System;

namespace Poloniex.API.Public
{
    public interface IPublicTrade
    {
        DateTime Time { get; }

        OrderType Type { get; }

        decimal PricePerCoin { get; }

        decimal AmountQuote { get; }
        decimal AmountBase { get; }
    }

    public class PublicTrade : IPublicTrade
    {
        [JsonProperty("date")]
        private string TimeInternal
        {
            set { Time = value.ParseDateTime(); }
        }

        public DateTime Time { get; private set; }

        [JsonProperty("type")]
        private string TypeInternal
        {
            set { Type = value.ToOrderType(); }
        }

        public OrderType Type { get; private set; }

        [JsonProperty("rate")]
        public decimal PricePerCoin { get; private set; }

        [JsonProperty("amount")]
        public decimal AmountQuote { get; private set; }

        [JsonProperty("total")]
        public decimal AmountBase { get; private set; }
    }
}