﻿using System.Net.Sockets;
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
                // TCPListener([접근이 가능한 IP], [포트])
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine($"Server started on {ipAddress} : {port}");

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

                        // 클라이언트 요청 처리
                        string[] requestParts = clientData.Split(',');

                        switch (requestParts[0])
                        {
                            case "createRoom":
                                IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                                string serverId = requestParts[1];
                                string serverIp = clientEndPoint.Address.ToString();
                                string password = requestParts[3];
                                string roomName = requestParts[4];
                                string currentCount = requestParts[5];
                                string maxCount = requestParts[6];

                                Console.WriteLine($"Room Name : {roomName}, currentCount : {currentCount}, maxCound : {maxCount}");

                                dBManager.InsertRoom(serverId, serverIp, password, roomName, currentCount, maxCount);
                                SendResponse(stream, "Room created success");
                                break;

                            case "removeRoom":
                                string serverid = requestParts[1];
                                string serverip = requestParts[2];
                                string Success = dBManager.RemoveRoom(serverid, serverip).ToString();
                                SendResponse(stream, Success);
                                break;

                            case "getRoomList":
                                string roomList = dBManager.GetRoomList();
                                SendResponse(stream, roomList);
                                break;

                            case "enterRoom":
                                string roomip = requestParts[1];
                                string roomid = requestParts[2];
                                string roomPassword = requestParts[3];

                                var response = dBManager.EnterRoom(roomip, roomid, roomPassword);

                                if (!string.IsNullOrEmpty(response))
                                {
                                    SendResponse(stream, response);
                                }
                                else
                                {
                                    SendResponse(stream, "Invalid room ID or password");
                                }
                                break;
                            case "enterSelectRoom":
                                string roomJoinCode = requestParts[1];

                                string roomData = dBManager.EnterSelectRoom(roomJoinCode);
                                SendResponse(stream, roomData);
                                break;

                            case "ChangedPlayerCount":
                                string ip = requestParts[1];
                                string id = requestParts[2];
                                string stringChangedType = requestParts[3];
                                int intChangedType = int.Parse(stringChangedType);

                                dBManager.ChangedPlayerCount(ip, id, intChangedType);
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
                    else
                    {
                        // 클라이언트가 연결을 끊으면 루프 종료
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
