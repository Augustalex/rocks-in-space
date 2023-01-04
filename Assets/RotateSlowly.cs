using UnityEngine;

public class RotateSlowly : MonoBehaviour
{
    public float x;
    public float y;

    void Update()
    {
        var objectTransform = transform;
        if (x != 0f) transform.RotateAround(objectTransform.position, objectTransform.right, x * Time.deltaTime);
        if (y != 0f) transform.RotateAround(objectTransform.position, objectTransform.up, y * Time.deltaTime);
    }
}