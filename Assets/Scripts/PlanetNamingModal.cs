using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetNamingModal : MonoBehaviour
{
    private PlanetNameInput _nameInput;
    private static PlanetNamingModal _instance;

    public GameObject modalRoot;
    public event Action<string> OnRename;

    public static PlanetNamingModal Get()
    {
        return _instance;
    }

    void Awake()
    {
        _nameInput = GetComponentInChildren<PlanetNameInput>();
        _instance = this;

        _nameInput.OnSubmit += FinishRenaming;

        modalRoot.SetActive(false);
    }

    public void StartRenaming(string currentName = "")
    {
        if (currentName.ToLower() == "unnamed" || currentName.ToLower() == "unknown") currentName = "";

        modalRoot.SetActive(true);
        _nameInput.UpdateText(currentName);

        if (GameSystemManager.Get().UsingTouch())
        {
            var firstParts = new[]
            {
                "ALPHA",
                "BETA",
                "CHARLIE",
                "DELTA",
                "BOB"
            };
            var secondPart = Random.Range(1, 1337).ToString();

            _nameInput.UpdateText($"{firstParts[Random.Range(0, firstParts.Length)]} {secondPart}");
            FinishRenaming();
        }
    }

    public void FinishRenaming()
    {
        var newName = _nameInput.GetText();
        OnRename?.Invoke(newName);
        modalRoot.SetActive(false);
    }
}