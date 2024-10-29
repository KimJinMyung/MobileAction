using PlayerEventEnum;
using TCP_Enum;

public class UserInformation
{
    public string playerID {  get; private set; }
    public string password {  get; private set; }

    public void AddEvent()
    {
        EventManager<UserData>.Binding<string, string>(true, UserData.ChangedUser, ChangedPlayerData);
    }

    public void RemoveEvent() 
    {
        EventManager<UserData>.Binding<string, string>(false, UserData.ChangedUser, ChangedPlayerData);
    }

    private void ChangedPlayerData(string playerID, string password)
    {
        this.playerID = playerID;
        this.password = password;

        EventManager<Tcp_Room_Command>.TriggerEvent(Tcp_Room_Command.connect);
    }
}
