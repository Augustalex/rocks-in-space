using UnityEngine;

public class StartingShip : MonoBehaviour
{
    private ColonyShip _convoyBeacon;
    private static readonly int Hiding = Animator.StringToHash("Hiding");

    public GameObject modelRoot;
    
    void Start()
    {
        _convoyBeacon = GetComponentInChildren<ColonyShip>();
    }

    public ColonyShip GetConvoyBeacon()
    {
        return _convoyBeacon;
    }

    public void StartHiding()
    {
        GetComponent<Animator>().SetBool(Hiding, true);
    }

    public void HideModel()
    {
        modelRoot.SetActive(false);
        GetComponent<Animator>().enabled = false;
    }
}
