using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private UnityEvent OnHit;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float despawnTime = 5f;
    public System.Action<GameObject> OnTargetHit { get; set; }

    private Rigidbody rb;

    private bool Hit;

    private GameObject spawner;

    public void SetSpawner(GameObject spawner) => this.spawner = spawner;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    public void AddForce(Vector3 force) => rb.AddForce(force, ForceMode.Impulse);

    private Vector3 lastPosition;
    private void Update()
    {
        if (Hit)
            return;

        RaycastHit hit;
        Vector3 dir = (transform.position - lastPosition).normalized;
        float dist = Vector3.Distance(transform.position, lastPosition);
        if (Physics.SphereCast(lastPosition, radius * transform.localScale.x, dir, out hit, dist, hitMask))
        {
            if (hit.transform.gameObject == spawner)
                return;
            Hit = true;
            OnTargetHit?.Invoke(hit.transform.gameObject);
            OnHit?.Invoke();
            rb.isKinematic = true;
            DespawnAfterTimeServerRpc(despawnTime);
            //transform.SetParent(hit.transform);
        }
        lastPosition = transform.position;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnAfterTimeServerRpc(float time)
    {
        StartCoroutine(DespawnAfterTimeCoroutine(time));
    }

    IEnumerator DespawnAfterTimeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<NetworkObject>().Despawn();
    }

    private void OnDrawGizmosSelected()
    {
        var color = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius*transform.localScale.x);
        Gizmos.color = color;
    }
}
