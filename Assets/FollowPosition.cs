using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    
    void Update()
    {
        transform.position = CurrentPlanetController.Get().CurrentPlanet().GetCenter();
    }
}
