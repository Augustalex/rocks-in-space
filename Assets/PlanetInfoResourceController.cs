using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfoResourceController : MonoBehaviour
{
    public RawImage resourceIcon;
    public TMP_Text resourceText;
    public RawImage trendIcon;

    public void Set(TinyPlanetResources.PlanetResourceType resourceType)
    {
        var uiAssetManager = UIAssetManager.Get();

        resourceIcon.texture = uiAssetManager.GetResourceTexture(resourceType);
    }

    public void Refresh(int amount, TinyPlanetResources.ResourceTrend trend)
    {
        var uiAssetManager = UIAssetManager.Get();

        resourceText.text = amount.ToString();
        if (trend == TinyPlanetResources.ResourceTrend.neutral)
        {
            trendIcon.gameObject.SetActive(false);
        }
        else
        {
            trendIcon.gameObject.SetActive(true);
            trendIcon.texture = uiAssetManager.GetTrendTexture(trend);
        }
    }
}