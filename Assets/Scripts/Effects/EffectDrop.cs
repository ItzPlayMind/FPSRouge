using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EffectDrop : Interactable
{
    [SerializeField] private Effect effect;
    [SerializeField] private GameObject ui;

    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;

    public void SetEffect(Effect effect) => this.effect = effect;

    private bool pickedUp = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (effect != null)
        {
            Setup();
        }
    }

    public void Setup()
    { 
        nameText.text = effect.name;
        descriptionText.text = effect.Description;
    }

    public override void _Interact(PlayerController player, InteractionType type)
    {
        pickedUp = true;
        PickupServerRpc(player.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupServerRpc(ulong id)
    {
        PickupClientRpc(id);
    }

    [ClientRpc]
    private void PickupClientRpc(ulong id)
    {
        var obj = GetNetworkObject(id);
        var weaponManager = obj.GetComponent<WeaponManager>();
        if(weaponManager != null)
        {
            if(weaponManager.MainHandItem is Weapon)
            {
                (weaponManager.MainHandItem as Weapon).AddEffect(effect, weaponManager);
                Destroy();
            }
        }
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
