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
                    "Standard issue ship for new colony managers. Lasers, 3D printers, a map & a trunk, it has it all.",
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
            DisplayController.Get().LastGiftActivated();
            mode = StartMode.AcceptedAllGifts;

            Finished();
        }
    }

    public void Finished()
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(5f);

            var notification = new TextNotification
            {
                Message = "You are now ready to find a suitable asteroid for your first colony!",
            };
            Notifications.Get().Send(notification);
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
            });
        }
    }
}