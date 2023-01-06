using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColonyShipUI : MonoBehaviour
{
    public Button acceptButton;
    public TMP_Text acceptErrorText;
    public Button rejectButton;

    public TMP_Text timerText;
    public TMP_Text colonyCountText;
    public TMP_Text rewardText;
    public TMP_Text requirementText;

    void Start()
    {
        CurrentPlanetController.Get().ShipSelected += OnShipSelected;
        CurrentPlanetController.Get().CurrentPlanetChanged += OnPlanetChanged;
        CameraController.Get().OnToggleZoom += OnToggleZoom;

        acceptButton.onClick.AddListener(Accept);
        rejectButton.onClick.AddListener(Reject);

        Hide();
    }

    private void OnToggleZoom(bool zoomedOut)
    {
        if (zoomedOut)
        {
            Hide();
        }
        else
        {
            var ship = CurrentPlanetController.Get().CurrentShip();
            if (ship != null)
            {
                Show(ship);
            }
        }
    }

    private void OnPlanetChanged(PlanetChangedInfo info)
    {
        Hide();
    }

    private void Update()
    {
        var currentShip = CurrentPlanetController.Get().CurrentShip();
        if (!currentShip) return;

        UpdateTimerText(currentShip);

        if (currentShip.ShipGone()) Hide();
    }

    private void Reject()
    {
        var currentShip = CurrentPlanetController.Get().CurrentShip();
        if (!currentShip) return;

        currentShip.MoveAway();
    }

    private void Accept()
    {
        var currentShip = CurrentPlanetController.Get().CurrentShip();
        if (!currentShip) return;

        var suitablePlanets = GetSuitablePlanets(currentShip);
        if (suitablePlanets.Length == 0)
        {
            acceptErrorText.text = "No planet meets the requirements";
        }
        else
        {
            currentShip.MoveInTo(suitablePlanets[0]);
        }
    }

    private void OnShipSelected(ColonyShip ship)
    {
        if (!ship.ShipGone() && !CameraController.Get().IsZoomedOut()) Show(ship);
    }

    private TinyPlanet[] GetSuitablePlanets(ColonyShip ship)
    {
        return PlanetsRegistry.Get().All().Where(ship.PlanetMeetRequirements).ToArray();
    }

    private void UpdateTimerText(ColonyShip ship)
    {
        var timeLeft = ship.TimeLeft();
        var ts = TimeSpan.FromSeconds(timeLeft);
        var timerPart = ts.ToString(@"mm\:ss");
        timerText.text = $"Leaving in {timerPart}";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show(ColonyShip ship)
    {
        UpdateTimerText(ship);

        colonyCountText.text = $"{ship.colonists} colonists are looking for a new home";

        if (ship.cashReward > 0)
        {
            rewardText.gameObject.SetActive(true);
            rewardText.text = $"Reward:\n{ship.cashReward}c";
        }
        else
        {
            rewardText.gameObject.SetActive(false);
        }

        requirementText.text = $"Requirements:\n{ship.colonists} housing";


        var suitablePlanets = GetSuitablePlanets(ship);

        var hasASuitablePlanet = suitablePlanets.Length > 0;
        acceptButton.GetComponentInChildren<TMP_Text>().text =
            hasASuitablePlanet ? "Welcome them" : "Welcome them";
        // acceptButton.enabled = hasASuitablePlanet;
        acceptErrorText.text = "";

        gameObject.SetActive(true);
    }
}