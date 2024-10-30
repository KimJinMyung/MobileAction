using MySql.Data.MySqlClient;
using System;
using System.Text;

class GameRoomDBManager
{
    private MySqlConnection connection;

    public GameRoomDBManager()
    {
        // MySQL 연결 문자열 (데이터베이스 정보)
        string connectionString = "Server=localhost;Database=server;User ID=user;Password=kjm@7940718;Port=3306;SslMode=None";
        connection = new MySqlConnection(connectionString);
    }

    // DB 연결
    public void OpenConnection()
    {
        try
        {
            connection.Open();
            Console.WriteLine("Connected to MySQL DB");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Connecting to DB : " + ex.Message);
        }
    }

    // DB 끊기
    public void CloseConnection()
    {
        connection.Close();
    }

    // 방 생성
    public bool CreateRoom(string serverID, string serverIP, string password, string roomName, string currentCount, string maxCount, string PlayerId)
    {
        string query = "INSERT INTO rooms (serverid, serverip, password, roomName, currentPlayerCount, maxPlayerCount, JoinCode, connectPlayerId) VALUES (@serverid, @serverip, @password, @roomName, @currentPlayerCount, @maxPlayerCount, @JoinCode, @playerId)";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@serverid", serverID);
        cmd.Parameters.AddWithValue("@serverip", serverIP);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Parameters.AddWithValue("@roomName", roomName);
        cmd.Parameters.AddWithValue("@currentPlayerCount", currentCount);
        cmd.Parameters.AddWithValue("@maxPlayerCount", maxCount);

        Random random = new Random();
        int randomJoinCode = random.Next(10000, 90001);
        cmd.Parameters.AddWithValue("@JoinCode", randomJoinCode.ToString());

        cmd.Parameters.AddWithValue("@playerId", PlayerId);

        Console.WriteLine("Cmd Ready");

        try
        {
            cmd.ExecuteNonQuery();
            return true;
            Console.WriteLine("Room inserted into DB");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting room into DB : " + ex.Message);
            return false;
        }
    }

