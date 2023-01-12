using System.Linq;
using GameNotifications;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private GameObject _contents;
    private static GameOverScreen _instance;
    private GlobalResources _globalResources;
    private bool _hasNotSentWarning;
    private bool _hasGotWarning;
    private bool _hasGotLoan;
    private bool _hasRepaidLoan;
    private bool _hasEndedGame;
    private const float GraceThreshold = -500f;

    public static GameOverScreen Get()
    {
        return _instance;
    }

    void Awake()
    {
        _contents = GetComponentInChildren<GameOverScreenContents>().gameObject;
        _contents.SetActive(false);

        _instance = this;
    }

    private void Start()
    {
        _globalResources = GlobalResources.Get();
    }

    private void Update()
    {
        if (!_hasGotLoan && _globalResources.GetCash() < 0)
        {
            Notifications.Get().Send(new TextNotification
            {
                message =
                    "The company has been made aware of your struggle to turn a profit. You have been giving a warning and a small sum to turn this situation around."
            });
            _globalResources.AddCash(500);
            _hasGotLoan = true;
        }
        else if (_hasGotLoan && !_hasRepaidLoan && _globalResources.GetCash() > 1000)
        {
            Notifications.Get().Send(new TextNotification
                { message = "The company has taken an extra piece of your profits as recourse for their earlier aid." });
            _globalResources.UseCash(500);
            _hasRepaidLoan = true;
        }
        else if (!_hasGotWarning && _globalResources.GetCash() < 0)
        {
            Notifications.Get().Send(new TextNotification
            {
                message =
                    "The company has issued you a final warning. If you don't turn a profit soon, you will be fired."
            });
            _hasGotWarning = true;
        }
        else if (_hasGotWarning && !_hasEndedGame && _globalResources.GetCash() < GraceThreshold)
        {
            NoMoreMoney();
            _hasEndedGame = true;
        }
    }

    public void ColonistsDead(int convoyColonistCount, int vacantHousing)
    {
        var cyclesServed = Mathf.RoundToInt(Time.time / (60 * 10));
        _contents.SetActive(true);

        var dead = convoyColonistCount - vacantHousing;

        _contents.GetComponentInChildren<TMP_Text>().text =
            $"The colony ship arrived without somewhere to house them. {dead} {(dead == 1 ? "precious life was" : "lives were")} lost. The Board has decided to terminate your assignment.\n\nYou lead the mission for {cyclesServed} {(cyclesServed == 1 ? "cycle" : "cycles")}.";
    }

    private void NoMoreMoney()
    {
        var cyclesServed = Mathf.RoundToInt(Time.time / (60 * 10));
        _contents.SetActive(true);

        var colonists = PlanetsRegistry.Get().All().Sum(p => p.GetResources().GetInhabitants());

        var text =
            $"You are out of credits, The Board has decided to terminate your employment.\n\nYou lead the mission for {cyclesServed} {(cyclesServed == 1 ? "cycle" : "cycles")}.";
        if (colonists > 0)
        {
            text +=
                $"\n\nYou gave {colonists} {(colonists == 1 ? "person" : "people")} a second chance in your colony.";
        }

        _contents.GetComponentInChildren<TMP_Text>().text = text;
    }

    public void Restart()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }
}