using System.IO;
using BeepLive.Config;
using BeepLive.World;
using CommandLine;
using SFML.Graphics;
using SFML.System;

namespace BeepLive
{
    internal class Program
    {
        [Verb("start-client", HelpText = "Starts the client of BeepLive", Hidden = false)]
        public class ClientOptions
        {
            [Option('c', "config", Default = "BeepConfig.xml", Required = false, HelpText = "Path to the config file to use")]
            public string ConfigPath { get; set; }
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ClientOptions>(args)
                .WithParsed(o =>
                {
                    if (!File.Exists(o.ConfigPath)) File.WriteAllText(o.ConfigPath, XmlHelper.ToXml(new MapConfig()));

                    Game.BeepLive beepLive = new Game.BeepLive()
                            .AddMap(map => map
                                .LoadConfig(o.ConfigPath)
                                .GenerateMap())
                            .AddTeam(team => team)
                        ;

                    File.WriteAllText(o.ConfigPath, XmlHelper.ToXml(beepLive.Map.Config));

                    beepLive.Run();
                });
        }
    }
}