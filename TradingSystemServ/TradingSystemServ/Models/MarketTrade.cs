using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystemServ.Models
{
    public class MarketTrade
    {
        public string BuyUserID { get; set; }
        public string SellerUserID { get; set; }
        public string symbol { get; set; }
        public int TradedQuantity { get; set; }
        public double TradedPrice { get; set; }
    }
}
