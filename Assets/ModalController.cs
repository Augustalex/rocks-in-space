using System;
using Interactors.Selectable;
using Menus;
using TMPro;
using UnityEngine;

public class ModalController : MonoBehaviour
{
    public TMP_Text header;

    public MenuScene startingMenuScene;

    public void Close()
    {
        DisplayController.Get().ExistModalMode();
        Clicker.Get().Enable();
     
        Destroy(gameObject);
    }

    public void Show(Block blockWithPort)
    {
        DisplayController.Get().SetToModalMode();
        Clicker.Get().Disable();
        
        startingMenuScene.OnShow(blockWithPort);
    }
}