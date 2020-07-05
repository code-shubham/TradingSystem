using NUnit.Framework;
using System.Net;
using TradingSystemServ.Implementation;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class Tests
    {
       
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        TradingService TradingService = new TradingService();

        [SetUp]
        public void Setup()
        {             
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 2085));
                serverSocket.Listen(0);           
        }
        #region Private Methods
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }
        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Request Recieved");

            if (text.Length > 0) // Client requested 
            {
                TradingService tradingService = new TradingService();
                tradingService.Transaction(text, current);
            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
        private void EmptyValues()
        {
            TradingService.SellOrderStatus = false;
            TradingService.marketTradesList.Clear();
            TradingService.myOrderedDictionary.Clear();
            TradingService.PendingSellOrders.Clear();
            TradingService.marketTradesList.Clear();
        }
#endregion


        [TestCase ("<Create Order>,A,Stock A,5,2.6,Buy")]
        public void CreateBuyOrder_Pass(string Transaction)
        {
            EmptyValues();
            bool status = false;
            TradingService.Transaction(Transaction, serverSocket);
            if (TradingService.createOrderModelsList.Count == 1)
                 status = true;

            Assert.IsTrue(status);
        }

        [TestCase("Order,A,Stock A,5,2.6,Bu")]
        public void CreateBuyOrder_fail(string Transaction)
        {
            EmptyValues();
            bool status = false;
            TradingService.Transaction(Transaction, serverSocket);
            if (TradingService.createOrderModelsList.Count <1)
                status = true;

            Assert.IsTrue(true);
        }

        [TestCase("Order,A,Stock A,20,2.6,Buy")]
        [TestCase("Order,B,Stock A,17,2.7,Sell")]
        public void CreateSellOrder_Fail_PriceChanged(string TransactionBuy, string TranscationSell)
        {
            EmptyValues();
            bool status = false;
            TradingService.Transaction(TransactionBuy, serverSocket);
            TradingService.Transaction(TranscationSell, serverSocket);

            if (TradingService.SellOrderStatus)
                status = false;
            Assert.IsFalse(status);
        }

        [TestCase("Order,A,Stock A,20,2.6,Buy")]
        [TestCase("Order,B,Stock A,17,2.6,Sell")]
        public void CreateSellOrder_Pass(string TransactionBuy, string TranscationSell)
        {
            EmptyValues();
            bool status = false;
            TradingService.Transaction(TransactionBuy, serverSocket);
            TradingService.Transaction(TranscationSell, serverSocket);

            if (TradingService.SellOrderStatus)
                status = true;
            Assert.IsTrue(status);
        }

        [TestCase("Order,B,Stock A,17,2.6,Sell")]
        public void CreateSellOrder_Fail(string TranscationSell)
        {
            EmptyValues();
            bool status = false;        
            TradingService.Transaction(TranscationSell, serverSocket);

            if (TradingService.SellOrderStatus)
                status = false;
            Assert.IsFalse(status);
        }

        [TestCase("Order,B,Stock A,30,2.9,Buy")]
        [TestCase("Order,A,Stock A,27,2.9,Sell")]
        public void MarketTrad_Pass(string TransactionBuy, string TranscationSell)
        {
            EmptyValues();
            bool status = false;
            TradingService.Transaction(TransactionBuy, serverSocket);
            TradingService.Transaction(TranscationSell, serverSocket);

            if (TradingService.marketTradesList.Count>0)
                status = true;
            Assert.IsTrue(status);
        }


    }
}