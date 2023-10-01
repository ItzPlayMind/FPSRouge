using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DescriptionCreator
{
    private static string ERROR_MESSAGE = "NOT_FOUND";

    static Dictionary<string, string> wordReplacements = new Dictionary<string, string>()
    {
        {"Physical","<color=#FFEA00>Physical</color>" },
        {"Magical","<color=#EB16FA>Magical</color>" },
        {"Fire","<color=#F06723>Fire</color>" },
        {"Lightning","<color=#95EAF9>Lightning</color>" },
        {"Earth","<color=#3C7F07>Earth</color>" },
        {"Blunt","<color=#9294B1>Blunt</color>" },
        {"Slash","<color=#DC1310>Slash</color>" },
        {"Pierce","<color=#678947>Pierce</color>" },
        {"Melee","<color=#B4C288>Melee</color>" },
        {"Ranged","<color=#9088C2>Ranged</color>" },
    };

    public static string Generate(string description, params object[] objs)
    {
        //Replace all Words with Word Replacments
        string s = "";
        var splits = description.Split(" ");
        foreach (var item in splits)
        {
            if (wordReplacements.ContainsKey(item))
                s += wordReplacements[item];
            else
                s += item;
            s += " ";
        }
        s = s.Trim();

        //Set Values given by ($+Index)
        string result = "";
        splits = s.Split("$");
        foreach (var item in splits)
        {
            int value = 0;
            if (int.TryParse(item.Substring(0, 1), out value))
            {
                if (value >= objs.Length)
                    result += ERROR_MESSAGE;
                else
                    result += $"<color=green>{objs[value]}</color>";
                result += item.Substring(1);
            }
            else
                result += item;
        }
        return result;
    }
}
