namespace BeepLive.Net
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public delegate void ServerPacketReveivedEventHandler(NetTcpServer netTcpServer, NetTcpClient netTcpClient, object packet);
    public delegate void ClientPacketReveivedEventHandler(NetTcpClient netTcpClient, object packet);
}
