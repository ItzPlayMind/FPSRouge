using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthball : MonoBehaviour
{
    [SerializeField] private GameObject gfx;
    [SerializeField] private float scaleIncrease = 0.1f;
    [SerializeField] private float rotationSpeed = 5f;

    private Rigidbody rb;
    private bool isStopped = false;

    public void Stop()
    {
        isStopped = true;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped)
            return;
        Vector3 dir = rb.velocity;
        dir.y = 0;
        gfx.transform.eulerAngles += dir.normalized * rotationSpeed;
        transform.localScale += Vector3.one * scaleIncrease;
    }
}
