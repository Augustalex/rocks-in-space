using TMPro;
using UnityEngine;

public class PlanetPopup : MonoBehaviour
{
    public TMP_Text header;
    public TMP_Text energy;
    public TMP_Text food;
    public TMP_Text housing;
    public TMP_Text inhabitants;

    private static PlanetPopup _instance;
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");

    public static PlanetPopup Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
        
        Hide();
    }

    public void Show(Vector2 position, TinyPlanet planet)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            _animator.SetBool(Visible, true);
        }
        
        UpdatePosition(position);

        var resources = planet.GetResources();
        header.text = planet.planetName;
        energy.text = "Energy: " + resources.GetEnergy();
        food.text = "Food: " + resources.GetFood();
        housing.text = "Housing: " + resources.GetVacantHousing();
        inhabitants.text = "Colonists: " + resources.GetInhabitants();
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void StartHide()
    {
        _animator.SetBool(Visible, false);
    }

    public bool HiddenAlready()
    {
        return !gameObject.activeSelf;
    }

    public bool StartedHiding()
    {
        return !_animator.GetBool(Visible) && !HiddenAlready();
    }
}