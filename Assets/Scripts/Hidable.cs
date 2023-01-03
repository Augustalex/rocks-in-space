using UnityEngine;

public class Hidable : MonoBehaviour
{
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
}