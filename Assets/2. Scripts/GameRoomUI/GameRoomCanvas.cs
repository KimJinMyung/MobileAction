using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomCanvas : MonoBehaviour
{
    [SerializeField] private List<RawImage> rawImages;

    private Dictionary<GameRoomPlayer, RawImage> gameRoomPlayerImageDIc = new Dictionary<GameRoomPlayer, RawImage>();

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void OnEnable()
    {
        if (gameRoomPlayerImageDIc.Count == 0)
        {
            foreach (var image in rawImages)
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    private void AddEvent()
    {
        EventManager<PlayerEventEnum.GameRoomPlayer>.Binding<GameRoomPlayer>(true, PlayerEventEnum.GameRoomPlayer.ConnectPlayer, ConnectPlayer);
        EventManager<PlayerEventEnum.GameRoomPlayer>.Binding<GameRoomPlayer>(true, PlayerEventEnum.GameRoomPlayer.DisConnectPlayer, DisConnectPlayer);
    }

    private void RemoveEvent()
    {
        EventManager<PlayerEventEnum.GameRoomPlayer>.Binding<GameRoomPlayer>(false, PlayerEventEnum.GameRoomPlayer.ConnectPlayer, ConnectPlayer);
        EventManager<PlayerEventEnum.GameRoomPlayer>.Binding<GameRoomPlayer>(false, PlayerEventEnum.GameRoomPlayer.DisConnectPlayer, DisConnectPlayer);
    }

    private void ConnectPlayer(GameRoomPlayer player)
    {
        if (gameRoomPlayerImageDIc.ContainsKey(player)) return;

        var renderTexture = new RenderTexture(256, 256, 16);
        renderTexture.Create();

        var camera = player.Cam;
        if(camera != null)
        {
            camera.targetTexture = renderTexture;
        }

        // 서버에서 만들어진 rawImages들을 받아와야 하지 않을까?


        // 이후 해당 데이터들을 기반으로 다음 rawImage를 활성화시킨다.
        foreach(var child in rawImages)
        {
            if (child.gameObject.activeSelf) continue;

            child.gameObject.SetActive(true);
            child.texture = renderTexture;
            gameRoomPlayerImageDIc.Add(player, child);
            break;
        }
    }

    private void DisConnectPlayer(GameRoomPlayer player)
    {
        if (gameRoomPlayerImageDIc.ContainsKey(player))
        {
            gameRoomPlayerImageDIc[player].texture = null;
            gameRoomPlayerImageDIc[player].gameObject.SetActive(false);
            gameRoomPlayerImageDIc.Remove(player);
        }
    }
}
