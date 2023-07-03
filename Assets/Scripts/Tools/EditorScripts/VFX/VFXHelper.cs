using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class VFXHelper
{
#if UNITY_EDITOR

    public static string[] AllVFXNames()
    {
        List<string> paths = Directory.GetFiles(Application.dataPath + "/Resources/VFX").ToList();
        paths = paths.Select(x => Path.GetFileName(x)).ToList();
        paths = paths.Where(x => !x.Contains("meta")).ToList();
        paths = paths.Select(x => x.Replace(".prefab","")).ToList();
        paths.Insert(0, "none");
        return paths.ToArray();
    }

#endif
}
