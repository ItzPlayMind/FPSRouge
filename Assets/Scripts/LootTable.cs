using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot/New Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public class Chance
    {
        public ScriptableObject loot;
        public float chance; // in %
    }

    [SerializeField] private List<Chance> chances = new List<Chance>();


    public ScriptableObject Generate(int maxCycles = 10)
    {
        Chance c = null;
        for (int i = 0; i < maxCycles; i++)
        {
            c = chances[Random.Range(0, chances.Count)];
            if (c.chance >= Random.Range(0f, 100f))
                return c.loot;
        }
        return c.loot;
    }
}
