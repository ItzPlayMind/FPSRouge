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
    [SerializeField] private LayerMask searchMask;
    [SerializeField] private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    [SerializeField] private Transform target = null;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        }
    }

    private void CheckForTargetsInArea()
    {
        if (target != null)
            return;
        Debug.Log("Checking for Targets!");
        var cols = Physics.OverlapSphere(transform.position, searchDistance, searchMask);
        if (cols != null && cols.Length > 0)
        {
            SetTarget(ulong.Parse(cols[0].transform.name));
            state.Value = State.Pathing;
        }
    }

    private void FollowTarget()
    {
        if (target == null)
            return;
        agent.SetDestination(target.position);
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
        var client = NetworkManager.Singleton.ConnectedClients[target];
        if (client != null)
        {
            this.target = client.PlayerObject.transform;
        }
    }
}
