using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IslandPortal : MonoBehaviour
{
    public System.Action OnEnter;
    [SerializeField] private Transform point;
    [SerializeField] private Camera cam;
    [SerializeField] private float maxPortalRenderDistance = 25f;
    [SerializeField] private MeshRenderer portalRenderer;


    private IslandPortal targetIsland;
    private RenderTexture renderTexture;

    public void OnPortalEnter(Collider other)
    {
        if (targetIsland == null)
            return;
        var playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            OnEnter?.Invoke();
            playerController.SetPosition(targetIsland.point.position);
            playerController.SetRotation(targetIsland.point.rotation);
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(NetworkManager.Singleton.LocalClient.PlayerObject.transform.position, transform.position);
        if (distanceToPlayer <= maxPortalRenderDistance)
        {
            if (!portalRenderer.gameObject.activeSelf)
            {
                portalRenderer.gameObject.SetActive(true);
                targetIsland?.cam?.gameObject.SetActive(true);
            }
        }
        else
        {
            if (portalRenderer.gameObject.activeSelf)
            {
                portalRenderer.gameObject.SetActive(false);
                targetIsland?.cam?.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        renderTexture?.Release();
    }

    private RenderTexture GetPortalTexture()
    {
        renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        cam.targetTexture = renderTexture;
        return renderTexture;
    }

    private void SetPortalFrameTexture(RenderTexture renderTexture)
    {
        var material = new UnityEngine.Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.mainTexture = renderTexture;
        portalRenderer.material = material;
    }

    public void SetTargetPortal(IslandPortal island)
    {
        targetIsland = island;
        SetPortalFrameTexture(island.GetPortalTexture());
    }
}
