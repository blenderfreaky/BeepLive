namespace BeepLive.Client
{
    public class ProgramClient
    {
        private static void Main()
        {
            BeepClient.BeepClientInstance = new BeepClient();
            BeepClient.BeepClientInstance.Start();
        }
    }
}