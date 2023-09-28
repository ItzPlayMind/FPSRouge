using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    private static ScriptableObjectManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static ScriptableObjectManager Instance { get => instance; }

    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private List<Material> materials = new List<Material>();

    public T Get<T>(string uid) where T : ScriptableObject
    {
        var type = typeof(T);
        if (type == typeof(Material))
            return (T)(ScriptableObject)materials.Find(x => x.UID() == uid);
        if (type == typeof(Item))
            return (T)(ScriptableObject)items.Find(x => x.UID() == uid);
        return null;
    }

    public T GetRandom<T>() where T : ScriptableObject
    {
        var type = typeof(T);
        if (type == typeof(Item))
            return (T)(ScriptableObject)items[Random.Range(0, items.Count)];
        if (type == typeof(Material))
            return (T)(ScriptableObject)materials[Random.Range(0, materials.Count)];
        return null;
    }
}
