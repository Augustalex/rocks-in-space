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
            notifications.Send(new TextNotification { message = "Hello there! You sure played this demo a lot." });
            yield return new WaitForSeconds(6);
            notifications.Send(new TextNotification
                { message = "In fact - you seem to have completed everything there is to do." });
            yield return new WaitForSeconds(4);
            notifications.Send(new TextNotification { message = "But there is one more thing..." });
            yield return new WaitForSeconds(3);
            notifications.Send(new TextNotification { message = "...drum roll..." });
            yield return new WaitForSeconds(6);
            notifications.Send(new TextNotification { message = "...You can play it again!" });
            yield return new WaitForSeconds(3);
            notifications.Send(
                new TextNotification { message = "Well, you can play it again using these neat tricks:" });
            yield return new WaitForSeconds(3);
            notifications.Send(new TextNotification { message = "Type TOOMUCHOREGANO to get 1000 ore" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { message = "Type SPARVAGNAR to get 1000 metals" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { message = "Type INSPECTORGADGET to get 1000 gadgets" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { message = "Type SMALLLOAN to get 1000000 credits" });
            yield return new WaitForSeconds(2);
            notifications.Send(new TextNotification { message = "Have fun and thanks for playing!" });
        }
    }
}
