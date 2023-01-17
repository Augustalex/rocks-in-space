using UnityEngine;

public class PrefabTemplateLibrary : MonoBehaviour
{
    private static PrefabTemplateLibrary _instance;
    
    public GameObject mapPopup;
    public GameObject rockDebrisTemplate;
    public GameObject iceDebrisTemplate;
    public GameObject oreDebrisTemplate;
    public GameObject routeLineTemplate;

    public static PrefabTemplateLibrary Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
}
