using UnityEngine;
using Random = UnityEngine.Random;

public class Spacer : MonoBehaviour
{
    private Camera _camera;
    private Animator _animator;
    private bool _spinning;

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

        if (_spinning)
        {
            transform.RotateAround(transform.position, Vector3.forward, 180f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _spinning = true;
        GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * .9f, ForceMode.Impulse);
    }
}