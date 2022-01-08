using System.Collections.Generic;
using UnityEngine;

public class SpacerCountDisplay : MonoBehaviour
{
    public GameObject spacerToken;

    private List<GameObject> _spaceTokens = new();
    
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
        var start = middle - Vector3.right * ((spacerInfoCount / 2) * increment);

        for (int i = 0; i < spacerInfoCount; i++)
        {
            var token = Instantiate(spacerToken);
            token.transform.SetParent(transform);
            
            var rectTransform = token.GetComponent<RectTransform>();

            var offset = -Vector3.right * (increment) * spacerInfoCount / 2;

            rectTransform.anchoredPosition = middle + Vector3.right * (i * increment) + offset;
            
            _spaceTokens.Add(token);
        }
    }
}
