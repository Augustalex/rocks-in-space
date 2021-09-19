using System.Linq;
using TMPro;
using UnityEngine;

namespace Buildings.Port.Menu
{
    public class ExistingTradePrograms : MonoBehaviour
    {
        public GameObject buttonTemplate;

        public void OnShow(TinyPlanet planet)
        {
            //TODO Add trade to planet

            var trade = planet.GetComponent<TinyPlanetTrade>();
            
            var buttonContainer = GetComponentInChildren<ButtonMenuContainer>();

            var buttons = trade.tradePrograms.Select(program =>
            {
                var button = Instantiate(buttonTemplate);
                var resourceName = TinyPlanetResources.ResourceName(program.resource);
                button.GetComponent<TMP_Text>().text =
                    $"{planet.planetName} {resourceName} -> {program.target.planetName}";

                return button;
            });
            buttonContainer.SetChildren(buttons.ToArray());
        }
    }
}