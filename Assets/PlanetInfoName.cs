using UnityEngine;

public class PlanetInfoName : MonoBehaviour
{
    public void StartRename()
    {
        DisplayController.Get().StartRenamingPlanet();
    }
}