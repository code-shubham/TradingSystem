using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystemServ.Models
{
    public class Model
    {
        public string Buyer { get; set; }
        public string Seller { get; set; }

        public int Qty { get; set; }
        public int Price { get; set; }
        public string Symbol { get; set; }
        public int MyBidQty { get; set; }
        public int MktBidQty { get; set; }
      
        public int MyAskQty { get; set; }
        public int MktAskQty { get; set; }
    }
}
