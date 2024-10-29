using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

struct ClientData
{
    public string PlayerId;
    public string PlayerIP;
    public string ConnectServerIp;
    public bool isConnected;
}

class TCPClientList
{
    public Dictionary<TcpClient, ClientData> connectedClients { get; private set; } = new Dictionary<TcpClient, ClientData>();
    public void AddConnectClient(TcpClient client, string playerId, string playerIp)
    {
        if (connectedClients.ContainsKey(client))
        {
            connectedClients[client] = new ClientData
            {
                PlayerId = playerId,
                PlayerIP = playerIp,
                ConnectServerIp = string.Empty,
                isConnected = true
            };
        }
        else
        {
            connectedClients.Add(client, new ClientData
            {
                PlayerId = playerId,
                PlayerIP = playerIp,
                ConnectServerIp = string.Empty,
                isConnected = true
            });
        }

        Console.WriteLine("Connect Client Complete");
    }

    public void EnterGameRoomPlayer(TcpClient client, string serverIP)
    {
        if(connectedClients.ContainsKey(client))
        {
            connectedClients[client] = new ClientData
            {
                PlayerId = connectedClients[client].PlayerId,
                PlayerIP = connectedClients[client].PlayerIP,
                ConnectServerIp = serverIP,
                isConnected = true
            };
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
                isConnected = false
            };
        }
    }

    public void UpdateClientState(TcpClient client, bool isConnected)
    {
        if (connectedClients.ContainsKey(client))
        {
            var clientData = connectedClients[client];
            clientData.isConnected = isConnected;
            connectedClients[client] = clientData;

            Console.WriteLine("PONG 신호 받음");
        }
    }

    public void RemoveConnectClient(TcpClient client) 
    {
        if (connectedClients.ContainsKey(client))
        {
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