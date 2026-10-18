using System.Collections.Generic;
using UnityEngine;

public static class Extend
{
    public static List<Transform> GetChildren(this Transform transform)
    {
        List<Transform> ret = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            ret.Add(transform.GetChild(i));
        }

        return ret;
    }
}
