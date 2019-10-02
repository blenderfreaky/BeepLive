namespace BeepLive.HostClient
{
    using BeepLive.Client;
    using BeepLive.Server;

    public static class ProgramHostClient
    {
        public static void Main()
        {
            var server = new BeepServer();

            var client = new BeepClient();
        }
    }
}
