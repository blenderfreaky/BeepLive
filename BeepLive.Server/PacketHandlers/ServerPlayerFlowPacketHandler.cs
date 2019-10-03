namespace BeepLive.Server
{
    using BeepLive.Network;
    using Microsoft.Extensions.Logging;
    using System;

    internal static partial class PacketHandlers
    {
        internal static void ProcessPacket(PacketContext<PlayerFlowPacket> packetContext)
        {
            packetContext.Logger.LogDebug("Received: " + packetContext.Packet);

            if (packetContext.Packet.Type != PlayerFlowPacket.FlowType.Join && !packetContext.Server.IsValid(packetContext.Packet))
            {
                packetContext.Logger.LogWarning($"Received packet with invalid Secret: {packetContext.Packet}\nSent by: {packetContext.Sender}");
                return;
            }

            ServerPlayer player = packetContext.Server.Players
                .Find(p => p.PlayerGuid
                    == packetContext.Packet.PlayerGuid);

            if (player == null && packetContext.Packet.Type != PlayerFlowPacket.FlowType.Join)
            {
                packetContext.Logger.LogWarning(
                    $"Unknown player sent flow-packet: {packetContext.Packet}\nSent by: {packetContext.Sender}");
                return;
            }

            switch (packetContext.Packet.Type)
            {
                case PlayerFlowPacket.FlowType.Join:
                    if (!packetContext.Server.AllPlayersInState(ServerPlayerState.InTeamSelection, false))
                    {
                        packetContext.Logger.LogWarning($"Player joined late: {packetContext.Packet}\nPlayer: {player}");
                        return;
                    }

                    if (player == null)
                    {
                        packetContext.Server.Players.Add(new ServerPlayer
                        {
                            PlayerGuid = packetContext.Packet.PlayerGuid,
                            Secret = packetContext.Packet.Secret,
                            State = ServerPlayerState.InTeamSelection
                        });
                    }

                    packetContext.Sender.SendToStream(new SyncPacket(packetContext.Server.BeepConfig));
                    packetContext.Sender.SendToStream(new ServerFlowPacket { Type = ServerFlowType.StartTeamSelection });

                    break;

                case PlayerFlowPacket.FlowType.Leave:
                    packetContext.Server.Players.Remove(player);
                    packetContext.Server.BroadcastWithoutSecret(packetContext.Packet);

                    break;

                case PlayerFlowPacket.FlowType.LockInTeam:
                    TryFlow(packetContext, player, ServerPlayerState.InTeamSelection, ServerPlayerState.InSpawning,
                        ServerFlowType.StartSpawning);

                    break;

                case PlayerFlowPacket.FlowType.Spawn:
                    TryFlow(packetContext, player, ServerPlayerState.InSpawning, ServerPlayerState.InPlanning,
                        ServerFlowType.StartSimulation);

                    break;

                case PlayerFlowPacket.FlowType.ReadyForSimulation:
                    TryFlow(packetContext, player, ServerPlayerState.InPlanning, ServerPlayerState.InSimulation,
                        ServerFlowType.StartSimulation);

                    break;

                case PlayerFlowPacket.FlowType.FinishedSimulation:
                    TryFlow(packetContext, player, ServerPlayerState.InSimulation, ServerPlayerState.InPlanning,
                        ServerFlowType.StartPlanning);

                    break;

                default:
                    packetContext.Logger.LogError("Received invalid player-flow packet: " + packetContext.Packet);
                    break;
            }
        }

        private static void TryFlow(PacketContext<PlayerFlowPacket> packetContext, ServerPlayer player, ServerPlayerState originState,
            ServerPlayerState targetState, ServerFlowType serverFlow)
        {
            if (!packetContext.Server.AllPlayersInState(originState, false))
            {
                packetContext.Logger.LogWarning($"Player sent disallowed flow-packet: {packetContext.Packet}\nPlayer: {player}");
                return;
            }

            player.Finished = true;
            packetContext.Server.BroadcastWithoutSecret(packetContext.Packet);

            if (!packetContext.Server.AllPlayersInState(originState, true)) return;

            packetContext.Server.Players.ForEach(p => p.MoveToState(targetState));
            packetContext.Server.GameServer.Broadcast(new ServerFlowPacket { Type = serverFlow });
        }
    }
}