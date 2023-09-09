using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public System.Action<GameObject> OnTargetHit { get; set; }

    private Rigidbody rb;

    private bool Hit;

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
        if(Physics.Linecast(lastPosition,transform.position, out hit))
        {
            Hit = true;
            OnTargetHit?.Invoke(hit.transform.gameObject);
            rb.isKinematic = true;
            //transform.SetParent(hit.transform);
        }
        lastPosition = transform.position;
    }
}
