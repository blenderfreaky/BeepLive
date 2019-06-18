namespace BeepLive.Client
{
    public class ProgramClient
    {
        private static void Main(string[] args)
        {
            BeepClient.BeepClientInstance = new BeepClient();
            BeepClient.BeepClientInstance.Start();
        }
    }
}