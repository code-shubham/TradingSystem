using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using TradingSystemServ.Models;

namespace TradingSystemServ
{
    class Program
    {

        static void Main(string[] args)
        {
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
                // serialize the client values to a model
                //Model model = JsonConvert.DeserializeObject<Model>(msg);
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
