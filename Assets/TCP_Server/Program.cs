using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

namespace TCP_Server
{
    internal class TCPServer
    {
        private TcpListener server;
        private GameRoomDBManager dBManager;

        // TCP 서버 시작
        public void StartServer(string ipAddress, int port)
        {
            try
            {
                // DB 매니저 초기화 및 연결
                dBManager = new GameRoomDBManager();
                dBManager.OpenConnection();

                // IP 주소와 포트로 서버 시작
                server = new TcpListener(IPAddress.Parse(ipAddress), port);
                server.Start();
                Console.WriteLine($"Server started on {ipAddress} : {port}");

                while (true)
                {
                    // 클라이언트 연결 수락
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected");

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
        private void HandleClient(TcpClient client)
        {
            try
            {
                // 네트워크 스트림 생성
                NetworkStream stream = client.GetStream();

                // 클라이언트로부터 데이터 수신
                byte[] buffer = new byte[1024];
                int byteRead = stream.Read(buffer, 0, buffer.Length);
                string clientData = Encoding.UTF8.GetString(buffer, 0, byteRead);
                Console.WriteLine($"Received from client : {clientData}");

                // 클라이언트 요청 처리
                string[] requestParts = clientData.Split(',');

                // 요청 타입에 따라 처리
                switch (requestParts[0])
                {
                    case "createRoom":
                        string serverId = requestParts[1];
                        string serverIp = requestParts[2];
                        string password = requestParts[3];

                        Console.WriteLine("Request Create Room");

                        dBManager.InsertRoom(serverId, serverIp, password);
                        SendResponse(stream, "Room created success");
                        break;
                    case "removeRoom":
                        string serverid = requestParts[1];
                        dBManager.RemoveRoom(serverid);
                        SendResponse(stream, "Room removed success");
                        break;
                    case "getRoomList":
                    string roomList = dBManager.GetRoomList();
                        SendResponse(stream, roomList);
                        break;
                    case "enterRoom":
                        string roomid = requestParts[1];
                        string roomPassword = requestParts[2];
                        string serverIP = dBManager.EnterSelectRoom(roomid, roomPassword);
                        if (!string.IsNullOrEmpty(serverIP))
                        {
                            SendResponse(stream, serverIP);
                        }
                        else
                        {
                            SendResponse(stream, "Invalid room ID or password");
                        }
                        break;
                    default:
                        SendResponse(stream, "Invalid request");
                        break;
                }

                // 연결 종료
                stream.Close();
                client.Close();
                Console.WriteLine("Client disConnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error2" + e.Message);
            }
        }

        // 클라이언트에게 응답 보내기
        private void SendResponse(NetworkStream stream, string message)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        // 서버 종료
        public void StopServer()
        {
            server.Stop();
            Console.WriteLine("Server Stopped");
        }

        static void Main(string[] args)
        {
            // 서버 실행
            TCPServer tcpServer = new TCPServer();
            tcpServer.StartServer("127.0.0.1", 5000);
        }
    }

}
