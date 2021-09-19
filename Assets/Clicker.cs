using Interactors;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private Vector3 _point;
    private InteractorController _interactorController;
    private static Clicker _instance;
    private bool _enabled = true;

    private void Awake()
    {
        _instance = this;
    }

    public static Clicker Get()
    {
        return _instance;
    }

    void Start()
    {
        _interactorController = GetComponent<InteractorController>();
    }

    void Update()
    {
        if (_enabled)
        {
            _interactorController.Interact();
        }
    }

    public void Disable()
    {
        _enabled = false;
    }

    public void Enable()
    {
        _enabled = true;
    }
}