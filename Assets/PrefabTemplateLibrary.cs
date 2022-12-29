using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTemplateLibrary : MonoBehaviour
{
    private static PrefabTemplateLibrary _instance;
    
    public GameObject rockDebrisTemplate;

    public static PrefabTemplateLibrary Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
}
