using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static ItemManager Instance { get => instance; }

    [SerializeField] private List<Item> items = new List<Item>();

    public Item GetItem(string uid)
    {
        return items.Find(x => x.UID() == uid);
    }

    public Item GetRandomItem()
    {
        return items[Random.Range(0, items.Count)];
    }
}
