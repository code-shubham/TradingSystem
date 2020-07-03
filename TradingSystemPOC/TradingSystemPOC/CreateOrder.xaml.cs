using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TradingSystemPOC
{
    /// <summary>
    /// Interaction logic for CreateOrder.xaml
    /// </summary>
    public partial class CreateOrder : Window
    {
        public CreateOrder()
        {
            InitializeComponent();
           
        }
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
         
        }   
        public class CreateOrderVal
        {
            public string symbol { get; set; }
            public string option { get; set; }
            public string price { get; set; }

            public int Qty { get; set; }
        }
        DataTable dt;
        Config config = new Config();

        private void Window_Loaded(object sender, RoutedEventArgs e)

        {

            dt = new DataTable("MarketTrades");

            DataColumn dc1 = new DataColumn("Symbol", typeof(string));

            DataColumn dc2 = new DataColumn("BS", typeof(string));

            DataColumn dc3 = new DataColumn("Qty", typeof(string));

            DataColumn dc4 = new DataColumn("Price", typeof(string));

            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            config.dataTable.Rows.Add(dt);
        }
        

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            dt = new DataTable();
            DataRow dr = dt.NewRow();
            dr[0] = textBox1.Text;        
            dr[1] = textBox2.Text;
            dr[2] = textBox3.Text;        
            dr[3] = textBox4.Text;
            dt.Rows.Add(dr);
            config.dataTable.Rows.Add(dr);
            textBox1.Focus();
            MainWindow mainWindow = new MainWindow();
            
        }


      
    }
}
