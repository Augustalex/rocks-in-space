using UnityEngine;

[CreateAssetMenu(fileName = "MiscSettings", menuName = "MiscSettings", order = 0)]
public class MiscSettings : ScriptableObject
{
    public int inactiveRouteFramesThreshold = 30;
}