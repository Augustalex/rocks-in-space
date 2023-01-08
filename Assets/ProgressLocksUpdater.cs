using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProgressLocksUpdater : MonoBehaviour
{
    private List<ProgressLock> _progressLocks;
    private const float LoopDelay = 2f;

    void Start()
    {
        _progressLocks = BottomBarController.Get().GetProgressLocks().ToList();

        StartCoroutine(UpdateLocksLoop());
    }

    private IEnumerator UpdateLocksLoop()
    {
        while (_progressLocks.Count > 0)
        {
            var toRemove = new List<ProgressLock>();
            foreach (var progressLock in _progressLocks)
            {
                yield return new WaitForNextFrameUnit();

                if (progressLock.Hidden()) toRemove.Add(progressLock);
                else
                {
                    progressLock.UpdateLocks();
                }
            }

            foreach (var progressLock in toRemove)
            {
                _progressLocks.Remove(progressLock);
            }

            yield return new WaitForSeconds(LoopDelay);
        }

        Debug.Log("OUT OF LOOP! YAY!");
    }
}