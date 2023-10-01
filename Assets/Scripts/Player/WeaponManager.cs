using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private Item offHandItem;
    [SerializeField] private Item mainHandItem;
    [SerializeField] private Hands hands;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private bool overrideAnimators = true;

    private CustomAnimator animator;

    public Item MainHandItem { get => mainHandItem; }
    public Item OffHandItem { get => offHandItem; }

    public Transform AttackPoint { get => attackPoint; }

    private CharacterStats stats;

    private float swapTimer = 0;

    protected bool canUse = true;
    public bool CanUse { get => canUse; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        stats = GetComponent<CharacterStats>();
        animator = GetComponent<CustomAnimator>();
    }

    public void SetupHands()
    {
        if(animator == null)
            animator = GetComponent<CustomAnimator>();
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
        animator.SetAnimator(hands.MainHandAnimator);
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
        if (swapTimer >= 0)
            swapTimer -= Time.deltaTime;
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

    public void SwapItems()
    {
        if (mainHandItem == null || offHandItem == null)
            return;
        if (!IsOwner)
            return;
        if (swapTimer > 0)
            return;
        swapTimer = 0.1f;
        SwapItemsServerRpc();
    }

    [ServerRpc]
    private void SwapItemsServerRpc()
    {
        SwapItemsClientRpc();
    }

    [ClientRpc]
    private void SwapItemsClientRpc()
    {
        offHandItem?.Destroy(this); 
        mainHandItem?.Destroy();

        var item = offHandItem.Clone();
        offHandItem = mainHandItem;
        mainHandItem = item;

        hands.Instantiate(mainHandItem, Hands.Hand.Main, null, overrideAnimators);
        hands.Instantiate(offHandItem, Hands.Hand.Off, this, overrideAnimators);
        /*if (offHandItem != null)
            offHandItem.SetupOnEquip(this);*/
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
                        if (stats is PlayerStats)
                            (stats as PlayerStats).TakeStamina(weapon.StaminaUsage);
                        weapon.Trail?.Play();
                        animator.Play("UseOwner", ()=>
                        {
                            OnAnimationFinished();
                            weapon.Trail?.Stop();
                        }, (1/weapon.AttackSpeed));
                    }
                    else
                        animator.Play("UseOwner", OnAnimationFinished);
                    AttackServerRpc();
                }
        }
    }

    private void OnAnimationFinished()
    {
        canUse = true;
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
        if (mainHandItem is Weapon)
        {
            var weapon = mainHandItem as Weapon;
            weapon.Trail?.Play();
            animator.Play("UseClient", ()=>
            {
                weapon.Trail?.Stop();
            }, (1 / weapon.AttackSpeed));
        }
        else
        {
            animator.Play("UseClient", null);
        }
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
