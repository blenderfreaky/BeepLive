using CommandLine;

namespace BeepLive.Server
{
    public class ProgramServer
    {
        private static void Main(string[] args)
        {
            new BeepServer().Start();
        }
    }
}