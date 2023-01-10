using System.Collections;
using UnityEngine;

public class Snapshooter : MonoBehaviour
{
    private int _index = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(Capture());
        }
    }

    IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot($"screenshots/snapshot-{_index++}.png");
    }
}