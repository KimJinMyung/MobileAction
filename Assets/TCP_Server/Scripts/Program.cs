using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace TCP_Server
{
    internal class TCPServer
    {
        private TcpListener server;
        private GameRoomDBManager dBManager;
        private TCPClientList clientDictionary;
        private MessageQueue messageQueue;

        private readonly int pingInterval = 3000;

        public TCPServer()
        {
            messageQueue = new MessageQueue();
            messageQueue.OnMessageReceived += RequestProcessing;
        }

        ~TCPServer()
        {
            if(messageQueue != null)
                messageQueue.OnMessageReceived -= RequestProcessing;
        }

        // TCP 서버 시작
        public void StartServer(string ipAddress, int port)
        {
            try
            {
                // DB 매니저 초기화 및 연결
                dBManager = new GameRoomDBManager();
                clientDictionary = new TCPClientList();

                clientDictionary.OnDecreasePlayerCount += dBManager.ChangedPlayerCount;

                dBManager.OpenConnection();

                // IP 주소와 포트로 서버 시작
                // TCPListener([접근이 가능한 IP], [포트])
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine($"Server started on {ipAddress} : {port}");

                _ = SendPingAsync();

                while (true)
                {
                    // 클라이언트 연결 수락
                    TcpClient client = server.AcceptTcpClient();                    
                    Console.WriteLine("Client connected");
                    IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine($"{clientEndPoint.Address}");

                    // 클라이언트 요청 처리
                    HandleClient(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error1" + e.Message);
            }
        }

        // 클라이언트와의 통신 처리
        private async void HandleClient(TcpClient client)
        {
            try
            {
                // 네트워크 스트림 생성
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (client.Connected)
                {
                    // 클라이언트로부터 데이터 수신
                    int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (byteRead > 0)
                    {
                        string clientData = Encoding.UTF8.GetString(buffer, 0, byteRead);
                        Console.WriteLine($"Received from client : {clientData}");

                        messageQueue.EnqueueMessage(client, stream, clientData);
                    }
                    else
                    {
                        break;
                    }
                }

                // 연결 종료
                stream.Close();
                client.Close();
                Console.WriteLine("Client disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error2" + e.Message);
            }
        }

        private void RequestProcessing(TcpClient client, NetworkStream stream, string message)
        {
            string[] requestParts = message.Split(',', StringSplitOptions.RemoveEmptyEntries);

            IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;

            switch (requestParts[0])
            {
                case "connect":
                    Console.WriteLine("연결 시도");
                    Console.WriteLine($"시도 정보 : {requestParts[1]}, {requestParts[2]}");
                    clientDictionary.AddConnectClient(client, requestParts[1], requestParts[2]);

                    break;
                case "PONG":
                    clientDictionary.UpdateClientState(client, true);
                    break;
                case "createRoom":
                    string serverId = requestParts[1];
                    string serverIp = clientEndPoint.Address.ToString();
                    string password = requestParts[3];
                    string roomName = requestParts[4];
                    string currentCount = requestParts[5];
                    string maxCount = requestParts[6];
                    string playerId = requestParts[7];

                    Console.WriteLine($"Room Name : {roomName}, currentCount : {currentCount}, maxCound : {maxCount}");

                    clientDictionary.EnterGameRoomPlayer(client, serverIp, serverId);
                    var result = dBManager.CreateRoom(serverId, serverIp, password, roomName, currentCount, maxCount, playerId);
                    SendResponse(stream, $"createRoom,{result}");
                    break;

                case "removeRoom":
                    string serverid = requestParts[1];
                    string serverip = requestParts[2];
                    string Success = dBManager.RemoveRoom(serverid, serverip).ToString();
                    SendResponse(stream, $"removeRoom,{Success}");
                    break;

                case "getRoomList":
                    string roomList = dBManager.GetRoomList();
                    SendResponse(stream, roomList);
                    break;

                case "enterRoom":
                    string roomip = requestParts[1];
                    string roomid = requestParts[2];
                    string roomPassword = requestParts[3];
                    string playerid = requestParts[4];
                    string playerip = requestParts[5];

                    var response = dBManager.EnterRoom(roomip, roomid, roomPassword);

                    if (!string.IsNullOrEmpty(response))
                    {
                        SendResponse(stream, $"enterRoom,{response}");
                    }
                    else
                    {
                        SendResponse(stream, "enterRoom,false");
                    }
                    break;
                case "enterSelectRoom":
                    string roomJoinCode = requestParts[1];

                    string roomData = dBManager.EnterSelectRoom(roomJoinCode);
                    SendResponse(stream, $"enterSelectRoom,{roomData}");
                    break;

                case "ChangedPlayerCount":
                    string ip = requestParts[1];
                    string id = requestParts[2];
                    string stringChangedType = requestParts[3];
                    int intChangedType = int.Parse(stringChangedType);
                    string connectPlayerId = requestParts[4];

                    if (intChangedType > 0)
                    {
                        clientDictionary.EnterGameRoomPlayer(client, ip, id);
                    }
                    else
                    {
                        clientDictionary.QuitGameRoomPlayer(client);
                    }

                    dBManager.ChangedPlayerCount(ip, id, intChangedType, connectPlayerId);
                    SendResponse(stream, "Changed PlayerCount");
                    break;
                case "GetPlayerCount":
                    string server_id = requestParts[1];

                    string playerCount = dBManager.GetPlayerCount(server_id);
                    if (!string.IsNullOrEmpty(playerCount))
                    {
                        SendResponse(stream, playerCount);
                    }
                    else
                    {
                        SendResponse(stream, "Not Found");
                    }

                    break;
                default:
                    SendResponse(stream, "Invalid request");
                    break;
            }
        }


        // 클라이언트에게 응답 보내기
        private void SendResponse(NetworkStream stream, string message)
        {
            if (stream == null)
            {
                Console.WriteLine("SendResponse Error: NetworkStream is null");
                return;
            }

            try
            {
                byte[] responseBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("Response sent: " + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendResponse Error: " + ex.Message);
            }
        }

        // 서버 종료
        public void StopServer()
        {
            clientDictionary.OnDecreasePlayerCount -= dBManager.ChangedPlayerCount;

            server.Stop();
            Console.WriteLine("Server Stopped");
        }

        private async Task SendPingAsync()
        {
            while (true)
            {
                foreach (var client in clientDictionary.connectedClients.Keys.ToList())
                {

                    try
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] pingMessage = Encoding.UTF8.GetBytes("CMD:PING");
                        await stream.WriteAsync(pingMessage, 0, pingMessage.Length);
                        Console.WriteLine("PING 신호 보냄");

                        // 클라이언트의 응답을 기다리기 위해 일단 False로 설정
                        clientDictionary.UpdateClientState(client, false);
                    }
                    catch (Exception e)
                    {

                        clientDictionary.RemoveConnectClient(client);
                        Console.WriteLine("Client disconnected due to ping failure.");
                    }
                }

                await Task.Delay(pingInterval);

                // isConnect가 false 인 TcpClient 들만 확인한다.
                foreach (var client in clientDictionary.connectedClients.Where(c => !c.Value.isConnected).Select(c => c.Key))
                {
                    clientDictionary.RemoveConnectClient(client);
                    Console.WriteLine("Client disconnected due to missing pong response.");
                }
            }
        }

        public static async Task Main(string[] args)
        {
            // 서버 실행
            TCPServer tcpServer = new TCPServer();
            tcpServer.StartServer("127.0.0.1", 5000);            
        }

        public struct RoomData
        {
            public string serverIP;
            public string serverPort;
            public string password;
            public string roomName;
            public string currentCount;
            public string maxCount;
        }
    }

}
