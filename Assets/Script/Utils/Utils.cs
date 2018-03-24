using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    
    public static Transform[] gameObjectsToTransforms(GameObject[] objects)
    {
        Transform[] transforms = new Transform[objects.Length];

        for (int i = 0; i < transforms.Length; ++i)
            transforms[i] = objects[i].transform;

        return transforms;
    }
}
