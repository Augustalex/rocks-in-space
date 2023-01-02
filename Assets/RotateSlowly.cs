using UnityEngine;

public class RotateSlowly : MonoBehaviour
{
    public float x;

    void Update()
    {
        var objectTransform = transform;
        transform.RotateAround(objectTransform.position, objectTransform.right, x * Time.deltaTime);
    }
}