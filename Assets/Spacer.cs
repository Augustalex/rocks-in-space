using Interactors.Digging;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spacer : MonoBehaviour, ILaserInteractable
{
    private Camera _camera;
    private Animator _animator;

    void Start()
    {
        _camera = Camera.main;
        _animator = GetComponent<Animator>();

        _animator.Play(0, -1, Random.value);
    }

    void Update()
    {
        var zRotation = transform.rotation.eulerAngles.z;
        transform.LookAt(_camera.transform);
        var newRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(
            newRotation.x,
            newRotation.y,
            zRotation);
    }
    
    public void LaserInteract()
    {
        Destroy(gameObject);
    }

    public bool CanInteract()
    {
        return gameObject != null;
    }

    public float DisintegrationTime()
    {
        return .5f;
    }

    public EntityOven GetOven()
    {
        return GetComponentInChildren<EntityOven>();
    }
}