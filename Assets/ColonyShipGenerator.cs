using UnityEngine;

public class ColonyShipGenerator : MonoBehaviour
{
    public GameObject colonyShipTemplate;
    private bool _hasSpawnedFirstShip;
    private float _spawnShipAt;
    private GameObject _ship;

    private int _level = 1;

    private void Update()
    {
        if (!_hasSpawnedFirstShip)
        {
            if (_spawnShipAt > 0f)
            {
                if (Time.time > _spawnShipAt)
                {
                    SpawnShip();
                    _hasSpawnedFirstShip = true;
                    _spawnShipAt = -1f;
                }
            }
            else if (ProgressManager.Get().ColonyBasicsProductionUnlocked())
            {
                _spawnShipAt = Time.time + 10f;
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

    private void SpawnShip()
    {
        _ship = Instantiate(colonyShipTemplate, transform, false);
        _ship.GetComponent<ColonyShip>().SetLevel(_level++);
    }
}