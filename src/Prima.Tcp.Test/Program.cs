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

        packetManager.RegisterPacket<ClientVersionRequest>();
        packetManager.RegisterPacket<LoginRequest>();
        packetManager.RegisterPacket<ConnectToGameServer>();
        packetManager.RegisterPacket<SelectServer>();
        packetManager.RegisterPacket<GameServerList>();
        packetManager.RegisterPacket<LoginDenied>();

        var server = new TcpServer("127.0.0.1", 2593);

        Console.WriteLine("Server started");

        server.OnConnection += id => { Console.WriteLine($"Client connected: {id}"); };

        server.OnDisconnection += id => { Console.WriteLine($"Client disconnected: {id}"); };


        server.OnReceive += async (id, buffer) =>
        {
            var data = buffer;

            var packets = packetManager.ReadPackets(data);

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
                            TimeZone = 2
                        }
                    );

                    byte[] bytes = new byte[]
                    {
                        0xa8, 0x00, 0x2e, 0x5d, 0x00, 0x01, 0x00, 0x00,
                        0x4d, 0x6f, 0x64, 0x65, 0x72, 0x6e, 0x55, 0x4f,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x02, 0x01, 0x00, 0x00, 0x7f
                    };

                    var input = packetManager.WritePacket(gameServerList);


                    //await server.SendAsync(e.IpPort, array);
                    //await server.SendAsync(e.IpPort, array);
                    server.Send(id, input);
                }
                //
                //
                // [0] = {byte} 140 0x8C
                // [1] = {byte} 127 0x7F
                // [2] = {byte} 0 0x0
                // [3] = {byte} 0 0x0
                // [4] = {byte} 1 0x1
                // [5] = {byte} 10 0xA
                // [6] = {byte} 33 0x21
                // [7] = {byte} 130 0x82
                // [8] = {byte} 89 0x59
                // [9] = {byte} 254 0xFE
                // [10] = {byte} 121 0x79

                if (packet is SelectServer selectServer)
                {
                    var array = new byte[] { 0x8c, 0x7F, 0x00, 0x00, 0x01, 0xA, 0x21, 0x43, 0x75, 0xEF, 0x25 };

                    var connectToServer = new ConnectToGameServer()
                    {
                        GameServerIP = IPAddress.Parse("127.0.0.1"),
                        GameServerPort = 2593,
                        SessionKey = 1131802405
                    };


                    var output = packetManager.WritePacket(connectToServer);

                    // foreach (var _ in Enumerable.Range(0, 3))
                    // {
                    //     server.Send(id, output);
                    //
                    //     await Task.Delay(1000);
                    // }


                    server.Send(id, array);
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
