namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using Net;
    using System;
    using System.Collections.Generic;

    internal readonly struct PacketContext<TPacket> : IEquatable<PacketContext<TPacket>> where TPacket : Packet
    {
        public readonly TPacket Packet;
        public readonly BeepServer Server;
        public readonly NetTcpClient Sender;
        public readonly ILogger Logger;

        public PacketContext(TPacket packet, BeepServer server, NetTcpClient sender, ILogger logger)
        {
            Packet = packet;
            Server = server;
            Sender = sender;
            Logger = logger;
        }

        public override bool Equals(object obj) => obj is PacketContext<TPacket> context
            && EqualityComparer<BeepServer>.Default.Equals(Server, context.Server)
            && EqualityComparer<NetTcpClient>.Default.Equals(Sender, context.Sender)
            && EqualityComparer<ILogger>.Default.Equals(Logger, context.Logger)
            && EqualityComparer<TPacket>.Default.Equals(Packet, context.Packet);

        public override int GetHashCode() => HashCode.Combine(Server, Sender, Logger, Packet);

        public static bool operator ==(PacketContext<TPacket> left, PacketContext<TPacket> right) => left.Equals(right);

        public static bool operator !=(PacketContext<TPacket> left, PacketContext<TPacket> right) => !(left == right);

        public bool Equals(PacketContext<TPacket> other) =>
            EqualityComparer<BeepServer>.Default.Equals(Server, other.Server)
            && EqualityComparer<NetTcpClient>.Default.Equals(Sender, other.Sender)
            && EqualityComparer<ILogger>.Default.Equals(Logger, other.Logger)
            && EqualityComparer<TPacket>.Default.Equals(Packet, other.Packet);
    }
}