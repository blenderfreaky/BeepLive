using BeepLive.Network;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using static BeepLive.Server.BeepServer;

#pragma warning disable 1998

namespace BeepLive.Server.PacketHandlers
{
    public class ServerPlayerFlowPacketHandler : PacketHandlerBase<PlayerFlowPacket>
    {
        private readonly ILogger<ServerPlayerFlowPacketHandler> _logger;

        public ServerPlayerFlowPacketHandler(ILogger<ServerPlayerFlowPacketHandler> logger)
        {
            _logger = logger;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override async Task Process(PlayerFlowPacket packet, IPacketContext packetContext)
        {
            _logger.LogDebug("Received: " + packet);

            if (packet.Type != PlayerFlowPacket.FlowType.Join && !IsValid(packet))
            {
                _logger.LogWarning(
                    $"Received packet with invalid Secret: {packet}\nSent by: {packetContext.Sender.EndPoint}");
                return;
            }

            ServerPlayer player = Players.Find(p => string.Equals(p.PlayerGuid, packet.PlayerGuid, StringComparison.InvariantCulture));

            if (player == null && packet.Type != PlayerFlowPacket.FlowType.Join)
            {
                _logger.LogWarning(
                    $"Unknown player sent flow-packet: {packet}\nSent by: {packetContext.Sender.EndPoint}");
                return;
            }

            switch (packet.Type)
            {
                case PlayerFlowPacket.FlowType.Join:
                    if (!AllPlayersInState(ServerPlayerState.InTeamSelection, false))
                    {
                        _logger.LogWarning($"Player joined late: {packet}\nPlayer: {player}");
                        return;
                    }

                    if (player == null)
                        Players.Add(new ServerPlayer
                        {
                            PlayerGuid = packet.PlayerGuid,
                            Secret = packet.Secret,
                            State = ServerPlayerState.InTeamSelection
                        });

                    packetContext.Sender.Send(new SyncPacket(BeepConfig));
                    packetContext.Sender.Send(new ServerFlowPacket { Type = ServerFlowType.StartTeamSelection });

                    break;

                case PlayerFlowPacket.FlowType.Leave:
                    Players.Remove(player);
                    BroadcastWithoutSecret(packet);

                    break;

                case PlayerFlowPacket.FlowType.LockInTeam:
                    TryFlow(packet, player, ServerPlayerState.InTeamSelection, ServerPlayerState.InSpawning,
                        ServerFlowType.StartSpawning);

                    break;

                case PlayerFlowPacket.FlowType.Spawn:
                    TryFlow(packet, player, ServerPlayerState.InSpawning, ServerPlayerState.InSimulation,
                        ServerFlowType.StartSimulation);

                    break;

                case PlayerFlowPacket.FlowType.ReadyForSimulation:
                    TryFlow(packet, player, ServerPlayerState.InSimulation, ServerPlayerState.InPlanning,
                        ServerFlowType.StartPlanning);

                    break;

                case PlayerFlowPacket.FlowType.FinishedSimulation:
                    TryFlow(packet, player, ServerPlayerState.InPlanning, ServerPlayerState.InSimulation,
                        ServerFlowType.StartSimulation);

                    break;

                default:
                    _logger.LogError("Received invalid player-flow packet: " + packet);
                    break;
            }
        }

        internal void TryFlow(PlayerFlowPacket packet, ServerPlayer player, ServerPlayerState originState,
            ServerPlayerState targetState, ServerFlowType serverFlow)
        {
            if (!AllPlayersInState(originState, false))
            {
                _logger.LogWarning($"Player sent disallowed flow-packet: {packet}\nPlayer: {player}");
                return;
            }

            player.Finished = true;
            BroadcastWithoutSecret(packet);

            if (!AllPlayersInState(originState, true)) return;

            Players.ForEach(p => p.MoveToState(targetState));
            GameServer.Broadcast(new ServerFlowPacket { Type = serverFlow });
        }
    }
}