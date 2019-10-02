using System.Net;
using System.Net.Sockets;
using System;
namespace BeepLive.Server
{
    using BeepLive.Client;
    using BeepLive.Config;
    using BeepLive.Net;
    using BeepLive.Network;
    using Microsoft.Extensions.Configuration;
    using Networker.Server.Abstractions;
    using ProtoBuf;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System;
    using Microsoft.Extensions.Logging;
    using System.Net;

    public class ServerPlayer
    {
        public string PlayerGuid { get; set; }
        public string Secret { get; set; }

        public ServerPlayerState State { get; set; }
        public bool Finished { get; set; }

        public void MoveToState(ServerPlayerState state)
        {
            State = state;
            Finished = false;
        }

        public override string ToString() =>
            $"{nameof(PlayerGuid)}: {PlayerGuid} | {nameof(Secret)}: {Secret} | {nameof(State)}: {State} | {nameof(Finished)}: {Finished}";
    }

    public enum ServerPlayerState
    {
        InTeamSelection,
        InSpawning,
        InPlanning,
        InSimulation
    }
}