using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingSystemServ.Models;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace TradingSystemServ.Implementation
{
    public class TradingService
    {
        //Contains CreateOrder Model Object as a key and Quantities as values.
        public OrderedDictionary myOrderedDictionary = new OrderedDictionary();

        //Contains CreateOrderModel as its element
        public List<CreateOrderModel> createOrderModelsList = new List<CreateOrderModel>();

        //Contains Successfully Traded items
        public List<MarketTrade> marketTradesList = new List<MarketTrade>();

        //Boolean value indicating a success or failure of a Trade
        public bool SellOrderStatus;

        //Contains All the Pending sell Orders
        public List<CreateOrderModel> PendingSellOrders = new List<CreateOrderModel>();

        //Current Socket Connection can be utilized for sending the message or making a live Transaction.
        public Socket CurrentSocket;
        

        /// <summary>
        /// Process Every Incoming Transaction
        /// </summary>
        /// <param name="Transaction">string Message consisting values delimited by ','</param>    
        public void Transaction(string Transaction, Socket socket)
        {
            //Saving the current Socket object to utilize it further.
            CurrentSocket = socket;
            string[] array = new string[5];

            //Validating whether the Transaction string contains "<Create Order>" Tag
            if (Transaction.Contains("<Create Order>"))           
                array = Transaction.Split(',');

            //Mapping the Values of Transaction string into the Create order Model
            CreateOrderModel createOrder = new CreateOrderModel()
            {
                userId = array[1],
                Symbol = array[2],
                Quantity = int.Parse(array[3]),
                Price = double.Parse(array[4])
            };

            //Validating Transaction the string contains "Buy Or Sell Tag"
            if (Transaction.Contains("<Buy>"))
            {
                CreateBuyOrder(createOrder);

                //Check Whether if there is any pending sell record in Queue If yes then Process it.
                if (PendingSellOrders.Count > 0)
                    ProcessPendingSaleOrders();                
            }

            else if (Transaction.Contains("<Sell>"))
            {
                // Add  The Sell record in Pending Queue
                PendingSellOrders.Add(createOrder);
                SellOrderStatus = CreateSellOrder(createOrder);
                
                //Check the Sell Record Status if Traded Succesfully Proceed than remove it from Queue
                if (SellOrderStatus)
                    PendingSellOrders.Remove(createOrder);
            }

        }

        /// <summary>
        /// Creates a sell order 
        /// </summary>
        /// <param name="createOrderSell"></param>
        /// <returns></returns>
        internal bool CreateSellOrder(CreateOrderModel createOrderSell)
        {
            //Gets the Current Sell Quantity
            int sellValue = createOrderSell.Quantity;

            //Proceed the sell order only if one Buy item is present else return false.
            if (myOrderedDictionary.Values.Count > 0)
            {
                OrderedDictionary LocalOrderedDict = new OrderedDictionary();
                List<CreateOrderModel> LocalcreateOrderModelsList = new List<CreateOrderModel>();

                //Add only those object having same values as sell price
                foreach (CreateOrderModel item in myOrderedDictionary.Keys)
                {
                    if ((item.Price == createOrderSell.Price) && (item.userId != createOrderSell.userId))
                    {
                        LocalOrderedDict.Add(item,item.Quantity);
                        LocalcreateOrderModelsList.Add(item);
                    }
                }

                //Gets the keys and values of the Dictionary 
                ICollection keyCollection = LocalOrderedDict.Keys;
                ICollection valueCollection = LocalOrderedDict.Values;

                CreateOrderModel[] myKeys = new CreateOrderModel[LocalOrderedDict.Keys.Count];
                int[] myValues = new int[LocalOrderedDict.Values.Count];

                LocalOrderedDict.Values.CopyTo(myValues, 0);
                LocalOrderedDict.Keys.CopyTo(myKeys, 0);

                //Add all the values of dictionary to find the sum of all Buy Orders Qunatites
                int SumOfAllQuantity = myValues.Sum();
                int tempval = 0;

                //Proceed only if the sum is greater than current sale Quantites else return false.
                if (SumOfAllQuantity > sellValue)
                {
                    for (int i = 0; i < myKeys.Count(); i++)
                    {
                        // Add Every Key untill it equates or Exceeds the current sale Quantity in a cronological order
                        tempval += myValues[i];
                        if (tempval >= SumOfAllQuantity)
                        {
                            //Remove the Traded item from Dictionary and List
                            //Sell the Quantites which are getting sold and reset the values with not sold ones.
                            LocalOrderedDict.RemoveAt(0);

                            //Update the remaining Qunatity and re-save it
                            CreateOrderModel model = myKeys[i];
                            model.Quantity = sellValue - (SumOfAllQuantity - myValues[i]);

                            LocalOrderedDict.Add(model, sellValue - (SumOfAllQuantity - myValues[i]));
                            LocalcreateOrderModelsList[0].Quantity = sellValue - (SumOfAllQuantity - myValues[i]);

                            //Add Traded item with the details of Buyer and Seller to send them to client
                            ProcessMarketTrade(LocalcreateOrderModelsList[0].userId, createOrderSell.userId, createOrderSell.Symbol, LocalcreateOrderModelsList[0].Quantity, LocalcreateOrderModelsList[0].Price);
                            return true;
                        }
                        else
                        {
                            //Add Trade with the details of Buyer and Seller
                            ProcessMarketTrade(LocalcreateOrderModelsList[0].userId, createOrderSell.userId, createOrderSell.Symbol, LocalcreateOrderModelsList[0].Quantity, LocalcreateOrderModelsList[0].Price);

                            //Remove the Traded item from Dictionary and List to Update the Market Order Book status
                            LocalOrderedDict.RemoveAt(0);
                            LocalcreateOrderModelsList.RemoveAt(0);
                        }
                    }
                }
            }
            //Returns False in case of any failure
            return false;
        }

        #region Matching System
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
            string OutData = "MarketTrade," + marketTrade.BuyUserID + "," + marketTrade.SellerUserID + "," + marketTrade.symbol + "," + marketTrade.TradedQuantity + "," + marketTrade.TradedPrice;
            SendDataToClient(OutData);
        }

        #endregion
        /// <summary>
        /// Process All the Pending Sell Transcations
        /// </summary>
        internal void ProcessPendingSaleOrders()
        {
            for(int i =0;i<PendingSellOrders.Count;i++)
            {
                bool status = CreateSellOrder(PendingSellOrders[i]);
                if (status)
                    PendingSellOrders.Remove(PendingSellOrders[i]);
            }
        }

        /// <summary>
        /// Sends the Data and Current Trades to the client machines in the current socket Session.
        /// </summary>
        /// <param name="OutData"></param>
        internal void SendDataToClient(string OutData)
        {
            byte[] Buffer = Encoding.ASCII.GetBytes(OutData);
            if (CurrentSocket != null)
            {

            }
               // CurrentSocket.Send(Buffer);

        }

    }
}
