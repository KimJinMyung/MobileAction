using PlayerEventEnum;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DB_ServerRoomManager : MonoBehaviour
{
    // Node.js 서버 주소
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

        // Json 변환
        string jsonData = JsonUtility.ToJson(data);

        string createRoomUrl = serverUrl + "/createServer";
        Debug.Log(createRoomUrl);

        using(UnityWebRequest www = new UnityWebRequest(createRoomUrl, "POST"))
        {
            // Json 문자열을 바이트 배열로 변환
            // HTTP 요청 본문은 바이트 배열의 형태로 전송되기에 변환이 필요했다.
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);  // UTF-8 로 인코딩하여 바이트 배열로 변환
            // HTTP 요청 본문에 Json 데이터를 포함시킨다.
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);  // 바이트 배열을 직접 전송
            // 서버로부터 수신받은 응답 데이터를 메모리에 버퍼로 저장
            www.downloadHandler = new DownloadHandlerBuffer();  // 서버에서 수신한 데이터를 메모리에 버퍼링하고, 이를 제공하는 역할
            // HTTp 요청의 헤더를 application/json으로 설정하여 본문의 데이터 형식이 Json임을 알림 
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response:" + jsonResponse);

                CreateServerResponse response = JsonUtility.FromJson<CreateServerResponse>(jsonResponse);

                // 서버 룸 생성 성공 여부 확인
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

                // 서버 룸 제거 성공 여부 확인
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

                // 서버 룸 접속 성공 여부 확인
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
