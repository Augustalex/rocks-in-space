using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltip
{
    public class TooltipTriggerEvent : EventTrigger
    {
        private bool _entered;
        public event Action Triggered;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            _entered = true;
            StartCoroutine(ShowTooltipSoon());
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            _entered = false;
            base.OnPointerExit(eventData);
        }

        private IEnumerator ShowTooltipSoon()
        {
            yield return new WaitForSeconds(.4f);
            if (_entered)
            {
                Triggered?.Invoke();
            }
        }
    }
}