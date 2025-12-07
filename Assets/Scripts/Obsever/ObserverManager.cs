using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPattern.Obsever
{
 public class ObserverManager : MonoBehaviour
    {
        private static ObserverManager _instance;

        public static ObserverManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<ObserverManager>(true);
                    if (!_instance)
                    {
                        GameObject newGameObject = new GameObject("EventDispatcher_" + typeof(GameEvent).Name);
                        _instance = newGameObject.AddComponent<ObserverManager>();
                        Debug.Log("ObserverManager created automatically.");
                    }
                }

                return _instance;
            }
        }

        private Dictionary<GameEvent, Action<object>> _events = new Dictionary<GameEvent, Action<object>>();

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        // Đăng ký lắng nghe sự kiện
        public void RegisterEvent(GameEvent eventID, Action<object> callback)
        {
            if (callback == null)
            {
                Debug.LogWarning($"Callback for event {eventID} is NULL.");
                return;
            }

            if (!_events.TryAdd(eventID, callback))
            {
                _events[eventID] += callback;
            }
        }

        // Hủy đăng ký sự kiện
        public void RemoveEvent(GameEvent eventID, Action<object> callback)
        {
            if (_events.ContainsKey(eventID))
            {
                _events[eventID] -= callback;
                if (_events[eventID] == null)
                {
                    _events.Remove(eventID);
                }
            }
            else
            {
                Debug.LogWarning($"Event '{eventID}' not found in ObserverManager.");
            }
        }

        // Xóa tất cả listener
        public void RemoveAllEvent()
        {
            _events.Clear();
        }

        // Post sự kiện
        public void PostEvent(GameEvent eventID, object param = null)
        {
            if (!_events.ContainsKey(eventID))
            {
                Debug.LogWarning($"Event '{eventID}' has no listener.");
                return;
            }

            _events[eventID]?.Invoke(param);
        }
    }
}
