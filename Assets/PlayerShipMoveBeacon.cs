using UnityEngine;

public class PlayerShipMoveBeacon : MonoBehaviour
{
    private PlayerShipManager _playerShipManager;

    [SerializeField] private GameObject mesh;

    private static PlayerShipMoveBeacon _instance;

    public static PlayerShipMoveBeacon Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _playerShipManager = PlayerShipManager.Get();

        UpdateVisibility();
        CameraController.Get().OnToggleZoom += (_) => UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        var zoomedOut = CameraController.Get().IsZoomedOut();
        if (zoomedOut) Show();
        else Hide();
    }

    private void Update()
    {
        var mover = _playerShipManager.ShipMover();
        var state = mover.GetState();

        if (state == PlayerShipMover.ShipState.Moving)
        {
            mover.Progress();
            mesh.transform.position = mover.ReadPosition();
        }
    }

    public void Hide()
    {
        mesh.SetActive(false);
    }

    public void Show()
    {
        mesh.SetActive(true);
    }
}