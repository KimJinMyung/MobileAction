using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

public struct MessageData
{
    public TcpClient messageClient;
    public NetworkStream stream;
    public string messageString;
}

namespace TCP_Server
{
    public class MessageQueue
    {
        public event Action<TcpClient, NetworkStream, string> OnMessageReceived; // �̺�Ʈ ����

        private ConcurrentQueue<MessageData> queue = new ConcurrentQueue<MessageData>();
        private bool isProcessing = false;

        // �޽����� ť�� �߰�
        public void EnqueueMessage(TcpClient client, NetworkStream stream, string message)
        {
            queue.Enqueue(new MessageData { messageClient = client, stream = stream, messageString = message }); ;
            ProcessQueue();
        }

        // �޽��� ó�� �޼���
        private async void ProcessQueue()
        {
            if (isProcessing)
                return;

            isProcessing = true;

            while (queue.TryDequeue(out MessageData messageData))
            {
                // ���⿡�� �� �޽����� ó��
                await HandleMessage(messageData);
            }

            isProcessing = false;
        }

        // ���� �޽��� ó�� ����
        private async Task HandleMessage(MessageData messageData)
        {
            OnMessageReceived?.Invoke(messageData.messageClient, messageData.stream, messageData.messageString);

            //return Task.CompletedTask;
        }
    }
}
