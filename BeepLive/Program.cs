using System.IO;
using BeepLive.Config;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Game.BeepLive beepLive = new Game.BeepLive()
                    .AddMap(map => map
                        .LoadConfig("BeepConfig.xml")
                        .GenerateMap())
                    .AddTeam(team => team)
                ;

            File.WriteAllText("BeepConfig.xml", XMLHelper.ToXML(beepLive.Map.Config));
            beepLive.Run();
        }
    }
}