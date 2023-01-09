using Interactors;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clicker : MonoBehaviour
{
    private Vector3 _point;
    private InteractorController _interactorController;
    private static Clicker _instance;

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
        if (WorldInteractionLock.Get().CanInteract()) _interactorController.Interact();
    }
}