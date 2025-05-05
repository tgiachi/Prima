using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Prima.Tcp.Test;

public class TcpServer
{
    // Delegati per gestire gli eventi
    public delegate void ConnectionHandler(string id);

    public delegate void DisconnectionHandler(string id);

    public delegate void ReceiveHandler(string id, byte[] buffer);

    // Eventi
    public event ConnectionHandler OnConnection;
    public event ReceiveHandler OnReceive;
    public event DisconnectionHandler OnDisconnection;


    // Dizionario per mantenere le connessioni attive
    private readonly ConcurrentDictionary<string, Socket> _connections = new();

    // Token per la cancellazione
    private CancellationTokenSource _cts;

    // Socket per l'ascolto
    private Socket _listener;

    // Porta e indirizzo di ascolto
    private IPEndPoint _localEndPoint;

    public TcpServer(string ipAddress, int port)
    {
        var ip = string.IsNullOrEmpty(ipAddress) ? IPAddress.Any : IPAddress.Parse(ipAddress);
        _localEndPoint = new IPEndPoint(ip, port);
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();

        // Crea il socket di ascolto
        _listener = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        {
            LingerState = new LingerOption(false, 0),
            ExclusiveAddressUse = true,
            NoDelay = true,
            Blocking = false,
            SendBufferSize = 64 * 1024,
            ReceiveBufferSize = 64 * 1024
        };

        try
        {
            _listener.Bind(_localEndPoint);
            _listener.Listen(256);

            Console.WriteLine($"Server avviato su {_localEndPoint}");

            // Inizia ad accettare connessioni
            _ = BeginAcceptingSockets(_listener, _cts.Token);
        }
        catch (SocketException se)
        {
            Console.WriteLine($"Errore durante l'avvio del server: {se.Message}");
        }
    }

    public void Stop()
    {
        _cts?.Cancel();

        foreach (var connection in _connections.Values)
        {
            CloseSocket(connection);
        }

        _connections.Clear();
        _listener?.Close();

        Console.WriteLine("Server arrestato");
    }

    // Metodo per inviare dati a un client specifico
    public bool Send(string id, byte[] buffer)
    {
        if (_connections.TryGetValue(id, out var socket) && socket.Connected)
        {
            try
            {
                socket.Send(buffer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'invio al client {id}: {ex.Message}");
                CloseSocket(socket);
                OnDisconnection?.Invoke(id);
                _connections.TryRemove(id, out _);
            }
        }

        return false;
    }

    private async Task BeginAcceptingSockets(Socket listener, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var socket = await listener.AcceptAsync(token);

                // Genera un nuovo ID per la connessione
                string connectionId = Guid.NewGuid().ToString();

                // Memorizza la connessione
                _connections[connectionId] = socket;

                // Solleva l'evento di connessione
                OnConnection?.Invoke(connectionId);

                // Gestisci la connessione in un task separato
                _ = HandleConnection(connectionId, socket, token);
            }
            catch (OperationCanceledException)
            {
                // Ignora l'eccezione se il token è stato cancellato
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'accettazione di una connessione: {ex.Message}");
            }
        }
    }

    private async Task HandleConnection(string id, Socket socket, CancellationToken token)
    {
        byte[] buffer = new byte[8192]; // Buffer per la lettura dei dati

        try
        {
            while (!token.IsCancellationRequested && socket.Connected)
            {
                // Leggi i dati dal socket
                int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None, token);

                if (bytesRead == 0)
                {
                    // La connessione è stata chiusa dal client
                    break;
                }

                // Crea una copia dei dati ricevuti
                byte[] receivedData = new byte[bytesRead];
                Array.Copy(buffer, receivedData, bytesRead);

                // Solleva l'evento di ricezione
                OnReceive?.Invoke(id, receivedData);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignora l'eccezione se il token è stato cancellato
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante la gestione della connessione {id}: {ex.Message}");
        }
        finally
        {
            // Chiudi il socket e rimuovi la connessione
            CloseSocket(socket);
            _connections.TryRemove(id, out _);
            OnDisconnection?.Invoke(id);
        }
    }

    private static void CloseSocket(Socket socket)
    {
        try
        {
            socket.Shutdown(SocketShutdown.Both);

        }
        catch
        {
            // Ignora eventuali errori durante la chiusura
        }
        finally
        {
            socket.Close();
        }
    }
}
