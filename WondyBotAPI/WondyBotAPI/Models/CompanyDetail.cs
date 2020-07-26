using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WondyBotAPI.Models
{
    public class CompanyDetail
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Currency { get; set; }
        public string Industry { get; set; } = "Other";
        public string Exchange { get; set; }
        public double CurrentPrice { get; set; }
        public double TargetPrice { get; set; }
        public dynamic Recommend { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Ticker}  \n\n" +
                $"{Industry} - {Exchange}  \n\n" +
                $"Current Price: {CurrentPrice} {Currency}  \n\n" +
                $"Target Price: {TargetPrice} {Currency}  \n\n" +
                $"{GetRecomendation()}";
        }

        private string GetRecomendation()
        {
            if (Recommend != null)
            {
                double total = Recommend.strongBuy + Recommend.buy +
                Recommend.hold + Recommend.sell + Recommend.strongSell;

                double strongBuy = (Recommend.strongBuy / total) * 100;
                double buy = (Recommend.buy / total) * 100;
                double hold = (Recommend.hold / total) * 100;
                double sell = (Recommend.sell / total) * 100;
                double strongSell = (Recommend.strongSell / total) * 100;

                return $"Strong Buy: {Math.Round(strongBuy, 1)}%|" +
                    $"Buy: {Math.Round(buy, 1)}%|" +
                    $"Hold: {Math.Round(hold, 1)}%|" +
                    $"Sell: {Math.Round(sell, 1)}%|" +
                    $"Strong Sell: {Math.Round(strongSell, 1)}%";
            }
            else
            {
                return null;
            }
        }
    }
}
