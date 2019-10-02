namespace BeepLive.Server
{
    using BeepLive.Client;

    public static class ProgramServer
    {
        private static void Main()
        {
            var server = new BeepServer();

            var beepClient = new BeepClient();
            beepClient.Start();
        }
    }
}