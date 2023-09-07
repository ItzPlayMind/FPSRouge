using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class BasicAI : NetworkBehaviour
{
    private enum State
    {
        Idle, Pathing, Attacking
    }

    [SerializeField] private float searchDistance = 50;
    [SerializeField] private float attackAngle = 30;
    [SerializeField] private LayerMask searchMask;
    [SerializeField] private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    [SerializeField] private Transform target = null;

    private NavMeshAgent agent;
    private WeaponManager weaponManager;
    private CharacterStats stats;

    private Dictionary<ulong,int> threadLevels = new Dictionary<ulong, int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); 
        agent = GetComponent<NavMeshAgent>();
        weaponManager = GetComponent<WeaponManager>();

        if (!IsOwner)
            return;

        stats = GetComponent<CharacterStats>();
        stats.OnTakeDamage += (damage, netID) =>
        {
            if (threadLevels.ContainsKey(netID))
                threadLevels[netID]++;
            else
                threadLevels.Add(netID, 1);

            SetTarget(threadLevels.Aggregate((x, y) => x.Value > y.Value ? x : y).Key);
        };
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        switch (state.Value)
        {
            case State.Idle:
                CheckForTargetsInArea();
                break;
            case State.Pathing:
                FollowTarget();
                break;
            case State.Attacking:
                AttackTarget();
                break;
        }
    }

    private void CheckForTargetsInArea()
    {
        if (target != null)
            return;
        var cols = Physics.OverlapSphere(transform.position, searchDistance, searchMask);
        if (cols != null && cols.Length > 0)
        {
            foreach (var item in cols)
            {
                var netOBJ = item.GetComponent<NetworkObject>();
                if(netOBJ != null)
                {
                    SetTarget(netOBJ.NetworkObjectId);
                    ChangeState(State.Pathing);
                    return;
                }
            }
        }
    }

    private void FollowTarget()
    {
        if (target == null)
        {
            ChangeState(State.Idle);
            return;
        }
        agent.stoppingDistance = weaponManager.GetWeapon(Hands.Hand.Main).AttackRange-0.5f;
        agent.SetDestination(target.position);
        if (weaponManager.IsInAttackRange(target))
        {
            ChangeState(State.Attacking);
        }
    }

    private void AttackTarget()
    {
        if (target == null)
        {
            ChangeState(State.Idle);
            return;
        }
        if (weaponManager.CanUse)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            if (Mathf.Abs(Utils.AngleBetween(weaponManager.AttackPoint.forward, dir)) <= attackAngle)
            {
                weaponManager.Attack();
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.01f);
            }
        }
        if (!weaponManager.IsInAttackRange(target))
        {
            ChangeState(State.Pathing);
        }
    }
    
    private State ChangeState(State newState)
    {
        state.Value = newState;
        switch (newState)
        {
            case State.Idle:
                agent.isStopped = false;
                break;
            case State.Pathing:
                agent.isStopped = false;
                break;
            case State.Attacking:
                agent.isStopped = true;
                break;
        }
        return state.Value;
    }

    public void SetTarget(ulong target)
    {
        Debug.Log("Setting Target to: " + target);
        SetTargetServerRpc(target);
    }

    [ServerRpc]
    private void SetTargetServerRpc(ulong target)
    {
        SetTargetClientRpc(target);
    }

    [ClientRpc]
    private void SetTargetClientRpc(ulong target)
    {
        var client = GetNetworkObject(target);
        if (client != null)
        {
            this.target = client.transform;
        }
    }
}
