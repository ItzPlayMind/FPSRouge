using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MaterialDrop : Interactable
{
    [SerializeField] private Material material;
    [SerializeField] private GameObject ui;

    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;

    public void SetMaterial(Material material) => this.material = material;

    private bool pickedUp = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (material != null)
        {
            material.Instantiate(transform);
            Setup();
        }
    }

    public void Setup()
    {
        material.Instantiate(transform);
        nameText.text = material.name;
        descriptionText.text = material.description;
    }

    public override void _Interact(PlayerController player, InteractionType type)
    {
        pickedUp = true;
        player.GetComponent<MaterialManager>().AddMaterial(material);
        PickupServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupServerRpc()
    {
        PickupClientRpc();
    }

    [ClientRpc]
    private void PickupClientRpc()
    {
        Destroy();
    }

    public override void OnHover(PlayerController player)
    {

    }

    public override void OnHoverStart(PlayerController player)
    {
        if (!pickedUp)
            ui?.SetActive(true);
    }

    public override void OnHoverEnd(PlayerController player)
    {
        if (!pickedUp)
            ui?.SetActive(false);
    }
}
