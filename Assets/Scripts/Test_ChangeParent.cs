using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ChangeParent : MonoBehaviour
{
    private Transform _firstParent;
    public Transform secondParent;
    private int _step = 0;

    void Start()
    {
        _firstParent = transform.parent;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("CURRENT STEP: " + _step);
            if (_step == 0)
            {
                Debug.Log("STEP 0");
                transform.SetParent(secondParent);
                _step += 1;
            }
            else if (_step == 1)
            {
                Debug.Log("STEP 1");
                transform.SetParent(_firstParent);
                _step = 0;
            }
        }
    }
}