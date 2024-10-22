namespace TCP_Enum
{
    public struct RoomData
    {
        public string id;
        public string ip;
        public string password;
    }

    public enum Tcp_Room_Command
    {
        createRoom,
        removeRoom,
        getRoomList,
        requestRoomList,
        enterRoom,
    }
}