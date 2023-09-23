using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private Item offHandItem;
    [SerializeField] private Item mainHandItem;
    [SerializeField] private Hands hands;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private bool overrideAnimators = true;

    public Item MainHandItem { get => mainHandItem; }
    public Item OffHandItem { get => offHandItem; }

    public Transform AttackPoint { get => attackPoint; }

    private CharacterStats stats;

    private float attackTimer = 0;

    protected bool canUse = false;
    public bool CanUse { get => canUse; }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        stats = GetComponent<CharacterStats>();
    }

    public void SetupHands()
    {
        hands.gameObject.SetActive(true);
        if (mainHandItem != null)
            mainHandItem = (Weapon)hands.Instantiate(mainHandItem.Clone(), Hands.Hand.Main, null, overrideAnimators);
        if (offHandItem != null)
            offHandItem = (Weapon)hands.Instantiate(offHandItem.Clone(), Hands.Hand.Off, this, overrideAnimators);
        if (IsOwner)
            hands.AttackEventSender.OnAnimationEvent = () =>
            {
                mainHandItem.Use(attackPoint, stats);
            };
    }

    public Item GetItem(Hands.Hand hand)
    {
        switch (hand)
        {
            case Hands.Hand.Main:
                return mainHandItem;
            case Hands.Hand.Off:
                return offHandItem;
        }
        return null;
    }

    public void SetHands(Hands hands)
    {
        this.hands?.gameObject?.SetActive(false);
        this.hands = hands;
        this.hands?.gameObject?.SetActive(true);
    }

    public void SetAttackPoint(Transform point)
    {
        attackPoint = point;
    }

    public bool hasWeapon { get => mainHandItem is Weapon; }

    public Weapon GetWeapon(Hands.Hand hand)
    {
        switch (hand)
        {
            case Hands.Hand.Off:
                if (offHandItem is Weapon)
                    return offHandItem as Weapon;
                break;
            case Hands.Hand.Main:
                if (mainHandItem is Weapon)
                    return mainHandItem as Weapon;
                break;
        }
        return null;
    }

    public bool IsInAttackRange(Transform target)
    {
        if (mainHandItem == null || mainHandItem is not Weapon)
            return false;
        return Vector3.Distance(target.position, attackPoint.position) <= (mainHandItem as Weapon).AttackRange;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        offHandItem?.Passive(AttackPoint, stats);

        if (attackTimer >= 0)
            attackTimer -= Time.deltaTime;
        else
            canUse = true;
    }

    public void ChangeItem(Item item, Hands.Hand hand)
    {
        switch (hand)
        {
            case Hands.Hand.Main:
                mainHandItem?.Destroy();
                mainHandItem = item;
                hands.Instantiate(mainHandItem, hand, null, overrideAnimators);
                if (offHandItem != null)
                    offHandItem.SetupOnEquip(this);
                break;
            case Hands.Hand.Off:
                offHandItem?.Destroy(this);
                offHandItem = item;
                hands.Instantiate(offHandItem, hand, this, overrideAnimators);
                break;
        }
    }

    public void Attack()
    {
        if (IsOwner)
        {
            if (canUse)
                if (mainHandItem.CanUse(stats))
                {
                    canUse = false;
                    if (mainHandItem is Weapon)
                    {
                        var weapon = (mainHandItem as Weapon);
                        attackTimer = weapon.AttackSpeed; 
                        if (stats is PlayerStats)
                            (stats as PlayerStats).TakeStamina(weapon.StaminaUsage);
                    }
                    hands.MainHandAnimator.Play("UseOwner");
                    AttackServerRpc();
                }
        }
    }


    [ServerRpc]
    private void AttackServerRpc()
    {
        AttackClientRpc();
    }

    [ClientRpc]
    private void AttackClientRpc()
    {
        if (IsOwner)
            return;
        hands.MainHandAnimator.Play("UseClient");
    }

    private void OnDrawGizmosSelected()
    {
        if (mainHandItem != null)
        {
            if (mainHandItem is Weapon)
            {
                var weapon = mainHandItem as Weapon;
                Gizmos.DrawLine(attackPoint.position, attackPoint.position + attackPoint.forward * weapon.AttackRange);
                if (mainHandItem is MeleeWeapon)
                {
                    var meleweapon = weapon as MeleeWeapon;
                    Gizmos.DrawWireCube(attackPoint.position + new Vector3(0,0, meleweapon.Hitbox.z/2f),meleweapon.Hitbox);
                }
            }
        }
    }
}
