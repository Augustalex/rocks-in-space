using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    private static TooltipController _instance;
    private float _showedAt;
    private TMP_Text _text;

    public static TooltipController Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void Show(string text, Vector3 mousePosition)
    {
        transform.position = mousePosition;
        _text.text = text;

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}