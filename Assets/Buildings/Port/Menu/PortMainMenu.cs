using Menus;
using TMPro;
using UnityEngine;

namespace Buildings.Port.Menu
{
    public class PortMainMenu : MenuScene
    {
        public TMP_Text header;
        public GameObject tradeList;

        public override void OnShow(Block blockWithPort)
        {
            var planet = blockWithPort.GetConnectedPlanet();

            var buttonContainer = GetComponentInChildren<ButtonMenuContainer>();
            
            buttonContainer.SetChildren();

            tradeList.GetComponent<MenuScene>()
            
            header.text = "Port of " + planet.planetName;
        }
    }
}