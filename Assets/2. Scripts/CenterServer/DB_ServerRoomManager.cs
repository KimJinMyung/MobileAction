using PlayerEventEnum;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DB_ServerRoomManager : MonoBehaviour
{
    // Node.js ���� �ּ�
    private string serverUrl = "http://192.168.0.129:3000";

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<DB_Event>.Binding<string, string, string>(true, DB_Event.CreateRoom, CreateRoom);
        EventManager<DB_Event>.Binding<string>(true, DB_Event.DeleteRoom, deleteRoom);
        EventManager<DB_Event>.Binding<string, string>(true, DB_Event.EnterRoom, enterRoom);
    }

    private void RemoveEvent()
    {
        EventManager<DB_Event>.Binding<string, string, string>(false, DB_Event.CreateRoom, CreateRoom);
        EventManager<DB_Event>.Binding<string>(false, DB_Event.DeleteRoom, deleteRoom);
        EventManager<DB_Event>.Binding<string, string>(false, DB_Event.EnterRoom, enterRoom);
    }

    private void CreateRoom(string id, string ip, string password)
    {
        StartCoroutine(CreateGameRoomCoroutine(id, ip, password));
    }

    private void deleteRoom(string id)
    {
        StartCoroutine (DeleteGameRoomCoroutine(id));   
    }

    private void enterRoom(string id, string password)
    {
        StartCoroutine(EnterGameRoomCoroutine(id, password));
    }

    private IEnumerator CreateGameRoomCoroutine(string serverid, string serverip, string password)
    {
        var data = new
        {
            serverid = serverid,
            serverip = serverip,
            password = password
        };

        // Json ��ȯ
        string jsonData = JsonUtility.ToJson(data);

        string createRoomUrl = serverUrl + "/createServer";
        Debug.Log(createRoomUrl);

        using(UnityWebRequest www = new UnityWebRequest(createRoomUrl, "POST"))
        {
            // Json ���ڿ��� ����Ʈ �迭�� ��ȯ
            // HTTP ��û ������ ����Ʈ �迭�� ���·� ���۵Ǳ⿡ ��ȯ�� �ʿ��ߴ�.
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);  // UTF-8 �� ���ڵ��Ͽ� ����Ʈ �迭�� ��ȯ
            // HTTP ��û ������ Json �����͸� ���Խ�Ų��.
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);  // ����Ʈ �迭�� ���� ����
            // �����κ��� ���Ź��� ���� �����͸� �޸𸮿� ���۷� ����
            www.downloadHandler = new DownloadHandlerBuffer();  // �������� ������ �����͸� �޸𸮿� ���۸��ϰ�, �̸� �����ϴ� ����
            // HTTp ��û�� ����� application/json���� �����Ͽ� ������ ������ ������ Json���� �˸� 
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response:" + jsonResponse);

                CreateServerResponse response = JsonUtility.FromJson<CreateServerResponse>(jsonResponse);

                // ���� �� ���� ���� ���� Ȯ��
                if (response.success)
                {
                    Debug.Log("Create Room successful: " + response.message);
                }
                else
                {
                    Debug.Log("Create Room failed: " + response.message);
                }
            }
            else
            {
                Debug.LogError("Request failed: " + www.error);
            }
        }
    }

    private IEnumerator DeleteGameRoomCoroutine(string serverid)
    {
        var data = new
        {
            serverid = serverid
        };

        string jsonData = JsonUtility.ToJson(data);
        string deleteUrl = serverUrl + "/deleteServer";

        using (UnityWebRequest www = new UnityWebRequest(deleteUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response:" + jsonResponse);

                DeleteServerResponse response = JsonUtility.FromJson<DeleteServerResponse>(jsonResponse);

                // ���� �� ���� ���� ���� Ȯ��
                if (response.success)
                {
                    Debug.Log("Delete successful: " + response.message);
                }
                else
                {
                    Debug.Log("Delete failed: " + response.message);
                }
            }
            else
            {
                Debug.LogError("Request failed: " + www.error);
            }
        }
    }

    private IEnumerator EnterGameRoomCoroutine(string serverid, string password)
    {
        var data = new
        {
            serverid = serverid,
            password = password
        };

        string jsonData = JsonUtility.ToJson(data);
        string deleteUrl = serverUrl + "/enterRoom";

        using (UnityWebRequest www = new UnityWebRequest(deleteUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response:" + jsonResponse);

                EnterServerResponse response = JsonUtility.FromJson<EnterServerResponse>(jsonResponse);

                // ���� �� ���� ���� ���� Ȯ��
                if (response.success)
                {
                    Debug.Log("Delete successful: " + response.message);
                }
                else
                {
                    Debug.Log("Delete failed: " + response.message);
                }
            }
            else
            {
                Debug.LogError("Request failed: " + www.error);
            }
        }
    }

    public struct CreateServerResponse
    {
        public bool success;
        public string message;
    }

    public struct DeleteServerResponse
    {
        public bool success;
        public string message;
    }
    public struct EnterServerResponse
    {
        public bool success;
        public string message;
        public string ip;
    }
}
