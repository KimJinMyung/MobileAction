using kcp2k;
using Mirror;
using PlayerEventEnum;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Enum;
using UnityEngine;

public class MyGameRoomManager : NetworkRoomManager
{
    private KcpTransport kcpTransport;
    private UserInformation user;

    public override void Awake()
    {
        base.Awake();

        kcpTransport = GetComponent<KcpTransport>();
        user = new UserInformation();
        user.AddEvent();
        AddEvent();

        // ������
        EventManager<UserData>.TriggerEvent(UserData.ChangedUser, "ssss", "sdasdasd");
        
    }

    public override void OnDestroy()
    {
        user.RemoveEvent();
        RemoveEvent();

        base.OnDestroy();
    }

    // ���α׷��� ����Ǹ� �����ϰ� �ִ� ������ �����ϰų� �÷��̾� ���� ���ҽ�Ų��.
    // ���� ���� �ʿ�
    // ���α׷��� �����ϸ� ���ߴ� ���� �߻�
    
    // => Ping Pong ������� �̸� �ذ�
    // - ���� ������ Ŭ���̾�Ʈ���� Ping ��ȣ�� ������ Ŭ���̾�Ʈ���� Pong ��ȣ�� ��ȯ�ϴ� ���
    // �̰����� ���� ���� ���¸� Ȯ���Ѵ�.

    private void AddEvent()
    {
        EventManager<Tcp_Room_Command>.Binding(true, Tcp_Room_Command.connect, ConnectLoginUser);
        EventManager<Tcp_Room_Command>.Binding<string, GameObject, Transform>(true, Tcp_Room_Command.getRoomList, UpdateRoomList);
        EventManager<Tcp_Room_Command>.Binding<string, string, int>(true, Tcp_Room_Command.createRoom, CreateRoom);
        EventManager<Tcp_Room_Command>.Binding(true, Tcp_Room_Command.removeRoom, async () => await RemoveRoom());
        EventManager<Tcp_Room_Command>.Binding<string, string, string>(true, Tcp_Room_Command.enterRoom, EnterRoom);
        EventManager<Tcp_Room_Command>.Binding<string>(true, Tcp_Room_Command.enterSelectRoom, EnterSelectRoom);
        EventManager<Tcp_Room_Command>.Binding<bool>(true, Tcp_Room_Command.StartHost, ConnectedComplete);
        EventManager<Tcp_Room_Command>.Binding<string, string>(true, Tcp_Room_Command.EnterGameRoomClient, EnterGameRoomClient);
    }

    private void RemoveEvent()
    {
        EventManager<Tcp_Room_Command>.Binding(false, Tcp_Room_Command.connect, ConnectLoginUser);
        EventManager<Tcp_Room_Command>.Binding<string, GameObject, Transform>(false, Tcp_Room_Command.getRoomList, UpdateRoomList);
        EventManager<Tcp_Room_Command>.Binding<string, string, int>(false, Tcp_Room_Command.createRoom, CreateRoom);
        EventManager<Tcp_Room_Command>.Binding(false, Tcp_Room_Command.removeRoom, async () => await RemoveRoom());
        EventManager<Tcp_Room_Command>.Binding<string, string, string>(false, Tcp_Room_Command.enterRoom, EnterRoom);
        EventManager<Tcp_Room_Command>.Binding<string>(false, Tcp_Room_Command.enterSelectRoom, EnterSelectRoom);
        EventManager<Tcp_Room_Command>.Binding<bool>(false, Tcp_Room_Command.StartHost, ConnectedComplete);
        EventManager<Tcp_Room_Command>.Binding<string, string>(false, Tcp_Room_Command.EnterGameRoomClient, EnterGameRoomClient);
    }

    private void ConnectLoginUser()
    {
        MyTCPClient.Instance.SendRequestToServer($"{Tcp_Room_Command.connect},{user.playerID},{networkAddress}");
        Debug.Log("Ŭ�� ���� �Ϸ�");
    }

