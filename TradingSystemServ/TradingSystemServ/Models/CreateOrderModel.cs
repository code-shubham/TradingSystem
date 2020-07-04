using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystemServ.Models
{
    public class CreateOrderModel
    {
        public string userId { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public enum side
        {
            buy,
            sell
        }
       
    }
}


