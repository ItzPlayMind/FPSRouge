using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemDrop : Interactable
{
    [SerializeField] private Item item;
    [SerializeField] private GameObject ui;

    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private UIField damageField;
    [SerializeField] private UIField rangeField;
    [SerializeField] private UIField speedField;
    [SerializeField] private UIField staminaField;

    public void SetItem(Item item) => this.item = item;

    private bool pickedUp = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (item != null)
        {
            item.Instantiate(transform);
            Setup();
        }
    }

    public void Setup()
    {
        item.Instantiate(transform);
        nameText.text = item.name;
        if(item is Weapon)
        {
            Weapon w = item as Weapon;
            damageField.Value = w.Damage.ToString();
            rangeField.Value = w.AttackRange.ToString();
            speedField.Value = w.AttackSpeed.ToString();
            staminaField.Value = w.StaminaUsage.ToString();
        }
        else
        {
            damageField.gameObject.SetActive(false);
            rangeField.gameObject.SetActive(false);
            staminaField.gameObject.SetActive(false);
            speedField.gameObject.SetActive(false);
        }
    }

    public override void Interact(PlayerController player, InteractionType type)
    {
        pickedUp = true;
        Hands.Hand hand = Hands.Hand.Main;
        if (type == InteractionType.Secondary)
            hand = Hands.Hand.Off;
        var item = player.GetComponent<WeaponManager>().GetItem(hand);
        if(item != null)
            SpawnManager.Instance.SpawnItemDrop(item.UID(), player.transform.position + Vector3.up, player.transform.forward, 10);
        PickupServerRpc(player.NetworkObjectId, hand);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupServerRpc(ulong id, Hands.Hand hand)
    {
        PickupClientRpc(id,hand);
    }

    [ClientRpc]
    private void PickupClientRpc(ulong id, Hands.Hand hand)
    {
        var networkOBJ = GetNetworkObject(id);
        if (networkOBJ != null)
        {
            var weaponManager = networkOBJ.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.ChangeItem(item, hand);
            }
        }
        Destroy();
    }

    public override void OnHover(PlayerController player)
    {
        
    }

    public override void OnHoverStart(PlayerController player)
    {
        if(!pickedUp)
            ui?.SetActive(true);
    }

    public override void OnHoverEnd(PlayerController player)
    {
        if(!pickedUp)
            ui?.SetActive(false);
    }
}
