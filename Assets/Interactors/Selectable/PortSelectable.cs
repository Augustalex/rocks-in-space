using UnityEngine;

namespace Interactors.Selectable
{
    public class PortSelectable : Selectable
    {
        public GameObject menuTemplate;
        
        public override void Select()
        {
            // var canvas = FindObjectOfType<Canvas>();
            // var block = transform.parent.GetComponentInChildren<Block>();
            //
            // var menu = Instantiate(menuTemplate, canvas.transform, false);
            // menu.GetComponent<ModalController>().Show(block);
        }
    }
}