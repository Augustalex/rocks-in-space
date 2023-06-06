using UnityEngine;
using UnityEngine.Serialization;

public class PrefabTemplateLibrary : MonoBehaviour
{
    private static PrefabTemplateLibrary _instance;
    
    public GameObject mapPopup;
    public GameObject dangeroniumOre;
    public GameObject ironOre;
    public GameObject ironOreDebrisTemplate;
    public GameObject graphiteOre;
    public GameObject graphiteOreDebrisTemplate;
    public GameObject copperOre;
    public GameObject copperOreDebrisTemplate;
    public GameObject rockDebrisTemplate;
    public GameObject iceDebrisTemplate;
    public GameObject iceResourceDebrisTemplate;
    [FormerlySerializedAs("oreDebrisTemplate")] public GameObject oreResourceDebrisTemplate;
    public GameObject routeLineTemplate;
    public GameObject corruptionParticlesTemplate;

    public Material corruptedRockMaterial;
    
    public static PrefabTemplateLibrary Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
}
