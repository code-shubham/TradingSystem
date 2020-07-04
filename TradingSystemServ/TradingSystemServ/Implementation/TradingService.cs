using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSystemServ.Models;
using System.Collections.Specialized;
using System.Collections;

namespace TradingSystemServ.Implementation
{
    public class TradingService
    {
      
        public OrderedDictionary myOrderedDictionary = new OrderedDictionary();
      
        public List<CreateOrderModel> createOrderModelsList = new List<CreateOrderModel>();
        public List<MarketTrade> marketTradesList = new List<MarketTrade>();

        public void Transcation(string Transcation)
        {
            

            string[] array = new string[5];
            if (Transcation.Contains("Order"))
            {
                array = Transcation.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            CreateOrderModel createOrder = new CreateOrderModel() {
                userId = array[1],
                Symbol = array[2],
                Quantity = int.Parse(array[3]),
                Price = double.Parse(array[4])                      
            };

            if(Transcation.Contains("Buy"))
            {
                CreateBuyOrder(createOrder);
            }
            else if(Transcation.Contains("sell"))
            {
                CreateSellOrder(createOrder);
            }
        }

        internal void CreateSellOrder(CreateOrderModel createOrderSell)
        {
           
            int sellValue = createOrderSell.Quantity;
            if(myOrderedDictionary.Values.Count > 0)
            {
                OrderedDictionary LocalOrderedDict = new OrderedDictionary();
                List<CreateOrderModel> LocalcreateOrderModelsList = new List<CreateOrderModel>();

                LocalOrderedDict = myOrderedDictionary;
                LocalcreateOrderModelsList = createOrderModelsList;

                ICollection keyCollection = LocalOrderedDict.Keys;
                ICollection valueCollection = LocalOrderedDict.Values;

                object[] myKeys = new object[LocalOrderedDict.Keys.Count];
                int[] myValues = new int[LocalOrderedDict.Values.Count];

                LocalOrderedDict.Values.CopyTo(myValues, 0);
                LocalOrderedDict.Keys.CopyTo(myKeys, 0);

                int SumOfAllQuantity = myValues.Sum();
                int tempval = 0;

                if(SumOfAllQuantity > sellValue)
                {
                    for (int i = 0; i < myKeys.Count(); i++)
                    {
                        tempval += myValues[i];
                        if(tempval >= SumOfAllQuantity)
                        {

                            LocalOrderedDict.RemoveAt(0);
                            LocalOrderedDict.Add(myValues[i], sellValue - (SumOfAllQuantity - myValues[i]));
                            LocalcreateOrderModelsList[0].Quantity = sellValue - (SumOfAllQuantity - myValues[i]);
                            ProcessMarketTrade(LocalcreateOrderModelsList[0].userId, createOrderSell.userId, createOrderSell.Symbol, LocalcreateOrderModelsList[0].Quantity, LocalcreateOrderModelsList[0].Price);
                            break;
                        }
                        else
                        {
                            ProcessMarketTrade(LocalcreateOrderModelsList[0].userId, createOrderSell.userId, createOrderSell.Symbol, LocalcreateOrderModelsList[0].Quantity, LocalcreateOrderModelsList[0].Price);
                            LocalOrderedDict.RemoveAt(0);
                            LocalcreateOrderModelsList.RemoveAt(0);
                            
                        }      
                    }
                }
            }           
        }

        internal void CreateBuyOrder(CreateOrderModel createOrder)
        {
            

             createOrderModelsList.Add(createOrder);
             myOrderedDictionary.Add(createOrder, createOrder.Quantity);

        }

        internal void ProcessMarketTrade(string BuyeruserID, string SellerID, string symbol, int Quantity, double price)
        {
            MarketTrade marketTrade = new MarketTrade()
            {
                BuyUserID = BuyeruserID,
                SellerUserID = SellerID,
                symbol = symbol,
                TradedQuantity = Quantity,
                TradedPrice = price
            };
            marketTradesList.Add(marketTrade);
        }
    }
}
