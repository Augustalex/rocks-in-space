using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private GameObject _contents;
    private static GameOverScreen _instance;
    private GlobalResources _globalResources;
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
        if (_globalResources.GetCash() < GraceThreshold)
        {
            NoMoreMoney();
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