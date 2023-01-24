using UI.Tooltip;
using UnityEngine;

[RequireComponent(typeof(TooltipTriggerEvent))]
public class TooltipTrigger : MonoBehaviour
{
    private string _message;
    public string message;

    void Awake()
    {
        var trigger = GetComponent<TooltipTriggerEvent>();
        trigger.Triggered += TriggerTooltip;

        SetMessage(message);
    }

    private void TriggerTooltip()
    {
        GlobalTooltip.Get().Show(_message, Input.mousePosition);
    }

    public void SetMessage(string newMessage)
    {
        Debug.Log("SET MESSAGE: " + newMessage);
        _message = newMessage;
    }
}