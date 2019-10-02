namespace BeepLive.Net
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public delegate void PacketReveivedEventHandler(NetTcpServer netTcpServer, NetTcpClient netTcpClient, object packet);
}
