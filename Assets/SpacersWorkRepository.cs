using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpacersWorkRepository : MonoBehaviour
{
    private List<SpacerMineralRelation> _mineralsBeingGatheredByWhatSpacer = new List<SpacerMineralRelation>();
    private readonly List<GameObject> _mineralsToGather = new List<GameObject>();
    private List<GameObject> _registeredSpacers = new List<GameObject>();

    void Update()
    {
        if (_mineralsToGather.Count > 0)
        {
            var mineralsToDelist = new List<GameObject>();
            foreach (var mineral in _mineralsToGather)
            {
                var worker = _registeredSpacers
                    .FirstOrDefault(spacer => spacer != null && spacer.GetComponent<SpacerWorkManager>().CanMine());

                if (worker != null && mineral != null)
                {
                    worker.GetComponent<SpacerWorkManager>().Mine(mineral);
                    _mineralsBeingGatheredByWhatSpacer.Add(
                        new SpacerMineralRelation
                        {
                            mineral = mineral,
                            spacer = worker
                        });
                    mineralsToDelist.Add(mineral);
                }
            }

            _mineralsToGather.RemoveAll(mineral => mineral == null || mineralsToDelist.Contains(mineral));
        }

        if (Random.value < .001)
        {
            _registeredSpacers = _registeredSpacers.Where(spacer => spacer != null).ToList();
        }

        if (Random.value < .01)
        {
            _mineralsBeingGatheredByWhatSpacer = _mineralsBeingGatheredByWhatSpacer
                .Where(mineralAndSpacer => mineralAndSpacer.MineralStillExistst()).ToList();

            var mineralsToReassign = new List<GameObject>();
            foreach (var spacerMineralRelation in _mineralsBeingGatheredByWhatSpacer)
            {
                if (spacerMineralRelation.SpacerIsDeadOrMissing())
                {
                    mineralsToReassign.Add(spacerMineralRelation.mineral);
                }
            }
            
            _mineralsBeingGatheredByWhatSpacer.RemoveAll(spacerMineralRelation => mineralsToReassign.Contains(spacerMineralRelation.mineral));
            _mineralsToGather.AddRange(mineralsToReassign);
        }
    }

    public void RegisterSpacer(GameObject spacer)
    {
        _registeredSpacers.Add(spacer);
    }

    public void RegisterMineralToMine(GameObject floatingMinerals)
    {
        _mineralsToGather.Add(floatingMinerals);
    }
}

public class SpacerMineralRelation
{
    public GameObject spacer;
    public GameObject mineral;

    public bool SpacerIsDeadOrMissing()
    {
        return spacer == null;
    }

    public bool MineralStillExistst()
    {
        return mineral == null;
    }
}