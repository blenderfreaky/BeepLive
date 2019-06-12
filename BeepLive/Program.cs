using OpenTK;
using System;

namespace BeepLive
{
    class Program
    {
        static void Main(string[] args)
        {
            using BeepLive beepLive = new BeepLive(500, 500);
            beepLive.Run(1d/60);
        }
    }
}
