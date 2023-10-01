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

    public T Get<T>(string uid, string metaData = "") where T : ScriptableObject
    {
        var type = typeof(T);
        if (type == typeof(Material))
            return (T)(ScriptableObject)materials.Find(x => x.UID() == uid).Clone();
        if (type == typeof(Item))
        {
            var item = items.Find(x => x.UID() == uid).Clone();
            if(item is Weapon)
                item.FromMetadata(JsonUtility.FromJson<Weapon.Metadata>(metaData));
            return (T)(ScriptableObject)item.Clone();
        }
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
