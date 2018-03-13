using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Single Instance/WeaponResources")]
public class WeaponResourcesManager : ScriptableObject
{

    public WeaponAbility[] weaponAbilities;
    public Dictionary<string, int> dict = new Dictionary<string, int>();

    public void Init()
    {
        for (int i = 0; i < weaponAbilities.Length; i++)
        {
            WeaponAbility wa = weaponAbilities[i];
            if (dict.ContainsKey(wa.aID))
            {

            }
            else
            {
                dict.Add(wa.aID, i);
            }
        }
    }

    public WeaponAbility GetWeapon(string id)
    {
        WeaponAbility retVal = null;
        int index = -1;
        if (dict.TryGetValue(id, out index))
        {
            retVal = weaponAbilities[index];
        }

        return retVal;
    }
}