using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Single Instance/FrameResources")]
public class FrameResourcesManager : ScriptableObject
{

    public Character[] frames;
    public Dictionary<string, int> dict = new Dictionary<string, int>();

    public void Init()
    {
        for (int i = 0; i < frames.Length; i++)
        {
            Character frame = frames[i];
            if (dict.ContainsKey(frame.characterID))
            {

            }
            else
            {
                dict.Add(frame.characterID, i);
            }
        }
    }

    public Character GetCharacter(string id)
    {
        Character retVal = null;
        int index = -1;
        if (dict.TryGetValue(id, out index))
        {
            retVal = frames[index];
        }

        return retVal;
    }
}