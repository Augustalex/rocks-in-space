using System;
using UnityEngine;
using UnityEngine.UI;

public class GifDisplay : MonoBehaviour
{
    [SerializeField] public Texture[] frames;

    [SerializeField] float framesPerSecond = 10f;
    private RawImage _rawImage;

    private void Awake()
    {
        var rawImage = gameObject.GetComponent<RawImage>();
        _rawImage = rawImage;
    }

    void Update()
    {
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        if (frames[index] != null)
        {
            _rawImage.texture = frames[index];
        }
    }
}