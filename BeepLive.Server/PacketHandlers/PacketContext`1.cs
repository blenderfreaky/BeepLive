namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal readonly struct PacketContext<TPacket> : IEquatable<PacketContext<TPacket>> where TPacket : Packet
    {
        public readonly BeepServer Server;
        public readonly Action<Packet> SendResponse;
        public readonly TcpClient Sender;
        public readonly ILogger Logger;
        public readonly TPacket Packet;

        public override bool Equals(object obj) => obj is PacketContext<TPacket> context
            && EqualityComparer<BeepServer>.Default.Equals(Server, context.Server)
            && EqualityComparer<Action<Packet>>.Default.Equals(SendResponse, context.SendResponse)
            && EqualityComparer<TcpClient>.Default.Equals(Sender, context.Sender)
            && EqualityComparer<ILogger>.Default.Equals(Logger, context.Logger)
            && EqualityComparer<TPacket>.Default.Equals(Packet, context.Packet);

        public override int GetHashCode() => HashCode.Combine(Server, SendResponse, Sender, Logger, Packet);

        public static bool operator ==(PacketContext<TPacket> left, PacketContext<TPacket> right) => left.Equals(right);

        public static bool operator !=(PacketContext<TPacket> left, PacketContext<TPacket> right) => !(left == right);

        public bool Equals(PacketContext<TPacket> other) => EqualityComparer<BeepServer>.Default.Equals(Server, other.Server)
            && EqualityComparer<Action<Packet>>.Default.Equals(SendResponse, other.SendResponse)
            && EqualityComparer<TcpClient>.Default.Equals(Sender, other.Sender)
            && EqualityComparer<ILogger>.Default.Equals(Logger, other.Logger)
            && EqualityComparer<TPacket>.Default.Equals(Packet, other.Packet);
    }
}