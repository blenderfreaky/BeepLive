namespace BeepLive.Server
{
    using System.Threading.Tasks;

    public static class ProgramServer
    {
        private static void Main()
        {
            _ = new BeepServer();

            Task.Delay(-1);
        }
    }
}