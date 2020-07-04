using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSystemServ.Models;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;

namespace TradingSystemServ.Implementation
{
    public class TradingService
    {

        public OrderedDictionary myOrderedDictionary = new OrderedDictionary();

        public List<CreateOrderModel> createOrderModelsList = new List<CreateOrderModel>();
        public List<MarketTrade> marketTradesList = new List<MarketTrade>();
        public bool SellOrderStatus;
        public List<CreateOrderModel> PendingSellOrders = new List<CreateOrderModel>();

        /// <summary>
        /// Process Every Incoming Transaction
        /// </summary>
        /// <param name="Transaction">string Message consisting values delimited by ','</param>

        public void Transaction(string Transaction)
        {
            string[] array = new string[5];

            if (Transaction.Contains("Order"))            
                array = Transaction.Split(',');
            
            CreateOrderModel createOrder = new CreateOrderModel()
            {
                userId = array[1],
                Symbol = array[2],
                Quantity = int.Parse(array[3]),
                Price = double.Parse(array[4])
            };

            if (Transaction.Contains("Buy"))
            {
                CreateBuyOrder(createOrder);
                if (PendingSellOrders.Count > 0)
                    ProcessPendingSaleOrders();                
            }

            else if (Transaction.Contains("sell"))
            {
                PendingSellOrders.Add(createOrder);
                SellOrderStatus = CreateSellOrder(createOrder);

                if (SellOrderStatus)
                    PendingSellOrders.Remove(createOrder);
            }

        }

        /// <summary>
        /// Creates a sell order if there is a
        /// </summary>
        /// <param name="createOrderSell"></param>
        /// <returns></returns>
        internal bool CreateSellOrder(CreateOrderModel createOrderSell)
        {

            int sellValue = createOrderSell.Quantity;
            if (myOrderedDictionary.Values.Count > 0)
            {
                OrderedDictionary LocalOrderedDict = new OrderedDictionary();
                List<CreateOrderModel> LocalcreateOrderModelsList = new List<CreateOrderModel>();

                foreach (CreateOrderModel item in myOrderedDictionary.Keys)
                {
                    if (item.Price == createOrderSell.Price)
                    {
                        LocalOrderedDict.Add(item,item.Quantity);
                        LocalcreateOrderModelsList.Add(item);
                    }
                }

                ICollection keyCollection = LocalOrderedDict.Keys;
                ICollection valueCollection = LocalOrderedDict.Values;

                object[] myKeys = new object[LocalOrderedDict.Keys.Count];
                int[] myValues = new int[LocalOrderedDict.Values.Count];

                LocalOrderedDict.Values.CopyTo(myValues, 0);
                LocalOrderedDict.Keys.CopyTo(myKeys, 0);

                int SumOfAllQuantity = myValues.Sum();
                int tempval = 0;

                if (SumOfAllQuantity > sellValue)
                {
                    for (int i = 0; i < myKeys.Count(); i++)
                    {
                        tempval += myValues[i];
                        if (tempval >= SumOfAllQuantity)
                        {

                            LocalOrderedDict.RemoveAt(0);
                            LocalOrderedDict.Add(myValues[i], sellValue - (SumOfAllQuantity - myValues[i]));
                            LocalcreateOrderModelsList[0].Quantity = sellValue - (SumOfAllQuantity - myValues[i]);
                            ProcessMarketTrade(LocalcreateOrderModelsList[0].userId, createOrderSell.userId, createOrderSell.Symbol, LocalcreateOrderModelsList[0].Quantity, LocalcreateOrderModelsList[0].Price);
                            return true;
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

            return false;
        }

        /// <summary>
        /// creates the order when Buy is specified in the input transcation.
        /// Addes the value Mapped Object into a List and Dictionary
        /// </summary>
        /// <param name="createOrder"> Value Mapped Object</param>
        internal void CreateBuyOrder(CreateOrderModel createOrder)
        {
            createOrderModelsList.Add(createOrder);
            myOrderedDictionary.Add(createOrder, createOrder.Quantity);
        }

        /// <summary>
        /// It Maps the Market Trades and binds the object of Market Trades with Recently Traded Values
        /// </summary>
        /// <param name="BuyeruserID">Unique Buyer's ID e.g. ABC</param>
        /// <param name="SellerID">Unique Seller's ID e.g. 123</param>
        /// <param name="symbol">Symbol e.g Symbol A</param>
        /// <param name="Quantity">Quantity of the Given Symbol e.g. 10</param>
        /// <param name="price">Price of the Given Symbol e,g. 20.00</param>
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

        /// <summary>
        /// Process All the Pending Sell Transcations
        /// </summary>
        internal void ProcessPendingSaleOrders()
        {
            foreach (CreateOrderModel item in PendingSellOrders)
            {
                bool status = CreateSellOrder(item);
                if (status)
                    PendingSellOrders.Remove(item);
            }
        }
    }
}
