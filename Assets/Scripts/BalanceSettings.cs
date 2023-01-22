using UnityEngine;

[CreateAssetMenu(fileName = "BalanceSettings", menuName = "BalanceSettings", order = 0)]
public class BalanceSettings : ScriptableObject
{
    [Header("Trading")] public float oreTradeAmount = 1f;
    public float ironTradeAmount = 1f;
    public float graphiteTradeAmount = 1f;
    public float copperTradeAmount = 1f;
    public float metalsTradeAmount = 1f;
    public float gadgetsTradeAmount = 1f;
    public float waterTradeAmount = 1f;
    public float refreshmentsTradeAmount = 1f;

    [Header("Digging")] public float rockDigTime = .5f;
    public float oreDigTime = 1f;

    [Header("Money")] public float startingCredits = 10000f;

    [Header("Build costs (build costs can be changed in each added interactor component on the camera")]
    [Header("Running costs (running costs can be changed in each building prefab")]

    [Header("Convoy")] 
    public float minTimeBetweenConvoySpawns = 60f;
    public float maxTimeBetweenConvoySpawns = 2 * 60f;
}