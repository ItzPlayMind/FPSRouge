using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        Vector3 dir = (Camera.main.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(-dir);
        transform.position = transform.parent.position + offset;
    }
}
