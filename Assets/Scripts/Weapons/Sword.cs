using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    protected override void _Attack(Transform attacker)
    {
        Debug.Log(attacker.name + " attacked with a Sword!");
        //GetComponentInChildren<MeshRenderer>().material.color = Random.ColorHSV();
    }
}
