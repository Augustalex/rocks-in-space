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
        StartingSequenceCameraController.Get().FinishedOpening += FinishedOpening;
        StartingSequenceCameraController.Get().FinishedEnteringShip += FinishedEnteringShip;

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
            StartingSequenceCameraController.Get().FocusOnStartingShip();
            mode = StartMode.Opening;
        }
    }

    public void FinishedOpening()
    {
        if (mode == StartMode.Opening)
        {
            mode = StartMode.FinishedOpening;
            StartingInstructions.Get().Print("This is your ship.");

            StartCoroutine(ShowSoon());

            IEnumerator ShowSoon()
            {
                yield return new WaitForSeconds(3f);
                EnteringShip();
            }
        }
    }

    public void EnteringShip()
    {
        if (mode == StartMode.FinishedOpening)
        {
            StartingSequenceCameraController.Get().EnterShip();

            StartingInstructions.Get().Clear();

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

                // Player could have already accepted all gifts at this point
                if (mode == StartMode.AcceptingGifts)
                {
                    NotificationSounds.Get().Play(NotificationTypes.Informative);
                    StartingInstructions.Get()
                        .Print("You've been issued some supplies to help you get started.");
                    yield return new WaitForSeconds(8f);
                }

                if (mode == StartMode.AcceptingGifts)
                {
                    StartingInstructions.Get()
                        .Print("Pick them up.");
                }
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
                yield return new WaitForSeconds(.5f);
                DisplayController.Get().ShowMapAndInventory();

                CameraController.Get().OnToggleZoom += (zoomedOut) =>
                {
                    StartingInstructions.Get().Clear();

                    if (mode == StartMode.AcceptedAllGifts && zoomedOut)
                    {
                        DisplayController.Get().SetToStaticMode();

                        StartCoroutine(ShowNavigationHint());

                        IEnumerator ShowNavigationHint()
                        {
                            yield return new WaitForSeconds(3f);
                            StartingInstructions.Get().Print("Right Click on an asteroid to move your ship there.");
                            yield return new WaitForSeconds(8f);
                            StartingInstructions.Get()
                                .Print(
                                    "There are different types of asteroids.\nExplore to find a suitable place to start.");
                            yield return new WaitForSeconds(10f);
                            StartingInstructions.Get().Clear();
                        }

                        mode = StartMode.ViewedMap;
                    }
                    else if (mode == StartMode.ViewedMap && !zoomedOut)
                    {
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

    public void Finished()
    {
        mode = StartMode.Finished;
    }

    public void SkipAll()
    {
        Finished();
        StartingSequenceCameraController.Get().ForceFocusOnStartingShip();
        DisplayController.Get().ExitCinematicMode();
    }
}