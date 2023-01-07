using System;
using UnityEngine;
using UnityEngine.UI;

public class GifDisplay : MonoBehaviour
{
    [SerializeField] public Texture[] frames;

    [SerializeField] float framesPerSecond = 10f;
    private RawImage _rawImage;
    private static readonly int ShadeId = Shader.PropertyToID("_Shade");
    private Material _material;

    private void Awake()
    {
        var rawImage = gameObject.GetComponent<RawImage>();
        _rawImage = rawImage;

        _material = Instantiate(rawImage.material);
        _rawImage.material = _material;
    }

    void Update()
    {
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        if (frames[index] != null)
        {
            _rawImage.texture = frames[index];
        }
    }

    public void RemoveShade()
    {
        _material.SetInt(ShadeId, 0);
    }

    public void Shade()
    {
        _material.SetInt(ShadeId, 1);
    }
}