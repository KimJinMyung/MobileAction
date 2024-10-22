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

    // 서버 연결 해제
    private void OnApplicationQuit()
    {
        if(stream != null) stream.Close();
        if(client != null) client.Close();
        Debug.Log("Disconnected from server");
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

    // 서버로부터 메시지 수신 (비동기 처리)
    public async Task<string> ReceiveMessageFromServer()
    {
        try
        {
            if(stream == null)
            {
                Debug.LogError("Stream is not initialized");
                return string.Empty;
            }

            byte[] buffer = new byte[1024]; // 수신 버퍼
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // 비동기 적으로 데이터 읽기

            if(bytesRead > 0)
            {
                // 바이트 배열을 문자열로 변환
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
