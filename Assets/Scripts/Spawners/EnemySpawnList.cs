using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Spawn List", menuName = "Enemy/Enemy Spawn List")]
public class EnemySpawnList : ScriptableObject
{
    public List<BasicAI> enemies = new List<BasicAI>();
}
