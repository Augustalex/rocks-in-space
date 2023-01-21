using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    private string _text;
    private TMP_Text _textComponent;

    public RawImage trendIcon;

    void Start()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        var globalResources = GlobalResources.Get();
        var cash = Math.Floor(globalResources.GetCash());
        _textComponent.text = $"{(int)cash}";
        var resourceTrend = globalResources.GetTrend();
        if (resourceTrend == TinyPlanetResources.ResourceTrend.Neutral)
        {
            trendIcon.gameObject.SetActive(false);
        }
        else
        {
            trendIcon.gameObject.SetActive(true);
            trendIcon.texture = UIAssetManager.Get().GetTrendTexture(resourceTrend);
        }
    }
}