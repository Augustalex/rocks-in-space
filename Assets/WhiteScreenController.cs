using UnityEngine;
using UnityEngine.UI;

public class WhiteScreenController : MonoBehaviour
{
    public RawImage screen;

    private static WhiteScreenController _instance;
    private float _showUntil;
    private const float Length = 6f;

    public static WhiteScreenController Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Flash()
    {
        screen.gameObject.SetActive(true);
        screen.color = new Color(1f, 1f, 1f, 1f);
        _showUntil = Time.time + Length;
    }

    private void Update()
    {
        if (Time.time < _showUntil)
        {
            var timeLeft = _showUntil - Time.time;
            var timeProgressed = Length - timeLeft;
            var progress = Mathf.Clamp(timeProgressed / Length, 0f, 1f);
            if (progress < .3f)
            {
                screen.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                var left = Mathf.Clamp((progress - .3f) / .7f, 0f, 1f);
                screen.color = new Color(1f, 1f, 1f, 1f - left);
            }
        }
        else
        {
            if (screen.gameObject.activeSelf) screen.gameObject.SetActive(false);
        }
    }
}