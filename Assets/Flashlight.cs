using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light _light;

    void Start()
    {
        _light = GetComponent<Light>();
        TurnOff();
    }

    private void TurnOff()
    {
        _light.enabled = false;
    }

    private void TurnOn()
    {
        _light.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_light.enabled)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }
    }
}
