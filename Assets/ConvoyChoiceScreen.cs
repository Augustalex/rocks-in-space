using System;
using System.Collections.Generic;
using UnityEngine;

public class ConvoyChoiceScreen : MonoBehaviour
{
    public GameObject convoyCardTemplate;

    public GameObject convoyCardsContainer;

    private static ConvoyChoiceScreen _instance;
    private bool _showing;
    private readonly List<GameObject> _cards = new();

    public static ConvoyChoiceScreen Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        Hide();
    }

    public void Set(Convoy[] convoys) // Note: Only supports 3 convoys
    {
        Show();

        foreach (var convoy in convoys)
        {
            var card = Instantiate(convoyCardTemplate, convoyCardsContainer.transform, true);
            var convoyCard = card.GetComponent<ConvoyCard>();
            convoyCard.selectButton.onClick.AddListener(() => SelectConvoy(convoy));
            convoyCard.Set(convoy);
            
            _cards.Add(card);
        }
    }

    private void SelectConvoy(Convoy convoy)
    {
        Hide();
        ColonistManager.Get().SendConvoy(convoy);
    }

    public bool Visible()
    {
        return gameObject.activeSelf;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        foreach (var card in _cards)
        {
            Destroy(card);
        }
        _cards.Clear();
        
        gameObject.SetActive(false);
    }
}