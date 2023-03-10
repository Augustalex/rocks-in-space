using System.Collections;
using System.Collections.Generic;
using GameNotifications;
using UnityEngine;

public class StartingSequence : MonoBehaviour
{
    private static StartingSequence _instance;

    public static StartingSequence Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public StartMode mode = StartMode.NotStarted;
    private bool _shownMoneyHint;

    public enum StartMode
    {
        NotStarted,
        Opening,
        EnteringShip,
        AcceptingGifts,
        AcceptedAllGifts,
        Finished
    }

    void Start()
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(.1f);

            if (mode == StartMode.NotStarted)
            {
                Opening();
            }
        }
    }

    public void Opening()
    {
        if (mode == StartMode.NotStarted)
        {
            CameraController.Get().FocusOnStartingShip();
            mode = StartMode.Opening;
        }
    }

    public void EnteringShip()
    {
        if (mode == StartMode.Opening)
        {
            var cameraController = CameraController.Get();
            cameraController.EnterShip();

            Notifications.Get().Send(new TextNotification
            {
                NotificationType = NotificationTypes.Positive,
                Message =
                    "You are now inside a standard issue ship for new colony managers. Mining lasers, 3D printers, it has all you need to get started.",
                TimeoutOverride = 20f,
            });

            mode = StartMode.EnteringShip;
        }
    }

    public void FinishedEnteringShip()
    {
        if (mode == StartMode.EnteringShip)
        {
            var notification = new TextNotification
            {
                Message = "Message from management: \"We left you some things to get started with.\"",
                TimeoutOverride = 20f,
            };
            Notifications.Get().Send(notification);

            DisplayController.Get().SetEnteredShip();

            mode = StartMode.AcceptingGifts;
        }
    }

    public void AcceptedAllGifts()
    {
        if (mode == StartMode.AcceptingGifts)
        {
            mode = StartMode.AcceptedAllGifts;

            StartCoroutine(DoSoon());

            IEnumerator DoSoon()
            {
                yield return new WaitForSeconds(5f);

                var notification = new TextNotification
                {
                    Message = "You are now ready to find a suitable asteroid for your first colony! Open the map.",
                    TimeoutOverride = 10f
                };
                Notifications.Get().Send(notification);
                
                yield return new WaitForSeconds(1f);
                
                DisplayController.Get().ShowMapAndInventory();

                CameraController.Get().OnToggleZoom += (zoomedOut) =>
                {
                    notification.Accept();
                    
                    if (mode == StartMode.AcceptedAllGifts && zoomedOut)
                    {
                        Finished();
                    }
                };
            }
        }
    }

    public void Finished()
    {
        if (mode == StartMode.AcceptedAllGifts)
        {
            DisplayController.Get().SetToStaticMode();

            var notification = new TextNotification
            {
                Message =
                    $"There are different types of planet. Try finding one with lots of {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Graphite)}, that can be a good start. Good luck!",
            };
            Notifications.Get().Send(notification);

            mode = StartMode.Finished;
        }
    }

    public void GiftMoneyHint()
    {
        if (!_shownMoneyHint)
        {
            _shownMoneyHint = true;
            Notifications.Get().Send(new TextNotification
            {
                Message =
                    $"The boxes contained some {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Cash)}",
                TimeoutOverride = 10f
            });
        }
    }
}