using MySql.Data.MySqlClient;
using System;
using System.Net.Sockets;
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
    public void InsertRoom(string serverID, string serverIP, string password)
    {
        string query = "INSERT INTO rooms (serverid, serverip, password) VALUES (@serverid, @serverip, @password)";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@serverid", serverID);
        cmd.Parameters.AddWithValue("@serverip", serverIP);
        cmd.Parameters.AddWithValue("@password", password);

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

    // 방 제거
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

    // 방 목록 조회
    public string GetRoomList()
    {
        string query = "SELECT serverid, serverip FROM rooms";
        MySqlCommand cmd = new MySqlCommand(query, connection);

        try
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            StringBuilder roomList = new StringBuilder();

            while (reader.Read())
            {
                string serverId = reader.GetString("serverid");
                string serverIp = reader.GetString("serverip");
                roomList.Append($"{serverId}, {serverIp};");
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

    // 방 입장
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

                // 비밀번호가 일치하는지 확인
                if(password == dbPassword)
                {
                    reader.Close();
                    return serverIp;    // 성공
                }
                else
                {
                    reader.Close();
                    return "Invalid password";  // 실패
                }
            }
            else{
                reader.Close();
                return "Room not found"; // 방 없음
            }
        }catch(Exception e)
        {
            Console.WriteLine("Error during room entry: " + e.Message);
            return "Error";
        }
    }
}