    // port, ip, password, playerCount
    private void UpdateRoomList(string roomListData, GameObject GameRoomButton, Transform GameRoomListContent)
    {
        string[] rooms = roomListData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        // ���� ���� �ֽ� �� �����͸� ��� HashSet
        HashSet<RoomData> latestRoomDataSet = new HashSet<RoomData>();

        // ������ �� ������ ����
        // �ߺ� ������ ���� �˻������� HashSet ���
        HashSet<RoomData> existingRoomDataSet = new HashSet<RoomData>();
        List<Transform> existingRoomObjects = new List<Transform>();

        foreach(Transform child in GameRoomListContent)
        {
            var gameRoom = child.GetComponent<GameRoom>();
            if(gameRoom != null)
            {
                existingRoomDataSet.Add(gameRoom.roomData);
                existingRoomObjects.Add(child);
            }
        }

        foreach (var room in rooms)
        {
            if (string.IsNullOrEmpty(room)) continue;
            string roomList = room.Substring("RoomList:".Length);
            string[] roomDatas = roomList.Split(',', StringSplitOptions.RemoveEmptyEntries);

            RoomData roomData = new RoomData();
            roomData.id = roomDatas[0];
            roomData.ip = roomDatas[1];
            roomData.isLock = bool.Parse(roomDatas[2]);
            roomData.roomName = roomDatas[3];
            roomData.currentPlayerCount = int.Parse(roomDatas[4]);
            roomData.maxPlayerCount = int.Parse(roomDatas[5]);
            roomData.joinCode = int.Parse(roomDatas[6]);

            // �������� ���� �� ����� �ֽ� HashSet�� �߰�
            latestRoomDataSet.Add(roomData);

            // ���� �̹� ������ ���� ���̶�� �ǳʶٱ�
            if (existingRoomDataSet.Contains(roomData)) continue;

            // ���ο� ���� �� ����
            GameObject newGameRoom = Instantiate(GameRoomButton);
            newGameRoom.transform.parent = GameRoomListContent;
            GameRoom GameRoomData = newGameRoom.GetComponent<GameRoom>();
            GameRoomData.InitGameRoomData(roomData);
        }

        // ���� �� ��� �� ������ �������� �ʴ� �� ����
        foreach (var roomObject in existingRoomObjects)
        {
            var gameRoom = roomObject.GetComponent<GameRoom>();
            if (gameRoom != null && !existingRoomDataSet.Contains(gameRoom.roomData))
            {
                Destroy(roomObject.gameObject); // ������ ���� ���� Ŭ���̾�Ʈ���� ����
            }
        }
    }

    // �� ����
    private async void CreateRoom(string roomName, string roomPassword, int maxCount)
    {
        // �� ���� �۾� ó��

        string hostIP = networkAddress;
        kcpTransport.port = (ushort)UnityEngine.Random.Range(1000, 6500);
        string hostPort = kcpTransport.port.ToString();
        string name = roomName;//input_RoomName.text;
        // �ִ� �÷��̾� ��
        int maxPlayerCount = maxCount;//this.maxPlayerCount;
        // ���� �� Password
        string passward = string.IsNullOrWhiteSpace(roomPassword) ? "1234" : roomPassword;


        string sendMessage = $"{Tcp_Room_Command.createRoom},{hostPort},{hostIP},{passward},{name},1,{maxPlayerCount},{user.playerID}";
        
        // TCP ����� ���� DB�� �� ���� ��û
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        /*var result = */
        
        await MyTCPClient.Instance.StartReceivingMessageAsync();
    }

    private void ConnectedComplete(bool isTrue)
    {
        if(isTrue) StartHost();
        else StopHost();
    }

    // ���� �� ����
    private async Task RemoveRoom()
    {
        string hostIp = networkAddress;
        string hostPort = kcpTransport.port.ToString();

        string sendMessage = $"{Tcp_Room_Command.removeRoom},{hostPort},{hostIp}";
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // ���� ���� ���
        await MyTCPClient.Instance.StartReceivingMessageAsync();
    }

    private async void EnterSelectRoom(string joinCode)
    {
        string sendMessage = $"{Tcp_Room_Command.enterSelectRoom},{joinCode}";
        
        // ���� �ڵ带 ���� ���� ��û
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // ���� ���� ��ٸ�
        await MyTCPClient.Instance.StartReceivingMessageAsync();      
    }

    private async void EnterRoom(string ip, string port, string password)
    {
        string sendMessage = $"{Tcp_Room_Command.enterRoom},{ip},{port},{password}";
        Debug.Log(sendMessage);
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        await MyTCPClient.Instance.StartReceivingMessageAsync();
    }

    private void EnterGameRoomClient(string ip, string port)
    {
        networkAddress = ip;
        kcpTransport.port = ushort.Parse(port);

        StartClient();

        ChangedPlayerCount(1);
    }

    private bool IsServerAvailable(string ip, int port, int timeout = 1000)
    {
        try
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                var result = tcpClient.BeginConnect(ip, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout));

                if (!success)
                {
                    // Ÿ�Ӿƿ� �߻�
                    return false;
                }

                // ���� ����
                tcpClient.EndConnect(result);
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private void ChangedPlayerCount(int changedType)
    {
        // ���� ���� ���� Id, IP, ���� Ÿ��(���� 1, ���� -1)
        string hostIP = networkAddress;
        string id = kcpTransport.port.ToString();
        string ChangedPlayerCountType = $"{changedType}";

        string sendMessage = $"{Tcp_Room_Command.ChangedPlayerCount},{hostIP},{id},{changedType},{user.playerID}";
        MyTCPClient.Instance.SendRequestToServer(sendMessage);
    }
}
