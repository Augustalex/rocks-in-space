using TMPro;
using UnityEngine;

public class ColonistsCounter : MonoBehaviour
{
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    public void Set(Convoy convoy)
    {
        var inhabitants = convoy.Colonists;
        _text.text = inhabitants > 0 ? $"REQUIRED HOUSING: {inhabitants}\nCREDIT REWARD: {convoy.CashReward}c" : "";
    }
}