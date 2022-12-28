using UnityEngine;

public class Flag : MonoBehaviour
{
    private CameraController _cameraController;

    void Start()
    {
        _cameraController = CameraController.Get();
        _cameraController.OnToggleZoom += OnToggleZoom;
        
        gameObject.SetActive(false);
    }

    private void OnToggleZoom(bool zoomedOut)
    {
        gameObject.SetActive(zoomedOut);
    }
}
