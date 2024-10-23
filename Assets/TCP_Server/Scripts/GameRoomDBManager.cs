using MySql.Data.MySqlClient;
using System;
using System.Net.Sockets;
using System.Text;

class GameRoomDBManager
{
    private MySqlConnection connection;

    public GameRoomDBManager()
    {
        // MySQL ���� ���ڿ� (�����ͺ��̽� ����)
        string connectionString = "Server=localhost;Database=server;User ID=user;Password=kjm@7940718;Port=3306;SslMode=None";
        connection = new MySqlConnection(connectionString);
    }

    // DB ����
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

    // DB ����
    public void CloseConnection()
    {
        connection.Close();
    }

    // �� ����
    public void InsertRoom(string serverID, string serverIP, string password, string roomName, string currentCount, string maxCount)
    {
        string query = "INSERT INTO rooms (serverid, serverip, password, roomName, currentPlayerCount, maxPlayerCount) VALUES (@serverid, @serverip, @password, @roomName, @currentPlayerCount, @maxPlayerCount)";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@serverid", serverID);
        cmd.Parameters.AddWithValue("@serverip", serverIP);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Parameters.AddWithValue("@roomName", roomName);
        cmd.Parameters.AddWithValue("@currentPlayerCount", currentCount);
        cmd.Parameters.AddWithValue("@maxPlayerCount", maxCount);

        Console.WriteLine("Cmd Ready");

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Room inserted into DB");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error inserting room into DB : " + ex.Message);
        }
    }

    // �� ����
    public void RemoveRoom(string serverID)
    {
        string query = "DELETE FROM rooms WHERE serverid = @serverid";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@serverid", serverID);

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Room removed from DB");
        }catch(Exception e)
        {
            Console.WriteLine("Error removing from DB" + e.Message);
        }
    }

    // �� ��� ��ȸ
    public string GetRoomList()
    {
        string query = "SELECT serverid, serverip, password, roomName, currentPlayerCount, maxPlayerCount FROM rooms";
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
                string roomName = reader.GetString("roomName");
                int currentPlayerCount = reader.GetInt32("currentPlayerCount");
                int maxPlayerCount = reader.GetInt32("maxPlayerCount");
                roomList.Append($"{serverId}, {serverIp}, {password}, {roomName}, {currentPlayerCount}, {maxPlayerCount};");
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

    // �� ����
    public string EnterSelectRoom(string roomID, string password)
    {
        string query = "SELECT serverip, password FROM rooms WHERE serverid = @serverid";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@serverid", roomID);

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string dbPassword = reader.GetString("password");
                string serverIp = reader.GetString("serverip");

                // ��й�ȣ�� ��ġ�ϴ��� Ȯ��
                if(password == dbPassword)
                {
                    reader.Close();
                    return serverIp;    // ����
                }
                else
                {
                    reader.Close();
                    return "Invalid password";  // ����
                }
            }
            else{
                reader.Close();
                return "Room not found"; // �� ����
            }
        }catch(Exception e)
        {
            Console.WriteLine("Error during room entry: " + e.Message);
            return "Error";
        }
    }

    // �÷��̾� �� ����, ����
    public void ChangedPlayerCount(string serverID, int ChangedType)
    {
        int ChangedCount = ChangedType > 0 ? 1 : -1;
        string query = $"UPDATE rooms SET playerCount = playerCount + {ChangedCount} WHERE serverid = @serverid";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@serverid", serverID);

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("Enter Player");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Enter room into DB : " + ex.Message);
        }
    }

    // �÷��̾� �� ��ȸ
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