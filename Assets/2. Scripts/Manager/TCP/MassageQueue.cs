using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class MessageQueue
{
    private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    public event Action<string> OnMessageReceived; // �޽��� ���� �̺�Ʈ
    private bool isProcessing = false;

    // �޽����� ť�� �߰�
    public void EnqueueMessage(string message)
    {
        queue.Enqueue(message);
        OnMessageReceived?.Invoke(message);
        //ProcessQueue();
    }

    
}
