namespace BeepLive.Client
{
    public class ProgramClient
    {
        private static void Main(string[] args)
        {
            _ = new BeepClient().Client.Connect();
        }
    }
}