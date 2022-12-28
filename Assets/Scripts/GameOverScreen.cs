using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void GameOver(int convoyColonistCount, int vacantHousing)
    {
        var cyclesServed = Mathf.RoundToInt(Time.time / (60 * 10));
        _contents.SetActive(true);

        var dead = convoyColonistCount - vacantHousing;

        _contents.GetComponentInChildren<TMP_Text>().text =
            $"The colony ship arrived without somewhere to house them. {dead} {(dead == 1 ? "precious life was" : "lives were")} lost. The Board has decided to terminate your assignment.\n\nYou lead the mission for {cyclesServed} {(cyclesServed == 1 ? "cycle" : "cycles")}.";
    }

    public void Restart()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }
}
