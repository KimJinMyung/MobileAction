using UnityEngine;
using Mirror;

public class BackMirror : NetworkBehaviour
{
    [SerializeField] private Camera backMirror;
    [SerializeField] private RenderTexture backMirrorTexture;

    public override void OnStartClient()
    {
        base.OnStartClient();

        SetBackMirrorRenderTexture();
    }

    private void SetBackMirrorRenderTexture()
    {
        if (!isLocalPlayer)
        {
            backMirror.enabled = false;
            return;
        }

        backMirror.targetTexture = backMirrorTexture;
    }
}
