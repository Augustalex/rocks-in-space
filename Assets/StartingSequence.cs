using System.Collections;
using GameNotifications;
using UnityEngine;

public class StartingSequence : MonoBehaviour
{
    private static StartingSequence _instance;

    private static readonly Vector3 StartingPosition = new(-500f, -500f, -500f);

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
        FinishedOpening,
        EnteringShip,
        AcceptingGifts,
        AcceptedAllGifts,
        ViewedMap,
        ControlsInstructions,
        Finished,
    }

    void Start()
    {
        PlayerShipManager.Get().ShipMover().PutInStartingPosition(StartingPosition);

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
        if (mode == StartMode.FinishedOpening)
        {
            var cameraController = CameraController.Get();
            cameraController.EnterShip();

            StartingInstructions.Get().Clear();

            // Notifications.Get().Send(new TextNotification
            // {
            //     NotificationType = NotificationTypes.Positive,
            //     Message =
            //         "You are now inside a standard issue ship for new colony managers. Mining lasers, 3D printers, it has all you need to get started.",
            //     TimeoutOverride = 20f,
            // });

            mode = StartMode.EnteringShip;
        }
    }

    public void FinishedEnteringShip()
    {
        if (mode == StartMode.EnteringShip)
        {
            StartCoroutine(DoSoon());

            IEnumerator DoSoon()
            {
                DisplayController.Get().SetEnteredShip();
                mode = StartMode.AcceptingGifts;

                yield return new WaitForSeconds(1f);
                NotificationSounds.Get().Play(NotificationTypes.Informative);
                StartingInstructions.Get().Print("What you are seeing are your ship controls");
                yield return new WaitForSeconds(6f);

                // Player could have already accepted all gifts at this point
                if (mode == StartMode.AcceptingGifts)
                {
                    NotificationSounds.Get().Play(NotificationTypes.Informative);
                    StartingInstructions.Get().Print("More controls will become available soon");
                    yield return new WaitForSeconds(4f);
                }

                // Player could have already accepted all gifts at this point
                if (mode == StartMode.AcceptingGifts)
                {
                    NotificationSounds.Get().Play(NotificationTypes.Informative);
                    StartingInstructions.Get()
                        .Print("Try picking up these crates, they contain some useful resources.");
                    yield return new WaitForSeconds(8f);
                    StartingInstructions.Get().Clear();
                }

                // var notification = new TextNotification
                // {
                //     Message = "Message from management: \"We left you some things to get started with.\"",
                //     TimeoutOverride = 20f,
                // };
                // Notifications.Get().Send(notification);
            }
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
                yield return new WaitForSeconds(3f);
                NotificationSounds.Get().Play(NotificationTypes.Positive);
                StartingInstructions.Get().Print("Good! You are now ready to explore the sector. Open the map.");

                // var notification = new TextNotification
                // {
                //     Message = "You are now ready to find a suitable asteroid for your first colony! Open the map.",
                //     TimeoutOverride = 10f
                // };
                // Notifications.Get().Send(notification);

                yield return new WaitForSeconds(1f);

                DisplayController.Get().ShowMapAndInventory();

                CameraController.Get().OnToggleZoom += (zoomedOut) =>
                {
                    StartingInstructions.Get().Clear();
                    // notification.Accept();

                    Notification notification = new TextNotification();

                    if (mode == StartMode.AcceptedAllGifts && zoomedOut)
                    {
                        DisplayController.Get().SetToStaticMode();

                        notification = new TextNotification
                        {
                            Message =
                                $"There are different types of asteroids. The colors of each tells you what the most common resource is there.",
                        };
                        Notifications.Get().Send(notification);

                        mode = StartMode.ViewedMap;
                    }
                    else if (mode == StartMode.ViewedMap && !zoomedOut)
                    {
                        notification.Accept();
                        mode = StartMode.ControlsInstructions;

                        StartCoroutine(ControlsInstructions());
                    }
                };
            }
        }
    }

    private IEnumerator ControlsInstructions()
    {
        if (mode != StartMode.ControlsInstructions) yield break;

        yield return new WaitForSeconds(1f);
        NotificationSounds.Get().Play(NotificationTypes.Informative);
        StartingInstructions.Get().Print("You can move your ship around a planet using the Right Mouse Button");
        yield return new WaitForSeconds(8f);
        NotificationSounds.Get().Play(NotificationTypes.Informative);
        StartingInstructions.Get().Print("You can construct buildings from the Build Menu");
        yield return new WaitForSeconds(8f);
        NotificationSounds.Get().Play(NotificationTypes.Informative);
        StartingInstructions.Get().Print("You can move goods easily between colonies using your ships Inventory");
        yield return new WaitForSeconds(8f);
        NotificationSounds.Get().Play(NotificationTypes.Informative);
        StartingInstructions.Get().Print("The key to a successful colony is happy colonists. Good luck!");
        yield return new WaitForSeconds(8f);
        StartingInstructions.Get().Clear();

        Finished();
    }

    public void Finished()
    {
        mode = StartMode.Finished;
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
                TimeoutOverride = 10f,
                NotificationType = NotificationTypes.Silent
            });
        }
    }

    public void FinishedOpening()
    {
        if (mode == StartMode.Opening)
        {
            mode = StartMode.FinishedOpening;
            StartingInstructions.Get().Print("This is your ship. Enter it when you're ready.");
        }
    }
}