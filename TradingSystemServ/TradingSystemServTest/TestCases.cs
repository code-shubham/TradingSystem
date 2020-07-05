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
        [Test]
        public void CreateBuyOrder_Pass()
        {
            TradingService TradingService = new TradingService();
           // EmptyValues();
            bool status = false;
            TradingService.Transaction("<Create Order>,A,Stock A,5,2.6,<Buy>", null);
            if (TradingService.createOrderModelsList.Count>0)
                 status = true;

            Assert.IsTrue(status);
        }

        [TestCase("<Create Order>,A,Stock A,5,2.6,Bu")]
        public void CreateBuyOrder_fail(string Transaction)
        {
            TradingService TradingService = new TradingService();

            //  EmptyValues();
            bool status = false;            
            TradingService.Transaction(Transaction, null);
            if (TradingService.createOrderModelsList.Count <1)
                status = true;

            Assert.IsTrue(status);
        }

        [Test]
        public void CreateSellOrder_Fail_PriceChanged()

        {
            TradingService TradingService = new TradingService();

            // EmptyValues();
            bool status = false;
            TradingService.Transaction("<Create Order>,A,Stock A,20,2.6,<Buy>", null);
            TradingService.Transaction("<Create Order>,B,Stock A,17,2.7,<Sell>", null);

            if (TradingService.SellOrderStatus)
                status = false;
            Assert.IsFalse(status);
        }

        [Test]
        public void CreateSellOrder_Pass()
        {
            //  EmptyValues();
            TradingService TradingService = new TradingService();

            bool status = false;
            TradingService.Transaction("<Create Order>,A,Stock A,20,2.6,<Buy>", null);
            TradingService.Transaction("<Create Order>,B,Stock A,17,2.6,<Sell>", null);

            if (TradingService.SellOrderStatus)
                status = true;
            Assert.IsTrue(status);
        }

        [Test]
        public void CreateSellOrder_Fail()
        {
            TradingService TradingService = new TradingService();

            //  EmptyValues();
            bool status = false;        
            TradingService.Transaction("<Create Order>,B,Stock A,17,2.6,<Sell>", null);

            if (TradingService.SellOrderStatus)
                status = false;
            Assert.IsFalse(status);
        }

        [Test]
        public void AMarketTrad_Pass()
        {
            TradingService TradingService = new TradingService();

            // EmptyValues();
            bool status = false;
            TradingService.Transaction("<Create Order>,B,Stock A,30,2.9,<Buy>", null);
            TradingService.Transaction("<Create Order>,A,Stock A,27,2.9,<Sell>", null);

            if (TradingService.marketTradesList.Count>0)
                status = true;
            Assert.IsTrue(status);
        }


    }
}