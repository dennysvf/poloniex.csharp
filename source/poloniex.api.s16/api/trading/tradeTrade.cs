using Newtonsoft.Json;
using Poloniex.LIB.Configuration;
using System;

namespace Poloniex.API.Trade
{
    public interface ITrade : ITradeOrder
    {
        DateTime Time { get; }
    }

    public class Trade : TradeOrder, ITrade
    {
        [JsonProperty("date")]
        private string TimeInternal
        {
            set { Time = value.ParseDateTime(); }
        }
        public DateTime Time { get; private set; }
    }
}