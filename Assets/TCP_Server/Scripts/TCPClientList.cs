using System;
using System.Collections.Generic;
using System.Net.Sockets;

struct ClientData
{
    public string PlayerId;
    public string PlayerIP;
    public string ConnectServerIp;
    public string ConnectServerPort;
    public bool isConnected;
}

class TCPClientList
{
    public Dictionary<TcpClient, ClientData> connectedClients { get; private set; } = new Dictionary<TcpClient, ClientData>();
    public event Action<string, string, int, string> OnDecreasePlayerCount;

    private readonly object clientLock = new object();

    public void AddConnectClient(TcpClient client, string playerId, string playerIp)
    {
        Console.WriteLine($"개수 : {connectedClients.Count}");

        if (connectedClients.ContainsKey(client))
        {
            connectedClients[client] = new ClientData
            {
                PlayerId = playerId,
                PlayerIP = playerIp,
                //ConnectServerIp = string.Empty,
                //ConnectServerPort = string.Empty,
                isConnected = true
            };
        }
        else
        {
            connectedClients.Add(client, new ClientData
            {
                PlayerId = playerId,
                PlayerIP = playerIp,
                //ConnectServerIp = string.Empty,
                //ConnectServerPort=string.Empty,
                isConnected = true
            });
        }

        Console.WriteLine("Connect Client Complete");
    }

    public void EnterGameRoomPlayer(TcpClient client, string serverIP, string serverPort)
    {
        lock(clientLock)
        {
            if (connectedClients.ContainsKey(client))
            {
                connectedClients[client] = new ClientData
                {
                    PlayerId = connectedClients[client].PlayerId,
                    PlayerIP = connectedClients[client].PlayerIP,
                    ConnectServerIp = serverIP,
                    ConnectServerPort = serverPort,
                    isConnected = true
                };

                Console.WriteLine($"디버깅 0 : {connectedClients[client].ConnectServerIp}");
            }
        }
    }

    public void QuitGameRoomPlayer(TcpClient client)
    {
        if (connectedClients.ContainsKey(client))
        {
            connectedClients[client] = new ClientData
            {
                PlayerId = connectedClients[client].PlayerId,
                PlayerIP = connectedClients[client].PlayerIP,
                ConnectServerIp = string.Empty,
                ConnectServerPort = string.Empty,
                isConnected = false
            };
        }
    }

    public void UpdateClientState(TcpClient client, bool isConnected)
    {
        lock (clientLock)
        {
            if (connectedClients.ContainsKey(client))
            {
                var clientData = connectedClients[client];
                clientData.isConnected = isConnected;
                connectedClients[client] = clientData;
            }
        }
    }

    public void RemoveConnectClient(TcpClient client) 
    {
        if (connectedClients.ContainsKey(client))
        {
            // 접속하고 있던 서버가 있었다면
            if (!string.IsNullOrEmpty(connectedClients[client].ConnectServerIp))
            {
                string serverIp = connectedClients[client].ConnectServerIp;
                string serverPort = connectedClients[client].ConnectServerPort;
                int changedType = -1;
                string playerId = connectedClients[client].PlayerId;

                // 접속하고 있는 currentPlayerCount 감소시킴
                OnDecreasePlayerCount?.Invoke(serverIp, serverPort, changedType, playerId);
            }

            connectedClients.Remove(client);
            client.Close();
            Console.WriteLine("Client removed.");
        }
    }

    public ClientData? GetClientData(TcpClient client)
    {
        if(connectedClients.TryGetValue(client, out var clientData))
        {
            return clientData;
        }

        return null;
    }
}