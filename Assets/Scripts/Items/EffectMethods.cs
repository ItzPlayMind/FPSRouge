using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectMethods
{
    //public static void NAME__OnPassive(Effect effect, Item item, Transform usepoint, CharacterStats user)
    //{
    //}
    //public static void NAME_OnEquip(Effect effect, Item item, WeaponManager manager)
    //{
    //}
    //public static void NAME_OnUnequip(Effect effect, Item item, WeaponManager manager)
    //{
    //}

    public static string GetEquipMethodName(string name) => name + "_OnEquip";
    public static string GetUnequipMethodName(string name) => name + "_OnUnequip";
    public static string GetPassiveMethodName(string name) => name + "_OnPassive";

    private static void WithDelay(MonoBehaviour obj, System.Action action, float time) => obj.StartCoroutine(WithDelayCoroutine(action, time));

    private static IEnumerator WithDelayCoroutine(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

    #region Physical
    public static void Physical_OnEquip(Effect effect, Item item, WeaponManager manager)
    {
        if (!Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            effect.VariableStore["OnChangeDamage"] = new Utils.OnChangeValue<float>((ref float value) =>
            {
                value *= 1 + (effect.Variables[0].Float / 100f);
            });
            (manager.MainHandItem as Weapon).OnChangeDamage += (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    public static void Physical_OnUnequip(Effect effect, Item item, WeaponManager manager)
    {
        if (!Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            (manager.MainHandItem as Weapon).OnChangeDamage -= (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    #endregion

    #region Magical
    public static void Magical_OnEquip(Effect effect, Item item, WeaponManager manager)
    {
        if (Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            effect.VariableStore["OnChangeDamage"] = new Utils.OnChangeValue<float>((ref float value) =>
            {
                value *= 1 + (effect.Variables[0].Float / 100f);
            });
            (manager.MainHandItem as Weapon).OnChangeDamage += (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    public static void Magical_OnUnequip(Effect effect, Item item, WeaponManager manager)
    {
        if (Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            (manager.MainHandItem as Weapon).OnChangeDamage -= (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    #endregion

    #region Archery
    public static void Archery_OnEquip(Effect effect, Item offHandItem, WeaponManager manager)
    {
        if (manager.MainHandItem is RangedWeapon)
        {
            Debug.Log("IS RANGED!");
            effect.VariableStore["OnUse"] = new System.Action<Item, Transform, CharacterStats>((Item item, Transform usePoint, CharacterStats stats) =>
            {
                Debug.Log("USED!");
                for (int i = 0; i < effect.Variables[0].Integer; i++)
                {
                    WithDelay(manager, () =>
                    {
                        Debug.Log("USE AGAIN!");
                        item.Use(usePoint, stats, false);
                    }, 0.1f * (i+1));
                }
            });
            manager.MainHandItem.OnUse += (System.Action<Item, Transform, CharacterStats>)effect.VariableStore["OnUse"];
        }
    }
    public static void Archery_OnUnequip(Effect effect, Item item, WeaponManager manager)
    {
        if (item is RangedWeapon)
        {
            manager.MainHandItem.OnUse -= (System.Action<Item, Transform, CharacterStats>)effect.VariableStore["OnUse"];
        }
    }
    #endregion
}
