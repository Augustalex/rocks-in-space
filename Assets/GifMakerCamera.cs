using System.Collections;
using System.IO;
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
                if (!dryRun)
                {
                    var texture = ScreenCapture.CaptureScreenshotAsTexture();
                    for (int y = 0; y < texture.height; y++)
                    {
                        for (int x = 0; x < texture.width; x++)
                        {
                            Color color = texture.GetPixel(x, y);
                            if (color == Color.black)
                            {
                                texture.SetPixel(x, y, Color.clear);
                            }
                            else
                            {
                                // texture.SetPixel(x, y, color);
                            }
                        }
                    }

                    texture.Apply();

                    byte[] bytes = texture.EncodeToPNG();
                    var path = $"/UI/ToolUI/buildings/gifs/24/transparent/{go.name}-{(i + 1)}.png";
                    var fullPath = Application.dataPath + path;
                    File.WriteAllBytes(fullPath, bytes);

                    // var image = ScreenCapture.CaptureScreenshot(path);
                }
            }

            Destroy(go);
        }
    }
}