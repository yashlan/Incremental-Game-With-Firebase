using UnityEngine;
using UnityEngine.EventSystems;
using Yashlan.manage;

namespace Yashlan.ui
{
    public class TapArea : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            GameManager.Instance.CollectByTap(eventData.position, transform);
        }
    }
}
