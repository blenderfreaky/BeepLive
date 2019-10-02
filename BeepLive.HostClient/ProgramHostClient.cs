namespace BeepLive.HostClient
{
    using BeepLive.Client;
    using BeepLive.Server;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ProgramHostClient
    {
        public static void Main()
        {
            var server = new BeepServer();
            Thread.Sleep(1000);
            var client = new BeepClient();
        }
    }
}
