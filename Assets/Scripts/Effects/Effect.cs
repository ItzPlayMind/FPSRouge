using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effects/New Effect")]
public class Effect : ScriptableObject
{
    [System.Serializable]
    public class CustomValue
    {
        public enum Type
        {
            String, Float, Integer, Object
        }
        [SerializeField] private Type type;
        [Header("String")]
        [SerializeField] private string stringValue;
        [Header("Float")]
        [SerializeField] private float floatValue;
        [SerializeField] private int floatValueLevelIncrease;
        [Header("Int")]
        [SerializeField] private int intValue;
        [SerializeField] private int intValueLevelIncrease;
        [Header("Object")]
        [SerializeField] private Object objectValue;

        public object Get(int level)
        {
            switch (type)
            {
                case Type.String:
                    return stringValue;
                case Type.Float:
                    return floatValue+(floatValueLevelIncrease*(level-1));
                case Type.Integer:
                    return intValue+(intValueLevelIncrease* (level - 1));
                case Type.Object:
                    return objectValue;
                default:
                    return null;
            }
        }
    }

    [Multiline]
    [SerializeField] private string description;
    [SerializeField] private CustomValue[] variables;

    private int level = 1;
    public int Level { get => level; }

    public void SetLevel(int level) => this.level = level;

    public CustomValue[] Variables { get => variables; }
   

    public string Description { get => DescriptionCreator.Generate(description, GetVariablesForDescription()); }

    private Dictionary<string, object> variableStore = new Dictionary<string, object>();
    public Dictionary<string,object> VariableStore { get => variableStore; }

    private MethodInfo passiv;
    private MethodInfo onequip;
    private MethodInfo onunequip;

    private object[] GetVariablesForDescription()
    {
        object[] objects = new object[Variables.Length];
        for (int i = 0; i < objects.Length; i++)
            objects[i] = Variables[i].Get(level);
        return objects;
    }

    public void Setup()
    {
        onequip = typeof(EffectMethods).GetMethod(EffectMethods.GetEquipMethodName(name));
        passiv = typeof(EffectMethods).GetMethod(EffectMethods.GetPassiveMethodName(name));
        onunequip = typeof(EffectMethods).GetMethod(EffectMethods.GetUnequipMethodName(name));
    }

    public void Passive(Item item, Transform usePoint, CharacterStats user) => passiv?.Invoke(null, new object[] { this, item, usePoint, user });
    public void OnEquip(Item item, WeaponManager manager) => onequip?.Invoke(null, new object[] { this, item, manager });
    public void OnUnequip(Item item, WeaponManager manager) => onunequip?.Invoke(null, new object[] { this, item, manager });
}
