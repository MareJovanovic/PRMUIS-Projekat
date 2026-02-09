using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PRIMUS_Projekat.Src
{
    class ClientInfo
    {
            public int Id { get; set; }
            public TcpClient Client { get; set; }
            public NetworkStream Stream { get; set; }
    }
}
