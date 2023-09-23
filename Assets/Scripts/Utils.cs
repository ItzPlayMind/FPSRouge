using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public static class Utils
{
    public delegate void OnChangeValue<T>(ref T value);
    public static float AngleBetween(Vector3 a, Vector3 b)
    {
        return Mathf.Acos(Vector3.Dot(a, b) / (Vector3.Magnitude(a) * Vector3.Magnitude(b))) * Mathf.Rad2Deg;
    }

    public static T Clone<T>(this T obj) where T : ScriptableObject
    {
        var script = GameObject.Instantiate(obj);
        script.name = obj.name;
        return script;
    }

    public static ClientRpcParams GetSendClientList(params ulong[] ids)
    {
        return new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = ids } };
    }

    public static Vector3 RandomVector3(Vector3 min, Vector3 max) => new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));


    [System.Serializable]
    public class Range<T>
    {
        public T min;
        public T max;

        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }
    }


    [System.Serializable]
    public class CustomValue
    {
        public enum Type
        {
            String, Float, Integer, Object
        }
        [SerializeField] private Type type;
        [SerializeField] private string stringValue;
        public string String { get => stringValue; }
        [SerializeField] private float floatValue;
        public float Float { get => floatValue; }
        [SerializeField] private int intValue;
        public int Integer { get => intValue; }
        [SerializeField] private Object objectValue;
        public Object Object { get => objectValue; }

        public override string ToString()
        {
            switch (type)
            {
                case Type.String:
                    return stringValue;
                case Type.Float:
                    return floatValue.ToString();
                case Type.Integer:
                    return intValue.ToString();
                case Type.Object:
                    return objectValue.ToString();
                default:
                    return "";
            }
        }
    }

    public static string UID(this ScriptableObject obj)
    {
        return obj.name.ToLower().Replace(" ", "_");
    }

    public static bool IsMagic(DamageType damageType) => damageType != DamageType.Slash && damageType != DamageType.Pierce && damageType != DamageType.Blunt;

}
