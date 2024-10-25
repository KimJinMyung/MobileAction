using System.Net.Sockets;
using System;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using TCP_Enum;
using kcp2k;
using Mirror;
using System.Collections.Specialized;

public class MyTCPClient : Singleton<MyTCPClient>
{
    private TcpClient client;
    private NetworkStream stream;

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
        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.requestRoomList);
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

    // ���� ���� ����
    private void CloseServer()
    {
        if(stream != null) stream.Close();
        if(client != null) client.Close();
        Debug.Log("Disconnected from server");
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

    // �����κ��� �޽��� ���� (�񵿱� ó��)
    public async Task<string> ReceiveRoomListFromServer(Tcp_Room_Command TcpEvent)
    {
        try
        {
            if(stream == null)
            {
                Debug.LogError("Stream is not initialized");
                return string.Empty;
            }

            byte[] buffer = new byte[1024]; // ���� ����
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // �񵿱� ������ ������ �б�

            if(bytesRead > 0)
            {
                // ����Ʈ �迭�� ���ڿ��� ��ȯ
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //Debug.LogError("Received from server : " + receivedMessage);

                // ������ ��ȯ
                return receivedMessage;

                // Game Room Lobby ���� ������ ����
                //EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.UpdateRoomList, receivedMessage);
            }
            else
            {
                return string.Empty;
            } 
        }
        catch(Exception e)
        {
            Debug.LogError("Error receive message : " + e.Message);
            return string.Empty;
        }
    }
}   
