namespace BeepLive.Client
{
    public static class ProgramClient
    {
        private static void Main()
        {
            var beepClient = new BeepClient();
            beepClient.Start();
        }
    }
}