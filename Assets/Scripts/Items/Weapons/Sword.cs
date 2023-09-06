using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    protected override void _Attack(Transform attacker)
    {
        //GetComponentInChildren<MeshRenderer>().material.color = Random.ColorHSV();
        var target = GetRaycastTarget();
        if (target == null)
        {
            Debug.Log("Hit nothing of value!");
            return;
        }
        target.TakeDamage(Damage);
    }

    private CharacterStats GetRaycastTarget()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, AttackRange))
        {
            if(hit.transform != null)
            {
                return hit.transform.GetComponent<CharacterStats>();
            }
        }
        return null;
    }
}
