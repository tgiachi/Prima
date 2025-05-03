using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging.Abstractions;
using Prima.Network.Packets;
using Prima.Network.Packets.Entries;
using Prima.Network.Serializers;
using Prima.Network.Services;
using SuperSimpleTcp;

namespace Prima.Tcp.Test;

class Program
{
    static void Main(string[] args)
    {
        var packetManager = new PacketManager(new NullLogger<PacketManager>());

        packetManager.RegisterPacket<ClientVersion>();
        packetManager.RegisterPacket<LoginRequest>();
        packetManager.RegisterPacket<ConnectToGameServer>();
        packetManager.RegisterPacket<SelectServer>();
        packetManager.RegisterPacket<GameServerList>();
        packetManager.RegisterPacket<LoginDenied>();

        var server = new SimpleTcpServer("127.0.0.1:2593");
        Console.WriteLine("Server started");

        server.Events.ClientConnected += (sender, args) => { Console.WriteLine($"Client connected: {args.IpPort}"); };

        server.Events.ClientDisconnected += (sender, args) => { Console.WriteLine($"Client disconnected: {args.IpPort}"); };


        server.Events.DataReceived += (s, e) =>
        {
            var data = e.Data;

            var packets = packetManager.ReadPackets(data.Array);

            foreach (var packet in packets)
            {
                if (packet is LoginRequest loginRequest)
                {
                    var gameServerList = new GameServerList();

                    gameServerList.Servers.Add(
                        new GameServerEntry()
                        {
                            IP = IPAddress.Parse("127.0.0.1"),
                            LoadPercent = 0x0,
                            Name = "Prima test",
                            TimeZone = 0
                        }
                    );

                    server.Send(e.IpPort, packetManager.WritePacket(gameServerList));
                }

                if (packet is SelectServer selectServer)
                {
                    var array = new byte[] { 0x8c, (byte)127, 0x00, 0x00, 0x00, 0x01, 0x1e, 0xcf, 0x7f, 0x8f, 0xb5 };

                    // var connectToServer = new ConnectToGameServer()
                    // {
                    //     GameServerIP = IPAddress.Parse("127.0.0.1"),
                    //     GameServerPort = 2592,
                    //     SessionKey = GenerateSessionKey()
                    // };

                    //var output = packetManager.WritePacket(bytes);

                    server.Send(e.IpPort, array);
                }
            }
        };
        server.Start();

        Console.WriteLine("Server started. Press any key to exit...");
        Console.ReadKey();
        server.Stop();
    }

    public static uint GenerateSessionKey()
    {
        byte[] keyBytes = new byte[4];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }

        return BitConverter.ToUInt32(keyBytes, 0);
    }
}
