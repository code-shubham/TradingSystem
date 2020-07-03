using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TradingSystemPOC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        Config config = new Config();

        public MainWindow()
        {
            Socket socket;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress destinationIP = IPAddress.Parse("192.168.1.4");
            int destinationPort = System.Convert.ToInt32("2085");
            IPEndPoint endPoint = new IPEndPoint(destinationIP, destinationPort);
            string obj = "";
            sendMessage(obj);
            config.socket = socket;
            config.endPoint = endPoint;
            InitializeComponent();            
        }
        public void sendMessage(string message)
        {
            config.socket.Connect(config.endPoint);            
            byte[] vs = System.Text.Encoding.ASCII.GetBytes(message.ToString());
            config.socket.Send(vs, SocketFlags.None);

            
        }
        private void Button1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            CreateOrder createOrder = new CreateOrder();
            createOrder.Show();
        }
        private void Button2(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MarketOrderBook marketOrderBook = new MarketOrderBook();
            marketOrderBook.Show();
         
        }
        private void Button3(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MarketTrades marketTrades = new MarketTrades();
            marketTrades.Show();
            
        }

    }
}
