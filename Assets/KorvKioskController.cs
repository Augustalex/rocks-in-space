using System.Collections;
using GameNotifications;
using UnityEngine;

public class KorvKioskController : MonoBehaviour
{
    
    void Start()
    {
        var notifications = Notifications.Get();

        StartCoroutine(TellTale());

        IEnumerator TellTale()
        {
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { Message = "Hello there! You sure played this demo a lot." });
            yield return new WaitForSeconds(6);
            notifications.Send(new TextNotification
                { Message = "In fact - you seem to have completed everything there is to do." });
            yield return new WaitForSeconds(4);
            notifications.Send(new TextNotification { Message = "But there is one more thing..." });
            yield return new WaitForSeconds(3);
            notifications.Send(new TextNotification { Message = "...drum roll..." });
            yield return new WaitForSeconds(6);
            notifications.Send(new TextNotification { Message = "...You can play it again!" });
            yield return new WaitForSeconds(3);
            notifications.Send(
                new TextNotification { Message = "Well, you can play it again using these neat tricks:" });
            yield return new WaitForSeconds(3);
            notifications.Send(new TextNotification { Message = "Type TOOMUCHOREGANO to get 1000 ore" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { Message = "Type SPARVAGNAR to get 1000 metals" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { Message = "Type INSPECTORGADGET to get 1000 gadgets" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { Message = "Type SMALLLOAN to get 1000000 credits" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { Message = "Have fun and thanks for playing!" });
        }
    }
}
