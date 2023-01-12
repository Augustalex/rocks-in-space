using System.Collections;
using GameNotifications;
using UnityEngine;

public class NudgeNotifications : MonoBehaviour
{
    private bool _hasNotifiedOnTrading;
    private const float LoopDelay = 2f;

    void Start()
    {
        StartCoroutine(UpdateLocksLoop());
    }

    private IEnumerator UpdateLocksLoop()
    {
        while (!_hasNotifiedOnTrading)
        {
            if (!_hasNotifiedOnTrading && ProgressManager.Get().CanTrade())
            {
                StartCoroutine(SendTradeNotificationsSoon());
                _hasNotifiedOnTrading = true;
            }

            yield return new WaitForSeconds(LoopDelay);
        }
    }

    private IEnumerator SendTradeNotificationsSoon()
    {
        yield return new WaitForSeconds(2f);
        
        Notifications.Get().Send(new TextNotification
        {
            message =
                "With 2 Beacons active you can now setup trade routes in the Map view!"
        });
    }
}