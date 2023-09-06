using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private Weapon leftHandWeapon;
    [SerializeField] private Weapon rightHandWeapon;
    [SerializeField] private Hands hands;

    InputManager inputManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
            hands = Camera.main.transform.Find("Hands")?.GetComponent<Hands>();
        hands.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (leftHandWeapon != null)
            leftHandWeapon = (Weapon)hands.Instantiate(leftHandWeapon, Hands.Hand.Left);
        if (rightHandWeapon != null)
            rightHandWeapon = (Weapon)hands.Instantiate(rightHandWeapon, Hands.Hand.Right);
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if(inputManager.PlayerAttackTrigger || inputManager.PlayerAttackHold)
        {
            AttackServerRpc();
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
        leftHandWeapon.Attack(transform);
    }
}
