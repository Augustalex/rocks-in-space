using UnityEngine;

public class ColonyShipGenerator : MonoBehaviour
{
    public GameObject colonyShipTemplate;
    private bool _hasSpawnedFirstShip;
    private float _spawnShipAt;

    void Start()
    {
        CameraController.Get().OnToggleZoom += OnToggleZoom;
    }

    private void Update()
    {
        if (_spawnShipAt > 0f && Time.time > _spawnShipAt)
        {
            if (IsCenterInView())
            {
                SpawnShip();
                _hasSpawnedFirstShip = true;
                _spawnShipAt = -1f;
            }
        }
    }

    private bool IsCenterInView()
    {
        return true;
    }

    private void OnToggleZoom(bool zoomedOut)
    {
        if (zoomedOut)
        {
            if (!_hasSpawnedFirstShip)
            {
                _spawnShipAt = Time.time + 3f;
            }
        }
        else
        {
            _spawnShipAt = -1f;
        }
    }

    private void SpawnShip()
    {
        Instantiate(colonyShipTemplate, transform, false);
    }
}