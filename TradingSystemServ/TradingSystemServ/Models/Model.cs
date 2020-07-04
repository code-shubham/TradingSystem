using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace TradingSystemServ.Models
{
    public  class Model
    {
        public OrderedDictionary myOrderedDictionary { get; set; }
        //  public Dictionary <string, int> UsersAndQuantity { get; set; }
        public List<CreateOrderModel> createOrderModelsList { get; set; }
        public List<MarketTrade> marketTradesList { get; set; }
    }
}
