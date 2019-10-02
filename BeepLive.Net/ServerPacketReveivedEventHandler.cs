namespace BeepLive.Net
{
    public delegate void ServerPacketReveivedEventHandler(NetTcpServer netTcpServer, NetTcpClient netTcpClient, object packet);

    public delegate void ClientPacketReveivedEventHandler(NetTcpClient netTcpClient, object packet);
}