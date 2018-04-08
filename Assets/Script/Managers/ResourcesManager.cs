using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "Single Instance/Resources")]
public class ResourcesManager : ScriptableObject
{

    public Projectile[] projectiles;
    public Dictionary<string, int> p_dict = new Dictionary<string, int>();

    public WeaponAbility[] weaponAbilities;
    public Dictionary<string, int> w_dict = new Dictionary<string, int>();

    public Character[] frames;
    public Dictionary<string, int> f_dict = new Dictionary<string, int>();

    public void Init()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            Projectile p = projectiles[i];
            if (p_dict.ContainsKey(p.projectileId))
            {

            }
            else
            {
                ClientScene.RegisterPrefab(p.projectilePrefab);
                p_dict.Add(p.projectileId, i);
            }
        }

        for (int i = 0; i < weaponAbilities.Length; i++)
        {
            WeaponAbility wa = weaponAbilities[i];
            if (w_dict.ContainsKey(wa.aID))
            {

            }
            else
            {
                w_dict.Add(wa.aID, i);
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
                ClientScene.RegisterPrefab(frame.characterPrefab);
                f_dict.Add(frame.characterID, i);
            }
        }
    }

    public Projectile GetProjectile(string id)
    {
        Projectile retVal = null;
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
            Debug.Log("FOUND " + index);
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
            //Debug.Log("FOUND");
        }

        //foreach(string n in f_dict.Keys)
        //{
        //    Debug.Log(n);
        //}

        return retVal;
    }
}