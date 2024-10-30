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

        // 디버깅용
        EventManager<UserData>.TriggerEvent(UserData.ChangedUser, "ssss", "sdasdasd");
        
    }

    public override void OnDestroy()
    {
        user.RemoveEvent();
        RemoveEvent();

        base.OnDestroy();
    }

    // 프로그램이 종료되면 접속하고 있는 서버를 삭제하거나 플레이어 수를 감소시킨다.
    // 현재 수정 필요
    // 프로그램을 중지하면 멈추는 현상 발생
    
    // => Ping Pong 방식으로 이를 해결
    // - 서버 측에서 클라이언트에게 Ping 신호를 보내고 클라이언트에서 Pong 신호를 반환하는 방식
    // 이것으로 둘의 연결 상태를 확인한다.

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
        Debug.Log("클라 연결 완료");
    }

    // port, ip, password, playerCount
    private void UpdateRoomList(string roomListData, GameObject GameRoomButton, Transform GameRoomListContent)
    {
        string[] rooms = roomListData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        // 수신 받은 최신 방 데이터를 담는 HashSet
        HashSet<RoomData> latestRoomDataSet = new HashSet<RoomData>();

        // 기존의 룸 데이터 수집
        // 중복 방지와 빠른 검색때문에 HashSet 사용
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

            // 서버에서 받은 방 목록을 최신 HashSet에 추가
            latestRoomDataSet.Add(roomData);

            // 만약 이미 생성된 게임 룸이라면 건너뛰기
            if (existingRoomDataSet.Contains(roomData)) continue;

            // 새로운 게임 룸 생성
            GameObject newGameRoom = Instantiate(GameRoomButton);
            newGameRoom.transform.parent = GameRoomListContent;
            GameRoom GameRoomData = newGameRoom.GetComponent<GameRoom>();
            GameRoomData.InitGameRoomData(roomData);
        }

        // 기존 방 목록 중 서버에 존재하지 않는 방 제거
        foreach (var roomObject in existingRoomObjects)
        {
            var gameRoom = roomObject.GetComponent<GameRoom>();
            if (gameRoom != null && !existingRoomDataSet.Contains(gameRoom.roomData))
            {
                Destroy(roomObject.gameObject); // 서버에 없는 방을 클라이언트에서 제거
            }
        }
    }

    // 방 생성
    private async void CreateRoom(string roomName, string roomPassword, int maxCount)
    {
        // 방 설정 작업 처리

        string hostIP = networkAddress;
        kcpTransport.port = (ushort)UnityEngine.Random.Range(1000, 6500);
        string hostPort = kcpTransport.port.ToString();
        string name = roomName;//input_RoomName.text;
        // 최대 플레이어 수
        int maxPlayerCount = maxCount;//this.maxPlayerCount;
        // 게임 룸 Password
        string passward = string.IsNullOrWhiteSpace(roomPassword) ? "1234" : roomPassword;


        string sendMessage = $"{Tcp_Room_Command.createRoom},{hostPort},{hostIP},{passward},{name},1,{maxPlayerCount},{user.playerID}";
        
        // TCP 통신을 통해 DB에 방 생성 요청
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        /*var result = */
        
        await MyTCPClient.Instance.StartReceivingMessageAsync();
    }

    private void ConnectedComplete(bool isTrue)
    {
        if(isTrue) StartHost();
        else StopHost();
    }

    // 서버 룸 제거
    private async Task RemoveRoom()
    {
        string hostIp = networkAddress;
        string hostPort = kcpTransport.port.ToString();

        string sendMessage = $"{Tcp_Room_Command.removeRoom},{hostPort},{hostIp}";
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // 서버 응답 대기
        await MyTCPClient.Instance.StartReceivingMessageAsync();
    }

    private async void EnterSelectRoom(string joinCode)
    {
        string sendMessage = $"{Tcp_Room_Command.enterSelectRoom},{joinCode}";
        
        // 입장 코드를 통해 입장 요청
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // 서버 응답 기다림
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
                    // 타임아웃 발생
                    return false;
                }

                // 연결 성공
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
        // 현재 속한 방의 Id, IP, 변경 타입(증가 1, 감소 -1)
        string hostIP = networkAddress;
        string id = kcpTransport.port.ToString();
        string ChangedPlayerCountType = $"{changedType}";

        string sendMessage = $"{Tcp_Room_Command.ChangedPlayerCount},{hostIP},{id},{changedType},{user.playerID}";
        MyTCPClient.Instance.SendRequestToServer(sendMessage);
    }
}
