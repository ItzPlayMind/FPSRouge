using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    protected override void _Attack(Transform usePoint, CharacterStats attacker)
    {
        //GetComponentInChildren<MeshRenderer>().material.color = Random.ColorHSV();
        var target = GetRaycastTarget(usePoint);
        if (target == null)
            return;
        target.TakeDamage(Damage,attacker.NetworkObjectId);
    }

    private CharacterStats GetRaycastTarget(Transform usePoint)
    {
        RaycastHit hit;
        if(Physics.Raycast(usePoint.transform.position, usePoint.transform.forward, out hit, AttackRange))
        {
            if(hit.transform != null)
            {
                Debug.Log("Hit " + hit.transform.name);
                return hit.transform.GetComponent<CharacterStats>();
            }
        }
        return null;
    }
}
