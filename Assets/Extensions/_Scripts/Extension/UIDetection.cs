using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Extension
{
    public class UIDetection : MonoBehaviour
    {
        public static bool IsPointerOverUIObject()
        {
            var results = new List<RaycastResult>();
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}