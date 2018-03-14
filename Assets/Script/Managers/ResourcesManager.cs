using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "Single Instance/Resources")]
public class ResourcesManager : ScriptableObject
{

    public GameObject[] projectiles;
    public Dictionary<string, int> p_dict = new Dictionary<string, int>();

    public WeaponAbility[] weaponAbilities;
    public Dictionary<string, int> w_dict = new Dictionary<string, int>();

    public Character[] frames;
    public Dictionary<string, int> f_dict = new Dictionary<string, int>();

    public void Init()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            Bullet pg = projectiles[i].GetComponent<Bullet>();
            if (p_dict.ContainsKey(pg.bulletId))
            {

            }
            else
            {
                ClientScene.RegisterPrefab(projectiles[i]);
                p_dict.Add(pg.bulletId, i);
            }
        }

        for (int i = 0; i < projectiles.Length; i++)
        {
            WeaponAbility wa = weaponAbilities[i];
            if (p_dict.ContainsKey(wa.aID))
            {

            }
            else
            {
                p_dict.Add(wa.aID, i);
            }
        }

        for (int i = 0; i < frames.Length; i++)
        {
            Character frame = frames[i];
            if (f_dict.ContainsKey(frame.characterID))
            {

            }
            else
            {
                f_dict.Add(frame.characterID, i);
            }
        }
    }

    public GameObject GetProjectile(string id)
    {
        GameObject retVal = null;
        int index = -1;
        if(p_dict.TryGetValue(id, out index))
        {
            retVal = projectiles[index];
        }

        return retVal;
    }

    public WeaponAbility GetWeapon(string id)
    {
        WeaponAbility retVal = null;
        int index = -1;
        if (w_dict.TryGetValue(id, out index))
        {
            retVal = weaponAbilities[index];
        }

        return retVal;
    }

    public Character GetCharacter(string id)
    {
        Character retVal = null;
        int index = -1;
        if (f_dict.TryGetValue(id, out index))
        {
            retVal = frames[index];
        }

        return retVal;
    }
}