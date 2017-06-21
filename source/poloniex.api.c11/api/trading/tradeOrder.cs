using Newtonsoft.Json;
using Poloniex.LIB.Configuration;
using Poloniex.LIB.Types;

namespace Poloniex.API.Trade
{
    public interface ITradeOrder
    {
        ulong IdOrder { get; }

        OrderType Type { get; }

        double PricePerCoin { get; }
        double AmountQuote { get; }
        double AmountBase { get; }
    }

    public class TradeOrder : ITradeOrder
    {
        [JsonProperty("orderNumber")]
        public ulong IdOrder { get; private set; }

        [JsonProperty("type")]
        private string TypeInternal
        {
            set { Type = value.ToOrderType(); }
        }
        public OrderType Type { get; private set; }

        [JsonProperty("rate")]
        public double PricePerCoin { get; private set; }
        [JsonProperty("amount")]
        public double AmountQuote { get; private set; }
        [JsonProperty("total")]
        public double AmountBase { get; private set; }
    }
}