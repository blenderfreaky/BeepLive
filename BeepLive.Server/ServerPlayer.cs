namespace BeepLive.Server
{
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