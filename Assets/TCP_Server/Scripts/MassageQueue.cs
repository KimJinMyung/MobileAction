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
        public event Action<TcpClient, NetworkStream, string> OnMessageReceived; // 이벤트 선언

        private ConcurrentQueue<MessageData> queue = new ConcurrentQueue<MessageData>();
        private bool isProcessing = false;

        // 메시지를 큐에 추가
        public void EnqueueMessage(TcpClient client, NetworkStream stream, string message)
        {
            queue.Enqueue(new MessageData { messageClient = client, stream = stream, messageString = message }); ;
            ProcessQueue();
        }

        // 메시지 처리 메서드
        private async void ProcessQueue()
        {
            if (isProcessing)
                return;

            isProcessing = true;

            while (queue.TryDequeue(out MessageData messageData))
            {
                // 여기에서 각 메시지를 처리
                await HandleMessage(messageData);
            }

            isProcessing = false;
        }

        // 실제 메시지 처리 로직
        private async Task HandleMessage(MessageData messageData)
        {
            OnMessageReceived?.Invoke(messageData.messageClient, messageData.stream, messageData.messageString);

            //return Task.CompletedTask;
        }
    }
}
