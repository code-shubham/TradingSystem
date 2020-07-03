using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystemPOC
{
    public class Config
    {
        public DataTable dataTable { get; set; }
        public Socket socket { get; set; }
        public IPEndPoint endPoint { get; set; }

    }
}
