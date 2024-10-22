using System.Net.Sockets;
using System;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

public class MyTCPClient : Singleton<MyTCPClient>
{
    private TcpClient client;
    private NetworkStream stream;

    protected override void Awake()
    {
        base.Awake();

        ConnectToServer("127.0.0.1", 5000);
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
    private void OnApplicationQuit()
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
    public async Task<string> ReceiveMessageFromServer()
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
                string receivedMessage = Encoding.UTF8.GetString(buffer);
                Debug.LogError("Received from server : " + receivedMessage);
                return receivedMessage;
            }

            return string.Empty;

        }
        catch(Exception e)
        {
            Debug.LogError("Error receive message : " + e.Message);
            return string.Empty;
        }
    }
}   
