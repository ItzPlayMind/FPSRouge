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


    public Transform AttackPoint { get => attackPoint; }

    private CharacterStats stats;

    private float attackTimer = 0;

    protected bool canUse = false;
    public bool CanUse { get => canUse; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        hands.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (offHandItem != null)
            offHandItem = (Weapon)hands.Instantiate(offHandItem.Clone(), Hands.Hand.Off);
        if (mainHandItem != null)
            mainHandItem = (Weapon)hands.Instantiate(mainHandItem.Clone(), Hands.Hand.Main);
        stats = GetComponent<CharacterStats>();
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
                mainHandItem.Destroy();
                mainHandItem = item;
                hands.Instantiate(mainHandItem, hand);
                break;
            case Hands.Hand.Off:
                offHandItem.Destroy();
                offHandItem = item;
                hands.Instantiate(offHandItem, hand);
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
                    mainHandItem.Use(attackPoint, stats);
                    if (mainHandItem is Weapon)
                    {
                        var weapon = (mainHandItem as Weapon);
                        attackTimer = weapon.AttackSpeed; 
                        if (stats is PlayerStats)
                            (stats as PlayerStats).TakeStamina(weapon.StaminaUsage);
                    }
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
        hands.MainHandAnimator.Play("Use");
    }
}
