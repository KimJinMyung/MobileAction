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
    private MessageQueue messageQueue = new MessageQueue();

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
                    //Debug.LogError("Received from server : " + receivedMessage);

                    // ������ ��ȯ
                    messageQueue.EnqueueMessage(receivedMessage);

                    //if (receivedMessage == "PING")
                    //{
                    //    Debug.Log("Ping ��ȣ ����");
                    //    // ������ Pong �޽��� ����
                    //    await Task.Delay(50);
                    //    SendRequestToServer($"{Tcp_Room_Command.PONG}");
                    //}
                    //else
                    //{
                    //    // ������ ��ȯ
                    //    messageQueue.EnqueueMessage(receivedMessage);
                    //}


                    // Game Room Lobby ���� ������ ����
                    //EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.UpdateRoomList, receivedMessage);
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
        if (message.StartsWith("CMD:CREATE_ROOM_RESULT"))
        {
            // ���� �����忡�� ���� ó��
            mainThreadContext.Post(_ =>
            {
                // ���⿡�� Unity ���� ���� ó��
                Debug.Log("�� ���� ���� ó��");
            }, null);
        }

        // ��Ÿ ��ɾ� ó��
    }

}   
