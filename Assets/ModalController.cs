using System;
using TMPro;
using UnityEngine;

public class ModalController : MonoBehaviour
{

    private bool _on;

    public bool On()
    {
        return _on;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _on = true;
        }
    }

    public void ModalUpdate()
    {
        
    }
}