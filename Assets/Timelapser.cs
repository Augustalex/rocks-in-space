using System.Collections;
using UnityEngine;

public class Timelapser : MonoBehaviour
{
    public int maxImages = 100;
    public float delay = 10f;

    private int _index = 0;
    private bool _started;

    private void Update()
    {
        if (!_started)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _started = true;
                StartCoroutine(Capture());
            }
        }
    }

    IEnumerator Capture()
    {
        while (gameObject != null)
        {
            if (_index >= maxImages) yield break;

            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot($"screenshots/timelapse-{_index++}.png");
            yield return new WaitForSeconds(delay);
        }
    }
}