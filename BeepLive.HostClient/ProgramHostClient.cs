namespace BeepLive.HostClient
{
    using BeepLive.Client;
    using BeepLive.Server;
    using System.Threading;

    public static class ProgramHostClient
    {
        public static void Main()
        {
            _ = new BeepServer();

            _ = new BeepClient();
        }
    }
}