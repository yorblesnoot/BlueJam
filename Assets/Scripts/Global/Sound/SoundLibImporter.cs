using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibImporter : MonoBehaviour
{
    [SerializeField] SoundLibrary library;
    private void Awake()
    {
        if (SoundManager.Library == null)
        {
            library.Initialize();
            SoundManager.Library = library;
        }
    }
}