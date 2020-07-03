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
    /// Interaction logic for MarketTrades.xaml
    /// </summary>
    public partial class MarketTrades : Window
    {
        public MarketTrades()
        {
            InitializeComponent();
           

        }
        Config config = new Config();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.ItemsSource = config.dataTable.DefaultView;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            config.dataTable = new DataTable();
            dataGrid1.ItemsSource = config.dataTable.DefaultView;

        }
 
      
    }
}
