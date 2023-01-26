using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConvoyCard : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text rewards;
    public TMP_Text requirements;
    public Button selectButton;
    
    public void Set(Convoy convoy)
    {
        title.text = "Colonist convoy";
        rewards.text = $"Reward: {convoy.CashReward}c";
        requirements.text = $"Requirement: Housing for {convoy.Colonists}";
    }
}
