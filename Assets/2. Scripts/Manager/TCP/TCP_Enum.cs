namespace TCP_Enum
{
    public struct RoomData
    {
        public string id;
        public string ip;
        public string roomName;
        public int currentPlayerCount;
        public int maxPlayerCount;
        public int joinCode;
        public bool isLock;
    }

    public enum Tcp_Room_Command
    {
        connect,
        createRoom,
        removeRoom,
        getRoomList,
        UpdateRoomList,
        requestRoomList,
        SelectRoom,
        enterRoom,
        enterSelectRoom,
        ChangedPlayerCount,
        GetPlayerCount,
        CloseServer,
        PONG,
    }
}