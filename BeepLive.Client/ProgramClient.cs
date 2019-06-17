using CommandLine.Text;
using System;
using System.IO;
using System.Net;
using BeepLive.Config;
using CommandLine;
using BeepLive.Game;

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
