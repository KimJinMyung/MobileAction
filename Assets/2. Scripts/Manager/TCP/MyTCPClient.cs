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
        // ���� �������� SynchronizationContext�� ����
        mainThreadContext = SynchronizationContext.Current;

        // OnMessageReceived �̺�Ʈ�� Unity �۾� ����
        messageQueue.OnMessageReceived += ProcessServerMessage;

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.requestRoomList);

        _ = ReceivePingPongContinuously();
    }

    // ���� ����
    public void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            // ������ ����
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

    // ���� ���� ����
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

    // ������ ��û ����
    public void SendRequestToServer(string request)
    {
        try
        {
            if (stream == null)
            {
                Debug.LogError("Stream is not initialized");
                return;
            }

            // ���ڿ��� ����Ʈ �迭�� ��ȯ
            byte[] data = Encoding.UTF8.GetBytes(request);

            // �����͸� ������ ����
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

    // �����κ��� �޽��� ����(�񵿱� ó��)
    public async Task StartReceivingMessageAsync()
    {
        try
        {
            if (stream == null)
            {
                Debug.LogError("Stream is not initialized");
            }

            byte[] buffer = new byte[1024]; // ���� ����

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // �񵿱� ������ ������ �б�

                if (bytesRead > 0)
                {
                    // ����Ʈ �迭�� ���ڿ��� ��ȯ
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // ������ ��ȯ
                    messageQueue.EnqueueMessage(receivedMessage);

                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receive message : " + e.Message);
        }
    }

    // �����κ����� �޽��� ó��
    private void ProcessServerMessage(string message)
    {
        // Ping ��ȣ ó��
        if (message.StartsWith("CMD:PING"))
        {
            SendRequestToServer($"{Tcp_Room_Command.PONG}");
            return;
        }

        // �� ���� ���� ó��
        if (message.StartsWith("RoomList"))
        {
            // ���� �����忡�� ���� ó��
            mainThreadContext.Post(_ =>
            {
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
                    Debug.LogError("������ ã�� �� �����ϴ�.");
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

        // ��Ÿ ��ɾ� ó��
    }

}   
