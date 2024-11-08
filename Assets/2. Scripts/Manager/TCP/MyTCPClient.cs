using System.Net.Sockets;
using System;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using TCP_Enum;
using kcp2k;
using Mirror;
using System.Collections.Concurrent;
using System.Threading;

public class MyTCPClient : Singleton<MyTCPClient>
{
    private TcpClient client;
    private NetworkStream stream;
    public MessageQueue messageQueue { get; private set; } = new MessageQueue();

    private SynchronizationContext mainThreadContext;

    protected override void Awake()
    {
        base.Awake();

        ConnectToServer("127.0.0.1", 5000);
    }

    private void OnEnable()
    {
        EventManager<Tcp_Room_Command>.Binding(true, Tcp_Room_Command.CloseServer, CloseServer);
    }

    private void OnDisable()
    {
        EventManager<Tcp_Room_Command>.Binding(false, Tcp_Room_Command.CloseServer, CloseServer);
    }

    private void Start()
    {
        // 메인 스레드의 SynchronizationContext를 저장
        mainThreadContext = SynchronizationContext.Current;

        // OnMessageReceived 이벤트에 Unity 작업 연결
        messageQueue.OnMessageReceived += ProcessServerMessage;

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.requestRoomList);

        _ = ReceivePingPongContinuously();
    }

    // 서버 연결
    public void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            // 서버에 연결
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("Connected to server");
        }
        catch(Exception e)
        {
            Debug.LogError("Error connecting to server : " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        CloseServer();
    }

    // 서버 연결 해제
    private void CloseServer()
    {
        try
        {
            if (stream != null) stream.Close();
            if (client != null) client.Close();
            Debug.Log("Disconnected from server");
        }
        catch(Exception e)
        {
            Debug.LogError("Error disconnecting from server: " + e.Message);
        }
    }

    // 서버로 요청 전송
    public void SendRequestToServer(string request)
    {
        try
        {
            if (stream == null)
            {
                Debug.LogError("Stream is not initialized");
                return;
            }

            // 문자열을 바이트 배열로 변환
            byte[] data = Encoding.UTF8.GetBytes(request);

            // 데이터를 서버로 전송
            stream.Write(data, 0, data.Length);
            Debug.Log("Send to server :" + request);

        }catch(Exception e)
        {
            Debug.LogError("Error sending message : " + e.Message);
        }
    }

    private async Task ReceivePingPongContinuously()
    {
        while (client != null && client.Connected)
        {
            await StartReceivingMessageAsync();
            //if (!string.IsNullOrEmpty(message))
            //{
            //    Debug.Log("Received message: " + message);
            //}
        }
    }

    // 서버로부터 메시지 수신(비동기 처리)
    public async Task StartReceivingMessageAsync()
    {
        try
        {
            if (stream == null)
            {
                Debug.LogError("Stream is not initialized");
            }

            byte[] buffer = new byte[1024]; // 수신 버퍼

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // 비동기 적으로 데이터 읽기

                if (bytesRead > 0)
                {
                    // 바이트 배열을 문자열로 변환
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // 데이터 반환
                    messageQueue.EnqueueMessage(receivedMessage);

                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receive message : " + e.Message);
        }
    }

    // 서버로부터의 메시지 처리
    private void ProcessServerMessage(string message)
    {
        // Ping 신호 처리
        if (message.StartsWith("CMD:PING"))
        {
            SendRequestToServer($"{Tcp_Room_Command.PONG}");
            return;
        }

        // 방 생성 응답 처리
        if (message.StartsWith("RoomList"))
        {
            // 메인 스레드에서 응답 처리
            mainThreadContext.Post(_ =>
            {
                Debug.Log("방 생성 응답" + message);
                EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.UpdateRoomList, message);
            }, null);
        }else if (message.StartsWith(nameof(Tcp_Room_Command.createRoom)))
        {
            string[] messageData = message.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = messageData[1];
            if (bool.TryParse(result, out bool success) && success)
            {
                mainThreadContext.Post(_ =>
                {
                    //EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.connect);
                    EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.StartHost, true);
                }, null);
            }
               
        }else if (message.StartsWith(nameof(Tcp_Room_Command.removeRoom)))
        {
            mainThreadContext.Post(_ =>
            {
                EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.StartHost, false);
            }, null);
        }else if (message.StartsWith(nameof(Tcp_Room_Command.enterSelectRoom)))
        {
            mainThreadContext.Post(_ =>
            {
                if (string.IsNullOrEmpty(message))
                {
                    Debug.LogError("서버를 찾을 수 없습니다.");
                    return;
                }

                string[] data = message.Split(',', StringSplitOptions.RemoveEmptyEntries);

                string roomIP = data[1];
                int roomPort = int.Parse(data[2]);

                EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.EnterGameRoomClient, roomIP, roomPort);

            }, null);
        }else if (message.StartsWith(nameof(Tcp_Room_Command.enterRoom)))
        {
            mainThreadContext.Post(_ =>
            {
                string[] data = message.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (bool.TryParse(data[1], out bool isSuccess) && isSuccess)
                {
                    string roomIP = data[2];
                    string roomPort = data[3];

                    EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.EnterGameRoomClient, roomIP, roomPort);
                }               
            }, null);
        }

        // 기타 명령어 처리
    }

}   
