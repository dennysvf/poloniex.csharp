using System;
using System.Collections.Generic;
using System.Linq;
using Poloniex.LIB.Configuration;

namespace Poloniex.API.Public
{
    public interface IPublicOrder
    {
        decimal PricePerCoin { get; }

        decimal AmountQuote { get; }
        decimal AmountBase { get; }
    }

    public class PublicOrder : IPublicOrder
    {
        public decimal PricePerCoin { get; private set; }

        public decimal AmountQuote { get; private set; }
        public decimal AmountBase
        {
            get { return (AmountQuote * PricePerCoin).Normalize(); }
        }

        internal PublicOrder(decimal pricePerCoin, decimal amountQuote)
        {
            PricePerCoin = pricePerCoin;
            AmountQuote = amountQuote;
        }

        internal PublicOrder()
        {

        }
    }
}