using System.Collections.Generic;
using UnityEngine;

public class SpacerCountDisplay : MonoBehaviour
{
    public GameObject spacerToken;

    private List<GameObject> _spaceTokens = new List<GameObject>();
    private bool _hidden;

    void Start()
    {
        DisplayCount(0);        
    }

    public void DisplayCount(int spacerInfoCount)
    {
        foreach (var spaceToken in _spaceTokens)
        {
            Destroy(spaceToken);
        }

        _spaceTokens = new List<GameObject>();

        var middle = Vector3.zero;
        var increment = 25f;

        for (int i = 0; i < spacerInfoCount; i++)
        {
            
            var token = Instantiate(spacerToken);
            token.transform.SetParent(transform);
            
            var rectTransform = token.GetComponent<RectTransform>();

            var offset = -Vector3.right * ((increment) * (spacerInfoCount));
            
            rectTransform.anchoredPosition = middle + Vector3.right * (i * increment) + (offset * .5f);
            
            _spaceTokens.Add(token);
        }
    }

    public void Hide()
    {
        _hidden = true;
    }

    public void Show()
    {
        _hidden = false;
    }
}
