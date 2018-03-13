using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Single Instance/Resources")]
public class ResourcesManager : ScriptableObject
{

    public GameObject[] gameObjects;
    public Dictionary<string, int> dict = new Dictionary<string, int>();

    public void Init()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Bullet pg = gameObjects[i].GetComponent<Bullet>();
            if (dict.ContainsKey(pg.bulletId))
            {

            }
            else
            {
                dict.Add(pg.bulletId, i);
            }
        }
    }

    public GameObject GetWeapon(string id)
    {
        GameObject retVal = null;
        int index = -1;
        if(dict.TryGetValue(id, out index))
        {
            retVal = gameObjects[index];
        }

        return retVal;
    }
}