    // 방 제거
    public bool RemoveRoom(string serverID, string serverIp)
    {
        string query = "DELETE FROM rooms WHERE serverid = @serverid AND serverip = @serverip";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@serverid", serverID);
        cmd.Parameters.AddWithValue("@serverip", serverIp);

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Room removed from DB");
            return true;
        }catch(Exception e)
        {
            Console.WriteLine("Error removing from DB" + e.Message);
            return false;
        }
    }

    // 방 목록 조회
    public string GetRoomList()
    {
        string query = "SELECT serverid, serverip, password, roomName, currentPlayerCount, maxPlayerCount, JoinCode FROM rooms";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            StringBuilder roomList = new StringBuilder();

            while (reader.Read())
            {
                string serverId = reader.GetString("serverid");
                string serverIp = reader.GetString("serverip");
                string password = reader.GetString("Password");
                bool isLock = false;
                if (!string.IsNullOrEmpty(password))
                {
                    isLock = string.Equals(password, "Null")? false : true;
                }
                string roomName = reader.GetString("roomName");
                int currentPlayerCount = reader.GetInt32("currentPlayerCount");
                int maxPlayerCount = reader.GetInt32("maxPlayerCount");
                int JoinCode = reader.GetInt32("JoinCode");
                roomList.Append($"RoomList:{serverId},{serverIp},{isLock},{roomName},{currentPlayerCount},{maxPlayerCount},{JoinCode};");
            }

            reader.Close();
            return roomList.ToString();

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching room list: " + ex.Message);
            return string.Empty;
        }
    }

    // 코드를 통한 방 접속
    public string EnterSelectRoom(string JoinCode)
    {
        string query = "SELECT serverip, serverid FROM rooms WHERE JoinCode = @JoinCode";
        MySqlCommand cmd = new MySqlCommand( query, connection);
        cmd.Parameters.AddWithValue("@JoinCode", JoinCode);

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            StringBuilder roomList = new StringBuilder();

            if (reader.Read())
            {
                string serverIp = reader.GetString("serverip");
                string serverId = reader.GetString("serverid");

                roomList.Append($"{serverIp}, {serverId}");
            }

            reader.Close();
            return roomList.ToString();

        }catch (Exception ex)
        {
            Console.WriteLine("Error Join Room: " + ex.Message);
            return string.Empty;
        }
    }

    // 방 입장
    public string EnterRoom(string serverIP, string serverID, string password)
    {
        Console.WriteLine();

        string query = $"SELECT password FROM rooms WHERE serverid = '{serverID}' AND serverip = '{serverIP}'";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        Console.WriteLine($"Executing Query: {query} with serverID: {serverID}, serverIP: {serverIP}");

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string dbPassword = reader.GetString("password");
                string result = string.Empty;

                // 비밀번호가 일치하는지 확인
                if (string.Equals(password, dbPassword))
                {
                    reader.Close();
                    result = $"true,{serverIP},{serverID}";
                    Console.WriteLine($"접속 성공 : {serverIP} , {serverID}");
                    return result;    // 성공
                }
                else
                {
                    reader.Close();
                    Console.WriteLine("접속 실패");
                    return "false";  // 실패
                }
            }
            else
            {
                Console.WriteLine("Data Read Fail");
                reader.Close();
                return "false"; // 방 없음
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error during room entry: " + e.Message);
            return "Error";
        }
    }

    // 현재 있는 게임방의 플레이어 리스트 출력
    public string GetConnectPlayerIdFromSelectGameRoom(string serverIp, string serverId)
    {
        string query = $"SELECT connectPlayerId FROM rooms WHERE serverid = '{serverId}' AND serverip = '{serverIp}'";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                string connectPlayersId = reader.GetString("connectPlayerId");

                reader.Close();
                return connectPlayersId;
            }
            else
            {
                Console.WriteLine("Data Read Fail");
                reader.Close();
                return "Room not found"; // 방 없음
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error during room entry: " + e.Message);
            return "Error";
        }
    }

    // 플레이어 수 증가, 감소
    public void ChangedPlayerCount(string serverIp, string serverID, int ChangedType, string playerId)
    {
        string connectPlayersId = GetConnectPlayerIdFromSelectGameRoom(serverIp, serverID);

        int ChangedCount = 0;
        string currnetConnectPlayersId = string.Empty;

        if (ChangedType > 0)
        {
            currnetConnectPlayersId = $"{connectPlayersId},{playerId}";
            ChangedCount = 1;
        }
        else
        {
            int lastIndex = currnetConnectPlayersId.LastIndexOf($",{playerId}");
            if(lastIndex == currnetConnectPlayersId.Length - $",{playerId}".Length)
            {
                currnetConnectPlayersId = currnetConnectPlayersId.Substring(0, lastIndex);
            }
            ChangedCount = -1;
        }

        string playerCount = GetPlayerCount(serverIp, serverID);
        int Count = int.Parse(playerCount);
        Count = Count+ChangedCount;

        string query = $"UPDATE rooms SET currentPlayerCount = @currentPlayerCount, connectPlayerId = @connectPlayerId WHERE serverid = @serverid AND serverip = @serverip";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@currentPlayerCount", Count);
        cmd.Parameters.AddWithValue("@connectPlayerId", currnetConnectPlayersId);
        cmd.Parameters.AddWithValue("@serverid", serverID);
        cmd.Parameters.AddWithValue("@serverip", serverIp);

        try
        {
            cmd.ExecuteNonQuery();
            
            if (Count <= 0)
            {
                RemoveRoom(serverID, serverIp);
            }

            Console.WriteLine("Enter Player");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Enter room into DB : " + ex.Message);
        }
    }

    private string GetPlayerCount(string serverIp, string serverID)
    {
        string getPlayerCountQuery = $"SELECT currentPlayerCount FROM rooms WHERE serverip = '{serverIp}' AND serverid = '{serverID}'";
        MySqlCommand cmd = new MySqlCommand(getPlayerCountQuery, connection);

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                string playerCount = reader.GetInt32("currentPlayerCount").ToString();
                reader.Close();
                return playerCount;
            }
            else
            {
                Console.WriteLine("무리데승");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("오류 발생" + ex.Message);
            return string.Empty;
        }
    }

    // 플레이어 수 조회
    public string GetPlayerCount(string serverID)
    {
        string query = "SELECT playerCount FROM rooms WHERE serverid = @serverid";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        try
        {
            cmd.Parameters.AddWithValue("@serverid", serverID);

            MySqlDataReader reader = cmd.ExecuteReader();
            StringBuilder roomList = new StringBuilder();

            if (reader.Read())
            {
                string playerCount = reader.GetString("playerCount");
                roomList.Append(playerCount);
            }

            reader.Close();
            return roomList.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching room list: " + ex.Message);
            return string.Empty;
        }
    }
}