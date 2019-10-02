namespace BeepLive.Server
{
    using System;

    public class ServerPlayer
    {
        public Guid PlayerGuid { get; set; }
        public Guid Secret { get; set; }

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