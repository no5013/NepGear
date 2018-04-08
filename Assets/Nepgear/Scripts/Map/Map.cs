using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Map")]
public class Map : ScriptableObject
{
    public string id;
    public string mapName;
    public SceneAsset mapScene;
}
