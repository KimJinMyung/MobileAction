using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class MessageQueue
{
    private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    public event Action<string> OnMessageReceived; // 메시지 수신 이벤트
    private bool isProcessing = false;

    // 메시지를 큐에 추가
    public void EnqueueMessage(string message)
    {
        queue.Enqueue(message);
        OnMessageReceived?.Invoke(message);
        //ProcessQueue();
    }

    
}
