using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    private GameObject _contents;
    private static GameOverScreen _instance;

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

    public void GameOver(int colonistsLost)
    {
        var cyclesServed = Mathf.RoundToInt(Time.time / (60 * 10));
        _contents.SetActive(true);

        _contents.GetComponentInChildren<TMP_Text>().text =
            $"The colony ship arrived without somewhere to house them. {colonistsLost} lives where lost. The Board has decided to terminate your assignment.\n\nYou lead the mission for {cyclesServed} {(cyclesServed == 1 ? "cycle" : "cycles")}.";
    }
}
