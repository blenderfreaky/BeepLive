namespace BeepLive.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class NetTcpServer
    {
        public TcpListener TcpListener { get; }

        public NetTcpServer(TcpListener tcpListener) => TcpListener = tcpListener;

        public void Broadcast<T>(T obj)
        {
        }
    }
}
