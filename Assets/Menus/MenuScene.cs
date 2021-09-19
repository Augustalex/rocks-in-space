using UnityEngine;

namespace Menus
{
    public abstract class MenuScene : MonoBehaviour
    {
        public abstract void OnShow(Block blockWithPort);
    }
}