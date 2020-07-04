using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using TradingSystemServ.Models;
using TradingSystemServ.Implementation;
using System.Collections.Specialized;

namespace TradingSystemServ
{
    class Program
    {

        static void Main(string[] args)
        {
            TradingService tradingService = new TradingService();
            

            string[] vs = new string[10];
            vs[0] = "Order,A,Stock A,5,2.6,Buy";
            vs[1] = "Order,B,Stock A,10,2.7,Buy";
            vs[2] = "Order,A,Stock A,5,2.6,Buy";
            vs[3] = "Order,C,Stock A,17,2.6,sell";

            foreach (var item in vs)
            {
                tradingService.Transaction(item);
            }

            Socket m_ListenSocket;
            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            int iPort = 2085;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, iPort); 
            m_ListenSocket.Bind(endPoint);
            Console.WriteLine(LocalIpAddress());
            Console.WriteLine(iPort);

            m_ListenSocket.Listen(4);

            Socket AcceptedSocket = m_ListenSocket.Accept();

            byte[] reciveBuffer = new byte[1024];
            int iRecieveByteCount;

            iRecieveByteCount = AcceptedSocket.Receive(reciveBuffer, SocketFlags.None);
            if(iRecieveByteCount >0)
            {

                string msg = Encoding.ASCII.GetString(reciveBuffer, 0, iRecieveByteCount);                
            }
            AcceptedSocket.Shutdown(SocketShutdown.Both);
            AcceptedSocket.Close();

        }
        public static string LocalIpAddress()
        {
            IPHostEntry host;
            string localIp = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in host.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = address.ToString();
                    break;
                }

            }
            return localIp;
        }
    }
}
