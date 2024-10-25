using kcp2k;
using Mirror;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TCP_Enum;
using UnityEngine;

public class MyGameRoomManager : NetworkRoomManager
{
    private KcpTransport kcpTransport;

    public override void Awake()
    {
        base.Awake();

        kcpTransport = GetComponent<KcpTransport>();

        AddEvent();
    }

    public override void OnDestroy()
    {
        RemoveEvent();

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            RemoveRoom().GetAwaiter().GetResult();
        }
        else if (NetworkClient.isConnected && !NetworkServer.active)
        {
            ChangedPlayerCount(-1);
        }

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.CloseServer);
        base.OnDestroy();
    }

    // 프로그램이 종료되면 접속하고 있는 서버를 삭제하거나 플레이어 수를 감소시킨다.
    // 현재 수정 필요
    // 프로그램을 중지하면 멈추는 현상 발생
    //public override void OnApplicationQuit()
    //{
       
    //}

    private void AddEvent()
    {
        EventManager<Tcp_Room_Command>.Binding<string, GameObject, Transform>(true, Tcp_Room_Command.getRoomList, UpdateRoomList);
        EventManager<Tcp_Room_Command>.Binding<string, string, int>(true, Tcp_Room_Command.createRoom, CreateRoom);
        EventManager<Tcp_Room_Command>.Binding(true, Tcp_Room_Command.removeRoom, async () => await RemoveRoom());
        EventManager<Tcp_Room_Command>.Binding<string, string, string>(true, Tcp_Room_Command.enterRoom, EnterRoom);
        EventManager<Tcp_Room_Command>.Binding<string>(true, Tcp_Room_Command.enterSelectRoom, EnterSelectRoom);
    }

    private void RemoveEvent()
    {
        EventManager<Tcp_Room_Command>.Binding<string, GameObject, Transform>(false, Tcp_Room_Command.getRoomList, UpdateRoomList);
        EventManager<Tcp_Room_Command>.Binding<string, string, int>(false, Tcp_Room_Command.createRoom, CreateRoom);
        EventManager<Tcp_Room_Command>.Binding(false, Tcp_Room_Command.removeRoom, async () => await RemoveRoom());
        EventManager<Tcp_Room_Command>.Binding<string, string, string>(false, Tcp_Room_Command.enterRoom, EnterRoom);
        EventManager<Tcp_Room_Command>.Binding<string>(false, Tcp_Room_Command.enterSelectRoom, EnterSelectRoom);
    }

    // port, ip, password, playerCount
    private void UpdateRoomList(string roomListData, GameObject GameRoomButton, Transform GameRoomListContent)
    {
        string[] rooms = roomListData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        // 기존의 룸 데이터 수집
        // 중복 방지와 빠른 검색때문에 HashSet 사용
        HashSet<RoomData> hashRoomRoomData = new HashSet<RoomData>();
        foreach(Transform child in GameRoomListContent)
        {
            var gameRoom = child.GetComponent<GameRoom>();
            if(gameRoom != null)
            {
                hashRoomRoomData.Add(gameRoom.roomData);
            }
        }

        foreach (var room in rooms)
        {
            if (string.IsNullOrEmpty(room)) continue;

            string[] roomDatas = room.Split(',', StringSplitOptions.RemoveEmptyEntries);

            RoomData roomData = new RoomData();
            roomData.id = roomDatas[0];
            roomData.ip = roomDatas[1];
            Debug.LogError($"Room IP : {roomData.ip}");
            roomData.isLock = bool.Parse(roomDatas[2]);
            roomData.roomName = roomDatas[3];
            roomData.currentPlayerCount = int.Parse(roomDatas[4]);
            roomData.maxPlayerCount = int.Parse(roomDatas[5]);
            roomData.joinCode = int.Parse(roomDatas[6]);

            // 만약 이미 생성된 게임 룸이라면 건너뛰기
            if (hashRoomRoomData.Contains(roomData)) continue;

            // 만약 삭제된 게임 룸이라면 Destory

            // 새로운 게임 룸 생성
            foreach(var child in roomDatas)
            {
                Debug.Log(child);
            }

            GameObject newGameRoom = Instantiate(GameRoomButton);
            newGameRoom.transform.parent = GameRoomListContent;
            GameRoom GameRoomData = newGameRoom.GetComponent<GameRoom>();
            GameRoomData.InitGameRoomData(roomData);
        }
    }

    private void CreateRoom(string roomName, string roomPassword, int maxCount)
    {
        // 방 설정 작업 처리

        // 호스트의 IP 주소를 중앙 서버나 관리 스크립트에 등록
        // 게임 룸 서버 주소 Ip
        string hostIP = networkAddress;
        // 게임 룸 포트
        // 로컬 용
        kcpTransport.port = (ushort)UnityEngine.Random.Range(1000, 6500);
        string hostPort = kcpTransport.port.ToString();

        string name = roomName;//input_RoomName.text;

        // 최대 플레이어 수
        int maxPlayerCount = maxCount;//this.maxPlayerCount;

        // 게임 룸 Password
        string passward = string.IsNullOrWhiteSpace(roomPassword) ? "1234" : roomPassword;

        string sendMessage = $"{Tcp_Room_Command.createRoom},{hostPort},{hostIP},{passward},{name},1,{maxPlayerCount}";
        // TCP 통신을 통해 DB에 방 생성 요청
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // 호스트가 방을 생성
        StartHost();
    }

    private async Task RemoveRoom()
    {
        string hostIp = networkAddress;
        string hostPort = kcpTransport.port.ToString();

        string sendMessage = $"{Tcp_Room_Command.removeRoom},{hostPort},{hostIp}";
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        string response = await MyTCPClient.Instance.ReceiveRoomListFromServer(Tcp_Room_Command.removeRoom);

        if (bool.TryParse(response, out bool isSuccess) && isSuccess)
        {
            Debug.Log("Room removed successfully.");
        }
        else
        {
            Debug.LogError("Failed to remove room.");
        }

        StopHost();
    }

    private async void EnterSelectRoom(string joinCode)
    {
        string sendMessage = $"{Tcp_Room_Command.enterSelectRoom},{joinCode}";
        
        // 입장 코드를 통해 입장 요청
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        // 서버 응답 기다림
        var roomData = await MyTCPClient.Instance.ReceiveRoomListFromServer(Tcp_Room_Command.enterSelectRoom);

        if (string.IsNullOrEmpty(roomData))
        {
            Debug.LogError("서버를 찾을 수 없습니다.");
            return;
        }

        string[] data = roomData.Split(',', StringSplitOptions.RemoveEmptyEntries);

        string roomIP = data[0];
        int roomPort = int.Parse(data[1]);

        networkAddress = roomIP;
        kcpTransport.port = ushort.Parse(data[1]);

        StartClient();

        ChangedPlayerCount(1);        
    }

    private async void EnterRoom(string ip, string port, string password)
    {
        string sendMessage = $"{Tcp_Room_Command.enterRoom},{ip},{port},{password}";
        Debug.Log(sendMessage);
        MyTCPClient.Instance.SendRequestToServer(sendMessage);

        var responseMessage = await MyTCPClient.Instance.ReceiveRoomListFromServer(Tcp_Room_Command.enterRoom);
        
        string[] message = responseMessage.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if(bool.TryParse(message[0], out bool isSuccess) && isSuccess)
        {
            networkAddress = ip;
            kcpTransport.port = ushort.Parse(port);

            StartClient();

            ChangedPlayerCount(1);
        }
        else
        {
            Debug.LogError("만들어진 서버가 없습니다.");
        }
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

        string sendMessage = $"{Tcp_Room_Command.ChangedPlayerCount},{hostIP},{id},{changedType}";
        MyTCPClient.Instance.SendRequestToServer(sendMessage);
    }
}
