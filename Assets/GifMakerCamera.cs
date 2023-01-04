using System.Collections;
using UnityEngine;

public class GifMakerCamera : MonoBehaviour
{
    public int images;
    public GameObject[] targetTemplates;
    public bool dryRun = false;

    void Start()
    {
        StartCoroutine(TakeScreenShot());
    }

    IEnumerator TakeScreenShot()
    {
        foreach (var targetTemplate in targetTemplates)
        {
            var go = Instantiate(targetTemplate);
            go.name = targetTemplate.name;

            for (int i = 0; i < images; i++)
            {
                var nextThreshold = i * (360f / images);
                go.transform.rotation = Quaternion.Euler(0, nextThreshold, 0);
                yield return new WaitForEndOfFrame();
                if (!dryRun) ScreenCapture.CaptureScreenshot($"screenshots/{go.name}-{(i + 1)}.png");
            }

            Destroy(go);
        }
    }
}