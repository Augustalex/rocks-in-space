using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilledCargoSlotController : MonoBehaviour
{
    private TinyPlanetResources.PlanetResourceType _resource;

    public RawImage icon;

    public TMP_Text title;
    public TMP_Text amountText;

    public Button unloadOne;
    public Button unloadTen;
    public Button unloadAll;
    public Button loadOne;
    public Button loadTen;
    public Button loadAll;

    private float _amount = 0;
    private const float MaxLoad = 100f;

    public event Action UnloadedAll;

    private void Start()
    {
        unloadOne.onClick.AddListener(() => Unload(1));
        unloadTen.onClick.AddListener(() => Unload(10));
        unloadAll.onClick.AddListener(() => Unload(-1));

        loadOne.onClick.AddListener(() => Load(1));
        loadTen.onClick.AddListener(() => Load(10));
        loadAll.onClick.AddListener(() => Load(-1));
    }

    public void SetResource(TinyPlanetResources.PlanetResourceType resource)
    {
        _resource = resource;

        icon.texture = UIAssetManager.Get().GetResourceTexture(resource);
        title.text = StringUtils.Capitalized(TinyPlanetResources.ResourceNameOnly(resource));
        UpdateAmountText();
    }

    private void Unload(int amount) // -1 is all
    {
        if (_amount == 0)
        {
            UnloadedAll?.Invoke();
        }
        else
        {
            var planet = CurrentPlanetController.Get().CurrentPlanet();
            if (!planet) return;

            var resources = planet.GetResources();

            var toUnload = amount < 0 ? _amount : Mathf.Min(_amount, amount);

            resources.AddResource(_resource, toUnload);
            _amount -= toUnload;

            if (_amount == 0)
            {
                UnloadedAll?.Invoke();
            }
        }
    }

    public void Load(int amount) // -1 is all
    {
        var planet = CurrentPlanetController.Get().CurrentPlanet();
        if (!planet) return;

        var planetResources = planet.GetResources();
        var currentAmount = planetResources.GetResource(_resource);
        var toLoad = amount < 0 ? currentAmount : Mathf.Min(currentAmount, amount, MaxLoad - _amount);
        planetResources.RemoveResource(_resource, toLoad);
        _amount += toLoad;

        UpdateAmountText();
    }

    public void Conjure(int amount)
    {
        _amount += amount;
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        var baseText = Mathf.FloorToInt(_amount).ToString();

        var reachedMaxLoad = Math.Abs(_amount - MaxLoad) < .5f;
        if (reachedMaxLoad)
        {
            baseText += " (MAX LOAD)";
        }

        amountText.text = baseText;

        loadOne.interactable = !reachedMaxLoad;
        loadTen.interactable = !reachedMaxLoad;
        loadAll.interactable = !reachedMaxLoad;
    }
}