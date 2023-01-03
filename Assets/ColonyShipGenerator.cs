using UnityEngine;

public class ColonyShipGenerator : MonoBehaviour
{
    public GameObject colonyShipTemplate;
    private bool _hasSpawnedFirstShip;
    private float _spawnShipAt;
    private GameObject _ship;

    void Start()
    {
        CameraController.Get().OnToggleZoom += OnToggleZoom;
    }

    private void Update()
    {
        if (!_hasSpawnedFirstShip)
        {
            if (_spawnShipAt > 0f)
            {
                if (Time.time > _spawnShipAt && IsCenterInView())
                {
                    SpawnShip();
                    _hasSpawnedFirstShip = true;
                    _spawnShipAt = -1f;
                }
            }
        }
        else
        {
            if (_spawnShipAt > 0f)
            {
                if (Time.time > _spawnShipAt)
                {
                    SpawnShip();
                    _spawnShipAt = -1f;
                }
            }
            else
            {
                if (_ship == null)
                {
                    var balanceSettings = SettingsManager.Get().balanceSettings;
                    var timeToWait = Random.Range(balanceSettings.minTimeBetweenConvoySpawns,
                        balanceSettings.maxTimeBetweenConvoySpawns);
                    _spawnShipAt = Time.time + timeToWait;
                }
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
        _ship = Instantiate(colonyShipTemplate, transform, false);
    }
}