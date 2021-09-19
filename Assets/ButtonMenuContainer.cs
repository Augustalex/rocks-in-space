using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMenuContainer : MonoBehaviour
{
    public void SetChildren(GameObject[] children)
    {
        foreach (var child in children)
        {
            child.transform.SetParent(transform);
        }
    }
}
