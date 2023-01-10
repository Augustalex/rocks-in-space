using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public bool on;
    private DebugHidable[] _hidable;

    void Start()
    {
        if (on)
        {
            _hidable = FindObjectsOfType<DebugHidable>();
        }
    }

    void Update()
    {
        if (!on) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var debugHidable in _hidable)
            {
                debugHidable.gameObject.SetActive(!debugHidable.gameObject.activeSelf);
            }
        }
    }
}