using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActionTarget))]
public class PortConvoyManager : MonoBehaviour
{
    private bool _message;

    public Animator portGlobeAnimator;
    public PortGlobeAnimationEventHandler portGlobeAnimationEventHandler;
    
    private static readonly int GotMessage = Animator.StringToHash("GotMessage");
    private ColonistManager _colonistManager;
    private bool _hasSentFirstMessage;
    private bool _waitingForMessage;
    private float _waitUntil;
    private ConvoyChoiceScreen _convoyChoiceScreen;
    private DisplayController _displayController;
    private bool _hasSettled;

    private void Awake()
    {
        GetComponent<ActionTarget>().OnClicked += Clicked;
        portGlobeAnimationEventHandler.OnSettled += Settled;
    }

    private void Start()
    {
        _colonistManager = ColonistManager.Get();
        _convoyChoiceScreen = ConvoyChoiceScreen.Get();
        _displayController = DisplayController.Get();
    }

    private void Update()
    {
        if (_hasSettled)
        {
            if (_displayController.inputMode == DisplayController.InputMode.Static)
            {
                TriggerMessageSoon();
            }

            return;
        }
        
        if (!_hasSentFirstMessage) return;
        if (_message) return;
        if (_convoyChoiceScreen.Visible()) return;

        if (_waitingForMessage)
        {
            if (Time.time > _waitUntil)
            {
                TriggerMessageSoon();
                _waitingForMessage = false;
            }
        }
        else
        {
            var planet = GetComponentInParent<TinyPlanet>();
            if (_colonistManager.ConvoyCount(planet) == 0)
            {
                _waitingForMessage = true;
                _waitUntil = Time.time + 20;
            }
        }
            
    }

    private void Clicked()
    {
        if (!_message) return;
        _message = false;
        portGlobeAnimator.SetBool(GotMessage, false);
        
        var planet = GetComponentInParent<TinyPlanet>();
        var convoys = new[]
        {
            new Convoy { PlanetId = planet.planetId, Colonists = 1000, CashReward = 1000 },
            new Convoy { PlanetId = planet.planetId, Colonists = 2000, CashReward = 5000 },
            new Convoy { PlanetId = planet.planetId, Colonists = 5000, CashReward = 25000 }
        };
        _convoyChoiceScreen.Set(convoys);
    }

    public void Settled()
    {
        if (_hasSentFirstMessage) return;
        _hasSettled = true;
    }

    private void TriggerMessageSoon()
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(4);
            TriggerMessage();
        }
    }

    private void TriggerMessage()
    {
        _message = true;
        portGlobeAnimator.SetBool(GotMessage, true);
        _hasSentFirstMessage = true;
    }
}