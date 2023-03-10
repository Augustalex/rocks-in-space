using GameNotifications;
using UnityEngine;

public class ColonyShip : MonoBehaviour
{
    public void MouseDown()
    {
        StartingSequence.Get().EnteringShip();
    }
}